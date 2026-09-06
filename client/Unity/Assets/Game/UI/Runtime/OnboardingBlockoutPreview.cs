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

        public static bool ShouldRun(string[] args, bool development) =>
            development && Array.IndexOf(args, "--lgo-onboarding-blockout") >= 0;

        private void Awake()
        {
            _world = gameObject.AddComponent<OnboardingBlockoutWorld>();
            var document = gameObject.AddComponent<UIDocument>();
            document.panelSettings = RuntimePanelSettingsProvider.LoadOrCreate();
            var root = document.rootVisualElement;
            var overlay = RuntimeUiFactory.NewWorldTouchControlsOverlay();
            _pad = (RuntimeTouchMovementPad)RuntimeUiFactory.NewWorldTouchPad();
            var cluster = RuntimeUiFactory.NewWorldTouchActionCluster();
            var quit = RuntimeUiFactory.NewWorldTouchActionButton("Thoát", () => Application.Quit());
            overlay.Add(_pad);
            overlay.Add(cluster);
            overlay.Add(quit);
            root.Add(overlay);
            root.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                var layout = RuntimeUiLayoutProfile.FromScreen(null, Screen.width, Screen.height,
                    Mathf.RoundToInt(root.contentRect.width), Mathf.RoundToInt(root.contentRect.height));
                RuntimeWorldHudResponsiveLayout.ApplyTouchAffordances(layout, true, false, false,
                    overlay, _pad, cluster, null, null, null, null, quit);
            });
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
            yield return WalkTo(new Vector3(-1.8f, 0f, 1f));
            yield return Capture(directory, "keeper-side");
            yield return WalkTo(new Vector3(0f, 0f, 2.5f));
            yield return WalkTo(new Vector3(2.3f, 0f, 4f));
            yield return Capture(directory, "stone-side");
            if (GetComponent<M4PlayableClientController>() != null)
                throw new InvalidOperationException("Blockout must not create the account client UI.");
            Debug.Log("LGO_ONBOARDING_BLOCKOUT_ROUTE_PASS movement=CharacterController no_teleport=true");
            Application.Quit(0);
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
            if (!_capturing) _world.Movement = Application.isFocused ? _pad.Value : Vector2.zero;
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        private void OnApplicationFocus(bool focused)
        {
            _pad?.ResetInput();
            if (_world != null) _world.Movement = Vector2.zero;
        }
    }
}
