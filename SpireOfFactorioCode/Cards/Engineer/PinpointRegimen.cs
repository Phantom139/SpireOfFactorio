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

public sealed class PinpointRegimen : EngineerCard
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (base.IsUpgraded)
            {
                return [LaserTurret_Upgraded.GetHoverTip()];
            }
            else
            {
                return [Characters.Constructs.LaserTurret.GetHoverTip()];
            }
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];

    public PinpointRegimen() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
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
                await EngineerCmd.AddConstruct<LaserTurret_Upgraded>(choiceContext, p, cardPlay,
                    new ConstructCreationArgs(LaserTurret_Upgraded.spawnHP, Source: this, Position: ConstructPosition.Front)
                    );
            }

        }
        else
        {
            foreach (Player p in Owner.Creature.CombatState.Players)
            {
                await EngineerCmd.AddConstruct<Characters.Constructs.LaserTurret>(choiceContext, p, cardPlay,
                    new ConstructCreationArgs(Characters.Constructs.LaserTurret.spawnHP, Source: this, Position: ConstructPosition.Front)
                    );
            }
        }
    }


}
