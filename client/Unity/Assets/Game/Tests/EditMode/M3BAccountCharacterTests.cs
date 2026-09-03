using System;
using LinhGioi.Account;
using LinhGioi.Foundation;
using NUnit.Framework;

namespace LinhGioi.Tests
{
    public sealed class M3BAccountCharacterTests
    {
        [Test]
        public void ClientRuntimeConfigCarriesApiEndpointForM3B()
        {
            var config = new ClientRuntimeConfig
            {
                environment = "m3b-test",
                apiBaseUrl = "http://127.0.0.1:18083",
                apiTimeoutSeconds = 10,
                realtimeHost = "127.0.0.1",
                realtimePort = 7777,
                protocolVersion = 1,
                clientVersion = "0.3.1-m3b-test",
                gamedataVersion = 1,
                platform = "unity-editmode",
                locale = "vi-VN"
            };

            Assert.DoesNotThrow(config.Validate);
            Assert.AreEqual("http://127.0.0.1:18083", AccountApiClient.NormalizeBaseUrl(config.apiBaseUrl));
        }

        [Test]
        public void AccountApiClientNormalizesAndRejectsInvalidBaseUrls()
        {
            Assert.AreEqual("http://127.0.0.1:18083", AccountApiClient.NormalizeBaseUrl(" http://127.0.0.1:18083/ "));
            Assert.Throws<ArgumentException>(() => AccountApiClient.NormalizeBaseUrl(""));
            Assert.Throws<ArgumentException>(() => AccountApiClient.NormalizeBaseUrl("file:///tmp/not-api"));
        }

        [Test]
        public void CharacterListParserHandlesM3TopLevelArrayContract()
        {
            var characters = AccountApiClient.ParseCharacterListJson("[{\"characterId\":\"character.abc\",\"accountId\":\"account.dev.abc\",\"name\":\"M3BHero\",\"classId\":\"class.sword\",\"entityId\":1001,\"x\":3.25,\"y\":0.5,\"z\":-7.75,\"yawDegrees\":270.0,\"createdAtUnixMs\":1,\"updatedAtUnixMs\":2}]");

            Assert.AreEqual(1, characters.Length);
            Assert.AreEqual("character.abc", characters[0].characterId);
            Assert.AreEqual("account.dev.abc", characters[0].accountId);
            Assert.AreEqual("M3BHero", characters[0].name);
            Assert.AreEqual("class.sword", characters[0].classId);
            Assert.AreEqual(1001L, characters[0].entityId);
            Assert.IsTrue(characters[0].HasSamePosition(3.25f, 0.5f, -7.75f, 270.0f));
        }

        [Test]
        public void CharacterListParserRejectsNonArrayBodies()
        {
            Assert.Throws<ArgumentException>(() => AccountApiClient.ParseCharacterListJson("{\"characters\":[]}"));
        }
    }
}
