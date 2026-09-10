using NUnit.Framework;
using LinhGioi.World;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDEquipmentCompatibilityTests
    {
        private static TwoDCharacterFitProfile MaleProfile()
        {
            return new TwoDCharacterFitProfile(
                "common_male_v1",
                "lgo_humanoid_2d_v1",
                new[] { "head", "torso", "hips", "weapon_r" });
        }

        private static TwoDEquipmentItemDefinition Item(
            string id,
            string slot,
            int unlockLevel,
            TwoDEquipmentFitStatus status = TwoDEquipmentFitStatus.Approved,
            string bodyProfile = "common_male_v1",
            string skeletonVersion = "lgo_humanoid_2d_v1",
            string bone = "torso",
            string[] coverage = null,
            string[] hides = null)
        {
            return new TwoDEquipmentItemDefinition(
                id,
                slot,
                unlockLevel,
                skeletonVersion,
                new[] { bodyProfile },
                new[] { "kiem" },
                status,
                new[] { new TwoDEquipmentAttachmentDefinition(id + "_part", bone, 100) },
                coverage ?? new string[0],
                hides ?? new string[0]);
        }

        [Test]
        public void ApprovedItemsFromDifferentLevelsEquipIndependentlyBySlot()
        {
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[]
            {
                Item("kiem_lv001_weapon", "main_weapon", 1, bone: "weapon_r"),
                Item("kiem_lv030_hair", "head_hair", 30, bone: "head"),
                Item("kiem_lv010_inner", "inner_top", 10),
                Item("kiem_lv020_outer", "outer_top", 20)
            });
            var loadout = new TwoDEquipmentLoadout(MaleProfile(), "kiem");

            Assert.That(catalog.TryEquip(loadout, "kiem_lv001_weapon", 30, out _), Is.True);
            Assert.That(catalog.TryEquip(loadout, "kiem_lv030_hair", 30, out _), Is.True);
            Assert.That(catalog.TryEquip(loadout, "kiem_lv010_inner", 30, out _), Is.True);
            Assert.That(catalog.TryEquip(loadout, "kiem_lv020_outer", 30, out _), Is.True);

            Assert.That(loadout.GetEquippedItemId("main_weapon"), Is.EqualTo("kiem_lv001_weapon"));
            Assert.That(loadout.GetEquippedItemId("head_hair"), Is.EqualTo("kiem_lv030_hair"));
            Assert.That(loadout.GetEquippedItemId("inner_top"), Is.EqualTo("kiem_lv010_inner"));
            Assert.That(loadout.GetEquippedItemId("outer_top"), Is.EqualTo("kiem_lv020_outer"));
            StringAssert.Contains("main_weapon=kiem_lv001_weapon", loadout.Snapshot);
            StringAssert.Contains("head_hair=kiem_lv030_hair", loadout.Snapshot);
        }

        [Test]
        public void UnlockLevelGatesEligibilityButDoesNotRequireOneLoadoutTier()
        {
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[]
            {
                Item("kiem_lv001_weapon", "main_weapon", 1, bone: "weapon_r"),
                Item("kiem_lv030_hair", "head_hair", 30, bone: "head")
            });
            var loadout = new TwoDEquipmentLoadout(MaleProfile(), "kiem");

            Assert.That(catalog.TryEquip(loadout, "kiem_lv030_hair", 20, out var reason), Is.False);
            Assert.That(reason, Is.EqualTo("ITEM_LEVEL_LOCKED"));
            Assert.That(catalog.TryEquip(loadout, "kiem_lv001_weapon", 20, out _), Is.True);
            Assert.That(loadout.GetEquippedItemId("main_weapon"), Is.EqualTo("kiem_lv001_weapon"));
        }

        [TestCase(TwoDEquipmentFitStatus.Candidate, "ITEM_FIT_NOT_APPROVED")]
        [TestCase(TwoDEquipmentFitStatus.RedrawRequired, "ITEM_REDRAW_REQUIRED")]
        public void ExtractedCandidatesCannotEnterRuntimeLoadout(
            TwoDEquipmentFitStatus status,
            string expectedReason)
        {
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[]
            {
                Item("kiem_crop", "outer_top", 1, status)
            });

            Assert.That(catalog.TryEquip(new TwoDEquipmentLoadout(MaleProfile(), "kiem"), "kiem_crop", 30, out var reason), Is.False);
            Assert.That(reason, Is.EqualTo(expectedReason));
        }

        [Test]
        public void SkeletonBodyAndAttachmentBonesMustMatchTheBaseProfile()
        {
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[]
            {
                Item("wrong_skeleton", "outer_top", 1, skeletonVersion: "other_rig"),
                Item("wrong_body", "outer_top", 1, bodyProfile: "common_female_v1"),
                Item("missing_bone", "outer_top", 1, bone: "cape_root")
            });
            var loadout = new TwoDEquipmentLoadout(MaleProfile(), "kiem");

            Assert.That(catalog.TryEquip(loadout, "wrong_skeleton", 30, out var skeletonReason), Is.False);
            Assert.That(skeletonReason, Is.EqualTo("SKELETON_VERSION_MISMATCH"));
            Assert.That(catalog.TryEquip(loadout, "wrong_body", 30, out var bodyReason), Is.False);
            Assert.That(bodyReason, Is.EqualTo("BODY_PROFILE_MISMATCH"));
            Assert.That(catalog.TryEquip(loadout, "missing_bone", 30, out var boneReason), Is.False);
            Assert.That(boneReason, Is.EqualTo("ATTACHMENT_BONE_MISSING:cape_root"));
        }

        [Test]
        public void CoverageAndOcclusionAreComposedFromEquippedItems()
        {
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[]
            {
                Item("inner", "inner_top", 1, coverage: new[] { "torso_inner" }),
                Item("outer", "outer_top", 20,
                    coverage: new[] { "torso_outer" },
                    hides: new[] { "torso_inner" })
            });
            var loadout = new TwoDEquipmentLoadout(MaleProfile(), "kiem");
            Assert.That(catalog.TryEquip(loadout, "inner", 30, out _), Is.True);
            Assert.That(catalog.TryEquip(loadout, "outer", 30, out _), Is.True);

            Assert.That(loadout.IsCoverageVisible("torso_inner"), Is.False);
            Assert.That(loadout.IsCoverageVisible("torso_outer"), Is.True);
        }

        [Test]
        public void ClassRestrictionIsIndependentFromBodyAndLevelCompatibility()
        {
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[]
            {
                Item("kiem_weapon", "main_weapon", 1, bone: "weapon_r")
            });

            Assert.That(catalog.TryEquip(
                new TwoDEquipmentLoadout(MaleProfile(), "vo"),
                "kiem_weapon",
                30,
                out var reason), Is.False);
            Assert.That(reason, Is.EqualTo("CLASS_NOT_ALLOWED"));
        }
    }
}
