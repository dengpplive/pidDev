using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace PBPid.CacheManageSpace
{
    /***************************************************************************
    * 说明：缓存管理类
    * 对缓存的查找、增加、删除、修改
    *       
    * 编写人：罗俊杰 2012-5-25
    * 
    * 修改人：
    * 
    * *************************************************************************/

    /// <summary>
    /// 缓存管理类，独立于其他类，
    /// </summary>
    public class CacheManage
    {

        #region 变量

        private List<UserSocketCache> _userSocketCaches = new List<UserSocketCache>();
        private List<UserCmdCache> _userCmdCaches = new List<UserCmdCache>();
        private List<ConfigSocketCache> _configSocketCaches = new List<ConfigSocketCache>();
        private List<AllCmdCache> _allCmdCaches = new List<AllCmdCache>();

        #endregion

        #region 获取缓存相关信息

        /// <summary>
        /// 获取用户SOCKET缓存区全部数据
        /// </summary>
        /// <returns></returns>
        public List<UserSocketCache> GetAllUserSocketCache()
        {
            return _userSocketCaches;
        }

        /// <summary>
        /// 获取命令缓存区全部数据
        /// </summary>
        /// <returns></returns>
        public List<UserCmdCache> GetAllUserCmdCache()
        {
            return _userCmdCaches;
        }

        /// <summary>
        /// 获取配置缓存区全部数据
        /// </summary>
        /// <returns></returns>
        public List<ConfigSocketCache> GetAllConfigSocketCache()
        {
            return _configSocketCaches;
        }

        /// <summary>
        /// 获取所发送过的全部指令
        /// </summary>
        /// <returns></returns>
        public List<AllCmdCache> GetAllCmdCache()
        {
            return _allCmdCaches;
        }

        /// <summary>
        /// 根据命令缓存中的用户KEY,获取用户socket缓存，获取下文属性
        /// </summary>
        /// <param name="UserCmdCache_UserKey">命令缓存中的用户KEY</param>
        /// <returns></returns>
        public UserSocketCache GetUserSocketCacheByUserKey(int userCmdCache_UserSocketCache_UserKey)
        {
            return _userSocketCaches.Find(delegate(UserSocketCache _s)
            {
                if (_s.UserSocketCache_Key == userCmdCache_UserSocketCache_UserKey)
                    return true;
                else
                    return false;
            });
        }

         ///<summary>
         /// 根据命令缓存中的配置KEY，返回配置SOCKET缓存，获取上文属性
         ///</summary>
         ///<param name="userCmdCache_ConfigSocketCache_Key">命令缓存中的配置KEY</param>
         ///<returns></returns>
        public ConfigSocketCache GetConfigSocketCacheByKey(int userCmdCache_ConfigSocketCache_Key)
        {
            return _configSocketCaches.Find(delegate(ConfigSocketCache _s)
            {
                if (_s.ConfigSocketCache_Key == userCmdCache_ConfigSocketCache_Key)
                    return true;
                else
                    return false;
            });
        }

        /// <summary>
        /// 根据命令，返回命令缓存
        /// </summary>
        /// <param name="Cmd">当前命令</param>
        /// <returns></returns>
        public List<UserCmdCache> GetUserCmdCacheByCmd(string Cmd)
        {
            return _userCmdCaches.FindAll(delegate(UserCmdCache _s)
            {
                if (_s.CurrentCmd == Cmd)
                    return true;
                else
                    return false;
            });
        }
        /// <summary>
        /// 根据命令、用户KEY，返回命令缓存
        /// </summary>
        /// <param name="Cmd">当前命令</param>
        /// <returns></returns>
        public List<UserCmdCache> GetUserCmdCacheByCmd(string Cmd, int UserSocketCache_Key)
        {
            return _userCmdCaches.FindAll(delegate(UserCmdCache _s)
            {
                if (_s.CurrentCmd == Cmd && _s.UserSocketCache_Key == UserSocketCache_Key)
                    return true;
                else
                    return false;
            });
        }

        /// <summary>
        /// 精确查找，根据命令、用户KEY、配置KEY，返回命令缓存
        /// </summary>
        /// <param name="Cmd">当前命令</param>
        /// <returns></returns>
        public UserCmdCache GetUserCmdCacheByCmd(string Cmd, int UserSocketCache_Key,int ConfigSocketCache_Key)
        {
            return _userCmdCaches.Find(delegate(UserCmdCache _s)
            {
                if (_s.CurrentCmd == Cmd && _s.UserSocketCache_Key == UserSocketCache_Key && _s.ConfigSocketCache_Key == ConfigSocketCache_Key)
                    return true;
                else
                    return false;
            });
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加用户命令缓存,同时会记录到“全部命令缓存区”
        /// </summary>
        /// <param name="_userCmdCache"></param>
        public void AddUserCmdCache(UserCmdCache _userCmdCache)
        {
            AllCmdCache all = new AllCmdCache();
            all.cmd = _userCmdCache.CurrentCmd;
            all.ConfigSocketCache_Key = _userCmdCache.ConfigSocketCache_Key;
            all.UserSocketCache_Key = _userCmdCache.UserSocketCache_Key;

            this._userCmdCaches.Add(_userCmdCache);
            this._allCmdCaches.Add(all);
        }

        /// <summary>
        /// 添加配置SOCKET缓存
        /// </summary>
        /// <param name="_configSocketCache"></param>
        public void AddConfigSocketCache(ConfigSocketCache _configSocketCache)
        {
            this._configSocketCaches.Add(_configSocketCache);
        }

        /// <summary>
        /// 添加用户SOCKET缓存
        /// </summary>
        /// <param name="_userSocketCache"></param>
        public void AddUserSocketCache(UserSocketCache _userSocketCache)
        {
            this._userSocketCaches.Add(_userSocketCache);
        }

        #endregion


        #region 修改缓存

        /// <summary>
        /// 修改原始配置属性
        /// </summary>
        /// <param name="_userCmdCache"></param>
        /// <returns></returns>
        public ConfigSocketCache ModifyConfigSocketCache(ConfigSocketCache _configSocketCache)
        {
            int i = _configSocketCaches.FindIndex(delegate(ConfigSocketCache _s)
            {
                if (_s.ConfigSocketCache_Key == _configSocketCache.ConfigSocketCache_Key)
                    return true;
                else
                    return false;
            });

            _configSocketCaches[i - 1] = _configSocketCache;
            return _configSocketCaches[i - 1];
        }


        #endregion


        #region 删除

        /// <summary>
        /// 删除用户SOCKET缓存区中的一个用户
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public bool DeleteUserSocketCache(UserSocketCache _user)
        {
            return _userSocketCaches.Remove(_user);
        }


        #endregion

    }
}
