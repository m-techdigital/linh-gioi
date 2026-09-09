using System;
using System.Text;

namespace LinhGioi.World
{
    public sealed class TwoDCharacterModuleCatalog
    {
        private TwoDCharacterModuleCatalog(CharacterModuleDefinition[] items)
        {
            Items = items;
            Snapshot = BuildSnapshot(items);
        }

        public CharacterModuleDefinition[] Items { get; }
        public string Snapshot { get; }

        public static TwoDCharacterModuleCatalog CreateDefault()
        {
            return new TwoDCharacterModuleCatalog(new[]
            {
                new CharacterModuleDefinition("hair_front_basic_male", "HairFront", "male_base", "Tóc trước nam cơ bản", "inventory_icon", "try_on_anchor=HairFrontAnchor"),
                new CharacterModuleDefinition("hair_front_basic_female", "HairFront", "female_base", "Tóc trước nữ cơ bản", "inventory_icon", "try_on_anchor=HairFrontAnchor"),
                new CharacterModuleDefinition("hair_back_basic_male", "HairBack", "male_base", "Tóc sau nam cơ bản", "inventory_icon", "try_on_anchor=HairBackAnchor"),
                new CharacterModuleDefinition("hair_back_basic_female", "HairBack", "female_base", "Tóc sau nữ cơ bản", "inventory_icon", "try_on_anchor=HairBackAnchor"),
                new CharacterModuleDefinition("eyes_default", "Eyes", "unisex", "Mắt mặc định có id riêng", "inventory_icon", "try_on_anchor=Head"),
                new CharacterModuleDefinition("top_common_male", "InnerShirt", "male_base", "Áo lót/tân thủ nam xám", "inventory_icon", "fit_profile=base_male_torso"),
                new CharacterModuleDefinition("top_common_female", "InnerShirt", "female_base", "Áo lót/tân thủ nữ xám", "inventory_icon", "fit_profile=base_female_torso"),
                new CharacterModuleDefinition("pants_common_unisex", "PantsOrSkirt", "unisex", "Quần tân thủ xám", "inventory_icon", "fit_profile=base_hips_legs"),
                new CharacterModuleDefinition("boots_common_unisex", "Boots", "unisex", "Giày/vớ tân thủ", "inventory_icon", "fit_profile=base_feet"),
                new CharacterModuleDefinition("top_vo_lv1_male", "OuterShirt", "male_base", "Áo Võ Lv1 nam vàng đen", "inventory_icon", "fit_profile=base_male_torso;class=Vo;level=1"),
                new CharacterModuleDefinition("top_vo_lv1_female", "OuterShirt", "female_base", "Áo Võ Lv1 nữ vàng đen", "inventory_icon", "fit_profile=base_female_torso;class=Vo;level=1"),
                new CharacterModuleDefinition("pants_vo_lv1_unisex", "PantsOrSkirt", "unisex", "Quần Võ Lv1 đen gọn", "inventory_icon", "fit_profile=base_hips_legs;class=Vo;level=1"),
                new CharacterModuleDefinition("waist_vo_lv1_unisex", "Waist", "unisex", "Đai Võ Lv1 đỏ đen", "inventory_icon", "fit_profile=base_hips;class=Vo;level=1"),
                new CharacterModuleDefinition("gloves_vo_lv1_unisex", "Gloves", "unisex", "Băng tay Võ Lv1", "inventory_icon", "fit_profile=base_hands;class=Vo;level=1"),
                new CharacterModuleDefinition("boots_vo_lv1_unisex", "Boots", "unisex", "Giày Võ Lv1", "inventory_icon", "fit_profile=base_feet;class=Vo;level=1"),
                new CharacterModuleDefinition("top_kiem_lv1_male", "OuterShirt", "male_base", "Áo Kiếm Lv1 nam xanh đen gọn", "inventory_icon", "fit_profile=base_male_torso;class=Kiem;level=1;vfx=sword_trail_seed"),
                new CharacterModuleDefinition("top_kiem_lv1_female", "OuterShirt", "female_base", "Áo Kiếm Lv1 nữ xanh đen gọn", "inventory_icon", "fit_profile=base_female_torso;class=Kiem;level=1;vfx=sword_trail_seed"),
                new CharacterModuleDefinition("pants_kiem_lv1_unisex", "PantsOrSkirt", "unisex", "Quần Kiếm Lv1 gọn nhẹ", "inventory_icon", "fit_profile=base_hips_legs;class=Kiem;level=1"),
                new CharacterModuleDefinition("waist_kiem_lv1_unisex", "Waist", "unisex", "Đai Kiếm Lv1 xanh đen", "inventory_icon", "fit_profile=base_hips;class=Kiem;level=1"),
                new CharacterModuleDefinition("gloves_kiem_lv1_unisex", "Gloves", "unisex", "Bọc tay Kiếm Lv1 nhẹ", "inventory_icon", "fit_profile=base_hands;class=Kiem;level=1"),
                new CharacterModuleDefinition("boots_kiem_lv1_unisex", "Boots", "unisex", "Giày Kiếm Lv1 linh hoạt", "inventory_icon", "fit_profile=base_feet;class=Kiem;level=1"),
                new CharacterModuleDefinition("weapon_kiem_lv1_starter", "Weapon", "unisex", "Kiếm gỗ luyện tập Lv1", "inventory_icon", "try_on_anchor=WeaponAnchor;class=Kiem;level=1;vfx=sword_trail_seed"),
                new CharacterModuleDefinition("weapon_training_staff", "Weapon", "unisex", "Vũ khí tập luyện", "inventory_icon", "try_on_anchor=WeaponAnchor"),
                new CharacterModuleDefinition("spirit_pet_seed", "PetSpirit", "unisex", "Linh chủng đi kèm", "inventory_icon", "try_on_anchor=PetAnchor")
            });
        }

        public bool TryFind(string id, out CharacterModuleDefinition item)
        {
            for (var i = 0; i < Items.Length; i++)
            {
                if (string.Equals(Items[i].Id, id, StringComparison.Ordinal))
                {
                    item = Items[i];
                    return true;
                }
            }
            item = default;
            return false;
        }

        private static string BuildSnapshot(CharacterModuleDefinition[] items)
        {
            var builder = new StringBuilder("LayeredEquipment Module Catalog");
            builder.Append("\nPreviewFlow: select_icon -> inspect_item -> try_on -> cancel_or_apply");
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                builder.Append("\n")
                    .Append(item.Id).Append(" | slot=")
                    .Append(item.Slot).Append(" | base=")
                    .Append(item.BaseFilter).Append(" | ")
                    .Append(item.DisplayName).Append(" | ")
                    .Append(item.RuntimeRule);
            }
            return builder.ToString();
        }
    }

    public sealed class TwoDCharacterLoadout
    {
        private readonly string _baseId;
        private string _hairFront;
        private string _hairBack;
        private string _eyes;
        private string _top;
        private string _pants;
        private string _boots;
        private string _waist;
        private string _gloves;
        private string _weapon;
        private string _pet;
        private CharacterModuleDefinition? _preview;

        private TwoDCharacterLoadout(string baseId)
        {
            _baseId = baseId;
        }

        public string Snapshot => BuildSnapshot();

        public static TwoDCharacterLoadout CreateStarter(string baseId, TwoDCharacterModuleCatalog modules)
        {
            var loadout = new TwoDCharacterLoadout(baseId);
            var isFemale = string.Equals(baseId, "female_base", StringComparison.Ordinal);
            loadout._hairFront = isFemale ? "hair_front_basic_female" : "hair_front_basic_male";
            loadout._hairBack = isFemale ? "hair_back_basic_female" : "hair_back_basic_male";
            loadout._eyes = "eyes_default";
            loadout._top = isFemale ? "top_common_female" : "top_common_male";
            loadout._pants = "pants_common_unisex";
            loadout._boots = "boots_common_unisex";
            loadout._waist = "none";
            loadout._gloves = "none";
            loadout._weapon = "weapon_training_staff";
            loadout._pet = "spirit_pet_seed";
            return loadout;
        }

        public bool TryPreview(string itemId, TwoDCharacterModuleCatalog modules)
        {
            if (!modules.TryFind(itemId, out var item)) return false;
            if (!IsCompatible(item)) return false;
            _preview = item;
            return true;
        }

        public void CancelPreview()
        {
            _preview = null;
        }

        public void ApplyPreview()
        {
            if (!_preview.HasValue) return;
            Equip(_preview.Value);
            _preview = null;
        }

        public bool TryEquip(string itemId, TwoDCharacterModuleCatalog modules)
        {
            if (!modules.TryFind(itemId, out var item)) return false;
            if (!IsCompatible(item)) return false;
            Equip(item);
            return true;
        }

        public void ApplyVoLv1Starter(TwoDCharacterModuleCatalog modules)
        {
            TryEquip(string.Equals(_baseId, "female_base", StringComparison.Ordinal) ? "top_vo_lv1_female" : "top_vo_lv1_male", modules);
            TryEquip("pants_vo_lv1_unisex", modules);
            TryEquip("waist_vo_lv1_unisex", modules);
            TryEquip("gloves_vo_lv1_unisex", modules);
            TryEquip("boots_vo_lv1_unisex", modules);
        }

        private bool IsCompatible(CharacterModuleDefinition item)
        {
            return string.Equals(item.BaseFilter, "unisex", StringComparison.Ordinal)
                || string.Equals(item.BaseFilter, _baseId, StringComparison.Ordinal);
        }

        private void Equip(CharacterModuleDefinition item)
        {
            switch (item.Slot)
            {
                case "HairFront": _hairFront = item.Id; break;
                case "HairBack": _hairBack = item.Id; break;
                case "Eyes": _eyes = item.Id; break;
                case "InnerShirt":
                case "OuterShirt": _top = item.Id; break;
                case "PantsOrSkirt": _pants = item.Id; break;
                case "Waist": _waist = item.Id; break;
                case "Gloves": _gloves = item.Id; break;
                case "Boots": _boots = item.Id; break;
                case "Weapon": _weapon = item.Id; break;
                case "PetSpirit": _pet = item.Id; break;
            }
        }

        private string BuildSnapshot()
        {
            var builder = new StringBuilder("LayeredEquipment loadout base=").Append(_baseId)
                .Append(" | hairFront=").Append(_hairFront)
                .Append(" | hairBack=").Append(_hairBack)
                .Append(" | eyes=").Append(_eyes)
                .Append(" | top=").Append(_top)
                .Append(" | pants=").Append(_pants)
                .Append(" | waist=").Append(_waist)
                .Append(" | gloves=").Append(_gloves)
                .Append(" | boots=").Append(_boots)
                .Append(" | weapon=").Append(_weapon)
                .Append(" | pet=").Append(_pet);
            if (_preview.HasValue)
                builder.Append(" | status=TRYING_ON | preview=").Append(_preview.Value.Id).Append("@slot=").Append(_preview.Value.Slot);
            else
                builder.Append(" | status=EQUIPPED");
            return builder.ToString();
        }
    }

    [Serializable]
    public readonly struct CharacterModuleDefinition
    {
        public CharacterModuleDefinition(string id, string slot, string baseFilter, string displayName, string inventoryPresentation, string runtimeRule)
        {
            Id = id;
            Slot = slot;
            BaseFilter = baseFilter;
            DisplayName = displayName;
            InventoryPresentation = inventoryPresentation;
            RuntimeRule = runtimeRule;
        }

        public string Id { get; }
        public string Slot { get; }
        public string BaseFilter { get; }
        public string DisplayName { get; }
        public string InventoryPresentation { get; }
        public string RuntimeRule { get; }
    }
}
