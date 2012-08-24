namespace Hailstone.Interface
{
    partial class EditorForm
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
            this._TextEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this._MenuStrip = new System.Windows.Forms.MenuStrip();
            this.SuspendLayout();
            // 
            // _TextEditor
            // 
            this._TextEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TextEditor.IsReadOnly = false;
            this._TextEditor.Location = new System.Drawing.Point(0, 24);
            this._TextEditor.Name = "_TextEditor";
            this._TextEditor.Size = new System.Drawing.Size(414, 389);
            this._TextEditor.TabIndex = 0;
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(414, 24);
            this._MenuStrip.TabIndex = 1;
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 413);
            this.Controls.Add(this._TextEditor);
            this.Controls.Add(this._MenuStrip);
            this.MainMenuStrip = this._MenuStrip;
            this.Name = "EditorForm";
            this.Text = "Code Editor";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl _TextEditor;
        private System.Windows.Forms.MenuStrip _MenuStrip;

    }
}