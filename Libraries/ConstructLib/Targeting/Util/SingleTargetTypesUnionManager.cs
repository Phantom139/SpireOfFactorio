using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using System.Collections.Immutable;

namespace SpireOfFactorio.Libraries.ConstructLib.Targeting.Util;

public static class SingleTargetTypesUnionManager
{
    private static readonly Dictionary<ImmutableHashSet<TargetType>, TargetType> Registry = new()
    {
        [ImmutableHashSet<TargetType>.Empty] = ConstructTargetTypes.Void
    };

    private static readonly Dictionary<TargetType, ImmutableHashSet<TargetType>> Components = new()
    {
        [ConstructTargetTypes.Void] = ImmutableHashSet<TargetType>.Empty
    };

    private static (ImmutableHashSet<TargetType>, IEnumerable<ICustomTargetType>) FilterSingleAndSelect(
        IEnumerable<TargetType> source)
    {
        var setBuilder = ImmutableHashSet.CreateBuilder<TargetType>();
        var customTargetTypes = new List<ICustomTargetType>();

        foreach (var targetType in Breakdown(source))
        {
            if (!CustomTargetTypeManager.TryGetCustomTargetType(targetType, out var customTargetType))
            {
                Log.Warn($"TargetType '{targetType}' is not a registered custom target type. Skipping.");
                continue;
            }

            if (!customTargetType.IsSingleTarget)
                continue;


            setBuilder.Add(targetType);
            customTargetTypes.Add(customTargetType);
        }

        return (setBuilder.ToImmutable(), customTargetTypes);
    }

    private static HashSet<TargetType> Breakdown(IEnumerable<TargetType> source)
    {
        var set = new HashSet<TargetType>();
        foreach (var targetType in source)
            if (Components.TryGetValue(targetType, out var components))
                foreach (var component in components)
                    set.Add(component);
            else
                set.Add(targetType);

        return set;
    }

    public static TargetType Get(IEnumerable<TargetType> targetTypes)
    {
        var types = targetTypes as TargetType[] ?? targetTypes.ToArray();
        var (set, customTargetTypes) = FilterSingleAndSelect(types);
        if (set.IsEmpty) return ConstructTargetTypes.Void;
        if (set.Count == 1)
            return set.Single();
        if (Registry.TryGetValue(set, out var registeredType))
            return registeredType;

        var union = new UnionTargetType(customTargetTypes.ToArray());
        var unionName = string.Join("|", types.Select(t => t.ToString("X")));
        var unionEnum = CustomTargetTypeManager.Register(union, "SpireOfFactorio-UnionTargetType", unionName);
        Registry[set] = unionEnum;
        Components[unionEnum] = set;
        return unionEnum;
    }

    public static TargetType GetWithBase(IEnumerable<TargetType> targetTypes, TargetType baseType)
    {
        var type = Get(targetTypes);
        return type == ConstructTargetTypes.Void ? baseType : type;
    }
}
