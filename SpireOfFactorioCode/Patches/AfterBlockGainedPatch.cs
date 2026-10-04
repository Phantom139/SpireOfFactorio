using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Characters;

namespace SpireOfFactorio.SpireOfFactorioCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterBlockGained))]
public static class AfterBlockGainedPatch
{
    public static void Postfix(ref Task __result, ICombatState combatState, Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature != null)
        {
            if (creature.IsPlayer && !creature.IsDead)
            {
                if (creature.Player.Character is Engineer e)
                {
                    __result = Engineer.AfterEngineerBlockGained(__result, combatState, creature, amount, props, cardSource);
                }
            }
        }
    }
}