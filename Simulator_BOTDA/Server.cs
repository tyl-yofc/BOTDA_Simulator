using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator_BOTDA.CreateDatas;
using System.Net;

namespace Simulator_BOTDA
{
    public class Server
    {
        public Dictionary<string, BotdaEquip> ExistEquips;

        private static Server _instance;
        public static Server Create()
        {
            return _instance ?? (_instance = new Server());
        }
        public bool ServerStart(IPAddress serverIP)
        {
            bool flag = false;
            ExistEquips = ReadChannelCfg.Create().ReadFile();
            foreach (KeyValuePair<string,BotdaEquip> kvp in ExistEquips)
            {
                kvp.Value.simpleTcpClient.Init(serverIP,kvp.Value.simpleTcpClient._remotePort, kvp.Key);
                //   kvp.Value.simpleTcpClient.Connect();
                kvp.Value.simpleTcpClient.StartThread();
                //   if (connected)
                {
                    flag = true;
                    kvp.Value.cfsd.StartThread(kvp.Value);
                }
            }
            return flag;
        }

        public void ServerStop()
        {
            foreach (KeyValuePair<string, BotdaEquip> kvp in ExistEquips)
            {
                kvp.Value.Stop();
            }            
        }


    }
}
