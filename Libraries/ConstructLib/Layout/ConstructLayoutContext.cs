using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireOfFactorio.Libraries.ConstructLib.Layout;

public class ConstructLayoutContext
{
    public NCombatRoom Room { get; }

    public IReadOnlyList<NCreature> AllConstructs { get; }

    public Dictionary<NCreature, Vector2> Positions { get; }

    public IEnumerable<NCreature> UnhandledConstructs => AllConstructs.Where(c => !Positions.ContainsKey(c));

    public ConstructLayoutContext(NCombatRoom room)
    {
        Room = room;
        AllConstructs = room.CreatureNodes.Where(n => n.IsConstructNode()).ToList();
        Positions = new Dictionary<NCreature, Vector2>();
    }
}
