using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ReducedScrapCostPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 1m)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (!Owner.IsPlayer)
        {
            return Task.CompletedTask;
        }
        if (card.Owner != Owner.Player)
        {
            return Task.CompletedTask;
        }
        if (Amount > 0)
        {
            var scrap = CustomResources<Scrap>.Cost(card);
            if (scrap != null && !scrap.CostsX)
            {
                if (scrap.Canonical == 1)
                {
                    scrap.AddThisTurnOrUntilPlayed(-1);
                }
                else
                {
                    int baseCost = scrap.Canonical;
                    decimal adjusted = decimal.Ceiling((decimal)baseCost / 2);
                    scrap.AddThisTurnOrUntilPlayed(((int)adjusted - baseCost));
                }
            }
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!base.Owner.IsPlayer)
        {
            return;
        }
        if (cardPlay.Card.Owner == base.Owner.Player)
        {
            var scrap = CustomResources<Scrap>.Cost(cardPlay.Card);
            if (scrap != null && !scrap.CostsX)
            {
                await PowerCmd.Decrement(this);
            }
        }
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (base.Owner.IsPlayer)
        {
            List<CardModel> cards = PileType.Hand.GetPile(base.Owner.Player).Cards.ToList();
            foreach (CardModel card in cards)
            {
                var scrap = CustomResources<Scrap>.Cost(card);
                if (scrap != null && !scrap.CostsX)
                {
                    if (scrap.Canonical == 1)
                    {
                        scrap.AddThisTurnOrUntilPlayed(-1);
                    }
                    else
                    {
                        int baseCost = scrap.Canonical;
                        decimal adjusted = decimal.Ceiling((decimal)baseCost / 2);
                        scrap.AddThisTurnOrUntilPlayed(((int)adjusted - baseCost));
                    }
                }
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.IsPlayer)
        {
            List<CardModel> cards = PileType.Hand.GetPile(oldOwner.Player).Cards.ToList();
            foreach (CardModel card in cards)
            {
                var scrap = CustomResources<Scrap>.Cost(card);
                if (scrap != null && !scrap.CostsX)
                {
                    scrap.AfterCardPlayedCleanup();
                }
            }
        }
        return Task.CompletedTask;
    }

}
