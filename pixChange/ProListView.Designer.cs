namespace pixChange
{
    partial class ProListView
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
            this.DataGrdView = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.DataGrdView)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGrdView
            // 
            this.DataGrdView.AllowUserToAddRows = false;
            this.DataGrdView.AllowUserToDeleteRows = false;
            this.DataGrdView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGrdView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGrdView.Location = new System.Drawing.Point(0, 10);
            this.DataGrdView.Name = "DataGrdView";
            this.DataGrdView.RowTemplate.Height = 23;
            this.DataGrdView.Size = new System.Drawing.Size(770, 357);
            this.DataGrdView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(770, 10);
            this.panel1.TabIndex = 1;
            // 
            // ProListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 367);
            this.Controls.Add(this.DataGrdView);
            this.Controls.Add(this.panel1);
            this.Name = "ProListView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "属性表";
            ((System.ComponentModel.ISupportInitialize)(this.DataGrdView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGrdView;
        private System.Windows.Forms.Panel panel1;

    }
}