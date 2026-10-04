using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode;

namespace SpireOfFactorio.Libraries.ConstructLib.Util;

public static class Debug
{
    public static void DumpNodeTree(Node node, int depth)
    {
        if (depth > 3)
        {
            return;
        }

        var indent = new string(' ', depth * 2);

        MainFile.Logger.Info(
            $"[ConstructDebug:Tree] " +
            $"{indent}{node.Name} " +
            $"[{node.GetType().FullName}]"
        );

        foreach (var child in node.GetChildren())
        {
            if (child is Node childNode)
            {
                DumpNodeTree(childNode, depth + 1);
            }
        }
    }

    public static void DumpTransform(string name, Node2D node)
    {
        MainFile.Logger.Info(
            $"[ConstructDebug:Transform] " +
            $"{name}: " +
            $"Position={node.Position}, " +
            $"GlobalPosition={node.GlobalPosition}, " +
            $"Scale={node.Scale}, " +
            $"Rotation={node.Rotation}, " +
            $"Visible={node.Visible}"
        );
    }

    public static void DumpTransform(string name, Control node)
    {
        MainFile.Logger.Info(
            $"[ConstructDebug:Transform] " +
            $"{name}: " +
            $"Position={node.Position}, " +
            $"GlobalPosition={node.GlobalPosition}, " +
            $"Size={node.Size}, " +
            $"Scale={node.Scale}, " +
            $"Visible={node.Visible}"
        );
    }

}

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Damage),
    new[]
    {
        typeof(PlayerChoiceContext),
        typeof(IEnumerable<Creature>),
        typeof(decimal),
        typeof(ValueProp),
        typeof(Creature),
        typeof(CardModel)
    })]
public static class CreatureCmdDamageDebugPatch
{
    [HarmonyPrefix]
    private static void Prefix(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        decimal amount,
        ValueProp props,
        Creature dealer,
        CardModel cardSource)
    {
        foreach (var target in targets)
        {
            MainFile.Logger.Info(
                $"[ConstructDebug:DamageEntry] " +
                $"Dealer={dealer?.Name ?? "NULL"}, " +
                $"Target={target.Name}, " +
                $"TargetType={target.Monster?.GetType().Name ?? "NULL"}, " +
                $"IsPet={target.IsPet}, " +
                $"PetOwner={target.PetOwner?.Creature?.Name ?? "NULL"}, " +
                $"HP={target.CurrentHp}/{target.MaxHp}, " +
                $"Amount={amount}, " +
                $"Props={props}"
            );
        }
    }
}