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

        static private int pgmID;
        static private int vsID;
        static private int fsID;

        static private int att_vcol;
        static private int att_vpos;
        static private int att_vnor;
        static private int uni_world;
        static private int uni_view;
        static private int uni_projection;
        static private int uni_light_direction;

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

            pgmID = GL.CreateProgram();

            vsID = LoadShader(@"assets\shaders\vs.glsl", ShaderType.VertexShader, pgmID);
            fsID = LoadShader(@"assets\shaders\fs.glsl", ShaderType.FragmentShader, pgmID);

            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            att_vpos = GL.GetAttribLocation(pgmID, "vPosition");
            att_vcol = GL.GetAttribLocation(pgmID, "vColor");
            att_vnor = GL.GetAttribLocation(pgmID, "vNormal");
            uni_world = GL.GetUniformLocation(pgmID, "world");
            uni_view = GL.GetUniformLocation(pgmID, "view");
            uni_projection = GL.GetUniformLocation(pgmID, "projection");
            uni_light_direction = GL.GetUniformLocation(pgmID, "light_direction");

            if (att_vpos == -1 || att_vcol == -1 || att_vnor == -1 ||
                uni_world == -1 || uni_view == -1 || uni_projection == -1 ||
                uni_light_direction == -1)
                Console.WriteLine("Error binding attributes");

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

                            vertices.Add(new VertexPositionColorNormal(positions[indexPos], new Vector4(0.5f, 0.5f, 0.5f, 1.0f), normals[indexNor]));
                        }

                meshes.Add(perceived.Name, new Mesh(vertices.ToArray()));
            }

            GL.UseProgram(pgmID);
        }
        
        public override void Draw(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            var projection = camera.Projection;
            var view = camera.View;

            GL.UniformMatrix4(uni_projection, false, ref projection);
            GL.UniformMatrix4(uni_view, false, ref view);

            GL.Uniform3(uni_light_direction, Vector3.One.Normalized());

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
            public int VBO;

            public int VerticesLength { get; private set; }

            public Mesh(VertexPositionColorNormal[] vertices)
            {
                //defining VertexBufferObject
                VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (VertexPositionColorNormal.SizeInBytes * vertices.Length), vertices, BufferUsageHint.StaticDraw);

                GL.VertexPointer(3, VertexPointerType.Float, VertexPositionColorNormal.SizeInBytes, 0);
                GL.VertexAttribPointer(att_vpos, 3, VertexAttribPointerType.Float, false, VertexPositionColorNormal.SizeInBytes, 0);

                GL.ColorPointer(4, ColorPointerType.Float, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes);
                GL.VertexAttribPointer(att_vcol, 4, VertexAttribPointerType.Float, true, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes);

                GL.NormalPointer(NormalPointerType.Float, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes + Vector4.SizeInBytes);
                GL.VertexAttribPointer(att_vnor, 3, VertexAttribPointerType.Float, true, VertexPositionColorNormal.SizeInBytes, Vector3.SizeInBytes + Vector4.SizeInBytes);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                VerticesLength = vertices.Length;
            }

            public void Draw(Matrix4 world)
            {
                GL.UniformMatrix4(uni_world, false, ref world);

                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.ColorArray);
                GL.EnableClientState(ArrayCap.NormalArray);

                GL.EnableVertexAttribArray(att_vpos);
                GL.EnableVertexAttribArray(att_vnor);
                GL.EnableVertexAttribArray(att_vcol);

                GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesLength);

                GL.DisableVertexAttribArray(att_vpos);
                GL.DisableVertexAttribArray(att_vnor);
                GL.DisableVertexAttribArray(att_vcol);

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
