using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Characters;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class OilSlick : EngineerCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlipperyPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SlipperyPower>("Slippery", 1m)];

    public OilSlick() : base(1, CardType.Skill, CardRarity.Uncommon, ConstructTargetTypes.AnyAlliedConstruct)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Safety check :)
        if (cardPlay.Target?.Monster is ConstructModel)
        {
            await PowerCmd.Apply<SlipperyPower>(choiceContext, cardPlay.Target, DynamicVars["Slippery"].BaseValue, base.Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Slippery"].UpgradeValueBy(1m);
    }

}
