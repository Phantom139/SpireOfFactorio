using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Hellfire : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ScorchPower>("Scorch", 10m), new DynamicVar("HellfireVulnerableIncrease", 20m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ScorchPower>()];

    //
    // Summary:
    //     The only difference between the starting Strike cards are portrait, attack vfx,
    //     and color.
    public Hellfire()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        decimal totalScorch = DynamicVars["Scorch"].BaseValue * (1 + ((cardPlay.Target.GetPowerAmount<VulnerablePower>() * DynamicVars["HellfireVulnerableIncrease"].BaseValue) / 100));
        await PowerCmd.Apply<ScorchPower>(choiceContext, cardPlay.Target, totalScorch, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Scorch"].UpgradeValueBy(4m);
        DynamicVars["HellfireVulnerableIncrease"].UpgradeValueBy(13m);
    }
}
