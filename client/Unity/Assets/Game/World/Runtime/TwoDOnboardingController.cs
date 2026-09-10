using System.Collections.Generic;
using LinhGioi.Art;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LinhGioi.World
{
    public sealed class TwoDOnboardingController : MonoBehaviour
    {
        private const float MoveSpeed = 2.75f;
        private static Sprite _solidSprite;
        private static readonly Dictionary<string, Sprite> _shapeSprites = new Dictionary<string, Sprite>();
        private readonly TwoDOnboardingState _state = new TwoDOnboardingState();
        private readonly TwoDMapDesignCatalog _mapCatalog = TwoDMapDesignCatalog.CreateDefault();
        private readonly TwoDCharacterBaseCatalog _characterBaseCatalog = TwoDCharacterBaseCatalog.CreateDefault();
        private readonly TwoDCharacterModuleCatalog _moduleCatalog = TwoDCharacterModuleCatalog.CreateDefault();
        private readonly TwoDLocomotionAnimationProfile _animationProfile = TwoDLocomotionAnimationProfile.CreateDefault();
        private TwoDCharacterLoadout _playerLoadout;
        private Transform _player;
        private Transform _gateKeeper;
        private Transform _trainingStone;
        private Transform _shadowSlime;
        private Transform _shadowSlimeLabel;
        private Transform _voLv1SkillCueRoot;
        private Transform _voLv1PaperDollRoot;
        private Transform _voLv1AnchorGizmoRoot;
        private Transform _pathGlow;
        private Transform _linhThanhUnlockBanner;
        private Transform _plazaUnlockPath;
        private Transform _hubTransitionPreviewRoot;
        private Transform _plazaHubRuntimeRoot;
        private Transform _plazaHubSelectorRing;
        private Transform _linhThanhDistrictPreviewRing;
        private Transform _linhThanhDistrictPreviewBackplate;
        private Transform _inventoryPanelRoot;
        private Transform _focusRing;
        private SpriteRenderer _playerOuterShirtRenderer;
        private SpriteRenderer _playerWaistRenderer;
        private SpriteRenderer _playerGlovesLeftRenderer;
        private SpriteRenderer _playerGlovesRightRenderer;
        private Transform _playerHead;
        private Transform _playerLeftArm;
        private Transform _playerRightArm;
        private Transform _playerLeftLeg;
        private Transform _playerRightLeg;
        private Transform _hudDialoguePanel;
        private TextMesh _hudTitle;
        private TextMesh _hudArea;
        private TextMesh _hudObjective;
        private TextMesh _hudHint;
        private TextMesh _hudAction;
        private TextMesh _hudDialogue;
        private TextMesh _hudFeedback;
        private TextMesh _miniMapProgress;
        private TextMesh _plazaHubSelectedLabel;
        private TextMesh _linhThanhDistrictPreviewLabel;
        private TextMesh _inventoryModeLabel;
        private TextMesh _inventorySelectedLabel;
        private TextMesh _inventoryHelpLabel;
        private Camera _camera;
        private string _worldHudSnapshot = string.Empty;
        private string _runtimeAnimationSnapshot = string.Empty;
        private string _runtimeVoLv1PaperDollPoseId = "vo_idle";
        private Vector2 _lastPresentedPlayerPosition = TwoDOnboardingState.PlayerStart;
        private int _presentationTick;
        private readonly List<string> _productionSceneBeats = new List<string>();
        private readonly string[] _inventoryItemIds = { "top_vo_lv1_male", "top_kiem_lv1_male", "weapon_kiem_lv1_starter" };
        private bool _inventoryOpen;
        private int _inventorySelectedIndex = 1;
        private string _inventoryInputState = "Closed";
        private bool _runtimeReady;
        private int _dongMonUnityTilemapCellCount;

        public TwoDOnboardingState State => _state;
        public string WorldHudSnapshot => _worldHudSnapshot;
        public int WorldHudLineCount { get; private set; }
        public string ProductionSceneBeatSnapshot => string.Join("\n", _productionSceneBeats.ToArray());
        public int ProductionSceneBeatCount => _productionSceneBeats.Count;
        public string RuntimeMapSnapshot => _mapCatalog.RuntimeSnapshot;
        public string RuntimeZoneNetworkSnapshot => _mapCatalog.ZoneNetworkSnapshot;
        public string RuntimeLinhThanhHubShellSnapshot => _mapCatalog.LinhThanhHubShellSnapshot;
        public string RuntimeLinhThanhPlazaShellSnapshot => _mapCatalog.LinhThanhPlazaShellSnapshot;
        public string RuntimeLinhThanhAcademyShellSnapshot => _mapCatalog.LinhThanhAcademyShellSnapshot;
        public string RuntimeLinhThanhMarketShellSnapshot => _mapCatalog.LinhThanhMarketShellSnapshot;
        public string RuntimeLinhThanhSpiritTempleShellSnapshot => _mapCatalog.LinhThanhSpiritTempleShellSnapshot;
        public string RuntimeLinhThanhResidentialShellSnapshot => _mapCatalog.LinhThanhResidentialShellSnapshot;
        public string RuntimeLinhThanhForgeShellSnapshot => _mapCatalog.LinhThanhForgeShellSnapshot;
        public string RuntimeLinhThanhGuildShellSnapshot => _mapCatalog.LinhThanhGuildShellSnapshot;
        public string RuntimeLinhThanhHarborShellSnapshot => _mapCatalog.LinhThanhHarborShellSnapshot;
        public string RuntimeLinhThanhPlazaHubSnapshot => BuildLinhThanhPlazaHubSnapshot();
        public string RuntimeLinhThanhDistrictPreviewSnapshot => BuildLinhThanhDistrictPreviewSnapshot();
        public string RuntimeLinhThanhDistrictDetailSnapshot => BuildLinhThanhDistrictDetailSnapshot();
        public string RuntimeLinhThanhDistrictReadabilitySnapshot => BuildLinhThanhDistrictReadabilitySnapshot();
        public string RuntimePlazaHubInputSnapshot => BuildPlazaHubInputSnapshot();
        public string RuntimePlazaHubDetailSnapshot => BuildPlazaHubDetailSnapshot();
        public string RuntimePlazaHubLayoutSnapshot => BuildPlazaHubLayoutSnapshot();
        public string RuntimePlazaReadabilitySnapshot => BuildPlazaReadabilitySnapshot();
        public string RuntimeDongMonReadabilitySnapshot => BuildDongMonReadabilitySnapshot();
        public string RuntimeMinimapReadabilitySnapshot => BuildMinimapReadabilitySnapshot();
        public string RuntimeHubTransitionSnapshot => BuildHubTransitionSnapshot();
        public string RuntimeLinhThanhUnlockSnapshot => BuildLinhThanhUnlockSnapshot();
        public string RuntimeCharacterBaseSnapshot => _characterBaseCatalog.Snapshot;
        public string RuntimeEquipmentSnapshot => _moduleCatalog.Snapshot + "\n" + EnsurePlayerLoadout().Snapshot;
        public string RuntimeVoLv1ClassSliceSnapshot => BuildVoLv1ClassSliceRuntimeSnapshot();
        public string RuntimeVoLv1PaperDollAtlasSnapshot => BuildVoLv1PaperDollAtlasRuntimeSnapshot();
        public string RuntimeVoLv1AnchorGizmoSnapshot => BuildVoLv1AnchorGizmoSnapshot();
        public string RuntimeVoLv1RuntimeFitSnapshot => BuildVoLv1RuntimeFitSnapshot();
        public string RuntimeInventoryTryOnSnapshot => BuildInventoryTryOnSnapshot();
        public string RuntimeInventoryInputSnapshot => BuildInventoryInputSnapshot();
        public bool RuntimeInventoryPanelVisible => _inventoryPanelRoot != null && _inventoryPanelRoot.gameObject.activeSelf;
        public string RuntimeTerrainCollisionSnapshot => _mapCatalog.CollisionSnapshot;
        public string RuntimeTilemapSnapshot => _mapCatalog.TilemapSnapshot;
        public string RuntimeDongMonUnityTilemapSnapshot => BuildDongMonUnityTilemapSnapshot();
        public string RuntimeDongMonTilePaletteSnapshot => _mapCatalog.DongMonTilePaletteSnapshot;
        public string RuntimeDongMonTilePaletteSourceSnapshot => TwoDMapDesignCatalog.LoadDongMonTilePaletteSourceSnapshot();
        public string RuntimeDongMonChunkPlacementSourceSnapshot => TwoDMapDesignCatalog.LoadDongMonChunkPlacementSourceSnapshot();
        public string RuntimeDongMonAuthoredDetailSourceSnapshot => TwoDMapDesignCatalog.LoadDongMonAuthoredDetailsSourceSnapshot();
        public string RuntimeDongMonNpcSpriteSourceSnapshot => TwoDMapDesignCatalog.LoadDongMonNpcSpriteSourceSnapshot();
        public string RuntimeDongMonInteractionMarkerSourceSnapshot => TwoDMapDesignCatalog.LoadDongMonInteractionMarkerSourceSnapshot();
        public string RuntimeDongMonPlayerSceneFitSnapshot => BuildDongMonPlayerSceneFitSnapshot();
        public string RuntimeDongMonAuthoredPassSnapshot => _mapCatalog.DongMonAuthoredPassSnapshot;
        public string RuntimeAnimationSnapshot => _animationProfile.Snapshot + "\n" + _runtimeAnimationSnapshot;
        public string RuntimeCombatSnapshot => "CombatMicroSlice: ShadowSlimeVisible=" + _state.ShadowSlimeVisible + " ShadowSlimeDefeated=" + _state.ShadowSlimeDefeated + " step=" + _state.Step;
        public string RuntimeRouteProgressSnapshot => "RouteProgress: current=" + _state.CurrentRouteNodeId + " step=" + _state.Step + " action=" + _state.AvailableAction;

        public static TwoDOnboardingController Attach(GameObject host)
        {
            var controller = host.GetComponent<TwoDOnboardingController>() ?? host.AddComponent<TwoDOnboardingController>();
            controller.EnsureRuntimeReady();
            return controller;
        }

        private void Awake()
        {
            EnsureRuntimeReady();
        }

        private void EnsureRuntimeReady()
        {
            if (_runtimeReady) return;
            _runtimeReady = true;
            EnsurePlayerLoadout();
            BuildScene();
            _state.Reset();
            RefreshPresentation();
        }

        private void Update()
        {
            var movement = ReadMovement();
            if (movement.sqrMagnitude > 0f)
            {
                _state.Move(movement.normalized * MoveSpeed * Time.deltaTime);
                RefreshPresentation();
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            {
                if (_state.TryUseAction() || UseSelectedPlazaHubTarget()) RefreshPresentation();
            }
            if (Input.GetKeyDown(KeyCode.R)) PreviewEastGateToPlazaTransition();
            if (Input.GetKeyDown(KeyCode.P)) SelectNextPlazaHubTarget();
            if (Input.GetKeyDown(KeyCode.M)) SelectNextLinhThanhDistrictPreview();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J))
            {
                if (_state.TryUseJump()) RefreshPresentation();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.K))
            {
                if (_state.TryUseDash()) RefreshPresentation();
            }
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.L))
            {
                if (_state.TryUseClassSkill()) RefreshPresentation();
            }
            if (Input.GetKeyDown(KeyCode.I)) ToggleInventoryPanel();
            if (_inventoryOpen && Input.GetKeyDown(KeyCode.Tab)) SelectNextInventoryItem();
            if (_inventoryOpen && Input.GetKeyDown(KeyCode.T)) PreviewSelectedInventoryItem();
            if (_inventoryOpen && Input.GetKeyDown(KeyCode.Y)) ApplyInventoryPreview();
            if (_inventoryOpen && Input.GetKeyDown(KeyCode.Escape)) CancelInventoryPreview();
        }

        private void BuildScene()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                var cameraHost = new GameObject("LGO 2D Camera");
                _camera = cameraHost.AddComponent<Camera>();
                cameraHost.tag = "MainCamera";
            }
            _camera.orthographic = true;
            _camera.orthographicSize = 3.1f;
            _camera.transform.position = new Vector3(0f, 0f, -10f);
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = RuntimeArtCatalog.Background;

            _productionSceneBeats.Clear();
            AddSceneSprite("LGO 2D Far Spirit Sky", "Nền trời linh khí xanh đêm", new Vector2(0f, 0.35f), new Vector2(9.5f, 5.45f), new Color(0.05f, 0.12f, 0.22f), -20);
            AddSceneSprite("LGO 2D Moon Glow", "Vầng linh nguyệt sau Linh Thành", new Vector2(2.95f, 1.55f), new Vector2(0.72f, 0.72f), new Color(0.42f, 0.74f, 0.82f, 0.28f), -19);
            AddSceneSprite("LGO 2D Distant Wall", "Tường thành xa tạo chiều sâu", new Vector2(-1.65f, 0.45f), new Vector2(3.8f, 1.45f), new Color(0.08f, 0.19f, 0.30f), -18);
            AddSceneSprite("LGO 2D Distant Roofline", "Mái thành nhiều lớp phía sau", new Vector2(-1.65f, 1.2f), new Vector2(4.15f, 0.18f), new Color(0.13f, 0.31f, 0.42f), -17);
            AddDongMonParallaxPolish();
            AddDongMonLandmarkSilhouettes();
            AddLinhThanhHubShellOverlay();
            AddLinhThanhPlazaShellPreview();
            AddLinhThanhAcademyShellPreview();
            AddLinhThanhMarketShellPreview();
            AddLinhThanhSpiritTempleShellPreview();
            AddLinhThanhResidentialShellPreview();
            AddLinhThanhForgeShellPreview();
            AddLinhThanhGuildShellPreview();
            AddLinhThanhHarborShellPreview();
            AddLinhThanhPlazaHubRuntimePreview();
            AddLinhThanhHubTransitionPreview();
            AddLinhThanhUnlockPresentation();

            AddSceneSprite("LGO 2D Linh Thanh Gate Left Pillar", "Cổng Linh Thành - trụ trái", new Vector2(-2.6f, 0.18f), new Vector2(0.34f, 1.65f), new Color(0.11f, 0.27f, 0.38f), -16);
            AddSceneSprite("LGO 2D Linh Thanh Gate Right Pillar", "Cổng Linh Thành - trụ phải", new Vector2(-0.75f, 0.18f), new Vector2(0.34f, 1.65f), new Color(0.11f, 0.27f, 0.38f), -16);
            AddSceneSprite("LGO 2D Gate Roof", "Cổng Linh Thành - mái chính", new Vector2(-1.68f, 1.1f), new Vector2(2.25f, 0.26f), new Color(0.18f, 0.38f, 0.48f), -15);
            AddSceneSprite("LGO 2D Gate Gold Trim", "Cổng Linh Thành - viền vàng môn phái", new Vector2(-1.68f, 1.25f), new Vector2(2.55f, 0.08f), RuntimeArtCatalog.Gold, -14);
            AddSceneSprite("LGO 2D Gate Inner Glow", "Cổng Linh Thành - cửa linh quang", new Vector2(-1.68f, -0.2f), new Vector2(1.42f, 1.05f), new Color(0.07f, 0.14f, 0.23f), -14);
            AddSceneSprite("LGO 2D Gate Jade Seal", "Ấn ngọc trên cổng", new Vector2(-1.68f, 0.62f), new Vector2(0.28f, 0.28f), new Color(0.14f, 0.84f, 0.76f, 0.85f), -13);
            AddWorldLabel("LGO 2D Gate Label", "LINH THÀNH", new Vector2(-2.2f, 1.37f), 0.055f, RuntimeArtCatalog.Gold, -12);
            AddSceneBeat("Biển tên Cổng Linh Thành");

            AddSceneSprite("LGO 2D Training Yard", "Sân luyện nhập môn", new Vector2(0f, -1.52f), new Vector2(8.9f, 1.82f), new Color(0.10f, 0.15f, 0.18f), -12);
            AddSceneSprite("LGO 2D Yard Front Shade", "Bóng nền sân luyện", new Vector2(0f, -2.22f), new Vector2(8.9f, 0.46f), new Color(0.06f, 0.10f, 0.15f), -11);
            AddDongMonUnityTilemapLayer();
            AddDongMonProceduralTilemap();
            AddDongMonAuthoredDetailPass();
            AddDongMonInteractionMarkers();
            AddSceneSprite("LGO 2D Jade Path", "Lối ngọc dẫn tới Bia Luyện Khí", new Vector2(0.8f, -0.95f), new Vector2(5.9f, 0.20f), new Color(0.11f, 0.52f, 0.48f, 0.55f), -10);
            _pathGlow = AddSceneSprite("LGO 2D Path Glow", "Lối ngọc phát sáng sau thoại", new Vector2(0.85f, -0.95f), new Vector2(5.7f, 0.08f), new Color(0.16f, 0.86f, 0.78f, 0.78f), -9).transform;
            AddDongMonTerrainCollisionCues();
            for (var i = 0; i < 7; i++)
            {
                var x = -2.05f + i * 0.72f;
                AddSceneSprite("LGO 2D Yard Tile " + i, "Đường kẻ gạch sân luyện", new Vector2(x, -1.55f), new Vector2(0.035f, 1.24f), new Color(0.19f, 0.27f, 0.30f, 0.52f), -8);
            }
            AddLantern("LGO 2D Left Lantern", "Lồng đèn trái trước cổng", new Vector2(-3.35f, 0.52f), -7);
            AddLantern("LGO 2D Right Lantern", "Lồng đèn phải gần bia", new Vector2(2.75f, 0.22f), -7);
            AddSceneSprite("LGO 2D Spirit Mote A", "Đốm linh khí dẫn đường", new Vector2(-0.2f, -0.75f), new Vector2(0.10f, 0.10f), RuntimeArtCatalog.Spirit, -6);
            AddSceneSprite("LGO 2D Spirit Mote B", "Đốm linh khí quanh bia", new Vector2(2.95f, -0.45f), new Vector2(0.08f, 0.08f), RuntimeArtCatalog.Spirit, -6);

            AddSceneBeat("Người Giữ Cổng - NPC dẫn nhập");
            AddSceneBeat(RuntimeDongMonNpcSpriteSourceSnapshot);
            _gateKeeper = AddNpcSpriteFromResource("LGO 2D Gate Keeper", "gate_keeper", TwoDOnboardingState.GateKeeperPosition, RuntimeArtCatalog.Gold, new Color(0.88f, 0.78f, 0.58f), new Color(0.12f, 0.09f, 0.07f), -2);
            AddSceneBeat("Bia Luyện Khí - mục tiêu tương tác");
            _trainingStone = AddTrainingStone("LGO 2D Training Stone", TwoDOnboardingState.TrainingStonePosition, -2);
            AddWorldLabel("LGO 2D Stone Label", "BIA LUYỆN KHÍ", TwoDOnboardingState.TrainingStonePosition + new Vector2(-0.64f, 0.78f), 0.038f, RuntimeArtCatalog.Spirit, 2);
            AddSceneBeat("Shadow Slime - mục tiêu combat nhập môn");
            _shadowSlime = AddShadowSlime("LGO 2D Shadow Slime", new Vector2(3.55f, -1.16f), -1);
            _shadowSlimeLabel = AddWorldLabel("LGO 2D Shadow Slime Label", "SHADOW SLIME", new Vector2(3.55f, -0.34f), 0.034f, new Color(0.82f, 0.60f, 1f), 3).transform;
            AddKiemLv1PreviewRack(new Vector2(3.35f, -2.02f), 4);
            _voLv1SkillCueRoot = AddVoLv1SkillCue("LGO 2D Vo Lv1 Skill Runtime Cue", new Vector2(2.98f, -1.18f), 6);
            BuildInventoryTryOnStrip();
            BuildRuntimeMapOverlay();
            AddSceneBeat("Nhân vật người chơi - tân thủ nhập thành");
            AddSceneBeat(RuntimeDongMonPlayerSceneFitSnapshot);
            _player = AddCharacter("LGO 2D Player", TwoDOnboardingState.PlayerStart, RuntimeArtCatalog.Text, RuntimeArtCatalog.Spirit, new Color(0.05f, 0.06f, 0.08f), 2);
            AddSceneBeat("VO_LV1_PAPER_DOLL_ATLAS runtime overlay parts anchors skill cues");
            AddSceneBeat(TwoDPaperDollAtlasCatalog.LoadVoLv1PaperDollAtlasSnapshot());
            _voLv1PaperDollRoot = AddVoLv1PaperDollOverlay(_player, 2);
            _voLv1AnchorGizmoRoot = AddVoLv1AnchorGizmo(_player, 14);
            CachePlayerEquipmentRenderers();
            _focusRing = AddSceneSprite("LGO 2D Focus Ring", "Vòng chọn mục tiêu tương tác", TwoDOnboardingState.GateKeeperPosition + Vector2.down * 0.54f, new Vector2(1.45f, 0.16f), RuntimeArtCatalog.Spirit, 1).transform;
            BuildWorldHud();
        }

        private void CachePlayerEquipmentRenderers()
        {
            if (_player == null) return;
            _playerOuterShirtRenderer = FindChildRenderer(_player, "LayerSlot InnerShirt");
            _playerWaistRenderer = FindChildRenderer(_player, "Sash");
            _playerGlovesLeftRenderer = FindChildRenderer(_player, "Wrist Guard Left");
            _playerGlovesRightRenderer = FindChildRenderer(_player, "Wrist Guard Right");
            _playerHead = FindChildTransform(_player, "Head");
            _playerLeftArm = FindChildTransform(_player, "Left Arm");
            _playerRightArm = FindChildTransform(_player, "Right Arm");
            _playerLeftLeg = FindChildTransform(_player, "Left Leg");
            _playerRightLeg = FindChildTransform(_player, "Right Leg");
        }

        private void RefreshPlayerEquipmentPresentation()
        {
            var loadout = EnsurePlayerLoadout();
            if (_state.Step == TwoDOnboardingStep.Complete && _inventoryInputState != "Applied")
                loadout.ApplyVoLv1Starter(_moduleCatalog);
            var kiemPreviewOrApplied = _inventoryInputState == "Trying" || _inventoryInputState == "Applied";
            var voApplied = _state.Step == TwoDOnboardingStep.Complete && !kiemPreviewOrApplied;
            if (_playerOuterShirtRenderer != null) _playerOuterShirtRenderer.color = kiemPreviewOrApplied ? new Color(0.06f, 0.23f, 0.34f) : voApplied ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Text;
            if (_playerWaistRenderer != null) _playerWaistRenderer.color = voApplied ? new Color(0.62f, 0.12f, 0.09f) : RuntimeArtCatalog.Spirit;
            if (_playerGlovesLeftRenderer != null) _playerGlovesLeftRenderer.color = voApplied ? new Color(0.08f, 0.07f, 0.06f) : RuntimeArtCatalog.Spirit;
            if (_playerGlovesRightRenderer != null) _playerGlovesRightRenderer.color = voApplied ? new Color(0.08f, 0.07f, 0.06f) : RuntimeArtCatalog.Spirit;
        }

        private void RefreshPlayerAnimationPresentation()
        {
            var delta = _state.PlayerPosition - _lastPresentedPlayerPosition;
            var moved = delta.sqrMagnitude > 0.0001f;
            _presentationTick++;
            var phase = (_presentationTick % 8) / 7f;
            var state = _state.Step == TwoDOnboardingStep.Complete ? "TrainingCompletePose" : _state.LastAnimationIntent == "Jump" ? "Jump" : _state.LastAnimationIntent == "Dash" ? "Dash" : _state.LastAnimationIntent == "ClassSkill" ? "ClassSkill" : moved ? "Walk" : "Idle";
            var bob = state == "Walk" ? Mathf.Sin(phase * Mathf.PI * 2f) * 0.035f : state == "Jump" ? 0.16f : state == "Dash" ? -0.025f : state == "ClassSkill" ? 0.075f : state == "TrainingCompletePose" ? 0.045f : 0f;
            var armSwing = state == "Walk" ? Mathf.Sin(phase * Mathf.PI * 2f) * 0.08f : state == "Dash" ? -0.10f : state == "ClassSkill" ? 0.18f : state == "TrainingCompletePose" ? 0.12f : 0f;
            var legSwing = state == "Walk" ? Mathf.Cos(phase * Mathf.PI * 2f) * 0.04f : state == "Jump" ? 0.08f : 0f;

            if (_player != null) _player.localScale = state == "TrainingCompletePose" ? new Vector3(1.04f, 1.04f, 1f) : state == "Dash" ? new Vector3(1.12f, 0.92f, 1f) : Vector3.one;
            if (_playerHead != null) _playerHead.localPosition = ToWorld(new Vector2(0f, 0.34f + bob), 0f);
            if (_playerLeftArm != null) _playerLeftArm.localPosition = ToWorld(new Vector2(-0.28f, -0.12f + armSwing), 0f);
            if (_playerRightArm != null) _playerRightArm.localPosition = ToWorld(new Vector2(0.28f, -0.12f - armSwing), 0f);
            if (_playerLeftLeg != null) _playerLeftLeg.localPosition = ToWorld(new Vector2(-0.11f, -0.54f + legSwing), 0f);
            if (_playerRightLeg != null) _playerRightLeg.localPosition = ToWorld(new Vector2(0.11f, -0.54f - legSwing), 0f);
            if (_voLv1SkillCueRoot != null) _voLv1SkillCueRoot.gameObject.SetActive(state == "ClassSkill" || state == "TrainingCompletePose");
            var sampledFrame = _animationProfile.Sample(state, phase);
            _runtimeVoLv1PaperDollPoseId = string.IsNullOrEmpty(sampledFrame.PoseId) ? ToVoLv1PaperDollPoseId(state) : sampledFrame.PoseId;
            ApplyVoLv1PaperDollPose(_voLv1PaperDollRoot, _runtimeVoLv1PaperDollPoseId, state == "ClassSkill" || state == "TrainingCompletePose");
            if (_voLv1PaperDollRoot != null) _voLv1PaperDollRoot.gameObject.SetActive(_state.Step >= TwoDOnboardingStep.LearnJump && _inventoryInputState != "Applied");
            RefreshVoLv1AnchorGizmoVisibility();

            _runtimeAnimationSnapshot = new TwoDAnimationRuntimeState(state, state == "TrainingCompletePose" ? "vo_lv1_training_complete" : state == "ClassSkill" ? "vo_lv1_first_skill" : state == "Dash" ? "dash_stretch" : state == "Jump" ? "jump_lift" : state == "Walk" ? "stride_bob" : "breathing_idle", phase, sampledFrame.FrameId, _runtimeVoLv1PaperDollPoseId, sampledFrame.Clip).Snapshot;
            _lastPresentedPlayerPosition = _state.PlayerPosition;
        }

        private static SpriteRenderer FindChildRenderer(Transform root, string namePart)
        {
            var renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
            for (var i = 0; i < renderers.Length; i++)
                if (renderers[i].gameObject.name.Contains(namePart)) return renderers[i];
            return null;
        }

        private static Transform FindChildTransform(Transform root, string namePart)
        {
            var children = root.GetComponentsInChildren<Transform>(true);
            for (var i = 0; i < children.Length; i++)
                if (children[i].gameObject.name.Contains(namePart)) return children[i];
            return null;
        }

        private TwoDCharacterLoadout EnsurePlayerLoadout()
        {
            if (_playerLoadout == null)
                _playerLoadout = TwoDCharacterLoadout.CreateStarter("male_base", _moduleCatalog);
            return _playerLoadout;
        }

        private string BuildLinhThanhPlazaHubSnapshot()
        {
            var interaction = _state.LinhThanhUnlocked ? _state.PlazaHubInteractionId : "locked-until-unlock";
            if (string.IsNullOrEmpty(interaction) || interaction == "locked") interaction = _state.LinhThanhUnlocked ? "hub-idle" : "locked-until-unlock";
            return _mapCatalog.LinhThanhPlazaHubRuntimeSnapshot + " | unlocked=" + _state.LinhThanhUnlocked + " | selected=" + _state.SelectedPlazaHubTargetId + " | interaction=" + interaction;
        }


        public bool SelectNextPlazaHubTarget()
        {
            var ok = _state.SelectNextPlazaHubTarget();
            if (ok) RefreshPresentation();
            return ok;
        }

        public bool SelectNextLinhThanhDistrictPreview()
        {
            var ok = _state.SelectNextLinhThanhDistrictPreview();
            if (ok) RefreshPresentation();
            return ok;
        }

        public bool UseSelectedPlazaHubTarget()
        {
            var ok = _state.TryUseSelectedPlazaHubTarget();
            if (ok) RefreshPresentation();
            return ok;
        }

        public bool PreviewEastGateToPlazaTransition()
        {
            var ok = _state.TryPreviewEastGateToPlazaTransition();
            if (ok) RefreshPresentation();
            return ok;
        }

        private string BuildLinhThanhDistrictPreviewSnapshot()
        {
            if (!_state.LinhThanhUnlocked)
                return "DistrictPreviewRail: locked-until-unlock | safe-local-no-backend";

            var selected = _state.SelectedLinhThanhDistrictId == "locked" ? "plaza" : _state.SelectedLinhThanhDistrictId;
            var label = _state.SelectedLinhThanhDistrictId == "locked" ? "Quảng Trường" : _state.SelectedLinhThanhDistrictLabel;
            var route = selected == "plaza" ? "east-gate->plaza" : "plaza->" + selected;
            var guard = "safe-no-district-backend";
            if (selected == "academy") guard += " | safe-no-skill-backend";
            else if (selected == "market") guard += " | safe-no-trade-backend | safe-no-economy-backend";
            else if (selected == "spirit-temple") guard += " | safe-no-buff-backend";
            else if (selected == "forge") guard += " | safe-no-crafting-backend";
            else if (selected == "guild") guard += " | safe-no-guild-backend";
            else if (selected == "harbor") guard += " | safe-no-travel-backend | safe-no-teleport-backend";

            return "DistrictPreviewRail: unlocked=True"
                + " | open=" + _state.LinhThanhDistrictPreviewOpen
                + " | selected=" + selected
                + " | label=" + label
                + " | route=" + route
                + " | rail=plaza,academy,market,spirit-temple,forge,guild,harbor"
                + " | controls=M select-district"
                + " | " + guard
                + " | safe-local-no-backend";
        }

        private string BuildLinhThanhDistrictReadabilitySnapshot()
        {
            if (!_state.LinhThanhUnlocked)
                return "DistrictRailReadability: locked-until-unlock | safe-local-no-backend";

            var selected = _state.SelectedLinhThanhDistrictId == "locked" ? "plaza" : _state.SelectedLinhThanhDistrictId;
            return "DistrictRailReadability: mode=selected-node-callout"
                + " | selected=" + selected
                + " | label-follows-selected=True"
                + " | backplate=follows-selected"
                + " | callout-size=readable"
                + " | avoids-hud-overlap"
                + " | safe-local-no-backend";
        }

        private string BuildLinhThanhDistrictDetailSnapshot()
        {
            if (!_state.LinhThanhUnlocked)
                return "DistrictDetail: locked-until-unlock | safe-local-no-backend";

            var selected = _state.SelectedLinhThanhDistrictId == "locked" ? "plaza" : _state.SelectedLinhThanhDistrictId;
            if (selected == "academy")
                return "DistrictDetail: selected=academy | role=skill-learning-preview | detail=class-trainer-locked | next=skill-hall-preview | safe-no-skill-backend | safe-no-district-backend | safe-local-no-backend";
            if (selected == "market")
                return "DistrictDetail: selected=market | role=starter-commerce-preview | detail=vendor-row-only | next=try-before-shop | safe-no-trade-backend | safe-no-economy-backend | safe-no-district-backend | safe-local-no-backend";
            if (selected == "spirit-temple")
                return "DistrictDetail: selected=spirit-temple | role=story-blessing-preview | detail=altar-local-only | next=quest-buff-gate | safe-no-buff-backend | safe-no-district-backend | safe-local-no-backend";
            if (selected == "forge")
                return "DistrictDetail: selected=forge | role=crafting-preview | detail=anvil-row-only | next=craft-board-locked | safe-no-crafting-backend | safe-no-district-backend | safe-local-no-backend";
            if (selected == "guild")
                return "DistrictDetail: selected=guild | role=social-guild-preview | detail=notice-board-locked | next=guild-hall-gate | safe-no-guild-backend | safe-no-district-backend | safe-local-no-backend";
            if (selected == "harbor")
                return "DistrictDetail: selected=harbor | role=travel-preview | detail=spirit-boat-locked | next=world-route-gate | safe-no-travel-backend | safe-no-teleport-backend | safe-no-district-backend | safe-local-no-backend";
            return "DistrictDetail: selected=plaza | role=social-spawn-preview | detail=event-board-and-npc-local | next=academy-market-rail | safe-no-district-backend | safe-local-no-backend";
        }

        private string BuildHubTransitionSnapshot()
        {
            return "HubTransition: unlocked=" + _state.LinhThanhUnlocked
                + " | open=" + _state.HubTransitionPreviewOpen
                + " | id=" + _state.HubTransitionPreviewId
                + " | from=east-gate"
                + " | to=plaza"
                + " | mode=local-route-preview"
                + " | controls=R preview route"
                + " | safe-local-no-teleport-backend";
        }

        private string BuildPlazaHubInputSnapshot()
        {
            return "PlazaHubInput: unlocked=" + _state.LinhThanhUnlocked
                + " | selected=" + _state.SelectedPlazaHubTargetId
                + " | label=" + _state.SelectedPlazaHubTargetLabel
                + " | layout=spaced-social-triangle"
                + " | controls=P select, E interact"
                + " | interaction=" + (_state.LinhThanhUnlocked ? _state.PlazaHubInteractionId : "locked-until-unlock")
                + " | safe-local-no-shop-backend";
        }




        private string BuildPlazaHubDetailLabel()
        {
            if (_state.SelectedPlazaHubTargetId == "gate-guide") return "Inspect: Người Giữ Cổng — route guide / local-only";
            if (_state.SelectedPlazaHubTargetId == "merchant-preview") return "Inspect: Thương Nhân — thử đồ trước, chưa mở shop";
            if (_state.SelectedPlazaHubTargetId == "event-board") return "Inspect: Bảng Sự Kiện — notice preview, chưa mở event";
            return "Inspect: Bang Hội — locked preview";
        }

        private string BuildPlazaHubDetailSnapshot()
        {
            if (!_state.LinhThanhUnlocked)
                return "PlazaHubDetail: locked-until-unlock | safe-local-no-backend";

            var selected = _state.SelectedPlazaHubTargetId;
            if (selected == "gate-guide")
                return "PlazaHubDetail: selected=gate-guide | role=route-guide | detail=east-gate-return-and-city-intro | safe-no-teleport-backend | safe-local-no-backend";
            if (selected == "merchant-preview")
                return "PlazaHubDetail: selected=merchant-preview | role=starter-gear-preview | detail=try-before-shop | safe-no-shop-backend | safe-local-no-backend";
            if (selected == "event-board")
                return "PlazaHubDetail: selected=event-board | role=community-event-preview | detail=notice-only | safe-no-event-backend | safe-local-no-backend";
            return "PlazaHubDetail: selected=guild-locked | role=future-social-system | detail=locked-guild-bulletin | safe-no-guild-backend | safe-local-no-backend";
        }

        private string BuildPlazaHubLayoutSnapshot()
        {
            return "PlazaHubLayout: anchors=5"
                + " | anchor=social-spawn@center"
                + " | anchor=event-board@upper-mid"
                + " | anchor=gate-guide@left"
                + " | anchor=merchant-preview@right"
                + " | anchor=guild-locked@far-right"
                + " | label-rail=bottom-safe-zone"
                + " | safe-local-no-backend";
        }

        private string BuildPlazaReadabilitySnapshot()
        {
            return "PlazaReadability: mode=label-rail"
                + " | world-label-density=reduced"
                + " | target-chips=event-board,gate-guide,merchant-preview"
                + " | rail=bottom-safe-zone"
                + " | selector-label=single-active-target"
                + " | avoids-hud-overlap"
                + " | safe-local-no-backend";
        }

        private string BuildMinimapReadabilitySnapshot()
        {
            return "MinimapReadability: mode=compact-district-route"
                + " | route-text=short"
                + " | district-chips=academy,market,spirit,forge,guild,harbor"
                + " | world-links=icon-only"
                + " | selected-node-progress"
                + " | avoids-hud-overlap"
                + " | safe-local-no-backend";
        }

        private string BuildDongMonReadabilitySnapshot()
        {
            return "DongMonReadability: mode=route-label-rail"
                + " | world-label-density=reduced"
                + " | chips=gate,stone,jump,dash,slime"
                + " | rail=bottom-safe-zone"
                + " | collision-labels=chip-only"
                + " | avoids-hud-overlap"
                + " | safe-local-no-backend";
        }

        private string BuildDongMonPlayerSceneFitSnapshot()
        {
            return "DongMonPlayerSceneFit: map=dong-mon"
                + " | actor=player"
                + " | groundBandY=-1.58..-0.78"
                + " | footAnchor=bottom-center"
                + " | contactShadow=LayeredCharacter Shadow Slot Shadow"
                + " | playerSortOrder=2"
                + " | groundSortOrder=-2"
                + " | markerSortOrder=5"
                + " | routeAnchors=gatekeeper,training-stone,jump,dash,shadow-slime"
                + " | safe-no-3d=True"
                + " | safe-no-source-image=True"
                + " | safe-runtime-player-evidence=True";
        }

        private Vector2 PlazaHubTargetPosition()
        {
            if (_state.SelectedPlazaHubTargetId == "gate-guide") return new Vector2(-0.98f, 0.50f);
            if (_state.SelectedPlazaHubTargetId == "merchant-preview") return new Vector2(0.54f, 0.49f);
            return new Vector2(-0.18f, 0.64f);
        }

        private Vector2 LinhThanhDistrictPreviewPosition()
        {
            var selected = _state.SelectedLinhThanhDistrictId;
            if (selected == "academy") return new Vector2(0.54f, 1.36f);
            if (selected == "market") return new Vector2(-2.92f, 1.02f);
            if (selected == "spirit-temple") return new Vector2(1.10f, 1.18f);
            if (selected == "forge") return new Vector2(-1.42f, 0.84f);
            if (selected == "guild") return new Vector2(1.58f, 0.92f);
            if (selected == "harbor") return new Vector2(-0.18f, -0.62f);
            return new Vector2(-0.36f, 1.48f);
        }

        private Vector2 LinhThanhDistrictPreviewLabelPosition()
        {
            var anchor = LinhThanhDistrictPreviewPosition();
            var yOffset = anchor.y < -0.20f ? 0.34f : -0.34f;
            return new Vector2(Mathf.Clamp(anchor.x, -3.05f, 2.65f), anchor.y + yOffset);
        }

        private string BuildLinhThanhUnlockSnapshot()
        {
            return "LinhThanhUnlock: unlocked=" + _state.LinhThanhUnlocked + " | unlock=plaza | source=shadow-slime-complete | route=return-gate->plaza | safe-local-no-teleport";
        }

        private Vector2 ReadMovement()
        {
            var x = 0f;
            var y = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;
            return new Vector2(x, y);
        }

        public void RefreshForSmoke()
        {
            RefreshPresentation();
        }

        private void RefreshPresentation()
        {
            if (_player != null) _player.position = ToWorld(_state.PlayerPosition, 1f);
            var focusedPosition = _state.AvailableAction == TwoDOnboardingAction.Train
                ? TwoDOnboardingState.TrainingStonePosition + Vector2.down * 0.48f
                : TwoDOnboardingState.GateKeeperPosition + Vector2.down * 0.54f;
            if (_focusRing != null)
            {
                _focusRing.position = ToWorld(focusedPosition, 0f);
                _focusRing.gameObject.SetActive(_state.AvailableAction != TwoDOnboardingAction.None);
            }
            if (_pathGlow != null) _pathGlow.gameObject.SetActive(_state.Step != TwoDOnboardingStep.FindGateKeeper);
            if (_trainingStone != null)
            {
                var pulse = _state.Step == TwoDOnboardingStep.Complete ? 1.18f : 1f;
                _trainingStone.localScale = new Vector3(pulse, pulse, 1f);
            }
            if (_shadowSlime != null)
            {
                var visible = _state.ShadowSlimeVisible;
                _shadowSlime.gameObject.SetActive(visible);
                if (visible)
                {
                    var pulse = 1f + Mathf.Sin(_presentationTick * 0.6f) * 0.06f;
                    _shadowSlime.localScale = new Vector3(pulse, pulse, 1f);
                }
            }
            if (_shadowSlimeLabel != null) _shadowSlimeLabel.gameObject.SetActive(_state.ShadowSlimeVisible);
            var linhThanhUnlocked = _state.LinhThanhUnlocked;
            if (_plazaUnlockPath != null) _plazaUnlockPath.gameObject.SetActive(linhThanhUnlocked);
            if (_linhThanhUnlockBanner != null) _linhThanhUnlockBanner.gameObject.SetActive(linhThanhUnlocked && _state.Step != TwoDOnboardingStep.Complete);
            if (_plazaHubRuntimeRoot != null) _plazaHubRuntimeRoot.gameObject.SetActive(linhThanhUnlocked);
            if (_hubTransitionPreviewRoot != null) _hubTransitionPreviewRoot.gameObject.SetActive(linhThanhUnlocked && _state.HubTransitionPreviewOpen);
            if (_linhThanhDistrictPreviewRing != null)
            {
                _linhThanhDistrictPreviewRing.gameObject.SetActive(linhThanhUnlocked && _state.LinhThanhDistrictPreviewOpen);
                _linhThanhDistrictPreviewRing.localPosition = ToWorld(LinhThanhDistrictPreviewPosition(), 0f);
            }
            var districtCalloutPosition = LinhThanhDistrictPreviewLabelPosition();
            if (_linhThanhDistrictPreviewBackplate != null)
            {
                _linhThanhDistrictPreviewBackplate.gameObject.SetActive(linhThanhUnlocked && _state.LinhThanhDistrictPreviewOpen);
                _linhThanhDistrictPreviewBackplate.localPosition = ToWorld(districtCalloutPosition, 0f);
            }
            if (_linhThanhDistrictPreviewLabel != null)
            {
                _linhThanhDistrictPreviewLabel.transform.localPosition = ToWorld(districtCalloutPosition, 0f);
            }
            if (_plazaHubSelectorRing != null)
            {
                _plazaHubSelectorRing.gameObject.SetActive(linhThanhUnlocked);
                _plazaHubSelectorRing.localPosition = ToWorld(PlazaHubTargetPosition(), 0f);
            }
            SetHudText(_plazaHubSelectedLabel, linhThanhUnlocked && _state.PlazaHubPreviewOpen ? BuildPlazaHubDetailLabel() : string.Empty);
            SetHudText(_linhThanhDistrictPreviewLabel, linhThanhUnlocked && _state.LinhThanhDistrictPreviewOpen ? "Map: " + _state.SelectedLinhThanhDistrictLabel + " / local-only" : string.Empty);
            SetHudText(_miniMapProgress, linhThanhUnlocked ? (_state.LinhThanhDistrictPreviewOpen ? "Node: plaza → " + _state.SelectedLinhThanhDistrictId : _state.HubTransitionPreviewOpen ? "Node: east-gate → plaza preview" : "Node: return-gate → plaza") : "Node: " + _state.CurrentRouteNodeId);
            RefreshPlayerEquipmentPresentation();
            RefreshInventoryPanelPresentation();
            RefreshPlayerAnimationPresentation();
            RefreshWorldHud();
        }




        private void AddLinhThanhHubShellOverlay()
        {
            AddSceneBeat("LINH_THANH_HUB_SHELL Đông Môn/Quảng Trường/Học Viện/Thương Phố staging");
            AddSprite("LGO 2D Hub District East Gate", new Vector2(-1.68f, 1.62f), new Vector2(0.42f, 0.08f), RuntimeArtCatalog.Gold, -13);
            AddSprite("LGO 2D Hub District Plaza", new Vector2(-0.36f, 1.48f), new Vector2(0.48f, 0.10f), new Color(0.22f, 0.50f, 0.58f, 0.46f), -18);
            AddSprite("LGO 2D Hub District Academy", new Vector2(0.52f, 1.36f), new Vector2(0.36f, 0.30f), new Color(0.18f, 0.40f, 0.62f, 0.38f), -18);
            AddSprite("LGO 2D Hub District Market", new Vector2(-2.92f, 1.02f), new Vector2(0.52f, 0.16f), new Color(0.58f, 0.36f, 0.18f, 0.36f), -18);
            AddWorldLabel("LGO 2D Hub Shell Label", "Hub: Đông Môn → Quảng Trường", new Vector2(-0.26f, 1.68f), 0.025f, new Color(0.73f, 0.87f, 0.88f, 0.82f), -10);
            _linhThanhDistrictPreviewRing = AddSprite("LGO 2D Linh Thanh District Preview Ring", new Vector2(-0.36f, 1.48f), new Vector2(0.62f, 0.10f), new Color(0.18f, 0.86f, 0.78f, 0.70f), 7).transform;
            _linhThanhDistrictPreviewBackplate = AddSprite("LGO 2D Linh Thanh District Preview Label Backplate", new Vector2(-1.10f, -1.02f), new Vector2(1.82f, 0.22f), new Color(0.02f, 0.08f, 0.12f, 0.88f), 7).transform;
            _linhThanhDistrictPreviewLabel = AddWorldLabel("LGO 2D Linh Thanh District Preview Label", string.Empty, new Vector2(-1.72f, -1.04f), 0.027f, RuntimeArtCatalog.Spirit, 8);
            _linhThanhDistrictPreviewRing.gameObject.SetActive(false);
            _linhThanhDistrictPreviewBackplate.gameObject.SetActive(false);
        }


        private void AddLinhThanhPlazaShellPreview()
        {
            AddSceneBeat("LINH_THANH_PLAZA_SHELL social-spawn/event-board/guild-bulletin-preview no backend");
            AddSprite("LGO 2D Plaza Social Spawn Preview", new Vector2(-0.18f, 1.02f), new Vector2(0.20f, 0.20f), new Color(0.18f, 0.86f, 0.78f, 0.38f), -12);
            AddSprite("LGO 2D Plaza Event Board Preview", new Vector2(0.32f, 0.86f), new Vector2(0.24f, 0.30f), new Color(0.56f, 0.34f, 0.16f, 0.42f), -12);
            AddSprite("LGO 2D Plaza Guild Bulletin Preview", new Vector2(0.70f, 0.88f), new Vector2(0.18f, 0.26f), new Color(0.30f, 0.22f, 0.62f, 0.38f), -12);
            AddWorldLabel("LGO 2D Plaza Shell Label", "Quảng Trường: social spawn", new Vector2(0.42f, 1.16f), 0.021f, new Color(0.73f, 0.87f, 0.88f, 0.76f), -9);
        }


        private void AddLinhThanhAcademyShellPreview()
        {
            AddSceneBeat("LINH_THANH_ACADEMY_SHELL skill-hall/class-trainer local-only no skill backend");
            AddSprite("LGO 2D Academy Hall Silhouette", new Vector2(0.54f, 1.24f), new Vector2(0.52f, 0.34f), new Color(0.12f, 0.28f, 0.46f, 0.48f), -16);
            AddSprite("LGO 2D Academy Roof", new Vector2(0.54f, 1.48f), new Vector2(0.62f, 0.10f), new Color(0.20f, 0.42f, 0.58f, 0.54f), -15);
            AddSprite("LGO 2D Academy Skill Board", new Vector2(0.40f, 1.05f), new Vector2(0.18f, 0.20f), new Color(0.18f, 0.86f, 0.78f, 0.32f), -14);
            AddSprite("LGO 2D Academy Trainer Locked", new Vector2(0.70f, 1.05f), new Vector2(0.10f, 0.24f), new Color(0.92f, 0.72f, 0.28f, 0.34f), -14);
            AddWorldLabel("LGO 2D Academy Shell Label", "Học Viện", new Vector2(0.34f, 1.70f), 0.020f, new Color(0.73f, 0.87f, 0.88f, 0.72f), -9);
        }


        private void AddLinhThanhMarketShellPreview()
        {
            AddSceneBeat("LINH_THANH_MARKET_SHELL vendor-row/auction-board local-only no trade backend");
            AddSprite("LGO 2D Market Awning Row", new Vector2(-2.92f, 0.86f), new Vector2(0.66f, 0.16f), new Color(0.58f, 0.36f, 0.18f, 0.46f), -15);
            AddSprite("LGO 2D Market Stall Left", new Vector2(-3.12f, 0.66f), new Vector2(0.20f, 0.24f), new Color(0.92f, 0.72f, 0.28f, 0.34f), -14);
            AddSprite("LGO 2D Market Stall Right", new Vector2(-2.74f, 0.66f), new Vector2(0.20f, 0.24f), new Color(0.18f, 0.86f, 0.78f, 0.26f), -14);
            AddSprite("LGO 2D Market Auction Board Locked", new Vector2(-2.92f, 0.48f), new Vector2(0.34f, 0.08f), new Color(0.30f, 0.22f, 0.62f, 0.32f), -13);
            AddWorldLabel("LGO 2D Market Shell Label", "Thương Phố", new Vector2(-3.26f, 1.08f), 0.019f, new Color(0.92f, 0.72f, 0.28f, 0.70f), -9);
        }


        private void AddLinhThanhSpiritTempleShellPreview()
        {
            AddSceneBeat("LINH_THANH_SPIRIT_TEMPLE_SHELL blessing-altar/story-shrine local-only no buff backend");
            AddSprite("LGO 2D Spirit Temple Shrine Base", new Vector2(1.10f, 1.10f), new Vector2(0.42f, 0.24f), new Color(0.22f, 0.18f, 0.38f, 0.42f), -16);
            AddSprite("LGO 2D Spirit Temple Roof", new Vector2(1.10f, 1.32f), new Vector2(0.52f, 0.09f), new Color(0.32f, 0.22f, 0.54f, 0.46f), -15);
            AddSprite("LGO 2D Spirit Temple Altar Preview", new Vector2(0.98f, 0.92f), new Vector2(0.16f, 0.12f), new Color(0.92f, 0.72f, 0.28f, 0.32f), -14);
            AddSprite("LGO 2D Spirit Temple Incense VFX", new Vector2(1.22f, 0.96f), new Vector2(0.06f, 0.30f), new Color(0.18f, 0.86f, 0.78f, 0.24f), -13);
            AddWorldLabel("LGO 2D Spirit Temple Shell Label", "Đền Linh", new Vector2(0.90f, 1.52f), 0.019f, new Color(0.73f, 0.87f, 0.88f, 0.70f), -9);
        }


        private void AddLinhThanhResidentialShellPreview()
        {
            AddSceneBeat("LINH_THANH_RESIDENTIAL_SHELL npc-home/social-chat local-only no housing backend");
            AddSprite("LGO 2D Residential Home Row", new Vector2(-0.84f, 1.02f), new Vector2(0.56f, 0.28f), new Color(0.12f, 0.24f, 0.30f, 0.42f), -16);
            AddSprite("LGO 2D Residential Roof A", new Vector2(-1.02f, 1.24f), new Vector2(0.28f, 0.08f), new Color(0.30f, 0.18f, 0.12f, 0.42f), -15);
            AddSprite("LGO 2D Residential Roof B", new Vector2(-0.66f, 1.22f), new Vector2(0.28f, 0.08f), new Color(0.30f, 0.18f, 0.12f, 0.36f), -15);
            AddSprite("LGO 2D Residential Citizen Ambient", new Vector2(-0.84f, 0.76f), new Vector2(0.08f, 0.18f), new Color(0.54f, 0.86f, 0.92f, 0.32f), -14);
            AddSprite("LGO 2D Residential Chat Node Locked", new Vector2(-0.70f, 0.90f), new Vector2(0.12f, 0.04f), new Color(0.18f, 0.86f, 0.78f, 0.26f), -13);
            AddWorldLabel("LGO 2D Residential Shell Label", "Khu Dân Cư", new Vector2(-1.18f, 1.42f), 0.018f, new Color(0.73f, 0.87f, 0.88f, 0.68f), -9);
        }


        private void AddLinhThanhForgeShellPreview()
        {
            AddSceneBeat("LINH_THANH_FORGE_SHELL anvil-row/craft-board local-only no crafting backend");
            AddSprite("LGO 2D Forge Workshop Silhouette", new Vector2(-1.42f, 0.82f), new Vector2(0.44f, 0.30f), new Color(0.16f, 0.12f, 0.10f, 0.48f), -16);
            AddSprite("LGO 2D Forge Roof", new Vector2(-1.42f, 1.05f), new Vector2(0.54f, 0.08f), new Color(0.38f, 0.21f, 0.10f, 0.48f), -15);
            AddSprite("LGO 2D Forge Anvil Preview", new Vector2(-1.54f, 0.62f), new Vector2(0.16f, 0.09f), new Color(0.64f, 0.66f, 0.64f, 0.38f), -14);
            AddSprite("LGO 2D Forge Glow Local", new Vector2(-1.30f, 0.62f), new Vector2(0.16f, 0.16f), new Color(0.92f, 0.38f, 0.16f, 0.28f), -13);
            AddSprite("LGO 2D Forge Craft Board Locked", new Vector2(-1.30f, 0.82f), new Vector2(0.14f, 0.06f), new Color(0.92f, 0.72f, 0.28f, 0.28f), -13);
            AddWorldLabel("LGO 2D Forge Shell Label", "Khu Rèn", new Vector2(-1.70f, 1.22f), 0.018f, new Color(0.92f, 0.72f, 0.28f, 0.68f), -9);
        }


        private void AddLinhThanhGuildShellPreview()
        {
            AddSceneBeat("LINH_THANH_GUILD_SHELL guild-hall/banner local-only no guild backend");
            AddSprite("LGO 2D Guild Hall Silhouette", new Vector2(1.58f, 0.90f), new Vector2(0.48f, 0.34f), new Color(0.10f, 0.18f, 0.34f, 0.46f), -16);
            AddSprite("LGO 2D Guild Hall Roof", new Vector2(1.58f, 1.16f), new Vector2(0.58f, 0.09f), new Color(0.24f, 0.28f, 0.58f, 0.44f), -15);
            AddSprite("LGO 2D Guild Banner Local", new Vector2(1.38f, 0.78f), new Vector2(0.08f, 0.26f), new Color(0.92f, 0.72f, 0.28f, 0.34f), -14);
            AddSprite("LGO 2D Guild Notice Board Locked", new Vector2(1.76f, 0.78f), new Vector2(0.18f, 0.08f), new Color(0.18f, 0.86f, 0.78f, 0.26f), -13);
            AddWorldLabel("LGO 2D Guild Shell Label", "Khu Bang Hội", new Vector2(1.26f, 1.38f), 0.018f, new Color(0.73f, 0.87f, 0.88f, 0.68f), -9);
        }


        private void AddLinhThanhHarborShellPreview()
        {
            AddSceneBeat("LINH_THANH_HARBOR_SHELL spirit-boat/travel-board local-only no travel backend");
            AddSprite("LGO 2D Harbor Dock Line", new Vector2(-0.18f, -0.74f), new Vector2(0.86f, 0.07f), new Color(0.38f, 0.21f, 0.10f, 0.42f), -12);
            AddSprite("LGO 2D Harbor Spirit Boat Hull", new Vector2(0.08f, -0.60f), new Vector2(0.42f, 0.10f), new Color(0.18f, 0.40f, 0.62f, 0.36f), -11);
            AddSprite("LGO 2D Harbor Spirit Boat Sail", new Vector2(0.18f, -0.42f), new Vector2(0.10f, 0.26f), new Color(0.18f, 0.86f, 0.78f, 0.26f), -10);
            AddSprite("LGO 2D Harbor Travel Board Locked", new Vector2(-0.42f, -0.54f), new Vector2(0.18f, 0.10f), new Color(0.92f, 0.72f, 0.28f, 0.26f), -10);
            AddSprite("LGO 2D Harbor Lantern Local", new Vector2(0.50f, -0.54f), new Vector2(0.06f, 0.18f), new Color(0.92f, 0.38f, 0.16f, 0.24f), -9);
            AddWorldLabel("LGO 2D Harbor Shell Label", "Cảng Linh Thuyền", new Vector2(-0.58f, -0.36f), 0.017f, new Color(0.73f, 0.87f, 0.88f, 0.66f), -8);
        }



        private void AddLinhThanhPlazaHubRuntimePreview()
        {
            AddSceneBeat("LINH_THANH_PLAZA_HUB_RUNTIME NPC/board local-only after Dong Mon unlock");
            AddSceneBeat("LINH_THANH_PLAZA_LAYOUT social-spawn/event-board/gate-guide/merchant/guild-locked anchors");
            AddSceneBeat("LINH_THANH_PLAZA_READABILITY label-rail reduced-density target chips");
            var root = new GameObject("LGO 2D Plaza Hub Runtime Root");
            root.transform.SetParent(transform, false);
            _plazaHubRuntimeRoot = root.transform;
            AddSprite("LGO 2D Plaza Social Spawn Anchor", new Vector2(-0.18f, 0.30f), new Vector2(0.54f, 0.055f), new Color(0.18f, 0.86f, 0.78f, 0.40f), -3, _plazaHubRuntimeRoot);
            AddWorldLabel("LGO 2D Plaza Social Spawn Label", "spawn", new Vector2(-0.18f, 0.24f), 0.014f, new Color(0.73f, 0.87f, 0.88f, 0.78f), 6, _plazaHubRuntimeRoot);
            AddSprite("LGO 2D Plaza Gate Guide NPC", new Vector2(-0.98f, 0.78f), new Vector2(0.12f, 0.34f), new Color(0.92f, 0.72f, 0.28f, 0.72f), -2, _plazaHubRuntimeRoot);
            AddWorldLabel("LGO 2D Plaza Gate Guide Chip", "01", new Vector2(-0.98f, 0.43f), 0.017f, RuntimeArtCatalog.Gold, 6, _plazaHubRuntimeRoot);
            AddSprite("LGO 2D Plaza Wandering Student NPC", new Vector2(-0.02f, 0.78f), new Vector2(0.10f, 0.28f), new Color(0.54f, 0.86f, 0.92f, 0.66f), -2, _plazaHubRuntimeRoot);
            AddSprite("LGO 2D Plaza Merchant Preview NPC", new Vector2(0.54f, 0.76f), new Vector2(0.13f, 0.30f), new Color(0.86f, 0.48f, 0.22f, 0.70f), -2, _plazaHubRuntimeRoot);
            AddSprite("LGO 2D Plaza Merchant Pack", new Vector2(0.68f, 0.56f), new Vector2(0.18f, 0.12f), RuntimeArtCatalog.Gold, -1, _plazaHubRuntimeRoot);
            AddWorldLabel("LGO 2D Plaza Merchant Preview Chip", "03", new Vector2(0.56f, 0.40f), 0.017f, RuntimeArtCatalog.Gold, 6, _plazaHubRuntimeRoot);
            AddSprite("LGO 2D Plaza Event Board Runtime", new Vector2(-0.18f, 0.88f), new Vector2(0.38f, 0.34f), new Color(0.58f, 0.34f, 0.16f, 0.70f), -2, _plazaHubRuntimeRoot);
            AddWorldLabel("LGO 2D Plaza Event Board Chip", "02", new Vector2(-0.18f, 0.52f), 0.017f, RuntimeArtCatalog.Spirit, 6, _plazaHubRuntimeRoot);
            AddSprite("LGO 2D Plaza Guild Bulletin Locked Runtime", new Vector2(0.92f, 0.68f), new Vector2(0.22f, 0.28f), new Color(0.30f, 0.22f, 0.62f, 0.62f), -2, _plazaHubRuntimeRoot);
            AddWorldLabel("LGO 2D Plaza Guild Locked Chip", "locked", new Vector2(0.94f, 0.36f), 0.014f, new Color(0.73f, 0.87f, 0.88f, 0.72f), 6, _plazaHubRuntimeRoot);
            AddWorldLabel("LGO 2D Plaza Hub Runtime Label", "Quảng Trường", new Vector2(0.02f, 0.19f), 0.020f, new Color(0.73f, 0.87f, 0.88f, 0.84f), 6, _plazaHubRuntimeRoot);
            _plazaHubSelectorRing = AddSprite("LGO 2D Plaza Target Selector Ring", new Vector2(-0.18f, 0.64f), new Vector2(0.46f, 0.08f), new Color(0.18f, 0.86f, 0.78f, 0.72f), 7, _plazaHubRuntimeRoot).transform;
            _plazaHubSelectedLabel = AddWorldLabel("LGO 2D Plaza Selected Target Label", string.Empty, new Vector2(-1.58f, -1.12f), 0.019f, RuntimeArtCatalog.Spirit, 7, _plazaHubRuntimeRoot);
            _plazaHubRuntimeRoot.gameObject.SetActive(false);
        }


        private void AddLinhThanhHubTransitionPreview()
        {
            AddSceneBeat("LINH_THANH_HUB_TRANSITION_PREVIEW east-gate->plaza local-route no teleport backend");
            var root = new GameObject("LGO 2D Hub Transition Preview Root");
            root.transform.SetParent(transform, false);
            _hubTransitionPreviewRoot = root.transform;
            AddSprite("LGO 2D Hub Transition East Gate Anchor", new Vector2(-1.46f, 0.18f), new Vector2(0.18f, 0.18f), RuntimeArtCatalog.Gold, 8, _hubTransitionPreviewRoot);
            AddSprite("LGO 2D Hub Transition Plaza Anchor", new Vector2(-0.18f, 0.18f), new Vector2(0.18f, 0.18f), RuntimeArtCatalog.Spirit, 8, _hubTransitionPreviewRoot);
            AddSprite("LGO 2D Hub Transition Route Beam", new Vector2(-0.82f, 0.18f), new Vector2(1.08f, 0.06f), new Color(0.18f, 0.86f, 0.78f, 0.74f), 7, _hubTransitionPreviewRoot);
            AddWorldLabel("LGO 2D Hub Transition Label", "Tuyến Đông Môn → Quảng Trường", new Vector2(-0.78f, -0.03f), 0.023f, RuntimeArtCatalog.Spirit, 9, _hubTransitionPreviewRoot);
            AddWorldLabel("LGO 2D Hub Transition Guard", "local route preview / no teleport backend", new Vector2(-0.78f, -0.22f), 0.016f, new Color(0.73f, 0.87f, 0.88f, 0.74f), 9, _hubTransitionPreviewRoot);
            _hubTransitionPreviewRoot.gameObject.SetActive(false);
        }

        private void AddLinhThanhUnlockPresentation()
        {
            AddSceneBeat("LINH_THANH_UNLOCK_PRESENTATION Đông Môn complete opens Plaza preview local-only");
            _plazaUnlockPath = AddSceneSprite("LGO 2D Plaza Unlock Path", "Đường sáng local preview từ Đông Môn về Quảng Trường", new Vector2(-0.42f, 0.12f), new Vector2(1.46f, 0.085f), new Color(0.92f, 0.72f, 0.28f, 0.78f), -3).transform;
            _linhThanhUnlockBanner = AddWorldLabel("LGO 2D Linh Thanh Unlock Banner", "MỞ LINH THÀNH → QUẢNG TRƯỜNG", new Vector2(-0.36f, 0.72f), 0.040f, RuntimeArtCatalog.Gold, 9).transform;
            _plazaUnlockPath.gameObject.SetActive(false);
            _linhThanhUnlockBanner.gameObject.SetActive(false);
        }

        private void AddDongMonParallaxPolish()
        {
            AddSceneBeat("PARALLAX_POLISH Đông Môn cloud-drift/mist/foreground grass-leaf-motes");
            AddSceneSprite("LGO 2D Parallax Cloud Drift A", "Mây linh khí trôi xa layer 5", new Vector2(-3.20f, 2.25f), new Vector2(1.24f, 0.16f), new Color(0.35f, 0.66f, 0.82f, 0.18f), -19);
            AddSceneSprite("LGO 2D Parallax Cloud Drift B", "Mây linh khí trôi xa layer 5", new Vector2(1.75f, 2.05f), new Vector2(1.62f, 0.13f), new Color(0.35f, 0.66f, 0.82f, 0.14f), -19);
            AddSceneSprite("LGO 2D Parallax Mountain Silhouette", "Dáng núi xa phía sau Đông Môn", new Vector2(1.78f, 1.02f), new Vector2(2.15f, 0.32f), new Color(0.05f, 0.16f, 0.25f, 0.64f), -18);
            AddSceneSprite("LGO 2D Parallax Mist Veil", "Sương mỏng tách lớp trung cảnh", new Vector2(0.45f, -0.18f), new Vector2(7.50f, 0.10f), new Color(0.48f, 0.88f, 0.92f, 0.12f), -6);
            AddSceneSprite("LGO 2D Foreground Grass Tuft A", "Cỏ foreground không che chân người chơi", new Vector2(-3.82f, -2.18f), new Vector2(0.18f, 0.20f), new Color(0.20f, 0.54f, 0.34f, 0.60f), 5);
            AddSceneSprite("LGO 2D Foreground Grass Tuft B", "Cỏ foreground không che combat", new Vector2(3.92f, -2.08f), new Vector2(0.22f, 0.18f), new Color(0.20f, 0.54f, 0.34f, 0.46f), 5);
        }

        private void AddDongMonLandmarkSilhouettes()
        {
            AddSceneBeat("Đông Môn landmark pass: Cầu Gỗ/Thác Nước/Sóng Linh/Rừng Ngoại Thành");
            AddSprite("LGO 2D Outer Forest Canopy A", new Vector2(2.95f, 0.88f), new Vector2(0.92f, 0.34f), new Color(0.06f, 0.25f, 0.20f, 0.78f), -15);
            AddSprite("LGO 2D Outer Forest Trunk A", new Vector2(2.78f, 0.50f), new Vector2(0.10f, 0.62f), new Color(0.13f, 0.10f, 0.07f, 0.82f), -14);
            AddSprite("LGO 2D Wood Bridge Deck", new Vector2(1.05f, -0.55f), new Vector2(1.34f, 0.08f), new Color(0.38f, 0.21f, 0.10f, 0.88f), -8);
            AddSprite("LGO 2D Wood Bridge Rail", new Vector2(1.05f, -0.36f), new Vector2(1.20f, 0.045f), new Color(0.50f, 0.29f, 0.13f, 0.86f), -7);
            AddSprite("LGO 2D Spirit Waterfall", new Vector2(3.18f, 0.08f), new Vector2(0.22f, 0.98f), new Color(0.18f, 0.75f, 0.88f, 0.36f), -13);
            AddSprite("LGO 2D Spirit Waterfall Foam", new Vector2(3.18f, -0.43f), new Vector2(0.56f, 0.10f), new Color(0.70f, 0.96f, 1f, 0.44f), -12);
            AddSprite("LGO 2D Song Linh Wave A", new Vector2(2.40f, -2.31f), new Vector2(0.74f, 0.045f), new Color(0.16f, 0.84f, 0.82f, 0.30f), 6);
            AddSprite("LGO 2D Song Linh Wave B", new Vector2(3.15f, -2.40f), new Vector2(0.64f, 0.035f), new Color(0.16f, 0.84f, 0.82f, 0.22f), 6);
        }



        private string BuildDongMonUnityTilemapSnapshot()
        {
            var sourceChunks = TwoDMapDesignCatalog.LoadDongMonAuthoredChunkPlacements();
            var sourceCells = 0;
            for (var i = 0; i < sourceChunks.Length; i++) sourceCells += sourceChunks[i].TileCount;
            var cells = _dongMonUnityTilemapCellCount > 0 ? _dongMonUnityTilemapCellCount : sourceCells;
            return "DongMonUnityTilemap: renderer=TilemapRenderer"
                + " | grid=Grid"
                + " | source=LGOMaps/DongMonChunkPlacement"
                + " | chunks=" + sourceChunks.Length
                + " | cells=" + cells
                + " | palette=LGOMaps/DongMonTilePalette"
                + " | layered-under-procedural-strip"
                + " | safe-no-source-image"
                + " | safe-no-3d";
        }

        private void AddDongMonUnityTilemapLayer()
        {
            AddSceneBeat("DONG_MON_UNITY_TILEMAP Grid+TilemapRenderer from authored placement resource");
            AddSceneBeat(RuntimeDongMonUnityTilemapSnapshot);

            var gridObject = new GameObject("LGO 2D Dong Mon Unity Tilemap Grid");
            gridObject.transform.SetParent(transform, false);
            gridObject.transform.localPosition = new Vector3(-3.70f, -2.235f, -0.095f);
            var grid = gridObject.AddComponent<Grid>();
            grid.cellSize = new Vector3(0.34f, 0.055f, 0f);

            var tilemapObject = new GameObject("LGO 2D Dong Mon Unity Tilemap");
            tilemapObject.transform.SetParent(gridObject.transform, false);
            var tilemap = tilemapObject.AddComponent<Tilemap>();
            var tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
            tilemapRenderer.sortingOrder = -8;
            tilemapRenderer.mode = TilemapRenderer.Mode.Chunk;

            var chunks = TwoDMapDesignCatalog.LoadDongMonAuthoredChunkPlacements();
            var cursor = 0;
            _dongMonUnityTilemapCellCount = 0;
            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                ResolveDongMonTileColors(chunks[chunkIndex].PrimaryTileId, out var bodyColor, out _);
                var tile = ScriptableObject.CreateInstance<Tile>();
                tile.name = "LGO 2D Unity Tile " + chunks[chunkIndex].PrimaryTileId;
                tile.sprite = SolidSprite();
                tile.color = new Color(bodyColor.r, bodyColor.g, bodyColor.b, Mathf.Min(bodyColor.a, 0.30f));
                for (var i = 0; i < chunks[chunkIndex].TileCount; i++)
                {
                    tilemap.SetTile(new Vector3Int(cursor + i, 0, 0), tile);
                    _dongMonUnityTilemapCellCount++;
                }
                cursor += chunks[chunkIndex].TileCount + 1;
            }
            tilemap.CompressBounds();
        }

        private void AddDongMonProceduralTilemap()
        {
            AddSceneBeat("TILEMAP Tile: grass/stone/wood/gap/dash/slime authored Đông Môn chunk placement spine");
            AddSceneBeat("LGO 2D Tile Chunk chunk_gate_entry -> chunk_training_stone -> chunk_jump_bridge -> chunk_dash_lane -> chunk_slime_arena");
            AddSceneBeat(RuntimeDongMonChunkPlacementSourceSnapshot);
            var chunks = TwoDMapDesignCatalog.LoadDongMonAuthoredChunkPlacements();
            for (var i = 0; i < chunks.Length; i++)
            {
                ResolveDongMonTileColors(chunks[i].PrimaryTileId, out var bodyColor, out var lipColor);
                AddDongMonProceduralTileChunk(chunks[i].Id, chunks[i].PrimaryTileId, new Vector2(chunks[i].OriginX, chunks[i].OriginY), chunks[i].TileCount, bodyColor, lipColor);
            }
            AddSprite("LGO 2D Tile tile_gap_marker", new Vector2(0.84f, -1.72f), new Vector2(0.84f, 0.06f), new Color(0.04f, 0.07f, 0.10f), -6);
        }

        private static void ResolveDongMonTileColors(string tileId, out Color bodyColor, out Color lipColor)
        {
            switch (tileId)
            {
                case "tile_ground_grass":
                    bodyColor = new Color(0.13f, 0.32f, 0.22f);
                    lipColor = new Color(0.25f, 0.64f, 0.42f);
                    break;
                case "tile_ground_stone":
                    bodyColor = new Color(0.24f, 0.30f, 0.31f);
                    lipColor = RuntimeArtCatalog.Spirit;
                    break;
                case "tile_platform_wood":
                    bodyColor = new Color(0.46f, 0.27f, 0.12f);
                    lipColor = new Color(0.58f, 0.34f, 0.16f);
                    break;
                case "tile_dash_lane":
                    bodyColor = new Color(0.10f, 0.48f, 0.47f, 0.70f);
                    lipColor = new Color(0.18f, 0.86f, 0.78f, 0.58f);
                    break;
                case "tile_slime_arena":
                    bodyColor = new Color(0.22f, 0.12f, 0.25f, 0.75f);
                    lipColor = new Color(0.48f, 0.20f, 0.82f, 0.38f);
                    break;
                default:
                    bodyColor = new Color(0.24f, 0.30f, 0.31f);
                    lipColor = RuntimeArtCatalog.Spirit;
                    break;
            }
        }

        private void AddDongMonProceduralTileChunk(string chunkId, string tileId, Vector2 origin, int count, Color bodyColor, Color lipColor)
        {
            for (var i = 0; i < count; i++)
            {
                var x = origin.x + i * 0.54f;
                AddSprite("LGO 2D Tile Chunk " + chunkId + " " + tileId + " body " + i, new Vector2(x, origin.y), new Vector2(0.50f, 0.14f), bodyColor, -7);
                AddSprite("LGO 2D Tile Chunk " + chunkId + " " + tileId + " lip " + i, new Vector2(x, origin.y + 0.11f), new Vector2(0.46f, 0.035f), lipColor, -6);
            }
        }

        private void AddDongMonTilePaletteSwatches()
        {
            AddSceneBeat("DONG_MON_TILE_PALETTE earth/stone/wood/gap/dash/slime swatches no source image");
            AddSceneBeat(RuntimeDongMonTilePaletteSourceSnapshot);
            AddSprite("LGO 2D Tile Palette Grass Swatch", new Vector2(-3.46f, -2.36f), new Vector2(0.28f, 0.045f), new Color(0.25f, 0.64f, 0.42f, 0.72f), -2);
            AddSprite("LGO 2D Tile Palette Stone Swatch", new Vector2(-3.12f, -2.36f), new Vector2(0.28f, 0.045f), new Color(0.52f, 0.61f, 0.60f, 0.70f), -2);
            AddSprite("LGO 2D Tile Palette Wood Swatch", new Vector2(-2.78f, -2.36f), new Vector2(0.28f, 0.045f), new Color(0.58f, 0.34f, 0.16f, 0.70f), -2);
            AddSprite("LGO 2D Tile Palette Gap Swatch", new Vector2(-2.44f, -2.36f), new Vector2(0.28f, 0.045f), new Color(0.04f, 0.07f, 0.10f, 0.70f), -2);
            AddSprite("LGO 2D Tile Palette Dash Swatch", new Vector2(-2.10f, -2.36f), new Vector2(0.28f, 0.045f), new Color(0.18f, 0.86f, 0.78f, 0.62f), -2);
            AddSprite("LGO 2D Tile Palette Slime Swatch", new Vector2(-1.76f, -2.36f), new Vector2(0.28f, 0.045f), new Color(0.48f, 0.20f, 0.82f, 0.60f), -2);
        }


        private void AddDongMonAuthoredDetailPass()
        {
            AddSceneBeat("DONG_MON_AUTHORED_PASS readable detail density from authored details resource no random decoration");
            AddSceneBeat(RuntimeDongMonAuthoredDetailSourceSnapshot);
            var details = TwoDMapDesignCatalog.LoadDongMonAuthoredDetails();
            for (var i = 0; i < details.Length; i++)
            {
                var detail = details[i];
                AddSprite(
                    "LGO 2D Authored Detail " + detail.id + " " + detail.kind,
                    new Vector2(detail.x, detail.y),
                    new Vector2(detail.w, detail.h),
                    new Color(detail.r, detail.g, detail.b, detail.a),
                    detail.sortOrder);
            }
        }

        private void AddDongMonInteractionMarkers()
        {
            AddSceneBeat("DONG_MON_INTERACTION_MARKERS data-driven playable route markers for first map");
            AddSceneBeat(RuntimeDongMonInteractionMarkerSourceSnapshot);
            var markers = TwoDMapDesignCatalog.LoadDongMonInteractionMarkers();
            for (var i = 0; i < markers.Length; i++)
            {
                var marker = markers[i];
                var color = new Color(marker.r, marker.g, marker.b, marker.a);
                var position = new Vector2(marker.x, marker.y);
                AddSprite("LGO 2D Interaction Marker " + marker.id + " Ring", position, new Vector2(marker.radiusW, marker.radiusH), color, marker.sortOrder, null, "ellipse");
                AddSprite("LGO 2D Interaction Marker " + marker.id + " Beam", position + new Vector2(0f, 0.18f), new Vector2(0.045f, 0.28f), new Color(marker.r, marker.g, marker.b, Mathf.Min(0.46f, marker.a)), marker.sortOrder - 1);
                AddSprite("LGO 2D Interaction Marker " + marker.id + " Label Backing", position + new Vector2(0f, 0.42f), new Vector2(0.54f, 0.14f), new Color(0.03f, 0.08f, 0.12f, 0.76f), marker.sortOrder);
                AddWorldLabel("LGO 2D Interaction Marker " + marker.id + " Label", marker.label, position + new Vector2(0f, 0.42f), 0.020f, new Color(marker.r, marker.g, marker.b, 0.92f), marker.sortOrder + 1);
            }
        }


        private void AddDongMonReadabilityRail()
        {
            AddSceneBeat("DONG_MON_READABILITY route-label-rail chips gate/stone/jump/dash/slime avoids HUD overlap");
            AddSceneBeat(RuntimeDongMonReadabilitySnapshot);
            AddSprite("LGO 2D Dong Mon Label Rail Backing", new Vector2(0.95f, -2.54f), new Vector2(4.40f, 0.28f), new Color(0.02f, 0.08f, 0.13f, 0.88f), 58);
            AddWorldLabel("LGO 2D Dong Mon Label Rail Title", "ĐÔNG MÔN ROUTE", new Vector2(-0.93f, -2.51f), 0.023f, RuntimeArtCatalog.Gold, 60);
            AddDongMonRouteChip("01", "Cổng", new Vector2(-0.20f, -2.56f), RuntimeArtCatalog.Gold);
            AddDongMonRouteChip("02", "Bia", new Vector2(0.48f, -2.56f), RuntimeArtCatalog.Spirit);
            AddDongMonRouteChip("03", "Jump", new Vector2(1.16f, -2.56f), RuntimeArtCatalog.Gold);
            AddDongMonRouteChip("04", "Dash", new Vector2(1.86f, -2.56f), RuntimeArtCatalog.Spirit);
            AddDongMonRouteChip("05", "Slime", new Vector2(2.58f, -2.56f), new Color(0.82f, 0.60f, 1f, 0.92f));
        }

        private void AddDongMonRouteChip(string index, string label, Vector2 position, Color accent)
        {
            AddSprite("LGO 2D Dong Mon Route Chip " + index + " Backing", position, new Vector2(0.58f, 0.15f), new Color(0.05f, 0.16f, 0.22f, 0.92f), 59);
            AddSprite("LGO 2D Dong Mon Route Chip " + index + " Accent", position + new Vector2(-0.25f, 0f), new Vector2(0.05f, 0.15f), accent, 60);
            AddWorldLabel("LGO 2D Dong Mon Route Chip " + index + " Label", index + " " + label, position + new Vector2(0.04f, -0.005f), 0.020f, RuntimeArtCatalog.Text, 61);
        }

        private void AddDongMonTerrainCollisionCues()
        {
            AddSceneBeat("Đông Môn terrain collision cues - ground gap dash lane slime arena");
            AddSprite("LGO 2D Collision Ground Main", new Vector2(1.05f, -2.13f), new Vector2(5.90f, 0.055f), new Color(0.11f, 0.52f, 0.48f, 0.72f), -5);
            AddSprite("LGO 2D Collision Jump Gap Left Edge", new Vector2(0.35f, -1.40f), new Vector2(0.045f, 0.42f), RuntimeArtCatalog.Gold, -4);
            AddSprite("LGO 2D Collision Jump Gap Right Edge", new Vector2(1.32f, -1.40f), new Vector2(0.045f, 0.42f), RuntimeArtCatalog.Gold, -4);
            AddWorldLabel("LGO 2D Collision Jump Gap Label", "03", new Vector2(0.84f, -1.18f), 0.021f, RuntimeArtCatalog.Gold, 7);
            AddSprite("LGO 2D Collision Dash Lane", new Vector2(2.45f, -0.77f), new Vector2(1.78f, 0.045f), new Color(0.18f, 0.86f, 0.78f, 0.52f), -4);
            AddWorldLabel("LGO 2D Collision Dash Lane Label", "04", new Vector2(2.45f, -0.62f), 0.021f, RuntimeArtCatalog.Spirit, 7);
            AddSprite("LGO 2D Collision Slime Arena", new Vector2(3.60f, -1.66f), new Vector2(0.92f, 0.06f), new Color(0.48f, 0.20f, 0.82f, 0.42f), -4);
        }

        public void ToggleInventoryPanel()
        {
            _inventoryOpen = !_inventoryOpen;
            _inventoryInputState = _inventoryOpen ? "Open" : "Closed";
            if (!_inventoryOpen) EnsurePlayerLoadout().CancelPreview();
            RefreshPresentation();
        }

        public void SelectNextInventoryItem()
        {
            _inventoryOpen = true;
            _inventorySelectedIndex = (_inventorySelectedIndex + 1) % _inventoryItemIds.Length;
            _inventoryInputState = "Open";
            EnsurePlayerLoadout().CancelPreview();
            RefreshPresentation();
        }

        public bool PreviewSelectedInventoryItem()
        {
            _inventoryOpen = true;
            var itemId = _inventoryItemIds[_inventorySelectedIndex];
            var ok = EnsurePlayerLoadout().TryPreview(itemId, _moduleCatalog);
            _inventoryInputState = ok ? "Trying" : "Open";
            RefreshPresentation();
            return ok;
        }

        public void ApplyInventoryPreview()
        {
            _inventoryOpen = true;
            EnsurePlayerLoadout().ApplyPreview();
            _inventoryInputState = "Applied";
            RefreshPresentation();
        }

        public void CancelInventoryPreview()
        {
            EnsurePlayerLoadout().CancelPreview();
            _inventoryOpen = false;
            _inventoryInputState = "Cancelled";
            RefreshPresentation();
        }

        private string BuildInventoryInputSnapshot()
        {
            var selectedId = _inventoryItemIds[_inventorySelectedIndex];
            return "InventoryInputState=" + _inventoryInputState
                + " | open=" + _inventoryOpen
                + " | selected=" + selectedId
                + BuildSelectedInventoryCompatibilitySnapshot(selectedId)
                + " | controls=I toggle, Tab select, T try, Y apply, Esc cancel"
                + " | " + EnsurePlayerLoadout().Snapshot;
        }

        private string BuildSelectedInventoryCompatibilitySnapshot(string selectedId)
        {
            if (!_moduleCatalog.TryFind(selectedId, out var item))
                return " | selectedCompat=False | selectedSlot=missing | selectedAnchor=missing | fitProfile=missing | source=runtime-authored-catalog";
            var compatible = item.BaseFilter == "unisex" || item.BaseFilter == "male_base";
            return " | selectedCompat=" + compatible
                + " | selectedSlot=" + item.Slot
                + " | selectedAnchor=" + ToDefaultAnchor(item.Slot)
                + " | fitProfile=" + ExtractRuntimeRuleValue(item.RuntimeRule, "fit_profile")
                + " | source=runtime-authored-catalog";
        }

        private static string ToDefaultAnchor(string slot)
        {
            switch (slot)
            {
                case "HairFront": return "HairFrontAnchor";
                case "HairBack": return "HairBackAnchor";
                case "Eyes": return "Head";
                case "InnerShirt":
                case "OuterShirt": return "Chest";
                case "PantsOrSkirt":
                case "Waist": return "Hips";
                case "Gloves": return "Hand_L,Hand_R";
                case "Boots": return "Foot_L,Foot_R";
                case "Weapon": return "WeaponAnchor";
                case "PetSpirit": return "PetAnchor";
                default: return "Unknown";
            }
        }

        private static string ExtractRuntimeRuleValue(string runtimeRule, string key)
        {
            if (string.IsNullOrEmpty(runtimeRule)) return "missing";
            var prefix = key + "=";
            var parts = runtimeRule.Split(';');
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                if (part.StartsWith(prefix, System.StringComparison.Ordinal)) return part.Substring(prefix.Length);
            }
            return "missing";
        }

        private string BuildVoLv1ClassSliceRuntimeSnapshot()
        {
            return TwoDClassSliceCatalog.LoadVoLv1ClassSliceSnapshot()
                + " | runtimeLoadout=" + EnsurePlayerLoadout().Snapshot
                + " | runtimeAnimation=" + _runtimeAnimationSnapshot
                + " | runtimeSkillCue=" + (_voLv1SkillCueRoot != null && _voLv1SkillCueRoot.gameObject.activeSelf);
        }

        private string BuildVoLv1PaperDollAtlasRuntimeSnapshot()
        {
            return TwoDPaperDollAtlasCatalog.LoadVoLv1PaperDollAtlasSnapshot()
                + " | runtimeLoadout=" + EnsurePlayerLoadout().Snapshot
                + " | runtimeAnimation=" + _runtimeAnimationSnapshot
                + " | currentPose=" + _runtimeVoLv1PaperDollPoseId
                + " | active=" + (_voLv1PaperDollRoot != null && _voLv1PaperDollRoot.gameObject.activeSelf);
        }

        private string BuildVoLv1AnchorGizmoSnapshot()
        {
            return "VoLv1AnchorGizmo: visible=" + (_voLv1AnchorGizmoRoot != null && _voLv1AnchorGizmoRoot.gameObject.activeSelf)
                + " | pivotPolicy=bottom-center-foot-anchor"
                + " | slot=OuterShirt anchor=Chest marker=gold-diamond"
                + " | slot=PantsOrSkirt anchor=Hips marker=red-diamond"
                + " | slot=Waist anchor=Hips marker=gold-small-diamond"
                + " | slot=Gloves anchor=Hand_L marker=cyan-diamond"
                + " | slot=Gloves anchor=Hand_R marker=cyan-diamond"
                + " | slot=Boots anchor=Foot_L marker=soft-white-diamond"
                + " | slot=Boots anchor=Foot_R marker=soft-white-diamond"
                + " | visibleWhen=inventory-or-class-training"
                + " | safe-runtime-gizmo=True | safe-no-source-image=True | safe-no-3d=True";
        }

        private string BuildVoLv1RuntimeFitSnapshot()
        {
            return "VoLv1RuntimeFit: fitStatus=ANCHOR_ALIGNED"
                + " | base=male_base"
                + " | scenePlane=dong-mon-gameplay-plane"
                + " | lane=training-route"
                + " | playerPivot=bottom-center"
                + " | footContact=contact-shadow-bottom"
                + " | slot=OuterShirt item=top_vo_lv1_male anchor=Chest pivot=bottom-center bounds=-0.26,-0.48,0.52,0.72 sort=4"
                + " | slot=PantsOrSkirt item=pants_vo_lv1_unisex anchor=Hips pivot=top-center bounds=-0.18,-0.82,0.36,0.46 sort=3"
                + " | slot=Waist item=waist_vo_lv1_unisex anchor=Hips pivot=center bounds=-0.28,-0.38,0.56,0.12 sort=6"
                + " | slot=Gloves item=gloves_vo_lv1_unisex anchor=Hand_L pivot=center bounds=-0.43,-0.43,0.16,0.16 sort=7"
                + " | slot=Gloves item=gloves_vo_lv1_unisex anchor=Hand_R pivot=center bounds=0.27,-0.43,0.16,0.16 sort=7"
                + " | slot=Boots item=boots_vo_lv1_unisex anchor=Foot_L pivot=bottom-center bounds=-0.22,-0.90,0.18,0.16 sort=6"
                + " | slot=Boots item=boots_vo_lv1_unisex anchor=Foot_R pivot=bottom-center bounds=0.04,-0.90,0.18,0.16 sort=6"
                + " | skill=vo_lv1_first_skill anchor=Hand_R pivot=palm-forward sort=9"
                + " | runtimeCheck=slot-bounds-follow-paperdoll-pose"
                + " | safe-runtime-fit=True | safe-no-source-image=True | safe-no-3d=True";
        }

        private string BuildInventoryTryOnSnapshot()
        {
            var previewLoadout = TwoDCharacterLoadout.CreateStarter("male_base", _moduleCatalog);
            previewLoadout.ApplyVoLv1Starter(_moduleCatalog);
            previewLoadout.TryPreview("top_kiem_lv1_male", _moduleCatalog);
            return "InventoryTryOn: flow=select_icon -> inspect_item -> try_on -> cancel_or_apply"
                + " | selected=top_kiem_lv1_male"
                + " | inspect=Áo Kiếm Lv1 nam xanh đen gọn"
                + " | preview=top_kiem_lv1_male"
                + " | apply=weapon_kiem_lv1_starter"
                + " | cancel=return_to_vo_lv1"
                + " | " + previewLoadout.Snapshot;
        }

        private Transform AddVoLv1SkillCue(string name, Vector2 position, int order)
        {
            AddSceneBeat("VO_LV1_CLASS_SLICE runtime paper-doll slots motion skill cue");
            AddSceneBeat(TwoDClassSliceCatalog.LoadVoLv1ClassSliceSnapshot());
            var root = new GameObject(name);
            root.transform.SetParent(transform, false);
            root.transform.localPosition = ToWorld(position, order * 0.01f);
            AddSprite(name + " Trail Core", new Vector2(0.00f, 0.00f), new Vector2(0.96f, 0.08f), new Color(0.95f, 0.72f, 0.28f, 0.78f), order, root.transform);
            AddSprite(name + " Trail Cyan Edge", new Vector2(0.24f, 0.10f), new Vector2(0.72f, 0.045f), new Color(0.18f, 0.86f, 0.78f, 0.56f), order + 1, root.transform);
            AddSprite(name + " Impact Palm", new Vector2(0.58f, 0.02f), new Vector2(0.18f, 0.18f), new Color(0.95f, 0.72f, 0.28f, 0.78f), order + 2, root.transform, "diamond");
            AddWorldLabel(name + " Label", "VÕ Q", new Vector2(0.22f, 0.26f), 0.026f, RuntimeArtCatalog.Gold, order + 3, root.transform);
            root.SetActive(false);
            return root.transform;
        }


        private static Transform AddVoLv1PaperDollOverlay(Transform player, int baseOrder)
        {
            var root = new GameObject("LGO 2D Player Vo PaperDoll Atlas Root");
            root.transform.SetParent(player, false);
            root.transform.localPosition = Vector3.zero;

            var atlas = TwoDPaperDollAtlasCatalog.LoadVoLv1PaperDollAtlas();
            if (atlas != null && atlas.parts != null)
            {
                for (var i = 0; i < atlas.parts.Length; i++)
                {
                    var part = atlas.parts[i];
                    AddVoLv1PaperDollPart(root.transform, part, baseOrder + part.sortOffset, false);
                }
            }

            if (atlas != null && atlas.skillCues != null)
            {
                for (var i = 0; i < atlas.skillCues.Length; i++)
                {
                    var cue = atlas.skillCues[i];
                    AddVoLv1PaperDollPart(root.transform, cue, baseOrder + cue.sortOffset, true);
                }
            }

            ApplyVoLv1PaperDollPose(root.transform, "vo_idle", false);
            root.SetActive(false);
            return root.transform;
        }

        private static void AddVoLv1PaperDollPart(Transform root, VoLv1PaperDollAtlasPart part, int order, bool skillCue)
        {
            var name = skillCue ? "LGO 2D Player Vo PaperDoll SkillCue " + part.id : "LGO 2D Player Vo PaperDoll " + part.slot + " " + part.id;
            AddSprite(
                name,
                new Vector2(part.x, part.y),
                new Vector2(part.w, part.h),
                new Color(part.r, part.g, part.b, part.a),
                order,
                root,
                part.shape);
            var anchor = new GameObject("LGO 2D Player Vo PaperDoll PoseAnchor " + part.id);
            anchor.transform.SetParent(root, false);
            anchor.transform.localPosition = ToWorld(new Vector2(part.x, part.y), 0f);
        }

        private static Transform AddVoLv1AnchorGizmo(Transform player, int baseOrder)
        {
            var root = new GameObject("LGO 2D Player Vo Anchor Gizmo Root");
            root.transform.SetParent(player, false);
            root.transform.localPosition = Vector3.zero;
            AddVoLv1AnchorMarker(root.transform, "OuterShirt", "Chest", new Vector2(0f, 0.03f), RuntimeArtCatalog.Gold, baseOrder);
            AddVoLv1AnchorMarker(root.transform, "PantsOrSkirt", "Hips", new Vector2(0f, -0.28f), new Color(0.86f, 0.32f, 0.20f, 0.88f), baseOrder);
            AddVoLv1AnchorMarker(root.transform, "Waist", "Hips", new Vector2(0.18f, -0.28f), new Color(0.95f, 0.72f, 0.28f, 0.76f), baseOrder + 1);
            AddVoLv1AnchorMarker(root.transform, "Gloves", "Hand_R", new Vector2(0.35f, -0.32f), RuntimeArtCatalog.Spirit, baseOrder + 1);
            AddVoLv1AnchorMarker(root.transform, "Gloves", "Hand_L", new Vector2(-0.35f, -0.32f), RuntimeArtCatalog.Spirit, baseOrder + 1);
            AddVoLv1AnchorMarker(root.transform, "Boots", "Foot_L", new Vector2(-0.12f, -0.72f), new Color(0.73f, 0.87f, 0.88f, 0.82f), baseOrder);
            AddVoLv1AnchorMarker(root.transform, "Boots", "Foot_R", new Vector2(0.12f, -0.72f), new Color(0.73f, 0.87f, 0.88f, 0.82f), baseOrder);
            AddWorldLabel("LGO 2D Player Vo Anchor Gizmo Label", "slot anchors", new Vector2(0f, -0.92f), 0.017f, RuntimeArtCatalog.Spirit, baseOrder + 2, root.transform);
            root.SetActive(false);
            return root.transform;
        }

        private static void AddVoLv1AnchorMarker(Transform root, string slot, string anchor, Vector2 position, Color color, int order)
        {
            AddSprite("LGO 2D Player Vo Anchor Gizmo " + slot + " " + anchor, position, new Vector2(0.065f, 0.065f), color, order, root, "diamond");
        }

        private void RefreshVoLv1AnchorGizmoVisibility()
        {
            if (_voLv1AnchorGizmoRoot == null) return;
            _voLv1AnchorGizmoRoot.gameObject.SetActive(_inventoryOpen || _state.Step >= TwoDOnboardingStep.LearnJump);
        }

        private static void ApplyVoLv1PaperDollPose(Transform root, string poseId, bool showSkillCue)
        {
            if (root == null) return;
            var atlas = TwoDPaperDollAtlasCatalog.LoadVoLv1PaperDollAtlas();
            if (atlas == null) return;

            ApplyVoLv1PaperDollParts(root, atlas.parts, poseId, false, showSkillCue);
            ApplyVoLv1PaperDollParts(root, atlas.skillCues, poseId, true, showSkillCue);
        }

        private static void ApplyVoLv1PaperDollParts(Transform root, VoLv1PaperDollAtlasPart[] parts, string poseId, bool skillCue, bool showSkillCue)
        {
            if (parts == null) return;
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var offset = FindVoLv1PaperDollPoseOffset(poseId, part.id);
                var dx = offset != null ? offset.dx : 0f;
                var dy = offset != null ? offset.dy : 0f;
                var sx = offset != null && offset.sx > 0.001f ? offset.sx : 1f;
                var sy = offset != null && offset.sy > 0.001f ? offset.sy : 1f;
                var partTransform = FindDirectChild(root, skillCue ? "LGO 2D Player Vo PaperDoll SkillCue " + part.id : "LGO 2D Player Vo PaperDoll " + part.slot + " " + part.id);
                if (partTransform != null)
                {
                    partTransform.localPosition = ToWorld(new Vector2(part.x + dx, part.y + dy), 0f);
                    partTransform.localScale = new Vector3(part.w * sx, part.h * sy, 1f);
                    partTransform.gameObject.SetActive(!skillCue || showSkillCue);
                }
                var anchorTransform = FindDirectChild(root, "LGO 2D Player Vo PaperDoll PoseAnchor " + part.id);
                if (anchorTransform != null)
                {
                    anchorTransform.localPosition = ToWorld(new Vector2(part.x + dx, part.y + dy), 0f);
                    anchorTransform.gameObject.SetActive(!skillCue || showSkillCue);
                }
            }
        }

        private static VoLv1PaperDollPoseOffset FindVoLv1PaperDollPoseOffset(string poseId, string partId)
        {
            var atlas = TwoDPaperDollAtlasCatalog.LoadVoLv1PaperDollAtlas();
            if (atlas == null || atlas.poseOffsets == null) return null;
            for (var i = 0; i < atlas.poseOffsets.Length; i++)
            {
                var offset = atlas.poseOffsets[i];
                if (offset.pose == poseId && offset.partId == partId) return offset;
            }
            return null;
        }

        private static Transform FindDirectChild(Transform root, string childName)
        {
            if (root == null) return null;
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == childName) return child;
            }
            return null;
        }

        private static string ToVoLv1PaperDollPoseId(string animationState)
        {
            if (animationState == "TrainingCompletePose") return "vo_lv1_training_complete";
            if (animationState == "ClassSkill") return "vo_skill_cast";
            if (animationState == "Dash") return "vo_dash_stretch";
            if (animationState == "Jump") return "vo_jump_lift";
            return "vo_idle";
        }

        private void BuildInventoryTryOnStrip()
        {
            AddSceneBeat("Inventory try-on panel - input inspect thử đồ áp dụng/hủy");
            var root = new GameObject("LGO 2D Inventory TryOn Root");
            root.transform.SetParent(transform, false);
            _inventoryPanelRoot = root.transform;
            AddSprite("LGO 2D Inventory TryOn Panel", new Vector2(-0.20f, -1.95f), new Vector2(3.28f, 0.72f), new Color(0.03f, 0.08f, 0.13f, 0.90f), 56, _inventoryPanelRoot);
            AddWorldLabel("LGO 2D Inventory TryOn Title", "HÀNH TRANG", new Vector2(-1.48f, -1.67f), 0.033f, RuntimeArtCatalog.Gold, 67, _inventoryPanelRoot);
            _inventoryModeLabel = AddWorldLabel("LGO 2D Inventory Input Mode", "OPEN: Tab chọn món", new Vector2(0.12f, -1.67f), 0.026f, RuntimeArtCatalog.Text, 67, _inventoryPanelRoot);
            AddSprite("LGO 2D Inventory Slot Vo", new Vector2(-1.34f, -2.00f), new Vector2(0.28f, 0.28f), RuntimeArtCatalog.Gold, 66, _inventoryPanelRoot);
            AddSprite("LGO 2D Inventory Slot Kiem", new Vector2(-0.88f, -2.00f), new Vector2(0.28f, 0.28f), RuntimeArtCatalog.Spirit, 66, _inventoryPanelRoot);
            AddSprite("LGO 2D Inventory Slot Sword", new Vector2(-0.42f, -2.00f), new Vector2(0.065f, 0.34f), new Color(0.77f, 0.91f, 0.95f), 67, _inventoryPanelRoot);
            AddSprite("LGO 2D Inventory Selected Ring", new Vector2(-0.88f, -2.00f), new Vector2(0.38f, 0.38f), new Color(0.18f, 0.86f, 0.78f, 0.34f), 65, _inventoryPanelRoot);
            _inventorySelectedLabel = AddWorldLabel("LGO 2D Inventory TryOn Preview", "THỬ: top_kiem_lv1_male", new Vector2(0.48f, -1.96f), 0.027f, RuntimeArtCatalog.Spirit, 67, _inventoryPanelRoot);
            _inventoryHelpLabel = AddWorldLabel("LGO 2D Inventory Input Help", "T THỬ  •  Y ÁP DỤNG  •  Esc HỦY", new Vector2(0.30f, -2.18f), 0.025f, RuntimeArtCatalog.Gold, 67, _inventoryPanelRoot);
            _inventoryPanelRoot.gameObject.SetActive(false);
        }

        private void RefreshInventoryPanelPresentation()
        {
            if (_inventoryPanelRoot != null) _inventoryPanelRoot.gameObject.SetActive(_inventoryOpen);
            SetHudText(_inventoryModeLabel, (_inventoryOpen ? "OPEN" : "I mở") + ": Tab chọn món");
            SetHudText(_inventorySelectedLabel, (_inventoryInputState == "Trying" ? "THỬ: " : _inventoryInputState == "Applied" ? "ĐANG MẶC: " : "Chọn: ") + _inventoryItemIds[_inventorySelectedIndex]);
            SetHudText(_inventoryHelpLabel, "T THỬ  •  Y ÁP DỤNG  •  Esc HỦY");
        }

        private void BuildRuntimeMapOverlay()
        {
            AddSceneBeat("Minimap Đông Môn theo route A-Z");
            AddSceneBeat(RuntimeMinimapReadabilitySnapshot);
            AddSprite("LGO 2D Mini Map Panel", new Vector2(3.05f, 2.14f), new Vector2(2.24f, 1.36f), new Color(0.03f, 0.08f, 0.13f, 0.92f), 55);
            AddWorldLabel("LGO 2D Mini Map Title", "BẢN ĐỒ", new Vector2(2.58f, 2.70f), 0.036f, RuntimeArtCatalog.Gold, 66);
            AddWorldLabel("LGO 2D Mini Map Hub", "Linh Thành", new Vector2(3.18f, 2.46f), 0.032f, RuntimeArtCatalog.Text, 66);
            AddWorldLabel("LGO 2D Mini Map Route", "Đông Môn → Linh Thành", new Vector2(3.18f, 2.23f), 0.027f, RuntimeArtCatalog.Spirit, 66);
            AddWorldLabel("LGO 2D Mini Map Chapter", "Tutorial route + hub rail", new Vector2(3.18f, 2.08f), 0.023f, RuntimeArtCatalog.Gold, 66);
            _miniMapProgress = AddWorldLabel("LGO 2D Mini Map Progress", "Node: spawn", new Vector2(3.18f, 1.93f), 0.024f, RuntimeArtCatalog.Text, 66);
            AddWorldLabel("LGO 2D Mini Map District Chips", "HV  TP  ĐL  KR  BH  CẢ", new Vector2(3.18f, 1.58f), 0.024f, RuntimeArtCatalog.Spirit, 66);
            AddWorldZoneNetworkOverlay();

            var route = _mapCatalog.DongMonRoute;
            for (var i = 0; i < route.Length && i < 6; i++)
            {
                var x = 2.35f + i * 0.31f;
                var markerColor = i <= 2 ? RuntimeArtCatalog.Spirit : new Color(0.42f, 0.54f, 0.62f, 0.85f);
                AddSprite("LGO 2D Mini Map Node " + route[i].Id, new Vector2(x, 1.80f), new Vector2(0.10f, 0.10f), markerColor, 64);
                if (i > 0)
                    AddSprite("LGO 2D Mini Map Link " + i, new Vector2(x - 0.16f, 1.80f), new Vector2(0.19f, 0.025f), new Color(0.18f, 0.70f, 0.75f, 0.72f), 63);
            }
        }


        private void AddWorldZoneNetworkOverlay()
        {
            AddSceneBeat("ZONE_NETWORK_OVERLAY World Map hub Linh Thành -> Đông Vực/Âm Giới");
            AddWorldLabel("LGO 2D World Zone Network Label", "World links", new Vector2(3.18f, 1.30f), 0.021f, RuntimeArtCatalog.Gold, 66);
            AddSprite("LGO 2D World Zone Link East", new Vector2(3.02f, 1.17f), new Vector2(0.36f, 0.025f), new Color(0.18f, 0.70f, 0.75f, 0.72f), 63);
            AddSprite("LGO 2D World Zone Link Underworld", new Vector2(3.34f, 1.17f), new Vector2(0.36f, 0.025f), new Color(0.48f, 0.20f, 0.82f, 0.58f), 63);
            AddSprite("LGO 2D World Zone Node Linh Thanh", new Vector2(3.18f, 1.17f), new Vector2(0.12f, 0.12f), RuntimeArtCatalog.Gold, 64);
            AddSprite("LGO 2D World Zone Node Dong Vuc", new Vector2(2.78f, 1.17f), new Vector2(0.10f, 0.10f), RuntimeArtCatalog.Spirit, 64);
            AddSprite("LGO 2D World Zone Node Am Gioi", new Vector2(3.58f, 1.17f), new Vector2(0.10f, 0.10f), new Color(0.62f, 0.28f, 0.92f, 0.90f), 64);
        }

        private void BuildWorldHud()
        {
            AddSprite("LGO 2D HUD Panel", new Vector2(-1.82f, 2.33f), new Vector2(4.7f, 1.12f), new Color(0.03f, 0.07f, 0.12f, 0.92f), 60);
            AddSprite("LGO 2D HUD Accent", new Vector2(-3.96f, 2.82f), new Vector2(0.18f, 0.12f), RuntimeArtCatalog.Spirit, 61);
            _hudTitle = AddHudText("LGO 2D HUD Title", new Vector2(-3.82f, 2.80f), 0.046f, RuntimeArtCatalog.Gold, 70);
            _hudArea = AddHudText("LGO 2D HUD Area", new Vector2(-3.82f, 2.58f), 0.030f, RuntimeArtCatalog.Text, 70);
            _hudObjective = AddHudText("LGO 2D HUD Objective", new Vector2(-3.82f, 2.42f), 0.030f, RuntimeArtCatalog.Text, 70);
            _hudHint = AddHudText("LGO 2D HUD Hint", new Vector2(-3.82f, 2.26f), 0.027f, new Color(0.73f, 0.87f, 0.88f), 70);
            _hudAction = AddHudText("LGO 2D HUD Action", new Vector2(-3.82f, 2.10f), 0.027f, RuntimeArtCatalog.Spirit, 70);

            _hudDialoguePanel = AddSprite("LGO 2D Dialogue Panel", new Vector2(-1.5f, 0.86f), new Vector2(5.35f, 0.46f), new Color(0.04f, 0.10f, 0.15f, 0.94f), 60).transform;
            _hudDialogue = AddHudText("LGO 2D Dialogue Text", new Vector2(-3.92f, 0.96f), 0.026f, RuntimeArtCatalog.Gold, 70);
            _hudFeedback = AddHudText("LGO 2D Feedback Text", new Vector2(-3.82f, -2.62f), 0.030f, RuntimeArtCatalog.Gold, 70);
            AddSprite("LGO 2D Feedback Panel", new Vector2(-1.08f, -2.52f), new Vector2(5.9f, 0.34f), new Color(0.03f, 0.07f, 0.12f, 0.84f), 59);
        }

        private void RefreshWorldHud()
        {
            var lines = new List<string>
            {
                "Linh Giới Online — 2D nhập môn",
                "Khu vực: " + _state.AreaText,
                "Mục tiêu: " + _state.ObjectiveText,
                "Gợi ý: " + _state.HintText,
                "Hành động: " + DescribeAction(_state.AvailableAction),
                "Cảm nhận: " + _state.FeedbackText
            };
            if (_state.DialogueOpen) lines.Add(_state.DialogueSpeaker + ": " + _state.DialogueLine);

            SetHudText(_hudTitle, lines[0]);
            SetHudText(_hudArea, Shorten(lines[1], 58));
            SetHudText(_hudObjective, Shorten(lines[2], 68));
            SetHudText(_hudHint, Shorten(lines[3], 70));
            SetHudText(_hudAction, Shorten(lines[4], 54));
            SetHudText(_hudFeedback, Shorten(lines[5], 74));
            SetHudText(_hudDialogue, _state.DialogueOpen ? Shorten(lines[6], 82) : string.Empty);
            if (_hudDialoguePanel != null) _hudDialoguePanel.gameObject.SetActive(_state.DialogueOpen);
            if (_hudDialogue != null) _hudDialogue.gameObject.SetActive(_state.DialogueOpen);

            WorldHudLineCount = lines.Count;
            _worldHudSnapshot = string.Join("\n", lines.ToArray());
        }

        private static void SetHudText(TextMesh target, string value)
        {
            if (target != null) target.text = value;
        }

        private static string Shorten(string value, int maxCharacters)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxCharacters) return value;
            return value.Substring(0, Mathf.Max(0, maxCharacters - 1)) + "…";
        }

        private static TextMesh AddHudText(string name, Vector2 position, float size, Color color, int order)
        {
            var host = new GameObject(name);
            host.transform.position = ToWorld(position, order * 0.01f);
            var text = host.AddComponent<TextMesh>();
            text.anchor = TextAnchor.UpperLeft;
            text.alignment = TextAlignment.Left;
            text.characterSize = size;
            text.fontSize = 36;
            text.color = color;
            var renderer = host.GetComponent<MeshRenderer>();
            renderer.sortingOrder = order;
            return text;
        }

        private GameObject AddSceneSprite(string name, string beat, Vector2 position, Vector2 scale, Color color, int order)
        {
            AddSceneBeat(beat);
            return AddSprite(name, position, scale, color, order);
        }

        private void AddLantern(string name, string beat, Vector2 position, int order)
        {
            AddSceneBeat(beat);
            AddSprite(name + " Post", position + new Vector2(0f, -0.16f), new Vector2(0.05f, 0.54f), new Color(0.18f, 0.11f, 0.06f), order);
            AddSprite(name + " Light", position + new Vector2(0f, 0.15f), new Vector2(0.22f, 0.26f), new Color(0.95f, 0.48f, 0.16f, 0.88f), order + 1);
            AddSprite(name + " Glow", position + new Vector2(0f, 0.15f), new Vector2(0.40f, 0.40f), new Color(1f, 0.70f, 0.25f, 0.20f), order);
        }

        private static TextMesh AddWorldLabel(string name, string value, Vector2 position, float size, Color color, int order)
        {
            return AddWorldLabel(name, value, position, size, color, order, null);
        }

        private static TextMesh AddWorldLabel(string name, string value, Vector2 position, float size, Color color, int order, Transform parent)
        {
            var host = new GameObject(name);
            host.transform.SetParent(parent, false);
            host.transform.localPosition = ToWorld(position, parent == null ? order * 0.01f : 0f);
            var text = host.AddComponent<TextMesh>();
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = size;
            text.fontSize = 36;
            text.color = color;
            text.text = value;
            var renderer = host.GetComponent<MeshRenderer>();
            renderer.sortingOrder = order;
            return text;
        }

        private void AddSceneBeat(string beat)
        {
            if (!string.IsNullOrEmpty(beat)) _productionSceneBeats.Add(beat);
        }

        private static Transform AddNpcSpriteFromResource(string name, string npcId, Vector2 position, Color fallbackRobe, Color fallbackAccent, Color fallbackHair, int order)
        {
            var npc = TwoDMapDesignCatalog.LoadDongMonNpcSprite(npcId);
            if (npc == null || npc.parts == null || npc.parts.Length == 0)
            {
                return AddCharacter(name, position, fallbackRobe, fallbackAccent, fallbackHair, order);
            }

            var root = new GameObject(name);
            root.transform.position = ToWorld(position, order * 0.01f);
            for (var i = 0; i < npc.parts.Length; i++)
            {
                var part = npc.parts[i];
                AddSprite(
                    name + " SpritePart " + part.id + " " + part.slot,
                    new Vector2(part.x, part.y),
                    new Vector2(part.w, part.h),
                    new Color(part.r, part.g, part.b, part.a),
                    order + part.sortOffset,
                    root.transform,
                    part.shape);
            }

            return root.transform;
        }

        private static Transform AddCharacter(string name, Vector2 position, Color robe, Color accent, Color hair, int order)
        {
            var root = new GameObject(name);
            root.transform.position = ToWorld(position, order * 0.01f);
            AddSprite(name + " LayeredCharacter Shadow Slot Shadow", new Vector2(0f, -0.58f), new Vector2(0.72f, 0.12f), new Color(0f, 0f, 0f, 0.25f), order - 1, root.transform);
            AddSprite(name + " Left Leg", new Vector2(-0.11f, -0.54f), new Vector2(0.13f, 0.28f), robe * 0.72f, order - 1, root.transform);
            AddSprite(name + " Right Leg", new Vector2(0.11f, -0.54f), new Vector2(0.13f, 0.28f), robe * 0.72f, order - 1, root.transform);
            AddSprite(name + " LayerSlot Body", new Vector2(0f, 0.04f), new Vector2(0.30f, 0.52f), new Color(0.86f, 0.70f, 0.56f), order - 1, root.transform);
            AddSprite(name + " LayerSlot InnerShirt", new Vector2(0f, -0.12f), new Vector2(0.36f, 0.46f), robe, order, root.transform);
            AddSprite(name + " Shoulder Line", new Vector2(0f, 0.13f), new Vector2(0.50f, 0.07f), accent, order + 1, root.transform);
            AddSprite(name + " Sash", new Vector2(0f, -0.2f), new Vector2(0.46f, 0.09f), accent, order + 1, root.transform);
            AddSprite(name + " Robe Trim", new Vector2(0f, -0.42f), new Vector2(0.34f, 0.045f), accent, order + 1, root.transform);
            AddSprite(name + " Head", new Vector2(0f, 0.34f), new Vector2(0.30f, 0.30f), new Color(0.86f, 0.70f, 0.56f), order + 2, root.transform);
            AddSprite(name + " LayerSlot HairBack", new Vector2(0f, 0.51f), new Vector2(0.38f, 0.16f), hair, order + 1, root.transform);
            AddSprite(name + " LayerSlot Eyes eyes_default", new Vector2(0f, 0.35f), new Vector2(0.18f, 0.035f), new Color(0.03f, 0.05f, 0.07f), order + 4, root.transform);
            AddSprite(name + " LayerSlot HairFront", new Vector2(-0.05f, 0.43f), new Vector2(0.28f, 0.08f), hair, order + 5, root.transform);
            AddSprite(name + " Left Arm", new Vector2(-0.28f, -0.12f), new Vector2(0.12f, 0.46f), robe, order - 1, root.transform);
            AddSprite(name + " Right Arm", new Vector2(0.28f, -0.12f), new Vector2(0.12f, 0.46f), robe, order - 1, root.transform);
            AddSprite(name + " Wrist Guard Left", new Vector2(-0.28f, -0.36f), new Vector2(0.16f, 0.08f), accent, order + 1, root.transform);
            AddSprite(name + " Wrist Guard Right", new Vector2(0.28f, -0.36f), new Vector2(0.16f, 0.08f), accent, order + 1, root.transform);
            return root.transform;
        }


        private void AddKiemLv1PreviewRack(Vector2 position, int order)
        {
            AddSceneBeat("Kiếm Lv1 module rack - preview trang bị class thứ hai");
            AddSprite("LGO 2D Kiem Lv1 Rack Panel", position, new Vector2(1.18f, 0.54f), new Color(0.04f, 0.10f, 0.15f, 0.86f), order);
            AddSprite("LGO 2D Kiem Lv1 Top Preview", position + new Vector2(-0.33f, 0.02f), new Vector2(0.28f, 0.36f), new Color(0.06f, 0.23f, 0.34f), order + 1);
            AddSprite("LGO 2D Kiem Lv1 Top Trim", position + new Vector2(-0.33f, 0.20f), new Vector2(0.34f, 0.045f), RuntimeArtCatalog.Spirit, order + 2);
            AddSprite("LGO 2D Kiem Lv1 Sword Blade", position + new Vector2(0.23f, 0.06f), new Vector2(0.055f, 0.46f), new Color(0.77f, 0.91f, 0.95f), order + 2);
            AddSprite("LGO 2D Kiem Lv1 Sword Guard", position + new Vector2(0.23f, -0.17f), new Vector2(0.26f, 0.045f), RuntimeArtCatalog.Gold, order + 3);
            AddSprite("LGO 2D Kiem Lv1 Sword Trail Seed", position + new Vector2(0.42f, 0.16f), new Vector2(0.34f, 0.055f), new Color(0.20f, 0.86f, 0.92f, 0.55f), order + 1);
            AddWorldLabel("LGO 2D Kiem Lv1 Rack Label", "KIẾM LV1", position + new Vector2(0f, -0.38f), 0.031f, RuntimeArtCatalog.Spirit, order + 8);
        }

        private static Transform AddTrainingStone(string name, Vector2 position, int order)
        {
            var root = new GameObject(name);
            root.transform.position = ToWorld(position, order * 0.01f);
            AddSprite(name + " Aura", new Vector2(0f, -0.05f), new Vector2(0.86f, 1.1f), new Color(0.07f, 0.75f, 0.72f, 0.55f), order - 1, root.transform);
            AddSprite(name + " Body", new Vector2(0f, 0f), new Vector2(0.48f, 0.82f), new Color(0.90f, 0.91f, 0.84f), order, root.transform);
            AddSprite(name + " Jade Core", new Vector2(0f, 0.08f), new Vector2(0.18f, 0.44f), RuntimeArtCatalog.Spirit, order + 1, root.transform);
            AddSprite(name + " Base", new Vector2(0f, -0.48f), new Vector2(0.74f, 0.12f), new Color(0.16f, 0.36f, 0.34f), order, root.transform);
            return root.transform;
        }

        private static Transform AddShadowSlime(string name, Vector2 position, int order)
        {
            var root = new GameObject(name);
            root.transform.position = ToWorld(position, order * 0.01f);
            AddSprite(name + " Ground Shadow", new Vector2(0f, -0.26f), new Vector2(0.70f, 0.12f), new Color(0f, 0f, 0f, 0.30f), order - 1, root.transform);
            AddSprite(name + " Body", new Vector2(0f, -0.02f), new Vector2(0.58f, 0.44f), new Color(0.22f, 0.08f, 0.36f, 0.96f), order, root.transform);
            AddSprite(name + " Aura", new Vector2(0f, -0.02f), new Vector2(0.76f, 0.58f), new Color(0.48f, 0.20f, 0.82f, 0.24f), order - 1, root.transform);
            AddSprite(name + " Core", new Vector2(0.05f, 0.03f), new Vector2(0.18f, 0.14f), new Color(0.70f, 0.45f, 1f, 0.92f), order + 1, root.transform);
            AddSprite(name + " Eye Left", new Vector2(-0.10f, 0.07f), new Vector2(0.045f, 0.035f), RuntimeArtCatalog.Text, order + 2, root.transform);
            AddSprite(name + " Eye Right", new Vector2(0.13f, 0.07f), new Vector2(0.045f, 0.035f), RuntimeArtCatalog.Text, order + 2, root.transform);
            return root.transform;
        }

        private static GameObject AddSprite(string name, Vector2 position, Vector2 scale, Color color, int order)
        {
            return AddSprite(name, position, scale, color, order, null);
        }

        private static GameObject AddSprite(string name, Vector2 position, Vector2 scale, Color color, int order, Transform parent)
        {
            return AddSprite(name, position, scale, color, order, parent, null);
        }

        private static GameObject AddSprite(string name, Vector2 position, Vector2 scale, Color color, int order, Transform parent, string shape)
        {
            var host = new GameObject(name);
            host.transform.SetParent(parent, false);
            host.transform.localPosition = ToWorld(position, parent == null ? order * 0.01f : 0f);
            host.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            var renderer = host.AddComponent<SpriteRenderer>();
            renderer.sprite = ShapeSprite(shape);
            renderer.color = color;
            renderer.sortingOrder = order;
            return host;
        }

        private static Vector3 ToWorld(Vector2 position, float z)
        {
            return new Vector3(position.x, position.y, z);
        }

        private static Sprite SolidSprite()
        {
            if (_solidSprite != null) return _solidSprite;
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false) { name = "LGO 2D Solid Pixel" };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply(false, true);
            _solidSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            _solidSprite.name = "LGO 2D Solid Sprite";
            return _solidSprite;
        }

        private static Sprite ShapeSprite(string shape)
        {
            if (string.IsNullOrWhiteSpace(shape) || shape == "rect") return SolidSprite();
            if (shape != "ellipse" && shape != "diamond" && shape != "tapered") return SolidSprite();
            if (_shapeSprites.TryGetValue(shape, out var cached) && cached != null) return cached;

            const int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "LGO 2D Shape " + shape, wrapMode = TextureWrapMode.Clamp };
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var nx = (x + 0.5f) / size * 2f - 1f;
                    var ny = (y + 0.5f) / size * 2f - 1f;
                    var fill = false;
                    switch (shape)
                    {
                        case "ellipse":
                            fill = nx * nx + ny * ny <= 1f;
                            break;
                        case "diamond":
                            fill = Mathf.Abs(nx) + Mathf.Abs(ny) <= 1f;
                            break;
                        case "tapered":
                            var halfWidth = 0.34f + (1f - ny) * 0.24f;
                            fill = Mathf.Abs(nx) <= halfWidth && Mathf.Abs(ny) <= 1f;
                            break;
                    }
                    texture.SetPixel(x, y, fill ? Color.white : Color.clear);
                }
            }
            texture.Apply(false, true);
            var sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect);
            sprite.name = "LGO 2D Shape Sprite " + shape;
            _shapeSprites[shape] = sprite;
            return sprite;
        }

        private static string DescribeAction(TwoDOnboardingAction action)
        {
            switch (action)
            {
                case TwoDOnboardingAction.Talk: return "Trò chuyện";
                case TwoDOnboardingAction.Continue: return "Tiếp tục";
                case TwoDOnboardingAction.Train: return "Cộng hưởng Bia Luyện Khí";
                case TwoDOnboardingAction.Jump: return "Nhảy";
                case TwoDOnboardingAction.Dash: return "Dash";
                case TwoDOnboardingAction.Skill: return "Kỹ năng Võ Lv1";
                default: return "Không có";
            }
        }
    }
}
