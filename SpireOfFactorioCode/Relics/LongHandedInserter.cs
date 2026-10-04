using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class LongHandedInserter : SpireOfFactorioRelic
{
    //public override RelicRarity Rarity => RelicRarity.Event;
    public override RelicRarity Rarity => RelicRarity.Event;

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != base.Owner.Creature.Side)
        {
            return Task.CompletedTask;
        }

        IEnumerable<CardModel> cards = PileType.Discard.GetPile(base.Owner).Cards;
        CardModel random = cards.TakeRandom(1, base.Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (random != null)
        {
            CardPileCmd.Add(random, PileType.Draw, CardPilePosition.Top);
        }

        return Task.CompletedTask;
    }

}