using System;
using System.Collections.Generic;
using System.Text;
using PBPid.Base;
using PBPid.DBModel;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections;
using System.Net;
using System.IO;

namespace PBPid.ServerManage
{
    public class ServerManage
    {
        /// <summary>
        /// 原始配置信息列表
        /// </summary>
        private List<Base_OriginalConfigManage> m_OriginalConfigManage = null;

        /// <summary>
        /// 配置处理类列表
        /// </summary>
        private NoSortHashTable m_ServerInfoList = null;

        /// <summary>
        /// 是否输出调试信息
        /// </summary>
        private bool m_DebugFlag = false;

        /// <summary>
        /// 是否输出收发数据包调试信息
        /// </summary>
        private bool m_DebugDataFlag = false;

        /// <summary>
        /// F6F7监听端口
        /// </summary>
        private int m_SharePort = 3510;

        /// <summary>
        /// 最大指令计数
        /// </summary>
        private readonly int m_MaxCommandCount = 10000;

        /// <summary>
        /// 不自动连接时间段
        /// </summary>
        private string m_NoAutoConTime = "";

        /// <summary>
        /// 网站用户名
        /// </summary>
        private string m_WebUser = "";

        /// <summary>
        /// 发送指令时间间隔（单位：毫秒）
        /// </summary>
        private int m_Interval = 10;

        /// <summary>
        /// 原始配置信息列表
        /// </summary>
        public List<Base_OriginalConfigManage> OriginalConfigManage
        {
            get { return m_OriginalConfigManage; }
            set { m_OriginalConfigManage = value; }
        }

        /// <summary>
        /// 是否输出调试信息
        /// </summary>
        public bool DebugFlag
        {
            get { return m_DebugFlag; }
            set { m_DebugFlag = value; }
        }

        /// <summary>
        /// 是否输出收发数据包调试信息
        /// </summary>
        public bool DebugDataFlag
        {
            get { return m_DebugDataFlag; }
            set { m_DebugDataFlag = value; }
        }

        /// <summary>
        /// F6F7监听端口
        /// </summary>
        public int SharePort
        {
            get { return m_SharePort; }
            set { m_SharePort = value; }
        }

        /// <summary>
        /// 配置处理类列表
        /// </summary>
        public NoSortHashTable ServerInfoList
        {
            get { return m_ServerInfoList; }
            set { m_ServerInfoList = value; }
        }

        /// <summary>
        /// 不自动连接时间段
        /// </summary>
        public string NoAutoConTime
        {
            get { return m_NoAutoConTime; }
            set { m_NoAutoConTime = value; }
        }

        /// <summary>
        /// 网站用户名
        /// </summary>
        public string WebUser
        {
            get { return m_WebUser; }
            set { m_WebUser = value; }
        }

        /// <summary>
        /// 发送指令时间间隔（单位：毫秒）
        /// </summary>
        public int Interval
        {
            get { return m_Interval; }
            set { m_Interval = value; }
        }


        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                if (m_ServerInfoList != null)
                {
                    m_ServerInfoList.Clear();
                    m_ServerInfoList = null;
                }

                m_ServerInfoList = new NoSortHashTable();

                //循环启动配置
                for (int i = 0; i < m_OriginalConfigManage.Count; i++)
                {
                    //判断配置是否启用
                    if (m_OriginalConfigManage[i].Original_Status == "启用")
                    {
                        if (m_OriginalConfigManage[i].ProtocolType == "信天游")
                        {
                            //声明信天游处理类
                            EtermXTYManage tmpEtermXTYManage = new EtermXTYManage();
                            //原始配置类信息
                            tmpEtermXTYManage.OriginalConfigManage = m_OriginalConfigManage[i];
                            //认证端口
                            tmpEtermXTYManage.EtermPort = m_OriginalConfigManage[i].Original_Port;
                            //配置类型（地址认证  密码认证  信天游）
                            tmpEtermXTYManage.EtermType = m_OriginalConfigManage[i].ProtocolType;
                            //是否输出调试信息标志
                            tmpEtermXTYManage.DebugFlag = m_DebugFlag;
                            //输出调试数据包标志
                            tmpEtermXTYManage.DebugDataFlag = m_DebugDataFlag;
                            //不自动连接时间段
                            tmpEtermXTYManage.NoAutoConTime = m_NoAutoConTime;
                            //网站用户名
                            tmpEtermXTYManage.WebUser = m_WebUser;
                            //指令间隔时间
                            tmpEtermXTYManage.Interval = m_Interval;
                            //F6F7监听端口
                            //tmpEtermXTYManage.SharePort = SharePort;
                            //启动配置连接认证服务器
                            //tmpEtermXTYManage.DoWithXTY();
                            //启动线程处理
                            tmpEtermXTYManage.EtermThread = new Thread(new ThreadStart(tmpEtermXTYManage.EtermTimeProc));
                            tmpEtermXTYManage.EtermThread.Start();

                            //添加到配置处理类列表
                            m_ServerInfoList.Add(m_OriginalConfigManage[i].Original_Id, tmpEtermXTYManage);

                            break;
                        }
                        else
                        {
                            if (m_OriginalConfigManage[i].ProtocolType.IndexOf("地址认证") != -1)
                            {
                                //声明350处理类
                                Eterm350Manage tmpEterm350Manage = new Eterm350Manage();
                                //原始配置类信息
                                tmpEterm350Manage.OriginalConfigManage = m_OriginalConfigManage[i];
                                //认证端口
                                tmpEterm350Manage.EtermPort = m_OriginalConfigManage[i].Original_Port;
                                //配置类型（地址认证  密码认证  信天游）
                                tmpEterm350Manage.EtermType = m_OriginalConfigManage[i].ProtocolType;
                                //是否输出调试信息标志
                                tmpEterm350Manage.DebugFlag = m_DebugFlag;
                                //输出调试数据包标志
                                tmpEterm350Manage.DebugDataFlag = m_DebugDataFlag;
                                //不自动连接时间段
                                tmpEterm350Manage.NoAutoConTime = m_NoAutoConTime;
                                //网站用户名
                                tmpEterm350Manage.WebUser = m_WebUser;
                                //指令间隔时间
                                tmpEterm350Manage.Interval = m_Interval;
                                //启动配置连接认证服务器
                                //tmpEterm350Manage.DoWith350();
                                //启动线程处理
                                tmpEterm350Manage.EtermThread = new Thread(new ThreadStart(tmpEterm350Manage.EtermTimeProc));
                                tmpEterm350Manage.EtermThread.Start();

                                //添加到配置处理类列表
                                m_ServerInfoList.Add(m_OriginalConfigManage[i].Original_Id, tmpEterm350Manage);
                            }
                            else
                            {
                                //判断配置类型
                                switch (m_OriginalConfigManage[i].Original_Port)
                                {
                                    //加密认证
                                    case 443:
                                        {
                                            //声明443处理类
                                            Eterm443Manage tmpEterm443Manage = new Eterm443Manage();
                                            //原始配置类信息
                                            tmpEterm443Manage.OriginalConfigManage = m_OriginalConfigManage[i];
                                            //认证端口
                                            tmpEterm443Manage.EtermPort = m_OriginalConfigManage[i].Original_Port;
                                            //配置类型（地址认证  密码认证  信天游）
                                            tmpEterm443Manage.EtermType = m_OriginalConfigManage[i].ProtocolType;
                                            //是否输出调试信息标志
                                            tmpEterm443Manage.DebugFlag = m_DebugFlag;
                                            //输出调试数据包标志
                                            tmpEterm443Manage.DebugDataFlag = m_DebugDataFlag;
                                            //F6F7监听端口
                                            tmpEterm443Manage.SharePort = SharePort;
                                            //不自动连接时间段
                                            tmpEterm443Manage.NoAutoConTime = m_NoAutoConTime;
                                            //网站用户名
                                            tmpEterm443Manage.WebUser = m_WebUser;
                                            //指令时间间隔
                                            tmpEterm443Manage.Interval = m_Interval;
                                            //启动配置连接认证服务器
                                            //tmpEterm443Manage.DoWith443();
                                            //启动线程处理
                                            tmpEterm443Manage.EtermThread = new Thread(new ThreadStart(tmpEterm443Manage.EtermTimeProc));
                                            tmpEterm443Manage.EtermThread.Start();

                                            //添加到配置处理类列表
                                            m_ServerInfoList.Add(m_OriginalConfigManage[i].Original_Id, tmpEterm443Manage);

                                            break;
                                        }

                                    //无加密（地址认证、密码认证或信天游）
                                    default:
                                        {
                                            //声明350处理类
                                            Eterm350Manage tmpEterm350Manage = new Eterm350Manage();
                                            //原始配置类信息
                                            tmpEterm350Manage.OriginalConfigManage = m_OriginalConfigManage[i];
                                            //认证端口
                                            tmpEterm350Manage.EtermPort = m_OriginalConfigManage[i].Original_Port;
                                            //配置类型（地址认证  密码认证  信天游）
                                            tmpEterm350Manage.EtermType = m_OriginalConfigManage[i].ProtocolType;
                                            //是否输出调试信息标志
                                            tmpEterm350Manage.DebugFlag = m_DebugFlag;
                                            //输出调试数据包标志
                                            tmpEterm350Manage.DebugDataFlag = m_DebugDataFlag;
                                            //不自动连接时间段
                                            tmpEterm350Manage.NoAutoConTime = m_NoAutoConTime;
                                            //网站用户名
                                            tmpEterm350Manage.WebUser = m_WebUser;
                                            //指令间隔时间
                                            tmpEterm350Manage.Interval = m_Interval;
                                            //启动配置连接认证服务器
                                            //tmpEterm350Manage.DoWith350();
                                            //启动线程处理
                                            tmpEterm350Manage.EtermThread = new Thread(new ThreadStart(tmpEterm350Manage.EtermTimeProc));
                                            tmpEterm350Manage.EtermThread.Start();

                                            //添加到配置处理类列表
                                            m_ServerInfoList.Add(m_OriginalConfigManage[i].Original_Id, tmpEterm350Manage);

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "启动服务出现异常，错误信息：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            try
            {
                //关闭配置连接
                if (m_ServerInfoList.Count != 0)
                {
                    foreach (BaseManage etermmgr in m_ServerInfoList.Values)
                    {
                        etermmgr.ReleaseEterm();
                    }

                    //清空列表
                    m_ServerInfoList.Clear();
                    m_ServerInfoList = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "关闭配置服务出现异常，错误信息：" + ex.Message);
                return false;
            }
        }

        #region 获取可用配置
        /// <summary>
        /// 获取可用配置
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="OfficeCode">Office编号（可以为空）</param>
        /// <param name="EtermId">返回找到的可用配置key值</param>
        /// <param name="EtermName">返回找到的可用配置名称，用于记录日志</param>
        /// <returns></returns>
        public bool GetIdleEtermInfo(string command, string OfficeCode, ref int EtermId,ref string EtermName)
        {
            //不使用信天游配置
            bool NoXTYFlag = false;
            //行程单打印只能使用非信天游配置
            if ((command.Trim().ToLower().IndexOf("prinv") != -1) || (command.Trim().ToLower().IndexOf("vtinv") != -1))
            {
                NoXTYFlag = true;
            }

            //配置发送指令计数
            int tmpCommandCount = -1;
            int tmpEtermId = -1;
            BaseManage ResBaseManage = null;

            lock (m_ServerInfoList)
            {
                foreach (int id in m_ServerInfoList.Keys)
                {
                    BaseManage tmpBaseManage = m_ServerInfoList[id] as BaseManage;

                    //配置不是空闲状态
                    if (tmpBaseManage.IdleFlag != 0)
                    {
                        continue;
                    }

                    //配置Office号不是当前指令指定的Office号
                    if (tmpBaseManage.OriginalConfigManage.Original_OfficeNumber.Trim().ToUpper() != OfficeCode.Trim().ToUpper())
                    {
                        continue;
                    }

                    //信天游配置，且当前指令不使用信天游配置
                    if ((tmpBaseManage.EtermType == "信天游") && (NoXTYFlag))
                    {
                        continue;
                    }

                    //如果发送指令数量大于了最大指令数量，则减去最大指令数量
                    if (tmpBaseManage.CommandCount > m_MaxCommandCount)
                    {
                        tmpBaseManage.CommandCount -= m_MaxCommandCount;
                    }

                    //均衡配置
                    if (tmpCommandCount == -1)
                    {
                        tmpCommandCount = tmpBaseManage.CommandCount;
                        ResBaseManage = tmpBaseManage;
                        tmpEtermId = id;
                    }
                    else
                    {
                        if (Math.Abs(tmpCommandCount - tmpBaseManage.CommandCount) > (m_MaxCommandCount / 2))
                        {
                            if (tmpCommandCount < tmpBaseManage.CommandCount)
                            {
                                tmpCommandCount = tmpBaseManage.CommandCount;
                                ResBaseManage = tmpBaseManage;
                                tmpEtermId = id;
                            }
                        }
                        else
                        {
                            if (tmpCommandCount > tmpBaseManage.CommandCount)
                            {
                                tmpCommandCount = tmpBaseManage.CommandCount;
                                ResBaseManage = tmpBaseManage;
                                tmpEtermId = id;
                            }
                        }
                    }
                }

                if (ResBaseManage != null)
                {
                    //设置为忙碌
                    ResBaseManage.IdleFlag = 1;
                    //忙碌开始时间
                    ResBaseManage.SetBusyTime = DateTime.Now;
                }
            }

            if (ResBaseManage != null)
            {
                EtermId = tmpEtermId;
                if (ResBaseManage.EtermType.IndexOf("地址认证") != -1)
                {
                    EtermName = "("+ ResBaseManage.OriginalConfigManage.Original_OfficeNumber + ")" + ResBaseManage.OriginalConfigManage.Original_LocalIp + ":" + ResBaseManage.OriginalConfigManage.Original_Port;
                }
                else
                {
                    EtermName = "(" + ResBaseManage.OriginalConfigManage.Original_OfficeNumber + ")" + ResBaseManage.OriginalConfigManage.Original_ConfigName + ":" + ResBaseManage.OriginalConfigManage.Original_Port;
                }
                return true;
            }
            else
            {
                EtermId = -1;
                return false;
            }
        }
        #endregion 获取可用配置

        #region 判断是否为信天游配置
        public bool IfXinTianYouEterm(int EtermId)
        {
            try
            {
                BaseManage tmpBaseManage = m_ServerInfoList[EtermId] as BaseManage;
                if (tmpBaseManage.EtermType == "信天游")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "获取配置的类型信息出错，错误信息：" + ex.Message);
                return false;
            }
        }
        #endregion 判断是否为信天游配置

        #region 释放对配置的占用
        /// <summary>
        /// 释放对配置的占用
        /// </summary>
        /// <param name="EtermId">配置序号</param>
        public void FreeEtermInfo(int EtermId)
        {
            try
            {
                BaseManage tmpBaseManage = m_ServerInfoList[EtermId] as BaseManage;
                //Kevin 2013-01-11 Edit
                //如果配置为忙碌状态，则改为空闲
                if (tmpBaseManage.IdleFlag == 1)
                {
                    tmpBaseManage.IdleFlag = 0;
                }
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "释放配置的占用操作出错，错误信息：" + ex.Message);
            }
        }
        #endregion 释放对配置的占用

        #region 直接转发指令并返回结果信息
        public bool SendCmdDirectAndGetResult(int EtermId,string username,byte[] CmdBuf, ref byte[] ResBuf,ref int RecCount)
        {
            try
            {
                BaseManage tmpBaseManage = m_ServerInfoList[EtermId] as BaseManage;
                if (tmpBaseManage.EtermType == "信天游")
                {
                    return ((EtermXTYManage)tmpBaseManage).SendCmdDirectAndGetResult(username, CmdBuf, ref ResBuf,ref RecCount);
                }
                else if (tmpBaseManage.EtermType == "地址认证")
                {
                    return ((Eterm350Manage)tmpBaseManage).SendCmdDirectAndGetResult(username, CmdBuf, ref ResBuf, ref RecCount);
                }
                else
                {
                    if (tmpBaseManage.EtermPort == 350)
                    {
                        return ((Eterm350Manage)tmpBaseManage).SendCmdDirectAndGetResult(username, CmdBuf, ref ResBuf, ref RecCount);
                    }
                    else if (tmpBaseManage.EtermPort == 443)
                    {
                        return ((Eterm443Manage)tmpBaseManage).SendCmdDirectAndGetResult(username, CmdBuf, ref ResBuf, ref RecCount);
                    }
                    else
                    {
                        return ((Eterm350Manage)tmpBaseManage).SendCmdDirectAndGetResult(username, CmdBuf,  ref ResBuf, ref RecCount);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "帐号：" + username + " 直接发送指令：，处理出错，错误信息：" + ex.Message);
                return false;
            }
            return true;
        }
        #endregion 直接转发指令并返回结果信息

        #region 发送指令并获取返回结果信息
        /// <summary>
        /// 发送指令并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, int EtermId, ref byte[] ResBuf, ref string ErrorMessage)
        {
            try
            {
                BaseManage tmpBaseManage = m_ServerInfoList[EtermId] as BaseManage;
                if (tmpBaseManage.EtermType == "信天游")
                {
                    return ((EtermXTYManage)tmpBaseManage).SendCommandAndGetResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                }
                else if (tmpBaseManage.EtermType == "地址认证")
                {
                    return ((Eterm350Manage)tmpBaseManage).SendCommandAndGetResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                }
                else
                {
                    if (tmpBaseManage.EtermPort == 350)
                    {
                        return ((Eterm350Manage)tmpBaseManage).SendCommandAndGetResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                    }
                    else if (tmpBaseManage.EtermPort == 443)
                    {
                        return ((Eterm443Manage)tmpBaseManage).SendCommandAndGetResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                    }
                    else
                    {
                        return ((Eterm350Manage)tmpBaseManage).SendCommandAndGetResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "帐号：" + username + " 发送指令：" + cmd + "，处理出错，错误信息：" + ex.Message);
                return false;
            }
        }
        #endregion 发送指令并获取返回结果信息

        #region 发送指令自动PN并获取返回结果信息
        /// <summary>
        /// 发送指令自动根据最后一个指令进行PN并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetAllPnResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, int EtermId, ref byte[] ResBuf, ref string ErrorMessage)
        {
            try
            {
                BaseManage tmpBaseManage = m_ServerInfoList[EtermId] as BaseManage;
                if (tmpBaseManage.EtermType == "信天游")
                {
                    return ((EtermXTYManage)tmpBaseManage).SendCommandAndGetAllPnResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                }
                else if (tmpBaseManage.EtermType == "地址认证")
                {
                    return ((Eterm350Manage)tmpBaseManage).SendCommandAndGetAllPnResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                }
                else
                {
                    if (tmpBaseManage.EtermPort == 350)
                    {
                        return ((Eterm350Manage)tmpBaseManage).SendCommandAndGetAllPnResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                    }
                    else if (tmpBaseManage.EtermPort == 443)
                    {
                        return ((Eterm443Manage)tmpBaseManage).SendCommandAndGetAllPnResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                    }
                    else
                    {
                        return ((Eterm350Manage)tmpBaseManage).SendCommandAndGetAllPnResult(username, cmd, CmdBuf, OfficeCode, allFlag, ref ResBuf, ref ErrorMessage);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "帐号：" + username + " 发送指令：" + cmd + "，处理出错，错误信息：" + ex.Message);
                return false;
            }
        }
        #endregion 发送指令自动PN并获取返回结果信息
    }

    /// <summary>
    /// 350Eterm处理类（包括密码认证和地址认证两种）
    /// </summary>
    public class Eterm350Manage : BaseManage
    {
        //处理350配置
        public void DoWith350()
        {
            try
            {
                if (NetClient != null)
                {
                    NetClient = null;
                }

                //无客户端的地址认证则指定本地IP
                if (EtermType == "DDN地址认证")
                {
                    IPAddress ipAddress = IPAddress.Parse(OriginalConfigManage.Original_LocalIp);
                    IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 0);
                    NetClient = new TcpClient(ipLocalEndPoint);
                }
                //VPN地址认证从地址表中获取 10.的地址
                else if (EtermType == "VPN地址认证")
                {
                    IPAddress ipAddress = IPAddress.Parse(Address.GetDlanIP());
                    IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 0);
                    NetClient = new TcpClient(ipLocalEndPoint);
                }
                else
                {
                    NetClient = new TcpClient();
                }
                NetClient.Connect(OriginalConfigManage.Original_Ip, OriginalConfigManage.Original_Port);


                if (DebugFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置（地址认证）：" + OriginalConfigManage.Original_LocalIp + " 正在准备连接航信服务器：" + OriginalConfigManage.Original_Ip + "，端口：" + OriginalConfigManage.Original_Port.ToString());
                    }
                    else
                    {
                        Log.Record("server.log", "配置（密码认证）：" + OriginalConfigManage.Original_ConfigName + " 正在准备连接航信服务器：" + OriginalConfigManage.Original_Ip + "，端口：" + OriginalConfigManage.Original_Port.ToString());
                    }
                }

                NetStream = NetClient.GetStream();

                //登入（350地址认证的包头）
                byte[] C = //{ 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x30, 0x32, 0x31, 0x38, 0x36, 0x35, 0x34, 0x66, 0x62, 0x30, 0x63, 0x31, 0x39, 0x32, 0x2E, 0x31, 0x36, 0x38, 0x2E, 0x31, 0x2E, 0x34, 0x20, 0x20, 0x20, 0x20, 0x33, 0x38, 0x34, 0x37, 0x30, 0x31, 0x30, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//{ 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x30, 0x32, 0x31, 0x38, 0x36, 0x35, 0x34, 0x66, 0x62, 0x30, 0x63, 0x31, 0x39, 0x32, 0x2E, 0x31, 0x36, 0x38, 0x2E, 0x31, 0x2E, 0x34, 0x20, 0x20, 0x20, 0x20, 0x33, 0x38, 0x34, 0x37, 0x30, 0x31, 0x30, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//{ 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x30, 0x32, 0x31, 0x38, 0x36, 0x35, 0x34, 0x66, 0x62, 0x30, 0x63, 0x31, 0x30, 0x2E, 0x31, 0x35, 0x30, 0x2E, 0x36, 0x36, 0x2E, 0x31, 0x39, 0x36, 0x20, 0x20, 0x33, 0x36, 0x31, 0x30, 0x34, 0x31, 0x30, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                {0x01 ,0xA2 ,0x6E ,0x6B ,0x67 ,0x30 ,0x31 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x31 
               ,0x32 ,0x33 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x30 ,0x30 ,0x65 ,0x30 ,0x34 ,0x63 ,0x31 
               ,0x64 ,0x33 ,0x37 ,0x64 ,0x37 ,0x31 ,0x31 ,0x36 ,0x2E ,0x32 ,0x35 ,0x34 ,0x2E ,0x32 ,0x30 ,0x36 ,0x2E ,0x32 ,0x33 
               ,0x20 ,0x33 ,0x38 ,0x34 ,0x37 ,0x34 ,0x31 ,0x30 ,0x00 ,0x30 ,0x30 ,0x30 ,0x30 ,0x30 ,0x30 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00};


                //移动控制台标志
                bool dlanFlag = false;

                //地址认证
                if (EtermType.IndexOf("地址认证") != -1)
                {
                    byte[] px = { 0x21, 0x21, 0x21, 0x21, 0x21, 0x21, 0x21, 0x21 };
                    Array.Copy(px, 0, C, 2, px.Length);
                    Array.Copy(px, 0, C, 18, px.Length);

                    //取得本机IP地址
                    string dlanip = OriginalConfigManage.Original_LocalIp;

                    //替换IP地址
                    byte[] ipbuf = Encoding.Default.GetBytes(dlanip);
                    Array.Copy(ipbuf, 0, C, 62, ipbuf.Length);
                    dlanFlag = true;
                }
                else
                {
                    //帐号
                    byte[] px = ASCIIEncoding.Default.GetBytes(OriginalConfigManage.Original_ConfigName);
                    Array.Copy(px, 0, C, 2, px.Length);

                    //密码
                    byte[] px1 = ASCIIEncoding.Default.GetBytes(OriginalConfigManage.Original_ConfigPass);
                    Array.Copy(px1, 0, C, 18, px1.Length);
                }

                NetStream.Write(C, 0, C.Length);
                NetStream.Flush();

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送认证数据包：", C, C.Length);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送认证数据包：", C, C.Length);
                    }
                }


                byte[] D = new byte[1024];
                int jscount = 0;
                jscount = NetStream.Read(D, 0, 0x37);

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 收到认证结果数据包：", D, jscount);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 收到认证结果数据包：", D, jscount);
                    }
                }

                if (D[2] == 0x00) //error
                {
                    string s = ASCIIEncoding.Default.GetString(D, 3, 0x37 - 3);
                    //登录失败，写日志和显示信息

                    if (DebugFlag)
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "帐号：" + OriginalConfigManage.Original_LocalIp + " 登录航信服务器失败，失败返回信息：" + s);
                        }
                        else
                        {
                            Log.Record("server.log", "帐号：" + OriginalConfigManage.Original_ConfigName + " 登录航信服务器失败，失败返回信息：" + s);
                        }
                    }

                    if (s.IndexOf("请检查用户名和口令") != -1)
                    {
                        IdleFlag = 2;
                    }
                    else
                    {
                        IdleFlag = 4;
                    }

                    NetClient = null;
                    NetStream = null;

                    return;
                }

                if (DebugFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "帐号：" + OriginalConfigManage.Original_LocalIp + " 登录航信服务器成功！");
                    }
                    else
                    {
                        Log.Record("server.log", "帐号：" + OriginalConfigManage.Original_ConfigName + " 登录航信服务器成功！");
                    }
                }

                //黑屏包头
                Array.Copy(D, 8, m_cmdHeadBuf, 0, 2);

                //Kevin 特殊处理
                //如果标志头为 0xD1，则替换为0x51
                if (m_cmdHeadBuf[1] == 0xD1)
                {
                    m_cmdHeadBuf[1] = 0x51;
                }


                byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE, 0, FE.Length);
                NetStream.Flush();

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送数据包：", FE, FE.Length);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE, FE.Length);
                    }
                }

                byte[] FE2 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE2, 0, FE2.Length);
                NetStream.Flush();

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送数据包：", FE2, FE2.Length);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE2, FE2.Length);
                    }
                }

                byte[] FE3 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE3, 0, FE3.Length);
                NetStream.Flush();

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送数据包：", FE3, FE3.Length);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE3, FE3.Length);
                    }
                }

                if (this.EtermType == "胜意PID")
                {
                    byte[] FE4 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    NetStream.Write(FE4, 0, FE4.Length);
                    NetStream.Flush();

                    if (DebugDataFlag)
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送数据包：", FE4, FE4.Length);
                        }
                        else
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE4, FE4.Length);
                        }
                    }
                }


                #region 屏蔽
                //移动控制台处理
                //Kevin 2009-12-02 屏蔽
                //航信更改通信格式，去掉F6F7数据信息
                //if (dlanFlag)
                //{
                //    //判断是否收到F6数据
                //    byte[] F6 = new byte[1024];
                //    netStream.Read(F6, 0, 0x84);

                //    if (F6[1] == 0xF6)
                //    {

                //        //Log.Record("server.log", "收到航信F6数据包：", F6, 0x84);

                //        lock (ThreadWork.lockX)
                //        {
                //            if (eterm3 == null)
                //            {
                //                eterm3 = new Eterm3Lib.ApplicationClass();
                //                eterm3.Connect(0, "127.0.0.1:" + Convert.ToString(m_shareport * 10), "$", "$");
                //            }

                //            ThreadWork.F6 = F6;
                //            ThreadWork.newflag = true;

                //            //Console.WriteLine("获取 F6..." + server);

                //            //Log.Record("server.log", "正在准备获取F7数据信息...");

                //            //等待获取到F7信息
                //            //lock (ThreadWork.lockY) ;
                //            int count = 0;
                //            while ((ThreadWork.newflag) && (count < 20))
                //            {
                //                Thread.Sleep(100);
                //                count++;
                //                continue;
                //            }

                //            if (ThreadWork.newflag)
                //            {
                //                //取得F7信息失败
                //                if (eterm3 != null)
                //                {
                //                    eterm3.Exit();
                //                    eterm3 = null;
                //                }

                //                //停止ThreadWork的监听端口，然后再重新开启监听端口
                //                if (ThreadWork.sslServer != null)
                //                {
                //                    ThreadWork.sslServer.Stop();
                //                }

                //                return;
                //            }


                //            //Log.Record("server.log", "已经获取到F7数据信息...");

                //            //Console.WriteLine("发送 F7..." + server);
                //            //byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //            //netStream.Write(FE, 0, FE.Length);
                //            //netStream.Flush();

                //            //Log.Record("server.log", "向航信发送数据包：", FE, 8);

                //            netStream.Write(ThreadWork.F7, 0, ThreadWork.F7.Length);
                //            netStream.Flush();

                //            //Log.Record("server.log", "向航信发送F7数据包：", ThreadWork.F7, ThreadWork.F7.Length);

                //            if (eterm3 != null)
                //            {
                //                eterm3.Exit();
                //                eterm3 = null;
                //            }
                //        }
                //        sendcmdTime = DateTime.Now;

                //        byte[] FD = new byte[1024];
                //        netStream.Read(FD, 0, 0x06);
                //    }
                //    else
                //    {
                //        //byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //        //netStream.Write(FE, 0, 0x11);
                //        //netStream.Flush();
                //        sendcmdTime = DateTime.Now;
                //    }
                //}
                //else
                //{
                //Log.Record("server.log", "分析黑屏包头：", cmdhead, 2);

                //byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //netStream.Write(FE, 0, 0x11);
                //netStream.Flush();

                //Log.Record("server.log", "向航信发送数据包：", FE, 8);
                #endregion 屏蔽

                SendCmdTime = DateTime.Now;

                int rcount = 0;

                bool F6Flag = false;

                if (this.EtermType == "胜意PID")
                {
                    byte[] FD = new byte[1024*1024];

                    rcount = NetStream.Read(FD, 0, 1024*1024);
                    int tolcount = rcount;

                    if (DebugDataFlag)
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "1配置：" + OriginalConfigManage.Original_LocalIp + " 收到返回数据包：", FD, rcount);
                        }
                        else
                        {
                            Log.Record("server.log", "1配置：" + OriginalConfigManage.Original_ConfigName + " 收到返回数据包：", FD, rcount);
                        }
                    }

                    rcount = NetStream.Read(FD, 0, 1024);

                    rcount = NetStream.Read(FD, 0, 1024);

                }
                else
                {

                    byte[] FD = new byte[1024];

                    rcount = NetStream.Read(FD, 0, 1024);
                    int tolcount = rcount;

                    if (DebugDataFlag)
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "1配置：" + OriginalConfigManage.Original_LocalIp + " 收到返回数据包：", FD, rcount);
                        }
                        else
                        {
                            Log.Record("server.log", "1配置：" + OriginalConfigManage.Original_ConfigName + " 收到返回数据包：", FD, rcount);
                        }
                    }

                    while (tolcount < 18)
                    {
                        rcount = NetStream.Read(FD, 0, 1024);
                        tolcount += rcount;

                        if (rcount == 0)
                        {
                            IdleFlag = 4;
                            //错误计数
                            ErrorCount++;
                            Log.Record("server.log", "2配置：" + OriginalConfigManage.Original_LocalIp + " 收到空数据包，退出重新连接...");
                            break;
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "2配置：" + OriginalConfigManage.Original_LocalIp + " 收到返回数据包：", FD, rcount);
                            }
                            else
                            {
                                Log.Record("server.log", "2配置：" + OriginalConfigManage.Original_ConfigName + " 收到返回数据包：", FD, rcount);
                            }
                        }
                    }

                    //判断是否F6数据
                    if (FD[0] == 0x01 && FD[1] == 0xF6)
                    {
                        F6Flag = true;
                    }
                }



                //Kevin 2013-11-22 增加
                //有VPN是接收到F6数据，发送F7数据包
                if (F6Flag)
                {
                    //Kevin 发送F7并接受返回信息
                    byte[] FE5 = { 0x01, 0xF7, 0x01, 0x04, 0x33, 0x37, 0x33, 0x30, 0x34, 0x31, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00 };

                    NetStream.Write(FE5, 0, FE5.Length);
                    NetStream.Flush();

                    if (DebugDataFlag)
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送数据包：", FE5, FE5.Length);
                        }
                        else
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE5, FE5.Length);
                        }
                    }

                    //接收返回结果
                    byte[] FD9 = new byte[m_MaxPacket];

                    rcount = NetStream.Read(FD9, 0, m_MaxPacket);

                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                    if (rcount != 0)
                    {
                        int tmpRecCount = rcount;

                        byte[] RecBuf2 = new byte[m_MaxPacket];

                        //数据包中长度
                        int RecCount2 = Convert.ToInt32(FD9[2]) * 16 * 16 + Convert.ToInt32(FD9[3]);

                        //未接收完毕，继续接收数据包
                        while ((tmpRecCount != 0) && (rcount < RecCount2))
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                            }

                            tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                            if (tmpRecCount != 0)
                            {
                                //合并数据包
                                Array.Copy(RecBuf2, 0, FD9, rcount, tmpRecCount);
                            }

                            rcount += tmpRecCount;
                        }
                    }
                }

                //Kevin 发送F9并接受返回信息
                byte[] FE6 = { 0x01, 0xF9, 0x00, 0x44, 0x00, 0x01, 0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ,0x00, 0x00, 0x00 ,0x00, 0x00,
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE6, 0, FE6.Length);
                NetStream.Flush();

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送数据包：", FE6, FE6.Length);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE6, FE6.Length);
                    }
                }
                



                byte[] FD2 = new byte[m_MaxPacket];

                if (this.EtermType == "胜意PID")
                {
                    rcount = NetStream.Read(FD2, 0, m_MaxPacket);
                }

                rcount = NetStream.Read(FD2, 0, m_MaxPacket);

                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                if (rcount != 0)
                {
                    int tmpRecCount = rcount;

                    byte[] RecBuf2 = new byte[m_MaxPacket];

                    //数据包中长度
                    int RecCount2 = Convert.ToInt32(FD2[2]) * 16 * 16 + Convert.ToInt32(FD2[3]);

                    //未接收完毕，继续接收数据包
                    while ((tmpRecCount != 0) && (rcount < RecCount2))
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                        }
                        else
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                        }

                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                        if (tmpRecCount != 0)
                        {
                            //合并数据包
                            Array.Copy(RecBuf2, 0, FD2, rcount, tmpRecCount);
                        }

                        rcount += tmpRecCount;
                    }
                }

                if (DebugDataFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收数据包：", FD2, rcount);
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", FD2, rcount);
                    }
                }

                //Log.Record("server.log", "收到航信返回数据包：", FD, 0x06);

                //Log.Record("server.log", "登录成功...");
                //}

                //so（签出），并以当前工作号签入

                if (OriginalConfigManage.Original_ConfigSi.Trim() != "")
                {
                    //签入工作号
                    if (!SignIn())
                    {
                        if (DebugFlag)
                        {
                            if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 签入工作号失败，准备释放资源...");
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 签入工作号失败，准备释放资源...");
                            }
                        }

                        //签入工作号出现异常
                        ReleaseEterm();
                        //IdleFlag = -1;
                        return;
                    }
                }

                //登陆成功，则把错误计数清零
                ErrorCount = 0;

                //置为空闲
                IdleFlag = 0;

                if (DebugFlag)
                {
                    if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 空闲...");
                    }
                    else
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 空闲...");
                    }
                }
            }
            catch (Exception e)
            {
                if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + "认证过程出现异常，异常信息：" + e.Message);
                }
                else
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "认证过程出现异常，异常信息：" + e.Message);
                }

                //ReleaseEterm();
                IdleFlag = 4;
                //错误计数
                ErrorCount++;
            }
        }


        /// <summary>
        /// 签入工作号
        /// </summary>
        /// <returns></returns>
        public bool SignIn()
        {
            try
            {
                if (DebugFlag)
                {
                    Log.Record("server.log", "准备签出签入工作号...");
                }

                byte[] headBuf = { 0x01, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x01, 0x39, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E };
                byte[] endBuf = { 0x20, 0x03 };

                //替换黑屏包头 
                Array.Copy(m_cmdHeadBuf, 0, headBuf, 8, 2);

                //先签出工作号
                byte[] soBuf = Encoding.Default.GetBytes("so");
                //替换数据包长度字节
                short tmplen = (short)(soBuf.Length + headBuf.Length + endBuf.Length);
                short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] solenbuf = BitConverter.GetBytes(count2);

                Array.Copy(solenbuf, 0, headBuf, 2, 2);

                ArrayList soal = new ArrayList();
                soal.AddRange(headBuf);
                soal.AddRange(soBuf);
                soal.AddRange(endBuf);

                byte[] so_contentbuf = (byte[])(soal.ToArray(typeof(byte)));

                //组织工作号密码信息
                byte[] workBuf = null;

                if ((OriginalConfigManage.Original_ConfigSi.Trim().Length > 3) && (OriginalConfigManage.Original_ConfigSi.Trim().ToUpper().Substring(0, 2) == "SI"))
                {
                    workBuf = Encoding.Default.GetBytes(OriginalConfigManage.Original_ConfigSi);
                }
                else
                {
                    workBuf = Encoding.Default.GetBytes("SI:" + OriginalConfigManage.Original_ConfigSi);
                }

                //替换数据包长度字节
                tmplen = (short)(workBuf.Length + headBuf.Length + endBuf.Length);
                count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] lenbuf = BitConverter.GetBytes(count2);

                Array.Copy(lenbuf, 0, headBuf, 2, 2);

                byte[] recbuf = new byte[m_MaxPacket];

                int rcount = 0;

                string tmpcontent = "";

                ArrayList al = new ArrayList();
                al.AddRange(headBuf);
                al.AddRange(workBuf);
                al.AddRange(endBuf);

                byte[] contentbuf = (byte[])(al.ToArray(typeof(byte)));

                if (DebugFlag)
                {
                    if (EtermType.IndexOf("地址认证") != -1)
                    {
                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 正在自动签入工作号...");
                    }
                    else
                    {
                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 正在自动签入工作号...");
                    }
                }

                if (NetStream != null)
                {
                    SendCmdTime = DateTime.Now;

                    lock (NetStream)
                    {
                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 正在签出原工作号...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 正在签出原工作号...");
                            }
                        }

                        //Kevin 2009-09-28 Edit
                        //如果缓冲区有内容，则继续读取
                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)                        
                        //{
                        //    rcount = NetStream.Read(recbuf, 0, m_MaxPacket);
                        //    if (DebugDataFlag)
                        //    {
                        //        if (EtermType == "地址认证")
                        //        {
                        //            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到返回数据包：", recbuf, rcount);
                        //        }
                        //        else
                        //        {
                        //            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到返回数据包：", recbuf, rcount);
                        //        }
                        //    }
                        //}

                        NetStream.Write(so_contentbuf, 0, so_contentbuf.Length);
                        NetStream.Flush();

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送数据包：", so_contentbuf, so_contentbuf.Length);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送数据包：", so_contentbuf, so_contentbuf.Length);
                            }
                        }

                        #region 接收返回直到结束
                        //如果缓冲区有内容，则继续读取
                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        rcount = NetStream.Read(recbuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (rcount != 0)
                        {
                            int tmpRecCount = rcount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(recbuf[2]) * 16 * 16 + Convert.ToInt32(recbuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (rcount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, recbuf, rcount, tmpRecCount);
                                }

                                rcount += tmpRecCount;
                            }
                        }
                        #endregion 接收返回直到结束

                        if (DebugDataFlag)
                        {
                            if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 3接收到返回数据包：", recbuf, rcount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 3接收到返回数据包：", recbuf, rcount);
                            }
                        }
                        //}

                        if (DebugFlag)
                        {
                            if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送签入指令...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送签入指令...");
                            }
                        }

                        //指令间隔时间
                        if (this.Interval > 0)
                        {
                            Thread.Sleep(this.Interval);
                        }

                        NetStream.Write(contentbuf, 0, contentbuf.Length);
                        NetStream.Flush();

                        if (DebugDataFlag)
                        {
                            if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送签入数据包：", contentbuf, contentbuf.Length);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送签入数据包：", contentbuf, contentbuf.Length);
                            }
                        }

                        #region 接收返回直到结束
                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        rcount = NetStream.Read(recbuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (rcount != 0)
                        {
                            int tmpRecCount = rcount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(recbuf[2]) * 16 * 16 + Convert.ToInt32(recbuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (rcount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, recbuf, rcount, tmpRecCount);
                                }

                                rcount += tmpRecCount;
                            }
                        }
                        #endregion 接收返回直到结束

                        if (DebugDataFlag)
                        {
                            if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到签入返回数据包：", recbuf, rcount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到签入返回数据包：", recbuf, rcount);
                            }
                        }

                        tmpcontent = AnalyseServerContent(recbuf, rcount);
                        if (DebugFlag)
                        {                            
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收到签入返回结果：" + tmpcontent);
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到签入返回结果：" + tmpcontent);
                            }
                        }

                        #region 接收结果信息错位处理
                        //接收到签入返回结果：bPLEASE SIGN IN FIRST. 
                        //接收信息错位，则继续接收
                        if ((tmpcontent.ToLower().IndexOf("please sign in first") != -1)||(tmpcontent.ToLower().IndexOf("lease wait - transaction in progress") != -1))
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收签入返回结果信息错位，继续接收...");
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收签入返回结果信息错位，继续接收...");
                            }

                            //继续接收
                            rcount = NetStream.Read(recbuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (rcount != 0)
                            {
                                int tmpRecCount = rcount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(recbuf[2]) * 16 * 16 + Convert.ToInt32(recbuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (rcount < RecCount2))
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                    }

                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, recbuf, rcount, tmpRecCount);
                                    }

                                    rcount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到签入返回数据包：", recbuf, rcount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到签入返回数据包：", recbuf, rcount);
                                }
                            }

                            tmpcontent = AnalyseServerContent(recbuf, rcount);
                            if (DebugFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收到签入返回结果：" + tmpcontent);
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到签入返回结果：" + tmpcontent);
                                }
                            }
                        }
                        #endregion 接收结果信息错位处理


                        //指令间隔时间
                        if (this.Interval > 0)
                        {
                            Thread.Sleep(this.Interval);
                        }
                    }
                }
                //设置发送指令时间
                SendCmdTime = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                if (EtermType.IndexOf("地址认证") != -1)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 签入工作号出现异常，异常信息：" + ex.Message);
                }
                else
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 签入工作号出现异常，异常信息：" + ex.Message);
                }

                //ReleaseEterm();
                IdleFlag = 4;
                //错误计数
                ErrorCount++;
                return false;
            }
        }


        /// <summary>
        /// 断开重连，定时发送心跳数据包
        /// </summary>
        public void EtermTimeProc()
        {
            //循环处理,检测并发送心跳包
            int tmpsecondtime = 0;

            while (true)
            {
                //检查是否退出程序
                if (CloseFlag)
                {
                    Log.Record("server.log", "程序退出");
                    break;
                }

                try
                {
                    //kevin 2011-05-26 屏蔽
                    ////如果达到最大错误数，则停用配置，退出循环
                    //if (errorcount >= _maxErrorCount)
                    //{
                    //    m_etermstatus.status = 3;
                    //    m_idle = false;
                    //    sslStream = null;
                    //    netStream = null;
                    //    Log.Record("error.log", "配置：" + m_username + " 错误次数达到最大次数，异常停用");                        
                    //    break;
                    //}

                    //如果还没有认证配置，则开始认证
                    if ((IdleFlag == -1) && (NetStream == null))
                    {
                        IdleFlag = -2;
                        //Log.Record("login.log", "配置");
                        DoWith350();
                        continue;
                    }

                    //如果配置状态异常，并且Stream不为空，则置空
                    if (((IdleFlag == 2) || (IdleFlag == 3) || (IdleFlag == 4)) && (NetStream != null))
                    {
                        if (NetStream != null)
                        {
                            if (DebugFlag)
                            {
                                if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 状态异常netStream不为空，重置为null");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 状态异常netStream不为空，重置为null");
                                }
                            }
                            NetStream.Close();
                        }
                        NetStream = null;
                    }

                    //检测Socket是否已经断开与服务器的连接
                    if ((NetStream == null) && ((IdleFlag!=-2)&&(IdleFlag != -1) && (IdleFlag != 2) && (IdleFlag != 3)))
                    {
                        Log.Record("server.log", "2");
                        IdleFlag = -2;
                        //重新连接认证服务器
                        DoWith350();
                        continue;
                    }

                    //if (DebugFlag)
                    //{
                    //    string tmpss = "";
                    //    if (NetStream == null)
                    //    {
                    //        tmpss = "null";
                    //    }
                    //    else
                    //    {
                    //        tmpss = " not null";
                    //    }
                    //    if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                    //    {
                    //        Log.Record("heart.log", OriginalConfigManage.Original_LocalIp + " 状态：IdleFlag=" + IdleFlag.ToString() + ",NetStream="+tmpss);
                    //    }
                    //    else
                    //    {
                    //        Log.Record("heart.log", OriginalConfigManage.Original_ConfigName + " 状态：IdleFlag=" + IdleFlag.ToString() + ",NetStream=" + tmpss);
                    //    }
                    //}
                    //心跳包
                    tmpsecondtime = PublicInfo.DateDiff(DateTime.Now, SendCmdTime);

                    int rcount = -1;

                    //空闲状态并且超过最大心跳包等待时间，则发送心跳包
                    if ((tmpsecondtime >= m_MaxHeartTimes) && (IdleFlag == 0))
                    {
                        byte[] buf = new byte[m_MaxPacket];
                        //发送心跳包
                        if (NetStream != null)
                        {
                            SendCmdTime = DateTime.Now;

                            lock (NetStream)
                            {
                                HeartStartTime = DateTime.Now;

                                NetStream.Write(m_HeartBuf, 0, 5);
                                NetStream.Flush();
                                SendCmdTime = DateTime.Now;

                                //输出调试信息
                                if (DebugFlag)
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 向航信发送心跳包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 向航信发送心跳包...");
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 向航信发送心跳包：", m_HeartBuf, 5);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 向航信发送心跳包：", m_HeartBuf, 5);
                                    }
                                }

                                //while (NetStream.DataAvailable)
                                //while(NetClient.Available>0)
                                //{
                                rcount = NetStream.Read(buf, 0, m_MaxPacket);

                                #region 读取数据包是否完整，如果未接收完毕则继续
                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (rcount != 0)
                                {
                                    int tmpRecCount = rcount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(buf[2]) * 16 * 16 + Convert.ToInt32(buf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (rcount < RecCount2))
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收心跳包返回数据包1...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收心跳包返回数据包1...");
                                        }

                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, buf, rcount, tmpRecCount);
                                        }

                                        rcount += tmpRecCount;
                                    }
                                }
                                #endregion 读取数据包是否完整，如果未接收完毕则继续

                                #region 如果接收到的长度大于5，则继续接收
                                if (rcount > 5)
                                {
                                    rcount = NetStream.Read(buf, 0, m_MaxPacket);

                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收心跳包返回数据包2...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收心跳包返回数据包2...");
                                    }
                                }
                                #endregion 如果接收到的长度大于5，则继续接收

                                HeartEndTime = DateTime.Now;

                                //输出调试信息
                                if (DebugFlag)
                                {
                                    if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到心跳包返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到心跳包返回数据包...");
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到心跳包返回数据包：", buf, rcount);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到心跳包返回数据包：", buf, rcount);
                                    }
                                }
                                // }
                            }
                        }
                    }
                    //5秒钟检测一次
                    Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    HeartEndTime = HeartStartTime;
                    SendCmdTime = DateTime.Now;
                    if (DebugFlag)
                    {
                        if (OriginalConfigManage.ProtocolType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "发送心跳包过程出错，错误信息：" + e.Message);
                        }
                        else
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + "发送心跳包过程出错，错误信息：" + e.Message);
                        }
                    }
                    //ReleaseEterm();
                    IdleFlag = 4;
                    //错误计数
                    ErrorCount++;
                }
            }
        }

        #region 直接转发指令并返回结果信息
        public bool SendCmdDirectAndGetResult(string username,byte[] CmdBuf, ref byte[] ResBuf,ref int RecCount)
        {
            ResBuf = null;

            try
            {
                //发送指令
                NetStream.Write(CmdBuf, 0, CmdBuf.Length);
                NetStream.Flush();

                //接收返回信息
                ResBuf = new byte[m_MaxPacket];
                RecCount = 0;
                RecCount = NetStream.Read(ResBuf, 0, m_MaxPacket);

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                //配置设置为空闲
                IdleFlag = 0;

                #region 屏蔽
                ////Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                //if (RecCount != 0)
                //{
                //    int tmpRecCount = RecCount;

                //    byte[] tmpbuf2 = new byte[m_MaxPacket];

                //    //数据包中长度
                //    int RecCount2 = Convert.ToInt32(tmpbuf[2]) * 16 * 16 + Convert.ToInt32(tmpbuf[3]);

                //    //未接收完毕，继续接收数据包
                //    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                //    {
                //        if (EtermType.IndexOf("地址认证") != -1)
                //        {
                //            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                //        }
                //        else
                //        {
                //            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                //        }

                //        tmpRecCount = NetStream.Read(tmpbuf2, 0, m_MaxPacket);

                //        if (tmpRecCount != 0)
                //        {
                //            //合并数据包
                //            Array.Copy(tmpbuf2, 0, tmpbuf, RecCount, tmpRecCount);
                //        }

                //        RecCount += tmpRecCount;
                //    }
                //}
                #endregion 屏蔽
            }
            catch (Exception ex)
            {
                IdleFlag = 4;
                return false;
            }

            return true;
        }
        #endregion 直接转发指令并返回结果信息

        #region 发送指令并获取返回结果信息
        /// <summary>
        /// 发送指令并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, ref byte[] ResBuf, ref string ErrorMessage)
        {
            ResBuf = null;
            ErrorMessage = "";

            //是否需要重新认证配置
            bool ResetFlag = false;

            try
            {
                //I指令数据包
                byte[] ibuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x69, 0x20, 0x03 };
                //封口指令数据包
                byte[] fengkouBuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x5C, 0x20, 0x03 };
                //PN指令数据包
                byte[] tmpPnBuf = { 0x1, 0x0, 0x0, 0x17, 0x0, 0x0, 0x0, 0x1, 0x41, 0x51, 0x70, 0x2, 0x1B, 0x0B, 0x22, 0x20, 0x0, 0xF, 0x1E, 0x70, 0x6E, 0x20, 0x3 };

                //回复信息的报头和报尾
                byte[] reshead = { 0x01, 0x00, 0x01, 0xAE, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x21, 0x20, 0x0F, 0x1B, 0x4D };
                byte[] resend = { 0x0D, 0x1E, 0x1B, 0x62, 0x03 };

                ArrayList al2 = new ArrayList();

                int rcount = -1;

                //行程单打印指令直接发送
                #region 行程单打印指令处理
                byte[] tmpprintbuf = new byte[5];
                if (CmdBuf.Length > 24)
                {
                    Array.Copy(CmdBuf, 19, tmpprintbuf, 0, 5);
                }
                if (((cmd.Length > 5) && (cmd.ToLower().Substring(0, 5) == "prinv"))|| //&& (Encoding.Default.GetString(tmpprintbuf).ToLower() == "prinv")) ||
                    ((cmd.Length > 5) && (cmd.ToLower().Substring(0, 5) == "vtinv")))// && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "vtinv")))
                {
                    byte[] tmppBuf1 = { 0x01, 0x00, 0x00, 0x32, 0x0C, 0x00, 0x00, 0x01, 0x8C, 0x0C, 0x00, 0x02 };
                    byte[] tmppBuf2 = { 0x03 };

                    lock (NetStream)
                    {
                        //组织返回信息
                        byte[] buf3 = Encoding.Default.GetBytes(cmd.Trim().ToUpper().Replace("PRINV:", "PRINV ").Replace("VTINV:", "VTINV "));

                        //替换数据包长度信息
                        short tmplen = (short)(buf3.Length + tmppBuf1.Length + tmppBuf2.Length);
                        short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                        byte[] lenbuf = BitConverter.GetBytes(count2);
                        Array.Copy(lenbuf, 0, tmppBuf1, 2, 2);
                        al2.AddRange(tmppBuf1);
                        al2.AddRange(buf3);
                        al2.AddRange(tmppBuf2);

                        byte[] tmpCmdBuf = (byte[])(al2.ToArray(typeof(byte)));

                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        //发送指令
                        NetStream.Write(tmpCmdBuf, 0, tmpCmdBuf.Length);
                        NetStream.Flush();

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送行程单数据包：", tmpCmdBuf, tmpCmdBuf.Length);
                                Log.Record("server.log", "行程单指令字节包组成：指令长度" + buf3.Length.ToString() + "报头" + tmppBuf1.Length.ToString() + "报尾" + tmppBuf2.Length.ToString());
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送行程单数据包：", tmpCmdBuf, tmpCmdBuf.Length);
                                Log.Record("server.log", "行程单指令字节包组成：指令长度" + buf3.Length.ToString() + "报头" + tmppBuf1.Length.ToString() + "报尾" + tmppBuf2.Length.ToString());
                            }
                        }

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        //接收返回信息
                        byte[] tmpbuf = new byte[m_MaxPacket];
                        int RecCount = 0;
                        RecCount = NetStream.Read(tmpbuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] tmpbuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(tmpbuf[2]) * 16 * 16 + Convert.ToInt32(tmpbuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(tmpbuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(tmpbuf2, 0, tmpbuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收返回数据包：", tmpbuf, RecCount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收返回数据包：", tmpbuf, RecCount);
                            }
                        }

                        //返回成功
                        ResBuf = new byte[RecCount];
                        Array.Copy(tmpbuf, ResBuf, RecCount);

                        #region 检查连接是否断开
                        if (RecCount == 0)
                        {
                            IdleFlag = 4;

                            ErrorCount++;

                            Log.Record("server.log", "发送指令：" + cmd + "，接收返回信息失败，可能通讯断开...");

                            ErrorMessage = "发送指令：" + cmd + "，接收返回信息失败，可能通讯断开...";
                            return false;
                        }
                        #endregion 检查连接是否断开
                        // break;
                        // }
                    }
                }
                #endregion 行程单打印指令处理
                #region 其他指令处理
                else
                {
                    int RecCount = -1;
                    byte[] RecBuf = new byte[m_MaxPacket];

                    //int m_WhileCount = 0;

                    //把指令的黑屏报头信息替换
                    Array.Copy(m_cmdHeadBuf, 0, CmdBuf, 8, 2);

                    //替换i指令报头
                    Array.Copy(m_cmdHeadBuf, 0, ibuf, 8, 2);

                    //替换封口指令的报头
                    Array.Copy(m_cmdHeadBuf, 0, fengkouBuf, 8, 2);

                    //替换PN指令报头
                    Array.Copy(m_cmdHeadBuf, 0, tmpPnBuf, 8, 2);


                    //判断最后一个字符是否为封口符号
                    string tmpcmd = cmd.Trim();

                    //指令最后封口标志
                    //bool fengkouFlag = false;
                    //if ((tmpcmd[tmpcmd.Length - 1] == '@') || (tmpcmd[tmpcmd.Length - 1] == '\\'))
                    //{
                    //    fengkouFlag = true;
                    //}

                    //从数据包中获取发送指令
                    byte[] SendCmdBuf = new byte[CmdBuf.Length - 21];
                    Array.Copy(CmdBuf, 19, SendCmdBuf, 0, SendCmdBuf.Length);
                    string sendcmd = Encoding.Default.GetString(SendCmdBuf);

                    if (DebugFlag)
                    {
                        if (EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 准备发送指令：" + sendcmd);
                        }
                        else
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + sendcmd);
                        }
                    }

                    #region 检查是否有未接收的接收返回数据
                    //lock (NetStream)
                    //{
                    //    //while (NetStream.DataAvailable)
                    //    while(NetClient.Available>0)
                    //    {
                    //        rcount = NetStream.Read(RecBuf, 0, m_MaxPacket);
                    //        if (DebugFlag)
                    //        {
                    //            if (EtermType == "地址认证")
                    //            {
                    //                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 存在未接收到返回数据包...");
                    //            }
                    //            else
                    //            {
                    //                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 存在未接收到返回数据包...");
                    //            }
                    //        }

                    //        if (DebugDataFlag)
                    //        {
                    //            if (EtermType == "地址认证")
                    //            {
                    //                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, rcount);
                    //            }
                    //            else
                    //            {
                    //                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, rcount);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion 检查是否有未接收的接收返回数据

                    #region 发送I还原指令
                    lock (NetStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        NetStream.Write(ibuf, 0, ibuf.Length);
                        NetStream.Flush();

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送I指令...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送I指令...");
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送还原指令数据包：", ibuf, ibuf.Length);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送还原指令数据包：", ibuf, ibuf.Length);
                            }
                        }

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收I指令后的返回结果数据包...");
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收I指令后的返回结果数据包...");
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }
                        }

                        #region 判断I指令的返回值，避免指令结果错乱
                        string tmpIResult = "";
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount > 23)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount > 21)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        //如果返回结果不是空，并且包含NO PNR ， 则继续接收一次
                        if ((tmpIResult.Trim() != "") && (tmpIResult.ToLower().IndexOf("no pnr") == -1) 
                            && (tmpIResult.ToUpper().IndexOf("TRANSACTION IN PROGRESS") == -1)
                            && (tmpIResult.Trim().ToLower() != "s") && (tmpIResult.Trim().ToLower() != "s"))
                        {
                            if (DebugFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 继续接收I指令后的返回结果数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 继续接收I指令后的返回结果数据包...");
                                }
                            }

                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续到接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续到接收数据包：", RecBuf, RecCount);
                                }
                            }
                        }
                        #endregion 判断I指令的返回值，避免指令结果错乱

                        //}

                        #region 屏蔽更改为上面的While处理
                        //m_WhileCount = 0;
                        //while (RecCount < 23)
                        //{
                        //    if ((RecCount == 0) || (m_WhileCount >= m_MaxWhileCount))
                        //    {
                        //        if (EtermType == "地址认证")
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：I" +
                        //                " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }
                        //        else
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：I" +
                        //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }

                        //        if (NetStream != null)
                        //        {
                        //            NetStream.Close();
                        //        }
                        //        NetStream = null;
                        //        IdleFlag = 4;
                        //        ErrorMessage = "发送I指令后接收返回信息异常";
                        //        return false;
                        //    }

                        //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //    m_WhileCount++;
                        //}
                        #endregion 屏蔽更改为上面的While处理
                    }

                    #endregion 发送I还原指令

                    //指令间隔时间
                    if (this.Interval > 0)
                    {
                        Thread.Sleep(this.Interval);
                    }

                    //分解指令"|"，并
                    string[] sl = sendcmd.Split('|');

                    //返回结果内容
                    string tmpcontent = "";

                    #region 循环发送指令并获取返回结果
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (sl[i].Trim() == "")
                            continue;

                        tmpcmd = sl[i];
                        short tmplen = -1;
                        short count2 = -1;
                        byte[] lenbuf = null;
                        ArrayList al = new ArrayList();

                        //指令报头
                        byte[] head = new byte[19];
                        //指令报尾
                        byte[] end = new byte[2];
                        //指令内容
                        byte[] contentbuf = null;
                        //取得待发送的指令数据包

                        //网站用户转换汉字编码及拼音
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            AnalyseWebCmdAndMakeServerInfo(tmpcmd, out contentbuf, true);
                        }
                        else
                        {
                            contentbuf = Encoding.Default.GetBytes(tmpcmd);
                        }

                        //指令报头
                        Array.Copy(CmdBuf, 0, head, 0, 19);
                        //指令报尾
                        Array.Copy(CmdBuf, CmdBuf.Length - 2, end, 0, 2);
                        //替换指令报头标志
                        Array.Copy(m_cmdHeadBuf, 0, head, 8, 2);

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 准备发送指令：" + tmpcmd);
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + tmpcmd);
                            }
                        }

                        //替换数据包长度字节
                        tmplen = (short)(head.Length + end.Length + contentbuf.Length);
                        count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                        lenbuf = BitConverter.GetBytes(count2);
                        Array.Copy(lenbuf, 0, head, 2, 2);
                        al.Clear();
                        al.AddRange(head);
                        al.AddRange(contentbuf);
                        al.AddRange(end);

                        byte[] tmpcmdbuf = (byte[])(al.ToArray(typeof(byte)));

                        lock (NetStream)
                        {
                            #region 屏蔽
                            //如果缓冲区有内容，则继续读取
                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);
                            //    if (DebugFlag)
                            //    {
                            //        if (this.EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收缓存区未接收的返回数据包...");
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收缓存区未接收的返回数据包...");
                            //        }
                            //    }

                            //    if (DebugDataFlag)
                            //    {
                            //        if (EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            //        }
                            //    }
                            //}
                            #endregion 屏蔽

                            #region 发送指令前先检查配置状态
                            if (this.IdleFlag != 1)
                            {
                                //不是忙碌状态则退出

                                ErrorCount++;

                                Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                ErrorMessage = "未找到可用配置...";
                                return false;
                            }
                            #endregion 发送指令前先检查配置状态

                            //发送指令
                            NetStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                            NetStream.Flush();

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送数据包：", tmpcmdbuf, tmpcmdbuf.Length);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送数据包：", tmpcmdbuf, tmpcmdbuf.Length);
                                }
                            }

                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //接收返回结果
                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);


                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                    }

                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                            }

                            while (RecCount < 21)
                            {
                                #region 检查连接是否断开
                                if (RecCount == 0)
                                {
                                    IdleFlag = 4;

                                    ErrorCount++;

                                    Log.Record("server.log", "发送指令：" + tmpcmd + "，接收返回信息失败，可能通讯断开...");

                                    ErrorMessage = "发送指令：" + tmpcmd + "，接收返回信息失败，可能通讯断开...";
                                    return false;
                                }
                                #endregion 检查连接是否断开

                                if (DebugFlag)
                                {
                                    if (this.EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                    }
                                }

                                RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                        }

                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                    }
                                }

                                continue;
                            }

                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            {
                                if (RecCount > 23)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                }
                            }
                            else
                            {
                                if (RecCount > 21)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                }
                            }

                            if (DebugFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回信息：" + tmpcontent);
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回信息：" + tmpcontent);
                                }
                            }

                            //}

                            #region 屏蔽，更改为上面的while处理
                            //int m_whilecount = 0;
                            //while (RecCount < 23)
                            //{
                            //    if ((RecCount == 0) || (m_whilecount >= m_MaxWhileCount))
                            //    {
                            //        if (EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", "配置:"+OriginalConfigManage.Original_LocalIp+" 发送指令："+tmpcmd+
                            //                " 出错；收到返回长度："+RecCount.ToString()+" 接收次数："+m_whilecount.ToString()+"；最大接收次数限制："+m_MaxWhileCount.ToString());
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_whilecount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                            //        }

                            //        if (NetStream != null)
                            //        {
                            //            NetStream.Close();
                            //        }
                            //        NetStream = null;
                            //        IdleFlag = 4;
                            //        ErrorMessage = "发送指令："+tmpcmd+" 获取航信返回信息出错！";
                            //        return false;
                            //    }

                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    m_whilecount++;
                            //}    
                            #endregion 屏蔽，更改为上面的while处理


                            #region 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端
                            ////Kevin 2010-12-07 Add
                            ////如果指令返回结果为S，则把配置设置为未连接
                            //if (tmpcontent.ToLower().Trim() == "s")
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：" + tmpcmd +
                            //            " 出错；收到返回s；返回结果异常...");
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //           " 出错；收到返回s；返回结果异常...");
                            //    }


                            //    if (NetStream != null)
                            //    {
                            //        NetStream.Close();
                            //    }
                            //    NetStream = null;
                            //    IdleFlag = 4;
                            //    ErrorMessage = "航信返回结果异常";
                            //    return false;
                            //}
                            #endregion 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端


                            #region 屏蔽，尽量与原始返回一致，不做特殊处理
                            ////Kevin 2010-06-15 Add
                            ////如果收到的返回结果是空，则继续接收返回信息
                            ////Kevin 2010-06-15 Add
                            ////如果SS:收到 NO PNR ，则继续接收返回信息
                            //while (((tmpcmd.Trim() != "\\") && (tmpcmd.ToUpper().IndexOf("SFC:") == -1) && (tmpcontent.Trim() == "")) || ((tmpcmd.ToUpper().IndexOf("SS:") != -1) && (tmpcontent.ToUpper().IndexOf("NO PNR") != -1)))
                            //{
                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //    }
                            //}
                            #endregion 屏蔽，尽量与原始返回一致，不做特殊处理

                            //指令间隔时间
                            if (this.Interval > 0)
                            {
                                Thread.Sleep(this.Interval);
                            }

                            #region 判断是否需要签入工作号，如果需要则自动签入并重发指令
                            //判断是否需要签入
                            if (IfNeedSignIn(tmpcontent))
                            {
                                //重置接收结果数据
                                tmpcontent = "";
                                if (DebugFlag)
                                {
                                    if (this.EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                }

                                #region 发送指令前先检查配置状态
                                if (this.IdleFlag != 1)
                                {
                                    //不是忙碌状态则退出

                                    ErrorCount++;

                                    Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                    ErrorMessage = "未找到可用配置...";
                                    return false;
                                }
                                #endregion 发送指令前先检查配置状态

                                //重发指令
                                NetStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                                NetStream.Flush();


                                //while (NetStream.DataAvailable)
                                //while(NetClient.Available>0)
                                //{
                                //接收返回结果
                                RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                        }

                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                    }
                                }

                                while (RecCount < 21)
                                {
                                    #region 检查连接是否断开
                                    if (RecCount == 0)
                                    {
                                        IdleFlag = 4;

                                        ErrorCount++;

                                        Log.Record("server.log", "发送指令：" + tmpcmd + "，接收返回信息失败，可能通讯断开...");

                                        ErrorMessage = "发送指令：" + tmpcmd + "，接收返回信息失败，可能通讯断开...";
                                        return false;
                                    }
                                    #endregion 检查连接是否断开

                                    if (DebugFlag)
                                    {
                                        if (this.EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                    }


                                    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                    if (RecCount != 0)
                                    {
                                        int tmpRecCount = RecCount;

                                        byte[] RecBuf2 = new byte[m_MaxPacket];

                                        //数据包中长度
                                        int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                        //未接收完毕，继续接收数据包
                                        while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                        {
                                            if (EtermType.IndexOf("地址认证") != -1)
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                            }
                                            else
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                            }

                                            tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                            if (tmpRecCount != 0)
                                            {
                                                //合并数据包
                                                Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                            }

                                            RecCount += tmpRecCount;
                                        }
                                    }

                                    if (DebugDataFlag)
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                        }
                                    }

                                    continue;
                                }

                                if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                {
                                    if (RecCount > 23)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                    }
                                }
                                else
                                {
                                    if (RecCount > 21)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                    }
                                }
                                //}

                                //指令间隔时间
                                if (this.Interval > 0)
                                {
                                    Thread.Sleep(this.Interval);
                                }
                            }
                            #endregion 判断是否需要签入工作号，如果需要则自动签入并重发指令

                            //if (DebugFlag)
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置："+OriginalConfigManage.Original_LocalIp+"收到航信返回信息：" + tmpcontent);
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //}
                        }

                        //Kevin 如果收到的结果为S，则重新认证配置
                        //if (tmpcontent.Trim().ToUpper() == "S")
                        //{
                        //    ResetFlag = true;
                        //}


                        //最后一个指令结果
                        if (i == sl.Length - 1)
                        {
                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            //替换数据包长度信息
                            tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));

                            break;
                        }
                    }
                    #endregion 循环发送指令并获取返回结果

                    #region 发送封口指令信息
                    //系统自动封口
                    //封口返回结果为空或者NO PNR，则返回封口之前的结果信息
                    //否则返回封口后的结果信息
                    lock (NetStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        //网站用户发送I指令
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            //发送I指令
                            NetStream.Write(ibuf, 0, ibuf.Length);
                            NetStream.Flush();
                        }
                        //客户端用户发送封口指令
                        else
                        {
                            //发送封口指令
                            NetStream.Write(fengkouBuf, 0, fengkouBuf.Length);
                            NetStream.Flush();
                        }

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送封口指令...");
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送封口指令...");
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送封口数据包：", fengkouBuf, fengkouBuf.Length);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送封口数据包：", fengkouBuf, fengkouBuf.Length);
                            }
                        }

                        //把返回结果字符串置空
                        string tmpcontent2 = "";

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        //接收返回数据包
                        RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }
                        }

                        while (RecCount < 21)
                        {
                            #region 检查连接是否断开
                            if (RecCount == 0)
                            {
                                IdleFlag = 4;

                                ErrorCount++;

                                Log.Record("server.log", "发送封口指令接收返回信息失败，可能通讯断开...");

                                ErrorMessage = "发送封口指令接收返回信息失败，可能通讯断开...";
                                return false;
                            }
                            #endregion 检查连接是否断开

                            if (DebugFlag)
                            {
                                if (this.EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                }
                            }
                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                    }

                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                            }
                            continue;
                        }

                        //Kevin 2011-04-28 Add
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount >= 23)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount >= 21)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到返回结果：" + tmpcontent2);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到返回结果：" + tmpcontent2);
                            }
                        }
                        //}

                        //指令间隔时间
                        if (this.Interval > 0)
                        {
                            Thread.Sleep(this.Interval);
                        }

                        //Kevin 如果收到的结果为S，则重新认证配置
                        //if (tmpcontent2.Trim().ToUpper() == "S")
                        //{
                        //    ResetFlag = true;
                        //}

                        //如果返回结果不包含NO PNR 并且 返回结果不为空，则返回此结果信息
                        if ((tmpcontent2.ToUpper().IndexOf("NO PNR") == -1) && (tmpcontent2.Trim() != "")
                            && (tmpcontent2.ToUpper().IndexOf("CHECK") == -1)&&(sendcmd.ToUpper().IndexOf("ETDZ ")==-1))
                        {
                            //如果需要返回所有指令结果，则把结果累加
                            if (allFlag)
                            {
                                tmpcontent += tmpcontent2;
                            }
                            //如果只返回最后一个指令的结果，则把当前封口指令的结果返回
                            else
                            {
                                tmpcontent = tmpcontent2;
                            }

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            //替换数据包长度信息
                            short tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            byte[] lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.Clear();
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));
                        }
                    }
                    #endregion 发送封口指令信息
                }
                #endregion 其他指令处理

                //指令数量统计，用于配置均衡
                this.CommandCount++;

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                if (ResetFlag)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "接收到异常返回结果，需要重新进行配置认证...");
                    }
                    //需要重新进行配置认证，设置状态为异常
                    IdleFlag = 4;
                }
                else
                {
                    //配置设置为空闲
                    IdleFlag = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "发送指令：" + cmd + "，处理出错，错误信息：" + ex.Message);
                IdleFlag = 4;
                ErrorMessage = ex.Message;
                return false;
            }
        }
        #endregion 发送指令并获取返回结果信息

        #region 发送指令自动PN并获取返回结果信息
        /// <summary>
        /// 发送指令自动根据最后一个指令进行PN并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetAllPnResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, ref byte[] ResBuf, ref string ErrorMessage)
        {
            ResBuf = null;
            ErrorMessage = "";

            //是否需要重新进行配置认证标志
            bool ResetFlag = false;

            try
            {
                //I指令数据包
                byte[] ibuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x69, 0x20, 0x03 };
                //封口指令数据包
                byte[] fengkouBuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x5C, 0x20, 0x03 };
                //PN指令数据包
                byte[] tmpPnBuf = { 0x1, 0x0, 0x0, 0x17, 0x0, 0x0, 0x0, 0x1, 0x41, 0x51, 0x70, 0x2, 0x1B, 0x0B, 0x22, 0x20, 0x0, 0xF, 0x1E, 0x70, 0x6E, 0x20, 0x3 };

                //回复信息的报头和报尾
                byte[] reshead = { 0x01, 0x00, 0x01, 0xAE, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x21, 0x20, 0x0F, 0x1B, 0x4D };
                byte[] resend = { 0x0D, 0x1E, 0x1B, 0x62, 0x03 };

                ArrayList al2 = new ArrayList();

                //行程单打印指令直接发送
                #region 行程单打印指令处理
                //byte[] tmpprintbuf = new byte[5];
                //Array.Copy(CmdBuf, 19, tmpprintbuf, 0, 5);
                //if (((cmd.ToLower() == "prinv") && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "prinv")) ||
                //    ((cmd.ToLower() == "vtinv") && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "vtinv")))
                //{
                //    lock (NetStream)
                //    {
                //        //发送指令
                //        NetStream.Write(CmdBuf, 0, CmdBuf.Length);
                //        NetStream.Flush();

                //        if (DebugFlag)
                //        {
                //            if (EtermType == "地址认证")
                //            {
                //                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送行程单指令...");
                //            }
                //            else
                //            {
                //                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送行程单指令...");
                //            }
                //        }

                //        if (DebugDataFlag)
                //        {
                //            if (EtermType == "地址认证")
                //            {
                //                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送行程单数据包：", CmdBuf, CmdBuf.Length);
                //            }
                //            else
                //            {
                //                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送行程单数据包：", CmdBuf, CmdBuf.Length);
                //            }
                //        }

                //        //while (NetStream.DataAvailable)
                //        //while(NetClient.Available>0)
                //        //{
                //        //接收返回信息
                //        byte[] tmpbuf = new byte[m_MaxPacket];
                //        int RecCount = 0;
                //        RecCount = NetStream.Read(tmpbuf, 0, m_MaxPacket);

                //        if (DebugDataFlag)
                //        {
                //            if (EtermType == "地址认证")
                //            {
                //                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", tmpbuf, RecCount);
                //            }
                //            else
                //            {
                //                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", tmpbuf, RecCount);
                //            }
                //        }

                //        //返回成功
                //        ResBuf = new byte[RecCount];
                //        Array.Copy(tmpbuf, ResBuf, RecCount);
                //        //    break;
                //        //}
                //    }
                //}
                #endregion 行程单打印指令处理
                #region 其他指令处理
                //else
                {
                    int RecCount = -1;
                    byte[] RecBuf = new byte[m_MaxPacket];

                    //int m_WhileCount = 0;

                    //把指令的黑屏报头信息替换
                    Array.Copy(m_cmdHeadBuf, 0, CmdBuf, 8, 2);

                    //替换i指令报头
                    Array.Copy(m_cmdHeadBuf, 0, ibuf, 8, 2);

                    //替换封口指令的报头
                    Array.Copy(m_cmdHeadBuf, 0, fengkouBuf, 8, 2);

                    //替换PN指令报头
                    Array.Copy(m_cmdHeadBuf, 0, tmpPnBuf, 8, 2);


                    //判断最后一个字符是否为封口符号
                    string tmpcmd = cmd.Trim();

                    //指令最后封口标志
                    //bool fengkouFlag = false;
                    //if ((tmpcmd[tmpcmd.Length - 1] == '@') || (tmpcmd[tmpcmd.Length - 1] == '\\'))
                    //{
                    //    fengkouFlag = true;
                    //}

                    //从数据包中获取发送指令
                    byte[] SendCmdBuf = new byte[CmdBuf.Length - 21];
                    Array.Copy(CmdBuf, 19, SendCmdBuf, 0, SendCmdBuf.Length);
                    string sendcmd = Encoding.Default.GetString(SendCmdBuf);

                    if (DebugFlag)
                    {
                        if (this.EtermType.IndexOf("地址认证") != -1)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 准备发送指令：" + sendcmd);
                        }
                        else
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + sendcmd);
                        }
                    }

                    #region 检查是否有未接收的接收返回数据
                    //lock (NetStream)
                    //{
                    //    //while (NetStream.DataAvailable)
                    //    while(NetClient.Available>0)
                    //    {
                    //        RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);
                    //        if (DebugFlag)
                    //        {
                    //            if (EtermType == "地址认证")
                    //            {
                    //                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 存在未接收到返回数据包...");
                    //            }
                    //            else
                    //            {
                    //                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 存在未接收到返回数据包...");
                    //            }
                    //        }

                    //        if (DebugDataFlag)
                    //        {
                    //            if (EtermType == "地址认证")
                    //            {
                    //                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                    //            }
                    //            else
                    //            {
                    //                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion 检查是否有未接收的接收返回数据

                    #region 发送I还原指令
                    lock (NetStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        NetStream.Write(ibuf, 0, ibuf.Length);
                        NetStream.Flush();

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送I指令...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送I指令...");
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送I指令数据包：", ibuf, ibuf.Length);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送I指令数据包：", ibuf, ibuf.Length);
                            }
                        }

                        // while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收I指令后的返回结果数据包...");
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收I指令后的返回结果数据包...");
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }
                        }

                        #region 判断I指令的返回值，避免指令结果错乱
                        string tmpIResult = "";
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount > 23)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount > 21)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        //如果返回结果不是空，并且包含NO PNR ， 则继续接收一次
                        if ((tmpIResult.Trim() != "") && (tmpIResult.ToLower().IndexOf("no pnr") == -1)
                            && (tmpIResult.ToUpper().IndexOf("TRANSACTION IN PROGRESS") == -1)
                            && (tmpIResult.Trim().ToLower() != "s") && (tmpIResult.Trim().ToLower() != "s"))
                        {
                            if (DebugFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 继续接收I指令后的返回结果数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 继续接收I指令后的返回结果数据包...");
                                }
                            }

                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);


                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续到接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续到接收数据包：", RecBuf, RecCount);
                                }
                            }
                        }
                        #endregion 判断I指令的返回值，避免指令结果错乱

                        //}

                        #region 屏蔽更改为上面的While处理
                        //m_WhileCount = 0;
                        //while (RecCount < 23)
                        //{
                        //    if ((RecCount == 0) || (m_WhileCount >= m_MaxWhileCount))
                        //    {
                        //        if (EtermType == "地址认证")
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：I" +
                        //                " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }
                        //        else
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：I" +
                        //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }

                        //        if (NetStream != null)
                        //        {
                        //            NetStream.Close();
                        //        }
                        //        NetStream = null;
                        //        IdleFlag = 4;
                        //        ErrorMessage = "发送I指令后接收返回信息异常";
                        //        return false;
                        //    }

                        //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //    m_WhileCount++;
                        //}
                        #endregion 屏蔽更改为上面的While处理
                    }

                    #endregion 发送I还原指令

                    //指令间隔时间
                    if (this.Interval > 0)
                    {
                        Thread.Sleep(this.Interval);
                    }

                    //分解指令"|"，并
                    string[] sl = sendcmd.Split('|');

                    //返回结果内容
                    string tmpcontent = "";

                    //最后一个指令的返回结果，用于判断是否翻页结束
                    string tmplastcontent = "";

                    #region 循环发送指令并获取返回结果
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (sl[i].Trim() == "")
                            continue;

                        tmpcmd = sl[i];
                        short tmplen = -1;
                        short count2 = -1;
                        byte[] lenbuf = null;
                        ArrayList al = new ArrayList();

                        //指令报头
                        byte[] head = new byte[19];
                        //指令报尾
                        byte[] end = new byte[2];
                        //指令内容
                        byte[] contentbuf = null;
                        //取得待发送的指令数据包

                        //网站用户转换汉字编码及拼音
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            AnalyseWebCmdAndMakeServerInfo(tmpcmd, out contentbuf, true);
                        }
                        else
                        {
                            contentbuf = Encoding.Default.GetBytes(tmpcmd);
                        }

                        //contentbuf = Encoding.Default.GetBytes(tmpcmd);

                        //指令报头
                        Array.Copy(CmdBuf, 0, head, 0, 19);
                        //指令报尾
                        Array.Copy(CmdBuf, CmdBuf.Length - 2, end, 0, 2);
                        //替换指令报头标志
                        Array.Copy(m_cmdHeadBuf, 0, head, 8, 2);

                        if (DebugFlag)
                        {
                            if (this.EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 准备发送指令：" + tmpcmd);
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + tmpcmd);
                            }
                        }

                        //替换数据包长度字节
                        tmplen = (short)(head.Length + end.Length + contentbuf.Length);
                        count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                        lenbuf = BitConverter.GetBytes(count2);
                        Array.Copy(lenbuf, 0, head, 2, 2);
                        al.Clear();
                        al.AddRange(head);
                        al.AddRange(contentbuf);
                        al.AddRange(end);

                        byte[] tmpcmdbuf = (byte[])(al.ToArray(typeof(byte)));

                        lock (NetStream)
                        {
                            //如果缓冲区有内容，则继续读取
                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);
                            //    if (DebugFlag)
                            //    {
                            //        if (this.EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收缓存区未接收的返回数据包...");
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收缓存区未接收的返回数据包...");
                            //        }
                            //    }

                            //    if (DebugDataFlag)
                            //    {
                            //        if (EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            //        }
                            //    }
                            //}

                            #region 发送指令前先检查配置状态
                            if (this.IdleFlag != 1)
                            {
                                //不是忙碌状态则退出

                                ErrorCount++;

                                Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                ErrorMessage = "未找到可用配置...";
                                return false;
                            }
                            #endregion 发送指令前先检查配置状态

                            //发送指令
                            NetStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                            NetStream.Flush();

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送指令数据包：", tmpcmdbuf, tmpcmdbuf.Length);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送指令数据包：", tmpcmdbuf, tmpcmdbuf.Length);
                                }
                            }

                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //接收返回结果
                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                    }

                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                            }

                            while (RecCount < 21)
                            {
                                #region 检查连接是否断开
                                if (RecCount == 0)
                                {
                                    IdleFlag = 4;

                                    ErrorCount++;

                                    Log.Record("server.log", "发送指令：" + tmpcmd + "，接收返回信息失败，可能通讯断开...");

                                    ErrorMessage = "发送指令：" + tmpcmd + "，接收返回信息失败，可能通讯断开...";
                                    return false;
                                }
                                #endregion 检查连接是否断开

                                if (DebugFlag)
                                {
                                    if (this.EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                    }
                                }

                                RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                        }

                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                    }
                                }

                                continue;
                            }

                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            {
                                if (RecCount > 23)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                    tmplastcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                }
                            }
                            else
                            {
                                if (RecCount > 21)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                    tmplastcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                }
                            }

                            //接收到异常返回，设置重新认证配置标志
                            //if (tmplastcontent.Trim().ToUpper() == "S")
                            //{
                            //    ResetFlag = true;
                            //}

                            if (DebugFlag)
                            {
                                if (this.EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回结果：" + tmpcontent);
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果：" + tmpcontent);
                                }
                            }
                            //}

                            //指令间隔时间
                            if (this.Interval > 0)
                            {
                                Thread.Sleep(this.Interval);
                            }

                            #region 屏蔽，更改为上面的while处理
                            //int m_whilecount = 0;
                            //while (RecCount < 23)
                            //{
                            //    if ((RecCount == 0) || (m_whilecount >= m_MaxWhileCount))
                            //    {
                            //        if (EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", "配置:"+OriginalConfigManage.Original_LocalIp+" 发送指令："+tmpcmd+
                            //                " 出错；收到返回长度："+RecCount.ToString()+" 接收次数："+m_whilecount.ToString()+"；最大接收次数限制："+m_MaxWhileCount.ToString());
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_whilecount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                            //        }

                            //        if (NetStream != null)
                            //        {
                            //            NetStream.Close();
                            //        }
                            //        NetStream = null;
                            //        IdleFlag = 4;
                            //        ErrorMessage = "发送指令："+tmpcmd+" 获取航信返回信息出错！";
                            //        return false;
                            //    }

                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    m_whilecount++;
                            //}    
                            #endregion 屏蔽，更改为上面的while处理


                            #region 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端
                            ////Kevin 2010-12-07 Add
                            ////如果指令返回结果为S，则把配置设置为未连接
                            //if (tmpcontent.ToLower().Trim() == "s")
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：" + tmpcmd +
                            //            " 出错；收到返回s；返回结果异常...");
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //           " 出错；收到返回s；返回结果异常...");
                            //    }


                            //    if (NetStream != null)
                            //    {
                            //        NetStream.Close();
                            //    }
                            //    NetStream = null;
                            //    IdleFlag = 4;
                            //    ErrorMessage = "航信返回结果异常";
                            //    return false;
                            //}
                            #endregion 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端


                            #region 屏蔽，尽量与原始返回一致，不做特殊处理
                            ////Kevin 2010-06-15 Add
                            ////如果收到的返回结果是空，则继续接收返回信息
                            ////Kevin 2010-06-15 Add
                            ////如果SS:收到 NO PNR ，则继续接收返回信息
                            //while (((tmpcmd.Trim() != "\\") && (tmpcmd.ToUpper().IndexOf("SFC:") == -1) && (tmpcontent.Trim() == "")) || ((tmpcmd.ToUpper().IndexOf("SS:") != -1) && (tmpcontent.ToUpper().IndexOf("NO PNR") != -1)))
                            //{
                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //    }
                            //}
                            #endregion 屏蔽，尽量与原始返回一致，不做特殊处理


                            #region 判断是否需要签入工作号，如果需要则自动签入并重发指令
                            //判断是否需要签入
                            if (IfNeedSignIn(tmpcontent))
                            {
                                //重置接收结果数据
                                tmpcontent = "";
                                if (DebugFlag)
                                {
                                    if (this.EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                }

                                #region 发送指令前先检查配置状态
                                if (this.IdleFlag != 1)
                                {
                                    //不是忙碌状态则退出

                                    ErrorCount++;

                                    Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                    ErrorMessage = "未找到可用配置...";
                                    return false;
                                }
                                #endregion 发送指令前先检查配置状态

                                //重发指令
                                NetStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                                NetStream.Flush();


                                //while (NetStream.DataAvailable)
                                //while(NetClient.Available>0)
                                //{
                                //接收返回结果
                                RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                        }

                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                while (RecCount < 21)
                                {
                                    #region 检查连接是否断开
                                    if (RecCount == 0)
                                    {
                                        IdleFlag = 4;

                                        ErrorCount++;

                                        Log.Record("server.log", "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...");

                                        ErrorMessage = "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...";
                                        return false;
                                    }
                                    #endregion 检查连接是否断开

                                    if (DebugFlag)
                                    {
                                        if (this.EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                    }

                                    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                    if (RecCount != 0)
                                    {
                                        int tmpRecCount = RecCount;

                                        byte[] RecBuf2 = new byte[m_MaxPacket];

                                        //数据包中长度
                                        int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                        //未接收完毕，继续接收数据包
                                        while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                        {
                                            if (EtermType.IndexOf("地址认证") != -1)
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                            }
                                            else
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                            }

                                            tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                            if (tmpRecCount != 0)
                                            {
                                                //合并数据包
                                                Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                            }

                                            RecCount += tmpRecCount;
                                        }
                                    }

                                    if (DebugDataFlag)
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                        }
                                    }

                                    continue;
                                }

                                if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                {
                                    if (RecCount > 23)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                        tmplastcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                }
                                else
                                {
                                    if (RecCount > 21)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                        tmplastcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                }
                                //}

                                //指令间隔时间
                                if (this.Interval > 0)
                                {
                                    Thread.Sleep(this.Interval);
                                }
                            }
                            #endregion 判断是否需要签入工作号，如果需要则自动签入并重发指令

                            //if (DebugFlag)
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //}
                        }

                        //是否为航班查询指令
                        //由于航班查询指令PN会直接翻天，因此需要对于航班查询自动PN的结果日期进行专门处理
                        bool avflag = false;
                        //判断是否为av 或者 avh 指令
                        if (cmd.Trim().ToLower().Substring(0, 2) == "av")
                        {
                            avflag = true;
                        }

                        //最后一个指令结果
                        if (i == sl.Length - 1)
                        {
                            //PN返回结果信息，用于结果比较
                            string tmpPnContent = "";

                            #region 发送指令前先检查配置状态
                            if (this.IdleFlag != 1)
                            {
                                //不是忙碌状态则退出

                                ErrorCount++;

                                Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                ErrorMessage = "未找到可用配置...";
                                return false;
                            }
                            #endregion 发送指令前先检查配置状态

                            //最后一个指令自动发送PN指令直到尾页
                            NetStream.Write(tmpPnBuf, 0, tmpPnBuf.Length);
                            NetStream.Flush();

                            if (DebugFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送PN指令...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送PN指令...");
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送PN指令数据包：", tmpPnBuf, tmpPnBuf.Length);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送PN指令数据包：", tmpPnBuf, tmpPnBuf.Length);
                                }
                            }

                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //读取返回结果信息
                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                    }

                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                            }

                            #region 数据包长度不足，则继续循环接收
                            while (RecCount < 21)
                            {
                                #region 检查连接是否断开
                                if (RecCount == 0)
                                {
                                    IdleFlag = 4;

                                    ErrorCount++;

                                    Log.Record("server.log", "发送指令：" + tmpcmd + "接收返回信息失败，可能通讯断开...");

                                    ErrorMessage = "发送指令：" + tmpcmd + "接收返回信息失败，可能通讯断开...";
                                    return false;
                                }
                                #endregion 检查连接是否断开

                                if (DebugFlag)
                                {
                                    if (this.EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                    }
                                }

                                RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        if (EtermType.IndexOf("地址认证") != -1)
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                        }

                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                    }
                                }
                                continue;
                            }
                            #endregion 数据包长度不足，则继续循环接收


                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            {
                                if (RecCount > 23)
                                {
                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                }
                            }
                            else
                            {
                                if (RecCount > 21)
                                {
                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                }
                            }

                            if (DebugFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到返回结果：" + tmpPnContent);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到返回结果：" + tmpPnContent);
                                }
                            }
                            //}

                            //判断是否需要签入工作号
                            if (!IfNeedSignIn(tmpPnContent))
                            {
                                //指令间隔时间
                                if (this.Interval > 0)
                                {
                                    Thread.Sleep(this.Interval);
                                }

                                //上一次PN结果，用于结果对比
                                string prePnContent = tmplastcontent;
                                //如果是AV 或 AVH指令，则进行专门比较
                                #region AV或AVH 的PN结果判断处理
                                if (avflag)
                                {
                                    //没有结束，继续PN循环
                                    while (!IfAvhEnd(cmd, tmpPnContent))
                                    {
                                        tmpcontent += tmpPnContent;

                                        //发送PN指令
                                        NetStream.Write(tmpPnBuf, 0, tmpPnBuf.Length);
                                        NetStream.Flush();

                                        if (DebugFlag)
                                        {
                                            if (EtermType.IndexOf("地址认证") != -1)
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送PN指令...");
                                            }
                                            else
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送PN指令...");
                                            }
                                        }

                                        if (DebugDataFlag)
                                        {
                                            if (EtermType.IndexOf("地址认证") != -1)
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送PN指令数据包：", tmpPnBuf, tmpPnBuf.Length);
                                            }
                                            else
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送PN指令数据包：", tmpPnBuf, tmpPnBuf.Length);
                                            }
                                        }

                                        //while (NetStream.DataAvailable)
                                        //while(NetClient.Available>0)
                                        //{
                                        //读取返回结果信息
                                        RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                        if (RecCount != 0)
                                        {
                                            int tmpRecCount = RecCount;

                                            byte[] RecBuf2 = new byte[m_MaxPacket];

                                            //数据包中长度
                                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                            //未接收完毕，继续接收数据包
                                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                                }

                                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                                if (tmpRecCount != 0)
                                                {
                                                    //合并数据包
                                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                }

                                                RecCount += tmpRecCount;
                                            }
                                        }

                                        if (DebugDataFlag)
                                        {
                                            if (EtermType.IndexOf("地址认证") != -1)
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                            }
                                            else
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                            }
                                        }

                                        #region 数据包长度不足，则继续循环接收
                                        while (RecCount < 21)
                                        {
                                            #region 检查连接是否断开
                                            if (RecCount == 0)
                                            {
                                                IdleFlag = 4;

                                                ErrorCount++;

                                                Log.Record("server.log", "发送PN指令接收返回信息失败，可能通讯断开...");

                                                ErrorMessage = "发送PN指令接收返回信息失败，可能通讯断开...";
                                                return false;
                                            }
                                            #endregion 检查连接是否断开

                                            if (DebugFlag)
                                            {
                                                if (this.EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                                }
                                            }
                                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                            if (RecCount != 0)
                                            {
                                                int tmpRecCount = RecCount;

                                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                                //数据包中长度
                                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                                //未接收完毕，继续接收数据包
                                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                                {
                                                    if (EtermType.IndexOf("地址认证") != -1)
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                                    }
                                                    else
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                                    }

                                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                                    if (tmpRecCount != 0)
                                                    {
                                                        //合并数据包
                                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                    }

                                                    RecCount += tmpRecCount;
                                                }
                                            }

                                            if (DebugDataFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                                }
                                            }
                                            continue;
                                        }
                                        #endregion 数据包长度不足，则继续循环接收

                                        tmpPnContent = "";
                                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                        {
                                            if (RecCount > 23)
                                            {
                                                tmpPnContent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                            }
                                        }
                                        else
                                        {
                                            if (RecCount > 21)
                                            {
                                                tmpPnContent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                            }
                                        }

                                        if (DebugFlag)
                                        {
                                            if (EtermType.IndexOf("地址认证") != -1)
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到返回结果：" + tmpPnContent);
                                            }
                                            else
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到返回结果：" + tmpPnContent);
                                            }
                                        }
                                        //}

                                        //指令间隔时间
                                        if (this.Interval > 0)
                                        {
                                            Thread.Sleep(this.Interval);
                                        }
                                    }
                                }
                                #endregion AV或AVH 的PN结果判断处理

                                #region 其他指令的PN结果处理
                                //其他指令则以重复返回为结束标志
                                else
                                {
                                    ////判断pn的返回信息是否一致，如果一致则表示结束
                                    //循环判断，直至符合条件退出

                                    //PN计数
                                    int tmppncount = 1;
                                    //最大PN次数20次
                                    int tmpMaxPnCount = 20;
                                    while (true)
                                    {
                                        //比较PN结果与上一次结果是否一样，如果一样则认为结束
                                        if (prePnContent == tmpPnContent)
                                        {
                                            if (DebugFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送指令：" + cmd + " 返回结果检查到与上次PN结果一致，认为PN结束...");
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + cmd + " 返回结果检查到与上次PN结果一致，认为PN结束...");
                                                }
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (this.EtermType == "胜意PID")
                                            {
                                                //胜意PID的RT指令pn判断头部是否重复
                                                if (tmpcmd.ToLower().Trim().Substring(0, 2) == "rt")
                                                {
                                                    if (tmpcontent.IndexOf(tmpPnContent.Substring(2, 20)) != -1)
                                                    {
                                                        //重复就覆盖前面的内容
                                                        tmpcontent = tmpPnContent;
                                                    }
                                                    else
                                                    {
                                                        //累加收到的结果信息
                                                        tmpcontent += tmpPnContent;
                                                    }
                                                }
                                                else
                                                {
                                                    //累加收到的结果信息
                                                    tmpcontent += tmpPnContent;
                                                }
                                            }
                                            else
                                            {
                                                //累加收到的结果信息
                                                tmpcontent += tmpPnContent;
                                            }

                                            prePnContent = tmpPnContent;
                                            tmpPnContent = "";

                                            //继续发送PN指令并接收返回
                                            //最后一个指令自动发送PN指令直到尾页
                                            NetStream.Write(tmpPnBuf, 0, tmpPnBuf.Length);
                                            NetStream.Flush();

                                            if (DebugFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送PN指令...");
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送PN指令...");
                                                }
                                            }

                                            if (DebugDataFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送PN指令数据包：", tmpPnBuf, tmpPnBuf.Length);
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送PN指令数据包：", tmpPnBuf, tmpPnBuf.Length);
                                                }
                                            }

                                            // while (NetStream.DataAvailable)
                                            //while(NetClient.Available>0)
                                            //{
                                            //读取返回结果信息
                                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                            if (RecCount != 0)
                                            {
                                                int tmpRecCount = RecCount;

                                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                                //数据包中长度
                                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                                //未接收完毕，继续接收数据包
                                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                                {
                                                    if (EtermType.IndexOf("地址认证") != -1)
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                                    }
                                                    else
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                                    }

                                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                                    if (tmpRecCount != 0)
                                                    {
                                                        //合并数据包
                                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                    }

                                                    RecCount += tmpRecCount;
                                                }
                                            }

                                            if (DebugDataFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                                }
                                            }

                                            #region 数据包长度不足，则继续循环接收
                                            while (RecCount < 21)
                                            {
                                                #region 检查连接是否断开
                                                if (RecCount == 0)
                                                {
                                                    IdleFlag = 4;

                                                    ErrorCount++;

                                                    Log.Record("server.log", "发送PN指令接收返回信息失败，可能通讯断开...");

                                                    ErrorMessage = "发送PN指令接收返回信息失败，可能通讯断开...";
                                                    return false;
                                                }
                                                #endregion 检查连接是否断开

                                                if (DebugFlag)
                                                {
                                                    if (this.EtermType.IndexOf("地址认证") != -1)
                                                    {
                                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                                    }
                                                    else
                                                    {
                                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                                    }
                                                }
                                                RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                                if (RecCount != 0)
                                                {
                                                    int tmpRecCount = RecCount;

                                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                                    //数据包中长度
                                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                                    //未接收完毕，继续接收数据包
                                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                                    {
                                                        if (EtermType.IndexOf("地址认证") != -1)
                                                        {
                                                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                                        }
                                                        else
                                                        {
                                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                                        }

                                                        tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                                        if (tmpRecCount != 0)
                                                        {
                                                            //合并数据包
                                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                        }

                                                        RecCount += tmpRecCount;
                                                    }
                                                }

                                                if (DebugDataFlag)
                                                {
                                                    if (EtermType.IndexOf("地址认证") != -1)
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                                    }
                                                    else
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                                    }
                                                }
                                                continue;
                                            }
                                            #endregion 数据包长度不足，则继续循环接收

                                            tmpPnContent = "";
                                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                            {
                                                if (RecCount > 23)
                                                {
                                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                                }
                                            }
                                            else
                                            {
                                                if (RecCount > 21)
                                                {
                                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                                }
                                            }

                                            if (DebugFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到返回结果：" + tmpPnContent);
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到返回结果：" + tmpPnContent);
                                                }
                                            }
                                            //}

                                            //指令间隔时间
                                            if (this.Interval > 0)
                                            {
                                                Thread.Sleep(this.Interval);
                                            }
                                        }

                                        //判断是否超出了最大PN循环次数
                                        tmppncount += 1;
                                        if (tmppncount > tmpMaxPnCount)
                                        {
                                            if (DebugFlag)
                                            {
                                                if (EtermType.IndexOf("地址认证") != -1)
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送指令：" + cmd + " 翻页次数超过" + tmpMaxPnCount.ToString() + " 异常退出循环处理...");
                                                }
                                                else
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + cmd + " 翻页次数超过" + tmpMaxPnCount.ToString() + " 异常退出循环处理...");
                                                }
                                            }
                                            break;
                                        }
                                        //m_MaxWhileCount
                                    }
                                }
                                #endregion 其他指令的PN结果处理
                            }

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            //替换数据包长度信息
                            tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));

                            break;
                        }
                    }
                    #endregion 循环发送指令并获取返回结果

                    #region 发送封口指令信息
                    //系统自动封口
                    //封口返回结果为空或者NO PNR，则返回封口之前的结果信息
                    //否则返回封口后的结果信息
                    lock (NetStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        //网站用户发送I指令
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            //发送I指令
                            NetStream.Write(ibuf, 0, ibuf.Length);
                            NetStream.Flush();
                        }
                        //客户端用户发送封口指令
                        else
                        {
                            //发送封口指令
                            NetStream.Write(fengkouBuf, 0, fengkouBuf.Length);
                            NetStream.Flush();
                        }

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 发送封口指令...");
                            }
                            else
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送封口指令...");
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 发送封口数据包：", CmdBuf, CmdBuf.Length);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 发送封口数据包：", CmdBuf, CmdBuf.Length);
                            }
                        }

                        //把返回结果字符串置空
                        string tmpcontent2 = "";

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        //接收返回数据包
                        RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                }

                                tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }
                        }

                        while (RecCount < 21)
                        {
                            #region 检查连接是否断开
                            if (RecCount == 0)
                            {
                                IdleFlag = 4;

                                ErrorCount++;

                                Log.Record("server.log", "发送封口指令接收返回信息失败，可能通讯断开...");

                                ErrorMessage = "发送封口指令接收返回信息失败，可能通讯断开...";
                                return false;
                            }
                            #endregion 检查连接是否断开

                            if (DebugFlag)
                            {
                                if (this.EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                }
                                else
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                }
                            }
                            RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    if (EtermType.IndexOf("地址认证") != -1)
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                                    }

                                    tmpRecCount = NetStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                if (EtermType.IndexOf("地址认证") != -1)
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收数据包：", RecBuf, RecCount);
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                            }

                            continue;
                        }

                        //Kevin 2011-04-28 Add
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount >= 23)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount >= 21)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        if (DebugFlag)
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 接收到返回结果：" + tmpcontent2);
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到返回结果：" + tmpcontent2);
                            }
                        }
                        //}

                        //指令间隔时间
                        if (this.Interval > 0)
                        {
                            Thread.Sleep(this.Interval);
                        }

                        //接收到异常返回信息，需要进行配置重新认证
                        //if (tmpcontent2.Trim().ToUpper() == "S")
                        //{
                        //    ResetFlag = true;
                        //}

                        //如果返回结果不包含NO PNR 并且 返回结果不为空，则返回此结果信息
                        if ((tmpcontent2.ToUpper().IndexOf("NO PNR") == -1) && (tmpcontent2.Trim() != "") && (tmpcontent2.ToUpper().IndexOf("CHECK") == -1))
                        {
                            //如果需要返回所有指令结果，则把结果累加
                            if (allFlag)
                            {
                                tmpcontent += tmpcontent2;
                            }
                            //如果只返回最后一个指令的结果，则把当前封口指令的结果返回
                            //else
                            //{
                            //    tmpcontent = tmpcontent2;
                            //}

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            //替换数据包长度信息
                            short tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            byte[] lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.Clear();
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));
                        }
                    }
                    #endregion 发送封口指令信息
                }
                #endregion 其他指令处理

                //指令数量统计，用于配置均衡
                this.CommandCount++;

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                if (ResetFlag)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "接收到异常返回结果，需要重新进行配置认证...");
                    }
                    //设置异常标志，进行配置重新认证
                    IdleFlag = 4;
                }
                else
                {
                    //配置设置为空闲
                    IdleFlag = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "发送指令：" + cmd + "，处理出错，错误信息：" + ex.Message);
                IdleFlag = 4;
                ErrorMessage = ex.Message;
                return false;
            }
        }
        #endregion 发送指令自动PN并获取返回结果信息

        #region 判断 AV 或 AVH 结果是否已经结束（翻天为结束标志）
        /// <summary>
        /// 判断 AV 或 AVH 结果是否已经结束（翻天为结束标志）
        /// </summary>
        /// <param name="cmd">发送的指令信息</param>
        /// <param name="returnContent">返回的结果信息</param>
        /// <returns>true：已经结束；false：没有结束</returns>
        private bool IfAvhEnd(string cmd, string returnContent)
        {
            #region 分析av指令中的日期信息
            string tmpdate = cmd.Substring(11, 5);

            //分析av指令中的日期信息
            if ((cmd.Length > 3) && (cmd.ToLower().Substring(0, 3) == "av:"))
            {
                tmpdate = cmd.Substring(10, 5);
            }

            #endregion 分析av指令中的日期信息

            #region 分析结果中的日期信息
            //如果两个日期不一致，则返回空
            int pos1 = returnContent.IndexOf("(");
            int pos2 = returnContent.IndexOf(")");
            if ((pos1 == -1) || (pos2 == -1))
            {
                //如果解析结果出错则认为已经结束
                return true;
            }

            //当前信息的航班日期
            string curdate = returnContent.Substring(1, pos1 - 1);

            if (curdate.ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
            #endregion 分析结果中的日期信息
        }
        #endregion 判断 AV 或 AVH 结果是否已经结束（翻天为结束标志）

        #region 判断是否需要签入
        /// <summary>
        /// 判断是否需要签入
        /// </summary>
        /// <param name="reccontent">指令返回信息</param>
        /// <returns></returns>
        private bool IfNeedSignIn(string reccontent)
        {
            ////Kevin 2009-09-05 Edit
            ////航空公司配置返回信息有可能换行符之前存在 0x1B, 0x62非法字符，去掉
            //byte[] Errorbuf = { 0x1B, 0x62 };
            //string strError = Encoding.Default.GetString(Errorbuf);
            //reccontent = reccontent.Replace(strError, "");
            //byte[] Errorbuf2 = { 0x1B, 0x0B };
            //string strError2 = Encoding.Default.GetString(Errorbuf2);
            //reccontent = reccontent.Replace(strError2, "");

            //PLEASE SIGN IN FIRST.
            //SI    
            string tmpstr = reccontent.Trim().ToLower();
            //航空公司配置，需要签入
            //Kevin 2011-03-24 调整si判断
            if (((tmpstr.Trim().Length > 1) && (tmpstr.Substring(0, 2) == "si"))
                || (tmpstr.IndexOf("please sign in first") != -1)
                || (tmpstr.IndexOf("signed out") != -1) || (tmpstr.ToLower() == "s"))
            {
                SignIn();
                return true;
            }
            return false;
        }
        #endregion 判断是否需要签入
    }

    /// <summary>
    /// 443加密Eterm处理类（包括代理人443配置和航空公司443大配置）
    /// </summary>
    public class Eterm443Manage : BaseManage
    {
        /// <summary>
        /// Eterm内部类（用于获取F7信息）
        /// </summary>
        private Eterm3Lib.ApplicationClass eterm3 = null;

        /// <summary>
        /// Eterm内部连接端口（用于获取F7信息）要与ThreadWork中监听端口一致
        /// </summary>
        private int m_SharePort = 3510;

        /// <summary>
        /// 是否F7认证（true：需要F7认证； false：无需F7认证）
        /// </summary>
        private bool m_AuthFlag = false;

        /// <summary>
        /// Eterm内部连接端口（用于获取F7信息）要与ThreadWork中监听端口一致
        /// </summary>
        public int SharePort
        {
            get { return m_SharePort; }
            set { m_SharePort = value; }
        }

        /// <summary>
        /// 443加密认证配置认证处理过程
        /// </summary>
        public void DoWith443()
        {
            try
            {
                lock (ThreadWork.lockX)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "检查并自动关闭开启的Eterm客户端...");
                    }

                    //检查Eterm是否运行，如果运行则先关闭
                    System.Diagnostics.Process[] CloseID = System.Diagnostics.Process.GetProcessesByName("eTerm3");
                    if (CloseID.Length != 0)
                    {
                        for (int i = 0; i < CloseID.Length; i++)
                        {
                            if (CloseID[i].Responding && !CloseID[i].HasExited)
                            {
                                CloseID[i].CloseMainWindow();
                                if (!CloseID[i].HasExited)
                                {
                                    CloseID[i].Kill();
                                }
                            }
                            else
                            {
                                CloseID[i].Kill();
                            }
                        }
                    }
                }

                if (DebugFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 正在准备连接航信服务器：" + OriginalConfigManage.Original_Ip
                        + "，端口：" + OriginalConfigManage.Original_Port.ToString());
                }

                if (NetClient != null)
                {
                    NetClient = null;
                }

                NetClient = new TcpClient();
                NetClient.Connect(OriginalConfigManage.Original_Ip, OriginalConfigManage.Original_Port);

                if (DebugFlag)
                {
                    Log.Record("server.log", "开始连接航信服务器：" + OriginalConfigManage.Original_Ip + "，端口：" + OriginalConfigManage.Original_Port.ToString() + "...");
                }

                //SslStream 

                SSlStream = new SslStream(NetClient.GetStream(),
                    false, new RemoteCertificateValidationCallback(CertificateValidationCallback),
                    null);

                //Console.WriteLine("握手..." + server);

                SSlStream.AuthenticateAsClient(OriginalConfigManage.Original_Ip);


                //登入
                byte[] C = //{ 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x30, 0x32, 0x31, 0x38, 0x36, 0x35, 0x34, 0x66, 0x62, 0x30, 0x63, 0x31, 0x39, 0x32, 0x2E, 0x31, 0x36, 0x38, 0x2E, 0x31, 0x2E, 0x34, 0x20, 0x20, 0x20, 0x20, 0x33, 0x38, 0x34, 0x37, 0x30, 0x31, 0x30, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//{ 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x30, 0x30, 0x33, 0x30, 0x64, 0x34, 0x37, 0x35, 0x36, 0x62, 0x33, 0x31, 0x39, 0x32, 0x2E, 0x31, 0x36, 0x38, 0x2E, 0x31, 0x2E, 0x31, 0x35, 0x39, 0x20, 0x20, 0x33, 0x36, 0x31, 0x30, 0x34, 0x31, 0x30, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


               {0x01 ,0xA2 ,0x6E ,0x6B ,0x67 ,0x30 ,0x31 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x31 
               ,0x32 ,0x33 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x30 ,0x30 ,0x65 ,0x30 ,0x34 ,0x63 ,0x31 
               ,0x64 ,0x33 ,0x37 ,0x64 ,0x37 ,0x31 ,0x31 ,0x36 ,0x2E ,0x32 ,0x35 ,0x34 ,0x2E ,0x32 ,0x30 ,0x36 ,0x2E ,0x32 ,0x33 
               ,0x20 ,0x33 ,0x38 ,0x34 ,0x37 ,0x34 ,0x31 ,0x30 ,0x00 ,0x30 ,0x30 ,0x30 ,0x30 ,0x30 ,0x30 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 
               ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00};

                //帐号
                byte[] px = ASCIIEncoding.Default.GetBytes(OriginalConfigManage.Original_ConfigName);
                Array.Copy(px, 0, C, 2, px.Length);

                //密码
                byte[] px1 = ASCIIEncoding.Default.GetBytes(OriginalConfigManage.Original_ConfigPass);
                Array.Copy(px1, 0, C, 18, px1.Length);

                //if (DebugFlag)
                //{
                //    Log.Record("server.log", "向航信发送数据包：", C, C.Length);
                //}

                SSlStream.Write(C);
                SSlStream.Flush();

                if (DebugFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 连接航信服务器进行认证...");
                }

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送认证数据包：", C, C.Length);
                }

                byte[] D = new byte[1024];
                int rcount = SSlStream.Read(D, 0, 0x37);

                if (DebugFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 收到航信服务器返回数据包...");
                }

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包", D, rcount);
                }

                if (D[2] == 0x00) //error
                {
                    string s = ASCIIEncoding.Default.GetString(D, 3, 0x37 - 3);
                    //登录失败，写日志和显示信息

                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "登录航信服务器失败，失败原因：" + s);
                    }

                    if (s.IndexOf("请检查用户名和口令") != -1)
                    {
                        //帐号或密码不正确
                        IdleFlag = 2;

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "认证失败，帐号或密码不正确！");
                        }
                    }
                    else
                    {
                        IdleFlag = 4;
                    }


                    return;
                }

                //
                if (DebugFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 登录航信服务器成功...");
                }

                //黑屏包头
                Array.Copy(D, 8, m_cmdHeadBuf, 0, 2);

                //Kevin 特殊处理
                //如果标志头为 0xD1，则替换为0x51
                if (m_cmdHeadBuf[1] == 0xD1)
                {
                    m_cmdHeadBuf[1] = 0x51;
                }

                byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                SSlStream.Write(FE, 0, FE.Length);
                SSlStream.Flush();

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE, FE.Length);
                }

                byte[] FE2 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                SSlStream.Write(FE2, 0, FE2.Length);
                SSlStream.Flush();

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE2, FE2.Length);
                }

                byte[] FE3 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                SSlStream.Write(FE3, 0, FE3.Length);
                SSlStream.Flush();

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE3, FE3.Length);
                }


                //如果接收到的为F6数据则需要发送F7认证，否则不发送
                //是否需要发送F7认证
                m_AuthFlag = false;

                byte[] F6 = new byte[1024];
                rcount = SSlStream.Read(F6, 0, 1024);

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", F6, rcount);
                }

                if (F6[1] == 0xF6)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到F6数据，需要获取F7数据信息...");
                    }

                    m_AuthFlag = true;
                    //接收到F6信息，发送F7信息
                    lock (ThreadWork.lockX)
                    {
                        if (eterm3 == null)
                        {
                            eterm3 = new Eterm3Lib.ApplicationClass();
                            eterm3.Connect(0, "127.0.0.1:" + Convert.ToString(m_SharePort), "$", "$");
                        }

                        ThreadWork.F6 = F6;
                        ThreadWork.newflag = true;

                        //Console.WriteLine("获取 F6..." + server);

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 正在准备获取F7数据信息...");
                        }

                        //等待获取到F7信息
                        //lock (ThreadWork.lockY) ;
                        int count = 0;
                        while ((ThreadWork.newflag) && (count < 20))
                        {
                            Thread.Sleep(100);
                            count++;
                            continue;
                        }

                        if (ThreadWork.newflag)
                        {
                            //取得F7信息失败
                            if (eterm3 != null)
                            {
                                eterm3.Exit();
                                eterm3 = null;
                            }

                            //停止ThreadWork的监听端口，然后再重新开启监听端口
                            if (ThreadWork.sslServer != null)
                            {
                                ThreadWork.sslServer.Stop();
                            }

                            m_AuthFlag = false;

                            return;
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 已经获取到F7数据信息并发送F7数据信息...");
                        }

                        //Console.WriteLine("发送 F7..." + server);

                        //Log.Record("server.log", "向航信发送数据包：", FE, 8);

                        SSlStream.Write(ThreadWork.F7);
                        SSlStream.Flush();

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", ThreadWork.F7, ThreadWork.F7.Length);
                        }

                        //Log.Record("server.log", "向航信发送F7数据包：", ThreadWork.F7, ThreadWork.F7.Length);
                        if (eterm3 != null)
                        {
                            eterm3.Exit();
                            eterm3 = null;
                        }
                    }
                }
                //上次发送指令时间
                SendCmdTime = DateTime.Now;

                byte[] FD = new byte[1024];

                int tolcount = rcount;

                //如果没有F6F7数据包，则累计之前的接收数据
                if (!m_AuthFlag)
                {
                    if (rcount < 18)
                    {
                        rcount = SSlStream.Read(FD, 0, 1024);
                        tolcount += rcount;
                    }
                }
                else
                {
                    rcount = SSlStream.Read(FD, 0, 1024);
                }



                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 收到返回数据包：", FD, rcount);
                }

                while ((!m_AuthFlag) && (tolcount < 18))
                {
                    rcount = SSlStream.Read(FD, 0, 1024);
                    tolcount += rcount;

                    //接收异常，退出
                    if (rcount == 0)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "认证过程出现异常，可能连接已断开...");

                        //ReleaseEterm();
                        IdleFlag = 4;
                        //错误计数
                        ErrorCount++;
                        return;
                    }

                    if (DebugDataFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 收到返回数据包：", FD, rcount);
                    }
                }

                //Kevin 发送F9并接受返回信息
                byte[] FE4 = { 0x01, 0xF9, 0x00, 0x44, 0x00, 0x01, 0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ,0x00, 0x00, 0x00 ,0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                SSlStream.Write(FE4, 0, FE4.Length);
                SSlStream.Flush();

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", FE4, FE4.Length);
                }

                byte[] FD2 = new byte[m_MaxPacket];

                rcount = SSlStream.Read(FD2, 0, m_MaxPacket);

                if (DebugDataFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", FD2, rcount);
                }

                if (OriginalConfigManage.Original_ConfigSi.Trim() != "")
                {
                    //so（签出），并以当前工作号签入
                    if (!SignIn())
                    {
                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 签入工作号操作失败，准备释放资源...");
                        }

                        //签入工作号出现异常
                        //ReleaseEterm();
                        return;
                    }
                }


                //登陆成功，则把错误计数清零
                ErrorCount = 0;

                //置为空闲
                IdleFlag = 0;
                if (DebugFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 空闲...");
                }
            }
            catch (Exception e)
            {
                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "认证过程出现异常，异常信息：" + e.Message);

                //ReleaseEterm();
                IdleFlag = 4;
                //错误计数
                ErrorCount++;
            }
        }

        /// <summary>
        /// 工作号签入
        /// </summary>
        /// <returns>true：签入成功；  false：潜入失败；</returns>
        private bool SignIn()
        {
            try
            {
                if (DebugFlag)
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备签出签入工作号...");
                }

                byte[] headBuf = { 0x01, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x01, 0x39, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E };
                //byte[] headBuf2 ={ 0x01, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x01, 0x39, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x20, 0x20, 0x00, 0x0F, 0x1E };
                byte[] endBuf = { 0x20, 0x03 };

                if (!m_AuthFlag)
                {
                    //替换黑屏包头 
                    Array.Copy(m_cmdHeadBuf, 0, headBuf, 8, 1);
                    //Array.Copy(headBuf2, 0, headBuf, 0, headBuf.Length);
                }
                else
                {
                    //替换黑屏包头 
                    Array.Copy(m_cmdHeadBuf, 0, headBuf, 8, 2);
                }

                //Kevin 2010-03-27 Add
                //先签出工作号
                byte[] soBuf = Encoding.Default.GetBytes("so");
                //替换数据包长度字节
                short tmplen = (short)(soBuf.Length + headBuf.Length + endBuf.Length);
                short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] solenbuf = BitConverter.GetBytes(count2);

                Array.Copy(solenbuf, 0, headBuf, 2, 2);

                ArrayList soal = new ArrayList();
                soal.AddRange(headBuf);
                soal.AddRange(soBuf);
                soal.AddRange(endBuf);

                byte[] so_contentbuf = (byte[])(soal.ToArray(typeof(byte)));

                byte[] workBuf = null;

                //组织工作号密码信息
                if ((OriginalConfigManage.Original_ConfigSi.Trim().Length > 3) && (OriginalConfigManage.Original_ConfigSi.Trim().ToUpper().Substring(0, 2) == "SI"))
                {
                    workBuf = Encoding.Default.GetBytes(OriginalConfigManage.Original_ConfigSi);
                }
                else
                {
                    workBuf = Encoding.Default.GetBytes("SI:" + OriginalConfigManage.Original_ConfigSi);
                }

                //替换数据包长度字节
                tmplen = (short)(workBuf.Length + headBuf.Length + endBuf.Length);
                count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] lenbuf = BitConverter.GetBytes(count2);

                Array.Copy(lenbuf, 0, headBuf, 2, 2);

                byte[] recbuf = new byte[m_MaxPacket];

                int rcount = 0;

                string tmpcontent = "";

                ArrayList al = new ArrayList();
                al.AddRange(headBuf);
                al.AddRange(workBuf);
                al.AddRange(endBuf);

                byte[] contentbuf = (byte[])(al.ToArray(typeof(byte)));

                if (DebugFlag)
                {
                    Log.Record("server.log", "配置" + OriginalConfigManage.Original_ConfigName + " 正在自动签出工作号...");
                }

                SendCmdTime = DateTime.Now;

                lock (SSlStream)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送签出数据包...");//：", so_contentbuf, so_contentbuf.Length);
                    }

                    SSlStream.Write(so_contentbuf);
                    SSlStream.Flush();

                    if (DebugDataFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送签出数据包：", so_contentbuf, so_contentbuf.Length);
                    }


                    #region 接收返回直到结束
                    //while (NetStream.DataAvailable)
                    //while(NetClient.Available>0)
                    //{
                    rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                    if (rcount != 0)
                    {
                        int tmpRecCount = rcount;

                        byte[] RecBuf2 = new byte[m_MaxPacket];

                        //数据包中长度
                        int RecCount2 = Convert.ToInt32(recbuf[2]) * 16 * 16 + Convert.ToInt32(recbuf[3]);

                        //未接收完毕，继续接收数据包
                        while ((tmpRecCount != 0) && (rcount < RecCount2))
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                            }

                            tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                            if (tmpRecCount != 0)
                            {
                                //合并数据包
                                Array.Copy(RecBuf2, 0, recbuf, rcount, tmpRecCount);
                            }

                            rcount += tmpRecCount;
                        }
                    }
                    #endregion 接收返回直到结束


                    //rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                    if (DebugDataFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", recbuf, rcount);
                    }

                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送签入数据包...");//：", contentbuf, contentbuf.Length);
                    }

                    //指令间隔时间
                    if (this.Interval > 0)
                    {
                        Thread.Sleep(this.Interval);
                    }

                    SSlStream.Write(contentbuf);
                    SSlStream.Flush();

                    if (DebugDataFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送签入数据包：", contentbuf, contentbuf.Length);
                    }

                    #region 接收返回直到结束
                    //while (NetStream.DataAvailable)
                    //while(NetClient.Available>0)
                    //{
                    rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                    if (rcount != 0)
                    {
                        int tmpRecCount = rcount;

                        byte[] RecBuf2 = new byte[m_MaxPacket];

                        //数据包中长度
                        int RecCount2 = Convert.ToInt32(recbuf[2]) * 16 * 16 + Convert.ToInt32(recbuf[3]);

                        //未接收完毕，继续接收数据包
                        while ((tmpRecCount != 0) && (rcount < RecCount2))
                        {
                            if (EtermType.IndexOf("地址认证") != -1)
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                            }
                            else
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                            }

                            tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                            if (tmpRecCount != 0)
                            {
                                //合并数据包
                                Array.Copy(RecBuf2, 0, recbuf, rcount, tmpRecCount);
                            }

                            rcount += tmpRecCount;
                        }
                    }
                    #endregion 接收返回直到结束

                    //rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                    if (DebugDataFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", recbuf, rcount);
                    }

                    #region 屏蔽
                    //while (rcount < 20)
                    //{
                    //    #region 检查连接是否断开
                    //    if (rcount == 0)
                    //    {
                    //        IdleFlag = 4;

                    //        ErrorCount++;

                    //        Log.Record("server.log", "发送封口指令接收返回信息失败，可能通讯断开...");

                    //        return false;
                    //    }
                    //    #endregion 检查连接是否断开

                    //    rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                    //    if (DebugDataFlag)
                    //    {
                    //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", recbuf, rcount);
                    //    }
                    //}
                    #endregion 屏蔽

                    if ((recbuf[17] == 0x1B) && (recbuf[18] == 0x4D))
                    {
                        tmpcontent = Encoding.Default.GetString(recbuf, 19, rcount - 23);
                    }
                    else
                    {
                        tmpcontent = Encoding.Default.GetString(recbuf, 17, rcount - 21);
                    }

                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到返回信息：" + tmpcontent);
                    }

                    #region 屏蔽
                    //int m_whilecount = 0;
                    //while (rcount <= 23)
                    //{
                    //    #region 检查连接是否断开
                    //    if (rcount == 0)
                    //    {
                    //        IdleFlag = 4;

                    //        ErrorCount++;

                    //        Log.Record("server.log", "发送封口指令接收返回信息失败，可能通讯断开...");
                    //        return false;
                    //    }
                    //    #endregion 检查连接是否断开

                    //    rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                    //    if (DebugDataFlag)
                    //    {
                    //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", recbuf, rcount);
                    //    }

                    //    m_whilecount++;

                    //    if ((recbuf[17] == 0x1B) && (recbuf[18] == 0x4D))
                    //    {
                    //        tmpcontent = Encoding.Default.GetString(recbuf, 19, rcount - 23);
                    //    }
                    //    else
                    //    {
                    //        tmpcontent = Encoding.Default.GetString(recbuf, 17, rcount - 21);
                    //    }


                    //    if (DebugFlag)
                    //    {
                    //        //Log.Record("server.log", EtermName + " 接收到返回数据包：", recbuf, rcount);
                    //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到返回信息：" + tmpcontent);
                    //    }
                    //}
                    #endregion 屏蔽

                    #region 接收结果信息错位处理
                    //接收到签入返回结果：bPLEASE SIGN IN FIRST. 
                    //接收信息错位，则继续接收
                    if ((tmpcontent.ToLower().IndexOf("please sign in first") != -1) || (tmpcontent.ToLower().IndexOf("lease wait - transaction in progress") != -1))
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收签入返回结果信息错位，继续接收...");

                        //继续接收
                        rcount = SSlStream.Read(recbuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (rcount != 0)
                        {
                            int tmpRecCount = rcount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(recbuf[2]) * 16 * 16 + Convert.ToInt32(recbuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (rcount < RecCount2))
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, recbuf, rcount, tmpRecCount);
                                }

                                rcount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到签入返回数据包：", recbuf, rcount);
                        }

                        tmpcontent = AnalyseServerContent(recbuf, rcount);
                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到签入返回结果：" + tmpcontent);
                        }
                    }
                    #endregion 接收结果信息错位处理

                    //指令间隔时间
                    if (this.Interval > 0)
                    {
                        Thread.Sleep(this.Interval);
                    }
                }

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 签入工作号出现异常，异常信息：" + ex.Message);
                //ReleaseEterm();
                IdleFlag = 4;
                //错误计数
                ErrorCount++;
                return false;
            }
        }


        /// <summary>
        /// 断开重连，定时发送心跳数据包
        /// </summary>
        public void EtermTimeProc()
        {
            //循环处理,检测并发送心跳包
            int tmpsecondtime = 0;

            while (true)
            {
                //检查是否退出程序
                if (CloseFlag)
                {
                    Log.Record("server.log", "程序退出");
                    break;
                }

                try
                {
                    //kevin 2011-05-26 屏蔽
                    ////如果达到最大错误数，则停用配置，退出循环
                    //if (errorcount >= _maxErrorCount)
                    //{
                    //    m_etermstatus.status = 3;
                    //    m_idle = false;
                    //    sslStream = null;
                    //    netStream = null;
                    //    Log.Record("error.log", "配置：" + m_username + " 错误次数达到最大次数，异常停用");                        
                    //    break;
                    //}

                    //如果还没有开始认证配置，则启动认证
                    if ((IdleFlag == -1) && (SSlStream == null))
                    {
                        IdleFlag = -2;
                        DoWith443();
                        continue;
                    }

                    //Kevin 2010-06-21Add
                    //如果配置状态异常，并且Stream不为空，则置空
                    if (((IdleFlag == 2) || (IdleFlag == 3) || (IdleFlag == 4)) && (SSlStream != null))
                    {

                        if (SSlStream != null)
                        {
                            Log.Record("server.log", "配置状态异常SslStream不为空，重置为null");
                            SSlStream.Close();
                        }
                        SSlStream = null;
                    }

                    //检测Socket是否已经断开与服务器的连接
                    if ((SSlStream == null) && ((IdleFlag!=-2)&&(IdleFlag != -1) && (IdleFlag != 2) && (IdleFlag != 3)))//((IdleFlag != -1) || (IdleFlag == 4)))
                    {
                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 异常，重新向航信进行认证连接...");
                        }
                        DoWith443();
                        continue;
                    }

                    //心跳包
                    tmpsecondtime = PublicInfo.DateDiff(DateTime.Now, SendCmdTime);

                    if ((tmpsecondtime >= m_MaxHeartTimes) && (IdleFlag == 0))
                    {
                        byte[] buf = new byte[m_MaxPacket];

                        lock (SSlStream)
                        {
                            HeartStartTime = DateTime.Now;

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 向航信发送心跳包...");
                            }
                            SSlStream.Write(m_HeartBuf, 0, 5);
                            SSlStream.Flush();

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送心跳包：", m_HeartBuf, 5);
                            }

                            int rcount2 = SSlStream.Read(buf, 0, m_MaxPacket);

                            #region 读取数据包是否完整，如果未接收完毕则继续
                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (rcount2 != 0)
                            {
                                int tmpRecCount = rcount2;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(buf[2]) * 16 * 16 + Convert.ToInt32(buf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (rcount2 < RecCount2))
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, buf, rcount2, tmpRecCount);
                                    }

                                    rcount2 += tmpRecCount;
                                }
                            }
                            #endregion 读取数据包是否完整，如果未接收完毕则继续

                            #region 如果接收到的数据包大于5，则继续接收
                            if (rcount2 > 5)
                            {
                                rcount2 = SSlStream.Read(buf, 0, m_MaxPacket);
                            }
                            #endregion 如果接收到的数据包大于5，则继续接收

                            HeartEndTime = DateTime.Now;

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收心跳包返回结果...");
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", buf, rcount2);
                            }

                        }
                        SendCmdTime = DateTime.Now;
                    }
                    //5秒钟检测一次
                    Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    HeartEndTime = HeartStartTime;
                    SendCmdTime = DateTime.Now;
                    if (DebugFlag)
                    {
                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + "发送心跳包过程出错，错误信息：" + e.Message);
                    }
                    IdleFlag = 4;
                    //ReleaseEterm();
                    //错误计数
                    ErrorCount++;
                }
            }
        }

        #region 直接转发指令并返回结果信息
        public bool SendCmdDirectAndGetResult(string username,byte[] CmdBuf, ref byte[] ResBuf,ref int RecCount)
        {
            ResBuf = null;

            try
            {
                //发送指令
                SSlStream.Write(CmdBuf, 0, CmdBuf.Length);
                SSlStream.Flush();

                //接收返回信息
                ResBuf = new byte[m_MaxPacket];
                RecCount = 0;
                RecCount = SSlStream.Read(ResBuf, 0, m_MaxPacket);

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                //配置设置为空闲
                IdleFlag = 0;

                #region 屏蔽
                ////Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                //if (RecCount != 0)
                //{
                //    int tmpRecCount = RecCount;

                //    byte[] tmpbuf2 = new byte[m_MaxPacket];

                //    //数据包中长度
                //    int RecCount2 = Convert.ToInt32(tmpbuf[2]) * 16 * 16 + Convert.ToInt32(tmpbuf[3]);

                //    //未接收完毕，继续接收数据包
                //    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                //    {
                //        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                //        tmpRecCount = SSlStream.Read(tmpbuf2, 0, m_MaxPacket);

                //        if (tmpRecCount != 0)
                //        {
                //            //合并数据包
                //            Array.Copy(tmpbuf2, 0, tmpbuf, RecCount, tmpRecCount);
                //        }

                //        RecCount += tmpRecCount;
                //    }
                //}
                #endregion 屏蔽
            }
            catch (Exception ex)
            {
                IdleFlag = 4;
                return false;
            }

            return true;
        }
        #endregion 直接转发指令并返回结果信息

        #region 发送指令并获取返回结果信息
        /// <summary>
        /// 发送指令并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, ref byte[] ResBuf, ref string ErrorMessage)
        {
            ResBuf = null;
            ErrorMessage = "";

            //是否需要重新认证配置标志
            bool ResetFlag = false;

            try
            {
                //I指令数据包
                byte[] ibuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x69, 0x20, 0x03 };
                //封口指令数据包
                byte[] fengkouBuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x5C, 0x20, 0x03 };
                //PN指令数据包
                byte[] tmpPnBuf = { 0x1, 0x0, 0x0, 0x17, 0x0, 0x0, 0x0, 0x1, 0x41, 0x51, 0x70, 0x2, 0x1B, 0x0B, 0x22, 0x20, 0x0, 0xF, 0x1E, 0x70, 0x6E, 0x20, 0x3 };

                //回复信息的报头和报尾
                byte[] reshead = { 0x01, 0x00, 0x01, 0xAE, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x21, 0x20, 0x0F, 0x1B, 0x4D };
                byte[] resend = { 0x0D, 0x1E, 0x1B, 0x62, 0x03 };

                ArrayList al2 = new ArrayList();

                //行程单打印指令直接发送
                #region 行程单打印指令处理
                byte[] tmpprintbuf = new byte[5];
                if (CmdBuf.Length > 24)
                {
                    Array.Copy(CmdBuf, 19, tmpprintbuf, 0, 5);
                }
                if (((cmd.Length > 5) && (cmd.ToLower().Substring(0, 5) == "prinv"))||// && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "prinv")) ||
                    ((cmd.Length > 5) && (cmd.ToLower().Substring(0, 5) == "vtinv")))// && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "vtinv")))
                {
                    byte[] tmppBuf1 = { 0x01, 0x00, 0x00, 0x32, 0x0C, 0x00, 0x00, 0x01, 0x8C, 0x0C, 0x00, 0x02 };
                    byte[] tmppBuf2 = { 0x03 };

                    lock (SSlStream)
                    {
                        //组织返回信息
                        byte[] buf3 = Encoding.Default.GetBytes(cmd.Trim().ToUpper().Replace("PRINV:","PRINV ").Replace("VTINV:","VTINV "));

                        //替换数据包长度信息
                        short tmplen = (short)(buf3.Length + tmppBuf1.Length + tmppBuf2.Length);
                        short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                        byte[] lenbuf = BitConverter.GetBytes(count2);
                        Array.Copy(lenbuf, 0, tmppBuf1, 2, 2);
                        al2.AddRange(tmppBuf1);
                        al2.AddRange(buf3);
                        al2.AddRange(tmppBuf2);

                        byte[] tmpCmdBuf = (byte[])(al2.ToArray(typeof(byte)));

                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        //发送指令
                        SSlStream.Write(tmpCmdBuf, 0, tmpCmdBuf.Length);
                        SSlStream.Flush();

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送行程单数据包：", tmpCmdBuf, tmpCmdBuf.Length);
                            Log.Record("server.log", "行程单指令字节包组成：指令长度" + buf3.Length.ToString() + "报头" + tmppBuf1.Length.ToString() + "报尾" + tmppBuf2.Length.ToString());
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送行程单指令：" + cmd + "...");
                        }


                        //接收返回信息
                        byte[] tmpbuf = new byte[m_MaxPacket];
                        int RecCount = 0;
                        RecCount = SSlStream.Read(tmpbuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] tmpbuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(tmpbuf[2]) * 16 * 16 + Convert.ToInt32(tmpbuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                tmpRecCount = SSlStream.Read(tmpbuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(tmpbuf2, 0, tmpbuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包：", tmpbuf, RecCount);
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果...");
                        }

                        //返回成功
                        ResBuf = new byte[RecCount];
                        Array.Copy(tmpbuf, ResBuf, RecCount);
                    }
                }

                #endregion 行程单打印指令处理
                #region 其他指令处理
                else
                {
                    int RecCount = -1;
                    byte[] RecBuf = new byte[m_MaxPacket];

                    //int m_WhileCount = 0;

                    //把指令的黑屏报头信息替换
                    Array.Copy(m_cmdHeadBuf, 0, CmdBuf, 8, 2);

                    //替换i指令报头
                    Array.Copy(m_cmdHeadBuf, 0, ibuf, 8, 2);

                    //替换封口指令的报头
                    Array.Copy(m_cmdHeadBuf, 0, fengkouBuf, 8, 2);

                    //替换PN指令报头
                    Array.Copy(m_cmdHeadBuf, 0, tmpPnBuf, 8, 2);


                    //判断最后一个字符是否为封口符号
                    string tmpcmd = cmd.Trim();

                    //指令最后封口标志
                    //bool fengkouFlag = false;
                    //if ((tmpcmd[tmpcmd.Length - 1] == '@') || (tmpcmd[tmpcmd.Length - 1] == '\\'))
                    //{
                    //    fengkouFlag = true;
                    //}

                    //从数据包中获取发送指令
                    byte[] SendCmdBuf = new byte[CmdBuf.Length - 21];
                    Array.Copy(CmdBuf, 19, SendCmdBuf, 0, SendCmdBuf.Length);
                    string sendcmd = Encoding.Default.GetString(SendCmdBuf);

                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + sendcmd);
                    }

                    #region 检查是否有未接收的接收返回数据
                    //lock (SSlStream)
                    //{
                    //    //while (NetStream.DataAvailable)
                    //    while(NetClient.Available>0)
                    //    {
                    //        SSlStream.Read(RecBuf, 0, m_MaxPacket);
                    //        if (DebugFlag)
                    //        {
                    //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 存在未接收的数据包，接收...");
                    //        }
                    //    }
                    //}
                    #endregion 检查是否有未接收的接收返回数据

                    #region 发送I还原指令
                    lock (SSlStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        SSlStream.Write(ibuf, 0, ibuf.Length);
                        SSlStream.Flush();

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送I指令...");
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送I指令数据包：", ibuf, ibuf.Length);
                        }

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收I指令后的返回结果数据包...");
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收I指令返回数据包：", RecBuf, RecCount);
                        }

                        #region 判断I指令的返回值，避免指令结果错乱
                        string tmpIResult = "";
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount > 23)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount > 21)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        //如果返回不为空并且不包含no pnr，则继续接收返回结果
                        if ((tmpIResult.Trim() != "") && (tmpIResult.ToLower().IndexOf("no pnr") == -1)
                            && (tmpIResult.ToUpper().IndexOf("TRANSACTION IN PROGRESS") == -1)
                            && (tmpIResult.Trim().ToLower() != "s") && (tmpIResult.Trim().ToLower() != "s"))
                        {
                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 继续接收I指令后的返回结果数据包...");
                            }

                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 继续接收到I指令返回数据包：", RecBuf, RecCount);
                            }
                        }
                        #endregion 判断I指令的返回值，避免指令结果错乱
                        //}

                        #region 屏蔽更改为上面的While处理
                        //m_WhileCount = 0;
                        //while (RecCount < 23)
                        //{
                        //    if ((RecCount == 0) || (m_WhileCount >= m_MaxWhileCount))
                        //    {
                        //        if (EtermType == "地址认证")
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：I" +
                        //                " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }
                        //        else
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：I" +
                        //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }

                        //        if (NetStream != null)
                        //        {
                        //            NetStream.Close();
                        //        }
                        //        NetStream = null;
                        //        IdleFlag = 4;
                        //        ErrorMessage = "发送I指令后接收返回信息异常";
                        //        return false;
                        //    }

                        //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //    m_WhileCount++;
                        //}
                        #endregion 屏蔽更改为上面的While处理
                    }

                    #endregion 发送I还原指令

                    //指令间隔时间
                    if (this.Interval > 0)
                    {
                        Thread.Sleep(this.Interval);
                    }

                    //分解指令"|"，并
                    string[] sl = sendcmd.Split('|');

                    //返回结果内容
                    string tmpcontent = "";

                    #region 循环发送指令并获取返回结果
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (sl[i].Trim() == "")
                            continue;

                        tmpcmd = sl[i];
                        short tmplen = -1;
                        short count2 = -1;
                        byte[] lenbuf = null;
                        ArrayList al = new ArrayList();

                        //指令报头
                        byte[] head = new byte[19];
                        //指令报尾
                        byte[] end = new byte[2];
                        //指令内容
                        byte[] contentbuf = null;
                        //取得待发送的指令数据包

                        //网站用户转换汉字编码及拼音
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            AnalyseWebCmdAndMakeServerInfo(tmpcmd, out contentbuf, true);
                        }
                        else
                        {
                            contentbuf = Encoding.Default.GetBytes(tmpcmd);
                        }

                        //contentbuf = Encoding.Default.GetBytes(tmpcmd);

                        //指令报头
                        Array.Copy(CmdBuf, 0, head, 0, 19);
                        //指令报尾
                        Array.Copy(CmdBuf, CmdBuf.Length - 2, end, 0, 2);
                        //替换指令报头标志
                        Array.Copy(m_cmdHeadBuf, 0, head, 8, 2);

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + tmpcmd + "...");
                        }

                        //替换数据包长度字节
                        tmplen = (short)(head.Length + end.Length + contentbuf.Length);
                        count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                        lenbuf = BitConverter.GetBytes(count2);
                        Array.Copy(lenbuf, 0, head, 2, 2);
                        al.Clear();
                        al.AddRange(head);
                        al.AddRange(contentbuf);
                        al.AddRange(end);

                        byte[] tmpcmdbuf = (byte[])(al.ToArray(typeof(byte)));

                        lock (SSlStream)
                        {
                            //如果缓冲区有内容，则继续读取
                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //    SSlStream.Read(RecBuf, 0, m_MaxPacket);
                            //    if (DebugFlag)
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 缓存区存在未接收的数据包，接收...");
                            //    }
                            //}

                            #region 发送指令前先检查配置状态
                            if (this.IdleFlag != 1)
                            {
                                //不是忙碌状态则退出

                                ErrorCount++;

                                Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                ErrorMessage = "未找到可用配置...";
                                return false;
                            }
                            #endregion 发送指令前先检查配置状态

                            //发送指令
                            SSlStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                            SSlStream.Flush();

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd + "...");
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", tmpcmdbuf, tmpcmdbuf.Length);
                            }

                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //接收返回结果
                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }

                            while (RecCount < 21)
                            {
                                #region 检查连接是否断开
                                if (RecCount == 0)
                                {
                                    IdleFlag = 4;

                                    ErrorCount++;

                                    Log.Record("server.log", "发送指令：" + tmpcmd + "接收返回信息失败，可能通讯断开...");

                                    ErrorMessage = "发送指令：" + tmpcmd + "接收返回信息失败，可能通讯断开...";
                                    return false;
                                }
                                #endregion 检查连接是否断开

                                if (DebugFlag)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                }

                                RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                        tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                                continue;
                            }

                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            {
                                if (RecCount > 23)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                }
                            }
                            else
                            {
                                if (RecCount > 21)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                }
                            }

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果：" + tmpcontent);
                            }
                            //}

                            //指令间隔时间
                            if (this.Interval > 0)
                            {
                                Thread.Sleep(this.Interval);
                            }

                            //接收返回异常，设置重新进行配置认证标志
                            //if (tmpcontent.Trim().ToUpper() == "S")
                            //{
                            //    ResetFlag = true;
                            //}

                            #region 屏蔽，更改为上面的while处理
                            //int m_whilecount = 0;
                            //while (RecCount < 23)
                            //{
                            //    if ((RecCount == 0) || (m_whilecount >= m_MaxWhileCount))
                            //    {
                            //        if (EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", "配置:"+OriginalConfigManage.Original_LocalIp+" 发送指令："+tmpcmd+
                            //                " 出错；收到返回长度："+RecCount.ToString()+" 接收次数："+m_whilecount.ToString()+"；最大接收次数限制："+m_MaxWhileCount.ToString());
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_whilecount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                            //        }

                            //        if (NetStream != null)
                            //        {
                            //            NetStream.Close();
                            //        }
                            //        NetStream = null;
                            //        IdleFlag = 4;
                            //        ErrorMessage = "发送指令："+tmpcmd+" 获取航信返回信息出错！";
                            //        return false;
                            //    }

                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    m_whilecount++;
                            //}    
                            #endregion 屏蔽，更改为上面的while处理


                            #region 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端
                            ////Kevin 2010-12-07 Add
                            ////如果指令返回结果为S，则把配置设置为未连接
                            //if (tmpcontent.ToLower().Trim() == "s")
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：" + tmpcmd +
                            //            " 出错；收到返回s；返回结果异常...");
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //           " 出错；收到返回s；返回结果异常...");
                            //    }


                            //    if (NetStream != null)
                            //    {
                            //        NetStream.Close();
                            //    }
                            //    NetStream = null;
                            //    IdleFlag = 4;
                            //    ErrorMessage = "航信返回结果异常";
                            //    return false;
                            //}
                            #endregion 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端


                            #region 屏蔽，尽量与原始返回一致，不做特殊处理
                            ////Kevin 2010-06-15 Add
                            ////如果收到的返回结果是空，则继续接收返回信息
                            ////Kevin 2010-06-15 Add
                            ////如果SS:收到 NO PNR ，则继续接收返回信息
                            //while (((tmpcmd.Trim() != "\\") && (tmpcmd.ToUpper().IndexOf("SFC:") == -1) && (tmpcontent.Trim() == "")) || ((tmpcmd.ToUpper().IndexOf("SS:") != -1) && (tmpcontent.ToUpper().IndexOf("NO PNR") != -1)))
                            //{
                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //    }
                            //}
                            #endregion 屏蔽，尽量与原始返回一致，不做特殊处理


                            #region 判断是否需要签入工作号，如果需要则自动签入并重发指令
                            //判断是否需要签入
                            if (IfNeedSignIn(tmpcontent))
                            {
                                //重置接收结果数据
                                tmpcontent = "";
                                if (DebugFlag)
                                {
                                    if (this.EtermType == "地址认证")
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                }

                                #region 发送指令前先检查配置状态
                                if (this.IdleFlag != 1)
                                {
                                    //不是忙碌状态则退出

                                    ErrorCount++;

                                    Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                    ErrorMessage = "未找到可用配置...";
                                    return false;
                                }
                                #endregion 发送指令前先检查配置状态

                                //重发指令
                                SSlStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                                SSlStream.Flush();


                                //while (NetStream.DataAvailable)
                                //while(NetClient.Available>0)
                                //{
                                //接收返回结果
                                RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                        tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                while (RecCount < 21)
                                {
                                    #region 检查连接是否断开
                                    if (RecCount == 0)
                                    {
                                        IdleFlag = 4;

                                        ErrorCount++;

                                        Log.Record("server.log", "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...");

                                        ErrorMessage = "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...";
                                        return false;
                                    }
                                    #endregion 检查连接是否断开

                                    if (DebugFlag)
                                    {
                                        if (this.EtermType == "地址认证")
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                    }

                                    RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                    if (RecCount != 0)
                                    {
                                        int tmpRecCount = RecCount;

                                        byte[] RecBuf2 = new byte[m_MaxPacket];

                                        //数据包中长度
                                        int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                        //未接收完毕，继续接收数据包
                                        while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                            tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                            if (tmpRecCount != 0)
                                            {
                                                //合并数据包
                                                Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                            }

                                            RecCount += tmpRecCount;
                                        }
                                    }

                                    if (DebugDataFlag)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                    }

                                    continue;
                                }

                                if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                {
                                    if (RecCount > 23)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                    }
                                }
                                else
                                {
                                    if (RecCount > 21)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                    }
                                }
                                //}

                                //指令间隔时间
                                if (this.Interval > 0)
                                {
                                    Thread.Sleep(this.Interval);
                                }
                            }
                            #endregion 判断是否需要签入工作号，如果需要则自动签入并重发指令

                            //if (DebugFlag)
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //}
                        }


                        //最后一个指令结果
                        if (i == sl.Length - 1)
                        {
                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 组织客户端返回结果：" + tmpcontent);
                            }

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            //替换数据包长度信息
                            tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));

                            break;
                        }
                    }
                    #endregion 循环发送指令并获取返回结果

                    #region 发送封口指令信息
                    //系统自动封口
                    //封口返回结果为空或者NO PNR，则返回封口之前的结果信息
                    //否则返回封口后的结果信息
                    lock (SSlStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        //网站用户发送I指令
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            //发送I指令
                            SSlStream.Write(ibuf, 0, ibuf.Length);
                            SSlStream.Flush();
                        }
                        //客户端用户发送封口指令
                        else
                        {
                            //发送封口指令
                            SSlStream.Write(fengkouBuf, 0, fengkouBuf.Length);
                            SSlStream.Flush();
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送封口指令...");
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送封口数据包：", fengkouBuf, fengkouBuf.Length);
                        }

                        //把返回结果字符串置空
                        string tmpcontent2 = "";

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        //接收返回数据包
                        RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                        }

                        while (RecCount < 21)
                        {
                            #region 检查连接是否断开
                            if (RecCount == 0)
                            {
                                IdleFlag = 4;

                                ErrorCount++;

                                Log.Record("server.log", "发送封口指令接收返回信息失败，可能通讯断开...");

                                ErrorMessage = "发送封口指令接收返回信息失败，可能通讯断开...";
                                return false;
                            }
                            #endregion 检查连接是否断开

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收到返回数据包长度：" + RecCount.ToString() + "，继续接收...");
                            }

                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }

                            continue;
                        }

                        //Kevin 2011-04-28 Add
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount >= 23)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount >= 21)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }
                        //}

                        //指令间隔时间
                        if (this.Interval > 0)
                        {
                            Thread.Sleep(this.Interval);
                        }

                        //接收返回异常，设置重新进行配置认证标志
                        //if (tmpcontent2.Trim().ToUpper() == "S")
                        //{
                        //    ResetFlag = true;
                        //}

                        //如果返回结果不包含NO PNR 并且 返回结果不为空，则返回此结果信息
                        if ((tmpcontent2.ToUpper().IndexOf("NO PNR") == -1) && (tmpcontent2.Trim() != "") 
                            && (tmpcontent2.ToUpper().IndexOf("CHECK") == -1)&&(sendcmd.ToUpper().IndexOf("ETDZ ")==-1))
                        {
                            //如果需要返回所有指令结果，则把结果累加
                            if (allFlag)
                            {
                                tmpcontent += tmpcontent2;
                            }
                            //如果只返回最后一个指令的结果，则把当前封口指令的结果返回
                            else
                            {
                                tmpcontent = tmpcontent2;
                            }

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 向客户端返回结果：" + tmpcontent);
                            }

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            //替换数据包长度信息
                            short tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            byte[] lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.Clear();
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));
                        }
                    }
                    #endregion 发送封口指令信息
                }
                #endregion 其他指令处理

                //指令数量统计，用于配置均衡
                this.CommandCount++;

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                if (ResetFlag)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "接收到异常返回结果，需要重新进行配置认证...");
                    }
                    //设置异常标志，进行配置重新认证
                    IdleFlag = 4;
                }
                else
                {
                    //配置设置为空闲
                    IdleFlag = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "发送指令：" + cmd + "，处理出错，错误信息：" + ex.Message);
                IdleFlag = 4;
                ErrorMessage = ex.Message;
                return false;
            }
        }
        #endregion 发送指令并获取返回结果信息

        #region 发送指令自动PN并获取返回结果信息
        /// <summary>
        /// 发送指令自动根据最后一个指令进行PN并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetAllPnResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, ref byte[] ResBuf, ref string ErrorMessage)
        {
            ResBuf = null;
            ErrorMessage = "";

            //配置重新认证标志
            bool ResetFlag = false;

            try
            {
                //I指令数据包
                byte[] ibuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x69, 0x20, 0x03 };
                //封口指令数据包
                byte[] fengkouBuf = { 0x01, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E, 0x5C, 0x20, 0x03 };
                //PN指令数据包
                byte[] tmpPnBuf = { 0x1, 0x0, 0x0, 0x17, 0x0, 0x0, 0x0, 0x1, 0x41, 0x51, 0x70, 0x2, 0x1B, 0x0B, 0x22, 0x20, 0x0, 0xF, 0x1E, 0x70, 0x6E, 0x20, 0x3 };

                //回复信息的报头和报尾
                byte[] reshead = { 0x01, 0x00, 0x01, 0xAE, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x21, 0x20, 0x0F, 0x1B, 0x4D };
                byte[] resend = { 0x0D, 0x1E, 0x1B, 0x62, 0x03 };

                ArrayList al2 = new ArrayList();

                //行程单打印指令直接发送
                #region 行程单打印指令处理
                //byte[] tmpprintbuf = new byte[5];
                //Array.Copy(CmdBuf, 12, tmpprintbuf, 0, 5);
                //if (((cmd.ToLower() == "prinv") && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "prinv")) ||
                //    ((cmd.ToLower() == "vtinv") && (Encoding.Default.GetString(tmpprintbuf).ToLower() == "vtinv")))
                //{
                //    lock (SSlStream)
                //    {
                //        //发送指令
                //        SSlStream.Write(CmdBuf, 0, CmdBuf.Length);
                //        SSlStream.Flush();

                //        if (DebugFlag)
                //        {
                //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送行程单指令...");
                //        }

                //        if (DebugDataFlag)
                //        {
                //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送行程单数据包：", CmdBuf, CmdBuf.Length);
                //        }

                //        //while (NetStream.DataAvailable)
                //        //while(NetClient.Available>0)
                //        //{
                //        //接收返回信息
                //        byte[] tmpbuf = new byte[m_MaxPacket];
                //        int RecCount = 0;
                //        RecCount = SSlStream.Read(tmpbuf, 0, m_MaxPacket);

                //        if (DebugFlag)
                //        {
                //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包...");
                //        }

                //        if (DebugDataFlag)
                //        {
                //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", tmpbuf, RecCount);
                //        }

                //        //返回成功
                //        ResBuf = new byte[RecCount];
                //        Array.Copy(tmpbuf, ResBuf, RecCount);
                //        //    break;
                //        //}
                //    }
                //}
                #endregion 行程单打印指令处理
                #region 其他指令处理
                //else
                {
                    int RecCount = -1;
                    byte[] RecBuf = new byte[m_MaxPacket];

                    //int m_WhileCount = 0;

                    //把指令的黑屏报头信息替换
                    Array.Copy(m_cmdHeadBuf, 0, CmdBuf, 8, 2);

                    //替换i指令报头
                    Array.Copy(m_cmdHeadBuf, 0, ibuf, 8, 2);

                    //替换封口指令的报头
                    Array.Copy(m_cmdHeadBuf, 0, fengkouBuf, 8, 2);

                    //替换PN指令报头
                    Array.Copy(m_cmdHeadBuf, 0, tmpPnBuf, 8, 2);


                    //判断最后一个字符是否为封口符号
                    string tmpcmd = cmd.Trim();

                    //指令最后封口标志
                    //bool fengkouFlag = false;
                    //if ((tmpcmd[tmpcmd.Length - 1] == '@') || (tmpcmd[tmpcmd.Length - 1] == '\\'))
                    //{
                    //    fengkouFlag = true;
                    //}

                    //从数据包中获取发送指令
                    byte[] SendCmdBuf = new byte[CmdBuf.Length - 21];
                    Array.Copy(CmdBuf, 19, SendCmdBuf, 0, SendCmdBuf.Length);
                    string sendcmd = Encoding.Default.GetString(SendCmdBuf);

                    if (DebugFlag)
                    {
                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + sendcmd + "...");
                    }

                    #region 检查是否有未接收的接收返回数据
                    //lock (SSlStream)
                    //{
                    //    //while (NetStream.DataAvailable)
                    //    while(NetClient.Available>0)
                    //    {
                    //        SSlStream.Read(RecBuf, 0, m_MaxPacket);
                    //        if (DebugFlag)
                    //        {
                    //            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 存在未接收的返回数据包，接收...");
                    //        }
                    //    }
                    //}
                    #endregion 检查是否有未接收的接收返回数据

                    #region 发送I还原指令
                    lock (SSlStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        SSlStream.Write(ibuf, 0, ibuf.Length);
                        SSlStream.Flush();

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送I指令...");
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送I指令数据包：", ibuf, ibuf.Length);
                        }

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收I指令后的返回结果数据包...");
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                        }

                        #region 判断I指令的返回值，避免指令结果错乱
                        string tmpIResult = "";
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount > 23)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount > 21)
                            {
                                tmpIResult = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        //如果返回不为空并且不包含no pnr，则继续接收返回结果
                        if ((tmpIResult.Trim() != "") && (tmpIResult.ToLower().IndexOf("no pnr") == -1)
                            && (tmpIResult.ToUpper().IndexOf("TRANSACTION IN PROGRESS") == -1)
                            && (tmpIResult.Trim().ToLower() != "s") && (tmpIResult.Trim().ToLower() != "s"))
                        {
                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 继续接收I指令后的返回结果数据包...");
                            }

                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 继续接收到I指令返回数据包：", RecBuf, RecCount);
                            }
                        }
                        #endregion 判断I指令的返回值，避免指令结果错乱

                        //}

                        #region 屏蔽更改为上面的While处理
                        //m_WhileCount = 0;
                        //while (RecCount < 23)
                        //{
                        //    if ((RecCount == 0) || (m_WhileCount >= m_MaxWhileCount))
                        //    {
                        //        if (EtermType == "地址认证")
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：I" +
                        //                " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }
                        //        else
                        //        {
                        //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：I" +
                        //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_WhileCount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                        //        }

                        //        if (NetStream != null)
                        //        {
                        //            NetStream.Close();
                        //        }
                        //        NetStream = null;
                        //        IdleFlag = 4;
                        //        ErrorMessage = "发送I指令后接收返回信息异常";
                        //        return false;
                        //    }

                        //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                        //    m_WhileCount++;
                        //}
                        #endregion 屏蔽更改为上面的While处理
                    }

                    #endregion 发送I还原指令

                    //指令间隔时间
                    if (this.Interval > 0)
                    {
                        Thread.Sleep(this.Interval);
                    }

                    //分解指令"|"，并
                    string[] sl = sendcmd.Split('|');

                    //返回结果内容
                    string tmpcontent = "";

                    //最后一个指令的返回结果，用于判断是否翻页结束
                    string tmplastcontent = "";

                    #region 循环发送指令并获取返回结果
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (sl[i].Trim() == "")
                            continue;

                        tmpcmd = sl[i];
                        short tmplen = -1;
                        short count2 = -1;
                        byte[] lenbuf = null;
                        ArrayList al = new ArrayList();

                        //指令报头
                        byte[] head = new byte[19];
                        //指令报尾
                        byte[] end = new byte[2];
                        //指令内容
                        byte[] contentbuf = null;
                        //取得待发送的指令数据包

                        //网站用户转换汉字编码及拼音
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            AnalyseWebCmdAndMakeServerInfo(tmpcmd, out contentbuf, true);
                        }
                        else
                        {
                            contentbuf = Encoding.Default.GetBytes(tmpcmd);
                        }

                        //contentbuf = Encoding.Default.GetBytes(tmpcmd);

                        //指令报头
                        Array.Copy(CmdBuf, 0, head, 0, 19);
                        //指令报尾
                        Array.Copy(CmdBuf, CmdBuf.Length - 2, end, 0, 2);
                        //替换指令报头标志
                        Array.Copy(m_cmdHeadBuf, 0, head, 8, 2);

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 准备发送指令：" + tmpcmd + "...");
                        }

                        //替换数据包长度字节
                        tmplen = (short)(head.Length + end.Length + contentbuf.Length);
                        count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                        lenbuf = BitConverter.GetBytes(count2);
                        Array.Copy(lenbuf, 0, head, 2, 2);
                        al.Clear();
                        al.AddRange(head);
                        al.AddRange(contentbuf);
                        al.AddRange(end);

                        byte[] tmpcmdbuf = (byte[])(al.ToArray(typeof(byte)));

                        lock (SSlStream)
                        {
                            //如果缓冲区有内容，则继续读取
                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //    SSlStream.Read(RecBuf, 0, m_MaxPacket);
                            //    if (DebugFlag)
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 缓存区存在未接收的数据包，接收...");
                            //    }
                            //}

                            #region 发送指令前先检查配置状态
                            if (this.IdleFlag != 1)
                            {
                                //不是忙碌状态则退出

                                ErrorCount++;

                                Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                ErrorMessage = "未找到可用配置...";
                                return false;
                            }
                            #endregion 发送指令前先检查配置状态

                            //发送指令
                            SSlStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                            SSlStream.Flush();

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送指令" + tmpcmd + "...");
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送数据包：", tmpcmdbuf, tmpcmdbuf.Length);
                            }

                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //接收返回结果
                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }

                            while (RecCount < 21)
                            {
                                #region 检查连接是否断开
                                if (RecCount == 0)
                                {
                                    IdleFlag = 4;

                                    ErrorCount++;

                                    Log.Record("server.log", "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...");

                                    ErrorMessage = "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...";
                                    return false;
                                }
                                #endregion 检查连接是否断开

                                if (DebugFlag)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "，继续接收...");
                                }

                                RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                        tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                                continue;
                            }

                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            {
                                if (RecCount > 23)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                    tmplastcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                }
                            }
                            else
                            {
                                if (RecCount > 21)
                                {
                                    if (allFlag)
                                    {
                                        tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                    else
                                    {
                                        tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                    tmplastcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                }
                            }

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果：" + tmpcontent);
                            }
                            //}

                            //指令间隔时间
                            if (this.Interval > 0)
                            {
                                Thread.Sleep(this.Interval);
                            }

                            //if (tmpcontent.Trim().ToUpper() == "S")
                            //{
                            //    ResetFlag = true;
                            //}

                            #region 屏蔽，更改为上面的while处理
                            //int m_whilecount = 0;
                            //while (RecCount < 23)
                            //{
                            //    if ((RecCount == 0) || (m_whilecount >= m_MaxWhileCount))
                            //    {
                            //        if (EtermType == "地址认证")
                            //        {
                            //            Log.Record("server.log", "配置:"+OriginalConfigManage.Original_LocalIp+" 发送指令："+tmpcmd+
                            //                " 出错；收到返回长度："+RecCount.ToString()+" 接收次数："+m_whilecount.ToString()+"；最大接收次数限制："+m_MaxWhileCount.ToString());
                            //        }
                            //        else
                            //        {
                            //            Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //               " 出错；收到返回长度：" + RecCount.ToString() + " 接收次数：" + m_whilecount.ToString() + "；最大接收次数限制：" + m_MaxWhileCount.ToString());
                            //        }

                            //        if (NetStream != null)
                            //        {
                            //            NetStream.Close();
                            //        }
                            //        NetStream = null;
                            //        IdleFlag = 4;
                            //        ErrorMessage = "发送指令："+tmpcmd+" 获取航信返回信息出错！";
                            //        return false;
                            //    }

                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    m_whilecount++;
                            //}    
                            #endregion 屏蔽，更改为上面的while处理


                            #region 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端
                            ////Kevin 2010-12-07 Add
                            ////如果指令返回结果为S，则把配置设置为未连接
                            //if (tmpcontent.ToLower().Trim() == "s")
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_LocalIp + " 发送指令：" + tmpcmd +
                            //            " 出错；收到返回s；返回结果异常...");
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置:" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + tmpcmd +
                            //           " 出错；收到返回s；返回结果异常...");
                            //    }


                            //    if (NetStream != null)
                            //    {
                            //        NetStream.Close();
                            //    }
                            //    NetStream = null;
                            //    IdleFlag = 4;
                            //    ErrorMessage = "航信返回结果异常";
                            //    return false;
                            //}
                            #endregion 屏蔽，配置返回异常不做处理，直接把返回结果返回到前端


                            #region 屏蔽，尽量与原始返回一致，不做特殊处理
                            ////Kevin 2010-06-15 Add
                            ////如果收到的返回结果是空，则继续接收返回信息
                            ////Kevin 2010-06-15 Add
                            ////如果SS:收到 NO PNR ，则继续接收返回信息
                            //while (((tmpcmd.Trim() != "\\") && (tmpcmd.ToUpper().IndexOf("SFC:") == -1) && (tmpcontent.Trim() == "")) || ((tmpcmd.ToUpper().IndexOf("SS:") != -1) && (tmpcontent.ToUpper().IndexOf("NO PNR") != -1)))
                            //{
                            //    RecCount = NetStream.Read(RecBuf, 0, m_MaxPacket);

                            //    if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (allFlag)
                            //        {
                            //            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //        else
                            //        {
                            //            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            //        }
                            //    }
                            //}
                            #endregion 屏蔽，尽量与原始返回一致，不做特殊处理


                            #region 判断是否需要签入工作号，如果需要则自动签入并重发指令
                            //判断是否需要签入
                            if (IfNeedSignIn(tmpcontent))
                            {
                                //重置接收结果数据
                                tmpcontent = "";
                                if (DebugFlag)
                                {
                                    if (this.EtermType == "地址认证")
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                    else
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 重新签入工作号后，重置接收返回结果信息...");
                                    }
                                }

                                #region 发送指令前先检查配置状态
                                if (this.IdleFlag != 1)
                                {
                                    //不是忙碌状态则退出

                                    ErrorCount++;

                                    Log.Record("server.log", "配置状态异常，可能通讯断开...");

                                    ErrorMessage = "未找到可用配置...";
                                    return false;
                                }
                                #endregion 发送指令前先检查配置状态

                                //重发指令
                                SSlStream.Write(tmpcmdbuf, 0, tmpcmdbuf.Length);
                                SSlStream.Flush();


                                //while (NetStream.DataAvailable)
                                //while(NetClient.Available>0)
                                //{
                                //接收返回结果
                                RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                        tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                while (RecCount < 21)
                                {
                                    #region 检查连接是否断开
                                    if (RecCount == 0)
                                    {
                                        IdleFlag = 4;

                                        ErrorCount++;

                                        Log.Record("server.log", "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...");

                                        ErrorMessage = "发送指令：" + tmpcmd + " 接收返回信息失败，可能通讯断开...";
                                        return false;
                                    }
                                    #endregion 检查连接是否断开

                                    if (DebugFlag)
                                    {
                                        if (this.EtermType == "地址认证")
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                        else
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "...");
                                        }
                                    }

                                    RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                    //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                    if (RecCount != 0)
                                    {
                                        int tmpRecCount = RecCount;

                                        byte[] RecBuf2 = new byte[m_MaxPacket];

                                        //数据包中长度
                                        int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                        //未接收完毕，继续接收数据包
                                        while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                        {
                                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                            tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                            if (tmpRecCount != 0)
                                            {
                                                //合并数据包
                                                Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                            }

                                            RecCount += tmpRecCount;
                                        }
                                    }

                                    if (DebugDataFlag)
                                    {
                                        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                    }

                                    continue;
                                }

                                if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                {
                                    if (RecCount > 23)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                        }
                                        tmplastcontent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                    }
                                }
                                else
                                {
                                    if (RecCount > 21)
                                    {
                                        if (allFlag)
                                        {
                                            tmpcontent += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                        else
                                        {
                                            tmpcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                        }
                                        tmplastcontent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                    }
                                }
                                //}

                                //指令间隔时间
                                if (this.Interval > 0)
                                {
                                    Thread.Sleep(this.Interval);
                                }
                            }
                            #endregion 判断是否需要签入工作号，如果需要则自动签入并重发指令

                            //if (DebugFlag)
                            //{
                            //    if (EtermType == "地址认证")
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "收到航信返回信息：" + tmpcontent);
                            //    }
                            //}
                        }

                        //是否为航班查询指令
                        //由于航班查询指令PN会直接翻天，因此需要对于航班查询自动PN的结果日期进行专门处理
                        bool avflag = false;
                        //判断是否为av 或者 avh 指令
                        if (cmd.Trim().ToLower().Substring(0, 2) == "av")
                        {
                            avflag = true;
                        }

                        //最后一个指令结果
                        if (i == sl.Length - 1)
                        {
                            //PN返回结果信息，用于结果比较
                            string tmpPnContent = "";

                            //最后一个指令自动发送PN指令直到尾页
                            SSlStream.Write(tmpPnBuf, 0, tmpPnBuf.Length);
                            SSlStream.Flush();

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送PN指令...");
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送PN数据包：", tmpPnBuf, tmpPnBuf.Length);
                            }

                            //while (NetStream.DataAvailable)
                            //while(NetClient.Available>0)
                            //{
                            //读取返回结果信息
                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果...");
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }

                            #region 数据包长度不足，则继续循环接收
                            while (RecCount < 21)
                            {
                                #region 检查连接是否断开
                                if (RecCount == 0)
                                {
                                    IdleFlag = 4;

                                    ErrorCount++;

                                    Log.Record("server.log", "发送PN指令接收返回信息失败，可能通讯断开...");

                                    ErrorMessage = "发送PN指令接收返回信息失败，可能通讯断开...";
                                    return false;
                                }
                                #endregion 检查连接是否断开

                                if (DebugFlag)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "，继续接收...");
                                }

                                RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                if (RecCount != 0)
                                {
                                    int tmpRecCount = RecCount;

                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                    //数据包中长度
                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                    //未接收完毕，继续接收数据包
                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                        tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                        if (tmpRecCount != 0)
                                        {
                                            //合并数据包
                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                        }

                                        RecCount += tmpRecCount;
                                    }
                                }

                                if (DebugDataFlag)
                                {
                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                }
                                continue;
                            }
                            #endregion 数据包长度不足，则继续循环接收

                            tmpPnContent = "";
                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                            {
                                if (RecCount > 23)
                                {
                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                }
                            }
                            else
                            {
                                if (RecCount > 21)
                                {
                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                }
                            }

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收结果：" + tmpPnContent);
                            }
                            //}

                            //判断是否需要签入工作号
                            if (!IfNeedSignIn(tmpPnContent))
                            {
                                //指令间隔时间
                                if (this.Interval > 0)
                                {
                                    Thread.Sleep(this.Interval);
                                }

                                //上一次PN结果，用于结果对比
                                string prePnContent = tmplastcontent;
                                //如果是AV 或 AVH指令，则进行专门比较
                                #region AV或AVH 的PN结果判断处理
                                if (avflag)
                                {
                                    //没有结束，继续PN循环
                                    while (!IfAvhEnd(cmd, tmpPnContent))
                                    {
                                        tmpcontent += tmpPnContent;

                                        //发送PN指令
                                        SSlStream.Write(tmpPnBuf, 0, tmpPnBuf.Length);
                                        SSlStream.Flush();

                                        if (DebugFlag)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送PN指令...");
                                        }

                                        if (DebugDataFlag)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送PN数据包：", tmpPnBuf, tmpPnBuf.Length);
                                        }

                                        //while (NetStream.DataAvailable)
                                        //while(NetClient.Available>0)
                                        //{
                                        //读取返回结果信息
                                        RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                        if (RecCount != 0)
                                        {
                                            int tmpRecCount = RecCount;

                                            byte[] RecBuf2 = new byte[m_MaxPacket];

                                            //数据包中长度
                                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                            //未接收完毕，继续接收数据包
                                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                            {
                                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                                tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                                if (tmpRecCount != 0)
                                                {
                                                    //合并数据包
                                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                }

                                                RecCount += tmpRecCount;
                                            }
                                        }

                                        if (DebugFlag)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果...");
                                        }

                                        if (DebugDataFlag)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                        }

                                        #region 数据包长度不足，则继续循环接收
                                        while (RecCount < 21)
                                        {
                                            #region 检查连接是否断开
                                            if (RecCount == 0)
                                            {
                                                IdleFlag = 4;

                                                ErrorCount++;

                                                Log.Record("server.log", "发送PN指令接收返回信息失败，可能通讯断开...");

                                                ErrorMessage = "发送PN指令接收返回信息失败，可能通讯断开...";
                                                return false;
                                            }
                                            #endregion 检查连接是否断开

                                            if (DebugFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "，继续接收...");
                                            }

                                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                            if (RecCount != 0)
                                            {
                                                int tmpRecCount = RecCount;

                                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                                //数据包中长度
                                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                                //未接收完毕，继续接收数据包
                                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                                    if (tmpRecCount != 0)
                                                    {
                                                        //合并数据包
                                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                    }

                                                    RecCount += tmpRecCount;
                                                }
                                            }

                                            if (DebugDataFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                            }
                                            continue;
                                        }
                                        #endregion 数据包长度不足，则继续循环接收

                                        tmpPnContent = "";
                                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                        {
                                            if (RecCount > 23)
                                            {
                                                tmpPnContent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                            }
                                        }
                                        else
                                        {
                                            if (RecCount > 21)
                                            {
                                                tmpPnContent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                            }
                                        }

                                        if (DebugFlag)
                                        {
                                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收结果：" + tmpPnContent);
                                        }
                                        //}

                                        //指令间隔时间
                                        if (this.Interval > 0)
                                        {
                                            Thread.Sleep(this.Interval);
                                        }
                                    }
                                }
                                #endregion AV或AVH 的PN结果判断处理

                                #region 其他指令的PN结果处理
                                //其他指令则以重复返回为结束标志
                                else
                                {
                                    ////判断pn的返回信息是否一致，如果一致则表示结束
                                    //循环判断，直至符合条件退出

                                    //PN计数
                                    int tmppncount = 1;
                                    //最大PN次数20次
                                    int tmpMaxPnCount = 20;
                                    while (true)
                                    {
                                        //比较PN结果与上一次结果是否一样，如果一样则认为结束
                                        if (prePnContent == tmpPnContent)
                                        {
                                            if (DebugFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + cmd + " 返回结果检查到与上次PN结果一致，认为PN结束...");
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            //累加收到的结果信息
                                            tmpcontent += tmpPnContent;

                                            prePnContent = tmpPnContent;
                                            tmpPnContent = "";

                                            //继续发送PN指令并接收返回
                                            //最后一个指令自动发送PN指令直到尾页
                                            SSlStream.Write(tmpPnBuf, 0, tmpPnBuf.Length);
                                            SSlStream.Flush();

                                            if (DebugFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送PN指令...");
                                            }

                                            if (DebugDataFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送PN数据包：", tmpPnBuf, tmpPnBuf.Length);
                                            }

                                            //while (NetStream.DataAvailable)
                                            //while(NetClient.Available>0)
                                            //{
                                            //读取返回结果信息
                                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                            if (RecCount != 0)
                                            {
                                                int tmpRecCount = RecCount;

                                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                                //数据包中长度
                                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                                //未接收完毕，继续接收数据包
                                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                                {
                                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                                    if (tmpRecCount != 0)
                                                    {
                                                        //合并数据包
                                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                    }

                                                    RecCount += tmpRecCount;
                                                }
                                            }

                                            if (DebugFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回结果...");
                                            }

                                            if (DebugDataFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                            }

                                            #region 数据包长度不足，则继续循环接收
                                            while (RecCount < 21)
                                            {
                                                #region 检查连接是否断开
                                                if (RecCount == 0)
                                                {
                                                    IdleFlag = 4;

                                                    ErrorCount++;

                                                    Log.Record("server.log", "发送PN指令接收返回信息失败，可能通讯断开...");

                                                    ErrorMessage = "发送PN指令接收返回信息失败，可能通讯断开...";
                                                    return false;
                                                }
                                                #endregion 检查连接是否断开

                                                if (DebugFlag)
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收返回数据包长度：" + RecCount.ToString() + "，继续接收...");
                                                }

                                                RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                                                //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                                                if (RecCount != 0)
                                                {
                                                    int tmpRecCount = RecCount;

                                                    byte[] RecBuf2 = new byte[m_MaxPacket];

                                                    //数据包中长度
                                                    int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                                    //未接收完毕，继续接收数据包
                                                    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                                    {
                                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                                        tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                                        if (tmpRecCount != 0)
                                                        {
                                                            //合并数据包
                                                            Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                                        }

                                                        RecCount += tmpRecCount;
                                                    }
                                                }

                                                if (DebugDataFlag)
                                                {
                                                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                                                }
                                                continue;
                                            }
                                            #endregion 数据包长度不足，则继续循环接收

                                            tmpPnContent = "";
                                            if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                                            {
                                                if (RecCount > 23)
                                                {
                                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                                                }
                                            }
                                            else
                                            {
                                                if (RecCount > 21)
                                                {
                                                    tmpPnContent = Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                                                }
                                            }

                                            if (DebugFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收结果：" + tmpPnContent);
                                            }
                                            //}
                                        }

                                        //判断是否超出了最大PN循环次数
                                        tmppncount += 1;
                                        if (tmppncount > tmpMaxPnCount)
                                        {
                                            if (DebugFlag)
                                            {
                                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送指令：" + cmd + " 翻页次数超过" + tmpMaxPnCount.ToString() + " 异常退出循环处理...");
                                            }
                                            break;
                                        }
                                        //m_MaxWhileCount

                                        //指令间隔时间
                                        if (this.Interval > 0)
                                        {
                                            Thread.Sleep(this.Interval);
                                        }
                                    }
                                }
                                #endregion 其他指令的PN结果处理
                            }

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 组织客户端返回结果：" + tmpcontent);
                            }

                            //替换数据包长度信息
                            tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));

                            break;
                        }
                    }
                    #endregion 循环发送指令并获取返回结果

                    #region 发送封口指令信息
                    //系统自动封口
                    //封口返回结果为空或者NO PNR，则返回封口之前的结果信息
                    //否则返回封口后的结果信息
                    lock (SSlStream)
                    {
                        #region 发送指令前先检查配置状态
                        if (this.IdleFlag != 1)
                        {
                            //不是忙碌状态则退出

                            ErrorCount++;

                            Log.Record("server.log", "配置状态异常，可能通讯断开...");

                            ErrorMessage = "未找到可用配置...";
                            return false;
                        }
                        #endregion 发送指令前先检查配置状态

                        //网站用户
                        if (username.ToLower() == WebUser.ToLower())
                        {
                            //发送I指令
                            SSlStream.Write(ibuf, 0, ibuf.Length);
                            SSlStream.Flush();
                        }
                        else
                        {
                            //发送封口指令
                            SSlStream.Write(fengkouBuf, 0, fengkouBuf.Length);
                            SSlStream.Flush();
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送封口指令...");
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 发送封口数据包：", fengkouBuf, fengkouBuf.Length);
                        }

                        //把返回结果字符串置空
                        string tmpcontent2 = "";

                        //while (NetStream.DataAvailable)
                        //while(NetClient.Available>0)
                        //{
                        //接收返回数据包
                        RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                        //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                        if (RecCount != 0)
                        {
                            int tmpRecCount = RecCount;

                            byte[] RecBuf2 = new byte[m_MaxPacket];

                            //数据包中长度
                            int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                            //未接收完毕，继续接收数据包
                            while ((tmpRecCount != 0) && (RecCount < RecCount2))
                            {
                                Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                if (tmpRecCount != 0)
                                {
                                    //合并数据包
                                    Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                }

                                RecCount += tmpRecCount;
                            }
                        }

                        if (DebugDataFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                        }


                        while (RecCount < 21)
                        {
                            #region 检查连接是否断开
                            if (RecCount == 0)
                            {
                                IdleFlag = 4;

                                ErrorCount++;

                                Log.Record("server.log", "发送封口指令接收返回信息失败，可能通讯断开...");

                                ErrorMessage = "发送封口指令接收返回信息失败，可能通讯断开...";
                                return false;
                            }
                            #endregion 检查连接是否断开

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 返回结果数据包长度：" + RecCount.ToString() + "，继续接收...");
                            }

                            RecCount = SSlStream.Read(RecBuf, 0, m_MaxPacket);

                            //Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                            if (RecCount != 0)
                            {
                                int tmpRecCount = RecCount;

                                byte[] RecBuf2 = new byte[m_MaxPacket];

                                //数据包中长度
                                int RecCount2 = Convert.ToInt32(RecBuf[2]) * 16 * 16 + Convert.ToInt32(RecBuf[3]);

                                //未接收完毕，继续接收数据包
                                while ((tmpRecCount != 0) && (RecCount < RecCount2))
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");

                                    tmpRecCount = SSlStream.Read(RecBuf2, 0, m_MaxPacket);

                                    if (tmpRecCount != 0)
                                    {
                                        //合并数据包
                                        Array.Copy(RecBuf2, 0, RecBuf, RecCount, tmpRecCount);
                                    }

                                    RecCount += tmpRecCount;
                                }
                            }

                            if (DebugDataFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收数据包：", RecBuf, RecCount);
                            }
                            continue;
                        }

                        //Kevin 2011-04-28 Add
                        if ((RecBuf[17] == 0x1B) && (RecBuf[18] == 0x4D))
                        {
                            if (RecCount >= 23)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 19, RecCount - 23);
                            }
                        }
                        else
                        {
                            if (RecCount >= 21)
                            {
                                tmpcontent2 += Encoding.Default.GetString(RecBuf, 17, RecCount - 21);
                            }
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 接收结果：" + tmpcontent2);
                        }
                        //}

                        //指令间隔时间
                        if (this.Interval > 0)
                        {
                            Thread.Sleep(this.Interval);
                        }

                        //if (tmpcontent2.Trim().ToUpper() == "S")
                        //{
                        //    ResetFlag = true;
                        //}

                        //如果返回结果不包含NO PNR 并且 返回结果不为空，则返回此结果信息
                        if ((tmpcontent2.ToUpper().IndexOf("NO PNR") == -1) && (tmpcontent2.Trim() != "") && (tmpcontent2.ToUpper().IndexOf("CHECK") == -1))
                        {
                            //如果需要返回所有指令结果，则把结果累加
                            if (allFlag)
                            {
                                tmpcontent += tmpcontent2;
                            }
                            //如果只返回最后一个指令的结果，则把当前封口指令的结果返回
                            //else
                            //{
                            //    tmpcontent = tmpcontent2;
                            //}

                            //组织返回信息
                            byte[] buf3 = Encoding.Default.GetBytes(tmpcontent);

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 返回客户端结果：" + tmpcontent);
                            }

                            //替换数据包长度信息
                            short tmplen = (short)(buf3.Length + reshead.Length + resend.Length);
                            short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                            byte[] lenbuf = BitConverter.GetBytes(count2);
                            Array.Copy(lenbuf, 0, reshead, 2, 2);
                            al2.Clear();
                            al2.AddRange(reshead);
                            al2.AddRange(buf3);
                            al2.AddRange(resend);

                            ResBuf = (byte[])(al2.ToArray(typeof(byte)));
                        }
                    }
                    #endregion 发送封口指令信息
                }
                #endregion 其他指令处理

                //指令数量统计，用于配置均衡
                this.CommandCount++;

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                if (ResetFlag)
                {
                    if (DebugFlag)
                    {
                        Log.Record("server.log", "接收到异常返回结果，需要重新进行配置认证...");
                    }

                    //设置配置异常，重新认证
                    IdleFlag = 4;
                }
                else
                {
                    //配置设置为空闲
                    IdleFlag = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "发送指令：" + cmd + "，处理出错，错误信息：" + ex.Message);
                IdleFlag = 4;
                ErrorMessage = ex.Message;
                return false;
            }
        }
        #endregion 发送指令自动PN并获取返回结果信息

        #region 检查Eterm是否已安装
        public bool CheckIfInstallEterm()
        {
            //把Eterm的配置信息更改为350非加密设置
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
            //检测是否安装了Eterm，并且把配置参数更改为 不使用安全传输的  350端口
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            RegistryKey pregkey;
            pregkey = Registry.CurrentUser.OpenSubKey("Software\\TravelSky\\eTerm3\\Settings\\eTermInfo", true);
            if (pregkey == null)
            {
                //未安装Eterm，请先安装
                return false;
            }
            return true;
        }
        #endregion 检查Eterm是否已安装

        #region 启动Eterm并更改为443加密传输
        public bool ChangeEtermPort443()
        {
            RegistryKey pregkey;
            pregkey = Registry.CurrentUser.OpenSubKey("Software\\TravelSky\\eTerm3\\Settings\\eTermInfo", true);

            if (pregkey == null)
            {
                return false;
            }

            byte[] tmpbyte = (byte[])pregkey.GetValue("eTermInfo");

            //倒数第八位（加密传输 标志）
            byte[] tmpSSL = { 0x00, 0x00 };
            Array.Copy(tmpSSL, 0, tmpbyte, tmpbyte.Length - 8, 1);

            //更改倒数第536位（端口）
            byte[] tmp350 = { 0x5e, 0x01 };
            Array.Copy(tmp350, 0, tmpbyte, tmpbyte.Length - 536, 2);

            //写回注册表
            pregkey.SetValue("eTermInfo", (object)tmpbyte);
            return true;
        }
        #endregion 启动Eterm并更改为443加密传输

        #region 加密证书调用
        //证书有效性直接返回成功
        private bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Console.WriteLine("获得证书...");
            return true;
        }
        #endregion 加密证书调用

        #region 判断是否需要签入
        /// <summary>
        /// 判断是否需要签入
        /// </summary>
        /// <param name="reccontent">指令返回信息</param>
        /// <returns></returns>
        private bool IfNeedSignIn(string reccontent)
        {
            ////Kevin 2009-09-05 Edit
            ////航空公司配置返回信息有可能换行符之前存在 0x1B, 0x62非法字符，去掉
            //byte[] Errorbuf = { 0x1B, 0x62 };
            //string strError = Encoding.Default.GetString(Errorbuf);
            //reccontent = reccontent.Replace(strError, "");
            //byte[] Errorbuf2 = { 0x1B, 0x0B };
            //string strError2 = Encoding.Default.GetString(Errorbuf2);
            //reccontent = reccontent.Replace(strError2, "");

            //PLEASE SIGN IN FIRST.
            //SI    
            string tmpstr = reccontent.Trim().ToLower();
            //航空公司配置，需要签入
            //Kevin 2011-03-24 调整si判断
            if (((tmpstr.Trim().Length > 1) && (tmpstr.Substring(0, 2) == "si"))
                || (tmpstr.IndexOf("please sign in first") != -1)
                || (tmpstr.IndexOf("signed out") != -1)
                || (tmpstr.ToLower() == "s"))
            {
                SignIn();
                return true;
            }
            return false;
        }
        #endregion 判断是否需要签入

        #region 判断 AV 或 AVH 结果是否已经结束（翻天为结束标志）
        /// <summary>
        /// 判断 AV 或 AVH 结果是否已经结束（翻天为结束标志）
        /// </summary>
        /// <param name="cmd">发送的指令信息</param>
        /// <param name="returnContent">返回的结果信息</param>
        /// <returns>true：已经结束；false：没有结束</returns>
        private bool IfAvhEnd(string cmd, string returnContent)
        {
            #region 分析av指令中的日期信息
            //分析av指令中的日期信息
            string tmpdate = cmd.Substring(11, 5);
            if ((cmd.Length > 3) && (cmd.ToLower().Substring(0, 3) == "av:"))
            {
                tmpdate = cmd.Substring(10, 5);
            }
            #endregion 分析av指令中的日期信息

            #region 分析结果中的日期信息
            //如果两个日期不一致，则返回空
            int pos1 = returnContent.IndexOf("(");
            int pos2 = returnContent.IndexOf(")");
            if ((pos1 == -1) || (pos2 == -1))
            {
                //如果解析结果出错则认为已经结束
                return true;
            }

            //当前信息的航班日期
            string curdate = returnContent.Substring(1, pos1 - 1);

            if (curdate.ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
            #endregion 分析结果中的日期信息
        }
        #endregion 判断 AV 或 AVH 结果是否已经结束（翻天为结束标志）
    }


    #region 获取F7数据
    /// <summary>
    /// 获取F7数据
    /// </summary>
    public class ThreadWork
    {
        static public byte[] F7 = new byte[4 + 0x80 + 0x80];
        static public bool newflag = false;
        static public byte[] F6 = null;
        static public object lockX = new object();
        static public object lockY = new object();
        static public TcpListener sslServer = null;
        static public bool closeFlag = false;//退出程序标志
        static public int shareport = 3510;

        public static void DoWork()
        {
            //Console.WriteLine("启动3510端口。。。。");                
            sslServer = new TcpListener(shareport);
            sslServer.Start();

            //lock (lockY)
            //{
            while (true)
            {
                TcpClient client = null;
                try
                {
                    client = sslServer.AcceptTcpClient();
                }
                catch (Exception)
                {
                    if (closeFlag)
                        break;
                    sslServer.Start();
                    client = sslServer.AcceptTcpClient();
                }

                try
                {
                    //Console.WriteLine("处理 eterm3.exe 连接...");

                    byte[] login = new byte[1024];
                    client.GetStream().Read(login, 0, 162);

                    if (closeFlag)
                    {
                        client.Close();
                        break;
                    }

                    //Log.Record("server1.log", "Eterm,Read", login, 162);

                    byte[] p1 = { 0x00, 0x0a, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x34, 0x51 };

                    client.GetStream().Write(p1, 0, p1.Length);
                    client.GetStream().Flush();

                    byte[] D = new byte[1024];
                    int rcount = client.GetStream().Read(D, 0, 1024);

                    // Log.Record("server1.log", "Eterm,Write", p1, p1.Length);

                    while (F6 == null)
                    {
                        if (closeFlag)
                            break;
                        Thread.Sleep(1000);
                        continue;
                    }

                    while (!newflag)
                    {
                        if (closeFlag)
                            break;
                        Thread.Sleep(1000);
                        continue;
                    }


                    client.GetStream().Write(F6, 0, F6.Length);
                    client.GetStream().Flush();

                    //Log.Record("server1.log", "Eterm,WriteF6", F6, F6.Length);

                    while (true)
                    {
                        if (closeFlag)
                            break;

                        byte[] D2 = new byte[1024];
                        int rcount2 = client.GetStream().Read(D2, 0, 1024);
                        if (rcount2 == 0)
                            continue;
                        //int nSize = D[2] * 256 + D[3] - 4;
                        //client.GetStream().Read(D, 4, nSize);

                        //Log.Record("server1.log", "Eterm,ReadF7", D, rcount);

                        if (D2[1] != 0xF7) continue;

                        Array.Copy(D2, 0, F7, 0, 4 + 0x80 + 0x80);

                        newflag = false;

                        break;
                    }
                    client.Close();

                    F6 = null;


                    //Console.WriteLine("获得 F7...");
                    //client.Close();
                }
                catch (Exception e)
                {
                    F6 = null;
                    newflag = false;
                    client.Close();
                    client = null;
                }
            }
            //}
        }
    }
    #endregion 获取F7数据



    /// <summary>
    /// 信天游处理类
    /// </summary>
    public class EtermXTYManage : BaseManage
    {
        //处理信天游登录认证
        public void DoWithXTY()
        {
            try
            {
                #region 信天游网站自动登录并获取黑屏登录信息

                #endregion 信天游网站自动登录并获取黑屏登录信息



                if (NetClient != null)
                {
                    NetClient = null;
                }

                NetClient = new TcpClient();
                NetClient.Connect(OriginalConfigManage.Original_Ip, OriginalConfigManage.Original_Port);


                if (DebugFlag)
                {
                    if (EtermType == "地址认证")
                    {
                        Log.Record("server.log", "配置（地址认证）：" + OriginalConfigManage.Original_LocalIp + " 正在准备连接航信服务器：" + OriginalConfigManage.Original_Ip + "，端口：" + OriginalConfigManage.Original_Port.ToString());
                    }
                    else
                    {
                        Log.Record("server.log", "配置（密码认证）：" + OriginalConfigManage.Original_ConfigName + " 正在准备连接航信服务器：" + OriginalConfigManage.Original_Ip + "，端口：" + OriginalConfigManage.Original_Port.ToString());
                    }
                }

                NetStream = NetClient.GetStream();

                //登入（350地址认证的包头）
                byte[] C = { 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x30, 0x32, 0x31, 0x38, 0x36, 0x35, 0x34, 0x66, 0x62, 0x30, 0x63, 0x31, 0x30, 0x2E, 0x31, 0x35, 0x30, 0x2E, 0x36, 0x36, 0x2E, 0x31, 0x39, 0x36, 0x20, 0x20, 0x33, 0x36, 0x31, 0x30, 0x34, 0x31, 0x30, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                //移动控制台标志
                bool dlanFlag = false;

                //地址认证
                if (EtermType == "地址认证")
                {
                    byte[] px = { 0x21, 0x21, 0x21, 0x21, 0x21, 0x21, 0x21, 0x21 };
                    Array.Copy(px, 0, C, 2, px.Length);
                    Array.Copy(px, 0, C, 18, px.Length);

                    //取得本机 10.150的IP地址
                    string dlanip = OriginalConfigManage.Original_LocalIp;

                    //替换IP地址
                    byte[] ipbuf = Encoding.Default.GetBytes(dlanip);
                    Array.Copy(ipbuf, 0, C, 62, ipbuf.Length);
                    dlanFlag = true;
                }
                else
                {
                    //帐号
                    byte[] px = ASCIIEncoding.Default.GetBytes(OriginalConfigManage.Original_ConfigName);
                    Array.Copy(px, 0, C, 2, px.Length);

                    //密码
                    byte[] px1 = ASCIIEncoding.Default.GetBytes(OriginalConfigManage.Original_ConfigPass);
                    Array.Copy(px1, 0, C, 18, px1.Length);
                }

                NetStream.Write(C, 0, C.Length);
                NetStream.Flush();


                byte[] D = new byte[1024];
                int jscount = 0;
                NetStream.Read(D, 0, 0x37);

                if (D[2] == 0x00) //error
                {
                    string s = ASCIIEncoding.Default.GetString(D, 3, 0x37 - 3);
                    //登录失败，写日志和显示信息

                    if (DebugFlag)
                    {
                        Log.Record("server.log", "帐号：" + OriginalConfigManage.Original_ConfigName + "登录航信服务器失败");
                    }

                    if (s.IndexOf("请检查用户名和口令") != -1)
                    {
                        IdleFlag = 2;
                    }

                    return;
                }

                if (DebugFlag)
                {
                    Log.Record("server.log", "帐号：" + OriginalConfigManage.Original_ConfigName + "登录航信服务器成功");
                }

                //黑屏包头
                Array.Copy(D, 8, m_cmdHeadBuf, 0, 2);


                byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE, 0, FE.Length);
                NetStream.Flush();

                byte[] FE2 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE2, 0, FE.Length);
                NetStream.Flush();

                byte[] FE3 = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                NetStream.Write(FE3, 0, FE.Length);
                NetStream.Flush();

                #region 屏蔽
                //移动控制台处理
                //Kevin 2009-12-02 屏蔽
                //航信更改通信格式，去掉F6F7数据信息
                //if (dlanFlag)
                //{
                //    //判断是否收到F6数据
                //    byte[] F6 = new byte[1024];
                //    netStream.Read(F6, 0, 0x84);

                //    if (F6[1] == 0xF6)
                //    {

                //        //Log.Record("server.log", "收到航信F6数据包：", F6, 0x84);

                //        lock (ThreadWork.lockX)
                //        {
                //            if (eterm3 == null)
                //            {
                //                eterm3 = new Eterm3Lib.ApplicationClass();
                //                eterm3.Connect(0, "127.0.0.1:" + Convert.ToString(m_shareport * 10), "$", "$");
                //            }

                //            ThreadWork.F6 = F6;
                //            ThreadWork.newflag = true;

                //            //Console.WriteLine("获取 F6..." + server);

                //            //Log.Record("server.log", "正在准备获取F7数据信息...");

                //            //等待获取到F7信息
                //            //lock (ThreadWork.lockY) ;
                //            int count = 0;
                //            while ((ThreadWork.newflag) && (count < 20))
                //            {
                //                Thread.Sleep(100);
                //                count++;
                //                continue;
                //            }

                //            if (ThreadWork.newflag)
                //            {
                //                //取得F7信息失败
                //                if (eterm3 != null)
                //                {
                //                    eterm3.Exit();
                //                    eterm3 = null;
                //                }

                //                //停止ThreadWork的监听端口，然后再重新开启监听端口
                //                if (ThreadWork.sslServer != null)
                //                {
                //                    ThreadWork.sslServer.Stop();
                //                }

                //                return;
                //            }


                //            //Log.Record("server.log", "已经获取到F7数据信息...");

                //            //Console.WriteLine("发送 F7..." + server);
                //            //byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //            //netStream.Write(FE, 0, FE.Length);
                //            //netStream.Flush();

                //            //Log.Record("server.log", "向航信发送数据包：", FE, 8);

                //            netStream.Write(ThreadWork.F7, 0, ThreadWork.F7.Length);
                //            netStream.Flush();

                //            //Log.Record("server.log", "向航信发送F7数据包：", ThreadWork.F7, ThreadWork.F7.Length);

                //            if (eterm3 != null)
                //            {
                //                eterm3.Exit();
                //                eterm3 = null;
                //            }
                //        }
                //        sendcmdTime = DateTime.Now;

                //        byte[] FD = new byte[1024];
                //        netStream.Read(FD, 0, 0x06);
                //    }
                //    else
                //    {
                //        //byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //        //netStream.Write(FE, 0, 0x11);
                //        //netStream.Flush();
                //        sendcmdTime = DateTime.Now;
                //    }
                //}
                //else
                //{
                //Log.Record("server.log", "分析黑屏包头：", cmdhead, 2);

                //byte[] FE = { 0x01, 0xFE, 0x00, 0x11, 0x14, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //netStream.Write(FE, 0, 0x11);
                //netStream.Flush();

                //Log.Record("server.log", "向航信发送数据包：", FE, 8);
                #endregion 屏蔽

                SendCmdTime = DateTime.Now;

                byte[] FD = new byte[1024];
                int rcount = NetStream.Read(FD, 0, 1024);
                while (rcount < 18)
                {
                    int recount2 = NetStream.Read(FD, 0, 1024);
                    rcount += recount2;

                    //接收异常，退出循环
                    if (recount2 == 0)
                    {
                        IdleFlag = 4;
                        //错误计数
                        ErrorCount++;
                    }
                }

                //Log.Record("server.log", "收到航信返回数据包：", FD, 0x06);

                //Log.Record("server.log", "登录成功...");
                //}

                //so（签出），并以当前工作号签入


                //签入工作号
                if (!SignIn())
                {
                    //签入工作号出现异常
                    ReleaseEterm();
                    //IdleFlag = -1;
                    return;
                }

                //登陆成功，则把错误计数清零
                ErrorCount = 0;

                //置为空闲
                IdleFlag = 0;
            }
            catch (Exception e)
            {
                if (OriginalConfigManage.ProtocolType == "地址认证")
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_LocalIp + "认证过程出现异常，异常信息：" + e.Message);
                }
                else
                {
                    Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + "认证过程出现异常，异常信息：" + e.Message);
                }

                ReleaseEterm();
                IdleFlag = 4;
                //错误计数
                ErrorCount++;
            }
        }


        /// <summary>
        /// 签入工作号
        /// </summary>
        /// <returns></returns>
        public bool SignIn()
        {
            try
            {
                if (DebugFlag)
                {
                    Log.Record("server.log", "准备签出签入工作号...");
                }

                byte[] headBuf = { 0x01, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x01, 0x39, 0x51, 0x70, 0x02, 0x1B, 0x0B, 0x22, 0x20, 0x00, 0x0F, 0x1E };
                byte[] endBuf = { 0x20, 0x03 };

                //替换黑屏包头 
                Array.Copy(m_cmdHeadBuf, 0, headBuf, 8, 2);

                //先签出工作号
                byte[] soBuf = Encoding.Default.GetBytes("so");
                //替换数据包长度字节
                short tmplen = (short)(soBuf.Length + headBuf.Length + endBuf.Length);
                short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] solenbuf = BitConverter.GetBytes(count2);

                Array.Copy(solenbuf, 0, headBuf, 2, 2);

                ArrayList soal = new ArrayList();
                soal.AddRange(headBuf);
                soal.AddRange(soBuf);
                soal.AddRange(endBuf);

                byte[] so_contentbuf = (byte[])(soal.ToArray(typeof(byte)));

                //组织工作号密码信息
                byte[] workBuf = null;

                if ((OriginalConfigManage.Original_ConfigSi.Trim().Length > 3) && (OriginalConfigManage.Original_ConfigSi.Trim().ToUpper().Substring(0, 2) == "SI"))
                {
                    workBuf = Encoding.Default.GetBytes(OriginalConfigManage.Original_ConfigSi);
                }
                else
                {
                    workBuf = Encoding.Default.GetBytes("SI:" + OriginalConfigManage.Original_ConfigSi);
                }

                //替换数据包长度字节
                tmplen = (short)(workBuf.Length + headBuf.Length + endBuf.Length);
                count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] lenbuf = BitConverter.GetBytes(count2);

                Array.Copy(lenbuf, 0, headBuf, 2, 2);

                byte[] recbuf = new byte[m_MaxPacket];

                int rcount = 0;

                string tmpcontent = "";

                ArrayList al = new ArrayList();
                al.AddRange(headBuf);
                al.AddRange(workBuf);
                al.AddRange(endBuf);

                byte[] contentbuf = (byte[])(al.ToArray(typeof(byte)));

                if (DebugFlag)
                {
                    if (OriginalConfigManage.ProtocolType == "地址认证")
                    {
                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "（地址认证） 正在自动签入工作号...");
                    }
                    else
                    {
                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 正在自动签入工作号...");
                    }
                }

                if (NetStream != null)
                {
                    SendCmdTime = DateTime.Now;

                    lock (NetStream)
                    {
                        if (DebugFlag)
                        {
                            Log.Record("server.log", "正在签出原有工作号...");
                        }

                        //Kevin 2009-09-28 Edit
                        //如果缓冲区有内容，则继续读取
                        while (NetStream.DataAvailable)
                        {
                            NetStream.Read(recbuf, 0, m_MaxPacket);
                            //if (DebugFlag)
                            //{
                            //    if (OriginalConfigManage.ProtocolType == "地址认证")
                            //    {
                            //        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "（地址认证） 接收到签出返回数据包：", recbuf, recbuf.Length);
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到签出返回数据包：", recbuf, recbuf.Length);
                            //    }
                            //}
                        }

                        NetStream.Write(so_contentbuf, 0, so_contentbuf.Length);
                        NetStream.Flush();

                        rcount = NetStream.Read(recbuf, 0, m_MaxPacket);


                        if (DebugFlag)
                        {
                            Log.Record("server.log", "发送签入数据包...");
                        }

                        //Kevin 2009-09-28 Edit
                        //如果缓冲区有内容，则继续读取
                        while (NetStream.DataAvailable)
                        {
                            NetStream.Read(recbuf, 0, m_MaxPacket);
                            //if (DebugFlag)
                            //{
                            //    if (OriginalConfigManage.ProtocolType == "地址认证")
                            //    {
                            //        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "（地址认证） 接收到签入返回数据包：", recbuf, recbuf.Length);
                            //    }
                            //    else
                            //    {
                            //        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到签入返回数据包：", recbuf, recbuf.Length);
                            //    }
                            //}
                        }

                        NetStream.Write(contentbuf, 0, contentbuf.Length);
                        NetStream.Flush();

                        rcount = NetStream.Read(recbuf, 0, m_MaxPacket);

                        if ((recbuf[17] == 0x1B) && (recbuf[18] == 0x4D))
                        {
                            tmpcontent = Encoding.Default.GetString(recbuf, 19, rcount - 23);
                        }
                        else
                        {
                            tmpcontent = Encoding.Default.GetString(recbuf, 17, rcount - 21);
                        }

                        if (DebugFlag)
                        {
                            Log.Record("server.log", "接收到返回数据包：", recbuf, rcount);
                            Log.Record("server.log", "接收到返回信息：" + tmpcontent);
                        }


                        int m_whilecount = 0;
                        while (rcount <= 23)
                        {
                            if ((rcount == 0) || (m_whilecount >= m_MaxWhileCount))
                            {
                                ReleaseEterm();

                                //配置状态置为未连接
                                IdleFlag = 3;
                                //错误计数
                                ErrorCount++;
                                return false;
                            }

                            //Kevin 2009-09-28 Edit
                            //如果缓冲区有内容，则继续读取
                            if (NetStream.DataAvailable)
                            {
                                rcount = NetStream.Read(recbuf, 0, m_MaxPacket);
                            }
                            else
                            {
                                break;
                            }

                            m_whilecount++;

                            if ((recbuf[17] == 0x1B) && (recbuf[18] == 0x4D))
                            {
                                tmpcontent = Encoding.Default.GetString(recbuf, 19, rcount - 23);
                            }
                            else
                            {
                                tmpcontent = Encoding.Default.GetString(recbuf, 17, rcount - 21);
                            }

                            if (DebugFlag)
                            {
                                Log.Record("server.log", "接收到返回数据包：", recbuf, rcount);
                                Log.Record("server.log", "接收到返回信息：" + tmpcontent);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("server.log", "配置：" + OriginalConfigManage.Original_ConfigName + " 签入工作号出现异常，异常信息：" + ex.Message);
                ReleaseEterm();
                IdleFlag = 4;
                //错误计数
                ErrorCount++;
                return false;
            }
        }


        /// <summary>
        /// 断开重连，定时发送心跳数据包
        /// </summary>
        public void EtermTimeProc()
        {
            //循环处理,检测并发送心跳包
            int tmpsecondtime = 0;

            while (true)
            {
                //检查是否退出程序
                if (CloseFlag)
                {
                    Log.Record("server.log", "程序退出");
                    break;
                }

                try
                {
                    //kevin 2011-05-26 屏蔽
                    ////如果达到最大错误数，则停用配置，退出循环
                    //if (errorcount >= _maxErrorCount)
                    //{
                    //    m_etermstatus.status = 3;
                    //    m_idle = false;
                    //    sslStream = null;
                    //    netStream = null;
                    //    Log.Record("error.log", "配置：" + m_username + " 错误次数达到最大次数，异常停用");                        
                    //    break;
                    //}

                    //如果还没启动配置认证，则开始认证配置
                    if ((IdleFlag == -1) && (NetStream == null))
                    {
                        DoWithXTY();
                        continue;
                    }

                    //如果配置状态异常，并且Stream不为空，则置空
                    if (((IdleFlag == 2) || (IdleFlag == 3) || (IdleFlag == 4)) && (NetStream != null))
                    {
                        if (NetStream != null)
                        {
                            if (DebugFlag)
                            {
                                if (OriginalConfigManage.ProtocolType == "地址认证")
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "状态异常netStream不为空，重置为null");
                                }
                                else
                                {
                                    Log.Record("server.log", OriginalConfigManage.Original_ConfigName + "状态异常netStream不为空，重置为null");
                                }
                            }
                            NetStream.Close();
                        }
                        NetStream = null;
                    }

                    //检测Socket是否已经断开与服务器的连接
                    if ((NetStream == null) && ((IdleFlag != -1) || (IdleFlag == 4)))
                    {
                        //重新连接认证服务器
                        DoWithXTY();
                    }


                    //心跳包
                    tmpsecondtime = PublicInfo.DateDiff(DateTime.Now, SendCmdTime);

                    if ((tmpsecondtime >= m_MaxHeartTimes) && ((IdleFlag == 0) || (IdleFlag == 1)))
                    {
                        byte[] buf = new byte[m_MaxPacket];
                        //发送心跳包
                        if (NetStream != null)
                        {
                            SendCmdTime = DateTime.Now;

                            lock (NetStream)
                            {
                                NetStream.Write(m_HeartBuf, 0, 5);
                                NetStream.Flush();
                                SendCmdTime = DateTime.Now;

                                //输出调试信息
                                if (DebugFlag)
                                {
                                    Log.Record("server.log", "向航信发送心跳包...");//：", m_HeartBuf, 5);
                                }

                                NetStream.Read(buf, 0, m_MaxPacket);

                                //输出调试信息
                                if (DebugFlag)
                                {
                                    if (OriginalConfigManage.ProtocolType == "地址认证")
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "（地址认证）接收到心跳包返回数据包...");//：", buf, buf.Length);
                                    }
                                    else
                                    {
                                        Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 接收到心跳包返回数据包...");//：", buf, buf.Length);
                                    }
                                }

                                //Kevin 2009-09-28 Edit
                                //如果缓冲区有内容，则继续读取
                                //while (NetStream.DataAvailable)
                                while (NetClient.Available > 0)
                                {
                                    NetStream.Read(buf, 0, m_MaxPacket);

                                    ////输出调试信息
                                    //if (DebugFlag)
                                    //{
                                    //    Log.Record("server.log", "（配置：地址认证）接收到返回数据包：", buf, buf.Length);
                                    //}
                                }
                            }
                        }
                    }
                    //5秒钟检测一次
                    Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    SendCmdTime = DateTime.Now;
                    if (DebugFlag)
                    {
                        if (OriginalConfigManage.ProtocolType == "地址认证")
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + "发送心跳包过程出错，错误信息：" + e.Message);
                        }
                        else
                        {
                            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + "发送心跳包过程出错，错误信息：" + e.Message);
                        }
                    }
                    ReleaseEterm();
                    IdleFlag = 4;
                    //错误计数
                    ErrorCount++;
                }
            }
        }

        #region 直接转发指令并返回结果信息
        public bool SendCmdDirectAndGetResult(string username,byte[] CmdBuf, ref byte[] ResBuf,ref int RecCount)
        {
            ResBuf = null;

            try
            {
                ////发送指令
                //NetStream.Write(CmdBuf, 0, CmdBuf.Length);
                //NetStream.Flush();

                ////接收返回信息
                //ResBuf = new byte[m_MaxPacket];
                //RecCount = 0;
                //RecCount = NetStream.Read(ResBuf, 0, m_MaxPacket);

                //设置发送指令时间
                SendCmdTime = DateTime.Now;

                //配置设置为空闲
                IdleFlag = 0;

                #region 屏蔽
                ////Kevin 读取数据包长度是否一致，如果未接收完毕则继续
                //if (RecCount != 0)
                //{
                //    int tmpRecCount = RecCount;

                //    byte[] tmpbuf2 = new byte[m_MaxPacket];

                //    //数据包中长度
                //    int RecCount2 = Convert.ToInt32(tmpbuf[2]) * 16 * 16 + Convert.ToInt32(tmpbuf[3]);

                //    //未接收完毕，继续接收数据包
                //    while ((tmpRecCount != 0) && (RecCount < RecCount2))
                //    {
                //        if (EtermType.IndexOf("地址认证") != -1)
                //        {
                //            Log.Record("server.log", OriginalConfigManage.Original_LocalIp + " 继续接收返回数据包...");
                //        }
                //        else
                //        {
                //            Log.Record("server.log", OriginalConfigManage.Original_ConfigName + " 继续接收返回数据包...");
                //        }

                //        tmpRecCount = NetStream.Read(tmpbuf2, 0, m_MaxPacket);

                //        if (tmpRecCount != 0)
                //        {
                //            //合并数据包
                //            Array.Copy(tmpbuf2, 0, tmpbuf, RecCount, tmpRecCount);
                //        }

                //        RecCount += tmpRecCount;
                //    }
                //}
                #endregion 屏蔽
            }
            catch (Exception ex)
            {
                IdleFlag = 4;
                return false;
            }

            return true;
        }
        #endregion 直接转发指令并返回结果信息

        #region 发送指令并获取返回结果信息
        /// <summary>
        /// 发送指令并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, ref byte[] ResBuf, ref string ErrorMessage)
        {
            return false;
        }
        #endregion 发送指令并获取返回结果信息

        #region 发送指令自动PN并获取返回结果信息
        /// <summary>
        /// 发送指令自动根据最后一个指令进行PN并获取返回结果信息，如果多条指令则以“|”分隔
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="cmd">发送指令信息</param>
        /// <param name="CmdBuf">发送指令的数据包</param>
        /// <param name="OfficeCode">指定Office</param>
        /// <param name="allFlag">是否返回所有指令结果信息，true：返回所有结果；false：只返回最后一条指令的结果</param>
        /// <param name="ResBuf">返回的结果数据包</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true:成功；false:失败</returns>
        public bool SendCommandAndGetAllPnResult(string username, string cmd, byte[] CmdBuf, string OfficeCode, bool allFlag, ref byte[] ResBuf, ref string ErrorMessage)
        {
            //信天游配置使用cmd，非信天游配置使用CmdBuf（进行了中文编码及拼音的处理）
            return false;
        }
        #endregion 发送指令自动PN并获取返回结果信息
    }

    /// <summary>
    /// Eterm处理基础类
    /// </summary>
    public class BaseManage
    {
        /// <summary>
        /// 配置类型（密码认证  地址认证  信天游）
        /// </summary>
        private string m_EtermType = "";

        /// <summary>
        /// 通信端口（350  443）
        /// </summary>
        private int m_EtermPort = 350;

        /// <summary>
        /// 原始配置信息
        /// </summary>
        private Base_OriginalConfigManage m_OriginalConfigManage;

        /// <summary>
        /// 是否空闲（-1：未初始化  0：空闲； 1：忙  2：配置密码不正确 3：登录工作号失败请检查 4：异常）
        /// </summary>
        private int m_IdleFlag = -1;

        /// <summary>
        /// 加密Socket流
        /// </summary>
        private SslStream m_SslStream = null;

        /// <summary>
        /// 未加密Socket流
        /// </summary>
        private NetworkStream m_NetStream = null;

        /// <summary>
        /// 配置Tcp连接Client
        /// </summary>
        private TcpClient m_NetClient = null;


        /// <summary>
        /// 配置处理线程（断线重连、心跳包发送）
        /// </summary>
        private Thread m_EtermThread = null;

        /// <summary>
        /// 是否关闭处理标志（释放资源）
        /// </summary>
        private bool m_CloseFlag = false;

        /// <summary>
        /// 是否开启调试输出信息标志
        /// </summary>
        private bool m_DebugFlag = false;

        /// <summary>
        /// 是否开启数据包调试输出信息标志
        /// </summary>
        private bool m_DebugDataFlag = false;

        /// <summary>
        /// 错误计数
        /// </summary>
        private int m_ErrorCount = 0;

        /// <summary>
        ///心跳包最大等待时间间隔(10*60)
        /// <summary>
        public const int m_MaxHeartTimes = 480;

        /// <summary>
        /// 心跳包（心跳包间隔为10分钟，系统默认8分钟发送一次）
        /// </summary>
        public readonly byte[] m_HeartBuf = { 0x01, 0xFB, 0x00, 0x05, 0x00 };

        /// <summary>
        /// 上次发送指令时间
        /// </summary>
        private DateTime m_SendCmdTime = DateTime.Now;

        /// <summary>
        /// 配置数据包头标识
        /// </summary>
        public byte[] m_cmdHeadBuf = { 0x00, 0x00 };

        /// <summary>
        /// 接收数据缓冲区大小1024K 
        /// </summary>
        public const int m_MaxPacket = 1024 * 1024;

        /// <summary>
        /// 最大循环指令计数（用于while循环收发指令），超过此数值时强行退出循环
        /// </summary>
        public const int m_MaxWhileCount = 10;

        /// <summary>
        /// 发送指令计数（用于配置指令平衡）
        /// </summary>
        private int m_CommandCount = 0;

        /// <summary>
        /// 忙碌开始时间
        /// </summary>
        private DateTime m_SetBusyTime = DateTime.Now;

        /// <summary>
        /// 不自动连接时间段
        /// </summary>
        private string m_NoAutoConTime = "";

        /// <summary>
        /// 网站用户帐号
        /// </summary>
        private string m_webUser = "";

        /// <summary>
        /// 发送指令时间间隔（单位：毫秒）
        /// </summary>
        private int m_Interval = 10;

        /// <summary>
        /// 心跳包发送时间
        /// </summary>
        private DateTime m_HeartStartTime = DateTime.Now;

        /// <summary>
        /// 心跳包接收时间
        /// </summary>
        private DateTime m_HeartEndTime = DateTime.Now;

        /// <summary>
        /// 异常时间
        /// </summary>
        private DateTime m_ErrorTime = DateTime.Now;


        /// <summary>
        /// 心跳包发送时间
        /// </summary>
        public DateTime HeartStartTime
        {
            get { return m_HeartStartTime; }
            set { m_HeartStartTime = value; }
        }

        /// <summary>
        /// 心跳包接收时间
        /// </summary>
        public DateTime HeartEndTime
        {
            get { return m_HeartEndTime; }
            set { m_HeartEndTime = value; }
        }

        /// <summary>
        /// 异常时间
        /// </summary>
        public DateTime ErrorTime
        {
            get { return m_ErrorTime; }
            set { m_ErrorTime = value; }
        }

        /// <summary>
        /// 配置类型（密码认证  地址认证  信天游）
        /// </summary>
        public string EtermType
        {
            get { return m_EtermType; }
            set { m_EtermType = value; }
        }

        /// <summary>
        /// 通信端口（350  443）
        /// </summary>
        public int EtermPort
        {
            get { return m_EtermPort; }
            set { m_EtermPort = value; }
        }        

        /// <summary>
        /// 原是配置信息
        /// </summary>
        public Base_OriginalConfigManage OriginalConfigManage
        {
            get { return m_OriginalConfigManage; }
            set { m_OriginalConfigManage = value; }
        }

        /// <summary>
        /// 是否空闲
        /// -2：正在认证状态
        /// -1：未初始化；  
        ///  0：空闲； 
        ///  1：忙碌；  
        ///  2：配置帐号或密码不正确 
        ///  3：登录工作号失败请检查 
        ///  4：其他异常
        /// </summary>
        public int IdleFlag
        {
            get { return m_IdleFlag; }
            set { m_IdleFlag = value; }
        }

        /// <summary>
        /// 加密Socket流
        /// </summary>
        public SslStream SSlStream
        {
            get { return m_SslStream; }
            set { m_SslStream = value; }
        }

        /// <summary>
        /// 未加密Socket流
        /// </summary>
        public NetworkStream NetStream
        {
            get { return m_NetStream; }
            set { m_NetStream = value; }
        }

        /// <summary>
        /// 配置Tcp连接Client
        /// </summary>
        public TcpClient NetClient
        {
            get { return m_NetClient; }
            set { m_NetClient = value; }
        }

        /// <summary>
        /// 配置处理线程（断线重连、心跳包发送）
        /// </summary>
        public Thread EtermThread
        {
            get { return m_EtermThread; }
            set { m_EtermThread = value; }
        }

        /// <summary>
        /// 是否关闭处理标志（释放资源）
        /// </summary>
        public bool CloseFlag
        {
            get { return m_CloseFlag; }
            set { m_CloseFlag = value; }
        }

        /// <summary>
        /// 是否开启调试输出信息标志
        /// </summary>
        public bool DebugFlag
        {
            get { return m_DebugFlag; }
            set { m_DebugFlag = value; }
        }

        /// <summary>
        /// 是否开启数据包调试输出信息标志
        /// </summary>
        public bool DebugDataFlag
        {
            get { return m_DebugDataFlag; }
            set { m_DebugDataFlag = value; }
        }

        /// <summary>
        /// 错误计数
        /// </summary>
        public int ErrorCount
        {
            get { return m_ErrorCount; }
            set { m_ErrorCount = value; }
        }

        /// <summary>
        /// 上次发送指令时间
        /// </summary>
        public DateTime SendCmdTime
        {
            get { return m_SendCmdTime; }
            set { m_SendCmdTime = value; }
        }

        /// <summary>
        /// 发送指令计数（用于配置指令平衡）
        /// </summary>
        public int CommandCount
        {
            get { return m_CommandCount; }
            set { m_CommandCount = value; }
        }

        /// <summary>
        /// 忙碌开始时间
        /// </summary>
        public DateTime SetBusyTime
        {
            get { return m_SetBusyTime; }
            set { m_SetBusyTime = value; }
        }

        /// <summary>
        /// 不自动连接时间段
        /// </summary>
        public string NoAutoConTime
        {
            get { return m_NoAutoConTime; }
            set { m_NoAutoConTime = value; }
        }

        /// <summary>
        /// 网站用户名
        /// </summary>
        public string WebUser
        {
            get { return m_webUser; }
            set { m_webUser = value; }
        }

        /// <summary>
        /// 发送指令时间间隔（单位：毫秒）
        /// </summary>
        public int Interval
        {
            get { return m_Interval; }
            set { m_Interval = value; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void ReleaseEterm()
        {
            m_CloseFlag = true;

            try
            {
                if ((m_EtermThread != null) && (m_EtermThread.IsAlive))
                {
                    m_EtermThread.Abort();
                }
            }
            catch
            {
            }

            try
            {
                if (m_NetClient != null)
                {
                    m_NetClient.Close();
                }
            }
            catch
            {
            }

            try
            {
                m_NetClient = null;
                m_NetStream = null;
                m_SslStream = null;
            }
            catch { }
        }

        /// <summary>
        /// 重启配置，重启Eterm线程
        /// </summary>
        public void ReStartEterm()
        {
            //
            if (this.EtermType == "信天游")
            {
                //
            }
            else if (this.EtermType == "地址认证")
            {
                try
                {
                    ((Eterm350Manage)this).EtermThread.Abort();
                    ((Eterm350Manage)this).EtermThread = null;                    
                }
                catch
                {
                }
                try
                {
                    ((Eterm350Manage)this).IdleFlag = 0;
                    ((Eterm350Manage)this).HeartEndTime = ((Eterm350Manage)this).HeartStartTime;
                    ((Eterm350Manage)this).NetClient = null;
                    ((Eterm350Manage)this).NetStream = null;
                    ((Eterm350Manage)this).SendCmdTime = DateTime.Now;                   
                    ((Eterm350Manage)this).EtermThread = new Thread(new ThreadStart(((Eterm350Manage)this).EtermTimeProc));
                    ((Eterm350Manage)this).EtermThread.Start();
                }
                catch
                {
                }
            }
            else if (this.EtermPort == 443)
            {
                try
                {
                    ((Eterm443Manage)this).EtermThread.Abort();
                    ((Eterm443Manage)this).EtermThread = null;
                }
                catch
                {
                }
                try
                {
                    ((Eterm443Manage)this).IdleFlag = 0;
                    ((Eterm443Manage)this).HeartEndTime = ((Eterm443Manage)this).HeartStartTime;
                    ((Eterm443Manage)this).NetClient = null;
                    ((Eterm443Manage)this).SSlStream = null;
                    ((Eterm443Manage)this).SendCmdTime = DateTime.Now;
                    ((Eterm443Manage)this).EtermThread = new Thread(new ThreadStart(((Eterm443Manage)this).EtermTimeProc));
                    ((Eterm443Manage)this).EtermThread.Start();
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    ((Eterm350Manage)this).EtermThread.Abort();
                    ((Eterm350Manage)this).EtermThread = null;
                }
                catch
                {
                }
                try
                {
                    ((Eterm350Manage)this).IdleFlag = 0;
                    ((Eterm350Manage)this).HeartEndTime = ((Eterm350Manage)this).HeartStartTime;
                    ((Eterm350Manage)this).NetClient = null;
                    ((Eterm350Manage)this).NetStream = null;
                    ((Eterm350Manage)this).SendCmdTime = DateTime.Now;
                    ((Eterm350Manage)this).EtermThread = new Thread(new ThreadStart(((Eterm350Manage)this).EtermTimeProc));
                    ((Eterm350Manage)this).EtermThread.Start();
                }
                catch
                {
                }
            }
        }

        #region 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// <summary>
        /// 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// </summary>
        /// <param name="command">原指令</param>
        /// <param name="resbuf">结果数据包</param>
        /// <param name="needpinyin">是否需要拼音</param>
        public void AnalyseWebCmdAndMakeServerInfo(string command, out byte[] resbuf, bool needpinyin)
        {
            resbuf = null;

            //先把字节包转换为字符串
            string tmpcontent = command;

            //源字符串长度
            int count = tmpcontent.Length;

            //序号
            int index = 0;

            //汉字的起始、结束字节标志
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };

            int code = 0;
            byte tmpbyte;

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);

            bool isChinese = false;//是否汉字

            //临时存储连续的汉字
            string tmpstr = "";
            string tmppinyin = "";//拼音

            ArrayList al = new ArrayList();

            PublicInfo thepub = new PublicInfo();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    //获得字符串中指定索引i处字符unicode编码
                    code = Char.ConvertToUtf32(tmpcontent, i);
                    if (code >= chfrom && code <= chend)
                    {
                        tmpstr += tmpcontent[i];
                        isChinese = true;     //当code在中文范围内
                    }
                    else
                    {
                        if (isChinese)
                        {
                            isChinese = false;
                            if (tmpstr.Length != 0)
                            {
                                //取得汉字的拼音

                                //暂时存储汉字编码
                                ArrayList al2 = new ArrayList();

                                for (int j = 0; j < tmpstr.Length; j++)
                                {
                                    byte[] hanziBt;
                                    tmppinyin = chs2py.convert(tmpstr.Substring(j, 1), out hanziBt);

                                    if (needpinyin)
                                    {
                                        al.AddRange(Encoding.Default.GetBytes(tmppinyin));
                                    }

                                    //如果没有找到汉字编码，则使用转换方法
                                    if (hanziBt == null)
                                    {
                                        //转换临时存储的汉字并添加到ArrayList
                                        byte[] tmpbuf = Encoding.Default.GetBytes(tmpstr.Substring(j, 1));
                                        index += 2;// tmpbuf.Length;
                                        for (int k = 0; k < tmpbuf.Length; k++)
                                        {
                                            tmpbyte = (byte)(tmpbuf[k] - 0x80);
                                            al2.Add(tmpbyte);
                                        }
                                    }
                                    else
                                    {
                                        al2.AddRange(hanziBt);
                                        index += 2;// hanziBt.Length;
                                    }
                                }

                                al.AddRange(beginbuf);

                                al.AddRange(al2);

                                al.AddRange(endbuf);
                                tmpstr = "";
                            }
                        }
                        //当code不在中文范围内
                        tmpbyte = Convert.ToByte(tmpcontent[i]);
                        index++;
                        al.Add(tmpbyte);
                    }
                }

                if ((isChinese) && (tmpstr.Length != 0))
                {
                    //暂时存储汉字编码
                    ArrayList al2 = new ArrayList();

                    for (int j = 0; j < tmpstr.Length; j++)
                    {
                        byte[] hanziBt;
                        tmppinyin = chs2py.convert(tmpstr.Substring(j, 1), out hanziBt);

                        if (needpinyin)
                        {
                            al.AddRange(Encoding.Default.GetBytes(tmppinyin));
                        }

                        //如果没有找到汉字编码，则使用转换方法
                        if (hanziBt == null)
                        {
                            //转换临时存储的汉字并添加到ArrayList
                            byte[] tmpbuf = Encoding.Default.GetBytes(tmpstr.Substring(j, 1));
                            index += tmpbuf.Length;
                            for (int k = 0; k < tmpbuf.Length; k++)
                            {
                                tmpbyte = (byte)(tmpbuf[k] - 0x80);
                                al2.Add(tmpbyte);
                            }
                        }
                        else
                        {
                            al2.AddRange(hanziBt);
                            index += hanziBt.Length;
                        }
                    }

                    al.AddRange(beginbuf);

                    al.AddRange(al2);

                    al.AddRange(endbuf);
                }

                resbuf = (byte[])al.ToArray(typeof(byte));
            }
            catch (Exception ex)
            {
                Log.Record("client.log", ex);
            }
        }
        #endregion 把网络的命令包转换为航信命令数据包，主要是处理中文

        #region 分析航信返回的数据包（把汉字编码转换成正常汉字）
        //分析航信返回数据信息
        public string AnalyseServerContent(byte[] buf, int count)
        {
            string result = "";

            //主要是解析中文
            byte[] beginbuf = { 0x1B, 0x0E };
            byte[] endbuf = { 0x1B, 0x0F };
            ArrayList al = new ArrayList();
            ArrayList al2 = new ArrayList();

            int index = 0;
            int tmpcount = 0;
            bool isChinese = false;//是否为汉字

            //汉字范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);
            byte tmpbyte;

            while (index < count)
            {
                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0E))
                {
                    //汉字开始标志
                    al2.Clear();
                    index += 2;
                    isChinese = true;
                    continue;
                }

                if ((buf[index] == 0x1B) && (buf[index + 1] == 0x0F))
                {
                    string tmpcontent = "";
                    bool doubleFlag = false;
                    for (int i = 0; i < (al2.Count / 2); i++)
                    {
                        //如果有占用4个字节的汉字，则跳过一次
                        if (doubleFlag)
                        {
                            doubleFlag = false;
                            continue;
                        }
                        if ((byte)(al2[i * 2]) == 0x78)
                        {
                            byte[] cbuf = new byte[4];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            cbuf[2] = (byte)(al2[i * 2 + 2]);
                            cbuf[3] = (byte)(al2[i * 2 + 3]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            //index += 2;
                            doubleFlag = true;
                        }
                        else
                        {
                            byte[] cbuf = new byte[2];
                            cbuf[0] = (byte)(al2[i * 2]);
                            cbuf[1] = (byte)(al2[i * 2 + 1]);
                            tmpcontent += GetHanZiByHangXinBianMa(cbuf);//chs2py.GetHanZi(cbuf);
                            doubleFlag = false;
                        }
                    }

                    //处理并添加汉字信息
                    //把内容转换为string，判断是否为汉字，如果不是则把高低位互换然后把低位加10
                    //string tmpcontent= Encoding.Default.GetString((byte[])al2.ToArray(typeof(byte)));

                    //for (int i = 0; i < tmpcontent.Length; i++)
                    //{
                    //    int code = Char.ConvertToUtf32(tmpcontent, i);

                    //    //非汉字，需要处理，把高低位互换然后把低位加10
                    //    if ((code < chfrom)||(code > chend))
                    //    {
                    //        tmpbyte =  (byte)al2[i * 2];
                    //        al2[i * 2] = al2[i * 2 + 1];
                    //        al2[i * 2 + 1] = (byte)(tmpbyte + 0x0A);
                    //    }
                    //}

                    //al2.Clear();
                    //al2.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //把转换过的汉字追加到al中
                    al.AddRange(Encoding.Default.GetBytes(tmpcontent));

                    //汉字结束
                    index += 2;
                    isChinese = false;
                    continue;
                }

                tmpbyte = buf[index];

                //如果是汉字则处理
                if (isChinese)
                {
                    //tmpbyte += 0x80;
                    al2.Add(tmpbyte);
                }
                else
                {
                    al.Add(tmpbyte);
                }
                tmpcount++;
                index++;
            }

            //从第20位开始，读取到末尾倒数第三位
            if (buf.Length < 23)
                return "";

            if ((buf[17] == 0x1B) && (buf[18] == 0x4D))
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, tmpcount - 23);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 19, al.Count - 23);
            }
            else
            {
                //Kevin 2010-06-24 Edit
                //result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, tmpcount - 21);
                result = ASCIIEncoding.Default.GetString((byte[])al.ToArray(typeof(byte)), 17, al.Count - 21);
            }

            return result;
        }
        #endregion 分析航信返回的数据包（把汉字编码转换成正常汉字）

        #region 根据汉字编码取得汉字信息
        /// <summary>
        /// 根据航信编码返回汉字信息
        /// </summary>
        /// <param name="hzBytes"></param>
        /// <returns></returns>
        public string GetHanZiByHangXinBianMa(byte[] hzBytes)
        {
            string ResHanZi = "";
            string tmpBianMa = "";
            tmpBianMa = chs2py.GetStringFromByte(hzBytes[0]);
            tmpBianMa += " " + chs2py.GetStringFromByte(hzBytes[1]);

            ResHanZi = chs2py.GetHanZi(hzBytes);


            return ResHanZi;
        }
        #endregion

        #region 根据汉字取得汉字信息
        /// <summary>
        /// 根据汉字取得汉字编码和拼音
        /// </summary>
        /// <param name="HanZi"></param>
        /// <param name="HanZiBt"></param>
        /// <returns></returns>
        public string GetPinYinBianMaFromHanZi(string HanZi, out byte[] HanZiBt)
        {
            HanZiBt = null;

            string StrPinYin = "";

            StrPinYin = chs2py.convert(HanZi, out HanZiBt);
            
            return StrPinYin;
        }
        #endregion
    }
}
