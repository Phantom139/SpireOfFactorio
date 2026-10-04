using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SpireOfFactorio.Libraries.ConstructLib.Layout;

public interface IConstructLayout
{
    bool IsActive { get; }
    void ApplyLayout(ConstructLayoutContext context);
}

public readonly record struct OwnerWithConstructNodes(NCreature Owner, IReadOnlyList<NCreature> Constructs);
public readonly record struct ConstructNodePosition(NCreature Node, Vector2 Position);