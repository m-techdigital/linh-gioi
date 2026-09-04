using UnityEngine;

namespace LinhGioi.Art
{
    public static class LgoVisualAssetRegistryV2
    {
        public const string Version = "2";
        public const string Classification = "STRUCTURAL_RUNTIME_PLACEHOLDER_V2";
        public const string QualityClaim = "Temporary integration placeholder only; not final visual quality or production art.";
        public const string ResourceRoot = "LGOArtV2/";

        public static Texture2D LoginBackgroundSpiritGate => LoadTexture("Login/login_background_spirit_gate_1920x1080");
        public static Texture2D LogoLinhGioiOnline => LoadTexture("Login/logo_linh_gioi_online_1024x512");
        public static Texture2D GateKeeperNpcLoginTexture => LoadTexture("Login/gate_keeper_npc_login_512x768");
        public static Sprite GateKeeperNpcLogin => LoadSprite("Login/gate_keeper_npc_login_512x768");

        public static Texture2D ButtonPrimaryNormalTexture => LoadTexture("UI/buttons/button_primary_normal_512x128");
        public static Texture2D ButtonPrimaryPressedTexture => LoadTexture("UI/buttons/button_primary_pressed_512x128");
        public static Texture2D ButtonDisabledTexture => LoadTexture("UI/buttons/button_disabled_512x128");
        public static Texture2D ButtonSecondaryTexture => LoadTexture("UI/buttons/button_secondary_512x128");
        public static Texture2D PanelMainLargeTexture => LoadTexture("UI/panels/panel_main_large_1024x512");
        public static Texture2D ServerSelectorPanelTexture => LoadTexture("UI/panels/server_selector_panel_768x128");
        public static Texture2D QuestHintPanelTexture => LoadTexture("UI/hud/quest_hint_panel_768x220");
        public static Texture2D IconAccountTexture => LoadTexture("UI/icons/icon_account_128x128");
        public static Texture2D IconNoticeTexture => LoadTexture("UI/icons/icon_notice_128x128");
        public static Texture2D IconSettingsTexture => LoadTexture("UI/icons/icon_settings_128x128");
        public static Texture2D IconServerTexture => LoadTexture("UI/icons/icon_server_128x128");
        public static Texture2D ServerOnlineTexture => LoadTexture("UI/status/server_online_64x64");

        public static Sprite DummyIdle => LoadSprite("World/dummy/dummy_idle_512x768");
        public static Sprite DummySelected => LoadSprite("World/dummy/dummy_selected_512x768");
        public static Sprite DummyHit => LoadSprite("World/dummy/dummy_hit_512x768");
        public static Sprite ShadowSlimeAlt => LoadSprite("World/dummy/shadow_slime_alt_512x512");
        public static Sprite PlayerMaleCultivator => LoadSprite("World/characters/player_male_cultivator_512x768");
        public static Sprite PlayerFemaleCultivator => LoadSprite("World/characters/player_female_cultivator_512x768");
        public static Sprite GateKeeperNpc => LoadSprite("World/npc/gate_keeper_npc_512x768");
        public static Sprite SpiritGate => LoadSprite("World/gate/spirit_gate_1024x1024");
        public static Sprite TrainingStone => LoadSprite("World/training-stone/training_stone_512x1024");
        public static Sprite BannerCultivation => LoadSprite("World/props/banner_cultivation_256x512");
        public static Sprite BridgeWood => LoadSprite("World/props/bridge_wood_1024x512");
        public static Sprite LanternProp => LoadSprite("World/props/lantern_prop_256x512");
        public static Sprite RockMoss => LoadSprite("World/props/rock_moss_512x512");
        public static Sprite TreeCherry => LoadSprite("World/props/tree_cherry_512x512");
        public static Sprite TreePine => LoadSprite("World/props/tree_pine_512x512");

        public static Sprite TargetSelectedBlue => LoadSprite("Combat/target-warning/target_selected_blue_256x256");
        public static Sprite WarningTelegraphRed => LoadSprite("Combat/target-warning/warning_telegraph_red_512x512");
        public static Sprite CooldownReady => LoadSprite("Combat/cooldown/cooldown_ready_256x256");
        public static Sprite CooldownFull => LoadSprite("Combat/cooldown/cooldown_full_256x256");
        public static Texture2D CooldownReadyTexture => LoadTexture("Combat/cooldown/cooldown_ready_256x256");
        public static Texture2D CooldownFullTexture => LoadTexture("Combat/cooldown/cooldown_full_256x256");
        public static Sprite WindSlashFrame01 => LoadSprite("VFX/wind-slash/wind_slash_frame_01_512x512");
        public static Sprite ImpactSpark => LoadSprite("VFX/impact/impact_spark_512x512");

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
