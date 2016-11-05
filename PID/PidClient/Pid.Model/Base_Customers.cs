using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    /// <summary>
    /// 客户信息
    /// </summary>
    [Serializable]
    public class Base_Customers
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string CustomerLoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string CustomerLoginPass { get; set; }
        /// <summary>
        /// 客户名字
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string CustomerAddress { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string CustomerPhone { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime CustomerBeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime CustomerEndDate { get; set; }
        /// <summary>
        /// 购买价格
        /// </summary>
        public int CustomerPrice { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string CustomerDescription { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public string CustomerLogIp { get; set; }

    }
}
