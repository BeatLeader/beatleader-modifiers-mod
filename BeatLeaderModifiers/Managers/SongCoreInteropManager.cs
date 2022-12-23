using System.Linq;
using System.Reflection;
using HarmonyLib;
using IPA.Loader;
using SongCore;
using SongCore.Data;
using SongCore.Utilities;

namespace BeatLeaderModifiers;

internal static class SongCoreInteropManager {
    private static Harmony _harmony;

    public static void ApplyPatches() {
        var plugin = PluginManager.GetPluginFromId("SongCore");
        if (plugin == null) return;

        var assembly = plugin.Assembly;
        var swingRatingEnhancerType = assembly.GetType("SongCore.Collections");

        var patch = new HarmonyPatchDescriptor(
            swingRatingEnhancerType.GetMethod("RetrieveDifficultyData", BindingFlags.Static | BindingFlags.Public),
            typeof(SongCoreInteropManager).GetMethod(nameof(RetrieveDifficultyData), BindingFlags.Static | BindingFlags.Public)
        );

        _harmony = new Harmony(nameof(SongCoreInteropManager));
        _harmony.Patch(patch);
    }

    public static bool RetrieveDifficultyData(IDifficultyBeatmap beatmap, ref ExtraSongData.DifficultyData? __result) {
        ExtraSongData? songData = null;

        if (beatmap.level is CustomPreviewBeatmapLevel customLevel)
        {
            songData = Collections.RetrieveExtraSongData(Hashing.GetCustomLevelHash(customLevel));
        }

        __result = songData?._difficulties.FirstOrDefault(x =>
            x._difficulty == beatmap.difficulty && 
            (x._beatmapCharacteristicName == beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.characteristicNameLocalizationKey ||
             x._beatmapCharacteristicName == beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName ||
             (x._beatmapCharacteristicName == "Standard" && beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName == CharacteristicsManager.BetterScoringCharacteristic.SerializedName)));

        return false;
    }
}