using System;
using System.Collections.Generic;
using System.Linq;

namespace LinhGioi.UI
{
    public enum CharacterHubIconCatalog
    {
        Hud,
        Skill
    }

    public sealed class CharacterHubSkillPreview
    {
        public CharacterHubSkillPreview(string id, string name, string level, string iconId,
            string description, CharacterHubIconCatalog iconCatalog = CharacterHubIconCatalog.Hud)
        {
            Id = id;
            Name = name;
            Level = level;
            IconId = iconId;
            Description = description;
            IconCatalog = iconCatalog;
        }

        public string Id { get; }
        public string Name { get; }
        public string Level { get; }
        public string IconId { get; }
        public string Description { get; }
        public CharacterHubIconCatalog IconCatalog { get; }
    }

    public sealed class CharacterHubPotentialPreview
    {
        public CharacterHubPotentialPreview(string name, string value, string iconId,
            string summary, string currentEffect, string nextEffect)
        {
            Name = name;
            Value = value;
            IconId = iconId;
            Summary = summary;
            CurrentEffect = currentEffect;
            NextEffect = nextEffect;
        }

        public string Name { get; }
        public string Value { get; }
        public string IconId { get; }
        public string Summary { get; }
        public string CurrentEffect { get; }
        public string NextEffect { get; }
    }

    public sealed class CharacterHubSpiritPetPreview
    {
        public sealed class SkillPreview
        {
            public SkillPreview(string name, string level, string iconId, string description)
            {
                Name = name;
                Level = level;
                IconId = iconId;
                Description = description;
            }

            public string Name { get; }
            public string Level { get; }
            public string IconId { get; }
            public string Description { get; }
        }

        public CharacterHubSpiritPetPreview(string name, string level, string artResource,
            string rarity, string role, string state, string stats,
            IReadOnlyList<SkillPreview> skills, string synergy)
        {
            Name = name;
            Level = level;
            ArtResource = artResource;
            Rarity = rarity;
            Role = role;
            State = state;
            Stats = stats;
            Skills = skills;
            Synergy = synergy;
        }

        public string Name { get; }
        public string Level { get; }
        public string ArtResource { get; }
        public string Rarity { get; }
        public string Role { get; }
        public string State { get; }
        public string Stats { get; }
        public IReadOnlyList<SkillPreview> Skills { get; }
        public string Synergy { get; }
    }

    public sealed class CharacterHubClassProfile
    {
        public CharacterHubClassProfile(
            string id,
            string label,
            string identity,
            string recommendation,
            IReadOnlyList<CharacterHubSkillPreview> skills,
            IReadOnlyList<int> equippedSkillIndices,
            IReadOnlyList<CharacterHubPotentialPreview> potentials,
            CharacterHubSpiritPetPreview spiritPet)
        {
            Id = id;
            Label = label;
            Identity = identity;
            Recommendation = recommendation;
            Skills = skills;
            EquippedSkillIndices = equippedSkillIndices;
            Potentials = potentials;
            SpiritPet = spiritPet;
        }

        public string Id { get; }
        public string Label { get; }
        public string Identity { get; }
        public string Recommendation { get; }
        public IReadOnlyList<CharacterHubSkillPreview> Skills { get; }
        public IReadOnlyList<int> EquippedSkillIndices { get; }
        public IReadOnlyList<CharacterHubPotentialPreview> Potentials { get; }
        public CharacterHubSpiritPetPreview SpiritPet { get; }
    }

    public sealed class CharacterHubSelectionState
    {
        private readonly Dictionary<string, string> _skillByClass =
            new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> _potentialByClass =
            new Dictionary<string, string>(StringComparer.Ordinal);

        public string SkillIdFor(CharacterHubClassProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (_skillByClass.TryGetValue(profile.Id, out var skillId)
                && profile.Skills.Any(skill => skill.Id == skillId)) return skillId;
            return profile.Skills[0].Id;
        }

        public CharacterHubSkillPreview SkillFor(CharacterHubClassProfile profile)
            => profile.Skills.First(skill => skill.Id == SkillIdFor(profile));

        public string PotentialNameFor(CharacterHubClassProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (_potentialByClass.TryGetValue(profile.Id, out var potentialName)
                && profile.Potentials.Any(potential => potential.Name == potentialName)) return potentialName;
            return profile.Potentials[Math.Min(2, profile.Potentials.Count - 1)].Name;
        }

        public CharacterHubPotentialPreview PotentialFor(CharacterHubClassProfile profile)
            => profile.Potentials.First(potential => potential.Name == PotentialNameFor(profile));

        public void SelectSkill(CharacterHubClassProfile profile, string skillId)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (!profile.Skills.Any(skill => skill.Id == skillId))
                throw new ArgumentException("Skill does not belong to Character Hub class " + profile.Id, nameof(skillId));
            _skillByClass[profile.Id] = skillId;
        }

        public void SelectPotential(CharacterHubClassProfile profile, string potentialName)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (!profile.Potentials.Any(potential => potential.Name == potentialName))
                throw new ArgumentException("Potential does not belong to Character Hub class " + profile.Id, nameof(potentialName));
            _potentialByClass[profile.Id] = potentialName;
        }
    }

    public static class CharacterHubClassCatalog
    {
        private static readonly string[] SharedHudIcons =
        {
            "attack", "run", "skill", "crest", "jump", "support", "character", "notice", "cinematic"
        };

        private static readonly IReadOnlyList<CharacterHubPotentialPreview> SharedPotentials = Array.AsReadOnly(
            new[]
            {
                new CharacterHubPotentialPreview("Công", "120", "attack",
                    "Tăng sức tấn công và hiệu quả gây sát thương.", "Công  +120", "Công  +2"),
                new CharacterHubPotentialPreview("Thủ", "118", "defense",
                    "Tăng khả năng phòng thủ và giảm sát thương phải chịu.", "Thủ  +118", "Thủ  +2"),
                new CharacterHubPotentialPreview("Sinh lực", "250", "vitality",
                    "Tăng cường thể chất, sinh lực và khả năng phòng thủ.",
                    "Sinh lực (HP)  +12.500\nPhòng thủ  +250",
                    "Sinh lực (HP)  +50\nPhòng thủ  +1"),
                new CharacterHubPotentialPreview("Linh lực", "96", "spirit",
                    "Tăng linh lực và khả năng duy trì kỹ năng.", "Linh lực  +96", "Linh lực  +2"),
                new CharacterHubPotentialPreview("Nhanh nhẹn", "110", "agility",
                    "Tăng tốc độ hành động và khả năng né tránh.", "Nhanh nhẹn  +110", "Nhanh nhẹn  +2")
            });

        private static CharacterHubSkillPreview[] SharedSkills(string classId, string identity, params string[] names)
        {
            var levels = new[] { "Lv.8", "Lv.5", "Lv.4", "Lv.3", "Lv.6", "Lv.2", "Lv.1", "Lv.3", "Lv.1" };
            return names.Select((name, index) => new CharacterHubSkillPreview(
                classId + "_skill_" + (index + 1), name, levels[index], SharedHudIcons[index],
                identity + "\n\nCấp hiện hành  " + levels[index]
                    + "\n\nThông tin hiệu ứng chi tiết sẽ hiển thị khi kỹ năng được lĩnh hội đầy đủ.")).ToArray();
        }

        private static CharacterHubSkillPreview[] KiemSkills()
        {
            var names = new[] { "Thiên Kiếm Quyết", "Lăng Không Bộ", "Kiếm Vũ", "Hộ Thể", "Song Kiếm", "Phong Trảm", "Kiếm Trận", "Ngự Kiếm", "Vạn Kiếm" };
            var icons = new[] { "thien_kiem_quyet", "lang_khong_bo", "kiem_vu", "ho_the", "song_kiem", "phong_tram", "kiem_tran", "ngu_kiem", "van_kiem" };
            var levels = new[] { "Lv.8", "Lv.5", "Lv.4", "Lv.3", "Lv.6", "Lv.2", "Lv.1", "Lv.3", "Lv.1" };
            return names.Select((name, index) => new CharacterHubSkillPreview(
                "kiem_skill_" + (index + 1), name, levels[index], icons[index],
                index == 0
                    ? "Vận kiếm khí thiên đạo, chém mục tiêu phía trước.\n\nSát thương  320% Công\nPhạm vi  Hình quạt trước mặt\nHồi chiêu  12 giây\nTiêu hao MP  180"
                    : "Tốc độ · kiếm thuật · phản kích · cơ động\n\nCấp hiện hành  " + levels[index]
                        + "\n\nThông tin hiệu ứng chi tiết sẽ hiển thị khi kỹ năng được lĩnh hội đầy đủ.",
                CharacterHubIconCatalog.Skill)).ToArray();
        }

        private static CharacterHubSpiritPetPreview SpiritPet(string synergy)
            => new CharacterHubSpiritPetPreview(
                "Thanh Vân Hồ", "Lv.20", "LGOMaps/CongDongLamMap01ACharacterHub/spirit-fox-preview",
                "Tinh phẩm", "Hỗ trợ", "Đang xuất chiến",
                "Thuộc tính Linh thú\nHP  +8720\nTấn Công  +860\nPhòng Thủ  +430\nHồi Phục  +28%\nGiảm Sát Thương  +12%",
                new[]
                {
                    new CharacterHubSpiritPetPreview.SkillPreview(
                        "Thanh Vân Hộ Thể", "Lv.1", "ho_the", "Tạo lá chắn trị liệu cho chủ nhân."),
                    new CharacterHubSpiritPetPreview.SkillPreview(
                        "Cửu Vĩ Linh Phong", "Lv.1", "phong_tram", "Tung linh phong hỗ trợ đồng đội.")
                },
                synergy);

        private static readonly CharacterHubClassProfile[] Items =
        {
            new CharacterHubClassProfile(
                "vo", "Võ", "Áp sát · combo · phá giáp · phản đòn", "Đề xuất Võ · Công / Sinh lực",
                SharedSkills("vo", "Áp sát · combo · phá giáp · phản đòn", "Liên Kích", "Phá Giáp", "Phản Đòn", "Chấn Kình", "Bộ Pháp", "Hộ Thể", "Đột Kích", "Kình Lực", "Quyền Ý"),
                new[] { 0, 1, 3, 6 }, SharedPotentials, SpiritPet("Thanh Vân Hồ hỗ trợ phòng thủ khi Võ áp sát.")),
            new CharacterHubClassProfile(
                "kiem", "Kiếm", "Tốc độ · kiếm thuật · phản kích · cơ động", "Đề xuất Kiếm · Nhanh nhẹn / Công",
                KiemSkills(), new[] { 0, 1, 5, 8 }, SharedPotentials, SpiritPet("Thanh Vân Hồ giữ nhịp hồi phục giữa các chuỗi kiếm.")),
            new CharacterHubClassProfile(
                "phap", "Pháp", "Tầm xa · nguyên tố · diện rộng · khống chế", "Đề xuất Pháp · Linh lực / Công",
                SharedSkills("phap", "Tầm xa · nguyên tố · diện rộng · khống chế", "Hỏa Thuật", "Băng Thuật", "Lôi Thuật", "Linh Thuật", "Kết Giới", "Trọng Lực", "Nguyên Tố", "Pháp Trận", "Tinh Thần"),
                new[] { 0, 1, 4, 7 }, SharedPotentials, SpiritPet("Thanh Vân Hồ bổ trợ kết giới và duy trì linh lực.")),
            new CharacterHubClassProfile(
                "co", "Cơ", "Tầm xa · cơ giới · bố trí · hỏa lực", "Đề xuất Cơ · Công / Nhanh nhẹn",
                SharedSkills("co", "Tầm xa · cơ giới · bố trí · hỏa lực", "Cơ Nỏ", "Pháo Kích", "Tháp Cơ", "Cơ Lôi", "Linh Cơ", "Thiết Vệ", "Truy Kích", "Hỏa Tuyến", "Cơ Trận"),
                new[] { 0, 1, 2, 7 }, SharedPotentials, SpiritPet("Thanh Vân Hồ bảo hộ vị trí triển khai cơ giới.")),
            new CharacterHubClassProfile(
                "linh", "Linh", "Triệu hồi · hỗ trợ · khống chế · thanh tẩy", "Đề xuất Linh · Linh lực / Sinh lực",
                SharedSkills("linh", "Triệu hồi · hỗ trợ · khống chế · thanh tẩy", "Triệu Linh", "Hồi Phục", "Linh Thuẫn", "Thanh Tẩy", "Linh Phù", "Trói Hồn", "Hộ Mệnh", "Cộng Hưởng", "Linh Giới"),
                new[] { 0, 1, 2, 6 }, SharedPotentials, SpiritPet("Thanh Vân Hồ cộng hưởng hồi phục và khống chế.")),
        };

        public static IReadOnlyList<CharacterHubClassProfile> Profiles => Items;

        public static CharacterHubClassProfile Get(string classId)
        {
            var profile = Items.FirstOrDefault(item => string.Equals(item.Id, classId, StringComparison.OrdinalIgnoreCase));
            if (profile == null) throw new ArgumentException("Unknown Character Hub class: " + classId, nameof(classId));
            return profile;
        }
    }
}
