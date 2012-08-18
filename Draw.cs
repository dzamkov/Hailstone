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
        /// Draws a digit of the given size to the given point.
        /// </summary>
        public static void Digit(Render Render, uint Digit, Color4 Color, Vector Center, double Size)
        {
            double uoffset = 0.0;
            double udelta = 0.099609375;
            double voffset = 0.828125;
            double vdelta = 0.13671875;
            double vsize = 0.1171875;

            double uleft = uoffset + udelta * Digit;
            double uright = uoffset + udelta * Digit + udelta;
            double vtop = voffset;
            double vbottom = voffset + vdelta;

            double left = Center.X - Size * 0.5 * udelta / vsize;
            double right = Center.X + Size * 0.5 * udelta / vsize;
            double top = Center.Y + Size * 0.5 * vdelta / vsize;
            double bottom = Center.Y - Size * 0.5 * vdelta / vsize;

            Render.Vertex(left, top, uleft, vtop, Color);
            Render.Vertex(left, bottom, uleft, vbottom, Color);
            Render.Vertex(right, bottom, uright, vbottom, Color);
            Render.Vertex(right, top, uright, vtop, Color);
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
        /// Ends a drawing context.
        /// </summary>
        public static void End(Render Render)
        {
            Render.Finish();
        }
    }
}
