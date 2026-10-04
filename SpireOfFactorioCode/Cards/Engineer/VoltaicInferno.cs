using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class VoltaicInferno : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FissionReactorPower>("VoltaicInferno", 5m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ScorchPower>()];

    public VoltaicInferno() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VoltaicInfernoPower>(choiceContext, base.Owner.Creature, DynamicVars["VoltaicInferno"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VoltaicInferno"].UpgradeValueBy(5m);
    }


}
