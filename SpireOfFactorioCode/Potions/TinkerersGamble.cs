using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Rngs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Characters.Constructs;

namespace SpireOfFactorio.SpireOfFactorioCode.Potions;

public class TinkerersGamble : EngineerPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

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

        int rng = base.Owner.PlayerRng.GetRng(PlayerRngType.Rewards).NextInt(8);
        switch (rng)
        {
            case 0:
                await EngineerCmd.AddConstruct<GunTurret_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(GunTurret_Upgraded.spawnHP, Position: ConstructPosition.FrontAbove)
                    );
                break;
            case 1:
                await EngineerCmd.AddConstruct<QualityGunTurret_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(QualityGunTurret_Upgraded.spawnHP, Position: ConstructPosition.FrontAbove)
                    );
                break;
            case 2:
                await EngineerCmd.AddConstruct<LaserTurret_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(LaserTurret_Upgraded.spawnHP, Position: ConstructPosition.Front)
                    );
                break;
            case 3:
                await EngineerCmd.AddConstruct<FlamethrowerTurret_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(FlamethrowerTurret_Upgraded.spawnHP, Position: ConstructPosition.Below)
                    );
                break;
            case 4:
                await EngineerCmd.AddConstruct<Wall_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(Wall_Upgraded.spawnHP, Position: ConstructPosition.Front)
                    );
                break;
            case 5:
                await EngineerCmd.AddConstruct<TeslaTurret_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(TeslaTurret_Upgraded.spawnHP, Position: ConstructPosition.Above)
                    );
                break;
            case 6:
                await EngineerCmd.AddConstruct<Beacon_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(Beacon_Upgraded.spawnHP, Position: ConstructPosition.Behind)
                    );
                break;
            case 7:
            default:
                await EngineerCmd.AddConstruct<ArtilleryTurret_Upgraded>(choiceContext, Owner, null,
                    new ConstructCreationArgs(ArtilleryTurret_Upgraded.spawnHP, Position: ConstructPosition.BehindBelow)
                    );
                break;
        }


    }
}
