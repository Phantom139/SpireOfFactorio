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

public sealed class FlamethrowerTurret : EngineerCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.FlamethrowerTurret_Upgraded.GetHoverTip(), HoverTipFactory.FromPower<ScorchPower>()];
            }
            else
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.FlamethrowerTurret.GetHoverTip(), HoverTipFactory.FromPower<ScorchPower>()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public FlamethrowerTurret() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 2);
    }

    protected override void OnUpgrade()
    {
        //CustomResources<Scrap>.SetCanonicalCost(this, 2);
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[Cards:FlamethrowerTurret] OnUpgrade() Could not resolve the CustomResource<Scrap> object.");
            return;
        }
        scrapCost.UpgradeCostBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<FlamethrowerTurret_Upgraded>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(FlamethrowerTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Below)
                );
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.FlamethrowerTurret>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Characters.Constructs.FlamethrowerTurret.spawnHP, Source: this, Position: ConstructPosition.Below)
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
