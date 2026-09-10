using System;
using System.Collections.Generic;
using System.Text;

namespace LinhGioi.World
{
    public enum TwoDEquipmentFitStatus
    {
        Candidate,
        Approved,
        RedrawRequired
    }

    public readonly struct TwoDEquipmentAttachmentDefinition
    {
        public TwoDEquipmentAttachmentDefinition(string componentId, string boneId, int sortOrder)
        {
            if (string.IsNullOrEmpty(componentId)) throw new ArgumentException("Component id is required", nameof(componentId));
            if (string.IsNullOrEmpty(boneId)) throw new ArgumentException("Bone id is required", nameof(boneId));
            ComponentId = componentId;
            BoneId = boneId;
            SortOrder = sortOrder;
        }

        public string ComponentId { get; }
        public string BoneId { get; }
        public int SortOrder { get; }
    }

    public sealed class TwoDCharacterFitProfile
    {
        private readonly HashSet<string> _bones;

        public TwoDCharacterFitProfile(string profileId, string skeletonVersion, string[] bones)
        {
            if (string.IsNullOrEmpty(profileId)) throw new ArgumentException("Profile id is required", nameof(profileId));
            if (string.IsNullOrEmpty(skeletonVersion)) throw new ArgumentException("Skeleton version is required", nameof(skeletonVersion));
            ProfileId = profileId;
            SkeletonVersion = skeletonVersion;
            _bones = RequireSet(bones, nameof(bones));
        }

        public string ProfileId { get; }
        public string SkeletonVersion { get; }
        public bool HasBone(string boneId) => _bones.Contains(boneId);

        internal static HashSet<string> RequireSet(string[] values, string name, bool allowEmpty = false)
        {
            if (values == null) throw new ArgumentNullException(name);
            if (!allowEmpty && values.Length == 0) throw new ArgumentException("At least one value is required", name);
            var result = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in values)
                if (string.IsNullOrEmpty(value) || !result.Add(value))
                    throw new ArgumentException("Values must be non-empty and unique", name);
            return result;
        }
    }

    public sealed class TwoDEquipmentItemDefinition
    {
        private readonly HashSet<string> _bodyProfiles;
        private readonly HashSet<string> _allowedClasses;

        public TwoDEquipmentItemDefinition(
            string itemId,
            string slotId,
            int unlockLevel,
            string skeletonVersion,
            string[] bodyProfiles,
            string[] allowedClasses,
            TwoDEquipmentFitStatus fitStatus,
            TwoDEquipmentAttachmentDefinition[] attachments,
            string[] coverageTags,
            string[] hideTags)
        {
            if (string.IsNullOrEmpty(itemId)) throw new ArgumentException("Item id is required", nameof(itemId));
            if (!TwoDEquipmentCompatibilityCatalog.IsCanonicalSlot(slotId))
                throw new ArgumentException("Unknown equipment slot: " + slotId, nameof(slotId));
            if (unlockLevel < 1) throw new ArgumentOutOfRangeException(nameof(unlockLevel));
            if (string.IsNullOrEmpty(skeletonVersion)) throw new ArgumentException("Skeleton version is required", nameof(skeletonVersion));
            if (attachments == null) throw new ArgumentNullException(nameof(attachments));
            if (attachments.Length == 0) throw new ArgumentException("At least one attachment is required", nameof(attachments));
            ItemId = itemId;
            SlotId = slotId;
            UnlockLevel = unlockLevel;
            SkeletonVersion = skeletonVersion;
            FitStatus = fitStatus;
            Attachments = (TwoDEquipmentAttachmentDefinition[])attachments.Clone();
            _bodyProfiles = TwoDCharacterFitProfile.RequireSet(bodyProfiles, nameof(bodyProfiles));
            _allowedClasses = TwoDCharacterFitProfile.RequireSet(allowedClasses, nameof(allowedClasses));
            CoverageTags = TwoDCharacterFitProfile.RequireSet(coverageTags, nameof(coverageTags), true);
            HideTags = TwoDCharacterFitProfile.RequireSet(hideTags, nameof(hideTags), true);
        }

        public string ItemId { get; }
        public string SlotId { get; }
        public int UnlockLevel { get; }
        public string SkeletonVersion { get; }
        public TwoDEquipmentFitStatus FitStatus { get; }
        public TwoDEquipmentAttachmentDefinition[] Attachments { get; }
        internal HashSet<string> CoverageTags { get; }
        internal HashSet<string> HideTags { get; }
        public bool SupportsBodyProfile(string profileId) => _bodyProfiles.Contains(profileId);
        public bool SupportsClass(string classId) => _allowedClasses.Contains(classId);
    }

    public sealed class TwoDEquipmentCompatibilityCatalog
    {
        public static readonly string[] CanonicalSlots =
        {
            "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
            "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory"
        };

        private readonly Dictionary<string, TwoDEquipmentItemDefinition> _items =
            new Dictionary<string, TwoDEquipmentItemDefinition>(StringComparer.Ordinal);

        public TwoDEquipmentCompatibilityCatalog(TwoDEquipmentItemDefinition[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
            {
                if (item == null) throw new ArgumentException("Items cannot contain null", nameof(items));
                if (!_items.TryAdd(item.ItemId, item)) throw new ArgumentException("Duplicate item id: " + item.ItemId, nameof(items));
            }
        }

        public static bool IsCanonicalSlot(string slotId) => Array.IndexOf(CanonicalSlots, slotId) >= 0;

        public bool TryEquip(TwoDEquipmentLoadout loadout, string itemId, int playerLevel, out string reason)
        {
            if (loadout == null) throw new ArgumentNullException(nameof(loadout));
            if (!_items.TryGetValue(itemId, out var item)) return Fail("ITEM_NOT_FOUND", out reason);
            if (item.FitStatus == TwoDEquipmentFitStatus.RedrawRequired) return Fail("ITEM_REDRAW_REQUIRED", out reason);
            if (item.FitStatus != TwoDEquipmentFitStatus.Approved) return Fail("ITEM_FIT_NOT_APPROVED", out reason);
            if (playerLevel < item.UnlockLevel) return Fail("ITEM_LEVEL_LOCKED", out reason);
            if (!item.SupportsClass(loadout.ClassId)) return Fail("CLASS_NOT_ALLOWED", out reason);
            if (!string.Equals(item.SkeletonVersion, loadout.Profile.SkeletonVersion, StringComparison.Ordinal))
                return Fail("SKELETON_VERSION_MISMATCH", out reason);
            if (!item.SupportsBodyProfile(loadout.Profile.ProfileId)) return Fail("BODY_PROFILE_MISMATCH", out reason);
            foreach (var attachment in item.Attachments)
                if (!loadout.Profile.HasBone(attachment.BoneId))
                    return Fail("ATTACHMENT_BONE_MISSING:" + attachment.BoneId, out reason);
            loadout.Equip(item);
            reason = "EQUIPPED";
            return true;
        }

        private static bool Fail(string value, out string reason)
        {
            reason = value;
            return false;
        }
    }

    public sealed class TwoDEquipmentLoadout
    {
        private readonly Dictionary<string, TwoDEquipmentItemDefinition> _equipped =
            new Dictionary<string, TwoDEquipmentItemDefinition>(StringComparer.Ordinal);

        public TwoDEquipmentLoadout(TwoDCharacterFitProfile profile, string classId)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            if (string.IsNullOrEmpty(classId)) throw new ArgumentException("Class id is required", nameof(classId));
            ClassId = classId;
        }

        public TwoDCharacterFitProfile Profile { get; }
        public string ClassId { get; }
        public string Snapshot => BuildSnapshot();

        public string GetEquippedItemId(string slotId)
        {
            return _equipped.TryGetValue(slotId, out var item) ? item.ItemId : null;
        }

        public bool IsCoverageVisible(string coverageTag)
        {
            var covered = false;
            foreach (var item in _equipped.Values)
            {
                if (item.HideTags.Contains(coverageTag)) return false;
                if (item.CoverageTags.Contains(coverageTag)) covered = true;
            }
            return covered;
        }

        internal void Equip(TwoDEquipmentItemDefinition item)
        {
            _equipped[item.SlotId] = item;
        }

        private string BuildSnapshot()
        {
            var builder = new StringBuilder("EquipmentCompatibility loadout profile=")
                .Append(Profile.ProfileId).Append(" class=").Append(ClassId)
                .Append(" skeleton=").Append(Profile.SkeletonVersion);
            foreach (var slot in TwoDEquipmentCompatibilityCatalog.CanonicalSlots)
                if (_equipped.TryGetValue(slot, out var item))
                    builder.Append(" | ").Append(slot).Append("=").Append(item.ItemId);
            return builder.ToString();
        }
    }
}
