using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Integration.Wiz
{
    public static class WizUdp
    {
        public const int DevicePort = 38899;
        public const int AppPort = 38900;

        public static async Task SendAsync(string ip, object payload, CancellationToken ct = default)
        {
            using var udp = new UdpClient();
            udp.EnableBroadcast = true;
            var json = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);
            await udp.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse(ip), DevicePort));
        }

        public static async Task<UdpReceiveResult> ReceiveAsync(int port = AppPort, int timeoutMs = 1000, CancellationToken ct = default)
        {
            using var udp = new UdpClient(port);
            udp.Client.ReceiveTimeout = timeoutMs;
            return await udp.ReceiveAsync(ct);
        }

        public static async IAsyncEnumerable<(string ip, string mac, string model)> DiscoverAsync(int listenMs = 1500)
        {
            using var udp = new UdpClient();
            udp.EnableBroadcast = true;
            var probe = Encoding.UTF8.GetBytes("{\"method\":\"getSystemConfig\",\"params\":{}}");
            await udp.SendAsync(probe, probe.Length, new IPEndPoint(IPAddress.Broadcast, WizUdp.DevicePort));

            using var rx = new UdpClient(WizUdp.AppPort);
            rx.Client.ReceiveTimeout = listenMs;

            var start = Environment.TickCount;
            while (Environment.TickCount - start < listenMs)
            {
                var res = await rx.ReceiveAsync();
                var json = JsonDocument.Parse(res.Buffer);
                var ip = res.RemoteEndPoint.Address.ToString();
                var mac = json.RootElement.GetProperty("result").GetProperty("mac").GetString() ?? "";
                var model = json.RootElement.GetProperty("result").GetProperty("moduleName").GetString() ?? "";
                yield return (ip, mac, model);
            }
        }

        public static async Task<JsonDocument?> GetPilotAsync(string ip)
        {
            using var udp = new UdpClient(WizUdp.AppPort);
            udp.EnableBroadcast = true;

            var req = "{\"method\":\"getPilot\",\"params\":{}}";
            var reqBytes = Encoding.UTF8.GetBytes(req);
            await udp.SendAsync(reqBytes, reqBytes.Length, new IPEndPoint(IPAddress.Parse(ip), WizUdp.DevicePort));

            udp.Client.ReceiveTimeout = 1000;
            var res = await udp.ReceiveAsync();
            return JsonDocument.Parse(res.Buffer);
        }

        public static Task TurnOnAsync(string ip) =>
            WizUdp.SendAsync(ip, new { method = "setPilot", @params = new { state = true } });

        public static Task TurnOffAsync(string ip) =>
            WizUdp.SendAsync(ip, new { method = "setPilot", @params = new { state = false } });

        // Brightness: 10–100
        public static Task SetBrightnessAsync(string ip, int brightness) =>
            WizUdp.SendAsync(ip, new { method = "setPilot", @params = new { dimming = Math.Clamp(brightness, 10, 100) } });

        // RGB: 0–255
        public static Task SetRgbAsync(string ip, byte r, byte g, byte b) =>
            WizUdp.SendAsync(ip, new { method = "setPilot", @params = new { r, g, b } });

        // White temperature (Kelvin), e.g. 2200–6500 depending on model
        public static Task SetWhiteAsync(string ip, int kelvin, int brightness = 100) =>
            WizUdp.SendAsync(ip, new { method = "setPilot", @params = new { temp = kelvin, dimming = brightness } });


    }
}