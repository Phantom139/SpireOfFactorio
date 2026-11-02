using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
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

    public static async Task<List<Creature>?> SelectTarget(
        Creature construct,
        ConstructTargetSelectionType targetType = ConstructTargetSelectionType.RandomEnemy)
    {
        if (construct == null || construct.CombatState == null)
            return null;

        List<Creature> targets = new();

        switch (targetType)
        {
            case ConstructTargetSelectionType.RandomEnemy:
                {
                    List<Creature> enemies = construct.CombatState.Enemies
                        .Where(c => !c.IsDead)
                        .ToList();

                    if (enemies.Count > 0)
                    {
                        targets.Add(enemies.TakeRandom(
                            1,
                            construct.CombatState.RunState.Rng.CombatTargets
                        ).First());
                    }
                    break;
                }

            case ConstructTargetSelectionType.RandomPlayer:
                {
                    List<Player> players = construct.CombatState.Players
                        .Where(p => !p.Creature.IsDead)
                        .ToList();

                    if (players.Count > 0)
                    {
                        Player player = players.TakeRandom(
                            1,
                            construct.CombatState.RunState.Rng.CombatTargets
                        ).First();

                        targets.Add(player.Creature);
                    }
                    break;
                }

            case ConstructTargetSelectionType.WeakestEnemy:
                {
                    Creature? weakest_target = construct.CombatState.Enemies
                        .Where(c => !c.IsDead)
                        .OrderBy(c => c.CurrentHp)
                        .FirstOrDefault();

                    if (weakest_target != null)
                        targets.Add(weakest_target);

                    break;
                }

            case ConstructTargetSelectionType.WeakestPlayer:
                {
                    Creature? weakest_player = construct.CombatState.Players
                        .Select(p => p.Creature)
                        .Where(c => !c.IsDead)
                        .OrderBy(c => c.CurrentHp)
                        .FirstOrDefault();

                    if (weakest_player != null)
                        targets.Add(weakest_player);

                    break;
                }

            case ConstructTargetSelectionType.StrongestEnemy:
                {
                    Creature? strongest_target = construct.CombatState.Enemies
                        .Where(c => !c.IsDead)
                        .OrderByDescending(c => c.CurrentHp)
                        .FirstOrDefault();

                    if (strongest_target != null)
                        targets.Add(strongest_target);

                    break;
                }

            case ConstructTargetSelectionType.StrongestPlayer:
                {
                    Creature? strongest_player = construct.CombatState.Players
                        .Select(p => p.Creature)
                        .Where(c => !c.IsDead)
                        .OrderByDescending(c => c.CurrentHp)
                        .FirstOrDefault();

                    if (strongest_player != null)
                        targets.Add(strongest_player);

                    break;
                }

            case ConstructTargetSelectionType.AllEnemies:
                targets.AddRange(
                    construct.CombatState.Enemies.Where(c => !c.IsDead)
                );
                break;

            case ConstructTargetSelectionType.AllPlayers:
                targets.AddRange(
                    construct.CombatState.Players
                        .Select(p => p.Creature)
                        .Where(c => !c.IsDead)
                );
                break;
        }

        await ConstructHooks.ModifyConstructTargetingList(
            construct,
            targetType,
            targets
        );

        Creature? target = targets?.FirstOrDefault();

        if (target == null)
            return null;

        return targets;
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