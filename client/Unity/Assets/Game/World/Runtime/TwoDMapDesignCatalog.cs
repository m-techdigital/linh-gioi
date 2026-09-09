using System;
using System.Text;

namespace LinhGioi.World
{
    public sealed class TwoDMapDesignCatalog
    {
        private TwoDMapDesignCatalog(
            MapZone[] worldZones,
            MapDistrict[] linhThanhDistricts,
            MapRouteNode[] dongMonRoute,
            MapLayerBudget[] layerBudgets,
            MapLandmark[] dongMonLandmarks)
        {
            WorldZones = worldZones;
            LinhThanhDistricts = linhThanhDistricts;
            DongMonRoute = dongMonRoute;
            LayerBudgets = layerBudgets;
            DongMonLandmarks = dongMonLandmarks;
            WorldSnapshot = BuildWorldSnapshot(worldZones, linhThanhDistricts);
            TutorialRouteSnapshot = BuildRouteSnapshot(dongMonRoute);
            LayerBudgetSnapshot = BuildLayerBudgetSnapshot(layerBudgets);
            LandmarkSnapshot = BuildLandmarkSnapshot(dongMonLandmarks);
        }

        public MapZone[] WorldZones { get; }
        public MapDistrict[] LinhThanhDistricts { get; }
        public MapRouteNode[] DongMonRoute { get; }
        public MapLayerBudget[] LayerBudgets { get; }
        public MapLandmark[] DongMonLandmarks { get; }
        public string WorldSnapshot { get; }
        public string TutorialRouteSnapshot { get; }
        public string LayerBudgetSnapshot { get; }
        public string LandmarkSnapshot { get; }
        public string RuntimeSnapshot => WorldSnapshot + "\nRoute: " + TutorialRouteSnapshot + "\n" + LayerBudgetSnapshot + "\n" + LandmarkSnapshot;

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
                },
                new[]
                {
                    new MapLayerBudget(5, "Sky/Fog", "mây, trời, ánh sáng, sương", "parallax chậm nhất; không che gameplay"),
                    new MapLayerBudget(4, "Far Background", "núi xa, thành phố xa", "tạo world scale của Linh Thành"),
                    new MapLayerBudget(3, "Mid Background", "kiến trúc lớn, rừng, thác", "nhận diện zone và Chapter 1"),
                    new MapLayerBudget(2, "Near Background", "cây, nhà, cầu, tháp", "đặt landmark/route đọc được"),
                    new MapLayerBudget(1, "Gameplay Plane", "terrain, platform, player, NPC, monster", "collision và tương tác rõ"),
                    new MapLayerBudget(0, "Foreground", "cỏ, đèn, lá, hạt linh khí", "che nhẹ, không che HUD/action")
                },
                new[]
                {
                    new MapLandmark("gate-landmark", "Cổng Linh Thành", 2, "spawn/gatekeeper", "cổng thành đọc ngay khi vào map"),
                    new MapLandmark("training-stone-landmark", "Bia Luyện Khí", 1, "training-stone", "mốc học kỹ năng đầu tiên"),
                    new MapLandmark("wood-bridge", "Cầu Gỗ", 2, "jump", "đọc trước bài học jump"),
                    new MapLandmark("spirit-waterfall", "Thác Nước", 3, "dash", "mốc chuyển nhịp dash và chiều sâu"),
                    new MapLandmark("song-linh", "Sóng Linh", 0, "class-skill", "hiệu ứng foreground báo linh lực"),
                    new MapLandmark("outer-forest", "Rừng Ngoại Thành", 3, "shadow-slime", "vùng quái cơ bản ngoài cổng")
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

        private static string BuildLayerBudgetSnapshot(MapLayerBudget[] layers)
        {
            var builder = new StringBuilder("LayerBudget: Chapter 1: Vết Nứt Đông Môn | ");
            for (var i = 0; i < layers.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(layers[i].Index).Append(':').Append(layers[i].Name);
            }
            return builder.ToString();
        }


        private static string BuildLandmarkSnapshot(MapLandmark[] landmarks)
        {
            var builder = new StringBuilder("Landmarks: Chapter 1: Vết Nứt Đông Môn | ");
            for (var i = 0; i < landmarks.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(landmarks[i].Id).Append(':').Append(landmarks[i].Name).Append("@L").Append(landmarks[i].LayerIndex).Append("->").Append(landmarks[i].RouteNodeId);
            }
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


    [Serializable]
    public readonly struct MapLandmark
    {
        public MapLandmark(string id, string name, int layerIndex, string routeNodeId, string purpose)
        {
            Id = id;
            Name = name;
            LayerIndex = layerIndex;
            RouteNodeId = routeNodeId;
            Purpose = purpose;
        }

        public string Id { get; }
        public string Name { get; }
        public int LayerIndex { get; }
        public string RouteNodeId { get; }
        public string Purpose { get; }
    }

    [Serializable]
    public readonly struct MapLayerBudget
    {
        public MapLayerBudget(int index, string name, string content, string rule)
        {
            Index = index;
            Name = name;
            Content = content;
            Rule = rule;
        }

        public int Index { get; }
        public string Name { get; }
        public string Content { get; }
        public string Rule { get; }
    }
}
