using fun.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fun.Basics;
using Environment = fun.Core.Environment;
using fun.Communication;

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

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public CameraComponent(Game game, InputComponent input, SimulationComponent simulation)
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

        public override void Update(GameTime gameTime)
        {
            var transform = player.GetElement<ITransform>() as ITransform;

            rotation += new Vector3(input.Mouse.Delta.Y / 100f, 0f, input.Mouse.Delta.X / 100f);

            Matrix rotationMatrix = Matrix.CreateRotationX(rotation.X) *
                Matrix.CreateRotationZ(rotation.Z);

            var forward = Vector3.Transform(Vector3.UnitY, rotationMatrix);
            var up = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            var right = Vector3.Transform(Vector3.UnitX, rotationMatrix);

            if (input.Keyboard.GetKeyDown(Keys.W) ||
                input.Keyboard.GetKeyDown(Keys.A) ||
                input.Keyboard.GetKeyDown(Keys.S) ||
                input.Keyboard.GetKeyDown(Keys.D))
                (player.GetElement<IInput>() as IInput).Content = new Vector3(0, 0, rotation.Z);

            View = Matrix.CreateLookAt(
                transform.Position - (forward * Distance), forward + (transform.Position - (forward * Distance)), up);

            Projection = Matrix.CreatePerspectiveFieldOfView(
                FieldOfView, Game.GraphicsDevice.DisplayMode.AspectRatio, NearPlaneDistance, FarPlaneDistance);
        }
    }
}
