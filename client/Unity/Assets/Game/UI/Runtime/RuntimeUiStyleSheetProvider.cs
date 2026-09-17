using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeUiStyleSheetProvider
    {
        internal const string ResourceName = "LGOUI/LgoRuntime";
        private static StyleSheet _sheet;

        internal static bool Attach(VisualElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (_sheet == null) _sheet = Resources.Load<StyleSheet>(ResourceName);
            if (_sheet == null)
                throw new InvalidOperationException("Missing product UI stylesheet resource: " + ResourceName);
            for (var index = 0; index < root.styleSheets.count; index++)
                if (root.styleSheets[index] == _sheet) return false;
            root.styleSheets.Add(_sheet);
            root.AddToClassList("lgo-product-ui");
            return true;
        }

        internal static void ResetForTests() => _sheet = null;
    }
}
