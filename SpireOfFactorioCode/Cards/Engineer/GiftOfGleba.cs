using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class GiftOfGleba : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CropHarvestPower>("CropHarvest", 15m), new DynamicVar("Turns", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CropHarvestPower>()];

    public GiftOfGleba() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CropHarvestPower>(choiceContext, base.Owner.Creature, DynamicVars["CropHarvest"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars["CropHarvest"].UpgradeValueBy(5m);
        DynamicVars["Turns"].UpgradeValueBy(-1m);
    }

}
