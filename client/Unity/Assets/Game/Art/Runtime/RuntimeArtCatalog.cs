using System;
using UnityEngine;

namespace LinhGioi.Art
{
    public static class RuntimeArtCatalog
    {
        public const string Version = "0.10.0";
        public const string HeroPlaceholder = "Assets/Game/Art/Characters/lgo_character_hero_sword_placeholder.svg";
        public const string NpcPlaceholder = "Assets/Game/Art/NPCs/lgo_npc_keeper_placeholder.svg";
        public const string MonsterPlaceholder = "Assets/Game/Art/Monsters/lgo_monster_shadow_slime_placeholder.svg";
        public const string TrainingGroundTile = "Assets/Game/Art/Maps/lgo_map_training_ground_tile.svg";
        public const string SpiritBurstVfx = "Assets/Game/Art/VFX/lgo_vfx_spirit_burst_marker.svg";

        public static readonly Color Background = Hex("#0B1324");
        public static readonly Color Surface = Hex("#111D32");
        public static readonly Color SurfaceRaised = Hex("#182741");
        public static readonly Color Spirit = Hex("#28D7C7");
        public static readonly Color Shadow = Hex("#9B5CFF");
        public static readonly Color Gold = Hex("#E6B85C");
        public static readonly Color Danger = Hex("#E35D6A");
        public static readonly Color Text = Hex("#F5F2EA");
        public static readonly Color Muted = Hex("#9BA7BC");

        public static Material CreateMaterial(string name, Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = shader == null ? new Material(Shader.Find("Sprites/Default")) : new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }

        private static Color Hex(string value)
        {
            if (!ColorUtility.TryParseHtmlString(value, out var color))
                throw new InvalidOperationException("Invalid runtime art color: " + value);
            return color;
        }
    }
}
