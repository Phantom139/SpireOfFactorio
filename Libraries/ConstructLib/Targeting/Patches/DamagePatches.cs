using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode;
using System.Reflection;
using System.Reflection.Emit;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Patches;

[HarmonyPatch]
public static class CreatureCmdDamagePatch
{
    private static Creature GetBlockTarget(Creature originalTarget)
    {
        if (originalTarget.Monster is ConstructModel)
        {
            return originalTarget;
        }

        return originalTarget.PetOwner?.Creature ?? originalTarget;
    }

    private static MethodBase TargetMethod()
    {
        Type? stateMachine = AccessTools.Inner(
            typeof(CreatureCmd),
            "<Damage>d__12"
        );

        if (stateMachine == null)
        {
            throw new Exception(
                "[CreatureCmdDamagePatch] " +
                "Could not find Damage state machine."
            );
        }

        return AccessTools.Method(stateMachine, "MoveNext") ?? throw new Exception("[CreatureCmdDamagePatch] Could not find MoveNext.");
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        MethodInfo getBlockTarget =
            AccessTools.Method(
                typeof(CreatureCmdDamagePatch),
                nameof(GetBlockTarget)
            );

        MethodInfo petOwnerGetter =
            AccessTools.PropertyGetter(
                typeof(Creature),
                nameof(Creature.PetOwner)
            );

        MethodInfo damageBlockInternal =
            AccessTools.Method(
                typeof(Creature),
                nameof(Creature.DamageBlockInternal)
            );

        bool patched = false;

        for (int i = 0; i < codes.Count; i++)
        {
            if (!codes[i].Calls(petOwnerGetter))
            {
                continue;
            }

            /*
             * This should be the PetOwner access used by:
             *
             * originalTarget2.PetOwner?.Creature ?? originalTarget2
             *
             * We expect:
             *
             *   ldarg.0
             *   ldfld originalTarget
             *   call get_PetOwner
             */

            if (i < 1)
            {
                continue;
            }

            MainFile.Logger.Info(
                $"[CreatureCmdDamagePatch] " +
                $"Found PetOwner getter at IL[{i}]."
            );

            /*
             * Find the stloc.3 which terminates the
             * PetOwner?.Creature ?? originalTarget expression.
             */
            int storeIndex = -1;

            for (int j = i; j < Math.Min(i + 20, codes.Count); j++)
            {
                if (codes[j].opcode == OpCodes.Stloc_3)
                {
                    storeIndex = j;
                    break;
                }
            }

            if (storeIndex == -1)
            {
                MainFile.Logger.Error(
                    "[CreatureCmdDamagePatch] " +
                    "Could not find stloc.3 after PetOwner getter."
                );

                continue;
            }

            MainFile.Logger.Info(
                $"[CreatureCmdDamagePatch] " +
                $"Replacing IL[{i - 1}] through IL[{storeIndex}] " +
                $"with GetBlockTarget."
            );

            /*
             * We want to preserve the originalTarget load:
             *
             * ldarg.0
             * ldfld originalTarget
             *
             * So remove everything starting with get_PetOwner
             * through stloc.3.
             */

            int removeStart = i;
            int removeCount = storeIndex - removeStart + 1;

            codes.RemoveRange(removeStart, removeCount);

            /*
             * Stack currently contains:
             *
             * originalTarget
             *
             * Replace the entire PetOwner resolution with:
             *
             * GetBlockTarget(originalTarget)
             * stloc.3
             */

            codes.Insert(
                removeStart,
                new CodeInstruction(
                    OpCodes.Call,
                    getBlockTarget
                )
            );

            codes.Insert(
                removeStart + 1,
                new CodeInstruction(
                    OpCodes.Stloc_3
                )
            );

            patched = true;
            break;
        }

        MainFile.Logger.Info(
            "[CreatureCmdDamagePatch] " +
            $"Patch applied = {patched}"
        );

        return codes;
    }
}