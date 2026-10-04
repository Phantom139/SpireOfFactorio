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

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class GunTurret_Upgraded : CustomConstructModel
{
    public override int MinInitialHp => 26;
    public override int MaxInitialHp => 26;

    public static int spawnHP = 26;

    public static int damage = 8;

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;
    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/gun_turret.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-GUN_TURRET_UPGRADED.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-GUN_TURRET_UPGRADED.description");
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
            await DamageCmd.Attack(outDamage).FromMonster(Creature.Monster)
                .TargetingRandomOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
            await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);
        }
        return;
    }

}
