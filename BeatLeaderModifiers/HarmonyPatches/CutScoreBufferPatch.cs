using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace BeatLeaderModifiers {
    [HarmonyPatch(typeof(CutScoreBuffer), "cutScore", MethodType.Getter)]
    internal class CutScoreBufferPatch {

        [UsedImplicitly]
        private static bool Prefix(
            ref int __result, 
            int ____afterCutScore,
            int ____beforeCutScore,
            NoteCutInfo ____noteCutInfo) {
            if (StandardLevelBlaBlaPatch.customCharacterisitic == CustomCharacterisitic.betterScoring) {
                int timingScore = Mathf.RoundToInt(Mathf.Max(0.1f - Mathf.Abs(____noteCutInfo.timeDeviation) - 0.02f, 0.0f) * 200);
                int beforeCutScore = Mathf.RoundToInt(((float)____beforeCutScore / 30.0f) * 20);
                int afterCutScore = Mathf.RoundToInt(((float)____afterCutScore / 70.0f) * 20f);
                int centerDistanceCutScore = Mathf.RoundToInt(45f * (1f - Mathf.Clamp01(____noteCutInfo.cutDistanceToCenter / 0.3f)));
                int angleScore = Mathf.RoundToInt((1f - Mathf.Min(Mathf.Abs(____noteCutInfo.cutAngle), 180f) / 180f) * 10f);

                __result = timingScore + beforeCutScore + afterCutScore + centerDistanceCutScore + angleScore;

                return false;
            } else {
                return true; 
            }

        }
    }
}
