using System;
using System.Net;
using System.Net.Sockets;

namespace PBPid.Base
{
    /// <summary>
    /// Address 的摘要说明。
    /// </summary>
    public class Address
    {
        public Address()
        {
        }
        //获取外网IP，如果没有则获取192的地址
        public static IPAddress GetExternalIPAddress()
        {
            IPAddress ret = null;
            string host = Dns.GetHostName();
            IPHostEntry ety = Dns.GetHostByName(host);

            //记录192的地址

            foreach (IPAddress ip in ety.AddressList)
            {
                byte cls = ip.GetAddressBytes()[0];
                if (cls != 127 && cls != 169 && cls != 10 && cls != 192 && (cls < 224 || cls > 239))
                {
                    ret = ip;
                    break;
                }
                else if(cls==192)
                {
                    ret = ip;
                }
            }
            return ret;
        }

        //获取第一个IP
        public static IPAddress GetFirstIPAddress()
        {
            string host = Dns.GetHostName();
            return Dns.GetHostByName(host).AddressList[0];
        }

        //取得10.150的IP地址（用于移动控制台）
        public static string GetDlanIP()
        {
            string host = Dns.GetHostName();
            IPHostEntry ety = Dns.GetHostByName(host);

            //记录192的地址

            foreach (IPAddress ip in ety.AddressList)
            {
                byte cls = ip.GetAddressBytes()[0];
                byte cls2 = ip.GetAddressBytes()[1];
                if (cls == 10)
                {
                    return ip.ToString();
                }
            }
            return "";
        }
    }
}
