using HomeControl.Application.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Infrastructure.Clients
{
    public class HomeAssistantWebsocketClient : IHomeAssistantWebsocketClient
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public HomeAssistantWebsocketClient(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        public async Task<string> GetDevicesJsonAsync()
        {
            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri(_baseUrl), CancellationToken.None);

            var buffer = new byte[32_000];
            var ms = new MemoryStream();

            // 1️⃣ Receive hello/auth_required
            var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            var hello = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"→ {hello}");

            // 2️⃣ Authenticate
            var authMessage = $"{{\"type\":\"auth\",\"access_token\":\"{_token}\"}}";
            await ws.SendAsync(Encoding.UTF8.GetBytes(authMessage), WebSocketMessageType.Text, true, CancellationToken.None);

            // 3️⃣ Receive auth_ok
            result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            var authResponse = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"→ {authResponse}");

            // 4️⃣ Request device list
            var request = "{\"id\":1,\"type\":\"config/entity_registry/list\"}";
            await ws.SendAsync(Encoding.UTF8.GetBytes(request), WebSocketMessageType.Text, true, CancellationToken.None);

            // 5️⃣ Read response in full
            WebSocketReceiveResult recievedResult;
            do
            {
                result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            // 6️⃣ Convert all bytes to JSON string
            var json = Encoding.UTF8.GetString(ms.ToArray());
            return json;
        }
    }
}
