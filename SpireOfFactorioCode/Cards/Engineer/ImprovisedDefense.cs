using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class ImprovisedDefense : EngineerCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ImprovisedDefensePower>()];

    public ImprovisedDefense() : base(2, CardType.Skill, CardRarity.Uncommon, ConstructTargetTypes.AnyConstructIOwn)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Safety check :)
        if (cardPlay.Target?.Monster is ConstructModel)
        {
            await PowerCmd.Apply<ImprovisedDefensePower>(choiceContext, cardPlay.Target, 1m, base.Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        base.EnergyCost.UpgradeBy(-1);
    }

}
