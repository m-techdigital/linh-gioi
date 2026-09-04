using System;
using System.Globalization;
using LinhGioi.Account;
using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class PlayableWorldController : MonoBehaviour
    {
        private const float MoveSpeed = 3.6f;
        private const float RotateSpeed = 105f;
        private const float InteractionRange = 1.45f;
        private static readonly Vector3 CameraFollowOffset = new Vector3(0f, 7.5f, -8.5f);
        private static readonly string[] GateKeeperDialogueLines =
        {
            "Gate Keeper: Welcome to the Spirit Gate. Keep your breath steady.",
            "Gate Keeper: Follow the cyan pulse north; the Training Stone will answer when you focus.",
            "Gate Keeper: This yard is safe. The eastern shadow is only a warning marker."
        };
        private static readonly Vector3 GateKeeperPosition = new Vector3(-3f, 0.75f, 3f);
        private static readonly Vector3 TrainingStonePosition = new Vector3(0f, 0.08f, 4.5f);
        private static readonly Vector3 ShadowSlimePosition = new Vector3(3f, 0.4f, 3f);
        private CharacterResponse _character;
        private Transform _marker;
        private Renderer _markerRenderer;
        private Transform _posePulse;
        private Transform _gateKeeperGuidePulse;
        private Transform _trainingSpiritPulse;
        private Transform _shadowWarningPulse;
        private Transform _portalGatePulse;
        private Transform _windSlashPreview;
        private Transform _shadowBindWarning;
        private InteractableState _nearestInteractable;
        private string _objectiveText = "Objective: enter the world and find the training stone.";
        private string _interactionText = "Move near the Gate Keeper or Training Stone.";
        private GuidedTrainingStep _guidedStep = GuidedTrainingStep.FindGateKeeper;
        private PlaceholderPoseState _playerPoseState = PlaceholderPoseState.Idle;
        private PlaceholderNpcState _gateKeeperState = PlaceholderNpcState.Idle;
        private PlaceholderSlimeState _shadowSlimeState = PlaceholderSlimeState.Idle;
        private PlaceholderVfxFeedbackState _vfxFeedbackState = PlaceholderVfxFeedbackState.Quiet;
        private float _posePulseUntil;
        private float _vfxPreviewUntil;
        private int _dialogueLineIndex;

        public event Action PositionChanged;
        public event Action InteractionStateChanged;

        public Vector3 CurrentPosition => _marker == null ? Vector3.zero : _marker.position;
        public float CurrentYawDegrees => _marker == null ? 0f : _marker.eulerAngles.y;
        public string ObjectiveText => _objectiveText;
        public string InteractionText => _interactionText;
        public string GuidedTrainingStepName => _guidedStep.ToString();
        public string CurrentAreaLabel => DescribeCurrentArea();
        public string ObjectiveDirectionHint => DescribeObjectiveDirection();
        public string WorldLandmarkSummary => "Landmarks: Spirit Gate south / Gate Keeper northwest / Training Stone north / Shadow Slime east.";
        public string PlayerPoseStateName => _playerPoseState.ToString();
        public string GateKeeperPoseStateName => _gateKeeperState.ToString();
        public string ShadowSlimeStateName => _shadowSlimeState.ToString();
        public string VfxFeedbackStateName => _vfxFeedbackState.ToString();
        public bool DialogueActive { get; private set; }
        public bool DialogueCompleted { get; private set; }
        public string DialogueSpeaker => "Gate Keeper";
        public string DialogueLine => DialogueActive ? GateKeeperDialogueLines[Mathf.Clamp(_dialogueLineIndex, 0, GateKeeperDialogueLines.Length - 1)] : string.Empty;
        public string DialogueProgress => DialogueActive ? (_dialogueLineIndex + 1) + "/" + GateKeeperDialogueLines.Length : "0/" + GateKeeperDialogueLines.Length;
        public bool HasNextDialogueLine => DialogueActive && _dialogueLineIndex < GateKeeperDialogueLines.Length - 1;
        public bool InteractionAcknowledged { get; private set; }

        public void Enter(CharacterResponse character)
        {
            _character = character ?? throw new ArgumentNullException(nameof(character));
            if (_marker == null) _marker = CreateMarker().transform;
            _markerRenderer = _marker.GetComponent<Renderer>();
            EnsurePoseFeedbackMarkers();
            _marker.position = character.Position;
            _marker.rotation = Quaternion.Euler(0f, character.yawDegrees, 0f);
            InteractionAcknowledged = false;
            DialogueActive = false;
            DialogueCompleted = false;
            _dialogueLineIndex = 0;
            _guidedStep = GuidedTrainingStep.FindGateKeeper;
            SetPlayerPose(PlaceholderPoseState.Idle);
            SetGateKeeperState(PlaceholderNpcState.Idle);
            SetShadowSlimeState(PlaceholderSlimeState.Idle);
            SetVfxFeedback(PlaceholderVfxFeedbackState.PortalGatePulse, 1.35f);
            _objectiveText = "Objective 1/2: talk to the Gate Keeper.";
            RefreshInteractionState();
            PositionChanged?.Invoke();
        }

        public SaveCharacterPositionRequest BuildSaveRequest()
        {
            var position = CurrentPosition;
            return new SaveCharacterPositionRequest(position.x, position.y, position.z, NormalizeYaw(CurrentYawDegrees));
        }

        public string FormatPosition()
        {
            var position = CurrentPosition;
            return string.Format(CultureInfo.InvariantCulture, "x={0:0.00} y={1:0.00} z={2:0.00} yaw={3:0.0}", position.x, position.y, position.z, NormalizeYaw(CurrentYawDegrees));
        }

        public void SetSmokePosition(float x, float y, float z, float yawDegrees)
        {
            if (_marker == null) _marker = CreateMarker().transform;
            _marker.position = new Vector3(x, y, z);
            _marker.rotation = Quaternion.Euler(0f, yawDegrees, 0f);
            RefreshInteractionState();
            PositionChanged?.Invoke();
        }

        public void SetSmokePositionNearTrainingStone()
        {
            SetSmokePosition(0f, 0.25f, 3.45f, 0f);
        }

        public void SetSmokePositionNearGateKeeper()
        {
            SetSmokePosition(-3f, 0.25f, 3f, 0f);
        }

        public bool TriggerInteractionForSmoke()
        {
            RefreshInteractionState();
            return TryTriggerInteraction();
        }

        public void PreviewSkillFeedback(string previewName)
        {
            if (string.IsNullOrWhiteSpace(previewName)) return;
            SetPlayerPose(PlaceholderPoseState.Interact);
            if (previewName == "Wind Slash")
            {
                SetVfxFeedback(PlaceholderVfxFeedbackState.WindSlashPreview, 1.25f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Gold);
                _interactionText = "Preview only: Wind Slash form traces a gold arc with no opponent or result.";
            }
            else if (previewName == "Shadow Bind")
            {
                SetShadowSlimeState(PlaceholderSlimeState.AlertWarning);
                SetVfxFeedback(PlaceholderVfxFeedbackState.ShadowBindWarning, 1.25f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Danger);
                _interactionText = "Preview only: Shadow Bind warning ring marks readable intent in the safe yard.";
            }
            else
            {
                SetPlayerPose(PlaceholderPoseState.SpiritChannel);
                SetVfxFeedback(PlaceholderVfxFeedbackState.SpiritPulse, 1.25f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Spirit);
                _interactionText = "Preview only: Spirit Guard pulse shows a defensive stance rehearsal.";
            }
            InteractionStateChanged?.Invoke();
        }

        private void Update()
        {
            if (_marker == null || _character == null) return;
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            var rotate = 0f;
            if (Input.GetKey(KeyCode.Q)) rotate -= 1f;
            if (Input.GetKey(KeyCode.E)) rotate += 1f;

            if (Mathf.Abs(rotate) > 0.001f)
                _marker.Rotate(Vector3.up, rotate * RotateSpeed * Time.deltaTime, Space.World);

            var input = new Vector3(horizontal, 0f, vertical);
            if (input.sqrMagnitude > 1f) input.Normalize();
            if (input.sqrMagnitude > 0.0001f)
            {
                _marker.position += input * MoveSpeed * Time.deltaTime;
                SetPlayerPose(PlaceholderPoseState.WalkMove);
                RefreshInteractionState();
                PositionChanged?.Invoke();
            }
            else if (_playerPoseState == PlaceholderPoseState.WalkMove)
            {
                SetPlayerPose(PlaceholderPoseState.Idle);
            }

            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
                TryTriggerInteraction();

            RefreshPoseFeedbackMarkers();
            RefreshVfxFeedbackMarkers();
            RefreshCameraFrame();
        }

        private static GameObject CreateMarker()
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            marker.name = "LGO Player Marker";
            marker.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = RuntimeArtCatalog.CreateMaterial("LGO Hero Placeholder", RuntimeArtCatalog.Spirit);
            }
            EnsureCamera(marker.transform);
            EnsureGround();
            EnsureWorldPlaceholders();
            return marker;
        }

        private static void EnsureCamera(Transform target)
        {
            if (Camera.main != null) return;
            var cameraObject = new GameObject("LGO Playable Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = target.position + CameraFollowOffset;
            camera.transform.rotation = Quaternion.Euler(38f, 0f, 0f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = RuntimeArtCatalog.Background;
            var lightObject = new GameObject("LGO Playable Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }

        private void RefreshCameraFrame()
        {
            var camera = Camera.main;
            if (camera == null || _marker == null) return;
            var desired = _marker.position + CameraFollowOffset;
            camera.transform.position = Vector3.Lerp(camera.transform.position, desired, Mathf.Clamp01(Time.deltaTime * 7f));
            camera.transform.rotation = Quaternion.Euler(38f, 0f, 0f);
        }

        private static void EnsureGround()
        {
            if (GameObject.Find("LGO World Ground") != null) return;
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "LGO World Ground";
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
            var renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = RuntimeArtCatalog.CreateMaterial("LGO Training Ground Tile", RuntimeArtCatalog.SurfaceRaised);
            }
        }

        private static void EnsureWorldPlaceholders()
        {
            // Preserve the M4 visual source marker: LGO NPC Keeper Placeholder.
            CreateMarkerCube("LGO Gate Keeper NPC Interactable", GateKeeperPosition, RuntimeArtCatalog.Gold, new Vector3(0.9f, 1.5f, 0.9f));
            CreateMarkerCube("LGO Gate Keeper Gold Readability Pillar", GateKeeperPosition + new Vector3(0f, 1.35f, 0f), RuntimeArtCatalog.Gold, new Vector3(0.25f, 1.4f, 0.25f));
            CreateMarkerCube("LGO Shadow Slime Non Combat Marker", ShadowSlimePosition, RuntimeArtCatalog.Shadow, new Vector3(1.4f, 0.8f, 1.4f));
            CreateMarkerCube("LGO Shadow Slime Warning Plinth", ShadowSlimePosition + new Vector3(0f, -0.2f, 0f), RuntimeArtCatalog.Danger, new Vector3(1.8f, 0.08f, 1.8f));
            CreateMarkerCube("LGO Training Stone Interactable", TrainingStonePosition, RuntimeArtCatalog.Spirit, new Vector3(1.2f, 0.16f, 1.2f));
            CreateMarkerCube("LGO Training Stone Cyan Beacon", TrainingStonePosition + new Vector3(0f, 0.8f, 0f), RuntimeArtCatalog.Spirit, new Vector3(0.35f, 1.2f, 0.35f));
            CreateMarkerCube("LGO Spirit Gate Landmark South", new Vector3(0f, 1.2f, -4.5f), RuntimeArtCatalog.Spirit, new Vector3(2.8f, 2.4f, 0.25f));
            CreateMarkerCube("LGO Safe Training Circle Center", new Vector3(0f, 0.04f, 0f), RuntimeArtCatalog.Gold, new Vector3(3.2f, 0.08f, 3.2f));
        }

        private static GameObject CreateMarkerCube(string name, Vector3 position, Color color, Vector3 scale)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = name;
            marker.transform.position = position;
            marker.transform.localScale = scale;
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null) renderer.material = RuntimeArtCatalog.CreateMaterial(name + " Material", color);
            return marker;
        }

        private static float NormalizeYaw(float yaw)
        {
            yaw %= 360f;
            return yaw < 0f ? yaw + 360f : yaw;
        }

        private void RefreshInteractionState()
        {
            if (_marker == null)
            {
                SetNearest(null, "Move near the Gate Keeper or Training Stone.");
                return;
            }

            var position = _marker.position;
            var training = new InteractableState(
                "Training Stone",
                "Press F or Space: stabilize spirit pulse.",
                "Spirit pulse stabilized. Training acknowledged.",
                TrainingStonePosition
            );
            var keeper = new InteractableState(
                "Gate Keeper",
                "Press F or Space: ask the Gate Keeper for guidance.",
                "Gate Keeper: your path is open. Try the Training Stone.",
                GateKeeperPosition
            );

            var nearest = _guidedStep == GuidedTrainingStep.FindGateKeeper ? keeper : training;
            if (Distance2D(position, nearest.position) <= InteractionRange)
                SetNearest(nearest, nearest.prompt);
            else if (_guidedStep == GuidedTrainingStep.FindGateKeeper && Distance2D(position, training.position) <= InteractionRange)
                SetNearest(training, training.prompt);
            else
                SetNearest(null, InteractionAcknowledged ? "Loop complete: save position or return to lobby." : NextMovementHint());
        }

        private bool TryTriggerInteraction()
        {
            if (_nearestInteractable == null) return false;
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper && _nearestInteractable.id == "Gate Keeper")
            {
                SetPlayerPose(PlaceholderPoseState.Interact);
                SetGateKeeperState(PlaceholderNpcState.TalkGuide);
                SetVfxFeedback(PlaceholderVfxFeedbackState.WindSlashPreview, 1.1f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Gold);
                OpenGateKeeperDialogue();
            }
            else if (_guidedStep == GuidedTrainingStep.FindTrainingStone && _nearestInteractable.id == "Training Stone")
            {
                CompleteTrainingStoneInteraction();
            }
            else if (_guidedStep == GuidedTrainingStep.FindGateKeeper && _nearestInteractable.id == "Training Stone")
            {
                DialogueCompleted = true;
                CompleteTrainingStoneInteraction();
            }
            else
            {
                _interactionText = NextMovementHint();
                InteractionStateChanged?.Invoke();
                return false;
            }
            InteractionStateChanged?.Invoke();
            return true;
        }

        private void CompleteTrainingStoneInteraction()
        {
            _guidedStep = GuidedTrainingStep.Complete;
            InteractionAcknowledged = true;
            SetPlayerPose(PlaceholderPoseState.SpiritChannel);
            SetGateKeeperState(PlaceholderNpcState.Idle);
            SetShadowSlimeState(PlaceholderSlimeState.DissolveQuiet);
            SetVfxFeedback(PlaceholderVfxFeedbackState.SpiritPulse, 1.5f);
            TriggerLocalPosePulse(RuntimeArtCatalog.Spirit);
            _objectiveText = "Objective complete: spirit pulse stabilized.";
            _interactionText = "Spirit pulse stabilized. Training acknowledged.";
        }

        public bool ContinueDialogue()
        {
            if (!DialogueActive) return false;
            if (_dialogueLineIndex < GateKeeperDialogueLines.Length - 1)
            {
                _dialogueLineIndex++;
                _interactionText = DialogueLine;
                InteractionStateChanged?.Invoke();
                return true;
            }
            return CloseDialogue();
        }

        public bool CloseDialogue()
        {
            if (!DialogueActive && DialogueCompleted) return false;
            DialogueActive = false;
            DialogueCompleted = true;
            _guidedStep = GuidedTrainingStep.FindTrainingStone;
            SetGateKeeperState(PlaceholderNpcState.Idle);
            _objectiveText = "Objective 2/2: stabilize the Training Stone.";
            _interactionText = "Gate Keeper: your path is open. Follow the cyan spirit pulse north.";
            RefreshInteractionState();
            InteractionStateChanged?.Invoke();
            return true;
        }

        private void OpenGateKeeperDialogue()
        {
            DialogueActive = true;
            DialogueCompleted = false;
            _dialogueLineIndex = 0;
            _objectiveText = "Objective 1/2: listen to the Gate Keeper.";
            _interactionText = DialogueLine;
        }

        private string NextMovementHint()
        {
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "Step 1: move toward the gold Gate Keeper at the northwest side of the yard.";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "Step 2: follow the cyan pulse to the Training Stone at the north center.";
            return "Loop complete: save position or return to lobby.";
        }

        private string DescribeObjectiveDirection()
        {
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "face northwest for Step 1 / Gate Keeper guidance.";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "face north for Step 2 / Training Stone focus.";
            return "training complete; use Save Position or return to Character Hall.";
        }

        private string DescribeCurrentArea()
        {
            if (_nearestInteractable != null) return _nearestInteractable.id;
            if (Distance2D(CurrentPosition, ShadowSlimePosition) <= 2.25f)
            {
                SetShadowSlimeState(PlaceholderSlimeState.AlertWarning);
                SetVfxFeedback(PlaceholderVfxFeedbackState.ShadowBindWarning, 1.2f);
                return "Safe yard / east shadow warning";
            }
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "Safe yard / path to Gate Keeper";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "Safe yard / path to Training Stone";
            return "Safe yard / training complete";
        }

        private void SetNearest(InteractableState state, string text)
        {
            if (_nearestInteractable == state && _interactionText == text) return;
            _nearestInteractable = state;
            _interactionText = text;
            InteractionStateChanged?.Invoke();
        }

        private static float Distance2D(Vector3 a, Vector3 b)
        {
            var dx = a.x - b.x;
            var dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        private void SetPlayerPose(PlaceholderPoseState state)
        {
            if (_playerPoseState == state) return;
            _playerPoseState = state;
            if (_markerRenderer != null)
            {
                var color = state == PlaceholderPoseState.SpiritChannel ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;
                _markerRenderer.material = RuntimeArtCatalog.CreateMaterial("LGO Hero " + state + " Placeholder", color);
            }
            InteractionStateChanged?.Invoke();
        }

        private void SetGateKeeperState(PlaceholderNpcState state)
        {
            if (_gateKeeperState == state) return;
            _gateKeeperState = state;
            InteractionStateChanged?.Invoke();
        }

        private void SetShadowSlimeState(PlaceholderSlimeState state)
        {
            if (_shadowSlimeState == state) return;
            _shadowSlimeState = state;
            InteractionStateChanged?.Invoke();
        }

        private void EnsurePoseFeedbackMarkers()
        {
            if (_posePulse == null)
            {
                _posePulse = CreateMarkerCube("LGO Player Pose Pulse Placeholder", CurrentPosition + Vector3.up * 0.2f, RuntimeArtCatalog.Spirit, new Vector3(1.4f, 0.08f, 1.4f)).transform;
            }
            if (_gateKeeperGuidePulse == null)
            {
                _gateKeeperGuidePulse = CreateMarkerCube("LGO Gate Keeper Talk Guide Pulse", GateKeeperPosition + Vector3.up * 0.9f, RuntimeArtCatalog.Gold, new Vector3(1.35f, 0.08f, 1.35f)).transform;
            }
            if (_trainingSpiritPulse == null)
            {
                _trainingSpiritPulse = CreateMarkerCube("LGO Training Stone Spirit Channel Pulse", TrainingStonePosition + Vector3.up * 0.12f, RuntimeArtCatalog.Spirit, new Vector3(1.8f, 0.08f, 1.8f)).transform;
            }
            if (_shadowWarningPulse == null)
            {
                _shadowWarningPulse = CreateMarkerCube("LGO Shadow Slime Alert Warning Pulse", ShadowSlimePosition + Vector3.up * 0.18f, RuntimeArtCatalog.Danger, new Vector3(2f, 0.08f, 2f)).transform;
            }
            RefreshPoseFeedbackMarkers();
        }

        private void RefreshPoseFeedbackMarkers()
        {
            if (_posePulse != null)
            {
                _posePulse.position = CurrentPosition + Vector3.up * 0.08f;
                _posePulse.gameObject.SetActive(Time.time < _posePulseUntil || _playerPoseState == PlaceholderPoseState.SpiritChannel);
            }
            if (_gateKeeperGuidePulse != null)
                _gateKeeperGuidePulse.gameObject.SetActive(_gateKeeperState == PlaceholderNpcState.TalkGuide);
            if (_trainingSpiritPulse != null)
                _trainingSpiritPulse.gameObject.SetActive(_playerPoseState == PlaceholderPoseState.SpiritChannel);
            if (_shadowWarningPulse != null)
                _shadowWarningPulse.gameObject.SetActive(_shadowSlimeState == PlaceholderSlimeState.AlertWarning);
            if (_portalGatePulse == null)
                _portalGatePulse = CreateMarkerCube("LGO Portal Gate Pulse Placeholder", new Vector3(0f, 0.18f, -4.5f), RuntimeArtCatalog.Spirit, new Vector3(3.2f, 0.08f, 0.55f)).transform;
            if (_windSlashPreview == null)
                _windSlashPreview = CreateMarkerCube("LGO Wind Slash Preview Placeholder", CurrentPosition + new Vector3(0f, 0.7f, 0.9f), RuntimeArtCatalog.Gold, new Vector3(1.8f, 0.12f, 0.22f)).transform;
            if (_shadowBindWarning == null)
                _shadowBindWarning = CreateMarkerCube("LGO Shadow Bind Warning Placeholder", ShadowSlimePosition + Vector3.up * 0.55f, RuntimeArtCatalog.Shadow, new Vector3(1.9f, 0.1f, 1.9f)).transform;
            RefreshVfxFeedbackMarkers();
        }

        private void TriggerLocalPosePulse(Color color)
        {
            _posePulseUntil = Time.time + 1.15f;
            if (_posePulse != null)
            {
                var renderer = _posePulse.GetComponent<Renderer>();
                if (renderer != null) renderer.material = RuntimeArtCatalog.CreateMaterial("LGO Local Pose Pulse", color);
            }
            RefreshPoseFeedbackMarkers();
        }

        private void SetVfxFeedback(PlaceholderVfxFeedbackState state, float durationSeconds)
        {
            _vfxFeedbackState = state;
            _vfxPreviewUntil = Mathf.Max(_vfxPreviewUntil, Time.time + durationSeconds);
            RefreshVfxFeedbackMarkers();
            InteractionStateChanged?.Invoke();
        }

        private void RefreshVfxFeedbackMarkers()
        {
            var active = Time.time < _vfxPreviewUntil;
            if (!active && _vfxFeedbackState != PlaceholderVfxFeedbackState.Quiet)
                _vfxFeedbackState = PlaceholderVfxFeedbackState.Quiet;
            if (_portalGatePulse != null)
                _portalGatePulse.gameObject.SetActive(active && _vfxFeedbackState == PlaceholderVfxFeedbackState.PortalGatePulse);
            if (_windSlashPreview != null)
            {
                _windSlashPreview.position = CurrentPosition + _marker.forward * 0.9f + Vector3.up * 0.7f;
                _windSlashPreview.rotation = _marker.rotation;
                _windSlashPreview.gameObject.SetActive(active && _vfxFeedbackState == PlaceholderVfxFeedbackState.WindSlashPreview);
            }
            if (_shadowBindWarning != null)
                _shadowBindWarning.gameObject.SetActive(active && _vfxFeedbackState == PlaceholderVfxFeedbackState.ShadowBindWarning);
        }

        private sealed class InteractableState
        {
            public readonly string id;
            public readonly string prompt;
            public readonly string acknowledged;
            public readonly Vector3 position;

            public InteractableState(string id, string prompt, string acknowledged, Vector3 position)
            {
                this.id = id;
                this.prompt = prompt;
                this.acknowledged = acknowledged;
                this.position = position;
            }
        }

        private enum GuidedTrainingStep
        {
            FindGateKeeper,
            FindTrainingStone,
            Complete
        }

        private enum PlaceholderPoseState
        {
            Idle,
            WalkMove,
            Interact,
            SpiritChannel
        }

        private enum PlaceholderNpcState
        {
            Idle,
            TalkGuide
        }

        private enum PlaceholderSlimeState
        {
            Idle,
            AlertWarning,
            DissolveQuiet
        }

        private enum PlaceholderVfxFeedbackState
        {
            Quiet,
            PortalGatePulse,
            WindSlashPreview,
            SpiritPulse,
            ShadowBindWarning
        }
    }
}
