using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LinhGioi.World
{
    // Playable Map01A visual slice. State remains local until the production quest backend is approved.
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
        public bool HasStarterSupplies { get; private set; }
        public bool HasAcceptedGatherQuest { get; private set; }
        public bool HasAcceptedCombatQuest { get; private set; }
        public bool HasDefeatedFirstEnemy { get; private set; }
        public bool HasLootedFirstEnemy { get; private set; }
        public bool PortalUnlocked { get; private set; }
        public bool HasInspectedInventory { get; private set; }
        public string ActiveQuestId { get; private set; } = "Q01";
        private readonly HashSet<string> _completedQuests = new HashSet<string>();
        private string _dialogueNodeId = "";
        public int CompletedQuestCount => _completedQuests.Count;
        public bool IsQuestComplete(string questId) => _completedQuests.Contains(questId);
        public string LastInteractionMessage { get; private set; } = "";
        public bool IsCapturing => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-art-capture") >= 0;
        public float PlayerX => _routeX;
        public bool CanTalk => Mathf.Abs(PlayerX + 2.65f) <= .95f;
        public string DialogueSpeaker => _dialogueNodeId == "quan-thu" ? "Quan Thủ Đông Lâm"
            : _dialogueNodeId == "tong-phu" ? "Tổng Phú"
            : _dialogueNodeId == "thanh-nhi" ? "Thanh Nhi"
            : _dialogueNodeId == "lao-tran" ? "Lão Trần" : "Hạ Vân";
        public string DialogueText => _dialogueNodeId == "quan-thu" ? "Trong làng cấm giao chiến. Qua rìa làng, hãy luôn giữ vũ khí sẵn sàng."
            : _dialogueNodeId == "tong-phu" ? "Mang Bình Máu Nhỏ và Bình Linh Lực này. Hành trang tốt sẽ cứu mạng ngươi."
            : _dialogueNodeId == "thanh-nhi" ? "Hãy hái Linh Thảo Non bên giếng. Ánh sáng của nó rất dịu, đừng nhầm với Thảo Yêu."
            : _dialogueNodeId == "lao-tran" ? "Ngoài cổng có thú non nhiễm linh khí. Hạ một con rồi mang chiến lợi phẩm về."
            : "Ngươi cũng đã tới rồi. Đây là Đông Lâm. Phía sau những ngọn núi kia là Linh Thành.";
        public string QuestTrackerText => ActiveQuestId == "COMPLETE" ? "Map01A hoàn tất\nPortal Suối Thanh Minh đã mở."
            : ActiveQuestId + " · " + QuestName(ActiveQuestId) + "\n" + QuestObjective(ActiveQuestId)
                + "\nTiến độ " + CompletedQuestCount + "/9";
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
            _dialogueNodeId = "spawn-ha-van";
            if (DialogueOpen)
            {
                DialogueOpen = false;
                HasMetHaVan = true;
                if (ActiveQuestId == "Q01") CompleteQuest("Q01", "Q02");
            }
            else DialogueOpen = true;
            return true;
        }

        private static string QuestName(string id)
        {
            switch (id)
            {
                case "Q01": return "Đường Hội Tụ";
                case "Q02": return "Nhìn Về Linh Thành";
                case "Q03": return "Luật Của Đông Lâm";
                case "Q04": return "Hành Trang Cần Thiết";
                case "Q05": return "Linh Thảo Ven Nước";
                case "Q06": return "Ngoài Cổng Không Yên";
                case "Q07": return "Chiến Lợi Phẩm Đầu Tiên";
                case "Q08": return "Bí Mật Bên Cầu";
                case "Q09": return "Đường Đến Suối Thanh Minh";
                default: return "Hoàn tất";
            }
        }

        private static string QuestObjective(string id)
        {
            switch (id)
            {
                case "Q01": return "Nói chuyện với Hạ Vân.";
                case "Q02": return "Ngắm Linh Thành từ Đại Cổng.";
                case "Q03": return "Học luật khu an toàn từ Quan Thủ.";
                case "Q04": return "Xem hành trang và nhận tiếp tế từ Tổng Phú.";
                case "Q05": return "Gặp Thanh Nhi, hái Linh Thảo bên giếng.";
                case "Q06": return "Nhận việc từ Lão Trần và hạ một quái non.";
                case "Q07": return "Nhặt chiến lợi phẩm đầu tiên.";
                case "Q09": return "Mở lối sang Suối Thanh Minh.";
                default: return "Tiếp tục khám phá Đông Lâm.";
            }
        }

        private void CompleteQuest(string completed, string next)
        {
            _completedQuests.Add(completed);
            ActiveQuestId = next;
        }

        private bool ToggleNpcDialogue(string nodeId, Action onComplete)
        {
            if (!DialogueOpen)
            {
                _dialogueNodeId = nodeId;
                DialogueOpen = true;
                LastInteractionMessage = DialogueSpeaker + " đang trò chuyện.";
                return true;
            }
            if (_dialogueNodeId != nodeId) return false;
            DialogueOpen = false;
            onComplete();
            return true;
        }
        private Camera _camera;
        private Transform _player;
        private SpriteRenderer _leftFoot, _rightFoot;
        private Transform _voAvatarRoot;
        private readonly Dictionary<string, SpriteRenderer> _voAvatarParts = new Dictionary<string, SpriteRenderer>();
        private readonly Dictionary<string, Tuple<Sprite, VoAvatarPart>> _voMotionFrames = new Dictionary<string, Tuple<Sprite, VoAvatarPart>>();
        private readonly HashSet<string> _voEquippedSlots = new HashSet<string>();
        private SpriteRenderer _voSkillVfx, _voMotionRenderer;
        private SpriteRenderer _voCombatTarget;
        private SpriteRenderer _spiritHerbRenderer, _hiddenChestRenderer;
        private int _voAvatarMode, _voAvatarGender, _voAvatarLevel, _voSelectedEquipmentSlot;
        private float _voAnimationPhase, _voWalkHold, _voSkillRemaining;
        private bool _voPendingHit;
        private static readonly string[] VoAvatarModes = { "full", "base", "modular" };
        private static readonly string[] VoAvatarGenders = { "male", "female" };
        private static readonly int[] VoAvatarLevels = { 1, 10, 20, 30 };
        private static readonly string[] VoEquipmentSlots =
        {
            "main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment",
            "waist", "arm_guard", "boots", "light_armor", "accessory"
        };
        public string VoAvatarMode => VoAvatarModes[_voAvatarMode];
        public string VoAvatarGender => VoAvatarGenders[_voAvatarGender];
        public int VoAvatarLevel => VoAvatarLevels[_voAvatarLevel];
        public int[] VoAvatarAvailableLevels => (int[])VoAvatarLevels.Clone();
        public string VoSelectedEquipmentSlot => VoEquipmentSlots[_voSelectedEquipmentSlot];
        public int VoEquippedSlotCount => _voEquippedSlots.Count;
        public string VoAvatarMotionState { get; private set; } = "idle";
        public string VoAvatarMotionFrameId { get; private set; } = "idle";
        public Vector2 VoAvatarMotionScale => _voAvatarRoot == null ? Vector2.one : _voAvatarRoot.localScale;
        public bool VoAvatarUsesFrameMotion => VoAvatarLevel == 1 && VoAvatarMode == "full" && VoAvatarMotionState != "idle";
        public bool VoAvatarUsesAlignedPaperDollMotion => VoAvatarMotionState != "idle" && !VoAvatarUsesFrameMotion;
        public int VoSkillCastCount { get; private set; }
        public int VoSkillHitCount { get; private set; }
        public int VoTrainingTargetHp { get; private set; } = 100;
        public bool CanTriggerVoSkill => _voSkillRemaining <= 0 && _voCombatTarget != null
            && Mathf.Abs(_voCombatTarget.transform.position.x - PlayerX) <= 2.4f && VoTrainingTargetHp > 0;
        [Serializable] private sealed class VoAvatarPackInfo
        {
            public string id;
            public string status;
            public string[] genders;
            public string[] slots;
            public int[] levels;
            public VoAvatarPart[] parts;
            public VoAvatarPart[] effects;
            public VoAvatarPart[] motionFrames;
        }
        [Serializable] private sealed class VoAvatarPart
        {
            public string id;
            public string gender, kind, slot, atlas;
            public int level;
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
            public string activeQuestId;
            public int completedQuestCount;
            public bool starterSupplies, spiritHerb, hiddenChest, combatAccepted, enemyDefeated, enemyLooted, portalUnlocked;
            public bool dialogueOpened, greetingCompleted;
            public bool voBaseVerified, voModularVerified, voWalkVerified, voSkillVerified;
            public bool voFemaleVerified, voSlotToggleVerified;
            public bool voFemaleMotionVerified, voProgressionVerified;
            public int voSkillCastCount, voSkillHitCount, voTrainingTargetHp;
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
        public bool CanUseCurrentRouteAction
        {
            get
            {
                if (Mathf.Abs(PlayerX - RouteNodeX[_currentRouteIndex]) > .95f) return false;
                if (DialogueOpen) return _dialogueNodeId == CurrentRouteNodeId;
                if (CurrentRouteNodeId == "spawn-ha-van") return ActiveQuestId == "Q01";
                if (CurrentRouteNodeId == "grand-gate") return ActiveQuestId == "Q02";
                if (CurrentRouteNodeId == "quan-thu") return ActiveQuestId == "Q03";
                if (CurrentRouteNodeId == "village-square") return ActiveQuestId == "Q04" && !HasInspectedInventory;
                if (CurrentRouteNodeId == "tong-phu") return ActiveQuestId == "Q04" && HasInspectedInventory;
                if (CurrentRouteNodeId == "thanh-nhi") return ActiveQuestId == "Q05" && !HasAcceptedGatherQuest;
                if (CurrentRouteNodeId == "well-bridge") return ActiveQuestId == "Q05" && !HasHarvestedSpiritHerb
                    || HasHarvestedSpiritHerb && !HasOpenedHiddenChest;
                if (CurrentRouteNodeId == "lao-tran") return ActiveQuestId == "Q06" && !HasAcceptedCombatQuest;
                if (CurrentRouteNodeId == "combat-edge") return ActiveQuestId == "Q07" && HasDefeatedFirstEnemy && !HasLootedFirstEnemy;
                if (CurrentRouteNodeId == "portal-suoi-thanh-minh") return ActiveQuestId == "Q09" && !PortalUnlocked;
                return false;
            }
        }
        public string CurrentActionLabel
        {
            get
            {
                if (CurrentRouteNodeId == "spawn-ha-van") return DialogueOpen ? "Tiếp tục" : "Trò chuyện";
                if (DialogueOpen) return "Tiếp tục";
                if (CurrentRouteNodeId == "grand-gate") return "Nhìn về Linh Thành";
                if (CurrentRouteNodeId == "village-square") return "Mở hành trang";
                if (CurrentRouteNodeId == "well-bridge") return HasHarvestedSpiritHerb ? "Mở rương ẩn" : "Hái Linh Thảo";
                if (CurrentRouteNodeId == "combat-edge") return "Nhặt chiến lợi phẩm";
                if (CurrentRouteNodeId == "portal-suoi-thanh-minh") return PortalUnlocked ? "Đã mở" : "Mở lối";
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
            if (CurrentRouteNodeId == "grand-gate" && ActiveQuestId == "Q02")
            {
                CompleteQuest("Q02", "Q03");
                LastInteractionMessage = "Đã nhìn thấy Linh Thành ở phía xa.";
                return true;
            }
            if (CurrentRouteNodeId == "quan-thu" && ActiveQuestId == "Q03")
                return ToggleNpcDialogue("quan-thu", () =>
                {
                    CompleteQuest("Q03", "Q04");
                    LastInteractionMessage = "Đã hiểu luật khu an toàn Đông Lâm.";
                });
            if (CurrentRouteNodeId == "village-square" && ActiveQuestId == "Q04" && !HasInspectedInventory)
            {
                HasInspectedInventory = true;
                LastInteractionMessage = "Đã xem hành trang tân thủ; hãy gặp Tổng Phú.";
                return true;
            }
            if (CurrentRouteNodeId == "tong-phu" && ActiveQuestId == "Q04")
                return ToggleNpcDialogue("tong-phu", () =>
                {
                    HasStarterSupplies = true;
                    CompleteQuest("Q04", "Q05");
                    LastInteractionMessage = "Nhận Bình Máu Nhỏ ×3 và Bình Linh Lực Nhỏ ×2.";
                });
            if (CurrentRouteNodeId == "thanh-nhi" && ActiveQuestId == "Q05")
                return ToggleNpcDialogue("thanh-nhi", () =>
                {
                    HasAcceptedGatherQuest = true;
                    LastInteractionMessage = "Thanh Nhi chỉ Linh Thảo phát sáng bên giếng.";
                });
            if (CurrentRouteNodeId == "well-bridge" && !HasHarvestedSpiritHerb)
            {
                HasHarvestedSpiritHerb = true;
                if (_spiritHerbRenderer != null) _spiritHerbRenderer.enabled = false;
                CompleteQuest("Q05", "Q06");
                LastInteractionMessage = "Đã thu thập Linh Thảo Non.";
                return true;
            }
            if (CurrentRouteNodeId == "well-bridge" && !HasOpenedHiddenChest)
            {
                HasOpenedHiddenChest = true;
                if (_hiddenChestRenderer != null) _hiddenChestRenderer.color = new Color(.62f, .62f, .62f, .58f);
                _completedQuests.Add("Q08");
                LastInteractionMessage = "Q08 hoàn tất · rương ẩn: 12 Vàng + Bánh Bao.";
                return true;
            }
            if (CurrentRouteNodeId == "lao-tran" && ActiveQuestId == "Q06")
                return ToggleNpcDialogue("lao-tran", () =>
                {
                    HasAcceptedCombatQuest = true;
                    LastInteractionMessage = "Lão Trần giao nhiệm vụ hạ một quái non ở rìa làng.";
                });
            if (CurrentRouteNodeId == "combat-edge" && ActiveQuestId == "Q07" && HasDefeatedFirstEnemy)
            {
                HasLootedFirstEnemy = true;
                CompleteQuest("Q07", "Q09");
                LastInteractionMessage = "Nhặt Da Lợn Non + 8 Vàng · chiến lợi phẩm đầu tiên.";
                return true;
            }
            if (CurrentRouteNodeId == "portal-suoi-thanh-minh")
            {
                HasInspectedPortal = true;
                PortalUnlocked = true;
                CompleteQuest("Q09", "COMPLETE");
                LastInteractionMessage = "Cổng đường sang Suối Thanh Minh đã mở.";
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
            var textures = new Dictionary<string, Texture2D>
            {
                { "lv001", Resources.Load<Texture2D>(path + "vo-lv1-map-avatar-atlas") },
                { "lv010", Resources.Load<Texture2D>(path + "vo-lv10-equipment-atlas") },
                { "lv020", Resources.Load<Texture2D>(path + "vo-lv20-equipment-atlas") },
                { "lv030", Resources.Load<Texture2D>(path + "vo-lv30-equipment-atlas") },
                { "motion_male", Resources.Load<Texture2D>(path + "vo-lv1-motion-atlas") },
                { "motion_female", Resources.Load<Texture2D>(path + "vo-lv1-female-motion-atlas") },
            };
            if (manifest == null || textures.Values.Any(texture => texture == null))
                throw new InvalidOperationException("Missing reviewed Võ Lv1-30 map avatar pack");
            var pack = JsonUtility.FromJson<VoAvatarPackInfo>(manifest.text);
            if (pack.id != "vo-lv1-30-map-avatar-v3" || pack.status != "DRAFT_RUNTIME_REVIEW"
                || pack.parts == null || pack.parts.Length != 96 || pack.genders == null || pack.genders.Length != 2
                || pack.slots == null || pack.slots.Length != 10 || pack.levels == null || pack.levels.Length != 4)
                throw new InvalidOperationException("Invalid Võ Lv1-30 map avatar manifest");
            _voEquippedSlots.Clear();
            foreach (var slot in VoEquipmentSlots) _voEquippedSlots.Add(slot);
            _voAvatarRoot = new GameObject("Map01A Võ avatar").transform;
            _voAvatarRoot.SetParent(transform, false);
            foreach (var part in pack.parts)
            {
                if (!textures.TryGetValue(part.atlas, out var texture))
                    throw new InvalidOperationException("Unknown Võ equipment atlas: " + part.atlas);
                if (part.x < 0 || part.y < 0 || part.w <= 0 || part.h <= 0
                    || part.x + part.w > texture.width || part.y + part.h > texture.height
                    || part.worldW <= 0 || part.worldH <= 0 || _voAvatarParts.ContainsKey(part.id))
                    throw new InvalidOperationException("Invalid Võ avatar part: " + part.id);
                var suffix = part.kind == "slot" ? "slot " + part.slot : part.kind;
                var host = new GameObject("Map01A Võ avatar lv" + part.level.ToString("000") + " " + part.gender + " " + suffix);
                host.transform.SetParent(_voAvatarRoot, false);
                host.transform.localPosition = new Vector3(part.dx, part.dy, 0);
                var sprite = MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h));
                host.transform.localScale = new Vector3(part.worldW / sprite.bounds.size.x, part.worldH / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = part.order;
                _voAvatarParts.Add(part.id, renderer);
            }
            foreach (var level in VoAvatarLevels)
            foreach (var gender in VoAvatarGenders)
            {
                var prefix = "lv" + level.ToString("000") + "_" + gender + "_";
                foreach (var required in new[] { "base", "full" })
                    if (!_voAvatarParts.ContainsKey(prefix + required))
                        throw new InvalidOperationException("Missing Võ avatar part: " + prefix + required);
                foreach (var slot in VoEquipmentSlots)
                    if (!_voAvatarParts.ContainsKey(prefix + "slot_" + slot))
                        throw new InvalidOperationException("Missing Võ avatar slot: " + prefix + slot);
            }
            if (pack.effects == null || pack.effects.Length != 1 || pack.effects[0].id != "skill_slash")
                throw new InvalidOperationException("Missing Võ skill effect");
            var effect = pack.effects[0];
            var effectHost = new GameObject("Map01A Võ skill");
            effectHost.transform.SetParent(_voAvatarRoot, false);
            effectHost.transform.localPosition = new Vector3(effect.dx, effect.dy, 0);
            var effectTexture = textures["lv001"];
            var effectSprite = MakeSprite(effectTexture, new Rect(effect.x, effect.y, effect.w, effect.h));
            effectHost.transform.localScale = new Vector3(effect.worldW / effectSprite.bounds.size.x,
                effect.worldH / effectSprite.bounds.size.y, 1);
            _voSkillVfx = effectHost.AddComponent<SpriteRenderer>();
            _voSkillVfx.sprite = effectSprite;
            _voSkillVfx.sortingOrder = effect.order;
            _voSkillVfx.enabled = false;
            if (pack.motionFrames == null || pack.motionFrames.Length != 12)
                throw new InvalidOperationException("Missing Võ two-gender motion frame batch");
            var motionHost = new GameObject("Map01A Võ motion frame");
            motionHost.transform.SetParent(_voAvatarRoot, false);
            _voMotionRenderer = motionHost.AddComponent<SpriteRenderer>();
            foreach (var frame in pack.motionFrames)
            {
                if (!textures.TryGetValue(frame.atlas, out var motionTexture))
                    throw new InvalidOperationException("Unknown Võ motion atlas: " + frame.atlas);
                if (frame.x < 0 || frame.y < 0 || frame.w <= 0 || frame.h <= 0
                    || frame.x + frame.w > motionTexture.width || frame.y + frame.h > motionTexture.height
                    || _voMotionFrames.ContainsKey(frame.id))
                    throw new InvalidOperationException("Invalid Võ motion frame: " + frame.id);
                _voMotionFrames.Add(frame.id, Tuple.Create(MakeSprite(motionTexture, new Rect(frame.x, frame.y, frame.w, frame.h)), frame));
            }
            SetVoMotionFrame("idle");
            _voMotionRenderer.enabled = false;
            RefreshVoAvatarMode();
        }

        public void CycleVoAvatarMode()
        {
            _voAvatarMode = (_voAvatarMode + 1) % VoAvatarModes.Length;
            RefreshVoAvatarMode();
        }

        public void CycleVoAvatarGender()
        {
            _voAvatarGender = (_voAvatarGender + 1) % VoAvatarGenders.Length;
            RefreshVoAvatarMode();
        }

        public void CycleVoAvatarLevel()
        {
            _voAvatarLevel = (_voAvatarLevel + 1) % VoAvatarLevels.Length;
            RefreshVoAvatarMode();
        }

        public void CycleVoEquipmentSlot()
        {
            _voSelectedEquipmentSlot = (_voSelectedEquipmentSlot + 1) % VoEquipmentSlots.Length;
        }

        public void ToggleVoEquipmentSlot()
        {
            var slot = VoSelectedEquipmentSlot;
            if (!_voEquippedSlots.Remove(slot)) _voEquippedSlots.Add(slot);
            RefreshVoAvatarMode();
        }

        private void RefreshVoAvatarMode()
        {
            foreach (var pair in _voAvatarParts)
            {
                var prefix = "lv" + VoAvatarLevel.ToString("000") + "_" + VoAvatarGender + "_";
                if (!pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                {
                    pair.Value.enabled = false;
                    continue;
                }
                if (VoAvatarMode == "full") pair.Value.enabled = pair.Key == prefix + "full";
                else if (VoAvatarMode == "base") pair.Value.enabled = pair.Key == prefix + "base";
                else if (pair.Key == prefix + "base") pair.Value.enabled = true;
                else if (pair.Key.StartsWith(prefix + "slot_", StringComparison.Ordinal))
                    pair.Value.enabled = _voEquippedSlots.Contains(pair.Key.Substring((prefix + "slot_").Length));
                else pair.Value.enabled = false;
            }
            // The reviewed key-pose sheets currently depict the Lv1 outfit only.
            // Higher tiers keep their selected paper-doll visible until tier-matched motion exists.
            var usesMotionFrame = VoAvatarUsesFrameMotion;
            if (usesMotionFrame)
                foreach (var renderer in _voAvatarParts.Values) renderer.enabled = false;
            if (_voMotionRenderer != null) _voMotionRenderer.enabled = usesMotionFrame;
        }

        private void SetVoMotionFrame(string frameId)
        {
            if (_voMotionRenderer == null || !_voMotionFrames.TryGetValue(frameId, out var visual)) return;
            VoAvatarMotionFrameId = frameId;
            _voMotionRenderer.sprite = visual.Item1;
            _voMotionRenderer.sortingOrder = visual.Item2.order;
            _voMotionRenderer.transform.localPosition = new Vector3(visual.Item2.dx, visual.Item2.dy, 0);
            _voMotionRenderer.transform.localScale = new Vector3(
                visual.Item2.worldW / visual.Item1.bounds.size.x,
                visual.Item2.worldH / visual.Item1.bounds.size.y, 1);
        }

        public bool TriggerVoSkill()
        {
            if (!CanTriggerVoSkill) return false;
            _voSkillRemaining = .42f;
            _voPendingHit = true;
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
                if (_voPendingHit && _voSkillRemaining <= .28f)
                {
                    _voPendingHit = false;
                    VoTrainingTargetHp = Mathf.Max(0, VoTrainingTargetHp - 35);
                    VoSkillHitCount++;
                    LastInteractionMessage = "Liệt Phong Kích trúng mục tiêu · -35 HP · còn " + VoTrainingTargetHp;
                    if (_voCombatTarget != null)
                        _voCombatTarget.color = VoTrainingTargetHp == 0 ? new Color(.28f, .28f, .28f, .75f) : new Color(1f, .42f, .32f, 1f);
                    if (VoTrainingTargetHp == 0 && ActiveQuestId == "Q06" && HasAcceptedCombatQuest)
                    {
                        HasDefeatedFirstEnemy = true;
                        CompleteQuest("Q06", "Q07");
                        LastInteractionMessage = "Q06 hoàn tất · quái non đã bị hạ; hãy nhặt chiến lợi phẩm.";
                    }
                }
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
            _voAvatarRoot.localScale = Vector3.one;
            _voAvatarRoot.localRotation = Quaternion.identity;
            if (VoAvatarMotionState == "walk")
            {
                var stride = Mathf.Sin(_voAnimationPhase * 22f);
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (stride >= 0 ? "walk_a" : "walk_b"));
                else
                {
                    VoAvatarMotionFrameId = "lv" + VoAvatarLevel + "_aligned_paper_doll_walk";
                    _voAvatarRoot.localRotation = Quaternion.Euler(0, 0, stride * 1.4f);
                    _voAvatarRoot.localScale = new Vector3(1f + Mathf.Abs(stride) * .008f, 1f - Mathf.Abs(stride) * .006f, 1f);
                }
            }
            else if (VoAvatarMotionState == "skill")
            {
                var progress = 1f - _voSkillRemaining / .42f;
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (progress < .45f ? "punch_windup" : "punch_impact"));
                else
                {
                    VoAvatarMotionFrameId = "lv" + VoAvatarLevel + "_aligned_paper_doll_skill";
                    _voAvatarRoot.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-2f, -6f, progress));
                    _voAvatarRoot.localScale = new Vector3(Mathf.Lerp(1f, 1.045f, progress), Mathf.Lerp(1f, .982f, progress), 1f);
                }
            }
            else
            {
                SetVoMotionFrame(VoAvatarGender + "_idle");
                _voAvatarRoot.localScale = new Vector3(1f, 1f + Mathf.Sin(_voAnimationPhase * 2.6f) * .005f, 1f);
            }
            RefreshVoAvatarMode();
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
                if (layer.id == "young-spirit-herb") _spiritHerbRenderer = renderer;
                if (layer.id == "common-chest") _hiddenChestRenderer = renderer;
                if (!isWorldInteractable && _voCombatTarget == null)
                    _voCombatTarget = renderer;
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
            var targets = new[] { -3.58f, -3.58f, -3.58f, 0f, 5.15f, 5.15f, 18.5f, 22.15f, 26.15f, 30.2f, 30.2f, 34.1f, 39f, 39f, 42.15f,
                20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f };
            var names = new[] { "01-arrival-q01", "02-ha-van-dialogue", "03-q01-complete", "04-q02-grand-gate",
                "05-quan-thu-dialogue", "06-q03-complete", "07-q04-inventory", "08-q04-starter-supplies",
                "09-q05-thanh-nhi", "10-q05-spirit-herb", "11-q08-hidden-chest", "12-q06-lao-tran",
                "13-q06-combat", "14-q07-loot", "15-q09-portal-open",
                "16-vo-base", "17-vo-modular", "18-vo-walk", "19-vo-female-full", "20-vo-female-slot-toggle",
                "21-vo-female-walk", "22-vo-lv10-female", "23-vo-lv20-female", "24-vo-lv30-female" };
            for (var i = 0; i < targets.Length; i++)
            {
                _routeX = targets[i];
                Refresh();
                if (i == 1) result.dialogueOpened = UseCurrentRouteAction() && DialogueOpen;
                if (i == 2) result.greetingCompleted = UseCurrentRouteAction() && HasMetHaVan;
                if (i == 3) UseCurrentRouteAction();
                if (i == 4) UseCurrentRouteAction();
                if (i == 5) UseCurrentRouteAction();
                if (i == 6) UseCurrentRouteAction();
                if (i == 7) { UseCurrentRouteAction(); UseCurrentRouteAction(); }
                if (i == 8) { UseCurrentRouteAction(); UseCurrentRouteAction(); }
                if (i == 9) UseCurrentRouteAction();
                if (i == 10) UseCurrentRouteAction();
                if (i == 11) { UseCurrentRouteAction(); UseCurrentRouteAction(); }
                if (i == 12)
                {
                    result.voSkillVerified = true;
                    for (var hit = 0; hit < 3; hit++)
                    {
                        result.voSkillVerified &= TriggerVoSkill();
                        AdvanceVoAnimation(.16f);
                        if (hit < 2) AdvanceVoAnimation(.5f);
                    }
                }
                if (i == 13) { AdvanceVoAnimation(.5f); UseCurrentRouteAction(); }
                if (i == 14)
                {
                    UseCurrentRouteAction();
                    result.mapQuestFlowVerified = ActiveQuestId == "COMPLETE" && CompletedQuestCount == 9
                        && HasStarterSupplies && HasHarvestedSpiritHerb && HasOpenedHiddenChest
                        && HasAcceptedCombatQuest && HasDefeatedFirstEnemy && HasLootedFirstEnemy && PortalUnlocked;
                }
                if (i == 15)
                {
                    CycleVoAvatarMode();
                    result.voBaseVerified = VoAvatarMode == "base" && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 1;
                }
                if (i == 16)
                {
                    CycleVoAvatarMode();
                    result.voModularVerified = VoAvatarMode == "modular" && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 11;
                }
                if (i == 17)
                {
                    MoveOnLane(1, .1f);
                    result.voWalkVerified = VoAvatarMotionState == "walk" && VoAvatarUsesAlignedPaperDollMotion
                        && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 11 && !_voMotionRenderer.enabled;
                }
                if (i == 18)
                {
                    AdvanceVoAnimation(.1f);
                    AdvanceVoAnimation(.1f);
                    CycleVoAvatarMode();
                    CycleVoAvatarGender();
                    result.voFemaleVerified = VoAvatarMode == "full" && VoAvatarGender == "female"
                        && _voAvatarParts["lv001_female_full"].enabled;
                }
                if (i == 19)
                {
                    CycleVoAvatarMode();
                    CycleVoAvatarMode();
                    CycleVoEquipmentSlot();
                    CycleVoEquipmentSlot();
                    ToggleVoEquipmentSlot();
                    result.voSlotToggleVerified = VoAvatarMode == "modular" && VoSelectedEquipmentSlot == "inner_top"
                        && VoEquippedSlotCount == 9 && !_voAvatarParts["lv001_female_slot_inner_top"].enabled;
                }
                if (i == 20)
                {
                    MoveOnLane(1, .1f);
                    result.voFemaleMotionVerified = VoAvatarMotionState == "walk" && VoAvatarUsesAlignedPaperDollMotion
                        && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 10
                        && !_voAvatarParts["lv001_female_slot_inner_top"].enabled && !_voMotionRenderer.enabled;
                }
                if (i == 21)
                {
                    AdvanceVoAnimation(.5f);
                    CycleVoAvatarMode();
                    CycleVoAvatarLevel();
                }
                if (i == 22) CycleVoAvatarLevel();
                if (i == 23)
                {
                    CycleVoAvatarLevel();
                    result.voProgressionVerified = VoAvatarLevel == 30
                        && _voAvatarParts["lv030_female_full"].enabled;
                }
                _controller.RefreshForSmoke();
                Refresh();
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
                result.frames++;
            }
            result.voSkillCastCount = VoSkillCastCount;
            result.voSkillHitCount = VoSkillHitCount;
            result.voTrainingTargetHp = VoTrainingTargetHp;
            result.activeQuestId = ActiveQuestId;
            result.completedQuestCount = CompletedQuestCount;
            result.starterSupplies = HasStarterSupplies;
            result.spiritHerb = HasHarvestedSpiritHerb;
            result.hiddenChest = HasOpenedHiddenChest;
            result.combatAccepted = HasAcceptedCombatQuest;
            result.enemyDefeated = HasDefeatedFirstEnemy;
            result.enemyLooted = HasLootedFirstEnemy;
            result.portalUnlocked = PortalUnlocked;
            result.parallaxDelta = FarOffset - initial;
            if (!result.mapQuestFlowVerified || !result.dialogueOpened || !result.greetingCompleted || !result.voBaseVerified || !result.voModularVerified
                || !result.voWalkVerified || !result.voSkillVerified || !result.voFemaleVerified || !result.voSlotToggleVerified
                || !result.voFemaleMotionVerified || !result.voProgressionVerified
                || result.voSkillCastCount != 3 || result.voSkillHitCount != 3 || result.voTrainingTargetHp != 0
                || float.IsNaN(FootY) || result.maxFootError > .001f || Mathf.Abs(result.parallaxDelta) < .01f)
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
