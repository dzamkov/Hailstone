using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using KopiLua;

namespace Hailstone.Interface
{
    public partial class PropertyForm : Form
    {
        public PropertyForm(TypeInterface Type, Func<object> Default, object Object)
        {
            InitializeComponent();

            this.Type = Type;
            this.Default = Default;
            this.Text = Type.Name;
            this._File = new FileToolStripMenuItem(Type.Name, ".lua");
            this._File.Save += new Action<string>(this._Save);
            this._File.Load += new Action<string>(this._Load);
            this._File.Patch += new Action<string>(this._Patch);
            this._MenuStrip.Items.Add(this._File);
            this._MenuStrip.Items.Add("Reset", null, delegate { this.Object = this.Default(); });
            
            this._PropertyGrid.SelectedObject = Object;
        }

        /// <summary>
        /// The type interface for the type of object this property form is for.
        /// </summary>
        public readonly TypeInterface Type;

        /// <summary>
        /// A function that creates the default (unpatched) object of the type this property form is for.
        /// </summary>
        public readonly Func<object> Default;

        /// <summary>
        /// Gets or sets the object being viewed by this property form.
        /// </summary>
        public object Object
        {
            get
            {
                return this._PropertyGrid.SelectedObject;
            }
            set
            {
                if (this._PropertyGrid.SelectedObject != value)
                {
                    this._PropertyGrid.SelectedObject = value;
                    if (this.ObjectChanged != null) this.ObjectChanged(value);
                }
            }
        }
        /// <summary>
        /// An event fired whenever the associated object for this property form is changed (but not when it is mutated).
        /// </summary>
        public event Action<object> ObjectChanged;

        /// <summary>
        /// Saves the current object to the given path.
        /// </summary>
        private void _Save(string Path)
        {
            using (FileStream stream = File.OpenWrite(Path))
            {
                this.Type.Save(Global.Default, this.Default(), this.Object, stream);
            }
        }

        /// <summary>
        /// Loads an object from the given path.
        /// </summary>
        private void _Load(string Path)
        {
            this._Load(this.Default(), Path);
        }

        /// <summary>
        /// Patches the current object from the given path.
        /// </summary>
        private void _Patch(string Path)
        {
            this._Load(this.Object, Path);
        }

        /// <summary>
        /// Loads and patches an object from the given path.
        /// </summary>
        private void _Load(object Initial, string Path)
        {
            using (FileStream stream = File.OpenRead(Path))
            {
                this.Type.Load(Global.Default, ref Initial, stream);
                this.Object = Initial;
            }
        }

        private FileToolStripMenuItem _File;
    }
}
