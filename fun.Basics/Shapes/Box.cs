using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Basics.Shapes
{
    [Serializable]
    public struct Box
    {
        public Vector3 Min;
        public Vector3 Max;

        public Box(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}
