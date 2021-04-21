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


namespace Simulator_BOTDA
{
    public partial class ChannelMange : Form
    {   
        public const UInt16 DefaultReferenceFrequencyShift = 5; //默认参考频移
        public const UInt16 DefaultFiberLen = 1000;     //默认光纤长度
        private string oldEquipNum;

        private Dictionary<string, BotdaEquip> existEquips;   //利用ChannelInfos的Flag判断该通道是否有效
        internal Dictionary<string, BotdaEquip> ExistEquips { get => existEquips; set => existEquips = value; }
        public ChannelMange()
        {            
            InitializeComponent();
            ExistEquips = new Dictionary<string, BotdaEquip>();
        }     
        
        private void AddChannel_Load(object sender, EventArgs e)
        {
            cmb_ChooseEquip.Tag = null;
            ExistEquips = ReadChannelCfg.Create().ReadFile();
            cmb_ChooseEquip.Items.Clear();
            foreach(KeyValuePair<string, BotdaEquip> kvp in ExistEquips)
            {
                cmb_ChooseEquip.Items.Add("BOTDA:" + kvp.Key);                              
            }
            if (cmb_ChooseEquip.Items.Count > 0)
            {
                cmb_ChooseEquip.SelectedIndex = 0;
                int index = cmb_ChooseEquip.SelectedItem.ToString().IndexOf(":");
                oldEquipNum = cmb_ChooseEquip.SelectedItem.ToString().Substring(index + 1);
            }

            foreach (KeyValuePair<string, BotdaEquip> kvp in ExistEquips)
            {
                int index = cmb_ChooseEquip.SelectedItem.ToString().IndexOf(":");
                string equipnum = ((string)cmb_ChooseEquip.SelectedItem).Substring(index + 1);
                if (kvp.Key.Contains(equipnum))
                {
                    DGV_ChannelInfo.RowCount = ExistEquips[kvp.Key].channelCount;
                    List<int> channelnum = kvp.Value.channelNums;
                    channelnum.Sort((x, y) => x.CompareTo(y));
                    for (int i = 0; i < channelnum.Count; i++)
                    {
                        DGV_ChannelInfo.Rows[i].Cells[0].Value = channelnum[i];
                        DGV_ChannelInfo.Rows[i].Cells[1].Value = kvp.Value.botdaFrequencyShiftDatas[channelnum[i]].referenceFrequencyShift;
                        DGV_ChannelInfo.Rows[i].Cells[2].Value = kvp.Value.botdaFrequencyShiftDatas[channelnum[i]].fiberLen;
                    }
                }
            }
            DGV_ChannelInfo.Columns[0].ReadOnly = true;
            cmb_ChooseEquip.Tag = 1;
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
        private void DGV_ChannelInfo_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewX grid = (DataGridViewX)sender;
            TextBox tx = e.Control as TextBox;
            if (grid.CurrentCell.ColumnIndex == 0 )
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
            //保持当前设备的通道信息   
            int index = cmb_ChooseEquip.SelectedItem.ToString().IndexOf(":");
            oldEquipNum = cmb_ChooseEquip.SelectedItem.ToString().Substring(index + 1);
            Dictionary<int, BotdaData> channels = new Dictionary<int, BotdaData>();
            for (int i = 0; i < DGV_ChannelInfo.RowCount; i++)
            {
                int channelnum = int.Parse(DGV_ChannelInfo.Rows[i].Cells[0].Value.ToString());
                BotdaData channel = new BotdaData(channelnum);
                channel.channelNum = channelnum;
                channel.referenceFrequencyShift = float.Parse(DGV_ChannelInfo.Rows[i].Cells[1].Value.ToString());
                channel.fiberLen = float.Parse(DGV_ChannelInfo.Rows[i].Cells[2].Value.ToString());

                if (!channels.Keys.Contains(channelnum))
                    channels.Add(channelnum, channel);
                else
                    channels[channelnum] = channel;
            }
            if (ExistEquips.Keys.Contains(oldEquipNum))
            {
                ExistEquips[oldEquipNum].botdaFrequencyShiftDatas = channels;
            }
            //更新配置文件
            ReadChannelCfg.Create().SetValue(ExistEquips);
        }

        private void cmb_ChooseEquip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_ChooseEquip.Tag != null)
            {
                //切换设备前需要保存当前设备所有通道信息
                int index = cmb_ChooseEquip.SelectedItem.ToString().IndexOf(":");
                string num = cmb_ChooseEquip.SelectedItem.ToString().Substring(index + 1);
                if (oldEquipNum != null)
                {
                    Dictionary<int, BotdaData> channels = new Dictionary<int, BotdaData>();
                    for (int i = 0; i < DGV_ChannelInfo.RowCount; i++)
                    {
                        int cn = int.Parse(DGV_ChannelInfo.Rows[i].Cells[0].Value.ToString());
                        BotdaData channel = new BotdaData(cn);
                        channel.channelNum = cn;
                        channel.referenceFrequencyShift = float.Parse(DGV_ChannelInfo.Rows[i].Cells[1].Value.ToString());
                        channel.fiberLen = float.Parse(DGV_ChannelInfo.Rows[i].Cells[2].Value.ToString());

                        if (!channels.Keys.Contains(cn))
                            channels.Add(cn, channel);
                        else
                            channels[cn] = channel;
                    }
                    if (ExistEquips.Keys.Contains(oldEquipNum))
                    {
                        ExistEquips[oldEquipNum].botdaFrequencyShiftDatas = channels;
                    }     
                    ReadChannelCfg.Create().SetValue(ExistEquips);

                    oldEquipNum = num;
                    //更新Datagridview
                    DGV_ChannelInfo.Rows.Clear();
                    DGV_ChannelInfo.RowCount = ExistEquips[oldEquipNum].channelCount;
                    List<int> channelnum = ExistEquips[oldEquipNum].channelNums;
                    channelnum.Sort((x, y) => x.CompareTo(y));
                    for (int i = 0; i < channelnum.Count; i++)
                    {
                        DGV_ChannelInfo.Rows[i].Cells[0].Value = channelnum[i];
                        DGV_ChannelInfo.Rows[i].Cells[1].Value = ExistEquips[oldEquipNum].botdaFrequencyShiftDatas[channelnum[i]].referenceFrequencyShift;
                        DGV_ChannelInfo.Rows[i].Cells[2].Value = ExistEquips[oldEquipNum].botdaFrequencyShiftDatas[channelnum[i]].fiberLen;
                    }                    
                }
            }
        }




    }
}
