using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Status;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class UtilitySciencePack : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("Cards", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> list = new List<IHoverTip>();
            list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<ComplexRecipe>());
            return list;
        }
    }

    private CardModel CreateComplexRecipe()
    {
        return base.Owner.Creature.CombatState.CreateCard(
            ModelDb.Card<ComplexRecipe>(),
            base.Owner);
    }

    /**
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        CardModel replace = CreateComplexRecipe();
        if (card.Type == CardType.Status && (card.Id != replace.Id && card != replace) && (card.Id != ModelDb.Card<FranticEscape>().Id))
        {
            await Cmd.Wait(0.01f);
            await CardPileCmd.RemoveFromCombat(card);
            await CardPileCmd.Add(replace, PileType.Hand, CardPilePosition.Bottom);
            Flash();
        }
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        //CardModel replace = CreateComplexRecipe();
        //if (card.Type == CardType.Status && (card.Id != replace.Id && card != replace) && (card.Id != ModelDb.Card<FranticEscape>().Id))
        //{
        //    await Cmd.Wait(0.01f);
        //    await CardPileCmd.RemoveFromCombat(card);
        //    await CardPileCmd.Add(replace, PileType.Hand, CardPilePosition.Bottom);
        //    Flash();
        //}
    }
    **/

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        int replaces = 0;
        CardModel replace = ModelDb.Card<ComplexRecipe>();

        List<CardModel> cards = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        foreach (CardModel cI in cards)
        {
            if (cI.Type == CardType.Status && (cI.Id != replace.Id && cI != replace) && (cI.Id != ModelDb.Card<FranticEscape>().Id))
            {
                await CardPileCmd.RemoveFromCombat(cI);
                replaces++;
            }
        }
        if (replaces > 0)
        {
            Flash();
        }
        List<CardModel> listAdd = new List<CardModel>();
        for (int i = 0; i < replaces; i++)
        {
            listAdd.Add(base.Owner.Creature.CombatState.CreateCard<ComplexRecipe>(base.Owner));
        }
        await CardPileCmd.AddGeneratedCardsToCombat(listAdd, PileType.Hand, player);
    }

}