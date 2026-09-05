using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace LinhGioi.UI
{
    public sealed class VisualRuntimeEvidenceRunner : MonoBehaviour
    {
        private const int DefaultReviewWidth = 1920;
        private const int DefaultReviewHeight = 1080;
        private const string DefaultOutputDirectoryName = "visual-runtime-evidence";
        private static readonly string[] ExpectedScreenshots =
        {
            "login.png",
            "character-lobby.png",
            "character-select.png",
            "enter-world.png",
            "world-hub.png",
            "near-gatekeeper-prompt.png",
            "near-training-stone-prompt.png",
            "target-dummy-state.png",
            "npc-dialogue.png",
            "session-menu.png"
        };

        private readonly List<VisualCheckpointEvidence> _checkpoints = new List<VisualCheckpointEvidence>();
        private string _outputDir;
        private int _reviewWidth = DefaultReviewWidth;
        private int _reviewHeight = DefaultReviewHeight;
        private M4PlayableClientController _controller;

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_VISUAL_RUNTIME_REVIEW"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-visual-runtime-review") return true;
            return false;
        }

        public static void Attach(GameObject host)
        {
            Debug.Log("[LinhGioi] Visual runtime evidence runner attached.");
            host.AddComponent<VisualRuntimeEvidenceRunner>();
        }

        private void Start()
        {
            Application.runInBackground = true;
            _outputDir = GetArg("--lgo-visual-runtime-evidence-dir") ?? Path.Combine(Application.persistentDataPath, DefaultOutputDirectoryName);
            Directory.CreateDirectory(_outputDir);
            _reviewWidth = GetIntArg("--lgo-visual-runtime-width", DefaultReviewWidth);
            _reviewHeight = GetIntArg("--lgo-visual-runtime-height", DefaultReviewHeight);
            Screen.SetResolution(_reviewWidth, _reviewHeight, false);
            _controller = M4PlayableClientController.Attach(gameObject);
            StartCoroutine(RunEvidenceFlow());
        }

        private IEnumerator RunEvidenceFlow()
        {
            Debug.Log("[LinhGioi] Visual runtime evidence flow started. output=" + _outputDir);
            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "login",
                "Login / Linh Mon",
                "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv; docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-PACK-v3b-runtime.md",
                "V3B background, text logo, Gate Keeper, server selector, Vao The Gioi CTA");

            yield return WaitForTask(_controller.CaptureEvidenceLoginAsync());
            yield return WaitFrames(6);
            yield return CaptureCheckpoint(
                "character-lobby",
                "Character Select / Dien Nhan Vat",
                "docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md",
                "Account connected, character list or empty/create state, enter-world action");

            yield return WaitForTask(_controller.CaptureEvidenceCreateCharacterIfNeededAsync("EvidenceHero"));
            yield return WaitFrames(6);
            yield return CaptureCheckpoint(
                "character-select",
                "Character Select Ready",
                "docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md",
                "Selected character preview and enter-world CTA readability");

            yield return WaitForTask(_controller.CaptureEvidenceEnterWorldAsync());
            yield return WaitFrames(10);
            yield return CaptureCheckpoint(
                "enter-world",
                "Enter World",
                "docs/reference-art/v0.16.5/lgo-world-hub-2d5-v0165.png",
                "World loaded, HUD visible, objective and movement affordances readable");

            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "world-hub",
                "World Hub",
                "docs/reference-art/v0.16.5/lgo-playable-hud-mockup-v0165.png",
                "World HUD hierarchy, status blocks, local combat shell, target clarity");

            _controller.CaptureEvidenceNearGateKeeperPrompt();
            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "near-gatekeeper-prompt",
                "Near Gate Keeper Prompt",
                "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv; docs/tasks/LGO-WORLD-HUB-INTERACTION-READABILITY-PASS-v1.0.md",
                "Player stands inside Gate Keeper interaction range; short Vietnamese prompt and compact HUD action are visible");

            _controller.CaptureEvidenceNearTrainingStonePrompt();
            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "near-training-stone-prompt",
                "Near Training Stone Prompt",
                "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv; docs/tasks/LGO-WORLD-HUB-INTERACTION-READABILITY-PASS-v1.0.md",
                "Player stands inside Training Stone interaction range; short Vietnamese prompt and compact HUD action are visible");

            _controller.CaptureEvidenceTargetDummyState();
            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "target-dummy-state",
                "Target Dummy Local Feedback",
                "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv",
                "V3B target dummy selected/hit/recover state clarity, cooldown ring, combat button fit, and local-only combat copy");

            _controller.CaptureEvidenceOpenDialogue();
            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "npc-dialogue",
                "NPC Dialogue",
                "docs/reference-art/v0.16.5/lgo-gate-character-ui-v0165.png",
                "Vietnamese dialogue panel, speaker, progress, action buttons");

            _controller.CaptureEvidenceOpenSessionMenu();
            yield return WaitFrames(8);
            yield return CaptureCheckpoint(
                "session-menu",
                "Session Menu",
                "docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md",
                "Resume/save/back/quit controls and settings foundation");

            WriteManifest();
            Debug.Log("[LinhGioi] Visual runtime evidence flow completed. checkpoints=" + _checkpoints.Count);
            Quit(0);
        }

        private static IEnumerator WaitFrames(int frameCount)
        {
            for (var i = 0; i < frameCount; i++) yield return null;
        }

        private static IEnumerator WaitForTask(Task task)
        {
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted)
            {
                var exception = task.Exception != null ? task.Exception.GetBaseException() : new InvalidOperationException("Task failed.");
                throw exception;
            }
        }

        private IEnumerator CaptureCheckpoint(string id, string title, string reference, string expectation)
        {
            yield return new WaitForEndOfFrame();
            var fileName = id + ".png";
            var path = Path.Combine(_outputDir, fileName);
            var evidence = new VisualCheckpointEvidence
            {
                id = id,
                title = title,
                file = fileName,
                path = path,
                reference = reference,
                expectation = expectation,
                width = Screen.width,
                height = Screen.height,
                status = "STARTED"
            };
            Debug.Log("[LinhGioi] Visual runtime capture started: " + id);
            CaptureFrame(path, evidence);
            Debug.Log("[LinhGioi] Visual runtime capture finished: " + id + " status=" + evidence.status + " bytes=" + evidence.bytes);
            evidence.review = BuildAutomatedReview(evidence, _reviewWidth, _reviewHeight);
            evidence.reviewChecklist = BuildReviewChecklist(evidence);
            _checkpoints.Add(evidence);
        }

        private static void CaptureFrame(string path, VisualCheckpointEvidence evidence)
        {
            try
            {
                var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                evidence.bytes = RuntimePngWriter.WriteRgbTexture(path, texture);
                evidence.status = evidence.bytes > 0 ? "CAPTURED" : "VISUAL_RUNTIME_SCREENSHOT_UNAVAILABLE";
                UnityEngine.Object.Destroy(texture);
            }
            catch (Exception exception)
            {
                evidence.status = "VISUAL_RUNTIME_SCREENSHOT_UNAVAILABLE";
                evidence.reason = exception.GetType().FullName + ": " + exception.Message;
            }
        }

        private static string BuildAutomatedReview(VisualCheckpointEvidence evidence, int reviewWidth, int reviewHeight)
        {
            if (evidence.status != "CAPTURED") return "Capture failed; visual review blocked.";
            if (evidence.width < reviewWidth || evidence.height < reviewHeight) return "Captured below target resolution; review requires rerun at requested profile size.";
            if (evidence.bytes < 64 * 1024) return "Suspiciously small screenshot; likely blank or missing visual content.";
            return "Captured at target resolution. Review categories are recorded, but this is not a VISUAL_RUNTIME_PASS claim.";
        }

        private static string BuildReviewChecklist(VisualCheckpointEvidence evidence)
        {
            if (evidence.status != "CAPTURED") return "blocked: screenshot capture failed.";
            return "layout=review; scale=review; spacing=review; sharpness=review; asset_quality=review; hierarchy=review; readability=review; reference_similarity=review; pass_claim=false";
        }

        private void WriteManifest()
        {
            var summary = new VisualRuntimeEvidenceSummary
            {
                marker = "LGO_VISUAL_RUNTIME_EVIDENCE_READY",
                unityVersion = Application.unityVersion,
                width = _reviewWidth,
                height = _reviewHeight,
                outputDir = _outputDir,
                visualRuntimePassClaimed = false,
                nonClaim = "Build/capture success is not VISUAL_RUNTIME_PASS; screenshots must be reviewed.",
                checkpoints = _checkpoints.ToArray()
            };
            File.WriteAllText(Path.Combine(_outputDir, "visual-runtime-evidence-manifest.json"), JsonUtility.ToJson(summary, true));
            using (var writer = new StreamWriter(Path.Combine(_outputDir, "visual-runtime-evidence-review.md")))
            {
                writer.WriteLine("# Visual Runtime Evidence Review");
                writer.WriteLine();
                writer.WriteLine("Marker: `LGO_VISUAL_RUNTIME_EVIDENCE_READY`");
                writer.WriteLine();
                writer.WriteLine("This harness captures the real Unity Player runtime. It does not claim `VISUAL_RUNTIME_PASS` from build or capture alone.");
                writer.WriteLine();
                foreach (var checkpoint in _checkpoints)
                {
                    writer.WriteLine("## " + checkpoint.title);
                    writer.WriteLine();
                    writer.WriteLine("- Screenshot: `" + checkpoint.file + "`");
                    writer.WriteLine("- Reference mapping: `" + checkpoint.reference + "`");
                    writer.WriteLine("- Expectation: " + checkpoint.expectation);
                    writer.WriteLine("- Status: `" + checkpoint.status + "`");
                    writer.WriteLine("- Automated review: " + checkpoint.review);
                    writer.WriteLine("- Review checklist: " + checkpoint.reviewChecklist);
                    writer.WriteLine();
                }
            }
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static int GetIntArg(string key, int fallback)
        {
            var raw = GetArg(key);
            int parsed;
            return int.TryParse(raw, out parsed) && parsed > 0 ? parsed : fallback;
        }

        private static void Quit(int exitCode)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(exitCode);
#else
            Application.Quit(exitCode);
#endif
        }

        [Serializable]
        private sealed class VisualRuntimeEvidenceSummary
        {
            public string marker;
            public string unityVersion;
            public int width;
            public int height;
            public string outputDir;
            public bool visualRuntimePassClaimed;
            public string nonClaim;
            public VisualCheckpointEvidence[] checkpoints;
        }

        [Serializable]
        private sealed class VisualCheckpointEvidence
        {
            public string id;
            public string title;
            public string file;
            public string path;
            public string reference;
            public string expectation;
            public int width;
            public int height;
            public int bytes;
            public string status;
            public string reason;
            public string review;
            public string reviewChecklist;
        }
    }
}
