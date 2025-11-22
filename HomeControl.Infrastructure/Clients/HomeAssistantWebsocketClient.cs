using HomeControl.Application.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomeControl.Infrastructure.Clients
{
    public class HomeAssistantWebsocketClient : IHomeAssistantWebsocketClient
    {
        private readonly ClientWebSocket _socket = new ClientWebSocket();
        private readonly string _url;
        private readonly string _token;

        public HomeAssistantWebsocketClient(string baseUrl, string token)
        {
            _url = baseUrl;
            _token = token;
        }

        public async Task ConnectAndAuthenticteAsync()
        {
            if (_socket.State == WebSocketState.Open)
            {
                return;
            }

            await _socket.ConnectAsync(new Uri(_url), CancellationToken.None);
            
            var authRequired = await ReceiveMessageAsync();
            var authMessage = new 
            { 
                type = "auth",
                access_token = _token
            };

            await SendMessageAsync(authMessage);

            var authOk = await ReceiveMessageAsync();
        }

        public async Task SendMessageAsync(object message)
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var buffer = new byte[4096];
            var result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task<string> ReceiveFullMessageAsync(ClientWebSocket ws)
        {
            var buffer = new byte[4096];
            var result = new ArraySegment<byte>(buffer);
            var message = new StringBuilder();

            while (true)
            {
                var res = await ws.ReceiveAsync(result, CancellationToken.None);

                if (res.MessageType == WebSocketMessageType.Close)
                    throw new Exception("WebSocket closed.");

                message.Append(Encoding.UTF8.GetString(buffer, 0, res.Count));

                if (res.EndOfMessage) break;
            }

            return message.ToString();
        }

        public async Task<string> GetDevicesJsonAsync()
        {
            await ConnectAndAuthenticteAsync();

            var request = new
            {
                id = 1,
                type = "config/device_registry/list"
            };

            await SendMessageAsync(request);

            var response = await ReceiveFullMessageAsync(_socket);
            return response;
        }
    }
}
