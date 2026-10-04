using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class DeeperResearch : EngineerCard
{

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [HoverTipFactory.FromPower<DeeperResearchUpgradedPower>()];
            }
            else
            {
                return [HoverTipFactory.FromPower<DeeperResearchPower>()];
            }
        }
    }

    public DeeperResearch() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await PowerCmd.Apply<DeeperResearchUpgradedPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, cardPlay.Card);
        }
        else
        {
            await PowerCmd.Apply<DeeperResearchPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, cardPlay.Card);
        }
    }

}
