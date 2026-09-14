using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace LinhGioi.Character
{
    public enum RigidBoneId
    {
        CharacterRoot,
        Pelvis,
        Torso,
        Chest,
        Neck,
        Head,
        ShoulderL,
        UpperArmL,
        ElbowL,
        LowerArmL,
        WristL,
        HandL,
        ShoulderR,
        UpperArmR,
        ElbowR,
        LowerArmR,
        WristR,
        HandR,
        HipL,
        UpperLegL,
        KneeL,
        LowerLegL,
        AnkleL,
        FootL,
        HipR,
        UpperLegR,
        KneeR,
        LowerLegR,
        AnkleR,
        FootR,
    }

    public enum RigidSortRole
    {
        BackAccessory = -80,
        BackHair = -70,
        RearWeapon = -60,
        RearClothing = -50,
        RearArm = -40,
        RearLeg = -30,
        Body = 0,
        MainClothing = 20,
        FrontLeg = 30,
        FrontArm = 40,
        Gloves = 50,
        Boots = 60,
        Waist = 70,
        FrontHair = 80,
        HeadGear = 90,
        FrontWeapon = 100,
        Fx = 120,
    }

    public sealed class RigidVisualPartDefinition
    {
        public RigidVisualPartDefinition(string partId, RigidBoneId targetBone, Sprite sprite,
            RigidSortRole sortRole, int sortingOffset, string coverage = "")
        {
            if (string.IsNullOrWhiteSpace(partId)) throw new ArgumentException("partId is required", nameof(partId));
            PartId = partId;
            TargetBone = targetBone;
            Sprite = sprite != null ? sprite : throw new ArgumentNullException(nameof(sprite));
            SortRole = sortRole;
            SortingOffset = sortingOffset;
            Coverage = coverage ?? string.Empty;
        }

        public string PartId { get; }
        public RigidBoneId TargetBone { get; }
        public Sprite Sprite { get; }
        public RigidSortRole SortRole { get; }
        public int SortingOffset { get; }
        public string Coverage { get; }
        public int SortingOrder => (int)SortRole + SortingOffset;
    }

    public sealed class RigidVisualBundle
    {
        public RigidVisualBundle(string bundleId, IEnumerable<RigidVisualPartDefinition> parts)
        {
            if (string.IsNullOrWhiteSpace(bundleId)) throw new ArgumentException("bundleId is required", nameof(bundleId));
            BundleId = bundleId;
            Parts = (parts ?? throw new ArgumentNullException(nameof(parts))).ToArray();
            if (Parts.Count == 0) throw new ArgumentException("A visual bundle must contain at least one part", nameof(parts));
            if (Parts.Select(part => part.PartId).Distinct(StringComparer.Ordinal).Count() != Parts.Count)
                throw new ArgumentException("Visual part IDs must be unique", nameof(parts));
        }

        public string BundleId { get; }
        public IReadOnlyList<RigidVisualPartDefinition> Parts { get; }
    }

    public sealed class RigidAttachmentFit
    {
        public RigidAttachmentFit(string partId, Vector2 localPosition, float localRotationDegrees,
            float overlapPixels)
        {
            if (string.IsNullOrWhiteSpace(partId)) throw new ArgumentException("partId is required", nameof(partId));
            if (overlapPixels < 0f) throw new ArgumentOutOfRangeException(nameof(overlapPixels));
            PartId = partId;
            LocalPosition = localPosition;
            LocalRotationDegrees = localRotationDegrees;
            OverlapPixels = overlapPixels;
        }

        public string PartId { get; }
        public Vector2 LocalPosition { get; }
        public float LocalRotationDegrees { get; }
        public float OverlapPixels { get; }
        public Vector3 LocalScale => Vector3.one;
    }

    public sealed class RigidFitProfile
    {
        private readonly RigidAttachmentFit[] _fits;

        public RigidFitProfile(string profileId, IEnumerable<RigidAttachmentFit> fits)
        {
            if (string.IsNullOrWhiteSpace(profileId)) throw new ArgumentException("profileId is required", nameof(profileId));
            ProfileId = profileId;
            _fits = (fits ?? throw new ArgumentNullException(nameof(fits))).ToArray();
        }

        public string ProfileId { get; }
        public IReadOnlyList<RigidAttachmentFit> Fits => _fits;

        public string Fingerprint
        {
            get
            {
                EnsureUniquePartIds();
                var rows = _fits.OrderBy(fit => fit.PartId, StringComparer.Ordinal).Select(fit => string.Join("|",
                    fit.PartId,
                    fit.LocalPosition.x.ToString("R", CultureInfo.InvariantCulture),
                    fit.LocalPosition.y.ToString("R", CultureInfo.InvariantCulture),
                    fit.LocalRotationDegrees.ToString("R", CultureInfo.InvariantCulture),
                    fit.OverlapPixels.ToString("R", CultureInfo.InvariantCulture),
                    "1,1,1"));
                return Sha256(ProfileId + "\n" + string.Join("\n", rows));
            }
        }

        public RigidAttachmentFit Require(string partId)
        {
            EnsureUniquePartIds();
            return _fits.SingleOrDefault(fit => fit.PartId == partId)
                ?? throw new InvalidOperationException("Fit profile has no entry for visual part: " + partId);
        }

        private void EnsureUniquePartIds()
        {
            if (_fits.Select(fit => fit.PartId).Distinct(StringComparer.Ordinal).Count() != _fits.Length)
                throw new ArgumentException("Fit part IDs must be unique");
        }

        internal static string Sha256(string value)
        {
            using var hash = SHA256.Create();
            return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(valueByte => valueByte.ToString("x2")));
        }
    }
}
