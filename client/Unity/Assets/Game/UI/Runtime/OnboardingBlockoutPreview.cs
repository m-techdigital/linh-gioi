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
        private readonly NpcDialogueSession _session = OnboardingDialogueContent.CreateGateKeeperSession();
        private readonly NpcDialogueSession _returnSession = OnboardingDialogueContent.CreateGateKeeperReturnSession();
        private RuntimeNpcDialogueView _dialogue;
        private RuntimeWorldGuidanceView _guidance;
        private ScrollView _guidanceScroll;
        private Button _interact;
        private Action _applyLayout;
        private bool _stoneCompleted;
        private float _stoneFeedbackStartedAt = float.NegativeInfinity;
        private bool _forecourtVisited;
        private float _forecourtArrivalUntil;
        private bool _locomotionVerified;
        private Action _returnToHall;
        private Button _quit;

        // Completed introduction remains progress; repeat conversations have their own lifecycle.
        private NpcDialogueSession Dialogue => _stoneCompleted ? _returnSession : _session;
        private bool InRange => IsNear(_session.Completed && !_stoneCompleted
            ? OnboardingBlockoutWorld.StonePoint : OnboardingBlockoutWorld.KeeperPoint);
        private bool IsNear(Vector3 point) => Vector2.Distance(new Vector2(_world.Position.x, _world.Position.z),
            new Vector2(point.x, point.z)) <= 1.45f;

        public static bool ShouldRun(string[] args, bool development) =>
            development && Array.IndexOf(args, "--lgo-onboarding-blockout") >= 0;

        public static bool ShouldEnterFromHall(string[] args, bool development) =>
            development && Array.IndexOf(args, "--lgo-technical-yard") < 0;

        internal void ConfigureHallReturn(string characterName, Action returnToHall)
        {
            _returnToHall = returnToHall;
            _quit.text = "Về sảnh";
            _world.SetPlayerName(characterName);
        }

        private void Leave()
        {
            _world.ResetMovementInput();
            _pad.ResetInput();
            if (_returnToHall != null) _returnToHall();
            else Application.Quit();
        }

        internal void HandleEscape()
        {
            if (Dialogue.Active) { Dialogue.Close(); RefreshDialogue(); }
            else Leave();
        }

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
            var quit = RuntimeUiFactory.NewWorldTouchActionButton("Thoát", Leave);
            _quit = quit;
            _interact = RuntimeUiFactory.NewWorldTouchActionButton("Gặp", Interact);
            cluster.Add(_interact);
            overlay.Add(_pad);
            overlay.Add(cluster);
            overlay.Add(quit);
            root.Add(overlay);
            _guidance = new RuntimeWorldGuidanceView();
            _guidance.Area.text = "Linh Môn";
            _guidance.Step.style.display = DisplayStyle.None;
            _guidance.Direction.style.display = DisplayStyle.None;
            _guidanceScroll = (ScrollView)RuntimeUiFactory.NewWorldHudRoot("LGO Standalone Guidance",
                RuntimeUiLayoutProfile.FromScreen(null, Screen.width, Screen.height).WorldHudMaxWidth(false));
            _guidanceScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            _guidanceScroll.Add(_guidance.Panel);
            root.Add(_guidanceScroll);
            _dialogue = new RuntimeNpcDialogueView(RuntimeUiLayoutProfile.FromScreen(null, Screen.width, Screen.height),
                AdvanceDialogue,
                () => { Dialogue.Close(); RefreshDialogue(); }, Resources.Load<Texture2D>("LGOGateKeeperPortrait"));
            root.Add(_dialogue.Panel);
            _applyLayout = () =>
            {
                var layout = RuntimeUiLayoutProfile.FromScreen(null, Screen.width, Screen.height,
                    Mathf.RoundToInt(root.contentRect.width), Mathf.RoundToInt(root.contentRect.height));
                RuntimeWorldHudResponsiveLayout.ApplyTouchAffordances(layout, true, false, Dialogue.Active,
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
            if (Dialogue.Active || !InRange) return;
            if (_stoneCompleted) Dialogue.Open();
            else if (_session.Completed)
            {
                _stoneCompleted = true;
                _stoneFeedbackStartedAt = Time.unscaledTime;
            }
            else _session.Open();
            RefreshDialogue();
        }

        private void AdvanceDialogue()
        {
            if (!Dialogue.Advance()) return;
            RefreshDialogue();
            // A completed return conversation points to its actual destination;
            // closing or reading an intermediate line must never trigger this.
            if (_stoneCompleted && _returnSession.Completed) _world.GuideToForecourt();
        }

        private bool _guideDirectionPresented;

        private void RefreshDialogue()
        {
            _world.ResetMovementInput();
            _pad.ResetInput();
            _world.DialogueVisible = Dialogue.Active;
            if (_session.Completed && !_guideDirectionPresented)
            {
                _guideDirectionPresented = true;
                _world.GuideToStone();
            }
            _dialogue.Refresh(Dialogue);
            _applyLayout();
        }

        private IEnumerator Start()
        {
            var args = Environment.GetCommandLineArgs();
            var playerQualityIndex = Array.IndexOf(args, "--lgo-player-quality-review");
            if (playerQualityIndex >= 0 && playerQualityIndex + 1 < args.Length)
            {
                _capturing = true;
                yield return new WaitForSeconds(1f);
                var playerQualityDirectory = Path.GetFullPath(args[playerQualityIndex + 1]);
                Directory.CreateDirectory(playerQualityDirectory);
                GetComponent<UIDocument>().rootVisualElement.style.visibility = Visibility.Hidden;
                var actor = GameObject.Find("Blockout player proxy").GetComponentInChildren<Animator>();
                var cameraReview = gameObject.AddComponent<CharacterQualityReviewCamera>();
                cameraReview.Configure(Camera.main, actor);
                foreach (var angle in new[] { 0f, 90f, 180f })
                {
                    cameraReview.Angle = angle;
                    yield return new WaitForSeconds(0.2f);
                    yield return Capture(playerQualityDirectory, "player-idle-" + angle);
                }
                var leg = actor.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                var initialLeg = leg.localRotation;
                var initialPosition = _world.Position;
                _world.Movement = Vector2.up;
                cameraReview.Angle = 0f;
                yield return new WaitForSeconds(0.5f);
                if (actor.GetFloat("Speed") < 0.2f || Vector3.Distance(initialPosition, _world.Position) < 0.1f ||
                    Quaternion.Angle(initialLeg, leg.localRotation) < 2f)
                    throw new InvalidOperationException("Player quality capture requires actual moving locomotion.");
                yield return Capture(playerQualityDirectory, "player-walk-front");
                cameraReview.Angle = 180f;
                yield return new WaitForSeconds(0.2f);
                yield return Capture(playerQualityDirectory, "player-walk-back");
                _world.Movement = Vector2.right;
                cameraReview.Angle = 90f;
                yield return new WaitForSeconds(0.3f);
                yield return Capture(playerQualityDirectory, "player-turn-side");
                _world.Movement = Vector2.zero;
                yield return new WaitForSeconds(0.5f);
                cameraReview.Angle = 0f;
                yield return Capture(playerQualityDirectory, "player-return-front");
                Debug.Log("LGO_PLAYER_QUALITY_CAPTURE_COMPLETE frames=7 runtime=true locomotion=true gameplay_camera=false");
                Application.Quit(0);
                yield break;
            }
            var qualityIndex = Array.IndexOf(args, "--lgo-character-quality-review");
            if (qualityIndex >= 0 && qualityIndex + 1 < args.Length)
            {
                _capturing = true;
                yield return new WaitForSeconds(1f);
                var qualityDirectory = Path.GetFullPath(args[qualityIndex + 1]);
                Directory.CreateDirectory(qualityDirectory);
                GetComponent<UIDocument>().rootVisualElement.style.visibility = Visibility.Hidden;
                var qualityKeeper = GameObject.Find("Blockout Keeper").GetComponent<Animator>();
                var review = gameObject.AddComponent<CharacterQualityReviewCamera>();
                review.Configure(Camera.main, qualityKeeper);
                foreach (var angle in new[] { 0f, 90f, 180f })
                {
                    review.Angle = angle;
                    yield return new WaitForSeconds(0.2f);
                    yield return Capture(qualityDirectory, "keeper-idle-" + angle);
                }
                review.Angle = 0f;
                _world.GuideToStone();
                yield return new WaitForSeconds(0.7f);
                if (!_world.KeeperGuiding) throw new InvalidOperationException("Keeper pointing pose was not active during quality review.");
                yield return Capture(qualityDirectory, "keeper-point-front");
                review.Angle = 180f;
                yield return new WaitForSeconds(0.2f);
                yield return Capture(qualityDirectory, "keeper-point-back");
                yield return new WaitForSeconds(2f);
                if (_world.KeeperGuiding) throw new InvalidOperationException("Keeper pointing pose did not finish during quality review.");
                yield return Capture(qualityDirectory, "keeper-return-back");
                // Arrival shares the retained half of the expanded body atlas.
                review.Configure(Camera.main, GameObject.Find("Blockout player proxy").GetComponentInChildren<Animator>());
                foreach (var angle in new[] { 0f, 180f })
                {
                    review.Angle = angle;
                    yield return new WaitForSeconds(0.2f);
                    yield return Capture(qualityDirectory, "player-shared-atlas-" + angle);
                }
                Debug.Log("LGO_CHARACTER_QUALITY_CAPTURE_COMPLETE frames=8 runtime=true gameplay_camera=false width="
                    + Screen.width + " height=" + Screen.height);
                Application.Quit(0);
                yield break;
            }
            var keeperVideoIndex = Array.IndexOf(args, "--lgo-keeper-motion-video");
            if (keeperVideoIndex >= 0 && keeperVideoIndex + 1 < args.Length)
            {
                _capturing = true;
                yield return new WaitForSeconds(1f);
                yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
                var frames = Path.GetFullPath(args[keeperVideoIndex + 1]);
                Directory.CreateDirectory(frames);
                GetComponent<UIDocument>().rootVisualElement.style.visibility = Visibility.Hidden;
                var previousRate = Time.captureFramerate;
                Time.captureFramerate = 24;
                for (var frame = 0; frame < 192; frame++)
                {
                    if (frame == 96) _world.GuideToStone();
                    yield return Capture(frames, frame.ToString("D4"));
                }
                Time.captureFramerate = previousRate;
                Debug.Log("LGO_KEEPER_VIDEO_FRAMES_COMPLETE frames=192 fps=24 seconds=8 runtime=true gameplay_camera=true");
                Application.Quit(0);
                yield break;
            }
            var videoIndex = Array.IndexOf(args, "--lgo-blockout-motion-video");
            if (videoIndex >= 0 && videoIndex + 1 < args.Length)
            {
                _capturing = true;
                yield return new WaitForSeconds(1f);
                var frames = Path.GetFullPath(args[videoIndex + 1]);
                Directory.CreateDirectory(frames);
                var previousRate = Time.captureFramerate;
                Time.captureFramerate = 24;
                for (var frame = 0; frame < 192; frame++)
                {
                    _world.Movement = frame >= 24 && frame < 168 ? Vector2.up : Vector2.zero;
                    yield return Capture(frames, frame.ToString("D4"));
                }
                Time.captureFramerate = previousRate;
                _world.Movement = Vector2.zero;
                Debug.Log("LGO_MOTION_VIDEO_FRAMES_COMPLETE frames=192 fps=24 movement_seconds=6");
                Application.Quit(0);
                yield break;
            }
            var index = Array.IndexOf(args, "--lgo-blockout-evidence-dir");
            if (index < 0 || index + 1 >= args.Length) yield break;
            _capturing = true;
            var arrivalProbe = Array.IndexOf(args, "--lgo-arrival-probe") >= 0;
            if (arrivalProbe)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 2;
            }
            var directory = Path.GetFullPath(args[index + 1]);
            Directory.CreateDirectory(directory);
            ConfigureHallReturn("WWWWWWWWWWWWWWWW", null);
            yield return new WaitForSeconds(1f);
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            foreach (var label in _guidance.Panel.Query<Label>().ToList())
                if (label.resolvedStyle.backgroundColor.a > 0f || label.resolvedStyle.borderLeftWidth > 0f)
                    throw new InvalidOperationException("Guidance lines must be unframed text inside the shared HUD root.");
            if (_guidanceScroll.resolvedStyle.backgroundColor.a <= 0f)
                throw new InvalidOperationException("Standalone guidance must use the shared readable HUD backdrop.");
            Debug.Log("LGO_GUIDANCE_SHARED_PRESENTATION_PASS unframed_lines=true readable_root=true");
            if (_guidanceScroll.verticalScroller.highValue > 1f)
                throw new InvalidOperationException("Default arrival guidance must fit without scrolling in this evidence viewport.");
            yield return Capture(directory, "arrival");
            var longGuidance = RuntimeUiFactory.NewWorldHudText(
                string.Join("\n", new string[40]).Replace("\n", "Nội dung kiểm tra cuộn\n"),
                LinhGioi.Art.RuntimeArtCatalog.Text, 16);
            _guidance.Panel.Add(longGuidance);
            yield return null;
            yield return new WaitForEndOfFrame();
            var guidanceSafe = RuntimeViewportMetrics.FromRoot(_guidanceScroll.panel.visualTree).SafePanelRect;
            if (_guidanceScroll.verticalScroller.highValue <= 0f ||
                _guidanceScroll.verticalScroller.highButton.resolvedStyle.display != DisplayStyle.None ||
                _guidanceScroll.worldBound.height > guidanceSafe.height * 0.3f + 1f ||
                _guidanceScroll.worldBound.yMax >= _pad.worldBound.yMin)
                throw new InvalidOperationException("Long guidance must stay height-bounded and scroll above the movement pad.");
            var guidanceTop = longGuidance.worldBound.yMin;
            _guidanceScroll.scrollOffset = new Vector2(0f, _guidanceScroll.verticalScroller.highValue);
            yield return null;
            yield return new WaitForEndOfFrame();
            if (_guidanceScroll.scrollOffset.y <= 0f || longGuidance.worldBound.yMin >= guidanceTop - 1f)
                throw new InvalidOperationException("Guidance scrollbar must move its real text content.");
            longGuidance.RemoveFromHierarchy();
            _guidanceScroll.scrollOffset = Vector2.zero;
            yield return null;
            yield return new WaitForEndOfFrame();
            Debug.Log("LGO_GUIDANCE_SCROLL_PASS long_text=true max_safe_height=0.3 pad_clear=true content_moves=true");
            var skyCamera = _world.GetComponentInChildren<Camera>();
            var sky = RenderSettings.skybox;
            Debug.Log("LGO_STREET_SKY_TRACE clear=" + skyCamera.clearFlags + " shader=" + (sky == null ? "none" : sky.shader.name));
            if (skyCamera.clearFlags != CameraClearFlags.Skybox || sky == null || !sky.shader.isSupported)
                throw new InvalidOperationException("Street daylight must render the scene's supported skybox, not a flat clear color.");
            Mesh lanternFrame = null;
            var lanternCount = 0;
            foreach (var filter in _world.GetComponentsInChildren<MeshFilter>())
            {
                if (filter.name != "Street lantern frame") continue;
                if (filter.sharedMesh == null || filter.sharedMesh.vertexCount == 0
                    || (lanternFrame != null && lanternFrame != filter.sharedMesh)
                    || filter.GetComponent<Collider>() != null)
                    throw new InvalidOperationException("Street lanterns need one shared visible frame mesh without collision.");
                lanternFrame = filter.sharedMesh;
                lanternCount++;
            }
            if (lanternCount != 6) throw new InvalidOperationException("Expected six street lanterns, got " + lanternCount);
            var sceneryCount = 0;
            foreach (var house in _world.GetComponentsInChildren<Transform>())
            {
                if (house.name != "Street scenery house") continue;
                sceneryCount++;
                if (house.GetComponentsInChildren<Collider>().Length != 0)
                    throw new InvalidOperationException("Distant scenery must not add walking collision.");
                foreach (var renderer in house.GetComponentsInChildren<MeshRenderer>())
                    if (renderer.bounds.min.z <= 27.1f)
                        throw new InvalidOperationException("Distant scenery must stay outside the garden wall.");
            }
            if (sceneryCount != 5) throw new InvalidOperationException("Expected five distant scenery houses, got " + sceneryCount);
            var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            var softShadowProperty = pipeline == null ? null : pipeline.GetType().GetProperty("supportsSoftShadows");
            if (softShadowProperty == null)
                throw new InvalidOperationException("Player needs a pipeline exposing its shadow support.");
            var softShadows = (bool)softShadowProperty.GetValue(pipeline);
            var daylight = _world.transform.Find("Blockout daylight").GetComponent<Light>();
            Debug.Log("LGO_SHADOW_CONFIGURATION pipeline=" + pipeline.name + " quality=" + QualitySettings.GetQualityLevel()
                + " requested=" + daylight.shadows + " supported=" + softShadows);
            if (daylight.shadows != LightShadows.Soft || !softShadows)
                throw new InvalidOperationException("Daylight needs both a soft-shadow light and pipeline support.");
            Mesh sharedFacade = null;
            var facadeCount = 0;
            foreach (var filter in _world.GetComponentsInChildren<MeshFilter>())
            {
                if (filter.name != "House facade") continue;
                facadeCount++;
                if (filter.sharedMesh == null || filter.sharedMesh.vertexCount == 0)
                    throw new InvalidOperationException("Every house facade needs visible mesh geometry.");
                if (sharedFacade != null && sharedFacade != filter.sharedMesh)
                    throw new InvalidOperationException("Street houses must reuse the same facade mesh.");
                sharedFacade = filter.sharedMesh;
                if (filter.GetComponent<Collider>() != null)
                    throw new InvalidOperationException("Facade decoration must not change the walking collision.");
            }
            if (facadeCount != 12 || sharedFacade == null)
                throw new InvalidOperationException("Expected seven nearby and five distant shared house facades, got " + facadeCount);
            var pavingSurface = _world.transform.Find("Street paving surface");
            if (pavingSurface == null)
                throw new InvalidOperationException("Street and forecourt need one continuous paving surface.");
            var pavingTexture = pavingSurface.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
            if (pavingTexture == null || pavingTexture.width != 256 || pavingTexture.height != 256
                || pavingTexture.mipmapCount < 2 || pavingTexture.wrapMode != TextureWrapMode.Repeat)
                throw new InvalidOperationException("Paving must use a small repeating texture with mipmaps.");
            var pavingMesh = pavingSurface.GetComponent<MeshFilter>().sharedMesh;
            if (Mathf.Abs(pavingMesh.bounds.size.x - 14f) > 0.01f || Mathf.Abs(pavingMesh.bounds.size.z - 39f) > 0.01f
                || Mathf.Abs((pavingMesh.uv[1].x - pavingMesh.uv[0].x) - 14f / 3f) > 0.01f
                || Mathf.Abs((pavingMesh.uv[2].y - pavingMesh.uv[0].y) - 39f / 2f) > 0.01f)
                throw new InvalidOperationException("Paving UVs must preserve the 3m x 2m repeat across the entire route.");
            var forecourtTree = _world.transform.Find("Forecourt pine").GetComponent<SpriteRenderer>();
            var soil = _world.transform.Find("Garden soil");
            var treeShadow = _world.transform.Find("Forecourt pine grounding");
            if (soil == null || treeShadow == null)
                throw new InvalidOperationException("Forecourt tree needs soil and a shared grounding shadow inside the bed.");
            if (soil.GetComponent<Collider>() != null || treeShadow.GetComponent<SpriteRenderer>()?.sprite == null
                || treeShadow.position.y <= soil.GetComponent<Renderer>().bounds.max.y
                || treeShadow.position.y >= forecourtTree.bounds.min.y)
                throw new InvalidOperationException("Tree contact shadow must sit above non-colliding soil and below the roots.");
            if (Mathf.Abs(forecourtTree.bounds.min.y - 0.325f) > 0.01f)
                throw new InvalidOperationException("Forecourt tree must stand on the raised bed: bottom=" + forecourtTree.bounds.min.y);
            var character = _world.transform.Find("Blockout player proxy").GetComponentInChildren<Animator>();
            var keeper = _world.transform.Find("Blockout Keeper");
            var keeperAnimator = keeper == null ? null : keeper.GetComponent<Animator>();
            if (keeperAnimator == null || !keeperAnimator.isHuman || keeperAnimator.applyRootMotion ||
                keeper.GetComponentsInChildren<SkinnedMeshRenderer>().Length != 1 || keeper.GetComponent<SpriteRenderer>() != null)
                throw new InvalidOperationException("Gate Keeper must be a separate Humanoid actor, not a flat sprite.");
            var keeperMesh = keeper.GetComponentInChildren<SkinnedMeshRenderer>();
            ulong keeperTriangles = 0;
            for (var submesh = 0; submesh < keeperMesh.sharedMesh.subMeshCount; submesh++)
                keeperTriangles += keeperMesh.sharedMesh.GetIndexCount(submesh) / 3;
            if (keeperTriangles == 0 || keeperMesh.sharedMaterials.Length != 2)
                throw new InvalidOperationException("Gate Keeper has invalid geometry or material sections.");
            for (var section = 0; section < keeperMesh.sharedMaterials.Length; section++)
            {
                var atlas = keeperMesh.sharedMaterials[section].GetTexture("_BaseMap");
                var atlasWidth = section == 0 ? 4096 : 1024;
                var atlasHeight = section == 0 ? 2048 : 512;
                if (atlas == null || atlas.width != atlasWidth || atlas.height != atlasHeight)
                    throw new InvalidOperationException("Gate Keeper is missing its native review body/face atlas: " + section);
            }
            Debug.Log("LGO_KEEPER_RECONSTRUCTION_BUDGET_PASS triangles=" + keeperTriangles + " materials=2 body_native=4096x2048 face_native=1024x512");
            var keeperSnapshot = new Mesh();
            var keeperGround = float.PositiveInfinity;
            try
            {
                keeperMesh.BakeMesh(keeperSnapshot, true);
                foreach (var vertex in keeperSnapshot.vertices)
                    keeperGround = Mathf.Min(keeperGround, keeperMesh.transform.TransformPoint(vertex).y);
            }
            finally { Destroy(keeperSnapshot); }
            if (keeperGround < -0.01f || keeperGround > 0.05f)
                throw new InvalidOperationException("Gate Keeper feet must rest near the ground: " + keeperGround);
            Debug.Log("LGO_KEEPER_GROUND baked_min=" + keeperGround + " culling_min=" + keeperMesh.bounds.min.y);
            var keeperHead = keeperAnimator.GetBoneTransform(HumanBodyBones.Head);
            var keeperPose = keeperHead.localRotation;
            var keeperStart = keeper.position;
            var keeperRest = keeper.rotation;
            yield return new WaitForSeconds(0.7f);
            if (Quaternion.Angle(keeperPose, keeperHead.localRotation) < 0.05f || Vector3.Distance(keeperStart, keeper.position) > 0.001f)
                throw new InvalidOperationException("Gate Keeper idle must animate without moving its root.");
            Debug.Log("LGO_KEEPER_HUMANOID_PASS idle=true root_held=true mesh_budget=true");
            if (character == null || !character.isHuman || character.runtimeAnimatorController == null || character.applyRootMotion)
                throw new InvalidOperationException("Blockout requires a Humanoid character with in-place locomotion.");
            _world.LogCameraShots();
            if (Vector3.Dot(Camera.main.transform.forward, Vector3.forward) < 0.9f)
            {
                Debug.LogError("LGO_ARRIVAL_CAMERA_FAIL expected street-facing composition");
                Application.Quit(1);
                yield break;
            }
            if (arrivalProbe)
            {
                Application.Quit(0);
                yield break;
            }
            var objective = GetComponent<UIDocument>().rootVisualElement.Q<Label>("LGO World Objective Touch Priority");
            if (objective == null || !objective.text.Contains("Người Giữ Cổng"))
            {
                Debug.LogError("LGO_BLOCKOUT_GUIDANCE_FAIL missing arrival objective");
                Application.Quit(1);
                yield break;
            }
            Interact();
            if (_session.Active) throw new InvalidOperationException("NPC opened outside interaction range.");
            CheckInteractionIcon("Gặp", "ActionTalk", false);
            CheckKeeperFocus(false);
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            CheckKeeperFocus(true);
            yield return WalkTo(new Vector3(0f, 0f, 1f));
            CheckKeeperFocus(false);
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            CheckKeeperFocus(true);
            yield return Capture(directory, "keeper-side");
            yield return new WaitForSeconds(0.25f);
            if (!KeeperConversing(keeperAnimator) || _world.DialogueVisible)
                throw new InvalidOperationException("Guide approach must show a brief greeting gesture before any modal covers the NPC.");
            if (!_world.KeeperGreeting || !_guidance.Hint.text.Contains(OnboardingDialogueContent.GateKeeperWelcome))
                throw new InvalidOperationException("Approach greeting needs synchronized welcome text in the shared guidance.");
            yield return Capture(directory, "keeper-greeting");
            var greetingOrigin = _world.Position;
            _world.Movement = Vector2.up;
            yield return new WaitForSeconds(0.12f);
            _world.Movement = Vector2.zero;
            if (Vector3.Distance(greetingOrigin, _world.Position) < 0.08f ||
                !KeeperConversing(keeperAnimator) || _world.DialogueVisible)
                throw new InvalidOperationException("Player must move while the greeting is still active, not after it expires. " +
                    "distance=" + Vector3.Distance(greetingOrigin, _world.Position).ToString("R") +
                    " conversing=" + KeeperConversing(keeperAnimator) + " focused=" + Application.isFocused +
                    " modal=" + _world.DialogueVisible + " frame_dt=" + Time.deltaTime.ToString("R"));
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            yield return new WaitForSeconds(3f);
            if (keeperAnimator.GetLayerWeight(keeperAnimator.GetLayerIndex("Conversation")) > 0.001f ||
                !keeperAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || _world.DialogueVisible)
                throw new InvalidOperationException("Approach gesture must end while the player stays nearby, without opening a modal.");
            if (_world.KeeperGreeting || _guidance.Hint.text.Contains(OnboardingDialogueContent.GateKeeperWelcome))
                throw new InvalidOperationException("Welcome text must expire with the greeting gesture.");
            yield return WalkTo(new Vector3(0f, 0f, 1f));
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            Debug.Log("LGO_KEEPER_GREETING_PASS visible_before_modal=true bounded=true movement_available=true");
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            CheckInteractionIcon("Gặp", "ActionTalk", true);
            using (var hover = MouseEnterEvent.GetPooled())
            {
                hover.target = _interact;
                _interact.SendEvent(hover);
            }
            yield return null;
            yield return Capture(directory, "interaction-tooltip");
            var tooltip = _interact.panel.visualTree.Q<Label>("LGO World Touch Tooltip");
            var safe = RuntimeViewportMetrics.FromRoot(_interact.panel.visualTree).SafePanelRect;
            if (tooltip == null || tooltip.text != "Gặp" || tooltip.worldBound.width <= 0f ||
                tooltip.worldBound.xMin < safe.xMin || tooltip.worldBound.xMax > safe.xMax ||
                tooltip.worldBound.yMin < safe.yMin || tooltip.worldBound.yMax > safe.yMax)
                throw new InvalidOperationException("Runtime hover must show the action name inside safe bounds.");
            using (var leave = MouseLeaveEvent.GetPooled())
            {
                leave.target = _interact;
                _interact.SendEvent(leave);
            }
            if (tooltip.parent != null) throw new InvalidOperationException("Tooltip persisted after hover ended.");
            Debug.Log("LGO_INTERACTION_TOOLTIP_PASS hover_event=true safe_bounds=true leave=true");
            Submit(_interact);
            if (!_session.Active) throw new InvalidOperationException("NPC action did not open shared dialogue.");
            yield return new WaitForSeconds(1f);
            if (Vector3.Angle(keeper.forward, Vector3.ProjectOnPlane(_world.Position - keeper.position, Vector3.up)) > 5f)
                throw new InvalidOperationException("Gate Keeper must face the player during dialogue.");
            Debug.Log("LGO_KEEPER_DIALOGUE_FACING_PASS target=true");
            if (!KeeperConversing(keeperAnimator))
                throw new InvalidOperationException("Guide dialogue must play its conversational gesture, not remain in locomotion idle.");
            if (character.GetLayerWeight(character.GetLayerIndex("Conversation")) > 0.001f)
                throw new InvalidOperationException("The shared conversation layer must remain disabled on the player.");
            var talkingHand = keeperAnimator.GetBoneTransform(HumanBodyBones.LeftHand).position;
            var talkingLeftFoot = keeperAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            var talkingRightFoot = keeperAnimator.GetBoneTransform(HumanBodyBones.RightFoot).position;
            yield return Capture(directory, "keeper-talking-start");
            var talkingDuration = keeperAnimator.GetCurrentAnimatorClipInfo(keeperAnimator.GetLayerIndex("Conversation"))[0].clip.length;
            var talkingUntil = Time.time + talkingDuration;
            var talkingHandMoved = false;
            var talkingSamples = 0;
            while (Time.time < talkingUntil)
            {
                yield return null;
                talkingSamples++;
                talkingHandMoved |= Vector3.Distance(talkingHand, keeperAnimator.GetBoneTransform(HumanBodyBones.LeftHand).position) >= 0.01f;
                if (Vector3.Distance(talkingLeftFoot, keeperAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position) > 0.02f ||
                    Vector3.Distance(talkingRightFoot, keeperAnimator.GetBoneTransform(HumanBodyBones.RightFoot).position) > 0.02f ||
                    Vector3.Distance(keeperStart, keeper.position) > 0.001f)
                    throw new InvalidOperationException("Both foot bones and the guide root must remain stable throughout the talking loop.");
            }
            if (!talkingHandMoved || talkingSamples < 10)
                throw new InvalidOperationException("Talking loop needs visible hand movement and sufficient observed frames.");
            Debug.Log("LGO_KEEPER_TALKING_SAMPLES frames=" + talkingSamples + " seconds=" + talkingDuration + " foot_bones=both");
            yield return Capture(directory, "keeper-talking-later");
            yield return Capture(directory, "dialogue");
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", false);
            CheckKeeperFocus(false);
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
            var facingKeeper = Vector3.ProjectOnPlane(OnboardingBlockoutWorld.KeeperPoint - _world.Position, Vector3.up);
            var dialogueFacingAngle = Vector3.Angle(character.transform.forward, facingKeeper);
            if (dialogueFacingAngle > 5f)
                throw new InvalidOperationException("Dialogue character must face the keeper: angle=" + dialogueFacingAngle);
            Debug.Log("LGO_DIALOGUE_FACING_PASS angle=" + dialogueFacingAngle);
            if (Vector2.Distance(new Vector2(before.x, before.z), new Vector2(_world.Position.x, _world.Position.z)) > 0.01f)
                throw new InvalidOperationException("Dialogue failed to hold movement.");
            Submit(_dialogue.ContinueButton);
            Submit(_dialogue.CloseButton);
            if (_session.Active || _session.Completed) throw new InvalidOperationException("Closing dialogue advanced onboarding.");
            yield return new WaitForSeconds(1f);
            if (Quaternion.Angle(keeperRest, keeper.rotation) > 5f || Vector3.Distance(keeperStart, keeper.position) > 0.001f)
                throw new InvalidOperationException("Gate Keeper must resume its resting direction without root movement after dialogue.");
            if (_dialogue.Portrait.resolvedStyle.backgroundImage.texture != Resources.Load<Texture2D>("LGOGateKeeperPortrait"))
                throw new InvalidOperationException("Dialogue must show the portrait rendered from the Keeper candidate.");
            Debug.Log("LGO_KEEPER_RETURN_PASS rest=true root_held=true matching_portrait=true");
            if (keeperAnimator.GetLayerWeight(keeperAnimator.GetLayerIndex("Conversation")) > 0.001f ||
                !keeperAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                throw new InvalidOperationException("Closing dialogue must return the guide to idle.");
            Debug.Log("LGO_KEEPER_TALKING_PASS animated=true feet_held=true root_held=true idle_restored=true");
            if (Array.IndexOf(args, "--lgo-keeper-presentation-only") >= 0)
            {
                Debug.Log("LGO_KEEPER_PRESENTATION_PASS scope=approach_dialogue_close full_route=false");
                Application.Quit(0);
                yield break;
            }
            yield return null;
            yield return new WaitForEndOfFrame();
            CheckKeeperFocus(true);
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            Debug.Log("LGO_PLAYER_IDENTITY_PASS long_name=true near_keeper=true dialogue_hide_restore=true");
            Submit(_interact);
            if (_session.Progress != "1/3") throw new InvalidOperationException("Reopening did not restart dialogue.");
            Submit(_dialogue.ContinueButton);
            Submit(_dialogue.ContinueButton);
            if (_session.Completed) throw new InvalidOperationException("Dialogue completed before final action.");
            yield return null;
            yield return new WaitForEndOfFrame();
            if (_dialogue.InformationButton.resolvedStyle.display == DisplayStyle.None ||
                _dialogue.InformationButton.text != "Tìm hiểu Linh Thành" || _dialogue.ContinueButton.text != "Đến Sân Luyện")
                throw new InvalidOperationException("Guide must offer city information and a clear next destination.");
            var informationBounds = _dialogue.InformationButton.worldBound;
            if (_dialogue.InformationButton.parent != _dialogue.Footer || informationBounds.height <= 0f ||
                informationBounds.yMin < _dialogue.Footer.worldBound.yMin ||
                informationBounds.yMax > _dialogue.ActionRow.worldBound.yMin + 1f ||
                informationBounds.xMin < _dialogue.Panel.worldBound.xMin ||
                informationBounds.xMax > _dialogue.Panel.worldBound.xMax ||
                _dialogue.Footer.worldBound.yMax > _dialogue.Panel.worldBound.yMax + 1f)
                throw new InvalidOperationException("City information action must stay fully visible in the fixed shared footer, never in body scroll.");
            yield return Capture(directory, "dialogue-choices");
            Submit(_dialogue.InformationButton);
            yield return null;
            yield return new WaitForEndOfFrame();
            if (!_session.ReadingInformation || _session.Completed || !_world.DialogueVisible ||
                _dialogue.ContinueButton.text != "Quay lại" || !_dialogue.Line.text.Contains("gặp gỡ"))
                throw new InvalidOperationException("Reading about the city must not complete onboarding or unlock movement.");
            yield return Capture(directory, "dialogue-city-information");
            _dialogue.Line.text = string.Join("\n", new string[24]).Replace("\n", "Linh Thành chào đón bạn.\n");
            yield return null;
            yield return new WaitForEndOfFrame();
            if (_dialogue.Scroll.verticalScroller.highValue <= 0f)
                throw new InvalidOperationException("Long information must scroll inside the shared dialogue body.");
            _dialogue.Scroll.scrollOffset = new Vector2(0, _dialogue.Scroll.verticalScroller.highValue);
            yield return null;
            if (_dialogue.Scroll.scrollOffset.y <= 0f || _dialogue.Footer.worldBound.yMax > _dialogue.Panel.worldBound.yMax + 1f)
                throw new InvalidOperationException("Information scrolling must preserve a contained, reachable footer.");
            _dialogue.Refresh(_session);
            Submit(_dialogue.ContinueButton);
            if (_session.Completed || _session.ReadingInformation || !_session.CanReadInformation)
                throw new InvalidOperationException("Returning from city information must restore the uncompleted destination choice.");
            Debug.Log("LGO_GUIDE_CITY_INFORMATION_PASS optional=true no_completion=true scroll=true return=true");
            Submit(_dialogue.ContinueButton);
            if (!_session.Completed || _session.Active) throw new InvalidOperationException("Dialogue did not complete.");
            yield return null;
            yield return new WaitForEndOfFrame();
            CheckKeeperFocus(false);
            Debug.Log("LGO_KEEPER_FOCUS_PASS proximity=true dialogue=true cancel=true completed=true shared_sprite=true");
            var guideAnimator = _world.transform.Find("Blockout Keeper").GetComponent<Animator>();
            var guideRoot = guideAnimator.transform.position;
            yield return new WaitForSeconds(0.8f);
            var guideShoulder = guideAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            var guideHand = guideAnimator.GetBoneTransform(HumanBodyBones.RightHand).position;
            var direction = Vector3.ProjectOnPlane(OnboardingBlockoutWorld.StonePoint - guideRoot, Vector3.up);
            var handDirection = Vector3.ProjectOnPlane(guideHand - guideShoulder, Vector3.up);
            if (Vector3.Angle(direction, handDirection) > 20f || handDirection.magnitude < 0.2f ||
                Vector3.Distance(guideRoot, guideAnimator.transform.position) > 0.001f || _world.DialogueVisible)
                throw new InvalidOperationException("Guide must indicate the stone with a raised hand without root motion or a blocking modal.");
            yield return Capture(directory, "keeper-guide-direction");
            yield return new WaitForSeconds(1.7f);
            if (guideAnimator.GetLayerWeight(guideAnimator.GetLayerIndex("Conversation")) > 0.01f)
                throw new InvalidOperationException("Guide direction must finish and restore base locomotion.");
            Debug.Log("LGO_GUIDE_DIRECTION_PASS target=stone root_motion=false modal=false completed=true");
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            Interact();
            if (_stoneCompleted) throw new InvalidOperationException("Stone completed from NPC location.");
            yield return WalkTo(new Vector3(0f, 0f, 2.5f));
            yield return WalkTo(new Vector3(2.3f, 0f, 4f));
            yield return Capture(directory, "stone-side");
            var stoneMesh = _world.transform.Find("Blockout Stone").GetComponent<MeshFilter>().sharedMesh;
            var stoneCollider = _world.transform.Find("Blockout Stone").GetComponent<BoxCollider>();
            if (stoneMesh.bounds.size.y > 1.15f || stoneMesh.bounds.size.z < 0.55f ||
                stoneMesh.triangles.Length / 3 > 300 ||
                stoneCollider.size != new Vector3(0.65f, 1.5f, 0.65f))
                throw new InvalidOperationException("Training stone must be a low broad volume within its unchanged collision footprint.");
            var frontSeal = _world.transform.Find("Blockout stone seal front");
            var sideSeal = _world.transform.Find("Blockout stone seal side");
            if (frontSeal == null || sideSeal == null || frontSeal.GetComponent<Collider>() != null || sideSeal.GetComponent<Collider>() != null ||
                frontSeal.GetComponent<MeshFilter>().sharedMesh != sideSeal.GetComponent<MeshFilter>().sharedMesh ||
                frontSeal.GetComponent<Renderer>().sharedMaterial != sideSeal.GetComponent<Renderer>().sharedMaterial ||
                sideSeal.position.x >= OnboardingBlockoutWorld.StonePoint.x - 0.2f)
                throw new InvalidOperationException("Stone needs shared seal geometry/material on its arrival and road faces without extra collision.");
            var inlayMesh = frontSeal.GetComponent<MeshFilter>().sharedMesh;
            if (Mathf.Abs(inlayMesh.bounds.size.x - 0.20f) > 0.001f || inlayMesh.bounds.center.sqrMagnitude > 0.000001f ||
                inlayMesh.triangles.Length / 3 > 160 || Array.Exists(inlayMesh.normals, normal => normal.z < 0.99f))
                throw new InvalidOperationException("Stone resonance inlay must face outward and remain compact within the shared mesh budget.");
            Debug.Log("LGO_STONE_INLAY_PASS shared_mesh=true outward=true compact=true");
            Debug.Log("LGO_STONE_SILHOUETTE_PASS low_volume=true collider_unchanged=true shared_seals=true");
            var sealMaterial = frontSeal.GetComponent<Renderer>().sharedMaterial;
            var sealRestColor = sealMaterial.color;
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            CheckInteractionIcon("Luyện", "ActionTouch", true);
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
            if (Array.IndexOf(args, "--lgo-gesture-early-input") >= 0)
            {
                var earlyStart = _world.Position;
                // Schedule input before Animator evaluates the newly requested crossfade.
                _world.SetStoneFeedback(false, true);
                Debug.Log("LGO_STONE_GESTURE_QUEUE_TRACE current=" + character.GetCurrentAnimatorStateInfo(0).fullPathHash
                    + " next=" + character.GetNextAnimatorStateInfo(0).fullPathHash + " transition=" + character.IsInTransition(0));
                _world.Movement = Vector2.left;
                yield return new WaitForSeconds(0.3f);
                _world.Movement = Vector2.zero;
                if (!character.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion") ||
                    Vector3.Distance(earlyStart, _world.Position) < 0.5f)
                    throw new InvalidOperationException("Input before Animator evaluation failed: state=" + character.GetCurrentAnimatorStateInfo(0).fullPathHash
                        + " expected=" + Animator.StringToHash("Base Layer.Locomotion") + " distance=" + Vector3.Distance(earlyStart, _world.Position)
                        + " focused=" + Application.isFocused);
                yield return Capture(directory, "gesture-early-input");
                Debug.Log("LGO_STONE_GESTURE_EARLY_INPUT_PASS queued_cancel=true movement=true");
                Application.Quit(0);
                yield break;
            }
            var stoneInteractionPosition = _world.Position;
            yield return new WaitForSeconds(0.3f);
            var stoneFacingAngle = Vector3.Angle(character.transform.forward,
                Vector3.ProjectOnPlane(OnboardingBlockoutWorld.StonePoint - _world.Position, Vector3.up));
            if (stoneFacingAngle > 5f || Vector3.Distance(stoneInteractionPosition, _world.Position) > 0.01f)
                throw new InvalidOperationException("Stone interaction must face its target without moving: angle=" + stoneFacingAngle);
            yield return new WaitForSeconds(0.3f);
            var reach = float.NegativeInfinity;
            foreach (var hand in new[] { HumanBodyBones.LeftHand, HumanBodyBones.RightHand })
            {
                var shoulder = character.GetBoneTransform(hand == HumanBodyBones.LeftHand ? HumanBodyBones.LeftUpperArm : HumanBodyBones.RightUpperArm);
                reach = Mathf.Max(reach, Vector3.Dot(character.GetBoneTransform(hand).position - shoulder.position, character.transform.forward));
            }
            if (!character.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Interact") || reach < 0.15f ||
                character.applyRootMotion || Vector3.Distance(stoneInteractionPosition, _world.Position) > 0.01f)
                throw new InvalidOperationException("Stone action needs a reaching Humanoid gesture without root displacement: reach=" + reach);
            var pulseColor = focus.GetComponent<SpriteRenderer>().color;
            if (sealMaterial.color.b < sealRestColor.b + 0.12f || pulseColor.r <= pulseColor.g ||
                frontSeal.GetComponent<Renderer>().sharedMaterial != sealMaterial || sideSeal.GetComponent<Renderer>().sharedMaterial != sealMaterial)
                throw new InvalidOperationException("Stone confirmation needs a warm pulse and a bright seal using the original shared material.");
            yield return Capture(directory, "complete");
            if (_guidance.Hint.text != "Đá Luyện cộng hưởng với linh lực của bạn.")
                throw new InvalidOperationException("Stone confirmation must explain the resonance before replacing it with travel directions.");
            CheckCompletedStoneRepeat();
            if (Array.IndexOf(args, "--lgo-gesture-finish") >= 0)
            {
                yield return new WaitForSeconds(2.5f);
                if (!character.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion") ||
                    Vector3.Distance(stoneInteractionPosition, _world.Position) > 0.01f)
                    throw new InvalidOperationException("Gesture must finish naturally into locomotion without root motion.");
                Debug.Log("LGO_STONE_GESTURE_FINISH_PASS natural_exit=true root_held=true");
            }
            CheckInteractionIcon("Đã xong", "ActionComplete", false);
            Debug.Log("LGO_INTERACTION_ICON_PASS talk=true touch=true complete=true bounded=true texture64=true");
            // Leave the synchronous capture frame before measuring held input in Update frames.
            yield return null;
            var cancelProbe = Array.IndexOf(args, "--lgo-gesture-finish") < 0;
            if (cancelProbe && !character.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Interact"))
                throw new InvalidOperationException("Cancellation probe needs an active gesture before movement input.");
            var facingMoveFrame = Time.frameCount;
            var facingMoveTime = 0f;
            _world.Movement = Vector2.left;
            while (facingMoveTime < 0.3f)
            {
                yield return null;
                facingMoveTime += Time.deltaTime;
            }
            _world.Movement = Vector2.zero;
            Debug.Log("LGO_STONE_MOVEMENT_MEASURE angle=" + Vector3.Angle(character.transform.forward, Vector3.left)
                + " distance=" + Vector3.Distance(stoneInteractionPosition, _world.Position)
                + " frames=" + (Time.frameCount - facingMoveFrame) + " simulation=" + facingMoveTime
                + " dt=" + Time.deltaTime + " focused=" + Application.isFocused);
            if (Vector3.Angle(character.transform.forward, Vector3.left) > 5f ||
                Vector3.Distance(stoneInteractionPosition, _world.Position) < 0.5f)
                throw new InvalidOperationException("Fresh movement must override stone facing during its pulse.");
            yield return new WaitForSeconds(0.15f);
            if (Vector3.Angle(character.transform.forward, Vector3.left) > 5f)
                throw new InvalidOperationException("Stone facing must not resume after releasing fresh input.");
            if (!character.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion"))
                throw new InvalidOperationException("Fresh movement must return the gesture to locomotion.");
            Debug.Log("LGO_STONE_GESTURE_PASS reached=true root_held=true cancel_exercised=" + cancelProbe
                + " natural_probe=" + !cancelProbe);
            Debug.Log("LGO_STONE_FACING_PASS target=true no_drift=true movement_priority=true");
            yield return new WaitForSeconds(1.3f);
            if (focus.gameObject.activeSelf)
            {
                Debug.LogError("LGO_STONE_FEEDBACK_FAIL completion pulse did not end");
                Application.Quit(1);
                yield break;
            }
            yield return Capture(directory, "complete-settled");
            var calmObserved = false;
            var conclusionObserved = false;
            while (Time.unscaledTime - _stoneFeedbackStartedAt < OnboardingDialogueContent.StoneFeedbackDuration)
            {
                yield return null;
                var elapsed = Time.unscaledTime - _stoneFeedbackStartedAt;
                var expected = OnboardingDialogueContent.StoneFeedback(elapsed);
                if (expected == null) break;
                if (_guidance.Hint.text != expected)
                    throw new InvalidOperationException("Stone narration does not match its current elapsed phase.");
                if (elapsed >= 3f && elapsed < 6f && !calmObserved)
                {
                    calmObserved = true;
                    if (!CheckGuidance("Đến sân phía trước.", true)) yield break;
                    yield return Capture(directory, "stone-result-calm");
                }
                if (elapsed >= 6f && !conclusionObserved)
                {
                    conclusionObserved = true;
                    yield return Capture(directory, "stone-result-conclusion");
                }
            }
            while (Time.unscaledTime - _stoneFeedbackStartedAt < OnboardingDialogueContent.StoneFeedbackDuration + 0.2f) yield return null;
            if (_guidance.Hint.text != "Đi tiếp theo đường đá, tới khoảng sân có cây.")
                throw new InvalidOperationException("Stone result failed to return to exploration guidance.");
            yield return WalkTo(stoneInteractionPosition);
            CheckCompletedStoneRepeat();
            // Slow captures may skip a timed phase; never label an unobserved phase as passing.
            if (calmObserved && conclusionObserved)
                Debug.Log("LGO_STONE_RESULT_FEEDBACK_PASS resonance=true calm=true conclusion=true repeat_in_range=true exploration_restored=true");
            else
                Debug.LogWarning("LGO_STONE_RESULT_OBSERVATION_INCOMPLETE calm=" + calmObserved + " conclusion=" + conclusionObserved);
            if (Vector4.Distance(sealRestColor, sealMaterial.color) > 0.01f)
                throw new InvalidOperationException("Stone seal must return to its resting color after confirmation.");
            Debug.Log("LGO_STONE_SEAL_PULSE_PASS bright_peak=true warm_ring=true restored=true shared_material=true");
            if (!CheckGuidance("Đến sân phía trước.", true)) yield break;
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
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            Debug.Log("LGO_PLAYER_IDENTITY_ROUTE_PASS stone=true completed_dialogue=true camera_alley=true");
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
            var heldStarted = Time.time;
            var heldFrame = Time.frameCount;
            var inputDeadline = Time.realtimeSinceStartup + 3f;
            while (Vector3.Distance(_world.Position, movementStart) < 7.2f && Time.realtimeSinceStartup < inputDeadline)
                yield return null;
            Debug.Log("LGO_HELD_INPUT_MEASURE simulation=" + (Time.time - heldStarted) + " frames=" + (Time.frameCount - heldFrame)
                + " real=" + (Time.realtimeSinceStartup - inputDeadline + 3f) + " input=" + _world.ScreenMovement
                + " position=" + _world.Position + " focused=" + Application.isFocused);
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
                Debug.LogError("LGO_CAMERA_INPUT_FAIL held travel outside expected corridor travel=" + travel
                    + " focused=" + Application.isFocused);
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
            // Retain the wide-name stress case through all camera edges, then
            // use a representative identity for the garden presentation captures.
            CheckPlayerIdentity("WWWWWWWWWWWWWWWW", true);
            _world.SetPlayerName("Minh An");
            yield return WalkTo(new Vector3(0f, 0f, 14f));
            CheckPlayerIdentity("Minh An", true);
            yield return Capture(directory, "street-outlook");
            yield return WalkTo(new Vector3(1f, 0f, 18f));
            yield return WalkTo(new Vector3(2f, 0f, 22f));
            yield return null;
            if (!CheckGuidance("Đã tới sân.", true)) yield break;
            yield return Capture(directory, "forecourt-arrival");
            yield return new WaitForSecondsRealtime(3.1f);
            if (!CheckGuidance("Đã tới sân.", false)) yield break;
            if (Mathf.Abs(_world.Position.y) > 0.1f)
                throw new InvalidOperationException("Forecourt route left the paving surface.");
            CheckPlayerIdentity("Minh An", true);
            yield return Capture(directory, "forecourt");
            yield return WalkTo(new Vector3(4.8f, 0f, 23f));
            if (Mathf.Abs(_world.Position.y) > 0.1f)
                throw new InvalidOperationException("Pavilion entry left the paving surface.");
            foreach (var elevation in new[] { 0.2f, 1.1f, 1.7f })
            {
                var target = _world.Position + Vector3.up * elevation;
                var projected = Camera.main.WorldToViewportPoint(target);
                if (Physics.Linecast(target, Camera.main.transform.position, out var pavilionObstruction, ~0, QueryTriggerInteraction.Ignore) ||
                    projected.z <= Camera.main.nearClipPlane || projected.x < 0.05f || projected.x > 0.95f ||
                    projected.y < 0.05f || projected.y > 0.95f)
                    throw new InvalidOperationException("Pavilion camera obscures the player: elevation=" + elevation
                        + " viewport=" + projected + " obstacle=" + (pavilionObstruction.collider == null ? "none" : pavilionObstruction.collider.name));
            }
            yield return Capture(directory, "pavilion-entry");
            yield return WalkTo(new Vector3(2f, 0f, 22f));
            Debug.Log("LGO_PAVILION_ROUTE_PASS entry=true camera_clear=true exit=true");
            yield return WalkTo(new Vector3(1f, 0f, 18f));
            yield return WalkTo(new Vector3(0f, 0f, 14f));
            if (!CheckGuidance("Đã tới sân.", false)) yield break;
            yield return WalkTo(new Vector3(1f, 0f, 18f));
            yield return WalkTo(new Vector3(2f, 0f, 22f));
            yield return null;
            if (!CheckGuidance("Đã tới sân.", false)) yield break;
            Debug.Log("LGO_FORECOURT_ARRIVAL_FEEDBACK_PASS timed=true replay=false shared_guidance=true");
            Debug.Log("LGO_FORECOURT_ROUTE_PASS continuous_ground=true return_route=true");
            yield return WalkTo(new Vector3(1f, 0f, 18f));
            yield return WalkTo(new Vector3(0f, 0f, 14f));
            yield return WalkTo(new Vector3(0f, 0f, 1f));
            Interact();
            if (_world.DialogueVisible) throw new InvalidOperationException("Returning guide opened outside range.");
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            if (!_interact.enabledSelf || _interact.tooltip != "Gặp")
                throw new InvalidOperationException("Completed onboarding must allow talking to the guide again in range.");
            CheckKeeperFocus(true);
            yield return null;
            if (!_world.KeeperGreeting || !_guidance.Hint.text.Contains(OnboardingDialogueContent.GateKeeperWelcomeBack))
                throw new InvalidOperationException("Returning approach needs its own welcome without resetting progress.");
            yield return Capture(directory, "keeper-welcome-back");
            var completedFeedbackAt = _stoneFeedbackStartedAt;
            Submit(_interact);
            yield return null;
            if (!_world.DialogueVisible || !_session.Completed || !_stoneCompleted || !_forecourtVisited)
                throw new InvalidOperationException("Returning conversation must preserve all onboarding progress.");
            yield return Capture(directory, "keeper-return-dialogue");
            Submit(_dialogue.CloseButton);
            yield return null;
            if (_world.DialogueVisible || !_session.Completed || !_stoneCompleted)
                throw new InvalidOperationException("Closing returning dialogue reset progress or retained input lock.");
            if (guideAnimator.GetComponent<NpcGuideGesture>().Active)
                throw new InvalidOperationException("Closing a return conversation must not start a destination gesture.");
            Submit(_interact);
            yield return null;
            if (_dialogue.Progress.text != "Đối thoại: 1/2")
                throw new InvalidOperationException("Returning guide must reopen its two-line conversation from the start.");
            HandleEscape();
            yield return null;
            if (_world.DialogueVisible || !_session.Completed || !_stoneCompleted)
                throw new InvalidOperationException("Escape must close returning dialogue without leaving or resetting progress.");
            Submit(_interact);
            yield return null;
            Submit(_dialogue.ContinueButton);
            yield return null;
            if (_dialogue.ContinueButton.text != "Khám phá tiếp")
                throw new InvalidOperationException("Returning guide must offer exploration, not repeat training.");
            yield return Capture(directory, "keeper-return-destination");
            Submit(_dialogue.ContinueButton);
            yield return null;
            if (_world.DialogueVisible || !_session.Completed || !_stoneCompleted || !_forecourtVisited ||
                _stoneFeedbackStartedAt != completedFeedbackAt)
                throw new InvalidOperationException("Returning guide completion replayed onboarding or retained input lock.");
            yield return new WaitForSeconds(0.8f);
            var returnDirection = Vector3.ProjectOnPlane(OnboardingBlockoutWorld.ForecourtPoint - guideRoot, Vector3.up);
            var returnHand = Vector3.ProjectOnPlane(guideAnimator.GetBoneTransform(HumanBodyBones.RightHand).position
                - guideAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm).position, Vector3.up);
            if (!guideAnimator.GetComponent<NpcGuideGesture>().Active || Vector3.Angle(returnDirection, returnHand) > 20f ||
                returnHand.magnitude < 0.2f || Vector3.Angle(guideAnimator.transform.forward, returnDirection) > 5f ||
                Vector3.Distance(guideRoot, guideAnimator.transform.position) > 0.001f || _world.DialogueVisible)
                throw new InvalidOperationException("Return guide must point toward the forecourt with a raised hand and fixed root.");
            if (!CheckGuidance("Đến sân phía trước.", true)) yield break;
            yield return Capture(directory, "keeper-guide-forecourt");
            Submit(_interact);
            yield return null;
            if (!_world.DialogueVisible || _dialogue.Progress.text != "Đối thoại: 1/2" ||
                !_session.Completed || !_stoneCompleted || !_forecourtVisited)
                throw new InvalidOperationException("A completed return conversation must remain available without resetting progress.");
            if (guideAnimator.GetComponent<NpcGuideGesture>().Active)
                throw new InvalidOperationException("Opening dialogue must cancel the previous destination gesture.");
            Debug.Log("LGO_GUIDE_RETURN_DIRECTION_PASS target=forecourt cancel_on_reopen=true close_no_point=true root_held=true");
            Submit(_dialogue.CloseButton);
            yield return null;
            yield return WalkTo(new Vector3(0f, 0f, 1f));
            yield return WalkTo(stoneInteractionPosition);
            CheckCompletedStoneRepeat();
            Debug.Log("LGO_GUIDE_REVISIT_PASS range=true close_reopen=true escape_handler=true progress_preserved=true movement_restored=true no_stone_replay=true");
            if (GetComponent<M4PlayableClientController>() != null)
                throw new InvalidOperationException("Blockout must not create the account client UI.");
            Debug.Log("LGO_ONBOARDING_BLOCKOUT_ROUTE_PASS movement=CharacterController no_teleport=true");
            Debug.Log("LGO_ONBOARDING_NPC_FLOW_PASS shared_session=true shared_modal=true cancel_reopen=true range=true input_lock=true");
            Application.Quit(0);
        }

        private static bool KeeperConversing(Animator animator)
        {
            var layer = animator.GetLayerIndex("Conversation");
            return layer > 0 && animator.GetLayerWeight(layer) >= 0.99f &&
                animator.GetCurrentAnimatorStateInfo(layer).IsName("Talking") &&
                animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion");
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

        private void CheckCompletedStoneRepeat()
        {
            if (!_stoneCompleted || !IsNear(OnboardingBlockoutWorld.StonePoint) || Dialogue.Active || _interact.enabledSelf)
                throw new InvalidOperationException("Repeat probe requires a completed stone while still in interaction range.");
            var startedAt = _stoneFeedbackStartedAt;
            Interact();
            if (!_stoneCompleted || _stoneFeedbackStartedAt != startedAt)
                throw new InvalidOperationException("Completed stone interaction restarted its feedback while in range.");
        }

        private IEnumerator WalkTo(Vector3 point)
        {
            var deadline = Time.realtimeSinceStartup + 10f;
            var character = _world.transform.Find("Blockout player proxy").GetComponentInChildren<Animator>();
            var leg = character.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            var initialRotation = leg.localRotation;
            var started = Time.time;
            while (Vector2.Distance(new Vector2(_world.Position.x, _world.Position.z), new Vector2(point.x, point.z)) > 0.08f)
            {
                if (Time.realtimeSinceStartup > deadline)
                    throw new InvalidOperationException("Blockout route blocked or unfocused before " + point);
                var direction = point - _world.Position;
                _world.Movement = Vector2.ClampMagnitude(new Vector2(direction.x, direction.z) * 2f, 1f);
                if (!_locomotionVerified && Time.time - started > 0.2f && character.GetFloat("Speed") > 0.2f
                    && Quaternion.Angle(initialRotation, leg.localRotation) > 2f)
                {
                    _locomotionVerified = true;
                    Debug.Log("LGO_ARRIVAL_CHARACTER_MOTION_PASS humanoid=true leg_animated=true speed_from_controller=true");
                }
                yield return null;
            }
            _world.Movement = Vector2.zero;
            if (!_locomotionVerified) throw new InvalidOperationException("Arrival character locomotion did not animate during route.");
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
            if (!_forecourtVisited && _stoneCompleted && Vector2.Distance(new Vector2(_world.Position.x, _world.Position.z), new Vector2(2f, 22f)) <= 2f)
            {
                _forecourtVisited = true;
                _forecourtArrivalUntil = Time.unscaledTime + 3f;
            }
            var returningToKeeper = _stoneCompleted && InRange;
            _guidanceScroll.style.display = Dialogue.Active || (_forecourtVisited && Time.unscaledTime >= _forecourtArrivalUntil && !returningToKeeper)
                ? DisplayStyle.None : DisplayStyle.Flex;
            _guidance.Objective.text = returningToKeeper ? "Gặp Người Giữ Cổng." : _forecourtVisited ? "Đã tới sân." : _stoneCompleted ? "Đến sân phía trước." : _session.Completed ? "Chạm Đá Luyện." : "Gặp Người Giữ Cổng.";
            _guidance.Hint.text = returningToKeeper ? "Bạn có thể hỏi lại đường tới sân nghỉ." : _forecourtVisited ? "Bạn có thể dừng chân ở đây." : _stoneCompleted
                ? OnboardingDialogueContent.StoneFeedback(Time.unscaledTime - _stoneFeedbackStartedAt) ?? "Đi tiếp theo đường đá, tới khoảng sân có cây."
                : _session.Completed ? (InRange ? "Đá Luyện sẵn sàng nhận linh khí." : "Đá ở bên phải đường phía trước.")
                : InRange ? "Người Giữ Cổng đang chờ." : "Theo đường đá đến Người Giữ Cổng.";
            if (_world.KeeperGreeting && InRange && !Dialogue.Active)
                _guidance.Hint.text = "Người Giữ Cổng: “" + (_stoneCompleted
                    ? OnboardingDialogueContent.GateKeeperWelcomeBack : OnboardingDialogueContent.GateKeeperWelcome) + "”";
            if (_stoneCompleted && _world.KeeperGuiding && !Dialogue.Active)
            {
                _guidance.Objective.text = "Đến sân phía trước.";
                _guidance.Hint.text = "Theo đường đá, tới khoảng sân có cây.";
                _guidanceScroll.style.display = DisplayStyle.Flex;
            }
            RuntimeUiFactory.ApplyWorldTouchInteraction(_interact, returningToKeeper ? "Gặp" : _stoneCompleted ? "Đã xong" : _session.Completed ? "Luyện" : "Gặp");
            _interact.SetEnabled(!Dialogue.Active && InRange);
            _world.KeeperReady = (!_session.Completed || _stoneCompleted) && InRange;
            _world.SetStoneFeedback(_session.Completed && !_stoneCompleted && InRange, _stoneCompleted);
            if (!_capturing) _world.ScreenMovement = Application.isFocused ? _pad.Value : Vector2.zero;
            if (Input.GetKeyDown(KeyCode.F)) Interact();
            if (Input.GetKeyDown(KeyCode.Escape)) HandleEscape();
        }

        private void CheckInteractionIcon(string label, string textureName, bool enabled)
        {
            var icon = _interact.Q<VisualElement>("LGO World Touch Action Icon");
            var texture = icon?.resolvedStyle.backgroundImage.texture;
            if (texture == null || texture.name != textureName || texture.width != 64 || texture.height != 64 ||
                _interact.text != string.Empty || _interact.tooltip != label || _interact.enabledSelf != enabled)
                throw new InvalidOperationException("Interaction icon/state missing or incorrect: " + label);
            var bounds = icon.worldBound;
            var button = _interact.worldBound;
            var viewport = RuntimeViewportMetrics.FromRoot(_interact.panel.visualTree);
            var scaleX = viewport.ScreenPixelWidth / (float)viewport.PanelWidth;
            var scaleY = viewport.ScreenPixelHeight / (float)viewport.PanelHeight;
            // UI Toolkit rounds edges to physical pixels, not panel units.
            if (bounds.width <= 0f || Mathf.Abs(bounds.width * scaleX - bounds.height * scaleY) > 1.01f ||
                Mathf.Abs(bounds.center.x - button.center.x) * scaleX > 1.01f ||
                Mathf.Abs(bounds.center.y - button.center.y) * scaleY > 1.01f ||
                bounds.xMin <= button.xMin || bounds.xMax >= button.xMax ||
                bounds.yMin <= button.yMin || bounds.yMax >= button.yMax || icon.pickingMode != PickingMode.Ignore)
                throw new InvalidOperationException("Interaction icon must fit inside its button without intercepting input: icon="
                    + bounds + " button=" + button + " content=" + _interact.contentRect + " picking=" + icon.pickingMode);
        }

        internal void CheckPlayerIdentity(string name, bool visible)
        {
            var holder = _world.transform.Find("Blockout Player Label");
            var label = holder == null ? null : holder.GetComponent<TextMesh>();
            if (label == null || label.text != name || label.gameObject.activeSelf != visible ||
                _guidance.Area.text != "Linh Môn")
                throw new InvalidOperationException("Player name must use a world label; the HUD area must identify Linh Mon.");
            var shadow = holder.GetChild(0).GetComponent<TextMesh>();
            if (shadow == null || shadow.text != name)
                throw new InvalidOperationException("Player name needs the shared synchronized label shadow.");
            if (!visible) return;
            var camera = _world.GetComponentInChildren<Camera>();
            var rect = IdentityScreenRect(camera, label.GetComponent<Renderer>().bounds);
            var player = _world.GetComponentInChildren<CharacterController>();
            var subject = IdentityScreenRect(camera, player.bounds);
            if (rect.width <= 0f || rect.height < 8f || rect.xMin < 0f || rect.xMax > camera.pixelWidth ||
                rect.yMin < subject.yMax || rect.yMax > camera.pixelHeight ||
                Mathf.Abs(rect.center.x - subject.center.x) > 2f)
                throw new InvalidOperationException("Player identity is clipped, too small or detached: " + rect + " subject=" + subject);
            foreach (var obstacleName in new[] { "Blockout Keeper Label", "Blockout Stone Label" })
            {
                var obstacle = _world.transform.Find(obstacleName).GetComponent<Renderer>();
                if (obstacle.gameObject.activeInHierarchy && camera.WorldToScreenPoint(obstacle.bounds.center).z > camera.nearClipPlane &&
                    rect.Overlaps(IdentityScreenRect(camera, obstacle.bounds)))
                    throw new InvalidOperationException("Player identity overlaps " + obstacleName);
            }
        }

        private static Rect IdentityScreenRect(Camera camera, Bounds bounds)
        {
            var min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            for (var x = -1; x <= 1; x += 2)
            for (var y = -1; y <= 1; y += 2)
            for (var z = -1; z <= 1; z += 2)
            {
                var screen = camera.WorldToScreenPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(x, y, z)));
                min = Vector2.Min(min, screen);
                max = Vector2.Max(max, screen);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private void CheckKeeperFocus(bool visible)
        {
            var focus = _world.transform.Find("Blockout Keeper Focus");
            var stone = _world.transform.Find("Blockout Stone Focus");
            if (focus == null || focus.gameObject.activeSelf != visible)
                throw new InvalidOperationException("Keeper focus visibility mismatch: expected=" + visible);
            if (focus.GetComponent<SpriteRenderer>().sprite == null ||
                focus.GetComponent<SpriteRenderer>().sprite != stone.GetComponent<SpriteRenderer>().sprite)
                throw new InvalidOperationException("Keeper and stone must share their focus sprite.");
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
            if (_capturing) Debug.Log("LGO_BLOCKOUT_FOCUS focused=" + focused + " time=" + Time.realtimeSinceStartup);
            _pad?.ResetInput();
            if (_world != null) _world.ResetMovementInput();
        }
    }
}
