using fun.Core;
using System;
using System.Collections.Generic;
using Environment = fun.Core.Environment;
using fun.Communication;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace fun.Client.Components
{
    internal sealed class CameraComponent : GameComponent
    {
        private Entity player;
        private InputComponent input;
        private Vector3 rotation;

        public IEnumerable<Entity> Seen { get { return (player.GetElement<IPerceiver>() as IPerceiver).Seen; } }
        public float FieldOfView { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public float Distance { get; set; }

        public Matrix4 View { get; private set; }
        public Matrix4 Projection { get; private set; }

        public CameraComponent(GameWindow game, InputComponent input, SimulationComponent simulation)
            : base(game)
        {
            this.player = simulation.Player;
            this.input = input;

            rotation = Vector3.Zero;
            FieldOfView = MathHelper.PiOver4;
            NearPlaneDistance = .1f;
            FarPlaneDistance = 1000f;
            Distance = 5;
        }

        public override void Update(FrameEventArgs e)
        {
            var transform = player.GetElement<ITransform>() as ITransform;

            rotation += new Vector3(input.Mouse.Delta.Y / 100f, 0f, input.Mouse.Delta.X / 100f);

            Matrix4 rotationMatrix = Matrix4.CreateRotationX(rotation.X) *
                Matrix4.CreateRotationZ(rotation.Z);

            var forward = Vector3.Transform(Vector3.UnitY, rotationMatrix);
            var up = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            var right = Vector3.Transform(Vector3.UnitX, rotationMatrix);

            if (input.Keyboard.GetKeyDown(Key.W) ||
                input.Keyboard.GetKeyDown(Key.A) ||
                input.Keyboard.GetKeyDown(Key.S) ||
                input.Keyboard.GetKeyDown(Key.D))
                (player.GetElement<IInput>() as IInput).Content = new Vector3(0, 0, rotation.Z);

            //View = Matrix4.LookAt(
            //    transform.Position - (forward * Distance), forward + (transform.Position - (forward * Distance)), up);
            View = Matrix4.LookAt(
                transform.Position, forward + transform.Position, up);

            Projection = Matrix4.CreatePerspectiveFieldOfView(
                FieldOfView, (Game.Width / (float)Game.Height), NearPlaneDistance, FarPlaneDistance);
        }
    }
}
