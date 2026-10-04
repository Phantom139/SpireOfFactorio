using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.Libraries.ConstructLib.Construct;


namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Patches;

[HarmonyPatch]
public static class AttackCommandPatches
{
    [HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.FromMonster))]
    [HarmonyPostfix]
    private static void FromMonsterPostfix(AttackCommand __instance, MonsterModel monster)
    {
        if (monster is not ConstructModel)
        {
            return;
        }
        else
        {
            // FromMonster() automatically establishes TargetingAllOpponents().
            // Constructs need to choose their own targeting method instead.
            AccessTools
                .Field(typeof(AttackCommand), "_combatState")
                ?.SetValue(__instance, null);
        }
    }

    [HarmonyPatch(typeof(AttackCommand), "GetPossibleTargets")]
    [HarmonyPrefix]
    private static bool GetPossibleTargetsPrefix(
        AttackCommand __instance,
        ref IReadOnlyList<Creature> __result)
    {
        if (__instance.Attacker?.Monster is not ConstructModel)
        {
            return true;
        }

        // Only change the behavior for multi-targeted constructs.
        if (!__instance.IsMultiTargeted)
        {
            return true;
        }

        // Constructs are player-side creatures, so their opponents
        // are the enemy creatures.
        __result = __instance.Attacker.CombatState
            .GetOpponentsOf(__instance.Attacker);

        return false;
    }

    /**
    [HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.TargetingAllOpponents))]
    [HarmonyPostfix]
    private static void TargetingAllOpponentsPostfix(AttackCommand __instance, ICombatState combatState)
    {
        foreach (Creature candidate in combatState.PlayerCreatures)
        {
            MainFile.Logger.Info(
                $"[ConstructTAODebug:PlayerCreatures] " +
                $"Attacker={__instance.Attacker.Name}, " +
                $"Candidate={candidate.Name}, " +
                $"IsPet={candidate.IsPet}, " +
                $"IsAlive={candidate.IsAlive}, " +
                $"IsHittable={candidate.IsHittable}"
            );
        }
        foreach (Creature candidate in combatState.Allies)
        {
            MainFile.Logger.Info(
                $"[ConstructTAODebug:AllAllies] " +
                $"Attacker={__instance.Attacker.Name}, " +
                $"Candidate={candidate.Name}, " +
                $"IsPet={candidate.IsPet}, " +
                $"IsAlive={candidate.IsAlive}, " +
                $"IsHittable={candidate.IsHittable}"
            );
        }
        foreach (Creature candidate in combatState.Enemies)
        {
            MainFile.Logger.Info(
                $"[ConstructTAODebug:AllEnemies] " +
                $"Attacker={__instance.Attacker.Name}, " +
                $"Candidate={candidate.Name}, " +
                $"IsPet={candidate.IsPet}, " +
                $"IsAlive={candidate.IsAlive}, " +
                $"IsHittable={candidate.IsHittable}"
            );
        }
    }
    **/

    [HarmonyPatch(typeof(AttackCommand), "GetPossibleTargets")]
    [HarmonyPostfix]
    private static void GetPossibleTargetsPostfix(
        AttackCommand __instance,
        ref IReadOnlyList<Creature> __result)
    {
        // We only want to modify multi-targeted enemy attacks.
        if (!__instance.IsMultiTargeted)
            return;

        // No attacker = nothing useful for us to inspect.
        if (__instance.Attacker == null)
            return;

        // We only want normal enemies attacking the player's side.
        if (__instance.Attacker.Side != CombatSide.Enemy)
            return;

        // Constructs should not use this modification when they are the attacker.
        if (__instance.Attacker.Monster is ConstructModel)
            return;

        // Start with exactly what the vanilla game returned.
        List<Creature> targets = new List<Creature>(__result);

        // Get everything this enemy considers to be an opponent.
        IReadOnlyList<Creature> opponents =
            __instance.Attacker.CombatState.GetOpponentsOf(__instance.Attacker);

        foreach (Creature candidate in opponents)
        {
            if (candidate.Monster is not ConstructModel)
                continue;

            if (targets.Contains(candidate))
                continue;

            targets.Add(candidate);
        }

        // Replace the method's return value.
        __result = targets.AsReadOnly();
    }

}