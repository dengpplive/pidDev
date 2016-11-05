using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IRemoteMethodSpace
{
    public enum cacheTableName
    {
        Bd_Air_CabinDiscount,
        Bd_Air_Fares,
        Bd_Air_Fuel,
        Bd_Base_City,
        Bd_Air_Aircraft,
        Bd_Air_Carrier,
        All_Table
    }

    public interface IRemoteMethod
    {
        DataSet GetBd_Air_CabinDiscount(string condition);
        DataSet GetBd_Air_Fares(string condition);
        DataSet GetBd_Air_Fuel(string condition);
        DataSet GetBd_Base_City(string condition);
        DataSet GetBd_Air_Aircraft();
        DataSet GetBd_Air_Carrier();

        /// <summary>
        /// 刷新全部缓存
        /// </summary>
        /// <returns></returns>
        bool RefreshCache();
        /// <summary>
        /// 根据输入表刷新缓存
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool RefreshCache(cacheTableName tableName);
        DateTime GetServerTime();
    }
}
