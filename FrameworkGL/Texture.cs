using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace FrameworkGL
{
    class Texture
    {
        #region Attributes

        private int textureID;

        #endregion


        #region Properties

        public int ID { get { return textureID; } }

        public static implicit operator int(Texture texture) { return texture.ID; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        #endregion


        #region Methods

        public Texture(string textureFilePath,
            TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear,
            TextureWrapMode wrapModeS = TextureWrapMode.ClampToBorder, TextureWrapMode wrapModeT = TextureWrapMode.ClampToBorder) {
            if (String.IsNullOrEmpty(textureFilePath))
                throw new ArgumentException("String was null or empty.", textureFilePath);

            if (!File.Exists(textureFilePath))
                throw new ArgumentException("The file specified does not exist.", textureFilePath);

            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            Bitmap bmp = new Bitmap(textureFilePath);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Width = bmpData.Width;
            Height = bmpData.Height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            bmp.UnlockBits(bmpData);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapModeS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapModeT);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public void Unbind() {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #endregion
    }
}
