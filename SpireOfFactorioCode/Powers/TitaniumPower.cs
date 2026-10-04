using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class TitaniumPower : SpireOfFactorioPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override string? CustomBigBetaIconPath => "res://images/powers/plating_power.png";
    public override string CustomBigIconPath => "res://images/powers/plating_power.png";
    public override string CustomPackedIconPath => "res://images/powers/plating_power.png";

    //
    // Summary:
    //     We want enemies that start with Plating to also start combat with block.
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player)
        {
            return Task.CompletedTask;
        }

        if (base.Owner.IsPlayer)
        {
            return Task.CompletedTask;
        }

        if (combatState.RoundNumber > 1)
        {
            return Task.CompletedTask;
        }

        return CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);
    }

    //
    // Summary:
    //     We do this in early so that it triggers before end-of-turn damage effects.
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);
        }
    }

}
