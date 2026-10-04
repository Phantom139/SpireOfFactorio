using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class EfficientPoweringNetwork : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EfficientPoweringNetworkPower>("EfficientPoweringNetwork", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverloadPower>()];

    public EfficientPoweringNetwork() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EfficientPoweringNetworkPower>(choiceContext, base.Owner.Creature, DynamicVars["EfficientPoweringNetwork"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EfficientPoweringNetwork"].UpgradeValueBy(2m);
        AddKeyword(CardKeyword.Innate);
    }

}