using System;
using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    internal static class WorldHubSetDressing
    {
        internal const string Marker = "LGO World Hub Set Dressing Helper v1";
        internal const string SceneDepthMarker = "LGO World Scene Depth Layering";
        internal const string ReadabilityMarker = "LGO World Hub Visual Readability Cleanup v1";
        internal const string DepthRichnessMarker = "LGO World Hub Depth Richness v1";
        internal const string VisualWeightMarker = "LGO World Hub Visual Depth And Weight v1";

        internal static void Ensure(
            Func<string, Sprite, Vector3, Vector3, int, SpriteRenderer> createBillboard,
            Vector3 gateKeeperPosition,
            Vector3 trainingStonePosition,
            Vector3 shadowSlimePosition,
            Vector3 readabilityDummyPosition,
            bool mobileViewport,
            bool narrowViewport)
        {
            if (createBillboard == null) return;
            EnsureDepthLighting(gateKeeperPosition, trainingStonePosition, shadowSlimePosition, readabilityDummyPosition, mobileViewport, narrowViewport);

            // LGO World Hub Visual Readability Cleanup v1: edge props scale down on narrow profiles so gameplay actors own the center.
            createBillboard("LGO World Cherry Tree Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreeCherry ?? LgoVisualAssetRegistryV2.TreeCherry, Point(-5.25f, 0.2f, 1.55f, mobileViewport, narrowViewport), Scale(1.02f, 0.88f, 0.70f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Cherry Tree Depth Shadow V3B", Point(-5.25f, 0.018f, 1.55f, mobileViewport, narrowViewport), ShadowScale(1.10f, 0.46f, 0.76f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Pine Tree Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreePine ?? LgoVisualAssetRegistryV2.TreePine, Point(5.25f, 0.2f, 2.35f, mobileViewport, narrowViewport), Scale(0.94f, 0.82f, 0.66f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Pine Tree Depth Shadow V3B", Point(5.25f, 0.018f, 2.35f, mobileViewport, narrowViewport), ShadowScale(1.02f, 0.42f, 0.72f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Cherry Tree Far Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreeCherry ?? LgoVisualAssetRegistryV2.TreeCherry, Point(5.1f, 0.16f, -0.95f, mobileViewport, narrowViewport), Scale(0.54f, 0.46f, 0.34f, mobileViewport, narrowViewport), 0);
            Shadow("LGO World Cherry Tree Far Depth Shadow V3B", Point(5.1f, 0.018f, -0.95f, mobileViewport, narrowViewport), ShadowScale(0.62f, 0.25f, 0.48f, mobileViewport, narrowViewport), -1);
            createBillboard("LGO World Pine Tree Far Runtime Sprite V3B", LgoVisualAssetRegistryV3B.TreePine ?? LgoVisualAssetRegistryV2.TreePine, Point(-5.55f, 0.16f, 3.9f, mobileViewport, narrowViewport), Scale(0.58f, 0.50f, 0.38f, mobileViewport, narrowViewport), 0);
            Shadow("LGO World Pine Tree Far Depth Shadow V3B", Point(-5.55f, 0.018f, 3.9f, mobileViewport, narrowViewport), ShadowScale(0.66f, 0.27f, 0.50f, mobileViewport, narrowViewport), -1);
            createBillboard("LGO World Cherry Tree Back Ridge Sprite V3B", LgoVisualAssetRegistryV3B.TreeCherry ?? LgoVisualAssetRegistryV2.TreeCherry, Point(-6.15f, 0.12f, 5.35f, mobileViewport, narrowViewport), Scale(0.42f, 0.36f, 0.26f, mobileViewport, narrowViewport), -1);
            Shadow("LGO World Cherry Tree Back Ridge Shadow V3B", Point(-6.15f, 0.018f, 5.35f, mobileViewport, narrowViewport), ShadowScale(0.45f, 0.18f, 0.34f, mobileViewport, narrowViewport), -2);
            createBillboard("LGO World Pine Tree Back Ridge Sprite V3B", LgoVisualAssetRegistryV3B.TreePine ?? LgoVisualAssetRegistryV2.TreePine, Point(6.05f, 0.12f, 5.05f, mobileViewport, narrowViewport), Scale(0.44f, 0.38f, 0.28f, mobileViewport, narrowViewport), -1);
            Shadow("LGO World Pine Tree Back Ridge Shadow V3B", Point(6.05f, 0.018f, 5.05f, mobileViewport, narrowViewport), ShadowScale(0.46f, 0.18f, 0.34f, mobileViewport, narrowViewport), -2);
            createBillboard("LGO World Lantern West Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, Point(-4.55f, 0.2f, -2.05f, mobileViewport, narrowViewport), Scale(0.58f, 0.48f, 0.36f, mobileViewport, narrowViewport), 2);
            Shadow("LGO World Lantern West Depth Shadow V3B", Point(-4.55f, 0.018f, -2.05f, mobileViewport, narrowViewport), ShadowScale(0.38f, 0.20f, 0.32f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Lantern East Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, Point(4.45f, 0.2f, -1.95f, mobileViewport, narrowViewport), Scale(0.58f, 0.48f, 0.36f, mobileViewport, narrowViewport), 2);
            Shadow("LGO World Lantern East Depth Shadow V3B", Point(4.45f, 0.018f, -1.95f, mobileViewport, narrowViewport), ShadowScale(0.38f, 0.20f, 0.32f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Lantern North Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, Point(-1.15f, 0.15f, 5.55f, mobileViewport, narrowViewport), Scale(0.36f, 0.32f, 0.24f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Lantern North Depth Shadow V3B", Point(-1.15f, 0.018f, 5.55f, mobileViewport, narrowViewport), ShadowScale(0.28f, 0.15f, 0.24f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Lantern South Runtime Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, Point(1.25f, 0.15f, -5.45f, mobileViewport, narrowViewport), Scale(0.36f, 0.32f, 0.24f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Lantern South Depth Shadow V3B", Point(1.25f, 0.018f, -5.45f, mobileViewport, narrowViewport), ShadowScale(0.28f, 0.15f, 0.24f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Lantern Back West Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, Point(-3.65f, 0.12f, 5.65f, mobileViewport, narrowViewport), Scale(0.30f, 0.26f, 0.20f, mobileViewport, narrowViewport), 0);
            Shadow("LGO World Lantern Back West Shadow V3B", Point(-3.65f, 0.018f, 5.65f, mobileViewport, narrowViewport), ShadowScale(0.22f, 0.12f, 0.18f, mobileViewport, narrowViewport), -1);
            createBillboard("LGO World Lantern Back East Sprite V3B", LgoVisualAssetRegistryV3B.LanternProp ?? LgoVisualAssetRegistryV2.LanternProp, Point(3.70f, 0.12f, 5.45f, mobileViewport, narrowViewport), Scale(0.30f, 0.26f, 0.20f, mobileViewport, narrowViewport), 0);
            Shadow("LGO World Lantern Back East Shadow V3B", Point(3.70f, 0.018f, 5.45f, mobileViewport, narrowViewport), ShadowScale(0.22f, 0.12f, 0.18f, mobileViewport, narrowViewport), -1);
            createBillboard("LGO World Rock Moss Runtime Sprite V3B", LgoVisualAssetRegistryV3B.RockMoss ?? LgoVisualAssetRegistryV2.RockMoss, Point(-2.15f, 0.15f, -1.95f, mobileViewport, narrowViewport), Scale(0.48f, 0.40f, 0.30f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Rock Moss Depth Shadow V3B", Point(-2.15f, 0.018f, -1.95f, mobileViewport, narrowViewport), ShadowScale(0.50f, 0.24f, 0.40f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Rock Moss East Runtime Sprite V3B", LgoVisualAssetRegistryV3B.RockMoss ?? LgoVisualAssetRegistryV2.RockMoss, Point(2.35f, 0.12f, 4.95f, mobileViewport, narrowViewport), Scale(0.30f, 0.26f, 0.20f, mobileViewport, narrowViewport), 0);
            Shadow("LGO World Rock Moss East Depth Shadow V3B", Point(2.35f, 0.018f, 4.95f, mobileViewport, narrowViewport), ShadowScale(0.32f, 0.15f, 0.26f, mobileViewport, narrowViewport), -1);
            createBillboard("LGO World Rock Moss Back West Runtime Sprite V3B", LgoVisualAssetRegistryV3B.RockMoss ?? LgoVisualAssetRegistryV2.RockMoss, Point(-4.25f, 0.10f, 4.75f, mobileViewport, narrowViewport), Scale(0.24f, 0.20f, 0.16f, mobileViewport, narrowViewport), -1);
            Shadow("LGO World Rock Moss Back West Shadow V3B", Point(-4.25f, 0.018f, 4.75f, mobileViewport, narrowViewport), ShadowScale(0.24f, 0.11f, 0.18f, mobileViewport, narrowViewport), -2);
            createBillboard("LGO World Cultivation Banner Runtime Sprite V3B", LgoVisualAssetRegistryV3B.BannerCultivation ?? LgoVisualAssetRegistryV2.BannerCultivation, Point(4.25f, 0.2f, -3.9f, mobileViewport, narrowViewport), Scale(0.46f, 0.38f, 0.28f, mobileViewport, narrowViewport), 2);
            Shadow("LGO World Cultivation Banner Depth Shadow V3B", Point(4.25f, 0.018f, -3.9f, mobileViewport, narrowViewport), ShadowScale(0.34f, 0.16f, 0.28f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Cultivation Banner West Runtime Sprite V3B", LgoVisualAssetRegistryV3B.BannerCultivation ?? LgoVisualAssetRegistryV2.BannerCultivation, Point(-5.3f, 0.16f, -3.55f, mobileViewport, narrowViewport), Scale(0.34f, 0.28f, 0.22f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Cultivation Banner West Depth Shadow V3B", Point(-5.3f, 0.018f, -3.55f, mobileViewport, narrowViewport), ShadowScale(0.28f, 0.13f, 0.22f, mobileViewport, narrowViewport), 0);
            createBillboard("LGO World Bridge Wood Runtime Sprite V3B", LgoVisualAssetRegistryV3B.BridgeWood ?? LgoVisualAssetRegistryV2.BridgeWood, Point(-3.25f, 0.1f, -4.15f, mobileViewport, narrowViewport), Scale(0.72f, 0.58f, 0.44f, mobileViewport, narrowViewport), 1);
            Shadow("LGO World Bridge Wood Depth Shadow V3B", Point(-3.25f, 0.018f, -4.15f, mobileViewport, narrowViewport), ShadowScale(0.82f, 0.24f, 0.58f, mobileViewport, narrowViewport), 0);
        }

        private static void EnsureDepthLighting(Vector3 gateKeeperPosition, Vector3 trainingStonePosition, Vector3 shadowSlimePosition, Vector3 readabilityDummyPosition, bool mobileViewport, bool narrowViewport)
        {
            // LGO World Scene Depth Layering: every major readable prop receives a light procedural ground shadow.
            // LGO World Hub Depth Richness v1: lightweight procedural glows give the hub a readable stage without importing heavy images.
            var platformGlow = WorldProceduralVisuals.GetWorldPlatformGlowSprite();
            // LGO World Hub Visual Depth And Weight v1: keep glows supportive so actor sprites own the first read.
            Glow("LGO World Central Cultivation Stage Glow V3B", platformGlow, new Vector3(0f, 0.065f, 0.18f), GroundGlowScale(2.45f, 2.06f, 1.58f, mobileViewport, narrowViewport), new Color(0.22f, 0.86f, 1f, 0.32f), -2);
            Glow("LGO World Spirit Gate Arrival Glow V3B", platformGlow, new Vector3(0f, 0.068f, -3.95f), GroundGlowScale(1.72f, 1.42f, 1.06f, mobileViewport, narrowViewport), new Color(0.20f, 0.78f, 1f, 0.34f), -1);
            Glow("LGO World Training Stone Focus Glow V3B", platformGlow, trainingStonePosition + Vector3.up * 0.065f, GroundGlowScale(1.32f, 1.08f, 0.84f, mobileViewport, narrowViewport), new Color(0.24f, 0.96f, 0.86f, 0.28f), -1);
            Glow("LGO World Dummy Practice Glow V3B", platformGlow, readabilityDummyPosition + Vector3.up * 0.065f, GroundGlowScale(1.34f, 1.10f, 0.84f, mobileViewport, narrowViewport), new Color(0.94f, 0.66f, 0.24f, 0.22f), -1);
            Glow("LGO World Gatekeeper Dialogue Glow V3B", platformGlow, gateKeeperPosition + Vector3.up * 0.065f, GroundGlowScale(1.26f, 1.04f, 0.80f, mobileViewport, narrowViewport), new Color(0.94f, 0.70f, 0.30f, 0.22f), -1);
            WorldProceduralVisuals.CreatePathGlowSprite("LGO World Path Glow Center To Gate V3B", new Vector3(0f, 0.07f, -2.02f), PathGlowScale(0.20f, 3.35f, 0.14f, 2.36f, mobileViewport, narrowViewport), new Color(0.20f, 0.82f, 1f, 0.23f), -1);
            WorldProceduralVisuals.CreatePathGlowSprite("LGO World Path Glow Center To Stone V3B", new Vector3(0f, 0.07f, 2.12f), PathGlowScale(0.18f, 2.85f, 0.13f, 2.02f, mobileViewport, narrowViewport), new Color(0.24f, 0.96f, 0.86f, 0.18f), -1);
            Rotate(WorldProceduralVisuals.CreatePathGlowSprite("LGO World Path Glow Center To Keeper V3B", new Vector3(-1.18f, 0.07f, 1.48f), PathGlowScale(0.15f, 2.10f, 0.10f, 1.50f, mobileViewport, narrowViewport), new Color(0.94f, 0.70f, 0.30f, 0.17f), -1), -39f);
            Rotate(WorldProceduralVisuals.CreatePathGlowSprite("LGO World Path Glow Center To Dummy V3B", new Vector3(1.42f, 0.07f, 0.48f), PathGlowScale(0.13f, 1.85f, 0.09f, 1.32f, mobileViewport, narrowViewport), new Color(0.96f, 0.72f, 0.24f, 0.14f), -1), 66f);
            Rotate(WorldProceduralVisuals.CreatePathGlowSprite("LGO World Path Glow Warning Edge V3B", new Vector3(2.85f, 0.07f, 1.65f), PathGlowScale(0.11f, 1.90f, 0.08f, 1.30f, mobileViewport, narrowViewport), new Color(0.58f, 0.36f, 0.92f, 0.11f), -2), 74f);
            WorldProceduralVisuals.CreateMistVeilSprite("LGO World Mist Veil North V3B", new Vector3(0f, 0.052f, 5.7f), MistScale(5.45f, 1.86f, 3.48f, 1.22f, mobileViewport, narrowViewport), new Color(0.30f, 0.66f, 0.92f, 0.24f), -3);
            WorldProceduralVisuals.CreateMistVeilSprite("LGO World Mist Veil West V3B", new Vector3(-5.6f, 0.052f, 0.8f), MistScale(3.25f, 1.58f, 2.04f, 0.94f, mobileViewport, narrowViewport), new Color(0.74f, 0.58f, 0.92f, 0.18f), -3);
            WorldProceduralVisuals.CreateMistVeilSprite("LGO World Mist Veil East V3B", new Vector3(5.7f, 0.052f, 0.35f), MistScale(3.50f, 1.62f, 2.16f, 0.96f, mobileViewport, narrowViewport), new Color(0.28f, 0.70f, 0.92f, 0.17f), -3);
            WorldProceduralVisuals.CreateMistVeilSprite("LGO World Mist Veil Practice Field V3B", new Vector3(2.85f, 0.054f, -0.15f), MistScale(2.60f, 1.14f, 1.62f, 0.74f, mobileViewport, narrowViewport), new Color(0.36f, 0.78f, 0.92f, 0.14f), -2);
            WorldProceduralVisuals.CreateMistVeilSprite("LGO World Mist Veil Back Ridge V3B", new Vector3(0f, 0.050f, 4.75f), MistScale(7.20f, 1.50f, 4.40f, 0.86f, mobileViewport, narrowViewport), new Color(0.62f, 0.82f, 1.00f, 0.24f), -4);
            WorldProceduralVisuals.CreateMistVeilSprite("LGO World Mist Veil Gate Approach V3B", new Vector3(0f, 0.053f, -3.35f), MistScale(4.00f, 1.00f, 2.60f, 0.60f, mobileViewport, narrowViewport), new Color(0.24f, 0.88f, 1.00f, 0.16f), -3);
        }

        private static Vector3 Point(float x, float y, float z, bool mobileViewport, bool narrowViewport)
        {
            if (mobileViewport) return new Vector3(x * 1.04f, y, z * 1.04f);
            if (narrowViewport) return new Vector3(x * 1.02f, y, z * 1.02f);
            return new Vector3(x, y, z);
        }

        private static Vector3 Scale(float desktop, float tablet, float mobile, bool mobileViewport, bool narrowViewport)
        {
            var value = mobileViewport ? mobile : narrowViewport ? tablet : desktop;
            return new Vector3(value, value, 1f);
        }

        private static Vector3 ShadowScale(float width, float height, float mobileWidth, bool mobileViewport, bool narrowViewport)
        {
            if (mobileViewport) return new Vector3(mobileWidth, height * 0.72f, 1f);
            if (narrowViewport) return new Vector3(width * 0.84f, height * 0.86f, 1f);
            return new Vector3(width, height, 1f);
        }

        private static Vector3 GroundGlowScale(float desktop, float tablet, float mobile, bool mobileViewport, bool narrowViewport) => Scale(desktop, tablet, mobile, mobileViewport, narrowViewport);

        private static Vector3 MistScale(float desktopWidth, float desktopHeight, float mobileWidth, float mobileHeight, bool mobileViewport, bool narrowViewport)
        {
            if (mobileViewport) return new Vector3(mobileWidth, mobileHeight, 1f);
            if (narrowViewport) return new Vector3(desktopWidth * 0.78f, desktopHeight * 0.78f, 1f);
            return new Vector3(desktopWidth, desktopHeight, 1f);
        }

        private static Vector3 PathGlowScale(float width, float length, float mobileWidth, float mobileLength, bool mobileViewport, bool narrowViewport)
        {
            if (mobileViewport) return new Vector3(mobileWidth, mobileLength, 1f);
            if (narrowViewport) return new Vector3(width * 0.86f, length * 0.82f, 1f);
            return new Vector3(width, length, 1f);
        }

        private static void Shadow(string name, Vector3 position, Vector3 scale, int sortingOrder) =>
            WorldProceduralVisuals.CreateGroundShadowSprite(name, position, scale, sortingOrder);

        private static void Glow(string name, Sprite sprite, Vector3 position, Vector3 scale, Color color, int sortingOrder) =>
            WorldProceduralVisuals.CreateGroundGlowSprite(name, sprite, position, scale, color, sortingOrder);

        private static void Rotate(SpriteRenderer renderer, float zDegrees)
        {
            if (renderer != null) renderer.transform.rotation = Quaternion.Euler(90f, 0f, zDegrees);
        }
    }
}
