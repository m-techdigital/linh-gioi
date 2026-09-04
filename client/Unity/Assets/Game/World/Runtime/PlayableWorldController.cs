using System;
using System.Globalization;
using LinhGioi.Account;
using LinhGioi.Art;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class PlayableWorldController : MonoBehaviour
    {
        private const float MoveSpeed = 3.6f;
        private const float RotateSpeed = 105f;
        private const float InteractionRange = 1.45f;
        private const float LocalCombatTargetSelectionRange = 10f;
        private const uint CombatProtocolVersion = 1;
        private static readonly Vector3 CameraFollowOffset = new Vector3(0f, 8.25f, -8.75f);
        private static readonly string[] GateKeeperDialogueLines =
        {
            "Người Giữ Cổng: Chào mừng đến Linh Môn. Giữ hơi thở thật ổn định.",
            "Người Giữ Cổng: Đi theo mạch sáng lam về phía bắc; Đá Luyện sẽ đáp lại khi con tập trung.",
            "Người Giữ Cổng: Sân này an toàn. Bóng ở phía đông chỉ là dấu cảnh báo."
        };
        private static readonly Vector3 GateKeeperPosition = new Vector3(-2.25f, 0.75f, 2.85f);
        private static readonly Vector3 TrainingStonePosition = new Vector3(0f, 0.08f, 4.5f);
        private static readonly Vector3 ShadowSlimePosition = new Vector3(3f, 0.4f, 3f);
        private static readonly Vector3 ReadabilityDummyPosition = new Vector3(2.6f, 0.65f, 0.5f);
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
        private Transform _targetDummyHitFlash;
        private Transform _targetDummyFocusRing;
        private Transform _targetDummyCooldownRing;
        private TextMesh _gateKeeperWorldLabel;
        private TextMesh _trainingStoneWorldLabel;
        private TextMesh _targetDummyWorldLabel;
        private TextMesh _spiritGateWorldLabel;
        private TextMesh _shadowSlimeWorldLabel;
        private TextMesh _interactionPromptWorldLabel;
        private SpriteRenderer _targetDummySprite;
        private SpriteRenderer _targetDummyFocusSprite;
        private SpriteRenderer _targetDummyCooldownSprite;
        private SpriteRenderer _targetDummyHitSprite;
        private SpriteRenderer _windSlashSprite;
        private SpriteRenderer _shadowTelegraphSprite;
        private SpriteRenderer _gateKeeperSprite;
        private SpriteRenderer _spiritGateSprite;
        private SpriteRenderer _trainingStoneSprite;
        private SpriteRenderer _shadowSlimeSprite;
        private SpriteRenderer _playerSprite;
        private InteractableState _nearestInteractable;
        private string _objectiveText = "Mục tiêu: vào thế giới và tìm Đá Luyện.";
        private string _interactionText = "Di chuyển tới gần Người Giữ Cổng hoặc Đá Luyện.";
        private GuidedTrainingStep _guidedStep = GuidedTrainingStep.FindGateKeeper;
        private PlaceholderPoseState _playerPoseState = PlaceholderPoseState.Idle;
        private PlaceholderNpcState _gateKeeperState = PlaceholderNpcState.Idle;
        private PlaceholderSlimeState _shadowSlimeState = PlaceholderSlimeState.Idle;
        private PlaceholderVfxFeedbackState _vfxFeedbackState = PlaceholderVfxFeedbackState.Quiet;
        private float _posePulseUntil;
        private float _vfxPreviewUntil;
        private readonly LocalCombatPrototypeState _localCombat = new LocalCombatPrototypeState();
        private LocalCombatPrototypeOutcome _lastLocalCombatOutcome;
        private bool _targetDummyHitAcknowledged;
        private int _dialogueLineIndex;

        public event Action PositionChanged;
        public event Action InteractionStateChanged;

        public Vector3 CurrentPosition => _marker == null ? Vector3.zero : _marker.position;
        public float CurrentYawDegrees => _marker == null ? 0f : _marker.eulerAngles.y;
        public string ObjectiveText => _objectiveText;
        public string InteractionText => _interactionText;
        public string GuidedTrainingStepName => DescribeGuidedTrainingStep();
        public string CurrentAreaLabel => DescribeCurrentArea();
        public string ObjectiveDirectionHint => DescribeObjectiveDirection();
        public string WorldLandmarkSummary => "Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.";
        public string PlayerPoseStateName => DescribePlayerPoseState();
        public string GateKeeperPoseStateName => DescribeGateKeeperState();
        public string ShadowSlimeStateName => DescribeShadowSlimeState();
        public string VfxFeedbackStateName => DescribeVfxFeedbackState();
        public string TargetDummyStatusText => "Mục tiêu luyện tập: sức bền mô phỏng " + _localCombat.TargetHp + "/" + LocalCombatPrototypeState.TargetDummyMaxHp + " - Chỉ là mô phỏng cục bộ.";
        public string TargetDummyRangeText => DescribeTargetDummyRangeState();
        public string TargetDummyVisualStateText => DescribeTargetDummyVisualState();
        public string CombatFeedbackText { get; private set; } = "Chưa phải chiến đấu thật: hãy đứng gần mục tiêu luyện tập để thử phản hồi.";
        public string CombatCooldownText => _localCombat.CooldownActive(NowMs()) ? "Hồi chiêu: Đang hồi chiêu mô phỏng." : "Hồi chiêu: Sẵn sàng";
        public string CombatAuthorityText { get; private set; } = "Mô phỏng cục bộ: chưa gửi ý định chiến đấu.";
        public bool LocalCombatCoolingDown => _localCombat.CooldownActive(NowMs());
        public bool TargetDummyHitAcknowledged => _targetDummyHitAcknowledged;
        public LocalCombatPrototypeOutcome LastLocalCombatOutcome => _lastLocalCombatOutcome;
        public string LocalCombatTargetStateName => _localCombat.TargetState.ToString();
        public bool DialogueActive { get; private set; }
        public bool DialogueCompleted { get; private set; }
        public string DialogueSpeaker => "Người Giữ Cổng";
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
            _localCombat.Reset();
            _targetDummyHitAcknowledged = false;
            CombatFeedbackText = "Chưa phải chiến đấu thật: mục tiêu luyện tập chỉ nhận phản hồi cục bộ.";
            CombatAuthorityText = "Mô phỏng cục bộ: chưa gửi ý định chiến đấu.";
            _guidedStep = GuidedTrainingStep.FindGateKeeper;
            SetPlayerPose(PlaceholderPoseState.Idle);
            SetGateKeeperState(PlaceholderNpcState.Idle);
            SetShadowSlimeState(PlaceholderSlimeState.Idle);
            SetVfxFeedback(PlaceholderVfxFeedbackState.PortalGatePulse, 1.35f);
            _objectiveText = "Mục tiêu 1/2: trò chuyện với Người Giữ Cổng.";
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
            SetSmokePosition(GateKeeperPosition.x + 1.25f, 0.25f, GateKeeperPosition.z - 0.65f, 300f);
        }

        public bool TriggerInteractionForSmoke()
        {
            RefreshInteractionState();
            return TryTriggerInteraction();
        }

        public void SetSmokePositionNearTargetDummy()
        {
            SetSmokePosition(ReadabilityDummyPosition.x - 2.35f, 0.25f, ReadabilityDummyPosition.z - 1.1f, 64f);
        }

        public bool TriggerLocalCombatForSmoke()
        {
            return TryLocalCombatPrototype();
        }

        public CombatIntent BuildCombatIntentForLocalPreview(uint sequence, string intentId)
        {
            var position = CurrentPosition;
            return new CombatIntent
            {
                ProtocolVersion = CombatProtocolVersion,
                Sequence = sequence,
                IntentId = string.IsNullOrWhiteSpace(intentId) ? "unity-local-intent-" + sequence : intentId,
                ActorEntityId = LocalCombatPrototypeState.ActorEntityId,
                TargetEntityId = LocalCombatPrototypeState.TargetDummyEntityId,
                SkillId = LocalCombatPrototypeState.WindSlashSkillId,
                TargetPosition = new LinhGioi.Protocol.V1.Vec3 { X = position.x, Y = position.y, Z = position.z },
                ClientTimeUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                LocalPreviewOnly = true
            };
        }

        public void MarkCombatIntentPending(CombatIntent intent)
        {
            CombatAuthorityText = "Gửi ý định chiến đấu: " + intent.IntentId + " - Đang xác thực - Mô phỏng cục bộ tách riêng kết quả máy chủ.";
            CombatFeedbackText = "Mô phỏng cục bộ: đang hiển thị phản hồi trước khi có Kết quả máy chủ.";
            InteractionStateChanged?.Invoke();
        }

        public void MarkCombatIntentAccepted(CombatAccepted accepted)
        {
            CombatAuthorityText = "Máy chủ chấp nhận: " + accepted.IntentId + " - Kết quả máy chủ đang chờ mô phỏng.";
            InteractionStateChanged?.Invoke();
        }

        public void MarkCombatIntentRejected(CombatRejected rejected)
        {
            CombatAuthorityText = "Máy chủ từ chối: " + rejected.Error.Code + " - giữ nguyên Mô phỏng cục bộ.";
            InteractionStateChanged?.Invoke();
        }

        public void RecoverLocalCombatCooldownForSmoke()
        {
            _localCombat.ForceCooldownReady();
            CombatFeedbackText = "Hồi chiêu: Sẵn sàng - Mô phỏng cục bộ đã hồi phục.";
            RefreshVfxFeedbackMarkers();
            InteractionStateChanged?.Invoke();
        }

        public bool TryLocalCombatPrototype()
        {
            return TryLocalCombatPrototypeAt(NowMs());
        }

        public bool TryLocalCombatPrototypeAt(long nowMs)
        {
            if (_marker == null)
            {
                CombatFeedbackText = "Chưa phải chiến đấu thật: chưa vào sân luyện tập.";
                _localCombat.SetTargetSelected(false);
                var missingIntent = BuildCombatIntentForLocalPreview(0, "unity-local-no-target");
                missingIntent.TargetEntityId = 0;
                missingIntent.ClientTimeUnixMs = nowMs;
                _lastLocalCombatOutcome = _localCombat.TryWindSlash(missingIntent, float.PositiveInfinity, nowMs);
                InteractionStateChanged?.Invoke();
                return false;
            }

            var distance = Distance2D(CurrentPosition, ReadabilityDummyPosition);
            var targetSelected = distance <= LocalCombatTargetSelectionRange;
            _localCombat.SetTargetSelected(targetSelected);
            var nextSequence = (uint)Mathf.Max(1, (int)_localCombat.LastAcceptedSequence + 1);
            var intent = BuildCombatIntentForLocalPreview(nextSequence, "unity-local-preview-" + nextSequence);
            intent.ClientTimeUnixMs = nowMs;
            _lastLocalCombatOutcome = _localCombat.TryWindSlash(intent, distance, nowMs);
            if (!_lastLocalCombatOutcome.Accepted)
            {
                if (_lastLocalCombatOutcome.RejectedReason == "OUT_OF_RANGE")
                    CombatFeedbackText = "Ngoài tầm: lại gần vòng chọn màu vàng quanh mục tiêu luyện tập rồi thử lại.";
                else if (_lastLocalCombatOutcome.RejectedReason == "COOLDOWN_ACTIVE")
                    CombatFeedbackText = "Chưa thể tấn công: Đang hồi chiêu, chờ vòng lam chuyển về xanh sẵn sàng rồi gửi lại ý định.";
                else
                    CombatFeedbackText = "Chưa chọn mục tiêu: đi về phía đông tới bia luyện tập có vòng đánh dấu.";
                CombatAuthorityText = "Từ chối cục bộ: " + _lastLocalCombatOutcome.RejectedReason + " - không tạo kết quả chiến đấu thật.";
                InteractionStateChanged?.Invoke();
                return false;
            }

            _targetDummyHitAcknowledged = true;
            CombatAuthorityText = "Chấp nhận cục bộ: " + _lastLocalCombatOutcome.Intent.IntentId + " - tạo kết quả nguyên mẫu từ hợp đồng hiện có.";
            CombatFeedbackText = "Trúng mục tiêu: Chém Gió gây " + _lastLocalCombatOutcome.EffectAmount + " điểm mô phỏng; bia chuyển đỏ rồi hồi phục. Chỉ là mô phỏng cục bộ.";
            SetPlayerPose(PlaceholderPoseState.Interact);
            SetVfxFeedback(PlaceholderVfxFeedbackState.TargetDummyHitFlash, 1.25f);
            TriggerLocalPosePulse(RuntimeArtCatalog.Gold);
            InteractionStateChanged?.Invoke();
            return true;
        }

        public LocalCombatPrototypeOutcome TryLocalCombatPrototypeWithoutTargetForSmoke(long nowMs)
        {
            _localCombat.SetTargetSelected(false);
            var intent = BuildCombatIntentForLocalPreview(1, "unity-local-no-target");
            intent.TargetEntityId = 0;
            intent.ClientTimeUnixMs = nowMs;
            _lastLocalCombatOutcome = _localCombat.TryWindSlash(intent, float.PositiveInfinity, nowMs);
            CombatFeedbackText = "Chưa chọn mục tiêu: không có bia luyện tập hợp lệ trong ý định.";
            CombatAuthorityText = "Từ chối cục bộ: " + _lastLocalCombatOutcome.RejectedReason + " - không tạo kết quả chiến đấu thật.";
            InteractionStateChanged?.Invoke();
            return _lastLocalCombatOutcome;
        }

        public void PreviewSkillFeedback(string previewName)
        {
            if (string.IsNullOrWhiteSpace(previewName)) return;
            SetPlayerPose(PlaceholderPoseState.Interact);
            if (previewName == "Wind Slash")
            {
                SetVfxFeedback(PlaceholderVfxFeedbackState.WindSlashPreview, 1.25f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Gold);
                _interactionText = "Chỉ xem thử: Chém Gió vẽ một cung vàng, không có đối thủ hay kết quả thật.";
            }
            else if (previewName == "Shadow Bind")
            {
                SetShadowSlimeState(PlaceholderSlimeState.AlertWarning);
                SetVfxFeedback(PlaceholderVfxFeedbackState.ShadowBindWarning, 1.25f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Danger);
                _interactionText = "Chỉ xem thử: Trói Bóng hiển thị vòng cảnh báo dễ đọc trong sân an toàn.";
            }
            else
            {
                SetPlayerPose(PlaceholderPoseState.SpiritChannel);
                SetVfxFeedback(PlaceholderVfxFeedbackState.SpiritPulse, 1.25f);
                TriggerLocalPosePulse(RuntimeArtCatalog.Spirit);
                _interactionText = "Chỉ xem thử: Hộ Linh tạo mạch sáng cho tư thế phòng thủ.";
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
            camera.transform.rotation = Quaternion.Euler(43f, 0f, 0f);
            camera.orthographic = true;
            camera.orthographicSize = 7.0f;
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
            camera.transform.rotation = Quaternion.Euler(43f, 0f, 0f);
            camera.orthographic = true;
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 7.0f, Mathf.Clamp01(Time.deltaTime * 5f));
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
                var material = RuntimeArtCatalog.CreateMaterial("LGO Procedural Cultivation Platform Material v1", RuntimeArtCatalog.SurfaceRaised);
                material.color = Color.white;
                material.mainTexture = CreateTrainingGroundTexture();
                renderer.material = material;
            }
        }

        private static Texture2D CreateTrainingGroundTexture()
        {
            const int size = 512;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "LGO Procedural Cultivation Platform Texture v1",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            var baseColor = new Color(0.095f, 0.145f, 0.215f, 1f);
            var stoneA = new Color(0.13f, 0.19f, 0.285f, 1f);
            var stoneB = new Color(0.085f, 0.135f, 0.205f, 1f);
            var mist = new Color(0.19f, 0.37f, 0.50f, 1f);
            var line = new Color(0.11f, 0.68f, 0.86f, 1f);
            var gold = new Color(0.78f, 0.56f, 0.25f, 1f);
            var center = new Vector2(0.5f, 0.46f);
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var uv = new Vector2((x + 0.5f) / size, (y + 0.5f) / size);
                    var noise = HashNoise(x, y);
                    var slowNoise = HashNoise(x / 8, y / 8);
                    var color = Color.Lerp(baseColor, stoneA, 0.10f + slowNoise * 0.12f);
                    color = Color.Lerp(color, stoneB, noise * 0.035f);

                    var tileX = Mathf.FloorToInt(uv.x * 14f);
                    var tileY = Mathf.FloorToInt(uv.y * 14f);
                    var grout = Mathf.Min(Frac(uv.x * 14f), Frac(uv.y * 14f));
                    grout = Mathf.Min(grout, Mathf.Min(1f - Frac(uv.x * 14f), 1f - Frac(uv.y * 14f)));
                    if (grout < 0.006f) color = Color.Lerp(color, Color.black, 0.055f);
                    if (((tileX + tileY) & 1) == 0) color = Color.Lerp(color, mist, 0.018f);

                    var toCenter = uv - center;
                    var dist = toCenter.magnitude;
                    var innerRing = SmoothBand(dist, 0.145f, 0.0075f);
                    var midRing = SmoothBand(dist, 0.245f, 0.0065f);
                    var outerRing = SmoothBand(dist, 0.355f, 0.009f);
                    color = Color.Lerp(color, line, innerRing * 0.20f);
                    color = Color.Lerp(color, line, midRing * 0.14f);
                    color = Color.Lerp(color, gold, outerRing * 0.13f);
                    var diagonalA = Mathf.Abs(toCenter.x - toCenter.y);
                    var diagonalB = Mathf.Abs(toCenter.x + toCenter.y);
                    if (dist < 0.34f)
                    {
                        color = Color.Lerp(color, line, SmoothBand(diagonalA, 0f, 0.006f) * 0.055f);
                        color = Color.Lerp(color, line, SmoothBand(diagonalB, 0f, 0.006f) * 0.055f);
                    }
                    if (dist < 0.36f)
                    {
                        color = Color.Lerp(color, line, SmoothBand(Mathf.Abs(toCenter.x), 0f, 0.005f) * 0.06f);
                        color = Color.Lerp(color, line, SmoothBand(Mathf.Abs(toCenter.y), 0f, 0.005f) * 0.06f);
                    }

                    var pathToGate = DistanceToSegment(uv, new Vector2(0.50f, 0.28f), new Vector2(0.50f, 0.08f));
                    var pathToStone = DistanceToSegment(uv, new Vector2(0.50f, 0.46f), new Vector2(0.50f, 0.78f));
                    var pathToKeeper = DistanceToSegment(uv, new Vector2(0.50f, 0.46f), new Vector2(0.31f, 0.70f));
                    var guide = Mathf.Min(pathToGate, Mathf.Min(pathToStone, pathToKeeper));
                    color = Color.Lerp(color, line, SmoothBand(guide, 0f, 0.010f) * 0.11f);

                    var platformGlow = Mathf.Clamp01(1f - dist / 0.44f);
                    color = Color.Lerp(color, mist, platformGlow * 0.045f);
                    var vignette = Mathf.Clamp01((dist - 0.18f) / 0.58f);
                    color = Color.Lerp(color, Color.black, vignette * 0.10f);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply(false, true);
            return texture;
        }

        private static float Frac(float value)
        {
            return value - Mathf.Floor(value);
        }

        private static float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var segment = end - start;
            var lengthSq = Vector2.Dot(segment, segment);
            if (lengthSq <= 0.0001f) return Vector2.Distance(point, start);
            var t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / lengthSq);
            return Vector2.Distance(point, start + segment * t);
        }

        private static float SmoothBand(float value, float target, float halfWidth)
        {
            return Mathf.Clamp01(1f - Mathf.Abs(value - target) / Mathf.Max(halfWidth, 0.0001f));
        }

        private static float HashNoise(int x, int y)
        {
            unchecked
            {
                var n = x * 374761393 + y * 668265263;
                n = (n ^ (n >> 13)) * 1274126177;
                return ((n ^ (n >> 16)) & 0xffff) / 65535f;
            }
        }

        private static void EnsureWorldPlaceholders()
        {
            // Preserve the M4 visual source marker: LGO NPC Keeper Placeholder.
            // Readability marker names retained for source validators while runtime prefers sprites over legacy cubes:
            // LGO Gate Keeper Ground Halo / LGO Target Dummy Non Combat Base / LGO Training Stone Cyan Beacon / LGO Safe Training Circle Center.
            if ((LgoVisualAssetRegistryV3B.GateKeeperNpc ?? LgoVisualAssetRegistryV2.GateKeeperNpc) == null)
            {
                CreateMarkerCube("LGO Gate Keeper NPC Interactable", GateKeeperPosition, RuntimeArtCatalog.Gold, new Vector3(0.9f, 1.5f, 0.9f));
                CreateMarkerCube("LGO Gate Keeper Ground Halo", GateKeeperPosition + new Vector3(0f, -0.03f, 0f), RuntimeArtCatalog.Gold, new Vector3(1.15f, 0.035f, 1.15f));
            }

            if (CombatPlaceholderAssets.TargetDummyIdle == null)
            {
                CreateMarkerCube("LGO Target Dummy Readability Marker Legacy Fallback", ReadabilityDummyPosition, RuntimeArtCatalog.Gold, new Vector3(0.42f, 1.15f, 0.42f));
                CreateMarkerCube("LGO Target Dummy Non Combat Base", ReadabilityDummyPosition + new Vector3(0f, -0.58f, 0f), RuntimeArtCatalog.Spirit, new Vector3(1.25f, 0.045f, 1.25f));
            }

            if (LgoVisualAssetRegistryV2.ShadowSlimeAlt == null)
            {
                CreateMarkerCube("LGO Shadow Slime Non Combat Marker", ShadowSlimePosition, RuntimeArtCatalog.Shadow, new Vector3(1.4f, 0.8f, 1.4f));
                CreateMarkerCube("LGO Shadow Slime Warning Plinth", ShadowSlimePosition + new Vector3(0f, -0.2f, 0f), RuntimeArtCatalog.Danger, new Vector3(1.8f, 0.045f, 1.8f));
            }

            if ((LgoVisualAssetRegistryV3B.TrainingStone ?? LgoVisualAssetRegistryV2.TrainingStone) == null)
            {
                CreateMarkerCube("LGO Training Stone Interactable", TrainingStonePosition, RuntimeArtCatalog.Spirit, new Vector3(1.2f, 0.16f, 1.2f));
                CreateMarkerCube("LGO Training Stone Cyan Beacon", TrainingStonePosition + new Vector3(0f, 0.08f, 0f), RuntimeArtCatalog.Spirit, new Vector3(1.45f, 0.045f, 1.45f));
            }

            if ((LgoVisualAssetRegistryV3B.SpiritGate ?? LgoVisualAssetRegistryV2.SpiritGate) == null)
                CreateMarkerCube("LGO Spirit Gate Landmark South", new Vector3(0f, 1.2f, -4.5f), RuntimeArtCatalog.Spirit, new Vector3(2.8f, 2.4f, 0.25f));
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
                SetNearest(null, "Di chuyển tới gần Người Giữ Cổng hoặc Đá Luyện.");
                return;
            }

            var position = _marker.position;
            var training = new InteractableState(
                "Training Stone",
                "Nhấn F hoặc Space: ổn định mạch linh khí.",
                "Mạch linh khí đã ổn định. Đã ghi nhận luyện tập.",
                TrainingStonePosition
            );
            var keeper = new InteractableState(
                "Gate Keeper",
                "Nhấn F hoặc Space: xin chỉ dẫn từ Người Giữ Cổng.",
                "Người Giữ Cổng: đường đã mở. Hãy thử Đá Luyện.",
                GateKeeperPosition
            );

            var nearest = _guidedStep == GuidedTrainingStep.FindGateKeeper ? keeper : training;
            if (Distance2D(position, nearest.position) <= InteractionRange)
                SetNearest(nearest, nearest.prompt);
            else if (_guidedStep == GuidedTrainingStep.FindGateKeeper && Distance2D(position, training.position) <= InteractionRange)
                SetNearest(training, training.prompt);
            else
                SetNearest(null, InteractionAcknowledged ? "Vòng hướng dẫn hoàn tất: lưu vị trí hoặc quay lại sảnh." : NextMovementHint());
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
            _objectiveText = "Mục tiêu hoàn tất: mạch linh khí đã ổn định.";
            _interactionText = "Mạch linh khí đã ổn định. Đã ghi nhận luyện tập.";
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
            _objectiveText = "Mục tiêu 2/2: ổn định Đá Luyện.";
            _interactionText = "Người Giữ Cổng: đường đã mở. Hãy đi theo mạch linh khí lam về phía bắc.";
            RefreshInteractionPromptWorldLabel();
            RefreshInteractionState();
            InteractionStateChanged?.Invoke();
            return true;
        }

        private void OpenGateKeeperDialogue()
        {
            DialogueActive = true;
            DialogueCompleted = false;
            _dialogueLineIndex = 0;
            _objectiveText = "Mục tiêu 1/2: lắng nghe Người Giữ Cổng.";
            _interactionText = DialogueLine;
            RefreshInteractionPromptWorldLabel();
        }

        private string NextMovementHint()
        {
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "Bước 1: đi về phía Người Giữ Cổng màu vàng ở góc tây bắc sân luyện.";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "Bước 2: đi theo mạch sáng lam tới Đá Luyện ở phía bắc.";
            return "Vòng hướng dẫn hoàn tất: lưu vị trí hoặc quay lại sảnh.";
        }

        private string DescribeObjectiveDirection()
        {
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "hướng tây bắc để gặp Người Giữ Cổng.";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "hướng bắc để tập trung vào Đá Luyện.";
            return "luyện tập hoàn tất; có thể lưu vị trí hoặc về Điện Nhân Vật.";
        }

        private string DescribeCurrentArea()
        {
            if (_nearestInteractable != null) return _nearestInteractable.id;
            if (Distance2D(CurrentPosition, ReadabilityDummyPosition) <= LocalCombatPrototypeState.WindSlashRangeM)
                return "Sân luyện an toàn / Mục tiêu luyện tập";
            if (Distance2D(CurrentPosition, ShadowSlimePosition) <= 2.25f)
            {
                SetShadowSlimeState(PlaceholderSlimeState.AlertWarning);
                SetVfxFeedback(PlaceholderVfxFeedbackState.ShadowBindWarning, 1.2f);
                return "Sân luyện an toàn / cảnh báo bóng phía đông";
            }
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "Sân luyện an toàn / đường tới Người Giữ Cổng";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "Sân luyện an toàn / đường tới Đá Luyện";
            return "Sân luyện an toàn / luyện tập hoàn tất";
        }

        private string DescribeGuidedTrainingStep()
        {
            if (_guidedStep == GuidedTrainingStep.FindGateKeeper) return "Bước 1: tìm Người Giữ Cổng";
            if (_guidedStep == GuidedTrainingStep.FindTrainingStone) return "Bước 2: ổn định Đá Luyện";
            return "Hoàn tất vòng hướng dẫn";
        }

        private string DescribePlayerPoseState()
        {
            if (_playerPoseState == PlaceholderPoseState.WalkMove) return "đang di chuyển";
            if (_playerPoseState == PlaceholderPoseState.Interact) return "đang tương tác";
            if (_playerPoseState == PlaceholderPoseState.SpiritChannel) return "dẫn linh khí";
            return "đứng yên";
        }

        private string DescribeGateKeeperState()
        {
            return _gateKeeperState == PlaceholderNpcState.TalkGuide ? "đang chỉ dẫn" : "chờ";
        }

        private string DescribeShadowSlimeState()
        {
            if (_shadowSlimeState == PlaceholderSlimeState.AlertWarning) return "cảnh báo";
            if (_shadowSlimeState == PlaceholderSlimeState.DissolveQuiet) return "tan dần";
            return "đứng yên";
        }

        private string DescribeVfxFeedbackState()
        {
            if (_vfxFeedbackState == PlaceholderVfxFeedbackState.PortalGatePulse) return "mạch sáng Linh Môn";
            if (_vfxFeedbackState == PlaceholderVfxFeedbackState.WindSlashPreview) return "Chém Gió xem thử";
            if (_vfxFeedbackState == PlaceholderVfxFeedbackState.SpiritPulse) return "mạch linh khí";
            if (_vfxFeedbackState == PlaceholderVfxFeedbackState.ShadowBindWarning) return "vòng cảnh báo Trói Bóng";
            if (_vfxFeedbackState == PlaceholderVfxFeedbackState.TargetDummyHitFlash) return "lóe sáng trúng bia";
            return "yên tĩnh";
        }

        private void SetNearest(InteractableState state, string text)
        {
            if (_nearestInteractable == state && _interactionText == text) return;
            _nearestInteractable = state;
            _interactionText = text;
            RefreshInteractionPromptWorldLabel();
            InteractionStateChanged?.Invoke();
        }

        private void RefreshInteractionPromptWorldLabel()
        {
            if (_interactionPromptWorldLabel == null) return;
            var active = _nearestInteractable != null && !DialogueActive;
            _interactionPromptWorldLabel.gameObject.SetActive(active);
            if (!active) return;

            _interactionPromptWorldLabel.transform.position = _nearestInteractable.position + new Vector3(0f, 2.2f, 0f);
            _interactionPromptWorldLabel.text = "F / Space";
            _interactionPromptWorldLabel.color = _nearestInteractable.id == "Gate Keeper" ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;
            EnsureWorldLabelShadow(_interactionPromptWorldLabel.transform, _interactionPromptWorldLabel.text);
        }

        private static void SetWorldLabel(TextMesh label, string text, Color color)
        {
            label.text = text;
            label.color = color;
            EnsureWorldLabelShadow(label.transform, text);
        }

        private static float Distance2D(Vector3 a, Vector3 b)
        {
            var dx = a.x - b.x;
            var dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        private static long NowMs()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
            if (_playerSprite == null)
                _playerSprite = CreateBillboardSprite("LGO Player Cultivator Runtime Sprite V3B", LgoVisualAssetRegistryV3B.PlayerMaleCultivator ?? LgoVisualAssetRegistryV2.PlayerMaleCultivator, CurrentPosition + Vector3.up * 0.22f, new Vector3(0.64f, 0.64f, 1f), 8);
            if (_playerSprite != null)
            {
                _playerSprite.transform.position = CurrentPosition + Vector3.up * 0.22f;
                _playerSprite.gameObject.SetActive(true);
                if (_markerRenderer != null) _markerRenderer.enabled = false;
            }
            if (_portalGatePulse == null)
                _portalGatePulse = CreateMarkerCube("LGO Portal Gate Pulse Placeholder", new Vector3(0f, 0.18f, -4.5f), RuntimeArtCatalog.Spirit, new Vector3(3.2f, 0.08f, 0.55f)).transform;
            if (_windSlashPreview == null)
                _windSlashPreview = CreateMarkerCube("LGO Wind Slash Preview Placeholder", CurrentPosition + new Vector3(0f, 0.7f, 0.9f), RuntimeArtCatalog.Gold, new Vector3(1.8f, 0.12f, 0.22f)).transform;
            if (_shadowBindWarning == null)
                _shadowBindWarning = CreateMarkerCube("LGO Shadow Bind Warning Placeholder", ShadowSlimePosition + Vector3.up * 0.55f, RuntimeArtCatalog.Shadow, new Vector3(1.9f, 0.1f, 1.9f)).transform;
            if (_targetDummyHitFlash == null)
                _targetDummyHitFlash = CreateMarkerCube("LGO Target Dummy Local Hit Flash", ReadabilityDummyPosition + Vector3.up * 1.2f, RuntimeArtCatalog.Danger, new Vector3(1.05f, 0.16f, 1.05f)).transform;
            if (_targetDummyFocusRing == null)
                _targetDummyFocusRing = CreateMarkerCube("LGO Target Dummy Focus Ring v0.37", ReadabilityDummyPosition + Vector3.up * 0.03f, RuntimeArtCatalog.Gold, new Vector3(1.65f, 0.06f, 1.65f)).transform;
            if (_targetDummyCooldownRing == null)
                _targetDummyCooldownRing = CreateMarkerCube("LGO Target Dummy Cooldown Ring v0.37", ReadabilityDummyPosition + Vector3.up * 0.09f, RuntimeArtCatalog.Spirit, new Vector3(1.95f, 0.05f, 1.95f)).transform;
            EnsureCombatPlaceholderSprites();
            RefreshVfxFeedbackMarkers();
        }

        private void EnsureCombatPlaceholderSprites()
        {
            if (_targetDummySprite == null)
                _targetDummySprite = CreateBillboardSprite("LGO Target Dummy Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TargetDummyIdle ?? CombatPlaceholderAssets.TargetDummyIdle, ReadabilityDummyPosition + Vector3.up * 0.15f, new Vector3(0.58f, 0.58f, 1f), 5);
            if (_targetDummyFocusSprite == null)
                _targetDummyFocusSprite = CreateBillboardSprite("LGO Target Marker Selected Sprite v0.46", CombatPlaceholderAssets.TargetMarkerSelected, ReadabilityDummyPosition + Vector3.up * 0.08f, new Vector3(0.86f, 0.86f, 1f), 4);
            if (_targetDummyCooldownSprite == null)
                _targetDummyCooldownSprite = CreateBillboardSprite("LGO Target Cooldown Ring Sprite v0.46", LgoVisualAssetRegistryV3B.CooldownActive ?? CombatPlaceholderAssets.CooldownActive, ReadabilityDummyPosition + Vector3.up * 0.1f, new Vector3(0.96f, 0.96f, 1f), 4);
            if (_targetDummyHitSprite == null)
                _targetDummyHitSprite = CreateBillboardSprite("LGO Target Impact Spark Sprite v0.46", LgoVisualAssetRegistryV3B.ImpactSpark ?? CombatPlaceholderAssets.ImpactSpark, ReadabilityDummyPosition + Vector3.up * 1.05f, new Vector3(0.54f, 0.54f, 1f), 7);
            if (_windSlashSprite == null)
                _windSlashSprite = CreateBillboardSprite("LGO Wind Slash Runtime Sprite v0.46", LgoVisualAssetRegistryV3B.WindSlashFrame01 ?? CombatPlaceholderAssets.WindSlashFrame01, CurrentPosition + new Vector3(0f, 0.85f, 0.9f), new Vector3(0.78f, 0.78f, 1f), 6);
            if (_shadowTelegraphSprite == null)
                _shadowTelegraphSprite = CreateBillboardSprite("LGO Warning Telegraph Circle Sprite v0.46", CombatPlaceholderAssets.WarningTelegraphCircle, ShadowSlimePosition + Vector3.up * 0.16f, new Vector3(1.08f, 1.08f, 1f), 4);
            if (_gateKeeperSprite == null)
                _gateKeeperSprite = CreateBillboardSprite("LGO Gate Keeper Runtime Sprite V3B", LgoVisualAssetRegistryV3B.GateKeeperNpc ?? LgoVisualAssetRegistryV2.GateKeeperNpc, GateKeeperPosition + Vector3.up * 0.2f, new Vector3(0.72f, 0.72f, 1f), 5);
            if (_spiritGateSprite == null)
                _spiritGateSprite = CreateBillboardSprite("LGO Spirit Gate Runtime Sprite V3B", LgoVisualAssetRegistryV3B.SpiritGate ?? LgoVisualAssetRegistryV2.SpiritGate, new Vector3(0f, 0.35f, -4.5f), new Vector3(0.58f, 0.58f, 1f), 3);
            if (_trainingStoneSprite == null)
                _trainingStoneSprite = CreateBillboardSprite("LGO Training Stone Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TrainingStone ?? LgoVisualAssetRegistryV2.TrainingStone, TrainingStonePosition + Vector3.up * 0.2f, new Vector3(0.62f, 0.62f, 1f), 5);
            if (_shadowSlimeSprite == null)
                _shadowSlimeSprite = CreateBillboardSprite("LGO Shadow Slime Runtime Sprite V3B", LgoVisualAssetRegistryV3B.ShadowSlime ?? LgoVisualAssetRegistryV2.ShadowSlimeAlt, ShadowSlimePosition + Vector3.up * 0.25f, new Vector3(0.74f, 0.74f, 1f), 5);
            EnsureWorldSetDressing();
            if (_gateKeeperWorldLabel == null)
                _gateKeeperWorldLabel = CreateWorldLabel("LGO Gate Keeper World Label", "Người Giữ Cổng", GateKeeperPosition + new Vector3(0f, 1.95f, 0f), RuntimeArtCatalog.Gold);
            if (_trainingStoneWorldLabel == null)
                _trainingStoneWorldLabel = CreateWorldLabel("LGO Training Stone World Label", "Đá Luyện", TrainingStonePosition + new Vector3(0f, 1.25f, 0f), RuntimeArtCatalog.Spirit);
            if (_targetDummyWorldLabel == null)
                _targetDummyWorldLabel = CreateWorldLabel("LGO Target Dummy World Label", "Bia luyện", ReadabilityDummyPosition + new Vector3(0f, 1.55f, 0f), RuntimeArtCatalog.Gold);
            if (_spiritGateWorldLabel == null)
                _spiritGateWorldLabel = CreateWorldLabel("LGO Spirit Gate World Label", "Linh Môn", new Vector3(0f, 2.15f, -4.5f), RuntimeArtCatalog.Spirit);
            if (_shadowSlimeWorldLabel == null)
                _shadowSlimeWorldLabel = CreateWorldLabel("LGO Shadow Slime World Label", "Cảnh báo", ShadowSlimePosition + new Vector3(0f, 1.1f, 0f), RuntimeArtCatalog.Danger);
            if (_interactionPromptWorldLabel == null)
            {
                _interactionPromptWorldLabel = CreateWorldLabel("LGO Interaction Prompt World Label", "F / Space", GateKeeperPosition + new Vector3(0f, 2.35f, 0f), RuntimeArtCatalog.Spirit);
                _interactionPromptWorldLabel.fontSize = 42;
                _interactionPromptWorldLabel.characterSize = 0.044f;
                _interactionPromptWorldLabel.gameObject.SetActive(false);
            }
        }

        private static void EnsureWorldSetDressing()
        {
            CreateBillboardSprite("LGO World Cherry Tree Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreeCherry ?? LgoVisualAssetRegistryV2.TreeCherry, new Vector3(-4.8f, 0.2f, 1.4f), new Vector3(1.12f, 1.12f, 1f), 1);
            CreateBillboardSprite("LGO World Pine Tree Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreePine ?? LgoVisualAssetRegistryV2.TreePine, new Vector3(4.8f, 0.2f, 2.1f), new Vector3(1.02f, 1.02f, 1f), 1);
            CreateBillboardSprite("LGO World Cherry Tree Far Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreeCherry ?? LgoVisualAssetRegistryV2.TreeCherry, new Vector3(4.6f, 0.16f, -0.65f), new Vector3(0.62f, 0.62f, 1f), 0);
            CreateBillboardSprite("LGO World Pine Tree Far Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreePine ?? LgoVisualAssetRegistryV2.TreePine, new Vector3(-5.25f, 0.16f, 3.65f), new Vector3(0.68f, 0.68f, 1f), 0);
            CreateBillboardSprite("LGO World Lantern West Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, new Vector3(-4.2f, 0.2f, -1.8f), new Vector3(0.68f, 0.68f, 1f), 2);
            CreateBillboardSprite("LGO World Lantern East Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, new Vector3(4.1f, 0.2f, -1.7f), new Vector3(0.68f, 0.68f, 1f), 2);
            CreateBillboardSprite("LGO World Lantern North Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, new Vector3(-0.95f, 0.15f, 5.35f), new Vector3(0.42f, 0.42f, 1f), 1);
            CreateBillboardSprite("LGO World Lantern South Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, new Vector3(1.15f, 0.15f, -5.25f), new Vector3(0.42f, 0.42f, 1f), 1);
            CreateBillboardSprite("LGO World Rock Moss Runtime Sprite V3B", LgoVisualAssetRegistryV3B.RockMoss ?? LgoVisualAssetRegistryV2.RockMoss, new Vector3(-1.9f, 0.15f, -1.7f), new Vector3(0.58f, 0.58f, 1f), 1);
            CreateBillboardSprite("LGO World Rock Moss East Runtime Sprite V3B", LgoVisualAssetRegistryV3B.RockMoss ?? LgoVisualAssetRegistryV2.RockMoss, new Vector3(2.1f, 0.12f, 4.75f), new Vector3(0.34f, 0.34f, 1f), 0);
            CreateBillboardSprite("LGO World Cultivation Banner Runtime Sprite V3B", LgoVisualAssetRegistryV3B.BannerCultivation ?? LgoVisualAssetRegistryV2.BannerCultivation, new Vector3(3.8f, 0.2f, -3.6f), new Vector3(0.56f, 0.56f, 1f), 2);
            CreateBillboardSprite("LGO World Cultivation Banner West Runtime Sprite V3B", LgoVisualAssetRegistryV3B.BannerCultivation ?? LgoVisualAssetRegistryV2.BannerCultivation, new Vector3(-4.95f, 0.16f, -3.25f), new Vector3(0.42f, 0.42f, 1f), 1);
            CreateBillboardSprite("LGO World Bridge Wood Runtime Sprite V3B", LgoVisualAssetRegistryV3B.BridgeWood ?? LgoVisualAssetRegistryV2.BridgeWood, new Vector3(-2.9f, 0.1f, -3.8f), new Vector3(0.82f, 0.82f, 1f), 1);
        }

        private static SpriteRenderer CreateBillboardSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, int sortingOrder)
        {
            if (sprite == null) return null;
            var existing = GameObject.Find(name);
            var holder = existing != null ? existing : new GameObject(name);
            holder.transform.position = position;
            holder.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            holder.transform.localScale = scale;
            var renderer = holder.GetComponent<SpriteRenderer>() ?? holder.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static TextMesh CreateWorldLabel(string name, string text, Vector3 position, Color color)
        {
            var existing = GameObject.Find(name);
            var holder = existing != null ? existing : new GameObject(name);
            holder.transform.position = position;
            holder.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            var label = holder.GetComponent<TextMesh>() ?? holder.AddComponent<TextMesh>();
            label.text = text;
            label.fontSize = 48;
            label.characterSize = 0.048f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = color;
            var renderer = holder.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.sortingOrder = 9;
            EnsureWorldLabelShadow(holder.transform, text);
            return label;
        }

        private static void EnsureWorldLabelShadow(Transform parent, string text)
        {
            var shadowName = parent.name + " Shadow";
            var existing = parent.Find(shadowName);
            var holder = existing != null ? existing.gameObject : new GameObject(shadowName);
            holder.transform.SetParent(parent, false);
            holder.transform.localPosition = new Vector3(0.025f, -0.025f, 0.01f);
            holder.transform.localRotation = Quaternion.identity;
            holder.transform.localScale = Vector3.one;
            var shadow = holder.GetComponent<TextMesh>() ?? holder.AddComponent<TextMesh>();
            shadow.text = text;
            shadow.fontSize = 48;
            shadow.characterSize = 0.048f;
            shadow.anchor = TextAnchor.MiddleCenter;
            shadow.alignment = TextAlignment.Center;
            shadow.color = new Color(0f, 0f, 0f, 0.72f);
            var renderer = holder.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.sortingOrder = 8;
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
                _portalGatePulse.gameObject.SetActive(_spiritGateSprite == null && active && _vfxFeedbackState == PlaceholderVfxFeedbackState.PortalGatePulse);
            if (_windSlashPreview != null)
            {
                _windSlashPreview.position = CurrentPosition + _marker.forward * 0.9f + Vector3.up * 0.7f;
                _windSlashPreview.rotation = _marker.rotation;
                _windSlashPreview.gameObject.SetActive(_windSlashSprite == null && active && _vfxFeedbackState == PlaceholderVfxFeedbackState.WindSlashPreview);
            }
            if (_windSlashSprite != null)
            {
                _windSlashSprite.transform.position = CurrentPosition + _marker.forward * 0.9f + Vector3.up * 0.88f;
                _windSlashSprite.transform.rotation = Quaternion.Euler(42f, _marker.eulerAngles.y, 0f);
                _windSlashSprite.gameObject.SetActive(active && _vfxFeedbackState == PlaceholderVfxFeedbackState.WindSlashPreview);
            }
            if (_shadowBindWarning != null)
                _shadowBindWarning.gameObject.SetActive(_shadowTelegraphSprite == null && active && _vfxFeedbackState == PlaceholderVfxFeedbackState.ShadowBindWarning);
            if (_shadowTelegraphSprite != null)
                _shadowTelegraphSprite.gameObject.SetActive(active && _vfxFeedbackState == PlaceholderVfxFeedbackState.ShadowBindWarning);
            if (_targetDummyHitFlash != null)
                _targetDummyHitFlash.gameObject.SetActive(_targetDummyHitSprite == null && active && _vfxFeedbackState == PlaceholderVfxFeedbackState.TargetDummyHitFlash);
            if (_targetDummyHitSprite != null)
                _targetDummyHitSprite.gameObject.SetActive(active && _vfxFeedbackState == PlaceholderVfxFeedbackState.TargetDummyHitFlash);
            RefreshTargetDummyReadabilityMarkers(active);
        }

        private void RefreshTargetDummyReadabilityMarkers(bool vfxActive)
        {
            var nearTarget = _marker != null && Distance2D(CurrentPosition, ReadabilityDummyPosition) <= LocalCombatPrototypeState.WindSlashRangeM;
            var coolingDown = _localCombat.CooldownActive(NowMs());
            if (_targetDummyFocusRing != null)
                _targetDummyFocusRing.gameObject.SetActive(_targetDummyFocusSprite == null && nearTarget && !coolingDown);
            if (_targetDummyCooldownRing != null)
                _targetDummyCooldownRing.gameObject.SetActive(_targetDummyCooldownSprite == null && (coolingDown || (nearTarget && vfxActive && _vfxFeedbackState == PlaceholderVfxFeedbackState.TargetDummyHitFlash)));
            if (_targetDummyFocusSprite != null)
                _targetDummyFocusSprite.gameObject.SetActive(nearTarget && !coolingDown);
            if (_targetDummyCooldownSprite != null)
            {
                _targetDummyCooldownSprite.sprite = coolingDown
                    ? LgoVisualAssetRegistryV3B.CooldownActive ?? CombatPlaceholderAssets.CooldownActive
                    : LgoVisualAssetRegistryV3B.CooldownReady ?? CombatPlaceholderAssets.CooldownReady;
                _targetDummyCooldownSprite.gameObject.SetActive(coolingDown || nearTarget);
            }
            if (_targetDummySprite != null)
            {
                _targetDummySprite.sprite = ResolveTargetDummyStateSprite(nearTarget, coolingDown, vfxActive);
            }
            if (_targetDummyWorldLabel != null)
            {
                if (vfxActive && _vfxFeedbackState == PlaceholderVfxFeedbackState.TargetDummyHitFlash)
                    SetWorldLabel(_targetDummyWorldLabel, "Trúng mục tiêu", RuntimeArtCatalog.Gold);
                else if (coolingDown)
                    SetWorldLabel(_targetDummyWorldLabel, "Đang hồi phục", RuntimeArtCatalog.Spirit);
                else if (nearTarget)
                    SetWorldLabel(_targetDummyWorldLabel, "Đã chọn", RuntimeArtCatalog.Spirit);
                else
                    SetWorldLabel(_targetDummyWorldLabel, "Bia luyện", RuntimeArtCatalog.Gold);
            }
        }

        private Sprite ResolveTargetDummyStateSprite(bool nearTarget, bool coolingDown, bool vfxActive)
        {
            var idle = LgoVisualAssetRegistryV3B.TargetDummyIdle ?? CombatPlaceholderAssets.TargetDummyIdle;
            if (vfxActive && _vfxFeedbackState == PlaceholderVfxFeedbackState.TargetDummyHitFlash)
                return LgoVisualAssetRegistryV3B.TargetDummyHit ?? CombatPlaceholderAssets.TargetDummyHit ?? idle;
            if (coolingDown)
                return LgoVisualAssetRegistryV3B.TargetDummyRecover ?? CombatPlaceholderAssets.TargetDummyRecover ?? idle;
            if (nearTarget)
                return LgoVisualAssetRegistryV3B.TargetDummySelected ?? CombatPlaceholderAssets.TargetDummySelected ?? idle;
            return idle;
        }

        private string DescribeTargetDummyVisualState()
        {
            if (_marker == null) return "Dấu hiệu mục tiêu: Chưa vào sân luyện.";
            if (_localCombat.CooldownActive(NowMs())) return "Dấu hiệu mục tiêu: Bia hồi phục màu xanh xám, vòng hồi chiêu lam/vàng đang chạy.";
            if (Distance2D(CurrentPosition, ReadabilityDummyPosition) <= LocalCombatPrototypeState.WindSlashRangeM) return "Dấu hiệu mục tiêu: Đã chọn, vòng tâm ngắm xanh và vòng vàng đang sáng.";
            return "Dấu hiệu mục tiêu: Chưa chọn, bia ở phía đông ngoài vòng tấn công thử.";
        }

        private string DescribeTargetDummyRangeState()
        {
            if (_marker == null) return "Tầm đánh: chưa vào sân.";
            var distance = Distance2D(CurrentPosition, ReadabilityDummyPosition);
            if (distance <= LocalCombatPrototypeState.WindSlashRangeM)
                return "Tầm đánh: trong tầm " + distance.ToString("0.0", CultureInfo.InvariantCulture) + "m / sẵn sàng gửi ý định.";
            return "Tầm đánh: ngoài tầm " + distance.ToString("0.0", CultureInfo.InvariantCulture) + "m / cần <= " + LocalCombatPrototypeState.WindSlashRangeM.ToString("0.0", CultureInfo.InvariantCulture) + "m.";
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
            ShadowBindWarning,
            TargetDummyHitFlash
        }
    }
}
