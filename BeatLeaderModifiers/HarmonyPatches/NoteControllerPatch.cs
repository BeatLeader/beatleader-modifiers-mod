using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace BeatLeaderModifiers {

    [HarmonyPatch(typeof(GameNoteController), "Init")]
    internal class NoteControllerPatch {
        private static float colliderScale = 0.58f;

        static void ScaleColider(BoxCuttableBySaber box, float value) {
            var localScale = box.transform.localScale;

            localScale.x *= value;
            localScale.y *= value;
            localScale.z *= value;

            box.transform.localScale = localScale;
        }

        [UsedImplicitly]
        private static void Postfix(
            BoxCuttableBySaber[] ____bigCuttableBySaberList,
            BoxCuttableBySaber[] ____smallCuttableBySaberList) {

            if (StandardLevelBlaBlaPatch.customCharacterisitic == CustomCharacterisitic.betterScoring) {
                foreach (var item in ____bigCuttableBySaberList)
                {
                    ScaleColider(item, colliderScale);
                }
                foreach (var item in ____smallCuttableBySaberList)
                {
                    ScaleColider(item, colliderScale);
                }
            }
        }
    }
}
