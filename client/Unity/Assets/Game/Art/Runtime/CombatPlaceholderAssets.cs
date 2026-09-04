using UnityEngine;

namespace LinhGioi.Art
{
    public static class CombatPlaceholderAssets
    {
        public const string Version = "0.46.0";
        public const string ResourceRoot = "CombatPlaceholders/";

        public static Sprite TargetDummyIdle => LoadSprite("target-dummy-idle-v0450");
        public static Sprite TargetDummySelected => LoadSprite("target-dummy-selected-v0450");
        public static Sprite TargetDummyHit => LoadSprite("target-dummy-hit-v0450");
        public static Sprite TargetDummyRecover => LoadSprite("target-dummy-recover-v0450");
        public static Sprite TargetMarkerSelected => LoadSprite("target-marker-selected-v0450");
        public static Sprite CooldownReady => LoadSprite("cooldown-ring-ready-v0450");
        public static Sprite CooldownActive => LoadSprite("cooldown-ring-cooldown-v0450");
        public static Sprite WindSlashFrame01 => LoadSprite("skill-wind-slash-frame-01-v0450");
        public static Sprite ImpactSpark => LoadSprite("skill-impact-spark-v0450");
        public static Sprite WarningTelegraphCircle => LoadSprite("warning-telegraph-circle-v0450");

        public static Texture2D CombatButtonNormalTexture => LoadTexture("combat-button-normal-v0450");
        public static Texture2D CombatButtonPressedTexture => LoadTexture("combat-button-pressed-v0450");
        public static Texture2D CombatButtonCooldownTexture => LoadTexture("combat-button-cooldown-v0450");
        public static Texture2D CombatPanelTexture => LoadTexture("combat-panel-9slice-v0450");
        public static Texture2D CooldownReadyTexture => LoadTexture("cooldown-ring-ready-v0450");
        public static Texture2D CooldownActiveTexture => LoadTexture("cooldown-ring-cooldown-v0450");

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
