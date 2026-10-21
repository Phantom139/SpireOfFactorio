using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!base.Owner.IsPlayer)
        {
            return;
        }
        if (side == CombatSide.Player)
        {
            int currentActiveConstructs = EngineerCmd.GetConstructCount(base.Owner.Player);
            int maxConstructs = base.Owner.Player.Character is Engineer
                ? EngineerCombatState.MaxConstructs[base.Owner.Player.PlayerCombatState]
                : 1;

            if (currentActiveConstructs == 0)
            {
                await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
                return;
            }

            decimal perc = currentActiveConstructs / maxConstructs;
            if (perc <= 0.5m)
            {
                await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
            }

        }
    }
}
