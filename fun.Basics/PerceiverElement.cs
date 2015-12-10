using fun.Communication;
using fun.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    /// <summary>
    /// Defines the element of an entity, which is able to draw other seen entities on the monitor.
    /// </summary>
    public sealed class PerceiverElement : Element, IPerceiver
    {
        private readonly TransformElement transform;

        public IEnumerable<Entity> Seen { get; private set; }
        public BoundingSphere BoundingSphere { get; set; }

        /// <summary>
        /// Creates a camera-Object.
        /// </summary>
        /// <param name="environment">environment of element</param>
        /// <param name="entity">entity of element</param>
        public PerceiverElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            if (!entity.ContainsElement<TransformElement>())
                // TODO: write message
                throw new NotImplementedException();

            transform = entity.GetElement<TransformElement>() as TransformElement;

            BoundingSphere = new BoundingSphere(Vector3.Zero, 100f);
        }

        public override void Update(GameTime gameTime)
        {
            Seen = environment.Entities
                .Where(e => 
                    ContainmentType.Contains == BoundingSphere.Contains((e.GetElement<TransformElement>() as TransformElement).Position - transform.Position) && 
                    e.ContainsElement<PerceivedElement>());
        }
    }
}
