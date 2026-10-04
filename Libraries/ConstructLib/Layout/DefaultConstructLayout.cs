using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.Libraries.ConstructLib.Layout;

public class DefaultConstructLayout : IConstructLayout
{
    public static readonly Vector2 ConstructSize = new(150f, 150f);
    public bool IsActive => true;

    public void ApplyLayout(ConstructLayoutContext context)
    {
        var unhandled = context.UnhandledConstructs.ToList();
        if (unhandled.Count == 0)
        {
            return;
        }
        var calculatedPositions = CalculateConstructPositions(context.Room, unhandled);
        foreach (var pos in calculatedPositions)
        {
            context.Positions[pos.Node] = pos.Position;
        }
    }

    public static IReadOnlyList<Vector2> GenerateGridPoints(ConstructPosition position, int count)
    {
        if (count <= 0)
        {
            return [];
        }
        IReadOnlyList<Vector2> result;
        if (position == ConstructPosition.Above)
        {
            var rows = count / 2;
            result = Enumerable.Range(1, count).Select(i => new Vector2(i == count && count % 2 == 1 ? 0f : -0.5f + i % 2, -(i - 1) / 2)).ToList();
        }
        else if (position == ConstructPosition.Below)
        {
            var rows = count / 2;
            result = Enumerable.Range(1, count).Select(i => new Vector2(i == count && count % 2 == 1 ? 0f : -0.5f + i % 2, (i - 1) / 2)).ToList();
        }
        else
        {
            var isFront = position is ConstructPosition.Front or ConstructPosition.FrontAbove or ConstructPosition.FrontBelow;
            var isUpper = position is ConstructPosition.FrontAbove or ConstructPosition.BehindAbove;
            var turningPoint = isFront ? 3f : 1.5f;
            var slope = isFront ? 0.75f : 0.25f;

            var fullWidth = (count + 1) * 0.5f;
            var width = fullWidth <= turningPoint ? fullWidth : turningPoint + slope * MathF.Log((fullWidth - turningPoint) / slope + 1);
            var last = 0f;
            var first = isFront ? width : -width;

            result = Enumerable.Range(2, count).Select(i => new Vector2(float.Lerp(first, last, (float)i / (count + 1)), isUpper ? -i % 2 : i % 2)).ToList();
        }
        return result;
    }

    public static IReadOnlyList<OwnerWithConstructNodes> GetConstructOwnerNodePairs(NCombatRoom room, IEnumerable<NCreature> unhandledConstructs)
    {
        var grouped = unhandledConstructs.GroupBy(c => c.Entity.PetOwner!);
        var result = grouped.Select(g =>
        {
            var player = g.Key;
            var creatureToNode = g.ToDictionary(c => c.Entity, c => c);
            var pets = player.PlayerCombatState!.Pets;
            var orderedConstructs = pets.Select(creatureToNode.GetValueOrDefault).OfType<NCreature>().ToList();
            return new OwnerWithConstructNodes(room.GetCreatureNode(g.Key.Creature)!, orderedConstructs);
        }).ToList();
        return result;
    }

    public static Vector2 CalculateBaseOffset(ConstructPosition constructPosition, ILookup<ConstructPosition, NCreature> lookup)
    {
        switch (constructPosition)
        {
            case ConstructPosition.Front:
                if (lookup.Contains(ConstructPosition.FrontAbove) || lookup.Contains(ConstructPosition.FrontBelow) && lookup[ConstructPosition.Front].Count() >= 2)
                {
                    return new Vector2(250f, 50f);
                }
                return new Vector2(200f, 0f);
            case ConstructPosition.Above:
                return new Vector2(0, -75f);
            case ConstructPosition.Below:
                return new Vector2(0, 75f);
            case ConstructPosition.Behind:
                if (lookup.Contains(ConstructPosition.BehindAbove) || lookup.Contains(ConstructPosition.BehindBelow) && lookup[ConstructPosition.Behind].Count() >= 2)
                {
                    return new Vector2(-250f, 50f);
                }
                return new Vector2(-200f, 0f);
            case ConstructPosition.FrontAbove:
                if (lookup[ConstructPosition.Above].Count() <= 2)
                {
                    return new Vector2(100f + 75f * lookup[ConstructPosition.Above].Count(), -75f);
                }
                return new Vector2(200f, -200f);
            case ConstructPosition.FrontBelow:
                if (lookup[ConstructPosition.Below].Count() <= 2)
                {
                    return new Vector2(100f + 75f * lookup[ConstructPosition.Below].Count(), 75f);
                }
                return new Vector2(200f, 200f);
            case ConstructPosition.BehindAbove:
                if (lookup[ConstructPosition.Above].Count() <= 2)
                {
                    return new Vector2(-100f - 75f * lookup[ConstructPosition.Above].Count(), -75f);
                }
                return new Vector2(-200f, -200f);
            case ConstructPosition.BehindBelow:
                if (lookup[ConstructPosition.Below].Count() <= 2)
                {
                    return new Vector2(-100f - 75f * lookup[ConstructPosition.Below].Count(), 75f);
                }
                return new Vector2(-200f, 75f);
            default:
                return Vector2.Zero;
        }
    }

    public static IReadOnlyList<ConstructNodePosition> CalculateConstructPositions(NCombatRoom room, IEnumerable<NCreature> unhandledConstructs)
    {
        return GetConstructOwnerNodePairs(room, unhandledConstructs).SelectMany(pair =>
        {
            var (ownerNode, constructNodes) = pair;
            var grouped = constructNodes.ToLookup(c => ((ConstructModel)c.Entity.Monster!).Position);
            var nodePositions = grouped.SelectMany(g =>
            {
                var constructPosition = g.Key;
                var offset = CalculateBaseOffset(constructPosition, grouped);
                var positions = GenerateGridPoints(constructPosition, g.Count()).Select(v => v * ConstructSize + offset + ownerNode.Position);
                return g.Zip(positions, (node, position) => new ConstructNodePosition(node, position));
            });
            return nodePositions;
        }).ToList();
    }
}