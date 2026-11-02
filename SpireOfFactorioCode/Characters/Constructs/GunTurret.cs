using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireOfFactorio.Libraries.ConstructLib.BaseLib;
using SpireOfFactorio.Libraries.ConstructLib.Commands;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class GunTurret : CustomConstructModel
{
    public override int MinInitialHp => 20;
    public override int MaxInitialHp => 20;

    public static int spawnHP = 20;

    public static int damage = 5;

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;

    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/gun_turret.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-GUN_TURRET.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-GUN_TURRET.description");
        description.Add("Damage", damage);

        return new HoverTip(title, description);
    }

    public override async Task OnConstructed(PlayerChoiceContext choiceContext, Creature construct, Player owner, ConstructCreationArgs args)
    {
        var self = Creature;
        if (args.MaxHp is decimal maxHp)
        {
            await CreatureCmd.SetMaxAndCurrentHp(self, maxHp);
        }
        //await PowerCmd.Apply<ImprovisedDefensePower>(choiceContext, self, 1, owner.Creature, null);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (IsDisabled())
        {
            return;
        }
        if (side == CombatSide.Player)
        {
            // Verify the presence of enemies
            if (base.CombatState.HittableEnemies.Count == 0)
            {
                return;
            }
            decimal outDamage = decimal.Ceiling(damage * OverclockPercent());

            List<Creature> tList = await ConstructCmd.SelectTarget(Creature, ConstructTargetSelectionType.RandomEnemy);
            Creature target = tList.First();

            if (target == null || target.IsDead)
            {
                // If the target died prior to this call, give it one re-attempt
                List<Creature> tList2 = await ConstructCmd.SelectTarget(Creature, ConstructTargetSelectionType.RandomEnemy);
                Creature target2 = tList2.First();
                if (target2 == null || target2.IsDead)
                {
                    // If still null or dead, abort.
                    return;
                }
                target = target2;
            }

            // Check for targeting routine
            outDamage += Decimal.Ceiling(((target.GetPowerAmount<TargetingRoutinePower>() / 100) * (outDamage)));

            await DamageCmd.Attack(outDamage).FromMonster(Creature.Monster)
                .Targeting(target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
            await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);
        }
        return;
    }
}
