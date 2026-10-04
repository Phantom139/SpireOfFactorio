using MegaCrit.Sts2.Core.Nodes.Combat;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.Libraries.ConstructLib.Layout;

public static class NCreatureExtension
{
    public static bool IsConstructNode(this NCreature node)
    {
        return node.Entity is { Monster: ConstructModel, IsAlive: true, PetOwner: not null };
    }
}