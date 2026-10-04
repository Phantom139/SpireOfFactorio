using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Util;

public class UnionTargetType(params ICustomTargetType[] targetTypes) : ICustomTargetType
{
    public bool IsSingleTarget => targetTypes.Any(targetType => targetType.IsSingleTarget);

    public bool IsValidTargetPreview(Creature target)
    {
        return targetTypes.Any(targetType => targetType.IsValidTargetPreview(target));
    }

    public bool IsValidTarget(CardModel card, Creature target)
    {
        return targetTypes.Any(targetType => targetType.IsValidTarget(card, target));
    }

    public bool IsValidTarget(PotionModel potion, Creature target)
    {
        return targetTypes.Any(targetType => targetType.IsValidTarget(potion, target));
    }
}
