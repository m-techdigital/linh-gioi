using System;
using UnityEngine;

namespace LinhGioi.UI
{
    [CreateAssetMenu(menuName = "Linh Gioi/UI/Theme Tokens", fileName = "ThemeTokens")]
    public sealed class ThemeTokens : ScriptableObject
    {
        public int version;
        public Color bg;
        public Color surface;
        public Color surfaceRaised;
        public Color spirit;
        public Color shadow;
        public Color gold;
        public Color danger;
        public Color text;
        public Color muted;
        public int[] spacing;
        public int minimumTouchTarget = 44;
        public string sourceSha256;

        public int SpaceXs => SpacingAt(0, 4);
        public int SpaceS => SpacingAt(1, 8);
        public int SpaceM => SpacingAt(2, 12);
        public int SpaceL => SpacingAt(3, 16);
        public int SpaceXl => SpacingAt(4, 24);
        public int Space2Xl => SpacingAt(5, 32);
        public int Space3Xl => SpacingAt(6, 48);
        public int Space4Xl => SpacingAt(7, 64);

        [Serializable]
        private sealed class TokenDocument
        {
            public int version;
            public ColorDocument colors;
            public int[] spacing;
            public int minimumTouchTarget;
        }

        [Serializable]
        private sealed class ColorDocument
        {
            public string bg;
            public string surface;
            public string surfaceRaised;
            public string spirit;
            public string shadow;
            public string gold;
            public string danger;
            public string text;
            public string muted;
        }

        public static ThemeTokens FromJson(string json, string sourceHash = "")
        {
            var doc = JsonUtility.FromJson<TokenDocument>(json);
            if (doc == null || doc.colors == null) throw new InvalidOperationException("Invalid UI design token JSON");
            var theme = CreateInstance<ThemeTokens>();
            theme.version = doc.version;
            theme.bg = ParseColor(doc.colors.bg, "bg");
            theme.surface = ParseColor(doc.colors.surface, "surface");
            theme.surfaceRaised = ParseColor(doc.colors.surfaceRaised, "surfaceRaised");
            theme.spirit = ParseColor(doc.colors.spirit, "spirit");
            theme.shadow = ParseColor(doc.colors.shadow, "shadow");
            theme.gold = ParseColor(doc.colors.gold, "gold");
            theme.danger = ParseColor(doc.colors.danger, "danger");
            theme.text = ParseColor(doc.colors.text, "text");
            theme.muted = ParseColor(doc.colors.muted, "muted");
            theme.spacing = doc.spacing ?? Array.Empty<int>();
            theme.minimumTouchTarget = doc.minimumTouchTarget;
            theme.sourceSha256 = sourceHash;
            return theme;
        }

        private static Color ParseColor(string value, string token)
        {
            if (!ColorUtility.TryParseHtmlString(value, out var color))
                throw new InvalidOperationException($"Invalid color token {token}: {value}");
            return color;
        }

        private int SpacingAt(int index, int fallback)
        {
            return spacing != null && spacing.Length > index ? spacing[index] : fallback;
        }
    }
}
