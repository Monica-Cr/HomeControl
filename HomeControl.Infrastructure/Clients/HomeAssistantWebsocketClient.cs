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
        private ClientWebSocket _socket = new ClientWebSocket();
        private readonly string _url;
        private readonly string _token;
        private bool _authenticated;
        private readonly SemaphoreSlim _connectLock = new SemaphoreSlim(1, 1);

        public HomeAssistantWebsocketClient(string baseUrl, string token)
        {
            _url = baseUrl;
            _token = token;
        }

        private void EnsureSocket()
        {
            if (_socket == null ||
                _socket.State == WebSocketState.Closed ||
                _socket.State == WebSocketState.Aborted ||
                _socket.State == WebSocketState.CloseReceived ||
                _socket.State == WebSocketState.CloseSent)
            {
                _socket?.Dispose();
                _socket = new ClientWebSocket();
            }
        }
        public async Task ConnectAndAuthenticteAsync()
        {
            await _connectLock.WaitAsync();
            try
            {
                EnsureSocket();

                if (_socket.State == WebSocketState.Open)
                {
                    return;
                }

                await _socket.ConnectAsync(new Uri(_url), CancellationToken.None);

                var authRequired = await ReceiveFullMessageAsync(_socket);
                Console.WriteLine("WS IN (welcome): " + authRequired);

                if (authRequired.Contains("auth_required"))
                {
                    var authMessage = new
                    {
                        type = "auth",
                        access_token = _token
                    };

                    Console.WriteLine("WS OUT (auth): " + JsonSerializer.Serialize(authMessage));

                    await SendMessageAsync(authMessage);

                    try
                    {
                        var authOk = await ReceiveFullMessageAsync(_socket);
                        Console.WriteLine("WS IN (auth result): " + authOk);

                        if (authOk.Contains("auth_ok"))
                        {
                            _authenticated = true;
                            return;
                        }

                        if (authOk.Contains("auth_invalid") || authOk.Contains("invalid"))
                        {
                            throw new InvalidOperationException("WebSocket authentication failed: " + authOk);
                        }

                        throw new InvalidOperationException("Unexpected auth response: " + authOk);
                    }
                    catch (WebSocketException wex)
                    {
                        var status = _socket.CloseStatus;
                        var desc = _socket.CloseStatusDescription;
                        throw new WebSocketException($"WebSocket closed during auth. CloseStatus={status}, Description='{desc}'. Inner: {wex.Message}", wex);
                    }
                }
                else if (authRequired.Contains("auth_ok"))
                {
                    _authenticated = true;
                    return;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected welcome response: " + authRequired);
                }
            }
            finally
            {
                _connectLock.Release();
            }

            //var authMessage = new 
            //{ 
            //    type = "auth",
            //    access_token = _token
            //};

            //await SendMessageAsync(authMessage);

            //var authOk = await ReceiveFullMessageAsync(_socket);
        }
        public async Task SendMessageAsync(object message)
        {
            if (_socket == null) EnsureSocket();
            if (_socket!.State != WebSocketState.Open)
                throw new InvalidOperationException("WebSocket is not open.");

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(bytes);
            await _socket.SendAsync(segment, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);

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
                {
                    try
                    {
                        if (ws.State == WebSocketState.CloseReceived)
                        {
                            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Ack close", CancellationToken.None);
                        }
                    }
                    catch
                    {
                        // ignore any exceptions during closing attempt
                    }

                    throw new WebSocketException("WebSocket closed");
                }

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
        public async Task<string> GetEntitiesJsonAsync()
        {
            await ConnectAndAuthenticteAsync();
            var request = new
            {
                id = 2,
                type = "get_states"
            };

            await SendMessageAsync(request);

            var response = await ReceiveFullMessageAsync(_socket);
            return response;
        }
    }
}
