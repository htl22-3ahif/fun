using fun.Core;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    public sealed class PerceivedElement : Element
    {
        public string Name { get; set; }

        //public VertexPositionNormalTexture 
        public PerceivedElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }
    }
}
