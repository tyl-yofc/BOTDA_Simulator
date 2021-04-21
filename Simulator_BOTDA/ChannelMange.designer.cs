namespace Simulator_BOTDA
{
    partial class ChannelMange
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV_ChannelInfo = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_ChooseEquip = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ChannelInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_ChannelInfo
            // 
            this.DGV_ChannelInfo.AllowUserToAddRows = false;
            this.DGV_ChannelInfo.AllowUserToResizeColumns = false;
            this.DGV_ChannelInfo.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.DGV_ChannelInfo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_ChannelInfo.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DGV_ChannelInfo.CausesValidation = false;
            this.DGV_ChannelInfo.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_ChannelInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DGV_ChannelInfo.ColumnHeadersHeight = 25;
            this.DGV_ChannelInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DGV_ChannelInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column5,
            this.Column4});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_ChannelInfo.DefaultCellStyle = dataGridViewCellStyle5;
            this.DGV_ChannelInfo.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.DGV_ChannelInfo.Location = new System.Drawing.Point(1, 40);
            this.DGV_ChannelInfo.MultiSelect = false;
            this.DGV_ChannelInfo.Name = "DGV_ChannelInfo";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_ChannelInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.DGV_ChannelInfo.RowHeadersWidth = 40;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.DGV_ChannelInfo.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.DGV_ChannelInfo.RowTemplate.Height = 25;
            this.DGV_ChannelInfo.Size = new System.Drawing.Size(299, 194);
            this.DGV_ChannelInfo.TabIndex = 0;
            this.DGV_ChannelInfo.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DGV_ChannelInfo_EditingControlShowing);
            this.DGV_ChannelInfo.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DGV_ChannelInfo_RowPostPaint);
            // 
            // Column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column1.HeaderText = "通道号";
            this.Column1.Name = "Column1";
            this.Column1.Width = 85;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "参考频移";
            this.Column5.Name = "Column5";
            this.Column5.Width = 85;
            // 
            // Column4
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column4.HeaderText = "光纤长度(m)";
            this.Column4.Name = "Column4";
            this.Column4.Width = 85;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "设备号：";
            // 
            // cmb_ChooseEquip
            // 
            this.cmb_ChooseEquip.DisplayMember = "Text";
            this.cmb_ChooseEquip.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_ChooseEquip.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_ChooseEquip.FormattingEnabled = true;
            this.cmb_ChooseEquip.ItemHeight = 17;
            this.cmb_ChooseEquip.Location = new System.Drawing.Point(73, 7);
            this.cmb_ChooseEquip.Name = "cmb_ChooseEquip";
            this.cmb_ChooseEquip.Size = new System.Drawing.Size(193, 23);
            this.cmb_ChooseEquip.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cmb_ChooseEquip.TabIndex = 6;
            this.cmb_ChooseEquip.SelectedIndexChanged += new System.EventHandler(this.cmb_ChooseEquip_SelectedIndexChanged);
            // 
            // ChannelMange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 235);
            this.Controls.Add(this.cmb_ChooseEquip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DGV_ChannelInfo);
            this.Name = "ChannelMange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "通道管理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddChannel_FormClosed);
            this.Load += new System.EventHandler(this.AddChannel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ChannelInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX DGV_ChannelInfo;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cmb_ChooseEquip;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}