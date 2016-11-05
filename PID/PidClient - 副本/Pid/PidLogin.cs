using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PBPid.DBModel;
using System.Xml;
using System.IO;

namespace PBPid
{
    public partial class PidLogin : Form
    {
        //客户信息
        public Base_Customers currentUser;

        //认证服务器IP
        public string ServerIp;

        //认证服务器ip
        private List<string> serverlist=new List<string>();

        //是否md5加密密码
        private bool md5flag=false;

        //是否已经登录标志
        private bool IfLogin = false;

        public PidLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pwd = "";
            string Name = string.Empty, Pass = string.Empty;
            Name = textBox_UserName.Text;
            Pass = textBox_Pwd.Text;

            if (!md5flag)
            {
                pwd = PBPid.Base.AppGlobal.EncryptMD5(PBPid.Base.AppGlobal.EncryptMD5(textBox_Pwd.Text) + "cgt");
            }
            else
            {
                pwd = Pass;
            }

            if (Name.Length == 0)
            {
                MessageBox.Show("请填写登录名!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Pass.Length == 0)
            {
                MessageBox.Show("请填写登录密码!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool IsCon = false;//是否连接
            ServerIp = serverlist[comboBox_ServerList.SelectedIndex];
            ManageSpace.Manage.m_ServerIp = ServerIp;
            currentUser = ManageSpace.Manage.Login(Name, pwd, ref IsCon);

            if (IsCon == false)
            {
                MessageBox.Show("与认证服务器连接失败!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (currentUser == null)
            {
                MessageBox.Show("登录失败!帐号或密码不正确！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            //记录密码(保存到本地文件) 
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists("sysinfo.xml"))
            {
                xmlDoc.Load("sysinfo.xml");

                XmlNode root = xmlDoc.SelectSingleNode("EtermClient");
                XmlNode xe1 = root.SelectSingleNode("LoginInfo");
                XmlElement xe2 = (XmlElement)xe1;
                if (checkBox_SavePwd.Checked)
                {
                    xe2.SetAttribute("savepwd", "1");
                }
                else
                {
                    xe2.SetAttribute("savepwd", "0");
                }

                if (checkBox_AutoLogin.Checked)
                {
                    xe2.SetAttribute("autologin", "1");
                }
                else
                {
                    xe2.SetAttribute("autologin", "0");
                }

                xe2.SetAttribute("staffid", Name);
                xe2.SetAttribute("pwd", pwd);               

                xmlDoc.Save("sysinfo.xml");
            }
            else
            {
                //创建配置文件

                //加入XML的声明段落
                
                XmlNode xn1 = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmlDoc.AppendChild(xn1);

                XmlElement xe1 = xmlDoc.CreateElement("", "EtermClient", "");
                xmlDoc.AppendChild(xe1);

                XmlElement xe2 = xmlDoc.CreateElement("", "LoginInfo", "");
                if (checkBox_SavePwd.Checked)
                {
                    xe2.SetAttribute("savepwd", "1");
                }
                else
                {
                    xe2.SetAttribute("savepwd", "0");
                }

                if (checkBox_AutoLogin.Checked)
                {
                    xe2.SetAttribute("autologin", "1");
                }
                else
                {
                    xe2.SetAttribute("autologin", "0");
                }

                xe2.SetAttribute("staffid", Name);
                xe2.SetAttribute("pwd", pwd);                 
                xmlDoc.ChildNodes.Item(1).AppendChild(xe2);

                XmlElement xe3 = xmlDoc.CreateElement("", "ServerInfo1", "");
                xe3.SetAttribute("servername", "认证服务器一");
                xe3.SetAttribute("serverip", "203.88.210.234");
                xmlDoc.ChildNodes.Item(1).AppendChild(xe3);

                XmlElement xe4 = xmlDoc.CreateElement("", "ServerInfo2", "");
                xe4.SetAttribute("servername", "认证服务器二");
                xe4.SetAttribute("serverip", "203.88.210.227");
                xmlDoc.ChildNodes.Item(1).AppendChild(xe4);

                XmlElement xe5 = xmlDoc.CreateElement("", "ServerInfo3", "");
                xe5.SetAttribute("servername", "认证服务器三");
                xe5.SetAttribute("serverip", "121.12.124.37");
                xmlDoc.ChildNodes.Item(1).AppendChild(xe5);

                XmlElement xe6 = xmlDoc.CreateElement("", "ServerInfo4", "");
                xe6.SetAttribute("servername", "认证服务器四");
                xe6.SetAttribute("serverip", "120.64.252.142");
                xmlDoc.ChildNodes.Item(1).AppendChild(xe6);

                //<ServerInfo2 servername="服务器2" serverip="121.12.124.37" />

                xmlDoc.Save("sysinfo.xml");
            }

            IfLogin = true;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Application.Exit();
        }

        private void FormLog_Load(object sender, EventArgs e)
        {
            string username = "";
            string pwd = "";

            //记录密码(保存到本地文件) 
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists("sysinfo.xml"))
            {
                xmlDoc.Load("sysinfo.xml");

                XmlNode root = xmlDoc.SelectSingleNode("EtermClient");
                XmlNode xe1 = root.SelectSingleNode("LoginInfo");
                XmlElement xe2 = (XmlElement)xe1;

                textBox_UserName.Text = xe2.GetAttribute("staffid");
                textBox_Pwd.Text = xe2.GetAttribute("pwd");

                md5flag = true;

                if (xe2.GetAttribute("savepwd") == "1")
                {
                    checkBox_SavePwd.Checked = true;
                }
                else
                {
                    checkBox_SavePwd.Checked = false;
                }

                if (xe2.GetAttribute("autologin") == "1")
                {
                    checkBox_AutoLogin.Checked = true;
                }
                else
                {
                    checkBox_AutoLogin.Checked = false;
                }

                int index = 1;
                while (root.SelectSingleNode("ServerInfo" + index.ToString()) != null)
                {
                    XmlNode xe3 = root.SelectSingleNode("ServerInfo" + index.ToString());
                    XmlElement xe4 = (XmlElement)xe3;
                    //获取认证服务器信息
                    comboBox_ServerList.Items.Add(xe4.GetAttribute("servername"));
                    serverlist.Add(xe4.GetAttribute("serverip"));
                    index++;
                }

                if (index != 1)
                {
                    comboBox_ServerList.SelectedIndex = 0;
                }
            }
            else
            {
                //添加默认服务器列表
                comboBox_ServerList.Items.Add("认证服务器一");
                comboBox_ServerList.Items.Add("认证服务器二");
                comboBox_ServerList.Items.Add("认证服务器三");
                comboBox_ServerList.Items.Add("认证服务器四");
                comboBox_ServerList.SelectedIndex = 0;
                serverlist.Add("203.88.210.234");
                serverlist.Add("203.88.210.227");
                serverlist.Add("121.12.124.37");
                serverlist.Add("120.64.252.142");
            }

            timer1.Enabled = true;
        }

        private void textBox_Pwd_TextChanged(object sender, EventArgs e)
        {
            md5flag = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((!IfLogin) && (checkBox_AutoLogin.Checked))
            {
                button1_Click(sender, e);
                timer1.Enabled = false;
            }
        }
    }
}
