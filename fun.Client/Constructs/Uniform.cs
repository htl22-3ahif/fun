using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Client.Constructs
{
    internal struct Uniform
    {
        public readonly int ID;

        public string Name { get; private set; }

        public ActiveUniformType Type { get; private set; }

        public Uniform(ShaderProgram program, string name, ActiveUniformType type)
        {
            Name = name;
            Type = type;

            ID = GL.GetUniformLocation(program.ID, name);
        }

        public void SetValue(float v0) { GL.Uniform1(ID, v0); }
        public void SetValue(float v0, float v1) { GL.Uniform2(ID, v0, v1); }
        public void SetValue(Vector2 vec) { GL.Uniform2(ID, vec); }
        public void SetValue(float v0, float v1, float v2) { GL.Uniform3(ID, v0, v1, v2); }
        public void SetValue(Vector3 vec) { GL.Uniform3(ID, vec); }
        public void SetValue(float v0, float v1, float v2, float v3) { GL.Uniform4(ID, v0, v1, v2, v3); }
        public void SetValue(Vector4 vec) { GL.Uniform4(ID, vec); }
        public void SetValue(Matrix4 mat) { GL.UniformMatrix4(ID, false, ref mat); }
    }
}
