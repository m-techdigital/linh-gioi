using System.IO;
using Google.Protobuf;
using LinhGioi.Networking;
using LinhGioi.Protocol.V1;
using NUnit.Framework;

namespace LinhGioi.Tests
{
    public sealed class M2OnlineSessionTests
    {
        [Test]
        public void MoveIntentSerializesDeterministicSequenceAxisAndDelta()
        {
            var intent = new MoveIntent
            {
                Sequence = 1,
                MoveAxis = new Vec2 { X = 1f, Y = 0f },
                ClientDeltaSeconds = 0.1f
            };

            var parsed = MoveIntent.Parser.ParseFrom(intent.ToByteArray());

            Assert.AreEqual(1u, parsed.Sequence);
            Assert.AreEqual(1f, parsed.MoveAxis.X, 0.0001f);
            Assert.AreEqual(0f, parsed.MoveAxis.Y, 0.0001f);
            Assert.AreEqual(0.1f, parsed.ClientDeltaSeconds, 0.0001f);
        }

        [Test]
        public void PlayerTransformSnapshotCarriesAuthoritativeAckAndPosition()
        {
            var snapshot = new PlayerTransformSnapshot
            {
                EntityId = 1001UL,
                AcknowledgedSequence = 1,
                Position = new Vec3 { X = 0.4f, Y = 0f, Z = 0f },
                YawDegrees = 90f,
                ServerTimeUnixMs = 1234L
            };

            var parsed = PlayerTransformSnapshot.Parser.ParseFrom(snapshot.ToByteArray());

            Assert.AreEqual(1001UL, parsed.EntityId);
            Assert.AreEqual(1u, parsed.AcknowledgedSequence);
            Assert.AreEqual(0.4f, parsed.Position.X, 0.0001f);
            Assert.AreEqual(90f, parsed.YawDegrees, 0.0001f);
            Assert.AreEqual(1234L, parsed.ServerTimeUnixMs);
        }


        [Test]
        public void TcpRealtimeClientRejectsNonNormalizedMoveIntentBeforeSend()
        {
            Assert.Throws<InvalidDataException>(() => TcpRealtimeClient.ValidateMoveIntent(new MoveIntent
            {
                Sequence = 1,
                MoveAxis = new Vec2 { X = 1f, Y = 1f },
                ClientDeltaSeconds = 0.1f
            }));
        }

        [Test]
        public void TcpRealtimeClientRejectsNonFiniteMoveIntentBeforeSend()
        {
            Assert.Throws<InvalidDataException>(() => TcpRealtimeClient.ValidateMoveIntent(new MoveIntent
            {
                Sequence = 1,
                MoveAxis = new Vec2 { X = float.NaN, Y = 0f },
                ClientDeltaSeconds = 0.1f
            }));
        }
    }
}
