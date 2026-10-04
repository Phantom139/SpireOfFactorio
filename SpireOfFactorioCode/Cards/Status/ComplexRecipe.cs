using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Status;

[Pool(typeof(StatusCardPool))]
public sealed class ComplexRecipe : SpireOfFactorioCard
{
    public ComplexRecipe() : base(0, CardType.Status, CardRarity.Status, TargetType.Self)
    {

    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(1m), new BlockVar(2m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];

    public override int MaxUpgradeLevel => 0;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<Powers.ComplexRecipe>(context, Owner.Creature, DynamicVars.Dexterity.BaseValue, Owner.Creature, this);

        int additional_copies = 0;

        List<CardModel> cards = PileType.Hand.GetPile(Owner).Cards.ToList();
        CardModel card = play.Card;
        foreach (CardModel cI in cards)
        {
            if (cI != card && cI.Id == card.Id)
            {
                additional_copies++;
            }
        }
        if (additional_copies > 0)
        {
            BlockVar add = new BlockVar(DynamicVars.Block.BaseValue * additional_copies, ValueProp.Move);
            await CreatureCmd.GainBlock(Owner.Creature, add, play);
        }
    }
}
