using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.BaseLib;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class Wall : CustomConstructModel
{
    public override int MinInitialHp => 30;
    public override int MaxInitialHp => 30;

    public static int spawnHP = 30;
    public static BlockVar block = new BlockVar(7m, ValueProp.Move);

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.AfterPlayerTurn;

    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/wall.tscn";

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-WALL.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-WALL.description");
        description.Add("Block", block.IntValue);

        return new HoverTip(title, description);
    }

    public override async Task OnConstructed(PlayerChoiceContext choiceContext, Creature construct, Player owner, ConstructCreationArgs args)
    {
        var self = Creature;
        if (args.MaxHp is decimal maxHp)
        {
            await CreatureCmd.SetMaxAndCurrentHp(self, maxHp);
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (IsDisabled())
        {
            return;
        }
        if (side == CombatSide.Player)
        {
            // Check if any enemies left, if none, ignore.
            if (base.CombatState.HittableEnemies.Count == 0)
            {
                return;
            }
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Creature)
                                               where c != null && c.IsAlive && c.Monster is not Wall && c.Monster is not Wall_Upgraded
                                               select c;
            foreach (Creature item in enumerable)
            {
                decimal outBlock = decimal.Ceiling(block.BaseValue * OverclockPercent());
                await CreatureCmd.GainBlock(item, outBlock, ValueProp.Move, null, true);
            }
            await ConstructHooks.OnConstructPerformedTask(choiceContext, Creature, Creature.PetOwner);
        }
        return;
    }
}
