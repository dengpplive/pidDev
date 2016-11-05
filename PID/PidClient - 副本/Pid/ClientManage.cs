using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PBPid.Base;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using PBPid.DBModel;
using PBPid.ServerManage;
using PBPid.ManageSpace;

namespace PBPid.ClientManage
{
    /// <summary>
    /// By Kevin 2012-06-28
    /// 处理所有Eterm客户端和网站的链接和指令信息
    /// 指令的接收、结果的返回
    /// </summary>
    [Serializable]
    public class ClientManage
    {
        /// <summary>
        /// 监听IP
        /// </summary>
        private string m_ListenIp = "127.0.0.1";

        /// <summary>
        /// 监听端口
        /// </summary>
        private int m_ListenPort = 350;

        /// <summary>
        /// 监听线程 
        /// </summary>
        private Thread m_ListenThread = null;

        /// <summary>
        /// Tcp监听
        /// </summary>
        private TcpListener m_TcpListener = null;

        /// <summary>
        /// 客户端处理类列表
        /// </summary>
        private NoSortHashTable m_ClientInfoList = null;

        /// <summary>
        /// 配置类列表
        /// </summary>
        private PBPid.ServerManage.ServerManage m_ServerManage = null;

        /// <summary>
        /// 客户信息类
        /// </summary>
        private Base_Customers m_Base_Customer = null;

        /// <summary>
        /// 调试信息输出开关
        /// </summary>
        private bool m_DebugFlag = false;

        /// <summary>
        /// 是否输出收发数据包调试信息开关
        /// </summary>
        private bool m_DebugDataFlag = false;

        /// <summary>
        /// 最大数据包字节数
        /// </summary>
        private const int _maxPacket = 1024 * 1024;

        /// <summary>
        /// 网站访问用户名
        /// </summary>
        private string m_WebUser = "webguest";

        /// <summary>
        /// 网站访问帐号密码
        /// </summary>
        private string m_WebPWD = "webguest";

        /// <summary>
        /// 网站回复数据包头
        /// </summary>
        private readonly byte[] m_WebHeadPack = { 0xEE, 0xEF };

        /// <summary>
        /// 网站回复数据包尾
        /// </summary>
        private readonly byte[] m_WebEndPack = { 0xFF, 0xEF };

        /// <summary>
        /// PID放大帐号信息
        /// </summary>
        private List<Base_UserConfigManage> m_UserConfigManageList = null;

        /// <summary>
        /// 上下文信息
        /// </summary>
        private List<Base_CommandManage> m_BaseCommandManageList = null;

        /// <summary>
        /// 汉字拼音编码信息
        /// </summary>
        private List<Base_PinYin> m_BasePinYinList = null;

        /// <summary>
        /// 城市信息
        /// </summary>
        private List<Base_CityInfoManage> m_BaseCityInfoManageList = null;

        /// <summary>
        /// 最大指令超时时间（秒）
        /// </summary>
        private const int m_MaxOutTimes = 6;

        /// <summary>
        /// 发送及接收到的信息（用于窗口内容显示和日志文档记录）
        /// </summary>
        private ArrayList m_ArrayContents = null;

        /// <summary>
        /// 监听IP
        /// </summary>
        public string ListenIP
        {
            get { return m_ListenIp; }
            set { m_ListenIp = value; }
        }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int ListenPort
        {
            get { return m_ListenPort; }
            set { m_ListenPort = value; }
        }

        /// <summary>
        /// 调试信息输出开关
        /// </summary>
        public bool DebugFlag
        {
            get { return m_DebugFlag; }
            set { m_DebugFlag = value; }
        }

        /// <summary>
        /// 收发数据包调试信息输出开关
        /// </summary>
        public bool DebugDataFlag
        {
            get { return m_DebugDataFlag; }
            set { m_DebugDataFlag = value; }
        }

        /// <summary>
        /// 网站访问用户名
        /// </summary>
        public string WebUserName
        {
            get { return m_WebUser; }
            set { m_WebUser = value; }
        }

        /// <summary>
        /// 网站访问密码
        /// </summary>
        public string WebPassWord
        {
            get { return m_WebPWD; }
            set { m_WebPWD = value; }
        }

        /// <summary>
        /// PID放大帐号信息
        /// </summary>
        public List<Base_UserConfigManage> UserConfigManageList
        {
            get { return m_UserConfigManageList; }
            set { m_UserConfigManageList = value; }
        }

        /// <summary>
        /// 配置类列表
        /// </summary>
        public ServerManage.ServerManage ServerManage
        {
            get { return m_ServerManage; }
            set { m_ServerManage = value; }
        }

        /// <summary>
        /// 发送及接收到的信息（用于窗口内容显示和日志文档记录）
        /// </summary>
        public ArrayList ArrayContents
        {
            set { m_ArrayContents = value; }
            get { return m_ArrayContents; }
        }

        /// <summary>
        /// 上下文信息
        /// </summary>
        public List<Base_CommandManage> BaseCommandManageList
        {
            set { m_BaseCommandManageList = value; }
            get { return m_BaseCommandManageList; }
        }

        /// <summary>
        /// 汉字拼音编码信息
        /// </summary>
        public List<Base_PinYin> BasePinYinList
        {
            get { return m_BasePinYinList; }
            set { m_BasePinYinList = value; }
        }

        /// <summary>
        /// 城市信息
        /// </summary>
        public List<Base_CityInfoManage> BaseCityInfoManageList
        {
            get { return m_BaseCityInfoManageList; }
            set { m_BaseCityInfoManageList = value; }
        }

        /// <summary>
        /// 客户信息类
        /// </summary>
        public Base_Customers Base_Customer
        {
            get { return m_Base_Customer; }
            set { m_Base_Customer = value; }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                if (m_ListenThread != null)
                {
                    Stop();
                }

                if (m_ClientInfoList == null)
                {
                    m_ClientInfoList = new NoSortHashTable();
                }

                //
                m_ListenThread = new Thread(new ThreadStart(ListenProc));
                m_ListenThread.Start();

                if (m_DebugFlag)
                {
                    Log.Record("client.log", "开启客户端监听服务线程...");
                }

                return true;
            }
            catch (Exception ex)
            {
                if (m_DebugFlag)
                {
                    Log.Record("client.log", "开启客户端监听服务线程出错，错误信息：" + ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            try
            {
                //停止监听线程
                if (m_ListenThread != null)
                {
                    m_ListenThread.Abort();
                    m_ListenThread = null;

                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "停止客户端监听服务线程...");
                    }
                }

                if (m_TcpListener != null)
                {
                    m_TcpListener.Stop();

                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "停止客户端监听服务...");
                    }
                }
                m_TcpListener = null;

                if (m_DebugFlag)
                {
                    Log.Record("client.log", "准备停止客户端线程及客户端Socket服务...");
                }

                ////释放所有的客户处理Socket
                if (m_ClientInfoList.Count != 0)
                {
                    foreach (ClientInfo clientmgr in m_ClientInfoList.Values)
                    {
                        //clientmgr.closeFlag = true;

                        if (clientmgr.ClientThread != null && clientmgr.ClientThread.IsAlive)
                        {
                            //关闭Eterm配置 Thread
                            clientmgr.ClientThread.Abort();
                        }
                        clientmgr.ClientThread = null;

                        if (clientmgr.ClientSocket != null)
                        {
                            //关闭Eterm配置 Socket
                            clientmgr.ClientSocket.Shutdown(SocketShutdown.Both);
                        }

                        clientmgr.ClientSocket = null;
                    }

                    //清空列表
                    m_ClientInfoList.Clear();
                    m_ClientInfoList = null;
                }

                if (m_DebugFlag)
                {
                    Log.Record("client.log", "停止客户端线程及客户端Socket服务完毕...");
                }

                return true;
            }
            catch (Exception ex)
            {
                if (m_DebugFlag)
                {
                    Log.Record("client.log", "停止服务出错，错误信息：" + ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// 监听线程处理方法
        /// </summary>
        public void ListenProc()
        {
            IPAddress _ip = IPAddress.Parse(m_ListenIp);

            if (DebugFlag)
            {
                Log.Record("client.log", "开启监听：IP地址" + m_ListenIp + "；端口" + m_ListenPort.ToString() + "...");
            }

            m_TcpListener = new TcpListener(m_ListenPort);
            m_TcpListener.Start();

            while (true)
            {
                //可用配置的Id
                int EtermId = -1;

                //可用配置的名称
                string EtermName = "";

                //登录用户类型 1 Eterm客户端；  2 网站用户
                int userType = 1;

                try
                {
                    Socket newclient = m_TcpListener.AcceptSocket();

                    if (newclient.Connected)
                    {
                        ClientInfo clientinfo = null;

                        NetworkStream ns = new NetworkStream(newclient);

                        //读取用户名、密码信息
                        byte[] packetBuff = new byte[_maxPacket];
                        int rcount = ns.Read(packetBuff, 0, _maxPacket);

                        if (rcount == 0)
                        {
                            if (DebugFlag)
                            {
                                Log.Record("client.log", "收到数据包长度为0，断开...");
                            }
                            //已断开
                            ns.Close();
                            newclient.Close();
                            continue;
                        }

                        if (DebugFlag)
                        {
                            Log.Record("client.log", "收到客户端连接请求...");//：", packetBuff, rcount);
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("client.log", "收到客户端连接请求数据包：", packetBuff, rcount);
                        }

                        //用户名
                        string tmpuser = "";
                        //密码
                        string tmppwd = "";
                        //指令
                        string tmpcmd = "";
                        string tmpcmd2 = "";

                        string tmpstr = "";
                        string tmpstr2 = "";

                        //接收到指令时间
                        DateTime ReceiveTime = DateTime.Now;

                        Base_UserConfigManage tmpUserConfigManage = null;

                        #region 网站用户
                        if ((packetBuff[0] == m_WebHeadPack[0]) && (packetBuff[1] == m_WebHeadPack[1]) &&
                            (packetBuff[rcount - 2] == m_WebEndPack[0]) && (packetBuff[rcount - 1] == m_WebEndPack[1]))
                        {
                            if (DebugFlag)
                            {
                                Log.Record("client.log", "收到网站用户数据...");
                            }

                            //创建网站处理类和线程进行数据处理
                            WebInfo tmpWebInfo = new WebInfo();
                            //调试标志
                            tmpWebInfo.m_DebugFlag = m_DebugFlag;
                            //接收到的数据包
                            tmpWebInfo.packetBuff = new byte[rcount];
                            //传输TCP连接
                            tmpWebInfo.newclient = newclient;
                            //传输数据流
                            tmpWebInfo.ns = ns;
                            //输出显示列表
                            tmpWebInfo.m_ArrayContents = m_ArrayContents;
                            //配置处理类
                            tmpWebInfo.m_ServerManage = m_ServerManage;
                            //接收到指令的时间
                            tmpWebInfo.ReceiveTime = ReceiveTime;

                            Array.Copy(packetBuff, tmpWebInfo.packetBuff, rcount);
                            Thread tmpWebThread = new Thread(new ThreadStart(tmpWebInfo.WebProcFunc));
                            tmpWebThread.Start();
                            continue;                            
                        }
                        #endregion 网站用户
                        #region Eterm客户端
                        else if(packetBuff[0]==0x01 && packetBuff[1]==0xa2)
                        {
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

                            //失败对话框的头结构
                            byte[] sendbuf = { 0x00, 0x37, 0x00, 0x31, 0x30, 0x30, 0x30, 0x31, 0x3A };
                            ArrayList a = new ArrayList();

                            byte[] bcount;//记录数据包的长度
                            short count = 0;//长度                            

                            //检测用户名、密码，如果不是网站用户，则检测是否已经连接
                            if (tmpuser != m_WebUser)
                            {

                                int index = 0;
                                //是否查找到放大帐号名
                                bool findFlag = false;

                                //查询用户信息
                                while ((m_UserConfigManageList != null) && (index < m_UserConfigManageList.Count))
                                {
                                    tmpUserConfigManage = m_UserConfigManageList[index];
                                    if (tmpUserConfigManage.User_Name == tmpuser)
                                    {
                                        findFlag = true;
                                        break;
                                    }
                                    index++;
                                }

                                if (findFlag)
                                {
                                    //检查用户的有效性
                                    int iresult = CheckUser(tmpUserConfigManage, tmppwd);

                                    switch (iresult)
                                    {
                                        //验证通过
                                        case 0:
                                            //检测该帐号是否已经连接
                                            if (CheckIfLogined(tmpuser))
                                            {
                                                //记录日志
                                                //Log.Record("client.log", tmpuser + " 登录失败：该帐号已被他人占用！");
                                                //回复该帐号已经连接，拒绝再次连接
                                                byte[] sendbuf2 = Encoding.Default.GetBytes("登录失败：该帐号已在其他计算机登录！");

                                                //替换数据包长度信息
                                                short count2 = (short)(sendbuf2.Length + 9);
                                                count = count2;

                                                //转换为网络字节顺序
                                                count2 = System.Net.IPAddress.HostToNetworkOrder(count2);
                                                bcount = BitConverter.GetBytes(count2);
                                                Array.Copy(bcount, 0, sendbuf, 2, 2);

                                                a.AddRange(sendbuf);
                                                a.AddRange(sendbuf2);

                                                if (m_DebugFlag)
                                                {
                                                    Log.Record("client.log", tmpuser + "登录失败：该帐号已在其他计算机登录！");
                                                }

                                                ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                                ns.Flush();
                                                ns.Close();
                                            }
                                            break;

                                        //密码不正确
                                        case -1:
                                            //记录日志
                                            //Log.Record("client.log", tmpuser + " 登录失败：密码不正确！");
                                            byte[] sendbuf3 = Encoding.Default.GetBytes("登录失败：密码不正确！");

                                            //替换数据包长度信息
                                            short count3 = (short)(sendbuf3.Length + 9);
                                            count = count3;

                                            //转换为网络字节顺序
                                            count3 = System.Net.IPAddress.HostToNetworkOrder(count3);
                                            bcount = BitConverter.GetBytes(count3);
                                            Array.Copy(bcount, 0, sendbuf, 2, 2);

                                            a.AddRange(sendbuf);
                                            a.AddRange(sendbuf3);

                                            if (m_DebugFlag)
                                            {
                                                Log.Record("client.log", tmpuser + "登录失败：密码不正确！");
                                            }

                                            ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                            ns.Flush();
                                            ns.Close();

                                            break;

                                        //帐户已过期
                                        case -2:
                                            //记录日志
                                            //Log.Record("client.log", tmpuser + " 登录失败：该帐号已过期！");
                                            byte[] sendbuf4 = Encoding.Default.GetBytes("登录失败：该帐号已过期！");

                                            //替换数据包长度信息
                                            short count4 = (short)(sendbuf4.Length + 9);
                                            count = count4;

                                            //转换为网络字节顺序
                                            count4 = System.Net.IPAddress.HostToNetworkOrder(count4);
                                            bcount = BitConverter.GetBytes(count4);
                                            Array.Copy(bcount, 0, sendbuf, 2, 2);

                                            a.AddRange(sendbuf);
                                            a.AddRange(sendbuf4);

                                            if (m_DebugFlag)
                                            {
                                                Log.Record("client.log", tmpuser + "登录失败：该帐号已过期！");
                                            }

                                            ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                            ns.Flush();
                                            ns.Close();
                                            break;

                                        //帐号被禁用
                                        case -3:
                                            //记录日志
                                            //Log.Record("client.log", tmpuser + " 登录失败：该帐号已过期！");
                                            byte[] sendbuf5 = Encoding.Default.GetBytes("登录失败：该帐号已禁用！");

                                            //替换数据包长度信息
                                            short count5 = (short)(sendbuf5.Length + 9);
                                            count = count5;

                                            //转换为网络字节顺序
                                            count4 = System.Net.IPAddress.HostToNetworkOrder(count5);
                                            bcount = BitConverter.GetBytes(count5);
                                            Array.Copy(bcount, 0, sendbuf, 2, 2);

                                            a.AddRange(sendbuf);
                                            a.AddRange(sendbuf5);

                                            if (m_DebugFlag)
                                            {
                                                Log.Record("client.log", tmpuser + "登录失败：该帐号已禁用！");
                                            }

                                            ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                            ns.Flush();
                                            ns.Close();
                                            break;
                                    }
                                }
                                else
                                {
                                    //记录日志
                                    //Log.Record("client.log", tmpuser + " 该帐号不存在或已被禁用！");
                                    //帐号不存在
                                    byte[] sendbuf5 = Encoding.Default.GetBytes("登录失败：该帐号不存在！");


                                    //替换数据包长度信息
                                    short count5 = (short)(sendbuf5.Length + 9);
                                    count = count5;

                                    //转换为网络字节顺序
                                    count5 = System.Net.IPAddress.HostToNetworkOrder(count5);
                                    bcount = BitConverter.GetBytes(count5);
                                    Array.Copy(bcount, 0, sendbuf, 2, 2);

                                    a.AddRange(sendbuf);
                                    a.AddRange(sendbuf5);

                                    if (m_DebugFlag)
                                    {
                                        Log.Record("client.log", tmpuser + "登录失败：该帐号不存在！");
                                    }

                                    ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                    ns.Flush();
                                    ns.Close();
                                    continue;
                                }
                            }
                            else
                            {
                                //检测网站用户的密码是否正确
                                if (tmppwd != m_WebPWD)
                                {
                                    //密码不正确
                                    byte[] sendbuf6 = Encoding.Default.GetBytes("登录失败：密码不正确！");

                                    //替换数据包长度信息
                                    short count6 = (short)(sendbuf6.Length + 9);
                                    count = count6;

                                    //转换为网络字节顺序
                                    count6 = System.Net.IPAddress.HostToNetworkOrder(count6);
                                    bcount = BitConverter.GetBytes(count6);
                                    Array.Copy(bcount, 0, sendbuf, 2, 2);

                                    a.AddRange(sendbuf);
                                    a.AddRange(sendbuf6);

                                    if (m_DebugFlag)
                                    {
                                        Log.Record("client.log", tmpuser + "登录失败：密码不正确！");
                                    }

                                    ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                    newclient.Close();
                                    continue;
                                }
                            }

                            //回复成功标志
                            byte[] sendbuf7 ={ 0x00, 0x14, 0x01, 0x00, 0x03, 
                                0x00, 0x00, 0x00, 0x41, 0x51, 
                                0x0C, 0x00, 0x00, 0x8C, 0x8C, 
                                0x29, 0x00, 0x00, 0xA9, 0xA9 };
                            newclient.Send(sendbuf7);
                            //回复成功标志
                            if (m_DebugFlag)
                            {
                                Log.Record("client.log", "用户名：" + tmpuser + "登录成功！");
                            }

                            #region 创建客户端处理线程并添加到客户端处理类列表
                            //创建客户端连接类
                            clientinfo = new ClientInfo();

                            //对于网站连接不统计
                            if (userType != 2)
                            {
                                //添加到登录客户列表中
                                lock (m_ClientInfoList)
                                {
                                    m_ClientInfoList.Add(tmpuser, clientinfo);
                                }
                            }


                            //创建客户端服务线程
                            Thread clientservice = new Thread(new ThreadStart(clientinfo.ClientProcFunc));
                            clientinfo.UserName = tmpuser;
                            clientinfo.ClientSocket = newclient;
                            clientinfo.UserConfigManage = tmpUserConfigManage;

                            //客户端处理线程
                            clientinfo.ClientThread = clientservice;
                            //可用配置列表
                            clientinfo.ServerManage = m_ServerManage;
                            //配置序号
                            clientinfo.EtermId = EtermId;
                            //输出调试标志
                            clientinfo.DebugFlag = m_DebugFlag;
                            //输出调试数据包标志
                            clientinfo.DebugDataFlag = m_DebugDataFlag;
                            //上下文处理类
                            clientinfo.BaseCommandManageList = m_BaseCommandManageList;
                            //指令发送返回列表
                            clientinfo.ArrayContents = m_ArrayContents;
                            //汉字拼音编码列表
                            clientinfo.BasePinYinList = m_BasePinYinList;
                            //城市信息
                            clientinfo.BaseCityInfoManageList = m_BaseCityInfoManageList;
                            clientservice.Start();
                            #endregion 创建客户端处理线程并添加到客户端处理类列表
                            continue;
                        }
                        #endregion Eterm客户端
                        #region 客户端软件
                        else if (packetBuff[0] == 0x01 && packetBuff[1] == 0xaa)
                        {
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

                            //失败对话框的头结构
                            byte[] sendbuf = { 0x00, 0x37, 0x00, 0x31, 0x30, 0x30, 0x30, 0x31, 0x3A };
                            ArrayList a = new ArrayList();

                            byte[] bcount;//记录数据包的长度
                            short count = 0;//长度                            

                            //检测用户名、密码，如果不是网站用户，则检测是否已经连接
                            if (tmpuser != m_WebUser)
                            {

                                int index = 0;
                                //是否查找到放大帐号名
                                bool findFlag = false;

                                //查询用户信息
                                while ((m_UserConfigManageList != null) && (index < m_UserConfigManageList.Count))
                                {
                                    tmpUserConfigManage = m_UserConfigManageList[index];
                                    if (tmpUserConfigManage.User_Name == tmpuser)
                                    {
                                        findFlag = true;
                                        break;
                                    }
                                    index++;
                                }

                                if (findFlag)
                                {
                                    //检查用户的有效性
                                    int iresult = CheckUser(tmpUserConfigManage, tmppwd);

                                    switch (iresult)
                                    {
                                        //验证通过
                                        case 0:
                                            //检测该帐号是否已经连接
                                            if (CheckIfLogined(tmpuser))
                                            {
                                                //记录日志
                                                //Log.Record("client.log", tmpuser + " 登录失败：该帐号已被他人占用！");
                                                //回复该帐号已经连接，拒绝再次连接
                                                byte[] sendbuf2 = Encoding.Default.GetBytes("登录失败：该帐号已在其他计算机登录！");

                                                //替换数据包长度信息
                                                short count2 = (short)(sendbuf2.Length + 9);
                                                count = count2;

                                                //转换为网络字节顺序
                                                count2 = System.Net.IPAddress.HostToNetworkOrder(count2);
                                                bcount = BitConverter.GetBytes(count2);
                                                Array.Copy(bcount, 0, sendbuf, 2, 2);

                                                a.AddRange(sendbuf);
                                                a.AddRange(sendbuf2);

                                                if (m_DebugFlag)
                                                {
                                                    Log.Record("client.log", tmpuser + "登录失败：该帐号已在其他计算机登录！");
                                                }

                                                ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                                ns.Flush();
                                                ns.Close();
                                            }
                                            break;

                                        //密码不正确
                                        case -1:
                                            //记录日志
                                            //Log.Record("client.log", tmpuser + " 登录失败：密码不正确！");
                                            byte[] sendbuf3 = Encoding.Default.GetBytes("登录失败：密码不正确！");

                                            //替换数据包长度信息
                                            short count3 = (short)(sendbuf3.Length + 9);
                                            count = count3;

                                            //转换为网络字节顺序
                                            count3 = System.Net.IPAddress.HostToNetworkOrder(count3);
                                            bcount = BitConverter.GetBytes(count3);
                                            Array.Copy(bcount, 0, sendbuf, 2, 2);

                                            a.AddRange(sendbuf);
                                            a.AddRange(sendbuf3);

                                            if (m_DebugFlag)
                                            {
                                                Log.Record("client.log", tmpuser + "登录失败：密码不正确！");
                                            }

                                            ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                            ns.Flush();
                                            ns.Close();

                                            break;

                                        //帐户已过期
                                        case -2:
                                            //记录日志
                                            //Log.Record("client.log", tmpuser + " 登录失败：该帐号已过期！");
                                            byte[] sendbuf4 = Encoding.Default.GetBytes("登录失败：该帐号已过期！");

                                            //替换数据包长度信息
                                            short count4 = (short)(sendbuf4.Length + 9);
                                            count = count4;

                                            //转换为网络字节顺序
                                            count4 = System.Net.IPAddress.HostToNetworkOrder(count4);
                                            bcount = BitConverter.GetBytes(count4);
                                            Array.Copy(bcount, 0, sendbuf, 2, 2);

                                            a.AddRange(sendbuf);
                                            a.AddRange(sendbuf4);

                                            if (m_DebugFlag)
                                            {
                                                Log.Record("client.log", tmpuser + "登录失败：该帐号已过期！");
                                            }

                                            ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                            ns.Flush();
                                            ns.Close();
                                            break;

                                        //帐号被禁用
                                        case -3:
                                            //记录日志
                                            //Log.Record("client.log", tmpuser + " 登录失败：该帐号已过期！");
                                            byte[] sendbuf5 = Encoding.Default.GetBytes("登录失败：该帐号已禁用！");

                                            //替换数据包长度信息
                                            short count5 = (short)(sendbuf5.Length + 9);
                                            count = count5;

                                            //转换为网络字节顺序
                                            count4 = System.Net.IPAddress.HostToNetworkOrder(count5);
                                            bcount = BitConverter.GetBytes(count5);
                                            Array.Copy(bcount, 0, sendbuf, 2, 2);

                                            a.AddRange(sendbuf);
                                            a.AddRange(sendbuf5);

                                            if (m_DebugFlag)
                                            {
                                                Log.Record("client.log", tmpuser + "登录失败：该帐号已禁用！");
                                            }

                                            ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                            ns.Flush();
                                            ns.Close();
                                            break;
                                    }
                                }
                                else
                                {
                                    //记录日志
                                    //Log.Record("client.log", tmpuser + " 该帐号不存在或已被禁用！");
                                    //帐号不存在
                                    byte[] sendbuf5 = Encoding.Default.GetBytes("登录失败：该帐号不存在！");


                                    //替换数据包长度信息
                                    short count5 = (short)(sendbuf5.Length + 9);
                                    count = count5;

                                    //转换为网络字节顺序
                                    count5 = System.Net.IPAddress.HostToNetworkOrder(count5);
                                    bcount = BitConverter.GetBytes(count5);
                                    Array.Copy(bcount, 0, sendbuf, 2, 2);

                                    a.AddRange(sendbuf);
                                    a.AddRange(sendbuf5);

                                    if (m_DebugFlag)
                                    {
                                        Log.Record("client.log", tmpuser + "登录失败：该帐号不存在！");
                                    }

                                    ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                    ns.Flush();
                                    ns.Close();
                                    continue;
                                }
                            }
                            else
                            {
                                //检测网站用户的密码是否正确
                                if (tmppwd != m_WebPWD)
                                {
                                    //密码不正确
                                    byte[] sendbuf6 = Encoding.Default.GetBytes("登录失败：密码不正确！");

                                    //替换数据包长度信息
                                    short count6 = (short)(sendbuf6.Length + 9);
                                    count = count6;

                                    //转换为网络字节顺序
                                    count6 = System.Net.IPAddress.HostToNetworkOrder(count6);
                                    bcount = BitConverter.GetBytes(count6);
                                    Array.Copy(bcount, 0, sendbuf, 2, 2);

                                    a.AddRange(sendbuf);
                                    a.AddRange(sendbuf6);

                                    if (m_DebugFlag)
                                    {
                                        Log.Record("client.log", tmpuser + "登录失败：密码不正确！");
                                    }

                                    ns.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                                    newclient.Close();
                                    continue;
                                }
                            }

                            //回复成功标志
                            byte[] sendbuf7 ={ 0x00, 0x14, 0x01, 0x00, 0x03, 
                                0x00, 0x00, 0x00, 0x41, 0x51, 
                                0x0C, 0x00, 0x00, 0x8C, 0x8C, 
                                0x29, 0x00, 0x00, 0xA9, 0xA9 };
                            ArrayList b = new ArrayList();
                            b.AddRange(sendbuf7);
                            //添加所属Office号
                            b.AddRange(Encoding.Default.GetBytes(tmpUserConfigManage.User_Office));
                            newclient.Send((byte[])b.ToArray(typeof(byte)));
                            //回复成功标志
                            if (m_DebugFlag)
                            {
                                Log.Record("client.log", "用户名：" + tmpuser + "登录成功！");
                            }

                            #region 创建客户端处理线程并添加到客户端处理类列表
                            //创建客户端连接类
                            clientinfo = new ClientInfo();

                            //对于网站连接不统计
                            if (userType != 2)
                            {
                                //添加到登录客户列表中
                                lock (m_ClientInfoList)
                                {
                                    m_ClientInfoList.Add(tmpuser, clientinfo);
                                }
                            }


                            //创建客户端服务线程
                            Thread clientservice = new Thread(new ThreadStart(clientinfo.ClientProcFunc));
                            clientinfo.UserName = tmpuser;
                            clientinfo.ClientSocket = newclient;
                            clientinfo.UserConfigManage = tmpUserConfigManage;

                            //客户端处理线程
                            clientinfo.ClientThread = clientservice;
                            //可用配置列表
                            clientinfo.ServerManage = m_ServerManage;
                            //配置序号
                            clientinfo.EtermId = EtermId;
                            //输出调试标志
                            clientinfo.DebugFlag = m_DebugFlag;
                            //输出调试数据包标志
                            clientinfo.DebugDataFlag = m_DebugDataFlag;
                            //上下文处理类
                            clientinfo.BaseCommandManageList = m_BaseCommandManageList;
                            //指令发送返回列表
                            clientinfo.ArrayContents = m_ArrayContents;
                            //汉字拼音编码列表
                            clientinfo.BasePinYinList = m_BasePinYinList;
                            //城市信息
                            clientinfo.BaseCityInfoManageList = m_BaseCityInfoManageList;
                            clientservice.Start();
                            #endregion 创建客户端处理线程并添加到客户端处理类列表
                            continue;
                        }
                        #endregion 客户端软件
                        #region 非法连接
                        else
                        {
                            ns.Close();
                        }
                        #endregion 非法连接
                    }
                }
                catch (Exception e)
                {
                    //异常释放配置的占用
                    if (EtermId != -1)
                    {
                        m_ServerManage.FreeEtermInfo(EtermId);
                    }

                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "客户端处理线程出现异常，异常信息：" + e.Message);
                    }
                }
            }
        }

        #region 分析航信返回的数据包（把汉字编码转换成正常汉字）
        //分析航信返回数据信息
        private string AnalyseServerContent(byte[] buf, int count)
        {
            string result = "";

            //主要是解析中文
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };
            ArrayList al = new ArrayList();
            ArrayList al2 = new ArrayList();

            int index = 0;
            int tmpcount = 0;
            bool isChinese = false;//是否为汉字

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);
            byte tmpbyte;

            while (index < count)
            {
                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0E))
                {
                    //汉字开始标志
                    al2.Clear();
                    index += 2;
                    isChinese = true;
                    continue;
                }

                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0F))
                {
                    string tmpcontent = "";
                    bool doubleFlag = false;
                    for (int i = 0; i < (al2.Count / 2); i++)
                    {
                        //如果有占用4个字节的汉字，则跳过一次
                        if (doubleFlag)
                        {
                            doubleFlag = false;
                            continue;
                        }
                        if ((byte)(al2[i * 2]) == 0x78)
                        {
                            byte[] cbuf = new byte[4];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            cbuf[2] = (byte)(al2[i * 2 + 2]);
                            cbuf[3] = (byte)(al2[i * 2 + 3]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            //index += 2;
                            doubleFlag = true;
                        }
                        else
                        {
                            byte[] cbuf = new byte[2];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            doubleFlag = false;
                        }
                    }

                    //处理并添加汉字信息
                    //把内容转换为string，判断是否为汉字，如果不是则把高低位互换然后把低位加10
                    //string tmpcontent= Encoding.Default.GetString((byte[])al2.ToArray(typeof(byte)));

                    //for (int i = 0; i < tmpcontent.Length; i++)
                    //{
                    //    int code = Char.ConvertToUtf32(tmpcontent, i);

                    //    //非汉字，需要处理，把高低位互换然后把低位加10
                    //    if ((code < chfrom)||(code > chend))
                    //    {
                    //        tmpbyte =  (byte)al2[i * 2];
                    //        al2[i * 2] = al2[i * 2 + 1];
                    //        al2[i * 2 + 1] = (byte)(tmpbyte + 0x0A);
                    //    }
                    //}

                    //al2.Clear();
                    //al2.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //把转换过的汉字追加到al中
                    al.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //汉字结束
                    index += 2;
                    isChinese = false;
                    continue;
                }

                tmpbyte = buf[index];

                //如果是汉字则处理
                if (isChinese)
                {
                    //tmpbyte += 0x80;
                    al2.Add(tmpbyte);
                }
                else
                {
                    al.Add(tmpbyte);
                }
                tmpcount++;
                index++;
            }

            //从第20位开始，读取到末尾倒数第三位
            if (buf.Length < 23)
                return "";

            if ((buf[17] == 0x1B) && (buf[18] == 0x4D))
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, tmpcount - 23);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, al.Count - 23);
            }
            else
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, tmpcount - 21);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, al.Count - 21);
            }

            return result;
        }
        #endregion 分析航信返回的数据包（把汉字编码转换成正常汉字）

        #region 格式化内容，每行80
        private string FormatResContent(string rescontent)
        {
            string tmpcontent = "";
            string tmpcontent1 = rescontent;
            string tmpstr = "";
            int pos = -1;
            pos = tmpcontent1.IndexOf('\r');
            while (pos != -1)
            {
                tmpstr = tmpcontent1.Substring(0, pos);
                if (tmpstr.Length > 80)
                {
                    string tmpstr2 = tmpstr;
                    while (tmpstr2.Length > 80)
                    {
                        //添加换行符
                        tmpcontent += tmpstr2.Substring(0, 80) + "\r";// + tmpstr.Substring(80) + "\r");
                        tmpstr2 = tmpstr2.Substring(80);
                    }
                    if (tmpstr2.Length > 0)
                    {
                        tmpcontent += tmpstr2 + "\r";
                    }
                }
                else
                {
                    tmpcontent += (tmpstr + "\r");
                }

                tmpcontent1 = tmpcontent1.Substring(pos + 1);
                pos = tmpcontent1.IndexOf('\r');
            }

            if (tmpcontent == "")
            {
                tmpcontent = rescontent;
            }

            return tmpcontent;
        }
        #endregion 格式化内容，每行80

        #region 把AVH结果格式化为网站返回格式
        //把AVH内容格式化为网站固定格式
        private void AnalyseAVHContentToWebInfo(string sourcmd, string sourcontent, out string destcontent)
        {
            destcontent = "";

            string tmpdate = "";
            try
            {
                tmpdate = sourcmd.Substring(11, 5);

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //返回信息与查询日期不一致
                if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
                {
                    destcontent = "当日无航班或者已经销售完毕！";
                    return;
                }

                tmpdate = getStandardDate(sourcontent.Substring(1, pos1 - 1));

                #region AVH结果信息（例子）
                // 30AUG(THU) CTUBJS DIRECT ONLY                                                  
                //1-  CA4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>                                                                   T2 T3  2:35 
                //2  *ZH4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>   CA4193                                                          T2 T3  2:35 
                //3   3U8881  DS# FA PA AA YA BA TA WS HA MA GA  CTUPEK 0730   1005   330 0 S  E  
                //>               SA LA QA EA VS US RS KQ NQ XS ZS                    T1 T3  2:35 
                //4   CA4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>                                                                   T2 T3  2:20 
                //5  *ZH4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>   CA4113                                                          T2 T3  2:20 
                //6   KN2611  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>               NQ RQ SQ VQ TQ WQ GQ XQ QA US I3                    T2 --  1:50 
                //7  *MU8071  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>   KN2611      NQ RQ SQ VQ TQ WQ GQ XQ QA I3                       T2 --  1:50 
                //8   CA4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>                                                                   T2 T3  2:25 
                //9+ *ZH4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>   CA4101                                                          T2 T3  2:25 
                //**  CZ  FARE    CTUPEK/PEKCTU   YI:CZ/TZ305                                   
                #endregion AVH结果信息（例子）

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[1] { '\r' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 
                string flongtime = "";//飞行时长

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理
                        //判断是否上一航段还有舱位信息没有添加完毕
                        if (tmpclassinfo != "")
                        {
                            //分析并添加舱位信息
                            string[] sl2 = tmpclassinfo.Split(' ');
                            for (int j = 0; j < sl2.Length; j++)
                            {
                                if (sl2[j].Trim().Length != 0)
                                {
                                    //判断舱位状态
                                    //去掉关闭的无效舱位
                                    if (sl2[j].Length == 2)
                                    {
                                        //int classnum = 0;
                                        string tmpstr4 = sl2[j].Substring(1, 1);

                                        if (tmpstr4.ToLower() == "a")
                                        {
                                            tmpstr4 = "9";
                                        }

                                        //跳过关闭的无效舱位
                                        //L      没有可利用座位,但旅客可以候补
                                        //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                        //S      因达到限制销售数而没有可利用座位,但可以候补
                                        //C      该等级彻底关闭,不允许候补或申请
                                        //X      该等级取消, 不允许候补或申请
                                        //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                        if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                            || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                            || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                        {
                                            continue;
                                        }

                                        //大于9个座位显示9，其他不变
                                        //int.TryParse(tmpstr4, out classnum);
                                        //if (classnum != 0)
                                        //{
                                        classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                        // }
                                    }
                                }
                            }

                            //避免加入重复航班信息
                            if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                            {
                                if (destcontent.Trim() == "")
                                {
                                    destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                                }
                                else
                                {
                                    destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                                }
                            }

                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                        }
                        #endregion 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理

                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();

                        byte[] tmpbuf = Encoding.Default.GetBytes(flightno);
                        if ((tmpbuf[tmpbuf.Length - 2] == 0x20) && (tmpbuf[tmpbuf.Length - 1] == 0x1C))
                        {
                            flightno = flightno.Substring(0, flightno.Length - 2).Trim();
                        }

                        //此处不做空格处理
                        tmpstr = tmpstr.Substring(10);

                        #region 跳过被屏蔽的航空公司信息
                        //判断是否被屏蔽的航空公司
                        if (tmpstr.Substring(0, 31).Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司信息

                        //舱位信息
                        tmpclassinfo = tmpstr.Substring(0, 31).Trim();
                        tmpstr = tmpstr.Substring(31).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatflag = "1";
                        }
                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司信息
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 1).Trim() != ">")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            //添加舱位信息
                            tmpclassinfo += " "+sl[i].Substring(11, 56).Trim();
                            tmpstr = tmpstr.Substring(68);

                            //航站楼信息
                            flightterminal = tmpstr.Substring(0, 2) + tmpstr.Substring(3, 2);
                            tmpstr = tmpstr.Substring(5).Trim();

                            //飞行时长
                            flongtime = tmpstr;
                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                #region 添加末尾数据信息
                if (tmpclassinfo != "")
                {
                    //分析并添加舱位信息
                    string[] sl2 = tmpclassinfo.Split(' ');
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j].Trim().Length != 0)
                        {
                            //判断舱位状态
                            if (sl2[j].Length == 2)
                            {
                                //int classnum = 0;
                                string tmpstr4 = sl2[j].Substring(1, 1);

                                if (tmpstr4.ToLower() == "a")
                                {
                                    tmpstr4 = "9";
                                }

                                //跳过关闭的无效舱位
                                //L      没有可利用座位,但旅客可以候补
                                //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                //S      因达到限制销售数而没有可利用座位,但可以候补
                                //C      该等级彻底关闭,不允许候补或申请
                                //X      该等级取消, 不允许候补或申请
                                //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                    || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                    || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                {
                                    continue;
                                }

                                //大于9个座位显示9，其他不变
                                //int.TryParse(tmpstr4, out classnum);
                                //if (classnum != 0)
                                //{
                                classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                // }
                            }
                        }
                    }

                    //避免加入重复航班信息
                    if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                    {
                        if (destcontent.Trim() == "")
                        {
                            destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                        }
                        else
                        {
                            destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                        }
                    }
                }
                #endregion 添加末尾数据信息
            }
            catch (Exception e)
            {
                string tmpstr = e.Message;
                return;
            }
        }
        #endregion 把AVH结果格式化为网站返回格式

        #region 把信天游AVH结果格式化为网站返回格式
        //把信天游AVH内容格式化为网站固定格式
        private void AnalyseXTYAVHContentToWebInfo(string sourcmd, string sourcontent, out string destcontent)
        {
            destcontent = "";

            string tmpdate = "";
            try
            {
                tmpdate = getStandardDate(sourcmd.Substring(11, 5));

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //返回信息与查询日期不一致
                if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
                {
                    destcontent = "当日无航班或者已经销售完毕！";
                    return;
                }

                string tmpdataDate = getStandardDate(sourcontent.Substring(1, pos1 - 1));

                #region AVH结果信息（例子）
                // 30AUG(THU) CTUBJS DIRECT ONLY                                                  
                //1-  CA4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>                                                                   T2 T3  2:35 
                //2  *ZH4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>   CA4193                                                          T2 T3  2:35 
                //3   3U8881  DS# FA PA AA YA BA TA WS HA MA GA  CTUPEK 0730   1005   330 0 S  E  
                //>               SA LA QA EA VS US RS KQ NQ XS ZS                    T1 T3  2:35 
                //4   CA4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>                                                                   T2 T3  2:20 
                //5  *ZH4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>   CA4113                                                          T2 T3  2:20 
                //6   KN2611  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>               NQ RQ SQ VQ TQ WQ GQ XQ QA US I3                    T2 --  1:50 
                //7  *MU8071  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>   KN2611      NQ RQ SQ VQ TQ WQ GQ XQ QA I3                       T2 --  1:50 
                //8   CA4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>                                                                   T2 T3  2:25 
                //9+ *ZH4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>   CA4101                                                          T2 T3  2:25 
                //**  CZ  FARE    CTUPEK/PEKCTU   YI:CZ/TZ305                                   
                #endregion AVH结果信息（例子）

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[1] { '\r' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 
                string flongtime = "";//飞行时长

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理
                        //判断是否上一航段还有舱位信息没有添加完毕
                        if (tmpclassinfo != "")
                        {
                            //分析并添加舱位信息
                            string[] sl2 = tmpclassinfo.Split(' ');
                            for (int j = 0; j < sl2.Length; j++)
                            {
                                if (sl2[j].Trim().Length != 0)
                                {
                                    //判断舱位状态
                                    if (sl2[j].Length == 2)
                                    {
                                        //int classnum = 0;
                                        string tmpstr4 = sl2[j].Substring(1, 1);

                                        if (tmpstr4.ToLower() == "a")
                                        {
                                            tmpstr4 = "9";
                                        }

                                        //跳过关闭的无效舱位
                                        //L      没有可利用座位,但旅客可以候补
                                        //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                        //S      因达到限制销售数而没有可利用座位,但可以候补
                                        //C      该等级彻底关闭,不允许候补或申请
                                        //X      该等级取消, 不允许候补或申请
                                        //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                        if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                            || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                            || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                        {
                                            continue;
                                        }

                                        //大于9个座位显示9，其他不变
                                        //int.TryParse(tmpstr4, out classnum);
                                        //if (classnum != 0)
                                        //{
                                        classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                        // }
                                    }
                                }
                            }

                            //避免加入重复航班信息
                            if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                            {
                                if (destcontent.Trim() == "")
                                {
                                    destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                                }
                                else
                                {
                                    destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                                }
                            }

                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                        }
                        #endregion 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理

                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();
                        //此处不做空格处理
                        tmpstr = tmpstr.Substring(16);

                        #region 跳过被屏蔽的航空公司信息
                        //判断是否被屏蔽的航空公司
                        if (tmpstr.Substring(0, 31).Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司信息

                        //舱位信息
                        tmpclassinfo = tmpstr.Substring(0, 31).Trim();
                        tmpstr = tmpstr.Substring(31).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatflag = "1";
                        }
                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司信息
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 1).Trim() != ">")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            //添加舱位信息
                            tmpclassinfo += " "+sl[i].Substring(11, 56).Trim();
                            tmpstr = tmpstr.Substring(67).Trim();

                            //航站楼信息
                            if (tmpstr.Substring(0, 2).Trim() == "")
                            {
                                flightterminal += "--";
                            }
                            else
                            {
                                flightterminal += tmpstr.Substring(0, 2);
                            }

                            if (tmpstr.Substring(3, 2).Trim() == "")
                            {
                                flightterminal += "--";
                            }
                            else
                            {
                                flightterminal += tmpstr.Substring(3, 2);
                            }

                            tmpstr = tmpstr.Substring(5).Trim();

                            //飞行时长
                            flongtime = tmpstr;
                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                #region 添加末尾数据信息
                if (tmpclassinfo != "")
                {
                    //分析并添加舱位信息
                    string[] sl2 = tmpclassinfo.Split(' ');
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j].Trim().Length != 0)
                        {
                            //判断舱位状态
                            if (sl2[j].Length == 2)
                            {
                                //int classnum = 0;
                                string tmpstr4 = sl2[j].Substring(1, 1);

                                if (tmpstr4.ToLower() == "a")
                                {
                                    tmpstr4 = "9";
                                }

                                //跳过关闭的无效舱位
                                //L      没有可利用座位,但旅客可以候补
                                //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                //S      因达到限制销售数而没有可利用座位,但可以候补
                                //C      该等级彻底关闭,不允许候补或申请
                                //X      该等级取消, 不允许候补或申请
                                //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                    || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                    || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                {
                                    continue;
                                }

                                //大于9个座位显示9，其他不变
                                //int.TryParse(tmpstr4, out classnum);
                                //if (classnum != 0)
                                //{
                                classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                // }
                            }
                        }
                    }

                    //避免加入重复航班信息
                    if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                    {
                        if (destcontent.Trim() == "")
                        {
                            destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                        }
                        else
                        {
                            destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                        }
                    }
                }
                #endregion 添加末尾数据信息
            }
            catch (Exception e)
            {
                string tmpstr = e.Message;
                return;
            }
        }
        #endregion 把信天游AVH结果格式化为网站返回格式

        #region 把AV结果格式化为网站返回格式
        //把AV结果格式化为网站返回格式
        private void AnalyseAVContentToWebInfo(string sourcmd, string sourcontent, out string destcontent)
        {
            destcontent = "";

            if (sourcontent.Length < 80)
                return;

            //判断结果是否为AV的结果，如果不是则不做处理
            if ((sourcontent.IndexOf(")") == -1) || (sourcontent.IndexOf("(") == -1))
            {
                destcontent = "查询结果异常！";
                return;
            }

            #region 判断日期是否一致
            //判断日期是否一致
            //av:ctupek/30aug/ca/d
            string tmpcmddate = sourcmd.Trim().Substring(10, 5).ToUpper();
            if (sourcontent.ToUpper().Substring(0, sourcontent.IndexOf(")")).IndexOf(tmpcmddate) == -1)
            {
                destcontent = "当日无航班或者已经销售完毕！";
                return;
            }
            #endregion 判断日期是否一致

            #region 返回结果格式（例子）
            //返回结果格式
            //日期，起飞时间，到达时间，承运人，航班号，舱位及数量，出发城市，到达城市，机型，经停，餐食，电子客票，共享航班，子舱位，出发乘机楼到达乘机楼^
            //2012-08-30,07:00,09:35,CA,4193,F9ASO9Y9B9MSH9K9L9Q9G9S9XSNSVSTSE9*9,CTU,PEK,321,0,1,1,False,M1S,T2T3^
            //2012-08-30,07:00,09:35,ZH,4193,F9Y9B9MSH9K9L9Q9G9S9VSTSE9*9,CTU,PEK,321,0,1,1,True,,T2T3^
            //2012-08-30,07:30,10:05,3U,8881,F9P9A9Y9B9T9WSH9M9G9S9L9Q9E9VSUSDSRSKQNQXSZSO9*9,CTU,PEK,330,0,0,1,False,F2A U1S,T1T3^
            //2012-08-30,08:00,10:20,CA,4113,F9ASOSY9B9MSH9K9L9Q9G9S9XSNSVSTSE9*9,CTU,PEK,321,0,1,1,False,M1S,T2T3^
            //2012-08-30,08:00,10:20,ZH,4113,F9Y9B9MSH9K9L9Q9G9S9VSTSE9*9,CTU,PEK,321,0,1,1,True,,T2T3^
            //2012-08-30,08:05,09:55,KN,2611,F8P5ASYAKSBAEAHALAMANQRQSQVQTQWQGQXQQAUSI3,CTU,NAY,738,0,0,1,False,,T2--^
            //2012-08-30,08:05,09:55,MU,8071,F8P5ASYAKSBAEAHALAMANQRQSQVQTQWQGQXQQAI3,CTU,NAY,738,0,0,1,True,,T2--^
            //2012-08-30,09:00,11:25,CA,4101,FAASYABAMSHAKALAQAGSSSNSVSTSEA,CTU,PEK,321,0,0,1,False,M1S,T2T3^
            //2012-08-30,09:00,11:25,ZH,4101,FAYABAMSHAKALAQAGSSSVSTSEA,CTU,PEK,321,0,0,1,True,M1S,T2T3^
            //2012-08-30,09:30,11:40,KN,2927,F8PSASYAKSBAEAHALAMANARASSVATSWSGSXAQAUSI5,CTU,NAY,738,0,0,1,False,,T2--^
            //2012-08-30,09:30,11:40,MU,8027,F8PSASYAKSBAEAHALAMANARASSVATSWSGSXAQAI5,CTU,NAY,738,0,0,1,True,,T2--^
            //2012-08-30,09:30,12:05,3U,8883,F8P8A8YABATAWSHAMAGASALAQAE7VSUSRSKQNQXSZS,CTU,PEK,321,0,1,1,False,,T1T3^
            //2012-08-30,10:00,12:30,ZH,4107,FAYABAMSHAKALAQAGSSSVSTSES,CTU,PEK,321,0,1,1,True,M1S,T2T3^
            //2012-08-30,10:00,12:30,SK,9516,C4D4J4Z4Y4S4BLE4MLH4Q4V4W4KL,CTU,PEK,321,0,1,1,True,,T2T3^
            //2012-08-30,10:00,12:30,CA,4107,FAASYABAMSHAKALAQAGSSSNSVSTSES,CTU,PEK,321,0,1,1,False,M1S,T2T3^
            //2012-08-30,11:00,13:40,CA,4115,F8ASYABAMSHAKALAQAGSSSNSVSTSES,CTU,PEK,757,0,1,1,False,M1S,T2T3^
            //2012-08-30,11:00,13:40,ZH,4115,F8YABAMSHAKALAQAGSSSVSTSES,CTU,PEK,757,0,1,1,True,M1S,T2T3^
            //2012-08-30,11:30,14:10,3U,8885,F7P7A7YABATAWSHAMAGASALAQAE8VSUSRSKQNQXSZS,CTU,PEK,321,0,1,1,False,,T1T3^
            //2012-08-30,11:50,14:25,HU,7148,FAPQA2YABAHAKALAMAQAXAUQEQTQZQVQGSOQSQ,CTU,PEK,334,0,1,1,False,,--T1^
            //2012-08-30,12:00,14:30,ZH,1406,FACAYABAMSHAKALAQAGSSSVSTSEA,CTU,PEK,747,0,1,1,True,M1S,T2T3^
            //2012-08-30,12:00,14:30,CA,1406,FAASCAD3Z4JSR2YABAMSHAKALAQAGSSSNSVSTSEA,CTU,PEK,747,0,1,1,False,M1S,T2T3^
            //2012-08-30,12:20,14:45,CZ,6162,FAP5WAZAYATAKAHAMAG6SALAQQUAEQVQBQXQNQRQ,CTU,PEK,330,0,1,1,False,,T2T2^
            //2012-08-30,12:20,14:45,MU,3011,YABAEAHALAM6NARASQKAVQTQWQ,CTU,PEK,330,0,1,1,True,,T2T2
            #endregion 返回结果格式（例子）

            #region AV指令返回信息（例子）
            // 30AUG(THU) CTUPEK      DIRECT ONLY                                             
            //1- *ZH4193  CTUPEK 0700   0935   321 0^B         EFA YA BA MS HA<T2T3>          
            //    CA4193         KA LA QA GA SA VS TS EA                                      
            //2   CA4193  CTUPEK 0700   0935   321 0^B         EFA AS OA YA BA<T2T3>          
            //                   MS HA KA LA QA GA SA XS NS VS TS EA                          
            //               ** M1S                                                           
            //3   3U8881  CTUPEK 0730   1005   330 0 S         EFA PA AA YA BA<T1T3>          
            //                   TA WS HA MA GA SA LA QA EA VS US DS RS KQ NQ XS ZS OA        
            //               ** F2A U1S                                                       
            //4  *ZH4113  CTUPEK 0800   1020   321 0^B         EFA YA BA MS HA<T2T3>          
            //    CA4113         KA LA QA GA SA VS TS EA                                      
            //5+  CA4113  CTUPEK 0800   1020   321 0^B         EFA AS OS YA BA<T2T3>          
            //                   MS HA KA LA QA GA SA XS NS VS TS EA                          
            //               ** M1S                                                           


            // 30AUG(THU) CTUSHA VIA 3U DIRECT ONLY                                           
            //1-  3U8961  CTUPVG 0800   1035   321 0 S         EFA PA AA YA BA<T1-->          
            //                   TA WA HA MS GA SS LA QS EA VS US DA RS KS NS XA ZS OA        
            //               ** F2A U1S                                                       
            //2   3U8963  CTUPVG 1145   1435   321 0 S         EFA PA AA YA BS<T1-->          
            //                   TA WS HA MS GA SS LA QS EA VS US DS RS KS NS XA ZS OS        
            //               ** F2A U1S                                                       
            //3   3U8965  CTUPVG 1600   1835   321 0 S         EFA PA AA YA BA<T1-->          
            //                   TA WA HA MS GA SS LA QS ES VS US DA RS KS NS XA ZS OA        
            //               ** F2A U1S                                                       
            //4+  3U8647  CTUPVG 1835   2105   330 0 S         EFA PA AA CC IC<T1-->          
            //                JC YA BA TA WA HA MS GA SS LA QS EA VS US DS RS KS NS XA ZS OA  
            //               ** F2A U1S                                                       
            //**  All scheduled MU or FM flights operated by MU or FM are "Eastern Express"   
            #endregion AV指令返回信息（例子）

            string tmpdate = "";
            try
            {
                //根据当前时间把查询日期转换为标准日期
                tmpdate = getStandardDate(sourcmd.Substring(10, 5));

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //返回信息与查询日期不一致
                if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpcmddate.ToUpper()) == -1)
                {
                    destcontent = "当日无航班或者已经销售完毕！";
                    return;
                }

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[1] { '\r' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理
                        //判断是否上一航段还有舱位信息没有添加完毕
                        if (tmpclassinfo != "")
                        {
                            //分析并添加舱位信息
                            string[] sl2 = tmpclassinfo.Split(' ');
                            for (int j = 0; j < sl2.Length; j++)
                            {
                                if (sl2[j].Trim().Length != 0)
                                {
                                    //判断舱位状态
                                    if (sl2[j].Length == 2)
                                    {
                                        //int classnum = 0;
                                        string tmpstr4 = sl2[j].Substring(1, 1);

                                        if (tmpstr4.ToLower() == "a")
                                        {
                                            tmpstr4 = "9";
                                        }

                                        //跳过关闭的无效舱位
                                        //L      没有可利用座位,但旅客可以候补
                                        //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                        //S      因达到限制销售数而没有可利用座位,但可以候补
                                        //C      该等级彻底关闭,不允许候补或申请
                                        //X      该等级取消, 不允许候补或申请
                                        //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                        if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                            || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                            || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                        {
                                            continue;
                                        }

                                        //大于9个座位显示9，其他不变
                                        //int.TryParse(tmpstr4, out classnum);
                                        //if (classnum != 0)
                                        //{
                                        classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                        // }
                                    }
                                }
                            }

                            //避免加入重复航班信息
                            if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                            {
                                if (destcontent.Trim() == "")
                                {
                                    destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                                }
                                else
                                {
                                    destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                                }
                            }

                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                        }
                        #endregion 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理

                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();
                        tmpstr = tmpstr.Substring(6).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatflag = "1";
                        }
                        tmpstr = tmpstr.Substring(2).Trim();

                        if (tmpstr.Substring(1, 1) == " ")
                        {
                            tmpstr = tmpstr.Substring(1).Trim();
                        }

                        //航班舱位信息
                        if (tmpstr.IndexOf("<") != -1)
                        {
                            tmpclassinfo = tmpstr.Substring(1, tmpstr.IndexOf("<") - 1).Trim();

                            tmpstr = tmpstr.Substring(15).Trim();

                            //航站楼信息
                            flightterminal = tmpstr.Substring(1, 4);
                        }
                        else
                        {
                            tmpclassinfo = tmpstr.Substring(1).Trim();

                            //航站楼信息
                            flightterminal = "----";
                        }

                        #region 跳过被屏蔽的航空公司
                        //跳过被屏蔽的航空公司
                        if (tmpclassinfo.Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司

                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 3).Trim() != "")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            tmpclassinfo += " "+sl[i].Substring(12).Trim();
                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                #region 添加末尾数据信息
                if (tmpclassinfo != "")
                {
                    //分析并添加舱位信息
                    string[] sl2 = tmpclassinfo.Split(' ');
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j].Trim().Length != 0)
                        {
                            //判断舱位状态
                            if (sl2[j].Length == 2)
                            {
                                //int classnum = 0;
                                string tmpstr4 = sl2[j].Substring(1, 1);

                                if (tmpstr4.ToLower() == "a")
                                {
                                    tmpstr4 = "9";
                                }

                                //跳过关闭的无效舱位
                                //L      没有可利用座位,但旅客可以候补
                                //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                //S      因达到限制销售数而没有可利用座位,但可以候补
                                //C      该等级彻底关闭,不允许候补或申请
                                //X      该等级取消, 不允许候补或申请
                                //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                    || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                    || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                {
                                    continue;
                                }

                                //大于9个座位显示9，其他不变
                                //int.TryParse(tmpstr4, out classnum);
                                //if (classnum != 0)
                                //{
                                classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                // }
                            }
                        }
                    }

                    //避免加入重复航班信息
                    if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                    {
                        if (destcontent.Trim() == "")
                        {
                            destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                        }
                        else
                        {
                            destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                        }
                    }
                }
                #endregion 添加末尾数据信息
            }
            catch (Exception e)
            {
                Log.Record("client.log", "把AV结果格式化成网站数据格式出错，错误信息：" + e.Message);
                return;
            }
        }
        #endregion 把AV结果格式化为网站返回格式

        #region 根据标准日期格式(2009-01-20)返回20JAN09
        /// <summary>
        /// 根据标准日期格式(2009-01-20)返回20JAN09
        /// </summary>
        /// <param name="Date">返回日期格式（20JAN09）</param>
        private string getEtermDate(string date)
        {
            string strResult = date.Substring(8, 2);
            string strYear = "";

            //判断是否跨年
            if (DateTime.Now.Year < int.Parse(date.Substring(0, 4)))
            {
                strYear = date.Substring(2, 2);
            }

            string month = date.Substring(5, 2);
            if (month == "01")
            {
                strResult += "JAN";
            }
            else if (month == "02")
            {
                strResult += "FEB";
            }
            else if (month == "03")
            {
                strResult += "MAR";
            }
            else if (month == "04")
            {
                strResult += "APR";
            }
            else if (month == "05")
            {
                strResult += "MAY";
            }
            else if (month == "06")
            {
                strResult += "JUN";
            }
            else if (month == "07")
            {
                strResult += "JUL";
            }
            else if (month == "08")
            {
                strResult += "AUG";
            }
            else if (month == "09")
            {
                strResult += "SEP";
            }
            else if (month == "10")
            {
                strResult += "OCT";
            }
            else if (month == "11")
            {
                strResult += "NOV";
            }
            else if (month == "12")
            {
                strResult += "DEC";
            }

            if (strYear == "")
            {
                return " " + strResult;
            }
            else
            {
                return strResult + strYear;
            }
        }

        #endregion 根据标准日期格式(2009-01-20)返回20JAN09

        #region 根据28JUL07返回标准日期格式2007-07-28
        /// <summary>
        /// 根据28JUL07返回标准日期格式2007-07-28
        /// </summary>
        /// <param name="Date">返回日期格式（2007-03-06）</param>
        private string getStandardDate(string Date)
        {
            if (Date.Length < 5)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            string tmpDate = Date.Substring(0, 2);

            string Month = Date.Substring(2, 3).ToUpper();

            string tmpMonth = "";

            switch (Month)
            {
                case "JAN":
                    tmpMonth = "01";
                    break;
                case "FEB":
                    tmpMonth = "02";
                    break;
                case "MAR":
                    tmpMonth = "03";
                    break;
                case "APR":
                    tmpMonth = "04";
                    break;
                case "MAY":
                    tmpMonth = "05";
                    break;
                case "JUN":
                    tmpMonth = "06";
                    break;
                case "JUL":
                    tmpMonth = "07";
                    break;
                case "AUG":
                    tmpMonth = "08";
                    break;
                case "SEP":
                    tmpMonth = "09";
                    break;
                case "OCT":
                    tmpMonth = "10";
                    break;
                case "NOV":
                    tmpMonth = "11";
                    break;
                case "DEC":
                    tmpMonth = "12";
                    break;
            }
            if (Date.Length >= 7)
            {
                if (Date.Substring(5, 2).Trim().Length == 2)
                {
                    return "20" + Date.Substring(5, 2) + "-" + tmpMonth + "-" + tmpDate;
                }
                else
                {
                    return DateTime.Now.ToString("yyyy") + "-" + tmpMonth + "-" + tmpDate;
                }
            }
            else
            {
                //判断日期是否小于当前日期，如果小于则把年份更改为明年
                string resDate = DateTime.Now.ToString("yyyy") + "-" + tmpMonth + "-" + tmpDate + " 23:59:59";
                if (DateTime.Compare(DateTime.Parse(resDate), DateTime.Now) < 0)
                {
                    //跨年
                    return DateTime.Parse(resDate).AddYears(1).ToString("yyyy-MM-dd");
                }
                else
                {
                    return resDate.Substring(0, 10);
                }
            }
        }
        #endregion 根据28JUL07返回标准日期格式2007-07-28

        #region 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// <summary>
        /// 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// </summary>
        /// <param name="command">原指令</param>
        /// <param name="resbuf">结果数据包</param>
        /// <param name="needpinyin">是否需要拼音</param>
        private void AnalyseWebCmdAndMakeServerInfo(string command, out byte[] resbuf, bool needpinyin)
        {
            resbuf = null;

            //先把字节包转换为字符串
            string tmpcontent = command;

            //源字符串长度
            int count = tmpcontent.Length;

            //序号
            int index = 0;

            //汉字的起始、结束字节标志
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };

            int code = 0;
            byte tmpbyte;

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);

            bool isChinese = false;//是否汉字

            //临时存储连续的汉字
            string tmpstr = "";
            string tmppinyin = "";//拼音

            ArrayList al = new ArrayList();

            PublicInfo thepub = new PublicInfo();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    //获得字符串中指定索引i处字符unicode编码
                    code = Char.ConvertToUtf32(tmpcontent, i);
                    if (code >= chfrom && code <= chend)
                    {
                        tmpstr += tmpcontent[i];
                        isChinese = true;     //当code在中文范围内
                    }
                    else
                    {
                        if (isChinese)
                        {
                            isChinese = false;
                            if (tmpstr.Length != 0)
                            {
                                //取得汉字的拼音

                                //暂时存储汉字编码
                                ArrayList al2 = new ArrayList();

                                for (int j = 0; j < tmpstr.Length; j++)
                                {
                                    byte[] hanziBt;
                                    tmppinyin = GetPinYinBianMaFromHanZi(tmpstr.Substring(j, 1), out hanziBt);//chs2py.convert(tmpstr.Substring(j, 1), out hanziBt);

                                    if (needpinyin)
                                    {
                                        al.AddRange(Encoding.Default.GetBytes(tmppinyin));
                                    }

                                    //如果没有找到汉字编码，则使用转换方法
                                    if (hanziBt == null)
                                    {
                                        //转换临时存储的汉字并添加到ArrayList
                                        byte[] tmpbuf = Encoding.Default.GetBytes(tmpstr.Substring(j, 1));
                                        index += 2;// tmpbuf.Length;
                                        for (int k = 0; k < tmpbuf.Length; k++)
                                        {
                                            tmpbyte = (byte)(tmpbuf[k] - 0x80);
                                            al2.Add(tmpbyte);
                                        }
                                    }
                                    else
                                    {
                                        al2.AddRange(hanziBt);
                                        index += 2;// hanziBt.Length;
                                    }
                                }

                                al.AddRange(beginbuf);

                                al.AddRange(al2);

                                al.AddRange(endbuf);
                                tmpstr = "";
                            }
                        }
                        //当code不在中文范围内
                        tmpbyte = Convert.ToByte(tmpcontent[i]);
                        index++;
                        al.Add(tmpbyte);
                    }
                }

                if ((isChinese) && (tmpstr.Length != 0))
                {
                    //暂时存储汉字编码
                    ArrayList al2 = new ArrayList();

                    for (int j = 0; j < tmpstr.Length; j++)
                    {
                        byte[] hanziBt;
                        tmppinyin = GetPinYinBianMaFromHanZi(tmpstr.Substring(j, 1), out hanziBt);//chs2py.convert(tmpstr.Substring(j, 1), out hanziBt);

                        if (needpinyin)
                        {
                            al.AddRange(Encoding.Default.GetBytes(tmppinyin));
                        }

                        //如果没有找到汉字编码，则使用转换方法
                        if (hanziBt == null)
                        {
                            //转换临时存储的汉字并添加到ArrayList
                            byte[] tmpbuf = Encoding.Default.GetBytes(tmpstr.Substring(j, 1));
                            index += tmpbuf.Length;
                            for (int k = 0; k < tmpbuf.Length; k++)
                            {
                                tmpbyte = (byte)(tmpbuf[k] - 0x80);
                                al2.Add(tmpbyte);
                            }
                        }
                        else
                        {
                            al2.AddRange(hanziBt);
                            index += hanziBt.Length;
                        }
                    }

                    al.AddRange(beginbuf);

                    al.AddRange(al2);

                    al.AddRange(endbuf);
                }

                resbuf = (byte[])al.ToArray(typeof(byte));
            }
            catch (Exception ex)
            {
                Log.Record("client.log", ex);
            }
        }
        #endregion 把网络的命令包转换为航信命令数据包，主要是处理中文

        #region 检查用户帐号、密码的正确性
        private int CheckUser(Base_UserConfigManage UserConfigManage, string pwd)
        {
            if (UserConfigManage.User_Pass == pwd)
            {
                //判断帐号状态
                if (UserConfigManage.User_Disable)
                {
                    if ((UserConfigManage.User_BeginDate > DateTime.Now) || (UserConfigManage.User_EndDate < DateTime.Now))
                    {
                        //帐号已过期
                        return -2;
                    }
                    else
                    {
                        //验证通过
                        return 0;
                    }
                }
                else
                {
                    //帐号被禁用
                    return -3;
                }
            }
            else
            {
                //密码不正确
                return -1;
            }
        }
        #endregion 检查用户帐号、密码的正确性

        #region 检查用户是否已经登录,如果已登录则踢掉之前的连接
        /// <summary>
        /// 帐号是否已经登录
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <returns></returns>
        private bool CheckIfLogined(string userName)
        {
            //判断登录帐号列表是否存在当前请求帐号
            if ((m_ClientInfoList != null) && (m_ClientInfoList.Contains(userName)))
            {
                ClientInfo clientmgr = (ClientInfo)m_ClientInfoList[userName];

                //判断查找到的登录信息是否有效
                //if ((clientmgr != null) && (clientmgr.UserConfigManage != null)
                //    && (clientmgr.UserConfigManage.User_Name == userName) && (clientmgr.ClientSocket != null))
                //{
                //    return true;
                //}
                //else

                if (clientmgr != null)
                {
                    //释放无效的登录信息
                    lock (m_ClientInfoList)
                    {
                        try
                        {
                            if (clientmgr != null)
                            {
                                clientmgr.Close();
                                clientmgr = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (m_DebugFlag)
                            {
                                Log.Record("client.log", "存在已无效的登录帐号信息，释放资源出错：" + ex.Message);
                            }
                        }

                        m_ClientInfoList.Remove(userName);
                    }
                }
            }

            return false;
        }
        #endregion 检查用户是否已经登录

        #region 检查中文编码和拼音是否完整
        /// <summary>
        /// 检查中文拼音是否支持，防止出现缺少汉字的情况
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private bool CheckChinesePinYin(string content)
        {
            //先把字节包转换为字符串
            string tmpcontent = content;

            //源字符串长度
            int count = tmpcontent.Length;

            //序号
            int index = 0;

            //汉字的起始、结束字节标志
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };

            int code = 0;
            byte tmpbyte;

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);

            bool isChinese = false;//是否汉字

            //临时存储连续的汉字
            string tmpstr = "";
            string tmppinyin = "";//拼音

            ArrayList al = new ArrayList();

            PublicInfo thepub = new PublicInfo();

            bool flag = true;

            for (int i = 0; i < count; i++)
            {
                //获得字符串中指定索引i处字符unicode编码
                code = Char.ConvertToUtf32(tmpcontent, i);
                if (code >= chfrom && code <= chend)
                {
                    //检查中文拼音是否支持
                    byte[] hanziBt;
                    tmppinyin = chs2py.convert(tmpcontent.Substring(i, 1), out hanziBt);
                    if (hanziBt == null)
                    {
                        try
                        {
                            //添加到数据库
                            Base_PinYin _Base_PinYin = new Base_PinYin();
                            _Base_PinYin.HanZi = tmpcontent.Substring(i, 1);
                            _Base_PinYin.PinYin = "";
                            _Base_PinYin.BianMa = "";
                            _Base_PinYin.Remarks = "";
                            ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                        }
                        catch (Exception ex)
                        {
                            Log.Record("error.log", ex);
                        }
                        Log.Record("chinese.log", "不支持汉字：" + tmpcontent.Substring(i, 1) + " ||| " + tmpcontent);
                        //return false;
                        flag = false;
                    }
                    #region 屏蔽
                    //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
                    //{
                    //    if (x.HanZi == tmpcontent.Substring(i, 1))
                    //    {
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //});

                    //if ((_Base_PinYin == null) || (_Base_PinYin.BianMa.Trim() == "") || (_Base_PinYin.PinYin.Trim() == ""))
                    //{
                    //    if (_Base_PinYin == null)
                    //    {
                    //        try
                    //        {
                    //            //添加到数据库
                    //            _Base_PinYin = new Base_PinYin();
                    //            _Base_PinYin.HanZi = tmpcontent.Substring(i, 1);
                    //            _Base_PinYin.PinYin = "";
                    //            _Base_PinYin.BianMa = "";
                    //            _Base_PinYin.Remarks = "";
                    //            ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Log.Record("error.log", ex);
                    //        }
                    //    }

                    //    flag = false;
                    //}
                    #endregion
                }
            }

            if (!flag)
            {
                Log.Record("chinese.log", "不支持汉字：" + tmpcontent);
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion 检查中文编码和拼音是否完整

        #region 根据汉字编码取得汉字信息
        /// <summary>
        /// 根据航信编码返回汉字信息
        /// </summary>
        /// <param name="hzBytes"></param>
        /// <returns></returns>
        public string GetHanZiByHangXinBianMa(byte[] hzBytes)
        {
            string ResHanZi = "";
            string tmpBianMa = "";
            tmpBianMa = chs2py.GetStringFromByte(hzBytes[0]);
            tmpBianMa += " " + chs2py.GetStringFromByte(hzBytes[1]);

            //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
            //{
            //    if (x.BianMa.ToUpper() == tmpBianMa.ToUpper())
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});

            ////如果系统拼音中不存在此编码，则添加到系统汉字编码数据库中
            //if (_Base_PinYin == null)
            //{
            //    try
            //    {
            //        //添加到数据库
            //        _Base_PinYin = new Base_PinYin();
            //        _Base_PinYin.HanZi = "";
            //        _Base_PinYin.PinYin = "";
            //        _Base_PinYin.BianMa = tmpBianMa.ToLower();
            //        _Base_PinYin.Remarks = "网站用户";
            //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Record("error.log", "根据汉字编码获取汉字信息出错，错误信息：" + ex.Message);
            //    }

            ResHanZi = chs2py.GetHanZi(hzBytes);
            if (ResHanZi == "")
            {
                try
                {
                    //添加到数据库
                    Base_PinYin _Base_PinYin = new Base_PinYin();
                    _Base_PinYin.HanZi = "";
                    _Base_PinYin.PinYin = "";
                    _Base_PinYin.BianMa = tmpBianMa.ToLower();
                    _Base_PinYin.Remarks = "网站用户";
                    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                }
                catch (Exception ex)
                {
                    Log.Record("error.log", "根据汉字编码获取汉字信息出错，错误信息：" + ex.Message);
                }
            }
            //}
            //else
            //{
            //    ResHanZi = _Base_PinYin.HanZi;
            //    if (ResHanZi.Trim() == "")
            //    {
            //        ResHanZi = chs2py.GetHanZi(hzBytes);
            //    }
            //}

            return ResHanZi;
        }
        #endregion

        #region 根据汉字取得汉字信息
        /// <summary>
        /// 根据汉字取得汉字编码和拼音
        /// </summary>
        /// <param name="HanZi"></param>
        /// <param name="HanZiBt"></param>
        /// <returns></returns>
        public string GetPinYinBianMaFromHanZi(string HanZi, out byte[] HanZiBt)
        {
            HanZiBt = null;

            string StrPinYin = "";

            //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
            //{
            //    if (x.HanZi == HanZi)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});

            ////如果系统拼音中不存在此汉字，则添加到系统汉字编码数据库中
            //if (_Base_PinYin == null)
            //{
            //    try
            //    {
            //        //添加到数据库
            //        _Base_PinYin = new Base_PinYin();
            //        _Base_PinYin.HanZi = HanZi;
            //        _Base_PinYin.PinYin = "";
            //        _Base_PinYin.BianMa = "";
            //        _Base_PinYin.Remarks = "网站用户" ;
            //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Record("error.log", ex);
            //    }
            StrPinYin = chs2py.convert(HanZi, out HanZiBt);
            if (StrPinYin == "")
            {
                try
                {
                    //添加到数据库
                    Base_PinYin _Base_PinYin = new Base_PinYin();
                    _Base_PinYin.HanZi = HanZi;
                    _Base_PinYin.PinYin = "";
                    _Base_PinYin.BianMa = "";
                    _Base_PinYin.Remarks = "网站用户";
                    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                }
                catch (Exception ex)
                {
                    Log.Record("error.log", ex);
                }
            }
            return StrPinYin;
            //}

            //try
            //{
            //    StrPinYin = _Base_PinYin.PinYin;
            //    HanZiBt = new byte[2];
            //    HanZiBt[0] = chs2py.GetByteFromString(_Base_PinYin.BianMa.Trim().Substring(0, _Base_PinYin.BianMa.Trim().IndexOf(" ")));
            //    HanZiBt[1] = chs2py.GetByteFromString(_Base_PinYin.BianMa.Trim().Substring(_Base_PinYin.BianMa.Trim().IndexOf(" ")));

            //    return StrPinYin;
            //}
            //catch (Exception ex)
            //{
            //    Log.Record("error.log", "根据汉字获取拼音和编码信息出错，错误信息：" + ex.Message);
            //    return "";
            //}
        }
        #endregion
    }

    /// <summary>
    /// 网站用户处理类
    /// </summary>
    public class WebInfo
    {
        /// <summary>
        /// 接收到的网站数据包
        /// </summary>
        public byte[] packetBuff = null;

        /// <summary>
        /// 网站访问用户名
        /// </summary>
        private string m_WebUser = "webguest";

        /// <summary>
        /// 网站访问帐号密码
        /// </summary>
        private string m_WebPWD = "webguest";

        /// <summary>
        /// 调试输出标志
        /// </summary>
        public bool m_DebugFlag = false;

        /// <summary>
        /// Socket连接
        /// </summary>
        public Socket newclient = null;

        /// <summary>
        /// TCP连接数据流
        /// </summary>
        public NetworkStream ns = null;

        /// <summary>
        /// 网站回复数据包头
        /// </summary>
        private readonly byte[] m_WebHeadPack = { 0xEE, 0xEF };

        /// <summary>
        /// 网站回复数据包尾
        /// </summary>
        private readonly byte[] m_WebEndPack = { 0xFF, 0xEF };

        /// <summary>
        /// 输出显示列表
        /// </summary>
        public ArrayList m_ArrayContents = null;

        /// <summary>
        /// 接收到指令的时间
        /// </summary>
        public DateTime ReceiveTime = DateTime.Now;

        /// <summary>
        /// 配置类列表
        /// </summary>
        public PBPid.ServerManage.ServerManage m_ServerManage = null;

        /// <summary>
        /// 最大指令超时时间（秒）
        /// </summary>
        private const int m_MaxOutTimes = 6;

        #region 检查中文编码和拼音是否完整
        /// <summary>
        /// 检查中文拼音是否支持，防止出现缺少汉字的情况
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private bool CheckChinesePinYin(string content)
        {
            //先把字节包转换为字符串
            string tmpcontent = content;

            //源字符串长度
            int count = tmpcontent.Length;

            //序号
            int index = 0;

            //汉字的起始、结束字节标志
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };

            int code = 0;
            byte tmpbyte;

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);

            bool isChinese = false;//是否汉字

            //临时存储连续的汉字
            string tmpstr = "";
            string tmppinyin = "";//拼音

            ArrayList al = new ArrayList();

            PublicInfo thepub = new PublicInfo();

            bool flag = true;

            for (int i = 0; i < count; i++)
            {
                //获得字符串中指定索引i处字符unicode编码
                code = Char.ConvertToUtf32(tmpcontent, i);
                if (code >= chfrom && code <= chend)
                {
                    //检查中文拼音是否支持
                    byte[] hanziBt;
                    tmppinyin = chs2py.convert(tmpcontent.Substring(i, 1), out hanziBt);
                    if (hanziBt == null)
                    {
                        try
                        {
                            //添加到数据库
                            Base_PinYin _Base_PinYin = new Base_PinYin();
                            _Base_PinYin.HanZi = tmpcontent.Substring(i, 1);
                            _Base_PinYin.PinYin = "";
                            _Base_PinYin.BianMa = "";
                            _Base_PinYin.Remarks = "";
                            ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                        }
                        catch (Exception ex)
                        {
                            Log.Record("error.log", ex);
                        }
                        Log.Record("chinese.log", "不支持汉字：" + tmpcontent.Substring(i, 1) + " ||| " + tmpcontent);
                        //return false;
                        flag = false;
                    }
                    #region 屏蔽
                    //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
                    //{
                    //    if (x.HanZi == tmpcontent.Substring(i, 1))
                    //    {
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //});

                    //if ((_Base_PinYin == null) || (_Base_PinYin.BianMa.Trim() == "") || (_Base_PinYin.PinYin.Trim() == ""))
                    //{
                    //    if (_Base_PinYin == null)
                    //    {
                    //        try
                    //        {
                    //            //添加到数据库
                    //            _Base_PinYin = new Base_PinYin();
                    //            _Base_PinYin.HanZi = tmpcontent.Substring(i, 1);
                    //            _Base_PinYin.PinYin = "";
                    //            _Base_PinYin.BianMa = "";
                    //            _Base_PinYin.Remarks = "";
                    //            ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Log.Record("error.log", ex);
                    //        }
                    //    }

                    //    flag = false;
                    //}
                    #endregion
                }
            }

            if (!flag)
            {
                Log.Record("chinese.log", "不支持汉字：" + tmpcontent);
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion 检查中文编码和拼音是否完整

        #region 根据汉字编码取得汉字信息
        /// <summary>
        /// 根据航信编码返回汉字信息
        /// </summary>
        /// <param name="hzBytes"></param>
        /// <returns></returns>
        public string GetHanZiByHangXinBianMa(byte[] hzBytes)
        {
            string ResHanZi = "";
            string tmpBianMa = "";
            tmpBianMa = chs2py.GetStringFromByte(hzBytes[0]);
            tmpBianMa += " " + chs2py.GetStringFromByte(hzBytes[1]);

            //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
            //{
            //    if (x.BianMa.ToUpper() == tmpBianMa.ToUpper())
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});

            ////如果系统拼音中不存在此编码，则添加到系统汉字编码数据库中
            //if (_Base_PinYin == null)
            //{
            //    try
            //    {
            //        //添加到数据库
            //        _Base_PinYin = new Base_PinYin();
            //        _Base_PinYin.HanZi = "";
            //        _Base_PinYin.PinYin = "";
            //        _Base_PinYin.BianMa = tmpBianMa.ToLower();
            //        _Base_PinYin.Remarks = "网站用户";
            //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Record("error.log", "根据汉字编码获取汉字信息出错，错误信息：" + ex.Message);
            //    }

            ResHanZi = chs2py.GetHanZi(hzBytes);
            if (ResHanZi == "")
            {
                try
                {
                    //添加到数据库
                    Base_PinYin _Base_PinYin = new Base_PinYin();
                    _Base_PinYin.HanZi = "";
                    _Base_PinYin.PinYin = "";
                    _Base_PinYin.BianMa = tmpBianMa.ToLower();
                    _Base_PinYin.Remarks = "网站用户";
                    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                }
                catch (Exception ex)
                {
                    Log.Record("error.log", "根据汉字编码获取汉字信息出错，错误信息：" + ex.Message);
                }
            }
            //}
            //else
            //{
            //    ResHanZi = _Base_PinYin.HanZi;
            //    if (ResHanZi.Trim() == "")
            //    {
            //        ResHanZi = chs2py.GetHanZi(hzBytes);
            //    }
            //}

            return ResHanZi;
        }
        #endregion

        #region 根据汉字取得汉字信息
        /// <summary>
        /// 根据汉字取得汉字编码和拼音
        /// </summary>
        /// <param name="HanZi"></param>
        /// <param name="HanZiBt"></param>
        /// <returns></returns>
        public string GetPinYinBianMaFromHanZi(string HanZi, out byte[] HanZiBt)
        {
            HanZiBt = null;

            string StrPinYin = "";

            //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
            //{
            //    if (x.HanZi == HanZi)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});

            ////如果系统拼音中不存在此汉字，则添加到系统汉字编码数据库中
            //if (_Base_PinYin == null)
            //{
            //    try
            //    {
            //        //添加到数据库
            //        _Base_PinYin = new Base_PinYin();
            //        _Base_PinYin.HanZi = HanZi;
            //        _Base_PinYin.PinYin = "";
            //        _Base_PinYin.BianMa = "";
            //        _Base_PinYin.Remarks = "网站用户" ;
            //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Record("error.log", ex);
            //    }
            StrPinYin = chs2py.convert(HanZi, out HanZiBt);
            if (StrPinYin == "")
            {
                try
                {
                    //添加到数据库
                    Base_PinYin _Base_PinYin = new Base_PinYin();
                    _Base_PinYin.HanZi = HanZi;
                    _Base_PinYin.PinYin = "";
                    _Base_PinYin.BianMa = "";
                    _Base_PinYin.Remarks = "网站用户";
                    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                }
                catch (Exception ex)
                {
                    Log.Record("error.log", ex);
                }
            }
            return StrPinYin;
            //}

            //try
            //{
            //    StrPinYin = _Base_PinYin.PinYin;
            //    HanZiBt = new byte[2];
            //    HanZiBt[0] = chs2py.GetByteFromString(_Base_PinYin.BianMa.Trim().Substring(0, _Base_PinYin.BianMa.Trim().IndexOf(" ")));
            //    HanZiBt[1] = chs2py.GetByteFromString(_Base_PinYin.BianMa.Trim().Substring(_Base_PinYin.BianMa.Trim().IndexOf(" ")));

            //    return StrPinYin;
            //}
            //catch (Exception ex)
            //{
            //    Log.Record("error.log", "根据汉字获取拼音和编码信息出错，错误信息：" + ex.Message);
            //    return "";
            //}
        }
        #endregion

        #region 分析航信返回的数据包（把汉字编码转换成正常汉字）
        //分析航信返回数据信息
        private string AnalyseServerContent(byte[] buf, int count)
        {
            string result = "";

            //主要是解析中文
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };
            ArrayList al = new ArrayList();
            ArrayList al2 = new ArrayList();

            int index = 0;
            int tmpcount = 0;
            bool isChinese = false;//是否为汉字

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);
            byte tmpbyte;

            while (index < count)
            {
                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0E))
                {
                    //汉字开始标志
                    al2.Clear();
                    index += 2;
                    isChinese = true;
                    continue;
                }

                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0F))
                {
                    string tmpcontent = "";
                    bool doubleFlag = false;
                    for (int i = 0; i < (al2.Count / 2); i++)
                    {
                        //如果有占用4个字节的汉字，则跳过一次
                        if (doubleFlag)
                        {
                            doubleFlag = false;
                            continue;
                        }
                        if ((byte)(al2[i * 2]) == 0x78)
                        {
                            byte[] cbuf = new byte[4];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            cbuf[2] = (byte)(al2[i * 2 + 2]);
                            cbuf[3] = (byte)(al2[i * 2 + 3]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            //index += 2;
                            doubleFlag = true;
                        }
                        else
                        {
                            byte[] cbuf = new byte[2];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            doubleFlag = false;
                        }
                    }

                    //处理并添加汉字信息
                    //把内容转换为string，判断是否为汉字，如果不是则把高低位互换然后把低位加10
                    //string tmpcontent= Encoding.Default.GetString((byte[])al2.ToArray(typeof(byte)));

                    //for (int i = 0; i < tmpcontent.Length; i++)
                    //{
                    //    int code = Char.ConvertToUtf32(tmpcontent, i);

                    //    //非汉字，需要处理，把高低位互换然后把低位加10
                    //    if ((code < chfrom)||(code > chend))
                    //    {
                    //        tmpbyte =  (byte)al2[i * 2];
                    //        al2[i * 2] = al2[i * 2 + 1];
                    //        al2[i * 2 + 1] = (byte)(tmpbyte + 0x0A);
                    //    }
                    //}

                    //al2.Clear();
                    //al2.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //把转换过的汉字追加到al中
                    al.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //汉字结束
                    index += 2;
                    isChinese = false;
                    continue;
                }

                tmpbyte = buf[index];

                //如果是汉字则处理
                if (isChinese)
                {
                    //tmpbyte += 0x80;
                    al2.Add(tmpbyte);
                }
                else
                {
                    al.Add(tmpbyte);
                }
                tmpcount++;
                index++;
            }

            //从第20位开始，读取到末尾倒数第三位
            if (buf.Length < 23)
                return "";

            if ((buf[17] == 0x1B) && (buf[18] == 0x4D))
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, tmpcount - 23);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, al.Count - 23);
            }
            else
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, tmpcount - 21);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, al.Count - 21);
            }

            return result;
        }
        #endregion 分析航信返回的数据包（把汉字编码转换成正常汉字）

        #region 格式化内容，每行80
        private string FormatResContent(string rescontent)
        {
            string tmpcontent = "";
            string tmpcontent1 = rescontent;
            string tmpstr = "";
            int pos = -1;
            pos = tmpcontent1.IndexOf('\r');
            while (pos != -1)
            {
                tmpstr = tmpcontent1.Substring(0, pos);
                if (tmpstr.Length > 80)
                {
                    string tmpstr2 = tmpstr;
                    while (tmpstr2.Length > 80)
                    {
                        //添加换行符
                        tmpcontent += tmpstr2.Substring(0, 80) + "\r";// + tmpstr.Substring(80) + "\r");
                        tmpstr2 = tmpstr2.Substring(80);
                    }
                    if (tmpstr2.Length > 0)
                    {
                        tmpcontent += tmpstr2 + "\r";
                    }
                }
                else
                {
                    tmpcontent += (tmpstr + "\r");
                }

                tmpcontent1 = tmpcontent1.Substring(pos + 1);
                pos = tmpcontent1.IndexOf('\r');
            }

            if (tmpcontent == "")
            {
                tmpcontent = rescontent;
            }

            return tmpcontent;
        }
        #endregion 格式化内容，每行80

        #region 把AVH结果格式化为网站返回格式
        //把AVH内容格式化为网站固定格式
        private void AnalyseAVHContentToWebInfo(string sourcmd, string sourcontent, out string destcontent)
        {
            destcontent = "";

            string tmpdate = "";
            try
            {
                tmpdate = sourcmd.Substring(11, 5);

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //返回信息与查询日期不一致
                if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
                {
                    destcontent = "当日无航班或者已经销售完毕！";
                    return;
                }

                tmpdate = getStandardDate(sourcontent.Substring(1, pos1 - 1));

                #region AVH结果信息（例子）
                // 30AUG(THU) CTUBJS DIRECT ONLY                                                  
                //1-  CA4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>                                                                   T2 T3  2:35 
                //2  *ZH4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>   CA4193                                                          T2 T3  2:35 
                //3   3U8881  DS# FA PA AA YA BA TA WS HA MA GA  CTUPEK 0730   1005   330 0 S  E  
                //>               SA LA QA EA VS US RS KQ NQ XS ZS                    T1 T3  2:35 
                //4   CA4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>                                                                   T2 T3  2:20 
                //5  *ZH4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>   CA4113                                                          T2 T3  2:20 
                //6   KN2611  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>               NQ RQ SQ VQ TQ WQ GQ XQ QA US I3                    T2 --  1:50 
                //7  *MU8071  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>   KN2611      NQ RQ SQ VQ TQ WQ GQ XQ QA I3                       T2 --  1:50 
                //8   CA4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>                                                                   T2 T3  2:25 
                //9+ *ZH4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>   CA4101                                                          T2 T3  2:25 
                //**  CZ  FARE    CTUPEK/PEKCTU   YI:CZ/TZ305                                   
                #endregion AVH结果信息（例子）

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[1] { '\r' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 
                string flongtime = "";//飞行时长

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理
                        //判断是否上一航段还有舱位信息没有添加完毕
                        if (tmpclassinfo != "")
                        {
                            //分析并添加舱位信息
                            string[] sl2 = tmpclassinfo.Split(' ');
                            for (int j = 0; j < sl2.Length; j++)
                            {
                                if (sl2[j].Trim().Length != 0)
                                {
                                    //判断舱位状态
                                    //去掉关闭的无效舱位
                                    if (sl2[j].Length == 2)
                                    {
                                        //int classnum = 0;
                                        string tmpstr4 = sl2[j].Substring(1, 1);

                                        if (tmpstr4.ToLower() == "a")
                                        {
                                            tmpstr4 = "9";
                                        }

                                        //跳过关闭的无效舱位
                                        //L      没有可利用座位,但旅客可以候补
                                        //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                        //S      因达到限制销售数而没有可利用座位,但可以候补
                                        //C      该等级彻底关闭,不允许候补或申请
                                        //X      该等级取消, 不允许候补或申请
                                        //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                        if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                            || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                            || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                        {
                                            continue;
                                        }

                                        //大于9个座位显示9，其他不变
                                        //int.TryParse(tmpstr4, out classnum);
                                        //if (classnum != 0)
                                        //{
                                        classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                        // }
                                    }
                                }
                            }

                            //避免加入重复航班信息
                            if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                            {
                                if (destcontent.Trim() == "")
                                {
                                    destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                                }
                                else
                                {
                                    destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                                }
                            }

                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                        }
                        #endregion 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理

                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();

                        byte[] tmpbuf = Encoding.Default.GetBytes(flightno);
                        if((tmpbuf[tmpbuf.Length-2]==0x20)&&(tmpbuf[tmpbuf.Length-1]==0x1C))
                        {
                            flightno = flightno.Substring(0, flightno.Length - 2).Trim();
                        }

                        //此处不做空格处理
                        tmpstr = tmpstr.Substring(10);

                        #region 跳过被屏蔽的航空公司信息
                        //判断是否被屏蔽的航空公司
                        if (tmpstr.Substring(0, 31).Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司信息

                        //舱位信息
                        tmpclassinfo = tmpstr.Substring(0, 31).Trim();
                        tmpstr = tmpstr.Substring(31).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 2)+":"+tmpstr.Substring(2,2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 2)+":"+tmpstr.Substring(2,2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatflag = "1";
                        }
                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司信息
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 1).Trim() != ">")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            //添加舱位信息
                            tmpclassinfo += " "+sl[i].Substring(11, 56).Trim();
                            tmpstr = tmpstr.Substring(68);

                            //航站楼信息
                            flightterminal = tmpstr.Substring(0, 2) + tmpstr.Substring(3, 2);
                            tmpstr = tmpstr.Substring(5).Trim();

                            //飞行时长
                            flongtime = tmpstr;
                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                #region 添加末尾数据信息
                if (tmpclassinfo != "")
                {
                    //分析并添加舱位信息
                    string[] sl2 = tmpclassinfo.Split(' ');
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j].Trim().Length != 0)
                        {
                            //判断舱位状态
                            if (sl2[j].Length == 2)
                            {
                                //int classnum = 0;
                                string tmpstr4 = sl2[j].Substring(1, 1);

                                if (tmpstr4.ToLower() == "a")
                                {
                                    tmpstr4 = "9";
                                }

                                //跳过关闭的无效舱位
                                //L      没有可利用座位,但旅客可以候补
                                //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                //S      因达到限制销售数而没有可利用座位,但可以候补
                                //C      该等级彻底关闭,不允许候补或申请
                                //X      该等级取消, 不允许候补或申请
                                //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                    || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                    || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                {
                                    continue;
                                }

                                //大于9个座位显示9，其他不变
                                //int.TryParse(tmpstr4, out classnum);
                                //if (classnum != 0)
                                //{
                                classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                // }
                            }
                        }
                    }

                    //避免加入重复航班信息
                    if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                    {
                        if (destcontent.Trim() == "")
                        {
                            destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                        }
                        else
                        {
                            destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                        }
                    }
                }
                #endregion 添加末尾数据信息
            }
            catch (Exception e)
            {
                string tmpstr = e.Message;
                return;
            }
        }
        #endregion 把AVH结果格式化为网站返回格式

        #region 把信天游AVH结果格式化为网站返回格式
        //把信天游AVH内容格式化为网站固定格式
        private void AnalyseXTYAVHContentToWebInfo(string sourcmd, string sourcontent, out string destcontent)
        {
            destcontent = "";

            string tmpdate = "";
            try
            {
                tmpdate = getStandardDate(sourcmd.Substring(11, 5));

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //返回信息与查询日期不一致
                if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
                {
                    destcontent = "当日无航班或者已经销售完毕！";
                    return;
                }

                string tmpdataDate = getStandardDate(sourcontent.Substring(1, pos1 - 1));

                #region AVH结果信息（例子）
                // 30AUG(THU) CTUBJS DIRECT ONLY                                                  
                //1-  CA4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>                                                                   T2 T3  2:35 
                //2  *ZH4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>   CA4193                                                          T2 T3  2:35 
                //3   3U8881  DS# FA PA AA YA BA TA WS HA MA GA  CTUPEK 0730   1005   330 0 S  E  
                //>               SA LA QA EA VS US RS KQ NQ XS ZS                    T1 T3  2:35 
                //4   CA4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>                                                                   T2 T3  2:20 
                //5  *ZH4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>   CA4113                                                          T2 T3  2:20 
                //6   KN2611  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>               NQ RQ SQ VQ TQ WQ GQ XQ QA US I3                    T2 --  1:50 
                //7  *MU8071  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>   KN2611      NQ RQ SQ VQ TQ WQ GQ XQ QA I3                       T2 --  1:50 
                //8   CA4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>                                                                   T2 T3  2:25 
                //9+ *ZH4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>   CA4101                                                          T2 T3  2:25 
                //**  CZ  FARE    CTUPEK/PEKCTU   YI:CZ/TZ305                                   
                #endregion AVH结果信息（例子）

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[1] { '\r' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 
                string flongtime = "";//飞行时长

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理
                        //判断是否上一航段还有舱位信息没有添加完毕
                        if (tmpclassinfo != "")
                        {
                            //分析并添加舱位信息
                            string[] sl2 = tmpclassinfo.Split(' ');
                            for (int j = 0; j < sl2.Length; j++)
                            {
                                if (sl2[j].Trim().Length != 0)
                                {
                                    //判断舱位状态
                                    if (sl2[j].Length == 2)
                                    {
                                        //int classnum = 0;
                                        string tmpstr4 = sl2[j].Substring(1, 1);

                                        if (tmpstr4.ToLower() == "a")
                                        {
                                            tmpstr4 = "9";
                                        }

                                        //跳过关闭的无效舱位
                                        //L      没有可利用座位,但旅客可以候补
                                        //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                        //S      因达到限制销售数而没有可利用座位,但可以候补
                                        //C      该等级彻底关闭,不允许候补或申请
                                        //X      该等级取消, 不允许候补或申请
                                        //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                        if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                            || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                            || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                        {
                                            continue;
                                        }

                                        //大于9个座位显示9，其他不变
                                        //int.TryParse(tmpstr4, out classnum);
                                        //if (classnum != 0)
                                        //{
                                        classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                        // }
                                    }
                                }
                            }

                            //避免加入重复航班信息
                            if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                            {
                                if (destcontent.Trim() == "")
                                {
                                    destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                                }
                                else
                                {
                                    destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                                }
                            }

                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                        }
                        #endregion 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理

                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();
                        //此处不做空格处理
                        tmpstr = tmpstr.Substring(16);

                        #region 跳过被屏蔽的航空公司信息
                        //判断是否被屏蔽的航空公司
                        if (tmpstr.Substring(0, 31).Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司信息

                        //舱位信息
                        tmpclassinfo = tmpstr.Substring(0, 31).Trim();
                        tmpstr = tmpstr.Substring(31).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatflag = "1";
                        }
                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司信息
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 1).Trim() != ">")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            //添加舱位信息
                            tmpclassinfo += " "+sl[i].Substring(11, 56).Trim();
                            tmpstr = tmpstr.Substring(67).Trim();

                            //航站楼信息
                            if (tmpstr.Substring(0, 2).Trim() == "")
                            {
                                flightterminal += "--";
                            }
                            else
                            {
                                flightterminal += tmpstr.Substring(0, 2);
                            }

                            if (tmpstr.Substring(3, 2).Trim() == "")
                            {
                                flightterminal += "--";
                            }
                            else
                            {
                                flightterminal += tmpstr.Substring(3, 2);
                            }

                            tmpstr = tmpstr.Substring(5).Trim();

                            //飞行时长
                            flongtime = tmpstr;
                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                #region 添加末尾数据信息
                if (tmpclassinfo != "")
                {
                    //分析并添加舱位信息
                    string[] sl2 = tmpclassinfo.Split(' ');
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j].Trim().Length != 0)
                        {
                            //判断舱位状态
                            if (sl2[j].Length == 2)
                            {
                                //int classnum = 0;
                                string tmpstr4 = sl2[j].Substring(1, 1);

                                if (tmpstr4.ToLower() == "a")
                                {
                                    tmpstr4 = "9";
                                }

                                //跳过关闭的无效舱位
                                //L      没有可利用座位,但旅客可以候补
                                //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                //S      因达到限制销售数而没有可利用座位,但可以候补
                                //C      该等级彻底关闭,不允许候补或申请
                                //X      该等级取消, 不允许候补或申请
                                //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                    || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                    || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                {
                                    continue;
                                }

                                //大于9个座位显示9，其他不变
                                //int.TryParse(tmpstr4, out classnum);
                                //if (classnum != 0)
                                //{
                                classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                // }
                            }
                        }
                    }

                    //避免加入重复航班信息
                    if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                    {
                        if (destcontent.Trim() == "")
                        {
                            destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                        }
                        else
                        {
                            destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                        }
                    }
                }
                #endregion 添加末尾数据信息
            }
            catch (Exception e)
            {
                string tmpstr = e.Message;
                return;
            }
        }
        #endregion 把信天游AVH结果格式化为网站返回格式

        #region 把AV结果格式化为网站返回格式
        //把AV结果格式化为网站返回格式
        private void AnalyseAVContentToWebInfo(string sourcmd, string sourcontent, out string destcontent)
        {
            destcontent = "";

            if (sourcontent.Length < 80)
                return;

            //判断结果是否为AV的结果，如果不是则不做处理
            if ((sourcontent.IndexOf(")") == -1) || (sourcontent.IndexOf("(") == -1))
            {
                destcontent = "查询结果异常！";
                return;
            }

            #region 判断日期是否一致
            //判断日期是否一致
            //av:ctupek/30aug/ca/d
            string tmpcmddate = sourcmd.Trim().Substring(10, 5).ToUpper();
            if (sourcontent.ToUpper().Substring(0, sourcontent.IndexOf(")")).IndexOf(tmpcmddate) == -1)
            {
                destcontent = "当日无航班或者已经销售完毕！";
                return;
            }
            #endregion 判断日期是否一致

            #region 返回结果格式（例子）
            //返回结果格式
            //日期，起飞时间，到达时间，承运人，航班号，舱位及数量，出发城市，到达城市，机型，经停，餐食，电子客票，共享航班，子舱位，出发乘机楼到达乘机楼^
            //2012-08-30,07:00,09:35,CA,4193,F9ASO9Y9B9MSH9K9L9Q9G9S9XSNSVSTSE9*9,CTU,PEK,321,0,1,1,False,M1S,T2T3^
            //2012-08-30,07:00,09:35,ZH,4193,F9Y9B9MSH9K9L9Q9G9S9VSTSE9*9,CTU,PEK,321,0,1,1,True,,T2T3^
            //2012-08-30,07:30,10:05,3U,8881,F9P9A9Y9B9T9WSH9M9G9S9L9Q9E9VSUSDSRSKQNQXSZSO9*9,CTU,PEK,330,0,0,1,False,F2A U1S,T1T3^
            //2012-08-30,08:00,10:20,CA,4113,F9ASOSY9B9MSH9K9L9Q9G9S9XSNSVSTSE9*9,CTU,PEK,321,0,1,1,False,M1S,T2T3^
            //2012-08-30,08:00,10:20,ZH,4113,F9Y9B9MSH9K9L9Q9G9S9VSTSE9*9,CTU,PEK,321,0,1,1,True,,T2T3^
            //2012-08-30,08:05,09:55,KN,2611,F8P5ASYAKSBAEAHALAMANQRQSQVQTQWQGQXQQAUSI3,CTU,NAY,738,0,0,1,False,,T2--^
            //2012-08-30,08:05,09:55,MU,8071,F8P5ASYAKSBAEAHALAMANQRQSQVQTQWQGQXQQAI3,CTU,NAY,738,0,0,1,True,,T2--^
            //2012-08-30,09:00,11:25,CA,4101,FAASYABAMSHAKALAQAGSSSNSVSTSEA,CTU,PEK,321,0,0,1,False,M1S,T2T3^
            //2012-08-30,09:00,11:25,ZH,4101,FAYABAMSHAKALAQAGSSSVSTSEA,CTU,PEK,321,0,0,1,True,M1S,T2T3^
            //2012-08-30,09:30,11:40,KN,2927,F8PSASYAKSBAEAHALAMANARASSVATSWSGSXAQAUSI5,CTU,NAY,738,0,0,1,False,,T2--^
            //2012-08-30,09:30,11:40,MU,8027,F8PSASYAKSBAEAHALAMANARASSVATSWSGSXAQAI5,CTU,NAY,738,0,0,1,True,,T2--^
            //2012-08-30,09:30,12:05,3U,8883,F8P8A8YABATAWSHAMAGASALAQAE7VSUSRSKQNQXSZS,CTU,PEK,321,0,1,1,False,,T1T3^
            //2012-08-30,10:00,12:30,ZH,4107,FAYABAMSHAKALAQAGSSSVSTSES,CTU,PEK,321,0,1,1,True,M1S,T2T3^
            //2012-08-30,10:00,12:30,SK,9516,C4D4J4Z4Y4S4BLE4MLH4Q4V4W4KL,CTU,PEK,321,0,1,1,True,,T2T3^
            //2012-08-30,10:00,12:30,CA,4107,FAASYABAMSHAKALAQAGSSSNSVSTSES,CTU,PEK,321,0,1,1,False,M1S,T2T3^
            //2012-08-30,11:00,13:40,CA,4115,F8ASYABAMSHAKALAQAGSSSNSVSTSES,CTU,PEK,757,0,1,1,False,M1S,T2T3^
            //2012-08-30,11:00,13:40,ZH,4115,F8YABAMSHAKALAQAGSSSVSTSES,CTU,PEK,757,0,1,1,True,M1S,T2T3^
            //2012-08-30,11:30,14:10,3U,8885,F7P7A7YABATAWSHAMAGASALAQAE8VSUSRSKQNQXSZS,CTU,PEK,321,0,1,1,False,,T1T3^
            //2012-08-30,11:50,14:25,HU,7148,FAPQA2YABAHAKALAMAQAXAUQEQTQZQVQGSOQSQ,CTU,PEK,334,0,1,1,False,,--T1^
            //2012-08-30,12:00,14:30,ZH,1406,FACAYABAMSHAKALAQAGSSSVSTSEA,CTU,PEK,747,0,1,1,True,M1S,T2T3^
            //2012-08-30,12:00,14:30,CA,1406,FAASCAD3Z4JSR2YABAMSHAKALAQAGSSSNSVSTSEA,CTU,PEK,747,0,1,1,False,M1S,T2T3^
            //2012-08-30,12:20,14:45,CZ,6162,FAP5WAZAYATAKAHAMAG6SALAQQUAEQVQBQXQNQRQ,CTU,PEK,330,0,1,1,False,,T2T2^
            //2012-08-30,12:20,14:45,MU,3011,YABAEAHALAM6NARASQKAVQTQWQ,CTU,PEK,330,0,1,1,True,,T2T2
            #endregion 返回结果格式（例子）

            #region AV指令返回信息（例子）
            // 30AUG(THU) CTUPEK      DIRECT ONLY                                             
            //1- *ZH4193  CTUPEK 0700   0935   321 0^B         EFA YA BA MS HA<T2T3>          
            //    CA4193         KA LA QA GA SA VS TS EA                                      
            //2   CA4193  CTUPEK 0700   0935   321 0^B         EFA AS OA YA BA<T2T3>          
            //                   MS HA KA LA QA GA SA XS NS VS TS EA                          
            //               ** M1S                                                           
            //3   3U8881  CTUPEK 0730   1005   330 0 S         EFA PA AA YA BA<T1T3>          
            //                   TA WS HA MA GA SA LA QA EA VS US DS RS KQ NQ XS ZS OA        
            //               ** F2A U1S                                                       
            //4  *ZH4113  CTUPEK 0800   1020   321 0^B         EFA YA BA MS HA<T2T3>          
            //    CA4113         KA LA QA GA SA VS TS EA                                      
            //5+  CA4113  CTUPEK 0800   1020   321 0^B         EFA AS OS YA BA<T2T3>          
            //                   MS HA KA LA QA GA SA XS NS VS TS EA                          
            //               ** M1S                                                           


            // 30AUG(THU) CTUSHA VIA 3U DIRECT ONLY                                           
            //1-  3U8961  CTUPVG 0800   1035   321 0 S         EFA PA AA YA BA<T1-->          
            //                   TA WA HA MS GA SS LA QS EA VS US DA RS KS NS XA ZS OA        
            //               ** F2A U1S                                                       
            //2   3U8963  CTUPVG 1145   1435   321 0 S         EFA PA AA YA BS<T1-->          
            //                   TA WS HA MS GA SS LA QS EA VS US DS RS KS NS XA ZS OS        
            //               ** F2A U1S                                                       
            //3   3U8965  CTUPVG 1600   1835   321 0 S         EFA PA AA YA BA<T1-->          
            //                   TA WA HA MS GA SS LA QS ES VS US DA RS KS NS XA ZS OA        
            //               ** F2A U1S                                                       
            //4+  3U8647  CTUPVG 1835   2105   330 0 S         EFA PA AA CC IC<T1-->          
            //                JC YA BA TA WA HA MS GA SS LA QS EA VS US DS RS KS NS XA ZS OA  
            //               ** F2A U1S                                                       
            //**  All scheduled MU or FM flights operated by MU or FM are "Eastern Express"   
            #endregion AV指令返回信息（例子）

            string tmpdate = "";
            try
            {
                //根据当前时间把查询日期转换为标准日期
                tmpdate = getStandardDate(sourcmd.Substring(10, 5));

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return;
                }

                //返回信息与查询日期不一致
                if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpcmddate.ToUpper()) == -1)
                {
                    destcontent = "当日无航班或者已经销售完毕！";
                    return;
                }

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[1] { '\r' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理
                        //判断是否上一航段还有舱位信息没有添加完毕
                        if (tmpclassinfo != "")
                        {
                            //分析并添加舱位信息
                            string[] sl2 = tmpclassinfo.Split(' ');
                            for (int j = 0; j < sl2.Length; j++)
                            {
                                if (sl2[j].Trim().Length != 0)
                                {
                                    //判断舱位状态
                                    if (sl2[j].Length == 2)
                                    {
                                        //int classnum = 0;
                                        string tmpstr4 = sl2[j].Substring(1, 1);

                                        if (tmpstr4.ToLower() == "a")
                                        {
                                            tmpstr4 = "9";
                                        }

                                        //跳过关闭的无效舱位
                                        //L      没有可利用座位,但旅客可以候补
                                        //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                        //S      因达到限制销售数而没有可利用座位,但可以候补
                                        //C      该等级彻底关闭,不允许候补或申请
                                        //X      该等级取消, 不允许候补或申请
                                        //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                        if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                            || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                            || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                        {
                                            continue;
                                        }

                                        //大于9个座位显示9，其他不变
                                        //int.TryParse(tmpstr4, out classnum);
                                        //if (classnum != 0)
                                        //{
                                        classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                        // }
                                    }
                                }
                            }

                            //避免加入重复航班信息
                            if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                            {
                                if (destcontent.Trim() == "")
                                {
                                    destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                                }
                                else
                                {
                                    destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                                }
                            }

                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                        }
                        #endregion 判断是否上一航段还有舱位信息没有添加完毕，如果有则添加处理

                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();
                        tmpstr = tmpstr.Substring(6).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatflag = "1";
                        }
                        tmpstr = tmpstr.Substring(2).Trim();

                        if (tmpstr.Substring(1, 1) == " ")
                        {
                            tmpstr = tmpstr.Substring(1).Trim();
                        }

                        //航班舱位信息
                        if (tmpstr.IndexOf("<") != -1)
                        {
                            tmpclassinfo = tmpstr.Substring(1, tmpstr.IndexOf("<") - 1).Trim();

                            tmpstr = tmpstr.Substring(15).Trim();

                            //航站楼信息
                            flightterminal = tmpstr.Substring(1, 4);
                        }
                        else
                        {
                            tmpclassinfo = tmpstr.Substring(1).Trim();

                            //航站楼信息
                            flightterminal = "----";
                        }

                        #region 跳过被屏蔽的航空公司
                        //跳过被屏蔽的航空公司
                        if (tmpclassinfo.Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司

                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 3).Trim() != "")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            tmpclassinfo += " "+sl[i].Substring(12).Trim();
                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                #region 添加末尾数据信息
                if (tmpclassinfo != "")
                {
                    //分析并添加舱位信息
                    string[] sl2 = tmpclassinfo.Split(' ');
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j].Trim().Length != 0)
                        {
                            //判断舱位状态
                            if (sl2[j].Length == 2)
                            {
                                //int classnum = 0;
                                string tmpstr4 = sl2[j].Substring(1, 1);

                                if (tmpstr4.ToLower() == "a")
                                {
                                    tmpstr4 = "9";
                                }

                                //跳过关闭的无效舱位
                                //L      没有可利用座位,但旅客可以候补
                                //Q      永久申请状态,没有可利用座位,但可以申请(HN)
                                //S      因达到限制销售数而没有可利用座位,但可以候补
                                //C      该等级彻底关闭,不允许候补或申请
                                //X      该等级取消, 不允许候补或申请
                                //Z      座位可利用情况不明,这种情况有可能在外航航班上出现
                                if ((tmpstr4.ToLower() == "l") || (tmpstr4.ToLower() == "q")
                                    || (tmpstr4.ToLower() == "s") || (tmpstr4.ToLower() == "c")
                                    || (tmpstr4.ToLower() == "x") || (tmpstr4.ToLower() == "z"))
                                {
                                    continue;
                                }

                                //大于9个座位显示9，其他不变
                                //int.TryParse(tmpstr4, out classnum);
                                //if (classnum != 0)
                                //{
                                classinfo += (sl2[j].Substring(0, 1) + tmpstr4);//sl2[j].Trim();
                                // }
                            }
                        }
                    }

                    //避免加入重复航班信息
                    if ((classinfo.Trim().Length != 0) && (destcontent.IndexOf(carrier + "," + flightno + ",") == -1))
                    {
                        if (destcontent.Trim() == "")
                        {
                            destcontent = tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal;
                        }
                        else
                        {
                            destcontent += ("^" + tmpdate + "," + flighttime + "," + arrivetime + "," + carrier + "," + flightno + "," + classinfo + "*9," + fromcity + "," + tocity + "," + plane + "," + stopflag + "," + eatflag + "," + etflag + "," + shareflag + "," + childclassinfo + "," + flightterminal);
                        }
                    }
                }
                #endregion 添加末尾数据信息
            }
            catch (Exception e)
            {
                Log.Record("client.log", "把AV结果格式化成网站数据格式出错，错误信息：" + e.Message);
                return;
            }
        }
        #endregion 把AV结果格式化为网站返回格式

        #region 根据28JUL07返回标准日期格式2007-07-28
        /// <summary>
        /// 根据28JUL07返回标准日期格式2007-07-28
        /// </summary>
        /// <param name="Date">返回日期格式（2007-03-06）</param>
        private string getStandardDate(string Date)
        {
            if (Date.Length < 5)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            string tmpDate = Date.Substring(0, 2);

            string Month = Date.Substring(2, 3).ToUpper();

            string tmpMonth = "";

            switch (Month)
            {
                case "JAN":
                    tmpMonth = "01";
                    break;
                case "FEB":
                    tmpMonth = "02";
                    break;
                case "MAR":
                    tmpMonth = "03";
                    break;
                case "APR":
                    tmpMonth = "04";
                    break;
                case "MAY":
                    tmpMonth = "05";
                    break;
                case "JUN":
                    tmpMonth = "06";
                    break;
                case "JUL":
                    tmpMonth = "07";
                    break;
                case "AUG":
                    tmpMonth = "08";
                    break;
                case "SEP":
                    tmpMonth = "09";
                    break;
                case "OCT":
                    tmpMonth = "10";
                    break;
                case "NOV":
                    tmpMonth = "11";
                    break;
                case "DEC":
                    tmpMonth = "12";
                    break;
            }
            if (Date.Length >= 7)
            {
                if (Date.Substring(5, 2).Trim().Length == 2)
                {
                    return "20" + Date.Substring(5, 2) + "-" + tmpMonth + "-" + tmpDate;
                }
                else
                {
                    return DateTime.Now.ToString("yyyy") + "-" + tmpMonth + "-" + tmpDate;
                }
            }
            else
            {
                //判断日期是否小于当前日期，如果小于则把年份更改为明年
                string resDate = DateTime.Now.ToString("yyyy") + "-" + tmpMonth + "-" + tmpDate + " 23:59:59";
                if (DateTime.Compare(DateTime.Parse(resDate), DateTime.Now) < 0)
                {
                    //跨年
                    return DateTime.Parse(resDate).AddYears(1).ToString("yyyy-MM-dd");
                }
                else
                {
                    return resDate.Substring(0, 10);
                }
            }
        }
        #endregion 根据28JUL07返回标准日期格式2007-07-28

        #region 网站请求指令处理
        /// <summary>
        /// 网站数据包处理
        /// </summary>
        public void WebProcFunc()
        {
            //可用配置的Id
            int EtermId = -1;

            //可用配置的名称
            string EtermName = "";

            try
            {
                int rcount = packetBuff.Length;
                //userType = 2;
                if (rcount < (39 + m_WebUser.Length))
                {
                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "收到网站用户数据包不全，断开连接...");
                    }

                    //数据包不全
                    ns.Close();
                    newclient.Close();
                    //continue;
                }
                //获取用户名和密码，比较正确性
                byte[] userbuf = new byte[m_WebUser.Length];
                byte[] pwdbuf = new byte[32];

                Array.Copy(packetBuff, 5, userbuf, 0, m_WebUser.Length);
                Array.Copy(packetBuff, 5 + m_WebUser.Length, pwdbuf, 0, 32);

                string tmpuser = Encoding.Default.GetString(userbuf);
                string tmppwd = Encoding.Default.GetString(pwdbuf);

                byte[] cmdbuf = new byte[rcount - 39 - tmpuser.Length];
                Array.Copy(packetBuff, 37 + tmpuser.Length, cmdbuf, 0, cmdbuf.Length);
                string tmpcmd = Encoding.Default.GetString(cmdbuf);
                string tmpcmd2 = tmpcmd;

                int len = -1;
                byte[] SendBuf = null;

                string tmpstr = "";
                string tmpstr2 = "";

                #region 判断网站用户名、密码
                //用户名或密码不正确
                if ((tmpuser != m_WebUser) || (AppGlobal.EncryptMD5(m_WebPWD).ToUpper() != tmppwd.ToUpper()))
                {
                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "收到网站用户数据，用户名或密码不正确，返回结果并断开...");
                    }
                    //返回错误提示

                    len = 4 + Encoding.Default.GetBytes("用户名或密码不正确！").Length;
                    SendBuf = new byte[len];

                    Array.Copy(m_WebHeadPack, 0, SendBuf, 0, 2);
                    Array.Copy(Encoding.Default.GetBytes("用户名或密码不正确！"), 0, SendBuf, 2, len - 4);
                    Array.Copy(m_WebEndPack, 0, SendBuf, len - 2, 2);

                    ns.Write(SendBuf, 0, len);

                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "收到网站请求指令：" + tmpcmd2);
                        Log.Record("client.log", "网站用户名或密码不正确！");
                    }

                    tmpstr = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser + " 发送指令]：\r" + tmpcmd2 + "\r";
                    tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser +
                        " 返回结果]：\r网站用户名或密码不正确！\r";

                    if (m_ArrayContents != null)
                    {
                        lock (m_ArrayContents)
                        {
                            m_ArrayContents.Add(tmpstr);
                            m_ArrayContents.Add(tmpstr2);
                        }
                    }

                    ns.Close();
                    newclient.Close();
                    //continue;
                }
                #endregion 判断网站用户名、密码

                #region 把网站发送指令的换行符^替换成\r
                tmpcmd = tmpcmd.Replace('^', '\r');
                #endregion 把网站发送指令的换行符^替换成\r

                #region 判断对于汉字编码及拼音的支持
                if (!CheckChinesePinYin(tmpcmd))
                {
                    //Log.Record("chinese.log", content);

                    tmpstr = "存在不支持的汉字，请检查！";

                    len = Encoding.Default.GetBytes(tmpstr).Length + 4;
                    SendBuf = new byte[len];
                    Array.Copy(m_WebHeadPack, SendBuf, 2);
                    Array.Copy(Encoding.Default.GetBytes(tmpstr), 0, SendBuf, 2, len - 4);
                    Array.Copy(m_WebEndPack, 0, SendBuf, len - 2, 2);

                    //存在不支持的汉字，返回错误
                    //返回错误，等待超时                                
                    newclient.Send(SendBuf);

                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "存在不支持的汉字，请检查！发送指令 ：" + tmpcmd);
                    }

                    //组织请求内容
                    tmpstr = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser + " 发送指令]：\r" + tmpcmd2 + "\r";
                    tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser +
                        " 返回结果]：\r存在航信不支持的汉字，请检查！\r";

                    //添加窗口显示信息
                    if (m_ArrayContents != null)
                    {
                        lock (m_ArrayContents)
                        {
                            m_ArrayContents.Add(tmpstr);
                            m_ArrayContents.Add(tmpstr2);
                        }
                    }

                    newclient.Close();
                    //continue;
                }
                #endregion 判断对于汉字编码及拼音的支持

                //指定Office号
                string OfficeCode = "";
                //是否自动PN
                bool PnFlag = false;
                //是否返回所有结果
                bool AllFlag = false;

                #region 是否自动PN
                int PnPos1 = -1;
                int PnPos2 = -1;
                PnPos1 = tmpcmd.ToLower().IndexOf("<pn>");
                PnPos2 = tmpcmd.ToLower().IndexOf("</pn>");
                if ((PnPos1 != -1) && (PnPos2 != -1))
                {
                    if (tmpcmd.Substring(PnPos1 + 4, PnPos2 - PnPos1 - 4) == "1")
                    {
                        PnFlag = true;
                    }
                    else
                    {
                        PnFlag = false;
                    }
                }
                #endregion 是否自动PN

                #region 是否返回所有指令结果
                int AllPos1 = -1;
                int AllPos2 = -1;
                AllPos1 = tmpcmd.ToLower().IndexOf("<all>");
                AllPos2 = tmpcmd.ToLower().IndexOf("</all>");
                if ((AllPos1 != -1) && (AllPos2 != -1))
                {
                    if (tmpcmd.Substring(AllPos1 + 5, AllPos2 - AllPos1 - 5) == "1")
                    {
                        AllFlag = true;
                    }
                    else
                    {
                        AllFlag = false;
                    }
                }
                #endregion 是否返回所有指令结果

                #region 分析指定Office号
                //分析是否指定配置处理<OFFICE></OFFICE>                            
                int officepos1 = -1;
                int officepos2 = -1;
                officepos1 = tmpcmd.ToLower().IndexOf("<office>");
                officepos2 = tmpcmd.ToLower().IndexOf("</office>");
                if ((officepos1 != -1) && (officepos2 != -1))
                {
                    OfficeCode = tmpcmd.Substring(officepos1 + 8, officepos2 - officepos1 - 8);
                    //去掉Office结构信息
                    tmpcmd = tmpcmd.Substring(0, officepos1);
                }
                #endregion 分析指定Office号

                //是否找到可用配置
                bool FindFlag = false;
                //开始查找时间
                DateTime BeginDate = DateTime.Now;

                #region 查找可用配置信息
                //如果查找到可用配置或者超时则退出循环
                while ((!FindFlag) && (PublicInfo.DateDiff(DateTime.Now, BeginDate) < m_MaxOutTimes))
                {
                    //如果查找到对应配置，则占用该配置并返回
                    FindFlag = m_ServerManage.GetIdleEtermInfo(tmpcmd, OfficeCode, ref EtermId, ref EtermName);
                    if (!FindFlag)
                    {
                        Thread.Sleep(100);
                    }
                }
                #endregion 查找可用配置信息

                #region 未找到可用配置的处理
                if (!FindFlag)
                {
                    //返回超时无可用配置结果提示
                    //newclient.Send(Encoding.Default.GetBytes(AppGlobal.Compress("航信服务器返回超时,请稍候重试")));
                    tmpstr = "航信服务器返回超时,请稍候重试！";

                    len = Encoding.Default.GetBytes(tmpstr).Length + 4;
                    SendBuf = new byte[len];
                    Array.Copy(m_WebHeadPack, SendBuf, 2);
                    Array.Copy(Encoding.Default.GetBytes(tmpstr), 0, SendBuf, 2, len - 4);
                    Array.Copy(m_WebEndPack, 0, SendBuf, len - 2, 2);

                    //返回错误，等待超时                                
                    newclient.Send(SendBuf);

                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "收到网站请求指令：" + tmpcmd2);
                        Log.Record("client.log", "向网站回复信息：航信服务器返回超时,请稍候重试！，未找到可用配置：" + OfficeCode);
                    }

                    //组织请求内容
                    tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser + " 发送指令]：\r" + tmpcmd2 + "\r";
                    tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser +
                        " 返回结果]：\r未找到可用配置" + OfficeCode + "...\r";

                    if (m_ArrayContents != null)
                    {
                        lock (m_ArrayContents)
                        {
                            m_ArrayContents.Add(tmpstr);
                            m_ArrayContents.Add(tmpstr2);
                        }
                    }

                    newclient.Close();
                    //continue;
                }
                #endregion 未找到可用配置的处理

                //组织发送数据包
                byte[] tmpbuf1 = { 0x01, 0x00, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x01, 0x36, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x21, 0x20, 0x00, 0x0F, 0x1E };
                byte[] tmpbuf2 = { 0x20, 0x03 };
                ArrayList al = new ArrayList();

                //处理指令（主要是汉字编码及拼音处理）
                //把收到的指令数据包处理成航信格式（主要是把中文处理及添加拼音）
                //byte[] CommandBuf = null;
                //AnalyseWebCmdAndMakeServerInfo(tmpcmd, out CommandBuf, true);
                byte[] CommandBuf = Encoding.Default.GetBytes(tmpcmd);

                //把发送内容组织成标准数据包格式（包括报头、报尾）
                al.AddRange(tmpbuf1);
                al.AddRange(CommandBuf);
                al.AddRange(tmpbuf2);

                //结果标志
                bool ResFlag = false;
                //返回错误消息
                string ErrorMessage = "";
                //返回结果数据包
                byte[] ResBuf = null;

                if (m_DebugFlag)
                {
                    Log.Record("client.log", "准备发送指令：" + tmpcmd);
                }

                #region 发送指令并获取返回信息
                if (PnFlag)
                {
                    ResFlag = m_ServerManage.SendCommandAndGetAllPnResult(tmpuser, tmpcmd, (byte[])al.ToArray(typeof(byte)), OfficeCode, AllFlag, EtermId, ref ResBuf, ref ErrorMessage);
                }
                else
                {
                    ResFlag = m_ServerManage.SendCommandAndGetResult(tmpuser, tmpcmd, (byte[])al.ToArray(typeof(byte)), OfficeCode, AllFlag, EtermId, ref ResBuf, ref ErrorMessage);
                }
                #endregion 发送指令并获取返回信息

                #region 处理出错并返回错误信息
                if (!ResFlag)
                {
                    if (m_DebugFlag)
                    {
                        Log.Record("client.log", "返回错误信息：" + ErrorMessage);
                    }

                    //出错，返回错误信息
                    tmpstr = ErrorMessage;

                    len = Encoding.Default.GetBytes(tmpstr).Length + 4;
                    SendBuf = new byte[len];
                    Array.Copy(m_WebHeadPack, SendBuf, 2);
                    Array.Copy(Encoding.Default.GetBytes(tmpstr), 0, SendBuf, 2, len - 4);
                    Array.Copy(m_WebEndPack, 0, SendBuf, len - 2, 2);

                    newclient.Send(SendBuf);

                    //组织请求内容
                    tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser + " 发送指令]：\r" + tmpcmd2 + "\r";
                    tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser +
                        " 返回结果]：\r" + ErrorMessage+"\r";

                    if (m_ArrayContents != null)
                    {
                        lock (m_ArrayContents)
                        {
                            m_ArrayContents.Add(tmpstr);
                            m_ArrayContents.Add(tmpstr2);
                        }
                    }

                    newclient.Close();
                    //continue;
                }
                #endregion 处理出错并返回错误信息

                #region 处理成功并返回结果
                else
                {
                    //成功，返回对应的结果信息
                    string strResContent = "";

                    //分析发送的内容（把汉字编码转换为正常汉字）
                    strResContent = AnalyseServerContent(ResBuf, ResBuf.Length);

                    #region 处理行程单创建指令
                    //处理行程单创建返回结果信息
                    if ((tmpcmd.Length>6)&&((tmpcmd.ToLower().Substring(0, 6) == "prinv:") || (tmpcmd.ToLower().Substring(0, 6) == "vtinv:")))
                    {
                        //创建失败
                        if (strResContent.ToLower().IndexOf("<flag>e</flag>") != -1)
                        {
                            int index1 = strResContent.ToLower().IndexOf("<errorreason>");
                            int index2 = strResContent.ToLower().IndexOf("</errorreason>");
                            if ((index1 != -1) && (index2 != -1))
                            {
                                strResContent = "ERROR:" + strResContent.Substring(index1 + 13, index2 - index1 - 13).Replace("！", "!").Replace("，", ",");
                            }
                            else
                            {
                                strResContent = "ERROR";
                            }
                        }
                        //创建成功
                        else if (strResContent.ToLower().IndexOf("<flag>s</flag>") != -1)
                        {
                            strResContent = "SUCCESS";
                        }

                        //直接返回
                        len = Encoding.Default.GetBytes(strResContent).Length + 4;
                        SendBuf = new byte[len];
                        Array.Copy(m_WebHeadPack, SendBuf, 2);
                        Array.Copy(Encoding.Default.GetBytes(strResContent), 0, SendBuf, 2, len - 4);
                        Array.Copy(m_WebEndPack, 0, SendBuf, len - 2, 2);

                        if (m_DebugFlag)
                        {
                            Log.Record("client.log", "收到网站请求指令：" + tmpcmd2);
                            Log.Record("client.log", "返回网站结果：" + strResContent);
                        }

                        //组织请求内容
                        tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser + " 发送指令]：\r" + tmpcmd2 + "\r";
                        tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser +
                            " 返回结果]：\r" + strResContent+"\r";

                        if (m_ArrayContents != null)
                        {
                            lock (m_ArrayContents)
                            {
                                m_ArrayContents.Add(tmpstr);
                                m_ArrayContents.Add(tmpstr2);
                            }
                        }

                        newclient.Send(SendBuf);

                        newclient.Close();
                        //continue;
                    }
                    #endregion 处理行程单创建指令
                    else
                    {

                        if (m_DebugFlag)
                        {
                            Log.Record("client.log", "收到" + EtermName + "返回结果：" + strResContent);
                        }

                        if (strResContent.IndexOf("需要授权") != -1)
                        {
                            strResContent = strResContent.Substring(0, strResContent.IndexOf("需要授权") + 4)+"\r";
                            if (m_DebugFlag)
                            {
                                //
                                Log.Record("client.log", "收到" + EtermName + "返回结果(去除无效返回)：" + strResContent);
                            }
                        }

                        //格式化（如果有大于80的行，则添加换行符）
                        strResContent = FormatResContent(strResContent);

                        //航空公司配置返回信息有可能换行符之前存在 0x1B, 0x62非法字符，去掉
                        byte[] Errorbuf = { 0x1B, 0x62 };
                        string strError = Encoding.Default.GetString(Errorbuf);
                        strResContent = strResContent.Replace(strError, "");
                        byte[] Errorbuf2 = { 0x1B, 0x0B };
                        string strError2 = Encoding.Default.GetString(Errorbuf2);
                        strResContent = strResContent.Replace(strError2, "");

                        string tmpcontent = strResContent;

                        #region 把AV 或 AVH 的结果处理成网站模式结果
                        //主要是把AVH结果处理成网站模式结果
                        if ((tmpcmd.Trim().Length > 3) && (tmpcmd.Trim().ToUpper().Substring(0, 3) == "AVH"))
                        {
                            //判断是否为信天游
                            if (m_ServerManage.IfXinTianYouEterm(EtermId))
                            {
                                //把信天游的AVH结果格式化为网站格式
                                AnalyseXTYAVHContentToWebInfo(tmpcmd, tmpcontent, out strResContent);
                            }
                            else
                            {
                                //把Eterm配置的AVH结果格式化为网站格式
                                AnalyseAVHContentToWebInfo(tmpcmd, tmpcontent, out strResContent);
                            }
                        }
                        else if ((tmpcmd.Trim().Length > 3) && (tmpcmd.Trim().ToUpper().Substring(0, 3) == "AV:"))
                        {
                            AnalyseAVContentToWebInfo(tmpcmd, tmpcontent, out strResContent);
                        }
                        #endregion 把AV 或 AVH 的结果处理成网站模式结果

                        len = Encoding.Default.GetBytes(strResContent).Length + 4;
                        SendBuf = new byte[len];
                        Array.Copy(m_WebHeadPack, SendBuf, 2);
                        Array.Copy(Encoding.Default.GetBytes(strResContent), 0, SendBuf, 2, len - 4);
                        Array.Copy(m_WebEndPack, 0, SendBuf, len - 2, 2);

                        if (m_DebugFlag)
                        {
                            Log.Record("client.log", "收到网站请求指令：" + tmpcmd2);
                            Log.Record("client.log", "返回" + EtermName + "网站结果：" + strResContent);
                        }

                        //组织请求内容
                        tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser + " 发送指令]：\r" + tmpcmd2 + "\r";
                        tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + tmpuser +
                            " 收到" + EtermName + "返回结果]：\r" + strResContent;

                        if (m_ArrayContents != null)
                        {
                            lock (m_ArrayContents)
                            {
                                m_ArrayContents.Add(tmpstr);
                                m_ArrayContents.Add(tmpstr2);
                            }
                        }

                        newclient.Send(SendBuf);

                        newclient.Close();
                        //continue;
                    }
                }
                #endregion 处理成功并返回结果
            }
            catch (Exception ex)
            {
                Log.Record("client.log", "配置:"+EtermName+" 网站数据处理异常，错误信息："+ex.Message);
                if (newclient != null)
                {
                    newclient.Close();
                    newclient = null;
                }
                ns = null;
            }
        }
        #endregion 网站请求指令处理
    }

    /// <summary>
    /// 客户端处理类
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// PID放大帐号信息
        /// </summary>
        private Base_UserConfigManage m_UserConfigManage = null;

        /// <summary>
        /// 配置类列表
        /// </summary>
        private ServerManage.ServerManage m_ServerManage = null;

        /// <summary>
        /// 发送及接收到的信息
        /// </summary>
        private ArrayList m_ArrayContents = null;

        /// <summary>
        /// 处理Socket
        /// </summary>
        private Socket m_ClientSocket = null;

        /// <summary>
        /// 收发指令处理线程
        /// </summary>
        private Thread m_ClientThread = null;

        /// <summary>
        /// 调试输出信息开关
        /// </summary>
        private bool m_DebugFlag = false;

        /// <summary>
        /// 调试数据包输出信息开关
        /// </summary>
        private bool m_DebugDataFlag = false;

        /// <summary>
        ///未加密Socket流
        /// <summary>
        private NetworkStream m_NetStream = null;

        /// <summary>
        /// 接收数据缓冲区大小1024KB 
        /// </summary>
        private const int m_MaxPacket = 1024 * 1024;

        /// <summary>
        /// 登录帐号
        /// </summary>
        private string m_UserName = "";

        /// <summary>
        /// 用于回复客户端的心跳包
        /// </summary>
        private readonly byte[] m_heartbuf = { 0x01, 0xFA, 0x00, 0x05, 0x00 };

        /// <summary>
        /// 黑屏包头
        /// </summary>
        private readonly byte[] cmdhead1 = { 0x41, 0x51 };

        /// <summary>
        /// 接收到的指令序号
        /// </summary>
        private byte recno = 0x20;

        /// <summary>
        /// 是否已经回复了成功登陆信息（标志）{ 0x01, 0xFD, 0x00, 0x06, 0x00, 0x00 };
        /// </summary>
        private bool m_SendSucFlag = false;

        /// <summary>
        /// 用于回复客户端的没有指令执行权限信息
        /// </summary>
        private byte[] m_nopowerbuf = null;

        /// <summary>
        /// 最大指令超时时间（秒）
        /// </summary>
        private const int m_MaxOutTimes = 6;

        /// <summary>
        /// 发送的上下文内容（发送i指令后清空）
        /// </summary>
        private ArrayList m_SendCommandList = new ArrayList();

        /// <summary>
        /// 上下文信息
        /// </summary>
        private List<Base_CommandManage> m_BaseCommandManageList = null;

        /// <summary>
        /// 配置序号
        /// </summary>
        private int m_EtermId = -1;

        /// <summary>
        /// 汉字拼音编码信息
        /// </summary>
        private List<Base_PinYin> m_BasePinYinList = null;

        /// <summary>
        /// 城市信息
        /// </summary>
        private List<Base_CityInfoManage> m_BaseCityInfoManageList = null;

        /// <summary>
        /// 发送指令计数
        /// </summary>
        private int m_SendCount = 0;

        /// <summary>
        /// PID放大帐号信息
        /// </summary>
        public Base_UserConfigManage UserConfigManage
        {
            get { return m_UserConfigManage; }
            set { m_UserConfigManage = value; }
        }

        /// <summary>
        /// 配置类列表
        /// </summary>
        public ServerManage.ServerManage ServerManage
        {
            get { return m_ServerManage; }
            set { m_ServerManage = value; }
        }

        /// <summary>
        /// 接收指令和回复结果的信息，用于窗口显示
        /// </summary>
        public ArrayList ArrayContents
        {
            get { return m_ArrayContents; }
            set { m_ArrayContents = value; }
        }

        /// <summary>
        /// 处理Socket
        /// </summary>
        public Socket ClientSocket
        {
            get { return m_ClientSocket; }
            set { m_ClientSocket = value; }
        }

        /// <summary>
        /// 收发指令处理线程
        /// </summary>
        public Thread ClientThread
        {
            get { return m_ClientThread; }
            set { m_ClientThread = value; }
        }

        /// <summary>
        /// 调试信息输出标志
        /// </summary>
        public bool DebugFlag
        {
            get { return m_DebugFlag; }
            set { m_DebugFlag = value; }
        }

        /// <summary>
        /// 调试数据包信息输出标志
        /// </summary>
        public bool DebugDataFlag
        {
            get { return m_DebugDataFlag; }
            set { m_DebugDataFlag = value; }
        }

        /// <summary>
        /// 登录帐号
        /// </summary>
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }

        /// <summary>
        /// 上下文信息
        /// </summary>
        public List<Base_CommandManage> BaseCommandManageList
        {
            set { m_BaseCommandManageList = value; }
            get { return m_BaseCommandManageList; }
        }

        /// <summary>
        /// 配置序号
        /// </summary>
        public int EtermId
        {
            get { return m_EtermId; }
            set { m_EtermId = value; }
        }

        /// <summary>
        /// 汉字拼音编码信息
        /// </summary>
        public List<Base_PinYin> BasePinYinList
        {
            get { return m_BasePinYinList; }
            set { m_BasePinYinList = value; }
        }

        /// <summary>
        /// 城市信息
        /// </summary>
        public List<Base_CityInfoManage> BaseCityInfoManageList
        {
            get { return m_BaseCityInfoManageList; }
            set { m_BaseCityInfoManageList = value; }
        }

        /// <summary>
        /// 指令处理
        /// </summary>
        public void ClientProcFunc()
        {
            string content = "";
            string tmpstr = "";

            int len = -1;
            byte[] SendBuf = null;

            byte[] received = new byte[m_MaxPacket];

            //是否已回复标志
            bool SendFlag = false;

            DateTime ReceiveTime = DateTime.Now;

            while (true)
            {
                try
                {
                    if (m_ClientSocket.Poll(-1, SelectMode.SelectRead))
                    {
                        int rcount = m_ClientSocket.Receive(received);

                        //已回复标志
                        SendFlag = false;

                        //接收指令时间
                        ReceiveTime = DateTime.Now;

                        if (DebugDataFlag)
                        {
                            Log.Record("client.log", "接收到帐号：" + UserName + "发送的数据包：", received, rcount);
                        }

                        //Kevin 2010-08-04 Edit
                        //如果当前用户帐号为空，则提示该账号已在其他地方登录，断开连接
                        if (m_UserName.Trim() == "")
                        {
                            if (DebugFlag)
                            {
                                Log.Record("client.log", "帐号：" + UserName + " 已在其他地方登录，断开连接...");
                            }

                            SendClientCmd("", "该帐号已在其他地方登录,连接已断开!", 2);
                            tmpstr = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + m_UserName + " 登出系统\n";

                            //断开Socket连接，退出线程处理
                            m_ClientSocket.Close();
                            m_ClientSocket = null;

                            m_UserConfigManage = null;
                            break;
                        }

                        //取得收到的数据包
                        byte[] recbuf = new byte[rcount];
                        Array.Copy(received, 0, recbuf, 0, rcount);

                        //如果没有数据信息，则继续
                        if (rcount == 0)
                        {
                            //socket连接已断开
                            //Log.Record("client.log", "已断开！");

                            if (m_DebugFlag)
                            {
                                Log.Record("client.log", "接收指令异常（空），断开" + m_UserName + "的连接！");
                            }

                            //更新帐号的使用指令计数
                            try
                            {
                                UserConfigManage.User_SendCount += m_SendCount;
                                ManageSpace.Manage.Modify_Base_UserSendCmdCount(UserConfigManage);
                                m_SendCount = 0;
                            }
                            catch (Exception ex)
                            {
                                Log.Record("error.log", "更新帐号指令计数失败，帐号：" + UserName + "，使用指令数：" + m_SendCount.ToString());
                            }


                            //断开Socket连接，退出线程处理
                            m_ClientSocket.Close();
                            m_ClientSocket = null;

                            m_UserConfigManage = null;

                            break;
                        }

                        //记录收到的数据包日志
                        //Log.Record("client.log", "收到客户端数据包：", recbuf, rcount);


                        //判断是否收到了初次登录标志，如果是则回复欢迎信息框
                        if (rcount > 7)
                        {
                            //0x1, 0xF9, 0x0, 0x44, 0x0, 0x1, 0x1B
                            if ((received[0] == 0x01) && (received[1] == 0xF9) && (received[2] == 0x00) && (received[3] == 0x44) && (received[4] == 0x00)
                                && (received[5] == 0x01) && (received[6] == 0x1B))
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "接收到帐号：" + UserName + " 的初次登录数据包，回复欢迎信息框...");
                                }

                                //回复登录成功欢迎提示框
                                //发送成功提示框信息
                                //其中0x00,0x0E为发送的总字节数
                                byte[] sendbuf9 = { 0x01, 0xF8, 0x00, 0x0E };
                                byte[] sendbuf10 = Encoding.Default.GetBytes("登录成功！");
                                ArrayList b = new ArrayList();
                                b.AddRange(sendbuf9);
                                b.AddRange(sendbuf10);
                                m_ClientSocket.Send((byte[])b.ToArray(typeof(byte)));

                                string strLimitcount = "";
                                if (UserConfigManage.User_LimitFlag)
                                {
                                    strLimitcount = "您帐号剩余:" + (UserConfigManage.User_LimitCount - UserConfigManage.User_SendCount).ToString() + "条指令.";
                                }

                                //发送欢迎及剩余指令数量信息
                                SendClientCmd("", "欢迎使用PID服务软件!\r" + strLimitcount + "\r您帐号的到期时间: " + UserConfigManage.User_EndDate.ToString("yyyy-MM-dd") + ".", 1);

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复成功数据包", (byte[])b.ToArray(typeof(byte)), sendbuf9.Length + sendbuf10.Length);
                                }

                                continue;
                            }

                            //Kevin 2010-10-20 Edit
                            //连续接收3次
                            if ((received[0] == 0x01) && (received[1] == 0xFE))
                            {

                                if (((received[7] == 0x02) && (received[8] == 0x29)) ||
                                    ((rcount == 34) && (received[7] == 0x02) && (received[8] == 0x0C)) ||
                                    (rcount == 51))
                                {
                                    //Log.Record("client.log", "收到客户端数据包：", received, rcount);

                                    byte[] sendbuf8 = { 0x01, 0xFD, 0x00, 0x06, 0x00, 0x00, 0x01, 0xFD, 0x00, 0x06, 0x00, 0x0C };
                                    byte[] sendbuf9 = { 0x01, 0xFD, 0x00, 0x06, 0x00, 0x29 };
                                    if (!m_SendSucFlag)
                                    {
                                        m_SendSucFlag = true;
                                        m_ClientSocket.Send(sendbuf8);

                                        if (DebugDataFlag)
                                        {
                                            Log.Record("client.log", "向帐号：" + UserName + " 发送数据包", sendbuf8, sendbuf8.Length);
                                        }

                                        m_ClientSocket.Send(sendbuf9);

                                        if (DebugDataFlag)
                                        {
                                            Log.Record("client.log", "向帐号：" + UserName + " 发送数据包", sendbuf9, sendbuf9.Length);
                                        }

                                        //记录发送信息
                                        //Log.Record("client.log", "向客户端发送数据包：", sendbuf8, sendbuf8.Length);
                                    }
                                }

                                continue;
                            }
                        }

                        #region 收到心跳包并回复心跳包
                        //如果长度为5，则为心跳包，回复心跳包
                        if (rcount == 5)
                        {
                            //Log.Record("client.log", "收到客户端的数据包为心跳包");
                            m_ClientSocket.Send(m_heartbuf);

                            if (DebugFlag)
                            {
                                Log.Record("client.log", "收到帐号：" + UserName + "的心跳包并回复心跳包...");
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("client.log", "向帐号：" + UserName + " 发送心跳包", m_heartbuf, m_heartbuf.Length);
                            }

                            continue;
                        }
                        #endregion 收到心跳包并回复心跳包

                        //行程单创建指令
                        #region 行程单创建指令并回发结果
                        byte[] tmpprintbuf = new byte[5];
                        if (rcount > 24)
                        {
                            Array.Copy(recbuf, 19, tmpprintbuf, 0, 5);
                        }
                        string strprint = Encoding.Default.GetString(tmpprintbuf);
                        if ((strprint.Trim().ToLower() == "prinv") || (strprint.Trim().ToLower() == "vtinv"))
                        {
                            content = AnalyseClientContent(recbuf, rcount);
                            Log.Record("client.log", "收到行程单操作指令：" + content);

                            //查询可用配置

                            //开始寻找时间
                            DateTime begindate = DateTime.Now;  //m_maxOutTimes
                            bool findflag = false;
                            int EtermId = -1;//可用配置的id
                            string EtermName = "";//可用配置的名称

                            //如果没有找到可用配置，并且等待时间未超过最大超时时间，则继续循环
                            while ((!findflag) && (PublicInfo.DateDiff(DateTime.Now, begindate) < m_MaxOutTimes))
                            {
                                findflag = m_ServerManage.GetIdleEtermInfo(strprint, m_UserConfigManage.User_Office, ref EtermId,ref EtermName);
                            }

                            #region 未找到可用配置
                            //未找到可用配置，时间已过最大超时时间
                            if (!findflag)
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "帐号：" + UserName + " 发送行程单打印指令未找到可用配置...");
                                }

                                byte[] errorbuf1 ={0x01, 0x00, 0x00, 0x2E, 0x0C, 0x00, 0x00, 0x01, 0x8C, 0x0C, 0x00, 0x02, 0x5B,
                                    0x49, 0x74, 0x69, 0x6E, 0x65, 0x72, 0x61, 0x72, 0x79, 0x5F, 0x49, 0x4E, 0x56, 0x3A, 0x20};
                                byte[] errorbuf2 = { 0x5D, 0x03 };

                                byte[] errorbuf3 = Encoding.Default.GetBytes("Please Try Again");


                                //Kevin 2011-03-25 Add
                                //替换数据包长度字节
                                short tmplen = (short)(errorbuf1.Length + errorbuf2.Length + errorbuf3.Length);
                                short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                                byte[] errorlenbuf = BitConverter.GetBytes(count2);

                                Array.Copy(errorlenbuf, 0, errorbuf1, 2, 2);

                                ArrayList al = new ArrayList();
                                al.AddRange(errorbuf1);
                                al.AddRange(errorbuf2);
                                al.AddRange(errorbuf3);

                                m_ClientSocket.Send((byte[])(al.ToArray(typeof(byte))));

                                //已回复标志
                                SendFlag = true;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包", (byte[])(al.ToArray(typeof(byte))),
                                        tmplen);
                                }

                                //组织请求内容
                                tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + m_UserName + " 发送指令]：\r" + content + "\r";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + m_UserName + " 收到" + EtermName + "返回结果]：\r未找到可用配置"+m_UserConfigManage.User_Office+"...\r";
                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion 未找到可用配置

                            if (DebugFlag)
                            {
                                Log.Record("client.log", "帐号：" + UserName + " 发送行程单打印指令已找到可用配置，准备发送打印指令...");
                            }

                            //返回结果数据包
                            byte[] resbuf = null;
                            //错误信息
                            string ErrorMessage = "";
                            //返回结果标志
                            bool ResFlag = false;
                            //行程单创建（信天游配置不能用）
                            ResFlag = m_ServerManage.SendCommandAndGetResult(m_UserName, content, recbuf, m_UserConfigManage.User_Office, false, EtermId, ref resbuf, ref ErrorMessage);

                            if (DebugFlag)
                            {
                                Log.Record("client.log", "帐号：" + UserName + " 发送行程单打印指令并接收到返回信息");
                            }

                            #region 打印行程单指令处理出错
                            if (!ResFlag)
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "帐号：" + UserName + " 向"+EtermName+"发送行程单打印指令处理失败，错误消息：" + ErrorMessage);
                                }

                                //返回提示：没有执行此指令的权限
                                //返回错误，等待超时
                                //组织回复数据
                                MakeSendBufByContent(ErrorMessage, out SendBuf);

                                m_ClientSocket.Send(SendBuf);

                                //已回复标志
                                SendFlag = true;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包：", SendBuf, SendBuf.Length);
                                }

                                //组织请求内容
                                tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + m_UserName + " 发送指令]：\r" + content + "\r";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + m_UserName + " 收到"+EtermName+"返回结果]：\r" + ErrorMessage+"\r";
                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }
                                continue;
                            }
                            #endregion 打印行程单指令处理出错
                            #region 打印行程单处理成功返回结果
                            else
                            {
                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "收到" + EtermName + "行程单创建返回结果：", resbuf, resbuf.Length);
                                }
                                string rescontent = AnalyseServerContent(resbuf, resbuf.Length);

                                //创建失败
                                if (rescontent.ToLower().IndexOf("<flag>e</flag>") != -1)
                                {
                                    int index1 = rescontent.ToLower().IndexOf("<errorreason>");
                                    int index2 = rescontent.ToLower().IndexOf("</errorreason>");
                                    if ((index1 != -1) && (index2 != -1))
                                    {
                                        rescontent = "ERROR:" + rescontent.Substring(index1 + 13, index2 - index1 - 13).Replace("！", "!").Replace("，", ",");
                                    }
                                    else
                                    {
                                        rescontent = "ERROR";
                                    }
                                }
                                //创建成功
                                else if (rescontent.ToLower().IndexOf("<flag>s</flag>") != -1)
                                {
                                    rescontent = "SUCCESS";
                                }

                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复行程单指令结果数据包...");
                                }

                                SendClientCmd(strprint, rescontent, 1);

                                //m_ClientSocket.Send(resbuf);

                                //已回复标志
                                SendFlag = true;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包：", SendBuf, SendBuf.Length);
                                }

                                //组织请求内容
                                tmpstr = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + m_UserName + " 发送指令]：\r" + content + "\r";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + m_UserName + " 收到" + EtermName + "返回结果]：\r" + rescontent+"\r";
                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion 打印行程单处理成功返回结果
                        }
                        #endregion 行程单创建指令并回发结果


                        //判断是否为指令信息
                        //如果不是指令信息则继续
                        byte[] tmpbuf = new byte[2];
                        Array.Copy(recbuf, 8, tmpbuf, 0, 2);

                        //收到指令信息
                        if ((tmpbuf[0] == cmdhead1[0]) && (tmpbuf[1] == cmdhead1[1]))
                        {
                            //客户端校验城市及日期数据指令
                            if (recbuf[4] == 0xF0 && recbuf[5] == 0x01)
                            {
                                //分析收到的客户端内容（可以在此函数中区分eterm客户端指令和网站指令）
                                //格式  城市|城市|日期
                                content = AnalyseClientContent(recbuf, rcount);
                                string[] sl=content.Split('|');
                                if (sl.Length != 3)
                                {
                                    //格式错误
                                    SendClientCmd(content, "格式错误！", 1);

                                    //已回复标志
                                    SendFlag = true;

                                    continue;
                                }
                                else
                                {
                                    string fromcitycode = "";
                                    string fromcityname = "";
                                    FindCityCode(sl[0], ref fromcitycode, ref fromcityname);
                                    if (fromcitycode == "")
                                    {
                                        //格式错误
                                        SendClientCmd(content, "未找到对应的出发城市！", 1);

                                        //已回复标志
                                        SendFlag = true;

                                        continue;
                                    }
                                    string tocitycode = "";
                                    string tocityname = "";
                                    FindCityCode(sl[1],ref tocitycode,ref tocityname);
                                    if (tocitycode == "")
                                    {
                                        //格式错误
                                        SendClientCmd(content, "未找到对应的到达城市！", 1);

                                        //已回复标志
                                        SendFlag = true;

                                        continue;
                                    }
                                    //判断日期是否合法
                                    DateTime tmpdate = DateTime.Parse(sl[2]).AddDays(1);
                                    if(tmpdate.CompareTo(DateTime.Now)<0)
                                    {
                                        //日期不合法
                                        SendClientCmd(content, "航班日期不能早于当前日期！", 1);

                                        //已回复标志
                                        SendFlag = true;

                                        continue;
                                    }

                                    SendClientCmd(content, fromcitycode + "|"+fromcityname+"|" + tocitycode +"|"+tocityname+ "|" + sl[2], 1);

                                    //已回复标志
                                    SendFlag = true;

                                    continue;     

                                }
                            }

                            //判断当前帐号指令条数是否达到上限
                            if (UserConfigManage.User_LimitFlag)
                            {
                                if (UserConfigManage.User_LimitCount <= (m_SendCount + UserConfigManage.User_SendCount))
                                {
                                    //已经超出最大可用条数
                                    if (DebugFlag)
                                    {
                                        Log.Record("client.log", "向帐号：" + UserName + " 回复结果：已经超出最大可用指令条数...");
                                    }

                                    SendClientCmd(content, "已经超出最大可用指令条数!", 1);

                                    //已回复标志
                                    SendFlag = true;

                                    continue;
                                }
                            }

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

                            //分析收到的客户端内容（可以在此函数中区分eterm客户端指令和网站指令）
                            content = AnalyseClientContent(recbuf, rcount);

                            if (DebugFlag)
                            {
                                Log.Record("client.log", "收到帐号：" + UserName + " 发送指令信息：" + content);
                            }

                            #region I指令或者封口指令，则清空历史指令记录并直接返回NO PNR
                            if ((content.Trim().ToLower() == "i") || (content.Trim().ToLower() == "\\") ||
                                (content.Trim().ToLower() == "@") || (content.Trim().ToLower() == "ig"))
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复结果：NO PNR");
                                }

                                //返回提示：NO PNR
                                SendClientCmd(content, "NO PNR", 1);

                                //已回复标志
                                SendFlag = true;

                                //清空历史指令记录
                                m_SendCommandList.Clear();

                                //组织请求内容
                                string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\n" + content + "\n";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                    " 返回结果]：\nNO PNR";

                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr1);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion I指令或者封口指令，则清空历史指令记录并直接返回NO PNR

                            #region RT指令没有带编码，直接返回不支持
                            if (content.ToUpper().Trim() == "RT")
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复结果：系统不支持直接RT,请加PNR编号提取内容信息...");
                                }

                                //返回提示：没有执行此指令的权限
                                SendClientCmd(content, "系统不支持直接RT,请加PNR编号提取内容信息!", 1);

                                //已回复标志
                                SendFlag = true;

                                //组织请求内容
                                string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\n" + content + "\n";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                    " 返回结果]：\n系统不支持直接RT,请加PNR编号提取内容信息!";

                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr1);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion RT指令没有带编码，直接返回不支持

                            #region 检查用户指令权限
                            //Kevin 2009-10-25 Edit
                            //检查用户权限
                            if (!CheckUserCmdPower(content))
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复结果：没有执行此指令的权限...");
                                }

                                SendClientCmd(content, "没有执行此指令的权限!", 1);

                                //已回复标志
                                SendFlag = true;

                                //组织请求内容
                                string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\n" + content + "\n";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                    " 返回结果]：\n没有执行此指令的权限!";

                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr1);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion 检查用户指令权限

                            #region 屏蔽，测试内容（模拟返回信息）
                            ////测试
                            //byte[] tbuf = new byte[rcount - 21];
                            //Array.Copy(recbuf, 19, tbuf, 0, rcount - 21);
                            //Log.Record("Client.log", "buf", tbuf, rcount - 21);
                            //Log.Record("client.log", "分析收到的内容(EtermClient)：" + content);

                            //byte[] m_outtimebuf=null;
                            ////测试返回
                            ////返回错误，等待超时
                            //if (m_outtimebuf == null)
                            //{
                            //    //组织回复数据
                            //    MakeSendBufByContent(content, out m_outtimebuf);
                            //}
                            //m_ClientSocket.Send(m_outtimebuf);
                            //continue;
                            #endregion 屏蔽，测试内容（模拟返回信息）

                            string SendCommand = content;
                            #region 对指令做上下文指令处理
                            SendCommand = DoWithUpDownCommand(content);
                            #endregion 对指令做上下文指令处理

                            if (DebugFlag)
                            {
                                Log.Record("client.log", "对帐号：" + UserName + " 发送指令做上下文处理后，待发送指令为：" + SendCommand + "...");
                            }

                            #region 组织待发送数据包

                            AnalyseWebCmdAndMakeServerInfo(Encoding.Default.GetBytes(SendCommand), out SendBuf, false);

                            //组织发送数据包
                            byte[] tmpbuf1 = { 0x01, 0x00, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x01, 0x36, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x21, 0x20, 0x00, 0x0F, 0x1E };
                            byte[] tmpbuf2 = { 0x20, 0x03 };
                            ArrayList al = new ArrayList();
                            short tmplen = 0;
                            short count2 = 0;
                            byte[] lenbuf = null;

                            //替换数据包长度字节
                            tmplen = (short)(21 + SendBuf.Length);
                            count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, tmpbuf1, 2, 2);

                            al.AddRange(tmpbuf1);
                            al.AddRange(SendBuf);
                            al.AddRange(tmpbuf2);

                            if (DebugDataFlag)
                            {
                                Log.Record("client.log", "帐号：" + UserName + " 待发送指令数据包：", (byte[])al.ToArray(typeof(byte)), tmplen);
                            }

                            #endregion 组织待发送数据包


                            #region 寻找可用配置
                            //开始寻找时间
                            DateTime begindate = DateTime.Now;  //m_maxOutTimes
                            bool findflag = false;
                            int EtermId = -1;//可用配置的ID
                            string EtermName = "";//可用配置的名称

                            //如果没有找到可用配置，并且等待时间未超过最大超时时间，则继续循环
                            while ((!findflag) && (PublicInfo.DateDiff(DateTime.Now, begindate) < m_MaxOutTimes))
                            {
                                findflag = m_ServerManage.GetIdleEtermInfo(strprint, m_UserConfigManage.User_Office, ref EtermId, ref EtermName);
                            }

                            //未找到可用配置，时间已过最大超时时间
                            if (!findflag)
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "帐号：" + UserName + " 未找到可用配置...");
                                }

                                SendClientCmd(SendCommand, "发送指令超时,请重试!", 1);

                                //已回复标志
                                SendFlag = true;

                                //组织请求内容
                                string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + SendCommand + "\r";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                    " 返回结果]：\r未找到可用配置" + m_UserConfigManage.User_Office + "...\r";

                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr1);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion 寻找可用配置

                            byte[] ResBuf = null;
                            string ErrorMessage = "";
                            bool ResFlag = false;
                            #region 发送指令并获取返回结果信息
                            ResFlag = m_ServerManage.SendCommandAndGetResult(m_UserName, SendCommand, (byte[])al.ToArray(typeof(byte)), m_UserConfigManage.User_Office, false, EtermId, ref ResBuf, ref ErrorMessage);
                            #endregion 发送指令并获取返回结果信息

                            #region 返回结果出错
                            if (!ResFlag)
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "帐号：" + UserName + " 发送指令操作出错，错误消息：" + ErrorMessage + "...");
                                }

                                SendClientCmd(SendCommand, ErrorMessage, 1);

                                //已回复标志
                                SendFlag = true;

                                //组织请求内容
                                string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + SendCommand + "\r";
                                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                    " 收到" + EtermName + "返回结果]：\r" + ErrorMessage + "\r";

                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr1);
                                        m_ArrayContents.Add(tmpstr2);
                                    }
                                }

                                continue;
                            }
                            #endregion 返回结果出错
                            #region 返回结果成功
                            else
                            {
                                //增加指令计数，每次计数2条指令（发收）
                                m_SendCount += 2;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "收到" + EtermName + "返回结果：", ResBuf, ResBuf.Length);
                                }

                                //替换黑屏包头
                                Array.Copy(cmdhead1, 0, ResBuf, 8, 2);

                                m_ClientSocket.Send(ResBuf);

                                //已回复标志
                                SendFlag = true;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包：", ResBuf, ResBuf.Length);
                                }

                                string tmpResMes = AnalyseServerContent(ResBuf, ResBuf.Length);

                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "帐号：" + UserName + " 发送指令成功取得返回结果：" + tmpResMes);
                                }

                                //组织请求内容
                                string tmpstr2 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + SendCommand + "\r";
                                string tmpstr3 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 收到" + EtermName + "返回结果]：\r" + tmpResMes;
                                if (m_ArrayContents != null)
                                {
                                    lock (m_ArrayContents)
                                    {
                                        m_ArrayContents.Add(tmpstr2);
                                        m_ArrayContents.Add(tmpstr3);
                                    }
                                }

                                //把指令添加到指令列表中
                                m_SendCommandList.Add(SendCommand);

                                continue;
                            }
                            #endregion 返回结果成功
                        }
                        else
                        {
                            //其他指令直接转发

                            #region 寻找可用配置
                            //开始寻找时间
                            DateTime begindate = DateTime.Now;  //m_maxOutTimes
                            bool findflag = false;
                            int EtermId = -1;//可用配置的ID
                            string EtermName = "";//可用配置的名称

                            //如果没有找到可用配置，并且等待时间未超过最大超时时间，则继续循环
                            while ((!findflag) && (PublicInfo.DateDiff(DateTime.Now, begindate) < m_MaxOutTimes))
                            {
                                findflag = m_ServerManage.GetIdleEtermInfo(strprint, m_UserConfigManage.User_Office, ref EtermId, ref EtermName);
                            }

                            //未找到可用配置，时间已过最大超时时间
                            if (!findflag)
                            {
                                if (DebugFlag)
                                {
                                    Log.Record("client.log", "帐号：" + UserName + " 未找到可用配置...");
                                }

                                //SendClientCmd(SendCommand, "发送指令超时,请重试!", 1);

                                //已回复标志
                                //SendFlag = true;

                                ////组织请求内容
                                //string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + SendCommand + "\r";
                                //string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                //    " 返回结果]：\r未找到可用配置" + m_UserConfigManage.User_Office + "...\r";

                                //if (m_ArrayContents != null)
                                //{
                                //    lock (m_ArrayContents)
                                //    {
                                //        m_ArrayContents.Add(tmpstr1);
                                //        m_ArrayContents.Add(tmpstr2);
                                //    }
                                //}

                                continue;
                            }
                            #endregion 寻找可用配置

                            byte[] ResBuf = null;
                            int ResCount = -1;
                            string ErrorMessage = "";
                            bool ResFlag = false;
                            #region 发送指令并获取返回结果信息
                            ResFlag = m_ServerManage.SendCmdDirectAndGetResult(EtermId,m_UserName, recbuf, ref ResBuf, ref ResCount);
                            #endregion 发送指令并获取返回结果信息

                            #region 返回结果出错
                            if (!ResFlag)
                            {
                                //if (DebugFlag)
                                //{
                                //    Log.Record("client.log", "帐号：" + UserName + " 发送指令操作出错，错误消息：" + ErrorMessage + "...");
                                //}

                                //SendClientCmd(SendCommand, ErrorMessage, 1);

                                ////已回复标志
                                //SendFlag = true;

                                ////组织请求内容
                                //string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + SendCommand + "\r";
                                //string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                                //    " 收到" + EtermName + "返回结果]：\r" + ErrorMessage + "\r";

                                //if (m_ArrayContents != null)
                                //{
                                //    lock (m_ArrayContents)
                                //    {
                                //        m_ArrayContents.Add(tmpstr1);
                                //        m_ArrayContents.Add(tmpstr2);
                                //    }
                                //}

                                continue;
                            }
                            #endregion 返回结果出错
                            #region 返回结果成功
                            else
                            {
                                //增加指令计数，每次计数2条指令（发收）
                                m_SendCount += 2;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "收到" + EtermName + "返回结果：", ResBuf, ResCount);
                                }

                                //替换黑屏包头
                               // Array.Copy(cmdhead1, 0, ResBuf, 8, 2);

                                byte[] buf = new byte[ResCount];
                                Array.Copy(ResBuf, buf, ResCount);
                                m_ClientSocket.Send(buf);

                                //已回复标志
                                SendFlag = true;

                                if (DebugDataFlag)
                                {
                                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包：", ResBuf, ResCount);
                                }

                                //string tmpResMes = AnalyseServerContent(ResBuf, ResBuf.Length);

                                //if (DebugFlag)
                                //{
                                //    Log.Record("client.log", "帐号：" + UserName + " 发送指令成功取得返回结果：" + tmpResMes);
                                //}

                                //组织请求内容
                                //string tmpstr2 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + SendCommand + "\r";
                                //string tmpstr3 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 收到" + EtermName + "返回结果]：\r" + tmpResMes;
                                //if (m_ArrayContents != null)
                                //{
                                //    lock (m_ArrayContents)
                                //    {
                                //        m_ArrayContents.Add(tmpstr2);
                                //        m_ArrayContents.Add(tmpstr3);
                                //    }
                                //}

                                ////把指令添加到指令列表中
                                //m_SendCommandList.Add(SendCommand);

                                continue;
                            }
                            #endregion 返回结果成功
                        }

                        //记录收到数据包信息
                        //Log.Record("client.log", "收到客户端数据包：", received, rcount);
                    }
                }
                catch (Exception ex)
                {
                    if (!SendFlag)
                    {
                        //返回提示：没有执行此指令的权限
                        SendClientCmd(content, "发送指令出错!", 1);

                        //组织请求内容
                        string tmpstr1 = ReceiveTime.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\r" + content + "\r";
                        string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName +
                            " 返回结果]：\r发送指令出错!\r";

                        if (m_ArrayContents != null)
                        {
                            lock (m_ArrayContents)
                            {
                                m_ArrayContents.Add(tmpstr1);
                                m_ArrayContents.Add(tmpstr2);
                            }
                        }
                    }

                    //释放占用配置
                    if (EtermId != -1)
                    {
                        ServerManage.FreeEtermInfo(EtermId);
                    }

                    Log.Record("client.log", "帐号：" + UserName + " 处理指令操作出错，错误信息：" + ex.Message);

                    if (m_ClientSocket == null)
                    {
                        m_UserConfigManage = null;
                        break;
                    }
                }
            }
        }

        #region 获取城市三字码
        public bool FindCityCode(string value,ref string citycode,ref string cityname)
        {
            citycode = "";
            cityname = "";
            foreach (Base_CityInfoManage tmpCityInfo in BaseCityInfoManageList)
            {
                if (tmpCityInfo.City_Code.ToLower() == value.ToLower())
                {
                    citycode = tmpCityInfo.City_Code;
                    cityname = tmpCityInfo.City_Name;
                    return true;
                }

                if (tmpCityInfo.City_Name.ToLower() == value.ToLower())
                {
                    citycode = tmpCityInfo.City_Code;
                    cityname = tmpCityInfo.City_Name;
                    return true;
                }

                if (tmpCityInfo.City_QuanPin.ToLower() == value.ToLower())
                {
                    citycode = tmpCityInfo.City_Code;
                    cityname = tmpCityInfo.City_Name;
                    return true;
                }
            }
            return false;
        }
        #endregion 获取城市三字码


        #region 做指令的上下文处理
        public string DoWithUpDownCommand(string sendcmd)
        {
            string strResult = "";

            //如果发送指令为 I 或者 RTXXXXXX，则清空历史指令列表并返回原指令内容
            if ((sendcmd.ToLower() == "i") || (sendcmd.ToLower() == "\\") || (sendcmd.ToLower() == "@") || (sendcmd.ToLower() == "ig") ||
                ((sendcmd.Length > 2) && (sendcmd.ToLower().Substring(0, 2) == "rt")))
            {
                m_SendCommandList.Clear();
                return sendcmd;
            }

            //for (int i = 0; i < m_SendCommandList.Count; i++)
            //{
            //    strResult += m_SendCommandList[i].ToString() + "|";
            //    //添加上下文处理操作
            //    //for (int j = 0; j < m_BaseCommandManageList.Count; j++)
            //    //{

            //    //}
            //}

            return strResult + sendcmd;
        }
        #endregion 做指令的上下文处理

        #region 检查用户指令权限
        /// <summary>
        /// 检查用户指令权限
        /// </summary>
        /// <param name="content">指令内容</param>
        /// <returns>true:有权限；false:无权限</returns>
        private bool CheckUserCmdPower(string content)
        {
            bool flag = true;
            int pos = -1;
            string tmpcmd = "";
            string tmpcmd2 = "";

            ////判断
            string[] disCmdList = m_UserConfigManage.User_DisableCmd.Split(new char[] { ',' });

            for (int i = 0; i < disCmdList.Length; i++)
            {
                if (disCmdList[i].Trim() == "")
                {
                    continue;
                }

                //判断开头是否有禁用指令
                //判断是否换行指令的开头有禁用指令
                //判断组合指令的开头有禁用指令
                if ((disCmdList[i].Trim().ToUpper() == content.Trim().ToUpper().Substring(0, disCmdList[i].Trim().Length)) ||
                    (content.Trim().ToUpper().IndexOf("|" + disCmdList[i].Trim().ToUpper()) != -1) ||
                    (content.Trim().ToUpper().IndexOf("\r" + disCmdList[i].Trim().ToUpper()) != -1))
                {
                    //无权限
                    return false;
                }
            }

            //有权限
            return true;
        }
        #endregion 检查用户指令权限

        #region 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// <summary>
        /// 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// </summary>
        /// <param name="sourbuf">原数据包</param>
        /// <param name="resbuf">结果数据包</param>
        /// <param name="needpinyin">是否需要拼音</param>
        private void AnalyseWebCmdAndMakeServerInfo(byte[] sourbuf, out byte[] resbuf, bool needpinyin)
        {
            resbuf = null;

            //先把字节包转换为字符串
            string tmpcontent = Encoding.Default.GetString(sourbuf);

            //源字符串长度
            int count = tmpcontent.Length;

            //序号
            int index = 0;

            //汉字的起始、结束字节标志
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };

            int code = 0;
            byte tmpbyte;

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);

            bool isChinese = false;//是否汉字

            //临时存储连续的汉字
            string tmpstr = "";
            string tmppinyin = "";//拼音

            ArrayList al = new ArrayList();

            PublicInfo thepub = new PublicInfo();

            for (int i = 0; i < count; i++)
            {
                //获得字符串中指定索引i处字符unicode编码
                code = Char.ConvertToUtf32(tmpcontent, i);
                if (code >= chfrom && code <= chend)
                {
                    tmpstr += tmpcontent[i];
                    isChinese = true;     //当code在中文范围内
                }
                else
                {
                    #region 汉字
                    if (isChinese)
                    {
                        isChinese = false;
                        if (tmpstr.Length != 0)
                        {
                            //取得汉字的拼音

                            //暂时存储汉字编码
                            ArrayList al2 = new ArrayList();

                            for (int j = 0; j < tmpstr.Length; j++)
                            {
                                byte[] hanziBt;
                                tmppinyin = GetPinYinBianMaFromHanZi(tmpstr.Substring(j, 1), out hanziBt);//chs2py.convert(tmpstr.Substring(j, 1), out hanziBt);

                                if (needpinyin)
                                {
                                    al.AddRange(Encoding.Default.GetBytes(tmppinyin));
                                }

                                //如果没有找到汉字编码，则使用转换方法
                                if (hanziBt == null)
                                {
                                    //转换临时存储的汉字并添加到ArrayList
                                    byte[] tmpbuf = Encoding.Default.GetBytes(tmpstr.Substring(j, 1));
                                    index += 2;// tmpbuf.Length;
                                    for (int k = 0; k < tmpbuf.Length; k++)
                                    {
                                        tmpbyte = (byte)(tmpbuf[k] - 0x80);
                                        al2.Add(tmpbyte);
                                    }
                                }
                                else
                                {
                                    al2.AddRange(hanziBt);
                                    index += 2;//hanziBt.Length;
                                }
                            }

                            al.AddRange(beginbuf);

                            al.AddRange(al2);

                            al.AddRange(endbuf);
                            tmpstr = "";
                        }
                    }
                    #endregion
                    //当code不在中文范围内
                    tmpbyte = sourbuf[index];
                    index++;
                    al.Add(tmpbyte);
                }
            }

            if ((isChinese) && (tmpstr.Length != 0))
            {
                //暂时存储汉字编码
                ArrayList al2 = new ArrayList();

                for (int j = 0; j < tmpstr.Length; j++)
                {
                    byte[] hanziBt;
                    tmppinyin = GetPinYinBianMaFromHanZi(tmpstr.Substring(j, 1), out hanziBt);//chs2py.convert(tmpstr.Substring(j, 1), out hanziBt);

                    if (needpinyin)
                    {
                        al.AddRange(Encoding.Default.GetBytes(tmppinyin));
                    }

                    //如果没有找到汉字编码，则使用转换方法
                    if (hanziBt == null)
                    {
                        //转换临时存储的汉字并添加到ArrayList
                        byte[] tmpbuf = Encoding.Default.GetBytes(tmpstr.Substring(j, 1));
                        index += tmpbuf.Length;
                        for (int k = 0; k < tmpbuf.Length; k++)
                        {
                            tmpbyte = (byte)(tmpbuf[k] - 0x80);
                            al2.Add(tmpbyte);
                        }
                    }
                    else
                    {
                        al2.AddRange(hanziBt);
                        index += hanziBt.Length;
                    }
                }

                al.AddRange(beginbuf);

                al.AddRange(al2);

                al.AddRange(endbuf);
            }

            resbuf = (byte[])al.ToArray(typeof(byte));
        }
        #endregion 把网络的命令包转换为航信命令数据包，主要是处理中文

        #region 把字符串组织成回复格式字节数组（不需要拼音）
        /// <summary>
        /// 把字符串组织成回复格式字节数组（不需要拼音）
        /// </summary>
        /// <param name="sendcontent">需要转换的字符串</param>
        /// <param name="sendbuf">组织好的数据包</param>
        private void MakeSendBufByContent(string sendcontent, out byte[] sendbuf)
        {
            byte[] bufcontent = Encoding.Default.GetBytes(sendcontent);
            sendbuf = null;
            byte[] contentbuf = null;
            AnalyseWebCmdAndMakeServerInfo(bufcontent, out contentbuf, false);
            ArrayList al = new ArrayList();
            byte[] head1 = { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x20, 0x20, 0x0F, 0x1B, 0x4D };
            byte[] head2 = { 0x0D, 0x1E, 0x1B, 0x62, 0x03 };

            //替换数据包长度字节
            short tmplen = (short)(contentbuf.Length + head1.Length + head2.Length);
            short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
            byte[] lenbuf = BitConverter.GetBytes(count2);

            Array.Copy(lenbuf, 0, head1, 2, 2);

            head1[14] = (byte)((int)recno + 1);

            al.AddRange(head1);
            al.AddRange(contentbuf);
            al.AddRange(head2);
            sendbuf = (byte[])al.ToArray(typeof(byte));
        }
        #endregion 把字符串组织成回复格式字节数组（不需要拼音）

        #region 解析收到的客户端信息，返回收到内容的字符串
        /// <summary>
        /// 解析收到的客户端信息，返回收到内容的字符串
        /// </summary>
        /// <param name="buf">收到的数据包</param>
        /// <param name="count">数据包的长度</param>
        /// <returns>数据包的解析结果字符串</returns>
        private string AnalyseClientContent(byte[] buf, int count)
        {
            string result = "";

            //主要是解析中文
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };
            ArrayList al = new ArrayList();

            int index = 0;
            int tmpcount = 0;
            bool isChinese = false;//是否为汉字
            while (index < count)
            {
                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0E))
                {
                    //汉字开始标志
                    index += 2;
                    isChinese = true;
                    continue;
                }

                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0F))
                {
                    //汉字结束
                    index += 2;
                    isChinese = false;
                    continue;
                }

                byte tmpbyte = buf[index];

                //如果是汉字则处理
                if (isChinese)
                {
                    byte[] cbuf = null;
                    //Kevin 2010-06-28 Edit
                    //对于汉字从编码表中解析再转换
                    if (buf[index] == 0x78)
                    {
                        cbuf = new byte[4];
                        cbuf[0] = (byte)(buf[index]);
                        cbuf[1] = (byte)(buf[index + 1]);
                        cbuf[2] = (byte)(buf[index + 2]);
                        cbuf[3] = (byte)(buf[index + 3]);
                        index += 2;
                    }
                    else
                    {
                        cbuf = new byte[2];
                        cbuf[0] = (byte)(buf[index]);
                        cbuf[1] = (byte)(buf[index + 1]);
                    }

                    string tmpcontent = GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);

                    //把转换过的汉字追加到al中
                    al.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    index++;
                }
                else
                {
                    al.Add(tmpbyte);
                }
                tmpcount++;
                index++;
            }

            //kevin 2010-06-25 Add
            //处理21字节的内容
            if (count == 21)
            {
                al.Add(buf[18]);
                return ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 0, 1);
            }


            //Kevin 2010-06-25 Add
            //处理快捷键指令信息，
            //F1(da)，F2(gi)，F3(qt)，F4(\)，F5(PN)，F6(PB)，
            //F7（PF），F8（PL），F9(QD)
            if (count == 13)
            {
                switch (buf[11])
                {
                    //F5
                    case 0x20:
                        return "PN";
                        break;

                    //F6
                    case 0x21:
                        return "PB";
                        break;

                    //F1
                    case 0x37:
                        return "da";
                        break;

                    //F2
                    case 0x47:
                        return "gi";
                        break;

                    //F3
                    case 0x57:
                        return "qt";
                        break;

                    //F4
                    case 0x67:
                        return "\\";
                        break;

                    //F7
                    case 0x22:
                        return "PF";
                        break;

                    //F8
                    case 0x23:
                        return "PL";
                        break;

                    //F9
                    case 0x24:
                        return "qd";
                        break;
                    default:
                        return "i";
                        break;
                }
            }
            else
            {
                //Kevin 2010-06-26 Add
                if (buf.Length < 21)
                {
                    return "i";
                }

                if (buf[18] != 0x1e)
                {
                    //从第20位开始，读取到末尾倒数第三位
                    result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 18, al.Count - 20);
                }
                else
                {
                    //从第20位开始，读取到末尾倒数第三位
                    result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, al.Count - 21);
                }
            }

            return result;
        }
        #endregion 解析收到的客户端信息，返回收到内容的字符串

        #region 向客户端回传指令
        /// <summary>
        /// 向客户端回传指令
        /// </summary>
        /// <param name="cmd">回传信息字符串</param>
        /// <param name="rescontent">回复内容</param>
        /// <param name="type">类型  1:标准回复信息  2：错误对话框</param>
        public void SendClientCmd(string cmd, string rescontent, int type)
        {
            if (type == 1)
            {
                byte[] m_ResBuf = new byte[m_MaxPacket];
                //返回提示：没有执行此指令的权限
                //返回错误，等待超时
                //组织回复数据
                MakeSendBufByContent(rescontent, out m_ResBuf);
               
                if (m_ClientSocket == null)
                {
                    return;
                }

                try
                {
                    m_ClientSocket.Send(m_ResBuf);
                }
                catch (Exception ex)
                {
                    m_ClientSocket = null;
                }

                if (DebugDataFlag)
                {
                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包：", m_ResBuf, m_ResBuf.Length);
                }

                //组织请求内容
                string tmpstr = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 发送指令]：\n" + cmd + "\n";
                string tmpstr2 = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss ") + UserName + " 返回结果]：\n" + rescontent+"\n";
                if (m_ArrayContents != null)
                {
                    lock (m_ArrayContents)
                    {
                        m_ArrayContents.Add(tmpstr);
                        m_ArrayContents.Add(tmpstr2);
                    }
                }
                // m_thread.Abort();                
            }
            else
            {
                //失败对话框的头结构
                byte[] sendbuf = { 0x00, 0x37, 0x00, 0x31, 0x30, 0x30, 0x30, 0x31, 0x3A };
                ArrayList a = new ArrayList();

                //记录日志
                //Log.Record("client.log", tmpuser + " 登录失败：该帐号已被他人占用！");
                //回复该帐号已经连接，拒绝再次连接
                byte[] sendbuf2 = Encoding.Default.GetBytes(rescontent);

                //替换数据包长度信息
                short count2 = (short)(sendbuf2.Length + 9);
                short count = count2;

                //转换为网络字节顺序
                count2 = System.Net.IPAddress.HostToNetworkOrder(count2);
                byte[] bcount = BitConverter.GetBytes(count2);
                Array.Copy(bcount, 0, sendbuf, 2, 2);

                a.AddRange(sendbuf);
                a.AddRange(sendbuf2);

                if (m_ClientSocket == null)
                    return;

                //Log.Record("client.log", "回复数据包：", (byte[])a.ToArray(typeof(byte)), count);

                try
                {
                    m_ClientSocket.Send((byte[])a.ToArray(typeof(byte)));
                }
                catch(Exception ex)
                {
                    m_ClientSocket=null;
                }

                if (DebugDataFlag)
                {
                    Log.Record("client.log", "向帐号：" + UserName + " 回复数据包：", (byte[])a.ToArray(typeof(byte)), count);
                }

                // m_thread.Abort();
            }
        }
        #endregion 向客户端回传指令

        #region 分析航信返回的数据包（把汉字编码转换成正常汉字）
        //分析航信返回数据信息
        private string AnalyseServerContent(byte[] buf, int count)
        {
            string result = "";

            //主要是解析中文
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };
            ArrayList al = new ArrayList();
            ArrayList al2 = new ArrayList();

            int index = 0;
            int tmpcount = 0;
            bool isChinese = false;//是否为汉字

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);
            byte tmpbyte;

            while (index < count)
            {
                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0E))
                {
                    //汉字开始标志
                    al2.Clear();
                    index += 2;
                    isChinese = true;
                    continue;
                }

                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0F))
                {
                    string tmpcontent = "";
                    bool doubleFlag = false;
                    for (int i = 0; i < (al2.Count / 2); i++)
                    {
                        //如果有占用4个字节的汉字，则跳过一次
                        if (doubleFlag)
                        {
                            doubleFlag = false;
                            continue;
                        }
                        if ((byte)(al2[i * 2]) == 0x78)
                        {
                            byte[] cbuf = new byte[4];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            cbuf[2] = (byte)(al2[i * 2 + 2]);
                            cbuf[3] = (byte)(al2[i * 2 + 3]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            //index += 2;
                            doubleFlag = true;
                        }
                        else
                        {
                            byte[] cbuf = new byte[2];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            doubleFlag = false;
                        }
                    }

                    //处理并添加汉字信息
                    //把内容转换为string，判断是否为汉字，如果不是则把高低位互换然后把低位加10
                    //string tmpcontent= Encoding.Default.GetString((byte[])al2.ToArray(typeof(byte)));

                    //for (int i = 0; i < tmpcontent.Length; i++)
                    //{
                    //    int code = Char.ConvertToUtf32(tmpcontent, i);

                    //    //非汉字，需要处理，把高低位互换然后把低位加10
                    //    if ((code < chfrom)||(code > chend))
                    //    {
                    //        tmpbyte =  (byte)al2[i * 2];
                    //        al2[i * 2] = al2[i * 2 + 1];
                    //        al2[i * 2 + 1] = (byte)(tmpbyte + 0x0A);
                    //    }
                    //}

                    //al2.Clear();
                    //al2.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //把转换过的汉字追加到al中
                    al.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //汉字结束
                    index += 2;
                    isChinese = false;
                    continue;
                }

                tmpbyte = buf[index];

                //如果是汉字则处理
                if (isChinese)
                {
                    //tmpbyte += 0x80;
                    al.Add(tmpbyte);
                }
                else
                {
                    al.Add(tmpbyte);
                }
                tmpcount++;
                index++;
            }

            //从第20位开始，读取到末尾倒数第三位
            if (buf.Length < 23)
                return "";

            if ((buf[17] == 0x1B) && (buf[18] == 0x4D))
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, tmpcount - 23);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, al.Count - 23);
            }
            else
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, tmpcount - 21);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, al.Count - 21);
            }

            return result;
        }
        #endregion 分析航信返回的数据包（把汉字编码转换成正常汉字）

        #region 根据汉字编码取得汉字信息
        /// <summary>
        /// 根据航信编码返回汉字信息
        /// </summary>
        /// <param name="hzBytes"></param>
        /// <returns></returns>
        public string GetHanZiByHangXinBianMa(byte[] hzBytes)
        {
            string ResHanZi = "";
            string tmpBianMa = "";
            tmpBianMa = chs2py.GetStringFromByte(hzBytes[0]);
            tmpBianMa += " " + chs2py.GetStringFromByte(hzBytes[1]);

            //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
            //{
            //    if (x.BianMa.ToUpper() == tmpBianMa.ToUpper())
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});

            ////如果系统拼音中不存在此编码，则添加到系统汉字编码数据库中
            //if (_Base_PinYin == null)
            //{
            //    try
            //    {
            //        //添加到数据库
            //        _Base_PinYin = new Base_PinYin();
            //        _Base_PinYin.HanZi = "";
            //        _Base_PinYin.PinYin = "";
            //        _Base_PinYin.BianMa = tmpBianMa.ToLower();
            //        _Base_PinYin.Remarks = "客户ID:" + UserConfigManage.Customer_Id.ToString() + "；客户端帐号：" + UserConfigManage.User_Name;
            //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Record("error.log", "根据汉字编码获取汉字信息出错，错误信息："+ex.Message);
            //    }

            ResHanZi = chs2py.GetHanZi(hzBytes);

            if (ResHanZi == "")
            {
                try
                {
                    //添加到数据库
                    Base_PinYin _Base_PinYin = new Base_PinYin();
                    _Base_PinYin.HanZi = "";
                    _Base_PinYin.PinYin = "";
                    _Base_PinYin.BianMa = tmpBianMa.ToLower();
                    _Base_PinYin.Remarks = "客户ID:" + UserConfigManage.Customer_Id.ToString() + "；客户端帐号：" + UserConfigManage.User_Name;
                    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                }
                catch (Exception ex)
                {
                    Log.Record("error.log", "根据汉字编码获取汉字信息出错，错误信息：" + ex.Message);
                }
            }
            //}
            //else
            //{
            //    ResHanZi = _Base_PinYin.HanZi;
            //    if (ResHanZi.Trim() == "")
            //    {
            //        ResHanZi = chs2py.GetHanZi(hzBytes);
            //    }
            //}

            return ResHanZi;
        }
        #endregion

        #region 根据汉字取得汉字信息
        /// <summary>
        /// 根据汉字取得汉字编码和拼音
        /// </summary>
        /// <param name="HanZi"></param>
        /// <param name="HanZiBt"></param>
        /// <returns></returns>
        public string GetPinYinBianMaFromHanZi(string HanZi, out byte[] HanZiBt)
        {
            HanZiBt = null;

            string StrPinYin = "";

            //Base_PinYin _Base_PinYin = m_BasePinYinList.Find(delegate(Base_PinYin x)
            //{
            //    if (x.HanZi == HanZi)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});

            ////如果系统拼音中不存在此汉字，则添加到系统汉字编码数据库中
            //if (_Base_PinYin == null)
            //{
            //    try
            //    {
            //        //添加到数据库
            //        _Base_PinYin = new Base_PinYin();
            //        _Base_PinYin.HanZi = HanZi;
            //        _Base_PinYin.PinYin = "";
            //        _Base_PinYin.BianMa = "";
            //        _Base_PinYin.Remarks = "客户ID:" + UserConfigManage.Customer_Id.ToString() + "；客户端帐号：" + UserConfigManage.User_Name;
            //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Record("error.log", ex);
            //    }
            StrPinYin = chs2py.convert(HanZi, out HanZiBt);
            if (StrPinYin == "")
            {
                try
                {
                    //添加到数据库
                    Base_PinYin _Base_PinYin = new Base_PinYin();
                    _Base_PinYin.HanZi = HanZi;
                    _Base_PinYin.PinYin = "";
                    _Base_PinYin.BianMa = "";
                    _Base_PinYin.Remarks = "客户ID:" + UserConfigManage.Customer_Id.ToString() + "；客户端帐号：" + UserConfigManage.User_Name;
                    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                }
                catch (Exception ex)
                {
                    Log.Record("error.log", ex);
                }
            }
            return StrPinYin;
            //}

            //try
            //{
            //    StrPinYin = _Base_PinYin.PinYin;
            //    HanZiBt = new byte[2];
            //    HanZiBt[0] = chs2py.GetByteFromString(_Base_PinYin.BianMa.Trim().Substring(0, _Base_PinYin.BianMa.Trim().IndexOf(" ")));
            //    HanZiBt[1] = chs2py.GetByteFromString(_Base_PinYin.BianMa.Trim().Substring(_Base_PinYin.BianMa.Trim().IndexOf(" ")));

            //    return StrPinYin;
            //}
            //catch (Exception ex)
            //{
            //    Log.Record("error.log", "根据汉字获取拼音和编码信息出错，错误信息：" + ex.Message);
            //    return "";
            //}
        }
        #endregion

        #region 关闭释放资源
        /// <summary>
        /// 关闭释放资源
        /// </summary>        
        public void Close()
        {
            try
            {
                if (ClientThread != null)
                {
                    ClientThread.Abort();
                }

                ClientThread = null;
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Close();
                }
                ClientSocket = null;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion 关闭释放资源
    }
}
