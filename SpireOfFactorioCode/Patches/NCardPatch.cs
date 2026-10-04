namespace SpireOfFactorio.SpireOfFactorioCode.Patches;

/**
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
public static class NCardUpdateVisualsPatch
{
    public static void Postfix(NCard __instance, PileType pileType, CardPreviewMode previewMode)
    {
        var display = __instance
            .GetChild(0)
            .GetNodeOrNull<CardScrapCostDisplay>("CardScrapCostDisplay");

        //display?.UpdateScrapCostText()
        display?.UpdateScrapCostVisuals(pileType);
    }
}
**/
