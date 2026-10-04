using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Util;

public class DifferenceTargetType(ICustomTargetType original, ICustomTargetType exclude, bool? overrideIsSingleTarget = null) : ICustomTargetType
{
    public bool IsSingleTarget =>
        overrideIsSingleTarget ?? original.IsSingleTarget || exclude.IsSingleTarget;


    public bool IsValidTargetPreview(Creature target)
    {
        return original.IsValidTargetPreview(target) && !exclude.IsValidTargetPreview(target);
    }

    public bool IsValidTarget(CardModel card, Creature target)
    {
        return original.IsValidTarget(card, target) && !exclude.IsValidTarget(card, target);
    }

    public bool IsValidTarget(PotionModel potion, Creature target)
    {
        return original.IsValidTarget(potion, target) && !exclude.IsValidTarget(potion, target);
    }
}
