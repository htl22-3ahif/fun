using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Basics.Shapes
{
    public struct Sphere
    {
        public Vector3 Center;
        public float Radius;

        public Sphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public bool Contains(Vector3 point)
        {
            float sqRadius = Radius * Radius;
            float sqDistance =
                (point.X - Center.X) * (point.X - Center.X) +
                (point.Y - Center.Y) * (point.Y - Center.Y) +
                (point.Z - Center.Z) * (point.Z - Center.Z);

            return sqDistance < sqRadius;
        }
    }
}
