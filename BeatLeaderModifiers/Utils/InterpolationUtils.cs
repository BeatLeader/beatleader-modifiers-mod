using UnityEngine;

namespace BeatLeaderModifiers;

public static class InterpolationUtils {
    #region FrameData

    public readonly struct FrameData {
        public readonly float Time;
        public readonly Vector3 SaberPosition;
        public readonly Vector3 SaberDirection;
        public readonly Vector3 NotePosition;

        public FrameData(float time, Vector3 saberPosition, Vector3 saberDirection, Vector3 notePosition) {
            Time = time;
            SaberPosition = saberPosition;
            SaberDirection = saberDirection;
            NotePosition = notePosition;
        }

        public static FrameData Lerp(FrameData a, FrameData b, float t) {
            return new FrameData(
                Mathf.LerpUnclamped(a.Time, b.Time, t),
                Vector3.LerpUnclamped(a.SaberPosition, b.SaberPosition, t),
                Vector3.LerpUnclamped(a.SaberDirection, b.SaberDirection, t).normalized,
                Vector3.LerpUnclamped(a.NotePosition, b.NotePosition, t)
            );
        }
    }

    #endregion

    #region CalculateClosestApproach

    private const int Resolution = 200;

    public static void CalculateClosestApproach(FrameData a, FrameData b, out float time, out float distance) {
        time = 0.0f;
        distance = float.MaxValue;

        for (var i = 0; i <= Resolution; i++) {
            var t = (float)i / (Resolution);
            t = -1 + 3 * t;
            
            var f = FrameData.Lerp(a, b, t);
            var d = GetDistance(f.SaberPosition, f.SaberDirection, f.NotePosition);
            if (d >= distance) continue;
            time = f.Time;
            distance = d;
        }
    }

    private static float GetDistance(Vector3 lineFrom, Vector3 lineDirection, Vector3 point) {
        var v = point - lineFrom;
        var angle = Vector3.Angle(v, lineDirection) * Mathf.Deg2Rad;
        return Mathf.Sin(angle) * v.magnitude;
    }

    #endregion
}