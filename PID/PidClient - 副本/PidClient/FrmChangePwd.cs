using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PidClient
{
    public partial class FrmChangePwd : Form
    {
        public FrmChangePwd()
        {
            InitializeComponent();
        }

        //修改密码
        private void button1_Click(object sender, EventArgs e)
        {
            //判断旧密码是否正确
            if (textBox1.Text != PublicClass.curUserPwd)
            {
                MessageBox.Show("旧密码不正确，请重新输入！");
                textBox1.Text = "";
                textBox1.Focus();
                return;
            }

            //判断两次新密码是否一致
            if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("两次输入的新密码不一致，请重新输入！");
                textBox2.Text = "";
                textBox3.Text = "";
                textBox2.Focus();
                return;
            }

            //发送修改密码指令

            //提示结果

        }

        private void FrmChangePwd_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
