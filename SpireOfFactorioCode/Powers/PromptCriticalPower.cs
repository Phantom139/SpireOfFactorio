using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class PromptCriticalPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task OnConstructPerformedTask(PlayerChoiceContext choiceContext, Creature construct, Player owner)
    {
        DamageVar OutDamage = new DamageVar((decimal)base.Amount, ValueProp.Unpowered);

        await Cmd.CustomScaledWait(0.2f, 0.4f);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
        }

        await Cmd.CustomScaledWait(0.2f, 0.4f);
        await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, OutDamage, base.Owner);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(base.Owner));
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        await CreatureCmd.Kill(base.Owner);
    }
}
