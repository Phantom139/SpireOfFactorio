using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class DefensivePerimeter : EngineerCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.QualityGunTurret_Upgraded.GetHoverTip()];
            }
            else
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.QualityGunTurret.GetHoverTip()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public DefensivePerimeter() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<QualityGunTurret_Upgraded>(choiceContext, Owner, cardPlay, new ConstructCreationArgs(QualityGunTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Front));
            await EngineerCmd.AddConstruct<QualityGunTurret_Upgraded>(choiceContext, Owner, cardPlay, new ConstructCreationArgs(QualityGunTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.FrontAbove));
            await EngineerCmd.AddConstruct<QualityGunTurret_Upgraded>(choiceContext, Owner, cardPlay, new ConstructCreationArgs(QualityGunTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.FrontBelow));
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.QualityGunTurret>(choiceContext, Owner, cardPlay, new ConstructCreationArgs(QualityGunTurret.spawnHP, Source: this, Position: ConstructPosition.Front));
            await EngineerCmd.AddConstruct<Characters.Constructs.QualityGunTurret>(choiceContext, Owner, cardPlay, new ConstructCreationArgs(QualityGunTurret.spawnHP, Source: this, Position: ConstructPosition.FrontAbove));
            await EngineerCmd.AddConstruct<Characters.Constructs.QualityGunTurret>(choiceContext, Owner, cardPlay, new ConstructCreationArgs(QualityGunTurret.spawnHP, Source: this, Position: ConstructPosition.FrontBelow));
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }


}
