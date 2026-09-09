using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    // Opt-in art review. The normal onboarding renderer and all class code remain separate.
    [ExecuteAlways]
    public sealed class DongMonIllustratedPreview : MonoBehaviour
    {
        private const string Pack = "LGOMaps/DongMonIllustrated/";
        private readonly Dictionary<Renderer, bool> _hidden = new Dictionary<Renderer, bool>();
        private readonly List<Sprite> _sprites = new List<Sprite>();
        private TwoDOnboardingController _controller;
        private Transform _far;
        private float _farParallax;
        private Vector2 _farOrigin;
        private TextMesh _objective;
        private TextMesh _dialogue;
        private TextMesh _playerLabel;
        public int AtlasPartCount { get; private set; }
        public Sprite GateSprite { get; private set; }
        public float FarOffset => _far == null ? 0f : _far.localPosition.x;

        [Serializable] public sealed class Part { public string id; public int x, y, w, h; }
        [Serializable] public sealed class Layer
        {
            public string id, part;
            public float x, y, width, height, parallax;
            public int order;
        }
        [Serializable] public sealed class PackInfo { public string id; public Part[] parts; public Layer[] layers; }
        [Serializable] private sealed class CaptureResult
        {
            public string status, pack, finalStep, npcArt, playerArt;
            public int frames, atlasParts;
            public bool dialogueOpened, guidanceReached;
            public float parallaxDelta;
        }

        public static bool ShouldRun()
        {
            return Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-dongmon-art-preview") >= 0
                || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-dongmon-art-capture") >= 0;
        }

        public static DongMonIllustratedPreview Attach(TwoDOnboardingController controller)
        {
            var existing = controller.GetComponentInChildren<DongMonIllustratedPreview>();
            if (existing != null) return existing;
            var host = new GameObject("Dong Mon Illustrated Preview");
            host.transform.SetParent(controller.transform, false);
            var preview = host.AddComponent<DongMonIllustratedPreview>();
            preview._controller = controller;
            try { preview.Build(); }
            catch { DestroyImmediate(host); throw; }
            return preview;
        }

        private void Build()
        {
            var source = Resources.Load<TextAsset>(Pack + "manifest");
            var atlas = Resources.Load<Texture2D>(Pack + "props-atlas");
            var sky = Resources.Load<Texture2D>(Pack + "skyline");
            if (source == null || atlas == null || sky == null)
                throw new InvalidOperationException("Missing declared Dong Mon draft art pack");
            var info = JsonUtility.FromJson<PackInfo>(source.text);
            if (info.id != "dongmon-illustrated-draft-v1" || info.parts == null || info.parts.Length != 3)
                throw new InvalidOperationException("Unexpected Dong Mon draft atlas layout");
            var parts = new Dictionary<string, Sprite>();
            foreach (var p in info.parts)
            {
                if (p.x < 0 || p.y < 0 || p.w <= 0 || p.h <= 0 || p.x + p.w > atlas.width || p.y + p.h > atlas.height)
                    throw new InvalidOperationException("Atlas rectangle out of bounds: " + p.id);
                parts.Add(p.id, MakeSprite(atlas, new Rect(p.x, p.y, p.w, p.h)));
            }
            AtlasPartCount = parts.Count;
            GateSprite = parts["gate"];
            foreach (var renderer in FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!renderer.name.StartsWith("LGO 2D", StringComparison.Ordinal)
                    || renderer.transform.root.name == "LGO 2D Player") continue;
                _hidden[renderer] = renderer.enabled;
                renderer.enabled = false;
            }
            parts.Add("skyline", MakeSprite(sky, new Rect(0, 0, sky.width, sky.height)));
            if (info.layers == null) throw new InvalidOperationException("Missing authored layer placement");
            foreach (var layer in info.layers)
            {
                if (layer.width <= 0f || layer.height <= 0f || !parts.ContainsKey(layer.part))
                    throw new InvalidOperationException("Invalid authored layer: " + layer.id);
                var drawn = Draw(layer.id, parts[layer.part], new Vector2(layer.x, layer.y),
                    new Vector2(layer.width, layer.height), layer.order);
                if (layer.part == "skyline")
                {
                    _far = drawn; _farParallax = layer.parallax; _farOrigin = new Vector2(layer.x, layer.y);
                }
            }
            if (_far == null) throw new InvalidOperationException("Missing parallax skyline");
            Panel(new Vector2(-3.32f, 2.67f), new Vector2(3.65f, 0.6f));
            Label("Title", "ĐÔNG MÔN  ·  DRAFT", new Vector2(-5.02f, 2.82f), 0.042f, new Color(1f, .88f, .6f));
            _objective = Label("Objective", "", new Vector2(-5.02f, 2.53f), 0.029f, Color.white);
            Panel(new Vector2(0f, -2.7f), new Vector2(10.35f, 0.54f));
            _dialogue = Label("Dialogue", "", new Vector2(-5.0f, -2.53f), 0.029f, Color.white);
            Label("Guide name", "Người Giữ Cổng", new Vector2(-1.95f, .66f), .031f, Color.white);
            _playerLabel = Label("Player placeholder label", "Nhân vật tạm", Vector2.zero, .026f, Color.white);
            Refresh();
        }

        private Sprite MakeSprite(Texture2D texture, Rect rect)
        {
            var sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), 100f, 0, SpriteMeshType.FullRect);
            _sprites.Add(sprite);
            return sprite;
        }

        private Transform Draw(string name, Sprite sprite, Vector2 position, Vector2 size, int order)
        {
            var host = new GameObject(name);
            host.transform.SetParent(transform, false);
            host.transform.localPosition = new Vector3(position.x, position.y, 0f);
            host.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1f);
            var renderer = host.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            return host.transform;
        }

        private void Panel(Vector2 position, Vector2 size)
        {
            var sprite = MakeSprite(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height));
            Draw("Draft HUD panel", sprite, position, size, 40).GetComponent<SpriteRenderer>().color = new Color(.035f,.07f,.1f,.88f);
        }

        private TextMesh Label(string name, string text, Vector2 position, float size, Color color)
        {
            var host = new GameObject(name);
            host.transform.SetParent(transform, false);
            host.transform.localPosition = new Vector3(position.x, position.y, -1f);
            var mesh = host.AddComponent<TextMesh>();
            mesh.anchor = TextAnchor.UpperLeft;
            mesh.characterSize = size;
            mesh.fontSize = 40;
            mesh.color = color;
            mesh.text = text;
            mesh.GetComponent<MeshRenderer>().sortingOrder = 41;
            return mesh;
        }

        public void Refresh()
        {
            if (_controller == null || _far == null) return;
            var state = _controller.State;
            _far.localPosition = new Vector3(_farOrigin.x - (state.PlayerPosition.x - TwoDOnboardingState.PlayerStart.x) * _farParallax, _farOrigin.y, 0f);
            _objective.text = state.ObjectiveText;
            _dialogue.text = state.DialogueOpen ? state.DialogueSpeaker + ": " + state.DialogueLine
                : "WASD / phím mũi tên: di chuyển     E: trò chuyện / tiếp tục     Bản thử mỹ thuật Đông Môn";
            _playerLabel.transform.localPosition = new Vector3(state.PlayerPosition.x - .35f, state.PlayerPosition.y + .83f, -1f);
        }

        private void LateUpdate() { Refresh(); }

        private IEnumerator Start()
        {
            if (!Application.isPlaying || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-dongmon-art-capture") < 0) yield break;
            _controller.enabled = false; // Capture uses deterministic state actions, no keyboard contamination.
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-dongmon-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing art capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            yield return null;
            Refresh();
            yield return Capture(directory, "01-arrival");
            var initial = FarOffset;
            _controller.State.Move(TwoDOnboardingState.GateKeeperPosition + Vector2.left * .85f - _controller.State.PlayerPosition);
            _controller.RefreshForSmoke(); Refresh();
            yield return Capture(directory, "02-approach");
            var delta = FarOffset - initial;
            var talked = _controller.State.TryUseAction() && _controller.State.DialogueOpen;
            _controller.RefreshForSmoke(); Refresh();
            yield return Capture(directory, "03-dialogue");
            var guided = _controller.State.TryUseAction() && _controller.State.Step == TwoDOnboardingStep.GoToTrainingStone;
            _controller.RefreshForSmoke(); Refresh();
            yield return Capture(directory, "04-guidance");
            _controller.State.Move(Vector2.left * .75f);
            _controller.RefreshForSmoke(); Refresh();
            yield return Capture(directory, "05-parallax");
            var result = new CaptureResult { status = talked && guided && Mathf.Abs(delta) > .01f ? "PASS" : "FIX_REQUIRED",
                pack = "dongmon-illustrated-draft-v1", finalStep = _controller.State.Step.ToString(), frames = 5,
                atlasParts = AtlasPartCount, dialogueOpened = talked, guidanceReached = guided, parallaxDelta = delta,
                npcArt = "NEW_DRAFT_ALPHA_SPRITE", playerArt = "EXISTING_PLACEHOLDER_CLASS_ART_SEPARATE" };
            File.WriteAllText(Path.Combine(directory, "manifest.json"), JsonUtility.ToJson(result, true));
            Debug.Log("LGO_DONGMON_ART_CAPTURE_" + result.status);
            Application.Quit(result.status == "PASS" ? 0 : 1);
        }

        private IEnumerator Capture(string directory, string name)
        {
            yield return new WaitForEndOfFrame();
            var camera = Camera.main;
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var target = new RenderTexture(1280, 720, 24);
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            try
            {
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                texture.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); texture.Apply();
                WriteBmp(Path.Combine(directory, name + ".bmp"), texture.GetPixels32(), 1280, 720);
            }
            finally
            {
                camera.targetTexture = previousTarget; RenderTexture.active = previousActive;
                Destroy(texture); target.Release(); Destroy(target);
            }
        }

        private static void WriteBmp(string path, Color32[] pixels, int width, int height)
        {
            var rowStride = ((width * 3 + 3) / 4) * 4;
            var imageSize = rowStride * height;
            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write)))
            {
                writer.Write((byte)'B');
                writer.Write((byte)'M');
                writer.Write(54 + imageSize);
                writer.Write(0);
                writer.Write(54);
                writer.Write(40);
                writer.Write(width);
                writer.Write(height);
                writer.Write((short)1);
                writer.Write((short)24);
                writer.Write(0);
                writer.Write(imageSize);
                writer.Write(2835);
                writer.Write(2835);
                writer.Write(0);
                writer.Write(0);

                var padding = new byte[rowStride - width * 3];
                for (var y = 0; y < height; y++)
                {
                    var row = y * width;
                    for (var x = 0; x < width; x++)
                    {
                        var pixel = pixels[row + x];
                        writer.Write(pixel.b);
                        writer.Write(pixel.g);
                        writer.Write(pixel.r);
                    }
                    if (padding.Length > 0) writer.Write(padding);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var pair in _hidden) if (pair.Key != null) pair.Key.enabled = pair.Value;
            foreach (var sprite in _sprites)
                if (sprite != null) { if (Application.isPlaying) Destroy(sprite); else DestroyImmediate(sprite); }
        }
    }
}
