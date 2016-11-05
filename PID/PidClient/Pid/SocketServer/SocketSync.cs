using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Configuration;

//套接字 BY 罗俊杰（异步）
namespace SocketServer.Sync
{
    [Serializable]
    public sealed class ServerSocketSync : Socket
    {

        #region 事件

        public delegate void Connection(string _Ip);                                                    //链接成功
        public event Connection OnConnection;

        public delegate void ReceivedBigData(Object ds, int _DataSize, Socket ck, int _ThreadID);       //数据集返回
        public event ReceivedBigData OnReceivedBigData;

        public delegate void CloseConnection(string _Ip);                                               //链接断开
        public event CloseConnection OnCloseConnection;

        public delegate void SendData(int _DataSize, string _Ip);                                       //发送数据
        public event SendData OnSendData;

        public delegate void Error(string Error_Message);                                               //错误捕获
        public event Error OnError;

        #endregion

        #region 属性

        private int port = 0;
        private int MaxConnection = 0;            //最大连接数
        private Thread t_Listen = null;           //监听线程
        private int _SendBufferSize = 65534;      //发送缓冲区大小
        private int _ReceiveBufferSize = 65534;   //接收缓冲区大小

        public List<Socket> clientSocket = null;  //存放客户端
        public string Error_Message = string.Empty;

        #endregion

        #region 构造

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="_port">端口号</param>
        /// <param name="_maxConnection">最大连接数</param>
        public ServerSocketSync(int _port, int _maxConnection)//构造
            : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            this.MaxConnection = _maxConnection;
            this.port = _port;
        }

        private IPAddress GetIp(string ip)
        {
            return IPAddress.Parse(ip);
        }

        #endregion

        #region 监听

        public bool RunServer(string ip)
        {
            try
            {
                IPEndPoint _ip = new IPEndPoint(GetIp(ip), port);
                this.Bind(_ip);
                this.Listen(MaxConnection);
                this.SendBufferSize = _SendBufferSize;
                this.ReceiveBufferSize = _ReceiveBufferSize;
                this.clientSocket = new List<Socket>();

                t_Listen = new Thread(Listen);//监听线程
                t_Listen.IsBackground = true;
                t_Listen.Priority = ThreadPriority.Lowest;
                t_Listen.Start();

                return true;
            }
            catch (Exception e)
            {
                Error_Message = e.Message;
                OnError(e.Message);
                return false;
            }
        }

        private void Listen()
        {
            int i = 0;
            try
            {
                for (i = 0; i <= MaxConnection; ++i)
                {
                    clientSocket.Add(Accept());
                    OnConnection.BeginInvoke(clientSocket[i].RemoteEndPoint.ToString(), null, null);
                    clientSocket[i].Blocking = true;
                    clientSocket[i].SendBufferSize = _SendBufferSize;
                    clientSocket[i].ReceiveBufferSize = _ReceiveBufferSize;

                    Thread t = new Thread(Received);
                    t.IsBackground = true;
                    t.Name = i.ToString();
                    t.Priority = ThreadPriority.Lowest;
                    t.Start();
                }
            }
            catch
            {
                Error_Message = "服务器已停止";
                OnError.BeginInvoke(Error_Message, null, null);
                CloseSocket(i);
            }
        }
        #endregion

        #region 接收

        private void Received()
        {
            int Size = 0;
            MemoryStream ms = new MemoryStream();

            int i = int.Parse(Thread.CurrentThread.Name);
            try
            {
                while (true)
                {
                    byte[] _byteData = new byte[this.clientSocket[i].ReceiveBufferSize];
                    int byteSize = this.clientSocket[i].Receive(_byteData, SocketFlags.None);
                    byte[] _newbyte = new byte[byteSize];
                    Buffer.BlockCopy(_byteData, 0, _newbyte, 0, byteSize);

                    byte[] _Data = new byte[byteSize];
                    if (Size == 0)
                    {
                        byte[] _PacketDataSize = new byte[100];
                        _Data = new byte[_newbyte.Length - 100];
                        Buffer.BlockCopy(_newbyte, 0, _PacketDataSize, 0, 100);
                        Size = int.Parse(System.Text.ASCIIEncoding.ASCII.GetString(_PacketDataSize).ToString());
                        Buffer.BlockCopy(_newbyte, 100, _Data, 0, _Data.Length);
                    }
                    else
                        Buffer.BlockCopy(_newbyte, 0, _Data, 0, _Data.Length);

                    ms.Write(_Data, 0, _Data.Length);
                    if (Size == ms.Length)
                    {
                        this.OnReceivedBigData.BeginInvoke(Deserialize(ms.ToArray()), byteSize, clientSocket[i], i, null, null);
                        ms.Close();
                        ms.Dispose();
                        ms = new MemoryStream();
                        Size = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Error_Message = e.Message;
                OnError.BeginInvoke(e.Message, null, null);
                CloseSocket(i);
            }
        }
        #endregion

        #region 发送

        /// <summary>
        /// 发送大量数据
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="_ThreadID"></param>
        public void SendTo(Object Data, int _ThreadID)
        {
            try
            {
                byte[] _Data = new byte[this.clientSocket[_ThreadID].SendBufferSize];
                _Data = this.Serialize(Data);
                int _DataSize = _Data.Length;
                byte[] _PacketDataSize = System.Text.ASCIIEncoding.ASCII.GetBytes(_DataSize.ToString());
                byte[] _Packet = new byte[100 + _Data.Length];

                Buffer.BlockCopy(_PacketDataSize, 0, _Packet, 0, _PacketDataSize.Length);
                Buffer.BlockCopy(_Data, 0, _Packet, 100, _Data.Length);
                this.OnSendData.BeginInvoke(this.clientSocket[_ThreadID].Send(_Packet), this.clientSocket[_ThreadID].RemoteEndPoint.ToString(), null, null);
            }
            catch (Exception e)
            {
                Error_Message = e.Message;
                OnError.BeginInvoke(e.Message, null, null);
                CloseSocket(_ThreadID);
            }
        }

        /// <summary>
        /// 发送给全部客户端
        /// </summary>
        /// <param name="_str"></param>
        public void SendAll(string _str)
        {
            for (int i = 0; i < this.clientSocket.Count; ++i)
            {
                try
                {
                    if (this.clientSocket[i] != null)
                    {
                        byte[] _Data = new byte[this.clientSocket[i].SendBufferSize];
                        _Data = this.Serialize(_str);
                        int _DataSize = _Data.Length;
                        byte[] _PacketDataSize = System.Text.ASCIIEncoding.ASCII.GetBytes(_DataSize.ToString());
                        byte[] _Packet = new byte[100 + _Data.Length];

                        Buffer.BlockCopy(_PacketDataSize, 0, _Packet, 0, _PacketDataSize.Length);
                        Buffer.BlockCopy(_Data, 0, _Packet, 100, _Data.Length);
                        this.OnSendData.BeginInvoke(this.clientSocket[i].Send(_Packet), this.clientSocket[i].RemoteEndPoint.ToString(), null, null);
                    }

                }
                catch (Exception e)
                {
                    Error_Message = e.Message;
                    OnError(e.Message);
                    CloseSocket(i);
                }
            }
        }

        #endregion

        #region 关闭、序列化方法
        /// <summary>
        /// 关闭某个主机的链接
        /// </summary>
        /// <param name="ThreadID"></param>
        private void CloseSocket(int _ThreadID)
        {
            try
            {
                if (clientSocket[_ThreadID] != null)
                {
                    this.OnCloseConnection.BeginInvoke(this.clientSocket[_ThreadID].RemoteEndPoint.ToString(), null, null);
                    this.clientSocket[_ThreadID].Close();
                    this.clientSocket[_ThreadID] = null;
                }
            }
            catch
            {
                Thread.Sleep(1);
            }
        }

        public void CloseServer()//关闭所有客户端的链接后关闭自己
        {
            this.t_Listen.Abort();
            this.Close();
            base.Dispose(true);

            for (int i = 0; i < clientSocket.Count; i++)
            {
                if (clientSocket[i] != null)
                {
                    clientSocket[i].Close();
                    clientSocket[i] = null;
                }
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="_byte"></param>
        /// <returns></returns>
        private Object Deserialize(byte[] _byte)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(_byte);
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="_byte"></param>
        /// <returns></returns>
        private byte[] Serialize(Object data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, data);
            byte[] _byte = new byte[ms.ToArray().Length];
            _byte = ms.ToArray();
            ms.Close();
            return _byte;
        }

        #endregion
    }
}
