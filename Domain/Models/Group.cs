using HomeControl.Domain.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Domain.Models
{
    public class Group
    {
        private readonly Guid _groupId;
        private readonly string _name;
        private readonly List<Device> _devices;
    }
}
