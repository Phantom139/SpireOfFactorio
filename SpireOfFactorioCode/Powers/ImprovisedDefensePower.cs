using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public class ImprovisedDefensePower : SpireOfFactorioPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlayVfx => false;

    public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
    {
        if (target != base.Owner.PetOwner?.Creature)
        {
            return target;
        }

        if (base.Owner.IsDead)
        {
            return target;
        }

        if (!props.IsPoweredAttack())
        {
            return target;
        }

        return base.Owner;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        return creature.IsAlive;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return true;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (base.Owner.Side != CombatSide.Enemy)
        {
            if (side == CombatSide.Enemy)
            {
                await PowerCmd.Decrement(this);
            }
        }
    }

}
