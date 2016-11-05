using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    /// <summary>
    /// 协议类型
    /// </summary>
    [Serializable]
    public class Base_ProtocolType
    {
        public int Protocol_Id { get; set; }
        public string ProtocolType { get; set; }
    }
}
