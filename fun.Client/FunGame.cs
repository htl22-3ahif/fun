using fun.Client.Components;
using fun.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ObjLoader.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Client
{
    internal sealed class FunGame : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        private InputComponent input;
        private SceneComponent scene;
        private CameraComponent camera;
        private SimulationComponent simulation;

        public FunGame()
            : base()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferHeight = 720;
            graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            this.IsMouseVisible = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 10);

            input = new InputComponent(this);
            input.UpdateOrder = 1;
            Components.Add(input);

            simulation = new SimulationComponent(this, input);
            simulation.UpdateOrder = 2;
            Components.Add(simulation);

            camera = new CameraComponent(this, input, simulation);
            camera.UpdateOrder = 3;
            Components.Add(camera);

            scene = new SceneComponent(this, simulation, camera);
            scene.DrawOrder = 1;
            scene.UpdateOrder = 4;
            Components.Add(scene);
        }
    }
}
