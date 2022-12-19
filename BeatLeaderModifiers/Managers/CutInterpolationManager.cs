using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace BeatLeaderModifiers;

[UsedImplicitly]
internal class CutInterpolationManager : IInitializable, IDisposable, ILateTickable {
    #region Harmony

    private static Harmony _harmony;
    private static CutInterpolationManager _instance;

    private void ApplyPatches() {
        _harmony = new Harmony(nameof(CutInterpolationManager));
        _instance = this;

        _harmony.Patch(NoteWasCutEventPatch);
    }

    private static void RemovePatches() {
        _harmony.UnpatchSelf();
        _harmony = default;
        _instance = default;
    }

    #endregion

    #region NoteWasCutEventPatch

    private static HarmonyPatchDescriptor NoteWasCutEventPatch => new(
        typeof(NoteController).GetMethod("SendNoteWasCutEvent", BindingFlags.Instance | BindingFlags.NonPublic),
        typeof(CutInterpolationManager).GetMethod(nameof(OnSendNoteWasCutEvent), BindingFlags.Static | BindingFlags.NonPublic)
    );

    // ReSharper disable InconsistentNaming
    private static bool OnSendNoteWasCutEvent(
        NoteController __instance,
        NoteCutInfo noteCutInfo,
        LazyCopyHashSet<INoteControllerNoteWasCutEvent> ____noteWasCutEvent
    ) {
        _instance.OnBeforeNoteWasCutEvent(__instance, ref noteCutInfo);
        foreach (var controllerNoteWasCutEvent in ____noteWasCutEvent.items)
            controllerNoteWasCutEvent.HandleNoteControllerNoteWasCut(__instance, in noteCutInfo);
        return false;
    }

    #endregion

    #region Initialize/Dispose

    [Inject, UsedImplicitly]
    private BeatmapObjectManager _beatmapObjectManager;

    [Inject, UsedImplicitly]
    private AudioTimeSyncController _audioTimeSyncController;

    public void Initialize() {
        ApplyPatches();
        _beatmapObjectManager.noteWasSpawnedEvent += OnNoteWasSpawned;
        _beatmapObjectManager.noteWasDespawnedEvent += OnNoteWasDespawned;
    }

    public void Dispose() {
        RemovePatches();
        _beatmapObjectManager.noteWasSpawnedEvent -= OnNoteWasSpawned;
        _beatmapObjectManager.noteWasDespawnedEvent -= OnNoteWasDespawned;
    }

    #endregion

    #region NoteMovementTracker

    private readonly HashSet<NoteController> _notesCache = new(10);
    private readonly Dictionary<NoteController, NoteMovementData> _noteMovementCache = new(10);

    public void LateTick() {
        var songTime = _audioTimeSyncController.songTime;

        foreach (var noteController in _notesCache) {
            _noteMovementCache[noteController] = new NoteMovementData(
                songTime,
                noteController.transform.position
            );
        }
    }

    private void OnNoteWasSpawned(NoteController noteController) {
        _noteMovementCache[noteController] = default;
        _notesCache.Add(noteController);
    }

    private void OnNoteWasDespawned(NoteController noteController) {
        _noteMovementCache.Remove(noteController);
        _notesCache.Remove(noteController);
    }

    private struct NoteMovementData {
        public readonly float SongTime;
        public readonly Vector3 NotePosition;

        public NoteMovementData(float songTime, Vector3 notePosition) {
            SongTime = songTime;
            NotePosition = notePosition;
        }
    }

    #endregion

    #region Events

    private void OnBeforeNoteWasCutEvent(NoteController noteController, ref NoteCutInfo noteCutInfo) {
        if (!_noteMovementCache.ContainsKey(noteController)) return;
        var previousNoteMovementData = _noteMovementCache[noteController];
        var currentNotePosition = noteCutInfo.notePosition;

        var saberMovementData = (SaberMovementData)noteCutInfo.saberMovementData;
        var previousBladeData = saberMovementData.prevAddedData;
        var currentBladeData = saberMovementData.lastAddedData;

        var previousFrameData = new InterpolationUtils.FrameData(
            previousNoteMovementData.SongTime,
            previousBladeData.bottomPos,
            previousBladeData.topPos - previousBladeData.bottomPos,
            previousNoteMovementData.NotePosition
        );

        var currentFrameData = new InterpolationUtils.FrameData(
            _audioTimeSyncController.songTime,
            currentBladeData.bottomPos,
            currentBladeData.topPos - currentBladeData.bottomPos,
            currentNotePosition
        );

        InterpolationUtils.CalculateClosestApproach(previousFrameData, currentFrameData, out var newTime, out var newDistance);
        var newTimeDeviation = noteCutInfo.noteData.time - newTime;

        Plugin.Log.Debug("" + noteCutInfo.timeDeviation + " " + newTimeDeviation);
        Plugin.Log.Debug("" + currentNotePosition.x + " " + currentNotePosition.y + " " + currentNotePosition.z + " " + previousNoteMovementData.NotePosition.x + " " + previousNoteMovementData.NotePosition.y + " " + previousNoteMovementData.NotePosition.z);

        noteCutInfo = new NoteCutInfo(
            noteCutInfo.noteData,
            noteCutInfo.speedOK,
            noteCutInfo.directionOK,
            noteCutInfo.saberTypeOK,
            noteCutInfo.wasCutTooSoon,
            noteCutInfo.saberSpeed,
            noteCutInfo.saberDir,
            noteCutInfo.saberType,
            newTimeDeviation,
            noteCutInfo.cutDirDeviation,
            noteCutInfo.cutPoint,
            noteCutInfo.cutNormal,
            noteCutInfo.cutDistanceToCenter,
            noteCutInfo.cutAngle,
            noteCutInfo.worldRotation,
            noteCutInfo.inverseWorldRotation,
            noteCutInfo.noteRotation,
            noteCutInfo.notePosition,
            noteCutInfo.saberMovementData
        );
    }

    #endregion
}