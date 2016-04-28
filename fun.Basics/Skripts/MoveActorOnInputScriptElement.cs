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
        private RigidbodyElement rigidbody;

        public float Speed { get; set; }

        public MoveActorOnInputScriptElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
        }

        public override void Initialize()
        {
            input = Entity.GetElement<InputElement>();
            transform = Entity.GetElement<TransformElement>();
            //colliding = Entity.GetElement<CollidingElement>();
            //gravitation = Entity.GetElement<GravitationElement>();
            rigidbody = Entity.GetElement<RigidbodyElement>();

            Speed = 10;
        }

        public override void Update(double time)
        {
            var rotation = Matrix4.CreateRotationZ(transform.Rotation.Z);

            var Right = Vector3.Transform(Vector3.UnitX, rotation);
            var Forward = Vector3.Transform(Vector3.UnitY, rotation);
            var Up = Vector3.Transform(Vector3.UnitZ, rotation);

            var moveVector =
                (input.GetKeyDown(Key.W) ? Forward : (input.GetKeyDown(Key.S) ? -Forward : Vector3.Zero))
                + (input.GetKeyDown(Key.D) ? Right : (input.GetKeyDown(Key.A) ? -Right : Vector3.Zero));

            rigidbody.Jump = input.GetKeyDown(Key.Space);
            rigidbody.VelocityDirection = moveVector;
            return;

            moveVector *= Speed;
            moveVector *= (input.GetKeyDown(Key.LShift) ? ((Speed * 0.1f) + 1) : 1);
            moveVector *= (float)time;

            transform.Position += moveVector;
        }
    }
}
