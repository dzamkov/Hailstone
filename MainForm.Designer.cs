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
            this.SuspendLayout();
            // 
            // _MenuStrip
            // 
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _MenuStrip;
        private OpenTK.GLControl _GLControl;
    }
}