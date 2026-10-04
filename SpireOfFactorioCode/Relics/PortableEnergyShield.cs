using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class PortableEnergyShield : SpireOfFactorioRelic
{
    //public override RelicRarity Rarity => RelicRarity.Event;
    public override RelicRarity Rarity => RelicRarity.Event;

    private const string _artifactStartKey = "ArtifactStart";
    private const string _artifactGainKey = "ArtifactGain";
    private const string _blockKey = "Block";

    private const int _artifactGainStart = 3;
    private const int _artifactGainBlock = 1;
    private const int _blockNeededToGain = 30;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ArtifactPower>(_artifactStartKey, _artifactGainStart), new PowerVar<ArtifactPower>(_artifactGainKey, _artifactGainBlock), new BlockVar((Decimal)_blockNeededToGain, ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ArtifactPower>()];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom && room.RoomType != RoomType.Boss)
        {
            Flash();
            await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, _artifactGainStart, base.Owner.Creature, null);
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (base.Owner.RunState.BaseRoom.RoomType == RoomType.Boss)
        {
            return Task.CompletedTask;
        }

        if (side != base.Owner.Creature.Side)
        {
            return Task.CompletedTask;
        }

        if (base.Owner.Creature.Block >= _blockNeededToGain)
        {
            Flash();
            PowerCmd.Apply<ArtifactPower>(choiceContext, base.Owner.Creature, _artifactGainBlock, base.Owner.Creature, null);
        }

        return Task.CompletedTask;
    }

}
