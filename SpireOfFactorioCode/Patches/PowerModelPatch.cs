using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.SpireOfFactorioCode.Patches;

[HarmonyPatch(typeof(PowerModel), nameof(PowerModel.AddDumbVariablesToDescription))]
public static class AddDumbVariablesToDescriptionPatch
{
    public static void Postfix(LocString description, int? amountOverride = null)
    {
        description.Add(
            "singleScrapIcon",
            "[img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]");
    }
}