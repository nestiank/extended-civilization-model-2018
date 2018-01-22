namespace WinformView
{
    partial class DeploySelector
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
            this.lbxSelection = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lbxSelection
            // 
            this.lbxSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxSelection.FormattingEnabled = true;
            this.lbxSelection.ItemHeight = 24;
            this.lbxSelection.Location = new System.Drawing.Point(0, 0);
            this.lbxSelection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbxSelection.Name = "lbxSelection";
            this.lbxSelection.Size = new System.Drawing.Size(698, 496);
            this.lbxSelection.TabIndex = 0;
            // 
            // DeploySelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 496);
            this.Controls.Add(this.lbxSelection);
            this.Name = "DeploySelector";
            this.Text = "DeploySelector";
            this.Load += new System.EventHandler(this.DeploySelector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbxSelection;
    }
}