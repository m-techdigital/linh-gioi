using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using LinhGioi.World;

namespace LinhGioi.Tests
{
    public sealed class TwoDRegisteredEquipmentTests
    {
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
        [TestCase("female", 4)]
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
                Assert.That(outfit.VisibleJointGarments, Is.EqualTo(4));
                Assert.That(outfit.VisibleRigidAttachments, Is.EqualTo(10));
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
