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
        public FileToolStripMenuItem(string Directory, string Extension)
        {
            this.Text = "File";
            this.Extension = Extension;

            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            this._Setup(Directory, save, _Mode.Save, delegate(string Path)
            {
                if (this.Save != null)
                    this.Save(Path);
            });
            this.DropDownItems.Add(save);

            ToolStripMenuItem load = new ToolStripMenuItem("Load");
            this._Setup(Directory, load, _Mode.Load, delegate(string Path)
            {
                if (this.Load != null)
                    this.Load(Path);
            });
            this.DropDownItems.Add(load);

            ToolStripMenuItem patch = new ToolStripMenuItem("Patch");
            this._Setup(Directory, patch, _Mode.Load, delegate(string Path)
            {
                if (this.Patch != null)
                    this.Patch(Path);
            });
            this.DropDownItems.Add(load);

            ToolStripMenuItem delete = new ToolStripMenuItem("Delete");
            this._Setup(Directory, delete, _Mode.Delete, delegate(string Path)
            {
                string message = String.Format("Are you sure you want to delete {0}?", System.IO.Path.GetFileNameWithoutExtension(Path));
                if (File.Exists(Path))
                    if (MessageBox.Show(message, "Delete File?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        File.Delete(Path);
                if (System.IO.Directory.Exists(Path))
                    if (MessageBox.Show(message, "Delete Directory?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        System.IO.Directory.Delete(Path, true);
            });
            this.DropDownItems.Add(load);

            
            this.DropDownItems.Add(load);
            this.DropDownItems.Add(patch);
            this.DropDownItems.Add(delete);
        }

        /// <summary>
        /// The file extension of the target object.
        /// </summary>
        public readonly string Extension;

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
        /// Sets up a drop-down menu to allow file-system selection.
        /// </summary>
        private void _Setup(string Path, ToolStripMenuItem Item, _Mode Mode, Action<string> OnSelect)
        {
            if (Mode == _Mode.Save)
            {
                ToolStripTextBox textbox = new ToolStripTextBox();
                Item.DropDownItems.Add(textbox);
                textbox.ToolTipText = "Type a file name here. Press Enter to save, or Control + Enter to make a new folder.";
                textbox.KeyDown += delegate(object sender, KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (e.Control)
                        {
                            string dirname = textbox.Text;
                            string dirpath = Path + System.IO.Path.DirectorySeparatorChar + dirname;
                            Directory.CreateDirectory(dirpath);
                            ToolStripMenuItem item = new ToolStripMenuItem(dirname);
                            this._Setup(dirpath, item, _Mode.Save, OnSelect);
                            Item.DropDownItems.Insert(1, item);
                            textbox.Clear();
                        }
                        else
                        {
                            this.HideDropDown();
                            OnSelect(Path + System.IO.Path.DirectorySeparatorChar + textbox.Text);
                        }
                    }
                };

                Item.DropDownOpening += delegate
                {
                    this._Populate(Path, Item, Mode, OnSelect);
                };
                Item.DropDownClosed += delegate
                {
                    Item.DropDownItems.Clear();
                    Item.DropDownItems.Add(textbox);
                };
            }
            else
            {
                ToolStripMenuItem dummy = new ToolStripMenuItem("(Nothing Here)");
                dummy.Enabled = false;
                Item.DropDownItems.Add(dummy);
                Item.DropDownOpening += delegate
                {
                    if (this._Populate(Path, Item, Mode, OnSelect))
                    {
                        Item.DropDownItems.Remove(dummy);
                    }
                };
                Item.DropDownClosed += delegate
                {
                    Item.DropDownItems.Clear();
                    Item.DropDownItems.Add(dummy);
                };
            }
        }

        /// <summary>
        /// Recursively a drop-down menu item with files from the given directory.
        /// </summary>
        private bool _Populate(string Path, ToolStripMenuItem Item, _Mode Mode, Action<string> OnSelect)
        {
            bool has = false;
            foreach (string file in System.IO.Directory.GetFiles(Path))
            {
                if (System.IO.Path.GetExtension(file) == this.Extension)
                {
                    string path = file;
                    ToolStripItem item = Item.DropDownItems.Add(System.IO.Path.GetFileNameWithoutExtension(path));
                    item.Click += delegate 
                    {
                        this.HideDropDown();
                        OnSelect(path); 
                    };
                    has = true;
                }
            }
            foreach (string directory in System.IO.Directory.GetDirectories(Path))
            {
                string path = directory;
                ToolStripMenuItem item = new ToolStripMenuItem(System.IO.Path.GetFileName(path));
                Item.DropDownItems.Add(item);
                if (Mode == _Mode.Delete) item.Click += delegate 
                {
                    this.HideDropDown();
                    OnSelect(path); 
                };
                this._Setup(path, item, Mode, OnSelect);
                has = true;
            }
            return has;
        }

        /// <summary>
        /// Represents a file selection mode.
        /// </summary>
        private enum _Mode
        {
            Save,
            Load,
            Delete
        }
    }
}
