using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading;
using IDBMethodSpace;
using PBPid.DBModel;


namespace PBPid.ConfigManageSpace
{
    /***************************************************************************
     * 说明：
     * 封装了远程数据操作
     * 本类建立了两条通道：remoting和socket
     * remoting与PID服务端交互数据，非直连数据库。
     * socket暂时未使用
     * 
     * 功能：增、删、改、查远程数据库
     *       
     * 编写人：罗俊杰 2012-5-24
     * 
     * 修改人：
     * 
     * *************************************************************************/

    /// <summary>
    /// 配置管理类，增、删、改、查远程数据库
    /// </summary>
    [Serializable]
    public class ConfigManage
    {

        #region 变量

        private IDBMethod remoteobj = null;         //连接服务器的remoting
        private ClientSocketSync clienSocket = null;//连接PID服务端的socket
        private string pidServer_Ip = string.Empty;
        private int pidServer_SKPort = 0;
        private int pidServer_RMPort = 0;

        //缓存在本地的数据,在构造时加载
        public List<Base_CommandManage> _Base_CommandManage = new List<Base_CommandManage>();
        public List<Base_ProtocolType> _Base_ProtocolType = new List<Base_ProtocolType>();

        public bool IsRmCon = false;
        public bool IsSkCon = false;

        #endregion

        #region 事件

        public delegate void Error(string Error_Message);
        public event Error OnError;

        #endregion

        #region 构造、加载协议类型、指令库

        public ConfigManage(string ip, int SKport, int RMport)
        {
            pidServer_Ip = ip;
            pidServer_SKPort = SKport;
            pidServer_RMPort = RMport;
            LoadNet();

            _Base_ProtocolType = Get_Base_ProtocolType();
            _Base_CommandManage = Get_Base_CommandManage();
        }

        #endregion

        #region 私有方法


        void LoadNet()
        {
            try
            {
                #region remoting 断线重连
                try
                {
                    if (IsRmCon == false)
                    {
                        remoteobj = (IDBMethod)Activator.GetObject(typeof(IDBMethod), "tcp://" + pidServer_Ip + ":" + pidServer_RMPort + "/RemoteService");
                        IsRmCon = true;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("积极拒绝") != -1)
                        IsRmCon = false;
                }
                #endregion

                //Kevin 2012-07-24  屏蔽 
                #region socket 断线自动重连
                if (clienSocket != null)
                {
                    if (clienSocket.Connected == false) //断线重连
                    {
                        clienSocket.Close();
                        clienSocket = null;
                        Thread.Sleep(1000);
                        clienSocket = new ClientSocketSync(pidServer_Ip, pidServer_SKPort);
                        clienSocket.OnConnection += new ClientSocketSync.Connection(clienSocket_OnConnection);
                        clienSocket.OnCloseConnection += new ClientSocketSync.CloseConnection(clienSocket_OnCloseConnection);
                        clienSocket.OnError += new ClientSocketSync.Error(clienSocket_OnError);
                        clienSocket.OnReceivedBigData += new ClientSocketSync.ReceivedBigData(clienSocket_OnReceivedBigData);
                        clienSocket.OnSendData += new ClientSocketSync.SendData(clienSocket_OnSendData);
                        clienSocket.ConnectionServer();

                        if (clienSocket.IsBound)
                            IsSkCon = true;

                    }
                }
                else
                {
                    clienSocket = new ClientSocketSync(pidServer_Ip, pidServer_SKPort);
                    clienSocket.OnConnection += new ClientSocketSync.Connection(clienSocket_OnConnection);
                    clienSocket.OnCloseConnection += new ClientSocketSync.CloseConnection(clienSocket_OnCloseConnection);
                    clienSocket.OnError += new ClientSocketSync.Error(clienSocket_OnError);
                    clienSocket.OnReceivedBigData += new ClientSocketSync.ReceivedBigData(clienSocket_OnReceivedBigData);
                    clienSocket.OnSendData += new ClientSocketSync.SendData(clienSocket_OnSendData);
                    clienSocket.ConnectionServer();
                    if (clienSocket.IsBound)
                        IsSkCon = true;


                }
                #endregion
                Thread.Sleep(1000);
            }

            catch
            {
                IsSkCon = false;
                Thread.Sleep(1000);
            }

        }

        public void CloseNet()
        {
            if (clienSocket != null)
            {
                try
                {
                    clienSocket.Close();
                    clienSocket = null;
                }
                catch
                {
                }
            }
        }

        #endregion

        #region remote 查询、添加、删除、修改

        #region Base_CommandManage
        /// <summary>
        /// 获取指令库
        /// </summary>
        /// <returns></returns>
        public List<Base_CommandManage> Get_Base_CommandManage()
        {
            try
            {

                return remoteobj.Get_Base_CommandManage();
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 根据指令获取一行数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Base_CommandManage Get_Base_CommandManageByCmd(string cmd)
        {
            try
            {
                return remoteobj.Get_Base_CommandManageByCmd(cmd);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }
        }

        #endregion

        #region Base_OriginalConfigManage
        /// <summary>
        /// 根据客户ID获取原始配置数据
        /// </summary>
        /// <returns></returns>
        public List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByCustomerID(int CustomerID)
        {
            try
            {
                return remoteobj.Get_Base_OriginalConfigManageByCustomerID(CustomerID);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据IP获取原始配置
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByIp(string ip)
        {
            try
            {
                return remoteobj.Get_Base_OriginalConfigManageByIp(ip);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 根据原始配置名获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByName(string Original_ConfigName)
        {
            try
            {
                return remoteobj.Get_Base_OriginalConfigManageByName(Original_ConfigName);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 根据原始配置ID获取配置
        /// </summary>
        /// <param name="Original_Id"></param>
        /// <returns></returns>
        public Base_OriginalConfigManage Get_Base_OriginalConfigManageByOriginal_Id(int Original_Id)
        {
            try
            {
                return remoteobj.Get_Base_OriginalConfigManageByOriginal_Id(Original_Id);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 根据公司名称获取原始配置
        /// </summary>
        /// <param name="CorporationName"></param>
        /// <returns></returns>
        public List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByCorporationName(string CorporationName)
        {
            try
            {
                return remoteobj.Get_Base_OriginalConfigManageByCorporationName(CorporationName);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 添加一条配置
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">原始配置实体</param>
        /// <returns></returns>
        public bool Add_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage)
        {
            try
            {
                return remoteobj.Add_Base_OriginalConfigManage(_Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 批量添加配置
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">原始配置实体集合</param>
        /// <returns></returns>
        public bool Add_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage)
        {
            try
            {
                return remoteobj.Add_Base_OriginalConfigManageArray(_Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 删除一条原始配置
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">原始配置实体</param>
        /// <returns></returns>
        public bool Delete_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage)
        {
            try
            {
                return remoteobj.Delete_Base_OriginalConfigManage(_Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 批量删除原始配置
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">原始配置实体集合</param>
        /// <returns></returns>
        public bool Delete_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage)
        {
            try
            {
                return remoteobj.Delete_Base_OriginalConfigManageArray(_Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 修改一条原始配置
        /// </summary>
        /// <param name="_Base_OriginalConfigManage"></param>
        /// <returns></returns>
        public bool Modify_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage)
        {
            try
            {
                return remoteobj.Modify_Base_OriginalConfigManage(_Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 批量添加原始配置
        /// </summary>
        /// <param name="_Base_OriginalConfigManage"></param>
        /// <returns></returns>
        public bool Modify_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage)
        {
            try
            {
                return remoteobj.Modify_Base_OriginalConfigManageArray(_Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        #endregion

        #region Base_UserConfigManage

        /// <summary>
        /// 获取全部放大用户
        /// </summary>
        /// <returns></returns>
        public List<Base_UserConfigManage> Get_Base_UserConfigManage()
        {
            try
            {
                return remoteobj.Get_Base_UserConfigManage();
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;

            }
        }
        /// <summary>
        /// 根据ID获取放大用户
        /// </summary>
        /// <param name="User_ID"></param>
        /// <returns></returns>
        public Base_UserConfigManage Get_Base_UserConfigManageById(int User_ID)
        {
            try
            {
                return remoteobj.Get_Base_UserConfigManageById(User_ID);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 根据用户名获取放大用户
        /// </summary>
        /// <param name="User_Name"></param>
        /// <returns></returns>
        public List<Base_UserConfigManage> Get_Base_UserConfigManageByName(string User_Name)
        {
            try
            {
                return remoteobj.Get_Base_UserConfigManageByName(User_Name);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 根据客户ID获取放大用户
        /// </summary>
        /// <param name="Customer_Id">客户ID</param>
        /// <returns></returns>
        public List<Base_UserConfigManage> Get_Base_UserConfigManageByCustomer_Id(int Customer_Id)
        {
            try
            {
                return remoteobj.Get_Base_UserConfigManageByCustomer_Id(Customer_Id);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 添加一条放大用户
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Add_Base_UserConfigManage(Base_UserConfigManage _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Add_Base_UserConfigManage(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 批量添加放大用户
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Add_Base_UserConfigManageArray(List<Base_UserConfigManage> _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Add_Base_UserConfigManageArray(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 批量删除放大用户
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Delete_Base_UserConfigManageArray(List<Base_UserConfigManage> _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Delete_Base_UserConfigManageArray(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 删除一条放大用户
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Delete_Base_UserConfigManage(Base_UserConfigManage _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Delete_Base_UserConfigManage(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 修改一条放大用户
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Modify_Base_UserConfigManage(Base_UserConfigManage _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Modify_Base_UserConfigManage(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 修改一条放大用户的发送指令条数
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Modify_Base_UserSendCmdCount(Base_UserConfigManage _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Modify_Base_UserSendCmdCount(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 批量修改放大用户
        /// </summary>
        /// <param name="_Base_UserConfigManage"></param>
        /// <returns></returns>
        public bool Modify_Base_UserConfigManageArray(List<Base_UserConfigManage> _Base_UserConfigManage)
        {
            try
            {
                return remoteobj.Modify_Base_UserConfigManageArray(_Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        #endregion

        #region Base_ProtocolType

        /// <summary>
        /// 获取全部协议
        /// </summary>
        /// <returns></returns>
        public List<Base_ProtocolType> Get_Base_ProtocolType()
        {
            try
            {
                _Base_ProtocolType = remoteobj.Get_Base_ProtocolType();
                return _Base_ProtocolType;
            }
            catch (Exception ex)
            {
                OnError.BeginInvoke(ex.Message, null, null);
                return null;
            }
        }

        /// <summary>
        /// 根据ID获取协议
        /// </summary>
        /// <param name="Protocol_Id"></param>
        /// <returns></returns>
        public Base_ProtocolType Get_Base_ProtocolTypeById(int Protocol_Id)
        {
            try
            {
                return remoteobj.Get_Base_ProtocolTypeById(Protocol_Id);
            }
            catch (Exception ex)
            {
                OnError.BeginInvoke(ex.Message, null, null);
                return null;
            }
        }

        /// <summary>
        /// 根据协议类型获取协议
        /// </summary>
        /// <param name="ProtocolType"></param>
        /// <returns></returns>
        public Base_ProtocolType Get_Base_ProtocolTypeByType(string ProtocolType)
        {
            try
            {
                return remoteobj.Get_Base_ProtocolTypeByType(ProtocolType);
            }
            catch (Exception ex)
            {
                OnError.BeginInvoke(ex.Message, null, null);
                return null;
            }
        }


        #endregion

        #region Base_Customer

        /// <summary>
        /// 登录验证，成功返回实体，失败返回NULL
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="LoginPass"></param>
        /// <returns></returns>
        public Base_Customers Login(string LoginName, string LoginPass, ref bool IsCon)
        {
            try
            {
                Base_Customers bc = remoteobj.Get_Base_CustomersByLogin(LoginName, LoginPass, ref IsCon);
                IsCon = true;
                return bc;
            }
            catch (Exception ex)
            {
                IsCon = false;
                OnError.BeginInvoke(ex.Message, null, null);
                return null;
            }

        }

        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="bc"></param>
        /// <returns>bool</returns>
        public bool Modify_Base_Customers(Base_Customers bc)
        {
            try
            {
                bool IsCon = remoteobj.Modify_Base_Customers(bc);
                return IsCon;
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 登出系统
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public void LoginOut(Base_Customers bc)
        {
            try
            {
                remoteobj.LoginOut(bc);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        #endregion

        #region 城市三字码及拼音信息

        /// <summary>
        /// 城市三字码信息
        /// </summary>
        /// <returns></returns>
        public List<Base_CityInfoManage> Get_Base_CityInfoManage()
        {
            try
            {
                int reccount = 10;
                int PageIndex=1;
                List<Base_CityInfoManage> returnList=new List<Base_CityInfoManage>();
                while (reccount > 0)
                {
                    List<Base_CityInfoManage> tmpList = remoteobj.Get_Base_CityInfoManage(PageIndex);
                    reccount = tmpList.Count;
                    if(reccount>0)
                    {
                        returnList.AddRange(tmpList);
                    }
                    PageIndex++;
                }

                return returnList;
                //return null;
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;

            }
        }
        #endregion

        #region Base_PinYin
        /// <summary>
        /// 根据IP获取汉字拼音信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public List<Base_PinYin> Get_Base_PinYin()
        {
            try
            {
                return remoteobj.Get_Base_PinYin();
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return null;
            }
        }


        public bool Add_Base_PinYin(Base_PinYin _Base_PinYin)
        {
            try
            {
                return remoteobj.Add_Base_PinYin(_Base_PinYin);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }
        }


        #endregion

        #endregion

        #region 刷新缓存

        public bool RefreshCache_Base_CommandManage()
        {
            try
            {
                return remoteobj.RefreshCache(CacheTabName.Base_CommandManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        public bool RefreshCache_Base_OriginalConfigManage()
        {
            try
            {
                return remoteobj.RefreshCache(CacheTabName.Base_OriginalConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }

        }
        public bool RefreshCache_Base_UserConfigManage()
        {
            try
            {
                return remoteobj.RefreshCache(CacheTabName.Base_UserConfigManage);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }
        }
        public bool RefreshCache_All_Table()
        {
            try
            {
                return remoteobj.RefreshCache(CacheTabName.All_Table);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }
        }
        #endregion

        #region Socket事件

        void clienSocket_OnSendData(int _DataSize, string _Ip)
        {

        }

        void clienSocket_OnReceivedBigData(object Data, int _DataSize, System.Net.Sockets.Socket sk)
        {

        }

        void clienSocket_OnError(string Error_Message)
        {
            if (OnError != null)
                OnError("Socket错误：" + Error_Message);
        }

        void clienSocket_OnCloseConnection()
        {
            if (OnError != null)
                OnError("已断开");
        }

        void clienSocket_OnConnection()
        {

        }

        #endregion

    }
}
