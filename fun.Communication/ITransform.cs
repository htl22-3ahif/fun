using OpenTK;
using System;

namespace fun.Communication
{
    public interface ITransform
    {
        Vector3 Position { get; }
        Vector3 Scale { get; }
        Vector3 Rotation { get; }
    }
}
