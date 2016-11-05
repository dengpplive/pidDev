using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace PBPid.CacheManageSpace
{
    /***************************************************************************
   * 说明：配置缓存区类
   * 配置缓存区实体模型，存储PID到航信连接的信息
   *       
   * 编写人：罗俊杰 2012-5-24
   * 
   * 修改人：
   * 
   * *************************************************************************/
    /// <summary>
    /// 配置缓存区，PID连接到航信的Socket
    /// 上文信息
    /// </summary>
    [Serializable]
    public class ConfigSocketCache
    {
        /// <summary>
        /// 唯一标识，用数据库主键
        /// </summary>
        public int ConfigSocketCache_Key = 0;

        /// <summary>
        /// 该配置发了几次指令
        /// </summary>
        public int SendCount = 0;

        /// <summary>
        /// 成功连接配置的配置名
        /// </summary>
        public string Config_Name = string.Empty;

        /// <summary>
        /// 已连接的配置,PID到航信的连接套接字
        /// </summary>
        public Socket PidToH = null;

        /// <summary>
        /// 空闲状态,True为空闲，False为占用
        /// 该状态为套接字在发送的时候变更为占用，接收到后变更为空闲
        /// </summary>
        public bool IdleStatus = true;

        ///// <summary>
        ///// 构造配置缓存区
        ///// </summary>
        ///// <param name="_config_Name">配置名称</param>
        ///// <param name="_pidToH">PID到航信连接的Socket</param>
        //public ConfigSocketCache(string _config_Name, Socket _pidToH)
        //{
        //    Config_Name = _config_Name;
        //    PidToH = _pidToH; 
        //}
    }
}
