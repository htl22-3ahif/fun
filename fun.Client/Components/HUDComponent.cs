using fun.Client.Components;
using OpenTK;
using System.Drawing;
using System.Text;
using System.Linq;
using Environment = fun.Core.Environment;
using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using fun.Client.Constructs;
using System.IO;

namespace fun.Client
{
    internal sealed class HUDComponend : GameComponent
    {
        //Resources = http://www.opentk.com/doc/graphics/how-to-render-text-using-opengl

        private int vao;

        private Texture2D fpsTexture;
        private Bitmap bitmap;
        private ShaderProgram program;

        private Font serif = new Font(FontFamily.GenericSerif, 32);
        private Font sans = new Font(FontFamily.GenericSansSerif, 32);
        private Font mono = new Font(FontFamily.GenericMonospace, 32);

        //private bool textRender = true;

        public HUDComponend(GameWindow game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            bitmap = new Bitmap(Game.ClientRectangle.Width, Game.ClientRectangle.Height);
            fpsTexture = new Texture2D(bitmap);
            program = new ShaderProgram(
                new Shader(VertexShader, ShaderType.VertexShader),
                new Shader(FragmentShader, ShaderType.FragmentShader));
            GL.UseProgram(program.ID);

            var points = new Vector3[]
            {
                new Vector3(-(Game.ClientRectangle.Width/2), (Game.ClientRectangle.Height/2), 0),
                new Vector3(-(Game.ClientRectangle.Width/2), -(Game.ClientRectangle.Height/2), 0),
                new Vector3((Game.ClientRectangle.Width/2), -(Game.ClientRectangle.Height/2), 0),
                new Vector3((Game.ClientRectangle.Width/2), (Game.ClientRectangle.Height/2), 0)
            };

            var uvs = new Vector2[]
            {
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(1, 1), new Vector2(1, 0)
            };

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GL.GenBuffer());
            GL.BufferData(BufferTarget.ArrayBuffer, points.Length * Vector3.SizeInBytes, points, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(program.GetAttrib("vPosition").ID, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, GL.GenBuffer());
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * Vector2.SizeInBytes, uvs, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(program.GetAttrib("vUV").ID, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            program.Enable();

            GL.BindVertexArray(0);

            program.GetUniform("proj").SetValue(Matrix4.CreateOrthographic(Game.ClientRectangle.Width, Game.ClientRectangle.Height, 0f, 1000f));
        }

        public override void Update(FrameEventArgs e)
        {

        }

        double time = 0;
        public override void Draw(FrameEventArgs e)
        {
            GL.UseProgram(program.ID);
            time += e.Time;
            if (time > 1)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                    graphics.DrawString("FPS: " + Game.RenderFrequency.ToString("0.00"), sans, Brushes.Honeydew, new PointF(0, 0));

                    GL.BindTexture(TextureTarget.Texture2D, fpsTexture.ID);

                    var data = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    bitmap.UnlockBits(data);

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
                time = 0;
            }
            GL.BindTexture(TextureTarget.Texture2D, fpsTexture.ID);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindVertexArray(0);
        }

        static string VertexShader = @"
#version 400

in vec3 vPosition;
in vec2 vUV;

uniform mat4 proj;

out vec2 uv;

void
main() {
    gl_Position = vec4(vPosition, 1.0) * proj;
    uv = vUV;
}
";

        static string FragmentShader = @"
#version 400

in vec2 uv;

uniform sampler2D texture;

out vec4 fragment;

void
main (){
    fragment = texture2D(texture, uv);
}
";
    }
}