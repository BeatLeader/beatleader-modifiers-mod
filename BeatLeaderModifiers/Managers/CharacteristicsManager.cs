using System.Linq;
using IPA.Utilities;
using SongCore;
using UnityEngine;
using SongCoreUtilities = SongCore.Utilities.Utils;

namespace BeatLeaderModifiers;

internal static class CharacteristicsManager {
    #region Characteritics

    public static readonly CharacteristicDescriptor BetterScoringCharacteristic = new(
        "BeatLeaderModifiers.Icons.RhythmGame.png",
        "RhythmGameStandard",
        "RhythmGameStandard",
        "It's a rhythm game!"
    );

    #endregion

    #region RegisterCharacteristics

    public static void RegisterCharacteristics() {
        RegisterCustomCharacteristic(BetterScoringCharacteristic);
    }

    private static void RegisterCustomCharacteristic(CharacteristicDescriptor characteristicDescriptor) {
        characteristicDescriptor.CharacteristicSO = Collections.RegisterCustomCharacteristic(
            characteristicDescriptor.Icon,
            characteristicDescriptor.Name,
            characteristicDescriptor.HintText,
            characteristicDescriptor.SerializedName,
            characteristicDescriptor.Name
        );
    }

    #endregion

    #region CharacteristicDescriptor

    public class CharacteristicDescriptor {
        public readonly Sprite Icon;
        public readonly string Name;
        public readonly string SerializedName;
        public readonly string HintText;
        public BeatmapCharacteristicSO CharacteristicSO = default;

        public CharacteristicDescriptor(string icon, string name, string serializedName, string hintText) {
            Name = name;
            SerializedName = serializedName;
            HintText = hintText;
            
            //TODO: AssetBundle?
            Icon = SongCoreUtilities.LoadSpriteFromResources(icon);
        }
    }

    #endregion
}