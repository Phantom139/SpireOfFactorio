using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class FirstGear : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<PristineGear>();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScrapVar(3), new ScrapVar("ScrapAdd", 1)];

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (base.Owner.PlayerCombatState.TurnNumber > 1)
        {
            var scrap = CustomResources<Scrap>.Get(base.Owner.PlayerCombatState!);
            scrap.ModifyAmount(1);
        }
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            var scrap = CustomResources<Scrap>.Get(
                base.Owner.PlayerCombatState!);
            scrap.ModifyAmount(3);
        }
        return Task.CompletedTask;
    }
}
