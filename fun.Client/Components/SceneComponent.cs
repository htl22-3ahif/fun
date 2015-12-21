using fun.Client.Constructs;
using fun.Communication;
using ObjLoader.Loader.Loaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.Client.Components
{
    internal sealed class SceneComponent : GameComponent
    {
        private SimulationComponent simulation;
        private CameraComponent camera;

        private Dictionary<string, Mesh> meshes;
        private ShaderProgram program;

        public SceneComponent(GameWindow game, SimulationComponent simulaiton, CameraComponent camera)
            : base(game)
        {
            this.simulation = simulaiton;
            this.camera = camera;

            meshes = new Dictionary<string, Mesh>();
        }

        public override void Initialize()
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            program = new ShaderProgram(
                new Shader(new StreamReader(@"assets\shaders\vs.glsl").ReadToEnd(), ShaderType.VertexShader),
                new Shader(new StreamReader(@"assets\shaders\fs.glsl").ReadToEnd(), ShaderType.FragmentShader));

            foreach (var perceived in simulation.Perceiveder)
            {
                Directory.SetCurrentDirectory("assets\\models");
                if (!File.Exists(perceived.Name))
                    continue;

                if (meshes.Keys.Contains(perceived.Name))
                    continue;

                var objFactory = new ObjLoaderFactory();
                var objloader = objFactory.Create();
                var result = objloader.Load(new FileStream(perceived.Name, FileMode.Open, FileAccess.Read));
                Directory.SetCurrentDirectory("..\\..");

                
                var positions = result.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                var normals = result.Normals.Select(n => new Vector3(n.X, n.Y, n.Z)).ToArray();
                var vertices = new List<VertexPositionColorNormal>();

                foreach (var group in result.Groups)
                    foreach (var face in group.Faces)
                        for (int i = 0; i < face.Count; i++)
                        {
                            var indexPos = face[i].VertexIndex - 1;
                            var indexNor = face[i].NormalIndex - 1;

                            vertices.Add(new VertexPositionColorNormal(positions[indexPos], new Vector4(1f, 1f, 1f, 1.0f), normals[indexNor]));
                        }

                meshes.Add(perceived.Name, new Mesh(program, vertices.ToArray()));
            }
        }

        float rot = 0;
        public override void Draw(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            rot += (float)e.Time / 4;
            program.GetUniform("projection").SetValue(camera.Projection);
            program.GetUniform("view").SetValue(camera.View);
            program.GetUniform("light_direction").SetValue(Vector3.Transform(Vector3.One.Normalized(), Matrix4.CreateRotationZ(rot)));

            foreach (var entity in camera.Seen)
            {
                var name = (entity.GetElement<IPerceived>() as IPerceived).Name;

                if (!meshes.Keys.Contains(name))
                    continue;

                var mesh = meshes[name];
                var transform = entity.GetElement<ITransform>() as ITransform;
                
                var world = Matrix4.CreateScale(transform.Scale) *
                    Matrix4.CreateRotationX(transform.Rotation.X) *
                    Matrix4.CreateRotationY(transform.Rotation.Y) *
                    Matrix4.CreateRotationZ(transform.Rotation.Z) *
                    Matrix4.CreateTranslation(transform.Position);

                mesh.Draw(world);
            }

            GL.Flush();
        }

        private int LoadShader(string file, ShaderType shaderType, int program)
        {
            int id = GL.CreateShader(shaderType);
            using (var sr = new StreamReader(file))
                GL.ShaderSource(id, sr.ReadToEnd());
            GL.CompileShader(id);
            GL.AttachShader(program, id);
            Console.WriteLine(GL.GetShaderInfoLog(id));
            return id;
        }

        private void LoadMatrix(Matrix4 mat) { GL.LoadMatrix(ref mat); }

        private class Mesh
        {
            public readonly int VBO;

            private ShaderProgram program;

            public int VerticesLength { get; private set; }

            public Mesh(ShaderProgram program, VertexPositionColorNormal[] vertices)
            {
                this.program = program;

                //defining VertexBufferObject
                VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (VertexPositionColorNormal.SizeInBytes * vertices.Length), vertices, BufferUsageHint.StaticDraw);

                GL.VertexPointer(3, VertexPointerType.Float, VertexPositionColorNormal.SizeInBytes, 0);
                GL.VertexAttribPointer(program.GetAttrib("vPosition").ID, 3, VertexAttribPointerType.Float, false, VertexPositionColorNormal.SizeInBytes, 0);

                GL.ColorPointer(4, ColorPointerType.Float, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes);
                GL.VertexAttribPointer(program.GetAttrib("vColor").ID, 4, VertexAttribPointerType.Float, true, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes);

                GL.NormalPointer(NormalPointerType.Float, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes + Vector4.SizeInBytes);
                GL.VertexAttribPointer(program.GetAttrib("vNormal").ID, 3, VertexAttribPointerType.Float, true, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes + Vector4.SizeInBytes);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                VerticesLength = vertices.Length;
            }

            public void Draw(Matrix4 world)
            {
                program.GetUniform("world").SetValue(world);

                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.ColorArray);
                GL.EnableClientState(ArrayCap.NormalArray);

                program.Enable();

                GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesLength);

                program.Disable();

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.LoadIdentity();
            }
        }

        private struct VertexPositionColorNormal
        {
            public static readonly int SizeInBytes = (Vector3.SizeInBytes * 2) + Vector4.SizeInBytes;

            public Vector3 Position;
            public Vector4 Color;
            public Vector3 Normal;

            public VertexPositionColorNormal(Vector3 position, Vector4 color, Vector3 normal)
            {
                Position = position;
                Color = color;
                Normal = normal;
            }
        }
    }
}
