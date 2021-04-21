using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;

namespace Simulator_BOTDA
{
    public class ReadChannelCfg
    {
        private readonly FileSystemWatcher _fsw;
        public object obj;
        private const string FileName = "ChannelCfg.config";
        private string ConfigPath;
        private bool readFileFlag;
        private Dictionary<string, BotdaEquip> existEquips;    //key为主机编号
        internal Dictionary<string, BotdaEquip> ExistEquips { get => existEquips; set => existEquips = value; }

             
        private static ReadChannelCfg _instance;
        public static ReadChannelCfg Create()
        {
            return _instance ?? (_instance = new ReadChannelCfg());
        }
        public ReadChannelCfg()
        {
            obj = new object();
            readFileFlag = false;
            ConfigPath = System.Environment.CurrentDirectory + "\\";
            if (_fsw == null)
            {
                _fsw = new FileSystemWatcher
                {
                    Path = ConfigPath,
                    Filter = FileName,
                    NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.Attributes
                };
                _fsw.Changed += new FileSystemEventHandler(FswChanged);
                _fsw.EnableRaisingEvents = true;
            }
        }

        public Dictionary<string, BotdaEquip> ReadFile()
        {
            if (existEquips == null)
                ExistEquips = ReadEquipCfg.Create().ReadFile();
         
         //   if (readFileFlag)
            {
                LoadOption();
                readFileFlag = false;
            }
            return ExistEquips;
        }

        private void LoadOption()
        {
            string ConfigFilePath = ConfigPath + FileName;
            ExeConfigurationFileMap ecf = new ExeConfigurationFileMap();
            ecf.ExeConfigFilename = ConfigFilePath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecf, ConfigurationUserLevel.None);
            string[] fileKeys = config.AppSettings.Settings.AllKeys;
            Dictionary<string, List<BotdaData>> fileEquipChannels = new Dictionary<string, List<BotdaData>>();    //key为主机编号，value为该设备的通道
            if(fileKeys.Length >0)
            {
                foreach (string key in fileKeys)     //将配置文件中的信息存入fileEquipChannels
                {
                    string value = GetIndexConfigValue(key);
                    if (value.Length >= 1)
                    {
                        if (key.Contains('_')) //主机编号_通道号
                        {
                            string equipnum = key.Split('_')[0].Trim();
                            string channelnum = key.Split('_')[1].Trim();
                            if(!fileEquipChannels.Keys.Contains(equipnum))
                            {
                                List<BotdaData> channels = new List<BotdaData>();
                                fileEquipChannels.Add(equipnum, channels);
                            }
                            string[] values = value.Split(';');
                            if (values.Length > 1 )
                            {
                                BotdaData bd = new BotdaData();
                                bd.channelNum = int.Parse(channelnum);
                                for (int j = 0; j < values.Length; j++)
                                {
                                    if (values[j].Contains('='))
                                    {
                                        string[] temp = values[j].Split('=');
                                        if (temp.Length > 1)
                                        {
                                            switch (temp[0].Trim().ToLower())
                                            {
                                                case "referencefrequencyshift":
                                                    if (temp[1] == null || temp[1] == "")
                                                        bd.referenceFrequencyShift = ChannelMange.DefaultReferenceFrequencyShift;
                                                    else
                                                        bd.referenceFrequencyShift = float.Parse(temp[1].Trim());
                                                    break;
                                                case "fiberlen":
                                                    if (temp[1] == null || temp[1] == "")
                                                        bd.fiberLen = ChannelMange.DefaultFiberLen;
                                                    else
                                                        bd.fiberLen = float.Parse(temp[1].Trim());
                                                    break;
                                            }
                                            if (!fileEquipChannels[equipnum].Contains(bd))
                                                fileEquipChannels[equipnum].Add(bd);
                                        }
                                    }
                                }      
                            }                           
                        }
                    }                    
                }

                foreach (KeyValuePair<string, BotdaEquip> kvp in ExistEquips)     //根据fileEquipChannels更新ExistEquips中的botdaFrequencyShiftDatas和channelNums
                {
                    if (fileEquipChannels.Keys.Contains(kvp.Key))
                    {
                        List<BotdaData> channels = fileEquipChannels[kvp.Key];
                        channels.Sort((x, y) => x.channelNum.CompareTo(y.channelNum));
                        if (kvp.Value.channelCount > fileEquipChannels[kvp.Key].Count)
                        {
                            int count = kvp.Value.channelCount - fileEquipChannels[kvp.Key].Count;
                            int maxchannelnum = channels[channels.Count - 1].channelNum;
                            for (int i = 0; i < count; i++)
                            {
                                BotdaData bd = new BotdaData();
                                bd.channelNum = maxchannelnum + i + 1;
                                bd.referenceFrequencyShift = ChannelMange.DefaultReferenceFrequencyShift;
                                bd.fiberLen = ChannelMange.DefaultFiberLen;
                                channels.Add(bd);
                            }
                        }
                        else
                        {
                            int count = fileEquipChannels[kvp.Key].Count - kvp.Value.channelCount;
                            int existcount = kvp.Value.channelCount;
                            for (int i = 0; i < count; i++)
                            {
                                int filecount = channels.Count;
                                channels.RemoveAt(filecount - 1);
                            }
                        }
                        //更新ExistEquips
                        for (int i = 0; i < channels.Count; i++)
                        {
                            int channelnum = channels[i].channelNum;
                            if (!kvp.Value.botdaFrequencyShiftDatas.Keys.Contains(channelnum))
                            {
                                kvp.Value.botdaFrequencyShiftDatas.Add(channelnum, channels[i]);
                            }
                            else
                                kvp.Value.botdaFrequencyShiftDatas[channelnum] = channels[i];

                            if (!kvp.Value.channelNums.Contains(channelnum))
                                kvp.Value.channelNums.Add(channelnum);
                        }
                    }
                }
            }
            else   //配置文件为空
            {
                foreach(KeyValuePair<string,BotdaEquip> kvp in ExistEquips)
                {
                    for (int i = 0; i < kvp.Value.channelCount; i++)
                    {
                        BotdaData bd = new BotdaData();
                        bd.channelNum = i + 1;
                        bd.referenceFrequencyShift = ChannelMange.DefaultReferenceFrequencyShift;
                        bd.fiberLen = ChannelMange.DefaultFiberLen;
                        kvp.Value.botdaFrequencyShiftDatas.Add(bd.channelNum, bd);
                        kvp.Value.channelNums.Add(bd.channelNum);
                    }
                }                
            }

            //根据ExistEquips的通道数更新botdaFrequencyShiftDatas和channelNums
            foreach (KeyValuePair<string, BotdaEquip> kvp in ExistEquips)
            {
                int channelcount = kvp.Value.channelCount;
                List<int> channelnums = kvp.Value.channelNums;
                channelnums.Sort((x, y) => x.CompareTo(y));
                if(channelcount < channelnums.Count)
                {
                    int deletecount = channelnums.Count - channelcount;                    
                    List<int> deletechannelnum = channelnums.GetRange(channelcount, deletecount);
                    kvp.Value.channelNums.RemoveRange(channelcount, deletecount);
                    for (int i=0;i< deletechannelnum.Count; i++)
                    {
                        kvp.Value.botdaFrequencyShiftDatas.Remove(deletechannelnum[i]);
                    }                    
                }   
                else
                {
                    int count = kvp.Value.channelNums.Count;
                    int addchannelcount = kvp.Value.channelCount - count;
                    int maxchannelnum = 0;
                    if (count > 0)
                    {
                        maxchannelnum = channelnums[count - 1];
                    }
                    else
                        maxchannelnum = 1;
                    for(int i=0;i<addchannelcount;i++)
                    {
                        BotdaData bd = new BotdaData();
                        bd.channelNum = maxchannelnum + i;
                        bd.referenceFrequencyShift = ChannelMange.DefaultReferenceFrequencyShift;
                        bd.fiberLen = ChannelMange.DefaultFiberLen;
                        kvp.Value.botdaFrequencyShiftDatas.Add(bd.channelNum, bd);
                        kvp.Value.channelNums.Add(bd.channelNum);
                    }
                }
            }
        }

        public string GetIndexConfigValue(string key)
        {
            string flag = "";
            string indexConfigPath = ConfigPath + FileName;
            if (string.IsNullOrEmpty(indexConfigPath))
                return flag = "-1";//配置文件为空
            if (!File.Exists(indexConfigPath))
                return flag = "-1";//配置文件不存在

            ExeConfigurationFileMap ecf = new ExeConfigurationFileMap();
            ecf.ExeConfigFilename = indexConfigPath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecf, ConfigurationUserLevel.None);
            try
            {
                flag = config.AppSettings.Settings[key].Value;
            }
            catch (Exception)
            {
                flag = "-2";
            }
            return flag;
        }

        private void FswChanged(object sender, FileSystemEventArgs e)
        {
            if (String.Compare(e.Name, FileName, StringComparison.OrdinalIgnoreCase) != 0) return;
            try
            {
                FileSystemWatcher watcher = (FileSystemWatcher)sender;
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    Thread th = new Thread(new ThreadStart(delegate ()
                    {
                        Thread.Sleep(1000);
                        watcher.EnableRaisingEvents = true;
                    }));
                    th.Start();
                    LoadOption();                    
                }
            }
            catch (Exception ex) { }
        }

        public void SetValue(Dictionary<string,BotdaEquip> existChannels)
        {
            //更新ChannelCfg配置文件 
            ExeConfigurationFileMap ecf = new ExeConfigurationFileMap();
            ecf.ExeConfigFilename = ConfigPath + FileName;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecf, ConfigurationUserLevel.None);
            string[] fileKeys = config.AppSettings.Settings.AllKeys;
            List<string> newkeys = new List<string>();
            List<string> equipnums = new List<string>();
            foreach(KeyValuePair<string, BotdaEquip> kvp in existChannels)
            {
                equipnums.Add(kvp.Key);
                kvp.Value.Stop();
            }
            //更新文件中已有的键值对
            for(int i=0; i< equipnums.Count;i++)
            {
                for(int j=0;j<existChannels[equipnums[i]].channelCount;j++)
                {
                    int channelnum = existChannels[equipnums[i]].channelNums[j];
                    string key = equipnums[i] + "_" + channelnum;

                    string value = "ReferenceFrequencyShift = " + existChannels[equipnums[i]].botdaFrequencyShiftDatas[channelnum].referenceFrequencyShift
                              + ";" + "FiberLen = " + existChannels[equipnums[i]].botdaFrequencyShiftDatas[channelnum].fiberLen + ";";
                    if (((IList)fileKeys).Contains(key))
                        config.AppSettings.Settings[key].Value = value;
                    else
                        config.AppSettings.Settings.Add(key, value);

                    if(!newkeys.Contains(key))
                        newkeys.Add(key);
                }                            
            }
            //删除文件中多余的键值对
            string[] delectkey = fileKeys.Except(newkeys).ToArray();
            if(delectkey != null)
            {
                for(int i = 0;i<(delectkey.Length);i++)               
                    config.AppSettings.Settings.Remove(delectkey[i]);                
            }            
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings"); 
            readFileFlag = true;
        }


    }
}
