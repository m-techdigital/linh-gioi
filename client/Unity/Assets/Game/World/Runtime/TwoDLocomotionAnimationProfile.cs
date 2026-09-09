using System;
using System.Text;

namespace LinhGioi.World
{
    public sealed class TwoDLocomotionAnimationProfile
    {
        private TwoDLocomotionAnimationProfile(string[] states)
        {
            States = states;
            Snapshot = BuildSnapshot(states);
        }

        public string[] States { get; }
        public string Snapshot { get; }

        public static TwoDLocomotionAnimationProfile CreateDefault()
        {
            return new TwoDLocomotionAnimationProfile(new[]
            {
                "Idle",
                "Walk",
                "Run",
                "Jump",
                "Dash",
                "ClassSkill",
                "TrainingCompletePose"
            });
        }

        private static string BuildSnapshot(string[] states)
        {
            var builder = new StringBuilder("LayeredCharacter Animation Profile | locomotion=side_scroll");
            builder.Append(" | rig=2d_skeletal_sprite_swap | states=");
            for (var i = 0; i < states.Length; i++)
            {
                if (i > 0) builder.Append(",");
                builder.Append(states[i]);
            }
            return builder.ToString();
        }
    }

    [Serializable]
    public readonly struct TwoDAnimationRuntimeState
    {
        public TwoDAnimationRuntimeState(string state, string pose, float phase)
        {
            State = state;
            Pose = pose;
            Phase = phase;
        }

        public string State { get; }
        public string Pose { get; }
        public float Phase { get; }

        public string Snapshot => "animation=" + State + " | pose=" + Pose + " | phase=" + Phase.ToString("0.00");
    }
}
