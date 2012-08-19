using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Hailstone
{
    /// <summary>
    /// Contains functions for drawing using the atlas texture.
    /// </summary>
    public struct Draw
    {
        /// <summary>
        /// An identifier for the atlas texture.
        /// </summary>
        public static readonly int AtlasTexture = Texture.CreateAlpha(Textures.Atlas);

        /// <summary>
        /// Starts a drawing context.
        /// </summary>
        public static void Begin(out Render Render)
        {
            Texture.Bind(AtlasTexture);
            Render = Render.Create(BeginMode.Quads);
        }

        /// <summary>
        /// Tests the drawing context.
        /// </summary>
        public static void Test(Render Render)
        {
            Render.Vertex(-0.5, 0.5, 0.0, 0.0, new Color4(1.0f, 0.0f, 0.0f, 1.0f));
            Render.Vertex(-0.5, -0.5, 0.0, 1.0, new Color4(0.0f, 1.0f, 0.0f, 1.0f));
            Render.Vertex(0.5, -0.5, 1.0, 1.0, new Color4(0.0f, 0.0f, 1.0f, 1.0f));
            Render.Vertex(0.5, 0.5, 1.0, 0.0, new Color4(0.0f, 0.0f, 0.0f, 0.0f));
        }

        /// <summary>
        /// Expands a source and destination rectangle by a border specified in source coordinates.
        /// </summary>
        private static void _Expand(ref Rectangle Source, ref Rectangle Destination, double Border)
        {
            double wborder =  Border * (Destination.Right - Destination.Left) / (Source.Right - Source.Left);
            double hborder = Border * (Destination.Top - Destination.Bottom) / (Source.Bottom - Source.Top);
            Source.Left -= Border;
            Source.Top -= Border;
            Source.Right += Border;
            Source.Bottom += Border;
            Destination.Left -= wborder;
            Destination.Top += hborder;
            Destination.Right += wborder;
            Destination.Bottom -= hborder;
        }

        /// <summary>
        /// Draws a quad from the atlas texture.
        /// </summary>
        private static void _Quad(Render Render, Color4 Color, Rectangle Source, Rectangle Destination)
        {
            Render.Vertex(Destination.TopLeft, Source.TopLeft, Color);
            Render.Vertex(Destination.BottomLeft, Source.BottomLeft, Color);
            Render.Vertex(Destination.BottomRight, Source.BottomRight, Color);
            Render.Vertex(Destination.TopRight, Source.TopRight, Color);
        }

        /// <summary>
        /// Draws a digit of the given size to the given point.
        /// </summary>
        public static void Digit(Render Render, uint Digit, Color4 Color, Vector Center, double Size)
        {
            double delta = 0.099609375;
            double left = 0.0078125 + delta * Digit;
            Rectangle source = new Rectangle(left, 0.8359375, left + 0.0859375, 0.953125);
            Rectangle destination = new Rectangle(Center.X - 0.36 * Size, Center.Y + 0.5 * Size, Center.X + 0.36 * Size, Center.Y - 0.5 * Size);
            _Expand(ref source, ref destination, 0.0078125);
            _Quad(Render, Color, source, destination);
        }

        /// <summary>
        /// Draws a number to the given point.
        /// </summary>
        public static void Number(Render Render, uint Number, Color4 Color, Vector Center, double Size, double Spacing)
        {
            List<uint> digits = new List<uint>(5);
            if (Number == 0)
                digits.Add(0);
            else
                while (Number > 0)
                {
                    digits.Add(Number % 10);
                    Number /= 10;
                }

            double offset = (Spacing * (digits.Count - 1)) / 2.0;
            foreach (uint digit in digits)
            {
                Draw.Digit(Render, digit, Color, new Vector(Center.X + offset, Center.Y), Size);
                offset -= Spacing;
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        public static void Circle(Render Render, Color4 Color, Vector Center, double Radius)
        {
            Rectangle source = new Rectangle(0.4375, 0.1875, 0.9375, 0.6875);
            Rectangle destination = new Rectangle(Center.X - Radius, Center.Y + Radius, Center.X + Radius, Center.Y - Radius);
            _Expand(ref source, ref destination, 0.0625);
            _Quad(Render, Color, source, destination);
        }

        /// <summary>
        /// Draws a filled circle.
        /// </summary>
        public static void FilledCircle(Render Render, Color4 Color, Vector Center, double Radius)
        {
            Rectangle source = new Rectangle(0.0625, 0.0625, 0.3125, 0.3125);
            Rectangle destination = new Rectangle(Center.X - Radius, Center.Y + Radius, Center.X + Radius, Center.Y - Radius);
            _Expand(ref source, ref destination, 0.03125);
            _Quad(Render, Color, source, destination);
        }

        /// <summary>
        /// Draws a stone containing the given number.
        /// </summary>
        public static void Stone(Render Render, uint Number, Color4 Highlight, Color4 Fill, Vector Center, double Radius)
        {
            Draw.FilledCircle(Render, Fill, Center, Radius);
            Draw.Circle(Render, Highlight, Center, Radius);
            Draw.Number(Render, Number, Highlight, Center, 0.1, 0.08);
        }

        /// <summary>
        /// Ends a drawing context.
        /// </summary>
        public static void End(Render Render)
        {
            Render.Finish();
        }
    }
}
