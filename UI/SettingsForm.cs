using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Hailstone.Interface;

namespace Hailstone.UI
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
            Instance.Update();
            ((Form)Instance).Show();
        }

        /// <summary>
        /// Updates the settings form before opening.
        /// </summary>
        public new void Update()
        {
            this._Selector.Items.Clear();
            foreach (var option in Settings.Options)
            {
                this._Selector.Items.Add(option);
            }
            this._PropertyGrid.SelectedObject = Settings.Current;
        }

        private void _SaveButton_Click(object sender, EventArgs e)
        {
            string name = this._Selector.Text;
            Settings.Save(Settings.Current, name);
            if (!this._Selector.Items.Contains(name))
                this._Selector.Items.Add(name);
        }

        private void _LoadButton_Click(object sender, EventArgs e)
        {
            string name = this._Selector.Text;
            if (Settings.Load(name, ref Settings.Current))
                this._PropertyGrid.SelectedObject = Settings.Current;
            else
                this._BadName();
        }

        private void _Delete_Click(object sender, EventArgs e)
        {
            string name = this._Selector.Text;
            if (Settings.Delete(name))
                this._Selector.Items.Remove(name);
            else
                this._BadName();
        }

        /// <summary>
        /// Yells at the user for not entering a good settings name.
        /// </summary>
        private void _BadName()
        {
            MessageBox.Show("Please enter a valid setting option");
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
