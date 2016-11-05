using System.Collections.Generic;
using PBPid.DBModel;
using System.Threading;
using PBPid.EtermManage;
using PBPid.Base;

namespace PBPid
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        //客户端服务监听处理类
        private PBPid.ClientManage.ClientManage m_ClientManage = null;

        //配置服务处理类
        private PBPid.ServerManage.ServerManage m_ServerManage = null;

        //放大帐号列表
        List<Base_UserConfigManage> m_UserConfigManageList = null;

        //原始配置列表
        List<Base_OriginalConfigManage> m_OriginalConfigManageList = null;

        //上下文信息
        List<Base_CommandManage> m_BaseCommandManageList = null;

        //汉字拼音编码
        List<Base_PinYin> m_BasePinYinList = null;

        //城市信息
        List<Base_CityInfoManage> m_BaseCityInfoManageList = null;

        //当前客户资料信息
        private Base_Customers currentUser = new Base_Customers();

        //F6F7处理线程
        private Thread myThread = null;

        //当前配置处理类列表
        private NoSortHashTable m_ServerInfoList = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                if (m_ClientManage != null)
                {
                    m_ClientManage.Stop();
                    m_ClientManage = null;
                }

                if (m_ServerManage != null)
                {
                    m_ServerManage.Stop();
                    m_ServerManage = null;
                }

                if (myThread != null)
                {
                    myThread.Abort();

                    ServerManage.ThreadWork.closeFlag = true;

                    if (ServerManage.ThreadWork.sslServer != null)
                    {
                        ServerManage.ThreadWork.sslServer.Stop();
                    }

                }

                ////停止监听线程
                //if (_listener != null)
                //{
                //    ListenThread.Abort();
                //    _listener.Stop();
                //}

                ////关闭客户端连接并清理资源
                //if (ht_clientserver.Count != 0)
                //{
                //    foreach (ClientMgr clientmgr in ht_clientserver.Values)
                //    {
                //        if (clientmgr.ClientThread != null && clientmgr.ClientThread.IsAlive)
                //        {
                //            //关闭客户端 Thread
                //            clientmgr.ClientThread.Abort();
                //        }

                //        if (clientmgr.ClientSocket != null)
                //        {
                //            //关闭客户端Socket
                //            clientmgr.ClientSocket.Shutdown(SocketShutdown.Both);
                //        }
                //    }

                //    //清空列表
                //    ht_clientserver.Clear();
                //    ht_clientserver = null;
                //}

                ////关闭配置连接
                //if (ht_etermserver.Count != 0)
                //{
                //    foreach (EtermMgr etermmgr in ht_etermserver.Values)
                //    {
                //        etermmgr.closeFlag = true;

                //        if (etermmgr.EtermThread != null && etermmgr.EtermThread.IsAlive)
                //        {
                //            //关闭Eterm配置 Thread
                //            etermmgr.EtermThread.Abort();
                //        }

                //        if (etermmgr.EtermSocket != null)
                //        {
                //            //关闭Eterm配置 Socket
                //            etermmgr.EtermSocket.Shutdown(SocketShutdown.Both);
                //        }
                //    }

                //    //清空列表
                //    ht_etermserver.Clear();
                //    ht_etermserver = null;
                //}

                //if (myThread != null)
                //{
                //    myThread.Abort();

                //    ThreadWork.closeFlag = true;

                //    if (ThreadWork.sslServer != null)
                //    {
                //        ThreadWork.sslServer.Stop();
                //        //ThreadWork.
                //    }

                //}
            }

            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_Run = new System.Windows.Forms.Button();
            this.label_Mem = new System.Windows.Forms.Label();
            this.label_RunTime = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.button_DeleteConfig = new System.Windows.Forms.Button();
            this.button_Modify = new System.Windows.Forms.Button();
            this.button_AddConfig = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.button_DeleteUser = new System.Windows.Forms.Button();
            this.button_ModifyUser = new System.Windows.Forms.Button();
            this.button_AddUser = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.textBox_Price = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker_EndDate = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_BeginDate = new System.Windows.Forms.DateTimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_CustomerDesc = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox_Phone = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_Address = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_CustomerName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_myloginPass = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_myloginName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Stop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label_MainKey = new System.Windows.Forms.Label();
            this.label_Max = new System.Windows.Forms.Label();
            this.label_Port = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_IP = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.listBox_Online = new PBPid.myControl.myListBox();
            this.myDataGridView_config = new PBPid.MyDataGridView();
            this.Original_Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_LocalIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_Ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_Port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_ConfigName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_ConfigPass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_ConfigSi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_CheckingType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_OfficeNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Original_CorporationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.myDataGridView_User = new PBPid.MyDataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView_User)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Run
            // 
            this.button_Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Run.Location = new System.Drawing.Point(606, 34);
            this.button_Run.Name = "button_Run";
            this.button_Run.Size = new System.Drawing.Size(75, 23);
            this.button_Run.TabIndex = 3;
            this.button_Run.Text = "启动服务";
            this.button_Run.UseVisualStyleBackColor = true;
            this.button_Run.Click += new System.EventHandler(this.button_Run_Click);
            // 
            // label_Mem
            // 
            this.label_Mem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Mem.AutoSize = true;
            this.label_Mem.Location = new System.Drawing.Point(14, 61);
            this.label_Mem.Name = "label_Mem";
            this.label_Mem.Size = new System.Drawing.Size(131, 12);
            this.label_Mem.TabIndex = 2;
            this.label_Mem.Text = "PID物理内存使用：0 MB";
            // 
            // label_RunTime
            // 
            this.label_RunTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_RunTime.AutoSize = true;
            this.label_RunTime.Location = new System.Drawing.Point(228, 61);
            this.label_RunTime.Name = "label_RunTime";
            this.label_RunTime.Size = new System.Drawing.Size(83, 12);
            this.label_RunTime.TabIndex = 3;
            this.label_RunTime.Text = "系统已运行：0";
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabControl1.Location = new System.Drawing.Point(4, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(914, 414);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Black;
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Controls.Add(this.listBox_Online);
            this.tabPage1.Controls.Add(this.richTextBox2);
            this.tabPage1.Location = new System.Drawing.Point(22, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(888, 406);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "收发状态";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(655, 400);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.Black;
            this.richTextBox2.Location = new System.Drawing.Point(277, 166);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(100, 96);
            this.richTextBox2.TabIndex = 3;
            this.richTextBox2.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.button_DeleteConfig);
            this.tabPage2.Controls.Add(this.button_Modify);
            this.tabPage2.Controls.Add(this.button_AddConfig);
            this.tabPage2.Controls.Add(this.myDataGridView_config);
            this.tabPage2.Location = new System.Drawing.Point(22, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(888, 406);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "原始配置设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(246, 382);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "双击以上数据可查看及修改";
            // 
            // button_DeleteConfig
            // 
            this.button_DeleteConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_DeleteConfig.Location = new System.Drawing.Point(165, 377);
            this.button_DeleteConfig.Name = "button_DeleteConfig";
            this.button_DeleteConfig.Size = new System.Drawing.Size(75, 23);
            this.button_DeleteConfig.TabIndex = 1;
            this.button_DeleteConfig.Text = "删除配置";
            this.button_DeleteConfig.UseVisualStyleBackColor = true;
            this.button_DeleteConfig.Click += new System.EventHandler(this.button_DeleteConfig_Click);
            // 
            // button_Modify
            // 
            this.button_Modify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Modify.Location = new System.Drawing.Point(84, 377);
            this.button_Modify.Name = "button_Modify";
            this.button_Modify.Size = new System.Drawing.Size(75, 23);
            this.button_Modify.TabIndex = 1;
            this.button_Modify.Text = "修改配置";
            this.button_Modify.UseVisualStyleBackColor = true;
            this.button_Modify.Click += new System.EventHandler(this.button_Modify_Click);
            // 
            // button_AddConfig
            // 
            this.button_AddConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_AddConfig.Location = new System.Drawing.Point(3, 377);
            this.button_AddConfig.Name = "button_AddConfig";
            this.button_AddConfig.Size = new System.Drawing.Size(75, 23);
            this.button_AddConfig.TabIndex = 1;
            this.button_AddConfig.Text = "添加配置";
            this.button_AddConfig.UseVisualStyleBackColor = true;
            this.button_AddConfig.Click += new System.EventHandler(this.button_AddConfig_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.button_DeleteUser);
            this.tabPage3.Controls.Add(this.button_ModifyUser);
            this.tabPage3.Controls.Add(this.button_AddUser);
            this.tabPage3.Controls.Add(this.myDataGridView_User);
            this.tabPage3.Location = new System.Drawing.Point(22, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(888, 406);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "放大用户设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(246, 382);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "双击以上数据可查看及修改";
            // 
            // button_DeleteUser
            // 
            this.button_DeleteUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_DeleteUser.Location = new System.Drawing.Point(165, 377);
            this.button_DeleteUser.Name = "button_DeleteUser";
            this.button_DeleteUser.Size = new System.Drawing.Size(75, 23);
            this.button_DeleteUser.TabIndex = 1;
            this.button_DeleteUser.Text = "删除用户";
            this.button_DeleteUser.UseVisualStyleBackColor = true;
            this.button_DeleteUser.Click += new System.EventHandler(this.button_DeleteUser_Click);
            // 
            // button_ModifyUser
            // 
            this.button_ModifyUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ModifyUser.Location = new System.Drawing.Point(84, 377);
            this.button_ModifyUser.Name = "button_ModifyUser";
            this.button_ModifyUser.Size = new System.Drawing.Size(75, 23);
            this.button_ModifyUser.TabIndex = 1;
            this.button_ModifyUser.Text = "修改用户";
            this.button_ModifyUser.UseVisualStyleBackColor = true;
            this.button_ModifyUser.Click += new System.EventHandler(this.button_ModifyUser_Click);
            // 
            // button_AddUser
            // 
            this.button_AddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_AddUser.Location = new System.Drawing.Point(3, 377);
            this.button_AddUser.Name = "button_AddUser";
            this.button_AddUser.Size = new System.Drawing.Size(75, 23);
            this.button_AddUser.TabIndex = 1;
            this.button_AddUser.Text = "添加用户";
            this.button_AddUser.UseVisualStyleBackColor = true;
            this.button_AddUser.Click += new System.EventHandler(this.button_AddUser_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.textBox_Price);
            this.tabPage4.Controls.Add(this.label17);
            this.tabPage4.Controls.Add(this.button1);
            this.tabPage4.Controls.Add(this.dateTimePicker_EndDate);
            this.tabPage4.Controls.Add(this.dateTimePicker_BeginDate);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.textBox_CustomerDesc);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.textBox_Phone);
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.textBox_Address);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.textBox_CustomerName);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.textBox_myloginPass);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.textBox_myloginName);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Location = new System.Drawing.Point(22, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(888, 406);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "我的信息";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // textBox_Price
            // 
            this.textBox_Price.Enabled = false;
            this.textBox_Price.Location = new System.Drawing.Point(126, 340);
            this.textBox_Price.Name = "textBox_Price";
            this.textBox_Price.Size = new System.Drawing.Size(100, 21);
            this.textBox_Price.TabIndex = 5;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(52, 344);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 12);
            this.label17.TabIndex = 4;
            this.label17.Text = "购买价格：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(498, 338);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "确认修改";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker_EndDate
            // 
            this.dateTimePicker_EndDate.Enabled = false;
            this.dateTimePicker_EndDate.Location = new System.Drawing.Point(381, 69);
            this.dateTimePicker_EndDate.Name = "dateTimePicker_EndDate";
            this.dateTimePicker_EndDate.Size = new System.Drawing.Size(148, 21);
            this.dateTimePicker_EndDate.TabIndex = 2;
            // 
            // dateTimePicker_BeginDate
            // 
            this.dateTimePicker_BeginDate.Enabled = false;
            this.dateTimePicker_BeginDate.Location = new System.Drawing.Point(126, 69);
            this.dateTimePicker_BeginDate.Name = "dateTimePicker_BeginDate";
            this.dateTimePicker_BeginDate.Size = new System.Drawing.Size(141, 21);
            this.dateTimePicker_BeginDate.TabIndex = 2;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(310, 73);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 0;
            this.label15.Text = "到期日期：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(55, 73);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 0;
            this.label14.Text = "起始日期：";
            // 
            // textBox_CustomerDesc
            // 
            this.textBox_CustomerDesc.Location = new System.Drawing.Point(126, 217);
            this.textBox_CustomerDesc.Multiline = true;
            this.textBox_CustomerDesc.Name = "textBox_CustomerDesc";
            this.textBox_CustomerDesc.Size = new System.Drawing.Size(447, 117);
            this.textBox_CustomerDesc.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(55, 220);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 0;
            this.label16.Text = "备注信息：";
            // 
            // textBox_Phone
            // 
            this.textBox_Phone.Enabled = false;
            this.textBox_Phone.Location = new System.Drawing.Point(381, 164);
            this.textBox_Phone.Name = "textBox_Phone";
            this.textBox_Phone.Size = new System.Drawing.Size(192, 21);
            this.textBox_Phone.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(310, 167);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "联系电话：";
            // 
            // textBox_Address
            // 
            this.textBox_Address.Location = new System.Drawing.Point(126, 115);
            this.textBox_Address.Name = "textBox_Address";
            this.textBox_Address.Size = new System.Drawing.Size(447, 21);
            this.textBox_Address.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(55, 118);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "客户地址：";
            // 
            // textBox_CustomerName
            // 
            this.textBox_CustomerName.Enabled = false;
            this.textBox_CustomerName.Location = new System.Drawing.Point(126, 164);
            this.textBox_CustomerName.Name = "textBox_CustomerName";
            this.textBox_CustomerName.Size = new System.Drawing.Size(141, 21);
            this.textBox_CustomerName.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(55, 167);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "客户姓名：";
            // 
            // textBox_myloginPass
            // 
            this.textBox_myloginPass.Location = new System.Drawing.Point(381, 22);
            this.textBox_myloginPass.Name = "textBox_myloginPass";
            this.textBox_myloginPass.Size = new System.Drawing.Size(148, 21);
            this.textBox_myloginPass.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(310, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "登录密码：";
            // 
            // textBox_myloginName
            // 
            this.textBox_myloginName.Enabled = false;
            this.textBox_myloginName.Location = new System.Drawing.Point(126, 22);
            this.textBox_myloginName.Name = "textBox_myloginName";
            this.textBox_myloginName.Size = new System.Drawing.Size(141, 21);
            this.textBox_myloginName.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(55, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "登录账户：";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "软件已授权给：";
            // 
            // button_Stop
            // 
            this.button_Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Stop.Enabled = false;
            this.button_Stop.Location = new System.Drawing.Point(700, 34);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(75, 23);
            this.button_Stop.TabIndex = 4;
            this.button_Stop.Text = "停止服务";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label_MainKey);
            this.groupBox1.Controls.Add(this.button_Run);
            this.groupBox1.Controls.Add(this.label_RunTime);
            this.groupBox1.Controls.Add(this.button_Stop);
            this.groupBox1.Controls.Add(this.label_Max);
            this.groupBox1.Controls.Add(this.label_Port);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label_IP);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label_Mem);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(1, 426);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(914, 86);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(808, 34);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "测试";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label_MainKey
            // 
            this.label_MainKey.AutoSize = true;
            this.label_MainKey.Location = new System.Drawing.Point(116, 17);
            this.label_MainKey.Name = "label_MainKey";
            this.label_MainKey.Size = new System.Drawing.Size(11, 12);
            this.label_MainKey.TabIndex = 8;
            this.label_MainKey.Text = "0";
            // 
            // label_Max
            // 
            this.label_Max.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Max.AutoSize = true;
            this.label_Max.Location = new System.Drawing.Point(300, 17);
            this.label_Max.Name = "label_Max";
            this.label_Max.Size = new System.Drawing.Size(11, 12);
            this.label_Max.TabIndex = 2;
            this.label_Max.Text = "0";
            // 
            // label_Port
            // 
            this.label_Port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Port.AutoSize = true;
            this.label_Port.Location = new System.Drawing.Point(300, 39);
            this.label_Port.Name = "label_Port";
            this.label_Port.Size = new System.Drawing.Size(11, 12);
            this.label_Port.TabIndex = 2;
            this.label_Port.Text = "0";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(228, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "最大连接数：";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(240, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "服务端口：";
            // 
            // label_IP
            // 
            this.label_IP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_IP.AutoSize = true;
            this.label_IP.Location = new System.Drawing.Point(116, 39);
            this.label_IP.Name = "label_IP";
            this.label_IP.Size = new System.Drawing.Size(47, 12);
            this.label_IP.TabIndex = 2;
            this.label_IP.Text = "0.0.0.0";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "服务IP：";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 5000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 60000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(700, 61);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listBox_Online
            // 
            this.listBox_Online.BackColor = System.Drawing.Color.Black;
            this.listBox_Online.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox_Online.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox_Online.Font = new System.Drawing.Font("宋体", 9F);
            this.listBox_Online.ForeColor = System.Drawing.Color.Lime;
            this.listBox_Online.FormattingEnabled = true;
            this.listBox_Online.ItemHeight = 12;
            this.listBox_Online.Location = new System.Drawing.Point(658, 3);
            this.listBox_Online.Name = "listBox_Online";
            this.listBox_Online.Size = new System.Drawing.Size(227, 400);
            this.listBox_Online.TabIndex = 1;
            // 
            // myDataGridView_config
            // 
            this.myDataGridView_config.AllowUserToAddRows = false;
            this.myDataGridView_config.AllowUserToDeleteRows = false;
            this.myDataGridView_config.AllowUserToResizeColumns = false;
            this.myDataGridView_config.AllowUserToResizeRows = false;
            this.myDataGridView_config.BackgroundColor = System.Drawing.Color.White;
            this.myDataGridView_config.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView_config.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Original_Id,
            this.Original_LocalIP,
            this.Original_Ip,
            this.Original_Port,
            this.Original_ConfigName,
            this.Original_ConfigPass,
            this.Original_Status,
            this.Original_ConfigSi,
            this.Original_CheckingType,
            this.Original_OfficeNumber,
            this.Original_CorporationName,
            this.Desc});
            this.myDataGridView_config.Location = new System.Drawing.Point(-1, 0);
            this.myDataGridView_config.Name = "myDataGridView_config";
            this.myDataGridView_config.ReadOnly = true;
            this.myDataGridView_config.RowHeadersVisible = false;
            this.myDataGridView_config.RowTemplate.Height = 23;
            this.myDataGridView_config.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.myDataGridView_config.ShowRowErrors = false;
            this.myDataGridView_config.Size = new System.Drawing.Size(811, 371);
            this.myDataGridView_config.TabIndex = 0;
            this.myDataGridView_config.DoubleClick += new System.EventHandler(this.myDataGridView_config_DoubleClick);
            // 
            // Original_Id
            // 
            this.Original_Id.DataPropertyName = "Original_Id";
            this.Original_Id.HeaderText = "序号";
            this.Original_Id.Name = "Original_Id";
            this.Original_Id.ReadOnly = true;
            this.Original_Id.Width = 60;
            // 
            // Original_LocalIP
            // 
            this.Original_LocalIP.DataPropertyName = "Original_LocalIP";
            this.Original_LocalIP.HeaderText = "本机IP地址";
            this.Original_LocalIP.Name = "Original_LocalIP";
            this.Original_LocalIP.ReadOnly = true;
            // 
            // Original_Ip
            // 
            this.Original_Ip.DataPropertyName = "Original_Ip";
            this.Original_Ip.HeaderText = "认证服务器IP";
            this.Original_Ip.Name = "Original_Ip";
            this.Original_Ip.ReadOnly = true;
            this.Original_Ip.Width = 150;
            // 
            // Original_Port
            // 
            this.Original_Port.DataPropertyName = "Original_Port";
            this.Original_Port.HeaderText = "认证端口";
            this.Original_Port.Name = "Original_Port";
            this.Original_Port.ReadOnly = true;
            // 
            // Original_ConfigName
            // 
            this.Original_ConfigName.DataPropertyName = "Original_ConfigName";
            this.Original_ConfigName.HeaderText = "配置名";
            this.Original_ConfigName.Name = "Original_ConfigName";
            this.Original_ConfigName.ReadOnly = true;
            // 
            // Original_ConfigPass
            // 
            this.Original_ConfigPass.DataPropertyName = "Original_ConfigPass";
            this.Original_ConfigPass.HeaderText = "密码";
            this.Original_ConfigPass.Name = "Original_ConfigPass";
            this.Original_ConfigPass.ReadOnly = true;
            // 
            // Original_Status
            // 
            this.Original_Status.DataPropertyName = "Original_Status";
            this.Original_Status.HeaderText = "状态";
            this.Original_Status.Name = "Original_Status";
            this.Original_Status.ReadOnly = true;
            // 
            // Original_ConfigSi
            // 
            this.Original_ConfigSi.DataPropertyName = "Original_ConfigSi";
            this.Original_ConfigSi.HeaderText = "Si";
            this.Original_ConfigSi.Name = "Original_ConfigSi";
            this.Original_ConfigSi.ReadOnly = true;
            // 
            // Original_CheckingType
            // 
            this.Original_CheckingType.DataPropertyName = "Original_CheckingType";
            this.Original_CheckingType.HeaderText = "认证类型";
            this.Original_CheckingType.Name = "Original_CheckingType";
            this.Original_CheckingType.ReadOnly = true;
            // 
            // Original_OfficeNumber
            // 
            this.Original_OfficeNumber.DataPropertyName = "Original_OfficeNumber";
            this.Original_OfficeNumber.HeaderText = "OFFICE号";
            this.Original_OfficeNumber.Name = "Original_OfficeNumber";
            this.Original_OfficeNumber.ReadOnly = true;
            // 
            // Original_CorporationName
            // 
            this.Original_CorporationName.DataPropertyName = "Original_CorporationName";
            this.Original_CorporationName.HeaderText = "公司名称";
            this.Original_CorporationName.Name = "Original_CorporationName";
            this.Original_CorporationName.ReadOnly = true;
            // 
            // Desc
            // 
            this.Desc.DataPropertyName = "Original_Description";
            this.Desc.HeaderText = "备注";
            this.Desc.Name = "Desc";
            this.Desc.ReadOnly = true;
            // 
            // myDataGridView_User
            // 
            this.myDataGridView_User.AllowUserToAddRows = false;
            this.myDataGridView_User.AllowUserToDeleteRows = false;
            this.myDataGridView_User.AllowUserToResizeColumns = false;
            this.myDataGridView_User.AllowUserToResizeRows = false;
            this.myDataGridView_User.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myDataGridView_User.BackgroundColor = System.Drawing.Color.White;
            this.myDataGridView_User.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView_User.Location = new System.Drawing.Point(0, 0);
            this.myDataGridView_User.Name = "myDataGridView_User";
            this.myDataGridView_User.ReadOnly = true;
            this.myDataGridView_User.RowHeadersVisible = false;
            this.myDataGridView_User.RowTemplate.Height = 23;
            this.myDataGridView_User.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.myDataGridView_User.ShowRowErrors = false;
            this.myDataGridView_User.Size = new System.Drawing.Size(811, 371);
            this.myDataGridView_User.TabIndex = 0;
            this.myDataGridView_User.DoubleClick += new System.EventHandler(this.myDataGridView_User_DoubleClick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 514);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PID服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView_User)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Run;
        private System.Windows.Forms.Label label_Mem;
        private System.Windows.Forms.Label label_RunTime;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private MyDataGridView myDataGridView_config;
        private System.Windows.Forms.Button button_AddConfig;
        private System.Windows.Forms.Button button_DeleteConfig;
        private System.Windows.Forms.Button button_Modify;
        private MyDataGridView myDataGridView_User;
        private System.Windows.Forms.Button button_DeleteUser;
        private System.Windows.Forms.Button button_ModifyUser;
        private System.Windows.Forms.Button button_AddUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_Max;
        private System.Windows.Forms.Label label_Port;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_IP;
        private System.Windows.Forms.Label label4;
        private PBPid.myControl.myListBox listBox_Online;
        private System.Windows.Forms.Label label_MainKey;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox textBox_Phone;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_Address;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_CustomerName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_myloginPass;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_myloginName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DateTimePicker dateTimePicker_BeginDate;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DateTimePicker dateTimePicker_EndDate;
        private System.Windows.Forms.TextBox textBox_CustomerDesc;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBox_Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_LocalIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_Ip;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_Port;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_ConfigName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_ConfigPass;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_ConfigSi;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_CheckingType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_OfficeNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Original_CorporationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

