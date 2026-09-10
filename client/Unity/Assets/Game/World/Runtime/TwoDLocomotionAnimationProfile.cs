using System;
using System.Text;

namespace LinhGioi.World
{
    public sealed class TwoDLocomotionAnimationProfile
    {
        private TwoDLocomotionAnimationProfile(string[] states, TwoDAnimationClipFrame[] frames)
        {
            States = states;
            Frames = frames;
            FrameCount = frames.Length;
            Snapshot = BuildSnapshot(states, frames);
        }

        public string[] States { get; }
        public TwoDAnimationClipFrame[] Frames { get; }
        public int FrameCount { get; }
        public string Snapshot { get; }

        public static TwoDLocomotionAnimationProfile CreateDefault()
        {
            return new TwoDLocomotionAnimationProfile(
                new[]
                {
                    "Idle",
                    "Walk",
                    "Run",
                    "Jump",
                    "Dash",
                    "ClassSkill",
                    "TrainingCompletePose"
                },
                new[]
                {
                    new TwoDAnimationClipFrame("Idle", "vo_idle_01", "vo_idle", 0.18f),
                    new TwoDAnimationClipFrame("Idle", "vo_idle_02", "vo_idle", 0.18f),
                    new TwoDAnimationClipFrame("Idle", "vo_idle_03", "vo_idle", 0.18f),
                    new TwoDAnimationClipFrame("Walk", "vo_walk_01", "vo_idle", 0.12f),
                    new TwoDAnimationClipFrame("Walk", "vo_walk_02", "vo_idle", 0.12f),
                    new TwoDAnimationClipFrame("Walk", "vo_walk_03", "vo_idle", 0.12f),
                    new TwoDAnimationClipFrame("Jump", "vo_jump_01", "vo_jump_lift", 0.10f),
                    new TwoDAnimationClipFrame("Jump", "vo_jump_02", "vo_jump_lift", 0.10f),
                    new TwoDAnimationClipFrame("Jump", "vo_jump_03", "vo_jump_lift", 0.10f),
                    new TwoDAnimationClipFrame("Dash", "vo_dash_01", "vo_dash_stretch", 0.08f),
                    new TwoDAnimationClipFrame("Dash", "vo_dash_02", "vo_dash_stretch", 0.08f),
                    new TwoDAnimationClipFrame("Dash", "vo_dash_03", "vo_dash_stretch", 0.08f),
                    new TwoDAnimationClipFrame("ClassSkill", "vo_skill_01", "vo_skill_cast", 0.07f),
                    new TwoDAnimationClipFrame("ClassSkill", "vo_skill_02", "vo_skill_cast", 0.07f),
                    new TwoDAnimationClipFrame("ClassSkill", "vo_skill_03", "vo_skill_cast", 0.07f),
                    new TwoDAnimationClipFrame("ClassSkill", "vo_skill_04", "vo_skill_cast", 0.07f),
                    new TwoDAnimationClipFrame("TrainingCompletePose", "vo_complete_01", "vo_lv1_training_complete", 0.20f)
                });
        }

        public TwoDAnimationClipFrame Sample(string state, float phase)
        {
            var count = CountFrames(state);
            if (count <= 0) return new TwoDAnimationClipFrame(state, "missing", "vo_idle", 0f);
            var normalized = Math.Max(0f, Math.Min(0.999f, phase));
            var target = Math.Min(count - 1, (int)(normalized * count));
            var seen = 0;
            for (var i = 0; i < Frames.Length; i++)
            {
                if (Frames[i].Clip != state) continue;
                if (seen == target) return Frames[i];
                seen++;
            }
            return Frames[0];
        }

        private int CountFrames(string state)
        {
            var count = 0;
            for (var i = 0; i < Frames.Length; i++)
                if (Frames[i].Clip == state) count++;
            return count;
        }

        private static string BuildSnapshot(string[] states, TwoDAnimationClipFrame[] frames)
        {
            var builder = new StringBuilder("LayeredCharacter Animation Profile | locomotion=side_scroll");
            builder.Append(" | rig=2d_skeletal_sprite_swap | states=");
            for (var i = 0; i < states.Length; i++)
            {
                if (i > 0) builder.Append(",");
                builder.Append(states[i]);
            }
            builder.Append(" | anchorPolicy=paper-doll-follows-frame-pose");
            builder.Append(" | clips=");
            AppendClipSummary(builder, frames, "Idle");
            builder.Append(',');
            AppendClipSummary(builder, frames, "Jump");
            builder.Append(',');
            AppendClipSummary(builder, frames, "Dash");
            builder.Append(',');
            AppendClipSummary(builder, frames, "ClassSkill");
            for (var i = 0; i < frames.Length; i++)
                builder.Append(" | frame=").Append(frames[i].FrameId).Append(" pose=").Append(frames[i].PoseId);
            return builder.ToString();
        }

        private static void AppendClipSummary(StringBuilder builder, TwoDAnimationClipFrame[] frames, string clip)
        {
            var count = 0;
            var duration = 0f;
            for (var i = 0; i < frames.Length; i++)
            {
                if (frames[i].Clip != clip) continue;
                count++;
                duration = frames[i].DurationSeconds;
            }
            builder.Append(clip).Append(':').Append(count).Append('@').Append(duration.ToString("0.00"));
        }
    }

    public readonly struct TwoDAnimationClipFrame
    {
        public TwoDAnimationClipFrame(string clip, string frameId, string poseId, float durationSeconds)
        {
            Clip = clip;
            FrameId = frameId;
            PoseId = poseId;
            DurationSeconds = durationSeconds;
        }

        public string Clip { get; }
        public string FrameId { get; }
        public string PoseId { get; }
        public float DurationSeconds { get; }
    }

    [Serializable]
    public readonly struct TwoDAnimationRuntimeState
    {
        public TwoDAnimationRuntimeState(string state, string pose, float phase)
            : this(state, pose, phase, string.Empty, string.Empty, string.Empty)
        {
        }

        public TwoDAnimationRuntimeState(string state, string pose, float phase, string motionFrame, string paperDollPose, string motionClip)
        {
            State = state;
            Pose = pose;
            Phase = phase;
            MotionFrame = motionFrame;
            PaperDollPose = paperDollPose;
            MotionClip = motionClip;
        }

        public string State { get; }
        public string Pose { get; }
        public float Phase { get; }
        public string MotionFrame { get; }
        public string PaperDollPose { get; }
        public string MotionClip { get; }

        public string Snapshot
        {
            get
            {
                var snapshot = "animation=" + State + " | pose=" + Pose + " | phase=" + Phase.ToString("0.00");
                if (!string.IsNullOrEmpty(MotionFrame)) snapshot += " | motionClip=" + MotionClip + " | motionFrame=" + MotionFrame + " | paperDollPose=" + PaperDollPose;
                return snapshot;
            }
        }
    }
}
