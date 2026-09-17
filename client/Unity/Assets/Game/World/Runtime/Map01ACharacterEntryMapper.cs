using System;
using LinhGioi.Account;

namespace LinhGioi.World
{
    public sealed class Map01ACharacterEntryState
    {
        public Map01ACharacterEntryState(
            string runtimeClassId, float laneX, int facing, bool usedLegacySpawnFallback)
        {
            RuntimeClassId = runtimeClassId;
            LaneX = laneX;
            Facing = facing;
            UsedLegacySpawnFallback = usedLegacySpawnFallback;
        }

        public string RuntimeClassId { get; }
        public float LaneX { get; }
        public int Facing { get; }
        public bool UsedLegacySpawnFallback { get; }
    }

    public static class Map01ACharacterEntryMapper
    {
        public const string MapId = "map-01a-cong-dong-lam";
        public const float SafeStartLaneX = -3.6f;
        public const float MinLaneX = -3.8f;
        public const float MaxLaneX = 44.4f;

        public static Map01ACharacterEntryState Resolve(CharacterResponse character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            var runtimeClassId = ResolveRuntimeClassId(character.runtimeClassId, character.classId);
            var state = character.runtimeState;
            if (state == null || !string.Equals(state.mapId, MapId, StringComparison.Ordinal))
                return new Map01ACharacterEntryState(runtimeClassId, SafeStartLaneX, 1, true);

            if (!IsFinite(state.laneX) || state.laneX < MinLaneX || state.laneX > MaxLaneX)
                throw new ArgumentException("Map01A laneX is outside the runtime contract.", nameof(character));
            if (state.facing != -1 && state.facing != 1)
                throw new ArgumentException("Map01A facing must be -1 or +1.", nameof(character));
            if (state.updatedAtUnixMs <= 0)
                throw new ArgumentException("Map01A runtime state timestamp must be positive.", nameof(character));

            return new Map01ACharacterEntryState(runtimeClassId, state.laneX, state.facing, false);
        }

        private static bool IsFinite(float value)
            => !float.IsNaN(value) && !float.IsInfinity(value);

        private static string ResolveRuntimeClassId(string runtimeClassId, string rawClassId)
        {
            if (!string.IsNullOrWhiteSpace(runtimeClassId))
                return RequireCanonical(runtimeClassId.Trim());
            switch (rawClassId)
            {
                case "class.martial": return "vo";
                case "class.sword": return "kiem";
                default: return RequireCanonical(rawClassId);
            }
        }

        private static string RequireCanonical(string classId)
        {
            switch (classId)
            {
                case "vo":
                case "kiem":
                case "phap":
                case "co":
                case "linh":
                    return classId;
                default:
                    throw new ArgumentException("Unsupported runtime class id: " + classId, nameof(classId));
            }
        }
    }
}
