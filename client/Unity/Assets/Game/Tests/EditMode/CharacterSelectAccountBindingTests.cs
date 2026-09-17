using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.UI;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.Tests.EditMode
{
    public sealed class CharacterSelectAccountBindingTests
    {
        [Test]
        public void AuthenticatedCharacterSelectBindsThreeServerSlotsWithoutMutatingRuntimeClass()
        {
            var before = Roots();
            try
            {
                var fake = FakeCharacterClient.Success(
                    Character(1, "character.1", "KiemTu", "class.sword"),
                    Character(3, "character.3", "VoGia", "class.martial"));
                var session = AuthenticatedSession("account.product.abc");
                var host = new GameObject("character select account binding test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var classBefore = scene.ActiveEquipmentClassId;
                CongDongLamArrivalHud.Attach(scene, fake, session);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                OpenCharacterSelect(host);
                Assert.That(fake.ListCallCount, Is.EqualTo(1));
                Assert.That(fake.LastAccessToken, Is.EqualTo("opaque-test-token"));
                Assert.That(root.Q<Button>("Map01A Character Slot 1").text, Does.Contain("KiemTu"));
                Assert.That(root.Q<Button>("Map01A Character Slot 2").text, Does.Contain("Chưa có nhân vật"));
                Assert.That(root.Q<Button>("Map01A Character Slot 3").text, Does.Contain("VoGia"));
                Assert.That(root.Q<Label>("Map01A Character Slot 1 Meta").text, Does.Contain("Kiếm"));
                Assert.That(root.Q<Label>("Map01A Character Slot 1 Meta").text, Does.Not.Contain("class.sword"));
                Assert.That(root.Q<Label>("Map01A Character Slot 3 Meta").text, Does.Contain("Võ"));
                Assert.That(root.Q<Label>("Map01A Character Select Stage Name").text, Is.EqualTo("KiemTu"));

                Invoke(root.Q<Button>("Map01A Character Slot 3"));

                Assert.That(root.Q<Label>("Map01A Character Select Stage Name").text, Is.EqualTo("VoGia"));
                Assert.That(root.Q<Label>("Map01A Character Select Status").text, Does.Contain("VoGia"));
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(classBefore));
            }
            finally { DestroyNewRoots(before); }
        }

        [Test]
        public void EnterGameLoadsSelectedCharacterBeforeClosingAndBindsPlayerName()
        {
            var before = Roots();
            try
            {
                var character = Character(1, "character.1", "KiemTu", "class.sword");
                character.runtimeClassId = "kiem";
                character.runtimeState = new CharacterRuntimeStateResponse
                {
                    mapId = "map-01a-cong-dong-lam", laneX = 18.5f, facing = -1,
                    updatedAtUnixMs = 1_700_000_000_000L
                };
                var fake = FakeCharacterClient.Success(character);
                var session = AuthenticatedSession("account.product.abc");
                var host = new GameObject("character load identity test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var rendererClassBefore = scene.ActiveEquipmentClassId;
                CongDongLamArrivalHud.Attach(scene, fake, session);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                OpenCharacterSelect(host);
                Invoke(root.Q<Button>("Map01A Character Select Enter Game"));

                Assert.That(fake.LoadCallCount, Is.EqualTo(1));
                Assert.That(fake.LastCharacterId, Is.EqualTo("character.1"));
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value,
                    Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Label>("Map01A Player Name").text, Is.EqualTo("KiemTu"));
                Assert.That(scene.PlayerX, Is.EqualTo(18.5f).Within(.0001f));
                Assert.That(scene.CurrentRouteNodeId, Is.EqualTo("village-square"));
                Assert.That(scene.ProductEntryFacingSign, Is.EqualTo(-1));
                Assert.That(scene.LoadedProductRuntimeClassId, Is.EqualTo("kiem"));
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(rendererClassBefore),
                    "Persisted semantic class must not impersonate an unavailable renderer.");
            }
            finally { DestroyNewRoots(before); }
        }

        [Test]
        public void LegacyCoordinatesUseSafeMap01ASpawnAndIgnoreRawXyzYaw()
        {
            var before = Roots();
            try
            {
                var character = Character(1, "character.legacy", "LegacyKiem", "class.sword");
                character.runtimeClassId = "kiem";
                character.x = 42f; character.y = 99f; character.z = -77f; character.yawDegrees = 270f;
                var fake = FakeCharacterClient.Success(character);
                var host = new GameObject("legacy map01a fallback test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var rendererClassBefore = scene.ActiveEquipmentClassId;
                CongDongLamArrivalHud.Attach(scene, fake, AuthenticatedSession("account.product.abc"));
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                OpenCharacterSelect(host);
                Invoke(root.Q<Button>("Map01A Character Select Enter Game"));

                Assert.That(scene.PlayerX, Is.EqualTo(-3.6f).Within(.0001f));
                Assert.That(scene.ProductEntryFacingSign, Is.EqualTo(1));
                Assert.That(scene.LoadedProductRuntimeClassId, Is.EqualTo("kiem"));
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(rendererClassBefore));
            }
            finally { DestroyNewRoots(before); }
        }

        [Test]
        public void FiveClassSemanticEntryNeverSwitchesReviewOnlyRenderer()
        {
            var before = Roots();
            try
            {
                var character = Character(1, "character.linh", "LinhTu", "linh");
                character.runtimeClassId = "linh";
                character.runtimeState = new CharacterRuntimeStateResponse
                { mapId = "map-01a-cong-dong-lam", laneX = 23f, facing = 1, updatedAtUnixMs = 100 };
                var fake = FakeCharacterClient.Success(character);
                var host = new GameObject("five class semantic entry test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var rendererClassBefore = scene.ActiveEquipmentClassId;
                CongDongLamArrivalHud.Attach(scene, fake, AuthenticatedSession("account.product.abc"));
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                OpenCharacterSelect(host);
                Invoke(root.Q<Button>("Map01A Character Select Enter Game"));

                Assert.That(scene.LoadedProductRuntimeClassId, Is.EqualTo("linh"));
                Assert.That(scene.PlayerX, Is.EqualTo(23f).Within(.0001f));
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(rendererClassBefore));
            }
            finally { DestroyNewRoots(before); }
        }

        [Test]
        public void CharacterSelectCaptureFlagUsesDeterministicInternalSeedOnly()
        {
            Assert.That(CongDongLamArrivalHud.ShouldSeedCharacterSelectCaptureForArgs(
                new[] { "LinhGioiOnline", "--lgo-map01a-character-select-capture" }), Is.True);
            Assert.That(CongDongLamArrivalHud.ShouldSeedCharacterSelectCaptureForArgs(
                new[] { "LinhGioiOnline" }), Is.False);
        }

        [Test]
        public void LoadFailureKeepsCharacterSelectOpenWithGenericMessage()
        {
            var before = Roots();
            try
            {
                var fake = FakeCharacterClient.LoadFailure(Character(1, "character.1", "KiemTu", "class.sword"));
                var session = AuthenticatedSession("account.product.abc");
                var host = new GameObject("character load failure test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene, fake, session);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                OpenCharacterSelect(host);

                Invoke(root.Q<Button>("Map01A Character Select Enter Game"));

                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value,
                    Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Character Select Status").text,
                    Is.EqualTo("Không thể tải nhân vật. Vui lòng thử lại."));
            }
            finally { DestroyNewRoots(before); }
        }
        private static ProductAuthSessionState AuthenticatedSession(string accountId)
        {
            var session = new ProductAuthSessionState();
            session.Set(new ProductLoginResponse
            {
                account = new AccountResponse { accountId = accountId, displayName = "minh@example.test" },
                accessToken = "opaque-test-token",
                expiresAtUnixMs = 9_999_999_999_999L
            });
            return session;
        }

        private static CharacterResponse Character(int slot, string id, string name, string classId)
        {
            return new CharacterResponse
            {
                slot = slot,
                characterId = id,
                accountId = "account.product.abc",
                name = name,
                classId = classId,
                entityId = 1000 + slot,
                createdAtUnixMs = 1,
                updatedAtUnixMs = 2
            };
        }

        private static void OpenCharacterSelect(GameObject host)
        {
            var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
            var open = typeof(CongDongLamArrivalHud).GetMethod(
                "OpenCharacterSelect", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(open, Is.Not.Null);
            open.Invoke(hud, null);
        }
        private static HashSet<GameObject> Roots()
            => new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());

        private static void DestroyNewRoots(HashSet<GameObject> before)
        {
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (!before.Contains(root)) Object.DestroyImmediate(root);
        }

        private static void Invoke(Button button)
        {
            Assert.That(button, Is.Not.Null);
            var callback = typeof(Clickable).GetField("clicked", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(button.clickable) as System.Action;
            Assert.That(callback, Is.Not.Null);
            callback();
        }

        private sealed class FakeCharacterClient : IProductAuthClient, IProductCharacterClient
        {
            private readonly CharacterResponse[] _listed;
            private readonly bool _failLoad;
            public int ListCallCount { get; private set; }
            public int LoadCallCount { get; private set; }
            public string LastAccessToken { get; private set; }
            public string LastCharacterId { get; private set; }

            private FakeCharacterClient(CharacterResponse[] listed, bool failLoad)
            {
                _listed = listed;
                _failLoad = failLoad;
            }
            public static FakeCharacterClient Success(params CharacterResponse[] listed)
                => new FakeCharacterClient(listed, false);

            public static FakeCharacterClient LoadFailure(params CharacterResponse[] listed)
                => new FakeCharacterClient(listed, true);

            public Task<CharacterResponse[]> ListProductCharactersAsync(string accessToken, CancellationToken cancellationToken)
            {
                ListCallCount++;
                LastAccessToken = accessToken;
                return Task.FromResult(_listed);
            }

            public Task<CharacterResponse> LoadProductCharacterAsync(string accessToken, string characterId, CancellationToken cancellationToken)
            {
                LoadCallCount++;
                LastAccessToken = accessToken;
                LastCharacterId = characterId;
                if (_failLoad)
                    return Task.FromException<CharacterResponse>(new AccountApiException(500, "safe failure"));
                foreach (var character in _listed)
                    if (character.characterId == characterId) return Task.FromResult(character);
                return Task.FromException<CharacterResponse>(new AccountApiException(404, "safe missing"));
            }

            public Task<CharacterResponse> SaveMap01AStateAsync(string accessToken, string characterId,
                float laneX, int facing, CancellationToken cancellationToken)
                => Task.FromException<CharacterResponse>(new System.NotSupportedException());

            public Task<ProductLoginResponse> LoginAsync(string identifier, string password,
                CancellationToken cancellationToken)
                => Task.FromException<ProductLoginResponse>(new System.NotSupportedException());

            public Task<ProductSessionResponse> ValidateSessionAsync(string accessToken,
                CancellationToken cancellationToken)
                => Task.FromException<ProductSessionResponse>(new System.NotSupportedException());

            public Task LogoutAsync(string accessToken, CancellationToken cancellationToken)
                => Task.CompletedTask;
        }
    }
}
