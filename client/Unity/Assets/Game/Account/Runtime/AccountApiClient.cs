using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Foundation;
using UnityEngine;
using UnityEngine.Networking;

namespace LinhGioi.Account
{
    public sealed class AccountApiClient : IProductAuthClient, IProductAccountClient, IProductCharacterClient, IDisposable
    {
        private const int DefaultTimeoutSeconds = 10;
        private readonly string _apiBaseUrl;
        private readonly int _timeoutSeconds;
        private bool _disposed;

        public AccountApiClient(ClientRuntimeConfig config)
            : this(config == null ? null : config.apiBaseUrl, config == null ? DefaultTimeoutSeconds : config.apiTimeoutSeconds)
        {
            config?.Validate();
        }

        public AccountApiClient(string apiBaseUrl, int timeoutSeconds = DefaultTimeoutSeconds)
        {
            _apiBaseUrl = NormalizeBaseUrl(apiBaseUrl);
            _timeoutSeconds = timeoutSeconds < 1 ? DefaultTimeoutSeconds : timeoutSeconds;
        }

        public Task<DevLoginResponse> LoginDevAsync(string devKey, string displayName, CancellationToken cancellationToken)
        {
            return SendJsonAsync<DevLoginResponse>("POST", "/dev/auth/login", new DevLoginRequest(devKey, displayName), 200, cancellationToken);
        }

        public Task<ProductLoginResponse> LoginAsync(string identifier, string password, CancellationToken cancellationToken)
        {
            return SendJsonAsync<ProductLoginResponse>("POST", "/auth/login",
                new ProductLoginRequest(identifier, password), 200, cancellationToken);
        }

        public Task<ProductSessionResponse> ValidateSessionAsync(string accessToken, CancellationToken cancellationToken)
        {
            return SendJsonAsync<ProductSessionResponse>("GET", "/auth/session", null, 200, cancellationToken, accessToken);
        }

        public async Task LogoutAsync(string accessToken, CancellationToken cancellationToken)
        {
            await SendJsonRawAsync("POST", "/auth/logout", null, 204, cancellationToken, accessToken);
        }

        public Task<ProductRegisterResponse> RegisterAsync(string email, string password, bool acceptedTerms,
            CancellationToken cancellationToken)
        {
            return SendJsonAsync<ProductRegisterResponse>("POST", ProductAccountRoutes.Register,
                new ProductRegisterRequest(email, password, acceptedTerms), 201, cancellationToken);
        }

        public Task<PasswordRecoveryRequestResponse> RequestPasswordRecoveryAsync(string email,
            CancellationToken cancellationToken)
        {
            return SendJsonAsync<PasswordRecoveryRequestResponse>("POST", ProductAccountRoutes.RecoveryRequest,
                new PasswordRecoveryRequest(email), 202, cancellationToken);
        }

        public Task<PasswordRecoveryVerifyResponse> VerifyPasswordRecoveryAsync(string challengeId, string code,
            CancellationToken cancellationToken)
        {
            return SendJsonAsync<PasswordRecoveryVerifyResponse>("POST", ProductAccountRoutes.RecoveryVerify,
                new PasswordRecoveryVerifyRequest(challengeId, code), 200, cancellationToken);
        }

        public async Task ResetPasswordAsync(string resetToken, string newPassword, CancellationToken cancellationToken)
        {
            await SendJsonRawAsync("POST", ProductAccountRoutes.RecoveryReset,
                new PasswordRecoveryResetRequest(resetToken, newPassword), 204, cancellationToken);
        }

        public async Task<CharacterResponse[]> ListProductCharactersAsync(string accessToken, CancellationToken cancellationToken)
        {
            var body = await SendJsonRawAsync("GET", ProductCharacterRoutes.List, null, 200, cancellationToken, accessToken);
            return ParseCharacterListJson(body);
        }

        public Task<CharacterResponse> LoadProductCharacterAsync(string accessToken, string characterId, CancellationToken cancellationToken)
        {
            return SendJsonAsync<CharacterResponse>("GET", ProductCharacterRoutes.LoadPrefix + EscapePath(characterId),
                null, 200, cancellationToken, accessToken);
        }

        public Task<CharacterResponse> SaveMap01AStateAsync(string accessToken, string characterId, float laneX,
            int facing, CancellationToken cancellationToken)
        {
            return SendJsonAsync<CharacterResponse>("POST",
                ProductCharacterRoutes.LoadPrefix + EscapePath(characterId) + ProductCharacterRoutes.Map01AStateSuffix,
                new SaveMap01AStateRequest(laneX, facing), 200, cancellationToken, accessToken);
        }

        public async Task<CharacterResponse[]> ListCharactersAsync(string accountId, CancellationToken cancellationToken)
        {
            var body = await SendJsonRawAsync("GET", "/accounts/" + EscapePath(accountId) + "/characters", null, 200, cancellationToken);
            return ParseCharacterListJson(body);
        }

        public Task<CharacterResponse> CreateCharacterAsync(string accountId, string name, string classId, CancellationToken cancellationToken)
        {
            return SendJsonAsync<CharacterResponse>("POST", "/accounts/" + EscapePath(accountId) + "/characters", new CreateCharacterRequest(name, classId), 201, cancellationToken);
        }

        public Task<CharacterResponse> CreateCharacterAsync(string accountId, string name, string classId, int slot, CancellationToken cancellationToken)
        {
            if (slot < 1 || slot > 3) throw new ArgumentOutOfRangeException(nameof(slot));
            return SendJsonAsync<CharacterResponse>("POST", "/accounts/" + EscapePath(accountId) + "/characters",
                new CreateCharacterInSlotRequest(name, classId, slot), 201, cancellationToken);
        }

        public Task<CharacterResponse> LoadCharacterAsync(string characterId, CancellationToken cancellationToken)
        {
            return SendJsonAsync<CharacterResponse>("GET", "/characters/" + EscapePath(characterId), null, 200, cancellationToken);
        }

        public Task<CharacterResponse> SaveCharacterPositionAsync(string characterId, float x, float y, float z, float yawDegrees, CancellationToken cancellationToken)
        {
            return SendJsonAsync<CharacterResponse>("POST", "/characters/" + EscapePath(characterId) + "/position", new SaveCharacterPositionRequest(x, y, z, yawDegrees), 200, cancellationToken);
        }

        public static CharacterResponse[] ParseCharacterListJson(string json)
        {
            var wrapped = JsonUtility.FromJson<CharacterListResponse>(WrapTopLevelArray("characters", json));
            return wrapped == null || wrapped.characters == null ? Array.Empty<CharacterResponse>() : wrapped.characters;
        }

        public static string NormalizeBaseUrl(string apiBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(apiBaseUrl)) throw new ArgumentException("apiBaseUrl must not be blank", nameof(apiBaseUrl));
            var normalized = apiBaseUrl.Trim().TrimEnd('/');
            if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("apiBaseUrl must be an absolute HTTP(S) URL", nameof(apiBaseUrl));
            return normalized;
        }

        private async Task<T> SendJsonAsync<T>(string method, string path, object payload, long expectedStatus, CancellationToken cancellationToken, string bearerToken = null)
        {
            var body = await SendJsonRawAsync(method, path, payload, expectedStatus, cancellationToken, bearerToken);
            var parsed = JsonUtility.FromJson<T>(body);
            if (parsed == null) throw new InvalidOperationException("API response could not be parsed as " + typeof(T).Name + ".");
            return parsed;
        }

        private async Task<string> SendJsonRawAsync(string method, string path, object payload, long expectedStatus, CancellationToken cancellationToken, string bearerToken = null)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            using (var request = new UnityWebRequest(_apiBaseUrl + path, method))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = _timeoutSeconds;
                request.SetRequestHeader("Accept", "application/json");
                if (bearerToken != null)
                {
                    if (string.IsNullOrWhiteSpace(bearerToken)) throw new ArgumentException("accessToken must not be blank", nameof(bearerToken));
                    request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
                }
                if (payload != null)
                {
                    var json = JsonUtility.ToJson(payload);
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                var responseBody = request.downloadHandler == null ? string.Empty : request.downloadHandler.text;
                if (request.result != UnityWebRequest.Result.Success || request.responseCode != expectedStatus)
                    throw new AccountApiException(request.responseCode,
                        $"API {method} {path} expected HTTP {expectedStatus} but got {request.responseCode}: {request.error} {responseBody}");
                return responseBody;
            }
        }

        private static string WrapTopLevelArray(string fieldName, string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON array body must not be blank", nameof(json));
            var trimmed = json.Trim();
            if (!trimmed.StartsWith("[", StringComparison.Ordinal) || !trimmed.EndsWith("]", StringComparison.Ordinal))
                throw new ArgumentException("Expected a top-level JSON array.", nameof(json));
            return "{\"" + fieldName + "\":" + trimmed + "}";
        }

        private static string EscapePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Path value must not be blank.", nameof(value));
            return UnityWebRequest.EscapeURL(value.Trim()).Replace("+", "%20");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AccountApiClient));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
