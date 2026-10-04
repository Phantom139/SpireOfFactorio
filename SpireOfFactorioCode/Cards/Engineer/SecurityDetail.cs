using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class SecurityDetail : EngineerCard
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [GunTurret_Upgraded.GetHoverTip()];
            }
            else
            {
                return [Characters.Constructs.GunTurret.GetHoverTip()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public SecurityDetail() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 3);
    }

    protected override void OnUpgrade()
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.IsUpgraded)
        {
            foreach (Player p in Owner.Creature.CombatState.Players)
            {
                await EngineerCmd.AddConstruct<GunTurret_Upgraded>(choiceContext, p, cardPlay,
                    new ConstructCreationArgs(GunTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.FrontAbove)
                    );
            }

        }
        else
        {
            foreach (Player p in Owner.Creature.CombatState.Players)
            {
                await EngineerCmd.AddConstruct<Characters.Constructs.GunTurret>(choiceContext, p, cardPlay,
                    new ConstructCreationArgs(Characters.Constructs.GunTurret.spawnHP, Source: this, Position: ConstructPosition.FrontAbove)
                    );
            }
        }
    }


}
