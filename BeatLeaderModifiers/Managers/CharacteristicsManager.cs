using System.Linq;
using IPA.Utilities;
using SongCore;
using UnityEngine;
using SongCoreUtilities = SongCore.Utilities.Utils;

namespace BeatLeaderModifiers;

internal static class CharacteristicsManager {
    #region Characteritics

    public static readonly CharacteristicDescriptor BetterScoringCharacteristic = new(
        "BeatLeaderModifiers.Resources.TestIcon.png",
        "BetterScoring",
        "BL_BS",
        "useful hint"
    );

    #endregion

    #region RegisterCharacteristics

    public static void RegisterCharacteristics() {
        RegisterCustomCharacteristic(BetterScoringCharacteristic);
    }

    private static void RegisterCustomCharacteristic(CharacteristicDescriptor characteristicDescriptor) {
        Collections.RegisterCustomCharacteristic(
            characteristicDescriptor.Icon,
            characteristicDescriptor.Name,
            characteristicDescriptor.HintText,
            characteristicDescriptor.SerializedName,
            characteristicDescriptor.Name
        );

        characteristicDescriptor.CharacteristicSO = CreateCharacteristicSO(characteristicDescriptor);
    }

    #endregion

    #region CreateCharacteristicSO

    private static BeatmapCharacteristicSO CreateCharacteristicSO(CharacteristicDescriptor characteristicDescriptor) {
        var beatmapCharacteristicSO = ScriptableObject.CreateInstance<BeatmapCharacteristicSO>();

        beatmapCharacteristicSO.SetField("_icon", characteristicDescriptor.Icon);
        beatmapCharacteristicSO.SetField("_characteristicNameLocalizationKey", characteristicDescriptor.Name);
        beatmapCharacteristicSO.SetField("_descriptionLocalizationKey", characteristicDescriptor.HintText);
        beatmapCharacteristicSO.SetField("_serializedName", characteristicDescriptor.SerializedName);
        beatmapCharacteristicSO.SetField("_compoundIdPartName", characteristicDescriptor.Name);
        beatmapCharacteristicSO.SetField("_sortingOrder", 100);
        beatmapCharacteristicSO.SetField("_containsRotationEvents", false);
        beatmapCharacteristicSO.SetField("_requires360Movement", false);
        beatmapCharacteristicSO.SetField("_numberOfColors", 2);

        return beatmapCharacteristicSO;
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
            Icon = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(it => it.name == icon);
        }
    }

    #endregion
}