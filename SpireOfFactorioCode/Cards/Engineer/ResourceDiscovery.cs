using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class ResourceDiscovery : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 1m), new PowerVar<ScrapNextTurnPower>(3m)];

    public ResourceDiscovery() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        var scrap = CustomResources<Scrap>.Get(
            Owner.PlayerCombatState!);
        scrap.ModifyAmount(DynamicVars["Scrap"].IntValue);

        await PowerCmd.Apply<ScrapNextTurnPower>(choiceContext, base.Owner.Creature, base.DynamicVars["ScrapNextTurnPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Scrap"].UpgradeValueBy(1m);
        DynamicVars["ScrapNextTurnPower"].UpgradeValueBy(1m);
    }

}
