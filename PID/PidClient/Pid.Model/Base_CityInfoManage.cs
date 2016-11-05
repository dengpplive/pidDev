using System;
using System.Collections.Generic;
using System.Text;

namespace PBPid.DBModel
{
    [Serializable]
    public class Base_CityInfoManage
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int City_Id_auto { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string City_Name { get; set; }

        /// <summary>
        /// 城市三字码
        /// </summary>
        public string City_Code { get; set; }

        /// <summary>
        /// 城市全拼
        /// </summary>
        public string City_QuanPin { get; set; }

    }
}
