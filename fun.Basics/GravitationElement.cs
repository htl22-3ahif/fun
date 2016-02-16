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
    public sealed class GravitationElement : Element
    {
        private TransformElement transform;
        private CollidingElement colliding;

        public float Weight { get; set; }
        private float fallingTime;

        public GravitationElement(Environment environment, Entity entity) 
            : base(environment, entity)
        {
            transform = entity.GetElement<TransformElement>();
            colliding = entity.GetElement<CollidingElement>();
        }

        public override void Initialize()
        {
            Weight = 80f;
        }

        public override void Update(double time)
        {
            if (colliding.IsCollidingZ)
            {
                fallingTime = 0;
            }
            else
            {
                fallingTime += (float)time;
            }


            float FallingStrength = (9.81f * (float)Math.Pow(fallingTime, 2f) / 2f);

            transform.Position -= new Vector3(0, 0, FallingStrength);
        }
    }
}
