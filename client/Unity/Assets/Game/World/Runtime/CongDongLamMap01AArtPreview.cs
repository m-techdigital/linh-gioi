using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LinhGioi.World
{
    // First Map01A art slice. Quest state is deliberately owned by the onboarding controller.
    [ExecuteAlways]
    public sealed class CongDongLamMap01AArtPreview : MonoBehaviour
    {
        public const string ResourcePath = "LGOMaps/CongDongLamMap01AArt/";
        private readonly List<Sprite> _sprites = new List<Sprite>();
        private readonly Dictionary<Renderer, bool> _hidden = new Dictionary<Renderer, bool>();
        private readonly List<Tuple<Transform, Vector3, float>> _parallax = new List<Tuple<Transform, Vector3, float>>();
        private readonly List<Tuple<GameObject, float>> _interactionMarkers = new List<Tuple<GameObject, float>>();
        private TwoDOnboardingController _controller;
        private bool _previousControllerEnabled;
        private float _routeX;
        public bool DialogueOpen { get; private set; }
        public bool HasMetHaVan { get; private set; }
        public bool HasHarvestedSpiritHerb { get; private set; }
        public bool HasOpenedHiddenChest { get; private set; }
        public bool HasInspectedPortal { get; private set; }
        public string LastInteractionMessage { get; private set; } = "";
        public bool IsCapturing => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-art-capture") >= 0;
        public float PlayerX => _routeX;
        public bool CanTalk => Mathf.Abs(PlayerX + 2.65f) <= .95f;
        public string DialogueText => "Ngươi cũng đã tới rồi. Đây là Đông Lâm. Phía sau những ngọn núi kia là Linh Thành.";
        public void MoveOnLane(float axis, float seconds)
        {
            AdvanceVoAnimation(seconds);
            if (DialogueOpen || _controller == null || VoAvatarMotionState == "skill") return;
            if (Mathf.Abs(axis) > .01f)
            {
                VoAvatarMotionState = "walk";
                _voWalkHold = .14f;
                ApplyVoPose();
            }
            var target = Mathf.Clamp(PlayerX + Mathf.Clamp(axis, -1, 1) * Mathf.Clamp(seconds, 0, .1f) * 2.4f, -3.8f, 44.4f);
            _routeX = target;
            _controller.RefreshForSmoke();
            Refresh();
        }
        public bool TalkToHaVan()
        {
            if (!CanTalk) return false;
            if (DialogueOpen) { DialogueOpen = false; HasMetHaVan = true; }
            else DialogueOpen = true;
            return true;
        }
        private Camera _camera;
        private Transform _player;
        private SpriteRenderer _leftFoot, _rightFoot;
        private Transform _voAvatarRoot;
        private readonly Dictionary<string, SpriteRenderer> _voAvatarParts = new Dictionary<string, SpriteRenderer>();
        private SpriteRenderer _voSkillVfx;
        private int _voAvatarMode;
        private float _voAnimationPhase, _voWalkHold, _voSkillRemaining;
        private static readonly string[] VoAvatarModes = { "full", "base", "modular" };
        public string VoAvatarMode => VoAvatarModes[_voAvatarMode];
        public string VoAvatarMotionState { get; private set; } = "idle";
        public Vector2 VoAvatarMotionScale => _voAvatarRoot == null ? Vector2.one : _voAvatarRoot.localScale;
        public int VoSkillCastCount { get; private set; }
        [Serializable] private sealed class VoAvatarPackInfo
        {
            public string id;
            public string status;
            public VoAvatarPart[] parts;
            public VoAvatarPart[] effects;
        }
        [Serializable] private sealed class VoAvatarPart
        {
            public string id;
            public int x, y, w, h, order;
            public float worldW, worldH, dx, dy;
        }
        [Serializable] private sealed class GroundInfo { public float groundY; }
        [Serializable] private sealed class CaptureInfo
        {
            public string status = "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED";
            public string pack = "cong-dong-lam-map01a-art-draft-v1";
            public int frames, width, height;
            public string deviceValidation = "macOS aspect simulation only";
            public float groundY, maxFootError, parallaxDelta;
            public bool mapQuestFlowVerified = false;
            public bool dialogueOpened, greetingCompleted;
            public bool voBaseVerified, voModularVerified, voWalkVerified, voSkillVerified;
            public int voSkillCastCount;
        }
        public float GroundY { get; private set; }
        public float FootY => _leftFoot == null || _rightFoot == null ? float.NaN
            : Mathf.Min(_leftFoot.bounds.min.y, _rightFoot.bounds.min.y);
        private float _previousCameraSize;
        private Vector3 _previousCameraPosition;
        private Transform _farLayer;
        private Vector3 _farScale;
        public int PartCount { get; private set; }
        public int SourcePropCount { get; private set; }
        public int LayerCount { get; private set; }
        public float FarOffset => _parallax.Count == 0 ? 0f : _parallax[0].Item1.localPosition.x - _parallax[0].Item2.x;
        private static readonly string[] RouteNodeIds =
        {
            "spawn-ha-van", "grand-gate", "quan-thu", "village-square", "tong-phu",
            "thanh-nhi", "well-bridge", "lao-tran", "combat-edge", "portal-suoi-thanh-minh"
        };
        private static readonly string[] RouteNodeLabels =
        {
            "Hạ Vân", "Đại Cổng", "Quan Thủ Đông Lâm", "Quảng trường", "Tổng Phú",
            "Thanh Nhi / Dược quán", "Giếng / Cầu gỗ", "Lão Trần", "Rìa làng", "Suối Thanh Minh"
        };
        private static readonly float[] RouteNodeX = { -3.0f, 0f, 6f, 18.5f, 23f, 27f, 31f, 35f, 39f, 43f };
        private int _currentRouteIndex;
        public string CurrentRouteNodeId => RouteNodeIds[_currentRouteIndex];
        public string CurrentRouteNodeLabel => RouteNodeLabels[_currentRouteIndex];
        public Vector3 CurrentInteractionPosition => new Vector3(RouteNodeX[_currentRouteIndex], GroundY + 1.55f, 0);
        public bool CanUseCurrentRouteAction => Mathf.Abs(PlayerX - RouteNodeX[_currentRouteIndex]) <= .95f
            && (CurrentRouteNodeId == "spawn-ha-van"
                || CurrentRouteNodeId == "well-bridge" && (!HasHarvestedSpiritHerb || !HasOpenedHiddenChest)
                || CurrentRouteNodeId == "portal-suoi-thanh-minh" && !HasInspectedPortal);
        public string CurrentActionLabel
        {
            get
            {
                if (CurrentRouteNodeId == "spawn-ha-van") return DialogueOpen ? "Tiếp tục" : "Trò chuyện";
                if (CurrentRouteNodeId == "well-bridge") return HasHarvestedSpiritHerb ? "Mở rương" : "Thu thập Linh Thảo";
                if (CurrentRouteNodeId == "portal-suoi-thanh-minh") return "Kiểm tra lối đi";
                return "Tương tác";
            }
        }
        public bool UseCurrentRouteAction()
        {
            if (!CanUseCurrentRouteAction) return false;
            if (CurrentRouteNodeId == "spawn-ha-van")
            {
                var used = TalkToHaVan();
                LastInteractionMessage = DialogueOpen ? "Hạ Vân đang hướng dẫn." : "Đã nhận chỉ dẫn từ Hạ Vân.";
                return used;
            }
            if (CurrentRouteNodeId == "well-bridge" && !HasHarvestedSpiritHerb)
            {
                HasHarvestedSpiritHerb = true;
                LastInteractionMessage = "Đã thu thập Linh Thảo Non.";
                return true;
            }
            if (CurrentRouteNodeId == "well-bridge" && !HasOpenedHiddenChest)
            {
                HasOpenedHiddenChest = true;
                LastInteractionMessage = "Đã mở rương ẩn bên cầu.";
                return true;
            }
            if (CurrentRouteNodeId == "portal-suoi-thanh-minh")
            {
                HasInspectedPortal = true;
                LastInteractionMessage = "Lối sang Suối Thanh Minh đang khóa theo tiến độ nhiệm vụ.";
                return true;
            }
            return false;
        }
        public static bool ShouldRun() => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-art-preview") >= 0
            || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-art-capture") >= 0;

        public static CongDongLamMap01AArtPreview Attach(TwoDOnboardingController controller)
        {
            var existing = controller.GetComponentInChildren<CongDongLamMap01AArtPreview>();
            if (existing != null) return existing;
            var host = new GameObject("Cong Dong Lam Map01A Art");
            host.transform.SetParent(controller.transform, false);
            var preview = host.AddComponent<CongDongLamMap01AArtPreview>();
            preview._controller = controller;
            preview._previousControllerEnabled = controller.enabled;
            controller.enabled = false;
            try { preview.Build(); }
            catch { DestroyImmediate(host); throw; }
            return preview;
        }

        private void Build()
        {
            var manifest = Resources.Load<TextAsset>(ResourcePath + "manifest");
            var atlas = Resources.Load<Texture2D>(ResourcePath + "props-atlas");
            var far = Resources.Load<Texture2D>(ResourcePath + "far-background");
            if (manifest == null || atlas == null || far == null)
                throw new InvalidOperationException("Missing Map01A art pack");
            var info = JsonUtility.FromJson<DongMonIllustratedPreview.PackInfo>(manifest.text);
            if (info.id != "cong-dong-lam-map01a-art-draft-v1" || info.parts == null || info.layers == null)
                throw new InvalidOperationException("Invalid Map01A art manifest");
            var parts = new Dictionary<string, Sprite>();
            foreach (var part in info.parts)
            {
                if (part.x < 0 || part.y < 0 || part.w <= 0 || part.h <= 0 || part.x + part.w > atlas.width || part.y + part.h > atlas.height)
                    throw new InvalidOperationException("Invalid Map01A atlas rect: " + part.id);
                parts.Add(part.id, MakeSprite(atlas, new Rect(part.x, part.y, part.w, part.h)));
            }
            GroundY = JsonUtility.FromJson<GroundInfo>(manifest.text).groundY;
            var player = GameObject.Find("LGO 2D Player");
            if (player != null)
            {
                _player = player.transform;
                _routeX = _controller.State.PlayerPosition.x;
                foreach (var renderer in player.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (renderer.name == "LGO 2D Player Left Leg") _leftFoot = renderer;
                    if (renderer.name == "LGO 2D Player Right Leg") _rightFoot = renderer;
                }
            }
            BuildVoAvatar();
            PartCount = parts.Count;
            parts.Add("skyline", MakeSprite(far, new Rect(0, 0, far.width, far.height)));
            foreach (var layer in info.layers)
            {
                if (!parts.ContainsKey(layer.part) || layer.width <= 0 || layer.height <= 0)
                    throw new InvalidOperationException("Invalid Map01A layer: " + layer.id);
                var host = new GameObject("Map01A " + layer.id);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(layer.x, layer.y, 0);
                if (layer.part == "terrain")
                {
                    var top = layer.y + layer.height * .5f;
                    layer.height = 3.3f;
                    host.transform.localPosition = new Vector3(layer.x, top - layer.height * .5f, 0);
                }
                var sprite = parts[layer.part];
                host.transform.localScale = new Vector3(layer.width / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
                if (layer.part == "ha-van") renderer.enabled = false;
                if (layer.part == "skyline") { _farLayer = host.transform; _farScale = host.transform.localScale; }
                if (layer.part == "terrain") renderer.enabled = false;
                if (layer.parallax != 0) _parallax.Add(Tuple.Create(host.transform, host.transform.localPosition, layer.parallax));
                LayerCount++;
            }
            BuildSourceProps();
            BuildAuthoredModules();
            BuildNpcs();
            BuildLandmarks();
            BuildCombatInteractables();
            BuildInteractionMarkers();
            _camera = Camera.main;
            if (_camera != null)
            {
                _previousCameraSize = _camera.orthographicSize;
                _previousCameraPosition = _camera.transform.position;
                // Authored viewport is 12.6 x 7.0875; preserve both dimensions.
                _camera.orthographicSize = 4.6f;
                _camera.transform.position = new Vector3(_camera.transform.position.x, GroundY + .23f * 9.2f, _camera.transform.position.z);
            }
            // Suppress the old world presentation only while this opt-in slice is attached.
            foreach (var renderer in FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!renderer.name.StartsWith("LGO 2D", StringComparison.Ordinal)
                    || renderer.transform.root.name == "LGO 2D Player") continue;
                _hidden.Add(renderer, renderer.enabled);
                renderer.enabled = false;
            }
            if (_player != null)
                foreach (var renderer in _player.GetComponentsInChildren<Renderer>(true))
                {
                    if (!_hidden.ContainsKey(renderer)) _hidden.Add(renderer, renderer.enabled);
                    renderer.enabled = false;
                }
            Refresh();
        }

        private void BuildVoAvatar()
        {
            const string path = "LGOClasses/VoLv1MapAvatarArt/";
            var manifest = Resources.Load<TextAsset>(path + "manifest");
            var texture = Resources.Load<Texture2D>(path + "vo-lv1-map-avatar-atlas");
            if (manifest == null || texture == null)
                throw new InvalidOperationException("Missing reviewed Võ Lv1 map avatar pack");
            var pack = JsonUtility.FromJson<VoAvatarPackInfo>(manifest.text);
            if (pack.id != "vo-lv1-map-avatar-v1" || pack.status != "DRAFT_RUNTIME_REVIEW" || pack.parts == null)
                throw new InvalidOperationException("Invalid Võ Lv1 map avatar manifest");
            _voAvatarRoot = new GameObject("Map01A Võ avatar").transform;
            _voAvatarRoot.SetParent(transform, false);
            foreach (var part in pack.parts)
            {
                if (part.x < 0 || part.y < 0 || part.w <= 0 || part.h <= 0
                    || part.x + part.w > texture.width || part.y + part.h > texture.height
                    || part.worldW <= 0 || part.worldH <= 0 || _voAvatarParts.ContainsKey(part.id))
                    throw new InvalidOperationException("Invalid Võ avatar part: " + part.id);
                var host = new GameObject("Map01A Võ avatar " + part.id);
                host.transform.SetParent(_voAvatarRoot, false);
                host.transform.localPosition = new Vector3(part.dx, part.dy, 0);
                var sprite = MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h));
                host.transform.localScale = new Vector3(part.worldW / sprite.bounds.size.x, part.worldH / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = part.order;
                _voAvatarParts.Add(part.id, renderer);
            }
            foreach (var required in new[] { "base", "full", "inner_top", "arm_guard", "main_weapon" })
                if (!_voAvatarParts.ContainsKey(required)) throw new InvalidOperationException("Missing Võ avatar part: " + required);
            if (pack.effects == null || pack.effects.Length != 1 || pack.effects[0].id != "skill_slash")
                throw new InvalidOperationException("Missing Võ skill effect");
            var effect = pack.effects[0];
            var effectHost = new GameObject("Map01A Võ skill");
            effectHost.transform.SetParent(_voAvatarRoot, false);
            effectHost.transform.localPosition = new Vector3(effect.dx, effect.dy, 0);
            var effectSprite = MakeSprite(texture, new Rect(effect.x, effect.y, effect.w, effect.h));
            effectHost.transform.localScale = new Vector3(effect.worldW / effectSprite.bounds.size.x,
                effect.worldH / effectSprite.bounds.size.y, 1);
            _voSkillVfx = effectHost.AddComponent<SpriteRenderer>();
            _voSkillVfx.sprite = effectSprite;
            _voSkillVfx.sortingOrder = effect.order;
            _voSkillVfx.enabled = false;
            RefreshVoAvatarMode();
        }

        public void CycleVoAvatarMode()
        {
            _voAvatarMode = (_voAvatarMode + 1) % VoAvatarModes.Length;
            RefreshVoAvatarMode();
        }

        private void RefreshVoAvatarMode()
        {
            foreach (var pair in _voAvatarParts)
                pair.Value.enabled = VoAvatarMode == "full" ? pair.Key == "full"
                    : VoAvatarMode == "base" ? pair.Key == "base"
                    : pair.Key != "full";
        }

        public bool TriggerVoSkill()
        {
            if (_voSkillRemaining > 0) return false;
            _voSkillRemaining = .42f;
            VoSkillCastCount++;
            VoAvatarMotionState = "skill";
            ApplyVoPose();
            return true;
        }

        public void AdvanceVoAnimation(float seconds)
        {
            var delta = Mathf.Clamp(seconds, 0, .1f);
            _voAnimationPhase += delta;
            if (_voSkillRemaining > 0)
            {
                _voSkillRemaining = Mathf.Max(0, _voSkillRemaining - Mathf.Max(0, seconds));
                if (_voSkillRemaining <= 0) VoAvatarMotionState = "idle";
            }
            else if (_voWalkHold > 0)
            {
                _voWalkHold = Mathf.Max(0, _voWalkHold - delta);
                VoAvatarMotionState = _voWalkHold > 0 ? "walk" : "idle";
            }
            ApplyVoPose();
        }

        private void ApplyVoPose()
        {
            if (_voAvatarRoot == null) return;
            if (VoAvatarMotionState == "walk")
            {
                var stride = Mathf.Sin(_voAnimationPhase * 22f);
                _voAvatarRoot.localScale = new Vector3(1f + stride * .035f, 1f - Mathf.Abs(stride) * .025f, 1f);
                _voAvatarRoot.localRotation = Quaternion.identity;
            }
            else if (VoAvatarMotionState == "skill")
            {
                _voAvatarRoot.localScale = new Vector3(1.07f, .94f, 1f);
                _voAvatarRoot.localRotation = Quaternion.Euler(0, 0, -7f);
            }
            else
            {
                _voAvatarRoot.localScale = new Vector3(1f, 1f + Mathf.Sin(_voAnimationPhase * 2.6f) * .005f, 1f);
                _voAvatarRoot.localRotation = Quaternion.identity;
            }
            if (_voSkillVfx != null)
            {
                _voSkillVfx.enabled = VoAvatarMotionState == "skill";
                if (_voSkillVfx.enabled)
                {
                    var progress = 1f - _voSkillRemaining / .42f;
                    _voSkillVfx.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-48f, -14f, progress));
                    _voSkillVfx.color = new Color(1, 1, 1, Mathf.Sin(progress * Mathf.PI));
                }
            }
        }

        private void BuildSourceProps()
        {
            var manifest = Resources.Load<TextAsset>(ResourcePath + "sheet-props-layout");
            var texture = Resources.Load<Texture2D>(ResourcePath + "sheet-props");
            if (manifest == null || texture == null)
                throw new InvalidOperationException($"Missing extracted Map01A props: layout={manifest != null}, texture2D={texture != null}");
            var pack = JsonUtility.FromJson<DongMonIllustratedPreview.PackInfo>(manifest.text);
            var parts = new Dictionary<string, Sprite>();
            foreach (var part in pack.parts)
            {
                if (part.x < 0 || part.y < 0 || part.w <= 0 || part.h <= 0
                    || part.x + part.w > texture.width || part.y + part.h > texture.height)
                    throw new InvalidOperationException("Invalid extracted prop rect: " + part.id);
                parts.Add(part.id, MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h)));
            }
            foreach (var layer in pack.layers)
            {
                var sprite = parts[layer.part];
                var host = new GameObject("Map01A source " + layer.id);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(layer.x, layer.y, 0);
                host.transform.localScale = new Vector3(layer.width / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
                SourcePropCount++;
            }
        }

        private void BuildAuthoredModules()
        {
            var manifest = Resources.Load<TextAsset>(ResourcePath + "modules-layout");
            var texture = Resources.Load<Texture2D>(ResourcePath + "modules-atlas");
            if (manifest == null || texture == null)
                throw new InvalidOperationException($"Missing authored Map01A modules: layout={manifest != null}, texture2D={texture != null}");
            var pack = JsonUtility.FromJson<DongMonIllustratedPreview.PackInfo>(manifest.text);
            var parts = new Dictionary<string, Sprite>();
            foreach (var part in pack.parts)
                parts.Add(part.id, MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h)));
            foreach (var layer in pack.layers)
            {
                var sprite = parts[layer.part];
                var objectName = layer.id.StartsWith("terrain-")
                    ? "Map01A authored terrain " + int.Parse(layer.id.Substring("terrain-".Length))
                    : "Map01A foreground " + layer.id;
                var host = new GameObject(objectName);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(layer.x, layer.y, 0);
                var displayWidth = layer.id.StartsWith("terrain-") ? layer.width + .12f : layer.width;
                host.transform.localScale = new Vector3(displayWidth / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
            }
        }

        private void BuildInteractionMarkers()
        {
            for (var index = 0; index < RouteNodeIds.Length; index++)
            {
                var host = new GameObject("Map01A interaction " + RouteNodeIds[index]);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(RouteNodeX[index], GroundY + 1.55f, 0);
                var text = host.AddComponent<TextMesh>();
                text.text = "!";
                text.anchor = TextAnchor.MiddleCenter;
                text.alignment = TextAlignment.Center;
                text.fontSize = 64;
                text.characterSize = .045f;
                text.color = new Color(1f, .75f, .18f);
                text.GetComponent<MeshRenderer>().sortingOrder = 10;
                _interactionMarkers.Add(Tuple.Create(host, RouteNodeX[index]));
            }
        }

        private void BuildNpcs()
        {
            var manifest = Resources.Load<TextAsset>(ResourcePath + "npcs-layout");
            var texture = Resources.Load<Texture2D>(ResourcePath + "npcs-atlas");
            if (manifest == null || texture == null)
                throw new InvalidOperationException($"Missing Map01A NPC batch: layout={manifest != null}, texture2D={texture != null}");
            var pack = JsonUtility.FromJson<DongMonIllustratedPreview.PackInfo>(manifest.text);
            var parts = new Dictionary<string, Sprite>();
            foreach (var part in pack.parts)
                parts.Add(part.id, MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h)));
            foreach (var layer in pack.layers)
            {
                var sprite = parts[layer.part];
                var host = new GameObject("Map01A NPC " + layer.id);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(layer.x, layer.y, 0);
                host.transform.localScale = new Vector3(layer.width / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
            }
        }

        private void BuildLandmarks()
        {
            var manifest = Resources.Load<TextAsset>(ResourcePath + "landmarks-layout");
            var texture = Resources.Load<Texture2D>(ResourcePath + "landmarks-atlas");
            if (manifest == null || texture == null)
                throw new InvalidOperationException($"Missing Map01A landmark batch: layout={manifest != null}, texture2D={texture != null}");
            var pack = JsonUtility.FromJson<DongMonIllustratedPreview.PackInfo>(manifest.text);
            var parts = new Dictionary<string, Sprite>();
            foreach (var part in pack.parts)
                parts.Add(part.id, MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h)));
            foreach (var layer in pack.layers)
            {
                var sprite = parts[layer.part];
                var host = new GameObject("Map01A landmark " + layer.id);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(layer.x, layer.y, 0);
                host.transform.localScale = new Vector3(layer.width / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
            }
        }

        private void BuildCombatInteractables()
        {
            var manifest = Resources.Load<TextAsset>(ResourcePath + "combat-layout");
            var texture = Resources.Load<Texture2D>(ResourcePath + "combat-atlas");
            if (manifest == null || texture == null)
                throw new InvalidOperationException($"Missing Map01A combat batch: layout={manifest != null}, texture2D={texture != null}");
            var pack = JsonUtility.FromJson<DongMonIllustratedPreview.PackInfo>(manifest.text);
            var parts = new Dictionary<string, Sprite>();
            foreach (var part in pack.parts)
                parts.Add(part.id, MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h)));
            foreach (var layer in pack.layers)
            {
                var isWorldInteractable = layer.id == "common-chest" || layer.id == "young-spirit-herb";
                var host = new GameObject(isWorldInteractable
                    ? "Map01A world interactable " + layer.id
                    : "Map01A enemy " + layer.id);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(layer.x, layer.y, 0);
                var sprite = parts[layer.part];
                host.transform.localScale = new Vector3(layer.width / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
            }
        }

        private Sprite MakeSprite(Texture2D texture, Rect rect)
        {
            var sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect);
            _sprites.Add(sprite);
            return sprite;
        }

        public void Refresh()
        {
            if (_controller == null) return;
            // Fit existing prototype feet to the flat authored lane. This does not approve class art.
            if (_player != null)
            {
                var position = _player.position;
                position.x = _routeX;
                _player.position = position;
                if (!float.IsNaN(FootY)) _player.position += Vector3.up * (GroundY - FootY);
            }
            if (_voAvatarRoot != null) _voAvatarRoot.localPosition = new Vector3(_routeX, GroundY, 0);
            if (_camera != null)
            {
                var cameraPosition = _camera.transform.position;
                cameraPosition.x = Mathf.Clamp(_routeX, 0f, 39.8f);
                _camera.transform.position = cameraPosition;
            }
            if (_camera != null)
                foreach (var layer in _parallax)
                    layer.Item1.localPosition = layer.Item2 + Vector3.right * _camera.transform.position.x * (1f - layer.Item3);
            if (_camera != null && _farLayer != null)
            {
                // Fit both camera extents after parallax, including camera-to-art offset.
                // Width-only fitting left the taller tablet viewport outside the image.
                var offset = _camera.transform.position - _farLayer.position;
                var halfHeight = _camera.orthographicSize;
                var requiredWidth = 2f * (halfHeight * _camera.aspect + Mathf.Abs(offset.x) + .1f);
                var requiredHeight = 2f * (halfHeight + Mathf.Abs(offset.y) + .1f);
                var cover = Mathf.Max(requiredWidth / 12.6f, requiredHeight / 7.0875f);
                _farLayer.localScale = _farScale * cover;
            }
            GameObject nearest = null;
            var nearestDistance = float.MaxValue;
            foreach (var marker in _interactionMarkers)
            {
                var distance = Mathf.Abs(PlayerX - marker.Item2);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = marker.Item1;
                    _currentRouteIndex = _interactionMarkers.IndexOf(marker);
                }
            }
            foreach (var marker in _interactionMarkers) marker.Item1.SetActive(marker.Item1 == nearest);
        }
        private IEnumerator Start()
        {
            var args = Environment.GetCommandLineArgs();
            if (!Application.isPlaying || Array.IndexOf(args, "--lgo-map01a-art-capture") < 0) yield break;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing Map01A capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            var result = new CaptureInfo { groundY = GroundY, width = Screen.width, height = Screen.height };
            var initial = FarOffset;
            var targets = new[] { TwoDOnboardingState.PlayerStart.x, -3.2f, 0f, 6f, 23f, 31f, 39f, 43f,
                20.5f, 20.5f, 20.5f, 39f };
            var names = new[] { "01-arrival", "02-dialogue", "03-grand-gate", "04-gate-captain",
                "05-market", "06-well-bridge", "07-combat-edge", "08-portal",
                "09-vo-base", "10-vo-modular", "11-vo-walk", "12-vo-skill" };
            for (var i = 0; i < targets.Length; i++)
            {
                _routeX = targets[i];
                if (i == 8)
                {
                    CycleVoAvatarMode();
                    result.voBaseVerified = VoAvatarMode == "base" && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 1;
                }
                if (i == 9)
                {
                    CycleVoAvatarMode();
                    result.voModularVerified = VoAvatarMode == "modular" && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 4;
                }
                if (i == 10)
                {
                    MoveOnLane(1, .1f);
                    result.voWalkVerified = VoAvatarMotionState == "walk" && VoAvatarMotionScale != Vector2.one;
                }
                if (i == 11)
                {
                    result.voSkillVerified = TriggerVoSkill();
                    AdvanceVoAnimation(.16f);
                }
                _controller.RefreshForSmoke();
                Refresh();
                if (i == 1) result.dialogueOpened = TalkToHaVan() && DialogueOpen;
                yield return null;
                yield return new WaitForEndOfFrame();
                result.maxFootError = Mathf.Max(result.maxFootError, Mathf.Abs(FootY - GroundY));
                var active = RenderTexture.active;
                var image = new Texture2D(result.width, result.height, TextureFormat.RGBA32, false);
                try
                {
                    // Capture the completed screen including UI Toolkit, not camera-only world art.
                    RenderTexture.active = null;
                    image.ReadPixels(new Rect(0, 0, result.width, result.height), 0, 0);
                    image.Apply();
                    DongMonIllustratedPreview.WriteBmp(Path.Combine(directory, names[i] + ".bmp"), image.GetPixels32(), result.width, result.height);
                }
                finally { RenderTexture.active = active; Destroy(image); }
                if (i == 1) result.greetingCompleted = TalkToHaVan() && HasMetHaVan;
                result.frames++;
            }
            result.voSkillCastCount = VoSkillCastCount;
            result.parallaxDelta = FarOffset - initial;
            if (!result.dialogueOpened || !result.greetingCompleted || !result.voBaseVerified || !result.voModularVerified
                || !result.voWalkVerified || !result.voSkillVerified
                || result.voSkillCastCount != 1 || float.IsNaN(FootY) || result.maxFootError > .001f || Mathf.Abs(result.parallaxDelta) < .01f)
                result.status = "FIX_REQUIRED";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), JsonUtility.ToJson(result, true));
            Application.Quit(result.status == "FIX_REQUIRED" ? 1 : 0);
        }

        private void LateUpdate()
        {
            if (Application.isPlaying) AdvanceVoAnimation(Time.deltaTime);
            Refresh();
        }
        private void OnDestroy()
        {
            if (_camera != null) { _camera.orthographicSize = _previousCameraSize; _camera.transform.position = _previousCameraPosition; }
            if (_controller != null) { _controller.enabled = _previousControllerEnabled; _controller.RefreshForSmoke(); }
            foreach (var pair in _hidden) if (pair.Key != null) pair.Key.enabled = pair.Value;
            foreach (var sprite in _sprites)
                if (sprite != null) { if (Application.isPlaying) Destroy(sprite); else DestroyImmediate(sprite); }
        }
    }
}
