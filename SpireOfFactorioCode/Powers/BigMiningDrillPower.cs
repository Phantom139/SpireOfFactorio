using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class BigMiningDrillPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 1m), new EnergyVar(1)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        CardModel extra = CardPileCmd.Draw(choiceContext, player).Result;
        if (extra != null)
        {
            // Scan the energy and scrap cost on the extra card
            Decimal nEnergyCost;
            Decimal nScrapCost;

            CardEnergyCost energyCost = extra.EnergyCost;
            var scrapCost = CustomResources<Scrap>.Cost(extra);
            if (scrapCost == null)
            {
                nScrapCost = 0;
            }
            // Ignore if X
            if (energyCost.CostsX || scrapCost.CostsX)
            {
                return;
            }
            nEnergyCost = energyCost.GetWithModifiers(CostModifiers.All);
            nScrapCost = scrapCost.GetWithModifiers(CostModifiers.All);
            // If both are equal, we skip
            if (nEnergyCost == nScrapCost)
            {
                return;
            }
            Flash();
            if (nEnergyCost > nScrapCost)
            {
                // Energy > Scrap, Gain Scrap
                Decimal scrapGain = Decimal.Ceiling(0.25m * nEnergyCost);
                var scrap = CustomResources<Scrap>.Get(
                    Owner.Player.PlayerCombatState!);
                scrap.ModifyAmount((int)scrapGain);
            }
            else
            {
                // Scrap > Energy, Gain Energy
                Decimal energyGain = Decimal.Ceiling(0.25m * nScrapCost);
                await PlayerCmd.GainEnergy(energyGain, Owner.Player);
            }

        }

    }

}
