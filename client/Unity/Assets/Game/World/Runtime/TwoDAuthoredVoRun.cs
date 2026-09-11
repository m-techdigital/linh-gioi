using UnityEngine;

namespace LinhGioi.World
{
    // Male v5 source authoring landmarks: unchanged base lengths, four distinct beats.
    // Hip in 1024x1536 source coordinates; angles relative to the registered bind vectors.
    public static class TwoDAuthoredVoRun
    {
        public readonly struct Leg
        {
            public readonly Vector2 Hip;
            public readonly float ThighDegrees, ShinDegrees;
            public Leg(Vector2 hip, float thigh, float shin)
            { Hip = hip; ThighDegrees = thigh; ShinDegrees = shin; }
            public static Leg Blend(Leg a, Leg b, float t) => new Leg(Vector2.Lerp(a.Hip, b.Hip, t),
                Mathf.LerpAngle(a.ThighDegrees, b.ThighDegrees, t), Mathf.LerpAngle(a.ShinDegrees, b.ShinDegrees, t));
        }
        public readonly struct Pose
        {
            public readonly Leg Left, Right;
            public Pose(Leg left, Leg right) { Left = left; Right = right; }
            public static Pose Blend(Pose a, Pose b, float t) => new Pose(Leg.Blend(a.Left, b.Left, t), Leg.Blend(a.Right, b.Right, t));
        }
        private static readonly Pose[] Loop = {
            new Pose(new Leg(new Vector2(485.000000f, 795.000000f), 37.475776f, -13.819416f), new Leg(new Vector2(595.000000f, 795.000000f), 68.699343f, -36.070130f)), // run_contact_a
            new Pose(new Leg(new Vector2(485.000000f, 795.000000f), 0.862185f, -52.032821f), new Leg(new Vector2(595.000000f, 795.000000f), 66.089125f, -19.874640f)), // run_a
            new Pose(new Leg(new Vector2(485.000000f, 795.000000f), 92.363257f, 4.691379f), new Leg(new Vector2(595.000000f, 795.000000f), 15.459539f, -28.496515f)), // run_contact_b
            new Pose(new Leg(new Vector2(485.000000f, 795.000000f), 87.442836f, 16.005200f), new Leg(new Vector2(595.000000f, 795.000000f), -21.904464f, -69.601148f)), // run_b
        };
        public static readonly Pose Entry = new Pose(new Leg(new Vector2(456.092620f, 809.604415f), 27.284910f, -26.372927f), new Leg(new Vector2(563.688856f, 832.474701f), 32.699644f, -23.560050f));
        public static readonly Pose Exit = new Pose(new Leg(new Vector2(455.190265f, 785.642213f), 27.041170f, -18.790051f), new Leg(new Vector2(564.771682f, 795.229345f), 21.703974f, -17.746683f));
        public static Pose Sample(float cycle)
        {
            var phase = Mathf.Repeat(cycle, 1) * 4;
            var index = Mathf.FloorToInt(phase);
            return Pose.Blend(Loop[index], Loop[(index + 1) % 4], Mathf.SmoothStep(0, 1, phase - index));
        }
    }
}
