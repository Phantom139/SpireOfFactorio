using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Pets;

public class AnyConstructOrOwnerTargetType : ICustomTargetType
{
    public bool IsSingleTarget => true;

    public bool IsValidTargetPreview(Creature target)
    {
        return IsValidTarget(target) && (LocalContext.IsMe(target) || LocalContext.IsMe(target.PetOwner));
    }

    public bool IsValidTarget(CardModel card, Creature target)
    {
        return IsValidTarget(target) && (target.Player == card.Owner || target.PetOwner == card.Owner);
    }

    public bool IsValidTarget(PotionModel potion, Creature target)
    {
        return IsValidTarget(target) && (target.Player == potion.Owner || target.PetOwner == potion.Owner);
    }

    private static bool IsValidTarget(Creature target)
    {
        return target.IsAlive && (target.IsPlayer || target is { Side: CombatSide.Player, IsPet: true, Monster: ConstructModel });
    }
}
