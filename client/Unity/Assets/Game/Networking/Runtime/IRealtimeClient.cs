using System;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Foundation;
using LinhGioi.Protocol.V1;

namespace LinhGioi.Networking
{
    public interface IRealtimeClient : IDisposable
    {
        ConnectionState State { get; }
        event Action<ConnectionState> StateChanged;
        Task<ServerHello> ConnectAndHandshakeAsync(ClientRuntimeConfig config, CancellationToken cancellationToken);
        Task<PlayerTransformSnapshot> SendMoveIntentAsync(MoveIntent intent, CancellationToken cancellationToken);
        Task DisconnectAsync();
    }
}
