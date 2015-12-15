using fun.Communication;
using fun.Core;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.Basics
{
    public sealed class InputElement : Element, IInput
    {
        private Key[] lastKeys;
        private Key[] currKeys;

        public IEnumerable<Key> Keys { get; set; }
        public Vector2 MouseDelta { get; set; }
        public Vector3 Content { get; set; }

        public InputElement(Environment environment, Entity entity)
            : base(environment, entity)
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

        public override void Update(double time)
        {
            if (Keys == null)
                return;
            lastKeys = currKeys;
            currKeys = Keys.ToArray();
        }
    }
}
