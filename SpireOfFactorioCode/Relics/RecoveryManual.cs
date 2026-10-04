using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Characters;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

[Pool(typeof(EngineerRelicPool))]
public class RecoveryManual : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public static decimal recover_percent = 25;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", recover_percent), new DynamicVar("Scrap", 1m)];

    // See CustomConstructModel.cs for the Recovery Manual code
}