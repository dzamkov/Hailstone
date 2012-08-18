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
            Render.Vertex(new Vector(-0.5, 0.5), new Vector(0.0, 0.0), new Color4(1.0f, 0.0f, 0.0f, 1.0f));
            Render.Vertex(new Vector(-0.5, -0.5), new Vector(0.0, 1.0), new Color4(0.0f, 1.0f, 0.0f, 1.0f));
            Render.Vertex(new Vector(0.5, -0.5), new Vector(1.0, 1.0), new Color4(0.0f, 0.0f, 1.0f, 1.0f));
            Render.Vertex(new Vector(0.5, 0.5), new Vector(1.0, 0.0), new Color4(0.0f, 0.0f, 0.0f, 0.0f));
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
