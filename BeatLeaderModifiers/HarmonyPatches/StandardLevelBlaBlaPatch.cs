using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using JetBrains.Annotations;
using static PlayerSaveData;

enum CustomCharacterisitic {
    standard = 0,
    betterScoring = 1
}

namespace BeatLeaderModifiers {
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), nameof(StandardLevelScenesTransitionSetupDataSO.Init))]
    class StandardLevelBlaBlaPatch
    {
        public static CustomCharacterisitic customCharacterisitic;
        static void Prefix(StandardLevelScenesTransitionSetupDataSO __instance, string gameMode, IDifficultyBeatmap difficultyBeatmap, IPreviewBeatmapLevel previewBeatmapLevel, OverrideEnvironmentSettings overrideEnvironmentSettings,
            ref GameplayModifiers gameplayModifiers, ColorScheme overrideColorScheme, PlayerSpecificSettings playerSpecificSettings, ref PracticeSettings practiceSettings, string backButtonText, bool useTestNoteCutSoundEffects)
        {
            if (difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName == CharacteristicsManager.BetterScoringCharacteristic.SerializedName) {
                gameplayModifiers = gameplayModifiers.CopyWith(proMode: true, strictAngles: true);

                customCharacterisitic = CustomCharacterisitic.betterScoring;
            } else {
                customCharacterisitic = CustomCharacterisitic.standard;
            }
        }
    }
}
