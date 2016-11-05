using System;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using System.Net;

using ClassLibRemotingIPSink;
using RemoteDataSerivice;
using System.Threading;
using System.Management;
using System.Diagnostics;


namespace PBPid.Server
{
    public partial class FormMain : Form
    {

        #region 变量

        int hours = 0;
        int minute = 0;
        int seconds = 0;

        SocketLib.ServerSocketSync serverSocket = null;
        ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT WorkingSetSize FROM Win32_Process WHERE Name='"+ AppDomain.CurrentDomain.FriendlyName +"'");
        

        //时间限制
        private const string m_flag = "20141231";
        #endregion

        #region 构造

        public FormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(FormMain_FormClosing);
        }

        void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverSocket != null)
            {
                if (MessageBox.Show("您确定要关闭PBPid.服务端吗？这将会影响PBPid.客户端的运行,请谨慎操作！", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = false;//退出
                else
                    e.Cancel = true; //不退出
            }
        }

        #endregion

        #region 运行

        private Thread Time = null;
        private Thread Mem = null;

        private void button_Run_Click(object sender, EventArgs e)
        {
            listBox_Online.Items.Clear();
            listBox_Status.Items.Clear();

            button_Stop.Enabled = true;
            button_Run.Enabled = false;

            //时间限制
            //if (int.Parse(DateTime.Now.ToString("yyyyMMdd"))>int.Parse(m_flag))
            //{
            //    return;
            //}

            InitNet();    //加载通信服务
            RefrshCache();//加载缓存
            

            button_ClearCache.Enabled = true;

            Time = new Thread(RunTime);
            Time.IsBackground = true;
            Time.Priority = ThreadPriority.Lowest;
            Time.Start();

            Mem = new Thread(MonitorMem);
            Mem.IsBackground = true;
            Mem.Priority = ThreadPriority.Lowest;
            Mem.Start();
           
        }


        void RunTime()
        {
            do
            {
                seconds = seconds + 1;

                if (seconds >= 60)
                {
                    minute = minute + 1;
                    seconds = 0;
                }

                if (minute >= 60)
                {
                    hours = hours + 1;
                    minute = 0;
                }

                label_RunTime.Text = "系统已运行：" + hours + " 时 " + minute + " 分 " + seconds + " 秒";
                Thread.Sleep(1000);

            } while (true);
        }

        void MonitorMem()
        {
            do
            {
                foreach (ManagementObject mo in mos.Get())
                    this.label_Mem.Text = "PBPid.服务端物理内存使用：" + Convert.ToString(float.Parse(mo["WorkingSetSize"].ToString()) / 1024 / 1000) + " MB";

                Thread.Sleep(5000);
            } while (true);
        }

        #endregion

        #region 初始化通信服务

        void InitNet()
        {
            InitRemoting();
            InitSocket();
        }

        TcpServerChannel Remotingserver = null;
        void InitRemoting()//初始化remoting
        {
            try
            {
                BinaryServerFormatterSinkProvider sinkProvider = new BinaryServerFormatterSinkProvider();
                ClientIPServerSinkProvider IPProvider = new ClientIPServerSinkProvider();//创建SINK
                IPProvider.Next = sinkProvider;

                Remotingserver = new TcpServerChannel(DateTime.Now.ToString(), int.Parse(ConfigurationManager.AppSettings["RMPORT"].ToString()), IPProvider);
                ChannelServices.RegisterChannel(Remotingserver, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteService), "RemoteService", WellKnownObjectMode.SingleCall);
                Remotingserver.StartListening(null);

                AddListStatus("Remoting->服务已启动......");

                AddListStatus("Remoting->服务IP: " + Dns.GetHostAddresses(Dns.GetHostName())[0].ToString()
                    + " ,端口：" + ConfigurationManager.AppSettings["RMPORT"].ToString() + ",服务启动成功！ ");
                listBox_Status.Items.Add("");

                DB_CaChe.OnCallEvent += new DB_CaChe.CallEvevt(DB_CaChe_OnCallEvent);
                DB_CaChe.OnRefrshCaChe += new DB_CaChe.RefrshCaChe(DB_CaChe_OnRefrshCaChe);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("端口") != 0)
                {
                    MessageBox.Show("客户端连接了端口，无法释放！程序将自动重新启动！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Restart();
                }

            }
        }

        void DB_CaChe_OnRefrshCaChe(string TabName)
        {
            AddListStatus("Remoting->" + TabName);
        }

        void DB_CaChe_OnCallEvent(string context)
        {
            AddListStatus("Remoting->" + context);
        }

        void InitSocket()//初始化SOCKET
        {
            serverSocket = new SocketLib.ServerSocketSync(
                int.Parse(ConfigurationManager.AppSettings["SKPORT"].ToString()),
                int.Parse(ConfigurationManager.AppSettings["MaxConnection"].ToString())
                );

            serverSocket.OnCloseConnection += new SocketLib.ServerSocketSync.CloseConnection(serverSocket_OnCloseConnection);
            serverSocket.OnConnection += new SocketLib.ServerSocketSync.Connection(serverSocket_OnConnection);
            serverSocket.OnError += new SocketLib.ServerSocketSync.Error(serverSocket_OnError);
            serverSocket.OnReceivedBigData += new SocketLib.ServerSocketSync.ReceivedBigData(serverSocket_OnReceivedBigData);
            serverSocket.OnSendData += new SocketLib.ServerSocketSync.SendData(serverSocket_OnSendData);
            serverSocket.RunServer(ConfigurationManager.AppSettings["ServiceIp"].ToString());

            if (serverSocket.IsBound)
            {
                AddListStatus("Socket->服务已启动......");
                AddListStatus("Socket->服务IP:" + serverSocket.LocalEndPoint.ToString() + ",开始监听......");
            }
            else
            {
                AddListStatus("Socket->服务启动失败！请查看原因后重试！");
                serverSocket = null;
            }
            listBox_Status.Items.Add("");
        }

        #endregion

        #region socket事件

        void serverSocket_OnSendData(int _DataSize, string _Ip)
        {
            AddListStatus("Socket IP:" + serverSocket.LocalEndPoint.ToString() + "->" + _Ip + ",数据大小：" + float.Parse(_DataSize.ToString()) / 1000 + " KB");
        }

        void serverSocket_OnReceivedBigData(object ds, int _DataSize, System.Net.Sockets.Socket ck, int _ThreadID)
        {
            //socket 接收端

        }

        string getStringStatus(Socket ck,string TabName,int _DataSize)
        {
            return "Socket IP:" + ck.RemoteEndPoint.ToString() + "->" + serverSocket.LocalEndPoint.ToString() + ",操作类型：" + TabName + ",数据大小：" + float.Parse(_DataSize.ToString()) / 1000 + " KB";
        }

        void serverSocket_OnError(string Error_Message)
        {
            AddListStatus("Socket Error:" + Error_Message);
        }

        void serverSocket_OnConnection(string _Ip)
        {
            AddListStatus("Socket IP:" + _Ip + "，连接服务器成功！");
            //AddListOnlineStatus(_Ip);
        }

        void serverSocket_OnCloseConnection(string _Ip)
        {
            AddListStatus("Socket IP:" + _Ip + ",已断开服务器！");
            //removeOnline(_Ip);
        }

        #endregion

        #region 列表状态

        /// <summary>
        /// 删除在线主机
        /// </summary>
        /// <param name="_ip"></param>
        void removeOnline(string _ip)
        {             
            //listBox_Online.Items.Remove(_ip);
        }

        /// <summary>
        /// 增加在线主机
        /// </summary>
        /// <param name="_onlineIp"></param>
        void AddListOnlineStatus(string _onlineIp)
        {
            //listBox_Online.Items.Add(_onlineIp);
            //listBox_Online.SelectedIndex = listBox_Online.Items.Count - 1;
        }

        /// <summary>
        /// 添加各种状态
        /// </summary>
        /// <param name="_str"></param>
        void AddListStatus(string _str)
        {
            try
            {
                listBox_Status.Items.Add(_str + "." + DateTime.Now);
                listBox_Status.SelectedIndex = listBox_Status.Items.Count - 1;
                if (listBox_Status.Items.Count > 500)
                    listBox_Status.Items.Clear();
            }
            catch
            {
            }
        }

        #endregion

        #region 关闭方法

        private void button_Stop_Click(object sender, EventArgs e)
        {

            hours = 0;
            minute = 0;
            seconds = 0;

            Mem.Abort();
            Mem = null;

            Time.Abort();
            Time = null;

            label_Mem.Text = "已停止";
            label_RunTime.Text = "已停止";

            button_Stop.Enabled = false;
            button_Run.Enabled = true;
            button_ClearCache.Enabled = false;

            listBox_Online.Items.Clear();
            listBox_Status.Items.Clear();

            Remotingserver.StopListening(null);

            if (Remotingserver.ChannelData == null)
                AddListStatus("Remoting->通道已关闭！");
            else
                AddListStatus("Remoting->通道关闭失败！");
            

            serverSocket.CloseServer();
            serverSocket = null;
            AddListStatus("Socket->通道已关闭！");


            DB_CaChe.Dispose();
        }

        #endregion

        #region 刷新缓存

        private void button_ClearCache_Click(object sender, EventArgs e)
        {
            RefrshCache();
        }

        void RefrshCache()
        {
            AddListStatus("Remoting->开始加载缓存......");
            button_ClearCache.Enabled = false;

            DB_CaChe.Get_Base_CommandManage(true, serverSocket.LocalEndPoint.ToString());
            DB_CaChe.Get_Base_OriginalConfigManage(true, serverSocket.LocalEndPoint.ToString());
            DB_CaChe.Get_Base_UserConfigManage(true, serverSocket.LocalEndPoint.ToString());
            DB_CaChe.Get_Base_ProtocolType(true, serverSocket.LocalEndPoint.ToString());
            DB_CaChe.Get_Base_PinYin(true, serverSocket.LocalEndPoint.ToString());
            DB_CaChe.Get_Base_CityInfoManage(true, serverSocket.LocalEndPoint.ToString());

            button_ClearCache.Enabled = true;
            AddListStatus("Remoting->加载缓存完成......");
            listBox_Status.Items.Add("");
        }

        #endregion

        #region 清空主屏

        private void button_ClearStatus_Click(object sender, EventArgs e)
        {
            listBox_Status.Items.Clear();
        }

        #endregion

        private void FormMain_Load(object sender, EventArgs e)
        {
            //启动计时器
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //自动启动服务
            if ((ckbautoStart.Checked) && (button_Run.Enabled))
            {
                button_Run_Click(sender, e);
                button_Run.Enabled = false;
            }
            
            //自动刷新在线用户列表
            bool tempb=false;            
            for (int i = 0; i < DB_CaChe.logins.Count; i++)
            {
                if (listBox_Online.Items.Count == i)
                {
                    tempb = true;
                    break;
                }

                if (listBox_Online.Items[i].ToString() != DB_CaChe.logins[i].Customers.CustomerName)
                {
                    tempb = true;
                    break;
                }
            }
           

            if ((tempb) || (DB_CaChe.logins.Count != listBox_Online.Items.Count))
            {
                listBox_Online.Items.Clear();
                for (int j = 0; j < DB_CaChe.logins.Count; j++)
                {
                    listBox_Online.Items.Add(DB_CaChe.logins[j].Customers.CustomerName);
                }
            }
        }

        private void FormMain_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
        }

    }
}
