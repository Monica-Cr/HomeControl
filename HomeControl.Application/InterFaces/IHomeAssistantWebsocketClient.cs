using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Application.InterFaces
{
    public interface IHomeAssistantWebsocketClient
    {
        Task<string> GetDevicesJsonAsync();
    }
}
