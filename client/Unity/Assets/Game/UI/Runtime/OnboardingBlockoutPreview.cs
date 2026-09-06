using System;
using System.Collections;
using System.IO;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed class OnboardingBlockoutPreview : MonoBehaviour
    {
        private OnboardingBlockoutWorld _world;
        private RuntimeTouchMovementPad _pad;
        private bool _capturing;
        private readonly NpcDialogueSession _session = new NpcDialogueSession("Người Giữ Cổng", new[]
        {
            "Chào mừng đến Linh Môn. Hãy dừng chân, ổn định hơi thở trước khi vào thành.",
            "Đá Luyện ở bên phải đường phía trước. Đến gần bên đá rồi tập trung linh khí.",
            "Đây là bước làm quen với linh khí, không phải giao chiến. Khi đã sẵn sàng, hãy thử một lần."
        });
        private RuntimeNpcDialogueView _dialogue;
        private Button _interact;
        private Action _applyLayout;
        private bool _stoneCompleted;

        private bool InRange => Vector2.Distance(new Vector2(_world.Position.x, _world.Position.z),
            _session.Completed ? new Vector2(OnboardingBlockoutWorld.StonePoint.x, OnboardingBlockoutWorld.StonePoint.z)
                : new Vector2(OnboardingBlockoutWorld.KeeperPoint.x, OnboardingBlockoutWorld.KeeperPoint.z)) <= 1.45f;

        public static bool ShouldRun(string[] args, bool development) =>
            development && Array.IndexOf(args, "--lgo-onboarding-blockout") >= 0;

        private void Awake()
        {
            _world = gameObject.AddComponent<OnboardingBlockoutWorld>();
            var document = gameObject.AddComponent<UIDocument>();
            document.panelSettings = RuntimePanelSettingsProvider.LoadOrCreate();
            var root = document.rootVisualElement;
            RuntimeUiTypography.ApplyBodyFont(root);
            var overlay = RuntimeUiFactory.NewWorldTouchControlsOverlay();
            _pad = (RuntimeTouchMovementPad)RuntimeUiFactory.NewWorldTouchPad();
            var cluster = RuntimeUiFactory.NewWorldTouchActionCluster();
            var quit = RuntimeUiFactory.NewWorldTouchActionButton("Thoát", () => Application.Quit());
            _interact = RuntimeUiFactory.NewWorldTouchActionButton("Gặp", Interact);
            cluster.Add(_interact);
            overlay.Add(_pad);
            overlay.Add(cluster);
            overlay.Add(quit);
            root.Add(overlay);
            _dialogue = new RuntimeNpcDialogueView(RuntimeUiLayoutProfile.FromScreen(null, Screen.width, Screen.height),
                () => { _session.Advance(); RefreshDialogue(); },
                () => { _session.Close(); RefreshDialogue(); });
            root.Add(_dialogue.Panel);
            _applyLayout = () =>
            {
                var layout = RuntimeUiLayoutProfile.FromScreen(null, Screen.width, Screen.height,
                    Mathf.RoundToInt(root.contentRect.width), Mathf.RoundToInt(root.contentRect.height));
                RuntimeWorldHudResponsiveLayout.ApplyTouchAffordances(layout, true, false, _session.Active,
                    overlay, _pad, cluster, _interact, null, null, null, quit);
                _dialogue.ApplyLayout(layout);
            };
            root.RegisterCallback<GeometryChangedEvent>(_ => _applyLayout());
            _applyLayout();
        }

        private void Interact()
        {
            if (_session.Active || _stoneCompleted || !InRange) return;
            if (_session.Completed) _stoneCompleted = true;
            else _session.Open();
            RefreshDialogue();
        }

        private void RefreshDialogue()
        {
            _world.Movement = Vector2.zero;
            _pad.ResetInput();
            _world.DialogueVisible = _session.Active;
            _dialogue.Refresh(_session);
            _applyLayout();
        }

        private IEnumerator Start()
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-blockout-evidence-dir");
            if (index < 0 || index + 1 >= args.Length) yield break;
            _capturing = true;
            var directory = Path.GetFullPath(args[index + 1]);
            Directory.CreateDirectory(directory);
            yield return new WaitForSeconds(1f);
            yield return Capture(directory, "arrival");
            Interact();
            if (_session.Active) throw new InvalidOperationException("NPC opened outside interaction range.");
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            yield return Capture(directory, "keeper-side");
            Submit(_interact);
            if (!_session.Active) throw new InvalidOperationException("NPC action did not open shared dialogue.");
            yield return Capture(directory, "dialogue");
            var bounds = _dialogue.Panel.worldBound;
            var viewport = _dialogue.Panel.parent.worldBound;
            var top = bounds.yMin - viewport.yMin;
            var bottom = viewport.yMax - bounds.yMax;
            if (top <= 0f || bottom <= 0f || Mathf.Abs(top - bottom) > 2f ||
                bounds.xMin < viewport.xMin || bounds.xMax > viewport.xMax ||
                _dialogue.Footer.worldBound.yMax > bounds.yMax ||
                _dialogue.CloseButton.worldBound.xMax > bounds.xMax)
                throw new InvalidOperationException("Shared NPC modal escaped its centered viewport bounds.");
            var before = _world.Position;
            _world.Movement = Vector2.one;
            yield return new WaitForSeconds(0.3f);
            if (Vector2.Distance(new Vector2(before.x, before.z), new Vector2(_world.Position.x, _world.Position.z)) > 0.01f)
                throw new InvalidOperationException("Dialogue failed to hold movement.");
            Submit(_dialogue.ContinueButton);
            Submit(_dialogue.CloseButton);
            if (_session.Active || _session.Completed) throw new InvalidOperationException("Closing dialogue advanced onboarding.");
            yield return null;
            Submit(_interact);
            if (_session.Progress != "1/3") throw new InvalidOperationException("Reopening did not restart dialogue.");
            Submit(_dialogue.ContinueButton);
            Submit(_dialogue.ContinueButton);
            if (_session.Completed) throw new InvalidOperationException("Dialogue completed before final action.");
            Submit(_dialogue.ContinueButton);
            if (!_session.Completed || _session.Active) throw new InvalidOperationException("Dialogue did not complete.");
            Interact();
            if (_stoneCompleted) throw new InvalidOperationException("Stone completed from NPC location.");
            yield return WalkTo(new Vector3(0f, 0f, 2.5f));
            yield return WalkTo(new Vector3(2.3f, 0f, 4f));
            yield return Capture(directory, "stone-side");
            Submit(_interact);
            if (!_stoneCompleted) throw new InvalidOperationException("Stone action did not complete onboarding.");
            yield return null;
            yield return Capture(directory, "complete");
            if (GetComponent<M4PlayableClientController>() != null)
                throw new InvalidOperationException("Blockout must not create the account client UI.");
            Debug.Log("LGO_ONBOARDING_BLOCKOUT_ROUTE_PASS movement=CharacterController no_teleport=true");
            Debug.Log("LGO_ONBOARDING_NPC_FLOW_PASS shared_session=true shared_modal=true cancel_reopen=true range=true input_lock=true");
            Application.Quit(0);
        }

        private IEnumerator WalkTo(Vector3 point)
        {
            var deadline = Time.realtimeSinceStartup + 10f;
            while (Vector2.Distance(new Vector2(_world.Position.x, _world.Position.z), new Vector2(point.x, point.z)) > 0.08f)
            {
                if (Time.realtimeSinceStartup > deadline)
                    throw new InvalidOperationException("Blockout route blocked or unfocused before " + point);
                var direction = point - _world.Position;
                _world.Movement = Vector2.ClampMagnitude(new Vector2(direction.x, direction.z) * 2f, 1f);
                yield return null;
            }
            _world.Movement = Vector2.zero;
        }

        private static IEnumerator Capture(string directory, string name)
        {
            yield return new WaitForEndOfFrame();
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            try
            {
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                if (RuntimePngWriter.WriteRgbTexture(Path.Combine(directory, name + ".png"), texture) <= 0)
                    throw new InvalidOperationException("Blockout screenshot was not written.");
            }
            finally { Destroy(texture); }
        }

        private void Update()
        {
            _interact.text = _stoneCompleted ? "Đã xong" : _session.Completed ? "Luyện" : "Gặp";
            _interact.SetEnabled(!_stoneCompleted && !_session.Active && InRange);
            if (!_capturing) _world.Movement = Application.isFocused ? _pad.Value : Vector2.zero;
            if (Input.GetKeyDown(KeyCode.F)) Interact();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_session.Active) { _session.Close(); RefreshDialogue(); }
                else Application.Quit();
            }
        }

        private static void Submit(Button button)
        {
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = button;
                button.SendEvent(submit);
            }
        }

        private void OnApplicationFocus(bool focused)
        {
            _pad?.ResetInput();
            if (_world != null) _world.Movement = Vector2.zero;
        }
    }
}
