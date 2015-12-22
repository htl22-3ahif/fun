using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace fun.Client.Constructs
{
    internal sealed class ShaderProgram : IDisposable
    {
        public readonly int ID;

        public Attrib[] Attribs { get; private set; }
        public Uniform[] Uniforms { get; private set; }
        public Shader[] Shaders { get; private set; }

        public ShaderProgram(params Shader[] shaders)
        {
            ID = GL.CreateProgram();
            foreach (var shader in shaders)
                GL.AttachShader(ID, shader.ID);
            GL.LinkProgram(ID);

            Shaders = shaders.ToArray();

            var log = GL.GetProgramInfoLog(ID);
            if (!string.IsNullOrEmpty(log))
                throw new ArgumentException(log);

            InitAttribs();
            InitUnifroms();

            GL.UseProgram(ID);
        }

        public void InitAttribs()
        {
            int attributeCount;
            GL.GetProgramInterface(ID, ProgramInterface.ProgramInput, ProgramInterfaceParameter.ActiveResources, out attributeCount);
            Attribs = new Attrib[attributeCount];

            for (var i = 0; i < attributeCount; i++)
            {
                int length, size;
                ActiveAttribType type;
                var sb = new StringBuilder();
                GL.GetActiveAttrib(ID, i, int.MaxValue, out length, out size, out type, sb);
                Attribs[i] = new Attrib(this, sb.ToString(), type);
            }
        }
        public void InitUnifroms()
        {
            int uniformCount;
            GL.GetProgramInterface(ID, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out uniformCount);
            Uniforms = new Uniform[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                int length, size;
                ActiveUniformType type;
                var sb = new StringBuilder();
                GL.GetActiveUniform(ID, i, int.MaxValue, out length, out size, out type, sb);
                Uniforms[i] = new Uniform(this, sb.ToString(), type);
            }
        }

        public Uniform GetUniform(string name) { return Uniforms.First(u => u.Name == name); }
        public Attrib GetAttrib(string name) { return Attribs.First(a => a.Name == name); }

        public void Enable() { foreach (var attrib in Attribs) attrib.Enable(); }
        public void Disable() { foreach (var attrib in Attribs) attrib.Disable(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
