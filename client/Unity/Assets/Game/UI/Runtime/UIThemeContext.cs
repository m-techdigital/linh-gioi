using System;

namespace LinhGioi.UI
{
    public static class UIThemeContext
    {
        public static ThemeTokens Current { get; private set; }

        public static void Set(ThemeTokens theme)
        {
            Current = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        public static ThemeTokens Require()
        {
            return Current ?? throw new InvalidOperationException("UI theme has not been configured.");
        }
    }
}
