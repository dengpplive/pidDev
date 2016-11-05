using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Management;


using SocketServer.Sync;
using PBPid.DBModel;
using System.Configuration;
using PBPid.ClientManage;
using PBPid.EtermManage;
using System.Collections;
using PBPid.Base;
using PBPid.ServerManage;
using System.IO;

namespace PBPid
{
    //[Serializable]
    public partial class FormMain : Form
    {

        #region 变量

        private ServerSocketSync serverSocket = null;
        private ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT WorkingSetSize FROM Win32_Process WHERE Name='" + AppDomain.CurrentDomain.FriendlyName + "'");

        private Thread Time = null;
        private Thread Mem = null;

        private int hours = 0;
        private int minute = 0;
        private int seconds = 0;

        private int Port = 0;
        private string Ip = string.Empty;
        private int Maxconnection = 0;

        //
        private int SharePort = 3510;

        /// <summary>
        /// 窗口显示的最大行数
        /// </summary>
        private const int m_MaxLines = 1000;

        /// <summary>
        /// 发送和接收到的信息（用于窗口显示和日志记录）
        /// </summary>
        private ArrayList m_ArrayContents = new ArrayList();

        //是否输出调试信息（true：输出调试信息；false：不输出调试信息）
        private bool m_DebugFlag = true;

        //是否输出收发数据包调试信息（true：输出调试信息；false：不输出调试信息）
        private bool m_DebugDataFlag = true;

        //自动重启日期
        private string m_ReStartDate = "";
        
        #endregion

        #region 构造

        public FormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            ControlEnabled(false);

            ManageSpace.Manage.OnError += new ManageSpace.Manage.Error(Manage_OnError);
        }

        void Manage_OnError(string Error_Message)
        {
            if (Error_Message.IndexOf("断") != -1 || Error_Message.IndexOf("拒绝") != -1)
            {
                button_Stop_Click(null, null);
            }

        }

        #endregion

        #region 启动服务

        private void button_Run_Click(object sender, EventArgs e)
        {
            try
            {
                Log.filepath = Application.StartupPath;
                button_Run.Enabled = false;

                LoadSocket();//加载本地SOCKET

                ControlEnabled(false);
            }
            catch (Exception ex)
            {
                Log.Record("client.log", ex);
            }
        }

        void LoadSocket()
        {
            Time = new Thread(RunTime);
            Time.IsBackground = true;
            Time.Priority = ThreadPriority.Lowest;
            Time.Start();

            Mem = new Thread(MonitorMem);
            Mem.IsBackground = true;
            Mem.Priority = ThreadPriority.Lowest;
            Mem.Start();

            Port = int.Parse(ConfigurationManager.AppSettings["LOCALPORT"].ToString());
            Ip = ConfigurationManager.AppSettings["LOCALIP"].ToString();
            Maxconnection = int.Parse(ConfigurationManager.AppSettings["MaxConnection"].ToString());

            label_IP.Text = Ip;
            label_Max.Text = Maxconnection.ToString();
            label_Port.Text = Port.ToString();

            try
            {
                chs2py.ReloadPinYinFile();
            }
            catch (Exception ex)
            {
                Log.Record("client.log", "重新加载拼音文件出错，错误信息："+ex.Message);
            }

            //启动F6F7监听线程
            try
            {
                //启动Eterm控件线程
                if (myThread == null)
                {
                    int tmpshartport = 3510;
                    int.TryParse(ConfigurationManager.AppSettings["EtermManage_Port"].ToString(), out tmpshartport);
                    PBPid.ServerManage.ThreadWork.shareport = tmpshartport;
                    ThreadStart myThreadDelegate = new ThreadStart(PBPid.ServerManage.ThreadWork.DoWork);
                    myThread = new Thread(myThreadDelegate);
                    myThread.Start();
                    Thread.Sleep(1000);
                }
            }
            catch(Exception ex)
            {
                Log.Record("client.log", ex);
            }

            //启动原始配置服务
            try
            {
                if (m_ServerManage == null)
                {
                    m_ServerManage = new ServerManage.ServerManage();
                }

                //传递原始配置列表
                m_ServerManage.OriginalConfigManage = m_OriginalConfigManageList;
                //是否调试标志
                m_ServerManage.DebugFlag = m_DebugFlag;
                //是否输出收发数据包调试标志
                m_ServerManage.DebugDataFlag = m_DebugDataFlag;
                //F6F7通讯端口
                int tmpshartport = 3510;
                int.TryParse(ConfigurationManager.AppSettings["EtermManage_Port"].ToString(), out tmpshartport);
                m_ServerManage.SharePort = tmpshartport;
                //不自动连接时间段
                m_ServerManage.NoAutoConTime = ConfigurationManager.AppSettings["NoAutoSiTime"].ToString();
                //网站用户名
                m_ServerManage.WebUser = "webguest";
                //指令发送间隔
                int tmpinterval=0;
                int.TryParse(ConfigurationManager.AppSettings["Command_Interval"].ToString(), out tmpinterval);
                m_ServerManage.Interval = tmpinterval;

                //启动服务
                if (m_ServerManage.Start())
                {
                    AddListStatus("原始配置服务已启动......" + DateTime.Now+"\r");
                }
                else
                {
                    AddListStatus("原始配置服务启动失败，请查看原因后重试！" + DateTime.Now + "\r");
                }
                m_ServerInfoList = m_ServerManage.ServerInfoList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动原始配置服务失败，错误消息：" + ex.Message);
            }

            //启动客户端监听处理服务
            try
            {
                //针对放大用户的socket，启动监听处理服务
                if (m_ClientManage == null)
                {
                    m_ClientManage = new ClientManage.ClientManage();
                }

                //监听端口
                m_ClientManage.ListenPort = Port;
                //客户信息
                m_ClientManage.Base_Customer = currentUser;
                //放大用户列表
                m_ClientManage.UserConfigManageList = m_UserConfigManageList;
                //上下文信息
                m_ClientManage.BaseCommandManageList = m_BaseCommandManageList;
                //汉字拼音编码信息
                m_ClientManage.BasePinYinList = m_BasePinYinList;
                //配置处理类
                m_ClientManage.ServerManage = m_ServerManage;
                //是否调试标志
                m_ClientManage.DebugFlag = m_DebugFlag;
                //收发数据包调试标志
                m_ClientManage.DebugDataFlag = m_DebugDataFlag;
                //发送指令及收到的结果信息（用于窗口显示及日志记录）
                m_ClientManage.ArrayContents = m_ArrayContents;

                //启动服务
                if (m_ClientManage.Start())
                {
                    AddListStatus("客户端监听服务已启动......" + DateTime.Now + "\r");
                    AddListStatus("服务开始监听，监听端口：" + Port.ToString() + "......" + DateTime.Now + "\r");
                    Text = "PID 正在运行......用户: " + currentUser.CustomerName + " 登录成功！" + "联系电话:" + currentUser.CustomerPhone + ",到期日期：" + currentUser.CustomerEndDate;
                    button_Stop.Enabled = true;
                }
                else
                {
                    AddListStatus("监听服务启动失败！请查看原因后重试！" + DateTime.Now + "\r");
                    serverSocket = null;
                    Text = "PID 启动失败......";
                    button_Stop.Enabled = false;
                    button_Run.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("启动客户端监听服务失败！错误消息："+ex.Message);
                Application.Exit();
            }

            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
        }

        #endregion

        #region 停止服务

        private void button_Stop_Click(object sender, EventArgs e)
        {
            ClosePid(false);

            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;

            listBox_Online.Items.Clear();
            m_ServerInfoList = null;
        }

        void ClosePid(bool IsCon)
        {
            try
            {
                //停止客户端监听处理服务
                if (m_ClientManage != null)
                {
                    m_ClientManage.Stop();
                    m_ClientManage = null;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("停止客户端服务失败，错误消息："+ex.Message);
            }

            try
            {
                //停止原始配置服务
                if(m_ServerManage!=null)
                {
                    m_ServerManage.Stop();
                    m_ServerManage=null;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("停止原始配置服务失败，错误消息："+ex.Message);
            }

            Text = "PID 已停止运行......";
            label_Mem.Text = "PID物理内存使用：已停止服务";
            label_RunTime.Text = "系统已运行：已停止服务";
            label_IP.Text = "已停止服务";
            label_Max.Text = "已停止服务";
            label_Port.Text = "已停止服务";
            label_MainKey.Text = "已停止服务";
            listBox_Online.Items.Clear();

            SaveToLogFile2();

            richTextBox1.Clear();


            button_Stop.Enabled = false;

            AddListStatus("Socket->" + ManageSpace.Manage.Error_Messages);


            //if (serverSocket != null)
            //{
            //    serverSocket.CloseServer();
            //    serverSocket = null;
            //}

            AddListStatus("Socket->通道已关闭！" + DateTime.Now + "\r");

            ControlEnabled(true);

            button_Run.Enabled = true;



            if (Mem != null)
            {
                Mem.Abort();
                Mem = null;
            }

            if (Time != null)
            {
                Time.Abort();
                Time = null;
            }

            ManageSpace.Manage.LoginOut(currentUser);
        }

        #endregion

        #region 原始配置加载、添加、修改、删除

        private void myDataGridView_config_DoubleClick(object sender, EventArgs e)
        {
            if (button_AddConfig.Enabled)
            {
                ModifyConfig();
            }
            else
            {
                MessageBox.Show("请先停止服务，再进行信息修改！");
            }
        }


        /// <summary>
        /// 根据登录的ID加载原始配置
        /// </summary>
        /// <returns></returns>
        bool LoadConfigToDataView()
        {
            try
            {   //加载成功后，Manage 类中的原始配置缓存也会被缓存
                m_OriginalConfigManageList = ManageSpace.Manage.InitConfigInfoByCustomerId(currentUser.CustomerID);//获取远程数据
                this.myDataGridView_config.DataSource = m_OriginalConfigManageList; //ManageSpace.Manage.Get_Base_OriginalConfigManageByCustomerId(currentUser.CustomerID);
                return true;
            }
            catch
            {
                MessageBox.Show("系统初始化失败！请稍后重试！原因：" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //添加
        private void button_AddConfig_Click(object sender, EventArgs e)
        {
            FormAddModifyConfig add = new FormAddModifyConfig();
            add.Tag = currentUser.CustomerID;
            if (add.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadConfigToDataView();//刷新缓存数据

        }

        //修改
        private void button_Modify_Click(object sender, EventArgs e)
        {
            ModifyConfig();
        }

        void ModifyConfig()
        {
            int id = GetConfigID();
            if (id == 0)
            {
                MessageBox.Show("请选择一条数据再修改！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Base_OriginalConfigManage bm = ManageSpace.Manage.Get_Base_OriginalConfigManageByOriginal_Id(id);

            FormAddModifyConfig add = new FormAddModifyConfig(bm);
            add.Tag = currentUser.CustomerID;
            if (add.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadConfigToDataView();//刷新缓存数据

        }

        //删除
        private void button_DeleteConfig_Click(object sender, EventArgs e)
        {
            int id = GetConfigID();
            if (id == 0)
            {
                MessageBox.Show("请选择一条数据再删除！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("确定删除选择的原始配置？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Base_OriginalConfigManage bm = ManageSpace.Manage.Get_Base_OriginalConfigManageByOriginal_Id(id);

                if (ManageSpace.Manage.Delete_Base_OriginalConfigManage(bm))
                {
                    LoadConfigToDataView();                                     //刷新缓存数据
                    MessageBox.Show("删除成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("删除失败，请重试！" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        int GetConfigID()
        {
            int id = 0;
            try
            {
                id = int.Parse(myDataGridView_config.SelectedRows[0].Cells["Original_Id"].Value.ToString());
            }
            catch { }

            return id;
        }
        #endregion

        #region 放大用户、加载、添加、修改、删除

        #region 加载

        private void myDataGridView_User_DoubleClick(object sender, EventArgs e)
        {
            if (button_AddUser.Enabled)
            {
                ModifyUser();
            }
            else
            {
                MessageBox.Show("请先停止服务，再进行信息修改！");
            }
        }


        Dictionary<int, string> hash_int_str = new Dictionary<int, string>();

        void LoadUserConfig()//加载配置下的放大用户
        {
            List<Base_UserConfigManage> bucms_All = new List<Base_UserConfigManage>();

            //根据原始配置ID取出放大用户
            m_UserConfigManageList = ManageSpace.Manage.Get_Base_UserConfigManageByCustomer_Id(currentUser.CustomerID);
            foreach (Base_UserConfigManage bucm in m_UserConfigManageList)//不同ID能绑定多个
                bucms_All.Add(bucm);

            this.myDataGridView_User.DataSource = GetUserData(bucms_All);

            myDataGridView_User.Columns[0].Width = 60;
            myDataGridView_User.Columns[6].Width = 150;
            myDataGridView_User.Columns[7].Width = 150;
            myDataGridView_User.Columns[8].Width = 150;
            myDataGridView_User.ColumnHeadersHeight = 10;
        }

        //数据转换
        DataTable GetUserData(List<Base_UserConfigManage> bucm)
        {
            DataSet ds = GetTabStruct();
            foreach (Base_UserConfigManage item in bucm)
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr[0] = item.User_Id;
                dr[1] = item.User_Office;
                dr[2] = item.User_Name;
                dr[3] = item.User_Pass;
                dr[4] = item.User_DisableCmd;
                dr[5] = item.User_Disable ? "是" : "否";
                dr[6] = item.User_BeginDate;
                dr[7] = item.User_EndDate;
                dr[8] = item.User_SendCount;
                dr[9] = item.User_Description;
                ds.Tables[0].Rows.Add(dr);
            }
            return ds.Tables[0];
        }

        //组织显示的结构
        DataSet GetTabStruct()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("序号");
            dt.Columns.Add("绑定Office");
            dt.Columns.Add("用户名");
            dt.Columns.Add("密码");
            dt.Columns.Add("禁用指令");
            dt.Columns.Add("启用");
            dt.Columns.Add("开始日期");
            dt.Columns.Add("结束日期");
            dt.Columns.Add("发送指令计数");
            dt.Columns.Add("备注");
            ds.Tables.Add(dt);
            return ds;
        }

        #endregion

        #region  添加
        //增加放大用户
        private void button_AddUser_Click(object sender, EventArgs e)
        {
            FormAddModifyUser user = new FormAddModifyUser();
            user.curr_CustomerId = currentUser.CustomerID;
            if (user.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadUserConfig();//加载缓存
            }
        }
        #endregion

        #region 修改
        //修改
        private void button_ModifyUser_Click(object sender, EventArgs e)
        {

            ModifyUser();
        }

        void ModifyUser()
        {
            int id = GetUserID();
            if (id == 0)
            {
                MessageBox.Show("请选择一条数据再修改！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FormAddModifyUser user = new FormAddModifyUser(ManageSpace.Manage.Get_Base_UserConfigManageById(id));
            user.curr_CustomerId = currentUser.CustomerID;
            if (user.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadUserConfig();//加载缓存
            }
        }

        #endregion

        #region 删除

        private void button_DeleteUser_Click(object sender, EventArgs e)
        {
            int id = GetUserID();
            if (id == 0)
            {
                MessageBox.Show("请选择一条数据再删除！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Base_UserConfigManage bm = ManageSpace.Manage.Get_Base_UserConfigManageById(id);

            if (ManageSpace.Manage.Delete_Base_UserConfigManage(bm))
            {
                LoadUserConfig();              //刷新缓存数据
                MessageBox.Show("删除成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("删除失败，请重试！" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        int GetUserID()
        {
            int id = 0;
            try
            {
                id = int.Parse(myDataGridView_User.SelectedRows[0].Cells[0].Value.ToString());
            }
            catch { }
            return id;
        }

        #endregion




        #endregion

        #region SocketServer事件


        void serverSocket_OnSendData(int _DataSize, string _Ip)
        {
            AddListStatus("Socket->:" + serverSocket.LocalEndPoint.ToString() + "->" + _Ip + ",数据大小:" + float.Parse(_DataSize.ToString()) / 1000 + " KB");
        }

        void serverSocket_OnReceivedBigData(object ds, int _DataSize, System.Net.Sockets.Socket ck, int _ThreadID)
        {
            //
            int len = 0;//ck.Receive(

            //查找配置
            //连接航信
            //收到命令后，存入队列，扫队列中的命令，发送




        }

        void serverSocket_OnError(string Error_Message)
        {
            AddListStatus("Socket->: Error " + Error_Message);
        }

        void serverSocket_OnConnection(string _Ip)//放大用户连接成功！
        {
            AddListStatus("Socket->:" + _Ip + "，连接服务器成功！");
            AddListOnlineStatus(_Ip);
        }

        void serverSocket_OnCloseConnection(string _Ip)
        {
            AddListStatus("Socket->:" + _Ip + ",已断开服务器！");
            RemoveOnline(_Ip);
        }

        #endregion

        #region 状态显示

        void ControlEnabled(bool Enabled)
        {
            button_AddConfig.Enabled = Enabled;
            button_AddUser.Enabled = Enabled;
            button_DeleteConfig.Enabled = Enabled;
            button_DeleteUser.Enabled = Enabled;
            button_Modify.Enabled = Enabled;
            button_ModifyUser.Enabled = Enabled;
        }


        void AddListStatus(string str)
        {
            try
            {
                richTextBox1.AppendText(str);

                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 删除在线主机
        /// </summary>
        /// <param name="_ip"></param>
        void RemoveOnline(string _ip)
        {
            listBox_Online.Items.Remove(_ip);
        }

        /// <summary>
        /// 增加在线主机
        /// </summary>
        /// <param name="_onlineIp"></param>
        void AddListOnlineStatus(string _onlineIp)
        {
            listBox_Online.Items.Add(_onlineIp);
            listBox_Online.SelectedIndex = listBox_Online.Items.Count - 1;
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
                    this.label_Mem.Text = "PID物理内存使用：" + Convert.ToString(float.Parse(mo["WorkingSetSize"].ToString()) / 1024 / 1000) + " MB";

                Thread.Sleep(5000);
            } while (true);
        }

        #endregion

        void LoadCurrentUserInfo()
        {
            textBox_myloginName.Text = currentUser.CustomerLoginName;
            textBox_myloginPass.Text = currentUser.CustomerLoginPass;
            textBox_Address.Text = currentUser.CustomerAddress;
            textBox_CustomerDesc.Text = currentUser.CustomerDescription;
            textBox_CustomerName.Text = currentUser.CustomerName;
            textBox_Phone.Text = currentUser.CustomerPhone;
            dateTimePicker_BeginDate.Value = currentUser.CustomerBeginDate;
            dateTimePicker_EndDate.Value = currentUser.CustomerEndDate;
            textBox_Price.Text = currentUser.CustomerPrice.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //修改当前帐号信息
            currentUser.CustomerLoginPass = textBox_myloginPass.Text;
            currentUser.CustomerPhone = textBox_Phone.Text;
            currentUser.CustomerDescription = textBox_CustomerDesc.Text;
            currentUser.CustomerAddress = textBox_Address.Text;

            if (ManageSpace.Manage.Modify_Base_Customers(currentUser))
            {
                MessageBox.Show("修改成功！");
            }
            else
            {
                MessageBox.Show("修改失败！");
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClosePid(false);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            FormLog tmplog = new FormLog();
            tmplog.currentUser = currentUser;
            if (tmplog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //加载配置
                //调用协议层全部登录，缓存登录对象
                //send.同步返回
                //协议类型、使用接口枚举

                currentUser = tmplog.currentUser;

                richTextBox1.Clear();
                listBox_Online.Items.Clear();                

                LoadConfigToDataView();//加载配置

                LoadBaseCommandManage();//加载上下文信息    

                //Kevin 屏蔽，汉字信息从本地文件读取，遇到无法识别汉字则写入数据库中等待更新
                //LoadBasePinYin();//获取汉字拼音编码

                label_MainKey.Text = currentUser.CustomerName;
                
                LoadCurrentUserInfo();//加载用户信息到控件
                LoadUserConfig();

                ControlEnabled(true);
            }
            else
            {
                Application.Exit();
            }
        }

        private void LoadBaseCommandManage()
        {
            //获取上下文信息
            m_BaseCommandManageList = ManageSpace.Manage.Get_Base_CommandManage();         
        }

        private void LoadBasePinYin()
        {
            //获取拼音
            //m_BasePinYinList = ManageSpace.Manage.Get_Base_PinYin();
            //if (m_BasePinYinList == null)
            //{
            //    Log.Record("error.log","加载汉字拼音信息失败...");
            //    m_BasePinYinList = ManageSpace.Manage.Get_Base_PinYin();
            //    if (m_BasePinYinList == null)
            //    {
            //        MessageBox.Show("加载拼音信息失败，请重新启动程序！");
            //        Application.ExitThread();
            //    }
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string tmpstr = "";
            if (m_ArrayContents != null)
            {
                if (richTextBox1.Lines.Length >= m_MaxLines)
                {
                    //考虑到效率问题，先把内容复制到另外一个RichEdit中
                    richTextBox2.AppendText(richTextBox1.Text);

                    richTextBox1.Clear();
                }

                //此处有可能导致配置返回处理慢，需要考虑处理优化
                if (m_ArrayContents.Count != 0)
                {
                    tmpstr = m_ArrayContents[0] as string;
                    richTextBox1.AppendText(tmpstr);
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    lock (m_ArrayContents)
                    {
                        m_ArrayContents.RemoveAt(0);
                    }
                }
            }

            if (m_ServerInfoList != null)
            {
                BaseManage tmpManage = null;
                int index = 0;
                string tmpstatus = "未连接";

                foreach (int id in m_ServerInfoList.Keys)
                {
                    tmpManage = m_ServerInfoList[id] as BaseManage;

                    if (listBox_Online.Items.Count >= (index + 1))
                    {
                        switch (tmpManage.IdleFlag)
                        {
                            case -1:
                                tmpstatus = "未连接";
                                break;
                            case 0:
                                tmpstatus = "空闲";
                                break;
                            case 1:
                                tmpstatus = "忙碌";
                                break;
                            case 2:
                                tmpstatus = "帐号密码错误";
                                break;
                            case 3:
                                tmpstatus = "工作号密码错误";
                                break;
                            case 4:
                                tmpstatus = "异常错误";
                                break;
                        }

                        int tmpsecondtime = PublicInfo.DateDiff(DateTime.Now, tmpManage.SetBusyTime);

                        //检查配置，如果超过10秒钟仍为忙碌状态，则重新进行配置认证
                        if ((tmpManage.IdleFlag == 1) && (tmpsecondtime > 10))
                        {
                            //设置为异常状态，等待重新进行配置认证
                            Log.Record("server.log", "配置："+tmpManage.OriginalConfigManage.Original_ConfigName+"("+tmpManage.OriginalConfigManage.Original_OfficeNumber+")"+tmpManage.OriginalConfigManage.Original_Ip+" 超过10秒钟仍为忙碌状态，设为状态为异常，等待重新进行配置认证...");
                            tmpManage.IdleFlag = 4;
                        }

                        if (tmpManage.EtermType == "地址认证")
                        {
                            listBox_Online.Items[index] = "("+tmpManage.OriginalConfigManage.Original_OfficeNumber.ToUpper()+")"+tmpManage.OriginalConfigManage.Original_LocalIp + ":" + tmpManage.OriginalConfigManage.Original_Port.ToString() + "  " + tmpstatus;
                        }
                        else
                        {
                            listBox_Online.Items[index] = "(" + tmpManage.OriginalConfigManage.Original_OfficeNumber.ToUpper() + ")" + tmpManage.OriginalConfigManage.Original_ConfigName + ":" + tmpManage.OriginalConfigManage.Original_Port.ToString() + "  " + tmpstatus;
                        }
                        index++;
                    }
                    else
                    {
                        if (tmpManage.EtermType == "地址认证")
                        {
                            listBox_Online.Items.Add("(" + tmpManage.OriginalConfigManage.Original_OfficeNumber.ToUpper() + ")" + tmpManage.OriginalConfigManage.Original_LocalIp + ":" + tmpManage.OriginalConfigManage.Original_Port.ToString() + "  " + tmpstatus);
                        }
                        else
                        {
                            listBox_Online.Items.Add("(" + tmpManage.OriginalConfigManage.Original_OfficeNumber.ToUpper() + ")" + tmpManage.OriginalConfigManage.Original_ConfigName + ":" + tmpManage.OriginalConfigManage.Original_Port.ToString() + "  " + tmpstatus);
                        }
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Length != 0)
            {
                lock (richTextBox2)
                {
                    SaveToLogFile();
                    richTextBox2.Clear();
                }
            }
        }

        #region 保存日志文件
        public void SaveToLogFile()
        {
            //判断是否有日志 
            if (richTextBox2.Lines.Length == 0)
                return;

            //创建日志目录和日志文件
            string tmpdir = @".\log";
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            //每天一个目录
            DateTime tmpdate = DateTime.Now;
            tmpdir += ("\\" + tmpdate.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            //每小时一个目录
            tmpdir += ("\\" + tmpdate.Hour.ToString());
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            //判断日志文件是否存在
            string filename = tmpdir + "\\" + tmpdate.ToString("yyyyMMdd HHmmss") + ".txt";
            if (!File.Exists(filename))
            {
                StreamWriter sw = File.CreateText(filename);
                for (int i = 0; i < richTextBox2.Lines.Length; i++)
                {
                    sw.WriteLine(richTextBox2.Lines[i]);
                    sw.Flush();
                }
                sw.Close();
            }
            else
            {
                //追加文件
                StreamWriter sw = File.AppendText(filename);
                for (int i = 0; i < richTextBox2.Lines.Length; i++)
                {
                    sw.WriteLine(richTextBox2.Lines[i]);
                    sw.Flush();
                }
                sw.Close();
            }
        }
        #endregion 

        #region 保存日志文件
        public void SaveToLogFile2()
        {
            //判断是否有日志 
            if (richTextBox1.Lines.Length == 0)
                return;

            //创建日志目录和日志文件
            string tmpdir = @".\log";
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            //每天一个目录
            DateTime tmpdate = DateTime.Now;
            tmpdir += ("\\" + tmpdate.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            //每小时一个目录
            tmpdir += ("\\" + tmpdate.Hour.ToString());
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            //判断日志文件是否存在
            string filename = tmpdir + "\\" + tmpdate.ToString("yyyyMMdd HHmmss") + ".txt";
            if (!File.Exists(filename))
            {
                StreamWriter sw = File.CreateText(filename);
                for (int i = 0; i < richTextBox1.Lines.Length; i++)
                {
                    sw.WriteLine(richTextBox1.Lines[i]);
                    sw.Flush();
                }
                sw.Close();
            }
            else
            {
                //追加文件
                StreamWriter sw = File.AppendText(filename);
                for (int i = 0; i < richTextBox1.Lines.Length; i++)
                {
                    sw.WriteLine(richTextBox1.Lines[i]);
                    sw.Flush();
                }
                sw.Close();
            }
        }
        #endregion


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
                    //        //添加到数据库
                    //        _Base_PinYin = new Base_PinYin();
                    //        _Base_PinYin.HanZi = tmpcontent.Substring(i, 1);
                    //        _Base_PinYin.PinYin = "";
                    //        _Base_PinYin.BianMa = "";
                    //        _Base_PinYin.Remarks = "";
                    //        ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
                    //    }

                    //    flag = false;
                    //}

                    //检查中文拼音是否支持
                    byte[] hanziBt;
                    tmppinyin = chs2py.convert(tmpcontent.Substring(i, 1), out hanziBt);
                    if (hanziBt == null)
                    {
                        Log.Record("chinese.log", "不支持汉字：" + tmpcontent.Substring(i, 1) + " ||| " + tmpcontent);
                        return false;
                    }

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

        private void button2_Click(object sender, EventArgs e)
        {
            CheckChinesePinYin("豇薑奬");

            ////重新加载拼音文件
            //chs2py.ReloadPinYinFile();
            //for (int i = 0; i < chs2py.pinyinList.Count; i++)
            //{
            //    Base_PinYin _Base_PinYin = new Base_PinYin();

            //    string tmpstr = chs2py.pinyinList[i];

            //    string[] sl = tmpstr.Split(new char[] { ' ' });

            //    int count = 0;
            //    for (int j = 0; j < sl.Length; j++)
            //    {
            //        if (sl[j] == "")
            //        {
            //            continue;
            //        }

            //        switch (count)
            //        {
            //            //汉字
            //            case 0:
            //                _Base_PinYin.HanZi = sl[j];
            //                break;
            //            //拼音
            //            case 1:
            //                _Base_PinYin.PinYin = sl[j];
            //                break;
            //            //汉字编码的高位
            //            case 2:
            //                _Base_PinYin.BianMa += sl[j];
            //                break;
            //            //汉字编码的低位
            //            case 3:
            //                _Base_PinYin.BianMa += " " + sl[j];
            //                break;
            //        }
            //        count++;
            //    }
            //    _Base_PinYin.Remarks = "";

            //    //写入
            //    ManageSpace.Manage.Add_Base_PinYin(_Base_PinYin);
            //}
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //重新停止、启动服务
            if ((m_ReStartDate != DateTime.Now.ToString("yyyy-MM-dd")) && (DateTime.Now.Hour == 7))
            {
                button_Stop_Click(sender, e);
                //button_Run_Click(sender, e);
                //m_ReStartDate = DateTime.Now.ToString("yyyy-MM-dd");
            }

            try
            {
                //重新加载拼音文件
                chs2py.ReloadPinYinFile();
            }
            catch (Exception ex)
            {
                Log.Record("client.log", "重新加载拼音文件出错，错误信息：" + ex.Message);
            }

            //重新停止、启动服务
            if ((m_ReStartDate != DateTime.Now.ToString("yyyy-MM-dd")) && (DateTime.Now.Hour == 7))
            {
                button_Run_Click(sender, e);
                m_ReStartDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
    }
}
