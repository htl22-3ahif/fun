using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Communication
{
    public interface IInput
    {
        IEnumerable<Keys> Keys { set; }
        Vector2 MouseDelta { set; }
        Vector3 Content { set; }
    }
}
