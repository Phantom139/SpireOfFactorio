using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class BigMiningDrill : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 1m), new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BigMiningDrillPower>()];

    public BigMiningDrill() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BigMiningDrillPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        AddKeyword(CardKeyword.Retain);
    }

}
