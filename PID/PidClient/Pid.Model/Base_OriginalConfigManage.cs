using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    /// <summary>
    /// 原始配置表
    /// </summary>
    [Serializable]
    public class Base_OriginalConfigManage
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Original_Id { get; set; }

        /// <summary>
        /// 本地IP地址
        /// </summary>
        public string Original_LocalIp { get; set; }

        /// <summary>
        /// 连接航信的IP地址
        /// </summary>
        public string Original_Ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Original_Port { get; set; }

        /// <summary>
        /// 配置状态（禁用 /启用 ）
        /// </summary>
        public string Original_Status { get; set; }

        /// <summary>
        /// 原始配置名
        /// </summary>
        public string Original_ConfigName { get; set; }
        /// <summary>
        /// 配置密码
        /// </summary>
        public string Original_ConfigPass { get; set; }
        /// <summary>
        /// 工作号/密码
        /// </summary>
        public string Original_ConfigSi { get; set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        public string ProtocolType { get; set; }
        /// <summary>
        /// 认证类型、验证
        /// </summary>
        public string Original_CheckingType { get; set; }
        /// <summary>
        /// OFFICE号
        /// </summary>
        public string Original_OfficeNumber { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Original_CorporationName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Original_Description { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerID { get; set; }
    }
}
