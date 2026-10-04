using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class HardenedSteelPower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    // Note: This power's logic is handled in the Construct Block code.
    // See: AfterEngineerBlockGained in Characters/Engineer.cs
}
