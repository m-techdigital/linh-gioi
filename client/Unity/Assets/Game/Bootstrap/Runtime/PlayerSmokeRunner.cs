using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Foundation;
using LinhGioi.Networking;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.Bootstrap
{
    public static class PlayerSmokeRunner
    {
        private const int DefaultTimeoutMs = 15000;

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_PLAYER_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "--lgo-player-smoke") return true;
            }
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var startedAt = DateTimeOffset.UtcNow;
            var exitCode = 99;
            var resultPath = GetArg("--lgo-smoke-result") ?? Path.Combine(Application.persistentDataPath, "lgo-player-smoke-result.json");
            var result = new PlayerSmokeResult
            {
                status = "STARTED",
                startedAtUtc = startedAt.ToString("O"),
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                resultPath = resultPath
            };

            try
            {
                var config = ClientRuntimeConfig.LoadStreamingAssets();
                ApplyCommandLineOverrides(config);
                config.Validate();
                result.realtimeHost = config.realtimeHost;
                result.realtimePort = config.realtimePort;
                result.protocolVersion = config.protocolVersion;
                result.gamedataVersion = config.gamedataVersion;
                result.clientVersion = config.clientVersion;

                var timeoutMs = GetIntArg("--lgo-timeout-ms", DefaultTimeoutMs);
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken))
                using (var client = new TcpRealtimeClient())
                {
                    timeout.CancelAfter(timeoutMs);
                    var response = await client.ConnectAndHandshakeAsync(config, timeout.Token);
                    result.accepted = response.Accepted;
                    result.errorCode = response.Error == null ? string.Empty : response.Error.Code;
                    result.errorMessage = response.Error == null ? string.Empty : response.Error.Message;
                    result.serverMessage = response.Accepted ? "accepted" : result.errorMessage;
                    result.status = response.Accepted ? "PASS" : "REJECTED";
                    exitCode = response.Accepted ? 0 : 12;
                    await client.DisconnectAsync();
                }
            }
            catch (OperationCanceledException exception)
            {
                result.status = "TIMEOUT";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 13;
            }
            catch (Exception exception)
            {
                result.status = "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 14;
            }
            finally
            {
                result.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log($"[LinhGioi] Player smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static void ApplyCommandLineOverrides(ClientRuntimeConfig config)
        {
            var host = GetArg("--lgo-server-host");
            if (!string.IsNullOrWhiteSpace(host)) config.realtimeHost = host;
            var port = GetIntArg("--lgo-server-port", config.realtimePort);
            config.realtimePort = port;
            var protocolVersion = GetUIntArg("--lgo-protocol-version", config.protocolVersion);
            config.protocolVersion = protocolVersion;
            var gamedataVersion = GetUIntArg("--lgo-gamedata-version", config.gamedataVersion);
            config.gamedataVersion = gamedataVersion;
            var clientVersion = GetArg("--lgo-client-version");
            if (!string.IsNullOrWhiteSpace(clientVersion)) config.clientVersion = clientVersion;
            var locale = GetArg("--lgo-locale");
            if (!string.IsNullOrWhiteSpace(locale)) config.locale = locale;
            config.connectOnStart = true;
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == key) return args[i + 1];
            }
            return null;
        }

        private static int GetIntArg(string key, int fallback)
        {
            var raw = GetArg(key);
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            if (!int.TryParse(raw, out var value)) throw new InvalidOperationException($"{key} must be an integer.");
            return value;
        }

        private static uint GetUIntArg(string key, uint fallback)
        {
            var raw = GetArg(key);
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            if (!uint.TryParse(raw, out var value)) throw new InvalidOperationException($"{key} must be an unsigned integer.");
            return value;
        }

        private static void WriteResult(string path, PlayerSmokeResult result)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, JsonUtility.ToJson(result, true));
        }

        private static void Quit(int exitCode)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(exitCode);
#else
            Application.Quit(exitCode);
#endif
        }

        [Serializable]
        private sealed class PlayerSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string realtimeHost;
            public int realtimePort;
            public uint protocolVersion;
            public uint gamedataVersion;
            public string clientVersion;
            public bool accepted;
            public string serverMessage;
            public string errorCode;
            public string errorMessage;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
