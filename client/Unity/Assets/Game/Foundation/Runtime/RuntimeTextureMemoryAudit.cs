using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace LinhGioi.Foundation
{
    [Serializable]
    public sealed class RuntimeTextureMemoryEntry
    {
        public string name;
        public int width;
        public int height;
        public string format;
        public int mipmapCount;
        public bool isReadable;
        public long runtimeBytes;
        public bool wasResidentBeforeAudit;
    }

    [Serializable]
    public sealed class RuntimeTextureMemorySnapshot
    {
        public int textureCount;
        public long totalRuntimeBytes;
        public RuntimeTextureMemoryEntry[] entries;
    }
    public static class RuntimeTextureMemoryAudit
    {
        public static RuntimeTextureMemorySnapshot Summarize(
            IEnumerable<Texture2D> textures,
            IEnumerable<int> residentBeforeIds)
        {
            var residents = new HashSet<int>(residentBeforeIds ?? Array.Empty<int>());
            var unique = new Dictionary<int, Texture2D>();
            foreach (var texture in textures ?? Array.Empty<Texture2D>())
            {
                if (texture == null) continue;
                unique[texture.GetInstanceID()] = texture;
            }

            var entries = new List<RuntimeTextureMemoryEntry>(unique.Count);
            foreach (var pair in unique)
            {
                var texture = pair.Value;
                entries.Add(new RuntimeTextureMemoryEntry
                {
                    name = texture.name,
                    width = texture.width,
                    height = texture.height,
                    format = texture.format.ToString(),
                    mipmapCount = texture.mipmapCount,
                    isReadable = texture.isReadable,
                    runtimeBytes = Math.Max(0L, Profiler.GetRuntimeMemorySizeLong(texture)),
                    wasResidentBeforeAudit = residents.Contains(pair.Key),
                });
            }

            entries.Sort((left, right) =>
            {
                var byBytes = right.runtimeBytes.CompareTo(left.runtimeBytes);
                return byBytes != 0 ? byBytes : string.CompareOrdinal(left.name, right.name);
            });

            long total = 0;
            foreach (var entry in entries) total += entry.runtimeBytes;
            return new RuntimeTextureMemorySnapshot
            {
                textureCount = entries.Count,
                totalRuntimeBytes = total,
                entries = entries.ToArray(),
            };
        }

        public static int[] CaptureLoadedTextureInstanceIds()
        {
            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            var ids = new int[textures.Length];
            for (var index = 0; index < textures.Length; index++)
                ids[index] = textures[index].GetInstanceID();
            return ids;
        }
    }
}
