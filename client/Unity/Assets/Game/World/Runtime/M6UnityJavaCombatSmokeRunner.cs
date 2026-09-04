using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M6UnityJavaCombatSmokeRunner
    {
        public static bool ShouldRun()
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m6-unity-java-combat-smoke") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var host = GetArg("--lgo-m6-combat-host") ?? "127.0.0.1";
            var port = int.TryParse(GetArg("--lgo-m6-combat-port"), out var parsedPort) ? parsedPort : 17843;
            var resultPath = GetArg("--lgo-m6-unity-java-combat-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m6-unity-java-combat-result.json");
            var result = new M6UnityJavaCombatSmokeResult { status = "STARTED", resultPath = resultPath, unityVersion = Application.unityVersion };
            var exitCode = 99;
            try
            {
                shutdownToken.ThrowIfCancellationRequested();
                var world = new GameObject("LGO M6 Unity Java Combat Smoke World").AddComponent<PlayableWorldController>();
                world.Enter(new CharacterResponse { characterId = "m6-unity-java-character", accountId = "m6-unity-java-account", name = "M6JavaHero", classId = "class.sword", x = 0f, y = 0.25f, z = 0f, yawDegrees = 0f });
                world.SetSmokePositionNearTargetDummy();
                var intent = world.BuildCombatIntentForLocalPreview(77, "unity-java-smoke-77");
                var accepted = await Send(host, port, intent.ToByteArray(), shutdownToken);
                result.accepted = accepted.kind == 1 && CombatAccepted.Parser.ParseFrom(accepted.payload).IntentId == intent.IntentId;
                var invalid = intent.ToBuilder().SetSequence(78).SetIntentId("unity-java-smoke-invalid-target").SetTargetEntityId(9999UL).Build();
                var rejected = await Send(host, port, invalid.ToByteArray(), shutdownToken);
                result.rejected = rejected.kind == 2 && CombatRejected.Parser.ParseFrom(rejected.payload).Error.Code.Contains("target_entity_id");
                var malformed = await Send(host, port, new byte[] { 1, 2, 3, 4, 5 }, shutdownToken);
                result.malformedSurvived = malformed.kind == 3;
                if (!result.accepted || !result.rejected || !result.malformedSurvived) throw new InvalidOperationException("Unity Java combat smoke response matrix failed");
                result.status = "PASS";
                result.marker = "M6_UNITY_JAVA_COMBAT_SMOKE_PASS";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                result.status = "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 16;
            }
            finally
            {
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log("[LinhGioi] M6 Unity Java combat smoke status=" + result.status + " marker=" + result.marker + " result=" + resultPath);
                Quit(exitCode);
            }
        }

        private static async Task<(byte kind, byte[] payload)> Send(string host, int port, byte[] payload, CancellationToken token)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(host, port);
            using var stream = client.GetStream();
            await WriteInt(stream, payload.Length, token);
            await stream.WriteAsync(payload, 0, payload.Length, token);
            int kind = stream.ReadByte();
            if (kind < 0) throw new IOException("server closed before response kind");
            var lengthBytes = new byte[4];
            await ReadExact(stream, lengthBytes, token);
            int length = FromBigEndian(lengthBytes);
            var response = new byte[length];
            await ReadExact(stream, response, token);
            return ((byte)kind, response);
        }

        private static async Task WriteInt(Stream stream, int value, CancellationToken token)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            await stream.WriteAsync(bytes, 0, bytes.Length, token);
        }

        private static async Task ReadExact(Stream stream, byte[] buffer, CancellationToken token)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                var read = await stream.ReadAsync(buffer, offset, buffer.Length - offset, token);
                if (read <= 0) throw new IOException("server closed before full response");
                offset += read;
            }
        }

        private static int FromBigEndian(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static void WriteResult(string path, M6UnityJavaCombatSmokeResult result)
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
        private sealed class M6UnityJavaCombatSmokeResult
        {
            public string status;
            public string marker;
            public string resultPath;
            public string unityVersion;
            public bool accepted;
            public bool rejected;
            public bool malformedSurvived;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
