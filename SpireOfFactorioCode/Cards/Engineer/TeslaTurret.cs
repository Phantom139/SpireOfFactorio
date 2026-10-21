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

public sealed class TeslaTurret : EngineerCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.TeslaTurret_Upgraded.GetHoverTip(), HoverTipFactory.FromPower<ShockPower>()];
            }
            else
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.TeslaTurret.GetHoverTip(), HoverTipFactory.FromPower<ShockPower>()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public TeslaTurret() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 3);
    }

    protected override void OnUpgrade()
    {
        //CustomResources<Scrap>.SetCanonicalCost(this, 3);
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[Cards:TeslaTurret] OnUpgrade() Could not resolve the CustomResource<Scrap> object.");
            return;
        }
        scrapCost.UpgradeCostBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<TeslaTurret_Upgraded>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(TeslaTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Above)
                );
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.TeslaTurret>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Characters.Constructs.TeslaTurret.spawnHP, Source: this, Position: ConstructPosition.Above)
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
