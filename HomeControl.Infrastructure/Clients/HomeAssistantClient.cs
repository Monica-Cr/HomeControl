using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

namespace HomeControl.Infrastructure.Clients
{
    public class HomeAssistantClient : HttpClient
    {
        private void GetDevces()
        {
            GetAsync("/api/devices");
            throw new NotImplementedException();
        }
    }
}
