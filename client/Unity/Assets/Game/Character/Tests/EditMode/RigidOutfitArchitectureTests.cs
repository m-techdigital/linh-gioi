using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using LinhGioi.Character.Editor;

namespace LinhGioi.Character.Tests
{
    public sealed class RigidOutfitArchitectureTests
    {
        private Texture2D _texture;
        private Sprite _sprite;

        [SetUp]
        public void SetUp()
        {
            _texture = new Texture2D(2, 2);
            _sprite = Sprite.Create(_texture, new Rect(0, 0, 2, 2), new Vector2(.5f, .5f), 100f);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_sprite);
            UnityEngine.Object.DestroyImmediate(_texture);
        }

        [Test]
        public void Skeleton_UsesCompleteCanonicalSemanticsAndSortingGroup()
        {
            var root = new GameObject("actor");
            try
            {
                var skeleton = RigidCharacterSkeleton.Create(root.transform, RigidSkeletonDefinition.ChibiSideView());

                CollectionAssert.AreEquivalent(Enum.GetValues(typeof(RigidBoneId)), skeleton.BoneIds);
                Assert.That(root.GetComponent<SortingGroup>(), Is.Not.Null);
                Assert.That(skeleton.CharacterRoot.localScale, Is.EqualTo(Vector3.one));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Equip_AttachesOneSpritePerPartAndKeepsAuthoredFit()
        {
            var root = new GameObject("actor");
            try
            {
                var skeleton = RigidCharacterSkeleton.Create(root.transform, RigidSkeletonDefinition.ChibiSideView());
                var bundle = new RigidVisualBundle("pilot.outfit.01", new[]
                {
                    new RigidVisualPartDefinition("chest", RigidBoneId.Chest, _sprite, RigidSortRole.MainClothing, 0),
                    new RigidVisualPartDefinition("sleeve.near", RigidBoneId.UpperArmL, _sprite, RigidSortRole.FrontArm, 0),
                });
                var profile = new RigidFitProfile("pilot.male", new[]
                {
                    new RigidAttachmentFit("chest", new Vector2(.1f, .2f), 3f, 10f),
                    new RigidAttachmentFit("sleeve.near", new Vector2(.3f, .4f), -2f, 8f),
                });

                var equipped = new RigidOutfitBinder(skeleton).Equip("outfit", bundle, profile);

                Assert.That(equipped.Renderers.Count, Is.EqualTo(2));
                Assert.That(equipped.Renderers.All(renderer => renderer.sprite == _sprite), Is.True);
                Assert.That(equipped.Renderers.All(renderer => renderer.transform.localScale == Vector3.one), Is.True);
                Assert.That(equipped.Renderers.Single(r => r.name == "chest").transform.parent,
                    Is.EqualTo(skeleton[RigidBoneId.Chest]));
                Assert.That(equipped.FitFingerprint, Is.EqualTo(profile.Fingerprint));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Motion_ChangesOnlyBonePositionAndRotationAndPreservesEquippedVisuals()
        {
            var root = new GameObject("actor");
            try
            {
                var skeleton = RigidCharacterSkeleton.Create(root.transform, RigidSkeletonDefinition.ChibiSideView());
                var bundle = new RigidVisualBundle("pilot.outfit.01", new[]
                {
                    new RigidVisualPartDefinition("chest", RigidBoneId.Chest, _sprite, RigidSortRole.MainClothing, 0),
                });
                var profile = new RigidFitProfile("pilot.male", new[]
                {
                    new RigidAttachmentFit("chest", Vector2.zero, 0f, 10f),
                });
                var equipped = new RigidOutfitBinder(skeleton).Equip("outfit", bundle, profile);
                var animator = new RigidCharacterAnimator(skeleton);
                var spriteBefore = equipped.SpriteFingerprint;
                var fitBefore = equipped.FitFingerprint;

                foreach (var state in new[]
                {
                    RigidMotionState.Idle, RigidMotionState.Walk, RigidMotionState.Run,
                    RigidMotionState.Jump, RigidMotionState.Attack, RigidMotionState.Roll, RigidMotionState.Hit,
                })
                {
                    animator.Apply(RigidMotionLibrary.Sample(state, .37f));
                    Assert.That(equipped.SpriteFingerprint, Is.EqualTo(spriteBefore), state.ToString());
                    Assert.That(equipped.FitFingerprint, Is.EqualTo(fitBefore), state.ToString());
                    Assert.That(equipped.Renderers.All(renderer => renderer.transform.localScale == Vector3.one), Is.True);
                    Assert.That(skeleton.AllTransforms.All(transform => transform.localScale == Vector3.one), Is.True);
                    Assert.That(skeleton.CharacterRoot.localRotation, Is.EqualTo(Quaternion.identity));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Equip_RejectsMissingOrDuplicateFitEntries()
        {
            var root = new GameObject("actor");
            try
            {
                var skeleton = RigidCharacterSkeleton.Create(root.transform, RigidSkeletonDefinition.ChibiSideView());
                var bundle = new RigidVisualBundle("pilot.outfit.01", new[]
                {
                    new RigidVisualPartDefinition("chest", RigidBoneId.Chest, _sprite, RigidSortRole.MainClothing, 0),
                });
                var missing = new RigidFitProfile("missing", Array.Empty<RigidAttachmentFit>());
                var duplicate = new RigidFitProfile("duplicate", new[]
                {
                    new RigidAttachmentFit("chest", Vector2.zero, 0f, 4f),
                    new RigidAttachmentFit("chest", Vector2.zero, 0f, 4f),
                });

                Assert.Throws<InvalidOperationException>(() => new RigidOutfitBinder(skeleton).Equip("outfit", bundle, missing));
                Assert.Throws<ArgumentException>(() => _ = duplicate.Fingerprint);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void MotionLibrary_HasRequiredStatesWithoutOutfitOrScaleChannels()
        {
            foreach (RigidMotionState state in Enum.GetValues(typeof(RigidMotionState)))
            {
                var pose = RigidMotionLibrary.Sample(state, .25f);
                Assert.That(pose.Bones.Keys, Is.SubsetOf(Enum.GetValues(typeof(RigidBoneId)).Cast<RigidBoneId>()));
                Assert.That(pose.GetType().GetFields().Any(field => field.Name.Contains("scale", StringComparison.OrdinalIgnoreCase)), Is.False);
                Assert.That(pose.GetType().GetFields().Any(field => field.Name.Contains("outfit", StringComparison.OrdinalIgnoreCase)), Is.False);
            }
        }

        [Test]
        public void PilotCatalog_UsesOnePoseIndependentOutfitForBothGenders()
        {
            var catalog = RigidOutfitPilotCatalog.Create(_ => _sprite);
            var forbidden = new[] { "idle", "walk", "run", "attack", "roll", "frame", "pose" };

            Assert.That(catalog.Male.Outfit, Is.SameAs(catalog.Female.Outfit));
            Assert.That(catalog.Male.Weapon, Is.SameAs(catalog.Female.Weapon));
            Assert.That(catalog.Male.Weapon.Parts.Single().TargetBone, Is.EqualTo(RigidBoneId.HandL));
            Assert.That(catalog.Male.Body, Is.Not.SameAs(catalog.Female.Body));
            Assert.That(catalog.Female.Body.Parts.Single(part => part.PartId == "female.head.hair").TargetBone,
                Is.EqualTo(RigidBoneId.Head));
            Assert.That(catalog.Male.Skeleton.Bones.Select(bone => bone.Id),
                Is.EqualTo(catalog.Female.Skeleton.Bones.Select(bone => bone.Id)));
            Assert.That(catalog.Male.Body.Parts.Count, Is.GreaterThanOrEqualTo(12));
            Assert.That(catalog.Male.Outfit.Parts.Count, Is.GreaterThanOrEqualTo(9));
            CollectionAssert.IsSubsetOf(new[]
            {
                RigidBoneId.HandL, RigidBoneId.HandR, RigidBoneId.FootL, RigidBoneId.FootR,
            }, catalog.Male.Body.Parts.Select(part => part.TargetBone).Distinct().ToArray());
            CollectionAssert.IsSubsetOf(new[]
            {
                RigidBoneId.HandL, RigidBoneId.HandR, RigidBoneId.FootL, RigidBoneId.FootR,
            }, catalog.Male.Outfit.Parts.Select(part => part.TargetBone).Distinct().ToArray());
            Assert.That(catalog.Male.Outfit.Parts.Select(part => part.PartId)
                .Any(id => forbidden.Any(token => id.Contains(token, StringComparison.OrdinalIgnoreCase))), Is.False);
            Assert.That(catalog.Male.Outfit.Parts.All(part => part.Sprite == _sprite), Is.True);
        }

        [Test]
        public void PilotActor_RunsEveryStateWithoutRebuildingVisualHierarchy()
        {
            var root = new GameObject("actor");
            try
            {
                var catalog = RigidOutfitPilotCatalog.Create(_ => _sprite);
                var actor = RigidOutfitPilotActor.Create(root.transform, catalog.Male);
                var initialVisuals = root.GetComponentsInChildren<SpriteRenderer>(true);
                var spriteFingerprint = actor.SpriteFingerprint;
                var fitFingerprint = actor.FitFingerprint;

                foreach (RigidMotionState state in Enum.GetValues(typeof(RigidMotionState)))
                    actor.Apply(state, .61f);

                CollectionAssert.AreEquivalent(initialVisuals, root.GetComponentsInChildren<SpriteRenderer>(true));
                Assert.That(actor.SpriteFingerprint, Is.EqualTo(spriteFingerprint));
                Assert.That(actor.FitFingerprint, Is.EqualTo(fitFingerprint));
                Assert.That(actor.HasUnitScale, Is.True);
                Assert.That(actor.HasForbiddenRendererComponents, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CapturePlan_CoversMaleAndFemaleAcrossRequiredMotionStates()
        {
            var frames = RigidOutfitPilotCapturePlan.Frames;

            Assert.That(frames.Count, Is.EqualTo(15));
            CollectionAssert.IsSubsetOf(new[]
            {
                RigidMotionState.Idle, RigidMotionState.Walk, RigidMotionState.Run,
                RigidMotionState.Jump, RigidMotionState.Attack, RigidMotionState.Roll,
            }, frames.Select(frame => frame.State).Distinct().ToArray());
            Assert.That(frames.Count(frame => frame.State == RigidMotionState.Run), Is.GreaterThanOrEqualTo(4));
            Assert.That(RigidOutfitPilotCapturePlan.VideoFps, Is.EqualTo(24));
            Assert.That(frames.All(frame => frame.NormalizedTime is >= 0f and <= 1f), Is.True);
        }

        [Test]
        public void MotionBlend_InterpolatesEveryBoneAcrossTheShortestRotationArc()
        {
            var from = new RigidMotionPose(new Vector2(-.2f, .1f), new System.Collections.Generic.Dictionary<RigidBoneId, RigidBonePose>
            {
                [RigidBoneId.Torso] = new RigidBonePose(new Vector2(.1f, 0f), 170f),
            });
            var to = new RigidMotionPose(new Vector2(.2f, .5f), new System.Collections.Generic.Dictionary<RigidBoneId, RigidBonePose>
            {
                [RigidBoneId.Torso] = new RigidBonePose(new Vector2(.3f, .2f), -170f),
                [RigidBoneId.Head] = new RigidBonePose(Vector2.zero, 20f),
            });

            var halfway = RigidMotionLibrary.Blend(from, to, .5f);

            Assert.That(halfway.RootOffset.x, Is.EqualTo(0f).Within(.001f));
            Assert.That(halfway.RootOffset.y, Is.EqualTo(.3f).Within(.001f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(halfway.Bones[RigidBoneId.Torso].RotationDegrees, 180f)), Is.LessThan(.01f));
            Assert.That(halfway.Bones[RigidBoneId.Torso].PositionOffset.x, Is.EqualTo(.2f).Within(.001f));
            Assert.That(halfway.Bones[RigidBoneId.Torso].PositionOffset.y, Is.EqualTo(.1f).Within(.001f));
            Assert.That(halfway.Bones[RigidBoneId.Head].RotationDegrees, Is.EqualTo(10f).Within(.01f));
        }

        [Test]
        public void FastComboPlan_ProvesContinuousSprintJumpAttackAndRollAtGameplayCadence()
        {
            var frames = RigidOutfitPilotComboPlan.Frames;

            Assert.That(RigidOutfitPilotComboPlan.VideoFps, Is.EqualTo(24));
            Assert.That(RigidOutfitPilotComboPlan.FastRunFramesPerCycle, Is.EqualTo(12));
            Assert.That(frames.Count(frame => frame.Label == "RUN FAST"),
                Is.GreaterThanOrEqualTo(RigidOutfitPilotComboPlan.FastRunFramesPerCycle * 3));
            CollectionAssert.IsSubsetOf(new[]
            {
                "ACCELERATE", "RUN FAST", "RUN JUMP", "LAND RUN", "RUN ATTACK", "RUN ROLL",
                "RECOVER IDLE",
            }, frames.Select(frame => frame.Label).Distinct().ToArray());
            Assert.That(frames.All(frame => Mathf.Abs(frame.Pose.RootOffset.x) < .001f), Is.True,
                "In-place proof must not snap backwards when a state-local root track resets.");

            for (var index = 1; index < frames.Count; index++)
            {
                Assert.That(Mathf.Abs(frames[index].Pose.RootOffset.y - frames[index - 1].Pose.RootOffset.y),
                    Is.LessThan(.24f), $"root pop at combo frame {index}");
                foreach (RigidBoneId bone in Enum.GetValues(typeof(RigidBoneId)))
                {
                    var previous = frames[index - 1].Pose.Bones.TryGetValue(bone, out var previousPose)
                        ? previousPose.RotationDegrees : 0f;
                    var current = frames[index].Pose.Bones.TryGetValue(bone, out var currentPose)
                        ? currentPose.RotationDegrees : 0f;
                    Assert.That(Mathf.Abs(Mathf.DeltaAngle(previous, current)), Is.LessThan(72f),
                        $"rotation pop at combo frame {index}, bone {bone}");
                }
            }

            var accelerationStart = frames.ToList().FindIndex(frame => frame.Label == "ACCELERATE");
            Assert.That(accelerationStart, Is.GreaterThan(0));
            AssertPoseStepBelow(frames[accelerationStart - 1].Pose, frames[accelerationStart].Pose,
                .05f, 18f, "idle-to-accelerate");
            AssertPoseStepBelow(frames[frames.Count - 1].Pose, frames[0].Pose,
                .05f, 18f, "loop-wrap");

            for (var index = accelerationStart; index < frames.Count; index++)
            {
                Assert.That(PoseStepMagnitude(frames[index - 1].Pose, frames[index].Pose),
                    Is.GreaterThan(.0001f), $"duplicate pose at combo frame {index}");
            }

            for (var segmentStart = 0; segmentStart < frames.Count;)
            {
                if (frames[segmentStart].Label != "RUN FAST")
                {
                    segmentStart++;
                    continue;
                }
                var segmentEnd = segmentStart;
                while (segmentEnd + 1 < frames.Count && frames[segmentEnd + 1].Label == "RUN FAST")
                    segmentEnd++;
                for (var index = segmentStart;
                     index + RigidOutfitPilotComboPlan.FastRunFramesPerCycle <= segmentEnd;
                     index++)
                {
                    Assert.That(PoseStepMagnitude(frames[index].Pose,
                            frames[index + RigidOutfitPilotComboPlan.FastRunFramesPerCycle].Pose),
                        Is.LessThan(.001f), $"run cadence drift at combo frame {index}");
                }
                segmentStart = segmentEnd + 1;
            }
        }

        private static void AssertPoseStepBelow(RigidMotionPose from, RigidMotionPose to,
            float rootLimit, float rotationLimit, string boundary)
        {
            Assert.That(Vector2.Distance(from.RootOffset, to.RootOffset), Is.LessThan(rootLimit),
                $"root pop at {boundary}");
            foreach (RigidBoneId bone in Enum.GetValues(typeof(RigidBoneId)))
            {
                var fromRotation = from.Bones.TryGetValue(bone, out var fromPose)
                    ? fromPose.RotationDegrees : 0f;
                var toRotation = to.Bones.TryGetValue(bone, out var toPose)
                    ? toPose.RotationDegrees : 0f;
                Assert.That(Mathf.Abs(Mathf.DeltaAngle(fromRotation, toRotation)), Is.LessThan(rotationLimit),
                    $"rotation pop at {boundary}, bone {bone}");
            }
        }

        private static float PoseStepMagnitude(RigidMotionPose from, RigidMotionPose to)
        {
            var magnitude = Vector2.Distance(from.RootOffset, to.RootOffset);
            foreach (RigidBoneId bone in Enum.GetValues(typeof(RigidBoneId)))
            {
                var fromPose = from.Bones.TryGetValue(bone, out var presentFrom)
                    ? presentFrom : new RigidBonePose(Vector2.zero, 0f);
                var toPose = to.Bones.TryGetValue(bone, out var presentTo)
                    ? presentTo : new RigidBonePose(Vector2.zero, 0f);
                magnitude += Vector2.Distance(fromPose.PositionOffset, toPose.PositionOffset);
                magnitude += Mathf.Abs(Mathf.DeltaAngle(fromPose.RotationDegrees, toPose.RotationDegrees));
            }
            return magnitude;
        }

        [Test]
        public void RunCycle_HasFourDistinctOpposedArmLegPhases()
        {
            var poses = new[] { 0f, .25f, .5f, .75f }
                .Select(time => RigidMotionLibrary.Sample(RigidMotionState.Run, time)).ToArray();
            var signatures = poses.Select(pose => string.Join("|", new[]
            {
                pose.Bones[RigidBoneId.UpperArmL].RotationDegrees,
                pose.Bones[RigidBoneId.UpperArmR].RotationDegrees,
                pose.Bones[RigidBoneId.UpperLegL].RotationDegrees,
                pose.Bones[RigidBoneId.UpperLegR].RotationDegrees,
                pose.Bones[RigidBoneId.LowerLegL].RotationDegrees,
                pose.Bones[RigidBoneId.LowerLegR].RotationDegrees,
            })).ToArray();

            Assert.That(signatures.Distinct().Count(), Is.EqualTo(4));
            foreach (var pose in new[] { poses[0], poses[2] })
            {
                Assert.That(Mathf.Sign(pose.Bones[RigidBoneId.UpperArmL].RotationDegrees),
                    Is.EqualTo(-Mathf.Sign(pose.Bones[RigidBoneId.UpperLegL].RotationDegrees)));
                Assert.That(Mathf.Sign(pose.Bones[RigidBoneId.UpperArmR].RotationDegrees),
                    Is.EqualTo(-Mathf.Sign(pose.Bones[RigidBoneId.UpperLegR].RotationDegrees)));
                Assert.That(Mathf.Sign(pose.Bones[RigidBoneId.UpperArmL].RotationDegrees),
                    Is.EqualTo(-Mathf.Sign(pose.Bones[RigidBoneId.UpperArmR].RotationDegrees)));
            }
            Assert.That(Mathf.Abs(poses[1].Bones[RigidBoneId.UpperLegL].RotationDegrees -
                                  poses[1].Bones[RigidBoneId.UpperLegR].RotationDegrees), Is.GreaterThan(25f));
            Assert.That(Mathf.Abs(poses[3].Bones[RigidBoneId.UpperLegL].RotationDegrees -
                                  poses[3].Bones[RigidBoneId.UpperLegR].RotationDegrees), Is.GreaterThan(25f));
        }

        [Test]
        public void Idle_UsesAStableOpenStanceAndRelaxedOpposedArms()
        {
            var pose = RigidMotionLibrary.Sample(RigidMotionState.Idle, 0f);

            Assert.That(pose.Bones[RigidBoneId.Head].RotationDegrees, Is.GreaterThanOrEqualTo(2f));
            Assert.That(pose.Bones[RigidBoneId.UpperLegL].RotationDegrees -
                        pose.Bones[RigidBoneId.UpperLegR].RotationDegrees, Is.GreaterThan(12f));
            Assert.That(Mathf.Abs(pose.Bones[RigidBoneId.UpperArmL].RotationDegrees), Is.GreaterThanOrEqualTo(8f));
            Assert.That(Mathf.Abs(pose.Bones[RigidBoneId.UpperArmR].RotationDegrees), Is.GreaterThanOrEqualTo(8f));
            Assert.That(Mathf.Sign(pose.Bones[RigidBoneId.UpperArmL].RotationDegrees),
                Is.EqualTo(-Mathf.Sign(pose.Bones[RigidBoneId.UpperArmR].RotationDegrees)));
        }

        [Test]
        public void RunCycle_MatchesTheSixPoseLeanHighKneeAndLevelHeadSilhouette()
        {
            var poses = new[] { 0f, .25f, .5f, .75f }
                .Select(time => RigidMotionLibrary.Sample(RigidMotionState.Run, time)).ToArray();

            foreach (var pose in poses)
            {
                var torso = pose.Bones[RigidBoneId.Torso].RotationDegrees;
                var head = pose.Bones[RigidBoneId.Head].RotationDegrees;
                Assert.That(torso, Is.LessThanOrEqualTo(-22f));
                Assert.That(Mathf.Abs(torso + head), Is.LessThanOrEqualTo(2f));
            }

            Assert.That(Mathf.Abs(poses[1].Bones[RigidBoneId.UpperLegL].RotationDegrees -
                                  poses[1].Bones[RigidBoneId.UpperLegR].RotationDegrees), Is.GreaterThan(60f));
            Assert.That(Mathf.Abs(poses[3].Bones[RigidBoneId.UpperLegL].RotationDegrees -
                                  poses[3].Bones[RigidBoneId.UpperLegR].RotationDegrees), Is.GreaterThan(60f));
        }

        [Test]
        public void Attack_ProjectsWeaponHandTowardFacingDirection()
        {
            var windup = RigidMotionLibrary.Sample(RigidMotionState.Attack, .24f);
            var impact = RigidMotionLibrary.Sample(RigidMotionState.Attack, .44f);

            Assert.That(windup.Bones[RigidBoneId.UpperArmL].RotationDegrees, Is.LessThan(0f));
            Assert.That(impact.Bones[RigidBoneId.UpperArmL].RotationDegrees,
                Is.GreaterThan(60f));
            Assert.That(impact.Bones[RigidBoneId.LowerArmL].RotationDegrees, Is.LessThanOrEqualTo(0f));
        }

        [Test]
        public void Jump_HasAnticipationApexAndAbsorbedLandingWithoutChangingScale()
        {
            var anticipation = RigidMotionLibrary.Sample(RigidMotionState.Jump, .14f);
            var apex = RigidMotionLibrary.Sample(RigidMotionState.Jump, .52f);
            var landing = RigidMotionLibrary.Sample(RigidMotionState.Jump, .90f);

            Assert.That(anticipation.RootOffset.y, Is.LessThan(0f));
            Assert.That(apex.RootOffset.y, Is.GreaterThan(.7f));
            Assert.That(landing.RootOffset.y, Is.LessThan(0f));
            Assert.That(anticipation.Bones[RigidBoneId.LowerLegL].RotationDegrees, Is.LessThan(-30f));
            Assert.That(landing.Bones[RigidBoneId.LowerLegR].RotationDegrees, Is.LessThan(-25f));
        }

        [Test]
        public void JumpAndRoll_FoldBothLegsIntoTheSixPoseTuckSilhouette()
        {
            var jumpApex = RigidMotionLibrary.Sample(RigidMotionState.Jump, .52f);
            var rollTuck = RigidMotionLibrary.Sample(RigidMotionState.Roll, .5f);

            foreach (var pose in new[] { jumpApex, rollTuck })
            {
                Assert.That(pose.Bones[RigidBoneId.UpperLegL].RotationDegrees, Is.GreaterThan(88f));
                Assert.That(pose.Bones[RigidBoneId.UpperLegR].RotationDegrees, Is.GreaterThan(82f));
                Assert.That(pose.Bones[RigidBoneId.LowerLegL].RotationDegrees, Is.LessThan(-128f));
                Assert.That(pose.Bones[RigidBoneId.LowerLegR].RotationDegrees, Is.LessThan(-124f));
            }
        }

        [Test]
        public void JumpApex_CurlsTheUpperBodyTowardTheRaisedKnees()
        {
            var apex = RigidMotionLibrary.Sample(RigidMotionState.Jump, .52f);
            var torso = apex.Bones[RigidBoneId.Torso].RotationDegrees;
            var head = apex.Bones[RigidBoneId.Head].RotationDegrees;

            Assert.That(torso, Is.LessThanOrEqualTo(-28f));
            Assert.That(torso + head, Is.InRange(-20f, -8f));
            Assert.That(apex.Bones[RigidBoneId.UpperArmL].RotationDegrees, Is.GreaterThanOrEqualTo(84f));
            Assert.That(apex.Bones[RigidBoneId.LowerArmL].RotationDegrees, Is.LessThanOrEqualTo(-116f));
        }

        [Test]
        public void PilotBodyUnderlayer_FollowsTorsoChainInsteadOfDriftingFromNeck()
        {
            var catalog = RigidOutfitPilotCatalog.Create(_ => _sprite);

            Assert.That(catalog.Male.Body.Parts.Single(part => part.PartId == "body.underlayer").TargetBone,
                Is.EqualTo(RigidBoneId.Torso));
            Assert.That(catalog.Female.Body.Parts.Single(part => part.PartId == "body.underlayer").TargetBone,
                Is.EqualTo(RigidBoneId.Torso));
        }

        [Test]
        public void Roll_ArticulatesCoreInsteadOfSpinningItAsOneRigidCard()
        {
            foreach (var time in new[] { .2f, .5f, .8f })
            {
                var pose = RigidMotionLibrary.Sample(RigidMotionState.Roll, time);
                Assert.That(pose.Bones[RigidBoneId.Pelvis].RotationDegrees, Is.LessThan(0f));
                Assert.That(pose.Bones[RigidBoneId.Torso].RotationDegrees, Is.LessThan(0f));
                Assert.That(pose.Bones[RigidBoneId.Head].RotationDegrees, Is.LessThan(0f));
            }
        }

        [Test]
        public void PilotSource_HeadAndBodyRectanglesOverlapAtLeastSixtyFourPixelsAtNeck()
        {
            var catalog = RigidOutfitPilotCatalog.LoadFromResources();

            AssertNeckOverlap(catalog.Male, "head.profile");
            AssertNeckOverlap(catalog.Female, "female.head.hair");
        }

        private static void AssertNeckOverlap(RigidOutfitPilotCharacter character, string headPartId)
        {
            var bind = character.Skeleton.Bones.ToDictionary(bone => bone.Id, bone => bone.BindPosition);
            var bodyPart = character.Body.Parts.Single(part => part.PartId == "body.underlayer");
            var headPart = character.Body.Parts.Single(part => part.PartId == headPartId);
            var bodyFit = character.BodyFit.Require(bodyPart.PartId);
            var headFit = character.BodyFit.Require(headPart.PartId);
            var bodyTop = bind[bodyPart.TargetBone].y + bodyFit.LocalPosition.y + bodyPart.Sprite.rect.height / (2f * bodyPart.Sprite.pixelsPerUnit);
            var headBottom = bind[headPart.TargetBone].y + headFit.LocalPosition.y - headPart.Sprite.rect.height / (2f * headPart.Sprite.pixelsPerUnit);
            var overlapPixels = (bodyTop - headBottom) * RigidOutfitPilotCatalog.PixelsPerUnit;

            Assert.That(overlapPixels, Is.GreaterThanOrEqualTo(64f), character.Gender);
        }

        [Test]
        public void PilotSource_UpperLegsRenderInsideTheLowerTunicAtTheHip()
        {
            var catalog = RigidOutfitPilotCatalog.Create(_ => _sprite);

            foreach (var character in new[] { catalog.Male, catalog.Female })
            {
                var lowerTunic = character.Outfit.Parts.Single(part => part.PartId == "tunic.lower");
                foreach (var partId in new[] { "leg.near.upper", "leg.far.upper" })
                {
                    var upperLeg = character.Body.Parts.Single(part => part.PartId == partId);
                    Assert.That(upperLeg.SortingOrder, Is.LessThan(lowerTunic.SortingOrder),
                        character.Gender + ":" + partId);
                }
            }
        }

        [Test]
        public void InvariantValidator_RejectsSpriteSwapScaleAndEquipmentBindings()
        {
            var clip = new AnimationClip { name = "illegal-outfit-run" };
            AnimationUtility.SetEditorCurve(clip,
                EditorCurveBinding.FloatCurve("Skeleton/UpperArmL", typeof(Transform), "m_LocalScale.x"),
                AnimationCurve.Linear(0f, 1f, 1f, 1.1f));
            AnimationUtility.SetEditorCurve(clip,
                EditorCurveBinding.FloatCurve("Equipment/Outfit/Torso", typeof(Transform), "m_LocalPosition.x"),
                AnimationCurve.Linear(0f, 0f, 1f, .1f));
            AnimationUtility.SetObjectReferenceCurve(clip,
                EditorCurveBinding.PPtrCurve("Equipment/Outfit/Torso", typeof(SpriteRenderer), "m_Sprite"),
                new[] { new ObjectReferenceKeyframe { time = 0f, value = _sprite } });
            try
            {
                var report = RigidOutfitInvariantValidator.ValidateAnimationClips(new[] { clip });

                CollectionAssert.IsSupersetOf(report.IssueCodes, new[]
                {
                    "ANIMATED_SCALE", "ANIMATED_SPRITE", "ANIMATION_BINDS_EQUIPMENT",
                });
                Assert.That(report.Passed, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(clip);
            }
        }

        [Test]
        public void InvariantValidator_AcceptsSkeletonPositionAndRotationOnly()
        {
            var clip = new AnimationClip { name = "canonical-run" };
            AnimationUtility.SetEditorCurve(clip,
                EditorCurveBinding.FloatCurve("Skeleton/UpperArmL", typeof(Transform), "localEulerAnglesRaw.z"),
                AnimationCurve.Linear(0f, -20f, 1f, 20f));
            AnimationUtility.SetEditorCurve(clip,
                EditorCurveBinding.FloatCurve("CharacterRoot", typeof(Transform), "m_LocalPosition.x"),
                AnimationCurve.Linear(0f, 0f, 1f, 1f));
            try
            {
                Assert.That(RigidOutfitInvariantValidator.ValidateAnimationClips(new[] { clip }).Passed, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(clip);
            }
        }

        [Test]
        public void InvariantValidator_RejectsDeformingRendererComponents()
        {
            var root = new GameObject("actor");
            try
            {
                root.AddComponent<SkinnedMeshRenderer>();
                var report = RigidOutfitInvariantValidator.ValidateHierarchy(root);

                CollectionAssert.Contains(report.IssueCodes, "DEFORMING_RENDERER_COMPONENT");
                Assert.That(report.Passed, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }
    }
}
