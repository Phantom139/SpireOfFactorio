using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class EmergencyRecall : EngineerCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [EngineerKeyWords.Construct];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 1m)];

    public EmergencyRecall() : base(1, CardType.Skill, CardRarity.Rare, ConstructTargetTypes.AnyConstructIOwn)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Safety check :)
        if (cardPlay.Target?.Monster is ConstructModel constructModel)
        {
            constructModel.PlayerDismantled = true;

            // Fetch the original card we used.
            CardPlay sourceCard = constructModel.SourceCard;
            if (sourceCard != null)
            {
                await CardPileCmd.Add(sourceCard.Card, PileType.Hand, CardPilePosition.Top);
                CustomResourceCost<Scrap> cardCost = CustomResources<Scrap>.Cost(sourceCard.Card);
                if (cardCost != null)
                {
                    cardCost.SetUntilPlayed(0);
                }
                if (IsUpgraded)
                {
                    CardCmd.ApplyKeyword(sourceCard.Card, CardKeyword.Retain);
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

    }

}
