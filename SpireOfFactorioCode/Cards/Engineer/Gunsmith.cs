using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Gunsmith : EngineerCard
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Gunsmith() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    private List<CardModel> CreateCardsForGunsmith(Player p)
    {
        if (p == null || p.Creature == null)
        {
            return [];
        }
        List<CardModel> outputCards = new List<CardModel>();

        outputCards.Add(p.Creature.CombatState.CreateCard(ModelDb.Card<TheNuclearOption>(), p));
        outputCards.Add(p.Creature.CombatState.CreateCard(ModelDb.Card<RailgunShot>(), p));
        if (p.Character is Characters.Engineer)
        {
            outputCards.Add(p.Creature.CombatState.CreateCard(ModelDb.Card<RainOfTrash>(), p));
        }
        if (IsUpgraded || p.Character is Characters.Engineer)
        {
            outputCards.Add(p.Creature.CombatState.CreateCard(ModelDb.Card<ShockAndAwe>(), p));
            outputCards.Add(p.Creature.CombatState.CreateCard(ModelDb.Card<TeslaTurret>(), p));
        }

        return outputCards;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        foreach (Player p in CombatState.Players)
        {
            List<CardModel> options = CreateCardsForGunsmith(p);
            foreach (CardModel card in options)
            {
                card.AddKeyword(CardKeyword.Ethereal);
                card.AddKeyword(CardKeyword.Exhaust);
            }
            if (IsUpgraded)
            {
                foreach (CardModel card in options)
                {
                    card.SetToFreeThisTurn();
                    var ScrapCost = CustomResources<Scrap>.Cost(card);
                    if (ScrapCost != null)
                    {
                        ScrapCost.SetThisTurnOrUntilPlayed(0);
                    }
                }
            }

            CardPile playerHand = PileType.Hand.GetPile(p);
            List<CardModel> selection = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, p, new CardSelectorPrefs(base.SelectionScreenPrompt, 1))).ToList();
            foreach (CardModel option in selection)
            {
                CardPileAddResult? addedCard = await CardPileCmd.Add(option, playerHand, CardPilePosition.Top);
            }
        }
    }

    protected override void OnUpgrade()
    {
        //RemoveKeyword(CardKeyword.Exhaust);
    }

}