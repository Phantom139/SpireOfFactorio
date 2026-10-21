using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Enchantments;

public sealed class Efficient : CustomEnchantmentModel
{
    protected override string? CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardEnchantmentImagePath();

    public override bool CanEnchant(CardModel card)
    {
        if (card.EnergyCost.CostsX)
        {
            return false;
        }
        if (card.EnergyCost.Canonical == 0)
        {
            return false;
        }
        return true;
    }

    protected override void OnEnchant()
    {
        base.Card.EnergyCost.UpgradeBy(-base.Amount);
    }

}
