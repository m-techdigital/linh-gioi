using System;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.UI.Editor
{
    public static class ThemeTokenImporter
    {
        public const string SourcePath = "Assets/Game/UI/design-tokens.json";
        public const string GeneratedAssetPath = "Assets/Game/Generated/UI/ThemeTokens.asset";

        public static ThemeTokens EnsureTheme()
        {
            var source = AssetDatabase.LoadAssetAtPath<TextAsset>(SourcePath);
            if (source == null) throw new InvalidOperationException($"Missing design tokens: {SourcePath}");
            var hash = ComputeSha256(source.text);
            var existing = AssetDatabase.LoadAssetAtPath<ThemeTokens>(GeneratedAssetPath);
            if (existing != null && existing.sourceSha256 == hash) return existing;

            EnsureFolder("Assets/Game/Generated/UI");
            var parsed = ThemeTokens.FromJson(source.text, hash);
            if (existing == null)
            {
                AssetDatabase.CreateAsset(parsed, GeneratedAssetPath);
                existing = parsed;
            }
            else
            {
                EditorUtility.CopySerialized(parsed, existing);
                UnityEngine.Object.DestroyImmediate(parsed);
                EditorUtility.SetDirty(existing);
            }
            AssetDatabase.SaveAssets();
            return existing;
        }

        private static string ComputeSha256(string text)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        private static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
