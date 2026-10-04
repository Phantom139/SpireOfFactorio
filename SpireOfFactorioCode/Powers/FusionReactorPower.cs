using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class FusionReactorPower : SpireOfFactorioPower
{
    public static int ConstructIncrease = 2;

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Single;

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!base.Owner.IsPlayer)
        {
            return Task.CompletedTask;
        }
        // 
        EngineerCombatState.MaxConstructs[base.Owner.Player.PlayerCombatState] += (int)amount * ConstructIncrease;

        return Task.CompletedTask;
    }

    public override async Task OnConstructCreated(PlayerChoiceContext choiceContext, Creature construct, Player owner, ConstructCreationArgs args)
    {
        await PowerCmd.Apply<OverclockPower>(choiceContext, construct, 1m, owner.Creature, null);
    }
}
