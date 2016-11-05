using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PidClient
{
    public class PublicInfo
    {
         /// <summary>
         /// 已重载.计算两个日期的时间间隔,返回的是时间间隔的日期差的秒数.
         /// </summary>
         /// <param name="DateTime1">第一个日期和时间</param>
         /// <param name="DateTime2">第二个日期和时间</param>
         /// <returns></returns>
         static public int DateDiff(DateTime DateTime1,DateTime DateTime2)
         {
             //string dateDiff=null;
             try
             {
                 TimeSpan ts1=new   TimeSpan(DateTime1.Ticks);
                 TimeSpan ts2=new   TimeSpan(DateTime2.Ticks);
                 TimeSpan ts=ts1.Subtract(ts2).Duration();

                 return ts.Minutes*60+ts.Seconds;
             }
             catch
             {

             }
             return 0;
         }


         /// <summary>
         /// 是否为双字节字符。
         /// </summary>
         static public bool IsTwoBytesChar(char chr)
         {
             string str = chr.ToString();
             // 使用中文支持编码
             Encoding ecode = Encoding.GetEncoding("GB18030");
             if (ecode.GetByteCount(str) == 2)
             {
                 return true;
             }
             else
             {
                 return false;
             }
         }
    }


    /// <summary>
    /// 无排序哈希表
    /// </summary>
    public class NoSortHashTable : Hashtable
    {
        private ArrayList list = new ArrayList();
        public override void Add(object key, object value)
        {
            base.Add(key, value);
            list.Add(key);
        }
        public override void Clear()
        {
            base.Clear();
            list.Clear();
        }
        public override void Remove(object key)
        {
            base.Remove(key);
            list.Remove(key);
        }
        public override ICollection Keys
        {
            get
            {
                return list;
            }
        }
    }
}
