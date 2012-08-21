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
    public static class Atlas
    {
        /// <summary>
        /// An identifier for the atlas texture.
        /// </summary>
        public static readonly int Texture = Hailstone.Texture.CreateAlpha(Textures.Atlas);

        /// <summary>
        /// Generates the shapes for the digits.
        /// </summary>
        private static Shape[] _GetDigits()
        {
            Shape[] shapes = new Shape[10];
            double delta = 0.099609375;
            double left = 0.0078125;
            double right = 0.091796875;
            for (int t = 0; t < 10; t++)
            {
                shapes[t] = new Shape(new Rectangle(left, 0.8359375, right, 0.953125), 0.0078125);
                left += delta;
                right += delta;
            }
            return shapes;
        }

        /// <summary>
        /// The shapes for the digits in the atlas texture.
        /// </summary>
        public static readonly Shape[] Digits = _GetDigits();

        /// <summary>
        /// The shape for the circle in the atlas texture.
        /// </summary>
        public static readonly Shape Circle = new Shape(new Rectangle(0.4375, 0.1875, 0.9375, 0.6875), 0.0625);

        /// <summary>
        /// The shape for the filled circle in the atlas texture.
        /// </summary>
        public static readonly Shape FilledCircle = new Shape(new Rectangle(0.0625, 0.0625, 0.3125, 0.3125), 0.03125);

        /// <summary>
        /// The shape for the line segment in the atlas texture.
        /// </summary>
        public static readonly Shape LineSegment = 
            new Shape(new Rectangle(0.171875, 0.5859375, 0.203125, 0.7109375), new Rectangle(-0.5, 1.0, 0.5, 0.0), 0.015625, 0.0);

        /// <summary>
        /// The shape for the arrow segment in the atlas texture.
        /// </summary>
        public static readonly Shape ArrowSegment =
            new Shape(new Rectangle(0.08984375, 0.359375, 0.28515625, 0.5546875), new Rectangle(-3.125, 6.25, 3.125, 0.0), 0.015625, 0.0);

        /// <summary>
        /// Starts a drawing context using the atlas texture.
        /// </summary>
        public static void Begin(out Render Render)
        {
            Hailstone.Texture.Bind(Atlas.Texture);
            Render = Render.Create(BeginMode.Quads);
        }

        /// <summary>
        /// Draws a digit of the given size to the given point.
        /// </summary>
        public static void DrawDigit(Render Render, uint Digit, Color4 Color, Vector Center, double Size)
        {
            Digits[Digit].Draw(Render, Color, Center, Size);
        }

        /// <summary>
        /// Draws a number to the given point.
        /// </summary>
        public static void DrawNumber(Render Render, uint Number, Color4 Color, Vector Center, double Size, double Spacing)
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
                DrawDigit(Render, digit, Color, new Vector(Center.X + offset, Center.Y), Size);
                offset -= Spacing;
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        public static void DrawCircle(Render Render, Color4 Color, Vector Center, double Radius)
        {
            Circle.Draw(Render, Color, Center, Radius * 2.0);
        }

        /// <summary>
        /// Draws a filled circle.
        /// </summary>
        public static void DrawFilledCircle(Render Render, Color4 Color, Vector Center, double Radius)
        {
            FilledCircle.Draw(Render, Color, Center, Radius * 2.0);
        }

        /// <summary>
        /// Draws a line connecting two points.
        /// </summary>
        public static void DrawLine(Render Render, Color4 Color, Vector A, Vector B, double Width)
        {
            Vector dif = B - A;
            double len = dif.Length;
            Vector dir = dif / len;
            LineSegment.Draw(Render, Color, new Transform(A, -dir.Cross * Width, dif));
        }

        /// <summary>
        /// Draws a line starting at a given point going in the given direction for the given length.
        /// </summary>
        public static void DrawLine(Render Render, Color4 Color, Vector Start, Vector Direction, double Length, double Width)
        {
            LineSegment.Draw(Render, Color, new Transform(Start, -Direction.Cross * Width, Direction * Length));
        }

        /// <summary>
        /// Draws an arrow connecting two points.
        /// </summary>
        public static void DrawArrow(Render Render, Color4 Color, Vector A, Vector B, double Width)
        {
            Vector dif = B - A;
            double len = dif.Length;
            Vector dir = dif / len;
            double linelen = len - ArrowSegment.Destination.Top * Width;

            LineSegment.Draw(Render, Color, new Transform(A, -dir.Cross * Width, dir * linelen));
            ArrowSegment.Draw(Render, Color, new Transform(A + dir * linelen, -dir.Cross * Width, dir * Width));
        }

        /// <summary>
        /// Draws an arrow starting at a given point going in the given direction for the given length.
        /// </summary>
        public static void DrawArrow(Render Render, Color4 Color, Vector Start, Vector Direction, double Length, double Width)
        {
            double linelen = Length - ArrowSegment.Destination.Top * Width;
            LineSegment.Draw(Render, Color, new Transform(Start, -Direction.Cross * Width, Direction * linelen));
            ArrowSegment.Draw(Render, Color, new Transform(Start + Direction * linelen, -Direction.Cross * Width, Direction * Width));
        }

        /// <summary>
        /// Draws a stone along with its "Next" link.
        /// </summary>
        public static void DrawStone(Render Render, Stone Stone, double Extent)
        {
            Color4 fillcolor;
            Color4 bordercolor;
            Color4 linecolor;
            Color4 highlightcolor;
            double numbersize = Settings.Current.StoneNumberSize;
            uint selection = Stone.GetSelectionIndex(Stone);
            if (selection != uint.MaxValue)
            {
                selection = (uint)Stone.Selection.Length - selection;
                float glow = (float)Math.Sin((Stone.SelectionPulsePhase + selection / Settings.Current.StonePulseLength) * Math.PI * 2.0) * 0.5f + 0.5f;

                fillcolor = Settings.Current.StoneFillColor.GetSelected(glow);
                bordercolor = Settings.Current.StoneBorderColor.GetSelected(glow);
                highlightcolor = Settings.Current.StoneHighlightColor.GetSelected(glow);
                linecolor = (selection != 0) ? bordercolor : Settings.Current.StoneBorderColor.GetUnselected();
                if (selection != 0 && Stone.Next != null && Stone.Next != Stone)
                {
                    Vector markerpos = Stone.Position + (Stone.Next.Position - Stone.Position).Normal.Cross * (Stone.Radius + 0.5);
                    Atlas.DrawNumber(Render, selection, new Color4(1.0f, 0.4f, 0.4f, 0.7f), markerpos, numbersize, numbersize);
                }
            }
            else
            {
                fillcolor = Settings.Current.StoneFillColor.GetUnselected();
                bordercolor = Settings.Current.StoneBorderColor.GetUnselected();
                highlightcolor = Settings.Current.StoneHighlightColor.GetUnselected();
                linecolor = bordercolor;
            }

            double size = Math.Max(1.0, Extent * 0.01);
            float light = (float)Math.Max(0.0, Math.Min(1.0, (Extent - 50.0) / 100.0));
            fillcolor = fillcolor.Mix(light, highlightcolor);
            Atlas.DrawFilledCircle(Render, fillcolor, Stone.Position, Stone.Radius * size);

            if (Extent < 120.0)
            {
                if (Stone.Next != null && Stone.Next != Stone)
                {
                    Vector dif = Stone.Next.Position - Stone.Position;
                    double len = dif.Length;
                    Vector dir = dif / len;
                    Vector start = Stone.Position + dir * Stone.Radius;
                    double linklen = len - Stone.Radius - Stone.Next.Radius;
                    if (linklen > 0.0)
                    {
                        double linkwidth = Settings.Current.LinkWidth;
                        if (linklen >= Settings.Current.LinkMinimumArrowLength)
                            Atlas.DrawArrow(Render, linecolor, start, dir, linklen, linkwidth);
                        else
                            Atlas.DrawLine(Render, linecolor, start, dir, linklen, linkwidth);
                    }
                }
                Atlas.DrawCircle(Render, bordercolor, Stone.Position, Stone.Radius);
                Atlas.DrawNumber(Render, Stone.Entry.Value, Settings.Current.StoneNumberColor, Stone.Position, numbersize, numbersize * 0.8);
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
