using System;
using System.Collections.Generic;
using System.Text;
using PBPid.DBModel;

namespace IDBMethodSpace
{
    /*****************************************************************
     * 说明：IDBMethod 为远程与本地提供一致的数据操作方法
     * 
     * 编写人：罗俊杰 2012-5-23
     * 
     * 修改人：
     * 
     * ***************************************************************/

    #region 枚举
    /// <summary>
    /// 数据表
    /// </summary>
    [Serializable]
    public enum CacheTabName
    {
        Base_CommandManage,
        Base_OriginalConfigManage,
        Base_UserConfigManage,
        Base_ProtocolType,
        Base_PinYin,
        Base_CityInfoManage,
        All_Table
    }
    #endregion

    #region 接口
    /// <summary>
    /// 数据操作接口
    /// </summary>
    public interface IDBMethod
    {
        #region Base_CommandManage

        /// <summary>
        /// 获取 Base_CommandManage 全部数据
        /// </summary>
        /// <returns></returns>
        List<Base_CommandManage> Get_Base_CommandManage();
        /// <summary>
        /// 根据命令获取 Base_CommandManage 数据
        /// </summary>
        /// <param name="Cmd_Command"></param>
        /// <returns></returns>
        Base_CommandManage Get_Base_CommandManageByCmd(string Cmd_Command);

        #endregion

        #region  Base_OriginalConfigManage
        /// <summary>
        /// 获取全部原始配置
        /// </summary>
        /// <returns></returns>
        List<Base_OriginalConfigManage> Get_Base_OriginalConfigManage();
        /// <summary>
        /// 根据主键ID获取 Base_OriginalConfigManage 数据
        /// </summary>
        /// <param name="Original_Id">主键ID</param>
        /// <returns></returns>
        Base_OriginalConfigManage Get_Base_OriginalConfigManageByOriginal_Id(int Original_Id);
        /// <summary>
        /// 根据IP获取 Base_OriginalConfigManage 数据
        /// </summary>
        /// <param name="Original_Ip">要查询的IP</param>
        /// <returns></returns>
        List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByIp(string Original_Ip);
        /// <summary>
        /// 根据原始配置名称获取 Base_OriginalConfigManage 数据
        /// </summary>
        /// <param name="Original_ConfigName">原始配置名称</param>
        /// <returns></returns>
        List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByName(string Original_ConfigName);
        /// <summary>
        /// 根据公司名称获取原始配置
        /// </summary>
        /// <param name="CorporationName">公司名称</param>
        /// <returns></returns>
        List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByCorporationName(string CorporationName);

        /// <summary>
        /// 根据客户ID获取相关配置
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        List<Base_OriginalConfigManage> Get_Base_OriginalConfigManageByCustomerID(int CustomerID);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">数据集合</param>
        /// <returns></returns>
        bool Add_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">Base_OriginalConfigManage 实体</param>
        /// <returns></returns>
        bool Add_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">要修改的实体集合</param>
        /// <returns></returns>
        bool Modify_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage);
        /// <summary>
        /// 修改一条
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">数据实体</param>
        /// <returns></returns>
        bool Modify_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">要删除的集合</param>
        /// <returns></returns>
        bool Delete_Base_OriginalConfigManageArray(List<Base_OriginalConfigManage> _Base_OriginalConfigManage);
        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="_Base_OriginalConfigManage">要删除的实体</param>
        /// <returns></returns>
        bool Delete_Base_OriginalConfigManage(Base_OriginalConfigManage _Base_OriginalConfigManage);

        #endregion

        #region Base_UserConfigManage

        /// <summary>
        /// 获取全部 Base_UserConfigManage 数据
        /// </summary>
        /// <returns></returns>
        List<Base_UserConfigManage> Get_Base_UserConfigManage();
        /// <summary>
        /// 根据主键ID获取 Base_UserConfigManage 数据
        /// </summary>
        /// <param name="User_Id"></param>
        /// <returns></returns>
        Base_UserConfigManage Get_Base_UserConfigManageById(int User_Id);
        /// <summary>
        /// 根据放大用户名获取 Base_UserConfigManage 数据
        /// </summary>
        /// <param name="User_Name"></param>
        /// <returns></returns>
        List<Base_UserConfigManage> Get_Base_UserConfigManageByName(string User_Name);

        /// <summary>
        /// 根据客户ID获取放大配置用户
        /// </summary>
        /// <param name="Customer_Id">客户ID</param>
        /// <returns></returns>
        List<Base_UserConfigManage> Get_Base_UserConfigManageByCustomer_Id(int Customer_Id);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="_Base_UserConfigManage">数据集合</param>
        /// <returns></returns>
        bool Add_Base_UserConfigManageArray(List<Base_UserConfigManage> _Base_UserConfigManage);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="_Base_UserConfigManage">Base_UserConfigManage 实体</param>
        /// <returns></returns>
        bool Add_Base_UserConfigManage(Base_UserConfigManage _Base_UserConfigManage);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="_Base_UserConfigManage">要修改的实体集合</param>
        /// <returns></returns>
        bool Modify_Base_UserConfigManageArray(List<Base_UserConfigManage> _Base_UserConfigManage);
        /// <summary>
        /// 修改一条
        /// </summary>
        /// <param name="_Base_UserConfigManage">数据实体</param>
        /// <returns></returns>
        bool Modify_Base_UserConfigManage(Base_UserConfigManage _Base_UserConfigManage);
        /// <summary>
        /// 修改帐号的指令发送条数
        /// </summary>
        /// <param name="_Base_UserConfigManage">数据实体</param>
        /// <returns></returns>
        bool Modify_Base_UserSendCmdCount(Base_UserConfigManage _Base_UserConfigManage);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="_Base_UserConfigManage">要删除的集合</param>
        /// <returns></returns>
        bool Delete_Base_UserConfigManageArray(List<Base_UserConfigManage> _Base_UserConfigManage);
        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="_Base_UserConfigManage">要删除的实体</param>
        /// <returns></returns>
        bool Delete_Base_UserConfigManage(Base_UserConfigManage _Base_UserConfigManage);

        #endregion

        #region Base_CityInfoManage

        /// <summary>
        /// 获取全部 Base_CityInfoManage 数据
        /// </summary>
        /// <returns></returns>
        List<Base_CityInfoManage> Get_Base_CityInfoManage(int PageIndex);
        #endregion Base_CityInfoManage

        #region ProtocolType

        /// <summary>
        /// 获取协议类型
        /// </summary>
        /// <returns></returns>
        List<Base_ProtocolType> Get_Base_ProtocolType();
        /// <summary>
        /// 根据ID获取协议类型
        /// </summary>
        /// <returns></returns>
        Base_ProtocolType Get_Base_ProtocolTypeById(int Protocol_Id);
        /// <summary>
        /// 根据协议类型获取ID
        /// </summary>
        /// <returns></returns>
        Base_ProtocolType Get_Base_ProtocolTypeByType(string ProtocolType);

        #endregion

        #region Base_PinYin

        /// <summary>
        /// 获取 Base_PinYin 数据
        /// </summary>
        /// <returns></returns>
        List<Base_PinYin> Get_Base_PinYin();

        bool Add_Base_PinYin(List<PBPid.DBModel.Base_PinYin> _Base_PinYin);

        bool Add_Base_PinYin(PBPid.DBModel.Base_PinYin _Base_PinYin);

        #endregion

        #region customers

        /// <summary>
        /// 登录验证客户
        /// </summary>
        /// <param name="LoginName">登录名</param>
        /// <param name="LoginPass">登录密码</param>
        /// <param name="IsLogin">是否登录过了</param>
        /// <returns></returns>
        Base_Customers Get_Base_CustomersByLogin(string LoginName, string LoginPass, ref bool IsLogin);

        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="bc">客户信息</param>
        /// <returns>bool</returns>
        bool Modify_Base_Customers(Base_Customers bc);

        /// <summary>
        /// 登出系统
        /// </summary>
        /// <param name="bc">客户信息</param>
        /// <returns></returns>
        void LoginOut(Base_Customers bc);

        #endregion

        #region 刷新
        /// <summary>
        /// 根据表名刷新缓存
        /// </summary>
        /// <param name="TabName"></param>
        /// <returns></returns>
        bool RefreshCache(CacheTabName TabName);
        #endregion
    }

    #endregion
}
