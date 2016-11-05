using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text; 
using System.Text.RegularExpressions;
using System.Data;

namespace PBPid.Base
{
    /// <summary>
    /// 站点通用调用方法
    /// </summary>
    public class AppGlobal
    {
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="EncryptConfigString">加密字符串</param>
        public static string Deciphering(string EncryptConfigString)
        {
            string mstrDeciphering = String.Empty;
            byte[] data = Convert.FromBase64String(EncryptConfigString);
            mstrDeciphering = System.Text.ASCIIEncoding.ASCII.GetString(data);
            return mstrDeciphering;
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="ConfigString">需要加密的字符串</param>
        public static string Encrypting(string ConfigString)
        {
            string mstrEncrypting = String.Empty;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigString);
            mstrEncrypting = Convert.ToBase64String(data);
            return mstrEncrypting;
        }

        public static string StringToMD5Hash(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 10进制字符串转化成16进制字符串
        /// </summary>
        /// <param name="str">10进制字符串</param>
        /// <returns>16进制字符串</returns>
        public static string StrToHex(string str)
        {
            string strTemp = "";
            byte[] bTemp = System.Text.Encoding.Default.GetBytes(str);

            for (int i = 0; i < bTemp.Length; i++)
            {
                strTemp += bTemp[i].ToString("X");
            }
            return strTemp;
        }

        /// <summary>
        /// 将字符串转化成byte数组
        /// </summary>
        /// <param name="HexBin">字符串</param>
        /// <param name="Length">字符定长</param>
        /// <returns>byte数组</returns>
        public static byte[] GetBin(string HexBin, int Length)
        {
            ArrayList rtn = new ArrayList();
            string temp = string.Empty;
            int i = 0;
            for (; i < Length - HexBin.Length; i++)
            {
                temp = temp + "0";
            }
            HexBin = temp + HexBin;
            i = 0;
            while (i < HexBin.Length)
            {
                rtn.Add(Convert.ToByte(GetIntFromString(HexBin.Substring(i, 2))));
                i = i + 2;
            }

            return (byte[])rtn.ToArray(typeof(byte));
        }

        /// <summary>
        /// 格式化16进制字符串
        /// </summary>
        /// <param name="InputStr">16进制字符串</param>
        public static int GetIntFromString(string InputStr)
        {
            return GetBin(InputStr.Substring(0, 1)) * 16 + GetBin(InputStr.Substring(1, 1));
        }

        /// <summary>
        /// 获取16进制字符串对应的数字
        /// </summary>
        /// <param name="Temp">16进制字符串</param>
        public static int GetBin(string Temp)
        {
            switch (Temp)
            {
                case "A":
                    return 10;
                case "B":
                    return 11;
                case "C":
                    return 12;
                case "D":
                    return 13;
                case "E":
                    return 14;
                case "F":
                    return 15;

                default:
                    return Convert.ToInt32(Temp);
            }
        }

        /// <summary>
        /// 根据日期获取月份的前3位英语字母
        /// </summary>
        /// <param name="Date">日期（2007-03-06 | 2007-3-6）</param>
        public static string getSimplyMonth(string Date)
        {
            int LastIndex = Date.LastIndexOf('-');
            int FirstIndex = Date.IndexOf('-');
            string Month = Date.Substring(5, LastIndex - (FirstIndex + 1));
            if (Month.Length == 1)
            {
                Month = "0" + Month;
            }

            switch (Month)
            {
                case "01":
                    return "Jan";
                case "02":
                    return "Feb";
                case "03":
                    return "Mar";
                case "04":
                    return "Apr";
                case "05":
                    return "May";
                case "06":
                    return "Jun";
                case "07":
                    return "Jul";
                case "08":
                    return "Aug";
                case "09":
                    return "Sep";
                case "10":
                    return "Oct";
                case "11":
                    return "Nov";
                case "12":
                    return "Dec";
            }

            return "NULL";
        }


        /// <summary>
        /// 判断一个字符串中是否只有英文字母
        /// </summary>
        /// <param name="pendingString">需验证的字符串</param>
        public static bool Check(string pendingString)
        {
            pendingString = pendingString.Replace(" ", "");

            if (Regex.IsMatch(pendingString, @"^[\da-zA-Z/]+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断一个字符串是否全部只有中文汉字
        /// </summary>
        /// <param name="pendingString">需验证的字符串</param>
        public static bool CheckAllChinese(string pendingString)
        {
            pendingString = pendingString.Replace(" ", "");

            char[] tmpString = pendingString.ToCharArray();
            for (int i = 0; i < tmpString.Length; i++)
            {
                int At = (int)tmpString[i];
                if (At < 128)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 组合后的乘机人姓名
        /// 1:张亮    == ZhangLiang张晶亮
        /// 2:张Liang   == Zhang张Liang
        /// </summary>
        public static string CheckChinese(string pendingStirng, DataTable DTable)
        {
            if (Check(pendingStirng))
                return pendingStirng;

            bool AllChinese = CheckAllChinese(pendingStirng.Replace("CHD", "")); // 判断一个字符串是否全部只有中文汉字

            DataRow[] DRow;
            string pinyin = ""; // 存放解析后的汉字拼音

            pendingStirng = pendingStirng.Replace(" ", "");
            char[] tmpString = pendingStirng.ToCharArray();

            int arrange = tmpString.Length;
            bool IsChild = false;                              // 默认成人票

            if (pendingStirng.IndexOf("CHD") != -1) // 儿童票
            {
                IsChild = true;
                arrange = arrange - 3;
            }


            if (AllChinese)
            {
                // 全中文
                for (int i = 0; i < arrange; i++)
                {
                    string tmp = tmpString[i].ToString();

                    DRow = DTable.Select("chinese = '" + tmp + "'");
                    if (DRow.Length > 0)
                    {
                        pinyin = pinyin + DRow[0]["pingyin"].ToString();
                    }
                    else
                    {
                        pinyin = pinyin + tmp;
                    }
                }

                return pinyin + pendingStirng;
            }
            else
            {
                // 中英文混合
                for (int i = 0; i < arrange; i++)
                {
                    string tmp = tmpString[i].ToString();

                    if (Check(tmp))
                    {
                        pinyin = pinyin + tmp;       // 如果是英文字母
                    }
                    else
                    {
                        DRow = DTable.Select("chinese = '" + tmp + "'");
                        if (DRow.Length > 0)
                        {
                            pinyin = pinyin + DRow[0]["pingyin"].ToString() + tmp; ;
                        }
                        else
                        {
                            pinyin = pinyin + tmp;
                        }
                    }
                }

                if (IsChild)
                    pinyin = pinyin + "CHD";    // 儿童票

                return pinyin;
            }
        }

        //**ELECTRONIC TICKET PNR** 
        // 1.陈开异 QGE4Z 
        // 2.  CA4107 Y   FR27JUL  CTUPEK RR1   1000 1225          E  
        // 3.CTU/T CTU/T 028-5446666/CTU JING YING AVATION PASSENGER & CARGO AGNECY/LUO   
        //    ZHEN YING ABCDEFG   
        // 4.EM GENPNR JY6
        // 5.68223229   
        // 6.T  
        // 7.SSR FOID CA HK1 NI51102619441204023X/P1  
        // 8.SSR TKNE CA HK1 CTUPEK 4107 Y27JUL 9995611295223/1/P1
        // 9.RMK CA/KBQHG 
        //10.RMK AUTOMATIC FARE QUOTE                                                    +
        //>
        //11.FN/A/FCNY1440.00/SCNY1440.00/C3.00/XCNY130.00/TCNY50.00CN/TCNY80.00YQ/      -
        //    ACNY1570.00 
        //12.TN/999-5611295223/P1 
        //13.FP/CASH,CNY  
        //14.CTU295 


        /// <summary>
        /// 解析PNR内容
        /// </summary>
        public static string AutoCheckTicketInfo2(string PNRContent)
        {
            try
            {
                string MessageInfo = PNRContent;

                string fengefu = "^";

                if (MessageInfo.IndexOf("^") != -1)
                {
                    fengefu = "^";
                }
                else
                {
                    fengefu = "\n";
                }

                string strResult = "";

                int i = MessageInfo.IndexOf("NO PNR");

                if (i != -1)
                    return "NO PNR";

                i = MessageInfo.IndexOf("THIS PNR WAS ENTIRELY CANCELLED");
                if (i != -1)
                    return "CANCELLED";

                if (MessageInfo.Length < 20)
                    return MessageInfo;

                //返回格式： 乘客名称,证件号码,票号,票面价,销售价,机建,燃油,其他税费,乘客类型|乘客名称,证件号码,票号,票面价,销售价,机建,燃油,其他税费,乘客类型
                string[] PassengerInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] PassengerType = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] NIInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] TicketInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] TicketPriceInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] SalesPriceInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] BuildPriceInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] OilPricesInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };
                string[] OtherPricesInfo = new string[99];// { "", "", "", "", "", "", "", "", "" };

                //初始化数组
                for (int z = 0; z < 99; z++)
                {
                    PassengerInfo[z] = "";
                    PassengerType[z] = "";
                    NIInfo[z] = "";
                    TicketInfo[z] = "";
                    TicketPriceInfo[z] = "";
                    SalesPriceInfo[z] = "";
                    BuildPriceInfo[z] = "";
                    OilPricesInfo[z] = "";
                    OtherPricesInfo[z] = "";
                }

                int index = 0;
                //乘客名称的起始位置
                i = MessageInfo.IndexOf("1.");
                //乘客名称的结束位置
                int j = MessageInfo.IndexOf(". ");
                //取得乘客姓名相关信息

                if ((i == -1) || (j == -1)) return "-1";

                string tmpstr = MessageInfo.Substring(i, j - i).Trim();

                i = tmpstr.LastIndexOf(fengefu);

                if (i == -1) return "-1";

                tmpstr = tmpstr.Substring(0, i - 1).Trim();

                string PNR = tmpstr.Substring(tmpstr.Length - 5, 5);

                tmpstr = tmpstr.Substring(0, tmpstr.Length - 5).Trim();

                i = tmpstr.IndexOf(Convert.ToString(index + 1) + ".");
                j = tmpstr.IndexOf(Convert.ToString(index + 2) + ".");

                string tmpPassengerName = "";
                int k = -1;
                int l = -1;
                int m = -1;

                while ((i != -1) && (j != -1))
                {
                    //超出范围
                    //if (index > 9) break;

                    tmpPassengerName = tmpstr.Substring(i + 1 + Convert.ToString(index + 1).Length, j - i - 2 - Convert.ToString(index + 2).Length).Trim();

                    //处理儿童的名字，去掉CHD
                    k = tmpPassengerName.IndexOf("CHD");
                    m = tmpPassengerName.IndexOf(" CHD");
                    l = tmpPassengerName.IndexOf("/");
                    if ((m != -1) && (l != -1))
                    {
                        PassengerInfo[index] = tmpPassengerName.Substring(0, m).Trim();
                        PassengerType[index] = "儿童";
                    }
                    else if ((k != -1) && (l == -1))
                    {
                        PassengerInfo[index] = tmpPassengerName.Substring(0, k).Trim();
                        PassengerType[index] = "儿童";
                    }
                    else
                    {
                        PassengerInfo[index] = tmpPassengerName.Trim();
                        PassengerType[index] = "成人";
                    }

                    index++;

                    i = tmpstr.IndexOf(Convert.ToString(index + 1) + ".");
                    j = tmpstr.IndexOf(Convert.ToString(index + 2) + ".");
                }

                if (i != -1)
                {
                    tmpPassengerName = tmpstr.Substring(i + 1 + Convert.ToString(index + 1).Length, tmpstr.Length - i - 1 - Convert.ToString(index + 1).Length);


                    //处理儿童的名字，去掉CHD
                    k = tmpPassengerName.IndexOf("CHD");
                    m = tmpPassengerName.IndexOf(" CHD");
                    l = tmpPassengerName.IndexOf("/");
                    if ((m != -1) && (l != -1))
                    {
                        PassengerInfo[index] = tmpPassengerName.Substring(0, m).Trim();
                        PassengerType[index] = "儿童";
                    }
                    else if ((k != -1) && (l == -1))
                    {
                        PassengerInfo[index] = tmpPassengerName.Substring(0, k).Trim();
                        PassengerType[index] = "儿童";
                    }
                    else
                    {
                        PassengerInfo[index] = tmpPassengerName.Trim();
                        PassengerType[index] = "成人";
                    }
                }
                else
                {
                    index--;
                }

                //记录婴儿位置
                int BabyIndex = -1;

                //查找是否存在婴儿
                //9.XN/IN/马晓涌INF(MAR01)/P1
                i = MessageInfo.IndexOf("XN/IN/");
                while (i != -1)
                {
                    tmpstr = MessageInfo.Substring(i);
                    j = tmpstr.IndexOf("INF");

                    //超出范围
                    //if (index == 9) break;

                    index++;

                    if (j != -1)
                    {
                        if (BabyIndex == -1)
                        {
                            BabyIndex = index;
                        }

                        PassengerInfo[index] = tmpstr.Substring(6, j - 6).Trim();
                        PassengerType[index] = "婴儿";

                        tmpstr = tmpstr.Substring(j + 3);
                        i = tmpstr.IndexOf("XN/IN/");
                    }
                }

                string tmpstr2 = "";
                string tmpstr3 = "";

                //查找证件号码信息
                //7.SSR FOID 3U HK1 NI441621198507085546/P2         
                //8.SSR FOID 3U HK1 NI44512219820216561X/P1
                i = MessageInfo.IndexOf("SSR FOID");
                tmpstr2 = MessageInfo;
                while (i != -1)
                {
                    tmpstr = tmpstr2.Substring(i);
                    //只有一个乘客
                    if (index == 0)
                    {
                        j = tmpstr.IndexOf("/P1");
                        k = tmpstr.IndexOf("NI");
                        if ((j != -1) && (k != -1))
                        {
                            NIInfo[0] = tmpstr.Substring(k + 2, j - k - 2);
                            tmpstr2 = tmpstr.Substring(j + 3);
                        }
                        else
                        {
                            tmpstr2 = tmpstr.Substring(8);
                        }
                        i = tmpstr2.IndexOf("SSR FOID");
                        continue;
                    }
                    else
                    {
                        j = tmpstr.IndexOf("/P");
                        k = tmpstr.IndexOf("NI");
                        l = tmpstr.IndexOf(fengefu);
                        if ((j != -1) && (k != -1) && (l != -1))
                        {
                            tmpstr3 = tmpstr.Substring(0, l).Trim();
                            // if ((Convert.ToInt32(tmpstr.Substring(j + 2, 1)) - 1) <= 9)
                            //{
                            NIInfo[Convert.ToInt32(tmpstr3.Substring(j + 2).Trim()) - 1] = tmpstr.Substring(k + 2, j - k - 2);
                            //}
                            tmpstr2 = tmpstr.Substring(j + 3);
                        }
                        else
                        {
                            if (j != -1)
                            {
                                tmpstr2 = tmpstr.Substring(j + 3);
                            }
                            else
                            {
                                tmpstr2 = tmpstr.Substring(8);
                            }
                        }
                        i = tmpstr2.IndexOf("SSR FOID");
                        continue;
                    }
                }

                //查找票号信息
                //22.TN/999-6970423705/P1 
                //23.TN/999-6970423706/P2 
                i = MessageInfo.IndexOf("TN/");
                tmpstr2 = MessageInfo;

                while (i != -1)
                {
                    tmpstr = tmpstr2.Substring(i);
                    //只有一个乘客
                    if (index == 0)
                    {
                        j = tmpstr.IndexOf("-");
                        if (j != -1)
                        {
                            TicketInfo[0] = tmpstr.Substring(j + 1, 10);
                            tmpstr2 = tmpstr.Substring(j + 1);
                        }
                        else
                        {
                            tmpstr2 = tmpstr.Substring(3);
                        }


                    }
                    else
                    {
                        //乘客序号
                        j = tmpstr.IndexOf("/P");
                        k = tmpstr.IndexOf("-");
                        l = tmpstr.IndexOf(fengefu);
                        if ((j != -1) && (k != -1))
                        {
                            try
                            {
                                tmpstr3 = tmpstr.Substring(0, l).Trim();
                                //if ((Convert.ToInt32(tmpstr.Substring(j + 2, 1)) - 1) <= 9)
                                //{
                                TicketInfo[Convert.ToInt32(tmpstr3.Substring(j + 2).Trim()) - 1] = tmpstr.Substring(k + 1, j - k - 1);
                                //}
                            }
                            catch (Exception ex)
                            {

                            }
                            tmpstr2 = tmpstr.Substring(j + 3);
                        }
                        else
                        {
                            if (j != -1)
                            {
                                tmpstr2 = tmpstr.Substring(j + 3);
                            }
                            else
                            {
                                tmpstr2 = tmpstr.Substring(3);
                            }
                        }
                    }

                    i = tmpstr2.IndexOf("TN/");
                }

                //查找价格信息

                //成人
                //11.FN/FCNY1440.00/SCNY1440.00/C3.00/XCNY130.00/TCNY50.00CN/TCNY80.00YQ/        -
                //ACNY1570.00 

                //婴儿
                //12.FN/IN/FCNY140.00/SCNY140.00/C0.00/TEXEMPTCN/TEXEMPTYQ/ACNY140.00/P1 

                //儿童
                //9.FN/FCNY720.00/SCNY720.00/C3.00/XCNY40.00/TEXEMPTCN/TCNY40.00YQ/ACNY760.00

                i = MessageInfo.IndexOf("FN/");
                string FCNY = "0";
                string SCNY = "0";
                string CN = "0";
                string YQ = "0";
                string Other = "0";

                tmpstr3 = "";

                int p, q, r, s, t, u, v, w = 0;

                tmpstr2 = MessageInfo;
                while (i != -1)
                {
                    tmpstr = tmpstr2.Substring(i);

                    j = tmpstr.IndexOf(fengefu);

                    //取得当前价格行信息
                    if (j != -1)
                    {
                        tmpstr3 = tmpstr.Substring(0, j);

                        k = tmpstr3.IndexOf("/P");

                        //是否固定乘客序号
                        if (k != -1)
                        {
                            //判断是否为婴儿
                            if (tmpstr3.IndexOf("TEXEMPTCN/TEXEMPTYQ") != -1)
                            {
                                if (BabyIndex != -1)
                                {
                                    p = tmpstr3.IndexOf("FCNY");
                                    q = tmpstr3.IndexOf("SCNY");
                                    r = tmpstr3.IndexOf("/C");

                                    for (int z = BabyIndex; z <= index; z++)
                                    {
                                        if ((p != -1) && (q != -1) && (r != -1))
                                        {
                                            TicketPriceInfo[z] = tmpstr3.Substring(p + 4, q - p - 5);
                                            SalesPriceInfo[z] = tmpstr3.Substring(q + 4, r - q - 4);
                                            BuildPriceInfo[z] = "0";
                                            OilPricesInfo[z] = "0";
                                            OtherPricesInfo[z] = "0";
                                        }
                                    }
                                }
                            }
                            //儿童的价格
                            //9.FN/FCNY720.00/SCNY720.00/C3.00/XCNY40.00/TEXEMPTCN/TCNY40.00YQ/ACNY760.00
                            else if (tmpstr3.IndexOf("TEXEMPTCN") != -1)
                            {
                                p = tmpstr3.IndexOf("FCNY");
                                q = tmpstr3.IndexOf("SCNY");
                                r = tmpstr3.IndexOf("/C");
                                s = tmpstr3.IndexOf("XCNY");
                                t = tmpstr3.IndexOf("TEXEMPTCN");
                                u = tmpstr3.IndexOf("TCNY");
                                v = tmpstr3.IndexOf("YQ/");
                                l = tmpstr3.IndexOf(fengefu);

                                if ((p != -1) && (q != -1) && (r != -1) && (s != -1) && (t != -1) && (u != -1) && (v != -1) && (l != -1))
                                {
                                    TicketPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr3.Substring(p + 4, q - p - 5);
                                    SalesPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr3.Substring(q + 4, r - q - 4);
                                    BuildPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = "0";
                                    OilPricesInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr3.Substring(u + 4, v - u - 4);
                                    OtherPricesInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] =
                                        Convert.ToString(Convert.ToDecimal(tmpstr3.Substring(s + 4, t - s - 5))
                                            - Convert.ToDecimal(OilPricesInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())]));
                                }
                            }
                            //成人的价格
                            else
                            {
                                p = tmpstr3.IndexOf("FCNY");
                                q = tmpstr3.IndexOf("SCNY");
                                r = tmpstr3.IndexOf("/C");
                                s = tmpstr3.IndexOf("XCNY");
                                t = tmpstr3.IndexOf("TCNY");
                                w = tmpstr3.IndexOf("CN/");

                                string tmpstr4 = "";

                                if (t != -1)
                                {
                                    //取得"TCNY"后的内容
                                    tmpstr4 = tmpstr3.Substring(t);

                                    u = tmpstr4.IndexOf("CN/");
                                    v = tmpstr4.IndexOf("YQ/");
                                }
                                else
                                {
                                    u = -1;
                                    v = -1;
                                }

                                if ((p != -1) && (q != -1) && (r != -1) && (s != -1) && (t != -1) && (u != -1) && (v != -1))
                                {

                                    TicketPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr3.Substring(p + 4, q - p - 5);
                                    SalesPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr3.Substring(q + 4, r - q - 4);
                                    BuildPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr3.Substring(t + 4, w - t - 4);
                                    OilPricesInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] = tmpstr4.Substring(u + 7, v - u - 7);
                                    OtherPricesInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())] =
                                        Convert.ToString(Convert.ToDecimal(tmpstr3.Substring(s + 4, t - s - 5))
                                            - Convert.ToDecimal(OilPricesInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())])
                                            - Convert.ToDecimal(BuildPriceInfo[Convert.ToInt32(tmpstr3.Substring(k + 2).Trim())]));
                                }
                            }
                            tmpstr2 = tmpstr.Substring(4);
                            i = tmpstr2.IndexOf("FN/");
                            continue;
                        }
                        //所有乘客价格
                        else
                        {
                            //是否儿童
                            //9.FN/FCNY720.00/SCNY720.00/C3.00/XCNY40.00/TEXEMPTCN/TCNY40.00YQ/ACNY760.00
                            if (tmpstr3.IndexOf("TEXEMPTCN") != -1)
                            {
                                p = tmpstr3.IndexOf("FCNY");
                                q = tmpstr3.IndexOf("SCNY");
                                r = tmpstr3.IndexOf("/C");
                                s = tmpstr3.IndexOf("XCNY");
                                t = tmpstr3.IndexOf("TEXEMPTCN");
                                u = tmpstr3.IndexOf("TCNY");
                                v = tmpstr3.IndexOf("YQ/");

                                if ((p != -1) && (q != -1) && (r != -1) && (s != -1) && (t != -1) && (u != -1) && (v != -1))
                                {
                                    for (int z = 0; z <= index; z++)
                                    {
                                        if (TicketPriceInfo[z].Trim() == "")
                                        {
                                            TicketPriceInfo[z] = tmpstr3.Substring(p + 4, q - p - 5);
                                            SalesPriceInfo[z] = tmpstr3.Substring(q + 4, r - q - 4);
                                            BuildPriceInfo[z] = "0";
                                            OilPricesInfo[z] = tmpstr3.Substring(u + 4, v - u - 4);
                                            OtherPricesInfo[z] = Convert.ToString(Convert.ToDecimal(tmpstr3.Substring(s + 4, t - s - 5))
                                                    - Convert.ToDecimal(OilPricesInfo[z]));
                                        }
                                    }
                                }
                            }
                            //成人
                            //11.FN/FCNY1440.00/SCNY1440.00/C3.00/XCNY130.00/TCNY50.00CN/TCNY80.00YQ/        -
                            //ACNY1570.00 
                            else
                            {
                                p = tmpstr3.IndexOf("FCNY");
                                q = tmpstr3.IndexOf("SCNY");
                                r = tmpstr3.IndexOf("/C");
                                s = tmpstr3.IndexOf("XCNY");
                                t = tmpstr3.IndexOf("TCNY");
                                w = tmpstr3.IndexOf("CN/");

                                string tmpstr4 = "";

                                if (t != -1)
                                {
                                    //取得"TCNY"后的内容
                                    tmpstr4 = tmpstr3.Substring(t);

                                    u = tmpstr4.IndexOf("CN/");
                                    v = tmpstr4.IndexOf("YQ/");
                                }
                                else
                                {
                                    u = -1;
                                    v = -1;
                                }

                                if ((p != -1) && (q != -1) && (r != -1) && (s != -1) && (t != -1) && (u != -1) && (v != -1))
                                {
                                    for (int z = 0; z <= index; z++)
                                    {
                                        if (TicketPriceInfo[z].Trim() == "")
                                        {
                                            TicketPriceInfo[z] = tmpstr3.Substring(p + 4, q - p - 5);
                                            SalesPriceInfo[z] = tmpstr3.Substring(q + 4, r - q - 4);
                                            BuildPriceInfo[z] = tmpstr3.Substring(t + 4, w - t - 4);
                                            OilPricesInfo[z] = tmpstr4.Substring(u + 7, v - u - 7);
                                            OtherPricesInfo[z] = Convert.ToString(Convert.ToDecimal(tmpstr3.Substring(s + 4, t - s - 5))
                                                    - Convert.ToDecimal(OilPricesInfo[z]) - Convert.ToDecimal(BuildPriceInfo[z]));
                                        }
                                    }
                                }
                            }

                            //继续下一循环
                            tmpstr2 = tmpstr.Substring(4);
                            i = tmpstr2.IndexOf("FN/");
                            continue;
                        }


                    }
                    //取得"FN/"后面的内容，并继续循环
                    else
                    {
                        tmpstr2 = tmpstr.Substring(3);
                        i = tmpstr2.IndexOf("FN/");
                        continue;
                    }
                }

                //分析航段信息
                int fIndex = 0;

                i = MessageInfo.IndexOf(". ");
                j = -1;
                tmpstr2 = MessageInfo;

                string tmpFlightNo, tmpClass, tmpDate, tmpFrom, tmpTo, tmpStatus, tmpTakeOffTime, tmpArriveTime;

                while ((i != -1) && ((j == -1) || (i == j)))
                {
                    tmpstr3 = tmpstr2.Substring(i + 1).Trim();
                    j = tmpstr3.IndexOf(fengefu);

                    if (j != 0)
                    {
                        //取得航段行信息
                        tmpstr = tmpstr3.Substring(0, j).Trim();

                        //截取剩余信息
                        tmpstr2 = tmpstr3.Substring(j + 1);
                        i = tmpstr2.IndexOf(". ");
                        j = tmpstr2.IndexOf(".");
                    }
                    else
                        break;

                    // 2.  CZ3402 H   TU17JUL  CTUCAN RR1   1655 1845          E      
                    tmpFlightNo = "";
                    tmpClass = "";
                    tmpDate = "";
                    tmpFrom = "";
                    tmpTo = "";
                    tmpStatus = "";
                    tmpTakeOffTime = "";
                    tmpArriveTime = "";
                    fIndex = 0;

                    string[] tmpSlist = tmpstr.Split(new char[] { ' ' });

                    for (int g = 0; g < tmpSlist.Length - 1; g++)
                    {
                        if (tmpSlist[g].Trim() == "")
                            continue;

                        switch (fIndex)
                        {
                            case 0:
                                if (tmpSlist[g].Substring(1, 1) == "*")
                                {
                                    tmpFlightNo = tmpSlist[g].Trim().Substring(1);
                                }
                                else
                                {
                                    tmpFlightNo = tmpSlist[g].Trim();
                                }
                                fIndex++;
                                break;
                            case 1:
                                tmpClass = tmpSlist[g].Trim();
                                fIndex++;
                                break;
                            case 2:
                                tmpDate = getStandardMonth(tmpSlist[g].Trim());
                                //处理日期和出发到达连在一起的情况
                                if (tmpSlist[g].Trim().Length == 15)
                                {
                                    tmpFrom = tmpSlist[g].Trim().Substring(9, 3);
                                    tmpTo = tmpSlist[g].Trim().Substring(12, 3);
                                    fIndex++;
                                    fIndex++;
                                }
                                else
                                {
                                    fIndex++;
                                }
                                break;
                            case 3:
                                tmpFrom = tmpSlist[g].Trim().Substring(0, 3);
                                tmpTo = tmpSlist[g].Trim().Substring(3, 3);
                                fIndex++;
                                break;
                            case 4:
                                tmpStatus = tmpSlist[g].Trim().Substring(0, 2);
                                fIndex++;
                                break;
                            case 5:
                                tmpTakeOffTime = tmpSlist[g].Trim();
                                fIndex++;
                                break;
                            case 6:
                                tmpArriveTime = tmpSlist[g].Trim();
                                fIndex++;
                                break;
                        }
                    }
                    if (strResult == "")
                    {
                        strResult = "{" + tmpFlightNo + "," + tmpClass + "," + tmpDate + "," + tmpFrom + "," +
                            tmpTo + "," + tmpStatus + "," + tmpTakeOffTime + "," + tmpArriveTime;
                    }
                    else
                    {
                        strResult += "|" + tmpFlightNo + "," + tmpClass + "," + tmpDate + "," + tmpFrom + "," +
                            tmpTo + "," + tmpStatus + "," + tmpTakeOffTime + "," + tmpArriveTime;
                    }
                }
                strResult += "}";

                //组织返回信息  {航段信息：航班号，舱位，日期，出发，到达，状态，起飞时间，到达时间}{乘客信息：姓名，证件号码，票号，票面价，销售价，机建，燃油，其他税费，乘客类型}
                for (int h = 0; h <= index; h++)
                {
                    if (PassengerInfo[h].Trim() == "")
                    {
                        continue;
                    }

                    if (h == 0)
                    {
                        strResult += "{" + PassengerInfo[h] + "," + NIInfo[h] + "," + TicketInfo[h]
                            + "," + TicketPriceInfo[h] + "," + SalesPriceInfo[h]
                            + "," + BuildPriceInfo[h] + "," + OilPricesInfo[h] + "," + OtherPricesInfo[h] + "," + PassengerType[h];
                    }
                    else
                    {
                        strResult += "|" + PassengerInfo[h] + "," + NIInfo[h] + "," + TicketInfo[h]
                            + "," + TicketPriceInfo[h] + "," + SalesPriceInfo[h]
                            + "," + BuildPriceInfo[h] + "," + OilPricesInfo[h] + "," + OtherPricesInfo[h] + "," + PassengerType[h];
                    }

                }

                strResult += "}{" + PNR + "}";

                return strResult;
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }

        /// <summary>
        /// 根据日期（TU17JUL）获取标准日期（yyyy-mm-dd）
        /// </summary>
        /// <param name="Date">日期（TU17JUL）</param>
        public static string getStandardMonth(string Date)
        {
            //TU17JUL
            string tmpDate = Date.Trim();

            string tmpDay = tmpDate.Substring(2, 2);

            string tmpMonth = tmpDate.Substring(4, 3);

            string strMonth = "0";

            switch (tmpMonth)
            {
                case "JAN":
                    strMonth = "01";
                    break;
                case "FEB":
                    strMonth = "02";
                    break;
                case "MAR":
                    strMonth = "03";
                    break;
                case "APR":
                    strMonth = "04";
                    break;
                case "MAY":
                    strMonth = "05";
                    break;
                case "JUN":
                    strMonth = "06";
                    break;
                case "JUL":
                    strMonth = "07";
                    break;
                case "AUG":
                    strMonth = "08";
                    break;
                case "SEP":
                    strMonth = "09";
                    break;
                case "OCT":
                    strMonth = "10";
                    break;
                case "NOV":
                    strMonth = "11";
                    break;
                case "DEC":
                    strMonth = "12";
                    break;
            }

            if (Convert.ToInt32(DateTime.Today.Month) <= Convert.ToInt32(strMonth))
            {
                return Convert.ToString(DateTime.Today.Year) + "-" + strMonth + "-" + tmpDay;
            }
            else
            {
                return Convert.ToString(DateTime.Today.Year + 1) + "-" + strMonth + "-" + tmpDay;
            }
        }

        /// <summary>
        /// 根据28JUL07返回标准日期格式
        /// </summary>
        /// <param name="Date">日期（2007-03-06 | 2007-3-6）</param>
        public static string getStandardDate(string Date)
        {
            string tmpDate = Date.Substring(0, 2);

            string Month = Date.Substring(2, 3);

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

            return "20" + Date.Substring(5, 2) + "-" + tmpMonth + "-" + tmpDate;
        }

        /// <summary>
        /// 取得全拼音字母
        /// 1:张亮    == ZhangLiang
        /// 2:张Liang   == ZhangLiang
        /// </summary>
        public static string GetPinYin(string pendingStirng, DataTable DTable)
        {
            string tmpResult = "";
            for (int i = 0; i < pendingStirng.Length; i++)
            {
                DataRow[] dw;

                dw = DTable.Select("chinese = '" + pendingStirng[i].ToString() + "'");

                if (dw.Length > 0)
                {
                    tmpResult += dw[0]["pingyin"].ToString();
                }
                else
                {
                    tmpResult += pendingStirng[i].ToString();
                }
            }
            return tmpResult;
        }

        /// 生成加密字符串
        /// </summary>
        /// <param name="PasswordString">需加密的字符串</param>
        /// <param name="PasswordFormat">加密格式</param>
        public static string EncryptPassword(string PasswordString, string PasswordFormat)
        {
            string returnPasswordString = String.Empty;

            switch (PasswordFormat.ToUpper())
            {
                case "SHA1":
                    //returnPasswordString = FormsAuthentication.HashPasswordForStoringInConfigFile(
                    //    PasswordString, "SHA1");
                    break;
                case "MD5":
                    returnPasswordString = StringToMD5Hash(PasswordString);
                    break;
                default:
                    returnPasswordString = "";
                    break;
            }
            return returnPasswordString;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="password">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static String EncryptMD5(string password)
        {
            return EncryptPassword(password, "MD5");
        }


        public static string DeleteUnVisibleChar(string sourceString)
        {
            System.Text.StringBuilder sbuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sbuilder.Append(sourceString[i].ToString());
                }
            }

            return sbuilder.ToString();
        }

        //判断字符是否英文半角字符或标点
        //32    空格
        //33-47    标点
        //48-57    0~9
        //58-64    标点
        //65-90    A~Z
        //91-96    标点
        //97-122    a~z
        //123-126  标点
        public static bool IsBjChar(char c)
        {
            int i = (int)c;
            return i >= 32 && i <= 126;
        }

        /// 判断字符是否全角字符或标点
        /// 全角字符 - 65248 = 半角字符
        /// 全角空格例外

        public static bool IsQjChar(char c)
        {
            if (c == '\u3000')
            {
                return true;
            }
            int i = (int)c - 65248;
            if (i < 32)
            {
                return false;
            }
            else
            {
                return IsBjChar((char)i);
            }
        }

        /// 将字符串中的全角字符转换为半角
        public static string ToBj(string s)
        {
            if (s == null || s.Trim() == string.Empty)
            {
                return s;
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(s.Length);
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '\u3000')
                    {
                        sb.Append('\u0020');
                    }
                    else if (IsQjChar(s[i]))
                    {
                        sb.Append((char)((int)s[i] - 65248));
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }
                return sb.ToString();
            }
        }

        //字符串压缩
        public static string Compress(string strSource)
        {
            if (strSource == null || strSource.Length > 1024 * 1024)
                throw new System.ArgumentException("字符串为空或长度太大！");

            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            byte[] buffer = encoding.GetBytes(strSource);
            //byte[] buffer = Convert.FromBase64String(strSource); //传入的字符串不一定是Base64String类型，这样写不行

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();

            buffer = ms.ToArray();
            ms.Close();

            return Convert.ToBase64String(buffer); //将压缩后的byte[]转换为Base64String
        }

        //字符串解压
        public static string Decompress(string strSource)
        {
            //System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            //byte[] buffer = encoding.GetBytes(strSource);
            byte[] buffer = Convert.FromBase64String(strSource);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Position = 0;
            System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress);
            stream.Flush();

            int nSize = 1024 * 1024 + 256; //字符串不会超过16K
            byte[] decompressBuffer = new byte[nSize];
            int nSizeIncept = stream.Read(decompressBuffer, 0, nSize);
            stream.Close();

            return System.Text.Encoding.Unicode.GetString(decompressBuffer, 0, nSizeIncept);//转换为普通的字符串
        }

    }
}
