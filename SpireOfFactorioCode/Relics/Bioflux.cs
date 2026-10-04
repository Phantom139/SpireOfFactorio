using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class Bioflux : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar("Heal1", 4), new HealVar("Heal2", 3)];

    private const int _healBegin = 4;
    private const int _healAfterDamage = 3;

    public int hp_at_start;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType == RoomType.Monster || room.RoomType == RoomType.Elite || room.RoomType == RoomType.Boss)
        {
            Flash();
            await CreatureCmd.Heal(base.Owner.Creature, _healBegin);

            hp_at_start = base.Owner.Creature.CurrentHp;
        }
    }

    public override async Task AfterCombatVictory(CombatRoom _)
    {
        if (!base.Owner.Creature.IsDead)
        {
            if (base.Owner.Creature.CurrentHp < hp_at_start)
            {
                Flash();
                await CreatureCmd.Heal(base.Owner.Creature, _healAfterDamage);
            }
        }
    }

}
