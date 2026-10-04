using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.SpireOfFactorioCode.Patches;

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.DynamicDescription), MethodType.Getter)]
public static class RelicModelDynamicDescriptionPatch
{
    private const string ScrapIconSprite =
        "[img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]";

    [HarmonyPostfix]
    public static void Postfix(ref LocString __result)
    {
        __result.Add("singleScrapIcon", ScrapIconSprite);
    }
}