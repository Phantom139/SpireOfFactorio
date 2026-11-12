using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

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
        MethodInfo damageMethod = AccessTools.Method(
            typeof(CreatureCmd),
            nameof(CreatureCmd.Damage),
            new[]
            {
            typeof(PlayerChoiceContext),
            typeof(IEnumerable<DamageTargetData>),
            typeof(decimal),
            typeof(ValueProp),
            typeof(Creature),
            typeof(CardModel),
            typeof(CardPlay),
            typeof(ModifyDamageHookType)
            }
        ) ?? throw new Exception(
            "[CreatureCmdDamagePatch] Could not find Damage method."
        );

        AsyncStateMachineAttribute? asyncAttribute =
            damageMethod.GetCustomAttribute<AsyncStateMachineAttribute>();

        if (asyncAttribute == null)
        {
            throw new Exception(
                "[CreatureCmdDamagePatch] Damage method has no async state machine."
            );
        }

        return AccessTools.Method(
            asyncAttribute.StateMachineType,
            "MoveNext"
        ) ?? throw new Exception(
            "[CreatureCmdDamagePatch] Could not find Damage MoveNext."
        );
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
       IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        MethodInfo getBlockTarget = AccessTools.Method(
            typeof(CreatureCmdDamagePatch),
            nameof(GetBlockTarget)
        );

        MethodInfo petOwnerGetter = AccessTools.PropertyGetter(
            typeof(Creature),
            nameof(Creature.PetOwner)
        );

        MethodInfo petOwnerCreatureGetter = AccessTools.PropertyGetter(
            petOwnerGetter.ReturnType,
            "Creature"
        );

        if (getBlockTarget == null ||
            petOwnerGetter == null ||
            petOwnerCreatureGetter == null)
        {
            throw new Exception(
                "[CreatureCmdDamagePatch] Could not find required property/method getters."
            );
        }

        int matches = 0;

        for (int i = 0; i < codes.Count; i++)
        {
            if (!codes[i].Calls(petOwnerGetter))
                continue;

            // Find the Creature getter belonging to PetOwner.
            int creatureGetterIndex = -1;

            for (int j = i + 1; j < Math.Min(i + 20, codes.Count); j++)
            {
                if (codes[j].Calls(petOwnerCreatureGetter))
                {
                    creatureGetterIndex = j;
                    break;
                }
            }

            if (creatureGetterIndex == -1)
                continue;

            // Find the local store terminating the expression.
            int storeIndex = -1;

            for (int j = creatureGetterIndex + 1;
                 j < Math.Min(creatureGetterIndex + 10, codes.Count);
                 j++)
            {
                if (codes[j].IsStloc())
                {
                    storeIndex = j;
                    break;
                }
            }

            if (storeIndex == -1)
                continue;

            MainFile.Logger.Info(
                $"[CreatureCmdDamagePatch] Found PetOwner target expression at IL[{i}]."
            );

            /*
             * The original target is already on the evaluation stack
             * immediately before the PetOwner getter.
             *
             * Replace:
             *
             *   originalTarget.PetOwner?.Creature ?? originalTarget
             *
             * With:
             *
             *   GetBlockTarget(originalTarget)
             *
             * Keep the original stloc instruction.
             */

            var replacement = new CodeInstruction(
                OpCodes.Call,
                getBlockTarget
            );

            // Preserve any incoming labels and exception markers.
            replacement.labels.AddRange(codes[i].labels);
            replacement.blocks.AddRange(codes[i].blocks);

            codes.RemoveRange(i, storeIndex - i);

            codes.Insert(i, replacement);

            matches++;
            break;
        }

        if (matches != 1)
        {
            throw new Exception(
                $"[CreatureCmdDamagePatch] Expected exactly one target expression, found {matches}."
            );
        }

        MainFile.Logger.Info(
            "[CreatureCmdDamagePatch] Successfully replaced Block target expression."
        );

        return codes;
    }
}