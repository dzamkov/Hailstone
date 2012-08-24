using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Hailstone.Interface
{
    public partial class PropertyForm : Form
    {
        public PropertyForm(string Type, object Object)
        {
            InitializeComponent();

            this._File = new FileToolStripMenuItem();
            this._MenuStrip.Items.Add(this._File);
            this._File.Type = Type;
            this._File.Extension = ".lua";

            this._PropertyGrid.SelectedObject = Object;
        }

        private FileToolStripMenuItem _File;
    }
}
