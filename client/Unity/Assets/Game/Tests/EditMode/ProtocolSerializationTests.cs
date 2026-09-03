using LinhGioi.Protocol.V1;
using NUnit.Framework;

namespace LinhGioi.Tests
{
    public sealed class ProtocolSerializationTests
    {
        [Test]
        public void ClientHelloParserConsumesCanonicalWirePayload()
        {
            var payload = new byte[]
            {
                0x08, 0x01,
                0x12, 0x07, 0x6d, 0x30, 0x2d, 0x74, 0x65, 0x73, 0x74,
                0x18, 0x01,
                0x22, 0x0a, 0x75, 0x6e, 0x69, 0x74, 0x79, 0x2d, 0x74, 0x65, 0x73, 0x74,
                0x2a, 0x05, 0x76, 0x69, 0x2d, 0x56, 0x4e
            };

            var output = ClientHello.Parser.ParseFrom(payload);
            Assert.AreEqual(1u, output.ProtocolVersion);
            Assert.AreEqual("m0-test", output.ClientVersion);
            Assert.AreEqual(1u, output.GamedataVersion);
            Assert.AreEqual("unity-test", output.Platform);
            Assert.AreEqual("vi-VN", output.Locale);
            Assert.Greater(output.CalculateSize(), 0);
        }
    }
}
