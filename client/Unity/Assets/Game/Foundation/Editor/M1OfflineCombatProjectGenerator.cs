using System;
using System.IO;
using LinhGioi.Combat;
using LinhGioi.CombatUI;
using LinhGioi.UI.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LinhGioi.Foundation.Editor
{
    [InitializeOnLoad]
    public static class M1OfflineCombatProjectGenerator
    {
        private const string GeneratedRoot = "Assets/Game/Generated";
        private const string GameDataFolder = GeneratedRoot + "/GameData";
        private const string GameDataManifestPath = GameDataFolder + "/gamedata-manifest.json";
        private const string PanelSettingsPath = GeneratedRoot + "/UI/LinhGioiM1CombatPanelSettings.asset";
        private const string M1ScenePath = GeneratedRoot + "/Scenes/M1OfflineCombatPrototype.unity";

        static M1OfflineCombatProjectGenerator()
        {
            EditorApplication.delayCall += EnsureGeneratedM1OfflineCombat;
        }

        [MenuItem("Tools/Linh Gioi/Rebuild M1 Offline Combat Prototype")]
        public static void RebuildM1OfflineCombatPrototype()
        {
            DeleteIfExists(M1ScenePath);
            DeleteIfExists(GameDataManifestPath);
            AssetDatabase.Refresh();
            EnsureGeneratedM1OfflineCombat();
        }

        public static void EnsureGeneratedM1OfflineCombat()
        {
            try
            {
                EnsureFolder(GeneratedRoot + "/Scenes");
                EnsureFolder(GeneratedRoot + "/UI");
                EnsureFolder(GameDataFolder);
                CopyCompiledGameDataManifest();
                AssetDatabase.ImportAsset(GameDataManifestPath, ImportAssetOptions.ForceUpdate);
                if (!File.Exists(M1ScenePath)) CreatePrototypeScene();
                AssetDatabase.SaveAssets();
                Debug.Log("[LinhGioi] M1 offline combat prototype scene is ready.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static void CreatePrototypeScene()
        {
            var theme = ThemeTokenImporter.EnsureTheme();
            var manifest = AssetDatabase.LoadAssetAtPath<TextAsset>(GameDataManifestPath);
            if (manifest == null) throw new InvalidOperationException("M1 GameData manifest TextAsset could not be loaded.");

            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panelSettings, PanelSettingsPath);
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "M1OfflineCombatPrototype";

            CreateDefaultCamera();
            CreateDefaultLight();

            var root = new GameObject("M1OfflineCombatPrototype");
            var document = root.AddComponent<UIDocument>();
            document.panelSettings = panelSettings;
            var hud = root.AddComponent<OfflineCombatHudController>();
            hud.Configure(theme, manifest);
            var prototype = root.AddComponent<M1OfflineCombatPrototypeController>();
            var serializedPrototype = new SerializedObject(prototype);
            serializedPrototype.FindProperty("compiledGameDataManifest").objectReferenceValue = manifest;
            serializedPrototype.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, M1ScenePath);
        }

        private static void CreateDefaultCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 2f, -10f);
            cameraObject.transform.rotation = Quaternion.identity;
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 60f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000f;
            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreateDefaultLight()
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
        }

        private static void CopyCompiledGameDataManifest()
        {
            var unityRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var sourcePath = Path.GetFullPath(Path.Combine(unityRoot, "..", "..", "gamedata", "compiled", "gamedata-manifest.json"));
            if (!File.Exists(sourcePath)) throw new FileNotFoundException("Canonical compiled GameData manifest is missing.", sourcePath);
            File.Copy(sourcePath, GameDataManifestPath, true);
        }

        private static void DeleteIfExists(string path)
        {
            if (AssetDatabase.IsValidFolder(path) || File.Exists(path)) AssetDatabase.DeleteAsset(path);
        }

        private static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
