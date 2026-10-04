using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class ProductivityModule : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    private class CardRestoreData
    {
        public decimal? Damage;
        public decimal? Block;
    }

    private readonly Dictionary<CardModel, CardRestoreData> modifiedCards = new();

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        modifiedCards.Clear();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        List<CardModel> cards = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        CardModel card = cardPlay.Card;

        foreach (CardModel cI in cards)
        {
            if (cI != card && cI.Id == card.Id)
            {
                if (card.EnergyCost.GetResolved() > 0 && card.EnergyCost.CostsX == false)
                {
                    if (!modifiedCards.ContainsKey(cI))
                    {
                        modifiedCards[cI] = new CardRestoreData();
                        if (cI.DynamicVars.TryGetValue("Damage", out var damage1))
                            modifiedCards[cI].Damage = damage1.BaseValue;
                        if (cI.DynamicVars.TryGetValue("Block", out var block1))
                            modifiedCards[cI].Block = block1.BaseValue;

                    }

                    Flash();
                    cI.EnergyCost.AddThisTurn(-1);
                    if (cI.DynamicVars.TryGetValue("Damage", out var damage2))
                        damage2.BaseValue = Math.Ceiling(damage2.BaseValue * 0.8m);
                    if (cI.DynamicVars.TryGetValue("Block", out var block2))
                        block2.BaseValue = Math.Ceiling(block2.BaseValue * 0.8m);
                }

            }
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != base.Owner.Creature.Side)
        {
            return Task.CompletedTask;
        }
        foreach (var kvp in modifiedCards)
        {
            CardModel card = kvp.Key;
            CardRestoreData data = kvp.Value;

            if (card.DynamicVars.TryGetValue("Damage", out var damage1) && data.Damage.HasValue)
                damage1.BaseValue = data.Damage.Value;
            if (card.DynamicVars.TryGetValue("Block", out var block1) && data.Block.HasValue)
                block1.BaseValue = data.Block.Value;

        }

        modifiedCards.Clear();
        return Task.CompletedTask;
    }

}