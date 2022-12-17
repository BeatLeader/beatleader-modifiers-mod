using JetBrains.Annotations;
using Zenject;

namespace BeatLeaderModifiers.Installers {
    [UsedImplicitly]
    public class OnMenuInstaller : Installer<OnMenuInstaller> {
        public override void InstallBindings() {
            Plugin.Log.Debug("OnMenuInstaller");
        }
    }
}