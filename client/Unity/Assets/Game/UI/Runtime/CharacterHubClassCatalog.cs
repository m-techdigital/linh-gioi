using System;
using System.Collections.Generic;
using System.Linq;

namespace LinhGioi.UI
{
    public sealed class CharacterHubSkillPreview
    {
        public CharacterHubSkillPreview(string id, string name, string level, string iconId, bool useKiemSkillArt = false)
        {
            Id = id;
            Name = name;
            Level = level;
            IconId = iconId;
            UseKiemSkillArt = useKiemSkillArt;
        }

        public string Id { get; }
        public string Name { get; }
        public string Level { get; }
        public string IconId { get; }
        public bool UseKiemSkillArt { get; }
    }

    public sealed class CharacterHubPotentialPreview
    {
        public CharacterHubPotentialPreview(string name, string value, string iconId)
        {
            Name = name;
            Value = value;
            IconId = iconId;
        }

        public string Name { get; }
        public string Value { get; }
        public string IconId { get; }
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
            string spiritSynergy)
        {
            Id = id;
            Label = label;
            Identity = identity;
            Recommendation = recommendation;
            Skills = skills;
            EquippedSkillIndices = equippedSkillIndices;
            Potentials = potentials;
            SpiritSynergy = spiritSynergy;
        }

        public string Id { get; }
        public string Label { get; }
        public string Identity { get; }
        public string Recommendation { get; }
        public IReadOnlyList<CharacterHubSkillPreview> Skills { get; }
        public IReadOnlyList<int> EquippedSkillIndices { get; }
        public IReadOnlyList<CharacterHubPotentialPreview> Potentials { get; }
        public string SpiritSynergy { get; }
    }

    public static class CharacterHubClassCatalog
    {
        private static readonly string[] SharedHudIcons =
        {
            "attack", "run", "skill", "crest", "jump", "support", "character", "notice", "cinematic"
        };

        private static readonly CharacterHubPotentialPreview[] BasePotentials =
        {
            new CharacterHubPotentialPreview("Công", "120", "attack"),
            new CharacterHubPotentialPreview("Thủ", "118", "defense"),
            new CharacterHubPotentialPreview("Sinh lực", "250", "vitality"),
            new CharacterHubPotentialPreview("Linh lực", "96", "spirit"),
            new CharacterHubPotentialPreview("Nhanh nhẹn", "110", "agility")
        };

        private static CharacterHubSkillPreview[] SharedSkills(string classId, params string[] names)
        {
            var levels = new[] { "Lv.8", "Lv.5", "Lv.4", "Lv.3", "Lv.6", "Lv.2", "Lv.1", "Lv.3", "Lv.1" };
            return names.Select((name, index) => new CharacterHubSkillPreview(
                classId + "_skill_" + (index + 1), name, levels[index], SharedHudIcons[index])).ToArray();
        }

        private static CharacterHubSkillPreview[] KiemSkills()
        {
            var names = new[] { "Thiên Kiếm Quyết", "Lăng Không Bộ", "Kiếm Vũ", "Hộ Thể", "Song Kiếm", "Phong Trảm", "Kiếm Trận", "Ngự Kiếm", "Vạn Kiếm" };
            var icons = new[] { "thien_kiem_quyet", "lang_khong_bo", "kiem_vu", "ho_the", "song_kiem", "phong_tram", "kiem_tran", "ngu_kiem", "van_kiem" };
            var levels = new[] { "Lv.8", "Lv.5", "Lv.4", "Lv.3", "Lv.6", "Lv.2", "Lv.1", "Lv.3", "Lv.1" };
            return names.Select((name, index) => new CharacterHubSkillPreview(
                "kiem_skill_" + (index + 1), name, levels[index], icons[index], true)).ToArray();
        }

        private static readonly CharacterHubClassProfile[] Items =
        {
            new CharacterHubClassProfile(
                "vo", "Võ", "Áp sát · combo · phá giáp · phản đòn", "Đề xuất Võ · Công / Sinh lực",
                SharedSkills("vo", "Liên Kích", "Phá Giáp", "Phản Đòn", "Chấn Kình", "Bộ Pháp", "Hộ Thể", "Đột Kích", "Kình Lực", "Quyền Ý"),
                new[] { 0, 1, 3, 6 }, BasePotentials, "Thanh Vân Hồ hỗ trợ phòng thủ khi Võ áp sát."),
            new CharacterHubClassProfile(
                "kiem", "Kiếm", "Tốc độ · kiếm thuật · phản kích · cơ động", "Đề xuất Kiếm · Nhanh nhẹn / Công",
                KiemSkills(), new[] { 0, 1, 5, 8 }, BasePotentials, "Thanh Vân Hồ giữ nhịp hồi phục giữa các chuỗi kiếm."),
            new CharacterHubClassProfile(
                "phap", "Pháp", "Tầm xa · nguyên tố · diện rộng · khống chế", "Đề xuất Pháp · Linh lực / Công",
                SharedSkills("phap", "Hỏa Thuật", "Băng Thuật", "Lôi Thuật", "Linh Thuật", "Kết Giới", "Trọng Lực", "Nguyên Tố", "Pháp Trận", "Tinh Thần"),
                new[] { 0, 1, 4, 7 }, BasePotentials, "Thanh Vân Hồ bổ trợ kết giới và duy trì linh lực."),
            new CharacterHubClassProfile(
                "co", "Cơ", "Tầm xa · cơ giới · bố trí · hỏa lực", "Đề xuất Cơ · Công / Nhanh nhẹn",
                SharedSkills("co", "Cơ Nỏ", "Pháo Kích", "Tháp Cơ", "Cơ Lôi", "Linh Cơ", "Thiết Vệ", "Truy Kích", "Hỏa Tuyến", "Cơ Trận"),
                new[] { 0, 1, 2, 7 }, BasePotentials, "Thanh Vân Hồ bảo hộ vị trí triển khai cơ giới."),
            new CharacterHubClassProfile(
                "linh", "Linh", "Triệu hồi · hỗ trợ · khống chế · thanh tẩy", "Đề xuất Linh · Linh lực / Sinh lực",
                SharedSkills("linh", "Triệu Linh", "Hồi Phục", "Linh Thuẫn", "Thanh Tẩy", "Linh Phù", "Trói Hồn", "Hộ Mệnh", "Cộng Hưởng", "Linh Giới"),
                new[] { 0, 1, 2, 6 }, BasePotentials, "Thanh Vân Hồ cộng hưởng hồi phục và khống chế."),
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
