using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class TwoDKiemMixedLoadoutFitPreview : IDisposable
    {
        private const string Resource = "LGOClasses/KiemMixedLoadoutFitPreview/";
        private static readonly int[] Levels = { 1, 10, 20, 30 };
        private readonly Transform _root;
        private readonly List<Sprite> _sprites = new List<Sprite>();
        private readonly List<ComponentView> _views = new List<ComponentView>();
        private readonly Dictionary<string, bool> _visible = new Dictionary<string, bool>(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _levels = new Dictionary<string, int>(StringComparer.Ordinal);
        private readonly string[] _slots;
        private bool _active;
        private string _gender = "male";
        private string _motion = "idle";

        [Serializable]
        private sealed class Manifest
        {
            public string id;
            public string status;
            public int runtimeEligibleCount;
            public string skeletonVersion;
            public int[] levels;
            public string[] genders;
            public string[] slots;
            public Component[] components;
        }

        [Serializable]
        private sealed class Component
        {
            public string itemId;
            public string gender;
            public int level;
            public string slotId;
            public string side;
            public string bone;
            public float worldX;
            public float worldY;
            public float worldW;
            public float worldH;
            public int order;
            public int[] atlasRect;
            public string atlas;
        }

        private sealed class ComponentView
        {
            public Component Source;
            public SpriteRenderer Renderer;
        }

        public TwoDKiemMixedLoadoutFitPreview(Transform parent, TwoDSkeletalPaperDollRig rig)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (rig == null) throw new ArgumentNullException(nameof(rig));
            _root = new GameObject("Map01A Kiếm ten-slot shared-rig review").transform;
            _root.SetParent(parent, false);

            var manifestAsset = Resources.Load<TextAsset>(Resource + "manifest");
            var maleAtlas = Resources.Load<Texture2D>(Resource + "kiem-equipment-male-atlas");
            var femaleAtlas = Resources.Load<Texture2D>(Resource + "kiem-equipment-female-atlas");
            if (manifestAsset == null || maleAtlas == null || femaleAtlas == null)
                throw new InvalidOperationException("Missing Kiếm ten-slot review pack");
            var manifest = JsonUtility.FromJson<Manifest>(manifestAsset.text);
            if (manifest == null || manifest.id != "kiem-lv1-30-equipment-runtime-v1"
                || manifest.status != "DRAFT_RUNTIME_FIT" || manifest.runtimeEligibleCount != 0
                || manifest.skeletonVersion != "lgo_humanoid_2d_v1"
                || manifest.levels == null || !manifest.levels.SequenceEqual(Levels)
                || manifest.genders == null || !manifest.genders.SequenceEqual(new[] { "male", "female" })
                || manifest.slots == null || manifest.slots.Length != 10
                || manifest.components == null || manifest.components.Length != 104)
                throw new InvalidOperationException("Invalid Kiếm ten-slot review manifest");
            _slots = manifest.slots;
            foreach (var slot in _slots)
            {
                _visible.Add(slot, true);
                _levels.Add(slot, 1);
            }

            foreach (var source in manifest.components)
            {
                if (!_slots.Contains(source.slotId) || !Levels.Contains(source.level)
                    || (source.gender != "male" && source.gender != "female")
                    || (source.side != "left" && source.side != "right" && source.side != "center")
                    || source.atlasRect == null || source.atlasRect.Length != 4
                    || source.worldW <= 0 || source.worldH <= 0)
                    throw new InvalidOperationException("Invalid Kiếm component: " + source.itemId);
                var texture = source.atlas == "kiem-equipment-male-atlas" ? maleAtlas
                    : source.atlas == "kiem-equipment-female-atlas" ? femaleAtlas : null;
                if (texture == null) throw new InvalidOperationException("Unknown Kiếm atlas: " + source.atlas);
                var rect = new Rect(source.atlasRect[0], source.atlasRect[1],
                    source.atlasRect[2], source.atlasRect[3]);
                if (rect.xMin < 0 || rect.yMin < 0 || rect.xMax > texture.width || rect.yMax > texture.height)
                    throw new InvalidOperationException("Kiếm atlas rect outside texture: " + source.itemId);
                var sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect);
                sprite.name = source.itemId + "-" + source.side;
                _sprites.Add(sprite);
                var renderer = rig.Attach(source.gender + "_" + source.bone,
                    "Map01A Kiếm " + source.itemId + " " + source.side, sprite,
                    new Vector2(source.worldX, source.worldY), new Vector2(source.worldW, source.worldH), source.order);
                renderer.enabled = false;
                _views.Add(new ComponentView { Source = source, Renderer = renderer });
            }
            SetActive(false, "male", "idle");
        }

        public bool Active => _active;
        public IReadOnlyList<int> AvailableLevels => Levels;
        public IReadOnlyList<string> SlotIds => _slots;
        public int VisibleComponentCount => _views.Count(view => view.Renderer.enabled && view.Renderer.sprite != null);
        public int VisibleSlotCount => _views.Where(view => view.Renderer.enabled)
            .Select(view => view.Source.slotId).Distinct(StringComparer.Ordinal).Count();
        public int VisibleItemCount => VisibleSlotCount;
        public int ValidSpriteSkinCount => 0;
        public string Snapshot => "KiemMixedLoadoutFitPreview: status=DRAFT_RUNTIME_FIT"
            + " | runtimeEligibleCount=0 | atlases=2x1024 | slots=" + VisibleSlotCount + "/10"
            + " | components=" + VisibleComponentCount + " | gender=" + _gender + " | motion=" + _motion
            + " | mixed=" + string.Join(",", _slots.Select(slot => slot + "=Lv" + _levels[slot]))
            + " | sharedSkeleton=lgo_humanoid_2d_v1 | productionEquipAllowed=False";

        public void SetActive(bool active, string gender, string motion)
        {
            if (gender != "male" && gender != "female") throw new ArgumentException("Unknown gender", nameof(gender));
            _gender = gender;
            _motion = motion ?? "idle";
            _active = active;
            _root.gameObject.SetActive(active);
            Refresh();
        }

        public void SetSlotVisible(string slot, bool visible)
        {
            RequireSlot(slot);
            _visible[slot] = visible;
            Refresh();
        }

        public bool IsSlotVisible(string slot)
        {
            RequireSlot(slot);
            return _visible[slot];
        }

        public void SetSlotLevel(string slot, int level)
        {
            RequireSlot(slot);
            if (!Levels.Contains(level)) throw new ArgumentException("Unknown Kiếm level", nameof(level));
            _levels[slot] = level;
            Refresh();
        }

        public int GetSlotLevel(string slot)
        {
            RequireSlot(slot);
            return _levels[slot];
        }

        public int NextSlotLevel(string slot, int current)
        {
            RequireSlot(slot);
            var index = Array.IndexOf(Levels, current);
            return Levels[(index + 1 + Levels.Length) % Levels.Length];
        }

        public string GetSlotItemId(string slot)
        {
            RequireSlot(slot);
            return _views.Select(view => view.Source).First(source =>
                source.gender == _gender && source.level == _levels[slot] && source.slotId == slot).itemId;
        }

        private void Refresh()
        {
            var active = Active;
            foreach (var view in _views)
                view.Renderer.enabled = active && view.Source.gender == _gender
                    && view.Source.level == _levels[view.Source.slotId] && _visible[view.Source.slotId];
        }

        private void RequireSlot(string slot)
        {
            if (string.IsNullOrEmpty(slot) || !_visible.ContainsKey(slot))
                throw new ArgumentException("Unknown Kiếm equipment slot: " + slot, nameof(slot));
        }

        public void Dispose()
        {
            foreach (var sprite in _sprites)
            {
                if (sprite == null) continue;
                if (Application.isPlaying) UnityEngine.Object.Destroy(sprite);
                else UnityEngine.Object.DestroyImmediate(sprite);
            }
            foreach (var view in _views)
            {
                if (view.Renderer == null) continue;
                if (Application.isPlaying) UnityEngine.Object.Destroy(view.Renderer.gameObject);
                else UnityEngine.Object.DestroyImmediate(view.Renderer.gameObject);
            }
            if (_root != null)
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(_root.gameObject);
                else UnityEngine.Object.DestroyImmediate(_root.gameObject);
            }
        }
    }
}
