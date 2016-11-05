using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.CacheManageSpace
{
    /// <summary>
    /// 全部命令缓存区,每发一次指令都会记录
    /// </summary>
    public class AllCmdCache
    {
        /// <summary>
        /// 命令
        /// </summary>
        public string cmd = string.Empty;
        /// <summary>
        /// 用户缓存区主键
        /// </summary>
        public int UserSocketCache_Key = 0;
        /// <summary>
        /// 配置缓存区主键
        /// </summary>
        public int ConfigSocketCache_Key = 0;
    }
}
