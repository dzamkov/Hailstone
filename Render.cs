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

    /// <summary>
    /// Contains information for drawing a shape or figure from a texture.
    /// </summary>
    public struct Shape
    {
        public Shape(Rectangle Source, Rectangle Destination)
        {
            this.Source = Source;
            this.Destination = Destination;
        }

        public Shape(Rectangle Source, Rectangle Destination, double Border)
        {
            this.Source = Source;
            this.Destination = Destination;
            _Expand(ref this.Source, ref this.Destination, Border, Border);
        }

        public Shape(Rectangle Source, Rectangle Destination, double UBorder, double VBorder)
        {
            this.Source = Source;
            this.Destination = Destination;
            _Expand(ref this.Source, ref this.Destination, UBorder, VBorder);
        }

        public Shape(Rectangle Source, double Border)
        {
            double swidth = Source.Right - Source.Left;
            double sheight = Source.Bottom - Source.Top;
            double scale = 1.0 / sheight;

            this.Source = Source;
            this.Destination = new Rectangle(-swidth * scale * 0.5, 0.5, swidth * scale * 0.5, -0.5);
            _Expand(ref this.Source, ref this.Destination, Border, Border);
        }

        /// <summary>
        /// Applies a border to a shape.
        /// </summary>
        private static void _Expand(ref Rectangle Source, ref Rectangle Destination, double UBorder, double VBorder)
        {
            double wborder = UBorder * (Destination.Right - Destination.Left) / (Source.Right - Source.Left);
            double hborder = VBorder * (Destination.Top - Destination.Bottom) / (Source.Bottom - Source.Top);
            Source.Left -= UBorder;
            Source.Top -= VBorder;
            Source.Right += UBorder;
            Source.Bottom += VBorder;
            Destination.Left -= wborder;
            Destination.Top += hborder;
            Destination.Right += wborder;
            Destination.Bottom -= hborder;
        }

        /// <summary>
        /// The source rectangle for the shape, in UV coordinates.
        /// </summary>
        public Rectangle Source;

        /// <summary>
        /// The destination rectangle for the shape, in world coordinates.
        /// </summary>
        public Rectangle Destination;

        /// <summary>
        /// Draws this shape using the given modulating color and transform.
        /// </summary>
        public void Draw(Render Render, Color4 Color, Transform Transform)
        {
            Render.Vertex(Transform * Destination.TopLeft, Source.TopLeft, Color);
            Render.Vertex(Transform * Destination.BottomLeft, Source.BottomLeft, Color);
            Render.Vertex(Transform * Destination.BottomRight, Source.BottomRight, Color);
            Render.Vertex(Transform * Destination.TopRight, Source.TopRight, Color);
        }

        /// <summary>
        /// Draws this shape using the given scale and offset.
        /// </summary>
        public void Draw(Render Render, Color4 Color, Vector Offset, double Scale)
        {
            Render.Vertex(Destination.TopLeft * Scale + Offset, Source.TopLeft, Color);
            Render.Vertex(Destination.BottomLeft * Scale + Offset, Source.BottomLeft, Color);
            Render.Vertex(Destination.BottomRight * Scale + Offset, Source.BottomRight, Color);
            Render.Vertex(Destination.TopRight * Scale + Offset, Source.TopRight, Color);
        }
    }

    /// <summary>
    /// Defines additional operations on color4.
    /// </summary>
    public static class Color4Extensions
    {
        /// <summary>
        /// Mixes this color with another by the given factor.
        /// </summary>
        public static Color4 Mix(this Color4 Source, float Factor, Color4 Other)
        {
            float af = 1.0f - Factor;
            float bf = Factor;
            float na = af * Source.A + bf * Other.A;
            float nr = (af * Source.A * Source.R + bf * Other.A * Other.R) / na;
            float ng = (af * Source.A * Source.G + bf * Other.A * Other.G) / na;
            float nb = (af * Source.A * Source.B + bf * Other.A * Other.B) / na;
            return new Color4(nr, ng, nb, na);
        }
    }
}
