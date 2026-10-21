using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;
using SpireOfFactorio.SpireOfFactorioCode.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class LaserTurret : EngineerCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.LaserTurret_Upgraded.GetHoverTip(), HoverTipFactory.FromPower<ScorchPower>()];
            }
            else
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.LaserTurret.GetHoverTip(), HoverTipFactory.FromPower<ScorchPower>()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public LaserTurret() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 1);
    }

    protected override void OnUpgrade()
    {
        //CustomResources<Scrap>.SetCanonicalCost(this, 2);
        base.EnergyCost.UpgradeBy(-1);
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[Cards:LaserTurret] OnUpgrade() Could not resolve the CustomResource<Scrap> object.");
            return;
        }
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<LaserTurret_Upgraded>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(LaserTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Front)
                );
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.LaserTurret>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Characters.Constructs.LaserTurret.spawnHP, Source: this, Position: ConstructPosition.Front)
                );
        }
        /**
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        **/
    }


}
