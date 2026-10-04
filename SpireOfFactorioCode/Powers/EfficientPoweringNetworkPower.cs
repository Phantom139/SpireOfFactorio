using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.SpireOfFactorioCode.Characters;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class EfficientPoweringNetworkPower : SpireOfFactorioPower
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
        if (side == Owner.Side)
        {
            // 
            if (EngineerCombatState.PendingOverload[Owner.Player.PlayerCombatState] > 0)
            {
                // If we're pending more overload, queue a short wait to make sure it gets applied first.
                await Cmd.Wait(0.05f);
            }

            if (Owner.HasPower<OverloadPower>())
            {
                await PowerCmd.ModifyAmount(choiceContext, Owner.GetPower<OverloadPower>(), -Amount, null, null);
                Flash();
            }
        }
    }

}
