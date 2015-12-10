using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace fun.Communication
{
    public interface IInput
    {
        IEnumerable<Key> Keys { set; }
        Vector2 MouseDelta { set; }
        Vector3 Content { set; }
    }
}
