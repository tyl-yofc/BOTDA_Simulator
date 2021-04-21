using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Simulator_BOTDA.CreateDatas
{
    public class CreateFrequencyShiftData
    {
        private  BotdaEquip _botdaEquip;
        private Thread _threadCreatData;
        private bool _isRunning;
        private bool _isStop;

        private Dictionary<string, List<byte[]>> _waitSendDatas;      //key为设备编号
        private object obj;

        public CreateFrequencyShiftData()
        {
            obj = new object();
            _waitSendDatas = new Dictionary<string, List<byte[]>>();
           
        }
        public void StartThread(BotdaEquip be)
        {
            _botdaEquip = be;
            _isRunning = true;
            _isStop = false;
            _threadCreatData = new Thread(CreateData);
            _threadCreatData.Start();
        }

        public void StopThread()
        {
            _isStop = true;
            _isRunning = false;
            if(_threadCreatData != null)
                _threadCreatData.Abort();
        }

        private void CreateData()
        {
            while(_isRunning)
            {
              //  if (_isRunning)
                {                   
                    int channelcount =_botdaEquip.channelNums.Count;
                    for (int i = 0; i < channelcount; i++)
                    {
                        //创建频移数据
                        BotdaData bd = _botdaEquip.botdaFrequencyShiftDatas[_botdaEquip.channelNums[i]].CloneChanelInfo();
                        int datalen = (int)(bd.fiberLen / (int)(_botdaEquip.samplingResolution *100))*100;
                        Random rd = new Random();
                        for (int j = 0; j < datalen; j++)
                        {
                            int r = rd.Next(-500, 500);
                            float data = r / 1000.0f + bd.referenceFrequencyShift;
                            bd.frequencyDatas.Add(data);
                        }

                        bd.dataLen = datalen;
                        DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0);
                        TimeSpan ts = DateTime.Now - dt;
                        bd.systemTime = (UInt64)(ts.Days * 86400 + ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds);

                        byte[] senddata = bd.DataConvert(bd, _botdaEquip.refractiveIndex, _botdaEquip.samplingResolution);
                        Push(_botdaEquip.equipNum.ToString(), senddata);
                       
                        if(_botdaEquip.equipNum == int.Parse(Main.oldEquipNum) && _botdaEquip.channelNums[i] == int.Parse(Main.oldChannelNum))
                        {
                            Main.channelCurvers[_botdaEquip.channelNums[i].ToString()].DrawCurver(bd, _botdaEquip.refractiveIndex, _botdaEquip.samplingResolution);
                            Main.ObjRefreshZed(_botdaEquip.channelNums[i].ToString());
                        }  
                        Thread.Sleep(1000);                       
                    }
                }
            }
        }

        public void Push(string equipnum, byte[] frequencyshiftdata)
        {
            if (!_waitSendDatas.Keys.Contains(equipnum))
            {
                List<byte[]> b = new List<byte[]>();
                b.Add(frequencyshiftdata);
                _waitSendDatas.Add(equipnum, b);
            }
            else
            {
                _waitSendDatas[equipnum].Add(frequencyshiftdata);
            }

            int count = _waitSendDatas[equipnum].Count;
            if(count > 10)
            {
                _waitSendDatas[equipnum].RemoveRange(0, count - 1);
            }
        }

        public byte[] Pop(string equipnum)
        {
            byte[] data = null;
            if (_waitSendDatas.Keys.Contains(equipnum))
            {
                int count = _waitSendDatas[equipnum].Count;
                if (count > 0)
                {
                    data = _waitSendDatas[equipnum][count - 1];
                }
            }
            return data;
        }



    }
}
