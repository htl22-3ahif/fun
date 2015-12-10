using fun.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Basics.Skripts
{
    public sealed class MoveActorOnInputScriptElement : Element
    {
        private readonly InputElement input;
        private readonly TransformElement transform;

        public MoveActorOnInputScriptElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            if (!entity.ContainsElement<InputElement>())
                throw new NotSupportedException();

            if (!entity.ContainsElement<TransformElement>())
                throw new NotSupportedException();

            input = entity.GetElement<InputElement>() as InputElement;
            transform = entity.GetElement<TransformElement>() as TransformElement;
        }

        public override void Update(GameTime gameTime)
        {
            Matrix rotation = Matrix.CreateRotationX(transform.Rotation.X) *
                Matrix.CreateRotationZ(transform.Rotation.Z);

            var Right = Vector3.Transform(Vector3.UnitX, rotation);
            var Forward = Vector3.Transform(Vector3.UnitY, rotation);
            var Up = Vector3.Transform(Vector3.UnitZ, rotation);

            transform.Position += input.GetKeyDown(Keys.W) ? Forward * 0.1f : Vector3.Zero;
            transform.Position -= input.GetKeyDown(Keys.S) ? Forward * 0.1f : Vector3.Zero;

            transform.Position += input.GetKeyDown(Keys.D) ? Right * 0.1f : Vector3.Zero;
            transform.Position -= input.GetKeyDown(Keys.A) ? Right * 0.1f : Vector3.Zero;

            transform.Position += input.GetKeyDown(Keys.Space) ? Up * 0.1f : Vector3.Zero;
            transform.Position -= input.GetKeyDown(Keys.LeftShift) ? Up * 0.1f : Vector3.Zero;
        }
    }
}
