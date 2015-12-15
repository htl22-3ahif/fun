using fun.Core;
using System;
using System.Collections.Generic;

namespace fun.Communication
{
    public interface IPerceiver
    {
        IEnumerable<Entity> Seen { get; }
    }
}
