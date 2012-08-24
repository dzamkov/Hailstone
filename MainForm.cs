﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Hailstone.Interface;

namespace Hailstone
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this._Camera = new Camera();
            this._Domain = new Domain(x => (x % 2 == 0) ? (x / 2) : (x * 3 + 1));
            this._World = new World(this._Domain);

            for (uint t = 1; t < 9500; t++)
            {
                Entry x = this._Domain[t];
            }
            this._World.Insert(this._Domain[1], Vector.Zero, Vector.Zero);
        }

        /// <summary>
        /// Gets the transform from GL-control coordinates to device coordinates.
        /// </summary>
        public Transform ControlToDevice
        {
            get
            {
                return new Transform(
                        new Vector(-1.0, 1.0),
                        new Vector(2.0 / this._GLControl.Width, 0.0),
                        new Vector(0.0, -2.0 / this._GLControl.Height));
            }
        }

        /// <summary>
        /// Gets the transform from device coordinates to viewspace.
        /// </summary>
        public Transform DeviceToView
        {
            get
            {
                double width = this._GLControl.Width;
                double height = this._GLControl.Height;
                if (width > this._GLControl.Height)
                    return Transform.Scale(width / height, 1.0);
                else
                    return Transform.Scale(1.0, height / width);
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
        /// Gets the transformation from GL-Control coordinates to worldspace.
        /// </summary>
        public Transform ControlToWorld
        {
            get
            {
                return this.ControlToDevice * this.DeviceToView * this.ViewToWorld;
            }
        }

        /// <summary>
        /// Renders the contents of this form.
        /// </summary>
        public void Render()
        {
            if (this._Loaded)
            {
                GL.ClearColor((Color4)Settings.Current.BackgroundColor);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Transform dtw = this.DeviceToView * this.ViewToWorld;
                Transform wtd = dtw.Inverse;
                Rectangle bounds = dtw.Project(new Rectangle(-1.0, 1.0, 1.0, -1.0));
                Matrix4d mat = wtd;
                GL.LoadMatrix(ref mat);
                this._World.Render(bounds, this._Camera.Extent);

                this._GLControl.SwapBuffers();
            }
        }

        /// <summary>
        /// Updates the state of this form by the given amount of time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            this._Camera.Update(Time);
            this._World.Update(Time);
            Stone.SelectionPulsePhase += (Settings.Current.StonePulseSpeed * Time) % 1.0;
        }

        private void _GLControl_Load(object sender, EventArgs e)
        {
            this._Loaded = true;
            this._GLControl.MakeCurrent();
            this._GLControl.VSync = true;
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private void _GLControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this._GLControl.Width, this._GLControl.Height);
        }

        private void _GLControl_MouseDown(object sender, MouseEventArgs e)
        {
            Vector pos = this.ControlToWorld.Project(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                Stone stone = this._World.Pick(pos);
                if (stone != null)
                {
                    Stone.Selection = new Chain(stone, this._World[this._Domain[1]]);
                }
                else
                {
                    Stone.Selection = null;
                }
            }
        }

        private void _GLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            this._Camera.ZoomTo(e.Delta * 0.01, this.ControlToWorld.Project(e.X, e.Y));
        }

        private void _SettingsButton_Click(object sender, EventArgs e)
        {
            PropertyForm form = new PropertyForm(TypeInterface.Get(typeof(Settings)), () => Settings.Default, Settings.Current);
            form.ObjectChanged += delegate(object settings) { Settings.Current = (Settings)settings; };
            form.Show();
        }

        private void _EditorButton_Click(object sender, EventArgs e)
        {
            new Interface.EditorForm().Show();
        }

        private bool _Loaded;
        private Camera _Camera;
        private Domain _Domain;
        private World _World;
    }
}
