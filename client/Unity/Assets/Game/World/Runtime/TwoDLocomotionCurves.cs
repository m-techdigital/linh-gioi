using UnityEngine;

namespace LinhGioi.World
{
    // Shared draft timing/targets; all outfits use the same character-space curves.
    public static class TwoDLocomotionCurves
    {
        public static float SomersaultDegrees(float progress) => -360f * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.03f, .40f, progress));
        public static float Tuck(float progress) => Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, .12f, progress))
            * (1 - Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.7f, 1, progress)));
        public static Vector2 FootOffset(bool run, float phase)
        {
            phase = Mathf.Repeat(phase, 1);
            var stride = run ? .4f : .3f;
            if (phase < .5f) return new Vector2(Mathf.Lerp(stride, -stride, phase * 2), 0);
            var swing = (phase - .5f) * 2;
            if (!run) return new Vector2(Mathf.Lerp(-stride, stride, swing), Mathf.Sin(swing * Mathf.PI) * .1f);
            // Toe-off -> heel recovery -> knee forward -> contact. Stance remains
            // linear so foot travel matches the actor's 2.4 world units/second at the shared run cadence.
            var back = new Vector2(-stride, 0);
            var recovery = new Vector2(-.25f, .34f);
            var forward = new Vector2(.12f, .28f);
            var contact = new Vector2(stride, 0);
            if (swing < .3f) return Vector2.Lerp(back, recovery, Mathf.SmoothStep(0, 1, swing / .3f));
            if (swing < .6f) return Vector2.Lerp(recovery, forward, Mathf.SmoothStep(0, 1, (swing - .3f) / .3f));
            return Vector2.Lerp(forward, contact, Mathf.SmoothStep(0, 1, (swing - .6f) / .4f));
        }
    }
}
