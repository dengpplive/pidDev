using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Data;

namespace IClientAttributeSpace
{
    public interface IClientAttribute
    {
        string userName { get; set; }
        string userPass { get; set; }
        string onlyIdentity { get; set; }
        string cmd { get; set; }
    }

    [Serializable]
    public class ClientInfo : IClientAttribute
    {
        public ClientInfo(string _userName, string _userPass, string _onlyIdentity, string _cmd)
        {
            userName = _userName;
            userPass = _userPass;
            onlyIdentity = _onlyIdentity;
            cmd = _cmd;
        }

        #region IClientAttribute
        public string userName
        {
            get;
            set;
        }

        public string userPass
        {
            get;
            set;
        }

        public string onlyIdentity
        {
            get;
            set;
        }

        public string cmd
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        public DataSet AddUser(DataSet ds, string userName, string userPass)
        {
            DataRow dr = ds.Tables["users"].NewRow();
            dr["userName"] = userName;
            dr["userPass"] = userPass;
            ds.Tables["users"].Rows.Add(dr);
            ds.AcceptChanges();
            return ds;
        }
        /// <summary>
        /// 对该用户追加命令
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataSet AddCmd(DataSet ds, string userName, string cmd)
        {
            DataRow dr1 = ds.Tables["cmds"].NewRow();
            dr1["userName"] = userName;
            dr1["cmd"] = cmd;
            dr1["cmdDateTime"] = DateTime.Now;
            ds.Tables["cmds"].Rows.Add(dr1);
            ds.AcceptChanges();
            return ds;
        }

        /// <summary>
        /// 删除用户和相关命令
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataSet Delete(DataSet ds, string userName)
        {
            ds.Tables["users"].Select("userName='" + userName + "'")[0].Delete();

            DataRow[] drs = ds.Tables["cmds"].Select("userName='" + userName + "'");
            foreach (DataRow item in drs)
                item.Delete();

            ds.AcceptChanges();

            return ds;
        }

        public DataSet DeleteCmd(DataSet ds, string userName,string cmd)
        {
            DataRow[] drs = ds.Tables["cmds"].Select("userName='" + userName + "' and cmd ='" + cmd + "'");
            foreach (DataRow item in drs)
                item.Delete();
            ds.AcceptChanges();
            return ds;
        }

        public DataSet DeleteCmd(DataSet ds, string userName)
        {
            DataRow[] drs = ds.Tables["cmds"].Select("userName='" + userName + "'");
            foreach (DataRow item in drs)
                item.Delete();
            ds.AcceptChanges();
            return ds;
        }
        /// <summary>
        /// 返回一个数据集包含2个空表
        /// </summary>
        /// <returns></returns>
        public DataSet CreateCaCheTable(DataSet info)
        {
            if (info.Tables.Count == 0)
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable("users");
                dt.Columns.Add("userName");
                dt.Columns.Add("userPass");
                ds.Tables.Add(dt);

                DataTable dt1 = new DataTable("cmds");
                dt1.Columns.Add("userName");
                dt1.Columns.Add("cmd");
                dt1.Columns.Add("cmdDateTime");
                ds.Tables.Add(dt1);
                ds.AcceptChanges();
                return ds;
            }
            else
                return info;
        }

        ///// <summary>
        ///// 获取需要上下文条件的命令集合,该条件作为判断命令是否有上下文的依据
        ///// </summary>
        ///// <returns></returns>
        //public DataSet GetConditionCmd()
        //{
        //    DataSet ds = new DataSet();
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("cmd_Condition");
        //    ds.Tables.Add(dt);

        //    DataRow dr = dt.NewRow();
        //    dr["cmd_Condition"] = "avh";

        //    ds.Tables[0].Rows.Add(dr);
        //    ds.AcceptChanges();
        //    return ds;
        //}
    }
}
