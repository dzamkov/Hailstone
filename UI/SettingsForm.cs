using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Hailstone.UI
{
    public partial class SettingsForm : Form
    {
        private SettingsForm()
        {
            InitializeComponent();
            this._PropertyGrid.SelectedObject = Settings.Current;
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
