using System;
using LinhGioi.Account;
using LinhGioi.World;
using NUnit.Framework;

namespace LinhGioi.Tests.EditMode
{
    public sealed class Map01ACharacterEntryMapperTests
    {
        [TestCase("vo")]
        [TestCase("kiem")]
        [TestCase("phap")]
        [TestCase("co")]
        [TestCase("linh")]
        public void CanonicalRuntimeClassesSurviveEntryMapping(string classId)
        {
            var state = Map01ACharacterEntryMapper.Resolve(Character(classId, classId, null));

            Assert.That(state.RuntimeClassId, Is.EqualTo(classId));
            Assert.That(state.LaneX, Is.EqualTo(-3.6f).Within(.0001f));
            Assert.That(state.Facing, Is.EqualTo(1));
            Assert.That(state.UsedLegacySpawnFallback, Is.True);
        }

        [TestCase("class.martial", "vo")]
        [TestCase("class.sword", "kiem")]
        public void LegacyAliasesMapToCanonicalRuntimeClasses(string rawClassId, string expected)
        {
            var state = Map01ACharacterEntryMapper.Resolve(Character(rawClassId, null, null));

            Assert.That(state.RuntimeClassId, Is.EqualTo(expected));
            Assert.That(state.UsedLegacySpawnFallback, Is.True);
        }

        [Test]
        public void ValidMap01AStateAppliesLaneAndFacing()
        {
            var runtime = new CharacterRuntimeStateResponse
            {
                mapId = "map-01a-cong-dong-lam",
                laneX = 18.5f,
                facing = -1,
                updatedAtUnixMs = 1_700_000_000_000L
            };

            var state = Map01ACharacterEntryMapper.Resolve(Character("class.sword", "kiem", runtime));

            Assert.That(state.RuntimeClassId, Is.EqualTo("kiem"));
            Assert.That(state.LaneX, Is.EqualTo(18.5f).Within(.0001f));
            Assert.That(state.Facing, Is.EqualTo(-1));
            Assert.That(state.UsedLegacySpawnFallback, Is.False);
        }

        [Test]
        public void MissingOrForeignRuntimeStateUsesSafeSpawnAndIgnoresLegacyCoordinates()
        {
            var legacy = Character("class.sword", "kiem", null);
            legacy.x = 42f;
            legacy.y = 99f;
            legacy.z = -77f;
            legacy.yawDegrees = 270f;
            var foreign = Character("class.sword", "kiem", new CharacterRuntimeStateResponse
            {
                mapId = "map-other",
                laneX = 33f,
                facing = -1,
                updatedAtUnixMs = 100
            });

            var legacyState = Map01ACharacterEntryMapper.Resolve(legacy);
            var foreignState = Map01ACharacterEntryMapper.Resolve(foreign);

            foreach (var state in new[] { legacyState, foreignState })
            {
                Assert.That(state.LaneX, Is.EqualTo(-3.6f).Within(.0001f));
                Assert.That(state.Facing, Is.EqualTo(1));
                Assert.That(state.UsedLegacySpawnFallback, Is.True);
            }
        }

        [Test]
        public void MalformedClassOrMap01AStateFailsClosed()
        {
            Assert.Throws<ArgumentException>(() =>
                Map01ACharacterEntryMapper.Resolve(Character("class.unknown", null, null)));
            Assert.Throws<ArgumentException>(() => Map01ACharacterEntryMapper.Resolve(Character("vo", "vo",
                new CharacterRuntimeStateResponse
                {
                    mapId = "map-01a-cong-dong-lam",
                    laneX = -3.81f,
                    facing = 1,
                    updatedAtUnixMs = 100
                })));
            Assert.Throws<ArgumentException>(() => Map01ACharacterEntryMapper.Resolve(Character("vo", "vo",
                new CharacterRuntimeStateResponse
                {
                    mapId = "map-01a-cong-dong-lam",
                    laneX = 0f,
                    facing = 0,
                    updatedAtUnixMs = 100
                })));
        }

        private static CharacterResponse Character(
            string rawClassId, string runtimeClassId, CharacterRuntimeStateResponse runtimeState)
        {
            return new CharacterResponse
            {
                slot = 1,
                characterId = "character.mapper",
                accountId = "account.product.mapper",
                name = "MapperHero",
                classId = rawClassId,
                runtimeClassId = runtimeClassId,
                runtimeState = runtimeState,
                entityId = 1001,
                x = 12f,
                y = 1f,
                z = -5f,
                yawDegrees = 270f,
                createdAtUnixMs = 1,
                updatedAtUnixMs = 2
            };
        }
    }
}
