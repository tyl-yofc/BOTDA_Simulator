using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Simulator_BOTDA
{
    [Serializable]
    public class BotdaData
    {
        public DataType dataType;
        public int channelNum;
        public Int32 dataLen;
        public UInt64 systemTime;
        public float referenceFrequencyShift;    //参考频移
        public float fiberLen;                 //参考光纤长度
        public List<float> frequencyDatas;
        

        public BotdaData(int channelNum)
        {
            this.channelNum = channelNum;
            frequencyDatas = new List<float>();
        }

        public BotdaData()
        {            
            frequencyDatas = new List<float>();
        }

        public byte[] DataConvert(BotdaData bd,float refractiveIndex,float samplingResolution)
        {
            List<byte> bts = new List<byte>();
            byte high = 0;
            byte low = 0;
            switch(bd.dataType)
            {
                case DataType.FrequencyShift:
                    high = 0x00;
                    break;
                case DataType.ReferencePointTemp:
                    high = 0x01;
                    break;
                case DataType.ReferencePointStrain:
                    high = 0x02;
                    break;
                case DataType.ReferenceCurveTemp:
                    high = 0x03;
                    break;
                case DataType.ReferenceCurveStrain:
                    high = 0x04;
                    break;
                case DataType.FiberBreak:
                    high = 0x05;
                    break;                    
            }
            switch(bd.channelNum)
            {
                case 1:
                    low = 0x01;
                    break;
                case 2:
                    low = 0x02;
                    break;
                case 3:
                    low = 0x03;
                    break;
                case 4:
                    low = 0x04;
                    break;
            }
            byte temp = (byte)((high << 4) | low);
            bts.Add(temp);
            Int32 len = bd.dataLen * 4 + 20;
            byte[] array = new byte[3];
            array[0] = Convert.ToByte((len & 0xff0000) >> 16);
            array[1] = Convert.ToByte((len & 0xff00) >> 8);
            array[2] = Convert.ToByte(len & 0xff);

            bts.Add(array[2]);
            bts.Add(array[1]);
            bts.Add(array[0]);
            bts.AddRange(BitConverter.GetBytes(bd.systemTime));
            bts.AddRange(BitConverter.GetBytes(refractiveIndex));
            bts.AddRange(BitConverter.GetBytes(samplingResolution));
            for (int i = 0; i < bd.dataLen; i++)
                bts.AddRange(BitConverter.GetBytes(bd.frequencyDatas[i]));

            return bts.ToArray();
        }

        public BotdaData CloneChanelInfo()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            memoryStream.Position = 0;
            return (BotdaData)formatter.Deserialize(memoryStream);
        }

    }

    public enum DataType
    {
        FrequencyShift = 0,     //频移数据
        ReferencePointTemp,    //参考点温度
        ReferencePointStrain,      //参考点应变
        ReferenceCurveTemp,      //参考曲线温度
        ReferenceCurveStrain,      //参考曲线应变
        FiberBreak       //断纤
    };

}
