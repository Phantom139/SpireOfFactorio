using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.BaseLib;
using SpireOfFactorio.Libraries.ConstructLib.Commands;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class TeslaTurret : CustomConstructModel
{
    public override int MinInitialHp => 20;
    public override int MaxInitialHp => 20;

    public static int spawnHP = 20;

    public static int damage = 15;
    public static int shock = 25;

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;
    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/tesla_turret.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-TESLA_TURRET.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-TESLA_TURRET.description");
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
            List<Creature> tList = await ConstructCmd.SelectTarget(Creature, ConstructTargetSelectionType.RandomEnemy);
            if (tList == null)
            {
                return;
            }
            Creature target = tList.First();

            if (target == null || target.IsDead)
            {
                // If the target died prior to this call, give it one re-attempt
                List<Creature> tList2 = await ConstructCmd.SelectTarget(Creature, ConstructTargetSelectionType.RandomEnemy);
                if (tList2 == null)
                {
                    return;
                }
                Creature target2 = tList2.First();
                if (target2 == null || target2.IsDead)
                {
                    // If still null or dead, abort.
                    return;
                }
                target = target2;
            }

            ConstructEffectContext context = new ConstructEffectContext(Creature)
            {
                BaseDamage = damage,
                Props = ValueProp.Move,

                Powers = new Dictionary<PowerModel, int>
                {
                    { ModelDb.Power<ShockPower>(), shock }
                }
            };

            // Fire Hooks
            await ConstructHooks.ModifyConstructDamageAdditive(context);
            await ConstructHooks.ModifyConstructDamageMultiplicative(context);
            await ConstructHooks.ModifyConstructPowerList(context);

            await DamageCmd.Attack(context.FinalDamage).FromMonster(Creature.Monster)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(choiceContext);
            // IF the target is still alive at this point, throw on the debuff
            if (target != null)
            {
                if (target.IsAlive)
                {
                    foreach (var power in context.Powers)
                    {
                        await PowerCmd.Apply(choiceContext, power.Key, target, power.Value, Creature, null);
                    }
                }
            }
            await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);


        }
        return;
    }
}
