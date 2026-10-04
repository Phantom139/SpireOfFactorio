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

public sealed class PromptCritical : EngineerCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OverclockPower>("Overclock", 10m), new PowerVar<PromptCriticalPower>("PromptCritical", 10m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverclockPower>()];

    public PromptCritical() : base(3, CardType.Skill, CardRarity.Rare, ConstructTargetTypes.AnyConstructIOwn)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Safety check :)
        if (cardPlay.Target?.Monster is ConstructModel constructModel)
        {
            await PowerCmd.Apply<OverclockPower>(choiceContext, cardPlay.Target, DynamicVars["Overclock"].BaseValue, base.Owner.Creature, cardPlay.Card);
            await PowerCmd.Apply<PromptCriticalPower>(choiceContext, cardPlay.Target, DynamicVars["PromptCritical"].BaseValue, base.Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["Overclock"].UpgradeValueBy(5m);
        DynamicVars["PromptCritical"].UpgradeValueBy(10m);
    }

}
