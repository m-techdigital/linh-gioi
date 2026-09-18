using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
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
        public long profilerRuntimeBytes;
        public long estimatedStorageBytes;
        public bool wasResidentBeforeAudit;
    }

    [Serializable]
    public sealed class RuntimeTextureMemorySnapshot
    {
        public int textureCount;
        public long totalProfilerRuntimeBytes;
        public long totalEstimatedStorageBytes;
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
                    profilerRuntimeBytes = Math.Max(0L, Profiler.GetRuntimeMemorySizeLong(texture)),
                    estimatedStorageBytes = EstimateStorageBytes(texture),
                    wasResidentBeforeAudit = residents.Contains(pair.Key),
                });
            }

            entries.Sort((left, right) =>
            {
                var byBytes = right.estimatedStorageBytes.CompareTo(left.estimatedStorageBytes);
                return byBytes != 0 ? byBytes : string.CompareOrdinal(left.name, right.name);
            });

            long profilerTotal = 0;
            long estimatedTotal = 0;
            foreach (var entry in entries)
            {
                profilerTotal += entry.profilerRuntimeBytes;
                estimatedTotal += entry.estimatedStorageBytes;
            }
            return new RuntimeTextureMemorySnapshot
            {
                textureCount = entries.Count,
                totalProfilerRuntimeBytes = profilerTotal,
                totalEstimatedStorageBytes = estimatedTotal,
                entries = entries.ToArray(),
            };
        }

        public static long EstimateStorageBytes(Texture2D texture)
        {
            if (texture == null) return 0L;
            var format = texture.graphicsFormat;
            var blockBytes = Math.Max(1L, (long)GraphicsFormatUtility.GetBlockSize(format));
            var blockWidth = Math.Max(1, (int)GraphicsFormatUtility.GetBlockWidth(format));
            var blockHeight = Math.Max(1, (int)GraphicsFormatUtility.GetBlockHeight(format));
            var width = Math.Max(1, texture.width);
            var height = Math.Max(1, texture.height);
            var mipCount = Math.Max(1, texture.mipmapCount);
            long total = 0L;
            for (var mip = 0; mip < mipCount; mip++)
            {
                var blocksX = Math.Max(1, (width + blockWidth - 1) / blockWidth);
                var blocksY = Math.Max(1, (height + blockHeight - 1) / blockHeight);
                total += (long)blocksX * blocksY * blockBytes;
                width = Math.Max(1, width / 2);
                height = Math.Max(1, height / 2);
            }
            return total;
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
