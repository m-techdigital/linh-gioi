using LinhGioi.Foundation;
using NUnit.Framework;

namespace LinhGioi.Tests
{
    public sealed class ClientRuntimeConfigTests
    {
        [Test]
        public void ParsesValidConfiguration()
        {
            var config = ClientRuntimeConfig.Parse("{\"environment\":\"test\",\"realtimeHost\":\"127.0.0.1\",\"realtimePort\":7777,\"protocolVersion\":1,\"clientVersion\":\"x\",\"gamedataVersion\":1,\"platform\":\"test\",\"locale\":\"vi-VN\"}");
            Assert.AreEqual(7777, config.realtimePort);
        }

        [Test]
        public void RejectsInvalidPort()
        {
            Assert.Throws<System.InvalidOperationException>(() => ClientRuntimeConfig.Parse("{\"environment\":\"test\",\"realtimeHost\":\"127.0.0.1\",\"realtimePort\":70000,\"protocolVersion\":1,\"clientVersion\":\"x\",\"gamedataVersion\":1,\"platform\":\"test\",\"locale\":\"vi-VN\"}"));
        }
    }
}
