namespace Hailstone
{
    partial class SettingsForm
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
            this._SaveButton = new System.Windows.Forms.Button();
            this._LoadButton = new System.Windows.Forms.Button();
            this._ResetButton = new System.Windows.Forms.Button();
            this._PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // _SaveButton
            // 
            this._SaveButton.Location = new System.Drawing.Point(0, 2);
            this._SaveButton.Name = "_SaveButton";
            this._SaveButton.Size = new System.Drawing.Size(75, 23);
            this._SaveButton.TabIndex = 0;
            this._SaveButton.Text = "Save";
            this._SaveButton.UseVisualStyleBackColor = true;
            // 
            // _LoadButton
            // 
            this._LoadButton.Location = new System.Drawing.Point(81, 2);
            this._LoadButton.Name = "_LoadButton";
            this._LoadButton.Size = new System.Drawing.Size(75, 23);
            this._LoadButton.TabIndex = 1;
            this._LoadButton.Text = "Load";
            this._LoadButton.UseVisualStyleBackColor = true;
            // 
            // _ResetButton
            // 
            this._ResetButton.Location = new System.Drawing.Point(162, 2);
            this._ResetButton.Name = "_ResetButton";
            this._ResetButton.Size = new System.Drawing.Size(75, 23);
            this._ResetButton.TabIndex = 2;
            this._ResetButton.Text = "Reset";
            this._ResetButton.UseVisualStyleBackColor = true;
            // 
            // _PropertyGrid
            // 
            this._PropertyGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._PropertyGrid.Location = new System.Drawing.Point(0, 31);
            this._PropertyGrid.Name = "_PropertyGrid";
            this._PropertyGrid.Size = new System.Drawing.Size(238, 335);
            this._PropertyGrid.TabIndex = 3;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 366);
            this.Controls.Add(this._PropertyGrid);
            this.Controls.Add(this._ResetButton);
            this.Controls.Add(this._LoadButton);
            this.Controls.Add(this._SaveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _SaveButton;
        private System.Windows.Forms.Button _LoadButton;
        private System.Windows.Forms.Button _ResetButton;
        private System.Windows.Forms.PropertyGrid _PropertyGrid;
    }
}