using MegaCrit.Sts2.Core.Entities.Creatures;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Pets;

public class VoidTargetType : CustomTargetType
{
    public override bool IsSingleTarget => true;
    protected override bool IsValidTarget(Creature target)
    {
        return false;
    }
}
