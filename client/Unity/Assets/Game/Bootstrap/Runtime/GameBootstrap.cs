using System;
using System.Threading;
using LinhGioi.Combat;
using LinhGioi.Account;
using LinhGioi.Foundation;
using LinhGioi.Networking;
using UnityEngine;

namespace LinhGioi.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private IRealtimeClient _realtimeClient;
        private CancellationTokenSource _shutdown;

        private async void Start()
        {
            _shutdown = new CancellationTokenSource();
            try
            {
                if (M3BAccountCharacterSmokeRunner.ShouldRun())
                {
                    await M3BAccountCharacterSmokeRunner.RunFromCommandLineAsync(_shutdown.Token);
                    return;
                }

                if (OnlineSessionSmokeRunner.ShouldRun())
                {
                    await OnlineSessionSmokeRunner.RunFromCommandLineAsync(_shutdown.Token);
                    return;
                }

                if (OfflineCombatSmokeRunner.ShouldRun())
                {
                    OfflineCombatSmokeRunner.RunFromCommandLine();
                    return;
                }

                if (PlayerSmokeRunner.ShouldRun())
                {
                    await PlayerSmokeRunner.RunFromCommandLineAsync(_shutdown.Token);
                    return;
                }

                var config = ClientRuntimeConfig.LoadStreamingAssets();
                Debug.Log($"[LinhGioi] Bootstrap environment={config.environment} protocol={config.protocolVersion} gamedata={config.gamedataVersion}");
                if (!config.connectOnStart) return;
                _realtimeClient = new TcpRealtimeClient();
                _realtimeClient.StateChanged += OnConnectionStateChanged;
                var response = await _realtimeClient.ConnectAndHandshakeAsync(config, _shutdown.Token);
                if (!response.Accepted)
                    Debug.LogWarning($"[LinhGioi] Server rejected handshake: {response.Error?.Code} {response.Error?.Message}");
            }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void OnConnectionStateChanged(ConnectionState state)
        {
            Debug.Log($"[LinhGioi] Realtime state={state}");
        }

        private async void OnDestroy()
        {
            if (_shutdown == null) return;
            _shutdown.Cancel();
            if (_realtimeClient != null)
            {
                _realtimeClient.StateChanged -= OnConnectionStateChanged;
                await _realtimeClient.DisconnectAsync();
                _realtimeClient.Dispose();
            }
            _shutdown.Dispose();
        }
    }
}
