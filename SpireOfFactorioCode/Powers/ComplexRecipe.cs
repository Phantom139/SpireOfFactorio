using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public class ComplexRecipe : SpireOfFactorioPower
{
    private bool _shouldIgnoreNextInstance;

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public CardModel OriginModel => ModelDb.Card<Cards.Status.ComplexRecipe>();

    public PowerModel InternallyAppliedPower => ModelDb.Power<DexterityPower>();

    public override LocString Title
    {
        get
        {
            return OriginModel.TitleLocString;
        }
    }

    public override LocString Description => new LocString("powers", "TEMPORARY_DEXTERITY_POWER.description");

    protected override string SmartDescriptionLocKey
    {
        get
        {
            return "TEMPORARY_DEXTERITY_POWER.smartDescription";
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> list = new List<IHoverTip>();
            List<IHoverTip> list2 = list;
            IEnumerable<IHoverTip> collection;
            collection = OriginModel.HoverTips;

            list2.AddRange(collection);
            list.Add(HoverTipFactory.FromPower<DexterityPower>());
            return list;
        }
    }

    public void IgnoreNextInstance()
    {
        _shouldIgnoreNextInstance = true;
    }

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
        }
        else
        {
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), target, (decimal)amount, applier, cardSource, silent: true);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!(amount == (decimal)base.Amount) && power == this)
        {
            if (_shouldIgnoreNextInstance)
            {
                _shouldIgnoreNextInstance = false;
            }
            else
            {
                await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner, (decimal)amount, applier, cardSource, silent: true);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner, -base.Amount, base.Owner, null);
        }
    }
}
