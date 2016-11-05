using System;
using System.Data;
using System.Windows.Forms;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using IRemoteMethodSpace;
using System.Threading;
using System.Management;
using System.Net;
using ClassLibRemotingIPSink;
using System.Diagnostics;
using System.Configuration;

namespace DataCacheService
{
    public partial class FormMain : Form
    {
        //Thread t = null;
        ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT WorkingSetSize FROM Win32_Process WHERE Name='DataCacheService.exe'");
        PerformanceCounter pc = new PerformanceCounter("System", "System Up Time");
        public FormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            Thread t1 = new Thread(TimerRun);
            t1.IsBackground = true;
            t1.Priority = ThreadPriority.Lowest;
            t1.Start();
        }

        void TimerRun()
        {
            do
            {
                foreach (ManagementObject mo in mos.Get())
                    this.label_Mem.Text = "缓存系统物理内存使用：" + Convert.ToString(float.Parse(mo["WorkingSetSize"].ToString()) / 1024 / 1000) + " MB";

                Thread.Sleep(5000);
            } while (true);
        }

        void setList(string s)
        {
            listBox_status.Items.Add(s + DateTime.Now);
            listBox_status.SelectedIndex = listBox_status.Items.Count - 1;
            if (listBox_status.Items.Count > 500)
                listBox_status.Items.Clear();
        }

        private void button_RUN_Click(object sender, EventArgs e)
        {
            try
            {
                button_RUN.Enabled = false;
                button_Load.Enabled = false;

                BinaryServerFormatterSinkProvider sinkProvider = new BinaryServerFormatterSinkProvider();
                ClientIPServerSinkProvider IPProvider = new ClientIPServerSinkProvider();//创建SINK
                IPProvider.Next = sinkProvider;
                TcpServerChannel server = new TcpServerChannel("", int.Parse(ConfigurationManager.AppSettings["port"].ToString()), IPProvider);

                ChannelServices.RegisterChannel(server, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(BaseData), "BaseData", WellKnownObjectMode.SingleCall);
                setList("服务IP: " + Dns.GetHostAddresses(Dns.GetHostName())[0].ToString() + " ,端口：" + ConfigurationManager.AppSettings["port"].ToString() + ",服务启动成功！ ");

                public_object.OnStatus += new public_object.Status(public_object_OnStatus);
                public_object.OnError += new public_object.Error(public_object_OnError);
                public_object.OnRefresh += new public_object.Refresh(public_object_OnRefresh);

                ThreadPool.QueueUserWorkItem(LoadCaChe);
                ThreadPool.QueueUserWorkItem(RunTime);

                label_counter.Text = "每分钟访问量统计：1分钟后开始记录";

            }
            catch (Exception ex)
            {
                setList(ex.Message);
            }
        }

        void public_object_OnRefresh(string tabName)
        {
            setList(tabName);
        }

        void public_object_OnError(string error_message)
        {
            setList("error:" + error_message);
        }

        void public_object_OnStatus(string s)
        {
            counter = counter + 1;
            setList(s);
        }

        int counter = 0;//每分钟访问量计数
        int hours = 0;
        int minute = 0;
        int seconds = 0;

        void RunTime( object obj)
        {
            do
            {
                seconds = seconds + 1;

                if (seconds >= 60)
                {
                    minute = minute + 1;
                    seconds = 0;
                    label_counter.Text = "每分钟访问量统计：" + counter;
                    counter = 0;
                }

                if (minute >= 60)
                {
                    hours = hours + 1;
                    minute = 0;
                }

                label_time.Text = "系统已运行：" + hours + " 时 " + minute + " 分 " + seconds + " 秒";
                Thread.Sleep(1000);

            } while (true);
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            public_object.i = 0;
            listBox_status.Items.Clear();
            public_object.Clear();

            ThreadPool.QueueUserWorkItem(LoadCaChe);
        }

        void LoadCaChe(object o)
        {
            DateTime dt = DateTime.Now;
            string error = string.Empty;
            button_Load.Enabled = false;

            setList("开始加载全部缓存，请稍等...... ");

            if (public_object.SetDsObject(ref error))
                setList("缓存已刷新! 数据刷新耗时：" + DateTime.Now.Subtract(dt).Seconds + " 秒。");
            else
                setList("加载缓存失败！error: " + error);
            button_Load.Enabled = true;
        }


        private void button_Load_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(LoadCaChe);
        }
    }

    public class BaseData : MarshalByRefObject, IRemoteMethod
    {

        #region 调用缓存
        DataSet IRemoteMethod.GetBd_Air_Aircraft()
        {
            public_object.i = public_object.i + 1;
            public_object.OnStatusMethod(getCallMethodString("Bd_Air_Aircraft", public_object.ds_Bd_Air_Aircraft, public_object.i));

            return public_object.ds_Bd_Air_Aircraft;
        }

        DataSet IRemoteMethod.GetBd_Air_CabinDiscount(string condition)
        {
            public_object.i = public_object.i + 1;
            DataSet ds = GetConditionData(condition, "Bd_Air_CabinDiscount", public_object.ds_Bd_Air_CabinDiscount);
            public_object.OnStatusMethod(getCallMethodString("Bd_Air_CabinDiscount", ds, public_object.i));//EVENT

            return ds;
        }

        DataSet IRemoteMethod.GetBd_Air_Carrier()
        {
            public_object.i = public_object.i + 1;
            public_object.OnStatusMethod(getCallMethodString("Bd_Air_Carrier", public_object.ds_Bd_Air_Carrier, public_object.i));

            return public_object.ds_Bd_Air_Carrier;
        }

        DataSet IRemoteMethod.GetBd_Air_Fares(string condition)
        {
            public_object.i = public_object.i + 1;
            DataSet ds = GetConditionData(condition, "Bd_Air_Fares", public_object.ds_Bd_Air_Fares);
            public_object.OnStatusMethod(getCallMethodString("Bd_Air_Fares", ds, public_object.i));

            return ds;
        }

        DataSet IRemoteMethod.GetBd_Air_Fuel(string condition)
        {
            public_object.i = public_object.i + 1;
            DataSet ds = GetConditionData(condition, "Bd_Air_Fuel", public_object.ds_Bd_Air_Fuel);
            public_object.OnStatusMethod(getCallMethodString("Bd_Air_Fuel", public_object.ds_Bd_Air_Fuel, public_object.i));

            return ds;
        }

        DataSet IRemoteMethod.GetBd_Base_City(string condition)
        {
            public_object.i = public_object.i + 1;
            DataSet ds = GetConditionData(condition, "Bd_Base_City", public_object.ds_Bd_Base_City);
            public_object.OnStatusMethod(getCallMethodString("Bd_Base_City", ds, public_object.i));

            return ds;
        }

        DateTime IRemoteMethod.GetServerTime()
        {
            public_object.OnStatusMethod(getIP()+",获取服务器时间！");
            return DateTime.Now;
        }
        #endregion

        #region 刷新缓存
        bool IRemoteMethod.RefreshCache(cacheTableName tableName)
        {
            string error = string.Empty;
            switch (tableName)
            {
                case IRemoteMethodSpace.cacheTableName.Bd_Air_Aircraft:
                    {
                        public_object.SetDsDataByTable("Bd_Air_Aircraft", ref error);
                        public_object.OnRefreshMethod(getRefreshString("Bd_Air_Aircraft"));

                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                case IRemoteMethodSpace.cacheTableName.Bd_Air_CabinDiscount:
                    {
                        public_object.SetDsDataByTable("Bd_Air_CabinDiscount", ref error);
                        public_object.OnRefreshMethod(getRefreshString("Bd_Air_CabinDiscount"));
                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                case IRemoteMethodSpace.cacheTableName.Bd_Air_Carrier:
                    {
                        public_object.SetDsDataByTable("Bd_Air_Carrier", ref error);
                        public_object.OnRefreshMethod(getRefreshString("Bd_Air_Carrier"));
                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                case IRemoteMethodSpace.cacheTableName.Bd_Air_Fares:
                    {
                        public_object.SetDsDataByTable("Bd_Air_Fares", ref error);
                        public_object.OnRefreshMethod(getRefreshString("Bd_Air_Fares"));
                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                case IRemoteMethodSpace.cacheTableName.Bd_Air_Fuel:
                    {
                        public_object.SetDsDataByTable("Bd_Air_Fuel", ref error);
                        public_object.OnRefreshMethod(getRefreshString("Bd_Air_Fuel"));
                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                case IRemoteMethodSpace.cacheTableName.Bd_Base_City:
                    {
                        public_object.SetDsDataByTable("Bd_Base_City", ref error);
                        public_object.OnRefreshMethod(getRefreshString("Bd_Base_City"));

                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                case IRemoteMethodSpace.cacheTableName.All_Table:
                    {
                        public_object.SetDsObject(ref error);
                        public_object.OnRefreshMethod(getRefreshString("全部"));

                        if (error.Length > 0)
                        {
                            public_object.OnErrorMethod(error);
                            return false;
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }

        bool IRemoteMethod.RefreshCache()
        {
            string error = string.Empty;
            return public_object.SetDsObject(ref error);
        }
        #endregion

        string getIP()
        {
            return ((IPAddress)System.Runtime.Remoting.Messaging.CallContext.GetData("ClientIPAddress")).ToString();
        }

        string getCallMethodString(string tabName, DataSet ds, int count)
        {
            return "IP：" + getIP() + "，调用缓存表：" + tabName + " 返回：" + ds.Tables[0].Rows.Count + " 行，调用计数：" + count + " 次。";
        }

        string getRefreshString(string tabName)
        {
            return "IP：" + getIP() + "，调用函数刷新缓存表：" + tabName + " 成功！";
        }

        DataSet GetConditionData(string condition, string tableName, DataSet ds)
        {
            try
            {
                DataRow[] drs = ds.Tables[0].Select(condition);
                DataSet newds = ds.Clone();
                newds.Tables[0].TableName = tableName;
                foreach (DataRow item in drs)
                {
                    DataRow dr = newds.Tables[0].NewRow();
                    dr.ItemArray = item.ItemArray;
                    newds.Tables[0].Rows.Add(dr);
                }
                newds.AcceptChanges();
                return newds;
            }
            catch (Exception)
            {
                return new DataSet();
            }
        }
    }

    public static class public_object
    {
        public static DataSet ds_Bd_Air_CabinDiscount = new DataSet();
        public static DataSet ds_Bd_Air_Fares = new DataSet();
        public static DataSet ds_Bd_Air_Aircraft = new DataSet();
        public static DataSet ds_Bd_Air_Fuel = new DataSet();
        public static DataSet ds_Bd_Air_Carrier = new DataSet();
        public static DataSet ds_Bd_Base_City = new DataSet();

        public static int i = 0;

        public delegate void Status(string s);//调用方法事件
        public static event Status OnStatus;

        public delegate void Refresh(string tabName);//刷新缓存事件
        public static event Refresh OnRefresh;

        public delegate void Error(string error_message);//错误事件
        public static event Error OnError;

        public static void OnRefreshMethod(string tabName)
        {
            OnRefresh.BeginInvoke(tabName, null, null);
        }
      
        public static void OnStatusMethod(string s)
        {
            OnStatus.BeginInvoke(s, null, null);
        }

        public static void OnErrorMethod(string error_message)
        {
            OnError.BeginInvoke(error_message, null, null);
        }


        public static void Clear()
        {
            ds_Bd_Air_CabinDiscount.Clear();
            ds_Bd_Air_CabinDiscount.Dispose();
            ds_Bd_Air_CabinDiscount = new DataSet();

            ds_Bd_Air_Fares.Clear();
            ds_Bd_Air_Fares.Dispose();
            ds_Bd_Air_Fares = new DataSet();

            ds_Bd_Air_Aircraft.Clear();
            ds_Bd_Air_Aircraft.Dispose();
            ds_Bd_Air_Aircraft = new DataSet();

            ds_Bd_Air_Fuel.Clear();
            ds_Bd_Air_Fuel.Dispose();
            ds_Bd_Air_Fuel = new DataSet();

            ds_Bd_Air_Carrier.Clear();
            ds_Bd_Air_Carrier.Dispose();
            ds_Bd_Air_Carrier = new DataSet();

            ds_Bd_Base_City.Clear();
            ds_Bd_Base_City.Dispose();
            ds_Bd_Base_City = new DataSet();

            i = 0;
        }

        public static bool SetDsDataByTable(string tableName,ref string error)
        {
            DB.DataBase_Cls db = new DB.DataBase_Cls();
            try
            {
                switch (tableName)
                {
                    case "Bd_Air_CabinDiscount":
                        {
                            ds_Bd_Air_CabinDiscount = db.GetDataSet("select id,Cabin,AirPortCode,FromCityCode,ToCityCode,BeginTime,EndTime,DiscountRate from Bd_Air_CabinDiscount");
                            ds_Bd_Air_CabinDiscount.Tables["table"].TableName = "Bd_Air_CabinDiscount";
                        }
                        break;
                    case "Bd_Air_Fares":
                        {
                            ds_Bd_Air_Fares = db.GetDataSet("select EffectTime,InvalidTime,FareFee,Mileage,CarryCode,FromCityCode,ToCityCode from Bd_Air_Fares");
                            ds_Bd_Air_Fares.Tables["table"].TableName = "Bd_Air_Fares";
                        }
                        break;
                    case "Bd_Air_Aircraft":
                        {
                            ds_Bd_Air_Aircraft = db.GetDataSet("select ABFeeN,Aircraft from Bd_Air_Aircraft");
                            ds_Bd_Air_Aircraft.Tables["table"].TableName = "Bd_Air_Aircraft";
                        }
                        break;
                    case "Bd_Air_Fuel":
                        {
                            ds_Bd_Air_Fuel = db.GetDataSet("select StartTime,EndTime,LowAdultFee,ExceedAdultFee,LowChildFee,ExceedChildFee from Bd_Air_Fuel");
                            ds_Bd_Air_Fuel.Tables["table"].TableName = "Bd_Air_Fuel";
                        }
                        break;
                    case "Bd_Air_Carrier":
                        {
                            ds_Bd_Air_Carrier = db.GetDataSet("select Code,ShortName,A1,Type from Bd_Air_Carrier");
                            ds_Bd_Air_Carrier.Tables["table"].TableName = "Bd_Air_Carrier";
                        }
                        break;
                    case "Bd_Base_City":
                        {
                            ds_Bd_Base_City = db.GetDataSet("select City,spell,Code,Countries,type from Bd_Base_City");
                            ds_Bd_Base_City.Tables["table"].TableName = "Bd_Base_City";
                        }
                        break;
                    case "ALL":
                        {
                            SetDsObject(ref error);
                        }
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        public static bool SetDsObject(ref string error)
        {
            try
            {
                DB.DataBase_Cls db = new DB.DataBase_Cls();

                ds_Bd_Air_CabinDiscount = db.GetDataSet("select id,Cabin,AirPortCode,FromCityCode,ToCityCode,BeginTime,EndTime,DiscountRate from Bd_Air_CabinDiscount");
                ds_Bd_Air_CabinDiscount.Tables["table"].TableName = "Bd_Air_CabinDiscount";

                ds_Bd_Air_Fares = db.GetDataSet("select EffectTime,InvalidTime,FareFee,Mileage,CarryCode,FromCityCode,ToCityCode from Bd_Air_Fares");
                ds_Bd_Air_Fares.Tables["table"].TableName = "Bd_Air_Fares";

                ds_Bd_Air_Aircraft = db.GetDataSet("select ABFeeN,Aircraft from Bd_Air_Aircraft");
                ds_Bd_Air_Aircraft.Tables["table"].TableName = "Bd_Air_Aircraft";

                ds_Bd_Air_Fuel = db.GetDataSet("select StartTime,EndTime,LowAdultFee,ExceedAdultFee,LowChildFee,ExceedChildFee from Bd_Air_Fuel");
                ds_Bd_Air_Fuel.Tables["table"].TableName = "Bd_Air_Fuel";

                ds_Bd_Air_Carrier = db.GetDataSet("select Code,ShortName,A1,Type from Bd_Air_Carrier");
                ds_Bd_Air_Carrier.Tables["table"].TableName = "Bd_Air_Carrier";

                ds_Bd_Base_City = db.GetDataSet("select City,spell,Code,Countries,type from Bd_Base_City");
                ds_Bd_Base_City.Tables["table"].TableName = "Bd_Base_City";

                return true;
            }
            catch(Exception e)
            {
                error = e.Message;
                return false;
            }
        }
    }

}
