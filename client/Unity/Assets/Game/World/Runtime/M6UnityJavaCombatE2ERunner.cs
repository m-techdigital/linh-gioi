using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using LinhGioi.Account;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M6UnityJavaCombatE2ERunner
    {
        public static bool ShouldRun()
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m6-unity-java-combat-e2e") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var host = GetArg("--lgo-m6-combat-host") ?? "127.0.0.1";
            var port = int.TryParse(GetArg("--lgo-m6-combat-port"), out var parsedPort) ? parsedPort : 17844;
            var resultPath = GetArg("--lgo-m6-unity-java-combat-e2e-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m6-unity-java-combat-e2e-result.json");
            var result = new M6UnityJavaCombatE2EResult { status = "STARTED", resultPath = resultPath, unityVersion = Application.unityVersion };
            var exitCode = 99;
            try
            {
                shutdownToken.ThrowIfCancellationRequested();
                var world = new GameObject("LGO M6 Unity Java Combat E2E World").AddComponent<PlayableWorldController>();
                world.Enter(new CharacterResponse { characterId = "m6-unity-java-e2e-character", accountId = "m6-unity-java-e2e-account", name = "M6JavaE2EHero", classId = "class.sword", x = 0f, y = 0.25f, z = 0f, yawDegrees = 0f });
                world.SetSmokePositionNearTargetDummy();

                var intent = world.BuildCombatIntentForLocalPreview(101, "unity-java-e2e-accepted-101");
                intent.LocalPreviewOnly = false;
                var acceptedResponses = await SendAll(host, port, intent.ToByteArray(), shutdownToken);
                var accepted = CombatAccepted.Parser.ParseFrom(RequireKind(acceptedResponses, 1));
                var combatResult = CombatResult.Parser.ParseFrom(RequireKind(acceptedResponses, 4));
                var snapshot = CombatStateSnapshot.Parser.ParseFrom(RequireKind(acceptedResponses, 5));
                world.MarkCombatIntentPending(intent);
                world.MarkCombatIntentAccepted(accepted);
                result.accepted = accepted.IntentId == intent.IntentId && accepted.Snapshot.TargetValid;
                result.resultObserved = combatResult.IntentId == intent.IntentId && combatResult.EffectAmount == LocalCombatPrototypeState.WindSlashPlaceholderAmount;
                result.snapshotObserved = snapshot.ActorEntityId == LocalCombatPrototypeState.ActorEntityId && snapshot.TargetValid;
                result.acceptedIntentId = accepted.IntentId;
                result.acceptedSequence = accepted.Sequence;
                result.acceptedCooldownMs = accepted.CooldownMs;
                result.acceptedSnapshotTargetValid = accepted.Snapshot.TargetValid;
                result.resultOutcome = combatResult.Outcome;
                result.resultEffectAmount = combatResult.EffectAmount;
                result.snapshotCooldownRemainingMs = snapshot.CooldownRemainingMs;

                var noTarget = intent.Clone();
                noTarget.Sequence = 102;
                noTarget.IntentId = "unity-java-e2e-no-target";
                noTarget.TargetEntityId = 0;
                var noTargetRejected = Rejection(await SendAll(host, port, noTarget.ToByteArray(), shutdownToken));
                result.rejectedNoTargetCode = noTargetRejected.Error.Code;
                result.rejectedNoTargetRetryable = noTargetRejected.Error.Retryable;
                result.rejectedNoTarget = result.rejectedNoTargetCode.Contains("no_target");

                var outOfRange = intent.Clone();
                outOfRange.Sequence = 103;
                outOfRange.IntentId = "unity-java-e2e-out-of-range";
                outOfRange.TargetPosition = new Vec3 { X = -10f, Y = 0.25f, Z = 0.5f };
                var outOfRangeRejected = Rejection(await SendAll(host, port, outOfRange.ToByteArray(), shutdownToken));
                result.rejectedOutOfRangeCode = outOfRangeRejected.Error.Code;
                result.rejectedOutOfRangeRetryable = outOfRangeRejected.Error.Retryable;
                result.rejectedOutOfRange = result.rejectedOutOfRangeCode.Contains("out_of_range");

                var cooldown = intent.Clone();
                cooldown.Sequence = 104;
                cooldown.IntentId = "unity-java-e2e-cooldown";
                var cooldownRejected = Rejection(await SendAll(host, port, cooldown.ToByteArray(), shutdownToken));
                result.rejectedCooldownCode = cooldownRejected.Error.Code;
                result.rejectedCooldownRetryable = cooldownRejected.Error.Retryable;
                result.rejectedCooldownRemainingMs = cooldownRejected.Snapshot.CooldownRemainingMs;
                result.rejectedCooldown = result.rejectedCooldownCode.Contains("cooldown");

                var invalidSkill = intent.Clone();
                invalidSkill.Sequence = 105;
                invalidSkill.IntentId = "unity-java-e2e-invalid-skill";
                invalidSkill.SkillId = "skill.unknown";
                var invalidSkillRejected = Rejection(await SendAll(host, port, invalidSkill.ToByteArray(), shutdownToken));
                result.rejectedInvalidSkillCode = invalidSkillRejected.Error.Code;
                result.rejectedInvalidSkillRetryable = invalidSkillRejected.Error.Retryable;
                result.rejectedInvalidSkill = result.rejectedInvalidSkillCode.Contains("skill_id");

                result.executedChecks = 7;
                result.localPrototypeStillSeparate = world.CombatAuthorityText.Contains("Máy chủ chấp nhận") && world.CombatFeedbackText.Contains("Mô phỏng cục bộ");
                result.nonClaims = "server-authoritative pilot only; not production combat; no production art; no enemy ai; no loot/reward/economy/db/auth/social/liveops";
                if (!result.accepted || !result.resultObserved || !result.snapshotObserved)
                    throw new InvalidOperationException("accepted/result/snapshot E2E evidence missing");
                if (!result.rejectedNoTarget || !result.rejectedOutOfRange || !result.rejectedCooldown || !result.rejectedInvalidSkill)
                    throw new InvalidOperationException("server rejection E2E matrix missing");
                if (result.acceptedSequence == 0 || result.acceptedCooldownMs == 0 || result.resultEffectAmount != LocalCombatPrototypeState.WindSlashPlaceholderAmount || result.resultOutcome.Length == 0)
                    throw new InvalidOperationException("server accepted diagnostic evidence missing");
                if (result.rejectedNoTargetRetryable || !result.rejectedOutOfRangeRetryable || !result.rejectedCooldownRetryable || result.rejectedInvalidSkillRetryable || result.rejectedCooldownRemainingMs == 0)
                    throw new InvalidOperationException("server rejection diagnostic evidence missing");
                if (!result.localPrototypeStillSeparate)
                    throw new InvalidOperationException("Unity local preview/server authority copy separation regressed");
                result.status = "PASS";
                result.marker = "M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                result.status = "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 17;
            }
            finally
            {
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log("[LinhGioi] M6 Unity Java combat E2E status=" + result.status + " marker=" + result.marker + " result=" + resultPath);
                Quit(exitCode);
            }
        }

        private static byte[] RequireKind(List<ResponseFrame> frames, byte kind)
        {
            foreach (var frame in frames)
                if (frame.kind == kind) return frame.payload;
            throw new InvalidOperationException("missing response kind " + kind);
        }

        private static CombatRejected Rejection(List<ResponseFrame> frames)
        {
            return CombatRejected.Parser.ParseFrom(RequireKind(frames, 2));
        }

        private static async Task<List<ResponseFrame>> SendAll(string host, int port, byte[] payload, CancellationToken token)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(host, port);
            using var stream = client.GetStream();
            await WriteInt(stream, payload.Length, token);
            await stream.WriteAsync(payload, 0, payload.Length, token);
            var frames = new List<ResponseFrame>();
            while (true)
            {
                int kind = stream.ReadByte();
                if (kind < 0) break;
                var lengthBytes = new byte[4];
                await ReadExact(stream, lengthBytes, token);
                int length = FromBigEndian(lengthBytes);
                var response = new byte[length];
                await ReadExact(stream, response, token);
                frames.Add(new ResponseFrame { kind = (byte)kind, payload = response });
                if (kind == 2 || kind == 3 || frames.Count >= 3) break;
            }
            return frames;
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

        private static void WriteResult(string path, M6UnityJavaCombatE2EResult result)
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

        private sealed class ResponseFrame
        {
            public byte kind;
            public byte[] payload;
        }

        [Serializable]
        private sealed class M6UnityJavaCombatE2EResult
        {
            public string status;
            public string marker;
            public string resultPath;
            public string unityVersion;
            public int executedChecks;
            public bool accepted;
            public bool resultObserved;
            public bool snapshotObserved;
            public string acceptedIntentId;
            public uint acceptedSequence;
            public uint acceptedCooldownMs;
            public bool acceptedSnapshotTargetValid;
            public string resultOutcome;
            public int resultEffectAmount;
            public uint snapshotCooldownRemainingMs;
            public bool rejectedNoTarget;
            public string rejectedNoTargetCode;
            public bool rejectedNoTargetRetryable;
            public bool rejectedOutOfRange;
            public string rejectedOutOfRangeCode;
            public bool rejectedOutOfRangeRetryable;
            public bool rejectedCooldown;
            public string rejectedCooldownCode;
            public bool rejectedCooldownRetryable;
            public uint rejectedCooldownRemainingMs;
            public bool rejectedInvalidSkill;
            public string rejectedInvalidSkillCode;
            public bool rejectedInvalidSkillRetryable;
            public bool localPrototypeStillSeparate;
            public string nonClaims;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
