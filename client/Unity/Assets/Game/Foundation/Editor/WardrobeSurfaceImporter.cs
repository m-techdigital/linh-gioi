using System;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    public static class WardrobeSurfaceImporter
    {
        private const string Root = "Assets/Game/Art/OnboardingCandidate/";

        public static void Apply()
        {
            const string path = Root + "WardrobeSurfaceMask.png";
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("Missing shared wardrobe surface mask.");
            importer.textureType = TextureImporterType.Default;
            importer.sRGBTexture = false;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = false;
            importer.maxTextureSize = 1024;
            importer.mipmapEnabled = true;
            importer.isReadable = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
            var mask = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "Keeper_Reconstruction.mat");
            if (material == null || material.shader.name != "Universal Render Pipeline/Lit")
                throw new InvalidOperationException("Shared wardrobe requires URP Lit material.");
            material.SetTexture("_MetallicGlossMap", mask);
            material.EnableKeyword("_METALLICSPECGLOSSMAP");
            material.DisableKeyword("_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A");
            material.SetFloat("_SmoothnessTextureChannel", 0f);
            material.SetFloat("_Smoothness", 1f);
            material.SetFloat("_Metallic", 1f);
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();
            Debug.Log($"LGO_WARDROBE_SURFACE_READY size={mask.width}x{mask.height} shared_material=true format={mask.format}");
        }
    }
}
