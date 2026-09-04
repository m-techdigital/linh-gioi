using UnityEngine;

namespace LinhGioi.Art
{
    public static class LgoFinalLoginAssetRegistry
    {
        public const string Classification = "LGO_FINAL_LOGIN_RUNTIME_ART_CANDIDATE";
        public const string ResourceRoot = "LGOFinalLogin/";

        public static Texture2D LoginBackgroundSpiritGate => Resources.Load<Texture2D>(ResourceRoot + "login_background_spirit_gate_final_1920x1080");
        public static Texture2D LogoLinhGioiOnline => Resources.Load<Texture2D>(ResourceRoot + "logo_linh_gioi_online_final_420");
        public static Texture2D ButtonEnterWorldTexture => Resources.Load<Texture2D>(ResourceRoot + "button_enter_world_final_384");
    }
}
