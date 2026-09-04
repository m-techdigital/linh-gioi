using UnityEngine;

namespace LinhGioi.Art
{
    public static class CombatPlaceholderAssets
    {
        public const string Version = "0.46.0";
        public const string ResourceRoot = "CombatPlaceholders/";

        public static Sprite TargetDummyIdle => LgoVisualAssetRegistryV3B.TargetDummyIdle ?? LgoVisualAssetRegistryV2.DummyIdle ?? LoadSprite("target-dummy-idle-v0450");
        public static Sprite TargetDummySelected => LgoVisualAssetRegistryV2.DummySelected ?? LoadSprite("target-dummy-selected-v0450");
        public static Sprite TargetDummyHit => LgoVisualAssetRegistryV2.DummyHit ?? LoadSprite("target-dummy-hit-v0450");
        public static Sprite TargetDummyRecover => LoadSprite("target-dummy-recover-v0450");
        public static Sprite TargetMarkerSelected => LgoVisualAssetRegistryV2.TargetSelectedBlue ?? LoadSprite("target-marker-selected-v0450");
        public static Sprite CooldownReady => LgoVisualAssetRegistryV3B.CooldownReady ?? LgoVisualAssetRegistryV2.CooldownReady ?? LoadSprite("cooldown-ring-ready-v0450");
        public static Sprite CooldownActive => LgoVisualAssetRegistryV3B.CooldownActive ?? LgoVisualAssetRegistryV2.CooldownFull ?? LoadSprite("cooldown-ring-cooldown-v0450");
        public static Sprite WindSlashFrame01 => LgoVisualAssetRegistryV3B.WindSlashFrame01 ?? LgoVisualAssetRegistryV2.WindSlashFrame01 ?? LoadSprite("skill-wind-slash-frame-01-v0450");
        public static Sprite ImpactSpark => LgoVisualAssetRegistryV3B.ImpactSpark ?? LgoVisualAssetRegistryV2.ImpactSpark ?? LoadSprite("skill-impact-spark-v0450");
        public static Sprite WarningTelegraphCircle => LgoVisualAssetRegistryV2.WarningTelegraphRed ?? LoadSprite("warning-telegraph-circle-v0450");

        public static Texture2D CombatButtonNormalTexture => LoadTexture("combat-button-normal-v0450");
        public static Texture2D CombatButtonPressedTexture => LoadTexture("combat-button-pressed-v0450");
        public static Texture2D CombatButtonCooldownTexture => LoadTexture("combat-button-cooldown-v0450");
        public static Texture2D CombatPanelTexture => LoadTexture("combat-panel-9slice-v0450");
        public static Texture2D CooldownReadyTexture => LgoVisualAssetRegistryV3B.CooldownReadyTexture ?? LgoVisualAssetRegistryV2.CooldownReadyTexture ?? LoadTexture("cooldown-ring-ready-v0450");
        public static Texture2D CooldownActiveTexture => LgoVisualAssetRegistryV3B.CooldownActiveTexture ?? LgoVisualAssetRegistryV2.CooldownFullTexture ?? LoadTexture("cooldown-ring-cooldown-v0450");

        private static Sprite LoadSprite(string name)
        {
            return Resources.Load<Sprite>(ResourceRoot + name);
        }

        private static Texture2D LoadTexture(string name)
        {
            return Resources.Load<Texture2D>(ResourceRoot + name);
        }
    }
}
