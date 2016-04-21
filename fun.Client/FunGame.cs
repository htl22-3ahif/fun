﻿using fun.Client.Components;
using fun.Core;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using Environment = fun.Core.Environment;

namespace fun.Client
{
    internal sealed class FunGame : GameWindow
    {
        private List<GameComponent> components;

        private InputComponent input;
        private SceneComponent scene;
        private CameraComponent camera;
        private SimulationComponent simulation;
        private HUDComponend UI;

        public FunGame(int width, int height, string env)
            : base(width, height)
        {
			Title = "fun";
            WindowBorder = WindowBorder.Hidden;
            //WindowState = WindowState.Fullscreen;
            CursorVisible = false;

            components = new List<GameComponent>();

            input = new InputComponent(this);
            components.Add(input);

            simulation = new SimulationComponent(this, input, env);
            components.Add(simulation);

            camera = new CameraComponent(this, input, simulation);
            components.Add(camera);

            scene = new SceneComponent(this, input, simulation, camera);
            components.Add(scene);

            UI = new HUDComponend(this, input);
            components.Add(UI);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (var component in components)
                component.Initialize();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            foreach (var component in components)
                component.Update(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

			//System.Threading.Thread.Sleep (1000);

            foreach (var component in components)
                component.Draw(e);
			
            SwapBuffers();
        }

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);

			foreach (var component in components) {
				if (component is IDisposable) {
					(component as IDisposable).Dispose ();
				}
			}
		}
    }
}
