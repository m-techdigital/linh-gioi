using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace LinhGioi.Tests
{
    public class NpcGuideGestureTests
    {
        [Test]
        public void GuideIkIsAvailableWithoutEnablingThePlayersConversationLayer()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
                "Assets/Game/Art/OnboardingCandidate/ArrivalLocomotion.controller");
            Assert.That(controller, Is.Not.Null);
            var layer = controller.layers.Single(l => l.name == "Conversation");
            Assert.That(layer.defaultWeight, Is.Zero);
            Assert.That(layer.iKPass, Is.True, "The guide needs a final-layer Humanoid IK callback.");
            Assert.That(layer.avatarMask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.RightHandIK), Is.True);
            Assert.That(layer.avatarMask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.Root), Is.False);
            Assert.That(layer.avatarMask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftLeg), Is.False);
            Assert.That(layer.avatarMask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.RightLeg), Is.False);
        }
    }
}
