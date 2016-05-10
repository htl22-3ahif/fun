using fun.Core;
using Environment = fun.Core.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using fun.Basics.Shapes;

namespace fun.Basics
{
    public sealed class RigidbodyElement : Element
    {
        private const float GAP = 0.0001f;
        private const float RADIUS = 1000;

        private TransformElement transform;
        private List<ICollider> colliders;

        public float Mass;
        public float Speed;
        public float JumpPower;

        public bool IsCollidingZ { get; private set; }

        public Vector3 VelocityDirection { get; set; }
        public bool Jump { get; set; }
        public Vector3 Velocity { get; private set; }

        public RigidbodyElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            var elements = new List<Element>();
            foreach (var entity in Environment.Entities)
                elements.AddRange(entity.Elements);
            colliders = new List<ICollider>(elements
                .Where(e => e is ICollider)
                .Select(e => e as ICollider));

            transform = Entity.GetElement<TransformElement>();
        }

        public override void Update(double time)
        {
            var gravity = new Vector3(0, 0, -9.81f * (Mass / RADIUS));
            var velmove = VelocityDirection * Speed;
            var friction = new Vector3(.1f, .1f, .1f);
            var jump = new Vector3();

            if (Jump && IsCollidingZ)
                jump.Z = JumpPower;

            Velocity += (gravity + velmove + jump) * (float)time;
            Velocity -= Velocity * friction;

            ICollider colliding = null;
            IsCollidingZ = false;
            do
            {
                var ray = new Ray(transform.Position, Vector3.Normalize(Velocity));

                colliding = colliders.FirstOrDefault(c =>
                {
                    var distance = c.Intersects(ray);
                    return distance.HasValue && Velocity.LengthSquared > (distance.Value * distance.Value);
                });

                if (colliding != null)
                {
                    // Rays in alle Richtungen initializieren
                    var xyz = new
                    {
                        XP = new Ray(transform.Position, Vector3.UnitX),
                        YP = new Ray(transform.Position, Vector3.UnitY),
                        ZP = new Ray(transform.Position, Vector3.UnitZ),
                        XM = new Ray(transform.Position, -Vector3.UnitX),
                        YM = new Ray(transform.Position, -Vector3.UnitY),
                        ZM = new Ray(transform.Position, -Vector3.UnitZ)
                    };

                    // Distances festlegen, falls es welche gibt
                    var distance = new
                    {
                        X = colliding.Intersects(Velocity.X > 0 ? xyz.XP : xyz.XM),
                        Y = colliding.Intersects(Velocity.Y > 0 ? xyz.YP : xyz.YM),
                        Z = colliding.Intersects(Velocity.Z > 0 ? xyz.ZP : xyz.ZM)
                    };

                    // Collisionen prüfen
                    var isColliding = new
                    {
                        X = distance.X.HasValue && Math.Abs(Velocity.X) > distance.X.Value,
                        Y = distance.Y.HasValue && Math.Abs(Velocity.Y) > distance.Y.Value,
                        Z = distance.Z.HasValue && Math.Abs(Velocity.Z) > distance.Z.Value
                    };

                    IsCollidingZ = isColliding.Z;

                    float reduction;
                    var move = Velocity;

                    //Collision verhindern
                    if (isColliding.X)
                    {
                        reduction = distance.X.Value / Math.Abs(Velocity.X);
                        move.X *= reduction - GAP;

                        //if (move.X < GAP && move.X > 0)
                        //    move.X -= GAP;
                        //else if (move.X > -GAP && move.X < 0)
                        //    move.X += GAP;
                    }
                    else if (isColliding.Y)
                    {
                        reduction = distance.Y.Value / Math.Abs(Velocity.Y);
                        move.Y *= reduction - GAP;

                        //if (move.Y < GAP && move.Y > 0)
                        //    move.Y -= GAP;
                        //else if (move.Y > -GAP && move.Y < 0)
                        //    move.Y += GAP;
                    }
                    else if (isColliding.Z)
                    {
                        reduction = distance.Z.Value / Math.Abs(Velocity.Z);
                        move.Z *= reduction - GAP;
                    }

                    Velocity = move;
                }
            } while (colliding != null);

            transform.Position += Velocity;
            if (IsCollidingZ)
                Velocity = new Vector3(Velocity.X, Velocity.Y, 0);
        }
    }
}
