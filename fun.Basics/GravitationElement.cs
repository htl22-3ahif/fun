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

        public float Weight { get; set; }

        public GravitationElement(Environment environment, Entity entity) 
            : base(environment, entity)
        {
            transform = entity.GetElement<TransformElement>();
        }

        public override void Initialize()
        {
            Weight = 80f;
        }

        public override void Update(double time)
        {
            transform.Position -= new Vector3(0, 0, Weight * (float)time * 0.05f);
        }
    }
}
