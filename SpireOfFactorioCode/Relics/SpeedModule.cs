using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class SpeedModule : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool ShowCounter => true;
    private int _currentExtraPlays = 0;

    private CardModel? storedCard;

    private readonly HashSet<CardModel> _speedModuleCards = new();

    [SavedProperty]
    public int StoredCardCost { get; set; }

    public override int DisplayAmount
    {
        get
        {
            return CurrentExtraPlays;
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (storedCard == null)
                return [];

            return [new CardHoverTip(storedCard)];
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Ignore autoplay, other players' cards, and cards created by Speed Module.
        if (cardPlay.IsAutoPlay ||
            cardPlay.Card.Owner != base.Owner ||
            _speedModuleCards.Contains(cardPlay.Card))
        {
            return;
        }

        if (storedCard == null)
        {
            storedCard = cardPlay.Card;
            StoredCardCost = cardPlay.Card.EnergyCost.GetWithModifiers(
                CostModifiers.All);

            InvokeDisplayAmountChanged();
        }
        else if (cardPlay.Card != storedCard)
        {
            CurrentExtraPlays++;
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Creature.Side)
        {
            if (storedCard != null)
            {
                // Add a copy of the original to the hand with extra cost, and replay X
                CardPile playerHand = PileType.Hand.GetPile(base.Owner);
                CardModel newCard = storedCard.CreateClone();

                if (!newCard.EnergyCost.CostsX)
                {
                    // We grab the current cost so each subsequent turn adds more
                    newCard.EnergyCost.SetThisTurnOrUntilPlayed(StoredCardCost + (int)DynamicVars.Energy.BaseValue);
                }

                // Replace the existing replay count rather than accumulating it.
                newCard.BaseReplayCount = CurrentExtraPlays;

                // Remember this exact card instance so it cannot feed back into the relic.
                _speedModuleCards.Add(newCard);
                newCard.AddKeyword(CardKeyword.Eternal);
                newCard.AddKeyword(CardKeyword.Exhaust);

                CardPileCmd.Add(newCard, playerHand, CardPilePosition.Top);
                // Clear the old values
                storedCard = null;
                CurrentExtraPlays = 0;
                InvokeDisplayAmountChanged();

                Flash();
            }
        }

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        CurrentExtraPlays = 0;
        storedCard = null;
        _speedModuleCards.Clear();
        return Task.CompletedTask;
    }

}