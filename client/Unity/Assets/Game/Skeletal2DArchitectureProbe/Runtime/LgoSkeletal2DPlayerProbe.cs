#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace LinhGioi.ArchitectureProbe
{
    public sealed class LgoSkeletal2DPlayerProbe : MonoBehaviour
    {
        public readonly struct ScenarioStep
        {
            public ScenarioStep(float atSeconds, string state) { AtSeconds = atSeconds; State = state; }
            public float AtSeconds { get; }
            public string State { get; }
        }

        public static readonly ScenarioStep[] Scenario =
        {
            new ScenarioStep(0.20f, "idle"), new ScenarioStep(1.00f, "run"),
            new ScenarioStep(2.20f, "jump"), new ScenarioStep(2.85f, "fall"),
            new ScenarioStep(3.45f, "land"), new ScenarioStep(3.80f, "run"),
            new ScenarioStep(4.80f, "attack"), new ScenarioStep(5.80f, "return_to_idle"),
            new ScenarioStep(6.45f, "run"),
        };

        public const float QuitAtSeconds = 11f;

        public readonly struct CutoutAxisMetrics
        {
            public CutoutAxisMetrics(Vector2 topCenter, Vector2 bottomCenter)
            {
                TopCenter = topCenter;
                BottomCenter = bottomCenter;
            }

            public Vector2 TopCenter { get; }
            public Vector2 BottomCenter { get; }
            public Vector2 TopToBottom => BottomCenter - TopCenter;
        }

        [Serializable]
        private sealed class RuntimeReport
        {
            public string status, unityVersion, renderer, drawCallMeasurementStatus, seamMeasurementStatus, geometryMeasurementLimit;
            public string[] statesPlayed, failures;
            public int transitionCount, measuredFrames, bodyCutoutPartCount, poseSpecificGarmentSourceCount, restMasterCount;
            public bool equipmentSwapDuringRun, equipmentSwapPreservedState, authoredGarmentWeights, runtimePromotionAllowed;
            public float stateTimeBeforeSwap, stateTimeAfterSwap, rootScaleMaxDrift, boneLengthMaxDriftRatio;
            public float rigidItemScaleMaxDriftRatio, frameTimeP95Ms, seamGapMaxSourcePixels;
            public int softTriangleInversionMax;
            public long peakAllocatedMemoryBytes, peakDrawCalls;
        }

        [SerializeField] private Transform _actorRoot, _pelvis, _head;
        [SerializeField] private Transform[] _bones;
        [SerializeField] private GameObject _upperA, _upperB;
        [SerializeField] private Transform _rigidItem;
        [SerializeField] private SpriteSkin _upperSkin;
        private SpriteRenderer _upperRenderer;
        private readonly List<float> _frameTimes = new List<float>();
        private readonly List<string> _states = new List<string>();
        private readonly List<string> _failures = new List<string>();
        private readonly Dictionary<string, Transform> _boneByName = new Dictionary<string, Transform>();
        private Vector3 _initialRootScale, _initialRigidScale, _pelvisRestLocalPosition;
        private float[] _boneLengths, _triangleSigns;
        private Vector3[] _upperVertices;
        private BoneWeight[] _upperWeights;
        private ushort[] _upperIndices;
        private Matrix4x4[] _upperBindPoses;
        private ProfilerRecorder _drawCalls;
        private float _elapsed, _stateElapsed, _swapStartedAt, _beforeSwap, _afterSwap, _rootScaleDrift, _boneDrift, _rigidScaleDrift;
        private int _nextStep, _triangleInversions;
        private bool _swapStarted, _swapFinished;
        private long _peakMemory, _peakDrawCalls;
        private string _outputDirectory, _currentState = "boot";

        public void Configure(Transform actorRoot, Transform pelvis, Transform head, Transform[] bones,
            GameObject upperA, GameObject upperB, Transform rigidItem, SpriteSkin upperSkin)
        {
            _actorRoot = actorRoot; _pelvis = pelvis; _head = head; _bones = bones;
            _upperA = upperA; _upperB = upperB; _rigidItem = rigidItem; _upperSkin = upperSkin;
        }

        public static Vector2[] PreserveChainLengths(Vector2[] bind, Vector2[] target)
        {
            if (bind == null || target == null || bind.Length != target.Length || bind.Length < 2)
                throw new ArgumentException("Bind and target chains must have the same length of at least two");
            var solved = new Vector2[bind.Length];
            solved[0] = target[0];
            for (var index = 1; index < bind.Length; index++)
            {
                var bindLength = Vector2.Distance(bind[index - 1], bind[index]);
                var direction = target[index] - target[index - 1];
                if (bindLength <= 0.000001f || direction.sqrMagnitude <= 0.000001f)
                    throw new ArgumentException("Bone length and target direction must be non-zero");
                solved[index] = solved[index - 1] + direction.normalized * bindLength;
            }
            return solved;
        }

        public static float Percentile95(IEnumerable<float> values)
        {
            var sorted = values.OrderBy(value => value).ToArray();
            if (sorted.Length == 0) return 0f;
            return sorted[Mathf.Clamp(Mathf.CeilToInt(sorted.Length * .95f) - 1, 0, sorted.Length - 1)];
        }

        public static CutoutAxisMetrics MeasureCutoutAxis(Color32[] pixels, int width, int height, float endpointBandRatio = .08f)
        {
            if (pixels == null || width <= 0 || height <= 0 || pixels.Length != width * height)
                throw new ArgumentException("Cutout pixels and dimensions must describe one image");
            if (endpointBandRatio <= 0f || endpointBandRatio > .5f)
                throw new ArgumentOutOfRangeException(nameof(endpointBandRatio));

            var minY = height;
            var maxY = -1;
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                if (pixels[y * width + x].a == 0) continue;
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y);
            }
            if (maxY < minY) throw new ArgumentException("Cutout alpha is empty", nameof(pixels));

            var band = Mathf.Max(2, Mathf.RoundToInt((maxY - minY + 1) * endpointBandRatio));
            Vector2 WeightedCenter(int fromY, int toY)
            {
                double sumX = 0, sumY = 0, totalAlpha = 0;
                for (var y = fromY; y <= toY; y++)
                for (var x = 0; x < width; x++)
                {
                    var alpha = pixels[y * width + x].a;
                    if (alpha == 0) continue;
                    sumX += (x + .5) * alpha;
                    sumY += (y + .5) * alpha;
                    totalAlpha += alpha;
                }
                if (totalAlpha <= 0) throw new ArgumentException("Cutout endpoint band is empty", nameof(pixels));
                return new Vector2((float)(sumX / totalAlpha), (float)(sumY / totalAlpha));
            }

            // Texture pixels use a bottom-left origin. The highest alpha band is
            // the anatomical top endpoint and the lowest band is the bottom.
            return new CutoutAxisMetrics(
                WeightedCenter(Mathf.Max(minY, maxY - band + 1), maxY),
                WeightedCenter(minY, Mathf.Min(maxY, minY + band - 1)));
        }

        public static float RegisteredUniformScale(float sourceAxisLengthPixels, float pixelsPerUnit, float bindLengthWorld)
        {
            if (sourceAxisLengthPixels <= 0f || pixelsPerUnit <= 0f || bindLengthWorld <= 0f)
                throw new ArgumentOutOfRangeException("Registered scale inputs must be positive");
            return bindLengthWorld / (sourceAxisLengthPixels / pixelsPerUnit);
        }

        private void Awake()
        {
            Application.runInBackground = true; QualitySettings.vSyncCount = 0; Application.targetFrameRate = 60;
            _outputDirectory = ReadOption("--lgo-probe-output") ?? Path.Combine(Application.persistentDataPath, "lgo-skeletal-2d-player-probe");
            Directory.CreateDirectory(_outputDirectory);
            if (_actorRoot == null || _pelvis == null || _head == null || _bones == null || _bones.Length < 2
                || _upperA == null || _upperB == null || _rigidItem == null || _upperSkin == null)
            {
                _failures.Add("RUNTIME_REFERENCE_MISSING"); WriteReportAndQuit(); enabled = false; return;
            }
            _initialRootScale = _actorRoot.localScale; _initialRigidScale = _rigidItem.lossyScale;
            _upperRenderer = _upperSkin.GetComponent<SpriteRenderer>();
            _pelvisRestLocalPosition = _pelvis.localPosition;
            foreach (var bone in _bones) _boneByName[bone.name] = bone;
            _boneLengths = new float[_bones.Length - 1];
            for (var i = 1; i < _bones.Length; i++) _boneLengths[i - 1] = Vector3.Distance(_bones[i].position, _bones[i].parent.position);
            try { CaptureUpperGeometry(); }
            catch (Exception error)
            {
                _failures.Add("GEOMETRY_CAPTURE_FAILED:" + error.GetType().Name + ":" + error.Message);
                WriteReportAndQuit(); enabled = false; return;
            }
            try { _drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 32); }
            catch (Exception error) { _failures.Add("DRAW_CALL_RECORDER_UNAVAILABLE:" + error.GetType().Name); }
        }

        private void CaptureUpperGeometry()
        {
            var sprite = _upperRenderer.sprite;
            _upperVertices = sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position).ToArray();
            _upperWeights = sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight).ToArray();
            _upperIndices = sprite.GetIndices().ToArray(); _upperBindPoses = sprite.GetBindPoses().ToArray();
            _triangleSigns = TriangleSigns(DeformedUpperVertices());
        }

        private void Update()
        {
            // Player startup can report one very large delta after the native
            // window is created. Advance the deterministic review scenario in
            // bounded steps while retaining the raw delta for performance data.
            var simulationDelta = Mathf.Min(Time.unscaledDeltaTime, .05f);
            _elapsed += simulationDelta; _stateElapsed += simulationDelta;
            while (_nextStep < Scenario.Length && _elapsed >= Scenario[_nextStep].AtSeconds)
            {
                _currentState = Scenario[_nextStep].State; _states.Add(_currentState); _stateElapsed = 0f; _nextStep++;
            }
            ApplyPose();
            if (!_swapStarted && _currentState == "run" && _stateElapsed >= .35f && _elapsed < 2.2f)
            {
                _beforeSwap = _stateElapsed; _swapStartedAt = _elapsed;
                _upperA.SetActive(false); _upperB.SetActive(true); _swapStarted = true;
            }
            else if (_swapStarted && !_swapFinished && _elapsed >= _swapStartedAt + .12f)
            {
                _afterSwap = _stateElapsed;
                if (_currentState != "run" || _afterSwap + .02f < _beforeSwap) _failures.Add("EQUIPMENT_SWAP_RESET_ANIMATION_STATE");
                _swapFinished = true;
            }
            MeasureGeometry();
            if (_elapsed > .5f)
            {
                _frameTimes.Add(Time.unscaledDeltaTime * 1000f);
                _peakMemory = Math.Max(_peakMemory, Profiler.GetTotalAllocatedMemoryLong());
                if (_drawCalls.Valid) _peakDrawCalls = Math.Max(_peakDrawCalls, _drawCalls.LastValue);
            }
            if (_elapsed >= QuitAtSeconds) { WriteReportAndQuit(); enabled = false; }
        }

        private void ApplyPose()
        {
            foreach (var bone in _bones) bone.localRotation = Quaternion.identity;
            _pelvis.localPosition = _pelvisRestLocalPosition;
            var phase = _stateElapsed * Mathf.PI * 3.6f;
            float nearArm = 0, farArm = 0, nearThigh = 0, farThigh = 0, nearShin = 0, farShin = 0, pelvis = 0;
            if (_currentState == "idle") pelvis = Mathf.Sin(_stateElapsed * 2f) * 1.5f;
            else if (_currentState == "run")
            {
                nearArm = Mathf.Sin(phase) * 34f; farArm = -nearArm;
                nearThigh = -nearArm * .75f; farThigh = -nearThigh;
                nearShin = Mathf.Max(0, Mathf.Sin(phase)) * 38f; farShin = Mathf.Max(0, -Mathf.Sin(phase)) * 38f;
                _pelvis.localPosition += Vector3.up * (Mathf.Abs(Mathf.Sin(phase)) * .025f);
            }
            else if (_currentState == "jump") { nearArm = -22; farArm = 25; nearThigh = -38; farThigh = 28; nearShin = 68; farShin = 58; _pelvis.localPosition += Vector3.up * .28f; }
            else if (_currentState == "fall") { nearArm = 20; farArm = -18; nearThigh = 18; farThigh = -12; nearShin = 18; farShin = 12; _pelvis.localPosition += Vector3.up * .18f; }
            else if (_currentState == "land") { nearArm = -8; farArm = 10; nearThigh = -20; farThigh = 20; nearShin = 42; farShin = 42; _pelvis.localPosition += Vector3.down * .06f; }
            else if (_currentState == "attack") { nearArm = -82; farArm = -48; pelvis = -7; }
            else if (_currentState == "return_to_idle") { var t = Mathf.Clamp01(_stateElapsed / .6f); nearArm = Mathf.Lerp(-82, 0, t); farArm = Mathf.Lerp(-48, 0, t); pelvis = Mathf.Lerp(-7, 0, t); }
            Rotate("near_upper_arm", nearArm); Rotate("far_upper_arm", farArm); Rotate("near_thigh", nearThigh); Rotate("far_thigh", farThigh);
            Rotate("near_shin", nearShin); Rotate("far_shin", farShin); Rotate("pelvis", pelvis);
        }

        private void Rotate(string bone, float degrees) { if (_boneByName.TryGetValue(bone, out var transform)) transform.localRotation = Quaternion.Euler(0, 0, degrees); }

        private void MeasureGeometry()
        {
            _rootScaleDrift = Mathf.Max(_rootScaleDrift, Vector3.Distance(_initialRootScale, _actorRoot.localScale));
            var rigidRatio = Vector3.Distance(_initialRigidScale, _rigidItem.lossyScale) / Mathf.Max(.000001f, _initialRigidScale.magnitude);
            _rigidScaleDrift = Mathf.Max(_rigidScaleDrift, rigidRatio);
            for (var i = 1; i < _bones.Length; i++)
            {
                var current = Vector3.Distance(_bones[i].position, _bones[i].parent.position);
                _boneDrift = Mathf.Max(_boneDrift, Mathf.Abs(current - _boneLengths[i - 1]) / Mathf.Max(.000001f, _boneLengths[i - 1]));
            }
            var signs = TriangleSigns(DeformedUpperVertices()); var inverted = 0;
            for (var i = 0; i < signs.Length; i++) if (_triangleSigns[i] * signs[i] < -.00000001f) inverted++;
            _triangleInversions = Mathf.Max(_triangleInversions, inverted);
        }

        private Vector3[] DeformedUpperVertices()
        {
            var result = new Vector3[_upperVertices.Length];
            for (var i = 0; i < result.Length; i++)
            {
                var w = _upperWeights[i];
                result[i] = Skin(_upperVertices[i], w.boneIndex0, w.weight0)
                    + Skin(_upperVertices[i], w.boneIndex1, w.weight1);
            }
            return result;
        }

        private Vector3 Skin(Vector3 point, int bone, float weight)
        {
            if (weight <= 0) return Vector3.zero;
            if (bone < 0 || bone >= _bones.Length || bone >= _upperBindPoses.Length)
                throw new IndexOutOfRangeException("Garment influence references missing bone " + bone);
            return _bones[bone].localToWorldMatrix.MultiplyPoint3x4(_upperBindPoses[bone].MultiplyPoint3x4(point)) * weight;
        }

        private float[] TriangleSigns(Vector3[] vertices)
        {
            var signs = new float[_upperIndices.Length / 3];
            for (var i = 0; i < signs.Length; i++)
            {
                var a = vertices[_upperIndices[i * 3]]; var b = vertices[_upperIndices[i * 3 + 1]]; var c = vertices[_upperIndices[i * 3 + 2]];
                signs[i] = Mathf.Sign((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
            }
            return signs;
        }

        private void WriteReportAndQuit()
        {
            if (_nextStep != Scenario.Length) _failures.Add("SCENARIO_INCOMPLETE");
            if (!_swapFinished) _failures.Add("EQUIPMENT_SWAP_NOT_COMPLETED");
            if (_rootScaleDrift > .001f) _failures.Add("ROOT_SCALE_DRIFT");
            if (_boneDrift > .001f) _failures.Add("BONE_LENGTH_DRIFT");
            if (_rigidScaleDrift > .001f) _failures.Add("RIGID_ITEM_SCALE_DRIFT");
            if (_triangleInversions > 0) _failures.Add("SOFT_TRIANGLE_INVERSION");
            var report = new RuntimeReport
            {
                status = _failures.Count == 0 ? "NARROW_PLAYER_TECHNICAL_PASS" : "FIX_REQUIRED",
                unityVersion = Application.unityVersion, renderer = SystemInfo.graphicsDeviceName,
                statesPlayed = _states.ToArray(), transitionCount = Math.Max(0, _states.Count - 1),
                equipmentSwapDuringRun = true, equipmentSwapPreservedState = !_failures.Contains("EQUIPMENT_SWAP_RESET_ANIMATION_STATE"),
                stateTimeBeforeSwap = _beforeSwap, stateTimeAfterSwap = _afterSwap,
                authoredGarmentWeights = true, bodyCutoutPartCount = 10, poseSpecificGarmentSourceCount = 0, restMasterCount = 1,
                rootScaleMaxDrift = _rootScaleDrift, boneLengthMaxDriftRatio = _boneDrift,
                rigidItemScaleMaxDriftRatio = _rigidScaleDrift, softTriangleInversionMax = _triangleInversions,
                seamGapMaxSourcePixels = -1f, frameTimeP95Ms = Percentile95(_frameTimes),
                peakAllocatedMemoryBytes = _peakMemory, peakDrawCalls = _peakDrawCalls,
                drawCallMeasurementStatus = _peakDrawCalls > 1000 ? "REJECT_IMPLAUSIBLE_COUNTER" : "RECORDED_UNVERIFIED",
                measuredFrames = _frameTimes.Count,
                seamMeasurementStatus = "NOT_IMPLEMENTED_GARMENT_TO_BODY_BOUNDARY",
                geometryMeasurementLimit = "Triangle and bone checks cannot replace Player visual review; body ownership must be reviewed per layer.",
                failures = _failures.ToArray(), runtimePromotionAllowed = false,
            };
            File.WriteAllText(Path.Combine(_outputDirectory, "runtime-report.json"), JsonUtility.ToJson(report, true) + Environment.NewLine);
            Application.Quit(_failures.Count == 0 ? 0 : 2);
        }

        private void OnDestroy() { if (_drawCalls.Valid) _drawCalls.Dispose(); }
        private void OnGUI()
        {
            GUI.Label(new Rect(20, 20, 800, 30), "SKELETAL 2D COMMON-TASK PROBE — REVIEW ONLY");
            GUI.Label(new Rect(20, 50, 800, 30), "State: " + _currentState + " | fixed bone lengths | one upper rest master | A/B swaps");
        }

        private static string ReadOption(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index++) if (args[index] == key) return args[index + 1];
            return null;
        }
    }
}
#endif
