using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireOfFactorio.Libraries.ConstructLib.Layout;

public static class ConstructLayoutManager
{
    private static readonly List<(IConstructLayout layout, int priority, int order)> LayoutsWithPriority = [];
    private static int _counter;

    public static IEnumerable<IConstructLayout> Layouts => LayoutsWithPriority.Select(x => x.layout);

    static ConstructLayoutManager()
    {
        Register(new DefaultConstructLayout());
    }

    public static void Register(IConstructLayout layout, int priorty = 0)
    {
        LayoutsWithPriority.Add((layout, priorty, _counter++));

        LayoutsWithPriority.Sort((a, b) =>
        {
            var pComp = b.priority.CompareTo(a.priority);
            return pComp != 0 ? pComp : b.order.CompareTo(a.order);
        });
    }

    public static IEnumerable<ConstructNodePosition> CalculateLayout(NCombatRoom room)
    {
        var context = new ConstructLayoutContext(room);
        foreach (var layout in Layouts)
        {
            if (layout.IsActive)
            {
                layout.ApplyLayout(context);
            }
        }
        return context.Positions.Select(entry => new ConstructNodePosition(entry.Key, entry.Value)).ToList();
    }

    public static IReadOnlyList<ConstructNodePosition> GetCurrentConstructPositions(NCombatRoom room)
    {
        var constructs = room.CreatureNodes.Where(n => n.IsConstructNode());
        return constructs.Select(c => new ConstructNodePosition(c, c.Position)).ToList();
    }
}
