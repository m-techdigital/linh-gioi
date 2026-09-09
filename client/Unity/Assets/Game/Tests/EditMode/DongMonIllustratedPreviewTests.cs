using System.Collections.Generic;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class DongMonIllustratedPreviewTests
    {
        [Test]
        public void DraftAtlasLoadsAndPreviewPreservesInteractionAndRestoresRenderers()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Illustrated preview test");
            DongMonIllustratedPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                var gate = GameObject.Find("LGO 2D Gate Roof").GetComponent<Renderer>();
                var previous = gate.enabled;
                preview = DongMonIllustratedPreview.Attach(controller);
                Assert.That(preview.AtlasPartCount, Is.EqualTo(3));
                Assert.That(preview.GateSprite.rect.width, Is.EqualTo(1024));
                Assert.That(preview.GateSprite.texture.width, Is.EqualTo(2048));
                Assert.IsFalse(gate.enabled);
                var before = preview.FarOffset;
                controller.State.Move(TwoDOnboardingState.GateKeeperPosition + Vector2.left * 0.85f - controller.State.PlayerPosition);
                preview.Refresh();
                Assert.That(preview.FarOffset, Is.Not.EqualTo(before));
                Assert.That(controller.State.AvailableAction, Is.EqualTo(TwoDOnboardingAction.Talk));
                Assert.IsTrue(controller.State.TryUseAction());
                Assert.IsTrue(controller.State.DialogueOpen);
                Object.DestroyImmediate(preview.gameObject);
                preview = null;
                Assert.That(gate.enabled, Is.EqualTo(previous));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }
    }
}
