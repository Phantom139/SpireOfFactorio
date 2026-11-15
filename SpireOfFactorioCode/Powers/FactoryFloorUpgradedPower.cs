using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Token;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class FactoryFloorUpgradedPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!base.Owner.IsPlayer)
        {
            return;
        }
        if (cardPlay.Player != base.Owner.Player)
        {
            return;
        }
        var scrapCost = CustomResources<Scrap>.Cost(cardPlay.Card);
        if (scrapCost == null)
        {
            return;
        }
        await Logistics.CreateInHand(cardPlay.Player, 1, CombatState, base.Owner.Player, true);
    }

}
