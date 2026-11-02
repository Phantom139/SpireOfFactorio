using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.Libraries.ConstructLib.Construct;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class TargetingRoutinePower : SpireOfFactorioPower
{
    public override PowerType Type
    {
        get
        {
            return PowerType.Debuff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task ModifyConstructTargetingList(Creature construct, ConstructTargetSelectionType targetingSelectionType, List<Creature> targetList)
    {
        if (targetingSelectionType == ConstructTargetSelectionType.RandomEnemy || targetingSelectionType == ConstructTargetSelectionType.WeakestEnemy || targetingSelectionType == ConstructTargetSelectionType.StrongestEnemy)
        {
            List<Creature> newList = new List<Creature>();
            newList.Add(Owner);
            targetList = newList;
        }
    }
}
