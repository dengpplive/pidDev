using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PBPid.Base
{
    /// <summary>
    /// Debug 的摘要说明。
    /// </summary>
    public class Debug
    {
        public static object lockf = new object();

        public Debug()
        {
        }
        public static void Write(System.Exception ex)
        {
            string file = System.Environment.CurrentDirectory + "\\exception.log";
            try
            {
                lock(lockf)
                {
                    System.IO.StreamWriter sw = new StreamWriter(file, true);
                    sw.WriteLine("#######################################");
                    sw.WriteLine("message:" + ex.Message);
                    sw.WriteLine("Source:" + ex.Source);
                    sw.WriteLine("StackTrace:" + ex.StackTrace);
                    sw.WriteLine("InnerException:" + ex.InnerException);
                    sw.WriteLine("#######################################");
                    sw.Close();
                }

                System.Diagnostics.Trace.WriteLine("#######################################");
                System.Diagnostics.Trace.WriteLine("message:" + ex.Message);
                System.Diagnostics.Trace.WriteLine("Source:" + ex.Source);
                System.Diagnostics.Trace.WriteLine("StackTrace:" + ex.StackTrace);
                System.Diagnostics.Trace.WriteLine("InnerException:" + ex.InnerException);
                System.Diagnostics.Trace.WriteLine("#######################################");
            }
            catch
            {
            }
        }
        public static void Write(string message)
        {
            string file = System.Environment.CurrentDirectory + "\\exception.log";
            try
            {
                lock(lockf)
                {
                    System.IO.StreamWriter sw = new StreamWriter(file, true);
                    sw.WriteLine("#######################################");
                    sw.WriteLine("message:" + message);
                    sw.WriteLine("#######################################");
                    sw.Close();
                }
            }
            catch
            {
            }
        }
    }
}
