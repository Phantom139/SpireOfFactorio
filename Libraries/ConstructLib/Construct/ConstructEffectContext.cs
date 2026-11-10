using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.Libraries.ConstructLib.Construct;

public class ConstructEffectContext
{
    public Creature Construct { get; set; }

    // Damage
    public Decimal BaseDamage { get; set; }
    public Decimal AdditiveDamage { get; set; }
    public Decimal MultiplicativeDamage { get; set; } = 1m;
    public Decimal FinalDamage
    {
        get
        {
            return (BaseDamage + AdditiveDamage) * MultiplicativeDamage;
        }

    }

    // Block
    public Decimal BaseBlock { get; set; }
    public Decimal AdditiveBlock { get; set; }
    public Decimal MultiplicativeBlock { get; set; } = 1m;
    public Decimal FinalBlock
    {
        get
        {
            return (BaseBlock + AdditiveBlock) * MultiplicativeBlock;
        }
    }

    // Buffs and Debuffs
    public Dictionary<Type, int> Powers { get; set; } = new();

    public ValueProp Props { get; set; }

    public ConstructEffectContext(Creature construct)
    {
        Construct = construct;
    }

}
