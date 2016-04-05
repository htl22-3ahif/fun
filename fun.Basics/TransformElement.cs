using fun.Core;
using OpenTK;
using System;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    /// <summary>
    /// Describes basic informations of an entity.
    /// </summary>
    [Serializable]
    public sealed class TransformElement : Element
    {
        /// <summary>
        /// Describes the position of an entity.
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Describes the scale of an entity.
        /// </summary>
        public Vector3 Scale { get; set; }
        /// <summary>
        /// Describes the rotation of an entity.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Creates a transform-Object.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="entity"></param>
        public TransformElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            this.Position = Vector3.Zero;
            this.Rotation = Vector3.Zero;
            this.Scale = Vector3.One;
        }
    }
}
