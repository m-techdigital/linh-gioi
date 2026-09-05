using UnityEngine;

namespace LinhGioi.Art
{
    public static class LgoVisualAssetRegistryV3B
    {
        public const string Classification = "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL";
        public const string ResourceRoot = "LGOArtV3B/";

        public static Texture2D LoginBackgroundSpiritGate => LoadTexture("Login/login_background_spirit_gate_1920x1080_v3b_candidate");
        public static Texture2D LogoLinhGioiOnline => LoadTexture("Login/logo_linh_gioi_online_v3b_light_runtime_candidate");
        public static Texture2D PanelMainDarkGoldTexture => LoadTexture("Login/panel_main_dark_gold_v3b_candidate");
        public static Texture2D ButtonEnterWorldGoldTexture => LoadTexture("Login/button_enter_world_gold_v3b_candidate");
        public static Texture2D GateKeeperNpcLoginTexture => LoadTexture("Login/gate_keeper_npc_login_v3b_candidate");
        public static Sprite GateKeeperNpc => LoadSprite("Login/gate_keeper_npc_login_v3b_candidate");
        public static Texture2D PlayerMaleCultivatorTexture => LoadTexture("World/characters/player_male_cultivator_idle_v3b_candidate");
        public static Sprite PlayerMaleCultivator => LoadSprite("World/characters/player_male_cultivator_idle_v3b_candidate");
        public static Sprite SpiritGate => LoadSprite("World/gate/spirit_gate_v3b_candidate");
        public static Sprite TrainingStone => LoadSprite("World/training-stone/training_stone_v3b_candidate");
        public static Sprite TreeCherry => LoadSprite("World/props/tree_cherry_v3b_candidate");
        public static Sprite TreePine => LoadSprite("World/props/tree_pine_v3b_candidate");
        public static Sprite LanternProp => LoadSprite("World/props/lantern_prop_v3b_candidate");
        public static Sprite RockMoss => LoadSprite("World/props/rock_moss_v3b_candidate");
        public static Sprite BannerCultivation => LoadSprite("World/props/banner_cultivation_v3b_candidate");
        public static Sprite BridgeWood => LoadSprite("World/props/bridge_wood_v3b_candidate");
        public static Sprite ShadowSlime => LoadSprite("World/creatures/shadow_slime_v3b_candidate");
        public static Sprite WindSlashFrame01 => LoadSprite("VFX/wind-slash/wind_slash_frame_01_v3b_candidate");
        public static Sprite ImpactSpark => LoadSprite("VFX/impact/impact_spark_v3b_candidate");
        public static Sprite CooldownReady => LoadSprite("Combat/cooldown/cooldown_ready_v3b_candidate");
        public static Sprite CooldownActive => LoadSprite("Combat/cooldown/cooldown_active_v3b_candidate");
        public static Sprite TargetDummyIdle => LoadSprite("Combat/target-dummy/target_dummy_idle_v3b_candidate");
        public static Sprite TargetDummySelected => LoadSprite("Combat/target-dummy/target_dummy_selected_v3b_candidate");
        public static Sprite TargetDummyHit => LoadSprite("Combat/target-dummy/target_dummy_hit_v3b_candidate");
        public static Sprite TargetDummyRecover => LoadSprite("Combat/target-dummy/target_dummy_recover_v3b_candidate");
        public static Texture2D CooldownReadyTexture => LoadTexture("Combat/cooldown/cooldown_ready_v3b_candidate");
        public static Texture2D CooldownActiveTexture => LoadTexture("Combat/cooldown/cooldown_active_v3b_candidate");

        private static Texture2D LoadTexture(string name)
        {
            return Resources.Load<Texture2D>(ResourceRoot + name);
        }

        private static Sprite LoadSprite(string name)
        {
            return Resources.Load<Sprite>(ResourceRoot + name);
        }
    }
}
