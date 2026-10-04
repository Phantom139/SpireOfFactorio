using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireOfFactorio.Libraries.ConstructLib.BaseLib;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class ArtilleryTurret_Upgraded : CustomConstructModel
{
    public override int MinInitialHp => 45;
    public override int MaxInitialHp => 45;

    public static int spawnHP = 45;

    public static int damage = 35;
    public static int shock = 20;

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;

    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/artillery_turret.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-ARTILLERY_TURRET_UPGRADED.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-ARTILLERY_TURRET_UPGRADED.description");
        description.Add("Damage", damage);
        description.Add("Shock", shock);

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
            // Find the strongest enemy
            Creature target = null;
            decimal highest = 0;
            foreach (Creature candidate in base.Creature.Monster.CombatState.Enemies)
            {
                if (candidate.IsDead)
                {
                    continue;
                }
                if (candidate.CurrentHp > highest)
                {
                    highest = candidate.CurrentHp;
                    target = candidate;
                }
            }
            if (target == null)
            {
                return;
            }
            // Apply Damage
            decimal outDamage = decimal.Ceiling(damage * OverclockPercent());
            decimal outShock = decimal.Ceiling(shock * OverclockPercent());

            await DamageCmd.Attack(outDamage).FromMonster(Creature.Monster)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
            // IF the target is still alive at this point, throw on the debuff
            if (target != null)
            {
                if (target.IsAlive)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(target));
                    await PowerCmd.Apply<ShockPower>(choiceContext, target, outShock, Creature, null);
                    await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);
                }
            }
        }
        return;
    }
}
