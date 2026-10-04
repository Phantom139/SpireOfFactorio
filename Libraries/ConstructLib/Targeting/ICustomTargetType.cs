using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting;

public interface ICustomTargetType
{
    bool IsSingleTarget { get; }

    bool IsValidTargetPreview(Creature target);
    bool IsValidTarget(CardModel card, Creature target);
    bool IsValidTarget(PotionModel potion, Creature target);
}