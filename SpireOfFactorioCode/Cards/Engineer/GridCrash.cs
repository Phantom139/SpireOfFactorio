using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class GridCrash : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OverclockPower>("Overclock", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverclockPower>()];

    public GridCrash()
        : base(3, CardType.Skill, CardRarity.Rare, ConstructTargetTypes.AllAlliedConstructs)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pets = base.Owner.Creature.Pets;
        foreach (Creature pet in pets)
        {
            if (pet.Monster is ConstructModel)
            {
                await PowerCmd.Apply<OverclockPower>(choiceContext, pet, DynamicVars["Overclock"].BaseValue, base.Owner.Creature, cardPlay.Card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Overclock"].UpgradeValueBy(2m);
    }
}
