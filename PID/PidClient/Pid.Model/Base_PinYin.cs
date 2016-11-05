using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    /// <summary>
    /// 汉字拼音编码信息
    /// </summary>
    [Serializable]
    public class Base_PinYin
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 汉字
        /// </summary>
        public string HanZi { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string BianMa { get; set; }
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }        
    }
}
