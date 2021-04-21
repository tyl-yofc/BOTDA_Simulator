using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTS.CreateData;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DTS.ConfigFiles
{
    public class EquipAllInfos
    {
        public  Dictionary<string, DTSEquip> EquipInfos;     //包含三个cfg配置文件信息，EquipCfg.config
        public Dictionary<string, List<ChannelInfos>> ChannelInfos;       //ChannelCfg.config
        public Dictionary<string, List<ChannelInfos>> ZoneInfos;          //AlarmZoneCfg.config


        private static EquipAllInfos _inastance;
        public static EquipAllInfos Create()
        {
            return _inastance ?? (_inastance = new EquipAllInfos());
        }

        public EquipAllInfos()
        {
            EquipInfos = new Dictionary<string, DTSEquip>();
            ChannelInfos = new Dictionary<string, List<DTS.ChannelInfos>>();
            ZoneInfos = new Dictionary<string, List<DTS.ChannelInfos>>();
        }

        public void ReadCfg()
        {
            EquipInfos = ReadEquipCfg.Create().ReadFile();

            ChannelInfos = ReadChannelCfg.Create().ReadFile();
            foreach(KeyValuePair<string, List<ChannelInfos>> kvp in ChannelInfos)
                kvp.Value.Sort((x, y) => x.ChannelNum.CompareTo(y.ChannelNum));     //按照通道号排序

            ZoneInfos = ReadAlarmZoneCfg.Create().ReadFile();
            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in ZoneInfos)
            {
                int count = kvp.Value.Count;
                for(int i=0;i<count;i++)
                {
                    kvp.Value[i].ZoneTempInfos.Sort((x, y) => x.ZoneNumber.CompareTo(y.ZoneNumber));      //按照分区号排序
                }
            }

            RefreshChannelInfo(EquipInfos, ChannelInfos);
            RefreshZoneInfo(ChannelInfos, ZoneInfos);
            CombineEquipChannelZoneInfo(EquipInfos);
        }

        public void RefreshChannelInfo(Dictionary<string, DTSEquip> equipinfos, Dictionary<string, List<ChannelInfos>> channelInfos)
        {
            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in channelInfos)
                kvp.Value.Sort((x, y) => x.ChannelNum.CompareTo(y.ChannelNum));     //按照通道号排序

            //根据EquipInfos更新channelsInfos
            foreach (KeyValuePair<string, DTSEquip> kvp in equipinfos)
            {
                int newchannelcount = kvp.Value.ChannelCount;
                if (channelInfos.Keys.Contains(kvp.Key))
                {
                    //比较两者的通道数                    
                    int oldchannelcount = channelInfos[kvp.Key].Count;
                    int[] channels = new int[newchannelcount + 1];
                    for (int j = 0; j < oldchannelcount; j++)
                    {
                        ushort temp = channelInfos[kvp.Key][j].ChannelNum;
                        for (int i = 1; i < channels.Length; i++)
                        {
                            if (i == temp)
                            {
                                channels[i] = temp;
                                break;
                            }
                        }
                    }
                    if (newchannelcount > oldchannelcount)
                    {
                        int index = 1;
                        for (int i = 1; i <= newchannelcount - oldchannelcount; i++)
                        {
                            for (int j = index; j < channels.Length; j++)
                            {
                                if (channels[j] == 0)
                                {
                                    channels[j] = j;
                                    ChannelInfos temp = new ChannelInfos();
                                    temp.ChannelNum = (ushort)j;
                                    temp.ZoneCount = 1;
                                    temp.BaseTemp = 30;
                                    temp.MeasureTime = 5;
                                    temp.SampleInterval = 40;
                                    temp.FiberLen = 10000;
                                    channelInfos[kvp.Key].Add(temp);

                                    channelInfos[kvp.Key].Sort((x, y) => x.ChannelNum.CompareTo(y.ChannelNum));
                                    index = j + 1;
                                    break;
                                }
                            }
                        }
                    }
                    else if (newchannelcount < oldchannelcount)
                    {
                        //  channelinfos[kvp.Key].RemoveRange(0, oldchannelcount - newchannelcount);
                        channelInfos[kvp.Key].Sort((x, y) => x.ChannelNum.CompareTo(y.ChannelNum));
                        int len = channelInfos[kvp.Key].Count;
                        channelInfos[kvp.Key].RemoveRange(len - (oldchannelcount - newchannelcount), oldchannelcount - newchannelcount);
                    }
                }
                else
                {
                    //添加该设备以及设备对应的通道数
                    List<ChannelInfos> channels = new List<ChannelInfos>();
                    for (int j = 1; j <= newchannelcount; j++)
                    {
                        ChannelInfos temp = new ChannelInfos();
                        temp.ChannelNum = (ushort)j;
                        temp.BaseTemp = 30;
                        temp.ZoneCount = 1;
                        temp.MeasureTime = 5;
                        temp.SampleInterval = 40;
                        temp.FiberLen = 10000;
                        channels.Add(temp);
                    }
                    channelInfos.Add(kvp.Key, channels);
                }
            }

            //删除channelcfg配置文件中多余的主机
            string[] newkeys = equipinfos.Keys.ToArray();
            if (newkeys.Length > 0)
            {
                string[] oldkeys1 = channelInfos.Keys.ToArray();
                string[] dif = oldkeys1.Except(newkeys).ToArray();

                if (dif.Length > 0)
                {
                    for (int i = 0; i < dif.Length; i++)
                    {
                        channelInfos.Remove(dif[i]);
                    }
                }
            }

            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in channelInfos)
            {
                int count = channelInfos[kvp.Key].Count;
                ushort sampleinterval = equipinfos[kvp.Key].SampleInterval;
                for(int i=0;i<count;i++)
                {
                    channelInfos[kvp.Key][i].SampleInterval = sampleinterval;
                }
            }
            ReadChannelCfg.Create().SetValue(channelInfos);
        }

        public void RefreshZoneInfo(Dictionary<string, List<ChannelInfos>> channelinfos, Dictionary<string, List<ChannelInfos>> zoneinfos)
        {
            
            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in channelinfos)
            {
                int channelcount = kvp.Value.Count;
                if (!zoneinfos.Keys.Contains(kvp.Key))       //添加设备
                {
                    List<ChannelInfos> ci = new List<ChannelInfos>();
                    zoneinfos.Add(kvp.Key, ci);
                }

                for(int i=0;i<channelcount;i++)
                {
                    ChannelInfos channel = zoneinfos[kvp.Key].Find(delegate (ChannelInfos c) { return c.ChannelNum == ushort.Parse(channelinfos[kvp.Key][i].ChannelNum.ToString()); });
                    int channelindex = zoneinfos[kvp.Key].FindIndex(item => item.ChannelNum == ushort.Parse(channelinfos[kvp.Key][i].ChannelNum.ToString()));
                    if (channel == null)       //添加通道
                    {
                        ChannelInfos ci = new ChannelInfos();
                        ci.ChannelNum = channelinfos[kvp.Key][i].ChannelNum;
                        zoneinfos[kvp.Key].Add(ci);
                    }
                }
            }

            //更新分区
            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in channelinfos)
            {
                int channelcount = kvp.Value.Count;
                if (zoneinfos.Keys.Contains(kvp.Key))
                {
                    for (int i = 0; i < channelcount; i++)
                    {
                        int channelzonecount = channelinfos[kvp.Key][i].ZoneCount;
                        float fiberlen = channelinfos[kvp.Key][i].FiberLen;
                        float sampleinterval = channelinfos[kvp.Key][i].SampleInterval;

                        ChannelInfos channel = zoneinfos[kvp.Key].Find(delegate (ChannelInfos c) { return c.ChannelNum == ushort.Parse(channelinfos[kvp.Key][i].ChannelNum.ToString()); });
                        int channelindex = zoneinfos[kvp.Key].FindIndex(item => item.ChannelNum == ushort.Parse(channelinfos[kvp.Key][i].ChannelNum.ToString()));

                        //比较分区数
                        if (channelzonecount != channel.ZoneTempInfos.Count)
                        {
                            channel.ZoneTempInfos = new List<ZoneTempInfo>();
                            for (int j = 0; j < channelzonecount; j++)
                            {
                                ZoneTempInfo zone = new ZoneTempInfo();
                                zone.ZoneNumber = (ushort)(j + 1);
                                zone.ZoneName = (j + 1).ToString();
                                if (j == 0)
                                    zone.StartPos = 0;
                                else
                                    zone.StartPos = j * sampleinterval;

                                if (j == channelzonecount - 1)
                                    zone.StopPos = fiberlen;
                                else
                                    zone.StopPos = j * sampleinterval + sampleinterval;
                                zone.TempRiseThreshold = AlarmZones.DefaultTempRiseThres;
                                zone.ConsTempThreshold = AlarmZones.DefaultConsTempThres;
                                zone.RegionTempDifThreshold = AlarmZones.DefaultRegionTempDifThres;
                                zone.TempRiseFlag = false;
                                zone.ConsTempFlag = false;
                                zone.RegionTempDifFlag = false;
                                zone.FiberBreakFlag = false;
                                channel.ZoneTempInfos.Add(zone);
                            }
                        }
                    }
                }
            }

            //删除alarmzonecfg中多余的主机
            string[] newkeys = channelinfos.Keys.ToArray();
            if (newkeys.Length > 0)
            {
                string[] oldkeys1 = zoneinfos.Keys.ToArray();
                string[] dif = oldkeys1.Except(newkeys).ToArray();

                if (dif.Length > 0)
                {
                    for (int i = 0; i < dif.Length; i++)
                    {
                        zoneinfos.Remove(dif[i]);
                    }
                }
            }

            //删除alarmzonecfg中多余的通道
            foreach (KeyValuePair<string, List<ChannelInfos>> kvp in zoneinfos)
            {
                int count = kvp.Value.Count;
                List<int> removeindex = new List<int>();
                for(int i=0;i<count;i++)
                {
                    ChannelInfos channel = channelinfos[kvp.Key].Find(delegate (ChannelInfos c) { return c.ChannelNum == ushort.Parse(kvp.Value[i].ChannelNum.ToString()); });
                    if(channel == null)
                    {
                        removeindex.Add(i);                       
                    }
                }
                if (removeindex.Count > 0)
                {
                    removeindex.Sort((x, y) => x.CompareTo(y));
                    for (int i = removeindex.Count - 1; i >= 0; i--)
                        zoneinfos[kvp.Key].RemoveAt(removeindex[i]);
                }
            }

            ReadAlarmZoneCfg.Create().SetValue(zoneinfos);    
        }

        public void CombineEquipChannelZoneInfo(Dictionary<string,DTSEquip> equipinfo)
        {
            foreach(KeyValuePair<string, DTSEquip> kvp in equipinfo)
            {
                if(ChannelInfos.Keys.Contains(kvp.Key))
                {
                    int channelcount = kvp.Value.ChannelCount;
                    kvp.Value.channelInfo = new List<ChannelInfos>();
                    for (int i=0;i<channelcount;i++)
                    {
                        ChannelInfos channel = ChannelInfos[kvp.Key][i];    

                        if (ZoneInfos.Keys.Contains(kvp.Key))
                        {
                            ChannelInfos zone = ZoneInfos[kvp.Key].Find(delegate (ChannelInfos c) { return c.ChannelNum == channel.ChannelNum; });
                            int zoneind = ZoneInfos[kvp.Key].FindIndex(item=> item.ChannelNum == channel.ChannelNum);
                            if(zone != null)
                            {
                                channel.ZoneTempInfos = zone.ZoneTempInfos;
                                channel.ZoneCount = (ushort)zone.ZoneTempInfos.Count;
                                channel.SampleInterval = equipinfo[kvp.Key].SampleInterval;
                            }
                        }
                        kvp.Value.channelInfo.Add(channel);
                    }
                }
            }
        }

        public static DTSEquip CloneDTSEquip(DTSEquip obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return (DTSEquip)formatter.Deserialize(memoryStream);
        }


    }
}
