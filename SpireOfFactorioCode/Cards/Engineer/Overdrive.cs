using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Overdrive : EngineerCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OverclockPower>("Overclock", 2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverclockPower>()];

    public Overdrive() : base(1, CardType.Skill, CardRarity.Uncommon, ConstructTargetTypes.AnyAlliedConstruct)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Safety check :)
        if (cardPlay.Target?.Monster is ConstructModel constructModel)
        {
            await PowerCmd.Apply<OverclockPower>(choiceContext, cardPlay.Target, DynamicVars["Overclock"].BaseValue, base.Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["Overclock"].UpgradeValueBy(1m);
    }

}
