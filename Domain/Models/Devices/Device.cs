using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Domain.Models.Devices
{
    public class Device
    {
        private readonly Guid _deviceId;
        private readonly List<Guid> _groups;
    }
}
