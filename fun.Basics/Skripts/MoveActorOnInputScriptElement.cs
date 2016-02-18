using fun.Core;
using OpenTK;
using OpenTK.Input;
using System;
using Environment = fun.Core.Environment;

namespace fun.Basics.Skripts
{
    public sealed class MoveActorOnInputScriptElement : Element
    {
        private InputElement input;
        private GravitationElement gravitation;
        private TransformElement transform;
        private CollidingElement colliding;

        public MoveActorOnInputScriptElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
        }

        public override void Initialize()
        {
            input = Entity.GetElement<InputElement>();
            transform = Entity.GetElement<TransformElement>();
            colliding = Entity.GetElement<CollidingElement>();
            gravitation = Entity.GetElement<GravitationElement>();
        }

        public override void Update(double time)
        {
            var rotation = Matrix4.CreateRotationX(transform.Rotation.X) *
                Matrix4.CreateRotationZ(transform.Rotation.Z);

            var Right = Vector3.Transform(Vector3.UnitX, rotation);
            var Forward = Vector3.Transform(Vector3.UnitY, rotation);
            var Up = Vector3.Transform(Vector3.UnitZ, rotation);

            if (input.GetKeyDown(Key.Space) && colliding.IsCollidingZ)
            {
                transform.Position += new Vector3(0, 0, 0.003f);
                gravitation.Speed = new Vector3(0, 0, 0.2f);
            }

            var moveVector =
                ((input.GetKeyDown(Key.W) ? Forward * (float)time * 5 : Vector3.Zero)
                -(input.GetKeyDown(Key.S) ? Forward * (float)time * 5 : Vector3.Zero)
                +(input.GetKeyDown(Key.D) ? Right * (float)time * 5 : Vector3.Zero )
                -(input.GetKeyDown(Key.A) ? Right * (float)time * 5 : Vector3.Zero))
                * (input.GetKeyDown(Key.LShift) ? new Vector3(3f, 3f, 1) : Vector3.One);

            transform.Position += moveVector;
        }
    }
}
