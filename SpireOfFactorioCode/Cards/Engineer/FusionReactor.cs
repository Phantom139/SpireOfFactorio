using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class FusionReactor : EngineerCard, ITomeCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FusionReactorPower>("FusionReactor", 1m), new DynamicVar("FusionReactorSlots", (decimal)FusionReactorPower.ConstructIncrease)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverclockPower>(), HoverTipFactory.FromPower<OverloadPower>()];

    public FusionReactor() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FusionReactorPower>(choiceContext, base.Owner.Creature, DynamicVars["FusionReactor"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }


}
