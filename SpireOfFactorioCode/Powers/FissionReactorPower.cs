using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Characters;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class FissionReactorPower : SpireOfFactorioPower
{
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
        if (!base.Owner.IsPlayer)
        {
            return Task.CompletedTask;
        }
        EngineerCombatState.MaxConstructs[base.Owner.Player.PlayerCombatState] += (int)amount;

        return Task.CompletedTask;
    }
}
