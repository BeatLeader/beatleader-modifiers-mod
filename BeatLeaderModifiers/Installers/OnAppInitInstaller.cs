using JetBrains.Annotations;
using Zenject;

namespace BeatLeaderModifiers.Installers {
    [UsedImplicitly]
    public class OnAppInitInstaller : Installer<OnAppInitInstaller> {
        public override void InstallBindings() {
            Plugin.Log.Debug("OnAppInitInstaller");
        }
    }
}