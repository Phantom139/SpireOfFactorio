using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ConstructionRobotPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            if (base.Owner.IsPlayer)
            {
                var pets = base.Owner.Pets;
                foreach (Creature pet in pets)
                {
                    if (pet.Monster is ConstructModel)
                    {
                        await CreatureCmd.Heal(pet, base.Amount, true);
                    }
                }
            }
        }
    }
}
