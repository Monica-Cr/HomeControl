using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Application.InterFaces
{
    public interface IHomeAssistantWebsocketClient
    {
        Task ConnectAndAuthenticteAsync();
        Task SendMessageAsync(object message);
        Task<string> ReceiveFullMessageAsync(ClientWebSocket ws);

        Task<string> GetDevicesJsonAsync();
        Task<string> GetEntitiesJsonAsync();
    }
}
