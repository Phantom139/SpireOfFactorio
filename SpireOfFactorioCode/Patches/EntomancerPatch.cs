using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.SpireOfFactorioCode.Patches;

[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived))]
public static class PersonalHivePowerPatch
{
    [HarmonyPrefix]
    public static bool Prefix(
        Creature? dealer,
        ref Task __result)
    {
        if (dealer?.Monster is ConstructModel)
        {
            MainFile.Logger.Info(
                "[PersonalHivePowerPatch] Suppressing Personal Hive " +
                "response to Construct attack.");

            __result = Task.CompletedTask;
            return false;
        }

        return true;
    }
}