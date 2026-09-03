using LinhGioi.UI;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class UIFoundationTests
    {
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
