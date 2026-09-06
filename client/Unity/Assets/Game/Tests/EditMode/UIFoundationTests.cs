using LinhGioi.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;

namespace LinhGioi.Tests
{
    public sealed class UIFoundationTests
    {
        [UnityTest]
        public IEnumerator TouchPadPointerReleaseAndCaptureLossClearMovement()
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
                Assert.Ignore("Pointer capture integration needs a UI panel; run this test without -nographics.");
            var window = ScriptableObject.CreateInstance<EditorWindow>();
            try
            {
                window.Show();
                var pad = new RuntimeTouchMovementPad();
                pad.style.width = 100;
                pad.style.height = 100;
                window.rootVisualElement.Add(pad);
                yield return null;
                yield return null;
                var point = pad.worldBound.center + new Vector2(40, 0);
                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                    pad.SendEvent(down);
                Assert.That(pad.Value.x, Is.GreaterThan(0.5f));
                using (var up = PointerUpEvent.GetPooled(new Event { type = EventType.MouseUp, button = 0, mousePosition = point + Vector2.right * 300 }))
                    pad.SendEvent(up);
                Assert.AreEqual(Vector2.zero, pad.Value);

                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                    pad.SendEvent(down);
                yield return null;
                Assert.That(pad.Value.x, Is.GreaterThan(0.5f));
                Assert.IsTrue(pad.HasPointerCapture(PointerId.mousePointerId));
                pad.ReleasePointer(PointerId.mousePointerId);
                using (var move = PointerMoveEvent.GetPooled(new Event { type = EventType.MouseMove, mousePosition = point }))
                    pad.SendEvent(move);
                yield return null;
                Assert.AreEqual(Vector2.zero, pad.Value);
            }
            finally { window.Close(); }
        }

        [Test]
        public void TouchPadDisplacementIsScaleIndependentAndBounded()
        {
            var small = RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(20, -10), 40);
            var large = RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(40, -20), 80);
            Assert.That(Vector2.Distance(small, large), Is.LessThan(0.0001f));
            var diagonal = RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(400, 400), 40);
            Assert.That(diagonal.magnitude, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(diagonal.x, Is.EqualTo(diagonal.y).Within(0.0001f));
        }

        [Test]
        public void TouchPadCenterAndInvalidGeometryDoNotMovePlayer()
        {
            Assert.AreEqual(Vector2.zero, RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(1, 1), 40));
            Assert.AreEqual(Vector2.zero, RuntimeTouchMovementPad.NormalizeDisplacement(Vector2.one, 0));
            Assert.AreEqual(Vector2.zero, RuntimeTouchMovementPad.NormalizeDisplacement(Vector2.one, float.NaN));
        }

        [Test]
        public void ThemeParsesAuthoritativeTokens()
        {
            const string json = "{\"version\":1,\"colors\":{\"bg\":\"#0B1324\",\"surface\":\"#111D32\",\"surfaceRaised\":\"#182741\",\"spirit\":\"#28D7C7\",\"shadow\":\"#9B5CFF\",\"gold\":\"#E6B85C\",\"danger\":\"#E35D6A\",\"text\":\"#F5F2EA\",\"muted\":\"#9BA7BC\"},\"spacing\":[4,8,12,16,24,32,48,64],\"minimumTouchTarget\":44}";
            var theme = ThemeTokens.FromJson(json);
            Assert.AreEqual(44, theme.minimumTouchTarget);
            Assert.AreEqual(8, theme.spacing.Length);
            Object.DestroyImmediate(theme);
        }

        [Test]
        public void SafeAreaCanBeAppliedWithoutHorizontalOverflow()
        {
            var root = new SafeAreaRoot();
            root.ApplySafeArea(new Rect(10, 20, 980, 1960), new Vector2(1000, 2000));
            Assert.AreEqual(10f, root.style.paddingLeft.value.value);
            Assert.AreEqual(10f, root.style.paddingRight.value.value);
        }
    }
}
