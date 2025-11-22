using HomeControl.Application.Dtos.HomeAssistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Application.InterFaces
{
    public interface IHomeAssistantClient
    {
        Task<string> GetStatesAsync();
        Task<string> GetServicesAsync();
        Task<string> GetEntity(string id);
    }
}
