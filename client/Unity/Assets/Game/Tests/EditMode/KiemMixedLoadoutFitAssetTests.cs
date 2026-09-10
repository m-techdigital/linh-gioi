using System.Linq;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.U2D;

namespace LinhGioi.Tests.EditMode
{
    public sealed class KiemMixedLoadoutFitAssetTests
    {
        private const string Atlas = "LGOClasses/KiemMixedLoadoutFitPreview/kiem-mixed-loadout-fit-atlas";

        [Test]
        public void ProofPackUsesOneBoundedAtlasWithEightNamedSprites()
        {
            var sprites = Resources.LoadAll<Sprite>(Atlas);
            Assert.That(sprites.Length, Is.EqualTo(8));
            Assert.That(sprites.Select(sprite => sprite.name).Distinct().Count(), Is.EqualTo(8));
            Assert.That(sprites.All(sprite => sprite.texture.width == 512 && sprite.texture.height == 512), Is.True);
            Assert.That(sprites.All(sprite => Mathf.Approximately(sprite.pixelsPerUnit, 208)), Is.True);
        }

        [TestCase("kiem-lv030-male-head_hair-fit-v1")]
        [TestCase("kiem-lv030-female-head_hair-fit-v1")]
        [TestCase("kiem-lv020-male-outer_top-fit-v1")]
        [TestCase("kiem-lv020-female-outer_top-fit-v1")]
        [TestCase("kiem-lv010-male-inner_top")]
        [TestCase("kiem-lv010-female-inner_top")]
        public void SkinnedProofSpriteHasCanonicalImportAndBoneData(string spriteName)
        {
            var sprite = Resources.LoadAll<Sprite>(Atlas).SingleOrDefault(candidate => candidate.name == spriteName);
            Assert.That(sprite, Is.Not.Null, spriteName);
            Assert.That(sprite.GetBones().Length, Is.GreaterThan(0), spriteName);
            Assert.That(sprite.GetVertexAttribute<BoneWeight>(
                UnityEngine.Rendering.VertexAttribute.BlendWeight).Length, Is.EqualTo(sprite.GetVertexCount()));
        }

        [TestCase("kiem-lv001-male-main_weapon")]
        [TestCase("kiem-lv001-female-main_weapon")]
        public void RigidWeaponIsAvailableByStableAtlasLabel(string spriteName)
        {
            Assert.That(Resources.LoadAll<Sprite>(Atlas).SingleOrDefault(candidate => candidate.name == spriteName), Is.Not.Null);
        }

        [Test]
        public void DraftPreviewUsesSharedSkeletonAndNeverBecomesProductionEligible()
        {
            var root = new GameObject("Kiếm preview test root");
            TwoDKiemMixedLoadoutFitPreview preview = null;
            try
            {
                var rig = new TwoDSkeletalPaperDollRig(root.transform);
                rig.Build(new[]
                {
                    Bone("male_torso-hips", "", 0, .7f), Bone("male_head", "male_torso-hips", 0, 1.32f),
                    Bone("male_right-upper-arm", "male_torso-hips", .17f, 1.3f),
                    Bone("male_right-forearm-hand", "male_right-upper-arm", .22f, .95f),
                    Bone("female_torso-hips", "", 0, .7f), Bone("female_head", "female_torso-hips", 0, 1.32f),
                    Bone("female_right-upper-arm", "female_torso-hips", .17f, 1.3f),
                    Bone("female_right-forearm-hand", "female_right-upper-arm", .22f, .95f)
                });
                preview = new TwoDKiemMixedLoadoutFitPreview(root.transform, rig);
                preview.SetActive(true, "male", "walk");
                Assert.That(preview.VisibleItemCount, Is.EqualTo(4));
                Assert.That(preview.ValidSpriteSkinCount, Is.EqualTo(3));
                StringAssert.Contains("runtimeEligibleCount=0", preview.Snapshot);
                StringAssert.Contains("productionEquipAllowed=False", preview.Snapshot);
                preview.SetActive(true, "female", "class_skill");
                Assert.That(preview.VisibleItemCount, Is.EqualTo(4));
                Assert.That(preview.ValidSpriteSkinCount, Is.EqualTo(3));
                StringAssert.Contains("gender=female", preview.Snapshot);
            }
            finally
            {
                preview?.Dispose();
                Object.DestroyImmediate(root);
            }
        }

        private static TwoDSkeletalPaperDollRig.BoneDefinition Bone(string id, string parent, float x, float y)
        {
            return new TwoDSkeletalPaperDollRig.BoneDefinition(id, parent, new Vector2(x, y), id);
        }
    }
}
