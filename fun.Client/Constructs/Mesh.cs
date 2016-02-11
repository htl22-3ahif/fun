using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Client.Constructs
{
	internal sealed class Mesh : IDisposable
    {
        public readonly int VAO;
        public readonly int POSITION_VBO;
        public readonly int UV_VBO;
        public readonly int NORMAL_VBO;
        //public readonly int INDICES;

        private ShaderProgram program;

        public int VerticesLength { get; set; }

        public Mesh(ShaderProgram program,
            Vector3[] positions,
            Vector2[] uvs,
            Vector3[] normals)
        {
            this.program = program;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            POSITION_VBO =  GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, POSITION_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (Vector3.SizeInBytes * positions.Length), positions, BufferUsageHint.StaticDraw);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);
            GL.VertexAttribPointer(program.GetAttrib("vPosition").ID, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            UV_VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, UV_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (Vector2.SizeInBytes * uvs.Length), uvs, BufferUsageHint.StaticDraw);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, 0);
            GL.VertexAttribPointer(program.GetAttrib("vUV").ID, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);

            NORMAL_VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, NORMAL_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (Vector3.SizeInBytes * normals.Length), normals, BufferUsageHint.StaticDraw);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, 0);
            GL.VertexAttribPointer(program.GetAttrib("vNormal").ID, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

            VerticesLength = positions.Length;

            //INDICES = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, INDICES);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, (indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
            //IndicesLength = indices.Length;
        }

        public void Draw(Matrix4 world)
        {
            program.GetUniform("world").SetValue(world);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            program.Enable();

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesLength);

            program.Disable();
        }

		public void Dispose() {
			GL.DeleteVertexArray (VAO);
			GL.DeleteBuffer (POSITION_VBO);
			GL.DeleteBuffer (UV_VBO);
			GL.DeleteBuffer (NORMAL_VBO);
		}
    }
}
