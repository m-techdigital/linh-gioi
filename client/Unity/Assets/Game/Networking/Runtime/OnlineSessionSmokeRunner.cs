using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Foundation;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.Networking
{
    public static class OnlineSessionSmokeRunner
    {
        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M2_ONLINE_SESSION_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m2-online-session-smoke") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken cancellationToken)
        {
            var resultPath = GetArg("--lgo-m2-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m2-online-session-result.json");
            var exitCode = 99;
            var wrapper = new OnlineSessionSmokeResult
            {
                status = "STARTED",
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                resultPath = resultPath,
                startedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            };

            try
            {
                var host = GetArg("--lgo-m2-host") ?? "127.0.0.1";
                var portText = GetArg("--lgo-m2-port") ?? "7777";
                if (!int.TryParse(portText, out var port) || port < 1 || port > 65535)
                    throw new InvalidOperationException("--lgo-m2-port must be a valid TCP port.");

                wrapper.host = host;
                wrapper.port = port;

                var config = new ClientRuntimeConfig
                {
                    environment = "m2-online-session-smoke",
                    realtimeHost = host,
                    realtimePort = port,
                    protocolVersion = 1,
                    clientVersion = "0.2.0-m2-smoke",
                    gamedataVersion = 1,
                    platform = "unity-m2-smoke",
                    locale = "vi-VN",
                    connectOnStart = false
                };

                using (var client = new TcpRealtimeClient())
                {
                    var hello = await client.ConnectAndHandshakeAsync(config, cancellationToken);
                    wrapper.handshakeAccepted = hello.Accepted;
                    wrapper.serverTimeUnixMs = hello.ServerTimeUnixMs;
                    if (!hello.Accepted)
                        throw new InvalidOperationException($"M2 server rejected smoke handshake: {hello.Error?.Code} {hello.Error?.Message}");

                    var move = new MoveIntent
                    {
                        Sequence = 1,
                        MoveAxis = new Vec2 { X = 1f, Y = 0f },
                        ClientDeltaSeconds = 0.1f
                    };
                    var snapshot = await client.SendMoveIntentAsync(move, cancellationToken);
                    wrapper.snapshot = SnapshotSummary.From(snapshot);

                    if (snapshot.EntityId != 1001UL) throw new InvalidOperationException("Unexpected M2 player entity id.");
                    if (snapshot.AcknowledgedSequence != 1) throw new InvalidOperationException("M2 snapshot did not acknowledge sequence 1.");
                    if (Mathf.Abs(snapshot.Position.X - 0.4f) > 0.001f) throw new InvalidOperationException("M2 snapshot position.x mismatch.");

                    var duplicate = await client.SendMoveIntentAsync(move, cancellationToken);
                    wrapper.duplicateSnapshot = SnapshotSummary.From(duplicate);
                    if (duplicate.AcknowledgedSequence != 1) throw new InvalidOperationException("M2 duplicate snapshot should keep acknowledgement sequence 1.");
                    if (Mathf.Abs(duplicate.Position.X - 0.4f) > 0.001f || Mathf.Abs(duplicate.Position.Z) > 0.001f)
                        throw new InvalidOperationException("M2 duplicate movement must not move the player again.");

                    var secondMove = new MoveIntent
                    {
                        Sequence = 2,
                        MoveAxis = new Vec2 { X = 0f, Y = 1f },
                        ClientDeltaSeconds = 0.05f
                    };
                    var second = await client.SendMoveIntentAsync(secondMove, cancellationToken);
                    wrapper.secondSnapshot = SnapshotSummary.From(second);
                    if (second.AcknowledgedSequence != 2) throw new InvalidOperationException("M2 second snapshot did not acknowledge sequence 2.");
                    if (Mathf.Abs(second.Position.X - 0.4f) > 0.001f || Mathf.Abs(second.Position.Z - 0.2f) > 0.001f)
                        throw new InvalidOperationException("M2 second movement authoritative position mismatch.");

                    await client.DisconnectAsync();
                }

                wrapper.status = "PASS";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                wrapper.status = "FAIL";
                wrapper.exceptionType = exception.GetType().FullName;
                wrapper.exceptionMessage = exception.Message;
                exitCode = 16;
            }
            finally
            {
                wrapper.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                wrapper.exitCode = exitCode;
                WriteResult(resultPath, wrapper);
                Debug.Log($"[LinhGioi] M2 online session smoke status={wrapper.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static void WriteResult(string path, OnlineSessionSmokeResult result)
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
        private sealed class OnlineSessionSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string host;
            public int port;
            public bool handshakeAccepted;
            public long serverTimeUnixMs;
            public SnapshotSummary snapshot;
            public SnapshotSummary duplicateSnapshot;
            public SnapshotSummary secondSnapshot;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }

        [Serializable]
        private sealed class SnapshotSummary
        {
            public ulong entityId;
            public uint acknowledgedSequence;
            public float x;
            public float y;
            public float z;
            public float yawDegrees;
            public long serverTimeUnixMs;

            public static SnapshotSummary From(PlayerTransformSnapshot snapshot)
            {
                return new SnapshotSummary
                {
                    entityId = snapshot.EntityId,
                    acknowledgedSequence = snapshot.AcknowledgedSequence,
                    x = snapshot.Position.X,
                    y = snapshot.Position.Y,
                    z = snapshot.Position.Z,
                    yawDegrees = snapshot.YawDegrees,
                    serverTimeUnixMs = snapshot.ServerTimeUnixMs
                };
            }
        }
    }
}
