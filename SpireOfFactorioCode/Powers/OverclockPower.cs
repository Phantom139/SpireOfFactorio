using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Relics;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

// Phantom Note: I'm sure there are better ways to handle this power, but I like having a case-by-case code block for each instead of generic modifiers since we deal with powers, damage, and block in various spots.
public sealed class OverclockPower : SpireOfFactorioPower
{
    // This is a percent number (e.g., 20% would be 20m), Ref: CustomConstructModel.OverclockPercent()
    public static decimal OverclockMultiplier = 50m;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", OverclockMultiplier)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverloadPower>()];

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        int currentAmount = base.Amount;
        decimal currentString = DynamicVars["Percent"].BaseValue;
        decimal change = (currentAmount * OverclockMultiplier) - currentString;

        DynamicVars["Percent"].UpgradeValueBy(change);
        return Task.CompletedTask;
    }

    public override async Task OnConstructPerformedTask(PlayerChoiceContext choiceContext, Creature construct, Player owner)
    {
        if (owner.Creature.IsDead)
        {
            return;
        }
        if (!construct.HasPower<OverclockPower>())
        {
            return;
        }
        // Check for Fusion Reactor
        if (owner.Creature.GetPower<FusionReactorPower>() != null)
        {
            return;
        }
        // Check for Supercapacitor
        decimal minimumStacks = 0;
        if (owner.GetRelic<Supercapacitor>() != null)
        {
            minimumStacks = Supercapacitor.SupercapacitorStacks;
        }
        // Check the count
        if (construct.GetPowerAmount<OverclockPower>() > minimumStacks)
        {
            ConstructModel c = construct.Monster as ConstructModel;
            if (c.ActionPhase != ConstructActionPhase.AfterPlayerTurn)
            {
                // If the construct is not firing when the player's turn ends, we can just apply the debuff immediately.
                await PowerCmd.Apply<OverloadPower>(choiceContext, owner.Creature, 1m, construct, null);
            }
            else
            {
                // Otherwise, we need to add to the pending overload.
                EngineerCombatState.PendingOverload[owner.PlayerCombatState]++;
            }
        }
        // Finally, decrease the Overclock count
        await PowerCmd.Decrement(this);
    }
}
