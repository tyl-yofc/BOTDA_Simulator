using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml.Serialization;

namespace DTS.ConfigFiles
{
    class ReadAlarmZoneCfg
    {
        private const string FileName = "AlarmZoneCfg.config";
        private readonly FileSystemWatcher _fsw;
        public object obj;        
        private string ConfigPath;
        private Dictionary<string, List<ChannelInfos>> channelZoneInfos;      //key为主机编号,AlarmZonCfg配置文件信息
        internal Dictionary<string, List<ChannelInfos>> ChannelZoneInfos { get => channelZoneInfos; set => channelZoneInfos = value; }


    //    private Dictionary<string, List<ChannelInfos>> existChannels;      //key为主机编号,channelCfg配置文件信息
    //    internal Dictionary<string, List<ChannelInfos>> ExistChannels { get => existChannels; set => existChannels = value; }

        private bool readFileFlag;
        private static ReadAlarmZoneCfg _instance;
        public static ReadAlarmZoneCfg Create()
        {
            return _instance ?? (_instance = new ReadAlarmZoneCfg());
        }       
        public ReadAlarmZoneCfg()
        {
            obj = new object();
            readFileFlag = false;
            ConfigPath = System.Environment.CurrentDirectory + "\\";
       //     ExistChannels = new Dictionary<string, List<ChannelInfos>>();
            ChannelZoneInfos = new Dictionary<string, List<ChannelInfos>>();
            LoadOption();
       //     CompareChannelInfo(ExistChannels, ChannelZoneInfos);
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

        public Dictionary<string, List<ChannelInfos>> ReadFile()
        {
            if (readFileFlag)
            {
                //    ExistChannels = ReadChannelCfg.Create().ReadFile();
                LoadOption();
                readFileFlag = false;
                //     CompareChannelInfo(ExistChannels, ChannelZoneInfos);
            }            
            return ChannelZoneInfos;
        }

        public void LoadOption()
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
                    string[] keys = key.Split('_');
                    string equipnum;     //主机编号
                    string channelnum;    //通道号
                    string zonenum;      //分区号
                    if (keys.Length >= 3)    //主机编号_通道号_分区号
                    {
                        equipnum = keys[0].Trim();
                        channelnum = keys[1].Trim();
                        zonenum = keys[2].Trim();

                        if (!ChannelZoneInfos.Keys.Contains(equipnum))   //集合中无该设备
                        {
                            List<ChannelInfos> zonetempinfo = new List<ChannelInfos>();
                            ChannelZoneInfos.Add(equipnum, zonetempinfo);
                        }

                        int channelNum = -1;
                        int zoneNum = -1;
                        if (int.TryParse(channelnum, out channelNum) && int.TryParse(zonenum, out zoneNum))
                        {
                            string[] values = value.Split(';');
                            if (values.Length > 1)
                            {
                                ChannelInfos channel = ChannelZoneInfos[equipnum].Find(delegate (ChannelInfos c) { return c.ChannelNum == ushort.Parse(channelnum); });
                                int channelindex = ChannelZoneInfos[equipnum].FindIndex(item => item.ChannelNum == ushort.Parse(channelnum));
                                if (channel == null)    //集合中无该通道
                                {
                                    channel = new ChannelInfos();
                                    channel.ChannelNum = ushort.Parse(channelnum);
                                    channel.ZoneCount = 1;
                                    channel.BaseTemp = 30;
                                    channel.MeasureTime = 5;
                                    channel.SampleInterval = 40;
                                    channel.FiberLen = 10000;
                                    channel.ZoneTempInfos = new List<ZoneTempInfo>();
                                    channelZoneInfos[equipnum].Add(channel);
                                    channelindex = channelZoneInfos[equipnum].Count - 1;
                                }

                                ZoneTempInfo zone = ChannelZoneInfos[equipnum][channelindex].ZoneTempInfos.Find(delegate (ZoneTempInfo z) { return z.ZoneNumber == ushort.Parse(zonenum); });
                                int zoneindex = ChannelZoneInfos[equipnum][channelindex].ZoneTempInfos.FindIndex(item => item.ZoneNumber == ushort.Parse(zonenum));
                                if(zoneindex == -1)     //集合中无该分区
                                {
                                    zone = new ZoneTempInfo();
                                    zone.ZoneNumber = ushort.Parse(zonenum);
                                    ChannelZoneInfos[equipnum][channelindex].ZoneTempInfos.Add(zone);
                                    zoneindex = ChannelZoneInfos[equipnum][channelindex].ZoneTempInfos.Count - 1;
                                    ChannelZoneInfos[equipnum][channelindex].ZoneCount = (ushort)(zoneindex + 1);
                                }
                                for (int j = 0; j < values.Length; j++)
                                {
                                    string[] temp = values[j].Split('=');
                                    if (temp.Length > 1)
                                    {
                                        if (temp[0].Trim().ToLower().Contains("zonename"))
                                            zone.ZoneName = temp[1].Trim();
                                        else if (temp[0].Trim().ToLower().Contains("startpos"))
                                            zone.StartPos = float.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("stoppos"))
                                            zone.StopPos = float.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("temprisethre"))
                                            zone.TempRiseThreshold = ushort.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("consttempthre"))
                                            zone.ConsTempThreshold = ushort.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("regiontempdifthre"))
                                            zone.RegionTempDifThreshold = ushort.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("temprise"))
                                            zone.TempRiseFlag = bool.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("consttemp"))
                                            zone.ConsTempFlag = bool.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("regiontempdif"))
                                            zone.RegionTempDifFlag = bool.Parse(temp[1].Trim());
                                        else if (temp[0].Trim().ToLower().Contains("fiberbreak"))
                                            zone.FiberBreakFlag = bool.Parse(temp[1].Trim());
                                    }
                                }
                                ChannelZoneInfos[equipnum][channelindex].ZoneTempInfos[zoneindex] = zone;                                
                            }
                        }
                    }
                }                
            }            
        }

        private void CompareChannelInfo(Dictionary<string,List<ChannelInfos>> channelcfg, Dictionary<string, List<ChannelInfos>> alarmzonecfg)
        {
            Dictionary<string, List<ChannelInfos>> channelCfg = channelcfg;
            Dictionary<string, List<ChannelInfos>> alarmzoneCfg = alarmzonecfg;

            string[] keys = channelCfg.Keys.ToArray();
            string[] keys1 = alarmzoneCfg.Keys.ToArray();
            string[] dif = keys1.Except(keys).ToArray();
            for (int i = 0; i < dif.Length; i++)
                alarmzonecfg.Remove(dif[i]);

            foreach(KeyValuePair<string, List<ChannelInfos>> kvp in channelCfg)
            {
                int channelcount = kvp.Value.Count;
                for(int i=0;i<channelcount;i++)
                {
                    ChannelInfos channelinfo = kvp.Value[i];

                    if (!alarmzoneCfg.Keys.Contains(kvp.Key))
                    {
                        List<ChannelInfos> cis = new List<ChannelInfos>();
                        cis.Add(DeepCopy<ChannelInfos>(channelinfo));
                        alarmzoneCfg.Add(kvp.Key, cis);
                    }

                    ChannelInfos alarmchannelinfo = alarmzoneCfg[kvp.Key].Find(delegate (ChannelInfos p) { return p.ChannelNum == channelinfo.ChannelNum; });
                    if (alarmchannelinfo != null)
                    {
                        //两个配置文件均存在该通道，然后比较分区数
                        int zonecount = channelinfo.ZoneCount;
                        int alarmzonecount = alarmchannelinfo.ZoneTempInfos.Count;
                        float interval = channelinfo.FiberLen / zonecount;
                        if (zonecount != alarmzonecount)
                        {
                            List<ZoneTempInfo> zones = new List<ZoneTempInfo>();
                            alarmchannelinfo.ZoneTempInfos = new List<ZoneTempInfo>();
                            channelCfg[kvp.Key][i].ZoneTempInfos = new List<ZoneTempInfo>();
                            for (int j = 0; j < zonecount; j++)
                            {
                                ZoneTempInfo zone = new ZoneTempInfo();
                                zone.ZoneNumber = (ushort)(j + 1);
                                zone.ZoneName = (j + 1).ToString();
                                if (j == 0)
                                    zone.StartPos = 0;
                                else
                                    zone.StartPos = j * interval;

                                if (j == zonecount - 1)
                                    zone.StopPos = channelinfo.FiberLen;
                                else
                                    zone.StopPos = j * interval + interval;
                                zone.TempRiseThreshold = AlarmZones.DefaultTempRiseThres;
                                zone.ConsTempThreshold = AlarmZones.DefaultConsTempThres;
                                zone.RegionTempDifThreshold = AlarmZones.DefaultRegionTempDifThres;
                                zones.Add(zone);

                                alarmchannelinfo.ZoneTempInfos.Add(DeepCopy<ZoneTempInfo>(zone));
                                channelCfg[kvp.Key][i].ZoneTempInfos.Add(DeepCopy<ZoneTempInfo>(zone));


                            }
                            alarmchannelinfo.ZoneCount = (ushort)alarmchannelinfo.ZoneTempInfos.Count;
                            channelCfg[kvp.Key][i].ZoneCount = (ushort)channelCfg[kvp.Key][i].ZoneTempInfos.Count;
                            //    alarmchannelinfo.ZoneTempInfos = DeepCopy(zones);
                            //     channelCfg[kvp.Key][i].ZoneTempInfos = DeepCopy(zones);

                        }
                    }
                    
                    else
                    {
                        /*
                        //添加通道并创建分区
                        int zonecount = channelinfo.ZoneCount;
                        float interval = channelinfo.FiberLen / zonecount;
                        List<ZoneTempInfo> zones = new List<ZoneTempInfo>();
                        for (int j = 0; j < zonecount; j++)
                        {
                            ZoneTempInfo zone = new ZoneTempInfo();
                            zone.ZoneNumber = (ushort)(j + 1);
                            zone.ZoneName = (j + 1).ToString();
                            if (j == 0)
                                zone.StartPos = 0;
                            else
                                zone.StartPos = j * interval;

                            if (j == zonecount - 1)
                                zone.StopPos = channelinfo.FiberLen;
                            else
                                zone.StopPos = j * interval + interval;
                            zone.TempRiseThreshold = AlarmZones.DefaultTempRiseThres;
                            zone.ConsTempThreshold = AlarmZones.DefaultConsTempThres;
                            zone.RegionTempDifThreshold = AlarmZones.DefaultRegionTempDifThres;
                            zones.Add(zone);

                            channelinfo.ZoneTempInfos.Add(DeepCopy<ZoneTempInfo>(zone));
                            channelCfg[kvp.Key][i].ZoneTempInfos.Add(DeepCopy<ZoneTempInfo>(zone));


                        }
                        */

                    }
                }
            }
       //     ChannelZoneInfos = (channelCfg);
        //    ExistChannels = DeepCopy(channelCfg);
        }

        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
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
                   // LoadOption();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void CreateZone(string equipmun,ushort channelnum,int zonecount,float fiberlen)
        {
            if(!ChannelZoneInfos.Keys.Contains(equipmun))
            {
                List<ChannelInfos> channel = new List<ChannelInfos>();
                channelZoneInfos.Add(equipmun, channel);
            }

            List<ChannelInfos> channels = channelZoneInfos[equipmun];
            ChannelInfos alarmchannelinfo = channels.Find(delegate (ChannelInfos p) { return p.ChannelNum == channelnum; });
            int index = channels.FindIndex(item => item.ChannelNum == channelnum);

            if (alarmchannelinfo == null)
            {
                alarmchannelinfo = new ChannelInfos();
                alarmchannelinfo.ChannelNum = channelnum;
                alarmchannelinfo.ZoneCount = 0;
                alarmchannelinfo.MeasureTime = 5;
                alarmchannelinfo.SampleInterval = 40;
                alarmchannelinfo.FiberLen = 10000;
                alarmchannelinfo.ZoneTempInfos = new List<ZoneTempInfo>();
                channelZoneInfos[equipmun].Add(alarmchannelinfo);
                index = channelZoneInfos[equipmun].Count - 1;
            }

            if (alarmchannelinfo.ZoneCount != zonecount)
            {
                List<ZoneTempInfo> zones = new List<ZoneTempInfo>();
                float interval = fiberlen / zonecount;
                alarmchannelinfo.ZoneTempInfos = new List<ZoneTempInfo>();
                channelZoneInfos[equipmun][index].ZoneTempInfos = new List<ZoneTempInfo>();
                for (int j = 0; j < zonecount; j++)
                {
                    ZoneTempInfo zone = new ZoneTempInfo();
                    zone.ZoneNumber = (ushort)(j + 1);
                    zone.ZoneName = (j + 1).ToString();
                    if (j == 0)
                        zone.StartPos = 0;
                    else
                        zone.StartPos = j * interval;

                    if (j == zonecount - 1)
                        zone.StopPos = fiberlen;
                    else
                        zone.StopPos = j * interval + interval;
                    zone.TempRiseThreshold = AlarmZones.DefaultTempRiseThres;
                    zone.ConsTempThreshold = AlarmZones.DefaultConsTempThres;
                    zone.RegionTempDifThreshold = AlarmZones.DefaultRegionTempDifThres;
                    zones.Add(zone);

                    alarmchannelinfo.ZoneTempInfos.Add(DeepCopy<ZoneTempInfo>(zone));
                    channelZoneInfos[equipmun][index].ZoneTempInfos.Add(DeepCopy<ZoneTempInfo>(zone));
                }
                alarmchannelinfo.ZoneCount = (ushort)zonecount;
                channelZoneInfos[equipmun][index].ZoneCount = (ushort)zonecount;
            }
        }

        public void SetValue(Dictionary<string,List<ChannelInfos>> existchannels)
        {
            //更新ChannelCfg配置文件 
            ExeConfigurationFileMap ecf = new ExeConfigurationFileMap();
            ecf.ExeConfigFilename = ConfigPath + FileName;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecf, ConfigurationUserLevel.None);
            string[] fileKeys = config.AppSettings.Settings.AllKeys; 

            List<string> newkeys = new List<string>();
            List<string> equipnums = new List<string>();
            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in existchannels)
            {
                equipnums.Add(kvp.Key);
            }
            //更新文件中已有的键值对
            for (int i = 0; i < equipnums.Count; i++)
            {
                for (int j = 0; j < existchannels[equipnums[i]].Count; j++)
                {                    
                    for(int k=0;k< existchannels[equipnums[i]][j].ZoneTempInfos.Count;k++)
                    {
                        string key = equipnums[i] + "_" + existchannels[equipnums[i]][j].ChannelNum;
                        ZoneTempInfo zone = existchannels[equipnums[i]][j].ZoneTempInfos[k];
                        key += "_" + zone.ZoneNumber;
                        string value = "ZoneName = " + zone.ZoneName + "; StartPos =" + (zone.StartPos).ToString("F2") + "; StopPos = " + zone.StopPos + "; TempRiseThre = " + zone.TempRiseThreshold.ToString() + "; ConstTempThre = " + zone.ConsTempThreshold.ToString() + ";RegionTempDifThre = " + zone.RegionTempDifThreshold.ToString() + ";" +
                                        "TempRiss = " + zone.TempRiseFlag + "; ConstTemp = " + zone.ConsTempFlag + "; RegionTempDif = " + zone.RegionTempDifFlag + "; FiberBreak = " + zone.FiberBreakFlag + ";";

                        if (((IList)fileKeys).Contains(key))
                            config.AppSettings.Settings[key].Value = value;
                        else
                            config.AppSettings.Settings.Add(key, value);

                        if (!newkeys.Contains(key))
                            newkeys.Add(key);
                    }                    
                }
            }
            //删除文件中多余的键值对
            string[] delectkey = fileKeys.Except(newkeys).ToArray();
            if (delectkey != null)
            {
                for (int i = 0; i < (delectkey.Length); i++)
                    config.AppSettings.Settings.Remove(delectkey[i]);
            }                      
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件   
            readFileFlag = true;
        }
    }
}
