using System;
using LinhGioi.Foundation;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LinhGioi.Tests
{
    public sealed class NpcAppearanceTests
    {
        [TestCase("LGOGateKeeperCandidate", 2)]
        [TestCase("LGOArrivalOutfitCandidate", 2)]
        public void SharedAppearanceDoesNotCloneMeshOrMaterialsAcrossNpcInstances(string resource, int materials)
        {
            var prefab = Resources.Load<GameObject>(resource);
            GameObject first = null, second = null;
            try
            {
                first = UnityEngine.Object.Instantiate(prefab); second = UnityEngine.Object.Instantiate(prefab);
                var animator = first.GetComponent<Animator>();
                Assert.That(animator.isHuman, Is.True);
                var mapping = animator.avatar.humanDescription.human.ToDictionary(bone => bone.humanName, bone => bone.boneName);
                Assert.That(mapping["Spine"], Is.EqualTo("spine_01"));
                Assert.That(mapping["Chest"], Is.EqualTo("spine_02"));
                Assert.That(mapping["UpperChest"], Is.EqualTo("spine_03"));
                Assert.That(animator.GetBoneTransform(HumanBodyBones.Chest), Is.Not.Null);
                Assert.That(animator.runtimeAnimatorController, Is.Not.Null);
                Assert.That(animator.applyRootMotion, Is.False);
                var binding = first.GetComponent<NpcAppearanceInstance>();
                Assert.That(binding, Is.Not.Null);
                var a = first.GetComponentInChildren<SkinnedMeshRenderer>();
                var b = second.GetComponentInChildren<SkinnedMeshRenderer>();
                binding.Appearance.Apply(a); binding.Appearance.Apply(b);
                Assert.That(a.sharedMesh, Is.SameAs(b.sharedMesh));
                Assert.That(a.sharedMaterials.Length, Is.EqualTo(materials));
                for (var i = 0; i < a.sharedMaterials.Length; i++) Assert.That(a.sharedMaterials[i], Is.SameAs(b.sharedMaterials[i]));
                if (resource == "LGOArrivalOutfitCandidate")
                {
                    var keeper = Resources.Load<GameObject>("LGOGateKeeperCandidate").GetComponentInChildren<SkinnedMeshRenderer>();
                    Assert.That(a.sharedMaterials[0], Is.SameAs(keeper.sharedMaterials[0]), "Wardrobes still share their body atlas.");
                    var face = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Game/Art/OnboardingCandidate/ArrivalFace.png");
                    Assert.That(face, Is.Not.Null);
                    Assert.That(a.sharedMaterials[1].GetTexture("_BaseMap"), Is.SameAs(face));
                    Assert.That(new[] { face.width, face.height }, Is.EqualTo(new[] { 1774, 887 }), "Keep native PC face detail during import.");
                    Assert.That(a.sharedMaterials[1].GetTexture("_BaseMap"), Is.Not.SameAs(keeper.sharedMaterials[1].GetTexture("_BaseMap")),
                        "The continuous player head must not sample the Keeper's incompatible face UV islands.");
                }
                Assert.That(a.bones[0], Is.Not.SameAs(b.bones[0]), "NPC poses must remain independent.");
            }
            finally { UnityEngine.Object.DestroyImmediate(first); UnityEngine.Object.DestroyImmediate(second); }
        }

        [TestCase("GateKeeper", "Keeper")]
        [TestCase("ArrivalScene", "Arrival")]
        public void SemanticModulesContainExactlyTheirTaggedSourceGeometry(string model, string stem)
        {
            const string root = "Assets/Game/Art/OnboardingCandidate/";
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(root + model + ".fbx").GetComponentInChildren<SkinnedMeshRenderer>();
            var recipe = AssetDatabase.LoadAssetAtPath<LinhGioi.Foundation.Editor.NpcAppearanceRecipe>(root + "Editor/" + stem + "Parts/" + stem + "Recipe.asset");
            var expectedSlots = new[] { "Head", "Hair", "UpperBody", "LowerBody", "Gloves", "Boots", "Shoulders" };
            if (stem == "Keeper") expectedSlots = expectedSlots.Concat(new[] { "Headwear", "Cape" }).ToArray();
            Assert.That(recipe.Slots, Is.EquivalentTo(expectedSlots), "Do not silently lose a wardrobe module during authoring/export.");
            Assert.That(recipe.Slots.Distinct().Count(), Is.EqualTo(recipe.Parts.Length));
            var vertices = source.sharedMesh.vertices;
            uint count = 0;
            for (var slot = 0; slot < recipe.Slots.Length; slot++)
            {
                var part = recipe.Parts[slot];
                var expected = Enumerable.Range(0, source.sharedMesh.subMeshCount)
                    .Where(sub => source.sharedMaterials[sub].name.StartsWith("LGO_" + recipe.Slots[slot] + "_", StringComparison.Ordinal))
                    .SelectMany(sub => source.sharedMesh.GetTriangles(sub)).Select(i => vertices[i]).Distinct().ToArray();
                Assert.That(expected.Length, Is.GreaterThan(0));
                var actual = part.vertices.Distinct().ToArray();
                Assert.That(expected.Except(actual).Count(), Is.Zero, "Missing tagged vertices in " + part.name);
                Assert.That(actual.Except(expected).Count(), Is.Zero, "Foreign vertices in " + part.name);
                for (var sub = 0; sub < part.subMeshCount; sub++) count += part.GetIndexCount(sub);
            }
            uint sourceCount = 0;
            for (var sub = 0; sub < source.sharedMesh.subMeshCount; sub++) sourceCount += source.sharedMesh.GetIndexCount(sub);
            Assert.That(count, Is.EqualTo(sourceCount));
        }

        [Test]
        public void IncompatibleRigRejectsAppearanceWithoutChangingExistingMesh()
        {
            var prefab = Resources.Load<GameObject>("LGOGateKeeperCandidate");
            var instance = UnityEngine.Object.Instantiate(prefab);
            try
            {
                var binding = instance.GetComponent<NpcAppearanceInstance>();
                Assert.That(binding, Is.Not.Null);
                var renderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                var original = renderer.sharedMesh;
                renderer.bones = Array.Empty<Transform>();
                Assert.Throws<InvalidOperationException>(() => binding.Appearance.Apply(renderer));
                Assert.That(renderer.sharedMesh, Is.SameAs(original));
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
    }
}
