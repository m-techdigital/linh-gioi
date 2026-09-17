using System;
using UnityEngine;

namespace LinhGioi.UI
{
    internal static class RuntimeUiTheme
    {
        internal const string ResourceName = "ThemeTokens";
        private static ThemeTokens _current;

        internal static ThemeTokens Current
        {
            get
            {
                if (_current == null) _current = Resources.Load<ThemeTokens>(ResourceName);
                if (_current == null)
                    throw new InvalidOperationException("Missing generated product UI ThemeTokens resource: " + ResourceName);
                return _current;
            }
        }

        internal static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        internal static void ResetForTests() => _current = null;
    }
}
