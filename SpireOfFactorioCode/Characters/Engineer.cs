using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.Commands;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;
using SpireOfFactorio.SpireOfFactorioCode.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Relics;
using SpireOfFactorio.SpireOfFactorioCode.UI;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters;

public sealed class Engineer : PlaceholderCharacterModel
{
    public const string CharacterId = "Engineer";

    public static readonly Color Color = new("cc8f02");
    public override Color MapDrawingColor => new("cc8f02");
    public override Color RemoteTargetingLineColor => new("cc8f02");
    public override Color RemoteTargetingLineOutline => Colors.Black;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 68;
    public override int StartingGold => 99;

    public static int StartingConstructLimit => 3;

    public override IEnumerable<CardModel> StartingDeck =>
        [
            ModelDb.Card<StrikeEngineer>(),
            ModelDb.Card<StrikeEngineer>(),
            ModelDb.Card<StrikeEngineer>(),
            ModelDb.Card<StrikeEngineer>(),
            ModelDb.Card<DefendEngineer>(),
            ModelDb.Card<DefendEngineer>(),
            ModelDb.Card<DefendEngineer>(),
            ModelDb.Card<DefendEngineer>(),
            ModelDb.Card<GunTurret>(),
            ModelDb.Card<Harvest>(),
            ModelDb.Card<Dismantle>()
        ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<FirstGear>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<EngineerCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<EngineerRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<EngineerPotionPool>();

    public override string CustomIconTexturePath => "head.png".CharacterUiPath();

    public override string CustomEnergyCounterPath => "res://SpireOfFactorio/scenes/engineer_energy_counter.tscn";

    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/engineer/engineer.tscn";

    public static readonly AddedNode<NEnergyCounter, EngineerScrapCounter> ScrapCounterNode =
        new(parent =>
        {
            var counter = new EngineerScrapCounter
            {
                Name = "EngineerScrapCounter",
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            counter.SetAnchorsPreset(Control.LayoutPreset.BottomLeft);
            counter.Position = new Vector2(-64, -64);
            counter.Size = new Vector2(128, 128);
            counter.ZIndex = 0;

            var visualScene = ResourceLoader.Load<PackedScene>(
                "res://SpireOfFactorio/scenes/engineer_scrap_counter.tscn");

            var visual = visualScene.Instantiate<Control>();
            visual.Name = "Visual";
            visual.MouseFilter = Control.MouseFilterEnum.Ignore;

            counter.AddChild(visual);

            return counter;

        });

    public static async Task AfterEngineerBlockGained(Task originalTask, ICombatState combatState, Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        await originalTask;
        decimal passedBlockPercent = 75;
        // Only block from cards is passed onto constructs. The Wall's block is unique and has its own implementation to get around this check.
        if (cardSource == null)
        {
            return;
        }
        if (creature.Player == null)
        {
            return;
        }
        if (creature.Player.Character == null)
        {
            return;
        }
        if (creature.Player.HasPower<HardenedSteelPower>())
        {
            passedBlockPercent += creature.GetPowerAmount<HardenedSteelPower>();
        }
        decimal passedBlock = decimal.Ceiling(amount * (passedBlockPercent / 100));
        BlockVar block = new BlockVar(passedBlock, ValueProp.Unpowered);
        if (creature.Player.Character is Engineer)
        {
            // Loop into all constructs
            var pets = creature.Player.PlayerCombatState.Pets;
            if (pets != null)
            {
                foreach (Creature pet in pets)
                {
                    if (pet.IsDead)
                    {
                        return;
                    }
                    if (pet.Monster is ConstructModel)
                    {
                        await CreatureCmd.GainBlock(pet, block, null, true);
                    }
                }
            }
        }
        return;
    }
}

public static class EngineerCombatState
{
    public static readonly SpireField<PlayerCombatState, int> MaxConstructs = new(() => Engineer.StartingConstructLimit);

    public static readonly SpireField<PlayerCombatState, int> PendingOverload = new(() => 0);
}

public static class EngineerCmd
{
    public static async Task<Creature> AddConstruct<T>(PlayerChoiceContext choiceContext, Player player, CardPlay? cardPlay, ConstructCreationArgs args = default) where T : ConstructModel
    {
        ArgumentNullException.ThrowIfNull(player);

        var pets = player.PlayerCombatState.Pets;
        if (pets != null)
        {
            int constructCount = pets.Count(
                p => p.Monster is ConstructModel);

            int maxConstructs = player.Character is Engineer ?
                EngineerCombatState.MaxConstructs[player.PlayerCombatState] : 1;

            if (constructCount >= maxConstructs)
            {
                return null;
            }
        }

        var pet = await PlayerCmd.AddPet<T>(player);
        if (pet.Monster is ConstructModel constructModel)
        {
            constructModel.Position = args.Position;
            constructModel.SourceCard = cardPlay;
        }
        PetOrderManager.TakeSnapshot(player);

        if (pet.Monster is ConstructModel construct)
        {
            await construct.OnConstructed(choiceContext, pet, player, args);
            await ConstructHooks.OnConstructed(choiceContext, pet, player, args);
            await ConstructAnimCmd.Rearrange();

            construct.PlayerDismantled = false;
        }

        foreach (var c in player.PlayerCombatState.Pets
            .Where(p => p.Monster is ConstructModel && p.IsAlive))
        {
            var constructNode = NCombatRoom.Instance?.GetCreatureNode(c);

            if (constructNode != null)
            {
                constructNode.ToggleIsInteractable(true);
            }
        }

        return pet;
    }

    public static AttackCommand FromConstruct(this AttackCommand command, ConstructModel construct)
    {
        return command;
    }
}