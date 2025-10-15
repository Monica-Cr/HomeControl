using HomeControl.Domain.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Domain.Models
{
    internal class Mood
    {
        private readonly Guid _moodId;
        private readonly string _name;
        private readonly List<Guid> _devices;
        private readonly List<Guid> _presets;

        private readonly TimeOnly? _duration;
        private readonly Trigger? _trigger;

    }
}
