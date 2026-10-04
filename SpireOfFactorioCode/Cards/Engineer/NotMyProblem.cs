using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class NotMyProblem : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("ExhaustCount", 2m), new DynamicVar("Scrap", 1m), new EnergyVar(1)];

    public NotMyProblem() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (CardModel item in await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.DynamicVars["ExhaustCount"].IntValue), context: choiceContext, player: base.Owner, filter: null, source: this))
        {
            var scrap = CustomResources<Scrap>.Get(base.Owner.PlayerCombatState!);
            if (!item.EnergyCost.CostsX && item.EnergyCost.Canonical > 0)
            {
                scrap.ModifyAmount(item.EnergyCost.Canonical);
            }
            await CardCmd.Exhaust(choiceContext, item);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExhaustCount"].UpgradeValueBy(1m);
    }

}
