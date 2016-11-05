using System;
using System.Windows.Forms;
using PBPid.UpDownProcessingSpace;

namespace 测试模块功能程序
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 数据

        // 获取
        private void button1_Click(object sender, EventArgs e)
        {
           // dataGridView1.DataSource = _ConfigManage.Get_Base_CommandManage();
        }

        //获取配置数据
        private void button5_Click(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = _ConfigManage.Get_Base_OriginalConfigManage();
        }

        //增加原始配置数据
        private void button6_Click(object sender, EventArgs e)
        {
            //Base_OriginalConfigManage ocm = new Base_OriginalConfigManage();
            //ocm.Original_ConfigName = "text";

            //bool _bool = _ConfigManage.Add_Base_OriginalConfigManage(ocm);
            //_ConfigManage.RefreshCache_Base_OriginalConfigManage();

            //if (_bool)
            //    MessageBox.Show("成功");
            //dataGridView1.DataSource = _ConfigManage.Get_Base_OriginalConfigManage();
        }

        //删除原始配置
        private void button7_Click(object sender, EventArgs e)
        {
            //Base_OriginalConfigManage ocm = new Base_OriginalConfigManage();
            //ocm.Original_Id = 1;

            //bool _bool = _ConfigManage.Delete_Base_OriginalConfigManage(ocm);
            //_ConfigManage.RefreshCache_Base_OriginalConfigManage();

            //if (_bool)
            //    MessageBox.Show("成功");
            //dataGridView1.DataSource = _ConfigManage.Get_Base_OriginalConfigManage();
        }

        //修改原始配置
        private void button8_Click(object sender, EventArgs e)
        {
            //Base_OriginalConfigManage ocm = new Base_OriginalConfigManage();
            //ocm.Original_Id = 1;
            //ocm.Original_ConfigName = "ljj";

            //bool _bool = _ConfigManage.Modify_Base_OriginalConfigManage(ocm);
            //_ConfigManage.RefreshCache_Base_OriginalConfigManage();

            //if (_bool)
            //    MessageBox.Show("成功");
            //dataGridView1.DataSource = _ConfigManage.Get_Base_OriginalConfigManage();
        }

        //获取用户数据
        private void button9_Click(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = _ConfigManage.Get_Base_UserConfigManage();
        }
        //添加用户数据
        private void button10_Click(object sender, EventArgs e)
        {
            //List<Base_UserConfigManage> ocms = new List<Base_UserConfigManage>();
            //Base_UserConfigManage ocm = new Base_UserConfigManage();
            //ocm.User_Description = "text";
            //ocm.User_BeginDate = DateTime.Now;
            //ocm.User_EndDate = DateTime.Now;
            //ocms.Add(ocm);

            //DBModelBLL bll = new DBModelBLL(DBModelBLL.db_Cmd.增加, ocms, "Base_UserConfigManage");

            //bool _bool = _ConfigManage.AddData(bll);
            //if (_bool)
            //    MessageBox.Show("成功");
        }

        //删除用户数据
        private void button11_Click(object sender, EventArgs e)
        {
            //List<Base_UserConfigManage> ocms = new List<Base_UserConfigManage>();
            //Base_UserConfigManage ocm = new Base_UserConfigManage();
            //ocm.User_Id = 5;
            //ocms.Add(ocm);

            //DBModelBLL bll = new DBModelBLL(DBModelBLL.db_Cmd.删除, ocms, "Base_UserConfigManage");
            //bool _bool = _ConfigManage.DeleteData(bll);
            //if (_bool)
            //    MessageBox.Show("成功");
        }

        //修改用户数据
        private void button12_Click(object sender, EventArgs e)
        {
            //List<Base_UserConfigManage> ocms = new List<Base_UserConfigManage>();
            //Base_UserConfigManage ocm = new Base_UserConfigManage();
            //ocm.User_Id = 7;
            //ocm.User_Description = "dsasddsfsdfsdfsdfsd";
            //ocm.User_BeginDate = DateTime.Now;
            //ocm.User_EndDate = DateTime.Now;
            //ocms.Add(ocm);

            //DBModelBLL bll = new DBModelBLL(DBModelBLL.db_Cmd.修改, ocms, "Base_UserConfigManage");
            //bool _bool = _ConfigManage.ModifyData(bll);
            //if (_bool)
            //    MessageBox.Show("成功");
        }
        #endregion

        private void button_test_Click(object sender, EventArgs e)
        {
           //UpDownProcessing up = new UpDownProcessing(
        }

    }
}
