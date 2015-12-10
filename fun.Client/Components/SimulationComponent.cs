using fun.Basics;
using fun.Basics.Skripts;
using fun.Communication;
using fun.Core;
using fun.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private InputComponent input;
        private Environment environment;

        public Entity Player { get; private set; }
        public IEnumerable<IPerceived> Perceiveder { get; set; }

        public SimulationComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;

            var loader = new EnvironmentLoader();
            environment = loader.Load(new FileStream("environment.xml", FileMode.Open, FileAccess.Read, FileShare.None))[0];
            Player = environment.GetEntity("player");
            Perceiveder = environment.Entities.Where(e => e.ContainsElement<IPerceived>()).Select(e => e.GetElement<IPerceived>() as IPerceived);
        }

        public override void Initialize()
        {
            environment.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var input in environment.Entities
                .Where(e => e.ContainsElement<IInput>())
                .Select(e => e.GetElement<IInput>() as IInput))
            {
                input.Keys = this.input.Keyboard.DownKeys;
                //input.MouseDelta = this.input.Mouse.Delta;
            }

            environment.Update(gameTime);


        }
    }
}
