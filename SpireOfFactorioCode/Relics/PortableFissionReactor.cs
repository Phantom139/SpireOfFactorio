using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class PortableFissionReactor : SpireOfFactorioRelic
{
    //public override RelicRarity Rarity => RelicRarity.Event;
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(3m), new PowerVar<DexterityPower>(3m)];

    /**
    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (card.Type == CardType.Power)
        {
            int cost = card.EnergyCost.Canonical * -1;
            if (cost != 0)
            {
                card.EnergyCost.AddThisCombat(cost);
            }
        }
        return Task.CompletedTask;
    }
    **/

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, -DynamicVars.Dexterity.BaseValue, base.Owner.Creature, null);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, -DynamicVars.Strength.BaseValue, base.Owner.Creature, null);

            foreach (CardModel card in Owner.Deck.Cards)
            {
                if (card.Type == CardType.Power)
                {
                    await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), card, base.Owner.Creature);
                }
            }
        }
    }

}