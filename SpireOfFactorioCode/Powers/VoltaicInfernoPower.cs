using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class VoltaicInfernoPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        int VoltaicInfernoAmount = 0;
        if (power is ShockPower)
        {
            if (applier == null)
            {
                return;
            }
            if (!applier.IsPlayer)
            {
                if (applier.Monster is ConstructModel)
                {
                    VoltaicInfernoAmount = applier.PetOwner.Creature.GetPowerAmount<VoltaicInfernoPower>();
                }
            }
            else
            {
                VoltaicInfernoAmount = applier.GetPowerAmount<VoltaicInfernoPower>();
            }
            if (amount > 0)
            {
                await PowerCmd.Apply<ScorchPower>(choiceContext, power.Owner, VoltaicInfernoAmount, applier, null);
            }
        }
    }

}
