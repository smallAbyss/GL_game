using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;


namespace OpenGL_2
{
    internal class Texture
    {
        private int Handle;
        private static int i = 0;
        public Texture(string tex_path)
        {
            Handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0 + i);
            i++;
            GL.BindTexture(TextureTarget.Texture2D, Handle);


            // stb_image loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            // This will correct that, making the texture display properly.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image.
            ImageResult image = ImageResult.FromStream(File.OpenRead(tex_path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);


            // generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public int GetActiveTextureSocketNumber()
        {
            return i;
        }

    }
}
