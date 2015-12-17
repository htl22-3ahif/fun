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

namespace fun.Client
{
    internal sealed class HUDComponend : GameComponent
    {
        //Resources = http://www.opentk.com/doc/graphics/how-to-render-text-using-opengl

        private Font serif = new Font(FontFamily.GenericSerif, 24);
        private Font sans = new Font(FontFamily.GenericSansSerif, 24);
        private Font mono = new Font(FontFamily.GenericMonospace, 24);

        private bool textRender = true;

        public HUDComponend(GameWindow game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            
        }

        public override void Update(FrameEventArgs e)
        {
            //Update TextDraw
            
        }
    }

    internal sealed class FPSRender
    {
        private GameWindow game;
        public Bitmap text_bmp;
        public Graphics gfx;
        public int text_texture;
        
        public FPSRender(GameWindow game)
        {
            this.game = game;
            gfx = Graphics.FromImage(text_bmp);
        }

        public void Initialize()
        {
            // Create Bitmap and OpenGL texture
            text_bmp = new Bitmap(game.Width, game.Height); // match window size

            text_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, text_texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, text_bmp.Width, text_bmp.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D
        }

        public void Clear()
        {
            gfx.Clear(Color.Transparent);
        }

        public void DrawString(string text, Font font, Brush brush, PointF position)
        {
            gfx.DrawString(text, font, brush, position);
        }

        public void UploadBitmap()
        {
            BitmapData data = text_bmp.LockBits(new Rectangle(0, 0, text_bmp.Width, text_bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            text_bmp.UnlockBits(data);
        }

        public void RenderTexture()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, game.Width, game.Height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Quads);
            //GL.MultiTexCoord1(0f, 1f); GL.Vertex2(0f, 0f);
            //GL.MultiTexCoord1(1f, 1f); GL.Vertex2(1f, 0f);
            //GL.MultiTexCoord1(1f, 0f); GL.Vertex2(1f, 1f);
            //GL.MultiTexCoord1(0f, 0f); GL.Vertex2(0f, 1f);
            GL.End();
        }
    }
}