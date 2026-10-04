using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting;

public abstract class CustomTargetType : ICustomTargetType
{
    public abstract bool IsSingleTarget { get; }

    public bool IsValidTargetPreview(Creature target)
    {
        return IsValidTarget(target);
    }

    public virtual bool IsValidTarget(CardModel card, Creature target)
    {
        return IsValidTarget(target);
    }

    public virtual bool IsValidTarget(PotionModel potion, Creature target)
    {
        return IsValidTarget(target);
    }

    protected abstract bool IsValidTarget(Creature target);

}
