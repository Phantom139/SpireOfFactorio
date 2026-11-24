using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

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

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player)
        {
            return;
        }
        Player player = base.Owner;
        if (player.PlayerCombatState.TurnNumber != 1)
        {
            return;
        }
        // Apply the stat penalties.
        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            -DynamicVars.Dexterity.BaseValue,
            player.Creature,
            null);

        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            -DynamicVars.Strength.BaseValue,
            player.Creature,
            null);

        // Snapshot all Power cards in the combat draw pile.
        List<CardModel> powerCards = PileType.Draw
            .GetPile(player)
            .Cards
            .Where(c => c.Type == CardType.Power)
            .ToList();

        foreach (CardModel card in powerCards)
        {
            await CardPileCmd.Add(card, PileType.Play);

            await CardCmd.AutoPlay(
                new BlockingPlayerChoiceContext(),
                card,
                null);
        }
    }

}