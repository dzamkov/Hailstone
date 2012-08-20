﻿using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Hailstone
{
    /// <summary>
    /// The main window for the application.
    /// </summary>
    public class Window : GameWindow
    {
        public Window()
            : base(640, 480, GraphicsMode.Default, Program.Title, GameWindowFlags.Default)
        {
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
            this._Camera = new Camera();
            this.Mouse.WheelChanged += delegate(object sender, MouseWheelEventArgs e)
            {
                this._Camera.ZoomTo(e.DeltaPrecise, this.WindowToWorld.Project(e.X, e.Y));
            };

            this.Mouse.ButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                Vector pos = this.WindowToWorld.Project(e.X, e.Y);
                if (e.Button == MouseButton.Left)
                {
                    Stone stone = this._World.Pick(pos);
                    if (stone != null)
                    {
                        Stone.Selection = new Chain(stone, this._World[1]);
                    }
                    else
                    {
                        Stone.Selection = null;
                    }
                }
            };

            this.VSync = VSyncMode.Off;

            this._World = new World(HailstoneSequence.Instance);
        }

        /// <summary>
        /// Gets the transform from window coordinates to device coordinates.
        /// </summary>
        public Transform WindowToDevice
        {
            get
            {
                return new Transform(
                        new Vector(-1.0, 1.0),
                        new Vector(2.0 / this.Width, 0.0),
                        new Vector(0.0, -2.0 / this.Height));
            }
        }
        
        /// <summary>
        /// Gets the transform from device coordinates to viewspace.
        /// </summary>
        public Transform DeviceToView
        {
            get
            {
                if (this.Width > this.Height)
                    return Transform.Scale((double)this.Width / this.Height, 1.0);
                else
                    return Transform.Scale(1.0, (double)this.Height / this.Width);
            }
        }

        /// <summary>
        /// Gets the transformation from viewspace to worldspace.
        /// </summary>
        public Transform ViewToWorld
        {
            get
            {
                return this._Camera.Transform;
            }
        }

        /// <summary>
        /// Gets the transformation from window coordinates to worldspace.
        /// </summary>
        public Transform WindowToWorld
        {
            get
            {
                return this.WindowToDevice * this.DeviceToView * this.ViewToWorld;
            }
        }

        /// <summary>
        /// Gets the transformation from worldspace to device coordinates.
        /// </summary>
        public Transform WorldToDevice
        {
            get
            {
                return Transform.Compose(this.DeviceToView, this.ViewToWorld).Inverse;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(0.5f, 0.7f, 0.9f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Transform trans = this.WorldToDevice;
            Matrix4d mat = trans;
            GL.LoadMatrix(ref mat);
            this._World.Render(this._Camera.Extent);

            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if ((this._Cycle = (this._Cycle + 1) % 30) == 0 && this._World.StoneCount < 2000)
            {
                this._World.Insert(this._Next++);
            }

            this.Title = String.Format("{0} ({1:#} fps)", Program.Title, this.RenderFrequency);
            this._Camera.Update(e.Time);
            this._World.Update(e.Time);
            Stone.SelectionGlowPhase += e.Time;
        }

        private uint _Next;
        private int _Cycle;
        private Camera _Camera;
        private World _World;
    }
}
