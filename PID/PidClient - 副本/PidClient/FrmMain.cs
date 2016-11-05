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
    public partial class FrmMain : Form
    {
        //白屏预订窗口
        public FrmYuDing tmpFrmYuDing = new FrmYuDing();

        //选择舱位及输入乘机人窗口
        public FrmSelectAndYuDing tmpFrmSelectAndYuDing = new FrmSelectAndYuDing();

        //黑屏窗口
        private FrmBlack tmpFrmBlack = new FrmBlack();

        //修改密码窗口
        private FrmChangePwd tmpFrmChangePwd = new FrmChangePwd();

        //锁定窗口
        private FrmLock tmpFrmLock = new FrmLock();

        //关于窗口
        private FrmAbout tmpFrmAbout = new FrmAbout();


        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Hide();
            FrmLogin tmplogin = new FrmLogin();
            if (tmplogin.ShowDialog() == DialogResult.OK)
            {
                //认证成功                
            }
            else
            {
                Application.ExitThread();
            }

            toolStripStatusLabel2.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            toolStripStatusLabel4.Text = PublicClass.curUserName;
            toolStripStatusLabel6.Text = PublicClass.curServerIp;
            toolStripStatusLabel8.Text = PublicClass.curServerPort.ToString();
            this.Show();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //默认开启黑屏窗口并显示欢迎信息。
            Black_ToolStripMenuItem_Click(sender, e);
        }
            
        private void Exit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //释放资源
            PublicClass.Free();
            Application.ExitThread();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (tmpFrmLock.ShowDialog() == DialogResult.OK)
            {
                tmpFrmLock.Hide();
            }
        }

        private void About_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmpFrmAbout.ShowDialog();
        }

        //黑屏
        private void Black_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((tmpFrmBlack == null) || (tmpFrmBlack.IsDisposed))
            {
                tmpFrmBlack = new FrmBlack();
            }
            tmpFrmBlack.WindowState = FormWindowState.Maximized;
            tmpFrmBlack.MdiParent = this;
            tmpFrmBlack.Show();
            tmpFrmBlack.Focus();
        }

        //白屏
        private void White_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((tmpFrmYuDing == null) || (tmpFrmYuDing.IsDisposed))
            {
                tmpFrmYuDing = new FrmYuDing();
            }
            tmpFrmYuDing.tmpMainForm = this;
            tmpFrmYuDing.WindowState = FormWindowState.Maximized;
            tmpFrmYuDing.MdiParent = this;
            tmpFrmYuDing.Show();
            tmpFrmYuDing.Focus();
        }

        //修改密码
        private void ChangPwd_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tmpFrmChangePwd.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
