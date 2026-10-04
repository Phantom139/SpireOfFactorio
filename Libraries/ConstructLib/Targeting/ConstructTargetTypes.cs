using MegaCrit.Sts2.Core.Entities.Cards;
using SpireOfFactorio.Libraries.ConstructLib.Targeting.Pets;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting;

public static class ConstructTargetTypes
{
    public static readonly TargetType AnyConstructIOwn = Register(new AnyConstructIOwnTargetType(), nameof(AnyConstructIOwn));
    public static readonly TargetType AllConstructsIOwn = Register(new AllConstructsIOwnTargetType(), nameof(AllConstructsIOwn));
    public static readonly TargetType AnyAlliedConstruct = Register(new AnyAlliedConstructTargetType(), nameof(AnyAlliedConstruct));
    public static readonly TargetType AllAlliedConstructs = Register(new AllAlliedConstructsTargetType(), nameof(AllAlliedConstructs));
    public static readonly TargetType AnyCreature = Register(new AnyCreatureTargetType(), nameof(AnyCreature));
    public static readonly TargetType AllCreatures = Register(new AllCreaturesTargetType(), nameof(AllCreatures));
    public static readonly TargetType AnyConstructOrOwner = Register(new AnyConstructOrOwnerTargetType(), nameof(AnyConstructOrOwner));
    public static readonly TargetType Void = Register(new VoidTargetType(), nameof(Void));

    private static TargetType Register(ICustomTargetType type, string name)
    {
        return CustomTargetTypeManager.Register(type, nameof(SpireOfFactorio), name);
    }
}
