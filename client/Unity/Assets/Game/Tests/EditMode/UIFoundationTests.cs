using LinhGioi.Foundation;
using LinhGioi.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace LinhGioi.Tests
{
    public sealed class UIFoundationTests
    {
        [UnityTest]
        public IEnumerator TouchPadPointerReleaseAndCaptureLossClearMovement()
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
                Assert.Ignore("Pointer capture integration needs a UI panel; run this test without -nographics.");
            var window = ScriptableObject.CreateInstance<EditorWindow>();
            try
            {
                window.Show();
                var pad = new RuntimeTouchMovementPad();
                pad.style.width = 100;
                pad.style.height = 100;
                window.rootVisualElement.Add(pad);
                yield return null;
                yield return null;
                var point = pad.worldBound.center + new Vector2(40, 0);
                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                    pad.SendEvent(down);
                Assert.That(pad.Value.x, Is.GreaterThan(0.5f));
                using (var up = PointerUpEvent.GetPooled(new Event { type = EventType.MouseUp, button = 0, mousePosition = point + Vector2.right * 300 }))
                    pad.SendEvent(up);
                Assert.AreEqual(Vector2.zero, pad.Value);

                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                    pad.SendEvent(down);
                yield return null;
                Assert.That(pad.Value.x, Is.GreaterThan(0.5f));
                Assert.IsTrue(pad.HasPointerCapture(PointerId.mousePointerId));
                pad.ReleasePointer(PointerId.mousePointerId);
                using (var move = PointerMoveEvent.GetPooled(new Event { type = EventType.MouseMove, mousePosition = point }))
                    pad.SendEvent(move);
                yield return null;
                Assert.AreEqual(Vector2.zero, pad.Value);
            }
            finally { window.Close(); }
        }

        [Test]
        public void TouchPadDisplacementIsScaleIndependentAndBounded()
        {
            var small = RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(20, -10), 40);
            var large = RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(40, -20), 80);
            Assert.That(Vector2.Distance(small, large), Is.LessThan(0.0001f));
            var diagonal = RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(400, 400), 40);
            Assert.That(diagonal.magnitude, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(diagonal.x, Is.EqualTo(diagonal.y).Within(0.0001f));
        }

        [Test]
        public void TouchPadCenterAndInvalidGeometryDoNotMovePlayer()
        {
            Assert.AreEqual(Vector2.zero, RuntimeTouchMovementPad.NormalizeDisplacement(new Vector2(1, 1), 40));
            Assert.AreEqual(Vector2.zero, RuntimeTouchMovementPad.NormalizeDisplacement(Vector2.one, 0));
            Assert.AreEqual(Vector2.zero, RuntimeTouchMovementPad.NormalizeDisplacement(Vector2.one, float.NaN));
        }

        [Test]
        public void ThemeParsesAuthoritativeTokens()
        {
            const string json = "{\"version\":1,\"colors\":{\"bg\":\"#0B1324\",\"surface\":\"#111D32\",\"surfaceRaised\":\"#182741\",\"spirit\":\"#28D7C7\",\"shadow\":\"#9B5CFF\",\"gold\":\"#E6B85C\",\"danger\":\"#E35D6A\",\"text\":\"#F5F2EA\",\"muted\":\"#9BA7BC\"},\"spacing\":[4,8,12,16,24,32,48,64],\"minimumTouchTarget\":44}";
            var theme = ThemeTokens.FromJson(json);
            Assert.AreEqual(44, theme.minimumTouchTarget);
            Assert.AreEqual(8, theme.spacing.Length);
            Object.DestroyImmediate(theme);
        }

        [Test]
        public void RuntimeTextureMemoryAuditSummarizesDistinctTextures()
        {
            var first = new Texture2D(64, 32, TextureFormat.RGBA32, false) { name = "audit-first" };
            var second = new Texture2D(32, 32, TextureFormat.RGBA32, false) { name = "audit-second" };
            try
            {
                var snapshot = RuntimeTextureMemoryAudit.Summarize(new[] { first, first, second }, new[] { first.GetInstanceID() });
                Assert.That(snapshot.textureCount, Is.EqualTo(2));
                Assert.That(snapshot.totalEstimatedStorageBytes, Is.EqualTo(64L * 32L * 4L + 32L * 32L * 4L));
                Assert.That(snapshot.entries.Length, Is.EqualTo(2));
                Assert.That(snapshot.entries.Count(entry => entry.wasResidentBeforeAudit), Is.EqualTo(1));
                Assert.That(snapshot.entries.Single(entry => entry.name == "audit-first").width, Is.EqualTo(64));
                Assert.That(snapshot.entries.Single(entry => entry.name == "audit-first").estimatedStorageBytes,
                    Is.EqualTo(64L * 32L * 4L));
                Assert.That(snapshot.entries.Single(entry => entry.name == "audit-second").height, Is.EqualTo(32));
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
            }
        }

        [Test]
        public void RuntimeTextureMemoryAuditRunnerRequiresExplicitFlag()
        {
            Assert.That(RuntimeTextureMemoryAuditRunner.ShouldRunForArgs(new[] { "LinhGioiOnline" }), Is.False);
            Assert.That(RuntimeTextureMemoryAuditRunner.ShouldRunForArgs(
                new[] { "LinhGioiOnline", "--lgo-runtime-texture-memory-audit" }), Is.True);
        }

        [Test]
        public void LargeRuntimeTexturesHaveExplicitMobileImportProfiles()
        {
            const string resourcesRoot = "Assets/Game/World/Runtime/Resources";
            var texturePaths = AssetDatabase.FindAssets("t:Texture2D", new[] { resourcesRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path)
                .ToArray();
            var largeCount = 0;
            foreach (var path in texturePaths)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null || Mathf.Max(texture.width, texture.height) < 1024) continue;
                largeCount++;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer, Is.Not.Null, path);
                Assert.That(importer.mipmapEnabled, Is.False, path + " must not waste memory on world/UI mipmaps.");
                Assert.That(importer.isReadable, Is.False, path + " must not keep a CPU-readable texture copy.");

                var environment = path.Contains("/LGOMaps/CongDongLamMap01AArt/")
                    || path.Contains("/LGOMaps/DongMonIllustrated/");
                var expectedFormatCode = environment ? 50 : 48;
                var minimumMaxSize = Mathf.NextPowerOfTwo(Mathf.Max(texture.width, texture.height));
                foreach (var target in new[] { BuildTarget.Android, BuildTarget.iOS })
                {
                    var targetName = BuildPipeline.GetBuildTargetName(target);
                    var settings = importer.GetPlatformTextureSettings(targetName);
                    Assert.That(settings.overridden, Is.True, path + " missing " + targetName + " override.");
                    Assert.That(settings.maxTextureSize, Is.InRange(minimumMaxSize, 2048),
                        path + " " + targetName + " must preserve source resolution without an unbounded cap.");
                    Assert.That((int)settings.format, Is.EqualTo(expectedFormatCode), path + " " + targetName);
                    Assert.That(settings.textureCompression, Is.EqualTo(TextureImporterCompression.CompressedHQ),
                        path + " " + targetName + " must use explicit high-quality mobile compression.");
                }
            }
            Assert.That(largeCount, Is.GreaterThanOrEqualTo(19), "Current runtime inventory unexpectedly lost large textures.");
        }

        [Test]
        public void RuntimeThemeProviderLoadsGeneratedDesignTokens()
        {
            var type = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeUiTheme");
            Assert.That(type, Is.Not.Null, "Product UI needs one runtime token provider.");
            var current = type.GetProperty("Current", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(current, Is.Not.Null);
            var theme = current.GetValue(null) as ThemeTokens;
            Assert.That(theme, Is.Not.Null);
            Assert.That(theme.minimumTouchTarget, Is.EqualTo(44));
            Assert.That(theme.gold.r, Is.EqualTo(230f / 255f).Within(0.0001f));
            Assert.That(theme.gold.g, Is.EqualTo(184f / 255f).Within(0.0001f));
            Assert.That(theme.gold.b, Is.EqualTo(92f / 255f).Within(0.0001f));
            Assert.That(theme.gold.a, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void ProductHudDoesNotDeclareSecondSemanticPalette()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath,
                "Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs"));
            foreach (var field in new[] { "UiGold", "UiText", "UiBlue", "UiGlass", "UiGlassStrong", "UiGlassRaised" })
                Assert.That(source, Does.Not.Contain("private static readonly Color " + field), field);
        }

        [Test]
        public void ProductRootAttachesOneSharedRuntimeStyleSheet()
        {
            var type = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeUiStyleSheetProvider");
            Assert.That(type, Is.Not.Null, "Product UI needs one shared USS provider.");
            var attach = type.GetMethod("Attach", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(attach, Is.Not.Null);
            var root = new VisualElement();
            Assert.That((bool)attach.Invoke(null, new object[] { root }), Is.True);
            Assert.That(root.styleSheets.count, Is.EqualTo(1));
            Assert.That((bool)attach.Invoke(null, new object[] { root }), Is.False);
            Assert.That(root.styleSheets.count, Is.EqualTo(1), "Shared stylesheet must not be attached twice.");
        }

        [Test]
        public void ProductSkinHelpersAttachSharedRoleClasses()
        {
            var action = new Button(); InvokeHudSkin("ApplyLgoButton", action, false);
            Assert.That(action.ClassListContains("lgo-action"), Is.True);
            var input = new VisualElement(); InvokeHudSkin("ApplyLgoInputField", input);
            Assert.That(input.ClassListContains("lgo-input"), Is.True);
            var tab = new Button(); InvokeHudSkin("ApplyLgoInventoryMainTab", tab, false);
            Assert.That(tab.ClassListContains("lgo-tab"), Is.True);
            var icon = new VisualElement(); InvokeHudSkin("ApplyLgoItemIcon", icon);
            Assert.That(icon.ClassListContains("lgo-icon-frame"), Is.True);
            var badge = new Label(); InvokeHudSkin("ApplyLgoInventoryBadge", badge);
            Assert.That(badge.ClassListContains("lgo-badge"), Is.True);
            var status = new VisualElement(); InvokeHudSkin("ApplyLgoStatusCard", status, 10f, 8f);
            Assert.That(status.ClassListContains("lgo-status"), Is.True);
            var utility = new Button(); InvokeHudSkin("ApplyLgoEntrySideAction", utility);
            Assert.That(utility.ClassListContains("lgo-utility-action"), Is.True);
            var ornament = new VisualElement(); InvokeHudSkin("ApplyLgoOrnamentRail", ornament);
            Assert.That(ornament.ClassListContains("lgo-ornament"), Is.True);
            var modal = new VisualElement(); InvokeHudSkin("ApplyLgoModalShell", modal, 12f);
            Assert.That(modal.ClassListContains("lgo-modal"), Is.True);
        }

        private static void InvokeHudSkin(string methodName, params object[] arguments)
        {
            var methods = typeof(CongDongLamArrivalHud).GetMethods(System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.Name != methodName || method.GetParameters().Length != arguments.Length) continue;
                method.Invoke(null, arguments);
                return;
            }
            Assert.Fail("Missing HUD skin helper: " + methodName);
        }

        [Test]
        public void RuntimePanelPolicyUsesCanonicalLandscapeHeightAuthority()
        {
            var type = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimePanelSettingsProvider");
            Assert.That(type, Is.Not.Null);
            var load = type.GetMethod("LoadOrCreate", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var settings = load.Invoke(null, null) as PanelSettings;
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.scaleMode, Is.EqualTo(PanelScaleMode.ScaleWithScreenSize));
            Assert.That(settings.referenceResolution, Is.EqualTo(new Vector2Int(1672, 941)));
            Assert.That(settings.screenMatchMode, Is.EqualTo(PanelScreenMatchMode.MatchWidthOrHeight));
            Assert.That(settings.match, Is.EqualTo(1f).Within(.001f),
                "Landscape game UI must scale from safe-panel height instead of becoming oversized on wide phones.");
        }

        [TestCase("desktop", 1600, 900, "desktop", 3.8f, .18f, .27f, .16f, .27f)]
        [TestCase("tablet", 1024, 768, "tablet", 3.8f, .18f, .27f, .16f, .27f)]
        [TestCase("mobile", 1600, 720, "mobile", 4.5f, .16f, .20f, .18f, .23f)]
        public void WorldPresentationProfileOwnsShippingMapScale(string forcedProfile, int width, int height,
            string expectedName, float cameraSize, float actorMin, float actorMax, float npcMin, float npcMax)
        {
            var profile = RuntimeWorldPresentationProfile.FromScreen(forcedProfile, width, height);
            Assert.That(profile.Name, Is.EqualTo(expectedName));
            Assert.That(profile.CameraOrthographicSize, Is.EqualTo(cameraSize).Within(.001f));
            Assert.That(profile.CameraGroundOffsetY, Is.EqualTo(1.5f).Within(.001f));
            Assert.That(profile.ActorMinScreenHeightRatio, Is.EqualTo(actorMin).Within(.001f));
            Assert.That(profile.ActorMaxScreenHeightRatio, Is.EqualTo(actorMax).Within(.001f));
            Assert.That(profile.NpcMinScreenHeightRatio, Is.EqualTo(npcMin).Within(.001f));
            Assert.That(profile.NpcMaxScreenHeightRatio, Is.EqualTo(npcMax).Within(.001f));
            Assert.That(profile.InteractionMarkerFontSize, Is.EqualTo(64));
            Assert.That(profile.InteractionMarkerCharacterSize, Is.EqualTo(.045f).Within(.001f));
        }

        [Test]
        public void WorldPresentationMetricsMeasureViewportHeightWithoutScreenGuessing()
        {
            var cameraObject = new GameObject("world presentation metric camera");
            try
            {
                var camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.orthographicSize = 3.8f;
                camera.transform.position = new Vector3(0f, 0f, -10f);
                var actorBounds = new Bounds(Vector3.zero, new Vector3(1f, 1.70f, .1f));
                var npcBounds = new Bounds(new Vector3(2f, 0f, 0f), new Vector3(1f, 1.90f, .1f));

                Assert.That(RuntimeWorldPresentationMetrics.ScreenHeightRatio(camera, actorBounds),
                    Is.EqualTo(1.70f / 7.6f).Within(.002f));
                Assert.That(RuntimeWorldPresentationMetrics.ScreenHeightRatio(camera, npcBounds),
                    Is.EqualTo(1.90f / 7.6f).Within(.002f));
                Assert.That(RuntimeWorldPresentationMetrics.ScreenHeightRatio(null, actorBounds), Is.Zero);
                Assert.That(RuntimeWorldPresentationMetrics.ScreenHeightRatio(camera, new Bounds(Vector3.zero, Vector3.zero)), Is.Zero);
            }
            finally
            {
                Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void PlayableWorldReusesSharedPresentationClassifier()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath,
                "Game/World/Runtime/PlayableWorldController.cs"));
            Assert.That(source, Does.Contain("RuntimeWorldPresentationProfile.FromScreen"),
                "Prototype/smoke world may retain its legacy camera values, but device classification must reuse the shared authority.");
            Assert.That(source, Does.Not.Contain("private readonly struct WorldLayoutProfile"),
                "World profile classification must not be duplicated inside PlayableWorldController.");
        }

        [Test]
        public void HudAttachDoesNotOverrideSharedPanelScalePolicy()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath,
                "Game/UI/Runtime/CongDongLamArrivalHud.cs"));
            Assert.That(source, Does.Not.Contain("hud._ownedPanel.referenceResolution ="));
            Assert.That(source, Does.Not.Contain("hud._ownedPanel.screenMatchMode ="));
            Assert.That(source, Does.Not.Contain("hud._ownedPanel.match ="));
            Assert.That(source, Does.Not.Contain("hud._ownedPanel.scaleMode ="));
        }

        [Test]
        public void ViewportMetricsConvertWidePhoneSafeAreaWithoutOverflow()
        {
            var viewport = CreateViewportMetrics(2532, 1170, new Rect(132, 63, 2268, 1070), 2037, 941, "mobile");
            Assert.That(GetMember<string>(viewport, "LayoutClass"), Is.EqualTo("mobile"));
            Assert.That(GetMember<string>(viewport, "InputClass"), Is.EqualTo("touch"));
            var safe = GetMember<Rect>(viewport, "SafePanelRect");
            var panelWidth = GetMember<int>(viewport, "PanelWidth");
            var panelHeight = GetMember<int>(viewport, "PanelHeight");
            Assert.That(safe.x, Is.GreaterThan(0));
            Assert.That(safe.y, Is.GreaterThanOrEqualTo(0));
            Assert.That(safe.xMax, Is.LessThanOrEqualTo(panelWidth + .01f));
            Assert.That(safe.yMax, Is.LessThanOrEqualTo(panelHeight + .01f));
        }

        [Test]
        public void MobileLandscapeProfileCapsCharacterHubHeightWithoutShrinkingTouchControls()
        {
            var viewport = CreateViewportMetrics(1600, 720,
                new Rect(0, 0, 1600, 720), 2091, 941, "mobile");
            var uiAssembly = typeof(ThemeTokens).Assembly;
            var layoutType = uiAssembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            var viewportType = uiAssembly.GetType("LinhGioi.UI.RuntimeViewportMetrics");
            var fromViewport = layoutType.GetMethod("FromViewport", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null,
                new[] { viewportType }, null);
            var layout = fromViewport.Invoke(null, new[] { viewport });

            Assert.That(GetMember<float>(layout, "CharacterHubShellMaxHeight"), Is.EqualTo(640f).Within(.01f),
                "Mobile landscape must expose a smaller vertical shell authority instead of reusing the 724-unit desktop shell.");
            Assert.That(GetMember<float>(layout, "GameplayHudTouchPadSize"), Is.EqualTo(124f).Within(.01f),
                "Fixing shell occupancy must not shrink the touch joystick target.");
            var themeType = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeUiTheme");
            var current = themeType.GetProperty("Current", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var theme = current.GetValue(null) as ThemeTokens;
            Assert.That(theme.minimumTouchTarget, Is.EqualTo(44),
                "Fixing mobile presentation scale must preserve the semantic minimum touch target.");
        }

        [Test]
        public void CharacterHubLayoutConsumesProfileOwnedShellHeight()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath,
                "Game/UI/Runtime/CongDongLamArrivalHud.cs"));
            Assert.That(source, Does.Contain("layout.CharacterHubShellMaxHeight"),
                "Character Hub placement must consume the profile-owned mobile height cap.");
        }

        [TestCase(1600, 900, 1673, 941, "desktop", "pointer", 286f, 154f, 15)]
        [TestCase(1600, 720, 2091, 941, "mobile", "touch", 228f, 226f, 16)]
        [TestCase(1024, 768, 1255, 941, "tablet", "touch", 286f, 226f, 16)]
        public void LayoutProfileOwnsHudDensityByDeviceProfile(int screenWidth, int screenHeight,
            int panelWidth, int panelHeight, string forcedProfile, string inputClass,
            float rightColumnWidth, float combatBarBottom, int talkFontSize)
        {
            var viewport = CreateViewportMetrics(screenWidth, screenHeight,
                new Rect(0, 0, screenWidth, screenHeight), panelWidth, panelHeight, forcedProfile);
            var uiAssembly = typeof(ThemeTokens).Assembly;
            var layoutType = uiAssembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            var viewportType = uiAssembly.GetType("LinhGioi.UI.RuntimeViewportMetrics");
            Assert.That(layoutType, Is.Not.Null);
            var fromViewport = layoutType.GetMethod("FromViewport", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null,
                new[] { viewportType }, null);
            Assert.That(fromViewport, Is.Not.Null);
            var layout = fromViewport.Invoke(null, new[] { viewport });
            Assert.That(GetMember<string>(layout, "Name"), Is.EqualTo(forcedProfile));
            Assert.That(GetMember<string>(layout, "InputClass"), Is.EqualTo(inputClass));
            Assert.That(GetMember<float>(layout, "WorldRightColumnWidth"), Is.EqualTo(rightColumnWidth).Within(.01f));
            Assert.That(GetMember<float>(layout, "WorldCombatBarBottom"), Is.EqualTo(combatBarBottom).Within(.01f));
            Assert.That(GetMember<int>(layout, "WorldTalkFontSize"), Is.EqualTo(talkFontSize));
        }

        [TestCase("desktop", 1600, 900, 1673, 941)]
        [TestCase("tablet", 1024, 768, 1255, 941)]
        [TestCase("mobile", 1600, 720, 2091, 941)]
        public void GameplayHudLayoutKeepsEdgeZonesInsideSafeArea(string profile, int screenWidth, int screenHeight,
            int panelWidth, int panelHeight)
        {
            var viewport = CreateViewportMetrics(screenWidth, screenHeight,
                new Rect(0, 0, screenWidth, screenHeight), panelWidth, panelHeight, profile);
            var uiAssembly = typeof(ThemeTokens).Assembly;
            var profileType = uiAssembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            var viewportType = uiAssembly.GetType("LinhGioi.UI.RuntimeViewportMetrics");
            var fromViewport = profileType.GetMethod("FromViewport", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null,
                new[] { viewportType }, null);
            var runtimeProfile = fromViewport.Invoke(null, new[] { viewport });
            var layoutType = uiAssembly.GetType("LinhGioi.UI.RuntimeGameplayHudLayout");
            Assert.That(layoutType, Is.Not.Null, "UIF-06 requires one pure gameplay-HUD geometry authority.");
            var calculate = layoutType.GetMethod("Calculate", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(calculate, Is.Not.Null);
            var safe = GetMember<Rect>(viewport, "SafePanelRect");
            var layout = calculate.Invoke(null, new object[] { safe, runtimeProfile });
            foreach (var name in new[] { "PlayerStatus", "RightInfo", "Combat", "Context", "SecondaryNav", "Dialogue" })
            {
                var rect = GetMember<Rect>(layout, name);
                Assert.That(rect.xMin, Is.GreaterThanOrEqualTo(safe.xMin - .01f), name);
                Assert.That(rect.yMin, Is.GreaterThanOrEqualTo(safe.yMin - .01f), name);
                Assert.That(rect.xMax, Is.LessThanOrEqualTo(safe.xMax + .01f), name);
                Assert.That(rect.yMax, Is.LessThanOrEqualTo(safe.yMax + .01f), name);
            }
            Assert.That(GetMember<Rect>(layout, "Combat").Overlaps(GetMember<Rect>(layout, "RightInfo")), Is.False);
            Assert.That(GetMember<Rect>(layout, "Context").Overlaps(GetMember<Rect>(layout, "RightInfo")), Is.False);
            Assert.That(GetMember<Rect>(layout, "SecondaryNav").Overlaps(GetMember<Rect>(layout, "Combat")), Is.False);
        }

        [TestCase("tablet", 1024, 768, 1255, 941)]
        [TestCase("mobile", 1600, 720, 2091, 941)]
        public void GameplayHudLayoutKeepsTouchThumbZonesSeparated(string profile, int screenWidth, int screenHeight,
            int panelWidth, int panelHeight)
        {
            var viewport = CreateViewportMetrics(screenWidth, screenHeight,
                new Rect(0, 0, screenWidth, screenHeight), panelWidth, panelHeight, profile);
            var uiAssembly = typeof(ThemeTokens).Assembly;
            var profileType = uiAssembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            var viewportType = uiAssembly.GetType("LinhGioi.UI.RuntimeViewportMetrics");
            var fromViewport = profileType.GetMethod("FromViewport", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null,
                new[] { viewportType }, null);
            var runtimeProfile = fromViewport.Invoke(null, new[] { viewport });
            var layoutType = uiAssembly.GetType("LinhGioi.UI.RuntimeGameplayHudLayout");
            Assert.That(layoutType, Is.Not.Null);
            var calculate = layoutType.GetMethod("Calculate", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var safe = GetMember<Rect>(viewport, "SafePanelRect");
            var layout = calculate.Invoke(null, new object[] { safe, runtimeProfile });
            var touchPad = GetMember<Rect>(layout, "TouchPad");
            var combat = GetMember<Rect>(layout, "Combat");
            var context = GetMember<Rect>(layout, "Context");
            Assert.That(touchPad.width, Is.GreaterThan(0f));
            Assert.That(touchPad.xMax, Is.LessThan(safe.center.x), "Movement belongs to the left thumb zone.");
            Assert.That(combat.xMin, Is.GreaterThan(safe.center.x), "Combat belongs to the right thumb zone.");
            Assert.That(touchPad.Overlaps(combat), Is.False);
            Assert.That(touchPad.Overlaps(context), Is.False);
            Assert.That(context.Overlaps(combat), Is.False);
        }

        [TestCase("desktop", 1600, 900, 1673, 941, 540f, 565f)]
        [TestCase("tablet", 1024, 768, 1255, 941, 460f, 480f)]
        [TestCase("mobile", 1600, 720, 2091, 941, 435f, 450f)]
        public void EntryLoginProfileKeepsCanonicalCardAndBrandScale(string profile, int screenWidth, int screenHeight,
            int panelWidth, int panelHeight, float minimumScreenCardWidth, float maximumScreenCardWidth)
        {
            var layoutType = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            Assert.That(layoutType, Is.Not.Null);
            var fromScreen = layoutType.GetMethod("FromScreen", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(fromScreen, Is.Not.Null);
            var layout = fromScreen.Invoke(null, new object[] { profile, screenWidth, screenHeight, panelWidth, panelHeight });
            var cardWidth = GetMember<float>(layout, "LoginCardWidth");
            var logoWidth = GetMember<float>(layout, "LoginLogoWidth");
            var screenCardWidth = cardWidth * screenHeight / panelHeight;
            Assert.That(screenCardWidth, Is.InRange(minimumScreenCardWidth, maximumScreenCardWidth),
                "Entry form must preserve the canonical height-scaled physical width instead of desktop-style oversizing on touch profiles.");
            Assert.That(logoWidth, Is.EqualTo(cardWidth * .96f).Within(.01f),
                "Brand and form scale must come from one responsive profile contract.");
        }

        private static object CreateViewportMetrics(int screenWidth, int screenHeight, Rect safeArea,
            int panelWidth, int panelHeight, string forcedProfile)
        {
            var type = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeViewportMetrics");
            Assert.That(type, Is.Not.Null);
            var method = type.GetMethod("FromMeasurements", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null, "Viewport math needs a pure measurement entry point for device tests.");
            return method.Invoke(null, new object[] { screenWidth, screenHeight, safeArea, panelWidth, panelHeight, forcedProfile });
        }

        private static T GetMember<T>(object target, string name)
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic;
            var type = target.GetType();
            var field = type.GetField(name, flags);
            if (field != null) return (T)field.GetValue(target);
            var property = type.GetProperty(name, flags);
            Assert.That(property, Is.Not.Null, "Missing semantic UI member: " + name);
            return (T)property.GetValue(target);
        }

        [Test]
        public void UiEvidenceMetricsQuantifyCompactLandscapeShellAndTouchScale()
        {
            var type = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeUiEvidenceMetrics");
            Assert.That(type, Is.Not.Null, "Runtime evidence needs one measurable UI metrics owner.");
            var create = type.GetMethod("CreateSnapshot", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(create, Is.Not.Null);
            var snapshot = create.Invoke(null, new object[] {
                1600, 720, 2091, 941, new Rect(0, 0, 2091, 941),
                new Rect(496.5f, 168.5f, 1098, 640), "mobile", "touch",
                "scaleMode=ScaleWithScreenSize referenceResolution=1672x941 screenMatchMode=MatchWidthOrHeight match=1",
                "macos-aspect-simulation", 44
            });
            Assert.That(GetMember<string>(snapshot, "evidenceAuthority"), Is.EqualTo("macos-aspect-simulation"));
            Assert.That(GetMember<float>(snapshot, "characterHubShellHeight"), Is.EqualTo(640f).Within(.01f));
            Assert.That(GetMember<float>(snapshot, "characterHubShellScreenHeightRatio"),
                Is.EqualTo(640f / 941f).Within(.001f));
            Assert.That(GetMember<float>(snapshot, "characterHubShellScreenHeightRatio"), Is.LessThan(.70f));
            Assert.That(GetMember<float>(snapshot, "minimumTouchTargetScreenPixels"),
                Is.EqualTo(44f * 720f / 941f).Within(.01f));
        }

        [Test]
        public void UiEvidenceMetricsExposeGameplayHudZonesForOcclusionAudit()
        {
            var uiAssembly = typeof(ThemeTokens).Assembly;
            var profileType = uiAssembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            var fromScreen = profileType.GetMethod("FromScreen", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var profile = fromScreen.Invoke(null, new object[] { "mobile", 1600, 720, 2091, 941 });
            var safe = new Rect(0, 0, 2091, 941);
            var gameplayType = uiAssembly.GetType("LinhGioi.UI.RuntimeGameplayHudLayout");
            var calculate = gameplayType.GetMethod("Calculate", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var gameplay = calculate.Invoke(null, new[] { (object)safe, profile });

            var evidenceType = uiAssembly.GetType("LinhGioi.UI.RuntimeUiEvidenceMetrics");
            var create = evidenceType.GetMethod("CreateGameplaySnapshot", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Assert.That(create, Is.Not.Null,
                "UIF-06 runtime evidence must export the same semantic HUD geometry used by Layout().");
            var snapshot = create.Invoke(null, new object[] {
                1600, 720, 2091, 941, safe, new Rect(496.5f, 168.5f, 1098, 640),
                "mobile", "touch",
                "scaleMode=ScaleWithScreenSize referenceResolution=1672x941 screenMatchMode=MatchWidthOrHeight match=1",
                "macos-aspect-simulation", 44, gameplay
            });

            foreach (var pair in new[] {
                ("gameplayPlayerStatus", "PlayerStatus"), ("gameplayRightInfo", "RightInfo"),
                ("gameplayCombat", "Combat"), ("gameplayContext", "Context"),
                ("gameplaySecondaryNav", "SecondaryNav"), ("gameplayTouchPad", "TouchPad"),
                ("gameplayDialogue", "Dialogue"),
            })
            {
                var actual = GetMember<object>(snapshot, pair.Item1);
                var expected = GetMember<Rect>(gameplay, pair.Item2);
                Assert.That(actual, Is.Not.Null, pair.Item1);
                Assert.That(GetMember<float>(actual, "x"), Is.EqualTo(expected.x).Within(.01f), pair.Item1);
                Assert.That(GetMember<float>(actual, "y"), Is.EqualTo(expected.y).Within(.01f), pair.Item1);
                Assert.That(GetMember<float>(actual, "width"), Is.EqualTo(expected.width).Within(.01f), pair.Item1);
                Assert.That(GetMember<float>(actual, "height"), Is.EqualTo(expected.height).Within(.01f), pair.Item1);
            }
        }

        [Test]
        public void SharedRuntimeStyleSheetOwnsCriticalPrimitiveLayoutRules()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath,
                "Resources/LGOUI/LgoRuntime.uss"));
            Assert.That(source, Does.Contain(".lgo-icon-frame"));
            Assert.That(source, Does.Contain("flex-grow: 0"));
            Assert.That(source, Does.Contain("flex-shrink: 0"));
            Assert.That(source, Does.Contain(".lgo-utility-action"));
            Assert.That(source, Does.Contain("position: relative"));
            Assert.That(source, Does.Contain("white-space: nowrap"));
            Assert.That(source, Does.Contain("-unity-text-align: lower-center"));
            Assert.That(source, Does.Contain(".lgo-entry-control-card"));
            Assert.That(source, Does.Contain("min-height: 420px"),
                "Canonical Entry card min-height belongs to shared USS, not per-screen inline style.");
            Assert.That(source, Does.Contain(".lgo-server-select-panel"));
            Assert.That(source, Does.Contain("min-height: 420px"));
            Assert.That(source, Does.Contain(".lgo-server-select-action"));
            Assert.That(source, Does.Contain("min-height: 58px"),
                "Canonical Server Select action height belongs to shared USS.");
            Assert.That(source, Does.Contain(".lgo-register-panel"));
            Assert.That(source, Does.Contain("min-height: 492px"),
                "Canonical Register panel height belongs to shared USS.");
            Assert.That(source, Does.Contain(".lgo-register-agreement"));
            Assert.That(source, Does.Contain(".lgo-register-agreement {\n    flex-direction: row;"),
                "Register agreement must keep checkbox and copy in one canonical row.");
            Assert.That(source, Does.Contain("min-height: 44px"),
                "Register agreement touch target belongs to shared USS.");
            Assert.That(source, Does.Contain(".lgo-register-primary"));
            Assert.That(source, Does.Contain(".lgo-register-back"));
            Assert.That(source, Does.Contain(".lgo-password-recovery-panel"));
            Assert.That(source, Does.Contain(".lgo-auth-flow-header"),
                "Register/Recovery title geometry must share one auth-flow header primitive.");
            Assert.That(source, Does.Contain(".lgo-auth-flow-subtitle-row"));
            Assert.That(source, Does.Contain(".lgo-password-recovery-rule"));
            Assert.That(source, Does.Contain(".lgo-password-recovery-verify-footer"));
        }

        [Test]
        public void TabletEntryNoticeDoesNotOverlapCenteredAuthPanel()
        {
            const int screenWidth = 1024;
            const int screenHeight = 768;
            const int panelWidth = 1255;
            const int panelHeight = 941;
            var layoutType = typeof(ThemeTokens).Assembly.GetType("LinhGioi.UI.RuntimeUiLayoutProfile");
            var fromScreen = layoutType.GetMethod("FromScreen", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var layout = fromScreen.Invoke(null, new object[] { "tablet", screenWidth, screenHeight, panelWidth, panelHeight });
            var cardWidth = GetMember<float>(layout, "LoginCardWidth");
            var noticeWidth = GetMember<float>(layout, "EntryNoticeWidth");
            var scale = (float)screenHeight / panelHeight;
            var panelLeft = (screenWidth - cardWidth * scale) * .5f;
            var noticeRight = (24f + noticeWidth) * scale;
            Assert.That(noticeRight, Is.LessThanOrEqualTo(panelLeft - 8f),
                "Tablet notification must not sit underneath the centered Entry/Auth panel.");
        }

        [Test]
        public void SafeAreaCanBeAppliedWithoutHorizontalOverflow()
        {
            var root = new SafeAreaRoot();
            root.ApplySafeArea(new Rect(10, 20, 980, 1960), new Vector2(1000, 2000));
            Assert.AreEqual(10f, root.style.paddingLeft.value.value);
            Assert.AreEqual(10f, root.style.paddingRight.value.value);
        }
    }
}
