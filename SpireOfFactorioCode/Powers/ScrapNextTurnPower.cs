using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ScrapNextTurnPower : SpireOfFactorioPower
{
    public override List<(string, string)> Localization => new PowerLoc(
        "Scrap Next Turn",
        "Gain 1 [img]res://SpireOfFactorio/images/charui/text_scrap.png[/img] next turn.",
        "Gain [blue]{Amount}[/blue] [img]res://SpireOfFactorio/images/charui/text_scrap.png[/img] next turn.");

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == base.Owner.Player)
        {
            //await PlayerCmd.GainStars(base.Amount, player);
            var scrap = CustomResources<Scrap>.Get(player.PlayerCombatState);
            scrap.ModifyAmount(base.Amount);
            await PowerCmd.Remove(this);
        }
    }

}
