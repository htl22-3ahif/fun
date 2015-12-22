using fun.Core;
using System;
using Environment = fun.Core.Environment;
using OpenTK;
using fun.Basics.Shapes;

namespace fun.Basics
{
    public sealed class ColliderBoxElement : Element, ICollider
    {
        private TransformElement transform;
        private Box box;

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
            transform = entity.GetElement<TransformElement>();
        }

        public float? Intersects(Ray ray)
        {
            var tempbox = new Box(
                box.Min + transform.Position,
                box.Max + transform.Position);

            return ray.Intersects(tempbox);
        }
    }
}
