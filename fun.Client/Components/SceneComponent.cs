using fun.Communication;
using fun.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjLoader.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Client.Components
{
    internal sealed class SceneComponent : DrawableGameComponent
    {
        private Dictionary<string, ModelMeshPart> meshes;
        private BasicEffect effect;

        private SimulationComponent simulation;
        private CameraComponent camera;

        public SceneComponent(Game game, SimulationComponent simulaiton, CameraComponent camera)
            : base(game)
        {
            this.meshes = new Dictionary<string, ModelMeshPart>();
            this.simulation = simulaiton;
            this.camera = camera;
        }

        public override void Initialize()
        {
            effect = new BasicEffect(GraphicsDevice);

            foreach (var perceived in simulation.Perceiveder)
            {
                Directory.SetCurrentDirectory("assets\\models");
                if (!File.Exists(perceived.Name))
                    continue;

                if (meshes.Keys.Any(p => p == perceived.Name))
                    continue;

                var objFactory = new ObjLoaderFactory();
                var objloader = objFactory.Create();
                var result = objloader.Load(new FileStream(perceived.Name, FileMode.Open, FileAccess.Read));
                Directory.SetCurrentDirectory("..\\..\\");

                var vertices = result.Vertices.Select(v => new VertexPositionColor(new Vector3(v.X, v.Y, v.Z), Color.Black)).ToArray();
                var indiceslist = new List<int>();

                foreach (var group in result.Groups)
                    foreach (var face in group.Faces)
                        for (int i = 0; i < face.Count; i++)
                            indiceslist.Add(face[i].VertexIndex);

                var indices = indiceslist.Select(i => i - 1).ToArray();

                var part = new ModelMeshPart();

                part.VertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.None);
                part.IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.None);
                part.VertexBuffer.SetData(vertices);
                part.IndexBuffer.SetData(indices);

                meshes.Add(perceived.Name, part);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Honeydew);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame
            };

            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.VertexColorEnabled = true;

            foreach (var entity in camera.Seen)
            {
                var mesh = entity.GetElement<IPerceived>() as IPerceived;

                if (!meshes.Keys.Contains(mesh.Name))
                    continue;

                var _transform = entity.GetElement<ITransform>() as ITransform;
                var part = meshes[mesh.Name];

                effect.World =
                    Matrix.CreateScale(_transform.Scale) *
                    Matrix.CreateRotationX(_transform.Rotation.X) * Matrix.CreateRotationY(_transform.Rotation.Y) * Matrix.CreateRotationZ(_transform.Rotation.Z) *
                    Matrix.CreateTranslation(_transform.Position);

                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                GraphicsDevice.Indices = part.IndexBuffer;

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.VertexBuffer.VertexCount, 0, part.IndexBuffer.IndexCount / 3);
                }
            }
        }
    }
}
