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
        private RuntimeWorldGuidanceView _guidance;
        private ScrollView _guidanceScroll;
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
            _guidance = new RuntimeWorldGuidanceView();
            _guidance.Area.style.display = DisplayStyle.None;
            _guidance.Step.style.display = DisplayStyle.None;
            _guidance.Direction.style.display = DisplayStyle.None;
            _guidanceScroll = new ScrollView(ScrollViewMode.Vertical) { name = "LGO Standalone Guidance" };
            _guidanceScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            _guidanceScroll.Add(_guidance.Panel);
            root.Add(_guidanceScroll);
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
                _guidance.ApplyTypography(layout);
                RuntimeWorldHudResponsiveLayout.ApplyStandaloneGuidance(layout, RuntimeViewportMetrics.FromRoot(root), _guidanceScroll);
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
            _world.ResetMovementInput();
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
            var objective = GetComponent<UIDocument>().rootVisualElement.Q<Label>("LGO World Objective Touch Priority");
            if (objective == null || !objective.text.Contains("Người Giữ Cổng"))
            {
                Debug.LogError("LGO_BLOCKOUT_GUIDANCE_FAIL missing arrival objective");
                Application.Quit(1);
                yield break;
            }
            Interact();
            if (_session.Active) throw new InvalidOperationException("NPC opened outside interaction range.");
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            yield return Capture(directory, "keeper-side");
            Submit(_interact);
            if (!_session.Active) throw new InvalidOperationException("NPC action did not open shared dialogue.");
            yield return Capture(directory, "dialogue");
            if (!CheckGuidance("Gặp Người Giữ Cổng.", false)) yield break;
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
            if (!CheckGuidance("Chạm Đá Luyện.", true)) yield break;
            var focus = _world.transform.Find("Blockout Stone Focus");
            if (focus == null || !focus.gameObject.activeSelf)
            {
                Debug.LogError("LGO_STONE_FEEDBACK_FAIL no focus on interactable stone");
                Application.Quit(1);
                yield break;
            }
            Submit(_interact);
            if (!_stoneCompleted) throw new InvalidOperationException("Stone action did not complete onboarding.");
            yield return null;
            yield return Capture(directory, "complete");
            yield return new WaitForSeconds(1.3f);
            if (focus.gameObject.activeSelf)
            {
                Debug.LogError("LGO_STONE_FEEDBACK_FAIL completion pulse did not end");
                Application.Quit(1);
                yield break;
            }
            yield return Capture(directory, "complete-settled");
            if (!CheckGuidance("Linh khí đã ổn định.", true)) yield break;
            var stoneLabel = GameObject.Find("Blockout Stone Label").GetComponent<TextMesh>();
            var shadow = stoneLabel.transform.Find("Blockout Stone Label Shadow").GetComponent<TextMesh>();
            if (stoneLabel.text != shadow.text || !stoneLabel.text.Contains("Đã ổn định"))
            {
                Debug.LogError("LGO_STONE_FEEDBACK_FAIL completion label and shadow differ");
                Application.Quit(1);
                yield break;
            }
            Debug.Log("LGO_STONE_FEEDBACK_PASS focus=true completion_pulse_finished=true");
            yield return WalkTo(new Vector3(2.3f, 0f, 3f));
            yield return WalkTo(new Vector3(6f, 0f, 3f));
            yield return Capture(directory, "camera-alley");
            if (Physics.Linecast(_world.Position + Vector3.up * 1.1f, Camera.main.transform.position, out var obstruction) ||
                Physics.Linecast(_world.Position + Vector3.up * 0.2f, Camera.main.transform.position, out obstruction) ||
                Physics.Linecast(_world.Position + Vector3.up * 1.7f, Camera.main.transform.position, out obstruction))
            {
                Debug.LogError("LGO_BLOCKOUT_CAMERA_FAIL player hidden by " + obstruction.collider.name);
                Application.Quit(1);
                yield break;
            }
            var feet = Camera.main.WorldToViewportPoint(_world.Position);
            var head = Camera.main.WorldToViewportPoint(_world.Position + Vector3.up * 1.8f);
            var height = head.y - feet.y;
            if (feet.z <= Camera.main.nearClipPlane || head.z <= Camera.main.nearClipPlane ||
                feet.x < 0.05f || feet.x > 0.95f || feet.y < 0.05f || head.y > 0.95f || height < 0.08f || height > 0.6f ||
                Vector3.Dot(Camera.main.transform.forward, Vector3.down) > 0.65f)
            {
                Debug.LogError("LGO_BLOCKOUT_CAMERA_FAIL player framing feet=" + feet + " head=" + head);
                Application.Quit(1);
                yield break;
            }
            Debug.Log("LGO_BLOCKOUT_CAMERA_PASS alley_line_of_sight=true player_in_frame=true height=" + height);
            var movementStart = _world.Position;
            var screenForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            _world.ScreenMovement = Vector2.down;
            var inputDeadline = Time.realtimeSinceStartup + 3f;
            while (Vector3.Distance(_world.Position, movementStart) < 7.2f && Time.realtimeSinceStartup < inputDeadline)
                yield return null;
            _world.ScreenMovement = Vector2.zero;
            yield return null;
            yield return Capture(directory, "camera-exit");
            var travel = Vector3.ProjectOnPlane(_world.Position - movementStart, Vector3.up);
            if (Vector3.Dot(screenForward, Camera.main.transform.forward) > 0.95f)
            {
                Debug.LogError("LGO_CAMERA_INPUT_FAIL fixture did not cross a camera switch travel=" + travel + " camera=" + Camera.main.transform.forward);
                Application.Quit(1);
                yield break;
            }
            if (Vector3.Dot(travel, -screenForward) < 6.5f || Vector3.Cross(travel, screenForward).magnitude > 0.2f)
            {
                Debug.LogError("LGO_CAMERA_INPUT_FAIL held direction changed during camera switch travel=" + travel);
                Application.Quit(1);
                yield break;
            }
            Debug.Log("LGO_CAMERA_INPUT_PASS held_direction_preserved=true");
            movementStart = _world.Position;
            screenForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            _world.ScreenMovement = Vector2.up;
            inputDeadline = Time.realtimeSinceStartup + 1f;
            while (Vector3.Distance(_world.Position, movementStart) < 0.45f && Time.realtimeSinceStartup < inputDeadline)
                yield return null;
            _world.ScreenMovement = Vector2.zero;
            yield return null;
            travel = Vector3.ProjectOnPlane(_world.Position - movementStart, Vector3.up);
            if (Vector3.Dot(travel, screenForward) < 0.35f || Vector3.Cross(travel, screenForward).magnitude > 0.1f)
            {
                Debug.LogError("LGO_CAMERA_INPUT_FAIL fresh input did not use new camera basis travel=" + travel
                    + " basis=" + screenForward + " focused=" + Application.isFocused);
                Application.Quit(1);
                yield break;
            }
            Debug.Log("LGO_CAMERA_INPUT_PASS fresh_input_uses_new_camera=true");
            var stoneVolume = GameObject.Find("Blockout Stone").GetComponent<MeshFilter>();
            if (stoneVolume == null || stoneVolume.sharedMesh.bounds.size.z < 0.4f)
            {
                Debug.LogError("LGO_STONE_VOLUME_FAIL stone has no readable side volume");
                Application.Quit(1);
                yield break;
            }
            yield return WalkTo(new Vector3(0f, 0f, 3f));
            foreach (var side in new[] { 1f, -1f })
            {
                foreach (var edge in new[] { 2.75f, 3.25f })
                {
                    yield return WalkTo(new Vector3(side * 3.4f, 0f, edge));
                    yield return WalkTo(new Vector3(side * 6f, 0f, edge));
                    var checkpoint = "camera-edge-" + (side > 0f ? "right" : "left") + "-" + (edge < 3f ? "near" : "far");
                    yield return Capture(directory, checkpoint);
                    foreach (var elevation in new[] { 0.2f, 1.1f, 1.7f })
                    {
                        var target = _world.Position + Vector3.up * elevation;
                        var projected = Camera.main.WorldToViewportPoint(target);
                        if (Physics.Linecast(target, Camera.main.transform.position, out var hit, ~0, QueryTriggerInteraction.Ignore) ||
                            projected.z <= Camera.main.nearClipPlane || projected.x < 0.05f || projected.x > 0.95f ||
                            projected.y < 0.05f || projected.y > 0.95f)
                        {
                            Debug.LogError("LGO_CAMERA_EDGE_FAIL " + checkpoint + " elevation=" + elevation + " viewport=" + projected
                                + " obstruction=" + (hit.collider == null ? "none" : hit.collider.name));
                            Application.Quit(1);
                            yield break;
                        }
                    }
                    Debug.Log("LGO_CAMERA_EDGE_PASS " + checkpoint);
                    yield return WalkTo(new Vector3(side * 3.4f, 0f, edge));
                    yield return WalkTo(new Vector3(0f, 0f, 3f));
                }
            }
            if (GetComponent<M4PlayableClientController>() != null)
                throw new InvalidOperationException("Blockout must not create the account client UI.");
            Debug.Log("LGO_ONBOARDING_BLOCKOUT_ROUTE_PASS movement=CharacterController no_teleport=true");
            Debug.Log("LGO_ONBOARDING_NPC_FLOW_PASS shared_session=true shared_modal=true cancel_reopen=true range=true input_lock=true");
            Application.Quit(0);
        }

        private bool CheckGuidance(string objective, bool visible)
        {
            var bounds = _guidanceScroll.worldBound;
            var root = GetComponent<UIDocument>().rootVisualElement;
            var safe = RuntimeViewportMetrics.FromRoot(root).SafePanelRect;
            var fits = bounds.width > 0f && bounds.height > 0f && bounds.xMin >= safe.xMin && bounds.xMax <= safe.xMax
                && bounds.yMin >= safe.yMin && bounds.yMax <= safe.yMax && bounds.height <= safe.height * 0.3f + 1f
                && !bounds.Overlaps(_pad.worldBound);
            if (_guidance.Objective.text == objective && (_guidanceScroll.resolvedStyle.display != DisplayStyle.None) == visible
                && (!visible || fits))
            {
                Debug.Log("LGO_BLOCKOUT_GUIDANCE_PASS " + objective + " visible=" + visible);
                return true;
            }
            Debug.LogError("LGO_BLOCKOUT_GUIDANCE_FAIL state or bounds objective=" + _guidance.Objective.text + " bounds=" + bounds);
            Application.Quit(1);
            return false;
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
            _guidanceScroll.style.display = _session.Active ? DisplayStyle.None : DisplayStyle.Flex;
            _guidance.Objective.text = _stoneCompleted ? "Linh khí đã ổn định." : _session.Completed ? "Chạm Đá Luyện." : "Gặp Người Giữ Cổng.";
            _guidance.Hint.text = _stoneCompleted ? "Đã hoàn tất bước làm quen tại Linh Môn."
                : _session.Completed ? (InRange ? "Chọn Luyện để tập trung linh khí." : "Đá ở bên phải đường phía trước.")
                : InRange ? "Chọn Gặp để trò chuyện." : "Theo đường đá đến Người Giữ Cổng.";
            _interact.text = _stoneCompleted ? "Đã xong" : _session.Completed ? "Luyện" : "Gặp";
            _interact.SetEnabled(!_stoneCompleted && !_session.Active && InRange);
            _world.SetStoneFeedback(_session.Completed && !_stoneCompleted && InRange, _stoneCompleted);
            if (!_capturing) _world.ScreenMovement = Application.isFocused ? _pad.Value : Vector2.zero;
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
            if (_world != null) _world.ResetMovementInput();
        }
    }
}
