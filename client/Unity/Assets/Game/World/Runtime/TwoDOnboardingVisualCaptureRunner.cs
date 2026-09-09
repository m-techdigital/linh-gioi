using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class TwoDOnboardingVisualCaptureRunner : MonoBehaviour
    {
        private readonly List<string> _screenshots = new List<string>();

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_2D_ONBOARDING_VISUAL_CAPTURE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-2d-onboarding-visual-capture") return true;
            return false;
        }

        public static void Attach(GameObject host)
        {
            if (host.GetComponent<TwoDOnboardingVisualCaptureRunner>() == null)
                host.AddComponent<TwoDOnboardingVisualCaptureRunner>();
        }

        private IEnumerator Start()
        {
            var evidenceDir = GetArg("--lgo-2d-visual-dir") ?? Path.Combine(Application.persistentDataPath, "lgo-2d-onboarding-visual");
            Directory.CreateDirectory(evidenceDir);
            var controller = TwoDOnboardingController.Attach(new GameObject("LGO 2D Visual Capture World"));
            yield return null;
            yield return Capture(evidenceDir, "01-initial");

            controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "02-gate-focus");

            controller.State.TryUseAction();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "03-dialogue");

            controller.State.TryUseAction();
            controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "04-stone-focus");

            controller.State.TryUseAction();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "05-jump-ready");

            controller.State.TryUseJump();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "06-dash-ready");

            controller.State.TryUseDash();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "07-skill-ready");

            controller.State.TryUseClassSkill();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "08-complete");

            controller.PreviewEastGateToPlazaTransition();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "14-plaza-transition-preview");

            controller.UseSelectedPlazaHubTarget();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "11-plaza-board-preview");

            controller.SelectNextPlazaHubTarget();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "12-plaza-target-selector");

            controller.SelectNextPlazaHubTarget();
            controller.UseSelectedPlazaHubTarget();
            controller.RefreshForSmoke();
            yield return null;
            yield return Capture(evidenceDir, "13-plaza-npc-preview");

            controller.ToggleInventoryPanel();
            controller.PreviewSelectedInventoryItem();
            yield return null;
            yield return Capture(evidenceDir, "09-inventory-try");

            controller.ApplyInventoryPreview();
            yield return null;
            yield return Capture(evidenceDir, "10-inventory-applied");

            var resultPath = Path.Combine(evidenceDir, "twod-onboarding-visual-manifest.json");
            var result = new TwoDOnboardingVisualCaptureResult
            {
                status = _screenshots.Count == 14 ? "PASS" : "FAIL",
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                evidenceDir = evidenceDir,
                finalStep = controller.State.Step.ToString(),
                finalObjective = controller.State.ObjectiveText,
                hudLineCount = controller.WorldHudLineCount,
                hudSnapshot = controller.WorldHudSnapshot,
                productionSceneBeatCount = controller.ProductionSceneBeatCount,
                productionSceneBeatSnapshot = controller.ProductionSceneBeatSnapshot,
                runtimeMapSnapshot = controller.RuntimeMapSnapshot,
                runtimeLinhThanhUnlockSnapshot = controller.RuntimeLinhThanhUnlockSnapshot,
                runtimeLinhThanhPlazaHubSnapshot = controller.RuntimeLinhThanhPlazaHubSnapshot,
                runtimeLinhThanhAcademyShellSnapshot = controller.RuntimeLinhThanhAcademyShellSnapshot,
                runtimeLinhThanhMarketShellSnapshot = controller.RuntimeLinhThanhMarketShellSnapshot,
                runtimeLinhThanhSpiritTempleShellSnapshot = controller.RuntimeLinhThanhSpiritTempleShellSnapshot,
                runtimeLinhThanhResidentialShellSnapshot = controller.RuntimeLinhThanhResidentialShellSnapshot,
                runtimeHubTransitionSnapshot = controller.RuntimeHubTransitionSnapshot,
                runtimeCharacterBaseSnapshot = controller.RuntimeCharacterBaseSnapshot,
                runtimeEquipmentSnapshot = controller.RuntimeEquipmentSnapshot,
                runtimeInventoryTryOnSnapshot = controller.RuntimeInventoryTryOnSnapshot,
                runtimeInventoryInputSnapshot = controller.RuntimeInventoryInputSnapshot,
                runtimePlazaHubInputSnapshot = controller.RuntimePlazaHubInputSnapshot,
                runtimePlazaReadabilitySnapshot = controller.RuntimePlazaReadabilitySnapshot,
                runtimeTerrainCollisionSnapshot = controller.RuntimeTerrainCollisionSnapshot,
                runtimeTilemapSnapshot = controller.RuntimeTilemapSnapshot,
                runtimeAnimationSnapshot = controller.RuntimeAnimationSnapshot,
                runtimeCombatSnapshot = controller.RuntimeCombatSnapshot,
                runtimeRouteProgressSnapshot = controller.RuntimeRouteProgressSnapshot,
                screenshotCount = _screenshots.Count,
                screenshots = _screenshots.ToArray()
            };
            File.WriteAllText(resultPath, JsonUtility.ToJson(result, true));
            Debug.Log($"[LinhGioi] 2D onboarding visual capture status={result.status} screenshots={result.screenshotCount} result={resultPath}");
            Quit(result.status == "PASS" ? 0 : 14);
        }

        private IEnumerator Capture(string evidenceDir, string name)
        {
            yield return new WaitForEndOfFrame();
            var path = Path.Combine(evidenceDir, name + ".bmp");
            var camera = Camera.main;
            if (camera == null) yield break;

            var width = Mathf.Max(320, Screen.width);
            var height = Mathf.Max(180, Screen.height);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            try
            {
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply(false, false);
                WriteBmp(path, texture.GetPixels32(), width, height);
                if (File.Exists(path) && new FileInfo(path).Length > 0) _screenshots.Add(path);
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                Destroy(texture);
                renderTexture.Release();
                Destroy(renderTexture);
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
        private sealed class TwoDOnboardingVisualCaptureResult
        {
            public string status;
            public string unityVersion;
            public string platform;
            public string evidenceDir;
            public string finalStep;
            public string finalObjective;
            public int hudLineCount;
            public string hudSnapshot;
            public int productionSceneBeatCount;
            public string productionSceneBeatSnapshot;
            public string runtimeMapSnapshot;
            public string runtimeLinhThanhUnlockSnapshot;
            public string runtimeLinhThanhPlazaHubSnapshot;
            public string runtimeLinhThanhAcademyShellSnapshot;
            public string runtimeLinhThanhMarketShellSnapshot;
            public string runtimeLinhThanhSpiritTempleShellSnapshot;
            public string runtimeLinhThanhResidentialShellSnapshot;
            public string runtimeHubTransitionSnapshot;
            public string runtimeCharacterBaseSnapshot;
            public string runtimeEquipmentSnapshot;
            public string runtimeInventoryTryOnSnapshot;
            public string runtimeInventoryInputSnapshot;
            public string runtimePlazaHubInputSnapshot;
            public string runtimePlazaReadabilitySnapshot;
            public string runtimeTerrainCollisionSnapshot;
            public string runtimeTilemapSnapshot;
            public string runtimeAnimationSnapshot;
            public string runtimeCombatSnapshot;
            public string runtimeRouteProgressSnapshot;
            public int screenshotCount;
            public string[] screenshots;
        }
    }
}
