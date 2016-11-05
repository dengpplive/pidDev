using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PBPid.DBModel;

namespace PBPid
{
    public partial class FormAddModifyUser : Form
    {

        #region 变量

        //实体数据
        Base_UserConfigManage user_Temp = new Base_UserConfigManage();

        //客户ID
        public int curr_CustomerId = -1;

        //是否为修改
        bool IsModify = false;

        #endregion

        #region 构造

        /// <summary>
        /// 添加用户
        /// </summary>
        public FormAddModifyUser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user">要修改的用户实体</param>
        public FormAddModifyUser(Base_UserConfigManage user)
        {
            InitializeComponent();
            user_Temp = user;
            IsModify = true;
        }

        #endregion

        #region 加载

        private void FormAddUser_Load(object sender, EventArgs e)
        {
            if (!IsModify)//添加用户
            {
                LoadConfigName();
            }
            else
            {
                LoadConfigName();
                LoadControl(user_Temp);//加载到控件
                button_AddModify.Text = "确定修改";
            }
        }

        Dictionary<string, int> hash_str_int = new Dictionary<string, int>();
        Dictionary<int, string> hash_int_str = new Dictionary<int, string>();

        void LoadConfigName()
        {
            //hash_str_int = new Dictionary<string, int>();
            //hash_int_str = new Dictionary<int, string>();
            //List<Base_OriginalConfigManage> bocms = ManageSpace.Manage._Base_OriginalConfigManage;

            //foreach (Base_OriginalConfigManage item in bocms)
            //{
            //    comboBox_configName.Items.Add(item.Original_ConfigName);
            //    hash_str_int.Add(item.Original_ConfigName, item.Original_Id);//存入ID和名称的关系
            //    hash_int_str.Add(item.Original_Id, item.Original_ConfigName);
            //}
        }

        void LoadControl(Base_UserConfigManage m)//修改用户时加载
        {
            textBox_Office.Text = m.User_Office;
            textBox_Desc.Text = m.User_Description;
            textBox_DisableCmd.Text = m.User_DisableCmd;
            textBox_UserName.Text = m.User_Name;
            textBox_UserPass.Text = m.User_Pass;
            if (m.User_Disable)
            {
                rad_Enable.Checked = true;
            }
            else
            {
                rad_Unable.Checked = true;
            }
            dateTimePicker_BeginDate.Value = m.User_BeginDate;
            dateTimePicker_EndDate.Value = m.User_EndDate;
            label_Id.Text = m.User_Id.ToString();
        }

        #endregion

        #region 添加、修改

        private void button_AddModify_Click(object sender, EventArgs e)
        {
            if (textBox_UserName.Text.Length == 0 || textBox_UserPass.Text.Length == 0 || textBox_Office.Text.Length == 0)
            {
                MessageBox.Show("请检查输入格式，关键属性不能为空！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (button_AddModify.Text == "确定添加")//添加
            {
                if (ManageSpace.Manage.Add_Base_UserConfigManage(GetEntity()))
                {
                    MessageBox.Show("添加成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
                else
                    MessageBox.Show("添加失败，请重试！" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else//修改
            {
                if (ManageSpace.Manage.Modify_Base_UserConfigManage(GetEntity()))
                {
                    MessageBox.Show("修改成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
                else
                    MessageBox.Show("修改失败，请重试！" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        //从控件获取值，封装成实体返回
        Base_UserConfigManage GetEntity()
        {
            Base_UserConfigManage m = new Base_UserConfigManage();
            m.User_Office = textBox_Office.Text;
            if (IsModify == true)
                m.User_Id = int.Parse(label_Id.Text);

            m.User_Description = textBox_Desc.Text;
            m.User_DisableCmd = textBox_DisableCmd.Text;
            m.User_Name = textBox_UserName.Text;
            m.User_Pass = textBox_UserPass.Text;
            if (rad_Enable.Checked)
            {
                m.User_Disable = true;
            }
            else
            {
                m.User_Disable = false;
            }
            
            m.User_BeginDate = dateTimePicker_BeginDate.Value;
            m.User_EndDate = dateTimePicker_EndDate.Value;
            m.Customer_Id = curr_CustomerId;
            
            return m;
        }
        private void button_close_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(textBox_UserPass.Text);
        }

      
    }
}
