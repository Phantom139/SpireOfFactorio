using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class DisabledPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Debuff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            // Reduce the counter by one, can be modified
            //
            await PowerCmd.Decrement(this);
        }
    }
}
