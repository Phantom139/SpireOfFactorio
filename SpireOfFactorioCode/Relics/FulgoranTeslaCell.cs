using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

[Pool(typeof(EngineerRelicPool))]
public class FulgoranTeslaCell : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Turns", 3m)];

    // This relic is special in that it acts as a flag in the Shock power's code for removal on non-adjustment.
}