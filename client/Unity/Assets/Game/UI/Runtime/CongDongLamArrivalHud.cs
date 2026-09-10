using System;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    // Arrival UI follows Map01A reference 01/05/10; reuses shared touch and viewport policies.
    public sealed class CongDongLamArrivalHud : MonoBehaviour
    {
        private CongDongLamMap01AArtPreview _scene;
        private VisualElement _root, _safe, _dialogue;
        private Label _quest, _marker;
        private Button _talk, _outfit, _skill;
        private RuntimeTouchMovementPad _pad;
        private RuntimeViewportMetrics _metrics;
        private PanelSettings _ownedPanel;
        private bool _touch;
        public static void Attach(CongDongLamMap01AArtPreview scene)
        {
            var host = new GameObject("Map01A Arrival HUD");
            host.transform.SetParent(scene.transform, false);
            var hud = host.AddComponent<CongDongLamArrivalHud>();
            hud._scene = scene;
            var document = host.AddComponent<UIDocument>();
            hud._ownedPanel = Instantiate(RuntimePanelSettingsProvider.LoadOrCreate());
            hud._ownedPanel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            hud._ownedPanel.referenceResolution = new Vector2Int(1600, 900);
            hud._ownedPanel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            hud._ownedPanel.match = 1f;
            document.panelSettings = hud._ownedPanel;
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-map01a-device");
            hud._touch = Application.isMobilePlatform || (index >= 0 && index + 1 < args.Length && args[index + 1] != "pc");
            hud.Build(document.rootVisualElement);
        }
        private static void Box(VisualElement e)
        {
            e.style.backgroundColor = new Color(.025f, .065f, .10f, .9f);
            e.style.color = new Color(.95f, .91f, .78f);
            e.style.paddingLeft = e.style.paddingRight = 12;
            e.style.paddingTop = e.style.paddingBottom = 8;
            e.style.fontSize = 20;
        }
        private static void Place(VisualElement e, float? left, float? right, float? top, float? bottom)
        {
            e.style.position = Position.Absolute;
            if (left.HasValue) e.style.left = left.Value;
            if (right.HasValue) e.style.right = right.Value;
            if (top.HasValue) e.style.top = top.Value;
            if (bottom.HasValue) e.style.bottom = bottom.Value;
        }
        private void Build(VisualElement root)
        {
            _root = root;
            _root.style.flexGrow = 1;
            _root.pickingMode = PickingMode.Ignore;
            _safe = new VisualElement { pickingMode = PickingMode.Ignore };
            _root.Add(_safe);
            var title = new Label("CỔNG ĐÔNG LÂM\nKhu an toàn • Lv1–3");
            Box(title); Place(title, 12, null, 12, null); _safe.Add(title);
            _quest = new Label(); Box(_quest); Place(_quest, null, 12, 12, null);
            _quest.style.width = 260; _quest.style.whiteSpace = WhiteSpace.Normal; _safe.Add(_quest);
            _pad = new RuntimeTouchMovementPad(); Box(_pad); Place(_pad, 16, null, null, 16);
            _pad.style.width = _pad.style.height = 112;
            _pad.style.display = _touch ? DisplayStyle.Flex : DisplayStyle.None;
            _pad.style.borderTopLeftRadius = _pad.style.borderTopRightRadius = 52;
            _pad.style.borderBottomLeftRadius = _pad.style.borderBottomRightRadius = 52;
            var nub = new VisualElement { name = "LGO World Touch Movement Nub", pickingMode = PickingMode.Ignore };
            nub.style.width = nub.style.height = 36;
            nub.style.backgroundColor = new Color(.4f,.72f,.8f,.9f);
            nub.style.marginLeft = nub.style.marginTop = 22;
            _pad.Add(nub); _safe.Add(_pad);
            _talk = new Button(() => _scene.UseCurrentRouteAction()) { text = "Tương tác · E" };
            Box(_talk); Place(_talk, null, 16, null, 24); _talk.style.minHeight = _touch ? 64 : 48; _talk.style.minWidth = 170; _safe.Add(_talk);
            _outfit = new Button(() => _scene.CycleVoAvatarMode()) { text = "Trang bị Võ · C" };
            Box(_outfit); Place(_outfit, null, 16, null, _touch ? 98 : 80);
            _outfit.style.minHeight = _touch ? 56 : 42; _outfit.style.minWidth = 170; _safe.Add(_outfit);
            _skill = new Button(() => _scene.TriggerVoSkill()) { text = "Liệt Phong Kích · X" };
            Box(_skill); Place(_skill, null, _touch ? 202 : 200, null, 24);
            _skill.style.minHeight = _touch ? 64 : 48; _skill.style.minWidth = 190; _safe.Add(_skill);
            _dialogue = new VisualElement(); Box(_dialogue); Place(_dialogue, 142, 204, null, 20);
            _dialogue.Add(new Label("Hạ Vân"));
            var line = new Label(_scene.DialogueText); line.style.whiteSpace = WhiteSpace.Normal; _dialogue.Add(line);
            _safe.Add(_dialogue);
            _marker = new Label("!\nHạ Vân") { pickingMode = PickingMode.Ignore };
            _marker.style.position = Position.Absolute; _marker.style.color = new Color(1,.83f,.3f);
            _marker.style.width = 96; _marker.style.height = 56;
            _marker.style.fontSize = 18; _marker.style.unityTextAlign = TextAnchor.MiddleCenter;
            _root.Add(_marker);
            _root.RegisterCallback<GeometryChangedEvent>(_ => Layout());
            Layout();
        }
        private void Layout()
        {
            _metrics = RuntimeViewportMetrics.FromRoot(_root);
            var r = _metrics.SafePanelRect;
            Place(_safe, r.x, null, r.y, null); _safe.style.width = r.width; _safe.style.height = r.height;
            _quest.style.width = r.width < 900 ? 220 : 260;
            _dialogue.style.left = _touch ? 150 : 20;
            _talk.style.fontSize = _touch ? 20 : 18;
        }
        private void Update()
        {
            if (_scene == null || _root == null) return;
            var metrics = RuntimeViewportMetrics.FromRoot(_root);
            if (!_metrics.LayoutEquals(metrics)) Layout();
            if (!_scene.IsCapturing)
            {
                var keyboard = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                    - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                _scene.MoveOnLane(Mathf.Abs(_pad.Value.x) > .01f ? _pad.Value.x : keyboard, Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.E)) _scene.UseCurrentRouteAction();
                if (Input.GetKeyDown(KeyCode.C)) _scene.CycleVoAvatarMode();
                if (Input.GetKeyDown(KeyCode.X)) _scene.TriggerVoSkill();
            }
            _quest.text = _scene.HasMetHaVan
                ? "Khám phá Cổng Đông Lâm\n" + _scene.CurrentRouteNodeLabel
                    + (string.IsNullOrEmpty(_scene.LastInteractionMessage) ? "" : "\n" + _scene.LastInteractionMessage)
                : "Đường Hội Tụ\nNói chuyện với Hạ Vân.";
            _talk.SetEnabled(_scene.CanUseCurrentRouteAction);
            _talk.text = _scene.CurrentActionLabel + (_touch ? "" : " · E");
            _outfit.text = "Trang bị Võ: " + _scene.VoAvatarMode + (_touch ? "" : " · C");
            _skill.text = _scene.VoAvatarMotionState == "skill" ? "Đang thi triển..." : "Liệt Phong Kích" + (_touch ? "" : " · X");
            _dialogue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _marker.text = "!\n" + _scene.CurrentRouteNodeLabel;
            var camera = Camera.main;
            if (camera != null)
            {
                var v = camera.WorldToViewportPoint(_scene.CurrentInteractionPosition);
                _marker.style.left = v.x * _metrics.PanelWidth - 48;
                _marker.style.top = (1-v.y) * _metrics.PanelHeight - 40;
            }
        }
        private void OnDestroy() { if (_ownedPanel != null) Destroy(_ownedPanel); }
        private void OnDisable() { _pad?.ResetInput(); }
    }
}
