using fun.Core;
using Environment = fun.Core.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace fun.Basics
{
    [Serializable]
    public sealed class GravitationElement : Element
    {
        private TransformElement transform;
        private CollidingElement colliding;

        public float Weight;// { get; set; }

        public Vector3 Speed;
        public Vector3 Gravity;

        public GravitationElement(Environment environment, Entity entity) 
            : base(environment, entity)
        {
			
        }

        public override void Initialize()
        {
			transform = Entity.GetElement<TransformElement>();
			colliding = Entity.GetElement<CollidingElement>();
         
			Weight = 80f;

            Speed = Vector3.Zero;
            Gravity = new Vector3(0, 0, -9.81f);
        }

        public override void Update(double time)
        {
            Speed += Gravity * 0.0005f; // would be good to take elapsed time into account here!

            transform.Position += Speed;
        }
    }
}
