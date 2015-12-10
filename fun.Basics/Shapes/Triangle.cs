using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Basics.Shapes
{
    public struct Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
            : this()
        {
            A = a;
            B = b;
            C = c;
        }

        public Plane GetPlane()
        {
            return new Plane(A, B, C);
        }

        public bool PointInTriangle(Vector3 P)
        {
            Vector3 A = this.A, B = this.B, C = this.C;
            if (SameSide(P, A, B, C) && SameSide(P, B, A, C) && SameSide(P, C, A, B))
            {
                Vector3 vc1 = Vector3.Cross(Vector3.Subtract(A, B), Vector3.Subtract(A, C));
                if (Math.Abs(Vector3.Dot(Vector3.Subtract(A, P), vc1)) <= .01f)
                    return true;
            }

            return false;
        }

        private bool SameSide(Vector3 p1, Vector3 p2, Vector3 A, Vector3 B)
        {
            Vector3 cp1 = Vector3.Cross(Vector3.Subtract(B, A), Vector3.Subtract(p1, A));
            Vector3 cp2 = Vector3.Cross(Vector3.Subtract(B, A), Vector3.Subtract(p2, A));
            if (Vector3.Dot(cp1, cp2) >= 0) return true;
            return false;
        }
    }
}
