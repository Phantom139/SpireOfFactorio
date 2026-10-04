using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace SpireOfFactorio.Libraries.ConstructLib.Construct;

public abstract class ConstructModel : MonsterModel
{
    public override string DeathSfx => "event:/sfx/characters/osty/osty_die";
    public override bool HasDeathSfx => true;

    public ConstructPosition Position { get; internal set; }

    public ConstructActionPhase ActionPhase { get; set; }

    public CardPlay? SourceCard { get; internal set; }

    public bool PlayerDismantled { get; set; }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState idle = new("CONSTRUCT_IDLE", _ => Task.CompletedTask)
        {
            FollowUpState = null
        };
        idle.FollowUpState = idle;
        return new MonsterMoveStateMachine([idle], idle);
    }

    public virtual Task OnConstructed(PlayerChoiceContext choiceContext, Creature construct, Player owner, ConstructCreationArgs args)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnConstructPerformedTask(PlayerChoiceContext choiceContext, Creature construct, Player owner)
    {
        return Task.CompletedTask;
    }

    public override bool ShouldAllowTargeting(Creature target)
    {
        return true;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        return true;
    }

}

public readonly record struct ConstructCreationArgs(
    decimal? MaxHp = null,
    decimal? PrimaryStatValue = null,
    decimal? SecondaryStatValue = null,
    decimal? TertiaryStatValue = null,
    CardModel? Source = null,
    ConstructPosition Position = ConstructPosition.Front);

public enum ConstructPosition
{
    Front = 0,
    Above,
    Below,
    Behind,
    FrontAbove,
    FrontBelow,
    BehindAbove,
    BehindBelow
}

public enum ConstructActionPhase
{
    BeforePlayerTurn = 0,
    AfterPlayerTurn,
    BeforeEnemyTurn,
    AfterEnemyTurn
}