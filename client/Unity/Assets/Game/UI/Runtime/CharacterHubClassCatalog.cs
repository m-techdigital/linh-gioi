using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            string description, CharacterHubIconCatalog iconCatalog = CharacterHubIconCatalog.Skill)
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

        public CharacterHubSpiritPetPreview(string name, string level, string artResource, string portraitResource,
            string rarity, string role, string state, IReadOnlyList<string> stats,
            IReadOnlyList<SkillPreview> skills, string synergy)
        {
            Name = name;
            Level = level;
            ArtResource = artResource;
            PortraitResource = portraitResource;
            Rarity = rarity;
            Role = role;
            State = state;
            Stats = Freeze(stats, nameof(stats));
            if (Stats.Count != 5 || Stats.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Spirit Pet detail requires five structured stat rows", nameof(stats));
            Skills = Freeze(skills, nameof(skills));
            Synergy = synergy;
        }

        private static IReadOnlyList<T> Freeze<T>(IReadOnlyList<T> source, string parameterName)
        {
            if (source == null) throw new ArgumentNullException(parameterName);
            return source is ReadOnlyCollection<T> ? source : Array.AsReadOnly(source.ToArray());
        }

        public string Name { get; }
        public string Level { get; }
        public string ArtResource { get; }
        public string PortraitResource { get; }
        public string Rarity { get; }
        public string Role { get; }
        public string State { get; }
        public IReadOnlyList<string> Stats { get; }
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
            string defaultPotentialName,
            IReadOnlyList<CharacterHubSkillPreview> skills,
            IReadOnlyList<int> equippedSkillIndices,
            IReadOnlyList<CharacterHubPotentialPreview> potentials,
            CharacterHubSpiritPetPreview spiritPet)
        {
            Id = id;
            Label = label;
            Identity = identity;
            Recommendation = recommendation;
            DefaultPotentialName = defaultPotentialName;
            Skills = Freeze(skills, nameof(skills));
            EquippedSkillIndices = Freeze(equippedSkillIndices, nameof(equippedSkillIndices));
            Potentials = Freeze(potentials, nameof(potentials));
            if (Skills.Count == 0 || Skills.Any(skill => skill == null || string.IsNullOrWhiteSpace(skill.Id)))
                throw new ArgumentException("Character Hub skills require stable ids", nameof(skills));
            if (Skills.Select(skill => skill.Id).Distinct(StringComparer.Ordinal).Count() != Skills.Count)
                throw new ArgumentException("Character Hub skill ids must be unique inside class " + id, nameof(skills));
            if (EquippedSkillIndices.Distinct().Count() != EquippedSkillIndices.Count
                || EquippedSkillIndices.Any(index => index < 0 || index >= Skills.Count))
                throw new ArgumentException("Equipped skill indices must be unique and belong to class " + id,
                    nameof(equippedSkillIndices));
            if (Potentials.Count == 0
                || Potentials.Any(potential => potential == null || string.IsNullOrWhiteSpace(potential.Name))
                || Potentials.Select(potential => potential.Name).Distinct(StringComparer.Ordinal).Count() != Potentials.Count)
                throw new ArgumentException("Potential names must be non-empty and unique", nameof(potentials));
            if (!Potentials.Any(potential => potential.Name == DefaultPotentialName))
                throw new ArgumentException("Default Potential does not belong to Character Hub class " + id,
                    nameof(defaultPotentialName));
            SpiritPet = spiritPet;
        }

        private static IReadOnlyList<T> Freeze<T>(IReadOnlyList<T> source, string parameterName)
        {
            if (source == null) throw new ArgumentNullException(parameterName);
            return source is ReadOnlyCollection<T> ? source : Array.AsReadOnly(source.ToArray());
        }

        public string Id { get; }
        public string Label { get; }
        public string Identity { get; }
        public string Recommendation { get; }
        public string DefaultPotentialName { get; }
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
            return profile.DefaultPotentialName;
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

        [Serializable] private sealed class SkillRecord
        {
            public string classId, id, name, level, iconId, description;
        }
        [Serializable] private sealed class SkillLibrary
        {
            public int version;
            public SkillRecord[] skills;
        }
        private static readonly Lazy<Dictionary<string, IReadOnlyList<CharacterHubSkillPreview>>> SkillData =
            new Lazy<Dictionary<string, IReadOnlyList<CharacterHubSkillPreview>>>(LoadSkillData);

        private static Dictionary<string, IReadOnlyList<CharacterHubSkillPreview>> LoadSkillData()
        {
            var asset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(
                "LGOMaps/CongDongLamMap01ASkillIcons/skill-library");
            if (asset == null) throw new InvalidOperationException("Missing Character Hub skill library");
            var library = UnityEngine.JsonUtility.FromJson<SkillLibrary>(asset.text);
            if (library == null || library.version != 1 || library.skills == null || library.skills.Length == 0)
                throw new InvalidOperationException("Invalid Character Hub skill library");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var row in library.skills)
                if (row == null || string.IsNullOrWhiteSpace(row.classId) || string.IsNullOrWhiteSpace(row.id)
                    || !row.id.StartsWith(row.classId + "_", StringComparison.Ordinal) || !ids.Add(row.id)
                    || string.IsNullOrWhiteSpace(row.name) || string.IsNullOrWhiteSpace(row.level)
                    || string.IsNullOrWhiteSpace(row.iconId) || string.IsNullOrWhiteSpace(row.description))
                    throw new InvalidOperationException("Invalid or duplicate Character Hub skill record");
            return library.skills.GroupBy(row => row.classId, StringComparer.Ordinal)
                .ToDictionary(group => group.Key,
                    group => (IReadOnlyList<CharacterHubSkillPreview>)Array.AsReadOnly(group.Select(row =>
                        new CharacterHubSkillPreview(row.id, row.name, row.level, row.iconId, row.description)).ToArray()),
                    StringComparer.Ordinal);
        }

        private static IReadOnlyList<CharacterHubSkillPreview> SkillsFor(string classId)
            => SkillData.Value.TryGetValue(classId, out var skills) ? skills
                : throw new InvalidOperationException("No Character Hub skills registered for " + classId);

        private static CharacterHubSpiritPetPreview SpiritPet(string synergy)
            => new CharacterHubSpiritPetPreview(
                "Thanh Vân Hồ", "Lv.20", "LGOMaps/CongDongLamMap01ACharacterHub/spirit-fox-preview",
                "LGOMaps/CongDongLamMap01ACharacterHub/spirit-fox-portrait",
                "Tinh phẩm", "Hỗ trợ", "Đang xuất chiến",
                new[] { "HP  +8720", "Tấn Công  +860", "Phòng Thủ  +430", "Hồi Phục  +28%", "Giảm Sát Thương  +12%" },
                new[]
                {
                    new CharacterHubSpiritPetPreview.SkillPreview(
                        "Thanh Vân Hộ Thể", "Lv.1", "ho_the",
                        "Tạo lá chắn trị liệu cho chủ nhân, hồi 12% HP tối đa, CD 15 giây."),
                    new CharacterHubSpiritPetPreview.SkillPreview(
                        "Cửu Vĩ Linh Phong", "Lv.1", "phong_tram",
                        "Tung linh phong trị liệu đồng đội, hồi 8% HP và tăng miễn thương, CD 20 giây.")
                },
                synergy);

        private static readonly CharacterHubClassProfile[] Items =
        {
            new CharacterHubClassProfile(
                "vo", "Võ", "Áp sát · combo · phá giáp · phản đòn", "Đề xuất Võ · Công / Sinh lực", "Sinh lực",
                SkillsFor("vo"),
                new[] { 0, 1, 3, 6 }, SharedPotentials, SpiritPet("Thanh Vân Hồ hỗ trợ phòng thủ khi Võ áp sát.")),
            new CharacterHubClassProfile(
                "kiem", "Kiếm", "Tốc độ · kiếm thuật · phản kích · cơ động", "Đề xuất Kiếm · Nhanh nhẹn / Công", "Nhanh nhẹn",
                SkillsFor("kiem"), new[] { 0, 1, 5, 8 }, SharedPotentials, SpiritPet("Thanh Vân Hồ giữ nhịp hồi phục giữa các chuỗi kiếm.")),
            new CharacterHubClassProfile(
                "phap", "Pháp", "Tầm xa · nguyên tố · diện rộng · khống chế", "Đề xuất Pháp · Linh lực / Công", "Linh lực",
                SkillsFor("phap"),
                new[] { 0, 1, 4, 7 }, SharedPotentials, SpiritPet("Thanh Vân Hồ bổ trợ kết giới và duy trì linh lực.")),
            new CharacterHubClassProfile(
                "co", "Cơ", "Tầm xa · cơ giới · bố trí · hỏa lực", "Đề xuất Cơ · Công / Nhanh nhẹn", "Công",
                SkillsFor("co"),
                new[] { 0, 1, 2, 7 }, SharedPotentials, SpiritPet("Thanh Vân Hồ bảo hộ vị trí triển khai cơ giới.")),
            new CharacterHubClassProfile(
                "linh", "Linh", "Triệu hồi · hỗ trợ · khống chế · thanh tẩy", "Đề xuất Linh · Linh lực / Sinh lực", "Linh lực",
                SkillsFor("linh"),
                new[] { 0, 1, 2, 6 }, SharedPotentials, SpiritPet("Thanh Vân Hồ cộng hưởng hồi phục và khống chế.")),
        };

        private static readonly IReadOnlyList<CharacterHubClassProfile> ReadOnlyItems = Array.AsReadOnly(Items);

        static CharacterHubClassCatalog()
        {
            if (Items.Select(profile => profile.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count() != Items.Length)
                throw new InvalidOperationException("Character Hub class ids must be unique");
            if (Items.SelectMany(profile => profile.Skills).Select(skill => skill.Id)
                    .Distinct(StringComparer.Ordinal).Count() != Items.Sum(profile => profile.Skills.Count))
                throw new InvalidOperationException("Character Hub skill ids must be unique across classes");
        }

        public static IReadOnlyList<CharacterHubClassProfile> Profiles => ReadOnlyItems;

        public static CharacterHubClassProfile Get(string classId)
        {
            var profile = ReadOnlyItems.FirstOrDefault(item => string.Equals(item.Id, classId, StringComparison.OrdinalIgnoreCase));
            if (profile == null) throw new ArgumentException("Unknown Character Hub class: " + classId, nameof(classId));
            return profile;
        }
    }
}
