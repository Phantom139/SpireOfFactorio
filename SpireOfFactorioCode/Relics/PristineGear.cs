using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class PristineGear : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScrapVar(5), new ScrapVar("ScrapAdd", 1)];

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (base.Owner.PlayerCombatState.TurnNumber > 1)
        {
            var scrap = CustomResources<Scrap>.Get(base.Owner.PlayerCombatState!);
            scrap.ModifyAmount(1);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            var scrap = CustomResources<Scrap>.Get(
                base.Owner.PlayerCombatState!);
            scrap.ModifyAmount(5);
        }
    }
}
