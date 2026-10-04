using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class ConstructionRobot : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ConstructionRobotPower>("ConstructionRobot", 4m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ConstructionRobotPower>()];

    public ConstructionRobot() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ConstructionRobotPower>(choiceContext, base.Owner.Creature, DynamicVars["ConstructionRobot"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ConstructionRobot"].UpgradeValueBy(1m);
    }


}
