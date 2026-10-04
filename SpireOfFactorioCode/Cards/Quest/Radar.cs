using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Quest;

[Pool(typeof(QuestCardPool))]
public sealed class Radar : SpireOfFactorioCard
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public Radar() : base(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
    {

    }

}

