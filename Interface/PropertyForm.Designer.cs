namespace Hailstone.Interface
{
    partial class PropertyForm
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
            this._PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this._MenuStrip = new System.Windows.Forms.MenuStrip();
            this.SuspendLayout();
            // 
            // _PropertyGrid
            // 
            this._PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PropertyGrid.Location = new System.Drawing.Point(0, 24);
            this._PropertyGrid.Name = "_PropertyGrid";
            this._PropertyGrid.Size = new System.Drawing.Size(276, 342);
            this._PropertyGrid.TabIndex = 3;
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(276, 24);
            this._MenuStrip.TabIndex = 4;
            // 
            // PropertyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 366);
            this.Controls.Add(this._PropertyGrid);
            this.Controls.Add(this._MenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this._MenuStrip;
            this.MinimumSize = new System.Drawing.Size(292, 300);
            this.Name = "PropertyForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid _PropertyGrid;
        private System.Windows.Forms.MenuStrip _MenuStrip;
    }
}