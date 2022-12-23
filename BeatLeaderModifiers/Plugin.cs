using BeatLeaderModifiers.Installers;
using IPA;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace BeatLeaderModifiers {
    [Plugin(RuntimeOptions.SingleStartInit)]
    [UsedImplicitly]
    public class Plugin {
        #region Init

        internal static IPALogger Log { get; private set; }

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector) {
            Log = logger;
            zenjector.Install<OnAppInitInstaller>(Location.App);
            zenjector.Install<OnMenuInstaller>(Location.Menu);
            zenjector.Install<OnGameplayCoreInstaller>(Location.GameCore);
        }

        #endregion

        #region OnApplicationStart

        [OnStart, UsedImplicitly]
        public void OnApplicationStart() {
            HarmonyHelper.ApplyPatches();
            BeatLeaderInteropManager.ApplyPatches();
            SongCoreInteropManager.ApplyPatches();
            CharacteristicsManager.RegisterCharacteristics();
        }

        #endregion

        #region OnApplicationQuit

        [OnExit, UsedImplicitly]
        public void OnApplicationQuit() { }

        #endregion
    }
}