using System;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    // Dedicated albedos, shared by the entire city; no reference board in Resources.
    public static class CitySurfaceImporter
    {
        public static void Import()
        {
            foreach (var name in new[] { "AgedPaving", "AgedTimber" })
            {
                var path = "Assets/Game/Art/CitySurfaces/Resources/LGOCitySurfaces/" + name + ".png";
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) throw new InvalidOperationException("Missing city surface " + path);
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = true;
                importer.alphaSource = TextureImporterAlphaSource.None;
                importer.isReadable = false;
                importer.mipmapEnabled = true;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Trilinear;
                importer.anisoLevel = 8;
                importer.npotScale = TextureImporterNPOTScale.ToNearest;
                importer.maxTextureSize = 1024;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                importer.compressionQuality = 100;
                importer.SaveAndReimport();
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null || texture.width != 1024 || texture.height != 1024 || texture.isReadable
                    || texture.mipmapCount < 2 || texture.wrapMode != TextureWrapMode.Repeat)
                    throw new InvalidOperationException("City surface import contract failed: " + path);
                Debug.Log("LGO_CITY_SURFACE_IMPORT name=" + name + " size=" + texture.width + "x" + texture.height
                    + " format=" + texture.format + " mips=" + texture.mipmapCount + " readable=" + texture.isReadable);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
