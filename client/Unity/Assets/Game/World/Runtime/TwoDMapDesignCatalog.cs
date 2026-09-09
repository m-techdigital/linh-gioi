using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class TwoDMapDesignCatalog
    {
        private TwoDMapDesignCatalog(
            MapZone[] worldZones,
            MapDistrict[] linhThanhDistricts,
            MapRouteNode[] dongMonRoute,
            MapLayerBudget[] layerBudgets,
            MapZoneConnection[] zoneConnections,
            MapLandmark[] dongMonLandmarks,
            MapCollisionBand[] dongMonCollisionBands,
            MapTileDefinition[] dongMonTileDefinitions,
            MapTileChunkDefinition[] dongMonTileChunks,
            MapTilePaletteEntry[] dongMonTilePalette)
        {
            WorldZones = worldZones;
            LinhThanhDistricts = linhThanhDistricts;
            DongMonRoute = dongMonRoute;
            LayerBudgets = layerBudgets;
            ZoneConnections = zoneConnections;
            DongMonLandmarks = dongMonLandmarks;
            DongMonCollisionBands = dongMonCollisionBands;
            DongMonTileDefinitions = dongMonTileDefinitions;
            DongMonTileChunks = dongMonTileChunks;
            DongMonTilePalette = dongMonTilePalette;
            WorldSnapshot = BuildWorldSnapshot(worldZones, linhThanhDistricts);
            TutorialRouteSnapshot = BuildRouteSnapshot(dongMonRoute);
            LayerBudgetSnapshot = BuildLayerBudgetSnapshot(layerBudgets);
            ZoneNetworkSnapshot = BuildZoneNetworkSnapshot(worldZones, linhThanhDistricts, zoneConnections);
            LinhThanhHubShellSnapshot = BuildLinhThanhHubShellSnapshot(linhThanhDistricts);
            LinhThanhPlazaShellSnapshot = BuildLinhThanhPlazaShellSnapshot();
            LinhThanhAcademyShellSnapshot = BuildLinhThanhAcademyShellSnapshot();
            LinhThanhMarketShellSnapshot = BuildLinhThanhMarketShellSnapshot();
            LinhThanhSpiritTempleShellSnapshot = BuildLinhThanhSpiritTempleShellSnapshot();
            LinhThanhResidentialShellSnapshot = BuildLinhThanhResidentialShellSnapshot();
            LinhThanhForgeShellSnapshot = BuildLinhThanhForgeShellSnapshot();
            LinhThanhGuildShellSnapshot = BuildLinhThanhGuildShellSnapshot();
            LinhThanhHarborShellSnapshot = BuildLinhThanhHarborShellSnapshot();
            LinhThanhPlazaHubRuntimeSnapshot = BuildLinhThanhPlazaHubRuntimeSnapshot();
            LandmarkSnapshot = BuildLandmarkSnapshot(dongMonLandmarks);
            CollisionSnapshot = BuildCollisionSnapshot(dongMonCollisionBands);
            TilemapSnapshot = BuildTilemapSnapshot(dongMonTileDefinitions, dongMonTileChunks);
            DongMonTilePaletteSnapshot = BuildDongMonTilePaletteSnapshot(dongMonTilePalette);
            DongMonAuthoredPassSnapshot = BuildDongMonAuthoredPassSnapshot(dongMonCollisionBands, dongMonTileChunks);
            ParallaxDepthSnapshot = BuildParallaxDepthSnapshot(layerBudgets);
        }

        public MapZone[] WorldZones { get; }
        public MapDistrict[] LinhThanhDistricts { get; }
        public MapRouteNode[] DongMonRoute { get; }
        public MapLayerBudget[] LayerBudgets { get; }
        public MapZoneConnection[] ZoneConnections { get; }
        public MapLandmark[] DongMonLandmarks { get; }
        public MapCollisionBand[] DongMonCollisionBands { get; }
        public MapTileDefinition[] DongMonTileDefinitions { get; }
        public MapTileChunkDefinition[] DongMonTileChunks { get; }
        public MapTilePaletteEntry[] DongMonTilePalette { get; }
        public string WorldSnapshot { get; }
        public string TutorialRouteSnapshot { get; }
        public string LayerBudgetSnapshot { get; }
        public string ZoneNetworkSnapshot { get; }
        public string LinhThanhHubShellSnapshot { get; }
        public string LinhThanhPlazaShellSnapshot { get; }
        public string LinhThanhAcademyShellSnapshot { get; }
        public string LinhThanhMarketShellSnapshot { get; }
        public string LinhThanhSpiritTempleShellSnapshot { get; }
        public string LinhThanhResidentialShellSnapshot { get; }
        public string LinhThanhForgeShellSnapshot { get; }
        public string LinhThanhGuildShellSnapshot { get; }
        public string LinhThanhHarborShellSnapshot { get; }
        public string LinhThanhPlazaHubRuntimeSnapshot { get; }
        public string LandmarkSnapshot { get; }
        public string CollisionSnapshot { get; }
        public string TilemapSnapshot { get; }
        public string DongMonTilePaletteSnapshot { get; }
        public string DongMonAuthoredPassSnapshot { get; }
        public string ParallaxDepthSnapshot { get; }
        public string RuntimeSnapshot => WorldSnapshot + "\n" + ZoneNetworkSnapshot + "\n" + LinhThanhHubShellSnapshot + "\n" + LinhThanhPlazaShellSnapshot + "\n" + LinhThanhAcademyShellSnapshot + "\n" + LinhThanhMarketShellSnapshot + "\n" + LinhThanhSpiritTempleShellSnapshot + "\n" + LinhThanhResidentialShellSnapshot + "\n" + LinhThanhForgeShellSnapshot + "\n" + LinhThanhGuildShellSnapshot + "\n" + LinhThanhHarborShellSnapshot + "\n" + LinhThanhPlazaHubRuntimeSnapshot + "\nRoute: " + TutorialRouteSnapshot + "\n" + LayerBudgetSnapshot + "\n" + LandmarkSnapshot + "\n" + CollisionSnapshot + "\n" + TilemapSnapshot + "\n" + DongMonTilePaletteSnapshot + "\n" + DongMonAuthoredPassSnapshot + "\n" + ParallaxDepthSnapshot;

        private const string DongMonTilePaletteResourcePath = "LGOMaps/DongMonTilePalette";
        private const string DongMonChunkPlacementResourcePath = "LGOMaps/DongMonChunkPlacement";
        private const string DongMonAuthoredDetailsResourcePath = "LGOMaps/DongMonAuthoredDetails";

        public static string LoadDongMonTilePaletteSourceSnapshot()
        {
            var asset = Resources.Load<TextAsset>(DongMonTilePaletteResourcePath);
            if (asset == null)
            {
                return "DongMonTilePaletteSource: resource=" + DongMonTilePaletteResourcePath + " | missing";
            }

            var text = asset.text ?? string.Empty;
            return "DongMonTilePaletteSource: resource=" + DongMonTilePaletteResourcePath
                + " | bytes=" + text.Length
                + " | tile_ground_grass=" + ContainsToken(text, "tile_ground_grass")
                + " | tile_dash_lane=" + ContainsToken(text, "tile_dash_lane")
                + " | tile_slime_arena=" + ContainsToken(text, "tile_slime_arena")
                + " | safe-no-source-image=" + ContainsToken(text, "safe-no-source-image")
                + " | safe-runtime-resource=" + ContainsToken(text, "safe-runtime-resource");
        }

        public static MapTileChunkDefinition[] LoadDongMonAuthoredChunkPlacements()
        {
            var source = LoadDongMonChunkPlacementSource();
            if (source == null || source.chunks == null || source.chunks.Length == 0)
            {
                return CreateDefault().DongMonTileChunks;
            }

            var chunks = new MapTileChunkDefinition[source.chunks.Length];
            for (var i = 0; i < source.chunks.Length; i++)
            {
                var chunk = source.chunks[i];
                chunks[i] = new MapTileChunkDefinition(
                    chunk.id ?? string.Empty,
                    chunk.name ?? string.Empty,
                    chunk.routeNodeId ?? string.Empty,
                    chunk.primaryTileId ?? string.Empty,
                    chunk.originX,
                    chunk.originY,
                    chunk.tileCount,
                    chunk.gameplayRead ?? string.Empty);
            }
            return chunks;
        }

        public static string LoadDongMonChunkPlacementSourceSnapshot()
        {
            var asset = Resources.Load<TextAsset>(DongMonChunkPlacementResourcePath);
            if (asset == null)
            {
                return "DongMonChunkPlacementSource: resource=" + DongMonChunkPlacementResourcePath + " | missing";
            }

            var text = asset.text ?? string.Empty;
            var chunks = LoadDongMonAuthoredChunkPlacements();
            var builder = new StringBuilder("DongMonChunkPlacementSource: resource=");
            builder.Append(DongMonChunkPlacementResourcePath);
            builder.Append(" | bytes=").Append(text.Length);
            for (var i = 0; i < chunks.Length; i++)
            {
                builder.Append(" | ").Append(chunks[i].Id).Append('@')
                    .Append(FormatMapNumber(chunks[i].OriginX)).Append(',')
                    .Append(FormatMapNumber(chunks[i].OriginY)).Append('x')
                    .Append(chunks[i].TileCount);
            }
            builder.Append(" | authored-placement=").Append(ContainsToken(text, "authored-placement"));
            builder.Append(" | safe-runtime-resource=").Append(ContainsToken(text, "safe-runtime-resource"));
            builder.Append(" | safe-no-3d=").Append(ContainsToken(text, "safe-no-3d"));
            return builder.ToString();
        }

        public static DongMonAuthoredDetailEntry[] LoadDongMonAuthoredDetails()
        {
            var source = LoadDongMonAuthoredDetailsSource();
            if (source == null || source.details == null || source.details.Length == 0)
            {
                return Array.Empty<DongMonAuthoredDetailEntry>();
            }

            return source.details;
        }

        public static string LoadDongMonAuthoredDetailsSourceSnapshot()
        {
            var asset = Resources.Load<TextAsset>(DongMonAuthoredDetailsResourcePath);
            if (asset == null)
            {
                return "DongMonAuthoredDetailSource: resource=" + DongMonAuthoredDetailsResourcePath + " | missing";
            }

            var text = asset.text ?? string.Empty;
            var details = LoadDongMonAuthoredDetails();
            var builder = new StringBuilder("DongMonAuthoredDetailSource: resource=");
            builder.Append(DongMonAuthoredDetailsResourcePath);
            builder.Append(" | bytes=").Append(text.Length);
            builder.Append(" | details=").Append(details.Length);
            builder.Append(" | moss=").Append(ContainsToken(text, "moss"));
            builder.Append(" | step=").Append(ContainsToken(text, "step"));
            builder.Append(" | rope=").Append(ContainsToken(text, "rope"));
            builder.Append(" | spirit-dust=").Append(ContainsToken(text, "spirit-dust"));
            builder.Append(" | rune=").Append(ContainsToken(text, "rune"));
            builder.Append(" | authored-detail=").Append(ContainsToken(text, "authored-detail"));
            builder.Append(" | safe-runtime-resource=").Append(ContainsToken(text, "safe-runtime-resource"));
            builder.Append(" | safe-no-source-image=").Append(ContainsToken(text, "safe-no-source-image"));
            builder.Append(" | safe-no-3d=").Append(ContainsToken(text, "safe-no-3d"));
            return builder.ToString();
        }

        private static DongMonChunkPlacementSource LoadDongMonChunkPlacementSource()
        {
            var asset = Resources.Load<TextAsset>(DongMonChunkPlacementResourcePath);
            if (asset == null || string.IsNullOrEmpty(asset.text)) return null;
            return JsonUtility.FromJson<DongMonChunkPlacementSource>(asset.text);
        }

        private static DongMonAuthoredDetailsSource LoadDongMonAuthoredDetailsSource()
        {
            var asset = Resources.Load<TextAsset>(DongMonAuthoredDetailsResourcePath);
            if (asset == null || string.IsNullOrEmpty(asset.text)) return null;
            return JsonUtility.FromJson<DongMonAuthoredDetailsSource>(asset.text);
        }

        private static bool ContainsToken(string text, string token)
        {
            return text.IndexOf(token, StringComparison.Ordinal) >= 0;
        }

        private static string FormatMapNumber(float value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

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
                    new MapZoneConnection("linh-thanh->east", "linh-thanh", "east", "Đông Môn mở Đông Vực Lv.1-30 sau tutorial"),
                    new MapZoneConnection("linh-thanh->city", "linh-thanh", "city", "Đô Thị Lv.1-40 cho xã hội/công nghệ sau onboarding"),
                    new MapZoneConnection("linh-thanh->spirit-mountain", "linh-thanh", "spirit-mountain", "Linh Sơn Lv.20-50 cho tu luyện/môn phái"),
                    new MapZoneConnection("linh-thanh->west", "linh-thanh", "west", "Tây Vực Lv.30-60 cho phụ bản trung cấp"),
                    new MapZoneConnection("linh-thanh->underworld", "linh-thanh", "underworld", "Âm Giới Lv.60-100 cho invasion/world boss dài hạn")
                },
                new[]
                {
                    new MapLandmark("gate-landmark", "Cổng Linh Thành", 2, "spawn/gatekeeper", "cổng thành đọc ngay khi vào map"),
                    new MapLandmark("training-stone-landmark", "Bia Luyện Khí", 1, "training-stone", "mốc học kỹ năng đầu tiên"),
                    new MapLandmark("wood-bridge", "Cầu Gỗ", 2, "jump", "đọc trước bài học jump"),
                    new MapLandmark("spirit-waterfall", "Thác Nước", 3, "dash", "mốc chuyển nhịp dash và chiều sâu"),
                    new MapLandmark("song-linh", "Sóng Linh", 0, "class-skill", "hiệu ứng foreground báo linh lực"),
                    new MapLandmark("outer-forest", "Rừng Ngoại Thành", 3, "shadow-slime", "vùng quái cơ bản ngoài cổng")
                },
                new[]
                {
                    new MapCollisionBand("ground-main", "Main Ground", -2.15f, 4.25f, -2.15f, "walk/run safe lane"),
                    new MapCollisionBand("training-platform", "Bia Platform", 2.18f, 3.28f, -1.16f, "interaction platform around Bia Luyện Khí"),
                    new MapCollisionBand("jump-gap", "Jump Gap", 0.35f, 1.32f, -1.58f, "gap cue for jump lesson"),
                    new MapCollisionBand("dash-lane", "Dash Lane", 1.55f, 3.35f, -1.02f, "horizontal dash read lane"),
                    new MapCollisionBand("slime-arena", "Shadow Slime Arena", 3.12f, 4.10f, -1.16f, "combat stop before forest edge")
                },
                new[]
                {
                    new MapTileDefinition("tile_ground_grass", "Grass Ground", 1, "ground-main", "walkable top edge and foreground grass fringe"),
                    new MapTileDefinition("tile_ground_stone", "Spirit Stone Path", 1, "ground-main", "main Dong Mon path surface"),
                    new MapTileDefinition("tile_platform_wood", "Wood Platform", 1, "training-platform", "bridge/platform around jump lesson"),
                    new MapTileDefinition("tile_gap_marker", "Jump Gap Marker", 1, "jump-gap", "empty tile span reserved for jump tutorial"),
                    new MapTileDefinition("tile_dash_lane", "Dash Lane Tile", 1, "dash-lane", "flat read lane for dash tutorial"),
                    new MapTileDefinition("tile_slime_arena", "Slime Arena Soil", 1, "slime-arena", "combat footing before outer forest")
                },
                new[]
                {
                    new MapTileChunkDefinition("chunk_gate_entry", "Gate Entry", "spawn", "tile_ground_grass", -3.70f, -2.02f, 4, "safe grass approach before NPC focus"),
                    new MapTileChunkDefinition("chunk_training_stone", "Training Stone Path", "training-stone", "tile_ground_stone", -1.54f, -2.02f, 3, "stone path toward Bia Luyện Khí"),
                    new MapTileChunkDefinition("chunk_jump_bridge", "Jump Bridge", "jump", "tile_platform_wood", 0.35f, -0.88f, 3, "wood platform and gap read for jump lesson"),
                    new MapTileChunkDefinition("chunk_dash_lane", "Dash Lane", "dash", "tile_dash_lane", 1.82f, -1.02f, 4, "flat run-up lane for dash lesson"),
                    new MapTileChunkDefinition("chunk_slime_arena", "Slime Arena", "shadow-slime", "tile_slime_arena", 3.25f, -1.36f, 2, "combat footing before outer forest")
                },
                new[]
                {
                    new MapTilePaletteEntry("tile_ground_grass", "earth-green", "soft-grass-edge", "walkable-ground"),
                    new MapTilePaletteEntry("tile_ground_stone", "spirit-cyan", "stone-step-line", "walkable-ground"),
                    new MapTilePaletteEntry("tile_platform_wood", "warm-wood", "rope-rail", "jump-platform"),
                    new MapTilePaletteEntry("tile_gap_marker", "void-shadow", "negative-space", "jump-gap"),
                    new MapTilePaletteEntry("tile_dash_lane", "spirit-cyan", "wind-streak", "dash-lane"),
                    new MapTilePaletteEntry("tile_slime_arena", "violet-corruption", "rune-boundary", "combat-arena")
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


        private static string BuildZoneNetworkSnapshot(MapZone[] zones, MapDistrict[] districts, MapZoneConnection[] connections)
        {
            var builder = new StringBuilder("WorldMapNetwork: hub=linh-thanh");
            for (var i = 0; i < connections.Length; i++)
            {
                builder.Append(" | ").Append(connections[i].Id).Append(':').Append(connections[i].Purpose);
            }
            builder.Append("\nLinhThanhHubRuntime: ");
            for (var i = 0; i < districts.Length && i < 5; i++)
            {
                if (i > 0) builder.Append(" -> ");
                builder.Append(districts[i].Id).Append(':').Append(districts[i].Name);
            }
            return builder.ToString();
        }


        private static string BuildLinhThanhHubShellSnapshot(MapDistrict[] districts)
        {
            // Snapshot contract: district=east-gate | district=plaza | district=academy | district=market.
            var builder = new StringBuilder("HubShell: linh-thanh");
            for (var i = 0; i < districts.Length; i++)
            {
                var district = districts[i];
                if (district.Id != "east-gate" && district.Id != "plaza" && district.Id != "academy" && district.Id != "market") continue;
                builder.Append(" | district=").Append(district.Id).Append(':').Append(district.Name).Append(':').Append(district.Role);
            }
            return builder.ToString();
        }


        private static string BuildLinhThanhPlazaShellSnapshot()
        {
            return "PlazaShell: district=plaza | social-spawn=local-safe | event-board=preview-only | guild-bulletin-preview=locked | safe-no-trade-backend";
        }


        private static string BuildLinhThanhAcademyShellSnapshot()
        {
            return "AcademyShell: district=academy | skill-hall=preview-only | class-trainer=locked | lecture-board=local-preview | safe-no-skill-backend | safe-local-no-backend";
        }


        private static string BuildLinhThanhMarketShellSnapshot()
        {
            return "MarketShell: district=market | vendor-row=preview-only | auction-board=locked | item-stall=local-preview | safe-no-trade-backend | safe-no-economy-backend | safe-local-no-backend";
        }


        private static string BuildLinhThanhSpiritTempleShellSnapshot()
        {
            return "SpiritTempleShell: district=spirit-temple | blessing-altar=preview-only | story-shrine=locked | incense-vfx=local-preview | safe-no-buff-backend | safe-no-story-backend | safe-local-no-backend";
        }


        private static string BuildLinhThanhResidentialShellSnapshot()
        {
            return "ResidentialShell: district=residential | npc-home-row=preview-only | social-chat-node=locked | ambient-citizen=local-preview | safe-no-housing-backend | safe-no-social-backend | safe-local-no-backend";
        }


        private static string BuildLinhThanhForgeShellSnapshot()
        {
            return "ForgeShell: district=forge | anvil-row=preview-only | craft-board=locked | forge-glow=local-preview | safe-no-crafting-backend | safe-no-upgrade-economy | safe-local-no-backend";
        }


        private static string BuildLinhThanhGuildShellSnapshot()
        {
            return "GuildShell: district=guild | guild-hall=preview-only | guild-banner=local-preview | notice-board=locked | safe-no-guild-backend | safe-no-membership-backend | safe-local-no-backend";
        }


        private static string BuildLinhThanhHarborShellSnapshot()
        {
            return "HarborShell: district=harbor | spirit-boat=preview-only | travel-board=locked | dock-lantern=local-preview | safe-no-travel-backend | safe-no-teleport-backend | safe-local-no-backend";
        }


        private static string BuildLinhThanhPlazaHubRuntimeSnapshot()
        {
            return "PlazaHubRuntime: district=plaza | npc=gate-guide | npc=wandering-student | npc=merchant-preview | board=event-local-preview | guild-bulletin=locked | social-spawn=local-safe | safe-local-no-backend | safe-local-no-shop-backend";
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

        private static string BuildCollisionSnapshot(MapCollisionBand[] bands)
        {
            var builder = new StringBuilder("Collision: Chapter 1: Vết Nứt Đông Môn | ");
            for (var i = 0; i < bands.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(bands[i].Id).Append(':').Append(bands[i].Name).Append('@').Append(bands[i].Y.ToString("0.00"));
            }
            return builder.ToString();
        }

        private static string BuildParallaxDepthSnapshot(MapLayerBudget[] layers)
        {
            return "ParallaxDepth: Layer5=Sky/Fog:cloud-drift | Layer4=Far Background:mountain-silhouette | Layer0=Foreground:grass-leaf-motes";
        }

        private static string BuildTilemapSnapshot(MapTileDefinition[] tiles, MapTileChunkDefinition[] chunks)
        {
            var builder = new StringBuilder("Chapter 1 Tilemap: Đông Môn | ");
            for (var i = 0; i < tiles.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(tiles[i].Id).Append('@').Append(tiles[i].CollisionBandId).Append("/L").Append(tiles[i].LayerIndex);
            }
            builder.Append("\nChunkFlow: ");
            for (var i = 0; i < chunks.Length; i++)
            {
                if (i > 0) builder.Append(" -> ");
                builder.Append(chunks[i].Id).Append('@').Append(chunks[i].RouteNodeId).Append('x').Append(chunks[i].TileCount);
            }
            return builder.ToString();
        }

        private static string BuildDongMonTilePaletteSnapshot(MapTilePaletteEntry[] palette)
        {
            var builder = new StringBuilder("DongMonTilePalette: ");
            for (var i = 0; i < palette.Length; i++)
            {
                if (i > 0) builder.Append(" | ");
                builder.Append(palette[i].TileId).Append(':').Append(palette[i].ColorRole).Append(':').Append(palette[i].PatternRole);
            }
            builder.Append(" | safe-no-source-image | safe-runtime-palette-contract");
            return builder.ToString();
        }

        private static string BuildDongMonAuthoredPassSnapshot(MapCollisionBand[] bands, MapTileChunkDefinition[] chunks)
        {
            var builder = new StringBuilder("DongMonAuthoredPass: route-segments=");
            builder.Append(chunks.Length);
            builder.Append(" | collision-bands=").Append(bands.Length);
            builder.Append(" | detail-density=readable | collision-boundaries=from-bands | foreground-fringe=controlled | landmark-silhouettes=anchored | no-random-decoration | safe-no-procedural-spam");
            return builder.ToString();
        }
    }

    [Serializable]
    public sealed class DongMonChunkPlacementSource
    {
        public string id;
        public string mapId;
        public string chapter;
        public string usage;
        public string[] safety;
        public DongMonChunkPlacementEntry[] chunks;
    }

    [Serializable]
    public sealed class DongMonChunkPlacementEntry
    {
        public string id;
        public string name;
        public string routeNodeId;
        public string primaryTileId;
        public float originX;
        public float originY;
        public int tileCount;
        public string gameplayRead;
    }

    [Serializable]
    public sealed class DongMonAuthoredDetailsSource
    {
        public string id;
        public string mapId;
        public string chapter;
        public string usage;
        public string[] safety;
        public DongMonAuthoredDetailEntry[] details;
    }

    [Serializable]
    public sealed class DongMonAuthoredDetailEntry
    {
        public string id;
        public string name;
        public string kind;
        public string routeNodeId;
        public float x;
        public float y;
        public float w;
        public float h;
        public float r;
        public float g;
        public float b;
        public float a;
        public int sortOrder;
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
    public readonly struct MapZoneConnection
    {
        public MapZoneConnection(string id, string fromZoneId, string toZoneId, string purpose)
        {
            Id = id;
            FromZoneId = fromZoneId;
            ToZoneId = toZoneId;
            Purpose = purpose;
        }

        public string Id { get; }
        public string FromZoneId { get; }
        public string ToZoneId { get; }
        public string Purpose { get; }
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
    public readonly struct MapCollisionBand
    {
        public MapCollisionBand(string id, string name, float minX, float maxX, float y, string rule)
        {
            Id = id;
            Name = name;
            MinX = minX;
            MaxX = maxX;
            Y = y;
            Rule = rule;
        }

        public string Id { get; }
        public string Name { get; }
        public float MinX { get; }
        public float MaxX { get; }
        public float Y { get; }
        public string Rule { get; }
    }

    [Serializable]
    public readonly struct MapTileDefinition
    {
        public MapTileDefinition(string id, string name, int layerIndex, string collisionBandId, string usage)
        {
            Id = id;
            Name = name;
            LayerIndex = layerIndex;
            CollisionBandId = collisionBandId;
            Usage = usage;
        }

        public string Id { get; }
        public string Name { get; }
        public int LayerIndex { get; }
        public string CollisionBandId { get; }
        public string Usage { get; }
    }


    [Serializable]
    public readonly struct MapTileChunkDefinition
    {
        public MapTileChunkDefinition(string id, string name, string routeNodeId, string primaryTileId, float originX, float originY, int tileCount, string usage)
        {
            Id = id;
            Name = name;
            RouteNodeId = routeNodeId;
            PrimaryTileId = primaryTileId;
            OriginX = originX;
            OriginY = originY;
            TileCount = tileCount;
            Usage = usage;
        }

        public string Id { get; }
        public string Name { get; }
        public string RouteNodeId { get; }
        public string PrimaryTileId { get; }
        public float OriginX { get; }
        public float OriginY { get; }
        public int TileCount { get; }
        public string Usage { get; }
    }

    [Serializable]
    public readonly struct MapTilePaletteEntry
    {
        public MapTilePaletteEntry(string tileId, string colorRole, string patternRole, string gameplayRole)
        {
            TileId = tileId;
            ColorRole = colorRole;
            PatternRole = patternRole;
            GameplayRole = gameplayRole;
        }

        public string TileId { get; }
        public string ColorRole { get; }
        public string PatternRole { get; }
        public string GameplayRole { get; }
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
