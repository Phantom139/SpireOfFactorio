namespace SpireOfFactorio.SpireOfFactorioCode.Patches;

/**
[HarmonyPatch(
    typeof(CustomResourceCost<Scrap>),
    nameof(CustomResourceCost<Scrap>.Clone))]
public static class BaseLibCustomResourceCostClonePatch
{
    private static readonly Version RequiredBaseLibVersion = new(3, 4, 5, 0);

    [HarmonyPrepare]
    private static bool Prepare()
    {
        Version? version = typeof(CustomResourceCost<Scrap>)
            .Assembly
            .GetName()
            .Version;

        bool shouldPatch = version == RequiredBaseLibVersion;

        MainFile.Logger.Info(
            $"[BaseLib #386 Workaround] BaseLib version detected: {version}. " +
            $"Patch {(shouldPatch ? "ENABLED" : "DISABLED")}.");

        return shouldPatch;
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo energyCostGetter =
            AccessTools.PropertyGetter(
                typeof(CardModel),
                nameof(CardModel.EnergyCost))!;

        MethodInfo energyCostsXGetter =
            AccessTools.PropertyGetter(
                typeof(CardEnergyCost),
                nameof(CardEnergyCost.CostsX))!;

        MethodInfo customCostsXGetter =
            AccessTools.PropertyGetter(
                typeof(CustomResourceCost<Scrap>),
                nameof(CustomResourceCost<Scrap>.CostsX))!;

        var matcher = new CodeMatcher(instructions);

        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Callvirt, energyCostGetter),
            new CodeMatch(OpCodes.Callvirt, energyCostsXGetter)
        );

        if (!matcher.IsValid)
        {
            throw new InvalidOperationException(
                "[BaseLib #386 Workaround] Could not find the " +
                "newCard.EnergyCost.CostsX expression in Clone(). " +
                "BaseLib's implementation may have changed.");
        }

        MainFile.Logger.Info(
            "[BaseLib #386 Workaround] Replacing newCard.EnergyCost.CostsX " +
            "with source CustomResourceCost<Scrap>.CostsX.");

        matcher
            .RemoveInstructions(3)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, customCostsXGetter)
            );

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(typeof(CustomResourceCost<Scrap>), nameof(CustomResourceCost<Scrap>.GetAmountToSpend))]
    public static class GetAmountToSpendPatch
    {
        public static void Postfix(CustomResourceCost<Scrap> __instance, ref int __result)
        {
            if (__instance.CostsX)
            {
                __instance.CapturedXValue = __result;
            }
        }
    }

    private static bool GetScrapCostsX(CardModel card)
    {
        return CustomResources<Scrap>.CanonicalCost(card) == int.MinValue;
    }
}
**/