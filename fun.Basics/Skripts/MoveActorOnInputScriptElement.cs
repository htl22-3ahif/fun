using fun.Core;
using OpenTK;
using OpenTK.Input;
using System;
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

            input = entity.GetElement<InputElement>();
            transform = entity.GetElement<TransformElement>();
        }

        public override void Update(double time)
        {
            var rotation = Matrix4.CreateRotationX(transform.Rotation.X) *
                Matrix4.CreateRotationZ(transform.Rotation.Z);

            var Right = Vector3.Transform(Vector3.UnitX, rotation);
            var Forward = Vector3.Transform(Vector3.UnitY, rotation);
            var Up = Vector3.Transform(Vector3.UnitZ, rotation);

            transform.Position += input.GetKeyDown(Key.W) ? Forward * (float)time * 5 : Vector3.Zero;
			transform.Position -= input.GetKeyDown(Key.S) ? Forward * (float)time * 5 : Vector3.Zero;

			transform.Position += input.GetKeyDown(Key.D) ? Right * (float)time * 5 : Vector3.Zero;
			transform.Position -= input.GetKeyDown(Key.A) ? Right * (float)time * 5 : Vector3.Zero;

			transform.Position += input.GetKeyDown(Key.Space) ? Up * (float)time * 10 : Vector3.Zero;
			transform.Position -= input.GetKeyDown(Key.LShift) ? Up * (float)time * 10 : Vector3.Zero;
        }
    }
}
