namespace Hailstone
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._MenuStrip = new System.Windows.Forms.MenuStrip();
            this._GLControl = new OpenTK.GLControl();
            this._OptionsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsButton = new System.Windows.Forms.ToolStripMenuItem();
            this._MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._OptionsMenu});
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(624, 24);
            this._MenuStrip.TabIndex = 0;
            // 
            // _GLControl
            // 
            this._GLControl.BackColor = System.Drawing.Color.Black;
            this._GLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._GLControl.Location = new System.Drawing.Point(0, 24);
            this._GLControl.Name = "_GLControl";
            this._GLControl.Size = new System.Drawing.Size(624, 418);
            this._GLControl.TabIndex = 1;
            this._GLControl.VSync = false;
            this._GLControl.Load += new System.EventHandler(this._GLControl_Load);
            this._GLControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this._GLControl_MouseWheel);
            this._GLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this._GLControl_MouseDown);
            this._GLControl.Resize += new System.EventHandler(this._GLControl_Resize);
            // 
            // _OptionsMenu
            // 
            this._OptionsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._SettingsButton});
            this._OptionsMenu.Name = "_OptionsMenu";
            this._OptionsMenu.Size = new System.Drawing.Size(61, 20);
            this._OptionsMenu.Text = "Options";
            // 
            // _SettingsButton
            // 
            this._SettingsButton.Name = "_SettingsButton";
            this._SettingsButton.Size = new System.Drawing.Size(152, 22);
            this._SettingsButton.Text = "Settings";
            this._SettingsButton.Click += new System.EventHandler(this._SettingsButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this._GLControl);
            this.Controls.Add(this._MenuStrip);
            this.MainMenuStrip = this._MenuStrip;
            this.Name = "MainForm";
            this.RightToLeftLayout = true;
            this.Text = "Hailstone";
            this._MenuStrip.ResumeLayout(false);
            this._MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _MenuStrip;
        private OpenTK.GLControl _GLControl;
        private System.Windows.Forms.ToolStripMenuItem _OptionsMenu;
        private System.Windows.Forms.ToolStripMenuItem _SettingsButton;
    }
}