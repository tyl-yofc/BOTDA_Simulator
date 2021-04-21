using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Net;

namespace Simulator_BOTDA
{
    public class ReadEquipCfg
    {
        private readonly FileSystemWatcher _fsw;
        public object obj;
        private string ConfigPath;
        private const string FileName = "EquipCfg.config";
        private bool readFileFlag;
        private Dictionary<string, BotdaEquip> existEquips;       //配置的设备，key为主机编号    
        internal Dictionary<string, BotdaEquip> ExistEquips { get => existEquips; set => existEquips = value; }


        private static ReadEquipCfg _instance;
        public static ReadEquipCfg Create()
        {
            return _instance ?? (_instance = new ReadEquipCfg());
        }       

        public ReadEquipCfg()
        {
            obj = new object();
            readFileFlag = false;
            ConfigPath = System.Environment.CurrentDirectory + "\\";
            ExistEquips = new Dictionary<string, BotdaEquip>();
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
          //  if (readFileFlag)
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
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                string value = GetIndexConfigValue(key);
                if (value.Length >= 1)
                {
                    string[] values = value.Split(';');
                    if (values.Length >= 1)
                    {
                        BotdaEquip equipInfo = new BotdaEquip();
                        if (ExistEquips.TryGetValue(key, out equipInfo))   //字典中有该分区，直接用新的值替换
                        {
                            //更新该通道的ChannelInfos
                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] temp = values[i].Split('=');
                                if (temp.Length > 1)
                                {
                                    if (temp[0].Trim().ToLower().Contains("channelcount"))
                                        equipInfo.channelCount = ushort.Parse(temp[1]);
                                    else if(temp[0].Trim().ToLower().Contains("refractiveindex"))
                                        equipInfo.refractiveIndex = float.Parse(temp[1]);
                                    else if (temp[0].Trim().ToLower().Contains("samplingresolution"))
                                        equipInfo.samplingResolution = float.Parse(temp[1]);
                                    else if (temp[0].Trim().ToLower().Contains("serverport"))
                                        equipInfo.simpleTcpClient._remotePort = int.Parse(temp[1]);
                                }
                            }
                        }
                        else
                        {
                            equipInfo = new BotdaEquip();
                            equipInfo.equipNum = int.Parse(key);
                            //新增该通道的ChannelInfos
                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] temp = values[i].Split('=');
                                if (temp.Length > 1)
                                {
                                    if (temp[0].Trim().ToLower().Contains("channelcount"))
                                        equipInfo.channelCount = ushort.Parse(temp[1]);
                                    else if (temp[0].Trim().ToLower().Contains("refractiveindex"))
                                        equipInfo.refractiveIndex = float.Parse(temp[1]);
                                    else if (temp[0].Trim().ToLower().Contains("samplingresolution"))
                                        equipInfo.samplingResolution = float.Parse(temp[1]);
                                    else if (temp[0].Trim().ToLower().Contains("serverport"))
                                        equipInfo.simpleTcpClient._remotePort = int.Parse(temp[1]);
                                }
                            }
                            ExistEquips.Add(key, equipInfo);
                        }
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
            catch (Exception ex){}
        }

        public void SetValue(Dictionary<string, BotdaEquip> existChannels)
        {
            //更新ChannelCfg配置文件 
            ExeConfigurationFileMap ecf = new ExeConfigurationFileMap();
            ecf.ExeConfigFilename = ConfigPath + FileName;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecf, ConfigurationUserLevel.None);
            string[] fileKeys = config.AppSettings.Settings.AllKeys;
            List<string> newkeys = new List<string>();
            foreach (KeyValuePair<string, BotdaEquip> kvp in existChannels)
            {
                newkeys.Add(kvp.Key);
                kvp.Value.Stop();
            }
            //更新文件中已有的键值对
            for (int i = 0; i < newkeys.Count; i++)
            {
                string value = "ChannelCount = " + existChannels[newkeys[i]].channelCount + ";" + "RefractiveIndex = " + (existChannels[newkeys[i]].refractiveIndex).ToString() + ";" 
                    + "SamplingResolution = " + existChannels[newkeys[i]].samplingResolution + "; ServerPort = "+(existChannels[newkeys[i]].simpleTcpClient._remotePort).ToString() + ";";                    
                if (((IList)fileKeys).Contains(newkeys[i]))
                    config.AppSettings.Settings[newkeys[i]].Value = value;
                else
                    config.AppSettings.Settings.Add(newkeys[i], value);
            }
            //删除文件中多余的键值对
            string[] delectkey = fileKeys.Except(newkeys).ToArray();
            if (delectkey != null)
            {
                for (int i = 0; i < (delectkey.Length); i++)
                    config.AppSettings.Settings.Remove(delectkey[i]);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            readFileFlag = true;
        }


    }
}
