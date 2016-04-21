using fun.Basics;
using fun.Client.Constructs;
using ObjLoader.Loader.Loaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.Client.Components
{
	internal sealed class SceneComponent : GameComponent, IDisposable
    {
        private SimulationComponent simulation;
        private CameraComponent camera;
        private InputComponent input;

        private Dictionary<string, Mesh> meshes;
        private ShaderProgram[] programs;
        private Texture2D texture;

        private int currentProgram = 0;

        public SceneComponent(GameWindow game, InputComponent input, SimulationComponent simulation, CameraComponent camera)
            : base(game)
        {
            this.simulation = simulation;
            this.camera = camera;
            this.input = input;

            meshes = new Dictionary<string, Mesh>();
        }

        public override void Initialize()
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DepthFunc(DepthFunction.Less);

            texture = new Texture2D(@"assets\textures\violet.png");

            programs = new ShaderProgram[3];

            programs[0] = new ShaderProgram(
                new Shader(new StreamReader(@"assets\shaders\regularVertex.glsl").ReadToEnd(), ShaderType.VertexShader),
                new Shader(new StreamReader(@"assets\shaders\regularFragment.glsl").ReadToEnd(), ShaderType.FragmentShader)
            );
            programs[1] = new ShaderProgram(
                new Shader(new StreamReader(@"assets\shaders\celVertex.glsl").ReadToEnd(), ShaderType.VertexShader),
                new Shader(new StreamReader(@"assets\shaders\celFragment.glsl").ReadToEnd(), ShaderType.FragmentShader)
            );
            programs[2] = new ShaderProgram(
                new Shader(new StreamReader(@"assets\shaders\phongVertex.glsl").ReadToEnd(), ShaderType.VertexShader),
                new Shader(new StreamReader(@"assets\shaders\phongFragment.glsl").ReadToEnd(), ShaderType.FragmentShader)
            );

            //GL.ActiveTexture(TextureUnit.Texture0);
            //GL.Uniform1(program.GetUniform("texture").ID, 0);

            foreach (var perceived in simulation.Perceiveder)
            {
                if (!File.Exists("assets\\models\\" + perceived.Name))
                    continue;

                if (meshes.Keys.Contains(perceived.Name))
                    continue;

                Directory.SetCurrentDirectory("assets\\models");
                var objFactory = new ObjLoaderFactory();
                var objloader = objFactory.Create();
                var result = objloader.Load(new FileStream(perceived.Name, FileMode.Open, FileAccess.Read));
                Directory.SetCurrentDirectory("..\\..");

                var positions = result.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                var uvs = result.Textures.Select(t => new Vector2(t.X, t.Y)).ToArray();
                var normals = result.Normals.Select(n => new Vector3(n.X, n.Y, n.Z)).ToArray();

                var _positions = new List<Vector3>();
                var _uvs = new List<Vector2>();
                var _normals = new List<Vector3>();

                foreach (var group in result.Groups)
                    foreach (var face in group.Faces)
                        for (int i = 0; i < face.Count; i++)
                        {
                            var indexPos = face[i].VertexIndex - 1;
                            var indexTex = face[i].TextureIndex - 1;
                            var indexNor = face[i].NormalIndex - 1;

                            _positions.Add(positions[indexPos]);
                            _uvs.Add(uvs[indexTex]);
                            _normals.Add(normals[indexNor]);
                        }

                meshes.Add(perceived.Name,
                    new Mesh(programs[currentProgram], _positions.ToArray(), _uvs.ToArray(), _normals.ToArray()));
            }
        }
        
        public override void Draw(FrameEventArgs e)
        {
            GL.UseProgram(programs[currentProgram].ID);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            programs[currentProgram].GetUniform("projection").SetValue(camera.Projection);
            programs[currentProgram].GetUniform("view").SetValue(camera.View);
            programs[currentProgram].GetUniform("light").SetValue(new Vector3(0, 0, 10));
            programs[currentProgram].GetUniform("range").SetValue(1000f);

            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            foreach (var entity in camera.Seen)
            {
                var name = entity.GetElement<PerceivedElement>().Name;

                if (!meshes.Keys.Contains(name))
                    continue;

                var mesh = meshes[name];
                var transform = entity.GetElement<TransformElement>();
                
                var world = Matrix4.CreateScale(transform.Scale) *
                    Matrix4.CreateRotationX(transform.Rotation.X) *
                    Matrix4.CreateRotationY(transform.Rotation.Y) *
                    Matrix4.CreateRotationZ(transform.Rotation.Z) *
                    Matrix4.CreateTranslation(transform.Position);

                mesh.Draw(world);
            }
        }

        public override void Update(FrameEventArgs e)
        {
            // complex codemess to switch shader on 'P' keypress
            if (input.Keyboard.GetKeyPressed(Key.P))
            {
                if (input.Keyboard.GetKeyDown(Key.AltLeft))
                    if (currentProgram < 1)
                        currentProgram = programs.Length - 2;
                    else
                        currentProgram -= 2;

                if (currentProgram < programs.Length - 1)
                    currentProgram++;
                else
                    currentProgram = 0;
            }
        }

        public void Dispose(){
			foreach (var mesh in meshes.Values)
				mesh.Dispose ();

            foreach (var program in programs)
                program.Dispose();

			texture.Dispose ();
		}
    }
}
