using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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

    public static async Task ModifyConstructTargetingList(
        Creature construct,
        ConstructTargetSelectionType targetingSelectionType,
        List<Creature> targetList
        )
    {
        if (construct.IsDead)
        {
            return;
        }
        if (targetList == null)
        {
            return;
        }
        Player owner = construct.PetOwner;
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructTargetingList(construct, targetingSelectionType, targetList);
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructTargetingList(construct, targetingSelectionType, targetList);
            }
        }
    }

    public static async Task ModifyConstructDamageAdditive(ConstructEffectContext context)
    {
        Creature construct = context.Construct;
        Player owner = construct.PetOwner;
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructDamageAdditive(context);
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructDamageAdditive(context);
            }
        }
    }

    public static async Task ModifyConstructDamageMultiplicative(ConstructEffectContext context)
    {
        Creature construct = context.Construct;
        Player owner = construct.PetOwner;
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructDamageMultiplicative(context);
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructDamageMultiplicative(context);
            }
        }
    }

    public static async Task ModifyConstructBlockAdditive(ConstructEffectContext context)
    {
        Creature construct = context.Construct;
        Player owner = construct.PetOwner;
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructBlockAdditive(context);
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructBlockAdditive(context);
            }
        }
    }

    public static async Task ModifyConstructBlockMultiplicative(ConstructEffectContext context)
    {
        Creature construct = context.Construct;
        Player owner = construct.PetOwner;
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructBlockMultiplicative(context);
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructBlockMultiplicative(context);
            }
        }
    }

    public static async Task ModifyConstructPowerList(ConstructEffectContext context)
    {
        Creature construct = context.Construct;
        Player owner = construct.PetOwner;
        var ownerPowers = owner.Creature.Powers.ToList();
        var constructPowers = construct.Powers.ToList();

        foreach (var power in ownerPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructPowerList(context);
            }
        }

        foreach (var power in constructPowers)
        {
            if (power is SpireOfFactorioPower sPower)
            {
                await sPower.ModifyConstructPowerList(context);
            }
        }
    }

    public static async Task ApplyConstructPower(
        PlayerChoiceContext choiceContext,
        Type powerType,
        Creature target,
        int amount,
        Creature applier)
    {
        if (powerType == typeof(ScorchPower))
        {
            await PowerCmd.Apply<ScorchPower>(
                choiceContext, target, amount, applier, null);
        }
        else if (powerType == typeof(ShockPower))
        {
            await PowerCmd.Apply<ShockPower>(
                choiceContext, target, amount, applier, null);
        }
    }

}