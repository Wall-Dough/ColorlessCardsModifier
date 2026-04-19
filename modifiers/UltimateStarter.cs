using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace ColorlessCardsModifier.modifiers;

public class UltimateStarter : ModifierModel
{
    public override bool ClearsPlayerDeck => true;
    
    public override LocString Description
    {
        get
        {
            LocString description = new LocString("modifiers", Id.Entry + ".description");
            description.Add("Card1", ModelDb.Card<UltimateStrike>().TitleLocString);
            description.Add("Card2", ModelDb.Card<UltimateDefend>().TitleLocString);
            return description;
        }
    }
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return (Func<Task>) (() => ObtainCards(eventModel.Owner, eventModel.Rng));
    }

    private static async Task PreviewCardResults(List<CardPileAddResult> results)
    {
        CardCmd.PreviewCardPileAdd(results);
        await Cmd.CustomScaledWait(0.6f, 1.2f);
        results.Clear();
    }

    private static async Task ObtainCards(Player player, Rng rng)
    {
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        results.Add(await CardPileCmd.Add(player.RunState.CreateCard<UltimateStrike>(player), PileType.Deck));
        results.Add(await CardPileCmd.Add(player.RunState.CreateCard<UltimateDefend>(player), PileType.Deck));
        await PreviewCardResults(results);
        
        List<CardPoolModel> cardPools = new List<CardPoolModel>();
        cardPools.Add(ModelDb.CardPool<ColorlessCardPool>());
        IReadOnlyList<CardPoolModel> readOnlyCardPools = cardPools.AsReadOnly();
        
        for (int i = 0; i < 8; ++i)
        {
            CardCreationOptions cardCreationOptions = CardCreationOptions.ForNonCombatWithUniformOdds(readOnlyCardPools)
                .WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);
            results.Add(await CardPileCmd.Add(
                CardFactory.CreateForReward(player, 1, cardCreationOptions).First().Card, PileType.Deck));
            if (results.Count >= 4)
            {
                await PreviewCardResults(results);
            }
        }
        
        if (results.Count > 0)
        {
            await PreviewCardResults(results);
        }
        results = (List<CardPileAddResult>) null;
    }
}