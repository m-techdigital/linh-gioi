using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace LinhGioi.World
{
    // Explicit local art review only. One actor stack shares pose, facing and roll.
    public sealed class TwoDSourcePoseReview : MonoBehaviour
    {
        [Serializable] private sealed class Pack
        {
            public string status, reviewSlot, basePoseAtlasSha256, basePoseManifestSha256;
            public bool runtimeEligible;
            public int samplingDivisor;
            public Part[] sprites;
            public int[] jumpPivotSource;
        }
        [Serializable] private sealed class Part
        {
            public string id, sourceSpaceProfile, componentId;
            public int order;
            public int[] atlasRectTopLeft, sourceCanvasRect;
        }
        private sealed class ReviewComponent
        {
            public readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
            public readonly Dictionary<string, Rect> SourceRects = new Dictionary<string, Rect>();
            public SpriteRenderer Renderer;
            public int Order;
        }
        private sealed class ReviewSlot
        {
            public Texture2D Texture;
            public readonly Dictionary<string, ReviewComponent> Components = new Dictionary<string, ReviewComponent>();
        }
        private static readonly string[] SlotIds =
        {
            "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
            "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory"
        };
        private static readonly string[] SlotDirectories =
        {
            "main-weapon-review", "head-hair-review", "inner-top-review", "outer-top-review", "lower-body-review",
            "waist-belt-review", "arm-guard-review", "footwear-review", "shoulder-chest-guard-review", "class-accessory-review"
        };
        private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Rect> _sourceRects = new Dictionary<string, Rect>();
        private readonly Dictionary<string, ReviewSlot> _reviewSlots = new Dictionary<string, ReviewSlot>();
        private Texture2D _texture;
        private SpriteRenderer _renderer;
        private Transform _rotationRoot;
        private Vector2 _jumpPivot;
        private string _frame;
        private readonly TwoDSourcePoseTimeline _timeline = new TwoDSourcePoseTimeline();
        public bool HasTransitions => _sprites.ContainsKey("run_start") && _sprites.ContainsKey("run_stop");
        public string CurrentFrame => _frame;

        public static TwoDSourcePoseReview CreateIfRequested(Transform parent)
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-vo-pose-review-dir");
            if (index < 0) return null;
            if (index + 1 >= args.Length) throw new ArgumentException("Pose review directory missing");
            var host = new GameObject("Võ male pose review — active stack");
            host.transform.SetParent(parent, false);
            var review = host.AddComponent<TwoDSourcePoseReview>();
            try { review.Load(args[index + 1]); }
            catch { Destroy(host); throw; }
            Debug.Log("LGO_POSE_REVIEW_LOADED " + args[index + 1]);
            return review;
        }

        private void Load(string directory)
        {
            var pack = JsonUtility.FromJson<Pack>(File.ReadAllText(Path.Combine(directory, "atlas-review.json")));
            if (pack == null || pack.sprites == null || (pack.sprites.Length != 6 && pack.sprites.Length != 8) || pack.samplingDivisor != 4 || pack.status != "REVIEW_ONLY" || pack.runtimeEligible)
                throw new InvalidDataException("Expected REVIEW_ONLY div4 idle/four-phase-run/jump pack");
            _texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!_texture.LoadImage(File.ReadAllBytes(Path.Combine(directory, "atlas-review.png"))))
                throw new InvalidDataException("Cannot load pose review atlas");
            _texture.filterMode = FilterMode.Bilinear;
            _texture.wrapMode = TextureWrapMode.Clamp;
            foreach (var part in pack.sprites)
            {
                var a = part.atlasRectTopLeft; var s = part.sourceCanvasRect;
                if (part.sourceSpaceProfile != "lgo_character_canvas_1024x1536_v1"
                    || a == null || a.Length != 4 || s == null || s.Length != 4
                    || a[0] < 0 || a[1] < 0 || a[2] <= 0 || a[3] <= 0
                    || a[0] + a[2] > _texture.width || a[1] + a[3] > _texture.height
                    || s[0] < 0 || s[1] < 0 || s[2] > 1024 || s[3] > 1536
                    || s[2] - s[0] != a[2] * pack.samplingDivisor
                    || s[3] - s[1] != a[3] * pack.samplingDivisor
                    || !IsValidPoseId(part.id))
                    throw new InvalidDataException("Invalid pose registration: " + part.id);
                _sprites.Add(part.id, Sprite.Create(_texture,
                    new Rect(a[0], _texture.height - a[1] - a[3], a[2], a[3]), new Vector2(.5f, .5f), 100));
                _sourceRects.Add(part.id, Rect.MinMaxRect(s[0], s[1], s[2], s[3]));
            }
            if (!_sprites.ContainsKey("idle") || !_sprites.ContainsKey("run_a") || !_sprites.ContainsKey("run_b") || !_sprites.ContainsKey("run_contact_a") || !_sprites.ContainsKey("run_contact_b") || !_sprites.ContainsKey("jump_tuck"))
                throw new InvalidDataException("Missing required locomotion pose");
            if (_sprites.ContainsKey("run_start") != _sprites.ContainsKey("run_stop"))
                throw new InvalidDataException("Run entry/exit must be supplied together");
            if (_sprites.ContainsKey("jump_tuck"))
            {
                var pivot = pack.jumpPivotSource;
                if (pivot == null || pivot.Length != 2 || pivot[0] < 0 || pivot[0] > 1024 || pivot[1] < 0 || pivot[1] > 1536)
                    throw new InvalidDataException("Jump rotation pivot missing/outside source canvas");
                _jumpPivot = new Vector2((pivot[0] - 512) * (1.70f / 1536), (1484 - pivot[1]) * (1.70f / 1536));
            }
            _rotationRoot = new GameObject("Whole pose rotation").transform;
            _rotationRoot.SetParent(transform, false);
            var host = new GameObject("Pose sprite"); host.transform.SetParent(_rotationRoot, false);
            _renderer = host.AddComponent<SpriteRenderer>(); _renderer.sortingOrder = 24;
            for (var index = 0; index < SlotIds.Length; index++)
            {
                var overlayDirectory = Path.Combine(directory, SlotDirectories[index]);
                if (Directory.Exists(overlayDirectory)) LoadReviewSlot(directory, overlayDirectory, SlotIds[index], index);
            }
            transform.localPosition = Vector3.zero;
            Apply("idle", 0, 1);
        }

        private void LoadReviewSlot(string bodyDirectory, string overlayDirectory, string expectedSlot, int slotOrder)
        {
            var manifestPath = Path.Combine(overlayDirectory, "atlas-review.json");
            var atlasPath = Path.Combine(overlayDirectory, "atlas-review.png");
            var pack = JsonUtility.FromJson<Pack>(File.ReadAllText(manifestPath));
            if (pack == null || pack.sprites == null || pack.sprites.Length < 6
                || pack.samplingDivisor != 4 || pack.status != "REVIEW_ONLY" || pack.runtimeEligible
                || pack.reviewSlot != expectedSlot || _reviewSlots.ContainsKey(expectedSlot)
                || pack.basePoseAtlasSha256 != Hash(Path.Combine(bodyDirectory, "atlas-review.png"))
                || pack.basePoseManifestSha256 != Hash(Path.Combine(bodyDirectory, "atlas-review.json")))
                throw new InvalidDataException("Slot review pack does not belong to this body pose pack: " + expectedSlot);
            var slot = new ReviewSlot { Texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) };
            if (!slot.Texture.LoadImage(File.ReadAllBytes(atlasPath)))
                throw new InvalidDataException("Cannot load slot review atlas: " + expectedSlot);
            slot.Texture.filterMode = FilterMode.Bilinear;
            slot.Texture.wrapMode = TextureWrapMode.Clamp;
            foreach (var part in pack.sprites)
            {
                var a = part.atlasRectTopLeft; var s = part.sourceCanvasRect;
                var componentId = string.IsNullOrEmpty(part.componentId) ? "main" : part.componentId;
                if (part.sourceSpaceProfile != "lgo_character_canvas_1024x1536_v1"
                    || a == null || a.Length != 4 || s == null || s.Length != 4
                    || a[0] < 0 || a[1] < 0 || a[2] <= 0 || a[3] <= 0
                    || a[0] + a[2] > slot.Texture.width || a[1] + a[3] > slot.Texture.height
                    || s[0] < 0 || s[1] < 0 || s[2] > 1024 || s[3] > 1536
                    || s[2] - s[0] != a[2] * pack.samplingDivisor
                    || s[3] - s[1] != a[3] * pack.samplingDivisor
                    || !IsValidPoseId(part.id))
                    throw new InvalidDataException("Invalid slot pose registration: " + expectedSlot + "/" + part.id);
                if (!slot.Components.TryGetValue(componentId, out var component))
                {
                    component = new ReviewComponent { Order = part.order == 0 ? 25 + slotOrder : part.order };
                    slot.Components.Add(componentId, component);
                }
                if (component.Sprites.ContainsKey(part.id) || (part.order != 0 && component.Order != part.order))
                    throw new InvalidDataException("Duplicate/inconsistent slot component pose: " + expectedSlot + "/" + componentId + "/" + part.id);
                component.Sprites.Add(part.id, Sprite.Create(slot.Texture,
                    new Rect(a[0], slot.Texture.height - a[1] - a[3], a[2], a[3]), new Vector2(.5f, .5f), 100));
                component.SourceRects.Add(part.id, Rect.MinMaxRect(s[0], s[1], s[2], s[3]));
            }
            foreach (var pair in slot.Components)
            {
                foreach (var required in new[] { "idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck" })
                    if (!pair.Value.Sprites.ContainsKey(required))
                        throw new InvalidDataException("Missing slot locomotion pose: " + expectedSlot + "/" + pair.Key + "/" + required);
                var host = new GameObject(expectedSlot + "/" + pair.Key); host.transform.SetParent(_rotationRoot, false);
                pair.Value.Renderer = host.AddComponent<SpriteRenderer>();
                pair.Value.Renderer.sortingOrder = pair.Value.Order;
            }
            _reviewSlots.Add(expectedSlot, slot);
            Debug.Log("LGO_POSE_REVIEW_OVERLAY_LOADED " + expectedSlot + " " + overlayDirectory);
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
        }

        public void Advance(float seconds) => _timeline.Advance(seconds);

        public void SetSlotVisible(string slot, bool visible)
        {
            if (_reviewSlots.TryGetValue(slot, out var reviewSlot))
                foreach (var component in reviewSlot.Components.Values) component.Renderer.enabled = visible;
        }

        public void SetOuterTopVisible(bool visible) => SetSlotVisible("outer_top", visible);

        public void Apply(string motion, float phaseSeconds, int facing, float actionProgress = 0)
        {
            if (_renderer == null) return;
            // The existing character state owns time and facing; no second animation clock.
            var moving = motion == "walk" || motion == "run";
            var hasContactFrames = _sprites.ContainsKey("run_contact_a") && _sprites.ContainsKey("run_contact_b");
            var frame = moving ? SelectRunFrame(Mathf.Repeat(phaseSeconds * TwoDSourcePoseTimeline.RunCyclesPerSecond, 1), hasContactFrames) : "idle";
            if (HasTransitions) frame = _timeline.Select(motion, phaseSeconds);
            var jumping = motion == "jump" && _sprites.ContainsKey("jump_tuck");
            if (jumping) frame = "jump_tuck";
            var sprite = _sprites[frame]; var rect = _sourceRects[frame];
            const float units = 1.70f / 1536;
            var direction = facing < 0 ? -1 : 1;
            var pivot = jumping ? new Vector3(_jumpPivot.x * direction, _jumpPivot.y, 0) : Vector3.zero;
            var jumpLean = jumping ? Mathf.Sin(Mathf.Clamp01(actionProgress) * Mathf.PI) : 0;
            _rotationRoot.localPosition = pivot + new Vector3(jumpLean * .18f * direction, jumpLean * .04f, 0);
            _rotationRoot.localRotation = Quaternion.Euler(0, 0, jumping ? (SomersaultDegrees(actionProgress) - jumpLean * 16f) * direction : 0);
            ApplySprite(_renderer, sprite, rect, units, direction, pivot);
            foreach (var slot in _reviewSlots.Values)
                foreach (var component in slot.Components.Values)
                    ApplySprite(component.Renderer, component.Sprites[frame], component.SourceRects[frame], units, direction, pivot);
            if (_frame != frame) { _frame = frame; Debug.Log("LGO_POSE_REVIEW_FRAME " + frame); }
        }

        private static void ApplySprite(SpriteRenderer renderer, Sprite sprite, Rect rect, float units, int direction, Vector3 pivot)
        {
            renderer.sprite = sprite;
            renderer.transform.localPosition = new Vector3((rect.center.x - 512) * units * direction,
                (1484 - rect.center.y) * units, 0) - pivot;
            renderer.transform.localScale = new Vector3(rect.width * units / sprite.bounds.size.x * direction,
                rect.height * units / sprite.bounds.size.y, 1);
        }

        public static float SomersaultDegrees(float progress) => -360f * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.03f, .40f, progress));

        public static string SelectRunFrame(float loopPhase, bool hasContactFrames)
        {
            var phase = Mathf.Repeat(loopPhase, 1);
            if (!hasContactFrames) return phase < .5f ? "run_a" : "run_b";
            if (phase < .25f) return "run_contact_a";
            if (phase < .5f) return "run_a";
            if (phase < .75f) return "run_contact_b";
            return "run_b";
        }

        private static bool IsValidPoseId(string id)
        {
            return id == "run_start" || id == "run_stop" || id == "idle" || id == "run_a" || id == "run_b" || id == "jump_tuck"
                || id == "run_contact_a" || id == "run_contact_b";
        }

        private void OnGUI()
        {
            var camera = Camera.main;
            if (camera == null) return;
            var point = camera.WorldToScreenPoint(transform.position + Vector3.up * 1.9f);
            if (point.z <= 0) return;
            var box = new Rect(point.x - 95, Screen.height - point.y - 40, 210, 40);
            var previousColor = GUI.color;
            GUI.color = new Color(0, 0, 0, .8f);
            GUI.DrawTexture(box, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(box, _reviewSlots.Count == 0
                ? "Võ nam · POSE THỬ\nĐứng / chạy / lộn · chưa đồ rời"
                : "Võ nam · POSE THỬ\n" + _reviewSlots.Count + "/10 slot · đứng / 4 nhịp chạy / lộn");
            GUI.color = previousColor;
        }

        private void OnDestroy()
        {
            foreach (var sprite in _sprites.Values) Destroy(sprite);
            foreach (var slot in _reviewSlots.Values)
            {
                foreach (var component in slot.Components.Values)
                    foreach (var sprite in component.Sprites.Values) Destroy(sprite);
                if (slot.Texture != null) Destroy(slot.Texture);
            }
            if (_texture != null) Destroy(_texture);
        }
    }
}
