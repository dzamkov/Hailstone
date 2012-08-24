namespace Hailstone.UI
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
            this._PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this._Selector = new System.Windows.Forms.ComboBox();
            this._SaveButton = new System.Windows.Forms.Button();
            this._LoadButton = new System.Windows.Forms.Button();
            this._Delete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _PropertyGrid
            // 
            this._PropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._PropertyGrid.Location = new System.Drawing.Point(0, 29);
            this._PropertyGrid.Name = "_PropertyGrid";
            this._PropertyGrid.Size = new System.Drawing.Size(276, 337);
            this._PropertyGrid.TabIndex = 3;
            // 
            // _Selector
            // 
            this._Selector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._Selector.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Selector.FormattingEnabled = true;
            this._Selector.Location = new System.Drawing.Point(0, 2);
            this._Selector.Name = "_Selector";
            this._Selector.Size = new System.Drawing.Size(128, 23);
            this._Selector.TabIndex = 4;
            // 
            // _SaveButton
            // 
            this._SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._SaveButton.Location = new System.Drawing.Point(134, 2);
            this._SaveButton.Name = "_SaveButton";
            this._SaveButton.Size = new System.Drawing.Size(43, 23);
            this._SaveButton.TabIndex = 5;
            this._SaveButton.Text = "Save";
            this._SaveButton.UseVisualStyleBackColor = true;
            this._SaveButton.Click += new System.EventHandler(this._SaveButton_Click);
            // 
            // _LoadButton
            // 
            this._LoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._LoadButton.Location = new System.Drawing.Point(183, 2);
            this._LoadButton.Name = "_LoadButton";
            this._LoadButton.Size = new System.Drawing.Size(39, 23);
            this._LoadButton.TabIndex = 6;
            this._LoadButton.Text = "Load";
            this._LoadButton.UseVisualStyleBackColor = true;
            this._LoadButton.Click += new System.EventHandler(this._LoadButton_Click);
            // 
            // _Delete
            // 
            this._Delete.Location = new System.Drawing.Point(228, 2);
            this._Delete.Name = "_Delete";
            this._Delete.Size = new System.Drawing.Size(48, 23);
            this._Delete.TabIndex = 7;
            this._Delete.Text = "Delete";
            this._Delete.UseVisualStyleBackColor = true;
            this._Delete.Click += new System.EventHandler(this._Delete_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 366);
            this.Controls.Add(this._Delete);
            this.Controls.Add(this._LoadButton);
            this.Controls.Add(this._SaveButton);
            this.Controls.Add(this._Selector);
            this.Controls.Add(this._PropertyGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(292, 300);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid _PropertyGrid;
        private System.Windows.Forms.ComboBox _Selector;
        private System.Windows.Forms.Button _SaveButton;
        private System.Windows.Forms.Button _LoadButton;
        private System.Windows.Forms.Button _Delete;
    }
}