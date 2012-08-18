using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Drawing;
using System.Drawing.Imaging;

namespace Hailstone
{
    /// <summary>
    /// Contains functions for manipulating textures.
    /// </summary>
    public static class Texture
    {
        /// <summary>
        /// Creates an unfilled texture.
        /// </summary>
        public static int Create()
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexEnv(TextureEnvTarget.TextureEnv,
                TextureEnvParameter.TextureEnvMode,
                (float)TextureEnvMode.Modulate);
            return id;
        }

        /// <summary>
        /// Creates an alpha texture from the given bitmap.
        /// </summary>
        public static unsafe int CreateAlpha(Bitmap Bitmap)
        {
            BitmapData bd = Bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] data = new byte[Bitmap.Width * Bitmap.Height];
            byte* row = (byte*)bd.Scan0.ToPointer();
            int dest = 0;
            for (int y = 0; y < Bitmap.Height; y++)
            {
                for (int x = 0; x < Bitmap.Width; x++)
                {
                    data[dest] = row[x * 4];
                    dest++;
                }
                row += bd.Stride;
            }

            Bitmap.UnlockBits(bd);

            int tex = Texture.Create();
            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Alpha8, Bitmap.Width, Bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Alpha, PixelType.UnsignedByte, data);
            GL.Ext.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            SetWrapMode(TextureWrapMode.Repeat, TextureWrapMode.Repeat);
            SetFilterMode(TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
            return tex;
        }

        /// <summary>
        /// Binds the given texture.
        /// </summary>
        public static void Bind(int Texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, Texture);
        }

        /// <summary>
        /// Sets the technique used for wrapping the currently-bound texture.
        /// </summary>
        public static void SetWrapMode(TextureWrapMode Horizontal, TextureWrapMode Vertical)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)Horizontal);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)Vertical);
        }

        /// <summary>
        /// Sets the filter mode for the currently-bound texture.
        /// </summary>
        public static void SetFilterMode(TextureMinFilter Min, TextureMagFilter Mag)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Mag);
        }
    }

    /// <summary>
    /// A rendering context that can draw arbitrary points and shapes.
    /// </summary>
    public abstract class Render
    {
        /// <summary>
        /// Draws a vertex with the given properties.
        /// </summary>
        public abstract void Vertex(double X, double Y, double U, double V, Color4 Color);

        /// <summary>
        /// Draws a vertex with the given properties.
        /// </summary>
        public void Vertex(Vector Position, Vector UV, Color4 Color)
        {
            this.Vertex(Position.X, Position.Y, UV.X, UV.Y, Color);
        }

        /// <summary>
        /// Closes the rendering context and sends all given vertices for rendering.
        /// </summary>
        public abstract void Finish();

        /// <summary>
        /// Creates a rendering context with the given beginmode.
        /// </summary>
        public static Render Create(BeginMode Mode)
        {
            return new ImmediateModeRender(Mode);
        }
    }

    /// <summary>
    /// A render context using immediate mode.
    /// </summary>
    public sealed class ImmediateModeRender : Render
    {
        public ImmediateModeRender(BeginMode Mode)
        {
            GL.Begin(Mode);
        }

        public override void Vertex(double X, double Y, double U, double V, Color4 Color)
        {
            GL.Color4(Color);
            GL.TexCoord2(U, V);
            GL.Vertex2(X, Y);
        }

        public override void Finish()
        {
            GL.End();
        }
    }
}
