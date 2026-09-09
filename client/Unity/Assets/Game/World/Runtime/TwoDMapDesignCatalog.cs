using System;
using System.Text;

namespace LinhGioi.World
{
    public sealed class TwoDMapDesignCatalog
    {
        private TwoDMapDesignCatalog(
            MapZone[] worldZones,
            MapDistrict[] linhThanhDistricts,
            MapRouteNode[] dongMonRoute)
        {
            WorldZones = worldZones;
            LinhThanhDistricts = linhThanhDistricts;
            DongMonRoute = dongMonRoute;
            WorldSnapshot = BuildWorldSnapshot(worldZones, linhThanhDistricts);
            TutorialRouteSnapshot = BuildRouteSnapshot(dongMonRoute);
        }

        public MapZone[] WorldZones { get; }
        public MapDistrict[] LinhThanhDistricts { get; }
        public MapRouteNode[] DongMonRoute { get; }
        public string WorldSnapshot { get; }
        public string TutorialRouteSnapshot { get; }
        public string RuntimeSnapshot => WorldSnapshot + "\nRoute: " + TutorialRouteSnapshot;

        public static TwoDMapDesignCatalog CreateDefault()
        {
            return new TwoDMapDesignCatalog(
                new[]
                {
                    new MapZone("city", "Đô Thị", "Lv.1-40", "xã hội, công nghệ, tutorial hiện đại"),
                    new MapZone("east", "Đông Vực", "Lv.1-30", "khu tân thủ, quái cơ bản, tutorial"),
                    new MapZone("linh-thanh", "Linh Thành", "Lv.1-100", "hub trung tâm, giao dịch, học viện, bang hội"),
                    new MapZone("west", "Tây Vực", "Lv.30-60", "phụ bản trung cấp, tài nguyên"),
                    new MapZone("sea", "Hải Vực", "Lv.40-70", "khám phá, tàu thuyền, tài nguyên biển"),
                    new MapZone("relic", "Cổ Di Tích", "Lv.30-60", "dungeon khảo cổ, bí mật thế giới"),
                    new MapZone("spirit-mountain", "Linh Sơn", "Lv.20-50", "tu luyện, môn phái, thiên nhiên"),
                    new MapZone("law", "Pháp Vực", "Lv.50-80", "nghiên cứu, pháp thuật, phụ bản"),
                    new MapZone("heaven", "Thiên Vực", "Lv.70-100", "endgame thần thoại"),
                    new MapZone("upper", "Thượng Giới", "Lv.80-100", "raid và event cao cấp"),
                    new MapZone("underworld", "Âm Giới", "Lv.60-100", "world boss, xâm lăng, social action")
                },
                new[]
                {
                    new MapDistrict("east-gate", "Đông Môn", "cửa thành/tutorial"),
                    new MapDistrict("plaza", "Quảng Trường", "social spawn/sự kiện"),
                    new MapDistrict("academy", "Học Viện", "kỹ năng/lớp học"),
                    new MapDistrict("spirit-temple", "Đền Linh", "tín ngưỡng/buff/story"),
                    new MapDistrict("residential", "Khu Dân Cư", "nhà ở/hội thoại NPC"),
                    new MapDistrict("forge", "Khu Rèn", "chế tạo"),
                    new MapDistrict("market", "Thương Phố", "giao dịch"),
                    new MapDistrict("guild", "Khu Bang Hội", "cộng đồng"),
                    new MapDistrict("north-gate", "Bắc Môn", "cổng khu vực"),
                    new MapDistrict("harbor", "Cảng Linh Thuyền", "travel/event")
                },
                new[]
                {
                    new MapRouteNode("spawn", "Đông Môn", "safe", "người chơi xuất hiện"),
                    new MapRouteNode("gatekeeper", "Người Giữ Cổng", "npc", "nhận hướng dẫn"),
                    new MapRouteNode("training-stone", "Bia Luyện Khí", "main-quest", "cộng hưởng linh lực"),
                    new MapRouteNode("movement", "Học di chuyển", "tutorial", "walk/run trên player plane"),
                    new MapRouteNode("jump", "Học Jump", "tutorial", "vượt platform"),
                    new MapRouteNode("dash", "Học Dash", "tutorial", "né/chạy nhanh"),
                    new MapRouteNode("class-skill", "Dùng Skill Class", "skill", "đọc identity class"),
                    new MapRouteNode("shadow-slime", "Shadow Slime", "monster", "combat cơ bản"),
                    new MapRouteNode("return-gate", "Quay về Người Giữ Cổng", "main-quest", "mở Linh Thành")
                });
        }

        private static string BuildWorldSnapshot(MapZone[] zones, MapDistrict[] districts)
        {
            var builder = new StringBuilder("World: ");
            for (var i = 0; i < zones.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(zones[i].Name).Append(' ').Append(zones[i].LevelBand);
            }
            builder.Append("\nLinh Thành: ");
            for (var i = 0; i < districts.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(districts[i].Name);
            }
            return builder.ToString();
        }

        private static string BuildRouteSnapshot(MapRouteNode[] nodes)
        {
            var builder = new StringBuilder("Đông Môn");
            for (var i = 1; i < nodes.Length; i++)
                builder.Append(" -> ").Append(nodes[i].Name);
            builder.Append(" -> Mini Boss Linh Thú Biến Dị -> Linh Thành");
            return builder.ToString();
        }
    }

    [Serializable]
    public readonly struct MapZone
    {
        public MapZone(string id, string name, string levelBand, string role)
        {
            Id = id;
            Name = name;
            LevelBand = levelBand;
            Role = role;
        }

        public string Id { get; }
        public string Name { get; }
        public string LevelBand { get; }
        public string Role { get; }
    }

    [Serializable]
    public readonly struct MapDistrict
    {
        public MapDistrict(string id, string name, string role)
        {
            Id = id;
            Name = name;
            Role = role;
        }

        public string Id { get; }
        public string Name { get; }
        public string Role { get; }
    }

    [Serializable]
    public readonly struct MapRouteNode
    {
        public MapRouteNode(string id, string name, string marker, string purpose)
        {
            Id = id;
            Name = name;
            Marker = marker;
            Purpose = purpose;
        }

        public string Id { get; }
        public string Name { get; }
        public string Marker { get; }
        public string Purpose { get; }
    }
}
