namespace WinformView
{
    partial class TileInfo
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
            this.tbUnit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTileBuilding = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTileType = new System.Windows.Forms.ComboBox();
            this.cbxCity = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPopulation = new System.Windows.Forms.TextBox();
            this.lbxInterior = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnInteriorDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbUnit
            // 
            this.tbUnit.Location = new System.Drawing.Point(95, 34);
            this.tbUnit.Name = "tbUnit";
            this.tbUnit.ReadOnly = true;
            this.tbUnit.Size = new System.Drawing.Size(230, 21);
            this.tbUnit.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "유닛 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "타일 건물 : ";
            // 
            // tbTileBuilding
            // 
            this.tbTileBuilding.Location = new System.Drawing.Point(95, 61);
            this.tbTileBuilding.Name = "tbTileBuilding";
            this.tbTileBuilding.ReadOnly = true;
            this.tbTileBuilding.Size = new System.Drawing.Size(230, 21);
            this.tbTileBuilding.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "소속 도시 : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "지형 :";
            // 
            // cbxTileType
            // 
            this.cbxTileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTileType.FormattingEnabled = true;
            this.cbxTileType.Location = new System.Drawing.Point(95, 115);
            this.cbxTileType.Name = "cbxTileType";
            this.cbxTileType.Size = new System.Drawing.Size(230, 20);
            this.cbxTileType.TabIndex = 7;
            // 
            // cbxCity
            // 
            this.cbxCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCity.FormattingEnabled = true;
            this.cbxCity.Location = new System.Drawing.Point(95, 141);
            this.cbxCity.Name = "cbxCity";
            this.cbxCity.Size = new System.Drawing.Size(230, 20);
            this.cbxCity.TabIndex = 9;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(250, 192);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(169, 192);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "인구 :";
            // 
            // tbPopulation
            // 
            this.tbPopulation.Enabled = false;
            this.tbPopulation.Location = new System.Drawing.Point(95, 88);
            this.tbPopulation.Name = "tbPopulation";
            this.tbPopulation.ReadOnly = true;
            this.tbPopulation.Size = new System.Drawing.Size(230, 21);
            this.tbPopulation.TabIndex = 5;
            // 
            // lbxInterior
            // 
            this.lbxInterior.Enabled = false;
            this.lbxInterior.FormattingEnabled = true;
            this.lbxInterior.ItemHeight = 12;
            this.lbxInterior.Location = new System.Drawing.Point(342, 58);
            this.lbxInterior.Name = "lbxInterior";
            this.lbxInterior.Size = new System.Drawing.Size(120, 160);
            this.lbxInterior.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(374, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "일반 건물";
            // 
            // btnInteriorDelete
            // 
            this.btnInteriorDelete.Location = new System.Drawing.Point(342, 32);
            this.btnInteriorDelete.Name = "btnInteriorDelete";
            this.btnInteriorDelete.Size = new System.Drawing.Size(120, 23);
            this.btnInteriorDelete.TabIndex = 14;
            this.btnInteriorDelete.Text = "삭제";
            this.btnInteriorDelete.UseVisualStyleBackColor = true;
            this.btnInteriorDelete.Click += new System.EventHandler(this.btnInteriorDelete_Click);
            // 
            // TileInfo
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(483, 230);
            this.Controls.Add(this.btnInteriorDelete);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbxInterior);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbPopulation);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cbxCity);
            this.Controls.Add(this.cbxTileType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbTileBuilding);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbUnit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TileInfo";
            this.Text = "TileInfo";
            this.Load += new System.EventHandler(this.TileInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbUnit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTileBuilding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxTileType;
        private System.Windows.Forms.ComboBox cbxCity;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbPopulation;
        private System.Windows.Forms.ListBox lbxInterior;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnInteriorDelete;
    }
}