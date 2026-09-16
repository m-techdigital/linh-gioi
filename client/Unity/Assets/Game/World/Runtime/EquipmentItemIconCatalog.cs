using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    // Content ownership is explicit. A slot illustration is never an item binding.
    public sealed class EquipmentItemIconCatalog
    {
        [Serializable] private sealed class Document
        {
            public Binding[] itemBindings;
        }

        [Serializable] private sealed class Binding
        {
            public string itemId;
            public string classId;
            public string slot;
            public string iconId;
            public string gender;
            public int level;
        }

        private readonly Dictionary<(string, string, string, string, int), Sprite> _sprites
            = new Dictionary<(string, string, string, string, int), Sprite>();
        private EquipmentItemIconCatalog() { }
        public static EquipmentItemIconCatalog FromJson(string json, Func<string, Sprite> resolveSprite)
        {
            if (resolveSprite == null) throw new ArgumentNullException(nameof(resolveSprite));
            var document = JsonUtility.FromJson<Document>(json);
            if (document == null) throw new ArgumentException("Missing item icon registration document", nameof(json));
            var catalog = new EquipmentItemIconCatalog();
            foreach (var binding in document.itemBindings ?? Array.Empty<Binding>())
            {
                if (binding == null || !IsIdentity(binding.itemId) || !IsIdentity(binding.classId)
                    || !IsIdentity(binding.slot) || !IsIdentity(binding.iconId)
                    || !IsIdentity(binding.gender) || binding.level <= 0)
                    throw new ArgumentException("Item icon binding requires exact itemId, classId, slot, gender, positive level and iconId");
                var key = (binding.classId, binding.itemId, binding.slot, binding.gender, binding.level);
                if (catalog._sprites.ContainsKey(key))
                    throw new ArgumentException("Duplicate item icon binding: " + binding.itemId);
                var sprite = resolveSprite(binding.iconId);
                if (sprite == null) throw new ArgumentException("Item icon binding references missing sprite: " + binding.iconId);
                catalog._sprites.Add(key, sprite);
            }
            return catalog;
        }

        private static bool IsIdentity(string value) => !string.IsNullOrWhiteSpace(value)
            && value == value.Trim() && value.IndexOf('*') < 0;
        public Sprite Resolve(string classId, string itemId, string slot, string gender, int level)
        {
            return _sprites.TryGetValue((classId, itemId, slot, gender, level), out var sprite) ? sprite : null;
        }
    }
}
