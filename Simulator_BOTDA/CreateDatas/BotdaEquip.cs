using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator_BOTDA.CreateDatas;

namespace Simulator_BOTDA
{
    public class BotdaEquip
    {   
        public SimpleTcpClient simpleTcpClient;
        public int channelCount;
        public List<int> channelNums;    //通道号集合
        public float refractiveIndex;      //折射率
        public float samplingResolution;     //采样分辨率
        public int equipNum;    //设备编号

        public Dictionary<int, BotdaData> botdaFrequencyShiftDatas;      //通道号对应的通道频移数据
        public CreateFrequencyShiftData cfsd;     //用于创建频移数据



        public BotdaEquip()
        {
            channelNums = new List<int>();
            botdaFrequencyShiftDatas = new Dictionary<int, BotdaData>();
            simpleTcpClient = new SimpleTcpClient();
            cfsd = new CreateFrequencyShiftData();
        }

        public void Stop()     //停止数据发送线程、创建频移数据线程
        {
            simpleTcpClient.Disconnect();
            cfsd.StopThread();
            
        }
    }
}
