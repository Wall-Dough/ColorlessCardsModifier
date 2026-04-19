using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace ColorlessCardsModifier.Modifiers;

public class ColorlessCards : ModifierModel
{
    protected override void AfterRunCreated(RunState runState)
    {
        // Add Dingy Rug to all players
        foreach (Player player in runState.Players)
        {
            TaskHelper.RunSafely(RelicCmd.Obtain<DingyRug>(player));
        }
    }
    
    public override IEnumerable<CardModel> ModifyMerchantCardPool(
        Player player,
        IEnumerable<CardModel> options)
    {
        CardPoolModel cardPool = player.Character.CardPool;
        CardModel[] array = options.ToArray();
        return array.Any((Func<CardModel, bool>) (c => c.Pool != cardPool)) ? array :
            array.Concat(ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint));
    }
}