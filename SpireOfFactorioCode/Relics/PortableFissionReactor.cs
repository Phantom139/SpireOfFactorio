using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class PortableFissionReactor : SpireOfFactorioRelic
{
    //public override RelicRarity Rarity => RelicRarity.Event;
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new PowerVar<StrengthPower>(2m), new PowerVar<DexterityPower>(2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Type == CardType.Power)
        {
            int cost = card.EnergyCost.Canonical * -1;
            if (cost != 0)
            {
                card.EnergyCost.AddThisCombat(cost);
            }
        }
        return Task.CompletedTask;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, -2, base.Owner.Creature, null);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, -2, base.Owner.Creature, null);
        }
    }

}