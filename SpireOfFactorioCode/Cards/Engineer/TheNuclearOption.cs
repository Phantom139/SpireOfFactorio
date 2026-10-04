using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class TheNuclearOption : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(50m, ValueProp.Unblockable), new PowerVar<ShockPower>("Shock", 100m), new PowerVar<ScorchPower>("Scorch", 25m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>(), HoverTipFactory.FromPower<ScorchPower>()];

    //
    // Summary:
    //     The only difference between the starting Strike cards are portrait, attack vfx,
    //     and color.
    public TheNuclearOption()
        : base(4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int playerCountIncrease = base.Owner.Creature.CombatState.Players.Count - 1;
        int shockStacks = (playerCountIncrease > 0 ? 10 * playerCountIncrease : 0) + 100;

        AttackCommand attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
        .WithHitFx("vfx/vfx_attack_blunt");
        Extensions.AttackCommandExtensions.Unblockable(attack);
        await attack.Execute(choiceContext);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ShockPower>(choiceContext, hittableEnemy, shockStacks, base.Owner.Creature, this);
            await PowerCmd.Apply<ScorchPower>(choiceContext, hittableEnemy, base.DynamicVars["Scorch"].BaseValue, base.Owner.Creature, this);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(hittableEnemy));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(15m);
    }
}
