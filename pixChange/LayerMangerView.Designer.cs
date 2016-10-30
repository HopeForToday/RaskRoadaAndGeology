namespace pixChange
{
    partial class LayerMangerView
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
            this.label1 = new System.Windows.Forms.Label();
            this.ok = new System.Windows.Forms.Button();
            this.exCheckedListBox1 = new CustomControls.ExCheckedListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "图层选择：";
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(256, 7);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(81, 23);
            this.ok.TabIndex = 5;
            this.ok.Text = "确认";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // exCheckedListBox1
            // 
            this.exCheckedListBox1.CheckOnClick = true;
            this.exCheckedListBox1.DataSource = null;
            this.exCheckedListBox1.FormattingEnabled = true;
            this.exCheckedListBox1.Location = new System.Drawing.Point(15, 35);
            this.exCheckedListBox1.Name = "exCheckedListBox1";
            this.exCheckedListBox1.Size = new System.Drawing.Size(322, 372);
            this.exCheckedListBox1.TabIndex = 6;
            this.exCheckedListBox1.Value = 0;
            // 
            // LayerMangerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 420);
            this.Controls.Add(this.exCheckedListBox1);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.label1);
            this.Name = "LayerMangerView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LayerMangerView";
            this.Load += new System.EventHandler(this.LayerMangerView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        //private CustomControls.ExCheckedListBox exCheckedListBox1;
        private System.Windows.Forms.Button ok;
        private CustomControls.ExCheckedListBox exCheckedListBox1;
    }
}