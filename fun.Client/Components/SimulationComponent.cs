﻿using fun.Basics;
using fun.Core;
using fun.IO;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.Client.Components
{
	internal sealed class SimulationComponent : GameComponent, IDisposable
    {
        private InputComponent input;
        private Environment environment;

        public Entity Player { get; private set; }
        public IEnumerable<PerceivedElement> Perceiveder { get; set; }

        public SimulationComponent(GameWindow game, InputComponent input, string env)
            : base(game)
        {
            this.input = input;

            var reader = new EnvironmentXmlReader();
            var writer = new EnvironmentXmlWriter();
            string[] libaries;

            using (var file = new FileStream(env, FileMode.Open, FileAccess.Read))
                environment = reader.Load(file, out libaries)[0];

            //using (var file = new FileStream("environment.xml", FileMode.Create, FileAccess.Write))
            //    writer.Save(file, environment, "fun.Basics.dll");

            Player = environment.Entities.First(e => e.Elements.Any(el => el.GetType().Name == "PerceiverElement"));
            Perceiveder = environment.Entities.Where(e => e.ContainsElement<PerceivedElement>()).Select(e => e.GetElement<PerceivedElement>());
        }

        public override void Initialize()
        {
            environment.Initialize();
        }

        public override void Update(FrameEventArgs e)
        {
            foreach (var input in environment.Entities
                .Where(en => en.ContainsElement<InputElement>())
                .Select(en => en.GetElement<InputElement>()))
            {
                input.Keys = this.input.Keyboard.DownKeys;
                //input.MouseDelta = this.input.Mouse.Delta;
            }

            environment.Update(e.Time);
        }

		public void Dispose() {
			foreach (var entity in environment.Entities) {
				foreach (var element in entity.Elements) {
					element.OnClose ();
				}
			}
		}
    }
}
