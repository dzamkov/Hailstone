using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hailstone
{
    public partial class SettingsForm : Form
    {
        private SettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the only instance of this form.
        /// </summary>
        public static readonly SettingsForm Instance = new SettingsForm();

        /// <summary>
        /// Shows the settings form.
        /// </summary>
        public static new void Show()
        {
            ((Form)Instance).Show();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
