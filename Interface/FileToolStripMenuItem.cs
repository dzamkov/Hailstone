using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Hailstone.Interface
{
    /// <summary>
    /// A ToolStripMenuItem to control file saving and loading for an object of a certain type.
    /// </summary>
    public class FileToolStripMenuItem : ToolStripMenuItem
    {
        public FileToolStripMenuItem()
        {
            this.Text = "File";

            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            ToolStripComboBox saveselect = new ToolStripComboBox();
            saveselect.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    save.Owner.Hide();
                    if (this.Save != null) this.Save.Invoke(this.GetPath(saveselect.Text));
                }
            };
            save.DropDownItems.Add(saveselect);
            save.DropDownItems.Add("Browse", null, delegate { this._BrowseSave(this.Save); });

            ToolStripMenuItem load = new ToolStripMenuItem("Load");
            ToolStripComboBox loadselect = new ToolStripComboBox();
            loadselect.DropDownStyle = ComboBoxStyle.DropDownList;
            loadselect.DropDown += delegate { this._Populate(loadselect); };
            loadselect.ComboBox.SelectionChangeCommitted += delegate
            {
                load.Owner.Hide();
                if (this.Load != null) this.Load.Invoke(this.GetPath(loadselect.Text));
            };
            load.DropDownItems.Add(loadselect);
            load.DropDownItems.Add("Browse", null, delegate { this._BrowseLoad(this.Load); });

            ToolStripMenuItem patch = new ToolStripMenuItem("Patch");
            ToolStripComboBox patchselect = new ToolStripComboBox();
            patchselect.DropDownStyle = ComboBoxStyle.DropDownList;
            patchselect.DropDown += delegate { this._Populate(patchselect); };
            patchselect.ComboBox.SelectionChangeCommitted += delegate
            {
                patch.Owner.Hide();
                if (this.Patch != null) this.Patch.Invoke(this.GetPath(patchselect.Text));
            };
            patch.DropDownItems.Add(patchselect);
            patch.DropDownItems.Add("Browse", null, delegate { this._BrowseLoad(this.Patch); });

            ToolStripMenuItem delete = new ToolStripMenuItem("Delete");
            ToolStripComboBox deleteselect = new ToolStripComboBox();
            deleteselect.DropDownStyle = ComboBoxStyle.DropDownList;
            deleteselect.DropDown += delegate { this._Populate(deleteselect); };
            deleteselect.ComboBox.SelectionChangeCommitted += delegate
            {
                delete.Owner.Hide();
                File.Delete(this.GetPath(deleteselect.Text));
            };
            delete.DropDownItems.Add(deleteselect);

            this.DropDownItems.Add(save);
            this.DropDownItems.Add(load);
            this.DropDownItems.Add(patch);
            this.DropDownItems.Add(delete);
        }

        /// <summary>
        /// The name of the type of objects this file menu is for.
        /// </summary>
        public string Type;

        /// <summary>
        /// The file extension of the target object.
        /// </summary>
        public string Extension;

        /// <summary>
        /// Gets the default directory this file menu saves/load to.
        /// </summary>
        public string Directory
        {
            get
            {
                return this.Type;
            }
        }

        /// <summary>
        /// Gets the full path for the file with the given name.
        /// </summary>
        public string GetPath(string Name)
        {
            return this.Directory + Path.DirectorySeparatorChar + Name + this.Extension;
        }

        /// <summary>
        /// An event fired when it's time to save the associated object to the given path.
        /// </summary>
        public event Action<string> Save;

        /// <summary>
        /// An event fired when it's time to load an object from the given file.
        /// </summary>
        public event Action<string> Load;

        /// <summary>
        /// An event fired when it's time to patch the current object with data from the given file.
        /// </summary>
        public event Action<string> Patch;

        /// <summary>
        /// Gets the file selection filter.
        /// </summary>
        private string _Filter
        {
            get
            {
                return String.Format("{0} Files|*{1}|All files|*", this.Type, this.Extension);
            }
        }

        /// <summary>
        /// Populates a combo box with items from the directory.
        /// </summary>
        private void _Populate(ToolStripComboBox Combo)
        {
            Combo.Items.Clear();
            foreach (string file in System.IO.Directory.GetFiles(this.Directory))
            {
                if (Path.GetExtension(file) == this.Extension)
                {
                    Combo.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        
        /// <summary>
        /// Opens a file dialog to select a file to save to.
        /// </summary>
        private void _BrowseSave(Action<string> OnSelect)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Path.GetFullPath(this.Directory);
            sfd.Filter = this._Filter;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (OnSelect != null)
                {
                    OnSelect(sfd.FileName);
                }
            }
        }

        /// <summary>
        /// Opens a file dialog to select a file to load from.
        /// </summary>
        private void _BrowseLoad(Action<string> OnSelect)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Path.GetFullPath(this.Directory);
            ofd.Filter = this._Filter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (OnSelect != null)
                {
                    OnSelect(ofd.FileName);
                }
            }
        }
    }
}
