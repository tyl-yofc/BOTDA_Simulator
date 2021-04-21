using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;
using System.Collections;
using System.Net;

namespace Simulator_BOTDA
{
    public partial class EquipMange : Form
    {
        
        private UInt16 DefaultChannelCount = 4;               //默认通道个数
        private float DefaultReferenceIndex = 1.4683f;       //默认折射率
        private float DefaultSampleResolution = 0.05f;            //默认采样间隔
        private Dictionary<string, BotdaEquip> existEquips;   //key为设备编号
        internal Dictionary<string, BotdaEquip> ExistEquips { get => existEquips; set => existEquips = value; }
        public EquipMange()
        {            
            InitializeComponent();           
            ExistEquips = new Dictionary<string, BotdaEquip>();
        }
        private void AddEquip_Load(object sender, EventArgs e)
        {
            ExistEquips = ReadEquipCfg.Create().ExistEquips;

            int rowindex = 0;
            DGV_ChannelInfo.RowCount = ExistEquips.Count;
            foreach (KeyValuePair<string, BotdaEquip> kvp in ExistEquips)
            {
                DGV_ChannelInfo.Rows[rowindex].Cells[0].Value = kvp.Key;
                DGV_ChannelInfo.Rows[rowindex].Cells[1].Value = kvp.Value.channelCount;
                DGV_ChannelInfo.Rows[rowindex].Cells[2].Value = kvp.Value.refractiveIndex.ToString();
                DGV_ChannelInfo.Rows[rowindex].Cells[3].Value = kvp.Value.samplingResolution.ToString();
                DGV_ChannelInfo.Rows[rowindex].Cells[4].Value = kvp.Value.simpleTcpClient._remotePort.ToString();
                rowindex++;
            }
        }

        /// <summary>
        /// 序号列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGV_ChannelInfo_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(((DataGridViewX)sender).RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_AddRow_Click(object sender, EventArgs e)
        {
           DGV_ChannelInfo.Rows.Add();
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_RemoveRow_Click(object sender, EventArgs e)
        {
            int num = 0;
            for (int i = 0; i < DGV_ChannelInfo.RowCount; i++)
            {
                if ((string)DGV_ChannelInfo.Rows[i].Cells[0].Value != "" && (string)DGV_ChannelInfo.Rows[i].Cells[0].Value != null)
                    num++;
            }

            if ((string)DGV_ChannelInfo.CurrentRow.Cells[0].Value != "" && (string)DGV_ChannelInfo.CurrentRow.Cells[0].Value != null)
            {                
                if ( num > 1)
                {
                    string msg = "确定需要删除主机" + DGV_ChannelInfo.CurrentRow.Cells[0].Value + " 吗？";
                    if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                    {
                        DGV_ChannelInfo.Rows.Remove(DGV_ChannelInfo.CurrentRow);
                    }
                }
            }
            else
                DGV_ChannelInfo.Rows.Remove(DGV_ChannelInfo.CurrentRow);
        }

        /// <summary>
        /// 保存DataGridView中的通道信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_Save_Click(object sender, EventArgs e)
        {            
            List<string> newkeys = new List<string>();
           
            for (int i=0;i<DGV_ChannelInfo.RowCount;i++)
            {
                string equipnum = (string)DGV_ChannelInfo.Rows[i].Cells[0].Value;
                if (equipnum != null && int.Parse(equipnum) > 0)
                {
                    if (!newkeys.Contains(equipnum))
                        newkeys.Add(equipnum);
                    
                    if (ExistEquips.Keys.Contains(equipnum))     //更新设备信息
                    {
                        if (DGV_ChannelInfo.Rows[i].Cells[1].Value == null || DGV_ChannelInfo.Rows[i].Cells[1].Value == "")
                            ExistEquips[equipnum].channelCount = DefaultChannelCount;      //默认
                        else
                            ExistEquips[equipnum].channelCount = int.Parse(DGV_ChannelInfo.Rows[i].Cells[1].Value.ToString());

                        if (DGV_ChannelInfo.Rows[i].Cells[2].Value == null || DGV_ChannelInfo.Rows[i].Cells[2].Value == "")
                            ExistEquips[equipnum].refractiveIndex = DefaultReferenceIndex;      //默认
                        else
                            ExistEquips[equipnum].refractiveIndex = float.Parse(DGV_ChannelInfo.Rows[i].Cells[2].Value.ToString());

                        if (DGV_ChannelInfo.Rows[i].Cells[3].Value == null || DGV_ChannelInfo.Rows[i].Cells[3].Value == "")
                            ExistEquips[equipnum].samplingResolution = DefaultSampleResolution;      //默认
                        else
                            ExistEquips[equipnum].samplingResolution = float.Parse(DGV_ChannelInfo.Rows[i].Cells[3].Value.ToString());

                        if (DGV_ChannelInfo.Rows[i].Cells[4].Value == null || DGV_ChannelInfo.Rows[i].Cells[4].Value == "")
                            ExistEquips[equipnum].simpleTcpClient._remotePort = 0;      //默认
                        else
                            ExistEquips[equipnum].simpleTcpClient._remotePort = int.Parse(DGV_ChannelInfo.Rows[i].Cells[4].Value.ToString());

                        ExistEquips[equipnum].equipNum = int.Parse(equipnum);
                    }
                    else     //新增设备
                    {
                      
                        BotdaEquip equipInfo = new BotdaEquip();
                        if (DGV_ChannelInfo.Rows[i].Cells[1].Value == null || DGV_ChannelInfo.Rows[i].Cells[1].Value == "")
                            equipInfo.channelCount = DefaultChannelCount;      //默认
                        else
                            equipInfo.channelCount = int.Parse(DGV_ChannelInfo.Rows[i].Cells[1].Value.ToString());

                        if (DGV_ChannelInfo.Rows[i].Cells[2].Value == null || DGV_ChannelInfo.Rows[i].Cells[2].Value == "")
                            equipInfo.refractiveIndex = DefaultReferenceIndex;      //默认
                        else
                            equipInfo.refractiveIndex = float.Parse(DGV_ChannelInfo.Rows[i].Cells[2].Value.ToString());

                        if (DGV_ChannelInfo.Rows[i].Cells[3].Value == null || DGV_ChannelInfo.Rows[i].Cells[3].Value == "")
                            equipInfo.samplingResolution = DefaultSampleResolution;      //默认
                        else
                            equipInfo.samplingResolution = float.Parse(DGV_ChannelInfo.Rows[i].Cells[3].Value.ToString());

                        if (DGV_ChannelInfo.Rows[i].Cells[4].Value == null || DGV_ChannelInfo.Rows[i].Cells[4].Value == "")
                            equipInfo.simpleTcpClient._remotePort = 0;      //默认
                        else
                            equipInfo.simpleTcpClient._remotePort = int.Parse(DGV_ChannelInfo.Rows[i].Cells[4].Value.ToString());

                        equipInfo.equipNum = int.Parse(equipnum);
                        ExistEquips.Add(equipnum, equipInfo);
                    }
                }
            }
            //删除字典中多余的主机
            if (newkeys.Count > 0)
            {
                string[] oldkeys = ExistEquips.Keys.ToArray();
                string[] dif = oldkeys.Except(newkeys).ToArray();
                if (dif.Length > 0)
                {
                    for (int i = 0; i < dif.Length; i++)
                    {
                        ExistEquips.Remove(dif[i]);
                    }
                }
            }
            this.Close();            
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void DGV_ChannelInfo_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewX grid = (DataGridViewX)sender;
            TextBox tx = e.Control as TextBox;

            if (grid.CurrentCell.ColumnIndex == 0 || grid.CurrentCell.ColumnIndex == 1 || grid.CurrentCell.ColumnIndex == 4)
            {
                tx.KeyPress -= new KeyPressEventHandler(tx_KeyPress1);
                tx.KeyPress -= new KeyPressEventHandler(tx_KeyPress);
                tx.KeyPress += new KeyPressEventHandler(tx_KeyPress1);
            }
            else
            {
                tx.KeyPress -= new KeyPressEventHandler(tx_KeyPress);
                tx.KeyPress -= new KeyPressEventHandler(tx_KeyPress1);
                tx.KeyPress += new KeyPressEventHandler(tx_KeyPress);
            }
                               
        }
        private void tx_KeyPress1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//允许输入退格键 
            {
                int len = ((TextBox)sender).Text.Length;
                if (len < 1 && e.KeyChar == '0')
                    e.Handled = true;
                else if ((e.KeyChar < '0') || (e.KeyChar > '9'))//允许输入0-9数字                 
                    e.Handled = true;
            }                
       }
        private void tx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //允许输入数字、小数点、删除键
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != (char)('.'))
                e.Handled = true;  
            //小数点只能输入一次
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text.IndexOf('.') != -1)
                e.Handled = true;
            if(e.KeyChar == (char)('.') && ((TextBox)sender).Text == "")
                e.Handled = true;
            //第一位是0，第二位必须为小数点
            if (e.KeyChar != (char)('.') && e.KeyChar != 8 && ((TextBox)sender).Text == "0")
                e.Handled = true;
        }

        private void AddChannel_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReadChannelCfg.Create().ExistEquips = ExistEquips;
            //更新配置文件
            ReadEquipCfg.Create().SetValue(ExistEquips);
        }

        
    }
}
