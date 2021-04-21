using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Runtime.InteropServices;
using DevComponents.DotNetBar;
using ZedGraph;

namespace Simulator_BOTDA
{
    public partial class Main : Form
    {
        private EquipMange EquipMangeDlg;
        private ChannelMange ChannelMangeDlg;

        public static string oldEquipNum;
        public static string oldChannelNum;
        public static Dictionary<string, CurverIni> channelCurvers;
        private Dictionary<string, BotdaEquip> equips;

        public delegate void SetBtText(string channelNum);
        public static SetBtText ObjSetBtText;

        public delegate void RefreshZed(string channelNum);
        public static RefreshZed ObjRefreshZed;

        public Main()
        {
            InitializeComponent();

            EquipMangeDlg = new EquipMange();
            ChannelMangeDlg = new ChannelMange();
            channelCurvers = new Dictionary<string, CurverIni>();
            equips = new Dictionary<string, BotdaEquip>();
            ObjSetBtText += SetControlText;
            ObjRefreshZed += SetZedGraphInvoke;            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            cmb_chooseEquip.Items.Clear();
            cmb_chooseEquip.Tag = null;
            RefreshCmbChooseEquip();
            cmb_chooseEquip.Tag = 1;
            tabControl1.Tag = null;

            serverip.Value = "192.168.1.81";
        }

        private void Menu_EquipMange_Click(object sender, EventArgs e)
        {
            EquipMangeDlg.ShowDialog();
        }

        private void Menu_ChannelMange_Click(object sender, EventArgs e)
        {
            ChannelMangeDlg.ShowDialog();
        }

        private void bt_Start_Click(object sender, EventArgs e)
        {
            //更新cmb_chooseequip
            RefreshCmbChooseEquip();
            if (bt_Start.Text == "启 动")
            {
                IPAddress serverIP;
                bool flag = IPAddress.TryParse(serverip.Text, out serverIP);
                if (flag)
                {
                    Server.Create().ServerStart(serverIP);
                    bt_Start.Text = "停 止";
                }
            }
            else
            {
                Server.Create().ServerStop();
                bt_Start.Text = "启 动";
            }
        }

        private void RefreshTableControl(string equipnum, BotdaEquip Channels)
        {
            tabControl1.Tag = null; 

            Size size = tabControl1.Tabs[0].AttachedControl.Size;
            tabControl1.SuspendLayout();
            this.SuspendLayout();

            tabControl1.Tabs.Clear();
            tabControl1.TabAlignment = eTabStripAlignment.Top;
            List<int> channelNums = Channels.channelNums;

            if (channelNums.Count == 0)
            {
                TabItem ti = tabControl1.CreateTab("通道1");
                ti.Name = "通道1";
                ti.AttachedControl.Size = size;
                ti.AttachedControl.Dock = DockStyle.Fill;
                ZedGraphControl zed = new ZedGraphControl();
                CurverIni curver = new CurverIni(ti, zed);

                if (channelCurvers.Keys.Contains("1"))
                    channelCurvers["1"] = curver;
                else
                    channelCurvers.Add("1", curver);
            }
            else
            {
                int[] num = channelNums.ToArray();
                Array.Sort(num);
                for (int i = 0; i < channelNums.Count; i++)
                {
                    TabItem ti = tabControl1.CreateTab("通道" + num[i]);
                    ti.Name = "通道" + num[i];
                    ti.AttachedControl.Size = size;
                    ti.AttachedControl.Dock = DockStyle.Fill;
                    ZedGraphControl zed = new ZedGraphControl();
                    CurverIni curver = new CurverIni(ti, zed);

                    if (channelCurvers.Keys.Contains(num[i].ToString()))
                        channelCurvers[num[i].ToString()] = curver;
                    else
                        channelCurvers.Add(num[i].ToString(), curver);
                }
            }
            tabControl1.SelectedTabIndex = 0;
            tabControl1.ResumeLayout(true);
            this.ResumeLayout(true);

            tabControl1.Tag = 1;
        }

        private void RefreshCmbChooseEquip()
        {
            equips = ReadChannelCfg.Create().ReadFile();
            cmb_chooseEquip.Items.Clear();
            if (equips.Keys.Count > 0)
            {
                foreach (KeyValuePair<string, BotdaEquip> kvp in equips)
                {
                    cmb_chooseEquip.Items.Add("BOTDA:" + kvp.Key);
                }
                if (cmb_chooseEquip.Items.Count > 0)
                {
                    cmb_chooseEquip.SelectedIndex = 0;
                    int index = cmb_chooseEquip.SelectedItem.ToString().IndexOf(":");
                    oldEquipNum = cmb_chooseEquip.SelectedItem.ToString().Substring(index + 1);
                    oldChannelNum = tabControl1.SelectedTab.Name.Substring(2);
                    RefreshTableControl(oldEquipNum, equips[oldEquipNum]);
                }
            }
        }

        private void SetZedGraphInvoke(string channelNum)
        {
            if (tabControl1.Tabs["通道" + channelNum].AttachedControl.Controls.Count > 0)
            {
                if (tabControl1.Tabs["通道" + channelNum].AttachedControl.Controls[0].InvokeRequired)
                    tabControl1.Tabs["通道" + channelNum].AttachedControl.Controls[0].Invoke(ObjRefreshZed, channelNum);
                else
                {
                    ((ZedGraphControl)tabControl1.Tabs["通道" + channelNum].AttachedControl.Controls[0]).MasterPane.AxisChange();
                    ((ZedGraphControl)tabControl1.Tabs["通道" + channelNum].AttachedControl.Controls[0]).Refresh();
                }
            }
        }


        private void SetControlText(string value)
        {
            if (this.bt_Start.InvokeRequired)
                this.bt_Start.Invoke(ObjSetBtText, value);
            else
            {
                /*
                if (value == "停 止")
                {
                    foreach (KeyValuePair<string, BotdaEquip> kvp in equips)
                        kvp.Value.Stop();
                }
                */
                this.bt_Start.Text = value;
            }
        }

        private void cmb_chooseEquip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_chooseEquip.Tag != null)
            {
                if (bt_Start.Text == "启 动")
                    equips = ReadChannelCfg.Create().ReadFile();
                //刷新tabitem的个数
                int index = cmb_chooseEquip.SelectedItem.ToString().IndexOf(":");
                string equipnum = cmb_chooseEquip.SelectedItem.ToString().Substring(index + 1);
                int channelcount = equips[equipnum].channelCount;
                oldEquipNum = equipnum;
               
                RefreshTableControl(equipnum, equips[equipnum]);
                oldChannelNum = tabControl1.SelectedTab.Name.Substring(2);
            }
        }

        private void tabControl1_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
        {
            if (tabControl1.Tag != null)
            {                
                if(tabControl1.Tabs.Count > 0 && tabControl1.SelectedTab.Name.Contains("通道"))
                {
                    oldChannelNum = tabControl1.SelectedTab.Name.Substring(2);
                }                    
            }                
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(bt_Start.Text == "停 止")
            {
                Server.Create().ServerStop();                
            }
        }
    }
}
