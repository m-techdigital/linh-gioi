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
        [Test]
        public void SharedAppearanceDoesNotCloneMeshOrMaterialsAcrossNpcInstances()
        {
            var prefab = Resources.Load<GameObject>("LGOGateKeeperCandidate");
            GameObject first = null, second = null;
            try
            {
                first = UnityEngine.Object.Instantiate(prefab); second = UnityEngine.Object.Instantiate(prefab);
                var animator = first.GetComponent<Animator>();
                Assert.That(animator.isHuman, Is.True);
                Assert.That(animator.runtimeAnimatorController, Is.Not.Null);
                Assert.That(animator.applyRootMotion, Is.False);
                var binding = first.GetComponent<NpcAppearanceInstance>();
                Assert.That(binding, Is.Not.Null);
                var a = first.GetComponentInChildren<SkinnedMeshRenderer>();
                var b = second.GetComponentInChildren<SkinnedMeshRenderer>();
                binding.Appearance.Apply(a); binding.Appearance.Apply(b);
                Assert.That(a.sharedMesh, Is.SameAs(b.sharedMesh));
                Assert.That(a.sharedMaterials.Length, Is.EqualTo(2));
                for (var i = 0; i < a.sharedMaterials.Length; i++) Assert.That(a.sharedMaterials[i], Is.SameAs(b.sharedMaterials[i]));
                Assert.That(a.bones[0], Is.Not.SameAs(b.bones[0]), "NPC poses must remain independent.");
            }
            finally { UnityEngine.Object.DestroyImmediate(first); UnityEngine.Object.DestroyImmediate(second); }
        }

        [Test]
        public void SemanticModulesContainExactlyTheirTaggedSourceGeometry()
        {
            const string root = "Assets/Game/Art/OnboardingCandidate/";
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(root + "GateKeeper.fbx").GetComponentInChildren<SkinnedMeshRenderer>();
            var hat = AssetDatabase.LoadAssetAtPath<Mesh>(root + "Editor/KeeperParts/Headwear.asset");
            var hair = AssetDatabase.LoadAssetAtPath<Mesh>(root + "Editor/KeeperParts/HeadAndHair.asset");
            var vertices = source.sharedMesh.vertices;
            foreach (var entry in new[] { (part: hat, labels: new[] { "Keeper Headwear" }),
                (part: hair, labels: new[] { "Keeper Head Hair", "Keeper Reconstruction Face" }) })
            {
                var expected = Enumerable.Range(0, source.sharedMesh.subMeshCount)
                    .Where(sub => entry.labels.Contains(source.sharedMaterials[sub].name))
                    .SelectMany(sub => source.sharedMesh.GetTriangles(sub)).Select(i => vertices[i]).Distinct().ToArray();
                Assert.That(expected.Length, Is.GreaterThan(0));
                var actual = entry.part.vertices.Distinct().ToArray();
                Assert.That(expected.Except(actual).Count(), Is.Zero, "Missing tagged vertices in " + entry.part.name);
                Assert.That(actual.Except(expected).Count(), Is.Zero, "Foreign vertices in " + entry.part.name);
            }
            uint count = 0;
            foreach (var part in new[] { hat, hair, AssetDatabase.LoadAssetAtPath<Mesh>(root + "Editor/KeeperParts/OutfitAndBody.asset") })
                for (var sub = 0; sub < part.subMeshCount; sub++) count += part.GetIndexCount(sub);
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
