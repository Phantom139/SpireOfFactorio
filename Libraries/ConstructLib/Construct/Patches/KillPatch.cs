using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;

namespace SpireOfFactorio.Libraries.ConstructLib.Construct.Patches;

[HarmonyPatch(typeof(CreatureCmd), "KillWithoutCheckingWinCondition")]
public static class KillPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Task __result, Creature creature)
    {
        __result = AwaitAndCleanup(__result, creature);
    }

    private static async Task AwaitAndCleanup(Task originalTask, Creature creature)
    {
        await originalTask;

        if (creature != null && creature.Side == CombatSide.Player && creature.IsPet && creature.Monster is ConstructModel && creature.CombatState != null)
        {
            ICombatState combatState = creature.CombatState;
            bool shouldRemove = combatState != null && Hook.ShouldCreatureBeRemovedFromCombatAfterDeath(combatState, creature);
            if (shouldRemove)
            {
                CombatManager.Instance?.RemoveCreature(creature);
                combatState?.RemoveCreature(creature);
            }
        }
    }
}