using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    /// <summary>
    /// 指令库表
    /// </summary>
    [Serializable]
    public class Base_CommandManage
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Cmd_Id { get; set; }
        /// <summary>
        /// 指令
        /// </summary>
        public string Cmd_Command { get; set; }
        /// <summary>
        /// true 为有上文，false 为无上文
        /// </summary>
        public bool Cmd_IsUp { get; set; }
        /// <summary>
        /// true 为有下文，false 为无下文
        /// </summary>
        public bool Cmd_IsDown { get; set; }
        /// <summary>
        /// 上文主键
        /// </summary>
        public string Cmd_UpCommand { get; set; }
        /// <summary>
        /// 下文主键
        /// </summary>
        public string Cmd_DownCommand { get; set; }
        
    }
}
