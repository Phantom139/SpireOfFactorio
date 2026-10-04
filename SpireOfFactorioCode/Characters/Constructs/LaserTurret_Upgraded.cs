using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireOfFactorio.Libraries.ConstructLib.BaseLib;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class LaserTurret_Upgraded : CustomConstructModel
{
    public override int MinInitialHp => 35;
    public override int MaxInitialHp => 35;

    public static int spawnHP = 35;

    public static int damage = 13;
    public static int scorch = 5;

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;
    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/laser_turret.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-LASER_TURRET_UPGRADED.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-LASER_TURRET_UPGRADED.description");
        description.Add("Damage", damage);
        description.Add("Scorch", scorch);

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
            // Find the weakest enemy
            Creature target = null;
            decimal lowest = 9999;
            foreach (Creature candidate in base.Creature.Monster.CombatState.Enemies)
            {
                if (candidate.IsDead)
                {
                    continue;
                }
                if (candidate.CurrentHp < lowest)
                {
                    lowest = candidate.CurrentHp;
                    target = candidate;
                }
            }
            if (target == null)
            {
                return;
            }
            // Apply Damage
            decimal outDamage = decimal.Ceiling(damage * OverclockPercent());
            decimal outScorch = decimal.Ceiling(scorch * OverclockPercent());

            await DamageCmd.Attack(outDamage).FromMonster(Creature.Monster)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
            // Apply Scorch
            await PowerCmd.Apply<ScorchPower>(choiceContext, target, outScorch, Creature, null);
            await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);
        }
        return;
    }
}
