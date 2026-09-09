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
        private Transform _player;
        private Transform _gateKeeper;
        private Transform _trainingStone;
        private Transform _pathGlow;
        private Transform _focusRing;
        private Transform _hudDialoguePanel;
        private TextMesh _hudTitle;
        private TextMesh _hudArea;
        private TextMesh _hudObjective;
        private TextMesh _hudHint;
        private TextMesh _hudAction;
        private TextMesh _hudDialogue;
        private TextMesh _hudFeedback;
        private Camera _camera;
        private string _worldHudSnapshot = string.Empty;

        public TwoDOnboardingState State => _state;
        public string WorldHudSnapshot => _worldHudSnapshot;
        public int WorldHudLineCount { get; private set; }

        public static TwoDOnboardingController Attach(GameObject host)
        {
            return host.GetComponent<TwoDOnboardingController>() ?? host.AddComponent<TwoDOnboardingController>();
        }

        private void Awake()
        {
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

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (_state.TryUseAction()) RefreshPresentation();
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

            AddSprite("LGO 2D Far Spirit Sky", new Vector2(0f, 0.35f), new Vector2(9.5f, 5.45f), new Color(0.05f, 0.12f, 0.22f), -20);
            AddSprite("LGO 2D Distant Wall", new Vector2(-1.65f, 0.45f), new Vector2(3.8f, 1.45f), new Color(0.08f, 0.19f, 0.30f), -18);
            AddSprite("LGO 2D Linh Thanh Gate Left Pillar", new Vector2(-2.6f, 0.18f), new Vector2(0.34f, 1.65f), new Color(0.11f, 0.27f, 0.38f), -16);
            AddSprite("LGO 2D Linh Thanh Gate Right Pillar", new Vector2(-0.75f, 0.18f), new Vector2(0.34f, 1.65f), new Color(0.11f, 0.27f, 0.38f), -16);
            AddSprite("LGO 2D Linh Thanh Gate Roof", new Vector2(-1.68f, 1.1f), new Vector2(2.25f, 0.26f), new Color(0.18f, 0.38f, 0.48f), -15);
            AddSprite("LGO 2D Gate Inner Glow", new Vector2(-1.68f, -0.2f), new Vector2(1.42f, 1.05f), new Color(0.07f, 0.14f, 0.23f), -14);
            AddSprite("LGO 2D Training Yard", new Vector2(0f, -1.52f), new Vector2(8.9f, 1.82f), new Color(0.10f, 0.15f, 0.18f), -12);
            AddSprite("LGO 2D Yard Front Shade", new Vector2(0f, -2.22f), new Vector2(8.9f, 0.46f), new Color(0.06f, 0.10f, 0.15f), -11);
            AddSprite("LGO 2D Jade Path", new Vector2(0.8f, -0.95f), new Vector2(5.9f, 0.20f), new Color(0.11f, 0.52f, 0.48f, 0.55f), -10);
            _pathGlow = AddSprite("LGO 2D Path Glow", new Vector2(0.85f, -0.95f), new Vector2(5.7f, 0.08f), new Color(0.16f, 0.86f, 0.78f, 0.78f), -9).transform;

            _gateKeeper = AddCharacter("LGO 2D Gate Keeper", TwoDOnboardingState.GateKeeperPosition, RuntimeArtCatalog.Gold, new Color(0.88f, 0.78f, 0.58f), new Color(0.12f, 0.09f, 0.07f), -2);
            _trainingStone = AddTrainingStone("LGO 2D Training Stone", TwoDOnboardingState.TrainingStonePosition, -2);
            _player = AddCharacter("LGO 2D Player", TwoDOnboardingState.PlayerStart, RuntimeArtCatalog.Text, RuntimeArtCatalog.Spirit, new Color(0.05f, 0.06f, 0.08f), 2);
            _focusRing = AddSprite("LGO 2D Focus Ring", TwoDOnboardingState.GateKeeperPosition + Vector2.down * 0.54f, new Vector2(1.45f, 0.16f), RuntimeArtCatalog.Spirit, 1).transform;
            BuildWorldHud();
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
            RefreshWorldHud();
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

            _hudDialoguePanel = AddSprite("LGO 2D Dialogue Panel", new Vector2(-1.5f, 1.48f), new Vector2(5.35f, 0.46f), new Color(0.04f, 0.10f, 0.15f, 0.94f), 60).transform;
            _hudDialogue = AddHudText("LGO 2D Dialogue Text", new Vector2(-3.92f, 1.58f), 0.026f, RuntimeArtCatalog.Gold, 70);
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

        private static Transform AddCharacter(string name, Vector2 position, Color robe, Color accent, Color hair, int order)
        {
            var root = new GameObject(name);
            root.transform.position = ToWorld(position, order * 0.01f);
            AddSprite(name + " Shadow", new Vector2(0f, -0.58f), new Vector2(0.72f, 0.12f), new Color(0f, 0f, 0f, 0.25f), order - 1, root.transform);
            AddSprite(name + " Robe", new Vector2(0f, -0.16f), new Vector2(0.38f, 0.66f), robe, order, root.transform);
            AddSprite(name + " Sash", new Vector2(0f, -0.2f), new Vector2(0.46f, 0.09f), accent, order + 1, root.transform);
            AddSprite(name + " Head", new Vector2(0f, 0.34f), new Vector2(0.30f, 0.30f), new Color(0.86f, 0.70f, 0.56f), order + 2, root.transform);
            AddSprite(name + " Hair", new Vector2(0f, 0.51f), new Vector2(0.38f, 0.16f), hair, order + 3, root.transform);
            AddSprite(name + " Left Arm", new Vector2(-0.28f, -0.12f), new Vector2(0.12f, 0.46f), robe, order - 1, root.transform);
            AddSprite(name + " Right Arm", new Vector2(0.28f, -0.12f), new Vector2(0.12f, 0.46f), robe, order - 1, root.transform);
            return root.transform;
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
                default: return "Không có";
            }
        }
    }
}
