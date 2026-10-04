using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Extensions;

public static class AttackCommandExtensions
{
    public static void Unblockable(AttackCommand attackCommand)
    {
        attackCommand.DamageProps |= ValueProp.Unblockable;
    }
}
