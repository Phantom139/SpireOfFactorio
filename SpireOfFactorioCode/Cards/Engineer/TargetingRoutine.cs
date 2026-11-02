using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class TargetingRoutine : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TargetingRoutinePower>("TargetingRoutine", 15m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TargetingRoutinePower>()];

    public TargetingRoutine()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<TargetingRoutinePower>(choiceContext, cardPlay.Target, base.DynamicVars["TargetingRoutine"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TargetingRoutine"].UpgradeValueBy(10m);
    }
}
