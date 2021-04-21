namespace Simulator_BOTDA
{
    partial class EquipMange
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
            this.bt_AddRow = new DevComponents.DotNetBar.ButtonX();
            this.bt_RemoveRow = new DevComponents.DotNetBar.ButtonX();
            this.bt_Save = new System.Windows.Forms.Button();
            this.bt_Cancel = new System.Windows.Forms.Button();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.折射率 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.Column2,
            this.折射率,
            this.Column3,
            this.Column4});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_ChannelInfo.DefaultCellStyle = dataGridViewCellStyle5;
            this.DGV_ChannelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.DGV_ChannelInfo.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.DGV_ChannelInfo.Location = new System.Drawing.Point(0, 0);
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
            this.DGV_ChannelInfo.Size = new System.Drawing.Size(483, 188);
            this.DGV_ChannelInfo.TabIndex = 0;
            this.DGV_ChannelInfo.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DGV_ChannelInfo_EditingControlShowing);
            this.DGV_ChannelInfo.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DGV_ChannelInfo_RowPostPaint);
            // 
            // bt_AddRow
            // 
            this.bt_AddRow.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.bt_AddRow.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.bt_AddRow.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_AddRow.Location = new System.Drawing.Point(396, 194);
            this.bt_AddRow.Name = "bt_AddRow";
            this.bt_AddRow.Size = new System.Drawing.Size(35, 23);
            this.bt_AddRow.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.bt_AddRow.TabIndex = 1;
            this.bt_AddRow.Text = "+";
            this.bt_AddRow.Click += new System.EventHandler(this.bt_AddRow_Click);
            // 
            // bt_RemoveRow
            // 
            this.bt_RemoveRow.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.bt_RemoveRow.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.bt_RemoveRow.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_RemoveRow.Location = new System.Drawing.Point(442, 194);
            this.bt_RemoveRow.Name = "bt_RemoveRow";
            this.bt_RemoveRow.Size = new System.Drawing.Size(30, 23);
            this.bt_RemoveRow.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.bt_RemoveRow.TabIndex = 2;
            this.bt_RemoveRow.Text = "-";
            this.bt_RemoveRow.Click += new System.EventHandler(this.bt_RemoveRow_Click);
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(132, 218);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(91, 23);
            this.bt_Save.TabIndex = 3;
            this.bt_Save.Text = "保 存";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(273, 219);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(87, 23);
            this.bt_Cancel.TabIndex = 4;
            this.bt_Cancel.Text = "取 消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // Column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column1.HeaderText = "主机编号";
            this.Column1.Name = "Column1";
            this.Column1.Width = 85;
            // 
            // Column2
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column2.HeaderText = "通道数";
            this.Column2.Name = "Column2";
            this.Column2.Width = 85;
            // 
            // 折射率
            // 
            this.折射率.HeaderText = "折射率";
            this.折射率.Name = "折射率";
            this.折射率.Width = 85;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "采样间隔";
            this.Column3.Name = "Column3";
            this.Column3.Width = 85;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "端口号";
            this.Column4.Name = "Column4";
            // 
            // EquipMange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 254);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_RemoveRow);
            this.Controls.Add(this.bt_AddRow);
            this.Controls.Add(this.DGV_ChannelInfo);
            this.Name = "EquipMange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设备管理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddChannel_FormClosed);
            this.Load += new System.EventHandler(this.AddEquip_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ChannelInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX DGV_ChannelInfo;
        private DevComponents.DotNetBar.ButtonX bt_AddRow;
        private DevComponents.DotNetBar.ButtonX bt_RemoveRow;
        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn 折射率;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}