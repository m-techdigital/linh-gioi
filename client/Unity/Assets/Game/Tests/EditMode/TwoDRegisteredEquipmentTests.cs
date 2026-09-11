using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.U2D;
using LinhGioi.World;

namespace LinhGioi.Tests
{
    public sealed class TwoDRegisteredEquipmentTests
    {
        [Test]
        public void ReviewPresentationCanHideRegisteredRenderersWithoutDestroyingWardrobeState()
        {
            var root = new GameObject("registered presentation visibility");
            var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, false, true, true);
                outfit.Apply("male", false, _ => true, _ => 0);
                var visibleLayers = outfit.VisibleLayers;
                var method = typeof(TwoDRegisteredOutfit).GetMethod("SetPresentationVisible");
                Assert.That(method, Is.Not.Null, "The selected pose/wardrobe stack must be able to replace the old presentation");
                method.Invoke(outfit, new object[] { false });
                Assert.That(root.GetComponentsInChildren<Renderer>().All(renderer => renderer.forceRenderingOff), Is.True);
                Assert.That(outfit.VisibleLayers, Is.EqualTo(visibleLayers), "Hiding the superseded visual must not roll back wardrobe state");
                method.Invoke(outfit, new object[] { true });
                Assert.That(root.GetComponentsInChildren<Renderer>().Any(renderer => renderer.forceRenderingOff), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); foreach (var sprite in sprites) UnityEngine.Object.DestroyImmediate(sprite); }
        }

        [Serializable] private sealed class BindPart { public string id; public int x, y, w, h; public float[] sourceCanvasRect; }
        [Serializable] private sealed class BindPack { public BindPart[] parts; }
        [Serializable] private sealed class BindLayer
        {
            public string id, atlas, atlasSha256, manifestSha256; public int order; public Vector2[] points, uv; public int[] triangles;
        }
        [Serializable] private sealed class BindInput { public string path, sha256; }
        [Serializable] private sealed class BindEvidence { public string gender; public BindLayer[] layers; public BindInput[] inputs; }

        [TestCase("male")]
        [TestCase("female")]
        public void BoundOutfitVerticesMatchCanonicalTextureCoordinates(string gender)
        {
            var root = new GameObject("canonical bind evidence"); var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, false, true, true);
                outfit.Apply(gender, false, _ => true, _ => 0);
                outfit.ApplyMovement(gender, "idle", 0, 0, 1);
                var layers = new List<BindLayer>();
                foreach (var renderer in root.GetComponentsInChildren<Renderer>().Where(r => r.enabled))
                {
                    var sr = renderer as SpriteRenderer;
                    Vector3[] local; Vector2[] uv; int[] triangles; Texture texture;
                    if (sr != null)
                    {
                        local = sr.sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position).ToArray();
                        uv = sr.sprite.GetVertexAttribute<Vector2>(UnityEngine.Rendering.VertexAttribute.TexCoord0).ToArray();
                        triangles = sr.sprite.triangles.Select(i => (int)i).ToArray(); texture = sr.sprite.texture;
                        var skin = sr.GetComponent<UnityEngine.U2D.Animation.SpriteSkin>();
                        if (skin != null)
                        {
                            var weights = sr.sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight);
                            var bind = sr.sprite.GetBindPoses();
                            for (var i = 0; i < local.Length; i++)
                            {
                                var w = weights[i]; var v = local[i];
                                local[i] = skin.boneTransforms[w.boneIndex0].TransformPoint(bind[w.boneIndex0].MultiplyPoint3x4(v)) * w.weight0
                                    + skin.boneTransforms[w.boneIndex1].TransformPoint(bind[w.boneIndex1].MultiplyPoint3x4(v)) * w.weight1;
                            }
                        }
                        else for (var i = 0; i < local.Length; i++) local[i] = sr.transform.TransformPoint(local[i]);
                    }
                    else
                    {
                        var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                        local = mesh.vertices.Select(renderer.transform.TransformPoint).ToArray(); uv = mesh.uv; triangles = mesh.triangles;
                        texture = renderer.sharedMaterial.mainTexture;
                    }
                    var atlas = UnityEditor.AssetDatabase.GetAssetPath(texture);
                    Assert.That(atlas, Is.Not.Empty, renderer.name);
                    var pack = JsonUtility.FromJson<BindPack>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(atlas), "manifest.json")));
                    var id = renderer.name.Replace("Registered equipment ", "").Replace("Closed arm ", "").Replace("Registered ", "");
                    var part = pack.parts.Single(p => p.id == id); var box = part.sourceCanvasRect;
                    var points = local.Select(v => outfit.BindSpace.InverseTransformPoint(v)).Select(v => new Vector2(v.x * 1536 / 1.7f + 512, 1484 - v.y * 1536 / 1.7f)).ToArray();
                    Assert.That(triangles.Length, Is.GreaterThan(0));
                    for (var i = 0; i < points.Length; i++)
                    {
                        var expected = new Vector2(box[0] + (uv[i].x * texture.width - part.x) / part.w * (box[2] - box[0]),
                            box[3] - (uv[i].y * texture.height - part.y) / part.h * (box[3] - box[1]));
                        Assert.That(Vector2.Distance(points[i], expected), Is.LessThan(.02f), id + " vertex " + i);
                    }
                    using (var sha = System.Security.Cryptography.SHA256.Create())
                    {
                        string Hash(string path) => BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
                        layers.Add(new BindLayer { id = id, atlas = atlas, atlasSha256 = Hash(atlas),
                            manifestSha256 = Hash(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(atlas), "manifest.json")),
                            order = renderer.sortingOrder, points = points, uv = uv, triangles = triangles });
                    }
                }
                Assert.That(layers.Count, Is.GreaterThan(20));
                var output = Environment.GetEnvironmentVariable("LGO_REGISTERED_BIND_DUMP");
                if (!string.IsNullOrEmpty(output))
                {
                    var inputs = System.IO.Directory.GetFiles("Assets/Game/World/Runtime", "TwoDRegistered*.cs").Concat(new[] {
                        "Assets/Game/Tests/EditMode/TwoDRegisteredEquipmentTests.cs",
                        "Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredLv1/manifest.json",
                        "Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredLv1/anatomical-meshes.json",
                        "Packages/packages-lock.json"
                    }).OrderBy(path => path).Select(path => {
                        using (var sha = System.Security.Cryptography.SHA256.Create())
                            return new BindInput { path = path, sha256 = BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() };
                    }).ToArray();
                    System.IO.Directory.CreateDirectory(output);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(output, gender + "-bind.json"), JsonUtility.ToJson(new BindEvidence { gender = gender, layers = layers.OrderBy(l => l.order).ToArray(), inputs = inputs }, true));
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(root); foreach (var sprite in sprites) UnityEngine.Object.DestroyImmediate(sprite); }
        }

        [Test]
        public void IndependentHairAndPantsStatesRestoreOriginalBodyAfterMovement()
        {
            var root = new GameObject("equipment state test"); var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, false, true, true);
                outfit.Apply("male", true, _ => false, _ => 0);
                outfit.ApplyMovement("male", "idle", 0, 0, 1);
                var original = outfit.SnapshotVertices(outfit.BindSpace);
                foreach (var slots in new[] { new[] { "head_hair" }, new[] { "lower_garment" }, new[] { "head_hair", "lower_garment" }, Array.Empty<string>() })
                {
                    outfit.Apply("male", false, slots.Contains, _ => 0);
                    Assert.That(outfit.VisibleBodyVariants, Is.EqualTo(slots.Length == 0 ? 0 : 1));
                    Assert.That(outfit.VisibleEquipmentAttachments, Is.EqualTo((slots.Contains("head_hair") ? 1 : 0) + (slots.Contains("lower_garment") ? 3 : 0)));
                    outfit.ApplyMovement("male", "jump", 0, .25f, -1);
                }
                outfit.Apply("male", false, _ => true, _ => 0);
                Assert.That(outfit.VisibleEquipmentAttachments, Is.EqualTo(16));
                Assert.That(outfit.VisibleLayers, Is.EqualTo(11));
                var renderers = root.GetComponentsInChildren<Renderer>();
                var feet = renderers.Where(r => r.name == "Closed arm male_left-foot" || r.name == "Closed arm male_right-foot").ToArray();
                var boots = renderers.Where(r => r.name.StartsWith("Registered equipment footwear-")).ToArray();
                Assert.That(feet.Length, Is.EqualTo(2)); Assert.That(boots.Length, Is.EqualTo(2));
                foreach (var boot in boots)
                {
                    Assert.That(boot.sortingOrder, Is.GreaterThan(feet.Max(f => f.sortingOrder)), "Equipped boot must cover fallback foot without sorting ties");
                    Assert.That(boot.sortingOrder, Is.LessThan(renderers.Where(r => r.name.StartsWith("Registered equipment lower_body-")).Min(r => r.sortingOrder)));
                }
                Assert.That(outfit.VisibleRigidAttachments, Is.EqualTo(10));
                Assert.That(outfit.VisibleClothAttachments, Is.Zero);
                Assert.That(outfit.VisibleJointGarments, Is.EqualTo(2));
                Assert.That(outfit.VisibleBodyVariants, Is.EqualTo(1));
                outfit.Apply("female", true, _ => false, _ => 0);
                Assert.That(outfit.VisibleEquipmentAttachments, Is.Zero);
                Assert.That(outfit.VisibleBodyVariants, Is.Zero);
                outfit.Apply("male", true, _ => false, _ => 0);
                outfit.ApplyMovement("male", "idle", 0, 0, 1);
                var restored = outfit.SnapshotVertices(outfit.BindSpace);
                Assert.That(restored.Length, Is.EqualTo(original.Length));
                for (var i = 0; i < original.Length; i++) Assert.That(Vector3.Distance(original[i], restored[i]), Is.LessThan(.00001f));
            }
            finally { UnityEngine.Object.DestroyImmediate(root); foreach (var sprite in sprites) UnityEngine.Object.DestroyImmediate(sprite); }
        }
        [TestCase("male", 2)]
        [TestCase("female", 6)]
        public void EquippedGarmentTrianglesKeepOrientationAcrossNativeMotion(string gender, int expectedMeshes)
        {
            var root = new GameObject("equipment motion test"); var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, false, true, true);
                outfit.Apply(gender, false, _ => true, _ => 0);
                var meshes = root.GetComponentsInChildren<TwoDRegisteredCloth>().Where(c => c.Visible && c.name.StartsWith("Registered equipment")).Select(c => c.GetComponent<MeshFilter>().sharedMesh).ToArray();
                Assert.That(meshes.Length, Is.EqualTo(expectedMeshes));
                foreach (var motion in new[] { "walk", "run", "jump" })
                for (var frame = 0; frame < 32; frame++)
                {
                    outfit.Apply(gender, false, _ => true, _ => 0);
                    outfit.ApplyMovement(gender, motion, frame / 32f * Mathf.PI * 2, frame / 32f, frame % 2 == 0 ? 1 : -1);
                    foreach (var mesh in meshes)
                    {
                        var v = mesh.vertices; var indices = mesh.triangles;
                        for (var i = 0; i < indices.Length; i += 3)
                        {
                            var a = v[indices[i + 1]] - v[indices[i]]; var b = v[indices[i + 2]] - v[indices[i]];
                            Assert.That(a.x * b.y - a.y * b.x, Is.LessThan(0), motion + "/" + frame + " triangle " + i / 3);
                        }
                    }
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(root); foreach (var sprite in sprites) UnityEngine.Object.DestroyImmediate(sprite); }
        }
        [Test]
        public void FemaleEquipmentRestoresBaseAcrossAllBodyStatesAndGenderSwap()
        {
            var root = new GameObject("female equipment states"); var sprites = new List<Sprite>();
            try
            {
                var outfit = new TwoDRegisteredOutfit(root.transform, sprites, false, true, false, true, true);
                outfit.Apply("female", true, _ => false, _ => 0);
                outfit.ApplyMovement("female", "idle", 0, 0, 1);
                var original = outfit.SnapshotVertices(outfit.BindSpace);
                var slots = new[] { "head_hair", "inner_top", "lower_garment" };
                for (var bits = 0; bits < 8; bits++)
                {
                    var selected = slots.Where((slot, index) => (bits & (1 << index)) != 0).ToArray();
                    outfit.Apply("female", false, selected.Contains, _ => 0);
                    Assert.That(outfit.VisibleBodyVariants, Is.EqualTo(bits == 0 ? 0 : 1), "state " + bits);
                    Assert.That(outfit.VisibleEquipmentAttachments, Is.EqualTo(selected.Sum(slot => slot == "lower_garment" ? 3 : 1)));
                    outfit.ApplyMovement("female", "jump", 0, .375f, -1);
                }
                outfit.Apply("female", false, _ => true, _ => 0);
                Assert.That(outfit.VisibleEquipmentAttachments, Is.EqualTo(17));
                Assert.That(outfit.VisibleJointGarments, Is.EqualTo(6));
                Assert.That(outfit.VisibleRigidAttachments, Is.EqualTo(8));
                Assert.That(outfit.VisibleLayers, Is.EqualTo(11));
                foreach (var foot in root.GetComponentsInChildren<SpriteRenderer>().Where(r => r.name == "Closed arm female_left-foot" || r.name == "Closed arm female_right-foot"))
                    Assert.That(foot.enabled, Is.False, "Equipped boots must replace original feet");
                outfit.Apply("male", false, _ => true, _ => 0);
                Assert.That(outfit.VisibleEquipmentAttachments, Is.EqualTo(16));
                outfit.Apply("female", true, _ => false, _ => 0);
                outfit.ApplyMovement("female", "idle", 0, 0, 1);
                var restored = outfit.SnapshotVertices(outfit.BindSpace);
                Assert.That(restored.Length, Is.EqualTo(original.Length));
                for (var i = 0; i < original.Length; i++) Assert.That(Vector3.Distance(original[i], restored[i]), Is.LessThan(.00001f));
            }
            finally { UnityEngine.Object.DestroyImmediate(root); foreach (var sprite in sprites) UnityEngine.Object.DestroyImmediate(sprite); }
        }
    }
}
