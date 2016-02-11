using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Client.Constructs
{
	internal sealed class Shader : IDisposable
    {
        public readonly int ID;

        private string source;

        public ShaderType ShaderType { get; private set; }

        public Shader(string source, ShaderType shadertype)
        {
            this.source = source;
            ID = GL.CreateShader(shadertype);
            GL.ShaderSource(ID, source);
            GL.CompileShader(ID);
        }

		public void Dispose(){
			GL.DeleteShader (ID);
		}
    }
}
