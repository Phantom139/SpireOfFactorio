using BaseLib.Abstracts;
using BaseLib.Extensions;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Enchantments;

public sealed class Replicating : CustomEnchantmentModel
{
    protected override string? CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardEnchantmentImagePath();

    protected override void OnEnchant()
    {
        base.Card.BaseReplayCount += base.Amount;
    }

}
