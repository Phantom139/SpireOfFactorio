using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Transmission : EngineerCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [Beacon_Upgraded.GetHoverTip()];
            }
            else
            {
                return [Beacon.GetHoverTip()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public Transmission() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 5);
    }

    protected override void OnUpgrade()
    {
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[Cards:Transmission] OnUpgrade() Could not resolve the CustomResource<Scrap> object.");
            return;
        }
        scrapCost.UpgradeCostBy(-1);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<Beacon_Upgraded>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Beacon_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Behind)
                );
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.Beacon>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Characters.Constructs.Beacon.spawnHP, Source: this, Position: ConstructPosition.Behind)
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
