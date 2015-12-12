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

        public SceneComponent(GameWindow game, SimulationComponent simulaiton, CameraComponent camera)
            : base(game)
        {
            this.simulation = simulaiton;
            this.camera = camera;

            meshes = new Dictionary<string, Mesh>();
        }

        public override void Initialize()
        {
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

                var vertices = result.Vertices.Select(v => new Vector3(new Vector3(v.X, v.Y, v.Z))).ToArray();
                var indiceslist = new List<uint>();

                foreach (var group in result.Groups)
                    foreach (var face in group.Faces)
                        for (int i = 0; i < face.Count; i++)
                            indiceslist.Add((uint)face[i].VertexIndex);

                var indices = indiceslist.Select(i => i - 1).ToArray();

                meshes.Add(perceived.Name, new Mesh(vertices, indices));
            }
        }

        public override void Draw(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Honeydew);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            foreach (var entity in camera.Seen)
            {
                var mesh = meshes[(entity.GetElement<IPerceived>() as IPerceived).Name];
                var transform = entity.GetElement<ITransform>() as ITransform;

                var world = Matrix4.CreateScale(transform.Scale) *
                    Matrix4.CreateRotationX(transform.Rotation.X) *
                    Matrix4.CreateRotationY(transform.Rotation.Y) *
                    Matrix4.CreateRotationZ(transform.Rotation.Z) *
                    Matrix4.CreateTranslation(transform.Position);

                mesh.Draw(world, camera.View, camera.Projection);
            }

            GL.Flush();
        }

        private class Mesh
        {
            public int VBO;
            public int IBO;

            public int IndicesLength { get; private set; }
            public int VerticesLength { get; private set; }

            public Mesh(Vector3[] vertices, uint[] indices)
            {
                //defining VertexBufferObject
                VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (Vector3.SizeInBytes * vertices.Length), vertices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                //defining IndexBufferObject
                IBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (sizeof(uint) * indices.Length), indices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                IndicesLength = indices.Length;
                VerticesLength = vertices.Length;
            }

            public void Draw(Matrix4 world, Matrix4 view, Matrix4 projection)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref world);
                GL.LoadMatrix(ref view);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projection);

                GL.EnableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

                GL.Color3(Color.Black);
                GL.DrawElements(PrimitiveType.Triangles, IndicesLength, DrawElementsType.UnsignedInt, 0);

                GL.LoadIdentity();
            }
        }

        #region old
        //        GraphicsDevice.Clear(Color.Honeydew);

        //            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        //            GraphicsDevice.BlendState = BlendState.Opaque;

        //            GraphicsDevice.RasterizerState = new RasterizerState()
        //        {
        //            CullMode = CullMode.None,
        //                FillMode = FillMode.WireFrame
        //            };

        //        effect.View = camera.View;
        //            effect.Projection = camera.Projection;
        //            effect.VertexColorEnabled = true;

        //            foreach (var entity in camera.Seen)
        //            {
        //                var mesh = entity.GetElement<IPerceived>() as IPerceived;

        //                if (!meshes.Keys.Contains(mesh.Name))
        //                    continue;

        //                var _transform = entity.GetElement<ITransform>() as ITransform;
        //        var part = meshes[mesh.Name];

        //        effect.World =
        //                    Matrix.CreateScale(_transform.Scale) *
        //                    Matrix.CreateRotationX(_transform.Rotation.X) * Matrix.CreateRotationY(_transform.Rotation.Y) * Matrix.CreateRotationZ(_transform.Rotation.Z) *
        //                    Matrix.CreateTranslation(_transform.Position);

        //                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
        //                GraphicsDevice.Indices = part.IndexBuffer;

        //                foreach (var pass in effect.CurrentTechnique.Passes)
        //                {
        //                    pass.Apply();
        //                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.VertexBuffer.VertexCount, 0, part.IndexBuffer.IndexCount / 3);
        //                }
        //}
        #endregion
    }
}
