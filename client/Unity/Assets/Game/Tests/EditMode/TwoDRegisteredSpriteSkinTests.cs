using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using LinhGioi.World;

namespace LinhGioi.Tests
{
    public sealed class TwoDRegisteredSpriteSkinTests
    {
        [System.Serializable] private sealed class LandmarkPack { public LandmarkBone[] bones; }
        [System.Serializable] private sealed class LandmarkBone { public string id, gender; public int parent; public float startX, startY, endX, endY; }
        [System.Serializable] private sealed class LandmarkReview { public LandmarkRoi[] joints; }
        [System.Serializable] private sealed class LandmarkRoi { public string bone, gender, visibility, reason, parentBone, endJoint; public float[] roi, endRoi; }

        [TestCase("male")]
        [TestCase("female")]
        public void ClosedBodyKeepsAllLimbsRegisteredThroughMotionAndGenderSwap(string gender)
        {
            var root = new GameObject("closed body integration test");
            var sprites = new System.Collections.Generic.List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, false, true);
                outfit.Apply(gender, true, _ => false, _ => 0);
                Assert.That(outfit.VisibleLayers, Is.EqualTo(1));
                Assert.That(outfit.VisibleRigidAttachments, Is.EqualTo(10));
                var rest = outfit.SnapshotVertices(root.transform);
                foreach (var motion in new[] { "run", "jump" })
                foreach (var facing in new[] { -1, 1 })
                {
                    outfit.Apply(gender, true, _ => false, _ => 0);
                    outfit.ApplyMovement(gender, motion, .15f, .5f, facing);
                    foreach (var point in outfit.SnapshotVertices(root.transform))
                        Assert.That(float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsInfinity(point.x) || float.IsInfinity(point.y), Is.False);
                }
                outfit.Apply(gender, true, _ => false, _ => 0);
                outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                var reset = outfit.SnapshotVertices(root.transform);
                Assert.That(reset.Length, Is.EqualTo(rest.Length));
                for (var n = 0; n < rest.Length; n++) Assert.That(Vector3.Distance(rest[n], reset[n]), Is.LessThan(.00001f));
                outfit.Apply(gender == "male" ? "female" : "male", true, _ => false, _ => 0);
                Assert.That(outfit.VisibleRigidAttachments, Is.EqualTo(10));
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }

        [TestCase("male")]
        [TestCase("female")]
        public void ClosedFarArmStaysRigidOnSharedBonesThroughMotionAndReset(string gender)
        {
            var root = new GameObject("closed arm test");
            var sprites = new System.Collections.Generic.List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, true);
                outfit.Apply(gender, true, _ => false, _ => 0);
                outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                var arms = System.Array.FindAll(root.GetComponentsInChildren<SpriteRenderer>(true), r => r.name.StartsWith("Closed arm " + gender));
                Assert.That(arms.Length, Is.EqualTo(2));
                Assert.That(outfit.VisibleLayers, Is.EqualTo(1), "Semantic base remains one loadout layer");
                var centers = new Vector3[2]; var scales = new Vector3[2]; var local = new Vector3[2];
                for (var i = 0; i < arms.Length; i++)
                {
                    Assert.That(arms[i].enabled, Is.True);
                    Assert.That(arms[i].transform.parent.name, Does.StartWith("Registered " + gender + " right-"));
                    centers[i] = arms[i].bounds.center; scales[i] = arms[i].transform.localScale; local[i] = arms[i].transform.localPosition;
                }
                foreach (var motion in new[] { "run", "jump" })
                foreach (var facing in new[] { -1, 1 })
                {
                    outfit.Apply(gender, true, _ => false, _ => 0);
                    outfit.ApplyMovement(gender, motion, .12f, .35f, facing);
                    for (var i = 0; i < arms.Length; i++)
                    {
                        Assert.That(arms[i].transform.localPosition, Is.EqualTo(local[i]), "No pose-dependent attachment refit");
                        Assert.That(arms[i].transform.localScale, Is.EqualTo(scales[i]), "No limb stretching");
                    }
                    Assert.That(outfit.SnapshotVertices(root.transform).Length, Is.GreaterThan(0));
                }
                outfit.Apply(gender, true, _ => false, _ => 0);
                outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                for (var i = 0; i < arms.Length; i++)
                    Assert.That(Vector3.Distance(arms[i].bounds.center, centers[i]), Is.LessThan(.00001f), "Rest registration changed");
                outfit.Apply(gender == "male" ? "female" : "male", true, _ => false, _ => 0);
                foreach (var arm in arms) Assert.That(arm.enabled, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(root);
                foreach (var sprite in sprites) Object.DestroyImmediate(sprite);
            }
        }

        [Test]
        public void RuntimeJointsStayInsideReviewedSourceAnatomy()
        {
            const string path = "LGOClasses/VoRegisteredLv1/";
            var reviewAsset = Resources.Load<TextAsset>(path + "landmark-review");
            Assert.That(reviewAsset, Is.Not.Null, "Rig needs an independent source anatomical review");
            var pack = JsonUtility.FromJson<LandmarkPack>(Resources.Load<TextAsset>(path + "manifest").text);
            var review = JsonUtility.FromJson<LandmarkReview>(reviewAsset.text);
            Assert.That(review.joints.Length, Is.EqualTo(pack.bones.Length));
            var seen = new System.Collections.Generic.HashSet<string>();
            foreach (var bone in pack.bones)
            {
                var key = bone.gender + "/" + bone.id;
                Assert.That(seen.Add(key), Is.True, "Duplicate " + key);
                var matches = System.Array.FindAll(review.joints, j => j.bone == bone.id && j.gender == bone.gender);
                Assert.That(matches.Length, Is.EqualTo(1), key);
                var joint = matches[0];
                Assert.That(joint.roi.Length, Is.EqualTo(4), key);
                Assert.That(bone.startX, Is.InRange(joint.roi[0], joint.roi[2]), key + " anatomical x");
                Assert.That(bone.startY, Is.InRange(joint.roi[1], joint.roi[3]), key + " anatomical y");
                Assert.That(joint.endRoi.Length, Is.EqualTo(4), key);
                Assert.That(bone.endX, Is.InRange(joint.endRoi[0], joint.endRoi[2]), key + " endpoint x");
                Assert.That(bone.endY, Is.InRange(joint.endRoi[1], joint.endRoi[3]), key + " endpoint y");
                var chain = System.Array.FindAll(pack.bones, b => b.gender == bone.gender);
                var index = System.Array.IndexOf(chain, bone);
                Assert.That(bone.parent, Is.InRange(-1, index - 1), key + " parent order");
                Assert.That(bone.parent < 0 ? "" : chain[bone.parent].id, Is.EqualTo(joint.parentBone ?? ""), key + " parent chain");
                if (!string.IsNullOrEmpty(joint.endJoint))
                {
                    var child = System.Array.Find(chain, b => b.id == joint.endJoint);
                    Assert.That(child, Is.Not.Null, key + " endpoint child");
                    Assert.That(Vector2.Distance(new Vector2(bone.endX, bone.endY), new Vector2(child.startX, child.startY)), Is.LessThan(.001f), key + " disconnected joint");
                }
                Assert.That(joint.visibility, Is.EqualTo("visible").Or.EqualTo("occluded"));
                if (joint.visibility == "occluded") Assert.That(joint.reason, Is.Not.Null.And.Not.Empty);
            }
            // This prevents the previous elbow/knee drift, not silhouette approval.
        }

        [Test]
        public void ForearmDoesNotFollowNearbyUnrelatedThigh()
        {
            var root = new GameObject("weight ownership");
            try
            {
                Transform Bone(string name, Transform parent)
                {
                    var t = new GameObject(name).transform; t.SetParent(parent, false); return t;
                }
                var hip = Bone("hip", root.transform);
                var arm = Bone("arm", hip);
                var hand = Bone("forearm", arm);
                var thigh = Bone("thigh", hip);
                var bones = new[]
                {
                    new TwoDRegisteredSpriteSkin.Bone(hip, new Vector2(0, 0), new Vector2(0, 1), -1),
                    new TwoDRegisteredSpriteSkin.Bone(arm, new Vector2(-.2f, 1), new Vector2(-.2f, .5f), 0),
                    new TwoDRegisteredSpriteSkin.Bone(hand, new Vector2(-.2f, .5f), new Vector2(-.2f, 0), 1),
                    new TwoDRegisteredSpriteSkin.Bone(thigh, new Vector2(-.1f, 0), new Vector2(-.1f, -.5f), 0)
                };
                var binder = new TwoDRegisteredSpriteSkin(root.transform, bones);
                var texture = new Texture2D(128, 128);
                var sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), Vector2.one * .5f, 100);
                try
                {
                    var host = new GameObject("authored forearm"); host.transform.SetParent(root.transform, false);
                    var renderer = host.AddComponent<SpriteRenderer>(); renderer.sprite = sprite;
                    var mesh = new TwoDRegisteredSpriteSkin.AuthoredMesh
                    {
                        vertices = new[]
                        {
                            new TwoDRegisteredSpriteSkin.AuthoredVertex { x = 330, y = 1450, bone0 = 2, bone1 = 1, weight0 = 1 },
                            new TwoDRegisteredSpriteSkin.AuthoredVertex { x = 350, y = 1450, bone0 = 2, bone1 = 1, weight0 = 1 },
                            new TwoDRegisteredSpriteSkin.AuthoredVertex { x = 330, y = 1470, bone0 = 2, bone1 = 1, weight0 = 1 }
                        }, indices = new[] { 0, 1, 2 }
                    };
                    var skin = binder.Bind(renderer, 1, 1, mesh);
                    var positions = sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position);
                    var blend = sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight);
                    var bind = sprite.GetBindPoses();
                    thigh.localRotation = Quaternion.Euler(0, 0, 110);
                    for (var i = 0; i < positions.Length; i++)
                    {
                        var w = blend[i];
                        Assert.That(w.boneIndex0, Is.EqualTo(2));
                        var actual = skin.boneTransforms[w.boneIndex0].localToWorldMatrix.MultiplyPoint3x4(bind[w.boneIndex0].MultiplyPoint3x4(positions[i])) * w.weight0;
                        Assert.That(Vector3.Distance(actual, renderer.transform.TransformPoint(positions[i])), Is.LessThan(.00001f), "Moving thigh must not deform authored forearm");
                    }
                }
                finally { Object.DestroyImmediate(sprite); Object.DestroyImmediate(texture); }

            }
            finally { Object.DestroyImmediate(root); }
        }

        [Test]
        public void DifferentCropsPreserveRegistrationUnderTranslatedScaledParent()
        {
            var root = new GameObject("registered root");
            var texture = new Texture2D(128, 128);
            Sprite first = null, second = null;
            try
            {
                root.transform.position = new Vector3(8, -3, 0);
                root.transform.localScale = Vector3.one * 1.3f;
                var hip = new GameObject("hip").transform;
                hip.SetParent(root.transform, false);
                var shoulder = new GameObject("shoulder").transform;
                shoulder.SetParent(hip, false);
                shoulder.localPosition = Vector3.up;
                var binder = new TwoDRegisteredSpriteSkin(root.transform, new[]
                {
                    new TwoDRegisteredSpriteSkin.Bone(hip, Vector2.zero, Vector2.up, -1),
                    new TwoDRegisteredSpriteSkin.Bone(shoulder, Vector2.up, Vector2.up * 2, 0)
                });
                first = Sprite.Create(texture, new Rect(0, 0, 128, 128), Vector2.one * .5f, 100);
                second = Sprite.Create(texture, new Rect(16, 16, 64, 96), Vector2.one * .5f, 100);
                foreach (var sprite in new[] { first, second })
                {
                    var host = new GameObject("cropped layer");
                    host.transform.SetParent(root.transform, false);
                    host.transform.localPosition = new Vector3(.25f, .6f, 0);
                    host.transform.localScale = new Vector3(.8f, 1.1f, 1);
                    var renderer = host.AddComponent<SpriteRenderer>();
                    renderer.sprite = sprite;
                    var originalBounds = sprite.bounds;
                    var skin = binder.Bind(renderer, sprite == first ? 8 : 4, sprite == first ? 8 : 6);
                    Assert.That(skin.boneTransforms.Length, Is.EqualTo(2));
                    Assert.That(sprite.bounds.center, Is.EqualTo(originalBounds.center));
                    Assert.That(Vector3.Distance(sprite.bounds.size, originalBounds.size), Is.LessThan(.0001f));
                    var positions = sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position);
                    Assert.That(positions.Length, Is.EqualTo(sprite == first ? 81 : 35), "Skinning grid must actually replace the quad");
                    var weights = sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight);
                    var bindPoses = sprite.GetBindPoses();
                    for (var i = 0; i < positions.Length; i++)
                    {
                        var expected = renderer.transform.TransformPoint(positions[i]);
                        var w = weights[i];
                        var actual = skin.boneTransforms[w.boneIndex0].localToWorldMatrix.MultiplyPoint3x4(
                            bindPoses[w.boneIndex0].MultiplyPoint3x4(positions[i])) * w.weight0
                            + skin.boneTransforms[w.boneIndex1].localToWorldMatrix.MultiplyPoint3x4(
                                bindPoses[w.boneIndex1].MultiplyPoint3x4(positions[i])) * w.weight1;
                        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(.00001f), "Bind must preserve source placement");
                        var canonical = root.transform.InverseTransformPoint(expected);
                        var shared = binder.WeightsAt(canonical);
                        for (var bone = 0; bone < 2; bone++)
                        {
                            var actualWeight = (w.boneIndex0 == bone ? w.weight0 : 0) + (w.boneIndex1 == bone ? w.weight1 : 0);
                            var sharedWeight = (shared.boneIndex0 == bone ? shared.weight0 : 0) + (shared.boneIndex1 == bone ? shared.weight1 : 0);
                            Assert.That(actualWeight, Is.EqualTo(sharedWeight).Within(.00001f));
                        }
                    }
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
                if (first != null) Object.DestroyImmediate(first);
                if (second != null) Object.DestroyImmediate(second);
                Object.DestroyImmediate(texture);
            }
        }
    }
}
