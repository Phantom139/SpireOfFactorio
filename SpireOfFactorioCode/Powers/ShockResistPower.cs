using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ShockResistPower : SpireOfFactorioPower
{
    public static int DefaultTurnCount = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ShockPower>("Shock", 25m)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (target == base.Owner)
        {
            if (power is ShockPower)
            {
                return (DynamicVars["Shock"].BaseValue / 100);
            }
        }
        return 1m;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        CombatSide applyingSide = Applier.Side;
        if (applyingSide == null)
        {
            applyingSide = CombatSide.Player;
        }
        if (side == applyingSide)
        {
            await PowerCmd.Decrement(this);
        }
    }
}
