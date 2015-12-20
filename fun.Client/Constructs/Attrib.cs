using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Client.Constructs
{
    internal struct Attrib
    {
        public readonly int ID;

        public string Name { get; private set; }

        public ActiveAttribType Type { get; private set; }

        public Attrib(ShaderProgram program, string name, ActiveAttribType type)
        {
            ID = GL.GetAttribLocation(program.ID, name);

            if (ID < 0)
                throw new ArgumentException(GL.GetShaderInfoLog(ID));

            Name = name;
            Type = type;
        }

        public void Enable() { GL.EnableVertexAttribArray(ID); }
        public void Disable() { GL.DisableVertexAttribArray(ID); }
    }
}
