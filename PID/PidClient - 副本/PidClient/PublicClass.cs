using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using PBPid.WebManage;

namespace PidClient
{
    static class PublicClass
    {
        //当前连接服务器
        public static string curServerIp = "";
        //当前连接服务器的端口
        public static int curServerPort = 350;

        //当前登录账号
        public static string curUserName = "";
        //当前账号密码
        public static string curUserPwd = "";

        //服务器连接
        public static TcpClient curTcpClient = new TcpClient(); 
      
        //Socket
        public static NetworkStream curTcpSocket = null;    
   
        //最大缓冲大小
        public static int MaxLen = 1024 * 1024;

        //所属Office号
        public static string curOfficeCode = "";

        //黑屏连接认证
        public static bool ConnectServer(ref string ErrorMessage)
        {
            try
            {
                if (curTcpClient.Connected)
                {
                    curTcpClient.Close();
                }
                curTcpClient.Connect(curServerIp, curServerPort);
                if (curTcpClient.Connected)
                {
                    curTcpSocket = curTcpClient.GetStream();

                    //组织认证指令
                    int count=0;
                    ArrayList a = new ArrayList();
                    byte[] headbuf={0x01,0xaa};
                    a.AddRange(headbuf);
                    byte[] contentbuf=Encoding.Default.GetBytes(curUserName+"\0\0\0"+curUserPwd+"\0\0\0");
                    a.AddRange(contentbuf);                 
                    count = headbuf.Length + contentbuf.Length;

                    //发送认证指令
                    curTcpSocket.Write((byte[])a.ToArray(typeof(byte)), 0, count);
                    curTcpSocket.Flush();

                    //读取返回结果
                    byte[] receivedbuf=new byte[1024];
                    int reccount = 0;
                    reccount = curTcpSocket.Read(receivedbuf, 0, 1024);

                    if(reccount>3 && receivedbuf[2]==0x01)
                    {
                        //获取所属Office号
                        byte[] officebuf=new byte[6];
                        Array.Copy(receivedbuf,reccount-6,officebuf,0,6);
                        curOfficeCode = Encoding.Default.GetString(officebuf);

                        //认证成功
                        return true;
                    }
                    else
                    {
                        if (reccount > 9)
                        {
                            byte[] errorbuf = new byte[reccount - 9];
                            Array.Copy(receivedbuf, 9, errorbuf, 0, reccount - 9);
                            ErrorMessage = Encoding.Default.GetString(errorbuf);
                        }
                        else
                        {
                            ErrorMessage = "未知通讯异常！";
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "服务器连接异常!";
                return false;
            }
            return false;
        }

        #region 根据汉字编码取得汉字信息
        /// <summary>
        /// 根据航信编码返回汉字信息
        /// </summary>
        /// <param name="hzBytes"></param>
        /// <returns></returns>
        public static string GetHanZiByHangXinBianMa(byte[] hzBytes)
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
        public static string GetPinYinBianMaFromHanZi(string HanZi, out byte[] HanZiBt)
        {
            HanZiBt = null;

            string StrPinYin = "";

            StrPinYin = chs2py.convert(HanZi, out HanZiBt);

            return StrPinYin;
        }
        #endregion

        #region 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// <summary>
        /// 把网络的命令包转换为航信命令数据包，主要是处理中文
        /// </summary>
        /// <param name="command">原指令</param>
        /// <param name="resbuf">结果数据包</param>
        /// <param name="needpinyin">是否需要拼音</param>
        public static void AnalyseWebCmdAndMakeServerInfo(string command, out byte[] resbuf, bool needpinyin)
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
        public static string AnalyseServerContent(byte[] buf, int count)
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

        /// <summary>
        /// 根据预订结果分析出对应的PNR编号
        /// </summary>
        /// <param name="strResult"></param>
        /// <returns></returns>
        public static string GetPNRCodeByYuDingResult(string strResult)
        {
            //
            string PNR="";
            string ErrorMessage="";
            if (WebManage.GetPNRNumByTKReturn(strResult, ref PNR, ref ErrorMessage))
            {
                return PNR;
            }
            else
            {
                return strResult;
            }
        }

        //封装待发送数据包
        public static byte[] GetSendBuf(string cmd,int cmdType)
        {
            try
            {
                byte[] headbuf={0x01, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00, 0x01, 0x41, 0x51, 
                                   0x70, 0x02, 0x1B, 0x0B, 0x20, 0x20, 0x00, 0x0F, 0x1E};
                byte[] endbuf={0x20, 0x03};

                switch (cmdType)
                { 
                        //标准指令
                    case 1:
                        break;
                        //发送 城市|城市|航班日期到PID端，正常返回 城市代码|城市代码|航班日期，否则返回错误消息
                    case 2:
                        headbuf[4] = 0xF0;
                        headbuf[5] = 0x01;
                        break;
                }

                byte[] cmdbuf=null;
                AnalyseWebCmdAndMakeServerInfo(cmd,out cmdbuf,true);

                //替换数据包长度字节
                short tmplen = (short)(headbuf.Length + endbuf.Length + cmdbuf.Length);
                short count2 = System.Net.IPAddress.HostToNetworkOrder(tmplen);
                byte[] lenbuf = BitConverter.GetBytes(count2);
                Array.Copy(lenbuf, 0, headbuf, 2, 2);

                ArrayList al = new ArrayList();
                al.AddRange(headbuf);
                al.AddRange(cmdbuf);
                al.AddRange(endbuf);

                return (byte[])(al.ToArray(typeof(byte)));
            }
            catch(Exception ex)
            {
                Log.Record("error.log","组织待发送指令数据包异常，异常原因："+ex.Message);
                return null;
            }
        }

        //发送指令并获取返回结果
        //cmdType 1:标准指令 2:自定义指令（判断城市、航班日期并返回对应的三字码或错误信息）
        public static bool SendCommand(string cmd,int cmdType, ref string strResult)
        {
            strResult = "";
            try
            {
                if (curTcpClient.Connected)
                {
                    //封装待发送指令
                    byte[] cmdbuf = GetSendBuf(cmd,cmdType);

                    //发送指令
                    if (cmdbuf != null)
                    {
                        curTcpSocket.Write(cmdbuf, 0, cmdbuf.Length);
                        curTcpSocket.Flush();                       

                        //读取返回结果
                        byte[] recbuf = new byte[MaxLen];
                        int recCount = curTcpSocket.Read(recbuf, 0, MaxLen);
                        if (recCount > 0)
                        {
                            strResult = AnalyseServerContent(recbuf, recCount);
                            return true;
                        }
                    }
                    else
                    {
                        strResult = "组织待发送指令出现异常！";
                        return false;
                    }
                }
                else
                {
                    strResult = "与服务器的连接已断开！";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Record("Error.log", "发送指令出现异常，异常原因：" + ex.Message);
                strResult = "操作出现异常！";
                return false;
            }
            return false;
        }


        //格式化返回结果
        public static string FormatResult(string cmd, string ResultInfo)
        {
            string[] sl = ResultInfo.Split(new char[] { '\r','\n'});
            ResultInfo = "";
            for (int i = 0; i < sl.Length; i++)
            {
                if (sl[i].Trim() == "")
                {
                    continue;
                }
                if (sl[i].Length > 80)
                {
                    ResultInfo += sl[i].Substring(0, 80) + "\n" + sl[i].Substring(80) + "\n";
                }
                else
                {
                    ResultInfo += sl[i]+"\n";
                }
            }

            ResultInfo = ResultInfo.Substring(0, ResultInfo.Length - 1);

            return ResultInfo;
        }

        //释放资源
        public static void Free()
        {
            try
            {
                if (curTcpClient.Connected)
                {
                    curTcpClient.Close();
                }
                curTcpSocket = null;
                curTcpClient = null;
            }
            catch (Exception ex)
            {
                Log.Record("Error.log", "释放资源出现异常，异常原因：" + ex.Message);
            }
        }
    }
}
