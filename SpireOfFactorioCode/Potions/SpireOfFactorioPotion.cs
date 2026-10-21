using BaseLib.Abstracts;
using BaseLib.Extensions;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.Potions;

public abstract class SpireOfFactorioPotion(bool autoAdd = true) :
    CustomPotionModel(autoAdd)
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png"
            .PotionImagePath();
}