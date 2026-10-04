using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireOfFactorio.Libraries.ConstructLib.Layout;

namespace SpireOfFactorio.Libraries.ConstructLib.Commands;

public static class ConstructAnimCmd
{
    private static Tween? _activeTween;

    public static async Task Rearrange(bool animated = true, float duration = 0.25f)
    {
        var room = NCombatRoom.Instance;
        if (room == null)
        {
            return;
        }
        var dst = ConstructLayoutManager.CalculateLayout(room);
        if (animated)
        {
            await AnimatedMove(dst, duration);
        }
        else
        {
            InstantMove(dst);
        }
    }

    public static void InstantMove(IEnumerable<ConstructNodePosition> nodePositions)
    {
        foreach (var (node, position) in nodePositions)
        {
            if (GodotObject.IsInstanceValid(node))
            {
                node.Position = position;
            }
        }
    }

    public static async Task AnimatedMove(IEnumerable<ConstructNodePosition> nodePositions, float duration = 0.25f)
    {
        var room = NCombatRoom.Instance;
        if (room == null)
        {
            return;
        }
        var validNodePositions = nodePositions.Where(nodePosition => GodotObject.IsInstanceValid(nodePosition.Node)).ToList();
        var activeTween = _activeTween;
        _activeTween = null;
        if (activeTween != null && activeTween.IsValid())
        {
            activeTween.EmitSignal(Tween.SignalName.Finished);
            activeTween.Kill();
        }
        if (validNodePositions.Count() == 0)
        {
            return;
        }
        var tween = room.CreateTween();
        tween.SetParallel();
        foreach (var (node, position) in validNodePositions)
        {
            tween.TweenProperty(node, "position", position, duration).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        }
        _activeTween = tween;
        await room.ToSignal(tween, Tween.SignalName.Finished);
        if (_activeTween == tween)
        {
            _activeTween = null;
        }
    }
}