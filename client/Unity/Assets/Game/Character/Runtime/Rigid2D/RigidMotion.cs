using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinhGioi.Character
{
    public enum RigidMotionState { Idle, Walk, Run, Jump, Attack, Roll, Hit }

    public readonly struct RigidBonePose
    {
        public RigidBonePose(Vector2 positionOffset, float rotationDegrees)
        {
            PositionOffset = positionOffset;
            RotationDegrees = rotationDegrees;
        }

        public Vector2 PositionOffset { get; }
        public float RotationDegrees { get; }
    }

    public sealed class RigidMotionPose
    {
        public RigidMotionPose(Vector2 rootOffset, IReadOnlyDictionary<RigidBoneId, RigidBonePose> bones)
        {
            RootOffset = rootOffset;
            Bones = bones ?? throw new ArgumentNullException(nameof(bones));
        }

        public Vector2 RootOffset { get; }
        public IReadOnlyDictionary<RigidBoneId, RigidBonePose> Bones { get; }
    }

    public static class RigidMotionLibrary
    {
        private readonly struct LegPose
        {
            public LegPose(float upper, float lower, float foot, float targetX)
            {
                Upper = upper;
                Lower = lower;
                Foot = foot;
                TargetX = targetX;
            }

            public float Upper { get; }
            public float Lower { get; }
            public float Foot { get; }
            public float TargetX { get; }
        }

        public static RigidMotionPose Sample(RigidMotionState state, float normalizedTime)
        {
            var t = Mathf.Repeat(normalizedTime, 1f);
            return state switch
            {
                RigidMotionState.Idle => Idle(t),
                RigidMotionState.Walk => Locomotion(t, false),
                RigidMotionState.Run => Locomotion(t, true),
                RigidMotionState.Jump => Jump(t),
                RigidMotionState.Attack => Attack(t),
                RigidMotionState.Roll => Roll(t),
                RigidMotionState.Hit => Hit(t),
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            };
        }

        public static RigidMotionPose Blend(RigidMotionPose from, RigidMotionPose to, float amount)
        {
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));
            amount = Mathf.Clamp01(amount);
            var bones = new Dictionary<RigidBoneId, RigidBonePose>();
            foreach (var bone in from.Bones.Keys.Concat(to.Bones.Keys).Distinct())
            {
                var fromPose = from.Bones.TryGetValue(bone, out var start)
                    ? start : new RigidBonePose(Vector2.zero, 0f);
                var toPose = to.Bones.TryGetValue(bone, out var end)
                    ? end : new RigidBonePose(Vector2.zero, 0f);
                bones[bone] = new RigidBonePose(
                    Vector2.Lerp(fromPose.PositionOffset, toPose.PositionOffset, amount),
                    Mathf.LerpAngle(fromPose.RotationDegrees, toPose.RotationDegrees, amount));
            }
            return new RigidMotionPose(Vector2.Lerp(from.RootOffset, to.RootOffset, amount), bones);
        }

        private static RigidMotionPose Idle(float t)
        {
            var breath = Mathf.Sin(t * Mathf.PI * 2f);
            return Pose(new Vector2(0f, breath * .018f),
                (RigidBoneId.Head, 1.2f * breath),
                (RigidBoneId.UpperArmL, -3f + breath),
                (RigidBoneId.UpperArmR, 3f - breath));
        }

        private static RigidMotionPose Locomotion(float t, bool running)
        {
            // Each foot follows one shared trajectory. During stance its target stays on the
            // ground line; two-bone IK derives hip/knee angles from canonical limb lengths.
            var stride = running ? .66f : .34f;
            var lift = running ? .68f : .30f;
            var rootY = (running ? -.22f : -.10f) +
                (running ? .075f : .04f) * (.5f - .5f * Mathf.Cos(t * Mathf.PI * 4f));
            var near = GaitLeg(t, stride, lift, rootY);
            var far = GaitLeg(Mathf.Repeat(t + .5f, 1f), stride, lift, rootY);
            var lean = running ? -15f : -4f;
            var armRange = running ? 40f : 18f;
            return Pose(new Vector2(0f, rootY),
                (RigidBoneId.Torso, lean),
                (RigidBoneId.Head, -lean * .35f),
                (RigidBoneId.UpperArmL, -near.TargetX / stride * armRange),
                (RigidBoneId.LowerArmL, running ? Cyclic(t, 48f, 72f, 82f, 56f) : Cyclic(t, 18f, 30f, 34f, 22f)),
                (RigidBoneId.WristL, running ? Cyclic(t, -10f, -18f, -8f, -14f) : -5f),
                (RigidBoneId.UpperArmR, -far.TargetX / stride * armRange),
                (RigidBoneId.LowerArmR, running ? Cyclic(t, 82f, 56f, 48f, 72f) : Cyclic(t, 34f, 22f, 18f, 30f)),
                (RigidBoneId.WristR, running ? Cyclic(t, -8f, -14f, -10f, -18f) : -5f),
                (RigidBoneId.UpperLegL, near.Upper),
                (RigidBoneId.LowerLegL, near.Lower),
                (RigidBoneId.FootL, near.Foot),
                (RigidBoneId.UpperLegR, far.Upper),
                (RigidBoneId.LowerLegR, far.Lower),
                (RigidBoneId.FootR, far.Foot));
        }

        private static RigidMotionPose Jump(float t)
        {
            // Anticipation -> takeoff -> apex -> descent -> absorbed landing.
            var rootY = Keyed(t,
                (0f, 0f), (.14f, -.42f), (.28f, .24f), (.52f, .82f), (.76f, .30f), (.90f, -.34f), (1f, 0f));
            var crouch = Keyed(t,
                (0f, 0f), (.14f, 1f), (.28f, .20f), (.52f, 0f), (.76f, .28f), (.90f, .86f), (1f, 0f));
            var airborne = Keyed(t,
                (0f, 0f), (.14f, 0f), (.28f, .55f), (.52f, 1f), (.76f, .62f), (.90f, 0f), (1f, 0f));
            var groundY = .435f - (2.30f + rootY);
            var near = SolveLeg(
                Keyed(t, (0f, 0f), (.14f, .31f), (.28f, .18f), (.52f, .29f), (.76f, .12f), (.90f, -.27f), (1f, 0f)),
                Mathf.Lerp(groundY, -1.10f, airborne), -8f * airborne);
            var far = SolveLeg(
                Keyed(t, (0f, 0f), (.14f, -.27f), (.28f, -.12f), (.52f, -.12f), (.76f, .22f), (.90f, .27f), (1f, 0f)),
                Mathf.Lerp(groundY, -1.02f, airborne), -12f * airborne);
            return Pose(new Vector2(Keyed(t, (0f, 0f), (.28f, .04f), (.52f, .16f), (.76f, .29f), (1f, .34f)), rootY),
                (RigidBoneId.Torso, -8f * crouch + 5f * airborne),
                (RigidBoneId.Head, 3f * crouch - 2f * airborne),
                (RigidBoneId.UpperArmL, -24f * crouch + 48f * airborne),
                (RigidBoneId.LowerArmL, 40f * crouch + 24f * airborne),
                (RigidBoneId.WristL, -12f * airborne),
                (RigidBoneId.UpperArmR, -18f * crouch + 38f * airborne),
                (RigidBoneId.LowerArmR, 34f * crouch + 30f * airborne),
                (RigidBoneId.WristR, -10f * airborne),
                (RigidBoneId.UpperLegL, near.Upper),
                (RigidBoneId.LowerLegL, near.Lower),
                (RigidBoneId.FootL, near.Foot),
                (RigidBoneId.UpperLegR, far.Upper),
                (RigidBoneId.LowerLegR, far.Lower),
                (RigidBoneId.FootR, far.Foot));
        }

        private static RigidMotionPose Attack(float t)
        {
            var weaponArm = Keyed(t, (0f, 0f), (.24f, -46f), (.34f, 18f), (.44f, 88f), (.58f, 72f), (1f, 0f));
            var supportArm = Keyed(t, (0f, 0f), (.24f, -20f), (.44f, 58f), (.64f, 32f), (1f, 0f));
            var drive = Keyed(t, (0f, 0f), (.24f, -.04f), (.44f, .16f), (.68f, .10f), (1f, 0f));
            return Pose(new Vector2(drive, 0f),
                (RigidBoneId.Torso, Keyed(t, (0f, 0f), (.24f, 10f), (.44f, -17f), (.68f, -8f), (1f, 0f))),
                (RigidBoneId.Head, Keyed(t, (0f, 0f), (.24f, -4f), (.44f, 6f), (1f, 0f))),
                (RigidBoneId.UpperArmL, weaponArm),
                (RigidBoneId.LowerArmL, Keyed(t, (0f, 0f), (.24f, 54f), (.44f, -24f), (.68f, -10f), (1f, 0f))),
                (RigidBoneId.WristL, Keyed(t, (0f, 0f), (.24f, -18f), (.44f, 8f), (1f, 0f))),
                (RigidBoneId.UpperArmR, supportArm),
                (RigidBoneId.LowerArmR, Keyed(t, (0f, 0f), (.24f, 42f), (.44f, -18f), (1f, 0f))),
                (RigidBoneId.UpperLegL, Keyed(t, (0f, 0f), (.24f, -14f), (.44f, 20f), (1f, 0f))),
                (RigidBoneId.LowerLegL, Keyed(t, (0f, 0f), (.24f, 30f), (.44f, 10f), (1f, 0f))),
                (RigidBoneId.UpperLegR, Keyed(t, (0f, 0f), (.24f, 18f), (.44f, -22f), (1f, 0f))),
                (RigidBoneId.LowerLegR, Keyed(t, (0f, 0f), (.24f, 8f), (.44f, 28f), (1f, 0f))));
        }

        private static RigidMotionPose Roll(float t)
        {
            var tuck = Mathf.Sin(t * Mathf.PI);
            var turn = -Smooth01(t) * 360f;
            var rootY = Keyed(t, (0f, 0f), (.16f, -.12f), (.36f, .12f), (.55f, .22f), (.78f, .06f), (1f, 0f));
            return Pose(new Vector2(t * .82f, rootY),
                (RigidBoneId.Pelvis, turn),
                (RigidBoneId.Torso, -28f * tuck),
                (RigidBoneId.Head, -16f * tuck),
                (RigidBoneId.UpperArmL, 62f * tuck),
                (RigidBoneId.LowerArmL, -86f * tuck),
                (RigidBoneId.WristL, 18f * tuck),
                (RigidBoneId.UpperArmR, 54f * tuck),
                (RigidBoneId.LowerArmR, -78f * tuck),
                (RigidBoneId.WristR, 14f * tuck),
                (RigidBoneId.UpperLegL, 58f * tuck),
                (RigidBoneId.LowerLegL, -112f * tuck),
                (RigidBoneId.FootL, 42f * tuck),
                (RigidBoneId.UpperLegR, 49f * tuck),
                (RigidBoneId.LowerLegR, -104f * tuck),
                (RigidBoneId.FootR, 38f * tuck));
        }

        private static RigidMotionPose Hit(float t)
        {
            var recoil = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
            return Pose(new Vector2(-.08f * recoil, 0f),
                (RigidBoneId.Torso, 12f * recoil),
                (RigidBoneId.Head, -8f * recoil),
                (RigidBoneId.UpperArmL, 20f * recoil),
                (RigidBoneId.UpperArmR, -18f * recoil));
        }

        private static LegPose GaitLeg(float phase, float stride, float lift, float rootY)
        {
            phase = Mathf.Repeat(phase, 1f);
            float x;
            float y;
            if (phase < .5f)
            {
                var stance = Smooth01(phase * 2f);
                x = Mathf.Lerp(stride, -stride, stance);
                y = .435f - (2.30f + rootY);
            }
            else
            {
                var swing = Smooth01((phase - .5f) * 2f);
                x = swing < .55f
                    ? Mathf.Lerp(-stride, stride * .42f, Smooth01(swing / .55f))
                    : Mathf.Lerp(stride * .42f, stride, Smooth01((swing - .55f) / .45f));
                y = .435f + Mathf.Sin(swing * Mathf.PI) * lift - (2.30f + rootY);
            }
            var toePitch = phase < .12f ? Mathf.Lerp(-12f, 0f, phase / .12f) :
                phase > .44f && phase < .62f ? Mathf.Sin((phase - .44f) / .18f * Mathf.PI) * 18f : 0f;
            return SolveLeg(x, y, toePitch);
        }

        private static LegPose SolveLeg(float x, float y, float footPitch)
        {
            const float thigh = .95847f;
            const float shin = .90750f;
            const float thighBindAngle = -91.793f;
            const float shinBindAngle = -91.894f;
            var distance = Mathf.Clamp(Mathf.Sqrt(x * x + y * y), Mathf.Abs(thigh - shin) + .001f, thigh + shin - .001f);
            var cosine = Mathf.Clamp((distance * distance - thigh * thigh - shin * shin) / (2f * thigh * shin), -1f, 1f);
            var knee = -Mathf.Acos(cosine);
            var target = Mathf.Atan2(y, x);
            var upperAbsolute = target - Mathf.Atan2(shin * Mathf.Sin(knee), thigh + shin * Mathf.Cos(knee));
            var lowerAbsolute = upperAbsolute + knee;
            var upper = upperAbsolute * Mathf.Rad2Deg - thighBindAngle;
            var lower = lowerAbsolute * Mathf.Rad2Deg - upper - shinBindAngle;
            return new LegPose(upper, lower, -upper - lower + footPitch, x);
        }

        private static float Cyclic(float t, float a, float b, float c, float d)
        {
            var phase = Mathf.Repeat(t, 1f) * 4f;
            var index = Mathf.FloorToInt(phase);
            var blend = Smooth01(phase - index);
            return index switch
            {
                0 => Mathf.Lerp(a, b, blend),
                1 => Mathf.Lerp(b, c, blend),
                2 => Mathf.Lerp(c, d, blend),
                _ => Mathf.Lerp(d, a, blend),
            };
        }

        private static float Keyed(float t,
            (float time, float value) k0, (float time, float value) k1,
            (float time, float value) k2, (float time, float value) k3)
        {
            t = Mathf.Clamp01(t);
            if (t <= k1.time) return Segment(t, k0, k1);
            if (t <= k2.time) return Segment(t, k1, k2);
            return Segment(t, k2, k3);
        }

        private static float Keyed(float t,
            (float time, float value) k0, (float time, float value) k1, (float time, float value) k2,
            (float time, float value) k3, (float time, float value) k4)
        {
            t = Mathf.Clamp01(t);
            if (t <= k1.time) return Segment(t, k0, k1);
            if (t <= k2.time) return Segment(t, k1, k2);
            if (t <= k3.time) return Segment(t, k2, k3);
            return Segment(t, k3, k4);
        }

        private static float Keyed(float t,
            (float time, float value) k0, (float time, float value) k1, (float time, float value) k2,
            (float time, float value) k3, (float time, float value) k4, (float time, float value) k5)
        {
            t = Mathf.Clamp01(t);
            if (t <= k1.time) return Segment(t, k0, k1);
            if (t <= k2.time) return Segment(t, k1, k2);
            if (t <= k3.time) return Segment(t, k2, k3);
            if (t <= k4.time) return Segment(t, k3, k4);
            return Segment(t, k4, k5);
        }

        private static float Keyed(float t,
            (float time, float value) k0, (float time, float value) k1, (float time, float value) k2,
            (float time, float value) k3, (float time, float value) k4, (float time, float value) k5,
            (float time, float value) k6)
        {
            t = Mathf.Clamp01(t);
            if (t <= k1.time) return Segment(t, k0, k1);
            if (t <= k2.time) return Segment(t, k1, k2);
            if (t <= k3.time) return Segment(t, k2, k3);
            if (t <= k4.time) return Segment(t, k3, k4);
            if (t <= k5.time) return Segment(t, k4, k5);
            return Segment(t, k5, k6);
        }

        private static float Segment(float t, (float time, float value) a, (float time, float value) b)
        {
            var duration = b.time - a.time;
            var blend = duration <= 0f ? 1f : Smooth01((t - a.time) / duration);
            return Mathf.Lerp(a.value, b.value, blend);
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }

        private static RigidMotionPose Pose(Vector2 rootOffset, params (RigidBoneId bone, float rotation)[] rotations)
        {
            return new RigidMotionPose(rootOffset, rotations.ToDictionary(
                row => row.bone, row => new RigidBonePose(Vector2.zero, row.rotation)));
        }
    }

    public sealed class RigidCharacterAnimator
    {
        private readonly RigidCharacterSkeleton _skeleton;
        private readonly Dictionary<RigidBoneId, Vector3> _bindPositions;
        private readonly Dictionary<RigidBoneId, Quaternion> _bindRotations;

        public RigidCharacterAnimator(RigidCharacterSkeleton skeleton)
        {
            _skeleton = skeleton ?? throw new ArgumentNullException(nameof(skeleton));
            _bindPositions = skeleton.BoneIds.ToDictionary(id => id, id => skeleton[id].localPosition);
            _bindRotations = skeleton.BoneIds.ToDictionary(id => id, id => skeleton[id].localRotation);
            if (skeleton.AllTransforms.Any(transform => transform.localScale != Vector3.one))
                throw new InvalidOperationException("Canonical rigid skeleton requires localScale (1,1,1)");
        }

        public void Apply(RigidMotionPose pose)
        {
            if (pose == null) throw new ArgumentNullException(nameof(pose));
            foreach (var id in _skeleton.BoneIds)
            {
                var transform = _skeleton[id];
                transform.localPosition = _bindPositions[id];
                transform.localRotation = _bindRotations[id];
            }
            _skeleton.CharacterRoot.localPosition = _bindPositions[RigidBoneId.CharacterRoot] + (Vector3)pose.RootOffset;
            _skeleton.CharacterRoot.localRotation = Quaternion.identity;
            foreach (var entry in pose.Bones)
            {
                var transform = _skeleton[entry.Key];
                transform.localPosition = _bindPositions[entry.Key] + (Vector3)entry.Value.PositionOffset;
                transform.localRotation = _bindRotations[entry.Key] * Quaternion.Euler(0f, 0f, entry.Value.RotationDegrees);
            }
        }
    }
}
