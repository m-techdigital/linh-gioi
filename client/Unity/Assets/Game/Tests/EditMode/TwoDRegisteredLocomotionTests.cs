using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;
using NUnit.Framework;
using UnityEngine;
using LinhGioi.World;

namespace LinhGioi.Tests
{
    public sealed class TwoDRegisteredLocomotionTests
    {
        [TestCase("male")]
        [TestCase("female")]
        public void HeldSomersaultRestartHasNoElbowBranchSnap(string gender)
        {
            var root = new GameObject("jump restart continuity");
            var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true);
                outfit.Apply(gender, true, slot => false, part => 0);
                var rest = outfit.SnapshotVertices(outfit.BindSpace);
                foreach (var progress in new[] { .9999f, 1f, 0f, .0001f })
                {
                    outfit.Apply(gender, true, slot => false, part => 0);
                    outfit.ApplyMovement(gender, "jump", 0, progress, 1);
                    var pose = outfit.SnapshotVertices(outfit.BindSpace);
                    for (var i = 0; i < pose.Length; i++)
                        Assert.That(Vector3.Distance(rest[i], pose[i]), Is.LessThan(.001f), "Tuck approaching zero must converge to bind, including elbow IK branch");
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }

        [TestCase("male")]
        [TestCase("female")]
        public void TuckedHandsFollowKneesBelowHeadWithoutStretchingArm(string gender)
        {
            var root = new GameObject("tuck anatomy");
            var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true);
                foreach (var side in new[] { "left", "right" })
                {
                    var nodes = root.GetComponentsInChildren<Transform>();
                    var tip = nodes.SingleOrDefault(t => t.name == "Registered handtip " + gender + "_" + side);
                    Assert.That(tip, Is.Not.Null, "Closed arm chain needs a measured hand endpoint");
                    var elbow = tip.parent;
                    var shoulder = elbow.parent;
                    var upperLength = Vector3.Distance(shoulder.position, elbow.position);
                    var lowerLength = Vector3.Distance(elbow.position, tip.position);
                    var knee = nodes.Single(t => t.name == "Registered " + gender + " " + side + "-shin-foot");
                    var head = nodes.Single(t => t.name == "Registered " + gender + " head");
                    foreach (var progress in new[] { .2f, .4f, .65f })
                    foreach (var facing in new[] { -1, 1 })
                    {
                        outfit.Apply(gender, true, slot => false, part => 0);
                        outfit.ApplyMovement(gender, "jump", 0, progress, facing);
                        var handPoint = outfit.BindSpace.InverseTransformPoint(tip.position);
                        var kneePoint = outfit.BindSpace.InverseTransformPoint(knee.position);
                        var headPoint = outfit.BindSpace.InverseTransformPoint(head.position);
                        Assert.That(Vector3.Distance(handPoint, kneePoint), Is.LessThan(.14f), "Hands should fold toward knees, not reach overhead");
                        Assert.That(handPoint.y, Is.LessThan(headPoint.y - .2f));
                        if (gender == "male") Assert.That(outfit.BindSpace.InverseTransformPoint(elbow.position).y,
                            Is.LessThan(outfit.BindSpace.InverseTransformPoint(shoulder.position).y), "Tuck elbow must fold below shoulder, not behind head");
                        Assert.That(Vector3.Distance(shoulder.position, elbow.position), Is.EqualTo(upperLength).Within(.00001f));
                        Assert.That(Vector3.Distance(elbow.position, tip.position), Is.EqualTo(lowerLength).Within(.00001f));
                    }
                    outfit.Apply(gender, true, slot => false, part => 0);
                    outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }

        [System.Serializable] private sealed class PoseDump
        {
            public string gender;
            public Vector3[] bind;
            public int[] indices;
            public List<PoseFrame> frames = new List<PoseFrame>();
        }
        [System.Serializable] private sealed class PoseFrame
        {
            public string motion;
            public float progress;
            public Vector3[] vertices;
            public Matrix4x4[] bones;
        }
        [TestCase("male", false)]
        [TestCase("female", false)]
        [TestCase("male", true)]
        [TestCase("female", true)]
        public void BaseMeshRemainsFiniteAcrossFullMotionSweep(string gender, bool anatomical)
        {
            var root = new GameObject("base strain diagnostic");
            var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, anatomical);
                outfit.Apply(gender, true, slot => false, part => 0);
                var dump = new PoseDump { gender = gender, bind = outfit.SnapshotVertices(root.transform), indices = root.GetComponentsInChildren<SpriteRenderer>().Single(r => r.enabled).sprite.GetIndices().Select(i => (int)i).ToArray() };
                foreach (var motion in new[] { "walk", "run", "jump" })
                for (var tick = 0; tick < 32; tick++)
                {
                    var progress = tick / 32f;
                    outfit.Apply(gender, true, slot => false, part => 0);
                    outfit.ApplyMovement(gender, motion, (motion == "run" ? progress / TwoDRegisteredOutfit.RunCyclesPerSecond(gender) : progress / 2f), progress, 1);
                    var vertices = outfit.SnapshotVertices(root.transform);
                    Assert.That(vertices.Length, Is.EqualTo(dump.bind.Length));
                    foreach (var point in vertices)
                        Assert.That(float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsInfinity(point.x) || float.IsInfinity(point.y), Is.False);
                    dump.frames.Add(new PoseFrame { motion = motion, progress = progress, vertices = vertices, bones = outfit.SnapshotBoneMatrices(gender, root.transform) });
                }
                // Numerical validity is not a silhouette gate. Export actual
                // skinned vertices so opaque-region folds can be located in source space.
                var directory = System.IO.Path.GetFullPath(Application.dataPath + "/../../../build/" + (anatomical ? "vo-anatomical-deformation" : "vo-base-deformation"));
                System.IO.Directory.CreateDirectory(directory);
                System.IO.File.WriteAllText(System.IO.Path.Combine(directory, gender + ".json"), JsonUtility.ToJson(dump));
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }

        [TestCase("male")]
        [TestCase("female")]
        public void StopTransitionAndEquipmentRefreshDoNotSnapOrAdvanceTime(string gender)
        {
            var root = new GameObject("transition test");
            var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, true);
                outfit.Apply(gender, false, slot => true, part => 0);
                outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                var rest = outfit.SnapshotVertices(root.transform);
                outfit.Apply(gender, false, slot => true, part => 0);
                outfit.ApplyMovement(gender, "run", .1f, 0, 1);
                outfit.Advance(.2f);
                outfit.Apply(gender, false, slot => true, part => 0);
                outfit.ApplyMovement(gender, "run", .1f, 0, 1);
                var running = outfit.SnapshotVertices(root.transform);
                foreach (var slot in new[] { "main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment", "waist", "arm_guard", "boots", "light_armor", "accessory" })
                {
                    outfit.Apply(gender, false, item => item != slot, part => 0);
                    outfit.ApplyMovement(gender, "run", .1f, 0, 1);
                    Assert.That(outfit.VisibleLayers, Is.EqualTo(10));
                    outfit.Apply(gender, false, item => true, part => 0);
                    outfit.ApplyMovement(gender, "run", .1f, 0, 1);
                    var restored = outfit.SnapshotVertices(root.transform);
                    for (var i = 0; i < running.Length; i++) Assert.That(Vector3.Distance(running[i], restored[i]), Is.LessThan(.00001f));
                }
                // The next state arrives on an ordinary frame, not a large seek.
                outfit.Advance(.016f);
                outfit.Apply(gender, false, slot => true, part => 0);
                outfit.ApplyMovement(gender, "idle", .1f, 0, 1);
                var stop = outfit.SnapshotVertices(root.transform);
                for (var i = 0; i < running.Length; i++) Assert.That(Vector3.Distance(running[i], stop[i]), Is.LessThan(.00001f), "First idle frame must start from the outgoing pose");
                outfit.Advance(.12f);
                outfit.Apply(gender, false, slot => true, part => 0);
                outfit.ApplyMovement(gender, "idle", .22f, 0, 1);
                var settled = outfit.SnapshotVertices(root.transform);
                for (var i = 0; i < rest.Length; i++) Assert.That(Vector3.Distance(rest[i], settled[i]), Is.LessThan(.00001f), "Must settle exactly without accumulating offsets");
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }

        [TestCase("male")]
        [TestCase("female")]
        public void FacingAndSomersaultKeepEveryLayerOnOneBase(string gender)
        {
            var root = new GameObject("locomotion root");
            var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites);
                outfit.Apply(gender, false, slot => true, part => 0);
                var rest = outfit.SnapshotVertices(root.transform);
                outfit.ApplyMovement(gender, "idle", 0, 0, -1);
                var mirrored = outfit.SnapshotVertices(root.transform);
                Assert.That(mirrored.Length, Is.EqualTo(rest.Length));
                for (var i = 0; i < rest.Length; i++)
                    Assert.That(Vector3.Distance(mirrored[i], new Vector3(-rest[i].x, rest[i].y, rest[i].z)), Is.LessThan(.00001f));
                foreach (var motion in new[] { "walk", "run" })
                for (var tick = 0; tick < 32; tick++)
                {
                    outfit.Apply(gender, false, slot => true, part => 0);
                    outfit.ApplyMovement(gender, motion, tick / 64f, 0, tick < 16 ? -1 : 1);
                    Assert.That(outfit.MaxFootTargetError(gender), Is.LessThan(.005f), gender + " " + motion + " tick " + tick);
                }
                foreach (var progress in new[] { .25f, .5f, .75f })
                {
                    outfit.Apply(gender, false, slot => true, part => 0);
                    outfit.ApplyMovement(gender, "jump", 0, progress, -1);
                    Assert.That(outfit.VisibleLayers, Is.EqualTo(11));
                    Assert.That(root.transform.localScale, Is.EqualTo(Vector3.one));
                    Assert.That(root.transform.localRotation, Is.EqualTo(Quaternion.identity));
                    Assert.That(outfit.RollDegrees, Is.EqualTo(TwoDLocomotionCurves.SomersaultDegrees(progress)).Within(.001f));
                }
                outfit.Apply(gender, false, slot => true, part => 0);
                outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                var after = outfit.SnapshotVertices(root.transform);
                for (var i = 0; i < rest.Length; i++) Assert.That(Vector3.Distance(after[i], rest[i]), Is.LessThan(.00001f));
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }
    }
}
