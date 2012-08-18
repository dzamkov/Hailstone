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

            this.TargetRenderFrequency = 200.0;
            this.VSync = VSyncMode.Off;
            

            this._Camera = new Camera();
            this.Mouse.WheelChanged += delegate(object sender, MouseWheelEventArgs e)
            {
                this._Camera.ZoomTo(e.DeltaPrecise, this.WindowToWorld.Project(e.X, e.Y));
            };
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
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4d mat = this.WorldToDevice;
            GL.LoadMatrix(ref mat);

            mat = Transform.Rotate(0.1);
            GL.MultMatrix(ref mat);

            GL.Begin(BeginMode.Quads);
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex2(-0.5, 0.5);
            GL.Vertex2(-0.5, -0.5);
            GL.Vertex2(0.5, -0.5);
            GL.Vertex2(0.5, 0.5);
            GL.End();

            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this.Title = String.Format("{0} ({1:#} fps)", Program.Title, this.RenderFrequency);
            this._Camera.Update(e.Time);
        }

        private Camera _Camera;
    }
}