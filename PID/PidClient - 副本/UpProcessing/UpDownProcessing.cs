using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

using PBPid.CacheManageSpace;
using PBPid.ConfigManageSpace;
using PBPid.ManageSpace;

namespace PBPid.UpDownProcessingSpace
{
    /// <summary>
    /// 下文处理类，接协议层返回的明文指令
    /// </summary>
    public class UpDownProcessing
    {
        /// <summary>
        /// 上下文处理类构造
        /// </summary>
        /// <param name="userToPidSocket">用户到PID连接的SOCKET</param>
        /// <param name="cmd">发送的命令</param>
        public UpDownProcessing(Socket userToPidSocket, string user_Name, string user_Pass, string cmd)
        {


            //接收返回的Socket
            UserSocketCache _user = new UserSocketCache(
                user_Name,
                user_Pass,
                userToPidSocket,
                0);//此处标识为数据库得到的用户主键

            //查数据库，找出指定的配置
            //加入缓存


            UserCmdCache _cmdCache = new UserCmdCache(cmd, 0, 0);

            _user.UserToPidSocket = userToPidSocket;
           // _cacheManage.AddUserSocketCache(_user); //添加用户到缓存
           // _cacheManage.AddUserCmdCache(_cmdCache);//添加命令到缓存
        }
        
    }
}
