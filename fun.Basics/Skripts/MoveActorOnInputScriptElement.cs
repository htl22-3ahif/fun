using fun.Core;
using OpenTK;
using OpenTK.Input;
using System;
using Environment = fun.Core.Environment;

namespace fun.Basics.Skripts
{
    [Serializable]
    public sealed class MoveActorOnInputScriptElement : Element
    {
        private InputElement input;
        private GravitationElement gravitation;
        private TransformElement transform;
        private CollidingElement colliding;

        public float Speed { get; set; }

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

            Speed = 10;
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
                (input.GetKeyDown(Key.W) ? Forward : (input.GetKeyDown(Key.S) ? -Forward : Vector3.Zero))
                + (input.GetKeyDown(Key.D) ? Right : (input.GetKeyDown(Key.A) ? -Right : Vector3.Zero));

            if (moveVector != Vector3.Zero)
                moveVector.Normalize();

            moveVector *= Speed;
            moveVector *= (input.GetKeyDown(Key.LShift) ? ((Speed * 0.1f) + 1) : 1);
            moveVector *= (float)time;

            transform.Position += moveVector;
        }
    }
}
