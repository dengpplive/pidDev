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
    public partial class FrmLock : Form
    {
        public FrmLock()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //判断操作员密码是否正确
            if (textBox1.Text != PublicClass.curUserPwd)
            {
                MessageBox.Show("密码不正确，请重新输入！");
                textBox1.Text = "";
                textBox1.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void FrmLock_Load(object sender, EventArgs e)
        {
            label2.Text = PublicClass.curUserName;
            textBox1.Focus();
        }
    }
}
