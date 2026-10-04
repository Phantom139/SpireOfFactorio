using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;


/**
 * Saving for now:
  "SPIREOFFACTORIO-LOGISTICS_ROBOT.title": "Logistics Robot",
  "SPIREOFFACTORIO-LOGISTICS_ROBOT.description": "Select another player, take {SelectCount} card{SelectCount:plural:|s} from that players discard pile and add it to your hand.{IfUpgraded:show:\nIf you play that card this turn, upgrade it and place it on the top of that player's deck.[/gold]}",
  "SPIREOFFACTORIO-LOGISTICS_ROBOT.selectionScreenPrompt": "Select {SelectCount} card{SelectCount:plural:|s} to add to your hand.",
 */
public sealed class LogisticsRobot : EngineerCard
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("SelectCount", 1m)];

    public LogisticsRobot()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardPile playerHand = PileType.Hand.GetPile(base.Owner);
        foreach (CardModel item in await CardSelectCmd.FromCombatPile(choiceContext, PileType.Discard.GetPile(cardPlay.Target.Player), base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 0, DynamicVars["SelectCount"].IntValue)))
        {
            // NOTE: We have to do some "hacky" stuff here, we're not actually moving the original card around, but simply creating a copy of it, while removing the old one. 
            CardModel CopiedCard = base.Owner.Creature.CombatState.CreateCard(item.CanonicalInstance, base.Owner);
            // Now we need to copy all "aspects" of the original card since we've essentially just created a new version of the old one.
            CopiedCard.Affliction = item.Affliction;
            CopiedCard.CurrentUpgradeLevel = item.CurrentUpgradeLevel;
            CopiedCard.Enchantment = item.Enchantment;

            CardModel ClonedCard = item.CreateClone();
            // The original card being "moved" has to be dropped from the original player's deck, unless upgraded in which case we use a "trick" to "move" it.
            await CardPileCmd.RemoveFromCombat(item);
            await CardPileCmd.Add(CopiedCard, playerHand, CardPilePosition.Top);

            CopiedCard.SetToFreeThisTurn();
            // If it has a scrap cost, make it free as well
            var scrapCost = CustomResources<Scrap>.Cost(CopiedCard);
            if (scrapCost != null)
            {
                scrapCost.SetThisTurnOrUntilPlayed(0);
            }

            if (IsUpgraded)
            {
                // Guarentee upgrade the copied card
                CardCmd.Upgrade(CopiedCard);
                // For the upgraded version we take a copy of the original card and place it on the top of the player's deck.
                CardCmd.Upgrade(ClonedCard);
                await CardPileCmd.Add(ClonedCard, PileType.Draw, CardPilePosition.Top);
            }
        }
    }

    protected override void OnUpgrade()
    {

    }
}
