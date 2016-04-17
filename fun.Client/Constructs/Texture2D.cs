using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Client.Constructs
{
    internal struct Texture2D
    {
        public readonly int ID;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture2D(string path)
            : this()
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Filename is null or empty");

            if (!File.Exists(path))
                throw new ArgumentException("File does not exist");

            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            var bitmap = new Bitmap(path);

            Width = bitmap.Width;
            Height = bitmap.Height;

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Texture2D(Bitmap bitmap)
            : this()
        {
            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            Width = bitmap.Width;
            Height = bitmap.Height;

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

		public void Dispose() {
			GL.DeleteTexture (ID);
		}
    }
}
