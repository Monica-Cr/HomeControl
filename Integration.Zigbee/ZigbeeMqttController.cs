using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Integration.Zigbee
{
    public sealed class ZigbeeMqttController : IAsyncDisposable
    {
        private readonly IManagedMqttClient _client;
        private readonly JsonSerializerOptions _json;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<JsonElement>> _pending =
            new(StringComparer.OrdinalIgnoreCase);

        public string MqttClientId { get; }
        public string BrokerHost { get; }
        public int BrokerPort { get; }
        public string? Username { get; }
        public string? Password { get; }
        public bool UseTls { get; }
        public string Z2MBase { get; }

        public bool IsConnected => _client?.IsConnected ?? false;

        public event Func<MqttApplicationMessageReceivedEventArgs, Task>? MessageReceived;

        public ZigbeeMqttController(
            string brokerHost,
            int brokerPort = 1883,
            string? username = null,
            string? password = null,
            bool useTls = false,
            string z2mBase = "zigbee2mqtt",
            string? clientId = null)
        {
            BrokerHost = brokerHost;
            BrokerPort = brokerPort;
            Username = username;
            Password = password;
            UseTls = useTls;
            Z2MBase = z2mBase.TrimEnd('/');
            MqttClientId = clientId ?? $"zigbeectl-{Environment.MachineName}-{Guid.NewGuid():N}".Substring(0, 30);

            _json = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _client = new MqttFactory().CreateManagedMqttClient();

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage?.Topic ?? string.Empty;

                if (_pending.TryRemove(topic, out var tcs))
                {
                    try
                    {
                        var payload = e.ApplicationMessage?.Payload;
                        var doc = JsonDocument.Parse(payload ?? Array.Empty<byte>());
                        tcs.TrySetResult(doc.RootElement.Clone());
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }

                if (MessageReceived is not null)
                    await MessageReceived.Invoke(e);
            };
        }

        #region Connect / Disconnect

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            var builder = new MqttClientOptionsBuilder()
                .WithClientId(MqttClientId)
                .WithTcpServer(BrokerHost, BrokerPort)
                .WithCleanSession();

            if (!string.IsNullOrEmpty(Username))
                builder = builder.WithCredentials(Username, Password);

            if (UseTls)
                builder = builder.WithTls(); // v4 API

            var clientOptions = builder.Build();

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(clientOptions)
                .WithMaxPendingMessages(1000)
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .Build();

            await _client.StartAsync(managedOptions);

            // Useful default subscriptions
            await SubscribeAsync($"{Z2MBase}/bridge/#", ct);
        }

        public Task DisconnectAsync(CancellationToken ct = default)
            => _client.StopAsync();

        public async ValueTask DisposeAsync()
        {
            await _client.StopAsync();
            _client?.Dispose();
        }

        #endregion

        #region Publish / Subscribe helpers

        public Task SubscribeAsync(string topic, CancellationToken ct = default)
        {
            var filter = new MqttTopicFilterBuilder().WithTopic(topic).Build();
            return _client.SubscribeAsync(new[] { filter });
        }

        public Task UnsubscribeAsync(string topic, CancellationToken ct = default)
            => _client.UnsubscribeAsync(new[] { topic });

        public Task PublishJsonAsync(string topic, object payload, int qos = 0, bool retain = false)
        {
            var json = JsonSerializer.Serialize(payload, _json);
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            return _client.EnqueueAsync(msg);
        }

        public Task PublishRawAsync(string topic, ReadOnlyMemory<byte> payload, int qos = 0, bool retain = false)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload.ToArray())
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            return _client.EnqueueAsync(msg);
        }

        #endregion

        #region Device control (Zigbee2MQTT)

        public Task SetPowerAsync(string device, bool on, int? brightness = null, int? colorTemp = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", new
            {
                state = on ? "ON" : "OFF",
                brightness,
                color_temp = colorTemp
            });

        public Task ToggleAsync(string device, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", new { state = "TOGGLE" });

        public Task SetBrightnessAsync(string device, int brightness, TimeSpan? transition = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", new
            {
                brightness = Math.Clamp(brightness, 0, 254),
                transition = transition?.TotalSeconds
            });

        public Task SetColorTemperatureAsync(string device, int mireds, TimeSpan? transition = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", new
            {
                color_temp = mireds,
                transition = transition?.TotalSeconds
            });

        public Task SetColorXyAsync(string device, double x, double y, int? brightness = null, TimeSpan? transition = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", new
            {
                color = new { x, y },
                brightness,
                transition = transition?.TotalSeconds
            });

        public Task SetColorHsvAsync(string device, int hue, int saturation, int? brightness = null, TimeSpan? transition = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", new
            {
                color = new { h = hue, s = saturation },
                brightness,
                transition = transition?.TotalSeconds
            });

        public Task DeviceSetAsync(string device, object payload, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{device}/set", payload);

        #endregion

        #region Groups & Scenes

        public Task AddToGroupAsync(string device, string groupId, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/group/members/add", new { group = groupId, device });

        public Task RemoveFromGroupAsync(string device, string groupId, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/group/members/remove", new { group = groupId, device });

        public Task SetGroupPowerAsync(string groupId, bool on, int? brightness = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{groupId}/set", new { state = on ? "ON" : "OFF", brightness });

        public Task RecallSceneAsync(string groupId, int sceneId, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/{groupId}/set/scene_recall", new { scene_recall = sceneId });

        #endregion

        #region Permit join, bind/unbind, rename/remove

        public Task PermitJoinAsync(bool enable, int? seconds = null, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/permit_join", new { value = enable, time = seconds });

        public Task BindAsync(string sourceDevice, string target, string cluster, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/device/bind", new { from = sourceDevice, to = target, cluster });

        public Task UnbindAsync(string sourceDevice, string target, string cluster, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/device/unbind", new { from = sourceDevice, to = target, cluster });

        public Task RenameDeviceAsync(string from, string to, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/device/rename", new { from, to });

        public Task RemoveDeviceAsync(string device, CancellationToken ct = default)
            => PublishJsonAsync($"{Z2MBase}/bridge/request/device/remove", new { id = device });

        #endregion

        #region Discovery / info (request-response helpers)

        public Task<JsonElement> GetDevicesAsync(TimeSpan? timeout = null, CancellationToken ct = default)
            => RequestReplyAsync(
                reqTopic: $"{Z2MBase}/bridge/request/devices",
                resTopic: $"{Z2MBase}/bridge/response/devices",
                payload: new { },
                timeout ?? TimeSpan.FromSeconds(5),
                ct);

        public Task<JsonElement> GetNetworkMapAsync(string format = "raw", TimeSpan? timeout = null, CancellationToken ct = default)
            => RequestReplyAsync(
                reqTopic: $"{Z2MBase}/bridge/request/networkmap",
                resTopic: $"{Z2MBase}/bridge/response/networkmap",
                payload: new { type = format },
                timeout ?? TimeSpan.FromSeconds(8),
                ct);

        private async Task<JsonElement> RequestReplyAsync(string reqTopic, string resTopic, object payload, TimeSpan timeout, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<JsonElement>(TaskCreationOptions.RunContinuationsAsynchronously);

            await SubscribeAsync(resTopic, ct);
            _pending[resTopic] = tcs;

            await PublishJsonAsync(reqTopic, payload);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeout);

            using var _ = cts.Token.Register(() =>
            {
                if (_pending.TryRemove(resTopic, out var pendingTcs))
                    pendingTcs.TrySetException(new TimeoutException($"Timeout waiting for '{resTopic}'"));
            });

            return await tcs.Task.ConfigureAwait(false);
        }

        #endregion
    }

    internal static class Payloads
    {
        public static ReadOnlyMemory<byte> Utf8(string s) => Encoding.UTF8.GetBytes(s);
    }
}
