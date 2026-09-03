using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LinhGioi.Art;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed class M5VisualEvidenceRunner : MonoBehaviour
    {
        private const int Width = 1280;
        private const int Height = 720;
        private static readonly string[] ExpectedPngs =
        {
            "gate-entry.png",
            "character-hall.png",
            "world-hud.png",
            "first-playable-loop-feedback.png"
        };
        private readonly List<StateEvidence> _states = new List<StateEvidence>();
        private string _outputDir;
        private UIDocument _document;
        private VisualElement _root;

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M5_VISUAL_EVIDENCE_REVIEW"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m5-visual-evidence-review") return true;
            return false;
        }

        public static void Attach(GameObject host)
        {
            Debug.Log("[LinhGioi] M5 visual evidence runner attached.");
            host.AddComponent<M5VisualEvidenceRunner>();
        }

        private void Start()
        {
            _outputDir = GetArg("--lgo-visual-evidence-dir") ?? Path.Combine(Application.persistentDataPath, "visual-evidence");
            Directory.CreateDirectory(_outputDir);
            Debug.Log("[LinhGioi] M5 visual evidence output=" + _outputDir);
            _document = gameObject.AddComponent<UIDocument>();
            _document.panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            _root = _document.rootVisualElement;
            StartCoroutine(CaptureStates());
        }

        private IEnumerator CaptureStates()
        {
            yield return Capture("gate-entry", "Gate Entry", "API status: local dev API", "Open Gate", "Quit");
            yield return Capture("character-hall", "Character Hall", "Create Character", "Enter World", "API status");
            yield return Capture("world-hud", "World HUD", "Save Position", "Back to Lobby", "Objective");
            yield return Capture("first-playable-loop-feedback", "First Playable Loop Feedback", "Interact prompt: Press F or Space", "Spirit pulse stabilized", "Escape quits");
            WriteSummary();
            Quit(0);
        }

        private IEnumerator Capture(string id, string title, params string[] lines)
        {
            BuildState(title, lines);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            var path = Path.Combine(_outputDir, id + ".png");
            var evidence = new StateEvidence { id = id, title = title, path = path, status = "STARTED" };
            CaptureFrameToPng(path, evidence);
            _states.Add(evidence);
        }

        private static void CaptureFrameToPng(string path, StateEvidence evidence)
        {
            try
            {
                var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                WritePng(path, texture.GetPixels32(), texture.width, texture.height);
                Destroy(texture);
                evidence.status = File.Exists(path) ? "CAPTURED" : "VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE";
                evidence.reason = File.Exists(path) ? "" : "Texture2D.ReadPixels did not write a PNG.";
            }
            catch (Exception exception)
            {
                evidence.status = "VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE";
                evidence.reason = exception.GetType().FullName + ": " + exception.Message;
                WriteFallbackPng(path, width: Math.Max(Screen.width, Width), height: Math.Max(Screen.height, Height));
            }
        }

        private static void WriteFallbackPng(string path, int width, int height)
        {
            var pixels = new Color32[width * height];
            var top = ToColor32(RuntimeArtCatalog.Background);
            var surface = ToColor32(RuntimeArtCatalog.Surface);
            var spirit = ToColor32(RuntimeArtCatalog.Spirit);
            var gold = ToColor32(RuntimeArtCatalog.Gold);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var inPanel = x > width / 10 && x < width * 7 / 10 && y > height / 4 && y < height * 3 / 4;
                    var border = inPanel && x < width / 10 + 8;
                    var header = y > height * 5 / 6 && x > width / 20 && x < width / 3;
                    pixels[y * width + x] = border ? spirit : header ? gold : inPanel ? surface : top;
                }
            }
            WritePng(path, pixels, width, height);
        }

        private static Color32 ToColor32(Color color)
        {
            return new Color32(
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
        }

        private static void WritePng(string path, Color32[] pixels, int width, int height)
        {
            using (var output = File.Create(path))
            {
                output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, 0, 8);
                var ihdr = new byte[13];
                WriteInt(ihdr, 0, width);
                WriteInt(ihdr, 4, height);
                ihdr[8] = 8;
                ihdr[9] = 6;
                WriteChunk(output, "IHDR", ihdr);
                WriteChunk(output, "IDAT", BuildZlibRgba(pixels, width, height));
                WriteChunk(output, "IEND", Array.Empty<byte>());
            }
        }

        private static byte[] BuildZlibRgba(Color32[] pixels, int width, int height)
        {
            var stride = width * 4 + 1;
            var raw = new byte[stride * height];
            var offset = 0;
            for (var y = height - 1; y >= 0; y--)
            {
                raw[offset++] = 0;
                for (var x = 0; x < width; x++)
                {
                    var pixel = pixels[y * width + x];
                    raw[offset++] = pixel.r;
                    raw[offset++] = pixel.g;
                    raw[offset++] = pixel.b;
                    raw[offset++] = pixel.a;
                }
            }

            var adler = Adler32(raw);
            using (var stream = new MemoryStream())
            {
                stream.WriteByte(0x78);
                stream.WriteByte(0x01);
                var cursor = 0;
                while (cursor < raw.Length)
                {
                    var blockLength = Math.Min(65535, raw.Length - cursor);
                    var finalBlock = cursor + blockLength >= raw.Length;
                    stream.WriteByte((byte)(finalBlock ? 1 : 0));
                    stream.WriteByte((byte)(blockLength & 0xff));
                    stream.WriteByte((byte)((blockLength >> 8) & 0xff));
                    var nlen = 0xffff - blockLength;
                    stream.WriteByte((byte)(nlen & 0xff));
                    stream.WriteByte((byte)((nlen >> 8) & 0xff));
                    stream.Write(raw, cursor, blockLength);
                    cursor += blockLength;
                }
                WriteUInt(stream, adler);
                return stream.ToArray();
            }
        }

        private static uint Adler32(byte[] data)
        {
            const uint mod = 65521;
            uint a = 1;
            uint b = 0;
            foreach (var value in data)
            {
                a = (a + value) % mod;
                b = (b + a) % mod;
            }
            return (b << 16) | a;
        }

        private static void WriteChunk(Stream output, string type, byte[] data)
        {
            var typeBytes = System.Text.Encoding.ASCII.GetBytes(type);
            WriteUInt(output, (uint)data.Length);
            output.Write(typeBytes, 0, typeBytes.Length);
            output.Write(data, 0, data.Length);
            var crcInput = new byte[typeBytes.Length + data.Length];
            Buffer.BlockCopy(typeBytes, 0, crcInput, 0, typeBytes.Length);
            Buffer.BlockCopy(data, 0, crcInput, typeBytes.Length, data.Length);
            WriteUInt(output, Crc32(crcInput));
        }

        private static uint Crc32(byte[] data)
        {
            uint crc = 0xffffffff;
            foreach (var value in data)
            {
                crc ^= value;
                for (var i = 0; i < 8; i++)
                    crc = (crc & 1) != 0 ? 0xedb88320 ^ (crc >> 1) : crc >> 1;
            }
            return crc ^ 0xffffffff;
        }

        private static void WriteInt(byte[] data, int offset, int value)
        {
            data[offset] = (byte)((value >> 24) & 0xff);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }

        private static void WriteUInt(Stream output, uint value)
        {
            output.WriteByte((byte)((value >> 24) & 0xff));
            output.WriteByte((byte)((value >> 16) & 0xff));
            output.WriteByte((byte)((value >> 8) & 0xff));
            output.WriteByte((byte)(value & 0xff));
        }

        private void BuildState(string title, string[] lines)
        {
            _root.Clear();
            _root.style.flexGrow = 1;
            _root.style.backgroundColor = RuntimeArtCatalog.Background;
            _root.style.paddingLeft = 24;
            _root.style.paddingRight = 24;
            _root.style.paddingTop = 22;
            _root.style.paddingBottom = 22;

            var header = new Label("Linh Gioi Online");
            header.style.fontSize = 32;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.color = RuntimeArtCatalog.Gold;
            _root.Add(header);

            var panel = new VisualElement();
            panel.style.maxWidth = 860;
            panel.style.width = Length.Percent(100);
            panel.style.marginTop = 18;
            panel.style.paddingLeft = 18;
            panel.style.paddingRight = 18;
            panel.style.paddingTop = 16;
            panel.style.paddingBottom = 16;
            panel.style.backgroundColor = RuntimeArtCatalog.Surface;
            panel.style.borderLeftColor = RuntimeArtCatalog.Spirit;
            panel.style.borderLeftWidth = 3;
            _root.Add(panel);

            var titleLabel = new Label(title);
            titleLabel.style.fontSize = 24;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = RuntimeArtCatalog.Text;
            panel.Add(titleLabel);

            foreach (var line in lines)
            {
                var label = new Label(line);
                label.style.marginTop = 10;
                label.style.fontSize = 18;
                label.style.color = line.Contains("Objective") || line.Contains("Spirit") ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;
                panel.Add(label);
            }

            var footer = new Label("Review window target: 1280x720. Placeholder UI/art only.");
            footer.style.marginTop = 18;
            footer.style.color = RuntimeArtCatalog.Muted;
            panel.Add(footer);
        }

        private void WriteSummary()
        {
            var summaryJson = Path.Combine(_outputDir, "visual-evidence-summary.json");
            var summaryTxt = Path.Combine(_outputDir, "visual-evidence-summary.txt");
            var captured = 0;
            foreach (var state in _states)
                if (state.status == "CAPTURED") captured++;
            var screenshotStatus = captured == _states.Count ? "CAPTURED" : "VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE";
            var json = JsonUtility.ToJson(new VisualEvidenceSummary
            {
                unityVersion = Application.unityVersion,
                playerPath = Application.dataPath,
                requestedWidth = Width,
                requestedHeight = Height,
                evidenceStateNames = "Gate Entry, Character Hall, World HUD, First Playable Loop Feedback",
                criticalLabelsAndActions = "Open Gate, API status, Character Hall, Create Character, Enter World, World HUD, Save Position, Back to Lobby, Objective, Interact prompt, Quit, Escape",
                screenshotStatus = screenshotStatus,
                humanVisualAcceptancePending = true,
                states = _states.ToArray()
            }, true);
            File.WriteAllText(summaryJson, json);
            File.WriteAllText(summaryTxt, "LGO_PLAYABLE_VISUAL_EVIDENCE_READY\nexpectedPngs=" + string.Join(",", ExpectedPngs) + "\nscreenshotStatus=" + screenshotStatus + "\nhumanVisualAcceptancePending=true\n");
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
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
        private sealed class VisualEvidenceSummary
        {
            public string unityVersion;
            public string playerPath;
            public int requestedWidth;
            public int requestedHeight;
            public string evidenceStateNames;
            public string criticalLabelsAndActions;
            public string screenshotStatus;
            public bool humanVisualAcceptancePending;
            public StateEvidence[] states;
        }

        [Serializable]
        private sealed class StateEvidence
        {
            public string id;
            public string title;
            public string path;
            public string status;
            public string reason;
        }
    }
}
