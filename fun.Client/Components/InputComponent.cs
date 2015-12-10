using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;

namespace fun.Client.Components
{
    internal sealed class InputComponent : GameComponent
    {
        public MouseInput Mouse { get; private set; }
        public KeyboardInput Keyboard { get; private set; }
        //public GamePadInput Gamepad { get; private set; }

        public InputComponent(GameWindow game)
            : base(game)
        {
            Mouse = new MouseInput(game);
            Keyboard = new KeyboardInput();
        }

        public override void Initialize()
        {
            //OpenTK.Input.Mouse.SetPosition(Game.Width / 2, Game.Height / 2);
        }

        public override void Update(FrameEventArgs e)
        {
            if (Game.Focused)
                Mouse.Update();
            Keyboard.Update();

            if (Keyboard.GetKeyDown(Key.Escape))
                Game.Close();
        }
    }

    internal sealed class MouseInput
    {
        private GameWindow game;
        private List<MouseButton> lastMouseButtons;
        private List<MouseButton> currMouseButtons;

        private MouseState curr;
        private MouseState prev;

        public Vector2 Delta { get; private set; }
        public float MouseSensitivity { get; set; }
        public bool ShouldCenter { get; set; }

        public MouseInput(GameWindow game)
        {
            this.game = game;
            this.ShouldCenter = true;

            lastMouseButtons = new List<MouseButton>();
            currMouseButtons = new List<MouseButton>();

            MouseSensitivity = 0.1f;

            prev = Mouse.GetState();
        }

        public bool GetMouseButtonPressed(MouseButton mouseButton)
        {
            return currMouseButtons.Contains(mouseButton) && !lastMouseButtons.Contains(mouseButton);
        }
        public bool GetMouseButtonDown(MouseButton mouseButton)
        {
            return currMouseButtons.Contains(mouseButton);
        }
        public bool GetMouseButtonRelease(MouseButton mouseButton)
        {
            return !currMouseButtons.Contains(mouseButton) && lastMouseButtons.Contains(mouseButton);
        }

        //public void Update()
        //{
        //    lastMouseButtons = currMouseButtons;
        //    var centreScreen = new Vector2(game.Width / 2, game.Height / 2);

        //    MouseState mouseState = Mouse.GetState();

        //    currMouseButtons = new List<MouseButton>();

        //    if (ShouldCenter)
        //        Mouse.SetPosition(centreScreen.X, centreScreen.Y);

        //    if (!currMouseButtons.Contains(MouseButton.Right) && mouseState.RightButton == ButtonState.Pressed)
        //        currMouseButtons.Add(MouseButton.Right);
        //    if (currMouseButtons.Contains(MouseButton.Right) && mouseState.RightButton == ButtonState.Released)
        //        currMouseButtons.Remove(MouseButton.Right);

        //    if (!currMouseButtons.Contains(MouseButton.Left) && mouseState.LeftButton == ButtonState.Pressed)
        //        currMouseButtons.Add(MouseButton.Left);
        //    if (currMouseButtons.Contains(MouseButton.Left) && mouseState.LeftButton == ButtonState.Released)
        //        currMouseButtons.Remove(MouseButton.Left);

        //    if (!currMouseButtons.Contains(MouseButton.Middle) && mouseState.MiddleButton == ButtonState.Pressed)
        //        currMouseButtons.Add(MouseButton.Middle);
        //    if (currMouseButtons.Contains(MouseButton.Middle) && mouseState.MiddleButton == ButtonState.Released)
        //        currMouseButtons.Remove(MouseButton.Middle);


        //    int deltaX = (int)(centreScreen.X - mouseState.X);
        //    int deltaY = (int)(centreScreen.Y - mouseState.Y);

        //    Delta = new Vector2(mouseState.X * MouseSensitivity, mouseState.Y * MouseSensitivity);//deltaX * MouseSensitivity, deltaY * MouseSensitivity);
        //}

        public void Update()
        {
            curr = Mouse.GetState();
            if (new Vector2(curr.X, curr.Y) != new Vector2(prev.X, prev.Y))
            {
                // Mouse state has changed
                int xdelta = curr.X - prev.X;
                int ydelta = curr.Y - prev.Y;
                int zdelta = curr.Wheel - prev.Wheel;

                Delta = new Vector2(xdelta * MouseSensitivity, ydelta * MouseSensitivity);
            }
            prev = curr;
        }
    }
    internal sealed class KeyboardInput
    {
        private Key[] lastKeys;
        private Key[] currKeys;

        public Key[] DownKeys { get { return currKeys; } }
        public Key[] ReleasedKeys { get { return lastKeys.Where(k => !currKeys.Contains(k)).ToArray(); } }

        public KeyboardInput()
        {
            currKeys = new Key[0];
            lastKeys = new Key[0];
        }

        public bool GetKeyPressed(Key key)
        {
            return !lastKeys.Contains(key) && currKeys.Contains(key);
        }
        public bool GetKeyDown(Key key)
        {
            return currKeys.Contains(key);
        }
        public bool GetKeyRelease(Key key)
        {
            return lastKeys.Contains(key) && !currKeys.Contains(key);
        }

        public void Update()
        {
            lastKeys = currKeys;
            var keyboardState = Keyboard.GetState();
            currKeys = Enum.GetValues(typeof(Key)).OfType<Key>()
                .Where(k => keyboardState.IsKeyDown(k)).ToArray(); ;
        }

    }

    // TODO: Gamepad implementireren
}
