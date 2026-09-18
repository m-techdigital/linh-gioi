using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using LinhGioi.Foundation;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace LinhGioi.World
{
    // Playable Map01A visual slice. State remains local until the production quest backend is approved.
    [ExecuteAlways]
    public sealed partial class CongDongLamMap01AArtPreview : MonoBehaviour
    {
        public const string ResourcePath = "LGOMaps/CongDongLamMap01AArt/";
        private const float CharacterHubAnimationSettleSeconds = .24f;
        private readonly List<Sprite> _sprites = new List<Sprite>();
        private readonly Dictionary<Renderer, bool> _hidden = new Dictionary<Renderer, bool>();
        private readonly List<Tuple<Transform, Vector3, float>> _parallax = new List<Tuple<Transform, Vector3, float>>();
        private readonly List<Tuple<GameObject, float>> _interactionMarkers = new List<Tuple<GameObject, float>>();
        private TwoDOnboardingController _controller;
        private bool _previousControllerEnabled;
        private float _routeX;
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
        public bool InventoryOpen { get; private set; }
        public bool MinimapUnlocked { get; private set; }
        public bool HasUsedHealthPotion { get; private set; }
        public bool HasClassRewardItem { get; private set; }
        public bool IsClassRewardEquipped { get; private set; }
        private const string StarterRewardClassId = "vo";
        private const string StarterRewardItemId = "map01a_vo_wrist_guard_reward";
        private const string StarterRewardDisplayName = "Hộ Uyển Võ Tân Thủ";
        public string ClassRewardItemId => StarterRewardItemId;
        public string ClassRewardDisplayName => StarterRewardDisplayName;
        public bool IsClassRewardCompatible => string.Equals(
            ActiveEquipmentClassId, StarterRewardClassId, StringComparison.Ordinal);
        public int HealthPotionCount { get; private set; }
        public int ManaPotionCount { get; private set; }
        public int PlayerHealth { get; private set; } = 60;
        public int PlayerMana { get; private set; } = 50;
        public string ActiveQuestId { get; private set; } = "Q01";
        private readonly HashSet<string> _completedQuests = new HashSet<string>();
        private string _dialogueNodeId = "";
        public int CompletedQuestCount => _completedQuests.Count;
        public bool IsQuestComplete(string questId) => _completedQuests.Contains(questId);
        public string LastInteractionMessage { get; private set; } = "";
        private TwoDRegisteredOutfit _registeredOutfit;
        private bool _voJumpHeld;
        public int VoJumpStartCount { get; private set; }
        public bool VoSomersaultEnabled => _registeredOutfit != null;
        private bool RegisteredRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-vo-registered") >= 0 || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-vo-registered-equipment") >= 0;
        public static bool HasRendererAuthorityConflict(IReadOnlyList<string> args)
        {
            var registered = args.Contains("--lgo-vo-registered")
                || args.Contains("--lgo-vo-registered-equipment");
            var sourcePose = args.Contains("--lgo-vo-pose-review-dir")
                || args.Contains("--lgo-vo-pose-review-female-dir")
                || args.Contains("--lgo-source-pose-class");
            return registered && sourcePose && !args.Contains("--lgo-registered-capture");
        }
        public RuntimeUiMetricsSnapshot RuntimeUiMetrics { get; private set; }
        public void SetRuntimeUiMetrics(RuntimeUiMetricsSnapshot snapshot) => RuntimeUiMetrics = snapshot;
        public RuntimeWorldPresentationMetrics WorldPresentationMetrics { get; private set; }
        public string WorldPresentationProfileName => _worldPresentationProfile.Name;
        private RuntimeWorldPresentationProfile _worldPresentationProfile;
        public float PlayerX => _routeX;
        public string LoadedProductRuntimeClassId { get; private set; }
        public int ProductEntryFacingSign => _voState.FacingSign;
        public bool CanTalk => Mathf.Abs(PlayerX + 2.65f) <= .95f;
        public string QuestDisplayTitle => ActiveQuestId == "COMPLETE"
            ? "Cổng Đông Lâm hoàn tất"
            : ActiveQuestId + " · " + QuestName(ActiveQuestId);
        public string QuestObjectiveText => ActiveQuestId == "COMPLETE"
            ? "Portal Suối Thanh Minh đã mở."
            : QuestObjective(ActiveQuestId);
        public string QuestProgressText => ActiveQuestId == "COMPLETE"
            ? "Cổng đường sang Suối Thanh Minh đã mở."
            : "Tiến độ " + CompletedQuestCount + "/9";
        public string QuestTrackerText => QuestDisplayTitle + "\n" + QuestObjectiveText
            + (ActiveQuestId == "COMPLETE" ? "" : "\n" + QuestProgressText);
        public string MinimapRouteText => !MinimapUnlocked ? "BẢN ĐỒ KHU VỰC · CHƯA MỞ"
            : "BẢN ĐỒ ĐÔNG LÂM\nHạ Vân — Cổng — Làng — Rìa — Suối\nĐang ở: " + CurrentRouteNodeLabel;
        public string InventorySummaryText => "HÀNH TRANG TÂN THỦ\n"
            + "HP " + PlayerHealth + "/100  •  MP " + PlayerMana + "/100\n"
            + "Bình Máu Nhỏ ×" + HealthPotionCount + "  •  Bình Linh Lực Nhỏ ×" + ManaPotionCount + "\n"
            + "Hộ Uyển Võ Tân Thủ: " + (!HasClassRewardItem ? "chưa nhận" : IsClassRewardEquipped ? "đã trang bị" : "chưa trang bị");

        public void ToggleInventory()
        {
            if (DialogueOpen) return;
            InventoryOpen = !InventoryOpen;
            LastInteractionMessage = InventoryOpen ? "Đã mở hành trang tân thủ." : "Đã đóng hành trang.";
        }

        public bool UseHealthPotion()
        {
            if (!InventoryOpen || HealthPotionCount <= 0 || PlayerHealth >= 100) return false;
            HealthPotionCount--;
            PlayerHealth = Mathf.Min(100, PlayerHealth + 50);
            HasUsedHealthPotion = true;
            LastInteractionMessage = "Đã dùng Bình Máu Nhỏ · HP " + PlayerHealth + "/100.";
            if (ActiveQuestId == "Q04" && HasStarterSupplies) CompleteQuest("Q04", "Q05");
            return true;
        }

        public bool UseManaPotion()
        {
            if (!InventoryOpen || ManaPotionCount <= 0 || PlayerMana >= 100) return false;
            ManaPotionCount--;
            PlayerMana = Mathf.Min(100, PlayerMana + 50);
            LastInteractionMessage = "Đã dùng Bình Linh Lực Nhỏ · MP " + PlayerMana + "/100.";
            return true;
        }

        public bool EquipClassReward()
        {
            if (!InventoryOpen || !HasClassRewardItem || IsClassRewardEquipped || !IsClassRewardCompatible) return false;
            IsClassRewardEquipped = true;
            LastInteractionMessage = "Đã trang bị " + ClassRewardDisplayName + ".";
            if (ActiveQuestId == "Q07") CompleteQuest("Q07", "Q09");
            return true;
        }
        public void MoveOnLane(float axis, float seconds)
        {
            _voState.FaceMovement(axis);
            if (_registeredOutfit != null && Mathf.Abs(axis) <= .01f) _voState.ReleaseMovement();
            if (Mathf.Abs(axis) > .01f) _sourcePoseFacing = axis < 0 ? -1 : 1;
            // Live HUD input is advanced once by LateUpdate; explicit capture/edit-mode steps own their clock.
            if (!Application.isPlaying || IsCapturing) AdvanceVoAnimation(seconds);
            if (DialogueOpen || _controller == null || VoAvatarMotionState == "skill" || VoAvatarMotionState == "basic_attack") return;
            if (Mathf.Abs(axis) > .01f && VoAvatarMotionState != "jump")
            {
                _voState.HoldMovement(.14f);
                ApplyVoPose();
            }
            var target = Mathf.Clamp(PlayerX + Mathf.Clamp(axis, -1, 1) * Mathf.Clamp(seconds, 0, .1f) * 2.4f, -3.8f, 44.4f);
            _routeX = target;
            _controller.RefreshForSmoke();
            Refresh();
        }
        public void ApplyProductCharacterEntryState(Map01ACharacterEntryState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (Array.IndexOf(CharacterHubClassOrder, state.RuntimeClassId) < 0)
                throw new ArgumentException("Unsupported product runtime class: " + state.RuntimeClassId, nameof(state));
            if (!float.IsFinite(state.LaneX) || state.LaneX < Map01ACharacterEntryMapper.MinLaneX
                || state.LaneX > Map01ACharacterEntryMapper.MaxLaneX)
                throw new ArgumentException("Map01A product entry lane is invalid.", nameof(state));
            if (state.Facing != -1 && state.Facing != 1)
                throw new ArgumentException("Map01A product entry facing is invalid.", nameof(state));

            LoadedProductRuntimeClassId = state.RuntimeClassId;
            _routeX = state.LaneX;
            _sourcePoseFacing = state.Facing;
            _voState.FaceMovement(state.Facing);
            _controller?.RefreshForSmoke();
            ApplyVoPose();
            Refresh();
        }

        public bool TalkToHaVan() => CanTalk && CurrentRouteNodeId == "spawn-ha-van" && UseNpcConversation();

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

        private Camera _camera;
        private Transform _player;
        private SpriteRenderer _leftFoot, _rightFoot;
        private Transform _voAvatarRoot;
        private TwoDSourcePoseReview _sourcePoseReview, _femaleSourcePoseReview;
        private sealed class SourcePoseClassOption
        {
            public string Id, MalePrimary, FemalePrimary;
            public string[] MaleAlternates, FemaleAlternates;
        }
        private readonly List<SourcePoseClassOption> _sourcePoseClassOptions = new List<SourcePoseClassOption>();
        private int _sourcePoseClassIndex;
        private float _sourcePoseClassSwitchReadyAt;
        private int _sourcePoseFacing = 1;
        private readonly Dictionary<string, SpriteRenderer> _voAvatarParts = new Dictionary<string, SpriteRenderer>();
        private readonly Dictionary<string, Tuple<Vector3, Vector3>> _voAvatarPartRest = new Dictionary<string, Tuple<Vector3, Vector3>>();
        private readonly Dictionary<string, SpriteRenderer> _voEquipmentComponents = new Dictionary<string, SpriteRenderer>();
        private readonly Dictionary<string, Tuple<Vector3, Vector3>> _voEquipmentComponentRest = new Dictionary<string, Tuple<Vector3, Vector3>>();
        private readonly Dictionary<string, VoEquipmentComponent> _voEquipmentComponentInfo = new Dictionary<string, VoEquipmentComponent>();
        private readonly Dictionary<string, SpriteRenderer> _voRigParts = new Dictionary<string, SpriteRenderer>();
        private TwoDSkeletalPaperDollRig _voRig;
        private readonly Dictionary<string, Tuple<Sprite, VoAvatarPart>> _voMotionFrames = new Dictionary<string, Tuple<Sprite, VoAvatarPart>>();
        private readonly Dictionary<string, VoAttachmentProfile> _voAttachmentProfiles = new Dictionary<string, VoAttachmentProfile>();
        private readonly Dictionary<string, VoRigPoseProfile> _voRigPoseProfiles = new Dictionary<string, VoRigPoseProfile>();
        private readonly Dictionary<string, Sprite> _map01AItemIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _map01ACharacterEquipmentIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _map01ABagCategoryIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _map01ASkillIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _map01APotentialIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _map01AHudIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _map01ANpcSprites = new Dictionary<string, Sprite>();
        private bool _map01AItemIconsLoaded;
        private bool _map01ACharacterEquipmentIconsLoaded;
        private bool _map01ABagCategoryIconsLoaded;
        private bool _map01ASkillIconsLoaded;
        private bool _map01APotentialIconsLoaded;
        private bool _map01AHudIconsLoaded;
        private Texture2D _characterHubClassPortrait;
        private string _characterHubClassPortraitKey;
        private sealed class CharacterHubLoadoutState
        {
            public string Mode;
            public string SelectedSlot;
            public string[] EquippedSlots;
            public readonly Dictionary<string, int> Levels = new Dictionary<string, int>(StringComparer.Ordinal);
        }
        private static readonly string[] CharacterHubClassOrder = { "vo", "kiem", "phap", "co", "linh" };
        private readonly Dictionary<string, CharacterHubLoadoutState> _characterHubLoadoutStates =
            new Dictionary<string, CharacterHubLoadoutState>(StringComparer.Ordinal);
        private SpriteRenderer _voSkillVfx, _voMotionRenderer;
        private SpriteRenderer _voCombatTarget;
        private SpriteRenderer _spiritHerbRenderer, _hiddenChestRenderer;
        private float _voPoseYOffset;
        private bool _voPendingHit, _voBasicPendingHit;
        private static readonly string[] VoAvatarModes = { "full", "base", "modular" };
        private static readonly string[] VoAvatarGenders = { "male", "female" };
        private static readonly int[] VoAvatarLevels = { 1, 10, 20, 30 };
        private static readonly string[] VoEquipmentSlots =
        {
            "main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment",
            "waist", "arm_guard", "boots", "light_armor", "accessory"
        };
        private static readonly string[] VoReviewSlotIds =
        {
            "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
            "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory"
        };
        private readonly Dictionary<string, int> _voEquipmentLevels = new Dictionary<string, int>();
        private readonly TwoDCharacterRuntimeState _voState = new TwoDCharacterRuntimeState(
            VoAvatarModes, VoAvatarGenders, VoAvatarLevels, VoEquipmentSlots);
        public string VoAvatarMode => _voState.Mode;
        public string VoAvatarGender => _voState.Gender;
        public int VoAvatarLevel => _voState.Level;
        public int[] VoAvatarAvailableLevels => _voState.AvailableLevels;
        public string VoSelectedEquipmentSlot => _voState.SelectedEquipmentSlot;
        public int VoSelectedEquipmentItemLevel => _voEquipmentLevels.TryGetValue(VoSelectedEquipmentSlot, out var level)
            ? level : VoAvatarLevel;
        private TwoDSourcePoseReview ActiveSourcePoseReview => VoAvatarGender == "female"
            ? _femaleSourcePoseReview : _sourcePoseReview;
        private bool HasAnySourcePoseReview => _sourcePoseReview != null || _femaleSourcePoseReview != null;
        public bool IsSourcePoseReviewActive => ActiveSourcePoseReview != null;
        public bool CanCycleSourcePoseGender => _sourcePoseReview != null && _femaleSourcePoseReview != null;
        public bool CanCycleSourcePoseClass => IsSourcePoseReviewActive && _sourcePoseClassOptions.Count > 1;
        // Character Hub follows the same registered source-pose actor as the map.
        // Static MixedLoadoutFitPreview packs are review-only and must never become
        // the interactive class selector merely because no source pack was supplied.
        public bool CanCycleCharacterHubClass => CanCycleSourcePoseClass;
        public IReadOnlyList<string> CharacterHubClassIds => CharacterHubClassOrder;
        public string ActiveEquipmentClassId => ActiveSourcePoseReview?.ClassId ?? "vo";
        public string ActiveEquipmentClassLabel => ActiveSourcePoseReview?.ClassLabel ?? "Võ";
        public IReadOnlyList<string> EquipmentSlotIds => VoEquipmentSlots;
        public bool IsEquipmentSlotEquipped(string slot) => _voState.IsEquipped(slot);
        public int GetEquipmentItemLevel(string slot) => _voEquipmentLevels.TryGetValue(slot, out var level)
            ? level : CharacterLevel;
        public string GetEquipmentItemId(string slot)
        {
            var index = Array.IndexOf(VoEquipmentSlots, slot);
            if (index < 0) throw new ArgumentException("Unknown character equipment slot: " + slot, nameof(slot));
            var reviewId = ActiveSourcePoseReview?.GetSlotItemId(VoReviewSlotIds[index]);
            return string.IsNullOrEmpty(reviewId)
                ? ActiveEquipmentClassId + "_" + slot + "_lv" + GetEquipmentItemLevel(slot).ToString("000")
                : reviewId;
        }
        private EquipmentItemIconCatalog _equipmentItemIconCatalog;
        public Sprite GetEquipmentThumbnailSprite(string slot)
        {
            if (Array.IndexOf(VoEquipmentSlots, slot) < 0)
                throw new ArgumentException("Unknown character equipment slot: " + slot, nameof(slot));
            if (_equipmentItemIconCatalog == null)
            {
                const string resources = "LGOMaps/CongDongLamMap01ACharacterEquipmentIcons/";
                EnsureMap01AIconAtlasLoaded(ref _map01ACharacterEquipmentIconsLoaded, _map01ACharacterEquipmentIcons,
                    resources, "map01a-character-equipment-icons", "map01a-character-equipment-icons-v1");
                var manifest = Resources.Load<TextAsset>(resources + "manifest");
                _equipmentItemIconCatalog = EquipmentItemIconCatalog.FromJson(manifest == null ? "{}" : manifest.text,
                    iconId => _map01ACharacterEquipmentIcons.TryGetValue(iconId, out var sprite) ? sprite : null);
            }
            return _equipmentItemIconCatalog.Resolve(ActiveEquipmentClassId, GetEquipmentItemId(slot), slot,
                CharacterGender, GetEquipmentItemLevel(slot));
        }
        public Sprite GetCharacterAvatarThumbnailSprite()
        {
            if (ActiveSourcePoseReview != null) return ActiveSourcePoseReview.CurrentBodySprite;
            if (!string.Equals(ActiveEquipmentClassId, "vo", StringComparison.OrdinalIgnoreCase)) return null;
            var partId = "lv" + VoAvatarLevel.ToString("000") + "_" + VoAvatarGender + "_full";
            return _voAvatarParts.TryGetValue(partId, out var renderer) ? renderer.sprite : null;
        }

        // Shared Character Hub contract. The source-pose actor supplies the active
        // class data; UI code must not branch on a Võ-specific presentation API.
        public string CharacterAvatarMode => _voState.Mode;
        public string CharacterGender => _voState.Gender;
        public int CharacterLevel => _voState.Level;
        public string SelectedEquipmentSlot => _voState.SelectedEquipmentSlot;
        public int SelectedEquipmentItemLevel => GetEquipmentItemLevel(SelectedEquipmentSlot);
        public int EquippedSlotCount => _voState.EquippedSlotCount;
        public bool HasEquipmentItemVariant(string slot)
        {
            if (ActiveSourcePoseReview == null) return false;
            var index = Array.IndexOf(VoEquipmentSlots, slot);
            if (index < 0) throw new ArgumentException("Unknown character equipment slot: " + slot, nameof(slot));
            var current = GetEquipmentItemLevel(slot);
            return ActiveSourcePoseReview.NextSlotItemLevel(VoReviewSlotIds[index], current) != current;
        }
        public bool CharacterRunEnabled => VoRunEnabled;
        public bool CanTriggerCharacterSkill => CanTriggerVoSkill;
        public void SetCharacterRun(bool enabled) => SetVoRun(enabled);
        public void SetCharacterJumpHeld(bool held) => SetVoJumpHeld(held);
        public void TriggerCharacterBasicAttack() => TriggerVoBasicAttack();
        public void TriggerCharacterSkill() => TriggerVoSkill();

        // Registered-outfit capture is isolated WIP. Keep these compatibility
        // names for its evidence tooling while product UI binds only the contract above.
        public IReadOnlyList<string> VoEquipmentSlotIds => EquipmentSlotIds;
        public bool IsVoEquipmentSlotEquipped(string slot) => IsEquipmentSlotEquipped(slot);
        public int GetVoEquipmentItemLevel(string slot) => GetEquipmentItemLevel(slot);
        public string GetVoEquipmentItemId(string slot) => GetEquipmentItemId(slot);
        public Sprite GetVoEquipmentThumbnailSprite(string slot) => GetEquipmentThumbnailSprite(slot);
        public Sprite GetVoAvatarThumbnailSprite() => GetCharacterAvatarThumbnailSprite();
        public Texture2D GetCharacterHubAvatarPreviewTexture()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) return null;
            var actor = ActiveSourcePoseReview != null ? ActiveSourcePoseReview.transform : _voAvatarRoot;
            if (actor == null || !actor.gameObject.activeInHierarchy) return null;
            var renderers = actor.GetComponentsInChildren<SpriteRenderer>(false)
                .Where(renderer => renderer.enabled && renderer.sprite != null)
                .ToArray();
            if (renderers.Length == 0) return null;
            var key = ActiveEquipmentClassId + "|" + VoAvatarGender + "|"
                + string.Join(";", renderers.Select(renderer => renderer.sprite.GetInstanceID() + ":" + renderer.sortingOrder));
            if (_characterHubClassPortrait != null && _characterHubClassPortraitKey == key)
                return _characterHubClassPortrait;
            if (_characterHubClassPortrait != null)
            {
                if (Application.isPlaying) Destroy(_characterHubClassPortrait);
                else DestroyImmediate(_characterHubClassPortrait);
            }
            _characterHubClassPortrait = RenderAvatarPortrait(renderers, 400, 428,
                "Character Hub " + ActiveEquipmentClassLabel + " active actor portrait");
            _characterHubClassPortraitKey = key;
            return _characterHubClassPortrait;
        }

        private static Texture2D RenderAvatarPortrait(SpriteRenderer[] renderers, int width, int height, string textureName)
        {
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            var previousLayers = renderers.Select(renderer => renderer.gameObject.layer).ToArray();
            var cameraHost = new GameObject(textureName + " camera");
            var camera = cameraHost.AddComponent<Camera>();
            var target = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            var previousActive = RenderTexture.active;
            try
            {
                const int portraitLayer = 30;
                foreach (var renderer in renderers) renderer.gameObject.layer = portraitLayer;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.clear;
                camera.orthographic = true;
                camera.allowHDR = false;
                camera.allowMSAA = false;
                camera.cullingMask = 1 << portraitLayer;
                camera.aspect = width / (float)height;
                camera.orthographicSize = Mathf.Max(bounds.extents.y, bounds.extents.x / camera.aspect) * 1.08f;
                camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.min.z - 10f);
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                var texture = new Texture2D(width, height, TextureFormat.RGBA32, false) { name = textureName };
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply(false, false);
                return texture;
            }
            finally
            {
                for (var index = 0; index < renderers.Length; index++) renderers[index].gameObject.layer = previousLayers[index];
                RenderTexture.active = previousActive;
                camera.targetTexture = null;
                target.Release();
                if (Application.isPlaying)
                {
                    Destroy(target);
                    Destroy(cameraHost);
                }
                else
                {
                    DestroyImmediate(target);
                    DestroyImmediate(cameraHost);
                }
            }
        }
        public Sprite GetMap01AItemThumbnailSprite(string itemId)
        {
            EnsureMap01AIconAtlasLoaded(ref _map01AItemIconsLoaded, _map01AItemIcons,
                "LGOMaps/CongDongLamMap01AItems/", "map01a-item-icons", "map01a-item-icons-v1");
            return _map01AItemIcons.TryGetValue(itemId, out var sprite) ? sprite : null;
        }
        public Sprite GetMap01ACharacterEquipmentIconSprite(string slot)
        {
            var index = Array.IndexOf(VoEquipmentSlots, slot);
            if (index < 0) throw new ArgumentException("Unknown character equipment slot: " + slot, nameof(slot));
            EnsureMap01AIconAtlasLoaded(ref _map01ACharacterEquipmentIconsLoaded, _map01ACharacterEquipmentIcons,
                "LGOMaps/CongDongLamMap01ACharacterEquipmentIcons/", "map01a-character-equipment-icons",
                "map01a-character-equipment-icons-v1");
            var iconId = VoReviewSlotIds[index];
            return _map01ACharacterEquipmentIcons.TryGetValue(iconId, out var sprite) ? sprite : null;
        }

        public Sprite GetMap01ABagCategoryIconSprite(string categoryId)
        {
            EnsureMap01AIconAtlasLoaded(ref _map01ABagCategoryIconsLoaded, _map01ABagCategoryIcons,
                "LGOMaps/CongDongLamMap01ABagCategoryIcons/", "map01a-bag-category-icons",
                "map01a-bag-category-icons-v1");
            return _map01ABagCategoryIcons.TryGetValue(categoryId, out var sprite) ? sprite : null;
        }

        public Sprite GetMap01ASkillIconSprite(string iconId)
        {
            EnsureMap01AIconAtlasLoaded(ref _map01ASkillIconsLoaded, _map01ASkillIcons,
                "LGOMaps/CongDongLamMap01ASkillIcons/", "map01a-skill-icons", "map01a-skill-icons-v1");
            return _map01ASkillIcons.TryGetValue(iconId, out var sprite) ? sprite : null;
        }

        public Sprite GetMap01APotentialIconSprite(string iconId)
        {
            EnsureMap01AIconAtlasLoaded(ref _map01APotentialIconsLoaded, _map01APotentialIcons,
                "LGOMaps/CongDongLamMap01APotentialIcons/", "map01a-potential-icons", "map01a-potential-icons-v1");
            return _map01APotentialIcons.TryGetValue(iconId, out var sprite) ? sprite : null;
        }

        private void EnsureMap01AIconAtlasLoaded(ref bool loaded, Dictionary<string, Sprite> sprites,
            string resourcePath, string atlasName, string manifestId)
        {
            if (loaded) return;
            loaded = true;
            var manifestAsset = Resources.Load<TextAsset>(resourcePath + "manifest");
            var atlas = Resources.Load<Texture2D>(resourcePath + atlasName);
            if (manifestAsset == null || atlas == null) return;
            var manifest = JsonUtility.FromJson<Map01AItemIconManifest>(manifestAsset.text);
            if (manifest == null || manifest.id != manifestId
                || manifest.status != "DRAFT_RUNTIME_REVIEW" || manifest.parts == null)
                return;
            foreach (var part in manifest.parts)
            {
                if (string.IsNullOrEmpty(part.id) || part.w <= 0 || part.h <= 0
                    || part.x < 0 || part.y < 0 || part.x + part.w > atlas.width || part.y + part.h > atlas.height
                    || sprites.ContainsKey(part.id))
                    continue;
                sprites.Add(part.id, MakeSprite(atlas, new Rect(part.x, part.y, part.w, part.h)));
            }
        }
        public Sprite GetMap01AHudIconSprite(string iconId)
        {
            if (!_map01AHudIconsLoaded)
            {
                _map01AHudIconsLoaded = true;
                const string path = "LGOMaps/CongDongLamMap01AHudIcons/";
                var manifestAsset = Resources.Load<TextAsset>(path + "manifest");
                var atlas = Resources.Load<Texture2D>(path + "map01a-hud-icons");
                if (manifestAsset != null && atlas != null)
                {
                    var manifest = JsonUtility.FromJson<Map01AItemIconManifest>(manifestAsset.text);
                    if (manifest != null && manifest.id == "map01a-hud-icons-v1"
                        && manifest.status == "DRAFT_RUNTIME_REVIEW" && manifest.parts != null)
                    {
                        foreach (var part in manifest.parts)
                        {
                            if (string.IsNullOrEmpty(part.id) || part.w <= 0 || part.h <= 0
                                || part.x < 0 || part.y < 0 || part.x + part.w > atlas.width || part.y + part.h > atlas.height
                                || _map01AHudIcons.ContainsKey(part.id))
                                continue;
                            _map01AHudIcons.Add(part.id, MakeSprite(atlas, new Rect(part.x, part.y, part.w, part.h)));
                        }
                    }
                }
            }
            return _map01AHudIcons.TryGetValue(iconId, out var sprite) ? sprite : null;
        }
        public Sprite GetCurrentDialogueNpcSprite()
        {
            string npcId;
            switch (_dialogueNodeId)
            {
                case "spawn-ha-van": npcId = "ha-van"; break;
                case "quan-thu": npcId = "quan-thu-dong-lam"; break;
                case "tong-phu": npcId = "tong-phu"; break;
                case "thanh-nhi": npcId = "thanh-nhi"; break;
                case "lao-tran": npcId = "lao-tran"; break;
                case "well-bridge": npcId = "tieu-dong"; break;
                default: return null;
            }
            return _map01ANpcSprites.TryGetValue(npcId, out var sprite) ? sprite : null;
        }
        public bool HasVoEquipmentItemVariant(string slot)
            => HasEquipmentItemVariant(slot);
        public string VoMixedEquipmentSnapshot => string.Join(",", VoEquipmentSlots.Select(slot =>
            slot + "=Lv" + (_voEquipmentLevels.TryGetValue(slot, out var level) ? level : VoAvatarLevel)));
        public int VoEquippedSlotCount => _voState.EquippedSlotCount;
        public string VoAvatarMotionState => _voState.MotionState;
        public string VoAvatarMotionFrameId { get; private set; } = "idle";
        public bool VoRunEnabled => _voState.RunEnabled;
        public string AvatarClassLabel => ActiveSourcePoseReview?.ClassLabel ?? "Võ";
        public string EquipmentFitSummary => (ActiveSourcePoseReview?.ClassLabel ?? "Võ") + " "
            + (VoAvatarGender == "female" ? "nữ" : "nam") + " · cùng canvas/pivot · đủ 6 pose";
        public string EquipmentLevelLabel => "Lv" + VoAvatarLevel;
        public string EquipmentSlotLabel => VoSelectedEquipmentSlot + " · Lv" + VoSelectedEquipmentItemLevel;
        public string SkillLabel => "Liên Quyền";
        public Vector2 VoAvatarMotionScale => _voAvatarRoot == null ? Vector2.one : _voAvatarRoot.localScale;
        public bool VoAvatarUsesFrameMotion => _registeredOutfit == null && VoAvatarLevel == 1 && VoAvatarMode == "full" && VoAvatarMotionState != "idle";
        public bool VoAvatarUsesAlignedPaperDollMotion => VoAvatarMotionState != "idle" && !VoAvatarUsesFrameMotion;
        public int VoSkillCastCount { get; private set; }
        public int VoSkillHitCount { get; private set; }
        public int VoTrainingTargetHp { get; private set; } = 100;
        public bool CanTriggerVoSkill => !DialogueOpen && !_voState.HasActiveAction && _voCombatTarget != null
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
            public VoAttachmentProfile[] attachmentProfiles;
            public VoEquipmentComponent[] equipmentComponents;
            public VoAvatarPart[] rigParts;
            public VoRigPoseProfile[] rigPoseProfiles;
        }
        [Serializable] private sealed class VoAvatarPart
        {
            public string id;
            public string gender, kind, slot, atlas, part, parent;
            public int level;
            public int x, y, w, h, order;
            public float worldW, worldH, dx, dy, pivotX, pivotY;
        }
        [Serializable] private sealed class VoAttachmentProfile
        {
            public string id, gender, pose, slot;
            public float dx, dy, rotation, scaleX, scaleY;
        }
        [Serializable] private sealed class VoEquipmentComponent
        {
            public string id, gender, slot, side, bone, atlas;
            public int level;
            public int x, y, w, h, order;
            public float worldW, worldH, dx, dy;
        }
        [Serializable] private sealed class VoRigPoseProfile
        {
            public string id, gender, pose, part;
            public float dx, dy, rotation;
        }
        [Serializable] private sealed class Map01AItemIconManifest
        {
            public string id, status;
            public Map01AItemIconPart[] parts;
        }
        [Serializable] private sealed class Map01AItemIconPart
        {
            public string id;
            public int x, y, w, h;
        }
        [Serializable] private sealed class GroundInfo { public float groundY; }
        public float GroundY { get; private set; }
        public float FootY => _leftFoot == null || _rightFoot == null ? float.NaN
            : Mathf.Min(_leftFoot.bounds.min.y, _rightFoot.bounds.min.y);
        private float _previousCameraSize;
        private Vector3 _previousCameraPosition;
        private Transform _farLayer;
        private Vector3 _farScale;
        private float _farCoverageScale = 1f;
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
        public float CurrentRouteProgress => RouteNodeIds.Length <= 1 ? 0f : (float)_currentRouteIndex / (RouteNodeIds.Length - 1);
        public Vector3 CurrentInteractionPosition => new Vector3(RouteNodeX[_currentRouteIndex], GroundY + 2.15f, 0);
        public bool CanUseCurrentRouteAction
        {
            get
            {
                if (Mathf.Abs(PlayerX - RouteNodeX[_currentRouteIndex]) > .95f) return false;
                if (DialogueOpen) return _dialogueNodeId == CurrentRouteNodeId;
                if (CanTalkToCurrentNpc) return true;
                if (CurrentRouteNodeId == "grand-gate") return ActiveQuestId == "Q02";
                if (CurrentRouteNodeId == "village-square") return ActiveQuestId == "Q04" && !HasInspectedInventory;
                if (CurrentRouteNodeId == "combat-edge") return ActiveQuestId == "Q07" && HasDefeatedFirstEnemy && !HasLootedFirstEnemy;
                if (CurrentRouteNodeId == "portal-suoi-thanh-minh") return ActiveQuestId == "Q09" && !PortalUnlocked;
                return false;
            }
        }
        public string CurrentActionLabel
        {
            get
            {
                if (DialogueOpen) return DialogueActionLabel;
                if (CurrentRouteNodeId == "grand-gate") return "Nhìn về Linh Thành";
                if (CurrentRouteNodeId == "village-square") return "Mở hành trang";
                if (CurrentRouteNodeId == "well-bridge") return ActiveQuestId == "Q05" && HasAcceptedGatherQuest && !HasHarvestedSpiritHerb
                    ? "Hái Linh Thảo" : HasHarvestedSpiritHerb && !HasOpenedHiddenChest ? "Mở rương ẩn" : "Trò chuyện";
                if (CanTalkToCurrentNpc) return "Trò chuyện";
                if (CurrentRouteNodeId == "combat-edge") return "Nhặt chiến lợi phẩm";
                if (CurrentRouteNodeId == "portal-suoi-thanh-minh") return PortalUnlocked ? "Đã mở" : "Mở lối";
                return "Tương tác";
            }
        }
        public bool UseCurrentRouteAction()
        {
            if (!CanUseCurrentRouteAction) return false;
            if (DialogueOpen) return AdvanceNpcDialogue();
            if (CanTalkToCurrentNpc && CurrentRouteNodeId != "well-bridge") return UseNpcConversation();
            if (CurrentRouteNodeId == "grand-gate" && ActiveQuestId == "Q02")
            {
                MinimapUnlocked = true;
                CompleteQuest("Q02", "Q03");
                LastInteractionMessage = "Đã nhìn thấy Linh Thành ở phía xa.";
                return true;
            }
            if (CurrentRouteNodeId == "village-square" && ActiveQuestId == "Q04" && !HasInspectedInventory)
            {
                HasInspectedInventory = true;
                InventoryOpen = true;
                LastInteractionMessage = "Đã xem hành trang tân thủ; hãy gặp Tổng Phú.";
                return true;
            }
            if (CurrentRouteNodeId == "well-bridge" && ActiveQuestId == "Q05" && HasAcceptedGatherQuest && !HasHarvestedSpiritHerb)
            {
                HasHarvestedSpiritHerb = true;
                if (_spiritHerbRenderer != null) _spiritHerbRenderer.enabled = false;
                CompleteQuest("Q05", "Q06");
                LastInteractionMessage = "Đã thu thập Linh Thảo Non.";
                return true;
            }
            if (CurrentRouteNodeId == "well-bridge" && HasHarvestedSpiritHerb && !HasOpenedHiddenChest)
            {
                HasOpenedHiddenChest = true;
                if (_hiddenChestRenderer != null) _hiddenChestRenderer.color = new Color(.62f, .62f, .62f, .58f);
                _completedQuests.Add("Q08");
                LastInteractionMessage = "Q08 hoàn tất · rương ẩn: 12 Vàng + Bánh Bao.";
                return true;
            }
            if (CurrentRouteNodeId == "well-bridge") return UseNpcConversation();
            if (CurrentRouteNodeId == "combat-edge" && ActiveQuestId == "Q07" && HasDefeatedFirstEnemy)
            {
                HasLootedFirstEnemy = true;
                HasClassRewardItem = true;
                InventoryOpen = true;
                LastInteractionMessage = "Nhặt Da Lợn Non + 8 Vàng + Hộ Uyển Võ Tân Thủ; hãy trang bị.";
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
        public static bool ShouldRun() => ShouldRunForArgs(Environment.GetCommandLineArgs());

        public static bool ShouldRunForArgs(string[] args)
            => Array.IndexOf(args, "--lgo-map01a-art-preview") >= 0 || HasCaptureRequestForArgs(args);

        private static string RequestedWorldPresentationProfile(string[] args)
        {
            if (args == null) return null;
            foreach (var key in new[] { "--lgo-map01a-device", "--lgo-device-profile" })
            {
                var index = Array.IndexOf(args, key);
                if (index >= 0 && index + 1 < args.Length) return args[index + 1];
            }
            return null;
        }

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
            if (HasRendererAuthorityConflict(Environment.GetCommandLineArgs()))
                throw new InvalidOperationException(
                    "Map01A Player selected registered and source-pose renderers together");
            _worldPresentationProfile = RuntimeWorldPresentationProfile.FromScreen(
                RequestedWorldPresentationProfile(Environment.GetCommandLineArgs()), Screen.width, Screen.height);
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
            _sourcePoseReview = TwoDSourcePoseReview.CreateIfRequested(transform);
            _femaleSourcePoseReview = TwoDSourcePoseReview.CreateIfRequested(transform,
                "--lgo-vo-pose-review-female-dir", "Source pose review — female stack");
            LoadSourcePoseClassOptions();
            if (HasAnySourcePoseReview)
            {
                _registeredOutfit?.SetPresentationVisible(false);
                if (!IsCapturing)
                {
                    InventoryOpen = true;
                    LastInteractionMessage = "Hành trang POSE THỬ đã mở · chọn trực tiếp 10 món để tháo/mặc.";
                }
            }
            ApplyVoPose();
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
                // Frame the gameplay lane from the shared world-presentation contract.
                // Aspect-specific evidence may change this contract later; product code does not own literals.
                _camera.orthographicSize = _worldPresentationProfile.CameraOrthographicSize;
                _camera.transform.position = new Vector3(
                    _camera.transform.position.x,
                    GroundY + _worldPresentationProfile.CameraGroundOffsetY,
                    _camera.transform.position.z);
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
                { "rig", Resources.Load<Texture2D>(path + "vo-lv1-rig-atlas") },
            };
            if (manifest == null || textures.Values.Any(texture => texture == null))
                throw new InvalidOperationException("Missing reviewed Võ Lv1-30 map avatar pack");
            var pack = JsonUtility.FromJson<VoAvatarPackInfo>(manifest.text);
            if (pack.id != "vo-lv1-30-map-avatar-v9" || pack.status != "DRAFT_RUNTIME_REVIEW"
                || pack.parts == null || pack.parts.Length != 96 || pack.genders == null || pack.genders.Length != 2
                || pack.slots == null || pack.slots.Length != 10 || pack.levels == null || pack.levels.Length != 4
                || pack.attachmentProfiles == null || pack.attachmentProfiles.Length != 120
                || pack.equipmentComponents == null || pack.equipmentComponents.Length != 112
                || pack.rigParts == null || pack.rigParts.Length != 20
                || pack.rigPoseProfiles == null || pack.rigPoseProfiles.Length != 120)
                throw new InvalidOperationException("Invalid Võ Lv1-30 map avatar manifest");
            _voState.EquipAllExcept(null);
            foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = VoAvatarLevel;
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
                _voAvatarPartRest.Add(part.id, Tuple.Create(host.transform.localPosition, host.transform.localScale));
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
            foreach (var profile in pack.attachmentProfiles)
            {
                var key = profile.gender + "_" + profile.pose + "_" + profile.slot;
                if (profile.id != key || !VoAvatarGenders.Contains(profile.gender)
                    || !VoEquipmentSlots.Contains(profile.slot) || profile.scaleX <= 0 || profile.scaleY <= 0
                    || _voAttachmentProfiles.ContainsKey(key))
                    throw new InvalidOperationException("Invalid Võ attachment profile: " + profile.id);
                _voAttachmentProfiles.Add(key, profile);
            }
            var rigDefinitions = pack.rigParts.Select(part => new TwoDSkeletalPaperDollRig.BoneDefinition(
                part.id, string.IsNullOrEmpty(part.parent) ? "" : part.gender + "_" + part.parent,
                new Vector2(part.pivotX, part.pivotY), "Map01A Võ bone " + part.gender + " " + part.part)).ToArray();
            _voRig = new TwoDSkeletalPaperDollRig(_voAvatarRoot);
            _voRig.Build(rigDefinitions);
            foreach (var part in pack.rigParts)
            {
                if (part.atlas != "rig" || part.w <= 0 || part.h <= 0 || part.worldW <= 0 || part.worldH <= 0
                    || part.x < 0 || part.y < 0 || part.x + part.w > textures["rig"].width
                    || part.y + part.h > textures["rig"].height || _voRigParts.ContainsKey(part.id))
                    throw new InvalidOperationException("Invalid Võ rig part: " + part.id);
                var sprite = MakeSprite(textures["rig"], new Rect(part.x, part.y, part.w, part.h));
                var renderer = _voRig.Attach(part.id, "Map01A Võ rig " + part.gender + " " + part.part,
                    sprite, new Vector2(part.dx, part.dy), new Vector2(part.worldW, part.worldH), part.order);
                renderer.enabled = false;
                _voRigParts.Add(part.id, renderer);
            }
            foreach (var component in pack.equipmentComponents)
            {
                if (!textures.TryGetValue(component.atlas, out var texture)
                    || component.x < 0 || component.y < 0 || component.w <= 0 || component.h <= 0
                    || component.x + component.w > texture.width || component.y + component.h > texture.height
                    || component.worldW <= 0 || component.worldH <= 0
                    || !VoEquipmentSlots.Contains(component.slot)
                    || !new[] { "left", "right", "center" }.Contains(component.side)
                    || _voEquipmentComponents.ContainsKey(component.id))
                    throw new InvalidOperationException("Invalid Võ equipment component: " + component.id);
                var boneId = component.gender + "_" + component.bone;
                var objectName = "Map01A Võ equipment component lv" + component.level.ToString("000")
                    + " " + component.gender + " " + component.slot + " " + component.side;
                var sprite = MakeSprite(texture, new Rect(component.x, component.y, component.w, component.h));
                var renderer = _voRig.Attach(boneId, objectName, sprite, new Vector2(component.dx, component.dy),
                    new Vector2(component.worldW, component.worldH), component.order);
                renderer.enabled = false;
                _voEquipmentComponents.Add(component.id, renderer);
                _voEquipmentComponentRest.Add(component.id,
                    Tuple.Create(renderer.transform.localPosition, renderer.transform.localScale));
                _voEquipmentComponentInfo.Add(component.id, component);
            }
            foreach (var profile in pack.rigPoseProfiles)
            {
                var key = profile.gender + "_" + profile.pose + "_" + profile.part;
                if (profile.id != key || _voRigPoseProfiles.ContainsKey(key))
                    throw new InvalidOperationException("Invalid Võ rig pose: " + profile.id);
                _voRigPoseProfiles.Add(key, profile);
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
            if (pack.motionFrames == null || pack.motionFrames.Length != 28)
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
            if (RegisteredRequested)
            {
                var equipment = Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-vo-registered-equipment") >= 0;
                var closedBody = equipment || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-vo-closed-body") >= 0;
                var closedArms = Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-vo-closed-far-arms") >= 0;
                var anatomy = closedBody || closedArms || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-vo-anatomical") >= 0;
                _registeredOutfit = new TwoDRegisteredOutfit(_voAvatarRoot, _sprites, true, anatomy, closedArms, closedBody, equipment);
                _voSkillVfx.transform.SetParent(_registeredOutfit.FacingRoot, false);
                foreach (var renderer in _voAvatarParts.Values) renderer.enabled = false;
                foreach (var renderer in _voRigParts.Values) renderer.enabled = false;
                foreach (var renderer in _voEquipmentComponents.Values) renderer.enabled = false;
            }
            SetVoMotionFrame("idle");
            _voMotionRenderer.enabled = false;
            RefreshVoAvatarMode();
        }

        public void CycleVoAvatarMode()
        {
            _voState.CycleMode();
            RefreshVoAvatarMode();
        }

        public void CycleCharacterAvatarMode() => CycleVoAvatarMode();

        public void CycleVoAvatarGender()
        {
            _voState.CycleGender();
            RefreshVoAvatarMode();
        }

        public void CycleCharacterGender() => CycleVoAvatarGender();

        public void CycleSourcePoseClass()
        {
            if (!CanCycleSourcePoseClass || Time.realtimeSinceStartup < _sourcePoseClassSwitchReadyAt) return;
            SaveCharacterHubLoadout(ActiveEquipmentClassId);
            _sourcePoseClassIndex = (_sourcePoseClassIndex + 1) % _sourcePoseClassOptions.Count;
            var option = _sourcePoseClassOptions[_sourcePoseClassIndex];
            _sourcePoseReview = ReloadClassGender(_sourcePoseReview, option.MalePrimary, option.MaleAlternates);
            _femaleSourcePoseReview = ReloadClassGender(_femaleSourcePoseReview, option.FemalePrimary, option.FemaleAlternates);
            if (ActiveSourcePoseReview == null) _voState.CycleGender();
            RestoreCharacterHubLoadout(ActiveEquipmentClassId);
            var complete = ActiveSourcePoseReview.GetCompleteItemLevels();
            if (!complete.Contains(VoAvatarLevel))
            {
                var selectedLevel = complete.FirstOrDefault();
                if (selectedLevel > 0)
                    foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = selectedLevel;
            }
            RefreshVoAvatarMode();
            ApplyVoPose();
            // Loading both gender stacks is synchronous in this review tool. Ignore key-repeat
            // events queued while the main thread was loading so one press advances one class.
            _sourcePoseClassSwitchReadyAt = Time.realtimeSinceStartup + .5f;
            LastInteractionMessage = "Đã đổi class sang " + ActiveEquipmentClassLabel + " · "
                + (VoAvatarGender == "female" ? "nữ" : "nam") + " · giữ trạng thái tháo/mặc.";
        }

        public void CycleCharacterHubClass()
        {
            if (CanCycleSourcePoseClass) CycleSourcePoseClass();
        }

        private void SaveCharacterHubLoadout(string classId)
        {
            if (Array.IndexOf(CharacterHubClassOrder, classId) < 0) return;
            var state = new CharacterHubLoadoutState
            {
                Mode = _voState.Mode,
                SelectedSlot = _voState.SelectedEquipmentSlot,
                EquippedSlots = VoEquipmentSlots.Where(_voState.IsEquipped).ToArray()
            };
            foreach (var slot in VoEquipmentSlots)
                state.Levels[slot] = GetVoEquipmentItemLevel(slot);
            _characterHubLoadoutStates[classId] = state;
        }

        private void RestoreCharacterHubLoadout(string classId)
        {
            if (!_characterHubLoadoutStates.TryGetValue(classId, out var state))
            {
                state = new CharacterHubLoadoutState
                {
                    Mode = "modular",
                    SelectedSlot = VoEquipmentSlots[0],
                    EquippedSlots = (string[])VoEquipmentSlots.Clone()
                };
                foreach (var slot in VoEquipmentSlots) state.Levels[slot] = 1;
                _characterHubLoadoutStates[classId] = state;
            }
            _voState.SelectMode(state.Mode);
            _voState.SetEquipmentState(state.SelectedSlot, state.EquippedSlots);
            _voEquipmentLevels.Clear();
            foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = state.Levels[slot];
        }

        private TwoDSourcePoseReview ReloadClassGender(TwoDSourcePoseReview review, string primary, string[] alternates)
        {
            if (string.IsNullOrEmpty(primary) || primary == "-")
            {
                if (review != null)
                {
                    review.SetPresentationVisible(false);
                    if (Application.isPlaying) Destroy(review.gameObject);
                    else DestroyImmediate(review.gameObject);
                }
                return null;
            }
            if (review == null)
            {
                var host = new GameObject("Source pose class stack");
                host.transform.SetParent(transform, false);
                review = host.AddComponent<TwoDSourcePoseReview>();
            }
            review.ReloadPack(primary, alternates);
            return review;
        }

        private void LoadSourcePoseClassOptions()
        {
            _sourcePoseClassOptions.Clear();
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length; index++)
            {
                if (args[index] != "--lgo-source-pose-class") continue;
                if (index + 5 >= args.Length) throw new ArgumentException("Source pose class requires id and four pack path fields");
                var option = new SourcePoseClassOption
                {
                    Id = args[index + 1],
                    MalePrimary = args[index + 2],
                    MaleAlternates = SplitPackPaths(args[index + 3]),
                    FemalePrimary = args[index + 4],
                    FemaleAlternates = SplitPackPaths(args[index + 5])
                };
                if (string.IsNullOrEmpty(option.Id)
                    || (SplitPackPaths(option.MalePrimary).Length == 0 && SplitPackPaths(option.FemalePrimary).Length == 0)
                    || _sourcePoseClassOptions.Any(existing => existing.Id == option.Id))
                    throw new ArgumentException("Invalid/duplicate source pose class option: " + option.Id);
                _sourcePoseClassOptions.Add(option);
                index += 5;
            }
            if (_sourcePoseClassOptions.Count == 0 || !HasAnySourcePoseReview) return;
            _sourcePoseClassIndex = _sourcePoseClassOptions.FindIndex(option => option.Id == (_sourcePoseReview ?? _femaleSourcePoseReview).ClassId);
            if (_sourcePoseClassIndex < 0)
                throw new ArgumentException("Active source pose class is absent from --lgo-source-pose-class options");
        }

        private static string[] SplitPackPaths(string value)
            => string.IsNullOrEmpty(value) || value == "-" ? Array.Empty<string>()
                : value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        public void CycleVoAvatarLevel()
        {
            if (ActiveSourcePoseReview != null)
            {
                var next = ActiveSourcePoseReview.NextCompleteItemLevel(VoAvatarLevel);
                for (var step = 0; step < VoAvatarLevels.Length && VoAvatarLevel != next; step++) _voState.CycleLevel();
                foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = next;
                RefreshVoAvatarMode();
                return;
            }
            if (_registeredOutfit != null) { LastInteractionMessage = "Đang kiểm chứng Võ Lv1 · Lv10 chưa mở"; return; }
            _voState.CycleLevel();
            foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = VoAvatarLevel;
            RefreshVoAvatarMode();
        }

        public void CycleCharacterLevel() => CycleVoAvatarLevel();

        public void CycleVoEquipmentSlot()
        {
            _voState.CycleEquipmentSlot();
        }

        public void CycleEquipmentSlot() => CycleVoEquipmentSlot();

        public void SelectVoEquipmentSlot(string slot)
        {
            _voState.SelectEquipmentSlot(slot);
            LastInteractionMessage = "Đã chọn " + slot + ".";
        }

        public void SelectEquipmentSlot(string slot) => SelectVoEquipmentSlot(slot);

        public void ToggleVoEquipmentSlot()
        {
            _voState.ToggleSelectedEquipmentSlot();
            RefreshVoAvatarMode();
        }

        public void ToggleEquipmentSlot() => ToggleVoEquipmentSlot();

        public void CycleVoSelectedEquipmentItemLevel()
        {
            if (ActiveSourcePoseReview != null)
            {
                _voEquipmentLevels[VoSelectedEquipmentSlot] = ActiveSourcePoseReview.NextSlotItemLevel(
                    VoReviewSlotIds[Array.IndexOf(VoEquipmentSlots, VoSelectedEquipmentSlot)], VoSelectedEquipmentItemLevel);
                RefreshVoAvatarMode();
                return;
            }
            if (_registeredOutfit != null) { LastInteractionMessage = "Đang kiểm chứng Võ Lv1 · Lv10 chưa mở"; return; }
            var current = VoSelectedEquipmentItemLevel;
            var index = Array.IndexOf(VoAvatarLevels, current);
            _voEquipmentLevels[VoSelectedEquipmentSlot] = VoAvatarLevels[(index + 1) % VoAvatarLevels.Length];
            RefreshVoAvatarMode();
        }

        public void CycleSelectedEquipmentItemLevel() => CycleVoSelectedEquipmentItemLevel();

        private void RefreshVoAvatarMode()
        {
            // A partial source catalog must not silently revive the legacy renderer.
            if (HasAnySourcePoseReview && ActiveSourcePoseReview == null)
                _voState.CycleGender();
            foreach (var review in new[] { _sourcePoseReview, _femaleSourcePoseReview })
                if (review != null)
                    for (var index = 0; index < VoEquipmentSlots.Length; index++)
                {
                    review.SetSlotItemLevel(VoReviewSlotIds[index],
                        _voEquipmentLevels.TryGetValue(VoEquipmentSlots[index], out var level) ? level : VoAvatarLevel);
                    review.SetSlotVisible(VoReviewSlotIds[index], _voState.IsEquipped(VoEquipmentSlots[index]));
                }
            var activeReview = ActiveSourcePoseReview;
            _sourcePoseReview?.SetPresentationVisible(activeReview == _sourcePoseReview);
            _femaleSourcePoseReview?.SetPresentationVisible(activeReview == _femaleSourcePoseReview);
            if (activeReview != null)
            {
                // Source-pose is the canonical actor for this review session. Keep the
                // older atlas/rig tree physically non-renderable so later loadout or
                // motion refreshes cannot accidentally show a second character.
                SetLegacyVoRenderersVisible(false);
                _registeredOutfit?.SetPresentationVisible(false);
                return;
            }
            SetLegacyVoRenderersVisible(true);
            if (_registeredOutfit != null)
            {
                _registeredOutfit.SetPresentationVisible(true);
                var cycle = _voState.AnimationPhase * (VoAvatarMotionState == "run" ? 4.4f : 3.5f);
                var weight = TwoDPaperDollPoseSampler.Sample(VoAvatarMotionState, cycle, _voState.ActionProgress);
                _registeredOutfit.Apply(VoAvatarGender, VoAvatarMode == "base", _voState.IsEquipped,
                    part => _voRigPoseProfiles.TryGetValue(VoAvatarGender + "_" + VoAvatarMotionState + "_" + part, out var profile)
                        ? profile.rotation * weight : 0);
                _registeredOutfit.ApplyMovement(VoAvatarGender, VoAvatarMotionState, _voState.AnimationPhase, _voState.ActionProgress, _voState.FacingSign);
                return;
            }

            var prefix = "lv" + VoAvatarLevel.ToString("000") + "_" + VoAvatarGender + "_";
            foreach (var pair in _voAvatarParts)
            {
                var rest = _voAvatarPartRest[pair.Key];
                pair.Value.transform.localPosition = rest.Item1;
                pair.Value.transform.localRotation = Quaternion.identity;
                pair.Value.transform.localScale = rest.Item2;
                if (!pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                {
                    pair.Value.enabled = false;
                    continue;
                }
                if (VoAvatarMode == "full") pair.Value.enabled = pair.Key == prefix + "full";
                else if (VoAvatarMode == "base") pair.Value.enabled = pair.Key == prefix + "base";
                else if (pair.Key == prefix + "base") pair.Value.enabled = false;
                else if (pair.Key.StartsWith(prefix + "slot_", StringComparison.Ordinal))
                    pair.Value.enabled = false;
                else pair.Value.enabled = false;
            }
            ApplyVoRigPose();
            ApplyVoEquipmentComponents();
            ApplyVoAttachmentProfiles();
            // The reviewed key-pose sheets currently depict the Lv1 outfit only.
            // Higher tiers keep their selected paper-doll visible until tier-matched motion exists.
            var usesMotionFrame = VoAvatarUsesFrameMotion;
            if (usesMotionFrame)
            {
                foreach (var renderer in _voAvatarParts.Values) renderer.enabled = false;
                foreach (var renderer in _voEquipmentComponents.Values) renderer.enabled = false;
            }
            if (_voMotionRenderer != null) _voMotionRenderer.enabled = usesMotionFrame;
        }

        private void SetLegacyVoRenderersVisible(bool visible)
        {
            if (_voAvatarRoot == null) return;
            foreach (var renderer in _voAvatarRoot.GetComponentsInChildren<SpriteRenderer>(true))
                renderer.forceRenderingOff = !visible;
        }

        private void ApplyVoEquipmentComponents()
        {
            foreach (var pair in _voEquipmentComponents)
            {
                var renderer = pair.Value;
                var rest = _voEquipmentComponentRest[pair.Key];
                renderer.transform.localPosition = rest.Item1;
                renderer.transform.localRotation = Quaternion.identity;
                renderer.transform.localScale = rest.Item2;
                var info = _voEquipmentComponentInfo[pair.Key];
                var itemLevel = _voEquipmentLevels.TryGetValue(info.slot, out var selectedLevel)
                    ? selectedLevel : VoAvatarLevel;
                renderer.enabled = VoAvatarMode == "modular" && info.gender == VoAvatarGender
                    && info.level == itemLevel
                    && _voState.IsEquipped(info.slot);
            }
        }

        private void ApplyVoRigPose()
        {
            _voRig.ResetPose();
            foreach (var pair in _voRigParts)
            {
                pair.Value.enabled = VoAvatarMode == "modular"
                    && pair.Key.StartsWith(VoAvatarGender + "_", StringComparison.Ordinal);
                if (!pair.Value.enabled) continue;
                var part = pair.Key.Substring(VoAvatarGender.Length + 1);
                var key = VoAvatarGender + "_" + VoAvatarMotionState + "_" + part;
                if (!_voRigPoseProfiles.TryGetValue(key, out var profile)) continue;
                _voRig.SetLocalRotation(pair.Key, profile.rotation);
            }
        }

        private void ApplyVoAttachmentProfiles()
        {
            var pose = VoAvatarMotionState;
            foreach (var slot in VoEquipmentSlots)
            {
                if (slot == "arm_guard" || slot == "boots") continue;
                var partId = "lv" + VoAvatarLevel.ToString("000") + "_" + VoAvatarGender + "_slot_" + slot;
                if (!_voAvatarParts.TryGetValue(partId, out var renderer)) continue;
                var key = VoAvatarGender + "_" + pose + "_" + slot;
                if (!_voAttachmentProfiles.TryGetValue(key, out var profile)) continue;
                var rest = _voAvatarPartRest[partId];
                renderer.transform.localPosition = rest.Item1 + new Vector3(profile.dx, profile.dy, 0);
                renderer.transform.localRotation = Quaternion.Euler(0, 0, profile.rotation);
                renderer.transform.localScale = new Vector3(rest.Item2.x * profile.scaleX,
                    rest.Item2.y * profile.scaleY, rest.Item2.z);
            }
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
            if (!_voState.TryStartAction("skill", .42f)) return false;
            _voPendingHit = true;
            VoSkillCastCount++;
            ApplyVoPose();
            return true;
        }

        public void SetVoRun(bool enabled)
        {
            _voState.SetRun(enabled);
        }

        public void SetVoJumpHeld(bool held)
        {
            _voJumpHeld = held;
            if (held && !_voState.HasActiveAction) TriggerVoJump();
        }

        public bool TriggerVoJump()
        {
            if (DialogueOpen || !_voState.TryStartAction("jump", _registeredOutfit == null ? .55f : .72f)) return false;
            VoJumpStartCount++;
            ApplyVoPose();
            return true;
        }

        public bool TriggerVoBasicAttack()
        {
            if (_voState.HasActiveAction || DialogueOpen
                || _voCombatTarget == null || Mathf.Abs(_voCombatTarget.transform.position.x - PlayerX) > 2.4f
                || VoTrainingTargetHp <= 0) return false;
            if (!_voState.TryStartAction("basic_attack", .30f)) return false;
            _voBasicPendingHit = true;
            ApplyVoPose();
            return true;
        }

        public void AdvanceVoAnimation(float seconds)
        {
            var activeAction = VoAvatarMotionState;
            _voState.Advance(seconds);
            _registeredOutfit?.Advance(seconds);
            _sourcePoseReview?.Advance(seconds);
            _femaleSourcePoseReview?.Advance(seconds);
            if (activeAction == "jump" && !_voState.HasActiveAction && _voJumpHeld) TriggerVoJump();
            if (activeAction == "skill")
            {
                if (_voPendingHit && _voState.ActionRemaining <= .28f)
                {
                    _voPendingHit = false;
                    VoTrainingTargetHp = Mathf.Max(0, VoTrainingTargetHp - 35);
                    VoSkillHitCount++;
                    LastInteractionMessage = "Liên Quyền trúng mục tiêu · -35 HP · còn " + VoTrainingTargetHp;
                    if (_voCombatTarget != null)
                        _voCombatTarget.color = VoTrainingTargetHp == 0 ? new Color(.28f, .28f, .28f, .75f) : new Color(1f, .42f, .32f, 1f);
                    if (VoTrainingTargetHp == 0 && ActiveQuestId == "Q06" && HasAcceptedCombatQuest)
                    {
                        HasDefeatedFirstEnemy = true;
                        CompleteQuest("Q06", "Q07");
                        LastInteractionMessage = "Q06 hoàn tất · quái non đã bị hạ; hãy nhặt chiến lợi phẩm.";
                    }
                }
            }
            else if (activeAction == "basic_attack")
            {
                if (_voBasicPendingHit && _voState.ActionRemaining <= .18f)
                {
                    _voBasicPendingHit = false;
                    VoTrainingTargetHp = Mathf.Max(0, VoTrainingTargetHp - 12);
                    LastInteractionMessage = "Đòn đánh thường trúng mục tiêu · -12 HP · còn " + VoTrainingTargetHp;
                    if (VoTrainingTargetHp == 0 && ActiveQuestId == "Q06" && HasAcceptedCombatQuest)
                    {
                        HasDefeatedFirstEnemy = true;
                        CompleteQuest("Q06", "Q07");
                        LastInteractionMessage = "Q06 hoàn tất · quái non đã bị hạ; hãy nhặt chiến lợi phẩm.";
                    }
                }
            }
            ApplyVoPose();
        }

        private void ApplyVoPose()
        {
            if (_voAvatarRoot == null) return;
            _voPoseYOffset = 0;
            _voAvatarRoot.localScale = Vector3.one;
            _voAvatarRoot.localRotation = Quaternion.identity;
            if (VoAvatarMotionState == "walk")
            {
                var stride = Mathf.Sin(_voState.AnimationPhase * 22f);
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (stride >= 0 ? "walk_a" : "walk_b"));
                else
                {
                    VoAvatarMotionFrameId = "lv" + VoAvatarLevel + "_aligned_paper_doll_walk";
                    _voAvatarRoot.localRotation = Quaternion.Euler(0, 0, stride * 1.4f);
                    _voAvatarRoot.localScale = new Vector3(1f + Mathf.Abs(stride) * .008f, 1f - Mathf.Abs(stride) * .006f, 1f);
                }
            }
            else if (VoAvatarMotionState == "run")
            {
                var stride = Mathf.Sin(_voState.AnimationPhase * 28f);
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (stride >= 0 ? "run_a" : "run_b"));
                else
                {
                    VoAvatarMotionFrameId = "lv" + VoAvatarLevel + "_aligned_paper_doll_run";
                    _voAvatarRoot.localRotation = Quaternion.Euler(0, 0, -3f + stride * 1.8f);
                    _voAvatarRoot.localScale = new Vector3(1.025f, .985f, 1f);
                }
            }
            else if (VoAvatarMotionState == "jump")
            {
                var progress = _voState.ActionProgress;
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (progress < .34f ? "jump_rise" : "jump_apex"));
                else VoAvatarMotionFrameId = "lv" + VoAvatarLevel + "_aligned_paper_doll_jump";
                _voPoseYOffset = Mathf.Sin(progress * Mathf.PI) * (_registeredOutfit == null ? .72f : 1.25f);
            }
            else if (VoAvatarMotionState == "basic_attack")
            {
                var progress = _voState.ActionProgress;
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (progress < .35f ? "basic_windup" : "basic_impact"));
                else VoAvatarMotionFrameId = "lv" + VoAvatarLevel + "_aligned_paper_doll_basic";
            }
            else if (VoAvatarMotionState == "skill")
            {
                var progress = _voState.ActionProgress;
                if (VoAvatarUsesFrameMotion) SetVoMotionFrame(VoAvatarGender + "_" + (progress < .55f ? "lien_quyen_hit_a" : "lien_quyen_finish_b"));
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
                _voAvatarRoot.localScale = new Vector3(1f, 1f + Mathf.Sin(_voState.AnimationPhase * 2.6f) * .005f, 1f);
            }
            if (_registeredOutfit != null)
            {
                _voAvatarRoot.localScale = Vector3.one;
                _voAvatarRoot.localRotation = Quaternion.identity;
            }
            _voAvatarRoot.localPosition = new Vector3(_routeX, GroundY + _voPoseYOffset, 0);
            RefreshVoAvatarMode();
            if (ActiveSourcePoseReview != null)
            {
                ActiveSourcePoseReview.transform.localPosition = new Vector3(_routeX, GroundY + _voPoseYOffset, 0);
                ActiveSourcePoseReview.Apply(VoAvatarMotionState, _voState.AnimationPhase, _sourcePoseFacing, _voState.ActionProgress);
            }
            if (_voSkillVfx != null)
            {
                _voSkillVfx.enabled = VoAvatarMotionState == "skill";
                if (_voSkillVfx.enabled)
                {
                    var progress = _voState.ActionProgress;
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
            var sourceParts = pack.parts.ToDictionary(part => part.id);
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
                var part = sourceParts[layer.part];
                var surfaceOffset = layer.id.StartsWith("terrain-")
                    ? part.walkSurfaceFromTop * layer.height / part.h : 0;
                host.transform.localPosition = new Vector3(layer.x, layer.y + surfaceOffset, 0);
                var displayWidth = layer.id.StartsWith("terrain-") ? layer.width + .12f : layer.width;
                host.transform.localScale = new Vector3(displayWidth / sprite.bounds.size.x, layer.height / sprite.bounds.size.y, 1);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = layer.order;
                if (layer.parallax != 0)
                    _parallax.Add(Tuple.Create(host.transform, host.transform.localPosition, layer.parallax));
            }
        }

        private void BuildInteractionMarkers()
        {
            for (var index = 0; index < RouteNodeIds.Length; index++)
            {
                var host = new GameObject("Map01A interaction " + RouteNodeIds[index]);
                host.transform.SetParent(transform, false);
                host.transform.localPosition = new Vector3(RouteNodeX[index], GroundY + 2.15f, 0);
                var text = host.AddComponent<TextMesh>();
                text.text = "!";
                text.anchor = TextAnchor.MiddleCenter;
                text.alignment = TextAlignment.Center;
                text.fontSize = _worldPresentationProfile.InteractionMarkerFontSize;
                text.characterSize = _worldPresentationProfile.InteractionMarkerCharacterSize;
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
            foreach (var part in pack.parts)
                _map01ANpcSprites.Add(part.id, MakeSprite(texture, new Rect(part.x, part.y, part.w, part.h)));
            foreach (var layer in pack.layers)
            {
                var sprite = _map01ANpcSprites[layer.part];
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
            if (_voAvatarRoot != null) _voAvatarRoot.localPosition = new Vector3(_routeX, GroundY + _voPoseYOffset, 0);
            if (_sourcePoseReview != null) _sourcePoseReview.transform.localPosition = new Vector3(_routeX, GroundY + _voPoseYOffset, 0);
            if (_femaleSourcePoseReview != null) _femaleSourcePoseReview.transform.localPosition = new Vector3(_routeX, GroundY + _voPoseYOffset, 0);
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
                _farCoverageScale = cover;
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
            foreach (var marker in _interactionMarkers)
                marker.Item1.SetActive(marker.Item1 == nearest);
            RefreshWorldPresentationMetrics();
        }

        private static bool TryEnabledSpriteBounds(Transform root, out Bounds bounds)
        {
            bounds = default;
            if (root == null) return false;
            var renderers = root.GetComponentsInChildren<SpriteRenderer>(true)
                .Where(renderer => renderer.enabled && renderer.sprite != null)
                .ToArray();
            if (renderers.Length == 0) return false;
            bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index++) bounds.Encapsulate(renderers[index].bounds);
            return bounds.size.y > 0f;
        }

        private void RefreshWorldPresentationMetrics()
        {
            if (_camera == null) return;
            var actorRoot = ActiveSourcePoseReview != null ? ActiveSourcePoseReview.transform : _voAvatarRoot;
            TryEnabledSpriteBounds(actorRoot, out var actorBounds);
            var npcRenderer = GetComponentsInChildren<SpriteRenderer>(true)
                .FirstOrDefault(renderer => renderer.name == "Map01A NPC quan-thu-dong-lam");
            var npcBounds = npcRenderer == null ? default : npcRenderer.bounds;
            var marker = _interactionMarkers
                .Select(item => item.Item1.GetComponent<TextMesh>())
                .FirstOrDefault(label => label != null && label.gameObject.activeSelf);
            WorldPresentationMetrics = new RuntimeWorldPresentationMetrics
            {
                profileName = _worldPresentationProfile.Name,
                cameraOrthographicSize = _camera.orthographicSize,
                cameraGroundOffsetY = _camera.transform.position.y - GroundY,
                actorWorldHeight = actorBounds.size.y,
                actorScreenHeightRatio = RuntimeWorldPresentationMetrics.ScreenHeightRatio(_camera, actorBounds),
                npcWorldHeight = npcBounds.size.y,
                npcScreenHeightRatio = RuntimeWorldPresentationMetrics.ScreenHeightRatio(_camera, npcBounds),
                backgroundCoverageScale = _farCoverageScale,
                interactionMarkerFontSize = marker == null ? 0 : marker.fontSize,
                interactionMarkerCharacterSize = marker == null ? 0f : marker.characterSize,
            };
        }

        private void LateUpdate()
        {
            if (Application.isPlaying && !_poseLoopCapturing && !_registeredCapturing) AdvanceVoAnimation(Mathf.Min(Time.deltaTime, .1f));
            Refresh();
        }
        private void OnDestroy()
        {
            if (_camera != null) { _camera.orthographicSize = _previousCameraSize; _camera.transform.position = _previousCameraPosition; }
            if (_controller != null) { _controller.enabled = _previousControllerEnabled; _controller.RefreshForSmoke(); }
            foreach (var pair in _hidden) if (pair.Key != null) pair.Key.enabled = pair.Value;
            if (_characterHubClassPortrait != null)
            {
                if (Application.isPlaying) Destroy(_characterHubClassPortrait);
                else DestroyImmediate(_characterHubClassPortrait);
            }
            foreach (var sprite in _sprites)
                if (sprite != null) { if (Application.isPlaying) Destroy(sprite); else DestroyImmediate(sprite); }
        }
    }
}
