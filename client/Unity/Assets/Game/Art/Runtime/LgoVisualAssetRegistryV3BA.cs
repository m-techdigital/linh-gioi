using UnityEngine;

namespace LinhGioi.Art
{
    public static class LgoVisualAssetRegistryV3BA
    {
        public const string Classification = "LGO_ART_V3BA_LOGIN_GATE_ENTRY_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL";
        public const string ResourceRoot = "LGOArtV3BA/";

        public static Texture2D LoginBackgroundSpiritGate => LoadTexture("login/login_background_spirit_gate_1920x1080");
        public static Texture2D LogoLinhGioiOnline => LoadTexture("login/logo_linh_gioi_online_2048x1024");
        public static Texture2D GateKeeperNpcLoginTexture => LoadTexture("login/gate_keeper_npc_1024x1536");
        public static Texture2D PanelMainTexture => LoadTexture("ui/panels/panel_main_1536x768");
        public static Texture2D PanelSmallTexture => LoadTexture("ui/panels/panel_small_512x256");
        public static Texture2D DialoguePanelTexture => LoadTexture("ui/panels/panel_dialogue_1024x384");
        public static Texture2D ServerSelectorPanelTexture => LoadTexture("ui/panels/server_selector_panel_1024x256");
        public static Texture2D ButtonEnterWorldNormalTexture => LoadTexture("ui/buttons/button_enter_world_normal_1024x256");
        public static Texture2D ButtonEnterWorldPressedTexture => LoadTexture("ui/buttons/button_enter_world_pressed_1024x256");
        public static Texture2D ButtonDisabledTexture => LoadTexture("ui/buttons/button_disabled_1024x256");
        public static Texture2D IconAccountTexture => LoadTexture("ui/icons/icon_account_512x512");
        public static Texture2D IconExitTexture => LoadTexture("ui/icons/icon_exit_512x512");
        public static Texture2D IconNoticeTexture => LoadTexture("ui/icons/icon_notice_512x512");
        public static Texture2D IconSettingsTexture => LoadTexture("ui/icons/icon_settings_512x512");
        public static Texture2D ServerOnlineTexture => LoadTexture("ui/status/server_online_256x256");
        public static Texture2D ServerBusyTexture => LoadTexture("ui/status/server_busy_256x256");
        public static Texture2D ServerFullTexture => LoadTexture("ui/status/server_full_256x256");
        public static Texture2D ServerMaintenanceTexture => LoadTexture("ui/status/server_maintenance_256x256");

        private static Texture2D LoadTexture(string name)
        {
            return Resources.Load<Texture2D>(ResourceRoot + name);
        }
    }
}
