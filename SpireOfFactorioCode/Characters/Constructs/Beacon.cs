using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireOfFactorio.Libraries.ConstructLib.BaseLib;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

public sealed class Beacon : CustomConstructModel
{
    public override int MinInitialHp => 20;
    public override int MaxInitialHp => 20;

    public static int spawnHP = 20;
    public override string CustomVisualPath => "res://SpireOfFactorio/scenes/creature_visuals/beacon.tscn";

    public static ConstructActionPhase ActionPhase => ConstructActionPhase.BeforePlayerTurn;

    public static HoverTip GetHoverTip()
    {
        LocString title = new("static_hover_tips", "SPIREOFFACTORIO-BEACON.title");
        LocString description = new("static_hover_tips", "SPIREOFFACTORIO-BEACON.description");
        description.Add("Scrap", 1);
        description.Add("Energy", 1);

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

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (IsDisabled())
        {
            return;
        }
        if (side == CombatSide.Player)
        {
            // Fetch the current scrap count
            if (Creature.PetOwner.Creature.IsDead)
            {
                return;
            }
            var scrap = CustomResources<Scrap>.Get(Creature.PetOwner.PlayerCombatState);
            int currentScrap = scrap.Amount;
            decimal energyToGain = decimal.Ceiling((currentScrap / 4) * OverclockPercent());
            //decimal energyToPass = decimal.Ceiling((energyToGain / 2) * OverclockPercent());

            await PlayerCmd.GainEnergy(energyToGain, Creature.PetOwner);
            await ConstructHooks.OnConstructPerformedTask(new ThrowingPlayerChoiceContext(), Creature, Creature.PetOwner);
        }
        return;
    }
}
