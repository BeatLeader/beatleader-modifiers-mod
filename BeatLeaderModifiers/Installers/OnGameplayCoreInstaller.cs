using JetBrains.Annotations;
using Zenject;

namespace BeatLeaderModifiers.Installers {
    [UsedImplicitly]
    public class OnGameplayCoreInstaller : Installer<OnGameplayCoreInstaller> {
        public override void InstallBindings() {
            var difficultyBeatmap = Container.TryResolve<IDifficultyBeatmap>();
            var characteristic = difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic;
            
            if (characteristic.serializedName == CharacteristicsManager.BetterScoringCharacteristic.SerializedName) {
                Container.BindInterfacesTo<CutInterpolationManager>().AsSingle();
            }
        }
    }
}