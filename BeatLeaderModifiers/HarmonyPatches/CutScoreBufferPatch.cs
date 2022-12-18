using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace BeatLeaderModifiers {

    [HarmonyPatch(typeof(CutScoreBuffer), "RefreshScores")]
    internal class CutScoreBufferPatch {

        private static float beforeCutMaxScore = 40f;
        private static float angleMaxScore = 30f;

        private static float goodAngle = 7.5f;
        private static float badAngle = 45f;

        private static float badTiming = 0.1f;
        private static float goodTiming = 0.02f;

        [UsedImplicitly]
        private static void Prefix(
            SaberSwingRatingCounter ____saberSwingRatingCounter,
            int ____beforeCutScore,
            NoteCutInfo ____noteCutInfo) {

            if (StandardLevelBlaBlaPatch.customCharacterisitic == CustomCharacterisitic.betterScoring) {
                
                int beforeCutScore = Mathf.RoundToInt(((float)____beforeCutScore / 70.0f) * beforeCutMaxScore);

                float angleRating = 1f - Mathf.Clamp01((Mathf.Abs(Mathf.Abs(____noteCutInfo.cutAngle) - 90f) - goodAngle) / badAngle);
                int angleScore = Mathf.RoundToInt(angleRating * angleMaxScore);

                Plugin.Log.Debug(beforeCutScore + " " + angleScore);

                float beforeCutScoreRating = (float)(beforeCutScore + angleScore) / 70f;
                
                ____saberSwingRatingCounter.SetField<SaberSwingRatingCounter, float>("_beforeCutRating", Mathf.Min(beforeCutScoreRating, 0.999f));

                float timingRating = 1.0f - Mathf.Clamp01((Mathf.Abs(____noteCutInfo.timeDeviation) - goodTiming) / badTiming);

                ____saberSwingRatingCounter.SetField<SaberSwingRatingCounter, float>("_afterCutRating", Mathf.Min(timingRating, 0.999f));
                Plugin.Log.Debug(timingRating + " ");
            }
        }
    }
}
