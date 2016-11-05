using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using IDBMethodSpace;
using PBPid.DBModel;
using System.Runtime.Serialization;
using System.Text;



namespace RemoteDataSerivice
{
    /*****************************************************************
     * 说明：RemoteService 为 remoting 数据服务类，为客户端提供数据缓存服务。
     * 
     * 编写人：罗俊杰 2012-5-24
     * 
     * 修改人：
     * 
     * ***************************************************************/
    [Serializable]
    public class RemoteService : MarshalByRefObject, IDBMethod
    {

        #region Base_CommandManage 查询缓存数据，在修改、更新、删除操作后会自动刷新缓存

        List<PBPid.DBModel.Base_CommandManage> IDBMethod.Get_Base_CommandManage()
        {
            return DB_CaChe.Get_Base_CommandManage(false, getIP());
        }

        PBPid.DBModel.Base_CommandManage IDBMethod.Get_Base_CommandManageByCmd(string Cmd_Command)
        {
            string IP = getIP();
            List<PBPid.DBModel.Base_CommandManage> entitys = DB_CaChe.Get_Base_CommandManage(false, IP);

            Base_CommandManage entity = entitys.Find(delegate(Base_CommandManage bcm)
            {
                if (bcm.Cmd_Command == Cmd_Command)
                    return true;
                else
                    return false;
            });
            DB_CaChe.CallEvevtMethod("IP:" + IP + "，条件查询：指令=" + Cmd_Command + ",缓存表：Base_CommandManage，返回数据: 1 条");

            return entity;
        }

        #endregion

        #region Base_OriginalConfigManage 查询缓存数据，在修改、更新、删除操作后会自动刷新缓存

        List<Base_OriginalConfigManage> IDBMethod.Get_Base_OriginalConfigManageByCustomerID(int CustomerID)
        {

            string IP = getIP();
            List<Base_OriginalConfigManage> entitys = DB_CaChe.Get_Base_OriginalConfigManage(false, IP);//获取缓存

            List<Base_OriginalConfigManage> request = entitys.FindAll(delegate(Base_OriginalConfigManage bm)
            {
                if (CustomerID == bm.CustomerID)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + IP + "，条件查询：CustomerID =" + CustomerID + ",缓存表：Base_OriginalConfigManage，返回数据：" + request.Count + " 条");

            return request;
        }

        List<PBPid.DBModel.Base_OriginalConfigManage> IDBMethod.Get_Base_OriginalConfigManage()
        {
            return DB_CaChe.Get_Base_OriginalConfigManage(false, getIP());
        }

        PBPid.DBModel.Base_OriginalConfigManage IDBMethod.Get_Base_OriginalConfigManageByOriginal_Id(int Original_Id)
        {
            string IP = getIP();
            List<Base_OriginalConfigManage> entitys = DB_CaChe.Get_Base_OriginalConfigManage(false, IP);//获取缓存
            Base_OriginalConfigManage entitysRequest = entitys.Find(delegate(Base_OriginalConfigManage bocm)
            {
                if (bocm.Original_Id == Original_Id)
                    return true;
                else
                    return false;
            });
            DB_CaChe.CallEvevtMethod("IP:" + IP + "，条件查询：配置ID=" + Original_Id + ",缓存表：Base_OriginalConfigManage，返回数据: 1 条");
            return entitysRequest;
        }

        List<PBPid.DBModel.Base_OriginalConfigManage> IDBMethod.Get_Base_OriginalConfigManageByIp(string Original_Ip)
        {
            string IP = getIP();
            List<Base_OriginalConfigManage> entitys = DB_CaChe.Get_Base_OriginalConfigManage(false, IP);//获取缓存
            List<Base_OriginalConfigManage> entitysRequest = entitys.FindAll(delegate(Base_OriginalConfigManage bocm)
            {
                if (bocm.Original_Ip == Original_Ip)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + IP + "，条件查询：配置IP=" + Original_Ip + ",缓存表：Base_OriginalConfigManage，返回数据: " + entitysRequest.Count + " 条");

            return entitysRequest;
        }

        List<PBPid.DBModel.Base_OriginalConfigManage> IDBMethod.Get_Base_OriginalConfigManageByName(string Original_ConfigName)
        {
            string IP = getIP();
            List<Base_OriginalConfigManage> entitys = DB_CaChe.Get_Base_OriginalConfigManage(false, IP);//获取缓存
            List<Base_OriginalConfigManage> entitysRequest = entitys.FindAll(delegate(Base_OriginalConfigManage bocm)
            {
                if (bocm.Original_ConfigName == Original_ConfigName)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + IP + "，条件查询：配置名=" + Original_ConfigName + ",缓存表：Base_OriginalConfigManage，返回数据: " + entitysRequest.Count + " 条");

            return entitysRequest;
        }

        List<PBPid.DBModel.Base_OriginalConfigManage> IDBMethod.Get_Base_OriginalConfigManageByCorporationName(string CorporationName)
        {
            string IP = getIP();
            List<Base_OriginalConfigManage> entitys = DB_CaChe.Get_Base_OriginalConfigManage(false, IP);//获取缓存
            List<Base_OriginalConfigManage> entitysRequest = entitys.FindAll(delegate(Base_OriginalConfigManage bocm)
            {
                if (bocm.Original_CorporationName == CorporationName)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + IP + "，条件查询：配置公司名=" + CorporationName + ",缓存表：Base_OriginalConfigManage，返回数据: " + entitysRequest.Count + " 条");

            return entitysRequest;
        }

        bool IDBMethod.Add_Base_OriginalConfigManageArray(List<PBPid.DBModel.Base_OriginalConfigManage> _Base_OriginalConfigManage)
        {
            return DB_CaChe.Add_Base_OriginalConfigManageArray(_Base_OriginalConfigManage, getIP());
        }

        bool IDBMethod.Add_Base_OriginalConfigManage(PBPid.DBModel.Base_OriginalConfigManage _Base_OriginalConfigManage)
        {
            return DB_CaChe.Add_Base_OriginalConfigManage(_Base_OriginalConfigManage, getIP());
        }

        bool IDBMethod.Modify_Base_OriginalConfigManageArray(List<PBPid.DBModel.Base_OriginalConfigManage> _Base_OriginalConfigManage)
        {
            return DB_CaChe.Modify_Base_OriginalConfigManageArray(_Base_OriginalConfigManage, getIP());
        }

        bool IDBMethod.Modify_Base_OriginalConfigManage(PBPid.DBModel.Base_OriginalConfigManage _Base_OriginalConfigManage)
        {
            return DB_CaChe.Modify_Base_OriginalConfigManage(_Base_OriginalConfigManage, getIP());
        }

        bool IDBMethod.Delete_Base_OriginalConfigManageArray(List<PBPid.DBModel.Base_OriginalConfigManage> _Base_OriginalConfigManage)
        {
            return DB_CaChe.Delete_Base_OriginalConfigManageArray(_Base_OriginalConfigManage, getIP());
        }

        bool IDBMethod.Delete_Base_OriginalConfigManage(PBPid.DBModel.Base_OriginalConfigManage _Base_OriginalConfigManage)
        {
            return DB_CaChe.Delete_Base_OriginalConfigManage(_Base_OriginalConfigManage, getIP());
        }

        #endregion

        #region Base_UserConfigManage 查询缓存数据，在修改、更新、删除操作后会自动刷新缓存

        List<PBPid.DBModel.Base_UserConfigManage> IDBMethod.Get_Base_UserConfigManage()
        {
            return DB_CaChe.Get_Base_UserConfigManage(false, getIP());
        }

        PBPid.DBModel.Base_UserConfigManage IDBMethod.Get_Base_UserConfigManageById(int User_Id)
        {
            string ip = getIP();
            List<Base_UserConfigManage> entitys = DB_CaChe.Get_Base_UserConfigManage(false, ip);//获取缓存
            Base_UserConfigManage entitysRequest = entitys.Find(delegate(Base_UserConfigManage bocm)
            {
                if (bocm.User_Id == User_Id)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + ip + "，条件查询：用户ID=" + User_Id + ",缓存表：Base_UserConfigManage，返回数据: 1 条");

            return entitysRequest;
        }

        List<PBPid.DBModel.Base_UserConfigManage> IDBMethod.Get_Base_UserConfigManageByName(string User_Name)
        {
            string ip = getIP();
            List<Base_UserConfigManage> entitys = DB_CaChe.Get_Base_UserConfigManage(false, ip);//获取缓存
            List<Base_UserConfigManage> entitysRequest = entitys.FindAll(delegate(Base_UserConfigManage bocm)
            {
                if (bocm.User_Name == User_Name)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + ip + "，条件查询：用户名=" + User_Name + ",缓存表：Base_UserConfigManage，返回数据: " + entitysRequest.Count + " 条");

            return entitysRequest;
        }

        List<PBPid.DBModel.Base_UserConfigManage> IDBMethod.Get_Base_UserConfigManageByCustomer_Id(int Customer_Id)
        {
            string ip = getIP();
            List<Base_UserConfigManage> entitys = DB_CaChe.Get_Base_UserConfigManage(false, ip);//获取缓存
            List<Base_UserConfigManage> entitysRequest = entitys.FindAll(delegate(Base_UserConfigManage bocm)
            {
                if (bocm.Customer_Id == Customer_Id)
                    return true;
                else
                    return false;
            });

            DB_CaChe.CallEvevtMethod("IP:" + ip + "，条件查询：用户配置ID=" + Customer_Id + ",缓存表：Base_UserConfigManage，返回数据: " + entitysRequest.Count + " 条");

            return entitysRequest;
        }

        bool IDBMethod.Add_Base_UserConfigManageArray(List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage)
        {
            return DB_CaChe.Add_Base_UserConfigManageArray(_Base_UserConfigManage, getIP());
        }

        bool IDBMethod.Add_Base_UserConfigManage(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage)
        {
            return DB_CaChe.Add_Base_UserConfigManage(_Base_UserConfigManage, getIP());
        }

        bool IDBMethod.Modify_Base_UserConfigManageArray(List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage)
        {
            return DB_CaChe.Modify_Base_UserConfigManageArray(_Base_UserConfigManage, getIP());
        }

        bool IDBMethod.Modify_Base_UserConfigManage(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage)
        {
            return DB_CaChe.Modify_Base_UserConfigManage(_Base_UserConfigManage, getIP());
        }

        bool IDBMethod.Modify_Base_UserSendCmdCount(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage)
        {
            return DB_CaChe.Modify_Base_UserSendCmdCount(_Base_UserConfigManage, getIP());
        }

        bool IDBMethod.Delete_Base_UserConfigManageArray(List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage)
        {
            return DB_CaChe.Delete_Base_UserConfigManageArray(_Base_UserConfigManage, getIP());
        }

        bool IDBMethod.Delete_Base_UserConfigManage(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage)
        {
            return DB_CaChe.Delete_Base_UserConfigManage(_Base_UserConfigManage, getIP());
        }

        #endregion

        #region Base_CityInfoManage 查询缓存数据，在修改、更新、删除操作后会自动刷新缓存

        List<PBPid.DBModel.Base_CityInfoManage> IDBMethod.Get_Base_CityInfoManage(int PageIndex)
        {
            return DB_CaChe.Get_Base_CityInfoManageByPage(false, getIP(),PageIndex);
        }

        #endregion

        #region Base_ProtocolType 查询缓存数据，暂时没有实时加载

        List<Base_ProtocolType> IDBMethod.Get_Base_ProtocolType()
        {
            return DB_CaChe.Get_Base_ProtocolType(false, getIP());
        }

        Base_ProtocolType IDBMethod.Get_Base_ProtocolTypeById(int Protocol_Id)
        {
            string ip = getIP();
            List<Base_ProtocolType> bpt = DB_CaChe.Get_Base_ProtocolType(false, ip);

            DB_CaChe.CallEvevtMethod("IP:" + ip + "，条件查询：协议类型ID=" + Protocol_Id + ",缓存表：Base_ProtocolType，返回数据: 1 条");

            return bpt.Find(delegate(Base_ProtocolType _bpt)
            {
                if (_bpt.Protocol_Id == Protocol_Id)
                    return true;
                else
                    return false;
            });


        }

        Base_ProtocolType IDBMethod.Get_Base_ProtocolTypeByType(string ProtocolType)
        {
            string ip = getIP();
            List<Base_ProtocolType> bpt = DB_CaChe.Get_Base_ProtocolType(false, ip);

            DB_CaChe.CallEvevtMethod("IP:" + ip + "，条件查询：协议类型=" + ProtocolType + ",缓存表：Base_ProtocolType，返回数据: 1 条");

            return bpt.Find(delegate(Base_ProtocolType _bpt)
            {
                if (_bpt.ProtocolType == ProtocolType)
                    return true;
                else
                    return false;
            });
        }

        #endregion

        #region Base_PinYin

        List<Base_PinYin> IDBMethod.Get_Base_PinYin()
        {
            string IP = getIP();
            return DB_CaChe.Get_Base_PinYin(false, IP);//获取缓存
            //List<Base_PinYin> entitys = DB_CaChe.Get_Base_PinYin(false, IP);//获取缓存

            //DB_CaChe.CallEvevtMethod("IP:" + IP + "，缓存表：Base_PinYin，返回数据: " + entitys.Count + " 条");

            //return entitys;
        }

        bool IDBMethod.Add_Base_PinYin(List<PBPid.DBModel.Base_PinYin> _Base_PinYin)
        {
            string IP = getIP();
            return DB_CaChe.Add_Base_PinYin(_Base_PinYin,IP);
        }

        bool IDBMethod.Add_Base_PinYin(PBPid.DBModel.Base_PinYin _Base_PinYin)
        {
            string IP = getIP();
            return DB_CaChe.Add_Base_PinYin(_Base_PinYin,IP);
        }

        #endregion

        #region Customers 登录验证

        Base_Customers IDBMethod.Get_Base_CustomersByLogin(string LoginName, string LoginPass,ref bool IsLogin)
        {
            return DB_CaChe.Get_Base_CustomersByLogin(LoginName, LoginPass, getIP(), ref IsLogin);
        }

        #endregion

        #region Customers 修改客户信息
        bool IDBMethod.Modify_Base_Customers(Base_Customers bc)
        {
            return DB_CaChe.Modify_Base_Customers(bc, getIP());
        }
        #endregion

        #region Customers 登出系统
        void IDBMethod.LoginOut(Base_Customers bc)
        {
            DB_CaChe.CustomersLoginOut(bc);
        }
        #endregion

        #region 缓存刷新
        bool IDBMethod.RefreshCache(CacheTabName TabName)
        {
            switch (TabName)
            {
                case CacheTabName.Base_CommandManage:
                    DB_CaChe.Get_Base_CommandManage(true, getIP());
                    return true;
                case CacheTabName.Base_OriginalConfigManage:
                    DB_CaChe.Get_Base_OriginalConfigManage(true, getIP());
                    return true;
                case CacheTabName.Base_UserConfigManage:
                    DB_CaChe.Get_Base_UserConfigManage(true, getIP());
                    return true;
                case CacheTabName.Base_ProtocolType:
                    DB_CaChe.Get_Base_ProtocolType(true, getIP());
                    return true;
                case CacheTabName.Base_PinYin:
                    DB_CaChe.Get_Base_PinYin(true, getIP());
                    return true;
                case CacheTabName.All_Table:
                    DB_CaChe.Get_Base_CommandManage(true, getIP());
                    DB_CaChe.Get_Base_OriginalConfigManage(true, getIP());
                    DB_CaChe.Get_Base_UserConfigManage(true, getIP());
                    DB_CaChe.Get_Base_PinYin(true, getIP());
                    DB_CaChe.Get_Base_ProtocolType(true, getIP());
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region 获取IP

        string getIP()
        {
            return ((IPAddress)System.Runtime.Remoting.Messaging.CallContext.GetData("ClientIPAddress")).ToString();
        }

        #endregion

    }


    /*****************************************************************
     * 说明：DB_CaChe 为数据缓存操作类，为 remoting 提供数据操作支撑
     * 
     * 编写人：罗俊杰 2012-5-24
     * 
     * 修改人：
     * 
     * ***************************************************************/
    /// <summary>
    /// 数据缓存操作类
    /// </summary>
    [Serializable]
    public static class DB_CaChe
    {
        #region 变量

        private static DB.DataBase_Cls db = null;
        private static List<PBPid.DBModel.Base_CommandManage> _Base_CommandManage = new List<PBPid.DBModel.Base_CommandManage>();
        private static List<PBPid.DBModel.Base_OriginalConfigManage> _Base_OriginalConfigManage = new List<PBPid.DBModel.Base_OriginalConfigManage>();
        private static List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage = new List<PBPid.DBModel.Base_UserConfigManage>();
        private static List<PBPid.DBModel.Base_ProtocolType> _Base_ProtocolType = new List<PBPid.DBModel.Base_ProtocolType>();
        private static List<PBPid.DBModel.Base_PinYin> _Base_PinYin = new List<PBPid.DBModel.Base_PinYin>();
        private static List<PBPid.DBModel.Base_CityInfoManage> _Base_CityInfoManage = new List<PBPid.DBModel.Base_CityInfoManage>();
        public static List<LoginInfo> logins = new List<LoginInfo>();

        #endregion

        #region 事件

        public delegate void CallEvevt(string context);   //调用事件
        public static event CallEvevt OnCallEvent;

        public delegate void RefrshCaChe(string TabName); //刷新缓存事件
        public static event RefrshCaChe OnRefrshCaChe;

        public static void CallEvevtMethod(string context)
        {
            if (OnCallEvent != null)
                OnCallEvent.BeginInvoke(context, null, null);


        }
        public static void RefrshCaCheMethod(string TabName)
        {
            if (OnRefrshCaChe != null)
                OnRefrshCaChe.BeginInvoke(TabName, null, null);

        }

        #endregion

        #region 构造

        static DB_CaChe()
        {
            if (db == null)
            {
                db = new DB.DataBase_Cls();
            }
        }

        #endregion

        #region 获取数据

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="LoginPass"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Base_Customers Get_Base_CustomersByLogin(string LoginName, string LoginPass, string ip,ref bool IsLogin)
        {
            DataSet ds = db.GetDataSet("select top 1 * from Pid.Base_Customers Where CustomerLoginName='" + LoginName + "'");// and CustomerLoginPass='" + LoginPass + "'");
            if (ds.Tables[0].Rows.Count > 0)
            {               
                DataRow item = ds.Tables[0].Rows[0];
                Base_Customers bc = new Base_Customers();

                string tmppwd = item["CustomerLoginPass"].ToString();

                if(LoginPass!=PBPid.Base.AppGlobal.EncryptMD5(PBPid.Base.AppGlobal.EncryptMD5(tmppwd)+"cgt"))
                {
                    IsLogin = false;

                    CallEvevtMethod("IP:" + ip + "，登录验证：用户名：" + LoginName + "，登录密码不正确！返回数据：null");
                    return null;
                }

                bc.CustomerID = int.Parse(item["CustomerID"].ToString());
                bc.CustomerLoginName = item["CustomerLoginName"].ToString();
                bc.CustomerLoginPass = item["CustomerLoginPass"].ToString();
                bc.CustomerName = item["CustomerName"].ToString();
                bc.CustomerPhone = item["CustomerPhone"].ToString();
                bc.CustomerAddress = item["CustomerAddress"].ToString();
                bc.CustomerBeginDate = DateTime.Parse(item["CustomerBeginDate"].ToString());
                bc.CustomerEndDate = DateTime.Parse(item["CustomerEndDate"].ToString());
                bc.CustomerPrice = int.Parse(item["CustomerPrice"].ToString());
                bc.CustomerDescription = item["CustomerDescription"].ToString();
                bc.CustomerLogIp = ip;

                IsLogin = true;

                CallEvevtMethod("IP:" + ip + "，登录验证：用户名：" + LoginName + ",密码:" + LoginPass + "，登录成功！返回数据：1 条");
                
                //添加到登录列表
                LoginInfo newLoginf=new LoginInfo();
                newLoginf.Customers = bc;
                logins.Add(newLoginf);

                return bc;
            }
            else
            {

                IsLogin = false;

                CallEvevtMethod("IP:" + ip + "，登录验证：用户名：" + LoginName + ",密码:" + LoginPass + "，无此用户！返回数据：null");
                return null;
            }
        }


        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="bc"></param>
        /// <param name="ip"></param>
        /// <returns>bool</returns>
        public static bool Modify_Base_Customers(Base_Customers bc,string ip)
        {
            string sql = @"Update Pid.Base_Customers set CustomerLoginPass ='" +
            bc.CustomerLoginPass + "',CustomerAddress='" + bc.CustomerAddress + "',CustomerPhone ='" +
            bc.CustomerPhone + "',CustomerDescription='" + bc.CustomerDescription + "' where CustomerID =" + bc.CustomerID.ToString() + "";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独修改：Base_Customers 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            //Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        /// <summary>
        /// 登出（连接断开）
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="LoginPass"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static void CustomersLoginOut(Base_Customers bc)
        {
            lock (logins)
            {
                for (int i = 0; i < logins.Count; i++)
                {
                    if (logins[i].Customers.CustomerID == bc.CustomerID)
                    {
                        logins.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static List<PBPid.DBModel.Base_ProtocolType> Get_Base_ProtocolType(bool IsUpdate, string ip)
        {
            if (IsUpdate)
            {
                _Base_ProtocolType = new List<Base_ProtocolType>();
                DataSet ds = db.GetDataSet("select * from Pid.Base_ProtocolType");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_ProtocolType bpt = new Base_ProtocolType();
                    bpt.Protocol_Id = int.Parse(item[0].ToString());
                    bpt.ProtocolType = item[1].ToString();
                    _Base_ProtocolType.Add(bpt);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：PBPid.Base_ProtocolType 表，" + _Base_ProtocolType.Count + " 条");
                return _Base_ProtocolType;
            }

            if (_Base_ProtocolType.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from Pid.Base_ProtocolType");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_ProtocolType bpt = new Base_ProtocolType();
                    bpt.Protocol_Id = int.Parse(item[0].ToString());
                    bpt.ProtocolType = item[1].ToString();
                    _Base_ProtocolType.Add(bpt);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_ProtocolType 表，" + _Base_ProtocolType.Count + " 条");
                return _Base_ProtocolType;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_ProtocolType 表，" + _Base_ProtocolType.Count + " 条");
                return _Base_ProtocolType;
            }
        }

        /// <summary>
        /// 获取 Base_CommandManage 数据
        /// </summary>
        /// <returns></returns>
        public static List<PBPid.DBModel.Base_CommandManage> Get_Base_CommandManage(bool IsUpdate, string ip)
        {
            if (IsUpdate)
            {
                _Base_CommandManage = new List<Base_CommandManage>();
                DataSet ds = db.GetDataSet("select * from Pid.Base_CommandManage");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_CommandManage bcm = new Base_CommandManage();
                    bcm.Cmd_Id = int.Parse(item["Cmd_Id"].ToString());
                    bcm.Cmd_Command = item["Cmd_Command"].ToString();
                    bcm.Cmd_IsUp = bool.Parse(item["Cmd_IsUp"].ToString());
                    bcm.Cmd_IsDown = bool.Parse(item["Cmd_IsDown"].ToString());
                    bcm.Cmd_UpCommand = item["Cmd_UpCommand"].ToString();
                    bcm.Cmd_DownCommand = item["Cmd_DownCommand"].ToString();
                    _Base_CommandManage.Add(bcm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_CommandManage 表，" + _Base_CommandManage.Count + " 条");
                return _Base_CommandManage;
            }

            if (_Base_CommandManage.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from Pid.Base_CommandManage");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_CommandManage bcm = new Base_CommandManage();
                    bcm.Cmd_Id = int.Parse(item["Cmd_Id"].ToString());
                    bcm.Cmd_Command = item["Cmd_Command"].ToString();
                    bcm.Cmd_IsUp = bool.Parse(item["Cmd_IsUp"].ToString());
                    bcm.Cmd_IsDown = bool.Parse(item["Cmd_IsDown"].ToString());
                    bcm.Cmd_UpCommand = item["Cmd_UpCommand"].ToString();
                    bcm.Cmd_DownCommand = item["Cmd_DownCommand"].ToString();
                    _Base_CommandManage.Add(bcm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_CommandManage 表，" + _Base_CommandManage.Count + " 条");
                return _Base_CommandManage;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_CommandManage 表，" + _Base_CommandManage.Count + " 条");
                return _Base_CommandManage;
            }
        }

        /// <summary>
        /// 获取 Base_OriginalConfigManage 数据，该数据会在更新、修改、删除后自动刷新
        /// </summary>
        /// <returns></returns>
        public static List<PBPid.DBModel.Base_OriginalConfigManage> Get_Base_OriginalConfigManage(bool IsUpdate, string ip)
        {
            if (IsUpdate)
            {
                _Base_OriginalConfigManage = new List<Base_OriginalConfigManage>();
                DataSet ds = db.GetDataSet("select * from Pid.Base_OriginalConfigManage order by CustomerID,Original_OfficeNumber,Original_Status");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_OriginalConfigManage bocm = new Base_OriginalConfigManage();
                    bocm.Original_Id = int.Parse(item["Original_Id"].ToString());
                    bocm.Original_LocalIp = item["Original_LocalIp"].ToString();
                    bocm.Original_Ip = item["Original_Ip"].ToString();
                    bocm.Original_Port = int.Parse(item["Original_Port"].ToString());
                    bocm.Original_Status = item["Original_Status"].ToString();
                    bocm.Original_ConfigName = item["Original_ConfigName"].ToString();
                    bocm.Original_ConfigPass = item["Original_ConfigPass"].ToString();
                    bocm.Original_ConfigSi = item["Original_ConfigSi"].ToString();
                    bocm.ProtocolType = item["ProtocolType"].ToString();
                    bocm.Original_CheckingType = item["Original_CheckingType"].ToString();
                    bocm.Original_OfficeNumber = item["Original_OfficeNumber"].ToString();
                    bocm.Original_CorporationName = item["Original_CorporationName"].ToString();
                    bocm.Original_Description = item["Original_Description"].ToString();
                    bocm.CustomerID = int.Parse(item["CustomerID"].ToString());

                    _Base_OriginalConfigManage.Add(bocm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_OriginalConfigManage 表，" + _Base_OriginalConfigManage.Count + " 条");
                return _Base_OriginalConfigManage;
            }

            if (_Base_OriginalConfigManage.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from Pid.Base_OriginalConfigManage order by CustomerID,Original_OfficeNumber,Original_Status");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_OriginalConfigManage bocm = new Base_OriginalConfigManage();
                    bocm.Original_Id = int.Parse(item["Original_Id"].ToString());
                    bocm.Original_LocalIp = item["Original_LocalIp"].ToString();
                    bocm.Original_Ip = item["Original_Ip"].ToString();
                    bocm.Original_Port = int.Parse(item["Original_Port"].ToString());
                    bocm.Original_Status = item["Original_Status"].ToString();
                    bocm.Original_ConfigName = item["Original_ConfigName"].ToString();
                    bocm.Original_ConfigPass = item["Original_ConfigPass"].ToString();
                    bocm.Original_ConfigSi = item["Original_ConfigSi"].ToString();
                    bocm.ProtocolType = item["ProtocolType"].ToString();
                    bocm.Original_CheckingType = item["Original_CheckingType"].ToString();
                    bocm.Original_OfficeNumber = item["Original_OfficeNumber"].ToString();
                    bocm.Original_CorporationName = item["Original_CorporationName"].ToString();
                    bocm.Original_Description = item["Original_Description"].ToString();
                    bocm.CustomerID = int.Parse(item["CustomerID"].ToString());
                    _Base_OriginalConfigManage.Add(bocm);//添加到表缓存
                }
                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_OriginalConfigManage 表，" + _Base_OriginalConfigManage.Count + " 条");
                return _Base_OriginalConfigManage;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_OriginalConfigManage 表，" + _Base_OriginalConfigManage.Count + " 条");
                return _Base_OriginalConfigManage;
            }
        }

        //汉字转编码
        public static string HanZiToByte(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            string strResult = "";
            //string[] strArr = new string[bytes.Length];
            //Console.WriteLine("16进制显示'你好'：");
            for (int i = 0; i < bytes.Length; i++)
            {
                strResult += "0X"+bytes[i].ToString("x");
                //Console.Write(strArr[i]+" ");
            }
            //Console.WriteLine();
            return strResult;
        }

        public static List<PBPid.DBModel.Base_CityInfoManage> Get_Base_CityInfoManageByPage(bool IsUpdate, string ip,int PageIndex)
        {
            //PageIndex 从 1开始，每次返回300条数据 

            List<PBPid.DBModel.Base_CityInfoManage> returnList = new List<Base_CityInfoManage>();

            if (IsUpdate)
            {
                _Base_CityInfoManage = new List<Base_CityInfoManage>();
                DataSet ds = db.GetDataSet("select * from dbo.Bd_Air_AirPort order by CityCodeWord");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_CityInfoManage bocm = new Base_CityInfoManage();
                    bocm.City_Code = item["CityCodeWord"].ToString();
                    bocm.City_Id_auto = int.Parse(item["id_auto"].ToString());
                    bocm.City_Name = item["CityName"].ToString();
                    bocm.City_QuanPin = item["CityQuanPin"].ToString();
                    _Base_CityInfoManage.Add(bocm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_CityInfoManage 表，" + _Base_CityInfoManage.Count + " 条");

                for (int i = (PageIndex - 1) * 300; i < _Base_CityInfoManage.Count && i < PageIndex * 300; i++)
                {
                    returnList.Add(_Base_CityInfoManage[i]);
                }

                return returnList;
            }

            if (_Base_CityInfoManage.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from dbo.Bd_Air_AirPort order by CityCodeWord");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_CityInfoManage bocm = new Base_CityInfoManage();
                    bocm.City_Code = item["CityCodeWord"].ToString();
                    bocm.City_Id_auto = int.Parse(item["id_auto"].ToString());
                    bocm.City_Name = item["CityName"].ToString();
                    bocm.City_QuanPin = item["CityQuanPin"].ToString();
                    _Base_CityInfoManage.Add(bocm);//添加到表缓存
                }
                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_CityInfoManage 表，" + _Base_CityInfoManage.Count + " 条");

                for (int i = (PageIndex - 1) * 300; i < _Base_CityInfoManage.Count && i < PageIndex * 300; i++)
                {
                    returnList.Add(_Base_CityInfoManage[i]);
                }

                return returnList;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_OriginalConfigManage 表，" + _Base_OriginalConfigManage.Count + " 条");
                for (int i = (PageIndex-1) * 300; i < _Base_CityInfoManage.Count && i<PageIndex*300; i++)
                {
                    returnList.Add(_Base_CityInfoManage[i]);
                }

                return returnList;
            }
        }


        /// <summary>
        /// 获取 Base_CityInfoManage 数据，该数据会在更新、修改、删除后自动刷新
        /// </summary>
        /// <returns></returns>
        public static List<PBPid.DBModel.Base_CityInfoManage> Get_Base_CityInfoManage(bool IsUpdate, string ip)
        {
            if (IsUpdate)
            {
                _Base_CityInfoManage = new List<Base_CityInfoManage>();
                DataSet ds = db.GetDataSet("select * from dbo.Bd_Air_AirPort order by CityCodeWord");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_CityInfoManage bocm = new Base_CityInfoManage();
                    bocm.City_Code = item["CityCodeWord"].ToString();
                    bocm.City_Id_auto = int.Parse(item["id_auto"].ToString());
                    bocm.City_Name = item["CityName"].ToString();
                    bocm.City_QuanPin = item["CityQuanPin"].ToString();
                    _Base_CityInfoManage.Add(bocm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_CityInfoManage 表，" + _Base_CityInfoManage.Count + " 条");
                return _Base_CityInfoManage;
            }

            if (_Base_CityInfoManage.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from dbo.Bd_Air_AirPort order by CityCodeWord");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_CityInfoManage bocm = new Base_CityInfoManage();
                    bocm.City_Code = item["CityCodeWord"].ToString();
                    bocm.City_Id_auto = int.Parse(item["id_auto"].ToString());
                    bocm.City_Name = item["CityName"].ToString();
                    bocm.City_QuanPin = item["CityQuanPin"].ToString();
                    _Base_CityInfoManage.Add(bocm);//添加到表缓存
                }
                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_CityInfoManage 表，" + _Base_CityInfoManage.Count + " 条");
                return _Base_CityInfoManage;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_OriginalConfigManage 表，" + _Base_OriginalConfigManage.Count + " 条");
                return _Base_CityInfoManage;
            }
        }

        /// <summary>
        /// 获取 Base_UserConfigManage  数据,该数据会在更新、修改、删除后自动刷新
        /// </summary>
        /// <returns></returns>
        public static List<PBPid.DBModel.Base_UserConfigManage> Get_Base_UserConfigManage(bool IsUpdate, string ip)
        {
            if (IsUpdate)
            {
                _Base_UserConfigManage = new List<Base_UserConfigManage>();
                DataSet ds = db.GetDataSet("select * from Pid.Base_UserConfigManage order by Customer_Id,[User_Name]");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_UserConfigManage bucm = new Base_UserConfigManage();
                    bucm.User_Id = int.Parse(item["User_Id"].ToString());
                    bucm.Customer_Id = int.Parse(item["Customer_Id"].ToString());
                    bucm.User_Name = item["User_Name"].ToString();
                    bucm.User_Pass = item["User_Pass"].ToString();
                    bucm.User_DisableCmd = item["User_DisableCmd"].ToString();
                    bucm.User_Disable = bool.Parse(item["User_Disable"].ToString());
                    bucm.User_BeginDate = DateTime.Parse(item["User_BeginDate"].ToString());
                    bucm.User_EndDate = DateTime.Parse(item["User_EndDate"].ToString());
                    bucm.User_SendCount = int.Parse(item["User_SendCount"].ToString());
                    bucm.User_Description = item["User_Description"].ToString();
                    bucm.User_Office = item["User_Office"].ToString();
                    bucm.User_LimitCount=int.Parse(item["User_LimitCount"].ToString());
                    bucm.User_LimitFlag = bool.Parse(item["User_LimitFlag"].ToString());
                    _Base_UserConfigManage.Add(bucm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_UserConfigManage 表，" + _Base_UserConfigManage.Count + " 条");
                return _Base_UserConfigManage;
            }

            if (_Base_UserConfigManage.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from Pid.Base_UserConfigManage order by Customer_Id,[User_Name]");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_UserConfigManage bucm = new Base_UserConfigManage();
                    bucm.User_Id = int.Parse(item["User_Id"].ToString());
                    bucm.Customer_Id = int.Parse(item["Customer_Id"].ToString());
                    bucm.User_Name = item["User_Name"].ToString();
                    bucm.User_Pass = item["User_Pass"].ToString();
                    bucm.User_DisableCmd = item["User_DisableCmd"].ToString();
                    bucm.User_Disable = bool.Parse(item["User_Disable"].ToString());
                    bucm.User_BeginDate = DateTime.Parse(item["User_BeginDate"].ToString());
                    bucm.User_EndDate = DateTime.Parse(item["User_EndDate"].ToString());
                    bucm.User_SendCount = int.Parse(item["User_SendCount"].ToString());
                    bucm.User_Description = item["User_Description"].ToString();
                    _Base_UserConfigManage.Add(bucm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_UserConfigManage 表，" + _Base_UserConfigManage.Count + " 条");
                return _Base_UserConfigManage;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_UserConfigManage 表，" + _Base_UserConfigManage.Count + " 条");
                return _Base_UserConfigManage;
            }
        }

        /// <summary>
        /// 获取 Base_PinYin  数据,该数据会在更新、修改、删除后自动刷新
        /// </summary>
        /// <returns></returns>
        public static List<PBPid.DBModel.Base_PinYin> Get_Base_PinYin(bool IsUpdate, string ip)
        {
            if (IsUpdate)
            {
                _Base_PinYin = new List<Base_PinYin>();
                DataSet ds = db.GetDataSet("select * from Pid.Base_PinYin order by pinyin");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_PinYin bucm = new Base_PinYin();
                    bucm.HanZi = item["hanzi"].ToString();
                    bucm.PinYin = item["pinyin"].ToString();
                    bucm.BianMa = item["bianma"].ToString();
                    bucm.Remarks = item["remarks"].ToString();                    
                    _Base_PinYin.Add(bucm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_PinYin 表，" + _Base_PinYin.Count + " 条");
                return _Base_PinYin;
            }

            if (_Base_PinYin.Count == 0)
            {
                DataSet ds = db.GetDataSet("select * from Pid.Base_PinYin order by pinyin");
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Base_PinYin bucm = new Base_PinYin();
                    bucm.ID = int.Parse(item["id"].ToString());
                    bucm.HanZi = item["hanzi"].ToString();
                    bucm.PinYin = item["pinyin"].ToString();
                    bucm.BianMa = item["bianma"].ToString();
                    bucm.Remarks = item["remarks"].ToString();                    
                    _Base_PinYin.Add(bucm);//添加到表缓存
                }

                RefrshCaCheMethod("IP:" + ip + "，刷新缓存：Base_PinYin 表，" + _Base_PinYin.Count + " 条");
                return _Base_PinYin;
            }
            else
            {
                //CallEvevtMethod("IP:" + ip + "，调用缓存：Base_UserConfigManage 表，" + _Base_UserConfigManage.Count + " 条");
                return _Base_PinYin;
            }
        }

        #endregion

        #region Base_OriginalConfigManage
        //批量添加
        public static bool Add_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_OriginalConfigManage.Count; i++)
            {
                Base_OriginalConfigManage bocm = _Base_OriginalConfigManage[i];
                string sql = "insert into Pid.Base_OriginalConfigManage (Original_Ip,Original_Port,Original_ConfigName,Original_ConfigPass,Original_ConfigSi,ProtocolType," +
                        "Original_CheckingType,Original_OfficeNumber,Original_CorporationName,Original_Description,CustomerID,Original_LocalIp,Original_Status) values('" + 
                        bocm.Original_Ip + "'," + bocm.Original_Port + ",'" +
                    bocm.Original_ConfigName + "','" + bocm.Original_ConfigPass + "','" + bocm.Original_ConfigSi + "','" +
                    bocm.ProtocolType + "','" + bocm.Original_CheckingType + "','" + bocm.Original_OfficeNumber + "','" +
                    bocm.Original_CorporationName + "','" + bocm.Original_Description + "'," + bocm.CustomerID + ",'" + bocm.Original_LocalIp + "','" + bocm.Original_Status.ToString() + "')";
                sqls.Add(sql);
            }
            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量添加：Base_OriginalConfigManage 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_OriginalConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }
        //单独添加
        public static bool Add_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage, string ip)
        {
            Base_OriginalConfigManage bocm = _Base_OriginalConfigManage;
            string sql = "insert into Pid.Base_OriginalConfigManage (Original_Ip,Original_Port,Original_ConfigName,Original_ConfigPass,Original_ConfigSi,ProtocolType,"+
                        "Original_CheckingType,Original_OfficeNumber,Original_CorporationName,Original_Description,CustomerID,Original_LocalIp,Original_Status) values('" + 
                bocm.Original_Ip + "'," + bocm.Original_Port + ",'" +
                bocm.Original_ConfigName + "','" + bocm.Original_ConfigPass + "','" + bocm.Original_ConfigSi + "','" +
                bocm.ProtocolType + "','" + bocm.Original_CheckingType + "','" + bocm.Original_OfficeNumber + "','" +
                bocm.Original_CorporationName + "','" + bocm.Original_Description + "'," + bocm.CustomerID +",'"+bocm.Original_LocalIp+"','"+bocm.Original_Status.ToString()+ "')";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独添加：Base_OriginalConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_OriginalConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        //批量修改
        public static bool Modify_Base_OriginalConfigManageArray(List<PBPid.DBModel.Base_OriginalConfigManage> _Base_OriginalConfigManage, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_OriginalConfigManage.Count; i++)
            {
                Base_OriginalConfigManage bocm = _Base_OriginalConfigManage[i];
                string sql = @"Update Pid.Base_OriginalConfigManage set Original_CheckingType ='" +
                bocm.Original_CheckingType +"',Original_LocalIp='"+bocm.Original_LocalIp+"',Original_Status='"+bocm.Original_Status.ToString()+ "',Original_ConfigName='" + bocm.Original_ConfigName + "',Original_ConfigPass ='" +
                bocm.Original_ConfigPass + "',Original_ConfigSi='" + bocm.Original_ConfigSi + "',Original_CorporationName ='" + bocm.Original_CorporationName +
                "',Original_Ip='" + bocm.Original_Ip + "',Original_OfficeNumber='" + bocm.Original_OfficeNumber +
                "',Original_Port=" + bocm.Original_Port + ",ProtocolType=" + bocm.ProtocolType + ",Original_Description ='" + bocm.Original_Description +
                "',CustomerID=" + bocm.CustomerID + " where Original_Id =" + bocm.Original_Id + "";
                sqls.Add(sql);
            }

            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量修改：Base_OriginalConfigManage 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_OriginalConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }
        //单独修改
        public static bool Modify_Base_OriginalConfigManage(PBPid.DBModel.Base_OriginalConfigManage _Base_OriginalConfigManage, string ip)
        {
            Base_OriginalConfigManage bocm = _Base_OriginalConfigManage;
            string sql = @"Update Pid.Base_OriginalConfigManage set Original_CheckingType ='" +
            bocm.Original_CheckingType +"',Original_LocalIp='"+bocm.Original_LocalIp+"',Original_Status='"+bocm.Original_Status.ToString()+ "',Original_ConfigName='" + bocm.Original_ConfigName + "',Original_ConfigPass ='" +
            bocm.Original_ConfigPass + "',Original_ConfigSi='" + bocm.Original_ConfigSi + "',Original_CorporationName ='" + bocm.Original_CorporationName +
            "',Original_Ip='" + bocm.Original_Ip + "',Original_OfficeNumber='" + bocm.Original_OfficeNumber +
            "',Original_Port=" + bocm.Original_Port + ",ProtocolType='" + bocm.ProtocolType + "',Original_Description ='" + bocm.Original_Description +
            "',CustomerID=" + bocm.CustomerID + " where Original_Id =" + bocm.Original_Id + "";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独修改：Base_OriginalConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_OriginalConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }
        //批量删除
        public static bool Delete_Base_OriginalConfigManageArray(List<PBPid.DBModel.Base_OriginalConfigManage> _Base_OriginalConfigManage, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_OriginalConfigManage.Count; i++)
            {
                Base_OriginalConfigManage bocm = _Base_OriginalConfigManage[i];
                string sql = "delete from Pid.Base_OriginalConfigManage where Original_Id =" + bocm.Original_Id + "";
                sqls.Add(sql);
            }
            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量删除：Base_OriginalConfigManage 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_OriginalConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }
        //单独删除
        public static bool Delete_Base_OriginalConfigManage(PBPid.DBModel.Base_OriginalConfigManage _Base_OriginalConfigManage, string ip)
        {
            Base_OriginalConfigManage bocm = _Base_OriginalConfigManage;
            string sql = "delete from Pid.Base_OriginalConfigManage where Original_Id =" + bocm.Original_Id + "";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独删除：Base_OriginalConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_OriginalConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        #endregion

        #region Base_UserConfigManage

        public static bool Add_Base_UserConfigManageArray(List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_UserConfigManage.Count; i++)
            {
                Base_UserConfigManage bucm = _Base_UserConfigManage[i];
                int IsDis = bucm.User_Disable == true ? 1 : 0;
                int IsLimit = bucm.User_LimitFlag == true ? 1 : 0;
                string sql = "insert into Pid.Base_UserConfigManage values(" + bucm.Customer_Id.ToString() + ",'" + bucm.User_Name + "','" +
                    bucm.User_Pass + "','" + bucm.User_DisableCmd + "'," + IsDis + ",'" +
                    bucm.User_BeginDate.ToString("yyyy-MM-dd") + "','" + bucm.User_EndDate.ToString("yyyy-MM-dd") + "','" + bucm.User_SendCount + "','" +
                    bucm.User_Description + "','"+bucm.User_Office + "',"+bucm.User_LimitCount+","+IsLimit+")";
                sqls.Add(sql);
            }
            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量添加：Base_UserConfigManage 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Add_Base_UserConfigManage(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage, string ip)
        {
            Base_UserConfigManage bucm = _Base_UserConfigManage;
            int IsDis = bucm.User_Disable == true ? 1 : 0;
            int IsLimit = bucm.User_LimitFlag == true ? 1 : 0;
            string sql = "insert into Pid.Base_UserConfigManage values(" + bucm.Customer_Id.ToString() + ",'" + bucm.User_Name + "','" +
                bucm.User_Pass + "','" + bucm.User_DisableCmd + "'," + IsDis + ",'" +
                bucm.User_BeginDate.ToString("yyyy-MM-dd") + "','" + bucm.User_EndDate.ToString("yyyy-MM-dd") + "','" + bucm.User_SendCount + "','" +
                bucm.User_Description + "','" + bucm.User_Office + "'," + bucm.User_LimitCount + "," + IsLimit + ")";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独添加：Base_UserConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Modify_Base_UserConfigManageArray(List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_UserConfigManage.Count; i++)
            {
                Base_UserConfigManage bucm = _Base_UserConfigManage[i];
                int IsDis = bucm.User_Disable == true ? 1 : 0;
                int IsLimit = bucm.User_LimitFlag == true ? 1 : 0;
                string sql = @"Update Pid.Base_UserConfigManage set Customer_Id =" +
                bucm.Customer_Id.ToString() + ",User_Name='" + bucm.User_Name + "',User_Pass ='" +
                bucm.User_Pass + "',User_DisableCmd='" + bucm.User_DisableCmd + "',User_Disable =" + IsDis +
                ",User_BeginDate='" + bucm.User_BeginDate.ToString("yyyy-MM-dd") + "',User_EndDate='" + bucm.User_EndDate.ToString("yyyy-MM-dd") +
                "',User_Description='" + bucm.User_Description + "',User_Office='"+bucm.User_Office+"',User_LimitCount="+
                bucm.User_LimitCount+",User_LimitFlag="+IsLimit+" where User_Id =" + bucm.User_Id + "";
                sqls.Add(sql);
            }

            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量修改：Base_UserConfigManage 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Modify_Base_UserConfigManage(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage, string ip)
        {
            Base_UserConfigManage bucm = _Base_UserConfigManage;
            int IsDis = bucm.User_Disable == true ? 1 : 0;
            int IsLimit = bucm.User_LimitFlag == true ? 1 : 0;
            string sql = @"Update Pid.Base_UserConfigManage set Customer_Id =" +
            bucm.Customer_Id.ToString() + ",User_Name='" + bucm.User_Name + "',User_Pass ='" +
            bucm.User_Pass + "',User_DisableCmd='" + bucm.User_DisableCmd + "',User_Disable =" + IsDis +
            ",User_BeginDate='" + bucm.User_BeginDate.ToString("yyyy-MM-dd") + "',User_EndDate='" + bucm.User_EndDate.ToString("yyyy-MM-dd") +
            "',User_Description='" + bucm.User_Description +"',User_Office='"+bucm.User_Office
            + "',User_LimitCount=" + bucm.User_LimitCount +",User_LimitFlag=" + IsLimit+ " where User_Id =" + bucm.User_Id + "";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独修改：Base_UserConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Modify_Base_UserSendCmdCount(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage, string ip)
        {
            Base_UserConfigManage bucm = _Base_UserConfigManage;
            int IsDis = bucm.User_Disable == true ? 1 : 0;
            int IsLimit = bucm.User_LimitFlag == true ? 1 : 0;
            string sql = @"Update Pid.Base_UserConfigManage set User_SendCount =" +bucm.User_SendCount 
                + " where User_Id =" + bucm.User_Id + "";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独修改：Base_UserConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Delete_Base_UserConfigManageArray(List<PBPid.DBModel.Base_UserConfigManage> _Base_UserConfigManage, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_UserConfigManage.Count; i++)
            {
                Base_UserConfigManage bucm = _Base_UserConfigManage[i];
                string sql = "delete from Pid.Base_UserConfigManage where User_Id =" + bucm.User_Id + "";
                sqls.Add(sql);
            }
            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量删除：Base_UserConfigManage 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Delete_Base_UserConfigManage(PBPid.DBModel.Base_UserConfigManage _Base_UserConfigManage, string ip)
        {
            Base_UserConfigManage bucm = _Base_UserConfigManage;
            string sql = "delete from Pid.Base_UserConfigManage where User_Id =" + bucm.User_Id + "";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独删除：Base_UserConfigManage 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_UserConfigManage(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        #endregion

        #region Base_CityInfoManage

        #endregion Base_CityInfoManage

        #region Base_PinYin

        public static bool Add_Base_PinYin(List<PBPid.DBModel.Base_PinYin> _Base_PinYin, string ip)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < _Base_PinYin.Count; i++)
            {
                Base_PinYin bucm = _Base_PinYin[i];
                string sql = "insert into Pid.Base_PinYin (hanzi,bianma,pinyin,remarks) values('" + bucm.HanZi + "','" + bucm.BianMa + "','" +
                    bucm.PinYin + "','" + bucm.Remarks  + "')";
                sqls.Add(sql);
            }
            bool _bool = db.RunTransaction(sqls);
            CallEvevtMethod("IP:" + ip + "，批量添加：Base_PinYin 表，" + sqls.Count + " 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_PinYin(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        public static bool Add_Base_PinYin(PBPid.DBModel.Base_PinYin _Base_PinYin, string ip)
        {
            Base_PinYin bucm = _Base_PinYin;

            string sql = "insert into Pid.Base_PinYin (hanzi,bianma,pinyin,remarks) values('" + bucm.HanZi + "','" + bucm.BianMa + "','" +
                bucm.PinYin + "','" + bucm.Remarks + "')";

            bool _bool = db.RunSql(sql);
            CallEvevtMethod("IP:" + ip + "，单独添加：Base_PinYin 表，1 条，执行结果：" + _bool.ToString() + " " + db.ErrorInfo);
            Get_Base_PinYin(true, ip);//添加完成后自动刷新缓存
            return _bool;
        }

        #endregion

        #region 释放

        /// <summary>
        /// 释放事件
        /// </summary>
        public static void Dispose()
        {
            OnCallEvent = null;
            OnRefrshCaChe = null;
        }

        #endregion

    }


    /*****************************************************************
     * 
    * 说明：LoginInfo 登录成功后存放在该类中用户信息
    * 
    * 编写人：罗俊杰 2012-6-1
    * 
    * 修改人：
    * 
    * ***************************************************************/
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// 客户信息
        /// </summary>
        public Base_Customers Customers = new Base_Customers();
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip = string.Empty;
    }

}
