using fun.Core;
using System;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    [Serializable]
    public sealed class PerceivedElement : Element
    {
        public string Name;// { get; set; }

        //public VertexPositionNormalTexture 
        public PerceivedElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }
    }
}
