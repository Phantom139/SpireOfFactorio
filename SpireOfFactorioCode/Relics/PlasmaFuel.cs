using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

[Pool(typeof(EngineerRelicPool))]
public class PlasmaFuel : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public static decimal debuff_count = 2m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ScorchPower>(), HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<DexterityPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>("Strength", debuff_count), new PowerVar<DexterityPower>("Dexterity", debuff_count)];
}