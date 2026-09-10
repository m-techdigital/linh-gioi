using UnityEditor;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    // One reusable atlas, platform-specific GPU compression; original sources stay outside Resources.
    public sealed class Map01AArtImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AArt/")) return;
            var importer = (TextureImporter)assetImporter;
            var alpha = !assetPath.EndsWith("far-background.png");
            importer.textureType = TextureImporterType.Default;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.isReadable = false;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;
            importer.alphaIsTransparency = alpha;
            importer.alphaSource = alpha ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            foreach (var platform in new[] { "Standalone", "Android", "iPhone" })
            {
                var settings = importer.GetPlatformTextureSettings(platform);
                settings.name = platform;
                settings.overridden = true;
                settings.maxTextureSize = 2048;
                settings.format = platform == "Standalone"
                    ? (alpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1)
                    : TextureImporterFormat.ASTC_6x6;
                settings.compressionQuality = 100;
                importer.SetPlatformTextureSettings(settings);
            }
        }
    }
}
