using System.Collections.Generic;
using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class TwoDOnboardingController : MonoBehaviour
    {
        private const float MoveSpeed = 2.75f;
        private static Sprite _solidSprite;
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
        private Transform _pathGlow;
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
        private Camera _camera;
        private string _worldHudSnapshot = string.Empty;
        private string _runtimeAnimationSnapshot = string.Empty;
        private Vector2 _lastPresentedPlayerPosition = TwoDOnboardingState.PlayerStart;
        private int _presentationTick;
        private readonly List<string> _productionSceneBeats = new List<string>();

        public TwoDOnboardingState State => _state;
        public string WorldHudSnapshot => _worldHudSnapshot;
        public int WorldHudLineCount { get; private set; }
        public string ProductionSceneBeatSnapshot => string.Join("\n", _productionSceneBeats.ToArray());
        public int ProductionSceneBeatCount => _productionSceneBeats.Count;
        public string RuntimeMapSnapshot => _mapCatalog.RuntimeSnapshot;
        public string RuntimeCharacterBaseSnapshot => _characterBaseCatalog.Snapshot;
        public string RuntimeEquipmentSnapshot => _moduleCatalog.Snapshot + "\n" + EnsurePlayerLoadout().Snapshot;
        public string RuntimeAnimationSnapshot => _animationProfile.Snapshot + "\n" + _runtimeAnimationSnapshot;
        public string RuntimeCombatSnapshot => "CombatMicroSlice: ShadowSlimeVisible=" + _state.ShadowSlimeVisible + " ShadowSlimeDefeated=" + _state.ShadowSlimeDefeated + " step=" + _state.Step;
        public string RuntimeRouteProgressSnapshot => "RouteProgress: current=" + _state.CurrentRouteNodeId + " step=" + _state.Step + " action=" + _state.AvailableAction;

        public static TwoDOnboardingController Attach(GameObject host)
        {
            return host.GetComponent<TwoDOnboardingController>() ?? host.AddComponent<TwoDOnboardingController>();
        }

        private void Awake()
        {
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
                if (_state.TryUseAction()) RefreshPresentation();
            }
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
            AddSceneSprite("LGO 2D Jade Path", "Lối ngọc dẫn tới Bia Luyện Khí", new Vector2(0.8f, -0.95f), new Vector2(5.9f, 0.20f), new Color(0.11f, 0.52f, 0.48f, 0.55f), -10);
            _pathGlow = AddSceneSprite("LGO 2D Path Glow", "Lối ngọc phát sáng sau thoại", new Vector2(0.85f, -0.95f), new Vector2(5.7f, 0.08f), new Color(0.16f, 0.86f, 0.78f, 0.78f), -9).transform;
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
            _gateKeeper = AddCharacter("LGO 2D Gate Keeper", TwoDOnboardingState.GateKeeperPosition, RuntimeArtCatalog.Gold, new Color(0.88f, 0.78f, 0.58f), new Color(0.12f, 0.09f, 0.07f), -2);
            AddSceneBeat("Bia Luyện Khí - mục tiêu tương tác");
            _trainingStone = AddTrainingStone("LGO 2D Training Stone", TwoDOnboardingState.TrainingStonePosition, -2);
            AddWorldLabel("LGO 2D Stone Label", "BIA LUYỆN KHÍ", TwoDOnboardingState.TrainingStonePosition + new Vector2(-0.64f, 0.78f), 0.038f, RuntimeArtCatalog.Spirit, 2);
            AddSceneBeat("Shadow Slime - mục tiêu combat nhập môn");
            _shadowSlime = AddShadowSlime("LGO 2D Shadow Slime", new Vector2(3.55f, -1.16f), -1);
            _shadowSlimeLabel = AddWorldLabel("LGO 2D Shadow Slime Label", "SHADOW SLIME", new Vector2(3.55f, -0.34f), 0.034f, new Color(0.82f, 0.60f, 1f), 3).transform;
            AddKiemLv1PreviewRack(new Vector2(3.35f, -2.02f), 4);
            BuildRuntimeMapOverlay();
            AddSceneBeat("Nhân vật người chơi - tân thủ nhập thành");
            _player = AddCharacter("LGO 2D Player", TwoDOnboardingState.PlayerStart, RuntimeArtCatalog.Text, RuntimeArtCatalog.Spirit, new Color(0.05f, 0.06f, 0.08f), 2);
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
            if (_state.Step == TwoDOnboardingStep.Complete)
                loadout.ApplyVoLv1Starter(_moduleCatalog);
            var voApplied = _state.Step == TwoDOnboardingStep.Complete;
            if (_playerOuterShirtRenderer != null) _playerOuterShirtRenderer.color = voApplied ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Text;
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

            _runtimeAnimationSnapshot = new TwoDAnimationRuntimeState(state, state == "TrainingCompletePose" ? "vo_lv1_training_complete" : state == "ClassSkill" ? "vo_lv1_first_skill" : state == "Dash" ? "dash_stretch" : state == "Jump" ? "jump_lift" : state == "Walk" ? "stride_bob" : "breathing_idle", phase).Snapshot;
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
            SetHudText(_miniMapProgress, "Node: " + _state.CurrentRouteNodeId);
            RefreshPlayerEquipmentPresentation();
            RefreshPlayerAnimationPresentation();
            RefreshWorldHud();
        }

        private void BuildRuntimeMapOverlay()
        {
            AddSceneBeat("Minimap Đông Môn theo route A-Z");
            AddSprite("LGO 2D Mini Map Panel", new Vector2(3.05f, 2.14f), new Vector2(2.24f, 1.36f), new Color(0.03f, 0.08f, 0.13f, 0.92f), 55);
            AddWorldLabel("LGO 2D Mini Map Title", "BẢN ĐỒ", new Vector2(2.58f, 2.70f), 0.036f, RuntimeArtCatalog.Gold, 66);
            AddWorldLabel("LGO 2D Mini Map Hub", "Linh Thành", new Vector2(3.18f, 2.46f), 0.032f, RuntimeArtCatalog.Text, 66);
            AddWorldLabel("LGO 2D Mini Map Route", "Đông Môn → Bia → Jump → Dash → Slime", new Vector2(3.18f, 2.23f), 0.026f, RuntimeArtCatalog.Spirit, 66);
            AddWorldLabel("LGO 2D Mini Map Chapter", "Chapter 1: Vết Nứt Đông Môn", new Vector2(3.18f, 2.08f), 0.024f, RuntimeArtCatalog.Gold, 66);
            _miniMapProgress = AddWorldLabel("LGO 2D Mini Map Progress", "Node: spawn", new Vector2(3.18f, 1.93f), 0.024f, RuntimeArtCatalog.Text, 66);
            AddWorldLabel("LGO 2D Mini Map Layers", "Layers: Sky/Far/Mid/Near/Gameplay/FG", new Vector2(3.18f, 1.69f), 0.021f, new Color(0.73f, 0.87f, 0.88f), 66);
            AddWorldLabel("LGO 2D Base Label", "Base: Male/Female layered", new Vector2(3.18f, 1.55f), 0.022f, RuntimeArtCatalog.Gold, 66);
            AddWorldLabel("LGO 2D Equipment Label", "Gear: Võ/Kiếm Lv1 mix slots", new Vector2(3.18f, 1.42f), 0.021f, RuntimeArtCatalog.Spirit, 66);

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
            if (_state.DialogueOpen) lines.Add("Người Giữ Cổng: " + _state.DialogueLine);

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
            var host = new GameObject(name);
            host.transform.position = ToWorld(position, order * 0.01f);
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
            var host = new GameObject(name);
            host.transform.SetParent(parent, false);
            host.transform.localPosition = ToWorld(position, parent == null ? order * 0.01f : 0f);
            host.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            var renderer = host.AddComponent<SpriteRenderer>();
            renderer.sprite = SolidSprite();
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
