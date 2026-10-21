using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class GunTurret : EngineerCard, ITranscendenceCard
{
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<DefensivePerimeter>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.GunTurret_Upgraded.GetHoverTip()];
            }
            else
            {
                return [SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs.GunTurret.GetHoverTip()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public GunTurret() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 1);
    }

    protected override void OnUpgrade()
    {
        //CustomResources<Scrap>.SetCanonicalCost(this, 1);
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[Cards:GunTurret] OnUpgrade() Could not resolve the CustomResource<Scrap> object.");
            return;
        }
        scrapCost.UpgradeCostBy(-1);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            await EngineerCmd.AddConstruct<GunTurret_Upgraded>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(GunTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.FrontAbove)
                );
        }
        else
        {
            await EngineerCmd.AddConstruct<Characters.Constructs.GunTurret>(choiceContext, Owner, cardPlay,
                new ConstructCreationArgs(Characters.Constructs.GunTurret.spawnHP, Source: this, Position: ConstructPosition.FrontAbove)
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
