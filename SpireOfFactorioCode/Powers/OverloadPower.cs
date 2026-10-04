using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class OverloadPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new StarsVar(1), new DynamicVar("Scrap", 1m)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Debuff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (Amount > 0)
        {
            card.EnergyCost.AddThisTurnOrUntilPlayed(1);
            if (card.CanonicalStarCost > 0)
            {
                card.SetStarCostThisTurn(1);
            }
            var scrap = CustomResources<Scrap>.Cost(card);
            if (scrap != null && scrap.Canonical > 0)
            {
                scrap.AddThisTurnOrUntilPlayed(1);
            }
            Flash();
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!base.Owner.IsPlayer)
        {
            return;
        }
        if (cardPlay.Card.Owner == base.Owner.Player)
        {
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            if (Amount <= 1)
            {
                await PowerCmd.Remove(this);
            }
            else
            {
                Decimal reduction = decimal.Floor(Amount * 0.5m);
                await PowerCmd.ModifyAmount(choiceContext, this, reduction, null, null);
            }
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            foreach (Creature creature in participants)
            {
                if (creature.IsPlayer)
                {
                    if (creature.Player.PlayerCombatState == null)
                    {
                        continue;
                    }
                    if (EngineerCombatState.PendingOverload[creature.Player.PlayerCombatState] > 0)
                    {
                        await PowerCmd.Apply<OverloadPower>(choiceContext, creature, EngineerCombatState.PendingOverload[creature.Player.PlayerCombatState], null, null);
                        EngineerCombatState.PendingOverload[creature.Player.PlayerCombatState] = 0;
                    }
                }
            }
        }
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (base.Owner.IsPlayer)
        {
            List<CardModel> cards = PileType.Hand.GetPile(base.Owner.Player).Cards.ToList();
            foreach (CardModel card in cards)
            {
                card.EnergyCost.AddThisTurnOrUntilPlayed(1);
                if (card.CanonicalStarCost > 0)
                {
                    card.SetStarCostThisTurn(1);
                }
                var scrap = CustomResources<Scrap>.Cost(card);
                if (scrap != null && scrap.Canonical > 0)
                {
                    scrap.AddThisTurnOrUntilPlayed(1);
                }
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.IsPlayer)
        {
            List<CardModel> cards = PileType.Hand.GetPile(oldOwner.Player).Cards.ToList();
            foreach (CardModel card in cards)
            {
                card.EnergyCost.AddThisTurnOrUntilPlayed(-1);
                if (card.CanonicalStarCost > 0)
                {
                    card.SetStarCostThisTurn(-1);
                }
                var scrap = CustomResources<Scrap>.Cost(card);
                if (scrap != null && scrap.Canonical > 0)
                {
                    scrap.AddThisTurnOrUntilPlayed(-1);
                }
            }
        }
        return Task.CompletedTask;
    }

}
