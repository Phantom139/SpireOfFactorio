using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class FactoryFloor : EngineerCard
{

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [HoverTipFactory.FromPower<FactoryFloorUpgradedPower>()];
            }
            else
            {
                return [HoverTipFactory.FromPower<FactoryFloorPower>()];
            }
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScrapVar("Scrap", 1)];

    public FactoryFloor() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await PowerCmd.Apply<FactoryFloorUpgradedPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, cardPlay.Card);
        }
        else
        {
            await PowerCmd.Apply<FactoryFloorPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, cardPlay.Card);
        }
    }

}
