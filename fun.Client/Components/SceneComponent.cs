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
        private ShaderProgram program;
        private Shader[] shaders;
        private Texture2D texture;

        private bool x = false;

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
            GL.DepthFunc(DepthFunction.Less);

            texture = new Texture2D(@"assets\textures\lel.png");

            shaders = new Shader[10];

            shaders[0] = new Shader(new StreamReader(@"assets\shaders\vs.glsl").ReadToEnd(), ShaderType.VertexShader);
            shaders[1] = new Shader(new StreamReader(@"assets\shaders\fs.glsl").ReadToEnd(), ShaderType.FragmentShader);
            shaders[2] = new Shader(new StreamReader(@"assets\shaders\betavs.glsl").ReadToEnd(), ShaderType.VertexShader);
            shaders[3] = new Shader(new StreamReader(@"assets\shaders\betafs.glsl").ReadToEnd(), ShaderType.FragmentShader);

            program = new ShaderProgram(shaders[0], shaders[1]);

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
                    new Mesh(program, _positions.ToArray(), _uvs.ToArray(), _normals.ToArray()));
            }
        }
        
        public override void Draw(FrameEventArgs e)
        {
            GL.UseProgram(program.ID);
            GL.Viewport(0, 0, Game.Width, Game.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            program.GetUniform("projection").SetValue(camera.Projection);
            program.GetUniform("view").SetValue(camera.View);
            program.GetUniform("light").SetValue(new Vector3(0, 0, 10));
            program.GetUniform("range").SetValue(1000f);
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

            GL.Flush();
        }

        public override void Update(FrameEventArgs e)
        {
            if (input.Keyboard.GetKeyDown(Key.P))
            {
                if (x)
                {
                    program.ChangeShader(shaders[0], shaders[1]);
                    x = false;
                }
                else
                {
                    program.ChangeShader(shaders[2], shaders[3]);
                    x = true;
                }
            }
        }

        public void Dispose(){
			foreach (var mesh in meshes.Values) {
				mesh.Dispose ();
			}

			program.Dispose ();
			texture.Dispose ();
		}
    }
}
