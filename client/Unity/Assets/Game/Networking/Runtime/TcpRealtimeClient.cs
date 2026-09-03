using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using LinhGioi.Foundation;
using LinhGioi.Protocol.V1;

namespace LinhGioi.Networking
{
    public sealed class TcpRealtimeClient : IRealtimeClient
    {
        private const int MaxFrameBytes = 64 * 1024;
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _disposed;

        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;
        public event Action<ConnectionState> StateChanged;

        public async Task<ServerHello> ConnectAndHandshakeAsync(ClientRuntimeConfig config, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (State != ConnectionState.Disconnected)
                throw new InvalidOperationException("Realtime client must be disconnected before connecting.");
            config.Validate();
            SetState(ConnectionState.Connecting);
            try
            {
                _client = new TcpClient();
                cancellationToken.ThrowIfCancellationRequested();
                await _client.ConnectAsync(config.realtimeHost, config.realtimePort);
                cancellationToken.ThrowIfCancellationRequested();
                _stream = _client.GetStream();
                SetState(ConnectionState.Handshaking);

                var hello = new ClientHello
                {
                    ProtocolVersion = config.protocolVersion,
                    ClientVersion = config.clientVersion,
                    GamedataVersion = config.gamedataVersion,
                    Platform = config.platform,
                    Locale = config.locale
                };
                await WriteFrameAsync(hello.ToByteArray(), cancellationToken);
                var responseBytes = await ReadFrameAsync(cancellationToken);
                var response = ServerHello.Parser.ParseFrom(responseBytes);
                if (!response.Accepted)
                {
                    SetState(ConnectionState.Faulted);
                    await DisconnectTransportAsync();
                    return response;
                }
                SetState(ConnectionState.Connected);
                return response;
            }
            catch
            {
                SetState(ConnectionState.Faulted);
                await DisconnectTransportAsync();
                throw;
            }
        }


        public async Task<PlayerTransformSnapshot> SendMoveIntentAsync(MoveIntent intent, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (State != ConnectionState.Connected)
                throw new InvalidOperationException("Realtime client must be connected before sending movement.");
            ValidateMoveIntent(intent);

            await WriteFrameAsync(intent.ToByteArray(), cancellationToken);
            var responseBytes = await ReadFrameAsync(cancellationToken);
            var snapshot = PlayerTransformSnapshot.Parser.ParseFrom(responseBytes);
            if (snapshot.AcknowledgedSequence == 0)
                throw new InvalidDataException("PlayerTransformSnapshot must acknowledge a positive sequence.");
            return snapshot;
        }

        public static void ValidateMoveIntent(MoveIntent intent)
        {
            if (intent == null) throw new ArgumentNullException(nameof(intent));
            if (intent.Sequence == 0) throw new InvalidDataException("MoveIntent.sequence must be positive.");
            if (intent.MoveAxis == null) throw new InvalidDataException("MoveIntent.move_axis must be present.");
            if (float.IsNaN(intent.ClientDeltaSeconds) || float.IsInfinity(intent.ClientDeltaSeconds) ||
                intent.ClientDeltaSeconds <= 0f || intent.ClientDeltaSeconds > 0.25f)
                throw new InvalidDataException("MoveIntent.client_delta_seconds must be > 0 and <= 0.25.");

            var axis = intent.MoveAxis;
            if (float.IsNaN(axis.X) || float.IsInfinity(axis.X) ||
                float.IsNaN(axis.Y) || float.IsInfinity(axis.Y))
                throw new InvalidDataException("MoveIntent.move_axis must contain finite values.");
            if (Math.Abs(axis.X) > 1f || Math.Abs(axis.Y) > 1f)
                throw new InvalidDataException("MoveIntent.move_axis values must be normalized to [-1, 1].");

            var magnitude = Math.Sqrt(axis.X * axis.X + axis.Y * axis.Y);
            if (magnitude > 1.0001d)
                throw new InvalidDataException("MoveIntent.move_axis magnitude must be <= 1.");
        }

        public async Task DisconnectAsync()
        {
            if (_disposed) return;
            await DisconnectTransportAsync();
            SetState(ConnectionState.Disconnected);
        }

        private async Task WriteFrameAsync(byte[] payload, CancellationToken cancellationToken)
        {
            if (payload == null || payload.Length < 1 || payload.Length > MaxFrameBytes)
                throw new InvalidDataException("Outgoing realtime frame length is invalid.");
            var header = new byte[4];
            header[0] = (byte)((payload.Length >> 24) & 0xff);
            header[1] = (byte)((payload.Length >> 16) & 0xff);
            header[2] = (byte)((payload.Length >> 8) & 0xff);
            header[3] = (byte)(payload.Length & 0xff);
            await _stream.WriteAsync(header, 0, header.Length, cancellationToken);
            await _stream.WriteAsync(payload, 0, payload.Length, cancellationToken);
            await _stream.FlushAsync(cancellationToken);
        }

        private async Task<byte[]> ReadFrameAsync(CancellationToken cancellationToken)
        {
            var header = await ReadExactAsync(4, cancellationToken);
            var length = (header[0] << 24) | (header[1] << 16) | (header[2] << 8) | header[3];
            if (length < 1 || length > MaxFrameBytes)
                throw new InvalidDataException("Incoming realtime frame length is invalid.");
            return await ReadExactAsync(length, cancellationToken);
        }

        private async Task<byte[]> ReadExactAsync(int length, CancellationToken cancellationToken)
        {
            var buffer = new byte[length];
            var offset = 0;
            while (offset < length)
            {
                var read = await _stream.ReadAsync(buffer, offset, length - offset, cancellationToken);
                if (read == 0) throw new EndOfStreamException("Realtime connection closed before the frame completed.");
                offset += read;
            }
            return buffer;
        }

        private Task DisconnectTransportAsync()
        {
            try { _stream?.Dispose(); } catch { }
            try { _client?.Dispose(); } catch { }
            _stream = null;
            _client = null;
            return Task.CompletedTask;
        }

        private void SetState(ConnectionState state)
        {
            if (State == state) return;
            State = state;
            StateChanged?.Invoke(state);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(TcpRealtimeClient));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _stream?.Dispose(); } catch { }
            try { _client?.Dispose(); } catch { }
            _stream = null;
            _client = null;
            State = ConnectionState.Disconnected;
        }
    }
}
