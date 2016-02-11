using OpenTK;
using System;

namespace fun.Client.Components
{
	internal abstract class GameComponent
    {
        public GameWindow Game { get; private set; }

        public GameComponent(GameWindow game)
        {
            this.Game = game;
        }

        public virtual void Initialize() { }
        public virtual void Update(FrameEventArgs e) { }
        public virtual void Draw(FrameEventArgs e) { }
    }
}