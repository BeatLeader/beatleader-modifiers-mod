using JetBrains.Annotations;
using Zenject;

namespace BeatLeaderModifiers.Installers {
    [UsedImplicitly]
    public class OnGameplayCoreInstaller : Installer<OnGameplayCoreInstaller> {
        public override void InstallBindings() {
            Plugin.Log.Debug("OnGameplayCoreInstaller");
        }
    }
}