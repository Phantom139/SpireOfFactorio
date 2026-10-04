using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Fortification : EngineerCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.Wall_Upgraded.GetHoverTip()];
            }
            else
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.Wall.GetHoverTip()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public Fortification() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 1);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<Wall_Upgraded>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Wall_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Front)
                );
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.Wall>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Characters.Constructs.Wall.spawnHP, Source: this, Position: ConstructPosition.Front)
                );
        }
    }


}
