using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinhGioi.Character
{
    public sealed class RigidOutfitPilotCharacter
    {
        internal RigidOutfitPilotCharacter(string gender, RigidSkeletonDefinition skeleton,
            RigidVisualBundle body, RigidFitProfile bodyFit, RigidVisualBundle outfit, RigidFitProfile outfitFit,
            RigidVisualBundle weapon, RigidFitProfile weaponFit)
        {
            Gender = gender;
            Skeleton = skeleton;
            Body = body;
            BodyFit = bodyFit;
            Outfit = outfit;
            OutfitFit = outfitFit;
            Weapon = weapon;
            WeaponFit = weaponFit;
        }

        public string Gender { get; }
        public RigidSkeletonDefinition Skeleton { get; }
        public RigidVisualBundle Body { get; }
        public RigidFitProfile BodyFit { get; }
        public RigidVisualBundle Outfit { get; }
        public RigidFitProfile OutfitFit { get; }
        public RigidVisualBundle Weapon { get; }
        public RigidFitProfile WeaponFit { get; }
    }

    public sealed class RigidOutfitPilotCatalog
    {
        public const float PixelsPerUnit = 158.73016f;
        public const string SourceSpaceProfile = "lgo_character_canvas_1024x1536_v1";
        private const float HeadNeckAnchorCorrection = 48f / PixelsPerUnit;

        private RigidOutfitPilotCatalog(RigidOutfitPilotCharacter male, RigidOutfitPilotCharacter female)
        {
            Male = male;
            Female = female;
        }

        public RigidOutfitPilotCharacter Male { get; }
        public RigidOutfitPilotCharacter Female { get; }

        public static RigidOutfitPilotCatalog LoadFromResources() => Create(name =>
            Resources.Load<Sprite>("LGORigidPilot/rigid-source-v2/" + name) ??
            Resources.Load<Sprite>("LGORigidPilot/v1/" + name));

        public static RigidOutfitPilotCatalog Create(Func<string, Sprite> spriteResolver)
        {
            if (spriteResolver == null) throw new ArgumentNullException(nameof(spriteResolver));
            Sprite Require(string name)
            {
                var sprite = spriteResolver(name);
                return sprite != null ? sprite : throw new InvalidOperationException("Rigid pilot sprite is missing: " + name);
            }
            var skeleton = RigidSkeletonDefinition.ChibiSideView();
            var bind = skeleton.Bones.ToDictionary(bone => bone.Id, bone => bone.BindPosition);

            var maleBody = new RigidVisualBundle("body.chibi.side.male.v1", new[]
            {
                Part("hair.back", RigidBoneId.Head, "hair_back", RigidSortRole.BackHair),
                Part("arm.far.upper", RigidBoneId.UpperArmR, "upper_arm_far", RigidSortRole.RearArm),
                Part("arm.far.forearm", RigidBoneId.LowerArmR, "forearm_far", RigidSortRole.RearArm, 1),
                Part("hand.far", RigidBoneId.HandR, "hand_far", RigidSortRole.RearArm, 2),
                Part("leg.far.upper", RigidBoneId.UpperLegR, "thigh_far", RigidSortRole.RearLeg),
                Part("leg.far.shin", RigidBoneId.LowerLegR, "shin_far", RigidSortRole.RearLeg, 1),
                Part("foot.far", RigidBoneId.FootR, "foot_far", RigidSortRole.RearLeg, 2),
                Part("body.underlayer", RigidBoneId.Torso, "body_underlayer", RigidSortRole.Body),
                Part("leg.near.upper", RigidBoneId.UpperLegL, "thigh_near", RigidSortRole.Body, 1),
                Part("leg.near.shin", RigidBoneId.LowerLegL, "shin_near", RigidSortRole.FrontLeg, 1),
                Part("foot.near", RigidBoneId.FootL, "foot_near", RigidSortRole.FrontLeg, 2),
                Part("arm.near.upper", RigidBoneId.UpperArmL, "upper_arm_near", RigidSortRole.FrontArm),
                Part("arm.near.forearm", RigidBoneId.LowerArmL, "forearm_near", RigidSortRole.FrontArm, 1),
                Part("hand.near", RigidBoneId.HandL, "hand_near", RigidSortRole.FrontArm, 2),
                Part("head.profile", RigidBoneId.Head, "head_profile", RigidSortRole.Body, 5),
                Part("hair.front", RigidBoneId.Head, "hair_front", RigidSortRole.FrontHair),
                Part("hair.charm", RigidBoneId.Head, "hair_charm", RigidSortRole.HeadGear),
            });
            var maleBodyFit = new RigidFitProfile("body.chibi.side.male.fit.v1", new[]
            {
                Fit("hair.back", -.24f, .90f - HeadNeckAnchorCorrection, 14f),
                Fit("arm.far.upper", .01260f, -.37800f, 12f),
                Fit("arm.far.forearm", .06615f, -.49140f, 18f),
                Reanchor("hand.far", .06615f, -.49140f, RigidBoneId.LowerArmR, RigidBoneId.HandR, 36f),
                Fit("leg.far.upper", .01260f, -.49455f, 13f),
                Fit("leg.far.shin", .09135f, -.53235f, 20f),
                Reanchor("foot.far", .09135f, -.53235f, RigidBoneId.LowerLegR, RigidBoneId.FootR, 40f),
                Reanchor("body.underlayer", -.04f, .845f, RigidBoneId.Pelvis, RigidBoneId.Torso, 18f),
                Fit("leg.near.upper", -.01890f, -.48510f, 13f),
                Fit("leg.near.shin", .09765f, -.53550f, 20f),
                Reanchor("foot.near", .09765f, -.53550f, RigidBoneId.LowerLegL, RigidBoneId.FootL, 40f),
                Fit("arm.near.upper", -.00945f, -.38745f, 12f),
                Fit("arm.near.forearm", .02835f, -.48825f, 18f),
                Reanchor("hand.near", .02835f, -.48825f, RigidBoneId.LowerArmL, RigidBoneId.HandL, 36f),
                Fit("head.profile", .26f, .90f - HeadNeckAnchorCorrection, 16f),
                Fit("hair.front", .04f, .95f - HeadNeckAnchorCorrection, 14f),
                Fit("hair.charm", -.46f, 1.39f - HeadNeckAnchorCorrection, 10f),
            });

            var femaleBody = new RigidVisualBundle("body.chibi.side.female.v1", new[]
            {
                Part("female.head.hair", RigidBoneId.Head, "female_head_hair", RigidSortRole.FrontHair),
                Part("arm.far.upper", RigidBoneId.UpperArmR, "upper_arm_far", RigidSortRole.RearArm),
                Part("arm.far.forearm", RigidBoneId.LowerArmR, "forearm_far", RigidSortRole.RearArm, 1),
                Part("hand.far", RigidBoneId.HandR, "hand_far", RigidSortRole.RearArm, 2),
                Part("leg.far.upper", RigidBoneId.UpperLegR, "thigh_far", RigidSortRole.RearLeg),
                Part("leg.far.shin", RigidBoneId.LowerLegR, "shin_far", RigidSortRole.RearLeg, 1),
                Part("foot.far", RigidBoneId.FootR, "foot_far", RigidSortRole.RearLeg, 2),
                Part("body.underlayer", RigidBoneId.Torso, "body_underlayer", RigidSortRole.Body),
                Part("leg.near.upper", RigidBoneId.UpperLegL, "thigh_near", RigidSortRole.Body, 1),
                Part("leg.near.shin", RigidBoneId.LowerLegL, "shin_near", RigidSortRole.FrontLeg, 1),
                Part("foot.near", RigidBoneId.FootL, "foot_near", RigidSortRole.FrontLeg, 2),
                Part("arm.near.upper", RigidBoneId.UpperArmL, "upper_arm_near", RigidSortRole.FrontArm),
                Part("arm.near.forearm", RigidBoneId.LowerArmL, "forearm_near", RigidSortRole.FrontArm, 1),
                Part("hand.near", RigidBoneId.HandL, "hand_near", RigidSortRole.FrontArm, 2),
            });
            var femaleBodyFit = new RigidFitProfile("body.chibi.side.female.fit.v1", new[]
            {
                Fit("female.head.hair", -.14f, .99f - HeadNeckAnchorCorrection, 16f),
                Fit("arm.far.upper", .01260f, -.37800f, 12f),
                Fit("arm.far.forearm", .06615f, -.49140f, 18f),
                Reanchor("hand.far", .06615f, -.49140f, RigidBoneId.LowerArmR, RigidBoneId.HandR, 36f),
                Fit("leg.far.upper", .01260f, -.49455f, 13f),
                Fit("leg.far.shin", .09135f, -.53235f, 20f),
                Reanchor("foot.far", .09135f, -.53235f, RigidBoneId.LowerLegR, RigidBoneId.FootR, 40f),
                Reanchor("body.underlayer", -.04f, .845f, RigidBoneId.Pelvis, RigidBoneId.Torso, 18f),
                Fit("leg.near.upper", -.01890f, -.48510f, 13f),
                Fit("leg.near.shin", .09765f, -.53550f, 20f),
                Reanchor("foot.near", .09765f, -.53550f, RigidBoneId.LowerLegL, RigidBoneId.FootL, 40f),
                Fit("arm.near.upper", -.00945f, -.38745f, 12f),
                Fit("arm.near.forearm", .02835f, -.48825f, 18f),
                Reanchor("hand.near", .02835f, -.48825f, RigidBoneId.LowerArmL, RigidBoneId.HandL, 36f),
            });

            var outfit = new RigidVisualBundle("outfit.pilot.teal.v1", new[]
            {
                Part("tunic.back.far", RigidBoneId.Torso, "tunic_back_far", RigidSortRole.RearClothing),
                Part("sleeve.far.upper", RigidBoneId.UpperArmR, "upper_sleeve_far", RigidSortRole.RearClothing, 2),
                Part("bracer.far", RigidBoneId.LowerArmR, "bracer_far", RigidSortRole.RearClothing, 3),
                Part("glove.far", RigidBoneId.HandR, "glove_far", RigidSortRole.RearClothing, 4),
                Part("boot.far.shaft", RigidBoneId.LowerLegR, "boot_shaft_far", RigidSortRole.RearClothing, 4),
                Part("boot.far.foot", RigidBoneId.FootR, "boot_foot_far", RigidSortRole.RearClothing, 5),
                Part("tunic.torso", RigidBoneId.Torso, "tunic_torso_core", RigidSortRole.MainClothing),
                Part("sleeve.near.upper", RigidBoneId.UpperArmL, "upper_sleeve_near", RigidSortRole.FrontArm, 2),
                Part("bracer.near", RigidBoneId.LowerArmL, "bracer_near", RigidSortRole.Gloves),
                Part("glove.near", RigidBoneId.HandL, "glove_near", RigidSortRole.Gloves, 1),
                Part("boot.near.shaft", RigidBoneId.LowerLegL, "boot_shaft_near", RigidSortRole.Boots),
                Part("boot.near.foot", RigidBoneId.FootL, "boot_foot_near", RigidSortRole.Boots, 1),
                Part("tunic.lower", RigidBoneId.Pelvis, "lower_tunic_hip", RigidSortRole.MainClothing, 4),
                Part("waist.sash", RigidBoneId.Pelvis, "waist_belt_tail", RigidSortRole.Waist),
                Part("waist.jade", RigidBoneId.Pelvis, "jade_accessory", RigidSortRole.Waist, 2),
            });
            var outfitFit = new RigidFitProfile("outfit.pilot.teal.fit.v1", new[]
            {
                Fit("tunic.back.far", -.03f, .44f, 18f),
                Fit("sleeve.far.upper", .00315f, -.43470f, 14f),
                Fit("bracer.far", .01260f, -.41895f, 16f),
                Reanchor("glove.far", .01260f, -.41895f, RigidBoneId.LowerArmR, RigidBoneId.HandR, 32f),
                Fit("boot.far.shaft", .12285f, -.51975f, 20f),
                Reanchor("boot.far.foot", .12285f, -.51975f, RigidBoneId.LowerLegR, RigidBoneId.FootR, 40f),
                Fit("tunic.torso", -.03f, .44f, 18f),
                Fit("sleeve.near.upper", .00315f, -.44415f, 14f),
                Fit("bracer.near", .01890f, -.40005f, 16f),
                Reanchor("glove.near", .01890f, -.40005f, RigidBoneId.LowerArmL, RigidBoneId.HandL, 32f),
                Fit("boot.near.shaft", .11655f, -.54810f, 20f),
                Reanchor("boot.near.foot", .11655f, -.54810f, RigidBoneId.LowerLegL, RigidBoneId.FootL, 40f),
                Fit("tunic.lower", 0f, .09f, 18f),
                Fit("waist.sash", -.22f, .32f, 18f),
                Fit("waist.jade", .24f, .22f, 12f),
            });
            var weapon = new RigidVisualBundle("weapon.pilot.dao.v1", new[]
            {
                Part("weapon.dao", RigidBoneId.HandL, "pilot_dao", RigidSortRole.FrontWeapon),
            });
            var weaponFit = new RigidFitProfile("weapon.pilot.dao.fit.v1", new[]
            {
                Fit("weapon.dao", 0f, -.78f, 12f),
            });

            var male = new RigidOutfitPilotCharacter("male", skeleton, maleBody, maleBodyFit, outfit, outfitFit, weapon, weaponFit);
            var female = new RigidOutfitPilotCharacter("female", skeleton, femaleBody, femaleBodyFit, outfit, outfitFit, weapon, weaponFit);
            return new RigidOutfitPilotCatalog(male, female);

            RigidVisualPartDefinition Part(string id, RigidBoneId bone, string sprite, RigidSortRole role, int order = 0) =>
                new(id, bone, Require(sprite), role, order);
            static RigidAttachmentFit Fit(string id, float x, float y, float overlap) =>
                new(id, new Vector2(x, y), 0f, overlap);
            RigidAttachmentFit Reanchor(string id, float originalX, float originalY,
                RigidBoneId originalBone, RigidBoneId targetBone, float overlap) =>
                new(id, bind[originalBone] + new Vector2(originalX, originalY) - bind[targetBone], 0f, overlap);
        }
    }
}
