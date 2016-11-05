using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace PBPid.CacheManageSpace
{
    /// <summary>
    /// 用户Socket缓存,缓存用户到PID的连接信息，也就是下文信息
    /// </summary>
    public class UserSocketCache
    {
        /// <summary>
        /// 唯一标识，用数据库主键
        /// </summary>
        public int UserSocketCache_Key = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName = string.Empty;

        /// <summary>
        /// 该用户发送了几次指令
        /// </summary>
        public int SendCount = 0;

        /// <summary>
        /// 密码
        /// </summary>
        public string UserPass = string.Empty;

        /// <summary>
        /// 用户到PID的Socket
        /// </summary>
        public Socket UserToPidSocket = null;


        public UserSocketCache(string _userName, string _userPass, Socket _userToPidSocket, int _UserSocketCache_Key)
        {
            UserName = _userName;
            UserPass = _userPass;
            UserToPidSocket = _userToPidSocket;
            UserSocketCache_Key = _UserSocketCache_Key;
        }
    }
}
