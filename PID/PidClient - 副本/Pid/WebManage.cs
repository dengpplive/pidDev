using System;
using System.Collections.Generic;
using System.Text;
using PBPid.Base;
using System.Net.Sockets;
using System.Collections;


namespace PBPid.WebManage
{
    /// <summary>
    /// 网站访问PID类
    /// </summary>
    public class WebManage
    {
        /// <summary>
        /// PID的服务IP
        /// </summary>
        private static string m_PidServerIP = "";

        /// <summary>
        /// PID的服务端口
        /// </summary>
        private static int m_PidServerPort = 350;

        /// <summary>
        /// 网站访问帐号
        /// </summary>
        private static string m_WebUser = "webguest";

        /// <summary>
        /// 网站访问密码
        /// </summary>
        private static string m_WebPwd = "webguest";

        /// <summary>
        /// 网站请求数据包头
        /// </summary>
        private static readonly byte[] m_WebHeadPack = { 0xEE, 0xEF };

        /// <summary>
        /// 网站请求数据包尾
        /// </summary>
        private static readonly byte[] m_WebEndPack = { 0xFF, 0xEF };

        /// <summary>
        /// PID的服务IP
        /// </summary>
        public static string ServerIp
        {
            get { return m_PidServerIP; }
            set { m_PidServerIP = value; }
        }

        /// <summary>
        /// PID的服务端口
        /// </summary>
        public static int ServerPort
        {
            get { return m_PidServerPort; }
            set { m_PidServerPort = value; }
        }

        /// <summary>
        /// 网站访问帐号
        /// </summary>
        public static string WebUserName
        {
            get { return m_WebUser; }
            set { m_WebUser = value; }
        }

        /// <summary>
        /// 网站访问密码
        /// </summary>
        public static string WebPwd
        {
            get { return m_WebPwd; }
            set { m_WebPwd = value; }
        }

        /// <summary>
        /// 向PID发送指令并获取返回结果信息
        /// </summary>
        /// <param name="cmdContent">指令信息</param>
        /// <param name="OfficeCode">指定操作Office</param>
        /// <param name="PnFlag">是否最后一个指令自动PN（true：自动PN；false：不PN）</param>
        /// <param name="AllResult">是否返回所有结果信息（true：返回所有指令结果；false：只返回最后一个指令的结果）</param>
        /// <returns>结果信息</returns>
        public static string SendCommand(string cmdContent,string OfficeCode,bool PnFlag,bool AllResult)
        {
            string tmpByData = cmdContent;

            try
            {
                if (OfficeCode.Trim() != "")
                {
                    tmpByData += "<OFFICE>" + OfficeCode.Trim() + "</OFFICE>";
                }

                if (PnFlag)
                {
                    tmpByData += "<PN>1</PN>";
                }
                else
                {
                    tmpByData += "<PN>0</PN>";
                }

                if (AllResult)
                {
                    tmpByData += "<ALL>1</ALL>";
                }
                else
                {
                    tmpByData += "<ALL>0</ALL>";
                }

                string ReturnValue = "";

                //读取Web.Config配置中的服务端IP地址
                string IPAddress = m_PidServerIP;
                //读取Web.Config配置中的服务端端口
                int Port = m_PidServerPort;

                //读取Web.Config配置中的用户名
                string User = m_WebUser;
                //读取Web.Config配置中的密码
                string PWD = AppGlobal.EncryptMD5(m_WebPwd);

                //底层通讯协议
                //数据包格式：包头+类型+长度+内容+包尾
                //包头起始（2字节）0xFF,0xFE
                //类型（1字节）0x01
                //总长度（2字节）
                //用户名（10字节）
                //密码（32字节）
                //内容（）
                //包尾结束（2字节）0xFF,0xFF

                TcpClient client;                  //创建Tcp网络服务

                short wLength = 26;         // 整个数据包长度
                short wCmdLen = 0;          // 命令数据包长度

                byte byType = 0x01;

                //计算需要发送信息的二进制流长度
                wCmdLen = (short)((System.Text.Encoding.Default.GetBytes(tmpByData)).Length);

                wLength = (short)(wCmdLen + 39 + User.Length);

                wLength = System.Net.IPAddress.HostToNetworkOrder(wLength);

                ArrayList al = new ArrayList();
                al.AddRange(m_WebHeadPack);//报头长度2
                al.Add(byType);//长度1
                al.AddRange(BitConverter.GetBytes(wLength));//长度2
                al.AddRange(AppGlobal.GetBin(AppGlobal.StrToHex(User), 10));//长度10
                al.AddRange(AppGlobal.GetBin(AppGlobal.StrToHex(PWD), 32));//长度32
                al.AddRange(AppGlobal.GetBin(AppGlobal.StrToHex(tmpByData), tmpByData.Length));
                al.AddRange(m_WebEndPack);

                byte[] SendBytes = (byte[])al.ToArray(typeof(byte));

                try
                {
                    client = new TcpClient(IPAddress, Port);
                }
                catch (SocketException ex)
                {
                    ReturnValue = ex.Message;
                    return ReturnValue;
                }

                NetworkStream ns = client.GetStream();
                ns.Write(SendBytes, 0, SendBytes.Length);
                ns.Flush();

                byte[] data = new byte[1024 * 1024];

                int recv = ns.Read(data, 0, data.Length);
                if (recv == 0)
                {
                    ReturnValue = "";
                }
                else
                {
                    //检查数据包是否完整
                    if ((data[0] == m_WebHeadPack[0]) && (data[1] == m_WebHeadPack[1]) &&
                        (data[recv - 2] == m_WebEndPack[0]) && (data[recv - 1] == m_WebEndPack[1]))
                    {
                        byte[] tmpdata2 = new byte[recv - 4];
                        Array.Copy(data, 2, tmpdata2, 0, recv - 4);
                        //转换并解压缩
                        //Kevin 2010-12-20 Add
                        //添加数据包压缩、解压缩
                        ReturnValue = System.Text.Encoding.Default.GetString(tmpdata2);//AppGlobal.Decompress(System.Text.Encoding.Default.GetString(tmpdata2));
                        ReturnValue = ReturnValue.Replace('\r', '^');
                    }
                    else
                    {
                        ReturnValue = "接收到的数据包不完整！";
                    }
                }


                ns.Close();
                client.Close();

                return ReturnValue;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            
        }

        /// <summary>
        /// 航班查询
        /// </summary>
        /// <param name="FromCity">出发城市三字码</param>
        /// <param name="ToCity">到达城市三字码</param>
        /// <param name="Carrier">承运人代码</param>
        /// <param name="AirDate">乘机日期（格式：2012-09-10）</param>
        /// <param name="AirTime">起飞时间（格式：0930）</param>
        /// <param name="OfficeCode">配置Office号</param>
        /// <param name="AVORAVH">代理人配置输入："AVH"，航空公司配置输入："AV"</param>
        /// <param name="ResultContent">返回结果</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true：成功；false：失败</returns>
        public static bool PidSendCommand_av(string FromCity, string ToCity, string Carrier, string AirDate, string AirTime,string OfficeCode,string AVORAVH,ref string ResultContent,ref string ErrorMessage)
        {
            string sendcmd = "";
            ErrorMessage = "";
            ResultContent = "";
            try
            {
                //把日期转换为航信格式
                string AVDate = getEtermDate(AirDate);
                string AVTime = "";

                if (AirTime.IndexOf(":") != -1)
                {
                    if (AirTime.Substring(0, AirTime.IndexOf(":")).Length == 1)
                    {
                        AVTime += "0" + AirTime.Substring(0, AirTime.IndexOf(":"));
                    }
                    else
                    {
                        AVTime += AirTime.Substring(0, AirTime.IndexOf(":"));
                    }

                    if (AirTime.Substring(AirTime.IndexOf(":") + 1).Length == 1)
                    {
                        AVTime += "0" + AirTime.Substring(AirTime.IndexOf(":") + 1);
                    }
                    else
                    {
                        AVTime += AirTime.Substring(AirTime.IndexOf(":") + 1);
                    }
                }

                if (AVORAVH.Trim().ToUpper() == "AV")
                {
                    sendcmd = "AV:" + FromCity + ToCity + "/" + AVDate + "/" + AVTime;
                }
                else
                {
                    sendcmd = "AVH/" + FromCity + ToCity + "/" + AVDate + "/" + AVTime;
                }

                if(Carrier.Trim()!="")
                {
                    sendcmd += "/" + Carrier.ToUpper();
                }

                sendcmd += "/D";

                ResultContent = SendCommand(sendcmd, OfficeCode, true, true);
                return true;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            return false;
        }

        /// <summary>
        /// 把大配置提取的PNR内容格式化为代理人的PNR标准格式
        /// </summary>
        /// <param name="BigPNRContent">大配置提取到的PNR内容</param>
        /// <param name="PNR">小编码</param>
        /// <param name="PNRContent">格式化后的PNR内容</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true：成功；false：失败</returns>
        public static bool FormatBigPNRContent(string BigPNRContent,string PNR, ref string PNRContent, ref string ErrorMessage)
        {
            PNRContent = BigPNRContent;
            ErrorMessage = "";
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            return false;
        }

        /// <summary>
        /// 解析预订编码返回内容中的PNR编码
        /// </summary>
        /// <param name="StrTKReturn">预订座位返回结果内容</param>
        /// <param name="PNR">PNR编号</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true：解析成功；false：解析失败</returns>
        public static bool GetPNRNumByTKReturn(string StrTKReturn, ref string PNR, ref string ErrorMessage)
        {       
            PNR="";
            ErrorMessage="";
            try
            {
                // 验证返回结果
                if (IsError(StrTKReturn.ToUpper().Trim()))
                {
                    Log.Record("Error.log", "航班预订返回错误，返回内容：" + StrTKReturn);
                    ErrorMessage = "预订失败，返回结果："+StrTKReturn;    // 订座失败
                    return false;
                }
                else
                {
                    //航空公司配置
                    //FVTWY - 请及时出票, 自动出票时限: 12SEP09/2140
                    //3U8881  D TU22SEP  CTUPEK HK1   0720 0940 
                    //- ADD SSR TKNE FOR NEW ET SEGMENT 
                    //|1

                    //代理人配置
                    //SGP7V -EOT SUCCESSFUL, BUT ASR UNUSED FOR 1 OR MORE SEGMENTS
                    //CA4117  Y SA12SEP  CTUPEK DK1   1700 1920 
                    //航空公司使用自动出票时限, 请检查PNR  

                    int tmpi = -1;
                    tmpi = StrTKReturn.ToUpper().IndexOf("EOT SUCCESSFUL");
                    int tmpi2 = -1;
                    tmpi2 = StrTKReturn.ToUpper().IndexOf("- 请及时出票");
                    if (tmpi != -1)
                    {
                        if (tmpi - 8 >= 0)
                        {
                            PNR = StrTKReturn.Substring(tmpi - 8, 6).Trim();     // 返回PNR编号
                        }
                        else
                        {
                            PNR = StrTKReturn.Substring(tmpi - 7, 6).Trim();     // 返回PNR编号
                        }
                        return true;
                    }
                    else if (tmpi2 != -1)
                    {
                        PNR = StrTKReturn.Substring(0, 6).Trim();
                        return true;
                    }
                    else
                    {
                        string[] PNRInfoList = StrTKReturn.Split(new char[1] { '^' });

                        if (PNRInfoList.Length > 2)
                        {
                            for (int k = PNRInfoList.Length - 1; k > 0; k--)
                            {
                                if (PNRInfoList[k].Length > 5)
                                {
                                    PNR = PNRInfoList[k].Substring(0, 6).Trim();
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            Log.Record("Error.log", "航班预订返回错误，返回内容：" + StrTKReturn);
                            ErrorMessage = "预订失败，返回结果：" + StrTKReturn;    // 订座失败
                            return false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorMessage=ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 根据代理人PNR内容获取小编码和大编码
        /// </summary>
        /// <param name="PNRContent">PNR内容</param>
        /// <param name="PNR">小编码</param>
        /// <param name="BigPNR">大编码</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true：成功；false：失败</returns>
        public static bool GetPNRAndBigPNRByPNRContent(string PNRContent, string PNR, ref string BigPNR,ref string ErrorMessage)
        {
            PNR = "";
            BigPNR = "";
            ErrorMessage = "";
            string tmpStr = "";
            try
            {
                string strResult = "";                
                int pos = PNRContent.IndexOf(". ");
                if (pos == -1)
                {
                    ErrorMessage = "解析PNR内容失败，请检查PNR内容格式是否正确！";
                    return false;
                }

                //判断是否为团队
                bool uflag = false;
                int upos = PNRContent.IndexOf("0.");
                int upos2 = PNRContent.IndexOf("1.");
                if ((upos != -1) && (upos < upos2))
                {
                    uflag = true;
                }

                //string bigpnr = "";

                //string tmpstr = PNRContent.Substring(0, pos);
                ////没有在航段信息之前查找到PNR编号，则认为是航空公司配置提取的编码
                //if (tmpstr.ToLower().IndexOf(pnr.ToLower()) == -1)
                //{
                //    //如果是团队，则查询1.之前的PNR，并替换
                //    if (uflag)
                //    {
                //        int i = oldPnrContent.IndexOf("1.");
                //        if (i == -1)
                //        {
                //            return pnr;
                //        }
                //        else
                //        {
                //            tmpstr = oldPnrContent.Substring(0, i);
                //            int j = tmpstr.LastIndexOf("^");
                //            if (j == -1)
                //            {
                //                return oldPnrContent.Replace("^", "\r\n");
                //            }
                //            tmpstr = tmpstr.Substring(0, j).Trim();
                //            bigpnr = tmpstr.Substring(tmpstr.Length - 6, 6).Trim();

                //            return bigpnr + "|" + pnr;
                //        }
                //    }
                //    else
                //    {
                //        //不是团队，则查询(. )之前的PNR，并替换
                //        int i = oldPnrContent.IndexOf(". ");
                //        if (i == -1)
                //        {
                //            return pnr;
                //        }
                //        else
                //        {
                //            tmpstr = oldPnrContent.Substring(0, i);
                //            int j = tmpstr.LastIndexOf("^");
                //            if (j == -1)
                //            {
                //                return oldPnrContent.Replace("^", "\r\n");
                //            }
                //            tmpstr = tmpstr.Substring(0, j).Trim();
                //            bigpnr = tmpstr.Substring(tmpstr.Length - 6, 6).Trim();
                //            return bigpnr + "|" + pnr;
                //        }
                //    }
                //}
                //else
                //{
                //    //14.RMK CA/HJP39 
                //    pos = oldPnrContent.ToLower().IndexOf("rmk ca/");
                //    if (pos != -1)
                //    {
                //        bigpnr = oldPnrContent.Substring(pos + 7, 6).Trim();
                //    }

                //    return bigpnr + "|" + pnr;
                //}



                ////航空公司大编码
                ////13.RMK CA/D8NQS
                //int i = PNRContent.IndexOf(".RMK CA/");

                
                //if (i == -1)
                //{
                //    //**ELECTRONIC TICKET PNR** ^
                //    //1.雷秀英 NK50MJ^
                //    //    2. HU7356 Y TH28JUL CTUCSX RR1 1615 1755 E ^
                //    //        3.NC ^
                //    //        4.NC ^ 5.T^ 6.SSR FOID HU HK1 NI511124197005281729/P1 ^ 7.SSR TKNE HU HK1 CTUCSX 7356 Y28JUL 8802129259776/1/P1^ 8.SSR OTHS 1E PNR RR AND PRINTED ^ 9.SSR TKTL HU XX/ CTU 1400/28JUL ^10.OSI CA CTC ^11.OSI HU CTCT ^12.OSI 1E HUET TN/880-2129259776/P1 ^13.RMK ET ^14.RMK ET ^15.RMK ET ^16.RMK ET ^17.RMK ET ^18.RMK ET ^19.RMK ET ^20.FN/M/^21.TN/880-2129259776/P1 ^22.FP/CC^23.HAK969
                //    int pos = PNRContent.IndexOf(". ");
                //    tmpStr = PNRContent.Substring(0, pos);
                //    pos = tmpStr.IndexOf("^");
                //    tmpStr = tmpStr.Substring(0, pos).Trim();
                //    pos = tmpStr.LastIndexOf(" ");

                //    BigPNR = tmpStr.Substring(pos).Trim();
                //}
                //else
                //{
                //    BigPNR = PNRContent.Substring(i + 8, 6);
                //}

                //小编码




                


                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 根据PNR内容分析编码状态
        /// </summary>
        /// <param name="PNRContent">PNR内容</param>
        /// <param name="PNRStatus">PNR状态（HK RR NO HL LL UK CANCELED）</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>true：成功；false：失败</returns>
        public static bool GetPNRStatus(string PNRContent, ref string PNRStatus,ref string ErrorMessage)
        {
            PNRStatus = "";
            ErrorMessage = "";
            try
            {
                //已被取消
                if (PNRContent.IndexOf("*THIS PNR WAS ENTIRELY CANCELLED*") != -1)
                {
                    PNRStatus = "CANCELLED";
                    ErrorMessage = "编码已被取消";
                    return true;
                }                   

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 验证航班订座是否成功
        /// </summary>
        /// <param name="strMessage">返回的字符串</param>
        /// <returns>true：订座失败；false：订座成功</returns>
        public static bool IsError(string strMessage)
        {
            bool ReturnValue = false;

            #region 错误信息数组

            string[] strError = new string[57];

            strError[0] = "ACTION";
            strError[1] = "AIRLINE";
            strError[2] = "CHECK CONTINUITY";
            strError[3] = "CONTACT ELEMENT MISSING";
            strError[4] = "DATE";
            strError[5] = "ELE NBR";
            strError[6] = "FLT NUMBER";
            strError[7] = "FORMAT";
            strError[8] = "ILIEGAL";
            strError[9] = "INFANT";
            strError[10] = "INVALID CHAR";
            strError[11] = "MAX TIME FOR EOT - IGNORE PNR AND RESART";
            strError[12] = "NAME LENGTH";
            strError[13] = "NAMES";
            strError[14] = "NO DISPALY";
            strError[15] = "NO NAME CHANGE FOR MU/Y";
            strError[16] = "NO QUEUE";
            strError[17] = "OFFICE";
            strError[18] = "PENDING";
            strError[19] = "PLEASE SIGN IN FIRST";
            strError[20] = "PLS INPUT FULL TICKET NUMBER";

            strError[21] = "PLS NM1XXXX/XXXXXX";
            strError[22] = "PROFILE PENDING";
            strError[23] = "PROT SET";
            strError[24] = "Q TYPE";
            strError[25] = "Q EMPTY";
            strError[26] = "QUE PENDING";
            strError[27] = "CHARACTER";
            strError[28] = "SCH NBR";
            strError[29] = "SEATS";
            strError[30] = "ERROR";
            strError[31] = "SIMULTANEOUS MODIFICATION-REENTER MODIFICATION";
            strError[32] = "TICKET PRINTER IN USE";
            strError[33] = "TIME";
            strError[34] = "UNABLE";
            strError[35] = "USER GRP";
            strError[36] = "WORKING Q";
            strError[37] = "CHECK TKT DATE";
            strError[38] = "INVALID CHARACTER";
            strError[39] = "FORMAT IS 'TIME/DATE'";
            strError[40] = "PNR CANCELLED";
            strError[41] = "TIMEOUT!,WAIT A MONENT INPUT";
            strError[42] = "SEATS/NAMES";
            strError[43] = "CHECK NAME";
            strError[44] = "CHECK TICKET";
            strError[45] = "FUNCTION";
            strError[46] = "DUPLICATE SEGMENT";
            strError[47] = "INVALID FOID";
            strError[48] = "NO PNR";
            strError[49] = "CHECK TKT TIME";
            strError[50] = "CHECK CONTINUITY  CONNECTION";
            strError[51] = "超出字库GB2312范围";
            strError[52] = "PLEASE WAIT - TRANSACTION IN PROGRESS";
            strError[53] = "ACTION CODE";
            strError[54] = "TIMEOUT!,WAIT A MONENT INPUT";
            strError[55] = "CLASS";
            strError[56] = "SERVICE TYPE";

            #endregion

            for (int i = 0; i < strError.Length; i++)
            {
                if (strMessage.IndexOf(strError[i]) != -1)
                {
                    ReturnValue = true;
                    break;
                }
                else
                {
                    ReturnValue = false;
                }
            }

            return ReturnValue;
        }

        /// <summary>
        /// 根据标准日期格式(2009-01-20)返回20JAN09
        /// </summary>
        /// <param name="Date">返回日期格式（20JAN09）</param>
        public static string getEtermDate(string date)
        {
            string strResult = date.Substring(8, 2);
            string strYear = "";

            //判断是否跨年
            if (DateTime.Now.Year < int.Parse(date.Substring(0, 4)))
            {
                strYear = date.Substring(2, 2);
            }

            string month = date.Substring(5, 2);
            if (month == "01")
            {
                strResult += "JAN";
            }
            else if (month == "02")
            {
                strResult += "FEB";
            }
            else if (month == "03")
            {
                strResult += "MAR";
            }
            else if (month == "04")
            {
                strResult += "APR";
            }
            else if (month == "05")
            {
                strResult += "MAY";
            }
            else if (month == "06")
            {
                strResult += "JUN";
            }
            else if (month == "07")
            {
                strResult += "JUL";
            }
            else if (month == "08")
            {
                strResult += "AUG";
            }
            else if (month == "09")
            {
                strResult += "SEP";
            }
            else if (month == "10")
            {
                strResult += "OCT";
            }
            else if (month == "11")
            {
                strResult += "NOV";
            }
            else if (month == "12")
            {
                strResult += "DEC";
            }

            if (strYear == "")
            {
                return " " + strResult;
            }
            else
            {
                return strResult + strYear;
            }
        }

        /// <summary>
        /// 根据28JUL07返回标准日期格式
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

        #region 测试内容，屏蔽
        ///// <summary>
        ///// 向PID发送指令并获取返回结果信息
        ///// </summary>
        ///// <param name="cmdContent">指令信息</param>
        ///// <param name="OfficeCode">指定操作Office</param>
        ///// <returns>结果信息</returns>
        //public byte[] TestSendCommand(string cmdContent, string OfficeCode)
        //{
        //    string tmpByData = cmdContent;

        //    if (OfficeCode.Trim() != "")
        //    {
        //        tmpByData += "<OFFICE>" + OfficeCode.Trim() + "</OFFICE>";
        //    }

        //    string ReturnValue = "";

        //    //读取Web.Config配置中的服务端IP地址
        //    string IPAddress = m_PidServerIP;
        //    //读取Web.Config配置中的服务端端口
        //    int Port = m_PidServerPort;

        //    //读取Web.Config配置中的用户名
        //    string User = m_WebUser;
        //    //读取Web.Config配置中的密码
        //    string PWD = AppGlobal.EncryptMD5(m_WebPwd);

        //    //底层通讯协议
        //    //数据包格式：包头+类型+长度+内容+包尾
        //    //包头起始（2字节）0xFF,0xFE
        //    //类型（1字节）0x01
        //    //总长度（2字节）
        //    //用户名（10字节）
        //    //密码（32字节）
        //    //内容（）
        //    //包尾结束（2字节）0xFF,0xFF

        //    TcpClient client;                  //创建Tcp网络服务

        //    short wLength = 26;         // 整个数据包长度
        //    short wCmdLen = 0;          // 命令数据包长度

        //    byte byType = 0x01;

        //    //计算需要发送信息的二进制流长度
        //    wCmdLen = (short)((System.Text.Encoding.Default.GetBytes(tmpByData)).Length);

        //    wLength = (short)(wCmdLen + 39 + User.Length);

        //    wLength = System.Net.IPAddress.HostToNetworkOrder(wLength);

        //    ArrayList al = new ArrayList();
        //    al.AddRange(m_WebHeadPack);//报头长度2
        //    al.Add(byType);//长度1
        //    al.AddRange(BitConverter.GetBytes(wLength));//长度2
        //    al.AddRange(AppGlobal.GetBin(AppGlobal.StrToHex(User), 10));//长度10
        //    al.AddRange(AppGlobal.GetBin(AppGlobal.StrToHex(PWD), 32));//长度32
        //    al.AddRange(AppGlobal.GetBin(AppGlobal.StrToHex(tmpByData), tmpByData.Length));
        //    al.AddRange(m_WebEndPack);

        //    byte[] SendBytes = (byte[])al.ToArray(typeof(byte));

        //    return SendBytes;
        //}

        #endregion 测试内容，屏蔽
    }
}
