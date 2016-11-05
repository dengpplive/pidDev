using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.CacheManageSpace
{
    /***************************************************************************
    * 说明：用户指令缓存区
    * 根据该类中的当前指令，可以找到是谁发送过来的，并且调用哪个配置发送！
    *       
    * 编写人：罗俊杰 2012-5-25
    * 
    * 修改人：
    * 
    * *************************************************************************/
    /// <summary>
    /// 用户指令缓存区
    /// </summary>
    [Serializable]
    public class UserCmdCache
    {
        /// <summary>
        /// 指向一个用户缓存
        /// </summary>
        public int UserSocketCache_Key = 0;

        /// <summary>
        /// 指向一个原始配置
        /// </summary>
        public int ConfigSocketCache_Key = 0;

        /// <summary>
        /// 用户当前发送的指令
        /// </summary>
        public string CurrentCmd = string.Empty;

        /// <summary>
        /// 用户发送的上一条指令
        /// </summary>
        public string LastCmd = string.Empty;

        /// <summary>
        /// 装载到用户缓存
        /// </summary>
        /// <param name="currentCmd">当前指令</param>
        /// <param name="userSocketCache_Key">用户SOCKET缓存主键,用数据库主键来标识</param>
        /// <param name="configSocketCache_Key">配置SOCKET缓存主键,用数据库主键来标识</param>
        public UserCmdCache(string currentCmd, int userSocketCache_Key, int configSocketCache_Key)
        {
            CurrentCmd = currentCmd;
            UserSocketCache_Key = userSocketCache_Key;
            ConfigSocketCache_Key = configSocketCache_Key;
        }
    }
}
