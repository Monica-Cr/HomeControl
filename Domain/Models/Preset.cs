using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Domain.Models
{
    internal class Preset
    {
        private readonly Guid _presetId;
        private readonly Guid _device;
        private readonly string _settings;
    }
}
