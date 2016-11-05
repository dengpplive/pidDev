using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PidClient
{
    public class PrepareCommand
    {
        //用于预处理订座指令
        private static string PreSendCmd = "";

        //上次的缓存指令
        private static string PreCmd = "";

        #region AVH缓存信息
        //上次AVH查询结果的航班日期
        private static string PreAVHLastDate = "";

        //上次AVH查询结果最后一个航班起飞时间
        private static string PreAVHLastTime = "";

        //上次AVH查询的结果
        private static string PreAVHResult = "";

        //上次AVH查询指令
        private static string PreAVHCmd = "";

        #endregion AVH缓存信息

        #region 订座缓存信息
        //SD指令
        private static string PreSDCmd = "";
        //NM指令
        private static string PreNMCmd = "";
        //CT指令
        private static string PreCTCmd = "";
        //TKTL指令
        private static string PreTKTLCmd = "";
        //SSR Foid 指令
        private static string PreSSRFoidCmd = "";
        //EI指令
        private static string PreEICmd = "";
        //其他指令
        private static string PreOtherCmd = "";
        #endregion 订座缓存信息

        //RT缓存指令
        private static string PreRTCmd = "";

        //订座标志
        private static bool YuDingFlag = false;

        //是否提取编码状态
        private static bool RTFlag = false;


        //历史指令
        private static List<string> CmdList = new List<string>();

        //清空缓存
        private static void ClearPreInfo()
        {
            RTFlag = false;
            YuDingFlag = false;
            //PreAVHResult = "";
            //PreAVHLastTime = "";
            //PreAVHLastDate = "";
            PreCmd = "";
            PreSendCmd = "";
            //SD指令
            PreSDCmd = "";
            //NM指令
            PreNMCmd = "";
            //CT指令
            PreCTCmd = "";
            //TKTL指令
            PreTKTLCmd = "";
            //SSR Foid 指令
            PreSSRFoidCmd = "";
            //EI指令
            PreEICmd = "";
            //其他指令
            PreOtherCmd = "";
            PreRTCmd = "";
        }

        //清空RT缓存
        private static void ClearRTInfo()
        {
            RTFlag = false;
            PreRTCmd = "";
        }

        //清空预订缓存
        private static void ClearYuDingInfo()
        {
            YuDingFlag = false;
            PreSDCmd = "";
            PreEICmd = "";
            PreNMCmd = "";
            PreCTCmd = "";
            PreSSRFoidCmd = "";
            PreOtherCmd = "";
            PreTKTLCmd = "";
        }

        //分析返回结果
        public static void AnalyseCommandResult(string cmd, string ResultInfo)
        {
            //查询结果分析
            if (cmd.Trim().Length>3 && cmd.ToLower().Trim().Substring(0, 3) == "avh")
            {
                //上次AVH查询结果
                PreAVHResult = ResultInfo;
                PreAVHCmd = cmd;

                //分析AVH返回结果，获取最后一个航班时间
                string lastTime="";
                string lastDate = "";
                if (AnalyseAVHGetLastDateTime(cmd, ResultInfo, out lastDate, out lastTime))
                {
                    PreAVHLastTime = lastTime;
                    PreAVHLastDate = lastDate;
                }
                else
                {
                    PreAVHLastDate = "";
                    PreAVHLastTime = "";
                }
            }
            //else if(cmd.ToLower().Trim().
        }

        //解析航班信息用于订座显示
        //no 序号，1第一程航班，2第二程航班
        //index 航班序号，对应AVH结果中的序号
        //classcode 舱位代码
        //seatcount 座位数
        public static bool GetFlightInfo(int no,int index, string classcode, int seatcount, ref string displayinfo)
        {
            displayinfo = "";

            try
            {
                //
                string AVHResult = "";

                AVHResult = PreAVHResult;


                //解析AVH结果
                return AnalyseAVHGetYuDingInfo(AVHResult, index, classcode, seatcount,1,ref displayinfo);
            }
            catch (Exception ex)
            {
                Log.Record("Error.log", "格式化显示预订航班信息出现异常，错误原因：" + ex.Message);
                return false;
            }
        }

        //把SD转换为SS指令
        public static bool GetSSCommandBySD(string sdcmd,ref string ssCmd)
        {
            ssCmd = "";
            try
            {
                //无缓存AVH结果信息，返回空
                if (PreAVHResult.Trim() == "")
                {
                    return false;
                }


                //nm1曹艳^
                //ss cz6966 b 10jan14 krlurc 1^
                //ss cz6728 b 11jan14 urchak 1^
                //ct0996-2028966^
                //ssr foid cz hk/ni513221198906190229/p1^
                //osi cz ctct18699616295^
                //tktl/2155/10jan/urc221^
                //@
                //根据SD指令获取SS对应信息
                return AnalyseAVHGetYuDingInfo(PreAVHResult, Convert.ToInt32(sdcmd.Substring(2, 1)), sdcmd.Substring(3, 1), Convert.ToInt32(sdcmd.Substring(4, 1)), 2, ref ssCmd);

            }
            catch (Exception ex)
            {
                Log.Record("error.log", "SD转换为SS指令出现异常，错误原因：" + ex.Message);
                return false ;
            }
        }

        //组织待发送的订座指令
        public static bool GetSendCommand(ref string sendCmd)
        {
            sendCmd = "";
            try
            {
                if (PreSDCmd.Trim() == "")
                {
                    sendCmd = "缺少航段信息！";
                    return false;
                }

                if (PreNMCmd.Trim() == "")
                {
                    sendCmd = "缺少乘客信息！";
                    return false;
                }

                if (PreTKTLCmd.Trim() == "")
                {
                    sendCmd = "缺少出票时限信息！";
                    return false;
                }

                if (PreCTCmd.Trim() == "")
                {
                    sendCmd = "缺少联系组信息！";
                    return false;
                }

                //把SD指令转换为SS
                string sscmd = "";
                if (!GetSSCommandBySD(PreSDCmd,ref sscmd))
                {
                    sendCmd = sscmd;
                    return false;
                }

                sendCmd = PreNMCmd + "\r" + sscmd + "\r" + PreTKTLCmd + "\r" + PreCTCmd ;

                if (PreSSRFoidCmd.Trim() != "")
                {
                    sendCmd += "\r" + PreSSRFoidCmd.Replace('|', '\r') ;
                }

                if (PreEICmd.Trim() != "")
                {
                    sendCmd += "\r" + PreEICmd ;
                }

                if (PreOtherCmd.Trim() != "")
                {
                    sendCmd += "\r" + PreOtherCmd.Replace('|', '\r');
                }

                sendCmd += "\r@";
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("error.log", "组织待发送的订座指令出现异常，错误原因："+ex.Message);
            }
            return false;
        }

        //检查证件信息是否已经存在，如果存在则替换，不存在则添加
        public static bool CheckAndReplaceSsrFoid(string cmd, ref string errorMsg)
        {
            errorMsg = "";
            try
            {
                //解析名字
                if (PreNMCmd.Trim() == "")
                {
                    errorMsg = "请先使用NM指令输入乘客姓名信息，指令格式：NM1姓名1姓名";
                    return false;
                }

                string[] sl=PreNMCmd.Trim().Substring(3).Split('1');

                if (cmd.Trim().Substring(cmd.Trim().Length - 5).IndexOf("/") == -1)
                {
                    cmd += "/P1";
                }

                string tmpstr = cmd.Trim().Substring(cmd.Trim().Length - 5);
                tmpstr = tmpstr.Substring(tmpstr.IndexOf("/"));
                
                int index=int.Parse(tmpstr.Substring(2).Trim());

                if(index<1 || index>sl.Length)
                {
                    errorMsg="没有对应的乘客序号！";
                    return false;
                }

                bool ExistFlag = false;
                string[] sl2 = PreSSRFoidCmd.Split('|');                
                for (int i = 0; i < sl2.Length; i++)
                {
                    if (sl2[i].Trim() == "")
                    {
                        continue;
                    }
                    //如果已经存在则替换
                    if (sl2[i].Substring(sl2[i].LastIndexOf("/")).Trim().ToLower() == tmpstr.ToLower())
                    {
                        sl2[i] = cmd;
                        ExistFlag = true;
                        break;
                    }
                }

                if (!ExistFlag)
                {
                    if (PreSSRFoidCmd.Trim() == "")
                    {
                        PreSSRFoidCmd = cmd;
                    }
                    else
                    {
                        PreSSRFoidCmd += "|" + cmd;
                    }
                }
                else
                {
                    PreSSRFoidCmd = "";
                    for (int i = 0; i < sl2.Length; i++)
                    {
                        if (sl2[i].Trim() == "")
                        {
                            continue;
                        }
                        PreSSRFoidCmd += sl2[i] + "|";
                    }
                    PreSSRFoidCmd = PreSSRFoidCmd.Substring(0, PreSSRFoidCmd.Length - 1);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Record("error.log", "订座过程中检查证件号码项出现异常，错误原因："+ex.Message);
                errorMsg = "操作出现异常！";
                return false;
            }
        }

        //删除预订预处理的序号指令
        public static bool DelYuDingCommandByIndex(string indexNo, ref string ErrorMsg)
        {
            ErrorMsg = "";
            try
            {
                int delIndex = int.Parse(indexNo);

                //查找对应的指令
                int index = 1;
                //处理NM显示
                if (PreNMCmd.Trim() != "")
                {
                    string[] sl1 = PreNMCmd.Trim().Substring(2).Split('1');
                    for (int i = 0; i < sl1.Length; i++)
                    {
                        if (index == delIndex)
                        {
                            ErrorMsg = "不支持删除姓名信息！";
                            return false;
                        }
                        index++;
                    }                   
                }

                //根据SD获取航班显示信息
                if (PreSDCmd.Trim() != "")
                {
                    string[] sl2 = PreSDCmd.Split('|');
                    for (int i = 0; i < sl2.Length; i++)
                    {
                        if (index == delIndex)
                        {
                            ErrorMsg = "不支持删除航段信息！";
                            return false;
                        }
                        index++;                        
                    }
                }

                //处理CT项
                if (PreCTCmd.Trim() != "")
                {
                    if (index == delIndex)
                    {
                        PreCTCmd = "";
                        return true;
                    }
                    index++;
                }

                //处理TKTL项
                if (PreTKTLCmd.Trim() != "")
                {
                    if (index == delIndex)
                    {
                        PreTKTLCmd = "";
                        return true;
                    }
                    index++;
                }

                //处理 SSR Foid 指令
                if (PreSSRFoidCmd.Trim() != "")
                {
                    string[] sl = PreSSRFoidCmd.Split('|');
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (index == delIndex)
                        {
                            sl[i] = "";
                            PreSSRFoidCmd = "";
                            for (int j = 0; j < sl.Length; j++)
                            {
                                if(sl[j]!="")
                                {
                                    PreSSRFoidCmd = sl[j] + "|";
                                }
                            }
                            PreSSRFoidCmd = PreSSRFoidCmd.Substring(0, PreSSRFoidCmd.Length - 1);
                            return true;
                        }
                        index++;
                    }
                }

                //添加其他指令
                if (PreOtherCmd.Trim() != "")
                {
                    string[] sl = PreOtherCmd.Split('|');
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (index == delIndex)
                        {
                            sl[i] = "";
                            PreOtherCmd = "";
                            for (int j = 0; j < sl.Length; j++)
                            {
                                if (sl[j] != "")
                                {
                                    PreOtherCmd += sl[j] + "|";
                                }
                            }
                            PreOtherCmd = PreOtherCmd.Substring(0, PreOtherCmd.Length - 1);
                            return true;
                        }
                        index++;
                    }
                }

                //添加EI项
                if (PreEICmd.Trim() != "" && PreEICmd.Trim().Length > 3)
                {
                    if (index == delIndex)
                    {
                        PreEICmd = "";
                        return true;
                    }
                    index++;
                }


                //添加Office编号
                if (index == delIndex)
                {
                    ErrorMsg = "不支持删除Office信息！";
                    return false;
                }

                ErrorMsg = "未找到对应的序号，请使用RT提取确认！";
                return false;
            }
            catch (Exception)
            {
                ErrorMsg = "指令格式错误，标准格式：XE序号";
                return false;
            }
        }

        //处理预订预处理显示
        public static string GetYuDingResult()
        {
            string strResult="";

            try
            {
                int index = 1;
                //处理NM显示
                if (PreNMCmd.Trim() != "")
                {
                    string[] sl1 = PreNMCmd.Trim().Substring(3).Split('1');
                    for (int i = 0; i < sl1.Length; i++)
                    {
                        strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." + sl1[i];
                        index++;
                    }
                    strResult += "\n";
                }

                //根据SD获取航班显示信息
                if (PreSDCmd.Trim() != "")
                {
                    string[] sl2=PreSDCmd.Split('|');                    
                    for (int i = 0; i < sl2.Length; i++)
                    {
                        string resSDInfo = "";
                        if (!GetFlightInfo(i+1,Convert.ToInt32(sl2[i].Trim().Substring(2, 1)), sl2[i].Trim().Substring(3, 1), Convert.ToInt32(sl2[i].Trim().Substring(4, 1)), ref resSDInfo))
                        {
                            return resSDInfo;
                        }
                        strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." + resSDInfo;
                        strResult += "\n";
                        index++;
                    }
                }

                //处理CT项
                if (PreCTCmd.Trim() != "")
                {
                    if (PreCTCmd.Trim().Length > 2 && PreCTCmd.Trim().ToLower().Substring(0, 2) == "ct")
                    {
                        strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." + 
                                PreCTCmd.Trim().Substring(2).Trim();
                    }
                    else
                    {
                        strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                PreCTCmd.Trim();
                    }
                    strResult += "\n";
                    index++;
                }

                //处理TKTL项
                if (PreTKTLCmd.Trim() != "")
                {
                    if (PreTKTLCmd.Trim().Length > 4)
                    {
                        strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                PreTKTLCmd.Trim().Substring(2).Trim();
                    }
                    else
                    {
                        strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                PreTKTLCmd.Trim();
                    }
                    strResult += "\n";
                    index++;
                }

                //处理 SSR Foid 指令
                if(PreSSRFoidCmd.Trim()!="")
                {
                    string[] sl = PreSSRFoidCmd.Split('|');
                    for (int i = 0; i < sl.Length; i++)
                    {
                        if (sl[i].Trim().Substring(sl[i].Trim().Length - 5).IndexOf("/") == -1)
                        {
                            strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                sl[i].Trim().Replace('/', ' ') + "/P1";
                        }
                        else
                        {
                            strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                sl[i].Trim().Substring(0, sl[i].Trim().IndexOf('/')) + " " + sl[i].Trim().Substring(sl[i].Trim().IndexOf('/')+1);
                        }
                        strResult += "\n";
                        index++;
                    }
                }

                //添加其他指令
                if (PreOtherCmd.Trim() != "")
                {
                    string[] sl = PreOtherCmd.Split('|');
                    for (int i = 0; i < sl.Length; i++)
                    {
                        strResult +=  (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                sl[i].Trim();
                        strResult += "\n";
                        index++;
                    }
                }

                //添加EI项
                if (PreEICmd.Trim() != "" && PreEICmd.Trim().Length>3)
                {
                    byte[] Buf=new byte[1024];

                    string pinyin = chs2py.convert(PreEICmd.Trim().Substring(3),out Buf);
                    strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." +
                                PreEICmd.Trim().Substring(0, 2) + "/" +pinyin.ToUpper()+PreEICmd.Trim().Substring(3) ;
                    strResult += "\n";
                    index++;
                }


                //添加Office编号
                strResult += (index.ToString().Length == 1 ? " " + index.ToString() : index.ToString()) + "." + PublicClass.curOfficeCode;

                strResult = strResult.ToUpper();
            }
            catch (Exception ex)
            {
                Log.Record("Error.log", "进行订座预处理显示处理时出现异常，错误原因：" + ex.Message);
            }

            return strResult;
        }

        public static string DuelCommand(string cmd, ref string ErrorMessage)
        {
            //预处理订座指令
            //1 AVH + PN
            //2 AVH + SD + TKTL + CT + SSR FOID +...+\ @
            //3 RT
            //4 RTXXXXX + PN
            //5 RT + AVH + SD
            //6 XE
            //7 I IG
            //8 \ @
            string strResult = cmd;

            try
            {
                //处理AVH指令
                #region 处理AVH指令
                if (cmd.Trim().Length>3 && cmd.Trim().ToLower().Substring(0, 2) == "av" && cmd.Trim().ToLower().Substring(0, 3) != "avh")
                {
                    ErrorMessage = "本系统不支持AV指令，请使用AVH进行航班查询！\nAVH指令格式：AVH/CTUPEK/20JAN/CA/D";
                    return "";
                }

                //AVH指令 avh/ctupek/20jun/ca/d  avh/ctupek/./ca/d
                if (cmd.Trim().Length>3 && cmd.Trim().ToLower().Substring(0, 3) == "avh")
                {
                    //检查指令格式
                    string[] sl = cmd.Split(new char[] { ':', ' ','/'});
                    if (sl.Length < 2)
                    {
                        ErrorMessage = "请使用AVH标准格式：AVH/CTUPEK/20JAN/CA/D";
                        return "";
                    }

                    try
                    {
                        if (sl[0].ToLower() != "avh" || sl[1].ToLower().Length != 6)
                        {
                            ErrorMessage = "请使用AVH标准格式：AVH/CTUPEK/20JAN/CA/D";
                            return ""; 
                        }

                        //检查航班日期项
                        if (sl.Length>2 && sl[2] != "." && sl[2] != "+")
                        {
                            getStandardDate(sl[2]);
                        }
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "请使用AVH标准格式：AVH/CTUPEK/20JAN/CA/D";
                        return ""; 
                    }

                    PreCmd = cmd;
                    return PreCmd;
                }
                #endregion 处理AVH指令

                //处理FD指令
                #region 处理FD指令
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 2) == "fd")
                {
                    try
                    {
                        int index = int.Parse(cmd.Trim().Substring(2).Trim());
                        if(PreAVHResult!="")
                        {                            
                            string sendcmd="";
                            //转换成标准FD指令： FD:CTUPEK/20JAN/CA
                            if (AnalyseAVHGetYuDingInfo(PreAVHResult,index,"y",1,3,ref sendcmd))
                            {
                                PreCmd = sendcmd;
                                return PreCmd;
                            }
                            else
                            {
                                PreCmd = PreAVHCmd + "|" + cmd;
                                return PreCmd;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                #endregion 处理FD指令

                //处理PAT指令
                #region 处理PAT指令
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 3) == "pat")
                {
                    if (cmd.Trim().Length < 5)
                    {
                        ErrorMessage = "请使用标准指令格式 PAT:A";
                        return "";
                    }

                    //预订状态则转换成SS + PAT指令并发送
                    if (YuDingFlag)
                    {
                        if(PreAVHResult.Trim()==""||PreSDCmd.Trim()=="")
                        {
                            ClearYuDingInfo();
                            ErrorMessage="出现异常，请重新查询预订！";
                            return "";
                        }
                        string sendcmd="";
                        if (AnalyseAVHGetYuDingInfo(PreAVHResult, Convert.ToInt32(PreSDCmd.Trim().Substring(2, 1)), PreSDCmd.Trim().Substring(3, 2), 1, 2, ref sendcmd))
                        {
                            return sendcmd + "|" + cmd;
                        }
                    }
                        //RT状态则组织 RT + PAT指令并发送
                    else if (RTFlag)
                    {
                        return PreRTCmd + "|" + cmd;
                    }
                }
                #endregion 处理PAT指令

                //处理PN指令
                #region 处理PN指令
                if (cmd.Trim().ToLower() == "pn")
                {
                    //AVH + PN 预处理
                    if (PreCmd.Trim().Length>3 && PreCmd.Trim().ToLower().Substring(0, 3) == "avh")
                    {
                        if (PreAVHLastTime.Trim() != "")
                        {
                            //组织指令
                            string[] sl=PreCmd.Split(new char[]{':',' ','/'});
                            if (sl.Length <= 2)
                            {
                                PreCmd = PreCmd + "/" + PreAVHLastDate + "/" + PreAVHLastTime;
                                return PreCmd;
                            }
                            else
                            {
                                try
                                {
                                    getStandardDate(sl[2]);
                                    sl[2] = PreAVHLastDate;
                                }
                                catch
                                {
                                    PreCmd = "";
                                    for(int i=0;i<sl.Length;i++)
                                    {
                                        //
                                        PreCmd += sl[i]+"/";
                                        if(i==1)
                                        {
                                            PreCmd+= PreAVHLastDate+"/"+PreAVHLastTime+"/";
                                        }
                                    }
                                    PreCmd = PreCmd.Substring(0,PreCmd.Length-1);
                                    return PreCmd;
                                }

                                try
                                {
                                    Convert.ToInt32(sl[3]);
                                    sl[3] = PreAVHLastTime;
                                    PreCmd = "";
                                    for (int i = 0; i < sl.Length; i++)
                                    {
                                        //
                                        PreCmd += sl[i] + "/";
                                    }
                                    PreCmd = PreCmd.Substring(0, PreCmd.Length - 1);
                                    return PreCmd;
                                }
                                catch
                                {
                                    PreCmd = "";
                                    for (int i = 0; i < sl.Length; i++)
                                    {
                                        //
                                        PreCmd += sl[i] + "/";
                                        if (i == 2)
                                        {
                                            PreCmd += PreAVHLastTime + "/";
                                        }
                                    }
                                    PreCmd = PreCmd.Substring(0, PreCmd.Length - 1);
                                    return PreCmd;
                                }
                            }                            
                        }
                        else
                        {
                            //如果上次AVH结果分析异常，则发送连续指令
                            return PreCmd + "|PN" ;
                        }
                    }
                    else
                    {
                        //
                        PreCmd += "|pn";
                        return PreCmd;
                    }
                }
                #endregion 处理PN指令

                //还原指令
                #region 还原指令
                if (cmd.Trim().ToLower() == "i" || cmd.Trim().ToLower() == "ig")
                {
                    //清空缓存
                    ClearPreInfo();
                    return "";
                }
                #endregion 还原指令

                //封口指令
                #region 处理封口指令
                if (cmd.Trim() == "\\" || cmd.Trim() == "@")
                {
                    //预订状态，则组织指令并提交到服务器
                    if (YuDingFlag)
                    {
                        //组织待发送的订座指令
                        string sendCmd = "";
                        if (!GetSendCommand(ref sendCmd))
                        {
                            ErrorMessage = sendCmd;
                            return "";
                        }
                        else
                        {
                            ClearYuDingInfo();
                            return sendCmd;
                        }
                    }
                    else
                    {
                        ClearRTInfo();
                        return "";
                    }
                }
                #endregion 处理封口指令

                //处理提取编码指令
                #region RT编码指令
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 2) == "rt")
                {
                    ClearYuDingInfo();
                    RTFlag = true;
                    PreRTCmd = cmd;
                    PreCmd = cmd;
                    return PreCmd;
                }
                #endregion RT编码指令

                //RT 预处理
                #region RT 预处理
                if (cmd.Trim().ToLower() == "rt")
                {
                    if (!YuDingFlag)
                    {
                        ErrorMessage = "无可显示的内容！";
                        return "";
                    }

                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion RT 预处理

                //XE 预处理
                #region XE 预处理
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 2) == "xe")
                {
                    //判断是否订座预处理状态
                    if (YuDingFlag)
                    {
                        //如果是订座预处理状态则直接删除缓存的对应指令，并返回预处理显示
                        if (!DelYuDingCommandByIndex(cmd.Trim().Substring(2), ref ErrorMessage))
                        {
                            return "";
                        }

                        //返回订座预处理显示
                        ErrorMessage = GetYuDingResult();
                        return "";
                    }
                    //是否取消编码指令
                    else if (cmd.Trim().Length > 5 && cmd.Trim().ToLower().Substring(0, 6) == "xepnr\\")
                    {
                        if (!RTFlag || PreRTCmd.Trim() == "")
                        {
                            ErrorMessage = "请先使用RT指令提取编码！";
                            return "";
                        }

                        if (cmd.Trim().Length < 12)
                        {
                            ErrorMessage = "XEPNR\\指令请跟对应的PNR编号，例如：XEPNR\\HYHEP3";
                            return "";
                        }

                        //判断编码是否一致
                        if (PreRTCmd.Trim().Substring(2).ToLower().Trim() != cmd.Trim().Substring(6).ToLower().Trim())
                        {
                            ErrorMessage = "RT编码与想取消的编码不一致，请确认！";
                            return "";
                        }

                        string sendcmd = PreRTCmd + "|XEPNR\\";
                        ClearRTInfo();
                        //发送连续指令取消PNR
                        return sendcmd;
                    }
                    //是否修改编码指令
                    else if (RTFlag)
                    {
                        string sendcmd = PreRTCmd + "|" + cmd;
                        ClearRTInfo();
                        return sendcmd;
                    }
                }
                #endregion XE 预处理

                //处理订座指令
                #region 处理订座指令

                #region SD
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 2) == "sd")
                {
                    if (PreAVHResult.Trim() == "")
                    {
                        ErrorMessage = "请先进行航班查询！";
                        return "";
                    }
                    if (cmd.Trim().Length != 5)
                    {
                        ErrorMessage = "请使用标准格式：SD序号 舱位/座位数，例如：SD1Y1";
                        return "";
                    }

                    //RT状态并且查询过航班信息，则发送RT+SS指令并封口
                    if (RTFlag&&PreRTCmd.Trim()!="" &&PreAVHResult.Trim()!="")
                    {
                        //组织连续指令
                        string sscmd = "";
                        if (GetSSCommandBySD(cmd, ref sscmd))
                        {
                            //清空RT状态
                            ClearRTInfo();
                            return PreRTCmd + "|" + sscmd + "\r@";
                        }
                        else
                        {
                            ErrorMessage = "指令处理异常，请还原指令再重试！";
                            return "";
                        }

                    }

                    //置订座标志为True
                    if (!YuDingFlag)
                    {
                        YuDingFlag = true;
                    }

                    //添加缓存指令
                    if (PreSDCmd.Trim()=="")
                    {
                        PreSDCmd = cmd;
                    }
                    else
                    {
                        ErrorMessage = "航段信息输入重复！";
                        return "";
                    }

                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion SD

                #region NM
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 2) == "nm")
                {
                    if (PreAVHResult.Trim() == "")
                    {
                        ErrorMessage = "请先进行航班查询！";
                        return "";
                    }

                    if (!YuDingFlag)
                    {
                        ErrorMessage="请先使用SD指令选择航段舱位！";
                        return "";
                    }

                    if (cmd.Trim().Length <4 || cmd.Trim().Substring(0,3).ToLower()!="nm1")
                    {
                        ErrorMessage = "请使用标准格式：NM1姓名1姓名，例如：NM1张三1李四";
                        return "";
                    }

                    PreNMCmd = cmd;
                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion NM

                #region CT
                if (cmd.Trim().Length > 2 && cmd.Trim().ToLower().Substring(0, 2) == "ct")
                {
                    if (PreAVHResult.Trim() == "")
                    {
                        ErrorMessage = "请先进行航班查询！";
                        return "";
                    }

                    if (!YuDingFlag)
                    {
                        ErrorMessage = "请先使用SD指令选择航段舱位！";
                        return "";
                    }

                    PreCTCmd = cmd;
                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion CT

                #region TKTL
                if (cmd.Trim().Length > 4 && cmd.Trim().ToLower().Substring(0, 4) == "tktl")
                {
                    if (PreAVHResult.Trim() == "")
                    {
                        ErrorMessage = "请先进行航班查询！";
                        return "";
                    }

                    if (!YuDingFlag)
                    {
                        ErrorMessage = "请先使用SD指令选择航段舱位！";
                        return "";
                    }

                    if (cmd.Trim().Length < 5 || cmd.Trim().Substring(0, 5).ToLower() != "tktl/")
                    {
                        ErrorMessage = "请使用标准格式：TKTL/时间/日期/Office号，例如：TKTL/1900/20JAN/CTU324";
                        return "";
                    }

                    PreTKTLCmd = cmd;
                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion TKTL

                #region SSR FOID
                if (cmd.Trim().Length > 8 && cmd.Trim().ToLower().Substring(0, 8) == "ssr foid")
                {
                    if (PreAVHResult.Trim() == "")
                    {
                        ErrorMessage = "请先进行航班查询！";
                        return "";
                    }

                    if (!YuDingFlag)
                    {
                        ErrorMessage = "请先使用SD指令选择航段舱位！";
                        return "";
                    }

                    //检查证件信息是否已经存在，如果存在则直接替换
                    if (!CheckAndReplaceSsrFoid(cmd,ref ErrorMessage))
                    {
                        return "";
                    }

                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion SSR FOID

                #region EI
                if (cmd.Trim().Length > 3 && cmd.Trim().ToLower().Substring(0, 3) == "ei:")
                {
                    if (PreAVHResult.Trim() == "")
                    {
                        ErrorMessage = "请先进行航班查询！";
                        return "";
                    }

                    if (!YuDingFlag)
                    {
                        ErrorMessage = "请先使用SD指令选择航段舱位！";
                        return "";
                    }

                    if (cmd.Trim().Length < 4)
                    {
                        ErrorMessage = "请使用标准格式：EI:退改签规定，例如：EI:不得退票，改签收费";
                        return "";
                    }

                    PreEICmd = cmd;
                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion EI

                #region 其他指令
                if (YuDingFlag)
                {
                    if (PreOtherCmd.Trim() == "")
                    {
                        PreOtherCmd = cmd;
                    }
                    else
                    {
                        PreOtherCmd += "|" + cmd;
                    }

                    //显示缓存指令结果
                    ErrorMessage = GetYuDingResult();
                    return "";
                }
                #endregion 其他指令

                #endregion 处理订座指令

                //其他指令连续发送
                #region 其他指令连续发送
                if (PreCmd != "")
                {
                    PreCmd += "|" + cmd;
                }
                else
                {
                    PreCmd = cmd;
                }
                return PreCmd;
                #endregion 其他指令连续发送
            }
            catch (Exception ex)
            {
                Log.Record("Error.log", "指令预处理操作出现异常，异常原因：" + ex.Message);
            }

            return strResult;
        }


        #region 根据28JUL07返回标准日期格式2007-07-28
        /// <summary>
        /// 根据28JUL07返回标准日期格式2007-07-28
        /// </summary>
        /// <param name="Date">返回日期格式（2007-03-06）</param>
        public static string getStandardDate(string Date)
        {
            if (Date.Length < 5)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            string tmpDate = Date.Substring(0, 2);

            string Month = Date.Substring(2, 3).ToUpper();

            string tmpMonth = "";

            switch (Month)
            {
                case "JAN":
                    tmpMonth = "01";
                    break;
                case "FEB":
                    tmpMonth = "02";
                    break;
                case "MAR":
                    tmpMonth = "03";
                    break;
                case "APR":
                    tmpMonth = "04";
                    break;
                case "MAY":
                    tmpMonth = "05";
                    break;
                case "JUN":
                    tmpMonth = "06";
                    break;
                case "JUL":
                    tmpMonth = "07";
                    break;
                case "AUG":
                    tmpMonth = "08";
                    break;
                case "SEP":
                    tmpMonth = "09";
                    break;
                case "OCT":
                    tmpMonth = "10";
                    break;
                case "NOV":
                    tmpMonth = "11";
                    break;
                case "DEC":
                    tmpMonth = "12";
                    break;
            }
            if (Date.Length >= 7)
            {
                if (Date.Substring(5, 2).Trim().Length == 2)
                {
                    return "20" + Date.Substring(5, 2) + "-" + tmpMonth + "-" + tmpDate;
                }
                else
                {
                    return DateTime.Now.ToString("yyyy") + "-" + tmpMonth + "-" + tmpDate;
                }
            }
            else
            {
                //判断日期是否小于当前日期，如果小于则把年份更改为明年
                string resDate = DateTime.Now.ToString("yyyy") + "-" + tmpMonth + "-" + tmpDate + " 23:59:59";
                if (DateTime.Compare(DateTime.Parse(resDate), DateTime.Now) < 0)
                {
                    //跨年
                    return DateTime.Parse(resDate).AddYears(1).ToString("yyyy-MM-dd");
                }
                else
                {
                    return resDate.Substring(0, 10);
                }
            }
        }
        #endregion 根据28JUL07返回标准日期格式2007-07-28

        #region 解析AVH结果，返回航班日期及最后一个航班时间
        public static bool AnalyseAVHGetLastDateTime(string sourcmd, string sourcontent,out string FlightDate ,out string LastTime)
        {
            FlightDate = "";
            LastTime = "";
            string destcontent = "";

            string tmpdate = "";
            try
            {
                //tmpdate = sourcmd.Substring(11, 5);

                //判断sourcontent的长度
                if (sourcontent.Length < 160)
                {
                    destcontent = "查询结果异常！";
                    return false;
                }

                //取得第一行 日期
                int pos1 = sourcontent.IndexOf("(");
                if (pos1 == -1)
                {
                    destcontent = "查询结果异常！";
                    return false;
                }

                //返回信息与查询日期不一致
                //if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
                //{
                //    destcontent = "当日无航班或者已经销售完毕！";
                //    return false;
                //}

                FlightDate = sourcontent.Substring(1, pos1 - 1);

                tmpdate = getStandardDate(sourcontent.Substring(1, pos1 - 1));

                #region AVH结果信息（例子）
                // 30AUG(THU) CTUBJS DIRECT ONLY                                                  
                //1-  CA4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>                                                                   T2 T3  2:35 
                //2  *ZH4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>   CA4193                                                          T2 T3  2:35 
                //3   3U8881  DS# FA PA AA YA BA TA WS HA MA GA  CTUPEK 0730   1005   330 0 S  E  
                //>               SA LA QA EA VS US RS KQ NQ XS ZS                    T1 T3  2:35 
                //4   CA4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>                                                                   T2 T3  2:20 
                //5  *ZH4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>   CA4113                                                          T2 T3  2:20 
                //6   KN2611  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>               NQ RQ SQ VQ TQ WQ GQ XQ QA US I3                    T2 --  1:50 
                //7  *MU8071  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>   KN2611      NQ RQ SQ VQ TQ WQ GQ XQ QA I3                       T2 --  1:50 
                //8   CA4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>                                                                   T2 T3  2:25 
                //9+ *ZH4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>   CA4101                                                          T2 T3  2:25 
                //**  CZ  FARE    CTUPEK/PEKCTU   YI:CZ/TZ305                                   
                #endregion AVH结果信息（例子）

                //使用\r分割字符串
                string[] sl = sourcontent.Split(new char[] { '\r','\n' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 
                string flongtime = "";//飞行时长

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();

                        byte[] tmpbuf = Encoding.Default.GetBytes(flightno);
                        if ((tmpbuf[tmpbuf.Length - 2] == 0x20) && (tmpbuf[tmpbuf.Length - 1] == 0x1C))
                        {
                            flightno = flightno.Substring(0, flightno.Length - 2).Trim();
                        }

                        //此处不做空格处理
                        tmpstr = tmpstr.Substring(10);

                        #region 跳过被屏蔽的航空公司信息
                        //判断是否被屏蔽的航空公司
                        if (tmpstr.Substring(0, 31).Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司信息

                        //舱位信息
                        tmpclassinfo = tmpstr.Substring(0, 31).Trim();
                        tmpstr = tmpstr.Substring(31).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        LastTime = tmpstr.Substring(0, 4);
                        tmpstr = tmpstr.Substring(6).Trim();

                        #endregion 分析航段信息的第一行数据
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                string tmpstr = e.Message;
                return false;
            }
        }
        #endregion 解析AVH结果，返回航班日期及最后一个航班时间

        #region 解析AVH结果，返回用于订座显示的信息
        //把AVH内容格式化为网站固定格式
        //valueType  1 用于显示订座信息   2 发送指令的SS订座信息 3 获取标准FD指令
        public static bool AnalyseAVHGetYuDingInfo(string AVHResult, int index, string classcode, int seatcount,int valueType,ref string displayinfo)
        {            
            string tmpdate = "";
            string displayStr = "";//用于显示
            string ssStr = "";//用于订座的SS指令  ss cz6966 b 10jan14 krlurc 1
            string ssDate = "";
            string fdStr = "";//用于FD标准指令
            try
            {
                //tmpdate = sourcmd.Substring(11, 5);

                //判断sourcontent的长度
                if (AVHResult.Length < 160)
                {
                    displayinfo = "查询结果异常！";
                    return false;
                }

                //取得第一行 日期
                int pos1 = AVHResult.IndexOf("(");
                if (pos1 == -1)
                {
                    displayinfo = "查询结果异常！";
                    return false;
                }

                //返回信息与查询日期不一致
                //if (sourcontent.Substring(0, pos1).ToUpper().IndexOf(tmpdate.ToUpper()) == -1)
                //{
                //    destcontent = "当日无航班或者已经销售完毕！";
                //    return false;
                //}

                tmpdate = AVHResult.Substring(pos1 + 1, 2) + AVHResult.Substring(1, pos1 - 1);
                ssDate = AVHResult.Substring(1, pos1 - 1);

                #region AVH结果信息（例子）
                // 30AUG(THU) CTUBJS DIRECT ONLY                                                  
                //1-  CA4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>                                                                   T2 T3  2:35 
                //2  *ZH4193  DS#                                CTUPEK 0700   0935   321 0^B  E  
                //>   CA4193                                                          T2 T3  2:35 
                //3   3U8881  DS# FA PA AA YA BA TA WS HA MA GA  CTUPEK 0730   1005   330 0 S  E  
                //>               SA LA QA EA VS US RS KQ NQ XS ZS                    T1 T3  2:35 
                //4   CA4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>                                                                   T2 T3  2:20 
                //5  *ZH4113  DS#                                CTUPEK 0800   1020   321 0^B  E  
                //>   CA4113                                                          T2 T3  2:20 
                //6   KN2611  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>               NQ RQ SQ VQ TQ WQ GQ XQ QA US I3                    T2 --  1:50 
                //7  *MU8071  DS# F8 P5 AS YA KS BA EA HA LA MA  CTUNAY 0805   0955   738 0    E  
                //>   KN2611      NQ RQ SQ VQ TQ WQ GQ XQ QA I3                       T2 --  1:50 
                //8   CA4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>                                                                   T2 T3  2:25 
                //9+ *ZH4101  DS#                                CTUPEK 0900   1125   321 0^   E  
                //>   CA4101                                                          T2 T3  2:25 
                //**  CZ  FARE    CTUPEK/PEKCTU   YI:CZ/TZ305                                   
                #endregion AVH结果信息（例子）

                //使用\r分割字符串
                string[] sl = AVHResult.Split(new char[] { '\r', '\n' });

                string flighttime = "";//起飞时间
                string arrivetime = "";//抵达时间
                string carrier = "";//航空公司编号
                string flightno = "";//航班号（如果为共享航班则带有*号）
                string shareflightno = "";//共享航班号
                string classinfo = "";//舱位信息
                string tmpclassinfo = "";
                string fromcity = "";//起飞城市
                string tocity = "";//抵达城市
                string plane = "";//机型
                string stopflag = "";//经停标志
                string eatflag = "";//餐食标志
                string etflag = "1";//电子客票
                string shareflag = "False";//共享航班标志
                string childclassinfo = "";//子舱位信息
                string flightterminal = "";//航站楼信息 
                string flongtime = "";//飞行时长

                string tmpstr = "";//临时处理字符串


                //循环处理
                for (int i = 0; i < sl.Length; i++)
                {
                    if (sl[i].Trim() == "")
                        continue;

                    //判断是否为第一行
                    char firstchar = sl[i][0];
                    int ifirst = 0;
                    int.TryParse(firstchar.ToString(), out ifirst);

                    //判断第一个字符是否为数字，如果为数字则认为是航班的第一行
                    if (ifirst != 0)
                    {
                        #region 分析航段信息的第一行数据
                        tmpstr = sl[i];
                        int tmppos = -1;

                        //判断是否共享航班
                        if (tmpstr.Substring(3, 1) == "*")
                        {
                            shareflag = "True";
                        }
                        else
                        {
                            shareflag = "false";
                        }

                        //承运人
                        carrier = tmpstr.Substring(4, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //航班号
                        flightno = tmpstr.Substring(0, 6).Trim();

                        byte[] tmpbuf = Encoding.Default.GetBytes(flightno);
                        if ((tmpbuf[tmpbuf.Length - 2] == 0x20) && (tmpbuf[tmpbuf.Length - 1] == 0x1C))
                        {
                            flightno = flightno.Substring(0, flightno.Length - 2).Trim();
                        }

                        //此处不做空格处理
                        tmpstr = tmpstr.Substring(10);

                        #region 跳过被屏蔽的航空公司信息
                        //判断是否被屏蔽的航空公司
                        if (tmpstr.Substring(0, 31).Trim() == "")
                        {
                            classinfo = "";
                            tmpclassinfo = "";
                            flighttime = "";//起飞时间
                            arrivetime = "";//抵达时间
                            carrier = "";//航空公司编号
                            flightno = "";//航班号（如果为共享航班则带有*号）
                            shareflightno = "";//共享航班号
                            fromcity = "";//起飞城市
                            tocity = "";//抵达城市
                            plane = "";//机型
                            stopflag = "";//经停标志
                            eatflag = "";//餐食标志
                            etflag = "1";//电子客票
                            shareflag = "False";//共享航班
                            childclassinfo = "";//子舱位信息
                            flightterminal = "";//航站楼信息
                            continue;
                        }
                        #endregion 跳过被屏蔽的航空公司信息

                        //舱位信息
                        tmpclassinfo = tmpstr.Substring(0, 31).Trim();
                        tmpstr = tmpstr.Substring(31).Trim();

                        //出发到达
                        fromcity = tmpstr.Substring(0, 3);
                        tocity = tmpstr.Substring(3, 3);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //起飞时间
                        flighttime = tmpstr.Substring(0, 2) + tmpstr.Substring(2, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //到达时间
                        arrivetime = tmpstr.Substring(0, 2) + tmpstr.Substring(2, 2);
                        tmpstr = tmpstr.Substring(6).Trim();

                        //机型
                        plane = tmpstr.Substring(0, 3);
                        tmpstr = tmpstr.Substring(4).Trim();

                        //经停
                        stopflag = tmpstr.Substring(0, 1);
                        //此处不能去空格
                        tmpstr = tmpstr.Substring(2);

                        //餐食标志
                        string eatstr="";
                        if (tmpstr.Substring(0, 1).Trim() == "")
                        {
                            eatflag = "0";
                        }
                        else
                        {
                            eatstr=tmpstr.Substring(0, 1).Trim();
                            eatflag = "1";
                        }
                        #endregion 分析航段信息的第一行数据

                        if (ifirst == index)
                        {
                            displayStr = "  " + carrier + flightno + " " + classcode + "   " +
                                fromcity + tocity + " DK" + seatcount.ToString() + "   " + flighttime + " " + arrivetime + "          " +
                                plane + " " + eatstr + " " + stopflag + "  R E ";
                            // ss cz6966 b 10jan14 krlurc 1
                            ssStr = "ss " + carrier + flightno + " " + classcode + " " + ssDate + " " + fromcity + tocity + " " + seatcount.ToString();
                            //FD:CTUPEK/20JAN/CA
                            fdStr = "FD:" + fromcity + tocity + "/" + ssDate + "/" + carrier;
                        }

                        continue;
                    }
                    else
                    {
                        //跳过被屏蔽的航空公司信息
                        if (carrier == "")
                        {
                            continue;
                        }

                        //2：航班第二行数据（舱位信息）   3：航班第三行数据（子舱位信息）
                        int rowindex = 2;
                        #region 判断是第二行数据还是第三行数据
                        tmpstr = sl[i].Trim();
                        if (tmpstr.Substring(0, 2) == "**")
                        {
                            //第三行子舱位信息
                            rowindex = 3;
                        }
                        else if (tmpstr.Substring(0, 1).Trim() != ">")
                        {
                            //其他信息直接处理下一行数据
                            continue;
                        }
                        #endregion 判断是第二行数据还是第三行数据

                        tmpstr = sl[i];
                        #region 第二行舱位数据信息处理
                        if (rowindex == 2)
                        {
                            //添加舱位信息
                            tmpclassinfo += " " + sl[i].Substring(11, 56).Trim();
                            tmpstr = tmpstr.Substring(68);

                            //航站楼信息
                            flightterminal = tmpstr.Substring(0, 2) + tmpstr.Substring(3, 2);
                            tmpstr = tmpstr.Substring(5).Trim();

                            //飞行时长
                            flongtime = tmpstr;

                            if ( displayStr.Trim() != "")
                            {
                                displayStr += flightterminal;

                                switch(valueType)
                                {
                                    case 1:                             
                                        displayinfo = displayStr;
                                        break;
                                    case 2:
                                        displayinfo = ssStr;
                                        break;
                                    case 3:
                                        displayinfo = fdStr;
                                        break;
                                }
                                return true;
                            }

                        }
                        #endregion 第二行舱位数据信息处理

                        #region 第三行子舱位信息处理
                        else
                        {
                            childclassinfo = tmpstr.Trim().Substring(2).Trim();
                        }
                        #endregion 第三行子舱位信息处理
                        continue;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                string tmpstr = e.Message;
                return false;
            }
        }
        #endregion 解析AVH结果，返回用于订座显示的信息
    }
}
