using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    public static class RuntimeTextureImportPolicy
    {
        private const string ResourcesRoot = "Assets/Game/World/Runtime/Resources";

        public static void ApplyFromCommandLine()
        {
            var changed = Apply();
            AssetDatabase.SaveAssets();
            Debug.Log("LGO_RUNTIME_TEXTURE_IMPORT_POLICY_PASS changed=" + changed);
        }

        public static int Apply()
        {
            var paths = AssetDatabase.FindAssets("t:Texture2D", new[] { ResourcesRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path)
                .ToArray();
            var changedCount = 0;
            foreach (var path in paths)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null || Mathf.Max(texture.width, texture.height) < 1024) continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                var changed = false;
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    changed = true;
                }
                if (importer.isReadable)
                {
                    importer.isReadable = false;
                    changed = true;
                }

                var environment = IsEnvironmentTexture(path);
                var format = (TextureImporterFormat)(environment ? 50 : 48);
                var minimumMaxSize = Mathf.NextPowerOfTwo(Mathf.Max(texture.width, texture.height));
                changed |= ApplyPlatform(importer, BuildTarget.Android, minimumMaxSize, format);
                changed |= ApplyPlatform(importer, BuildTarget.iOS, minimumMaxSize, format);
                if (!changed) continue;
                importer.SaveAndReimport();
                changedCount++;
                Debug.Log($"LGO_TEXTURE_POLICY_APPLIED path={path} mobileFormat={(int)format} minimumMax={minimumMaxSize}");
            }
            return changedCount;
        }

        private static bool ApplyPlatform(
            TextureImporter importer,
            BuildTarget target,
            int minimumMaxSize,
            TextureImporterFormat format)
        {
            var targetName = BuildPipeline.GetBuildTargetName(target);
            var settings = importer.GetPlatformTextureSettings(targetName);
            var validMaxSize = settings.maxTextureSize >= minimumMaxSize && settings.maxTextureSize <= 2048;
            var changed = !settings.overridden
                || !validMaxSize
                || settings.format != format
                || settings.textureCompression != TextureImporterCompression.CompressedHQ
                || settings.compressionQuality != 100
                || settings.crunchedCompression;

            if (!changed) return false;
            settings.name = targetName;
            settings.overridden = true;
            if (!validMaxSize) settings.maxTextureSize = minimumMaxSize;
            settings.format = format;
            settings.textureCompression = TextureImporterCompression.CompressedHQ;
            settings.compressionQuality = 100;
            settings.crunchedCompression = false;
            settings.allowsAlphaSplitting = false;
            importer.SetPlatformTextureSettings(settings);
            return true;
        }

        private static bool IsEnvironmentTexture(string path)
        {
            return path.Contains("/LGOMaps/CongDongLamMap01AArt/", StringComparison.Ordinal)
                || path.Contains("/LGOMaps/DongMonIllustrated/", StringComparison.Ordinal);
        }
    }
}
