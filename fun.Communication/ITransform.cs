using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Communication
{
    public interface ITransform
    {
        Vector3 Position { get; }
        Vector3 Scale { get; }
        Vector3 Rotation { get; }
    }
}
