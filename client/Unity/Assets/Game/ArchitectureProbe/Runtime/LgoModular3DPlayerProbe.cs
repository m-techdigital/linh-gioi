#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

namespace LinhGioi.ArchitectureProbe
{
    public sealed class LgoModular3DPlayerProbe : MonoBehaviour
    {
        public readonly struct ScenarioStep
        {
            public ScenarioStep(float atSeconds, string state)
            {
                AtSeconds = atSeconds;
                State = state;
            }

            public float AtSeconds { get; }
            public string State { get; }
        }

        public static readonly ScenarioStep[] Scenario =
        {
            new ScenarioStep(0.20f, "idle"),
            new ScenarioStep(1.00f, "run"),
            new ScenarioStep(2.20f, "jump"),
            new ScenarioStep(3.20f, "run"),
            new ScenarioStep(4.20f, "attack"),
            new ScenarioStep(5.20f, "return_to_idle"),
            new ScenarioStep(6.20f, "run"),
        };

        public const float EquipmentSwapAtSeconds = 1.55f;
        public const float QuitAtSeconds = 12f;

        [Serializable]
        private sealed class RuntimeReport
        {
            public string status;
            public string unityVersion;
            public string renderer;
            public string[] statesPlayed;
            public int transitionCount;
            public bool equipmentSwapDuringRun;
            public bool equipmentSwapPreservedState;
            public bool secondUpperItemActivated;
            public bool waistBeltPresent;
            public bool shoulderChestGuardPresent;
            public float normalizedTimeBeforeSwap;
            public float normalizedTimeAfterSwap;
            public float rootScaleMaxDrift;
            public float rigidItemScaleMaxDrift;
            public float frameTimeP95Ms;
            public long peakAllocatedMemoryBytes;
            public long peakDrawCalls;
            public string drawCallMeasurementStatus;
            public int measuredFrames;
            public string[] screenshots;
            public string[] missingCommonTaskEvidence;
            public string[] failures;
            public bool runtimePromotionAllowed;
        }

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _actorRoot;
        [SerializeField] private Transform _rigidItem;
        [SerializeField] private GameObject _upper;
        [SerializeField] private GameObject _upperVariant;
        [SerializeField] private GameObject _waistBelt;
        [SerializeField] private GameObject _shoulderChestGuard;
        private Vector3 _initialRootScale;
        private Vector3 _initialRigidScale;
        private readonly List<float> _frameTimesMs = new List<float>();
        private readonly List<string> _statesPlayed = new List<string>();
        private readonly List<string> _screenshots = new List<string>();
        private readonly List<string> _failures = new List<string>();
        private ProfilerRecorder _drawCalls;
        private float _elapsed;
        private int _nextStep;
        private bool _swapStarted;
        private bool _swapFinished;
        private int _swapStateHash;
        private float _beforeSwap;
        private float _afterSwap;
        private float _rootScaleDrift;
        private float _rigidScaleDrift;
        private long _peakMemory;
        private long _peakDrawCalls;
        private string _outputDirectory;
        private string _currentState = "boot";

        public void Configure(Animator animator, Transform actorRoot, Transform rigidItem, GameObject upper)
        {
            Configure(animator, actorRoot, rigidItem, upper, null, null, null);
        }

        public void Configure(Animator animator, Transform actorRoot, Transform rigidItem, GameObject upper,
            GameObject upperVariant, GameObject waistBelt, GameObject shoulderChestGuard)
        {
            _animator = animator;
            _actorRoot = actorRoot;
            _rigidItem = rigidItem;
            _upper = upper;
            _upperVariant = upperVariant;
            _waistBelt = waistBelt;
            _shoulderChestGuard = shoulderChestGuard;
        }

        public static float Percentile95(IEnumerable<float> values)
        {
            var sorted = values.OrderBy(value => value).ToArray();
            if (sorted.Length == 0) return 0f;
            var index = Mathf.Clamp(Mathf.CeilToInt(sorted.Length * 0.95f) - 1, 0, sorted.Length - 1);
            return sorted[index];
        }

        private void Awake()
        {
            Application.runInBackground = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            _outputDirectory = ReadOption("--lgo-probe-output") ?? Path.Combine(Application.persistentDataPath, "lgo-modular-3d-player-probe");
            Directory.CreateDirectory(_outputDirectory);
            Directory.CreateDirectory(Path.Combine(_outputDirectory, "screenshots"));
            if (_animator == null || _actorRoot == null || _rigidItem == null || _upper == null)
            {
                _failures.Add("RUNTIME_REFERENCE_MISSING");
                WriteReportAndQuit();
                enabled = false;
                return;
            }
            _initialRootScale = _actorRoot == null ? Vector3.one : _actorRoot.localScale;
            _initialRigidScale = _rigidItem == null ? Vector3.one : _rigidItem.lossyScale;
            try
            {
                _drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 32);
            }
            catch (Exception error)
            {
                _failures.Add("DRAW_CALL_RECORDER_UNAVAILABLE:" + error.GetType().Name);
            }
        }

        private void Update()
        {
            _elapsed += Time.unscaledDeltaTime;
            if (_elapsed > 0.5f)
            {
                _frameTimesMs.Add(Time.unscaledDeltaTime * 1000f);
                _peakMemory = Math.Max(_peakMemory, Profiler.GetTotalAllocatedMemoryLong());
                if (_drawCalls.Valid) _peakDrawCalls = Math.Max(_peakDrawCalls, _drawCalls.LastValue);
            }

            while (_nextStep < Scenario.Length && _elapsed >= Scenario[_nextStep].AtSeconds)
            {
                PlayState(Scenario[_nextStep].State);
                _nextStep++;
            }

            if (!_swapStarted && _elapsed >= EquipmentSwapAtSeconds)
            {
                var state = _animator.GetCurrentAnimatorStateInfo(0);
                _swapStateHash = state.fullPathHash;
                _beforeSwap = state.normalizedTime;
                _upper.SetActive(false);
                if (_upperVariant != null) _upperVariant.SetActive(true);
                _swapStarted = true;
            }
            else if (_swapStarted && !_swapFinished && _elapsed >= EquipmentSwapAtSeconds + 0.12f)
            {
                if (_upperVariant == null) _upper.SetActive(true);
                var state = _animator.GetCurrentAnimatorStateInfo(0);
                _afterSwap = state.normalizedTime;
                if (state.fullPathHash != _swapStateHash || _afterSwap + 0.02f < _beforeSwap)
                    _failures.Add("EQUIPMENT_SWAP_RESET_ANIMATION_STATE");
                _swapFinished = true;
            }

            if (_actorRoot != null)
                _rootScaleDrift = Mathf.Max(_rootScaleDrift, Vector3.Distance(_initialRootScale, _actorRoot.localScale));
            if (_rigidItem != null)
                _rigidScaleDrift = Mathf.Max(_rigidScaleDrift, Vector3.Distance(_initialRigidScale, _rigidItem.lossyScale));

            if (_elapsed >= QuitAtSeconds)
            {
                WriteReportAndQuit();
                enabled = false;
            }
        }

        private void PlayState(string state)
        {
            _currentState = state;
            _statesPlayed.Add(state);
            _animator.CrossFade(state, 0.15f, 0, 0f);
            // Runtime capture is orchestrated outside Player. The project does
            // not enable ScreenCaptureModule and this probe must not add it.
        }

        private void WriteReportAndQuit()
        {
            if (_nextStep != Scenario.Length) _failures.Add("SCENARIO_INCOMPLETE");
            if (!_swapFinished) _failures.Add("EQUIPMENT_SWAP_NOT_COMPLETED");
            if (_rootScaleDrift > 0.001f) _failures.Add("ROOT_SCALE_DRIFT");
            if (_rigidScaleDrift > 0.001f) _failures.Add("RIGID_ITEM_SCALE_DRIFT");
            var report = new RuntimeReport
            {
                status = _failures.Count == 0 ? "NARROW_PLAYER_TECHNICAL_PASS" : "FIX_REQUIRED",
                unityVersion = Application.unityVersion,
                renderer = SystemInfo.graphicsDeviceName,
                statesPlayed = _statesPlayed.ToArray(),
                transitionCount = Math.Max(0, _statesPlayed.Count - 1),
                equipmentSwapDuringRun = true,
                equipmentSwapPreservedState = !_failures.Contains("EQUIPMENT_SWAP_RESET_ANIMATION_STATE"),
                secondUpperItemActivated = _upperVariant != null && _upperVariant.activeInHierarchy,
                waistBeltPresent = _waistBelt != null && _waistBelt.activeInHierarchy,
                shoulderChestGuardPresent = _shoulderChestGuard != null && _shoulderChestGuard.activeInHierarchy,
                normalizedTimeBeforeSwap = _beforeSwap,
                normalizedTimeAfterSwap = _afterSwap,
                rootScaleMaxDrift = _rootScaleDrift,
                rigidItemScaleMaxDrift = _rigidScaleDrift,
                frameTimeP95Ms = Percentile95(_frameTimesMs),
                peakAllocatedMemoryBytes = _peakMemory,
                peakDrawCalls = _peakDrawCalls,
                drawCallMeasurementStatus = _peakDrawCalls > 1000 ? "REJECT_IMPLAUSIBLE_COUNTER" : "RECORDED_UNVERIFIED",
                measuredFrames = _frameTimesMs.Count,
                screenshots = _screenshots.ToArray(),
                missingCommonTaskEvidence = new[] { "LGO_ART", "FULL_NEUTRAL_ANATOMY", "RUN_JUMP_FALL_LAND_RUN", "UNSEEN_ITEM", "MIXED_MILESTONE", "MOBILE_DEVICE", "EXTERNAL_PLAYER_CAPTURE", "HUMAN_VISUAL_APPROVAL" },
                failures = _failures.ToArray(),
                runtimePromotionAllowed = false,
            };
            File.WriteAllText(Path.Combine(_outputDirectory, "runtime-report.json"), JsonUtility.ToJson(report, true) + Environment.NewLine);
            Application.Quit(_failures.Count == 0 ? 0 : 2);
        }

        private void OnDestroy()
        {
            if (_drawCalls.Valid) _drawCalls.Dispose();
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(20, 20, 700, 30), "MODULAR 3D TECHNICAL PROBE — REVIEW ONLY");
            GUI.Label(new Rect(20, 50, 700, 30), "State: " + _currentState + " | Synthetic geometry; no visual approval");
        }

        private static string ReadOption(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index++)
                if (args[index] == key) return args[index + 1];
            return null;
        }
    }
}
#endif
