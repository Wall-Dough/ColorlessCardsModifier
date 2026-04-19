using ColorlessCardsModifier.modifiers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace ColorlessCardsModifier.ColorlessCardsModifierCode;

//You're recommended but not required to keep all your code in this package and all your assets in the ColorlessCardsModifier folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string
        ModId = "ColorlessCardsModifier"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }

    [HarmonyPatch]
    public static class ColorlessCardsModifier
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NCustomRunModifiersList))]
        [HarmonyPatch("GetAllModifiers")]
        private static void AfterGetAllModifiers(ref IEnumerable<ModifierModel> __result)
        {
            __result = __result.AddItem(ModelDb.Modifier<ColorlessCards>().ToMutable());
        }
    }
}