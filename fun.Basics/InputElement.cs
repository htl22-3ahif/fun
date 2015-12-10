using fun.Communication;
using fun.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    public sealed class InputElement : Element, IInput
    {
        private Keys[] lastKeys;
        private Keys[] currKeys;

        public IEnumerable<Keys> Keys { get; set; }
        public Vector2 MouseDelta { get; set; }
        public Vector3 Content { get; set; }

        public InputElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            currKeys = new Keys[0];
            lastKeys = new Keys[0];
        }

        public bool GetKeyPressed(Keys key)
        {
            return !lastKeys.Contains(key) && currKeys.Contains(key);
        }

        public bool GetKeyDown(Keys key)
        {
            return currKeys.Contains(key);
        }
        
        public bool GetKeyRelease(Keys key)
        {
            return lastKeys.Contains(key) && !currKeys.Contains(key);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keys == null)
                return;
            lastKeys = currKeys;
            currKeys = Keys.ToArray();
        }
    }
}
