using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace PidClient
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        //取消
        private void button2_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        //登录验证
        private void button1_Click(object sender, EventArgs e)
        {
            string errorMsg = "";
            PublicClass.curServerIp = comboBox1.Text;
            PublicClass.curUserName = textBox1.Text;
            PublicClass.curUserPwd = textBox2.Text;

            //帐号、密码不能为空
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("用户名或密码不能为空！");
                return;
            }


            //
            if (PublicClass.ConnectServer(ref errorMsg))
            {
                //认证成功
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(errorMsg);
            }
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始服务器列表
            PublicClass.curServerIp = ConfigurationManager.AppSettings["ServerIp"];
            PublicClass.curServerPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            comboBox1.Items.Add(PublicClass.curServerIp);
            comboBox1.SelectedIndex = 0;

            textBox1.Focus();
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox2.Focus();
            }
        }
    }
}
