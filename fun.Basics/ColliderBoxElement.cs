using fun.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    public sealed class ColliderBoxElement : Element, ICollider
    {
        private TransformElement transform;
        private BoundingBox box;

        public Vector3 Min
        {
            get { return box.Min; }
            set { box.Min = value; }
        }

        public Vector3 Max
        {
            get { return box.Max; }
            set { box.Max = value; }
        }


        public ColliderBoxElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            transform = entity.GetElement<TransformElement>() as TransformElement;
        }

        public float? Intersects(Ray ray)
        {
            var tempbox = new BoundingBox(
                box.Min + transform.Position,
                box.Max + transform.Position);

            return ray.Intersects(tempbox);
        }
    }
}
