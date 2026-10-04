using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Recycle : EngineerCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("RecycleRecover", 50m), new DynamicVar("RecycleBlockFromHP", 50m), new DynamicVar("Scrap", 1m)];

    public Recycle() : base(1, CardType.Skill, CardRarity.Common, ConstructTargetTypes.AnyConstructIOwn)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Safety check :)
        if (cardPlay.Target?.Monster is ConstructModel constructModel)
        {
            constructModel.PlayerDismantled = true;

            // Fetch the current HP of the construct
            decimal currentHP = cardPlay.Target.CurrentHp;
            decimal blockGained = currentHP * (DynamicVars["RecycleBlockFromHP"].BaseValue / 100);
            await CreatureCmd.GainBlock(Owner.Creature, blockGained, ValueProp.Move, null);

            // Fetch the original card we used.
            CardPlay sourceCard = constructModel.SourceCard;
            if (sourceCard != null)
            {
                CustomResourceCost<Scrap> cardCost = CustomResources<Scrap>.Cost(sourceCard.Card);
                if (cardCost.Canonical > 0)
                {
                    decimal scrapGained = decimal.Ceiling(cardCost.Canonical * (DynamicVars["RecycleRecover"].BaseValue / 100));
                    var scrap = CustomResources<Scrap>.Get(
                        Owner.PlayerCombatState!);
                    scrap.ModifyAmount((int)scrapGained);
                }
            }

            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            await CreatureCmd.Kill(cardPlay.Target);
            //await CreatureCmd.Damage(choiceContext, cardPlay.Target, cardPlay.Target.MaxHp * 100, ValueProp.Unblockable, base.Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["RecycleRecover"].UpgradeValueBy(25m);
        DynamicVars["RecycleBlockFromHP"].UpgradeValueBy(25m);
    }

}
