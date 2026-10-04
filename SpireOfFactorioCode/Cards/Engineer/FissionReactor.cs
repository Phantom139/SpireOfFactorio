using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class FissionReactor : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FissionReactorPower>("FissionReactor", 1m)];

    //protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FissionReactorPower>()];

    public FissionReactor() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FissionReactorPower>(choiceContext, base.Owner.Creature, DynamicVars["FissionReactor"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FissionReactor"].UpgradeValueBy(1m);
    }


}
