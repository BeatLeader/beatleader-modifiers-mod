using System.Reflection;
using HarmonyLib;
using IPA.Loader;

namespace BeatLeaderModifiers;

internal static class BeatLeaderInteropManager {
    private static Harmony _harmony;

    public static void ApplyPatches() {
        var plugin = PluginManager.GetPluginFromId("BeatLeader");
        if (plugin == null) return;

        var assembly = plugin.Assembly;
        var swingRatingEnhancerType = assembly.GetType("BeatLeader.Core.Managers.NoteEnhancer.SwingRatingEnhancer");

        var patch = new HarmonyPatchDescriptor(
            swingRatingEnhancerType.GetMethod("ChooseSwingRating", BindingFlags.Static | BindingFlags.NonPublic),
            typeof(BeatLeaderInteropManager).GetMethod(nameof(ChooseSwingRating), BindingFlags.Static | BindingFlags.NonPublic)
        );

        _harmony = new Harmony(nameof(BeatLeaderInteropManager));
        _harmony.Patch(patch);
    }

    private static bool ChooseSwingRating(float real, float unclamped, ref float __result) {
        if (StandardLevelBlaBlaPatch.customCharacterisitic != CustomCharacterisitic.betterScoring) return true;
        __result = real;
        return false;
    }
}