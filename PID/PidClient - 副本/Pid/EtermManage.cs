using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Windows.Forms;

namespace PBPid.EtermManage
{
    /// <summary>
    /// 此类处理以下两个问题：
    /// 1、汉字拼音及编码问题；
    /// 2、443配置的F6、F7问题；
    /// </summary>
    public class EtermManage
    {
        #region 获取443的F7数据包所需参数
        ///////////////////////////////////////////////////////////////////////////////////////////////
        //eterm客户端
        static private Eterm3Lib.ApplicationClass eterm3 = null;

        //Eterm监听端口
        static int m_shareport = 35000;
        ///////////////////////////////////////////////////////////////////////////////////////////////
        #endregion

        #region 获取汉字拼音及编码所需参数
        ///////////////////////////////////////////////////////////////////////////////////////////////      
        public static int sport=350;

        public static string _ip = "";

        //监听线程
        static Thread ListenThread = null;
        static TcpListener _listener = null;

        //客户端服务线程
        static Thread clientservice = null;

        //客户端通讯Socket
        static Socket m_socketServer = null;

        //是否已经回复了成功登陆信息（标志）{ 0x01, 0xFD, 0x00, 0x06, 0x00, 0x00 };
        static bool m_sendsucFlag = false;

        //用于回复客户端的心跳包
        static byte[] m_heartbuf = { 0x01, 0xFA, 0x00, 0x05, 0x00 };

        //黑屏包头
        static byte[] cmdhead1 = { 0x41, 0x51 };

        static byte recno;

        //用于回复信息
        static byte[] m_sendbuf = null;

        const int _maxPacket = 1024*1024;

        //主窗口富文本类引用
        public static RichTextBox refRichTB = null;

        public delegate void SetTextCallback(string text);

        #endregion

        /// <summary>
        /// 启动监听线程
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <returns>true/false</returns>
        public static bool StartListenServer(string ip,int port)
        {
            //先停止已存在的监听线程
            StopListenServer();

            try
            {
                //创建监听线程
                ListenThread = new Thread(new ThreadStart(ListenerProc));
                ListenThread.Start();
                _ip = ip;
                sport = port;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool StopListenServer()
        {
            try
            {
                if (m_socketServer != null)
                {
                    m_socketServer.Close();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {                
                //停止客户端服务线程
                if (clientservice != null)
                {
                    clientservice.Abort();
                }

                //停止监听线程
                if (_listener != null)
                {
                    ListenThread.Abort();
                    _listener.Stop();
                }

                _listener = null;
                ListenThread = null;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static void ListenerProc()
        {
            IPAddress _ipaddr = IPAddress.Parse(_ip);

            //Log.Record("client.log","开启监听：IP地址" + _ip + "；端口" + shareport.ToString());            

            _listener = new TcpListener(_ipaddr, sport);
            _listener.Start();

            // Log.Record("server.log", "1");

            while (true)
            {
                try
                {
                    Socket newclient = _listener.AcceptSocket();

                    if (newclient.Connected)
                    {
                        NetworkStream ns = new NetworkStream(newclient);

                        //读取用户名、密码信息
                        byte[] packetBuff = new byte[_maxPacket];
                        int rcount = ns.Read(packetBuff, 0, _maxPacket);

                        if (rcount == 0)
                        {
                            //已断开
                            ns.Close();
                            newclient.Close();
                            continue;
                        }

                        //Log.Record("client.log","收到客户端连接数据包：",packetBuff,rcount);

                        //用户名
                        string tmpuser = "";
                        //密码
                        string tmppwd = "";

                        string tmpstr = "";

                        //登录用户类型 1 Eterm客户端；  2 网站用户 3网站Flash
                        int userType = 1;

                        m_sendsucFlag = false;

                       
                        //分析用户名、密码
                        tmpstr = ASCIIEncoding.Default.GetString(packetBuff, 2, rcount - 2);
                        string[] sl = tmpstr.Split('\0');

                        if (sl.Length >= 2)
                        {
                            tmpuser = sl[0];
                            rcount = 1;
                            while (rcount < sl.Length - 1)
                            {
                                if (sl[rcount] == "")
                                {
                                    rcount++;
                                    continue;
                                }
                                else
                                {
                                    tmppwd = sl[rcount];
                                    break;
                                }
                            }
                        }

                        //Log.Record("client.log", "分析收到的连接数据包，用户名：" + tmpuser + "，密码：" + tmppwd);



                        //失败对话框的头结构
                        byte[] sendbuf = { 0x00, 0x37, 0x00, 0x31, 0x30, 0x30, 0x30, 0x31, 0x3A };
                        ArrayList a = new ArrayList();

                        byte[] bcount;//记录数据包的长度
                        short count = 0;//长度                   
                                             
                        //回复成功标志
                        byte[] sendbuf7 ={ 0x00, 0x14, 0x01, 0x00, 0x03, 
                            0x00, 0x00, 0x00, 0x41, 0x51, 
                            0x0C, 0x00, 0x00, 0x8C, 0x8C, 
                            0x29, 0x00, 0x00, 0xA9, 0xA9 };
                        newclient.Send(sendbuf7);
                        //回复成功标志
                        //Log.Record("client.log", "客户端用户名：" + tmpuser + "登录成功！");
                        //Log.Record("client.log", "向客户端回复数据包：", sendbuf7, 10);
                       

                        tmpstr = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + tmpuser + " 登入系统\n";

                        //创建客户端服务线程
                        if (clientservice != null)
                        {
                            clientservice.Abort();
                        }

                        if (m_socketServer != null)
                        {
                            m_socketServer.Close();
                            m_socketServer = null;
                        }

                        m_socketServer = newclient;

                        clientservice = new Thread(new ThreadStart(ClientProcFunc));

                        clientservice.Start();
                    }
                }
                catch (Exception e)
                {
                    //Log.Record("client.log", e);
                }
            }
        }

        static void ClientProcFunc()
        {
            while (true)
            {
                try
                {
                    string tmpstr="";

                    string content = "";

                    byte[] received = new byte[_maxPacket];                   

                    //显示的配置名称
                    string emname = "";

                    #region eterm客户端用户
                    if (m_socketServer.Poll(-1, SelectMode.SelectRead))
                    {
                        int rcount = m_socketServer.Receive(received);

                        //取得收到的数据包
                        byte[] recbuf = new byte[rcount];
                        Array.Copy(received, 0, recbuf, 0, rcount);

                        //记录收到的数据包日志
                        //Log.Record("client.log", "收到客户端数据包：", recbuf, rcount);


                        //判断是否收到了初次登录标志，如果是则回复欢迎信息框
                        if (rcount > 7)
                        {
                            //0x1, 0xF9, 0x0, 0x44, 0x0, 0x1, 0x1B
                            if ((received[0] == 0x01) && (received[1] == 0xF9) && (received[2] == 0x00) && (received[3] == 0x44) && (received[4] == 0x00)
                                && (received[5] == 0x01) && (received[6] == 0x1B))
                            {
                                //回复登录成功欢迎提示框
                                //发送成功提示框信息
                                //其中0x00,0x0E为发送的总字节数
                                byte[] sendbuf9 ={ 0x01, 0xF8, 0x00, 0x0E };
                                byte[] sendbuf10 = Encoding.Default.GetBytes("登录成功！");
                                ArrayList b = new ArrayList();
                                b.AddRange(sendbuf9);
                                b.AddRange(sendbuf10);
                                m_socketServer.Send((byte[])b.ToArray(typeof(byte)));
                                continue;
                            }

                            //Kevin 2010-10-20 Edit
                            //连续接收3次
                            if ((received[0] == 0x01) && (received[1] == 0xFE))
                            {
                                    
                                if (((received[7]==0x02)&&(received[8] == 0x29))||
                                    ((rcount==34)&&(received[7] == 0x02) && (received[8] == 0x0C)) ||                                        
                                    (rcount==51))
                                {
                                    //Log.Record("client.log", "收到客户端数据包：", received, rcount);

                                    byte[] sendbuf8 ={ 0x01, 0xFD, 0x00, 0x06, 0x00, 0x00, 0x01, 0xFD, 0x00, 0x06, 0x00, 0x0C };
                                    byte[] sendbuf9 ={ 0x01, 0xFD, 0x00, 0x06, 0x00, 0x29};
                                    if (!m_sendsucFlag)
                                    {
                                        m_sendsucFlag = true;
                                        m_socketServer.Send(sendbuf8);
                                        m_socketServer.Send(sendbuf9);

                                        //记录发送信息
                                        //Log.Record("client.log", "向客户端发送数据包：", sendbuf8, sendbuf8.Length);
                                    }
                                }

                                continue;
                            }
                        }

                        //如果长度为5，则为心跳包，回复心跳包
                        if (rcount == 5)
                        {
                            //Log.Record("client.log", "收到客户端的数据包为心跳包");
                            m_socketServer.Send(m_heartbuf);

                            //Log.Record("client.log", "向客户端回复心跳包：", m_heartbuf, 5);
                            continue;
                        }
                        
                        //判断是否为指令信息
                        //如果不是指令信息则继续
                        byte[] tmpbuf = new byte[2];
                        Array.Copy(recbuf, 8, tmpbuf, 0, 2);

                        //收到指令信息
                        if ((tmpbuf[0] == cmdhead1[0]) && (tmpbuf[1] == cmdhead1[1]))
                        {
                            //Kevin 2010-04-27 Add
                            //分析指令编号
                            if (rcount > 14)
                            {
                                recno = recbuf[14];
                            }
                            else
                            {
                                recno = 0x20;
                            }

                            MakeSendBufByContent("Received OK", out m_sendbuf);
                            //回复
                            m_socketServer.Send(m_sendbuf);

                            //分析汉字、拼音及汉字编码
                            GetChinesePinYinCode(recbuf);
                            continue;                                
                        }

                        //记录收到数据包信息
                        //Log.Record("client.log", "收到客户端数据包：", received, rcount);
                    }
                    #endregion eterm客户端用户
                }
                catch (Exception e)
                {
                    //Log.Record("client.log",e);
                    //异常，断开Socket连接，退出线程处理
                    m_socketServer.Close();                    
                    break;
                }
               
            }
        }


        /// <summary>
        /// 根据汉字获取汉字的拼音及汉字编码
        /// </summary>
        /// <param name="ch">汉字</param>
        /// <param name="ch">拼音</param>
        /// <param name="ch">汉字编码</param>
        /// <returns>true/false</returns>
        static bool GetChinesePinYinCode(byte[] rec_buf)//,ref string ch, ref string pinyin, ref byte[] b_code)
        {
            //拼音标志
            bool pinyinFlag = false;
            //拼音
            string strPinYin = "";
            ArrayList pinyinlist=new ArrayList();

            //汉字标志
            bool chineseFlag = false;

            //编码
            string strCode = "";
            ArrayList codelist=new ArrayList();

            //分析收到的内容
            int index = 0;
            int len=rec_buf.Length;

            while(index<len)
            {
                //内容起始符
                if((rec_buf[index]==0x0F)&&(rec_buf[index+1]==0x1E))
                {
                    pinyinFlag = true;
                    index += 2;
                    continue;
                }


                if ((rec_buf[index] == 0x1B) && (rec_buf[index + 1] == 0x0E))
                {
                    //汉字开始标志
                    index += 2;
                    chineseFlag = true;
                    continue;
                }

                if ((rec_buf[index] == 0x1B) && (rec_buf[index + 1] == 0x0F))
                {
                    //汉字结束
                    index += 2;
                    chineseFlag = false;
                    continue;
                }

                if (index + 2 == len)
                {
                    break;
                }

                if ((pinyinFlag) && (!chineseFlag))
                {
                    pinyinlist.Add(rec_buf[index]);
                }
                else if ((pinyinFlag) && (chineseFlag))
                {
                    strCode += (" 0x"+Convert.ToString(Convert.ToInt32(rec_buf[index]),16));
                }
                index++;
            }

            strPinYin = ASCIIEncoding.Default.GetString((byte[])pinyinlist.ToArray(typeof(byte)), 0, pinyinlist.Count); 
            string text = strPinYin+"  |   "+strCode;

            SetText(text);

            return true;
        }

        //主窗口显示信息
        static void SetText(string text)
        {
            if (refRichTB.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                refRichTB.Invoke(d, new object[] { text });
            }
            else
            {
                refRichTB.AppendText("\r"+text);
            }
        }

        //把字符串组织成回复格式字节数组（不需要拼音）
        static void MakeSendBufByContent(string sendcontent, out byte[] sendbuf)
        {
            byte[] bufcontent = Encoding.Default.GetBytes(sendcontent);
            sendbuf = null;
            
            ArrayList al = new ArrayList();
            byte[] head1 = { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x20, 0x20, 0x0F, 0x1B, 0x4D };
            byte[] head2 = { 0x0D, 0x1E, 0x1B, 0x62, 0x03 };

            //替换数据包长度字节
            short tmplen = (short)(bufcontent.Length + head1.Length + head2.Length);
            short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
            byte[] lenbuf = BitConverter.GetBytes(count2);

            Array.Copy(lenbuf, 0, head1, 2, 2);

            head1[14] = (byte)((int)recno + 1);

            al.AddRange(head1);
            al.AddRange(bufcontent);
            al.AddRange(head2);
            sendbuf = (byte[])al.ToArray(typeof(byte));
        }

        /// <summary>
        /// 根据Eterm返回的F6获取F7信息
        /// </summary>
        /// <param name="b_code">F6信息</param>
        /// <returns>true/false</returns>
        public static bool GetEtermF7Info(byte[] F6_code,ref byte[] F7_code)
        {
            try
            {
                m_shareport = int.Parse(ConfigurationManager.AppSettings["EtermManage_Port"].ToString());
            }
            catch (Exception ex)
            {
            }

            //关闭已经开启的Eterm客户端程序
            CheckCloseEterm();

            //接收到F6信息，发送F7信息
            lock (ThreadWork.lockX)
            {
                if (eterm3 == null)
                {
                    eterm3 = new Eterm3Lib.ApplicationClass();
                    eterm3.Connect(0, "127.0.0.1:" + Convert.ToString(m_shareport), "$", "$");
                }

                ThreadWork.F6 = F6_code;
                ThreadWork.newflag = true;

                //Log.Record("server.log", "正在准备获取F7数据信息...");

                //等待获取到F7信息
                int count = 0;
                while ((ThreadWork.newflag) && (count < 20))
                {
                    Thread.Sleep(100);
                    count++;
                    continue;
                }

                if (ThreadWork.newflag)
                {
                    //取得F7信息失败
                    if (eterm3 != null)
                    {
                        eterm3.Exit();
                        eterm3 = null;
                    }

                    //停止ThreadWork的监听端口，然后再重新开启监听端口
                    if (ThreadWork.sslServer != null)
                    {
                        ThreadWork.sslServer.Stop();
                    }

                    return false;
                }


                //Log.Record("server.log", "已经获取到F7数据信息...");

                //Console.WriteLine("发送 F7..." + server);

                //Log.Record("server.log", "向航信发送数据包：", FE, 8);

                F7_code = new byte[ThreadWork.F7.Length];
                Array.Copy(ThreadWork.F7, F7_code, F7_code.Length);

                //Log.Record("server.log", "向航信发送F7数据包：", ThreadWork.F7, ThreadWork.F7.Length);
                if (eterm3 != null)
                {
                    eterm3.Exit();
                    eterm3 = null;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查关闭已开启的Eterm客户端程序
        /// </summary>
        static void CheckCloseEterm()
        {
            lock (ThreadWork.lockX)
            {
                //检查Eterm是否运行，如果运行则先关闭
                System.Diagnostics.Process[] CloseID = System.Diagnostics.Process.GetProcessesByName("eTerm3");
                if (CloseID.Length != 0)
                {
                    for (int i = 0; i < CloseID.Length; i++)
                    {
                        if (CloseID[i].Responding && !CloseID[i].HasExited)
                        {
                            //((Eterm3Lib.ApplicationClass)(CloseID[i])).DisConnect();
                            CloseID[i].CloseMainWindow();
                            if (!CloseID[i].HasExited)
                            {
                                CloseID[i].Kill();
                            }
                        }
                        else
                        {
                            CloseID[i].Kill();
                        }
                    }
                }
            }
        }
    }


    //获取F7数据
    public class ThreadWork
    {
        static public byte[] F7 = new byte[4 + 0x80 + 0x80];
        static public bool newflag = false;
        static public byte[] F6 = null;
        static public object lockX = new object();
        static public object lockY = new object();
        static public TcpListener sslServer = null;
        static public bool closeFlag = false;//退出程序标志
        static public int shareport = 351;

        public static void DoWork()
        {
            //Console.WriteLine("启动35000端口。。。。");                
            sslServer = new TcpListener(shareport * 10);
            sslServer.Start();

            //lock (lockY)
            //{
            while (true)
            {
                TcpClient client = null;
                try
                {
                    client = sslServer.AcceptTcpClient();
                }
                catch (Exception)
                {
                    if (closeFlag)
                        break;
                    sslServer.Start();
                    client = sslServer.AcceptTcpClient();
                }

                try
                {
                    //Console.WriteLine("处理 eterm3.exe 连接...");

                    byte[] login = new byte[1024];
                    client.GetStream().Read(login, 0, 162);

                    if (closeFlag)
                    {
                        client.Close();
                        break;
                    }

                    //Log.Record("server1.log", "Eterm,Read", login, 162);

                    byte[] p1 = { 0x00, 0x0a, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x34, 0x51 };

                    client.GetStream().Write(p1, 0, p1.Length);
                    client.GetStream().Flush();

                    byte[] D = new byte[1024];
                    int rcount = client.GetStream().Read(D, 0, 1024);

                    // Log.Record("server1.log", "Eterm,Write", p1, p1.Length);

                    while (F6 == null)
                    {
                        if (closeFlag)
                            break;
                        Thread.Sleep(1000);
                        continue;
                    }

                    while (!newflag)
                    {
                        if (closeFlag)
                            break;
                        Thread.Sleep(1000);
                        continue;
                    }


                    client.GetStream().Write(F6, 0, F6.Length);
                    client.GetStream().Flush();

                    //Log.Record("server1.log", "Eterm,WriteF6", F6, F6.Length);

                    while (true)
                    {
                        if (closeFlag)
                            break;

                        byte[] D2 = new byte[1024];
                        int rcount2 = client.GetStream().Read(D2, 0, 1024);
                        if (rcount2 == 0)
                            continue;
                        //int nSize = D[2] * 256 + D[3] - 4;
                        //client.GetStream().Read(D, 4, nSize);

                        //Log.Record("server1.log", "Eterm,ReadF7", D, rcount);

                        if (D2[1] != 0xF7) continue;

                        Array.Copy(D2, 0, F7, 0, 4 + 0x80 + 0x80);

                        newflag = false;

                        break;
                    }
                    client.Close();

                    F6 = null;


                    //Console.WriteLine("获得 F7...");
                    //client.Close();
                }
                catch (Exception e)
                {
                    F6 = null;
                    newflag = false;
                    client.Close();
                    client = null;
                }
            }
            //}
        }
    }
}
