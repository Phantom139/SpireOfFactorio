using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class DeeperResearchPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!base.Owner.IsPlayer)
        {
            return;
        }
        if (cardPlay.Card.Pool is ColorlessCardPool)
        {
            // Remove generated "colorless" cards (e.g., Shiv, Sovereign Blade, Soul)
            if (cardPlay.Card is not Shiv && cardPlay.Card is not SovereignBlade && cardPlay.Card is not Soul
                && cardPlay.Card is not MinionDiveBomb && cardPlay.Card is not MinionSacrifice && cardPlay.Card is not MinionStrike
                && cardPlay.Card is not SweepingGaze)
            {
                List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner.Player, ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(base.Owner.Player.UnlockState, base.Owner.Player.RunState.CardMultiplayerConstraint), base.Amount, base.Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
                await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, base.Owner.Player);
                Flash();
            }
        }
    }

}
