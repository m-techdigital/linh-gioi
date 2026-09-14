using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LinhGioi.Character
{
    public readonly struct RigidOutfitPilotCaptureFrame
    {
        public RigidOutfitPilotCaptureFrame(string label, RigidMotionState state, float normalizedTime)
        {
            Label = label;
            State = state;
            NormalizedTime = normalizedTime;
        }

        public string Label { get; }
        public RigidMotionState State { get; }
        public float NormalizedTime { get; }
    }

    public static class RigidOutfitPilotCapturePlan
    {
        public const int VideoFps = 24;

        public static IReadOnlyList<RigidOutfitPilotCaptureFrame> Frames { get; } = new[]
        {
            new RigidOutfitPilotCaptureFrame("idle", RigidMotionState.Idle, .25f),
            new RigidOutfitPilotCaptureFrame("walk-near", RigidMotionState.Walk, .125f),
            new RigidOutfitPilotCaptureFrame("walk-far", RigidMotionState.Walk, .625f),
            new RigidOutfitPilotCaptureFrame("run-near-contact", RigidMotionState.Run, 0f),
            new RigidOutfitPilotCaptureFrame("run-near-pass", RigidMotionState.Run, .25f),
            new RigidOutfitPilotCaptureFrame("run-far-contact", RigidMotionState.Run, .5f),
            new RigidOutfitPilotCaptureFrame("run-far-pass", RigidMotionState.Run, .75f),
            new RigidOutfitPilotCaptureFrame("jump-anticipation", RigidMotionState.Jump, .14f),
            new RigidOutfitPilotCaptureFrame("jump-apex", RigidMotionState.Jump, .52f),
            new RigidOutfitPilotCaptureFrame("jump-land", RigidMotionState.Jump, .90f),
            new RigidOutfitPilotCaptureFrame("attack-windup", RigidMotionState.Attack, .24f),
            new RigidOutfitPilotCaptureFrame("attack-impact", RigidMotionState.Attack, .44f),
            new RigidOutfitPilotCaptureFrame("roll-entry", RigidMotionState.Roll, .20f),
            new RigidOutfitPilotCaptureFrame("roll-tuck", RigidMotionState.Roll, .50f),
            new RigidOutfitPilotCaptureFrame("roll-open", RigidMotionState.Roll, .80f),
        };
    }

    public sealed class RigidOutfitPilotPlayer : MonoBehaviour
    {
        [Serializable]
        private sealed class Evidence
        {
            public string status;
            public string[] states;
            public string maleSpriteBefore;
            public string maleSpriteAfter;
            public string femaleSpriteBefore;
            public string femaleSpriteAfter;
            public string maleFitBefore;
            public string maleFitAfter;
            public string femaleFitBefore;
            public string femaleFitAfter;
            public bool scaleInvariant;
            public bool forbiddenRendererComponentsAbsent;
            public bool characterRootRotationIdentity;
            public int spriteRendererCount;
            public int captureCount;
            public int motionFrameCount;
            public int motionFps;
            public string sourceSpaceProfile;
            public string[] failures;
        }

        private RigidOutfitPilotActor _male;
        private RigidOutfitPilotActor _female;
        private string _currentLabel = "IDLE";
        private float _started;
        private bool _captureMode;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            var catalog = RigidOutfitPilotCatalog.LoadFromResources();
            _male = CreateActor("MALE", new Vector3(-2.15f, -2.55f, 0f), catalog.Male);
            _female = CreateActor("FEMALE", new Vector3(2.15f, -2.55f, 0f), catalog.Female);
            _started = Time.realtimeSinceStartup;
            var output = ReadOutputDirectory();
            _captureMode = output != null;
            if (_captureMode) StartCoroutine(Capture(output));
        }

        private void Update()
        {
            if (_male == null || _captureMode) return;
            var sequence = new[]
            {
                RigidMotionState.Idle, RigidMotionState.Walk, RigidMotionState.Run,
                RigidMotionState.Jump, RigidMotionState.Attack, RigidMotionState.Roll, RigidMotionState.Hit,
            };
            var elapsed = Time.realtimeSinceStartup - _started;
            var state = sequence[Mathf.FloorToInt(elapsed / 2f) % sequence.Length];
            var phase = Mathf.Repeat(elapsed / 2f, 1f);
            ApplyBoth(state, phase);
            _currentLabel = state.ToString().ToUpperInvariant();
        }

        private RigidOutfitPilotActor CreateActor(string name, Vector3 position, RigidOutfitPilotCharacter character)
        {
            var root = new GameObject(name).transform;
            root.SetParent(transform, false);
            root.localPosition = position;
            var actor = RigidOutfitPilotActor.Create(root, character);
            actor.Apply(RigidMotionState.Idle, 0f);
            return actor;
        }

        private IEnumerator Capture(string output)
        {
            Directory.CreateDirectory(output);
            var maleSpriteBefore = _male.SpriteFingerprint;
            var femaleSpriteBefore = _female.SpriteFingerprint;
            var maleFitBefore = _male.FitFingerprint;
            var femaleFitBefore = _female.FitFingerprint;
            var index = 0;
            foreach (var frame in RigidOutfitPilotCapturePlan.Frames)
            {
                index++;
                ApplyBoth(frame.State, frame.NormalizedTime);
                _currentLabel = frame.Label.ToUpperInvariant();
                yield return new WaitForEndOfFrame();
                var path = Path.Combine(output, index.ToString("00") + "-" + frame.Label + ".png");
                var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply(false, false);
                File.WriteAllBytes(path, texture.EncodeToPNG());
                Destroy(texture);
                yield return null;
            }

            var motionFrames = 0;
            var motionDirectory = Path.Combine(output, "motion-frames");
            Directory.CreateDirectory(motionDirectory);
            var motionSequence = new[]
            {
                (state: RigidMotionState.Idle, frames: 18),
                (state: RigidMotionState.Walk, frames: 24),
                (state: RigidMotionState.Run, frames: 24),
                (state: RigidMotionState.Jump, frames: 30),
                (state: RigidMotionState.Attack, frames: 24),
                (state: RigidMotionState.Roll, frames: 30),
            };
            foreach (var segment in motionSequence)
            {
                for (var frameIndex = 0; frameIndex < segment.frames; frameIndex++)
                {
                    var phase = segment.frames <= 1 ? 0f : frameIndex / (segment.frames - 1f);
                    ApplyBoth(segment.state, phase);
                    _currentLabel = segment.state.ToString().ToUpperInvariant();
                    yield return new WaitForEndOfFrame();
                    motionFrames++;
                    var path = Path.Combine(motionDirectory, "motion-" + motionFrames.ToString("D4") + ".png");
                    var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                    texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                    texture.Apply(false, false);
                    File.WriteAllBytes(path, texture.EncodeToPNG());
                    Destroy(texture);
                    yield return null;
                }
            }

            var evidence = new Evidence
            {
                states = RigidOutfitPilotCapturePlan.Frames.Select(frame => frame.State.ToString()).Distinct().ToArray(),
                maleSpriteBefore = maleSpriteBefore,
                maleSpriteAfter = _male.SpriteFingerprint,
                femaleSpriteBefore = femaleSpriteBefore,
                femaleSpriteAfter = _female.SpriteFingerprint,
                maleFitBefore = maleFitBefore,
                maleFitAfter = _male.FitFingerprint,
                femaleFitBefore = femaleFitBefore,
                femaleFitAfter = _female.FitFingerprint,
                scaleInvariant = _male.HasUnitScale && _female.HasUnitScale,
                forbiddenRendererComponentsAbsent = !_male.HasForbiddenRendererComponents && !_female.HasForbiddenRendererComponents,
                characterRootRotationIdentity = _male.Root.localRotation == Quaternion.identity && _female.Root.localRotation == Quaternion.identity,
                spriteRendererCount = GetComponentsInChildren<SpriteRenderer>(true).Length,
                captureCount = index,
                motionFrameCount = motionFrames,
                motionFps = RigidOutfitPilotCapturePlan.VideoFps,
                sourceSpaceProfile = RigidOutfitPilotCatalog.SourceSpaceProfile,
            };
            var failures = new List<string>();
            if (evidence.maleSpriteBefore != evidence.maleSpriteAfter) failures.Add("MALE_SPRITE_IDENTITY_CHANGED");
            if (evidence.femaleSpriteBefore != evidence.femaleSpriteAfter) failures.Add("FEMALE_SPRITE_IDENTITY_CHANGED");
            if (evidence.maleFitBefore != evidence.maleFitAfter) failures.Add("MALE_FIT_CHANGED");
            if (evidence.femaleFitBefore != evidence.femaleFitAfter) failures.Add("FEMALE_FIT_CHANGED");
            if (!evidence.scaleInvariant) failures.Add("LOCAL_SCALE_CHANGED");
            if (!evidence.forbiddenRendererComponentsAbsent) failures.Add("FORBIDDEN_RENDERER_COMPONENT_FOUND");
            if (!evidence.characterRootRotationIdentity) failures.Add("CHARACTER_ROOT_ROTATION_CHANGED");
            evidence.failures = failures.ToArray();
            evidence.status = failures.Count == 0 ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            File.WriteAllText(Path.Combine(output, "runtime-evidence.json"), JsonUtility.ToJson(evidence, true) + Environment.NewLine);
            Debug.Log("LGO_RIGID_OUTFIT_PILOT " + evidence.status);
            Application.Quit(failures.Count == 0 ? 0 : 2);
        }

        private void ApplyBoth(RigidMotionState state, float phase)
        {
            _male.Apply(state, phase);
            _female.Apply(state, phase);
        }

        private static string ReadOutputDirectory()
        {
            const string prefix = "--lgo-rigid-outfit-evidence=";
            var value = Environment.GetCommandLineArgs().FirstOrDefault(argument => argument.StartsWith(prefix, StringComparison.Ordinal));
            return value == null ? null : Path.GetFullPath(value.Substring(prefix.Length));
        }

        private void OnGUI()
        {
            if (_captureMode) return;
            var width = Screen.width;
            var title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(22, Screen.height / 28),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(.91f, .96f, 1f) },
            };
            var sub = new GUIStyle(title) { fontSize = Mathf.Max(14, Screen.height / 45), fontStyle = FontStyle.Normal };
            GUI.Label(new Rect(0, Screen.height - 122, width, 42), "LGO RIGID OUTFIT PILOT · " + _currentLabel, title);
            GUI.Label(new Rect(0, Screen.height - 88, width, 28), "SAME SPRITES · SAME FIT · BONE POSITION/ROTATION ONLY", sub);
            GUI.Label(new Rect(width * .12f, Screen.height - 50, width * .26f, 30), "MALE · PILOT SOURCE", sub);
            GUI.Label(new Rect(width * .62f, Screen.height - 50, width * .26f, 30), "FEMALE · PILOT SOURCE", sub);
        }
    }
}
