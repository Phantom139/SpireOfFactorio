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

public sealed class Overheat : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Unpowered), new PowerVar<ScorchPower>("ScorchEnemies", 20m), new PowerVar<ScorchPower>("ScorchSelf", 10m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ScorchPower>()];

    public Overheat()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AttackCommand attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
        .WithHitFx("vfx/vfx_attack_blunt");
        Extensions.AttackCommandExtensions.Unblockable(attack);
        await attack.Execute(choiceContext);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ScorchPower>(choiceContext, hittableEnemy, base.DynamicVars["ScorchEnemies"].BaseValue, base.Owner.Creature, this);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
        }
        await PowerCmd.Apply<ScorchPower>(choiceContext, base.Owner.Creature, base.DynamicVars["ScorchSelf"].BaseValue, base.Owner.Creature, this);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(base.Owner.Creature));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["ScorchEnemies"].UpgradeValueBy(5m);
        DynamicVars["ScorchSelf"].UpgradeValueBy(-3m);
    }
}
