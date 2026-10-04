using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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

public sealed class TeslaTurret_Upgraded : CustomConstructModel
{
    public override int MinInitialHp => 30;
    public override int MaxInitialHp => 30;

    public static int spawnHP = 30;

    public static int damage = 20;
    public static int shock = 35;

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;
    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/tesla_turret.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-TESLA_TURRET_UPGRADED.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-TESLA_TURRET_UPGRADED.description");
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
            // Verify the presence of enemies
            if (base.CombatState.HittableEnemies.Count == 0)
            {
                return;
            }
            decimal outDamage = decimal.Ceiling(damage * OverclockPercent());
            decimal outShock = decimal.Ceiling(shock * OverclockPercent());

            AttackCommand damageCommand = await DamageCmd.Attack(outDamage).FromMonster(Creature.Monster)
                .TargetingRandomOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_lightning")
                .Execute(choiceContext);
            Creature? hitCreature = damageCommand.Results
                .SelectMany(r => r)
                .Select(r => r.Receiver)
                .FirstOrDefault();

            if (hitCreature == null)
            {
                return;
            }
            await PowerCmd.Apply<ShockPower>(choiceContext, hitCreature, outShock, base.Creature, null);
            await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);
        }
        return;
    }
}
