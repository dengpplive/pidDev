using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    /// <summary>
    /// 放大用户配置表
    /// </summary>
    [Serializable]
    public class Base_UserConfigManage
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int User_Id { get; set; }

        /// <summary>
        /// 客户关联主键
        /// </summary>
        public int Customer_Id { get; set; }

        /// <summary>
        /// 归属Office
        /// </summary>
        public string User_Office { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string User_Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string User_Pass { get; set; }
        /// <summary>
        /// 禁用指令
        /// </summary>
        public string User_DisableCmd { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool User_Disable { get; set; }
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime User_BeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime User_EndDate { get; set; }
        /// <summary>
        /// 发送指令次数
        /// </summary>
        public int User_SendCount { get; set; }
        /// <summary>
        /// 备注、说明
        /// </summary>
        public string User_Description { get; set; }

        /// <summary>
        /// 发送指令限制条数
        /// </summary>
        public int User_LimitCount { get; set; }

        /// <summary>
        /// 是否限制发送指令条数
        /// </summary>
        public bool User_LimitFlag { get; set; }
    }
}
