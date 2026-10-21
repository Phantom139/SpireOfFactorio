using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Potions;

public class PowerCell : EngineerPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<OverclockPower>(3m)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null)
        {
            return;
        }
        if (!target.IsPlayer)
        {
            return;
        }

        var pets = Owner.Creature.Pets;
        foreach (Creature pet in pets)
        {
            if (pet.Monster is ConstructModel)
            {
                await PowerCmd.Apply<OverclockPower>(choiceContext, pet, DynamicVars.Power<OverclockPower>().BaseValue, base.Owner.Creature, null);
            }
        }

    }
}
