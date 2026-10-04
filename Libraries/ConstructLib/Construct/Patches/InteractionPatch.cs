using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireOfFactorio.Libraries.ConstructLib.Commands;
using SpireOfFactorio.Libraries.ConstructLib.Layout;

namespace SpireOfFactorio.Libraries.ConstructLib.Construct.Patches;

[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
public static class ConstructAddPatch
{
    [HarmonyPrefix]
    private static bool Prefix(NCombatRoom __instance, out IReadOnlyList<ConstructNodePosition> __state)
    {
        __state = ConstructLayoutManager.GetCurrentConstructPositions(__instance);
        return true;
    }

    [HarmonyPostfix]
    private static void Postfix(NCombatRoom __instance, Creature creature, IReadOnlyList<ConstructNodePosition> __state)
    {
        ConstructAnimCmd.InstantMove(__state);
        if (creature.PetOwner == null || creature.Monster is not ConstructModel)
        {
            return;
        }
        __instance.GetCreatureNode(creature)!.Position = __instance.GetCreatureNode(creature.PetOwner.Creature)!.Position;
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.ToggleIsInteractable))]
public static class ConstructInteractablePatch
{
    [HarmonyPrefix]
    private static void Prefix(NCreature __instance, ref bool on)
    {
        if (__instance.Entity.Monster is ConstructModel && LocalContext.IsMe(__instance.Entity.PetOwner))
        {
            on = true;
        }
    }
}

/**
[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.AddPet))]
public static class AddPetPatch
{
    [HarmonyPostfix]
    private static void Postfix(Task __instance, Creature pet, Player player)
    {

    }
}
**/