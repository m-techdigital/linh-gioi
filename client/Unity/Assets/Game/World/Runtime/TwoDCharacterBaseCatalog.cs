using System;
using System.Text;

namespace LinhGioi.World
{
    public sealed class TwoDCharacterBaseCatalog
    {
        private TwoDCharacterBaseCatalog(CharacterBaseDefinition[] bases, string[] requiredAnchors, string[] layerOrder)
        {
            Bases = bases;
            RequiredAnchors = requiredAnchors;
            LayerOrder = layerOrder;
            MinimumAnchorCount = requiredAnchors.Length;
            Snapshot = BuildSnapshot(bases, requiredAnchors, layerOrder);
        }

        public CharacterBaseDefinition[] Bases { get; }
        public string[] RequiredAnchors { get; }
        public string[] LayerOrder { get; }
        public int MinimumAnchorCount { get; }
        public string Snapshot { get; }

        public static TwoDCharacterBaseCatalog CreateDefault()
        {
            var layerOrder = new[]
            {
                "Shadow", "BackFX", "BackAccessory", "HairBack", "Body", "Underwear", "PantsOrSkirt",
                "InnerShirt", "OuterShirt", "Waist", "Shoulder", "Gloves", "Boots", "HairFront",
                "HeadAccessory", "Weapon", "Offhand", "PetSpirit", "FrontFX", "UIAnchor"
            };
            var anchors = new[]
            {
                "Root", "Hips", "Torso", "Chest", "Neck", "Head",
                "UpperArm_L", "UpperArm_R", "Forearm_L", "Forearm_R", "Hand_L", "Hand_R",
                "Thigh_L", "Thigh_R", "Calf_L", "Calf_R", "Foot_L", "Foot_R",
                "HairBackAnchor", "HairFrontAnchor", "WeaponAnchor", "OffhandAnchor", "PetAnchor", "UIAnchor"
            };

            return new TwoDCharacterBaseCatalog(
                new[]
                {
                    new CharacterBaseDefinition(
                        "male_base",
                        "Male Base",
                        "anime-stylized lean heroic silhouette",
                        "basic_hair_male",
                        "grey undershirt, grey shorts, grey socks"),
                    new CharacterBaseDefinition(
                        "female_base",
                        "Female Base",
                        "anime-stylized agile heroic silhouette",
                        "basic_hair_female",
                        "grey sports-bra inner top, grey shorts, grey socks")
                },
                anchors,
                layerOrder);
        }

        private static string BuildSnapshot(CharacterBaseDefinition[] bases, string[] anchors, string[] layers)
        {
            var builder = new StringBuilder("LayeredCharacter Base Catalog");
            builder.Append("\nLayers: ").Append(string.Join(" > ", layers));
            builder.Append("\nAnchors: ").Append(string.Join(", ", anchors));
            for (var i = 0; i < bases.Length; i++)
            {
                var b = bases[i];
                builder.Append("\n")
                    .Append(b.Id).Append(": ")
                    .Append(b.DisplayName).Append(" | ")
                    .Append(b.Silhouette).Append(" | hair=")
                    .Append(b.DefaultHairId).Append(" | default=")
                    .Append(b.DefaultOutfit);
            }
            return builder.ToString();
        }
    }

    [Serializable]
    public readonly struct CharacterBaseDefinition
    {
        public CharacterBaseDefinition(string id, string displayName, string silhouette, string defaultHairId, string defaultOutfit)
        {
            Id = id;
            DisplayName = displayName;
            Silhouette = silhouette;
            DefaultHairId = defaultHairId;
            DefaultOutfit = defaultOutfit;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Silhouette { get; }
        public string DefaultHairId { get; }
        public string DefaultOutfit { get; }
    }
}
