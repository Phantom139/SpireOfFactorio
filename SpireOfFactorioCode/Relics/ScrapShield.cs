using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

[Pool(typeof(EngineerRelicPool))]
public class ScrapShield : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(1, ValueProp.Move), new DynamicVar("Scrap", 1m)];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != base.Owner.Creature.Side)
        {
            return;
        }

        // Obtain current scrap
        var scrap = CustomResources<Scrap>.Get(base.Owner.PlayerCombatState);
        int currentScrap = scrap.Amount;

        // Convert to block
        await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block.BaseValue * currentScrap, ValueProp.Move, null);

        return;
    }

}