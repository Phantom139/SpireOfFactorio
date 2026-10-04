using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode;

namespace SpireOfFactorio.Libraries.ConstructLib.Commands;

public static class ConstructCmd
{
    public static async Task<Creature> AddConstruct<T>(PlayerChoiceContext choiceContext, Player player, ConstructCreationArgs args = default) where T : ConstructModel
    {
        ArgumentNullException.ThrowIfNull(player);

        var pet = await PlayerCmd.AddPet<T>(player);
        if (pet.Monster is ConstructModel constructModel)
        {
            constructModel.Position = args.Position;
        }
        PetOrderManager.TakeSnapshot(player);

        if (pet.Monster is ConstructModel construct)
        {
            await construct.OnConstructed(choiceContext, pet, player, args);

            await ConstructHooks.OnConstructed(choiceContext, pet, player, args);
        }

        return pet;
    }
}

public static class ConstructDebug
{
    public static void DumpConstruct(string stage, Creature creature, Player player)
    {
        var monster = creature.Monster;

        MainFile.Logger.Info(
            $"[ConstructDebug:{stage}] " +
            $"Creature={creature.GetType().FullName}, " +
            $"Monster={monster?.GetType().FullName}, " +
            $"Name={creature.Name}, " +
            $"IsAlive={creature.IsAlive}, " +
            $"IsPet={creature.IsPet}, " +
            $"PetOwner={creature.PetOwner?.Creature.Name ?? "NULL"}"
        );

        if (monster is ConstructModel construct)
        {
            MainFile.Logger.Info(
                $"[ConstructDebug:{stage}] " +
                $"Construct={construct.GetType().FullName}, " +
                $"Position={construct.Position}, " +
                $"HP={creature.CurrentHp}/{creature.MaxHp}"
            );
        }

        var pets = player.PlayerCombatState.Pets;

        MainFile.Logger.Info(
            $"[ConstructDebug:{stage}] " +
            $"Player Pets={pets?.Count ?? 0}, " +
            $"Construct Pets={pets?.Count(p => p.Monster is ConstructModel) ?? 0}"
        );
    }
}