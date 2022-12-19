using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using JetBrains.Annotations;

namespace BeatLeaderModifiers {
    [HarmonyPatch(typeof(StandardLevelDetailView), "SetContent")]
    internal class SetContentPatch {
        #region Prefix

        [UsedImplicitly]
        private static void Prefix(IBeatmapLevel level) {
            if (!level.levelID.StartsWith("custom_level")) return;

            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x =>
                    x.beatmapCharacteristic.serializedName.Equals(CharacteristicsManager.BetterScoringCharacteristic.SerializedName))) {
                return; //TODO: Filter all custom characteristics
            }

            AddCustomCharacteristic(level);
        }

        #endregion

        #region AddCustomCharacteristic

        private static async void AddCustomCharacteristic(IBeatmapLevel level) {
            var characteristicSO = CharacteristicsManager.BetterScoringCharacteristic.CharacteristicSO;

            var difficultyBeatmapSets = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);

            foreach (var originalBeatmapSet in level.beatmapLevelData.difficultyBeatmapSets) {
                if (originalBeatmapSet.beatmapCharacteristic.serializedName != "Standard") continue;

                var beatmapSet = new CustomDifficultyBeatmapSet(characteristicSO);
                var customDifficulties = await CreateCustomDifficulties(originalBeatmapSet.difficultyBeatmaps, beatmapSet);
                beatmapSet.SetCustomDifficultyBeatmaps(customDifficulties);

                if (beatmapSet.difficultyBeatmaps.Count > 0) {
                    difficultyBeatmapSets.Add(beatmapSet);
                }
            }

            ((BeatmapLevelData)level.beatmapLevelData).SetField<BeatmapLevelData, IReadOnlyList<IDifficultyBeatmapSet>>("_difficultyBeatmapSets", difficultyBeatmapSets);
        }

        #endregion

        #region CreateCustomDifficulties

        private static async Task<CustomDifficultyBeatmap[]> CreateCustomDifficulties(
            IEnumerable<IDifficultyBeatmap> difficultyBeatmaps,
            IDifficultyBeatmapSet beatmapSet
        ) {
            var customDifficulties = new List<CustomDifficultyBeatmap>();

            foreach (var difficultyBeatmap in difficultyBeatmaps) {
                var beatmapDataBasicInfo = await difficultyBeatmap.GetBeatmapDataBasicInfoAsync();
                var customBeatmap = new CustomDifficultyBeatmap(
                    difficultyBeatmap.level,
                    beatmapSet,
                    difficultyBeatmap.difficulty,
                    difficultyBeatmap.difficultyRank,
                    difficultyBeatmap.noteJumpMovementSpeed,
                    difficultyBeatmap.noteJumpStartBeatOffset,
                    difficultyBeatmap.level.beatsPerMinute,
                    ((CustomDifficultyBeatmap)difficultyBeatmap).beatmapSaveData,
                    beatmapDataBasicInfo
                );

                customDifficulties.Add(customBeatmap);
            }

            return customDifficulties.ToArray();
        }
    }

    #endregion
}