using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimePanelSettingsProvider
    {
        internal const string Marker = "LGO Runtime Panel Settings Provider v1";
        private const string ResourceName = "LGORuntimePanelSettings";

        internal static PanelSettings LoadOrCreate()
        {
            var resourceSettings = Resources.Load<PanelSettings>(ResourceName);
            if (resourceSettings != null) return resourceSettings;

            var existingDocuments = Object.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
            foreach (var document in existingDocuments)
            {
                if (document == null || document.panelSettings == null) continue;
                ApplyPolicy(document.panelSettings);
                return document.panelSettings;
            }

            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.name = "LGO Runtime Panel Settings";
            ApplyPolicy(settings);
            return settings;
        }

        internal static string Describe(PanelSettings settings)
        {
            if (settings == null) return "none";
            return "scaleMode=" + settings.scaleMode
                + " referenceResolution=" + settings.referenceResolution.x + "x" + settings.referenceResolution.y
                + " screenMatchMode=" + settings.screenMatchMode
                + " match=" + settings.match.ToString("0.###")
                + " theme=" + (settings.themeStyleSheet != null ? settings.themeStyleSheet.name : "none");
        }

        private static void ApplyPolicy(PanelSettings settings)
        {
            settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            settings.referenceResolution = new Vector2Int(1200, 800);
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 0f;
        }
    }
}
