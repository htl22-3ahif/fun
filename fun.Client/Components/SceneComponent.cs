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

        private static float[] light_pos = new float[] { 0.0f, 0.0f, 10.0f, 1.0f };
        private Dictionary<string, Mesh> meshes;

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
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.DepthFunc(DepthFunction.Lequal);

            // Enable Light 0 and set its parameters.
            GL.Light(LightName.Light0, LightParameter.Position, light_pos);
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
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

                            vertices.Add(new VertexPositionColorNormal(positions[indexPos], new Vector4(0, 0, 0, 1.0f), normals[indexNor]));
                        }

                meshes.Add(perceived.Name, new Mesh(vertices.ToArray()));
            }
        }
        
        public override void Draw(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            GL.MatrixMode(MatrixMode.Projection);
            LoadMatrix(camera.Projection);
            GL.MatrixMode(MatrixMode.Modelview);
            LoadMatrix(camera.View);

            GL.Light(LightName.Light0, LightParameter.Position, light_pos);

            foreach (var entity in camera.Seen)
            {
                var name = (entity.GetElement<IPerceived>() as IPerceived).Name;

                if (!meshes.Keys.Contains(name))
                    continue;

                var mesh = meshes[name];
                var transform = entity.GetElement<ITransform>() as ITransform;
                
                var model = Matrix4.CreateScale(transform.Scale) *
                    Matrix4.CreateRotationX(transform.Rotation.X) *
                    Matrix4.CreateRotationY(transform.Rotation.Y) *
                    Matrix4.CreateRotationZ(transform.Rotation.Z) *
                    Matrix4.CreateTranslation(transform.Position);

                mesh.Draw(model * camera.View);
            }

            GL.Flush();
        }

        private void LoadMatrix(Matrix4 mat) { GL.LoadMatrix(ref mat); }

        private class Mesh
        {
            public int VBO;

            public int VerticesLength { get; private set; }

            public Mesh(VertexPositionColorNormal[] vertices)
            {
                //defining VertexBufferObject
                VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (VertexPositionColorNormal.SizeInBytes * vertices.Length), vertices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                VerticesLength = vertices.Length;
            }

            public void Draw(Matrix4 modelview)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref modelview);

                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.ColorArray);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

                GL.VertexPointer(3, VertexPointerType.Float, VertexPositionColorNormal.SizeInBytes, 0);
                GL.ColorPointer(4, ColorPointerType.Float, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes);
                GL.NormalPointer(NormalPointerType.Float, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes + Vector4.SizeInBytes);

                GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesLength);

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
