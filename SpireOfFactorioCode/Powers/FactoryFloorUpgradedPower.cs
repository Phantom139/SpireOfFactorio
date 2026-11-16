using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Token;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class FactoryFloorUpgradedPower : SpireOfFactorioPower
{
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"factory_floor_power.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"factory_floor_power.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!base.Owner.IsPlayer)
        {
            return;
        }
        if (cardPlay.Player != base.Owner.Player)
        {
            return;
        }
        var scrapCost = CustomResources<Scrap>.Cost(cardPlay.Card);
        if (scrapCost == null)
        {
            return;
        }
        await Logistics.CreateInHand(cardPlay.Player, 1, CombatState, base.Owner.Player, true);
    }

}
