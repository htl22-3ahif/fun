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

        private Texture2D fpsTexture;
        private Bitmap bitmap;

        private Font serif = new Font(FontFamily.GenericSerif, 24);
        private Font sans = new Font(FontFamily.GenericSansSerif, 24);
        private Font mono = new Font(FontFamily.GenericMonospace, 24);

        //private bool textRender = true;

        public HUDComponend(GameWindow game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            bitmap = new Bitmap(Game.Width / 4, Game.Height / 4);
        }

        public override void Update(FrameEventArgs e)
        {

        }

        public override void Draw(FrameEventArgs e)
        { 
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.DrawString("FPS: " + Game.RenderFrequency, mono, Brushes.Tomato, new PointF(0, 0));
            }
            fpsTexture = new Texture2D(bitmap);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Game.Width, Game.Height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BindTexture(TextureTarget.Texture2D, fpsTexture.ID);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(1f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 1f);
            GL.End();
        }
    }
}