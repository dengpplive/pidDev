using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DataCacheService.DB
{
    ///////////////////////////////////////////
    // 编写时间:2012-2-2
    // 编写人：罗俊杰
    // 修改内容：
    // 修改人：
    ///////////////////////////////////////////

    public class DataBase_Cls
    {
        private string _SqlConStr = string.Empty;
        /// <summary>
        /// 连接对象,保存状态。
        /// </summary>
        private SqlConnection m_Connection;

        public string ErrorInfo = string.Empty;

        public DataBase_Cls()//构造时加载字符串
        {
            this.SqlConStr =ConfigurationManager.AppSettings["DB"].ToString();
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string SqlConStr
        {
            get { return _SqlConStr; }
            set { _SqlConStr = value; }
        }

        /// <summary>
        /// 根据查询语句返回数据
        /// </summary>
        /// <param name="SqlString">查询语句</param>
        /// <param name="_ConString">连接字符串</param>
        /// <param name="_DataTab">数据表名</param>
        /// <returns></returns>
        public DataSet GetDataSet(string SqlString, string _ConString, string _DataTab)
        {
            ErrorInfo = string.Empty;
            try
            {
                DataSet _ds = new DataSet();
                if (null == m_Connection)
                    m_Connection = new SqlConnection(_ConString);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlDataAdapter _DataAdapter = new SqlDataAdapter(SqlString, m_Connection);
                _DataAdapter.Fill(_ds, _DataTab);
                _DataAdapter.Dispose();
                
                m_Connection.Close();

                return _ds;
            }
            catch (Exception e)
            {
                m_Connection.Close();
                ErrorInfo = e.Message + e.Source;
                return null;
            }
        }
        /// <summary>
        /// 根据SqlConStr查询并返回数据
        /// </summary>
        /// <param name="SqlString">查询语句</param>
        /// <param name="_DataTab">数据表名</param>
        /// <returns>返回数据集</returns>
        public DataSet GetDataSet(string SqlString, string _DataTab)
        {
            ErrorInfo = string.Empty;
            try
            {
                DataSet _ds = new DataSet();
                if (null == m_Connection)
                    m_Connection = new SqlConnection(this.SqlConStr);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlDataAdapter _DataAdapter = new SqlDataAdapter(SqlString, m_Connection);
                _DataAdapter.Fill(_ds, _DataTab);
                _DataAdapter.Dispose();
                m_Connection.Close();

                return _ds;
            }
            catch (Exception e)
            {
                ErrorInfo = e.Message + e.Source;
                return null;
            }
        }
        /// <summary>
        /// 根据SqlConStr查询并返回数据
        /// </summary>
        /// <param name="SqlString">查询语句</param>
        /// <returns>返回数据集</returns>
        public DataSet GetDataSet(string SqlString)
        {
            ErrorInfo = string.Empty;
            try
            {
                DataSet _ds = new DataSet();
                if (null == m_Connection)
                    m_Connection = new SqlConnection(this.SqlConStr);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlDataAdapter _DataAdapter = new SqlDataAdapter(SqlString, m_Connection);
                _DataAdapter.Fill(_ds);
                _DataAdapter.Dispose();
                m_Connection.Close();
                return _ds;
            }
            catch (Exception e)
            {
                m_Connection.Close();
                ErrorInfo = e.Message + e.Source;
                return null;
            }
        }

        /// <summary>
        /// 保存数据集
        /// </summary>
        /// <param name="_Ds">要保存的数据集</param>
        /// <param name="_DataTab">要保存的数据表</param>
        /// <param name="_ConString">你要连接数据库的字符串</param>
        /// <returns>返回true为成功，否则失败</returns>
        public bool SetDataSet(DataSet _Ds, string _DataTab, string _ConString)
        {
            ErrorInfo = string.Empty;
            try
            {
                string _Temp_String = "SELECT TOP 1 * FROM " + _DataTab;
                if (null == m_Connection)
                    m_Connection = new SqlConnection(_ConString);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlDataAdapter _DataAdapter = new SqlDataAdapter(_Temp_String, m_Connection);
                _DataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                SqlCommandBuilder _Sqlcmdbuilder = new SqlCommandBuilder(_DataAdapter);
                _DataAdapter.Update(_Ds, _DataTab);
                _Sqlcmdbuilder.Dispose();
                _DataAdapter.Dispose();
                m_Connection.Close();

                return true;
            }
            catch (Exception e)
            {
                m_Connection.Close();
                ErrorInfo = e.Message + e.Source;
                return false;
            }
        }
        /// <summary>
        /// 根据SqlConStr保存数据集
        /// </summary>
        /// <param name="_Ds">要保存的数据集</param>
        /// <param name="_DataTab">要保存的数据表</param>
        /// <returns></returns>
        public bool SetDataSet(DataSet _Ds, string _DataTab)
        {
            ErrorInfo = string.Empty;
            try
            {
                string _Temp_String = "SELECT * FROM " + _DataTab;
                if (null == m_Connection)
                    m_Connection = new SqlConnection(this.SqlConStr);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlDataAdapter _DataAdapter = new SqlDataAdapter(_Temp_String, m_Connection);
                _DataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                SqlCommandBuilder _Sqlcmdbuilder = new SqlCommandBuilder(_DataAdapter);
                _Ds.Tables[0].TableName = _DataTab;
                int i = _DataAdapter.Update(_Ds, _DataTab);

                _DataAdapter.Dispose();
                _Sqlcmdbuilder.Dispose();
                m_Connection.Close();

                return true;
            }
            catch (Exception e)
            {
                m_Connection.Close();
                ErrorInfo = e.Message + e.Source;
                return false;
            }
        }
        /// <summary>
        /// 根据SqlConStr直接运行SQL语句。
        /// </summary>
        /// <param name="SqlString">SQL语句或存储过程</param>
        /// <returns>返回受影响的行数,失败返回原因</returns>
        public bool RunSql(string SqlString)
        {
            ErrorInfo = string.Empty;
            try
            {
                if (null == m_Connection)
                    m_Connection = new SqlConnection(this._SqlConStr);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlCommand _SqlCmd = new SqlCommand(SqlString, m_Connection);
                int _Temp_int = _SqlCmd.ExecuteNonQuery();
                _SqlCmd.Dispose();
                m_Connection.Close();

                if (_Temp_int > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                m_Connection.Close();
                ErrorInfo = e.Message + e.Source;
                return false;
            }
        }
        /// <summary>
        /// 开始事务，执行多条SQL语句
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public bool RunTransaction(List<string> SqlString)
        {
            ErrorInfo = string.Empty;

            SqlTransaction _Transaction;
            if (null == m_Connection)
                m_Connection = new SqlConnection(this._SqlConStr);
            if (m_Connection.State == ConnectionState.Closed)
            {
                m_Connection.Open();
            }
            _Transaction = m_Connection.BeginTransaction();
            SqlCommand _SqlCmd = new SqlCommand();
            _SqlCmd.Connection = m_Connection;
            _SqlCmd.Transaction = _Transaction;

            try
            {
                if (SqlString.Count != 0)
                {
                    for (int i = 0; i < SqlString.Count; i++)
                    {
                        _SqlCmd.CommandText = Convert.ToString(SqlString[i]);
                        _SqlCmd.ExecuteNonQuery();
                    }
                    _Transaction.Commit();//执行事务
                    _SqlCmd.Dispose();
                    m_Connection.Close();
                    return true;
                }
                else
                {
                    m_Connection.Close();
                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorInfo = e.Message + e.Source;
                _Transaction.Rollback();//事务回滚
                m_Connection.Close();
                return false;
            }
        }

        /// <summary>
        /// 获取表架构
        /// </summary>
        /// <param name="_DataTab"></param>
        /// <returns></returns>
        public DataSet GetTabStruct(string _DataTab)
        {
            ErrorInfo = string.Empty;
            try
            {
                DataSet _ds = new DataSet();
                string _Temp_String = "Select * from " + _DataTab + " where 1<>1";
                if (null == m_Connection)
                    m_Connection = new SqlConnection(this._SqlConStr);
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }
                SqlDataAdapter _DataAdapter = new SqlDataAdapter(_Temp_String, m_Connection);
                _DataAdapter.Fill(_ds, _DataTab);
                _DataAdapter.Dispose();
                m_Connection.Close();
                return _ds;
            }
            catch (Exception e)
            {
                m_Connection.Close();
                ErrorInfo = e.Message + e.Source;
                return null;
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <returns></returns>
        public bool ClosedConnection()
        {
            if (this.m_Connection.State == ConnectionState.Open)
            {
                ErrorInfo = string.Empty;
                this.m_Connection.Dispose();
            }
            if (this.m_Connection.State == ConnectionState.Closed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}