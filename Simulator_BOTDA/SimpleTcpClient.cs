using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Simulator_BOTDA.CreateDatas;
using System.Diagnostics;

namespace Simulator_BOTDA
{
    public class SimpleTcpClient
    {
        public string EquipNum;
        private Thread _hbThread;    //心跳线程        
        private Thread _sendThread;    //数据发送/接收线程
        private Thread _connectServer;    //连接服务端

        public Socket _socket;

        public bool _isConnected;
        private bool _stopThread;

        private IPAddress _remoteIP;
        public int _remotePort;
        public static string HeartBeatMessage = "client:online";

        public void Init(IPAddress hostNameorIPAdress, int port,string equipnum)
        {
            this._remoteIP = hostNameorIPAdress;
            this._remotePort = port;
            this.EquipNum = equipnum;

            _hbThread = new Thread(HeartBeat);
            _sendThread = new Thread(SendMessage);
            _connectServer = new Thread(Connect);
        }        

        public void StartThread()
        {
            _isConnected = false;
            _stopThread = false;
            _connectServer.Start();            
        }

        private void StopThread()
        {
            _isConnected = false;
            _stopThread = true;
            if(_hbThread != null)
                _hbThread.Abort();
            if(_sendThread != null)
                _sendThread.Abort();
            if(_connectServer != null)
                _connectServer.Abort();

          //  Main.ObjSetBtText("启 动");
        }

        public void Connect()
        {
            while (!_stopThread)
            {
                if (_isConnected)
                    return;

                IPEndPoint endPoint = new IPEndPoint(_remoteIP, _remotePort);
                _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    _socket.Connect(endPoint);
                }
                catch (SocketException ex)
                {
                    if (_socket?.Connected ?? false) _socket.Shutdown(SocketShutdown.Both);
                    _socket?.Close();
                }
                if (_socket.Connected)
                {
                    _isConnected = true;
                    _hbThread.Start();
                    _sendThread.Start();
                    return;
                }
                Thread.Sleep(1000);
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {           
                try
                {
                    if (_socket?.Connected ?? false) _socket.Shutdown(SocketShutdown.Both);
                    _socket?.Close();                    
                }
                catch (ObjectDisposedException) { }                
            }
            StopThread();
        }

        /// <summary>
        /// Reconnect the client (synchronous)
        /// </summary>
        /// <returns>'true' if the client was successfully reconnected, 'false' if the client is already reconnected</returns>
        public void Reconnect()
        {            
            IPEndPoint endPoint = new IPEndPoint(_remoteIP, _remotePort);
            try
            {
                if (_socket == null)
                {
                    _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(endPoint);
                }
                else if (!_socket.Connected)
                {
                    _socket.Close();
                    _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(endPoint);
                }                    
            }
            catch (Exception ex){}           
        }    

        private void HeartBeat()     //发送心跳数据
        {
            byte[] send = Encoding.Default.GetBytes(HeartBeatMessage);
            while (true)
            {
                if (_socket != null && _socket.Connected)
                {          
                    try
                    {
                       _socket.Send(send);      
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        Reconnect();
                    }                    
                }
                else if (_stopThread)
                    return;
                else
                {
                    Reconnect();
                }
                Thread.Sleep(5000);
            }
        }

        private void SendMessage()     //发送频移数据
        {
            while (true)
            {
                if (_socket != null && _socket.Connected)
                {
                    try
                    {                     
                        {
                            
                            byte[] data = Server.Create().ExistEquips[this.EquipNum].cfsd.Pop(this.EquipNum);
                            if (data != null)
                            {
                                _socket.Send(data);
                                /*
                                string str = "";
                                for (int j = 0; j < data.Length; j++)
                                    str += data[j].ToString("x2") + " ";
                                Debug.WriteLine(str);
                                */
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        Reconnect();
                    }                    
                }
                else if (_stopThread)
                    return;
                else   //重新连接
                {
                    Reconnect();
                }
                Thread.Sleep(1000);
            }
        }

    }
}
