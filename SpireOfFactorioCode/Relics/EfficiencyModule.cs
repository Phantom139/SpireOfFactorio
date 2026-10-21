using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class EfficiencyModule : SpireOfFactorioRelic
{
    //public override RelicRarity Rarity => RelicRarity.Event;
    public override RelicRarity Rarity => RelicRarity.Event;

    private const string _costIncreaseKey = "EfficiencyModuleCardsPlayed";
    private const int _numberOfExtrasBeforeCostUp = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new StarsVar(1), new ScrapVar(1), new DynamicVar(_costIncreaseKey, _numberOfExtrasBeforeCostUp)];

    public override bool ShowCounter => true;
    private int _currentExtraPlays = 0;

    private readonly List<CardModel> modifiedCards = new();

    public override int DisplayAmount
    {
        get
        {
            return _numberOfExtrasBeforeCostUp - CurrentExtraPlays;
        }
    }

    [SavedProperty]
    public int CurrentExtraPlays
    {
        get
        {
            return _currentExtraPlays;
        }
        set
        {
            AssertMutable();
            _currentExtraPlays = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == base.Owner)
        {
            //MainFile.Logger.Info($"[EfficiencyModule] Draw {card.Id.ToString()}");
            if (CurrentExtraPlays > _numberOfExtrasBeforeCostUp)
            {
                //MainFile.Logger.Info($"[EfficiencyModule] Stopped... {CurrentExtraPlays} > {_numberOfExtrasBeforeCostUp}");
                return Task.CompletedTask;
            }
            int cardCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
            if (cardCost > 1 && !card.EnergyCost.CostsX)
            {
                //MainFile.Logger.Info($"[EfficiencyModule] {card.Id.ToString()} energy cost is reducible");
                card.EnergyCost.AddThisTurnOrUntilPlayed(-1);

                int starCost = card.CurrentStarCost;
                if (starCost > 0 && !card.EnergyCost.CostsX && !card.HasStarCostX)
                {
                    card.SetStarCostThisTurn(starCost - 1);
                }

                var scrap = CustomResources<Scrap>.Cost(card);
                if (scrap != null)
                {
                    int scrapCost = scrap.GetWithModifiers(CostModifiers.All);
                    bool scrapX = scrap.CostsX;

                    //MainFile.Logger.Info($"[EfficiencyModule] {card.Id.ToString()} has a scrap cost. X? {scrapX} #: {scrapCost}");

                    if (scrapCost > 0 && !scrapX)
                    {
                        //MainFile.Logger.Info($"[EfficiencyModule] {card.Id.ToString()} scrap cost is reducible");
                        scrap.SetThisTurnOrUntilPlayed(scrapCost - 1);
                    }
                }

                modifiedCards.Add(card);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (modifiedCards.Contains(cardPlay.Card))
        {
            if (CurrentExtraPlays < _numberOfExtrasBeforeCostUp)
            {
                CurrentExtraPlays++;
            }
            modifiedCards.Remove(cardPlay.Card);
        }
        if (CurrentExtraPlays >= _numberOfExtrasBeforeCostUp)
        {
            foreach (CardModel card in modifiedCards)
            {
                card.EnergyCost.AddThisTurnOrUntilPlayed(1);

                int starCost = card.CurrentStarCost;
                if (starCost > 0 && !card.EnergyCost.CostsX && !card.HasStarCostX)
                {
                    card.SetStarCostThisTurn(starCost + 1);
                }

                var scrap = CustomResources<Scrap>.Cost(card);
                if (scrap != null)
                {
                    int scrapCost = scrap!.Base;
                    bool scrapX = scrap.CostsX;

                    if (scrapCost > 0 && !card.EnergyCost.CostsX && !scrapX)
                    {
                        scrap.SetThisTurnOrUntilPlayed(scrapCost + 1);
                    }
                }
            }
            modifiedCards.Clear();
        }
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            modifiedCards.Clear();

            CurrentExtraPlays = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        CurrentExtraPlays = 0;
        return Task.CompletedTask;
    }

}