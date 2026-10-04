using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class CoveringFirePower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(1m, ValueProp.Move)];

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker.Monster is ConstructModel construct)
        {
            if (construct.Creature.PetOwner.Creature.IsDead)
            {
                return;
            }
            decimal TotalDamage = (command.Results.SelectMany(hit => hit).Sum(result => result.TotalDamage));
            decimal BlockToPass = decimal.Ceiling(TotalDamage * ((decimal)base.Amount / 100));
            await CreatureCmd.GainBlock(construct.Creature.PetOwner.Creature, BlockToPass, ValueProp.Move, null);
        }
    }
}
