using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Util;

internal class LambdaTargetType(
    bool isSingleTarget,
    Func<Creature, bool> generalPredicate,
    Func<CardModel, Creature, bool>? cardPredicate = null,
    Func<PotionModel, Creature, bool>? potionPredicate = null
    ) : CustomTargetType
{
    public override bool IsSingleTarget => isSingleTarget;

    protected override bool IsValidTarget(Creature target)
    {
        return generalPredicate(target);
    }

    public override bool IsValidTarget(CardModel card, Creature target)
    {
        return cardPredicate?.Invoke(card, target) ?? generalPredicate(target);
    }

    public override bool IsValidTarget(PotionModel potion, Creature target)
    {
        return potionPredicate?.Invoke(potion, target) ?? generalPredicate(target);
    }
}
