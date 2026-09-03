using System;
using System.IO;
using UnityEngine;

namespace LinhGioi.Foundation
{
    [Serializable]
    public sealed class ClientRuntimeConfig
    {
        public string environment = "local";
        public string apiBaseUrl = "http://127.0.0.1:18083";
        public int apiTimeoutSeconds = 10;
        public string realtimeHost = "127.0.0.1";
        public int realtimePort = 7777;
        public uint protocolVersion = 1;
        public string clientVersion = "0.4.0-m4";
        public uint gamedataVersion = 1;
        public string platform = "unity";
        public string locale = "vi-VN";
        public bool connectOnStart;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(environment)) throw new InvalidOperationException("environment must not be blank");
            if (string.IsNullOrWhiteSpace(apiBaseUrl)) throw new InvalidOperationException("apiBaseUrl must not be blank");
            if (!Uri.TryCreate(apiBaseUrl.Trim(), UriKind.Absolute, out var apiUri) || (apiUri.Scheme != Uri.UriSchemeHttp && apiUri.Scheme != Uri.UriSchemeHttps))
                throw new InvalidOperationException("apiBaseUrl must be an absolute HTTP(S) URL");
            if (apiTimeoutSeconds < 1 || apiTimeoutSeconds > 120) throw new InvalidOperationException("apiTimeoutSeconds must be between 1 and 120");
            if (string.IsNullOrWhiteSpace(realtimeHost)) throw new InvalidOperationException("realtimeHost must not be blank");
            if (realtimePort < 1 || realtimePort > 65535) throw new InvalidOperationException("realtimePort must be between 1 and 65535");
            if (protocolVersion == 0) throw new InvalidOperationException("protocolVersion must be positive");
            if (gamedataVersion == 0) throw new InvalidOperationException("gamedataVersion must be positive");
            if (string.IsNullOrWhiteSpace(clientVersion)) throw new InvalidOperationException("clientVersion must not be blank");
            if (string.IsNullOrWhiteSpace(platform)) throw new InvalidOperationException("platform must not be blank");
            if (string.IsNullOrWhiteSpace(locale)) throw new InvalidOperationException("locale must not be blank");
        }

        public static ClientRuntimeConfig Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Configuration JSON must not be blank", nameof(json));
            var config = JsonUtility.FromJson<ClientRuntimeConfig>(json);
            if (config == null) throw new InvalidOperationException("Configuration JSON could not be parsed");
            config.Validate();
            return config;
        }

        public static ClientRuntimeConfig LoadStreamingAssets()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "linhgioi-client.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing Linh Gioi runtime configuration", path);
            return Parse(File.ReadAllText(path));
        }
    }
}
