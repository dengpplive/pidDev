using System;
using System.Collections.Generic;
using System.Text;

using PBPid.CacheManageSpace;
using PBPid.ConfigManageSpace;
using System.Configuration;
using PBPid.DBModel;
using System.Threading;

namespace PBPid.ManageSpace
{
    /***************************************************************************
    * 说明：远程数据、本地缓存管理层，所有的缓存都在此，配置管理中的套接字也只被加载一次，断开后有自动连接机制
    *       
    * 编写人：罗俊杰 2012-5-28
    * 
    * 修改人：
    * 
    * *************************************************************************/

    /// <summary>
    /// 所有的缓存都在此，配置管理中的套接字也只被加载一次，断开后有自动连接机制
    /// 当第一次加载后，直到关闭程序才会消失，且只会被加载一次。
    /// </summary>
    [Serializable]
    public static class Manage
    {
        #region 变量

        public static CacheManage cacheManage { get; set; }  //需要长时间保留
        public static ConfigManage configManage { get; set; }//有通信连接，所以不能再次初始化

        public delegate void Error(string Error_Message);
        public static event Error OnError;

        /// <summary>
        /// 加载后的原始配置缓存，会跟随修改、删除、增加而刷新
        /// </summary>
        public static List<Base_OriginalConfigManage> _Base_OriginalConfigManage = new List<Base_OriginalConfigManage>();

        /// <summary>
        /// 加载后的用户缓存，会跟随修改、删除、增加而刷新
        /// </summary>
        public static List<Base_UserConfigManage> _Base_UserConfigManage = new List<Base_UserConfigManage>();

        public static string Error_Messages = string.Empty;

        public static string m_ServerIp = "";

        #endregion

        #region 构造

        static Manage()
        {
        }

        static void CheckNet()
        {
            try
            {
                if (cacheManage == null)
                    cacheManage = new CacheManage();

                if (configManage == null)
                {
                    configManage = new ConfigManage(
                           m_ServerIp,//ConfigurationManager.AppSettings["REMOTEIP"].ToString(),
                      int.Parse(ConfigurationManager.AppSettings["RemoteSKPORT"].ToString()),
                      int.Parse(ConfigurationManager.AppSettings["RemoteRMPORT"].ToString())); //连接服务器套接字、REMOTING

                    configManage.OnError += new ConfigManage.Error(_configManage_OnError);     //错误处理
                }
            }
            catch { Thread.Sleep(1); }
        }

        static void _configManage_OnError(string Error_Message)
        {
            Error_Messages = Error_Message;
            if (OnError != null)
                OnError.BeginInvoke(Error_Message, null, null);

        }

        #endregion

        #region Base_OriginalConfigManage

        /// <summary>
        /// 加载原始配置表
        /// </summary>
        /// <param name="IsRefresh">是否需要刷新缓存</param>
        /// <returns></returns>
        public static List<Base_OriginalConfigManage> InitConfigInfoByCustomerId(int CustomerId)
        {
            try
            {
                _Base_OriginalConfigManage = configManage.Get_Base_OriginalConfigManageByCustomerID(CustomerId);
                return _Base_OriginalConfigManage;
            }
            catch (Exception ex)
            {
                Error_Messages = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 根据登录客户ID获取原始配置
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public static List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByCustomerId(int CustomerId)
        {
            return _Base_OriginalConfigManage.FindAll(delegate(Base_OriginalConfigManage bm)
            {
                if (bm.CustomerID == CustomerId)
                    return true;
                else
                    return false;
            });
        }

        /// <summary>
        /// 根据配置ID返回配置数据
        /// </summary>
        /// <param name="Original_Id"></param>
        /// <returns></returns>
        public static Base_OriginalConfigManage Get_Base_OriginalConfigManageByOriginal_Id(int Original_Id)
        {
            return _Base_OriginalConfigManage.Find(delegate(Base_OriginalConfigManage b)
            {
                if (b.Original_Id == Original_Id)
                    return true;
                else
                    return false;
            });
        }

        /// <summary>
        /// 删除一个原始配置
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Delete_Base_OriginalConfigManage(Base_OriginalConfigManage bm)
        {
            return configManage.Delete_Base_OriginalConfigManage(bm);
        }

        /// <summary>
        /// 增加一个原始配置
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Add_Base_OriginalConfigManage(Base_OriginalConfigManage bm)
        {
            return configManage.Add_Base_OriginalConfigManage(bm);
        }

        /// <summary>
        /// 修改一个配置
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Modify_Base_OriginalConfigManage(Base_OriginalConfigManage bm)
        {
            return configManage.Modify_Base_OriginalConfigManage(bm);
        }

        #endregion

        #region Base_UserConfigManage

        /// <summary>
        /// 根据客户ID返回绑定的放大用户
        /// </summary>
        /// <param name="Customer_Id">客户ID</param>
        /// <returns></returns>
        public static List<Base_UserConfigManage> Get_Base_UserConfigManageByCustomer_Id(int Customer_Id)
        {
            return configManage.Get_Base_UserConfigManageByCustomer_Id(Customer_Id);
        }

        ///// <summary>
        ///// 刷新远程用户缓存
        ///// </summary>
        ///// <returns></returns>
        //public static bool RefreshUserConfig()
        //{
        //    return manage.configManage.RefreshCache_Base_UserConfigManage();
        //}

        /// <summary>
        /// 根据用户ID获取用户配置
        /// </summary>
        /// <param name="User_id"></param>
        /// <returns></returns>
        public static Base_UserConfigManage Get_Base_UserConfigManageById(int User_id)
        {
            return configManage.Get_Base_UserConfigManageById(User_id);
        }

        /// <summary>
        /// 删除一条用户
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Delete_Base_UserConfigManage(Base_UserConfigManage bm)
        {
            return configManage.Delete_Base_UserConfigManage(bm);
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Delete_Base_UserConfigManageArray(List<Base_UserConfigManage> bm)
        {
            return configManage.Delete_Base_UserConfigManageArray(bm);
        }

        /// <summary>
        /// 添加一条用户
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Add_Base_UserConfigManage(Base_UserConfigManage bm)
        {
            return configManage.Add_Base_UserConfigManage(bm);
        }

        /// <summary>
        /// 批量添加用户
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Add_Base_UserConfigManageArray(List<Base_UserConfigManage> bm)
        {
            return configManage.Add_Base_UserConfigManageArray(bm);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Modify_Base_UserConfigManage(Base_UserConfigManage bm)
        {
            return configManage.Modify_Base_UserConfigManage(bm);
        }

        /// <summary>
        /// 修改用户发送指令条数
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool Modify_Base_UserSendCmdCount(Base_UserConfigManage bm)
        {
            return configManage.Modify_Base_UserSendCmdCount(bm);
        }
        #endregion

        #region 城市三字码及拼音信息

        /// <summary>
        /// 城市三字码信息
        /// </summary>
        /// <returns></returns>
        public static List<Base_CityInfoManage> Get_Base_CityInfoManage()
        {
            return configManage.Get_Base_CityInfoManage();
        }
        #endregion

        #region Base_ProtocolType

        public static List<Base_ProtocolType> Get_Base_ProtocolType()
        {
            return configManage._Base_ProtocolType;
        }

        #endregion

        #region Base_PinYin
        public static List<Base_PinYin> Get_Base_PinYin()
        {
            return configManage.Get_Base_PinYin();
        }

        public static bool Add_Base_PinYin(Base_PinYin _Base_PinYin)
        {
            return configManage.Add_Base_PinYin(_Base_PinYin);
        }

        #endregion

        #region Base_Customers

        /// <summary>
        /// 登录验证，成功返回实体，失败返回NULL
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="LoginPass"></param>
        /// <returns></returns>
        public static Base_Customers Login(string LoginName, string LoginPass, ref bool IsCon)
        {
            try
            {
                CheckNet();
                Base_Customers bc = configManage.Login(LoginName, LoginPass, ref IsCon);
                Error_Messages = string.Empty;
                return bc;
            }
            catch
            {
                IsCon = false;
                return null;
            }

        }


        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static bool Modify_Base_Customers(Base_Customers bc)
        {
            return configManage.Modify_Base_Customers(bc);
        }

        /// <summary>
        /// 登出系统
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static void LoginOut(Base_Customers bc)
        {
            try
            {
                configManage.LoginOut(bc);
                configManage.CloseNet();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Base_CommandManage
        /// <summary>
        /// 获取上下文信息
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static List<Base_CommandManage> Get_Base_CommandManage()
        {
            try
            {
                return configManage.Get_Base_CommandManage();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion Base_CommandManage

    }
}
