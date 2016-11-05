using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PBPid.ConfigManageSpace
{
    /// <summary>
    /// 同步套接字 by 罗俊杰
    /// </summary>
    [Serializable]
    internal sealed class ClientSocketSync : Socket //internal 不允许外部访问
    {
        #region 事件

        public delegate void ReceivedBigData(Object Data, int _DataSize, Socket sk);        //数据返回
        public event ReceivedBigData OnReceivedBigData;

        public delegate void CloseConnection();                                             //链接断开
        public event CloseConnection OnCloseConnection;

        public delegate void Connection();                                                  //链接
        public event Connection OnConnection;

        public delegate void SendData(int _DataSize, string _Ip);                           //发送数据
        public event SendData OnSendData;

        public delegate void Error(string Error_Message);                                   //错误捕获
        public event Error OnError;

        #endregion

        #region 属性

        private int port = 0;
        private string ip = string.Empty;
        private int _SendBufferSize = 65534;
        private int _ReceiveBufferSize = 65534;

        public string Error_Message = string.Empty;

        #endregion

        #region 构造

        public ClientSocketSync(string _ip, int _port)//构造
            : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            this.ip = _ip;
            this.port = _port;
        }

        #endregion

        #region 连接

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns>成功返回TRUE</returns>
        public bool ConnectionServer()
        {
            try
            {
                IPEndPoint _ip = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port);
                this.Connect(_ip);
                this.SendBufferSize = _SendBufferSize;
                this.ReceiveBufferSize = _ReceiveBufferSize;

                OnConnection.BeginInvoke(null, null);
                Thread t = new Thread(Receive);//数据返回监视
                t.IsBackground = true;
                t.Priority = ThreadPriority.Lowest;
                t.Start();
                return true;
            }
            catch (Exception e)
            {
                Error_Message = e.Message;
                OnError(e.Message);
                return false;
            }
        }

        #endregion

        #region 接收

        /// <summary>
        /// 接收数据
        /// </summary>
        private void Receive()
        {
            int Size = 0;
            MemoryStream ms = new MemoryStream();
            byte[] _byte = new byte[ReceiveBufferSize];

            try
            {
                while (true)
                {
                    int i = this.Receive(_byte, SocketFlags.None);
                    byte[] _newbyte = new byte[i];
                    Buffer.BlockCopy(_byte, 0, _newbyte, 0, i);

                    byte[] _Data = new byte[i];
                    if (Size == 0)//得到大小之后就不用再去得到
                    {
                        byte[] _PacketDataSize = new byte[100];
                        _Data = new byte[_newbyte.Length - 100];
                        Buffer.BlockCopy(_newbyte, 0, _PacketDataSize, 0, 100);
                        Size = int.Parse(System.Text.ASCIIEncoding.ASCII.GetString(_PacketDataSize).ToString());//数据大小
                        Buffer.BlockCopy(_newbyte, 100, _Data, 0, _Data.Length);    //数据
                    }
                    else
                        Buffer.BlockCopy(_newbyte, 0, _Data, 0, _Data.Length);      //已经接收到大小，这里就不用再接收。

                    ms.Write(_Data, 0, _Data.Length);

                    if (Size == ms.Length)//判断是否传送完毕
                    {
                        this.OnReceivedBigData.BeginInvoke(Deserialize(ms.ToArray()), i, this, null, null);
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
                OnCloseConnection.BeginInvoke(null, null);
                CloseSocket();
            }
        }

        #endregion

        #region 发送

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="_str">要发送的数据</param>
        public void SendTo(Object data)
        {
            try
            {
                byte[] _Data = new byte[SendBufferSize];
                _Data = this.Serialize(data);
                int _DataSize = _Data.Length;
                byte[] _PacketDataSize = System.Text.ASCIIEncoding.ASCII.GetBytes(_DataSize.ToString());
                byte[] _Packet = new byte[100 + _Data.Length];
                Buffer.BlockCopy(_PacketDataSize, 0, _Packet, 0, _PacketDataSize.Length);
                Buffer.BlockCopy(_Data, 0, _Packet, 100, _Data.Length);
                this.OnSendData.BeginInvoke(this.Send(_Packet), this.RemoteEndPoint.ToString(), null, null);
            }
            catch (Exception e)
            {
                Error_Message = e.Message;
                OnError.BeginInvoke(e.Message, null, null);
                OnCloseConnection.BeginInvoke(null, null);
                CloseSocket();
            }
        }

        #endregion

        #region 断开、序列化

        /// <summary>
        /// 关闭链接
        /// </summary>
        /// <param name="ThreadID"></param>
        public void CloseSocket()
        {
            this.Close();
            base.Dispose(true);
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
        private byte[] Serialize(Object Data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, Data);
            byte[] _byte = new byte[ms.ToArray().Length];
            _byte = ms.ToArray();
            ms.Close();
            return _byte;
        }

        #endregion
    }

}
