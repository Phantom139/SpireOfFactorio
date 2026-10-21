using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class MiningDrillPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 1m)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            if (base.Owner.IsPlayer)
            {
                int currentActiveConstructs = EngineerCmd.GetConstructCount(base.Owner.Player);
                int maxConstructs = EngineerCmd.GetMaxConstructs(base.Owner.Player);

                int miningDrill = maxConstructs - currentActiveConstructs;
                int scrapGain = miningDrill * base.Amount;
                var scrap = CustomResources<Scrap>.Get(base.Owner.Player.PlayerCombatState);
                scrap.ModifyAmount(scrapGain);

                //MainFile.Logger.Info($"[MiningDrillPower:Debug()] currentConstructs: {currentActiveConstructs}\n" +
                //    $"maximumConstructs: {maxConstructs}\n" +
                //    $"openSlots: {miningDrill}\n" +
                //    $"buffAmount: {base.Amount}\n" +
                //    $"scrapToGain: {scrapGain}");

                Flash();
            }
        }
    }
}
