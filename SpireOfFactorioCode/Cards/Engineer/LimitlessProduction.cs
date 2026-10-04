using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class LimitlessProduction : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new DynamicVar("Scrap", 1m)];

    private readonly List<CardModel> CardsWithAddedScrap = new();

    public LimitlessProduction() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> HandCards = PileType.Hand.GetPile(base.Owner).Cards;
        foreach (CardModel card in HandCards)
        {
            // Fetch the current energy cost
            if (card.EnergyCost.CostsX)
            {
                // X cost cards are skipped
                continue;
            }
            int currentEnergyCost = card.EnergyCost.Canonical;
            int currentScrapCost = 0;
            // Fetch the scrap cost object (if it exists)
            var scrapCost = CustomResources<Scrap>.Cost(card);
            if (scrapCost != null)
            {
                // If X, skip
                if (scrapCost.CostsX)
                {
                    continue;
                }
                currentScrapCost = scrapCost.Canonical;
                // Add the current cost up as a temporary for this turn
                scrapCost.AddThisTurnOrUntilPlayed(currentEnergyCost);
            }
            else
            {
                //Create a "temporary" scrap cost, we add it to a list of cards and once played or the turn ends, it is removed.
                CustomResources<Scrap>.SetCanonicalCost(card, 1);
                var newScrapCost = CustomResources<Scrap>.Cost(card);
                newScrapCost.SetThisTurnOrUntilPlayed(currentEnergyCost);
                CardsWithAddedScrap.Add(card);

                //MainFile.Logger.Info($"[LimitlessProduction] Added Scrap Cost Element to {card.Id} - Scrap: {newScrapCost.Canonical}, Original Energy: {currentEnergyCost}");
            }
            //Clear the energy cost
            card.EnergyCost.SetThisTurnOrUntilPlayed(0);
        }

    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CardsWithAddedScrap.Contains(cardPlay.Card))
        {
            var newScrapCost = CustomResources<Scrap>.Cost(cardPlay.Card);
            newScrapCost.SetCustomBaseCost(-1);
        }
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            foreach (CardModel card in CardsWithAddedScrap)
            {
                var newScrapCost = CustomResources<Scrap>.Cost(card);
                newScrapCost.SetCustomBaseCost(-1);
            }
            CardsWithAddedScrap.Clear();
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

}
