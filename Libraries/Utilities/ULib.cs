using System.Reflection;

namespace SpireOfFactorio.Libraries.Utilities;

public static class ULib
{
    public static void EnsureType(Type type, Type expectedBaseType, string args)
    {
        ArgumentNullException.ThrowIfNull(type, args);
        ArgumentNullException.ThrowIfNull(expectedBaseType, args);

        if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters || type.GetConstructor(Type.EmptyTypes) == null || !expectedBaseType.IsAssignableFrom(type))
        {
            throw new ArgumentException($"Type {type.FullName} must be a closed concrete subtype of {expectedBaseType.FullName} with a public parameterless constructor.", args);
        }
    }

    public static T GetStaticField<T>(Type owner, string name) where T : class
    {
        var field = owner.GetField(name, BindingFlags.Static | BindingFlags.NonPublic) ?? throw new MissingFieldException(owner.FullName, name);

        return (T)(field.GetValue(null)) ?? throw new InvalidOperationException($"Static field {owner.FullName}.{name} is null");
    }

    public static void SetStaticField(Type owner, string name, object? value)
    {
        var field = owner.GetField(name, BindingFlags.Static | BindingFlags.NonPublic) ?? throw new MissingFieldException(owner.FullName, name);
        field.SetValue(null, value);
    }
}
