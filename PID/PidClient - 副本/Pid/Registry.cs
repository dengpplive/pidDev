using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PBPid.Base
{
    /// <summary>
    /// Regestry 的摘要说明。
    /// </summary>
    public class Regestry
    {
        public Regestry()
        {
        }
        public static RegistryKey Root
        {
            get
            {
                RegistryKey sw = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software", true);
                RegistryKey gowk = sw.OpenSubKey("ChinaCgt", true);
                if (gowk == null)
                    gowk = sw.CreateSubKey("ChinaCgt");
                sw.Close();
                return gowk;
                //	RegistryKey skin=gowk.CreateSubKey("skin");
            }
        }
        public static RegistryKey GetKey(string strkey)
        {
            RegistryKey r = Root;
            string[] keys = strkey.Split('\\');
            foreach (string key in keys)
            {
                if (key == null || key == string.Empty) continue;
                RegistryKey sub = r.OpenSubKey(key, true);
                if (sub == null)
                    sub = r.CreateSubKey(key);
                r.Close();
                r = sub;
            }
            return r;
        }
        public static object GetValue(string strkey)
        {
            RegistryKey k = GetKey(strkey.Substring(0, strkey.LastIndexOf(@"\") - 1));
            object o = k.GetValue(strkey.Substring(strkey.LastIndexOf(@"\") + 1));
            k.Close();
            return o;
        }
        public static void SetValue(string key, object value)
        {
            RegistryKey k = GetKey(key.Substring(0, key.LastIndexOf(@"\") - 1));
            k.SetValue(key.Substring(key.LastIndexOf(@"\") + 1), value);
            k.Close();
        }
    }
}
