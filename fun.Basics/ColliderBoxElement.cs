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
        public Box[] Boxes { get; set; }

        //public Vector3 Min
        //{
        //    get { return box.Min; }
        //    set { box.Min = value; }
        //}

        //public Vector3 Max
        //{
        //    get { return box.Max; }
        //    set { box.Max = value; }
        //}

        public ColliderBoxElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            transform = entity.GetElement<TransformElement>();
        }

        public float? Intersects(Ray ray)
        {
            float? distance = null;

            foreach (var box in Boxes)
            {
                var tempbox = new Box(
                    (box.Min + transform.Position),
                    (box.Max + transform.Position));
                
                var _distance = ray.Intersects(tempbox);

                if (!distance.HasValue)
                    distance = _distance;
                else if (_distance.HasValue && _distance.Value < distance.Value)
                    distance = _distance;
            }


            return distance;
        }
    }
}
