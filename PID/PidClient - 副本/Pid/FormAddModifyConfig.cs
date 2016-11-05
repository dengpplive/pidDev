using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using PBPid.ManageSpace;
using PBPid.DBModel;


namespace PBPid
{
    public partial class FormAddModifyConfig : Form
    {
        #region 变量

        //是否修改
        bool IsModify = false;
        //要修改的数据
        Base_OriginalConfigManage _bocm_temp = new Base_OriginalConfigManage();

        #endregion

        #region 构造

        /// <summary>
        /// 构造为添加方式
        /// </summary>
        public FormAddModifyConfig()
        {
            InitializeComponent();
            Load += new EventHandler(FormAddConfig_Load);
        }

        /// <summary>
        /// 构造为修改方式
        /// </summary>
        /// <param name="bpt"></param>
        public FormAddModifyConfig(Base_OriginalConfigManage bpt)
        {
            InitializeComponent();
            _bocm_temp = bpt;
            IsModify = true;
            Load += new EventHandler(FormAddConfig_Load);
        }

        #endregion

        #region 初始化

        void FormAddConfig_Load(object sender, EventArgs e)
        {
            if (IsModify)          //是否为修改
            {
                LoadProtocolType();
                LoadEntityToControl();
                button_Add.Text = "确认修改";
            }
            else
            {
                LoadProtocolType();//加载协议类型
                button_Add.Text = "确认添加";
            }
        }

        void LoadEntityToControl()
        {
            label_ID.Text =_bocm_temp.Original_Id.ToString();
            textBox_Ip.Text = _bocm_temp.Original_Ip;
            textBox_office.Text =_bocm_temp.Original_OfficeNumber;
            textBox_port.Text = _bocm_temp.Original_Port.ToString();
            comboBox_type.Text = _bocm_temp.ProtocolType;
            textBox_corporationName.Text = _bocm_temp.Original_CorporationName;
            textBox_SI.Text = _bocm_temp.Original_ConfigSi;
            textBox_pass.Text = _bocm_temp.Original_ConfigPass;
            textBox_Name.Text = _bocm_temp.Original_ConfigName;
            textBox_Desc.Text = _bocm_temp.Original_Description;
            textBox_LocalIp.Text = _bocm_temp.Original_LocalIp;
            if (_bocm_temp.Original_Status=="启用")
            {
                rad_Enable.Checked = true;
            }
            else
            {
                rad_Unable.Checked = true;
            }
        }

        void LoadProtocolType()
        {
            //List<Base_ProtocolType> bpt = ManageSpace.Manage.Get_Base_ProtocolType();
            //foreach (Base_ProtocolType item in bpt)
            //    comboBox_type.Items.Add(item.ProtocolType);

            comboBox_type.SelectedIndex = 0;
        }

        #endregion

        #region 添加、修改

        private void button_Add_Click(object sender, EventArgs e)
        {
            Base_OriginalConfigManage m = GetValue();
            if (m==null)
                return;
            

            if (!IsModify)//添加
            {
                bool _bool = ManageSpace.Manage.Add_Base_OriginalConfigManage(m);
                if (_bool)
                {
                    MessageBox.Show("添加成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
                else
                    MessageBox.Show("添加失败！" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else//修改
            {
                bool _bool = ManageSpace.Manage.Modify_Base_OriginalConfigManage(m);
                if (_bool)
                {
                    MessageBox.Show("修改成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                    MessageBox.Show("修改失败!" + ManageSpace.Manage.Error_Messages, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                IsModify = false;
            }
        }

        #endregion

        #region 私有方法

        void Clear()
        {
            textBox_corporationName.Text = string.Empty;
            textBox_Desc.Text = string.Empty;
            textBox_Ip.Text = string.Empty;
            textBox_Name.Text = string.Empty;
            textBox_office.Text = string.Empty;
            textBox_pass.Text = string.Empty;
            textBox_port.Text = string.Empty;
            textBox_SI.Text = string.Empty;
            textBox_LocalIp.Text = string.Empty;
            rad_Enable.Checked = true;
            comboBox_type.SelectedIndex = 0;
            label_ID.Text = "0";
        }

        Base_OriginalConfigManage GetValue()
        {
            #region 验证部分
            string errMsg = "";

            //配置名匹配
            string AccountPatern = @"^\w{1,17}$";
            string IPPattern = @"((?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d))";
            string OfficePattern = @"^\w{3}\d{3}$";


            if (string.IsNullOrEmpty(textBox_Name.Text.Trim()))
            {
                errMsg = "配置名不能为空";
            }
            else if (string.IsNullOrEmpty(textBox_pass.Text.Trim()))
            {
                errMsg = "配置密码不能为空";
            }
            else if (string.IsNullOrEmpty(textBox_Ip.Text.Trim()))
            {
                errMsg = "服务器IP不能为空";
            }
            else if (string.IsNullOrEmpty(textBox_port.Text.Trim()))
            {
                errMsg = "端口号不能为空";
            }
            else if (string.IsNullOrEmpty(textBox_office.Text.Trim()))
            {
                errMsg = "Office号不能为空";
            }
            //else if (string.IsNullOrEmpty(textBox_SI.Text.Trim()))
            //{
            //    errMsg = "Si不能为空";
            //}
            else if (comboBox_type.Items.Count == 0)
            {
                errMsg = "请选择一项协议类型";
            }


            if (errMsg != "")
            {
                MessageBox.Show(errMsg, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            else
            {
                #region 验证
                if (!Regex.IsMatch(textBox_Name.Text.Trim(), AccountPatern, RegexOptions.IgnoreCase))
                {
                    errMsg = "配置名输入格式错误";
                }
                //else if (!Regex.IsMatch(textBox_Ip.Text.Trim(), IPPattern))
                //{
                //    errMsg = "服务器IP或者本机IP输入格式错误";
                //}
                else if (!Regex.IsMatch(textBox_office.Text.Trim(), OfficePattern, RegexOptions.IgnoreCase))
                {
                    errMsg = "输入Office格式错误";
                }
                if (errMsg != "")
                {
                    MessageBox.Show(errMsg, "提示", MessageBoxButtons.OK);
                    return null;
                }
                if (Regex.IsMatch(textBox_port.Text.Trim(), @"^\d+$", RegexOptions.IgnoreCase))
                {
                    if (int.Parse(textBox_port.Text.Trim()) > 65535 || int.Parse(textBox_port.Text.Trim()) < 0)
                    {
                        errMsg = "输入端口号无效";
                    }
                }
                else
                {
                    errMsg = "输入端口号格式错误";
                }

                if (errMsg != "")
                {
                    MessageBox.Show(errMsg, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
                #endregion

            #endregion

            Base_OriginalConfigManage m = new Base_OriginalConfigManage();
            m.Original_Ip = textBox_Ip.Text;
            m.Original_OfficeNumber = textBox_office.Text;
            m.Original_Port = int.Parse(textBox_port.Text);
            m.ProtocolType = comboBox_type.SelectedItem.ToString();
            m.Original_CorporationName = textBox_corporationName.Text;
            m.Original_ConfigSi = textBox_SI.Text;
            m.Original_ConfigPass = textBox_pass.Text;
            m.Original_ConfigName = textBox_Name.Text;
            m.Original_Description = textBox_Desc.Text;
            m.Original_LocalIp = textBox_LocalIp.Text;

            if (rad_Enable.Checked)
            {
                m.Original_Status = "启用";
            }
            else
            {
                m.Original_Status = "禁用";
            }


            if (Tag != null)
            {
                m.CustomerID = int.Parse(Tag.ToString());
            }
            if(IsModify==true)
                m.Original_Id = int.Parse(label_ID.Text);

            return m;
        }

        #endregion

        #region 关闭

        private void button_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
