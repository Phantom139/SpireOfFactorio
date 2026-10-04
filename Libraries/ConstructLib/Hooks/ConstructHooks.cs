using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using SpireOfFactorio.Libraries.ConstructLib.Commands;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.Libraries.ConstructLib.Hooks;

public static class ConstructHooks
{
    public static void Initialize()
    {
        CombatManager.Instance.TurnStarted += OnTurnStarted;
        CombatManager.Instance.TurnEnded += OnTurnEnded;
        CombatManager.Instance.CombatSetUp += OnCombatSetUp;
        CombatManager.Instance.CombatEnded += OnCombatEnded;
    }

    public static void Deinitialize()
    {
        CombatManager.Instance.TurnStarted -= OnTurnStarted;
        CombatManager.Instance.TurnEnded -= OnTurnEnded;
        CombatManager.Instance.CombatSetUp -= OnCombatSetUp;
        CombatManager.Instance.CombatEnded -= OnCombatEnded;
    }

    private static void OnTurnStarted(CombatState combatState)
    {
        if (combatState.CurrentSide == CombatSide.Player)
        {
            _ = ConstructAnimCmd.Rearrange();
        }
    }

    private static void OnTurnEnded(CombatState combatState)
    {
        if (combatState.CurrentSide == CombatSide.Player)
        {
            _ = ConstructAnimCmd.Rearrange();
        }
    }

    private static void OnCombatSetUp(CombatState combatState)
    {
        PetOrderManager.clear();
    }

    private static void OnCombatEnded(CombatRoom combatRoom)
    {
        PetOrderManager.clear();
    }

    public static async Task OnConstructed(
        PlayerChoiceContext choiceContext,
        Creature construct,
        Player owner,
        ConstructCreationArgs args)
    {
        foreach (var power in owner.Creature.Powers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.OnConstructCreated(
                    choiceContext,
                    construct,
                    owner,
                    args);
            }
        }
    }

    public static async Task OnConstructPerformedTask(
        PlayerChoiceContext choiceContext,
        Creature construct,
        Player owner)
    {
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.OnConstructPerformedTask(choiceContext, construct, owner);
                if (construct.IsDead)
                {
                    return;
                }
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.OnConstructPerformedTask(choiceContext, construct, owner);
                if (construct.IsDead)
                {
                    return;
                }
            }
        }
    }

}