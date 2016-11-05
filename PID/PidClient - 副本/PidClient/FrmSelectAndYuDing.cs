using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PidClient
{
    public partial class FrmSelectAndYuDing : Form
    {
        //主窗体
        public FrmMain tmpMain = null;

        //上一窗体 
        public FrmYuDing tmpYuDing = null;

        //航班查询结果
        public string avResult = "";
        //出发城市
        public string fromcitycode = "";
        public string fromcityname = "";

        //到达城市
        public string tocitycode = "";
        public string tocityname = "";

        //航班日期
        public string FlightDate = "";

        //选择的航段信息
        public string selectFlight = "";

        public FrmSelectAndYuDing()
        {
            InitializeComponent();
        }

        //组织显示的结构
        DataSet GetTabStruct()
        {
            //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //dt.Columns.Add("序号");
            dt.Columns.Add("航班日期");
            dt.Columns.Add("起飞时间");
            dt.Columns.Add("到达时间");
            dt.Columns.Add("航空公司");
            dt.Columns.Add("航班号");
            dt.Columns.Add("舱位代码");
            dt.Columns.Add("出发城市");
            dt.Columns.Add("到达城市");
            dt.Columns.Add("机型");
            dt.Columns.Add("航站楼");
            ds.Tables.Add(dt);
            return ds;
        }

        //数据转换
        DataTable GetUserData(string selectFlight)
        {
            DataSet ds = GetTabStruct();

            string[] sl2 = selectFlight.Split(',');

            //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^

            DataRow dr = ds.Tables[0].NewRow();
            //dr[0] = index;
            dr[0] = sl2[0];
            dr[1] = sl2[1];
            dr[2] = sl2[2];
            dr[3] = sl2[3];
            dr[4] = sl2[4];
            dr[5] = sl2[5].Substring(0, sl2[5].Length - 2);
            dr[6] = fromcityname;
            dr[7] = tocityname;
            dr[8] = sl2[8];
            dr[9] = sl2[14];
            ds.Tables[0].Rows.Add(dr);
            
            return ds.Tables[0];
        }

        public void InitShow()
        {
            dataGridView1.DataSource = GetUserData(selectFlight);

            //初始化舱位选择列表
            string[] sl= selectFlight.Split(',');
            if(sl.Length>2)
            {
                //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3
                string classinfo= sl[5].Substring(0,sl[5].Length-2);
                int index = 0;
                while (classinfo.Length > index*2)
                {
                    cmbClassList.Items.Add(classinfo.Substring(index*2, 2));
                    index++;
                }
                cmbClassList.SelectedIndex = 0;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((tmpYuDing == null) || (tmpYuDing.IsDisposed))
            {
                tmpYuDing = new FrmYuDing();
            }
            tmpYuDing.tmpMainForm = tmpMain;
            tmpYuDing.WindowState = FormWindowState.Maximized;
            tmpYuDing.MdiParent = tmpMain;
            tmpYuDing.Show();
            tmpYuDing.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = int.Parse(cmbPassengerCount.Text);

            bool flag = true;//信息是否合法

            //判断乘机人信息录入是否完整
            //判断证件号码信息录入是否完整
            switch (count)
            {
                case 1:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 2:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 3:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 4:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == ""
                        || txtPassenger4.Text.Trim() == "" || txtCard4.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 5:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == ""
                        || txtPassenger4.Text.Trim() == "" || txtCard4.Text.Trim() == ""
                        || txtPassenger5.Text.Trim() == "" || txtCard5.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 6:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == ""
                        || txtPassenger4.Text.Trim() == "" || txtCard4.Text.Trim() == ""
                        || txtPassenger5.Text.Trim() == "" || txtCard5.Text.Trim() == ""
                        || txtPassenger6.Text.Trim() == "" || txtCard6.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 7:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == ""
                        || txtPassenger4.Text.Trim() == "" || txtCard4.Text.Trim() == ""
                        || txtPassenger5.Text.Trim() == "" || txtCard5.Text.Trim() == ""
                        || txtPassenger6.Text.Trim() == "" || txtCard6.Text.Trim() == ""
                        || txtPassenger7.Text.Trim() == "" || txtCard7.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 8:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == ""
                        || txtPassenger4.Text.Trim() == "" || txtCard4.Text.Trim() == ""
                        || txtPassenger5.Text.Trim() == "" || txtCard5.Text.Trim() == ""
                        || txtPassenger6.Text.Trim() == "" || txtCard6.Text.Trim() == ""
                        || txtPassenger7.Text.Trim() == "" || txtCard7.Text.Trim() == ""
                        || txtPassenger8.Text.Trim() == "" || txtCard8.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;
                case 9:
                    if (txtPassenger1.Text.Trim() == "" || txtCard1.Text.Trim() == ""
                        || txtPassenger2.Text.Trim() == "" || txtCard2.Text.Trim() == ""
                        || txtPassenger3.Text.Trim() == "" || txtCard3.Text.Trim() == ""
                        || txtPassenger4.Text.Trim() == "" || txtCard4.Text.Trim() == ""
                        || txtPassenger5.Text.Trim() == "" || txtCard5.Text.Trim() == ""
                        || txtPassenger6.Text.Trim() == "" || txtCard6.Text.Trim() == ""
                        || txtPassenger7.Text.Trim() == "" || txtCard7.Text.Trim() == ""
                        || txtPassenger8.Text.Trim() == "" || txtCard8.Text.Trim() == ""
                        || txtPassenger9.Text.Trim() == "" || txtCard9.Text.Trim() == "")
                    {
                        flag = false;
                    }
                    break;

            }
                    
            if (!flag)
            {
                MessageBox.Show("乘客姓名和证件号码信息不能为空！");
                return;
            }

            //判断联系人信息是否完整
            if (txtLinkMan.Text.Trim() == "" || txtLinkPhone.Text.Trim() == "")
            {
                MessageBox.Show("联系人和联系电话不能为空！");
                return;
            }

            //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3
            string[] sl= selectFlight.Split(',');


            //组织订座指令
            string cmd = "nm";

            //ss hu7742 t 21jan14 kweszx 1
            string ssInfo="^ss "+sl[3]+sl[4]+" "+cmbClassList.Text.Substring(0,1)+" "+getEtermDate(sl[0])+" "+fromcitycode+tocitycode+" "+cmbPassengerCount.Text;
            string cardInfo = "";
            string ctInfo = "^ct" + txtLinkPhone.Text.Trim();
            //osi hu ctct 13688514890
            string osiInfo = "^osi "+sl[3]+" ctct "+txtLinkPhone.Text;

            DateTime tmpdate=DateTime.Parse(sl[0]+" "+sl[1]).AddHours(-1);

            string tktlInfo = "^tktl/" + tmpdate.ToString("HHmm") + "/" + getEtermDate(tmpdate.ToString("yyyy-MM-dd")) + "/"+PublicClass.curOfficeCode;

            switch(count)
            {
                case 1:
                    cmd+="1"+txtPassenger1.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1";
                    break;
                case 2:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2";
                    break;
                case 3:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3";
                    break;
                case 4:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text+"1"+txtPassenger4.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard4.Text+"/P4";
                    break;
                case 5:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text+"1"+txtPassenger4.Text+
                        "1"+txtPassenger5.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard4.Text+"/P4"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard5.Text+"/P5";
                    break;
                case 6:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text+"1"+txtPassenger4.Text+
                        "1"+txtPassenger5.Text+"1"+txtPassenger6.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard4.Text+"/P4"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard5.Text+"/P5"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard6.Text+"/P6";
                    break;
                case 7:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text+"1"+txtPassenger4.Text+
                        "1"+txtPassenger5.Text+"1"+txtPassenger6.Text+"1"+txtPassenger7.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard4.Text+"/P4"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard5.Text+"/P5"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard6.Text+"/P6"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard7.Text+"/P7";
                    break;
                case 8:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text+"1"+txtPassenger4.Text+
                        "1"+txtPassenger5.Text+"1"+txtPassenger6.Text+"1"+txtPassenger7.Text+"1"+txtPassenger8.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard4.Text+"/P4"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard5.Text+"/P5"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard6.Text+"/P6"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard7.Text+"/P7"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard8.Text+"/P8";
                    break;
                case 9:
                    cmd+="1"+txtPassenger1.Text+"1"+txtPassenger2.Text+"1"+txtPassenger3.Text+"1"+txtPassenger4.Text+
                        "1"+txtPassenger5.Text+"1"+txtPassenger6.Text+"1"+txtPassenger7.Text+"1"+txtPassenger8.Text+
                        "1"+txtPassenger9.Text;
                    cardInfo+="^ssr foid "+sl[3]+" hk/ni"+txtCard1.Text+"/P1"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard2.Text+"/P2"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard3.Text+"/P3"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard4.Text+"/P4"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard5.Text+"/P5"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard6.Text+"/P6"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard7.Text+"/P7"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard8.Text+"/P8"+
                        "^ssr foid "+sl[3]+" hk/ni"+txtCard9.Text+"/P9";
                    break;
            }

            //nm1吴俊^ss hu7742 t 21jan14 kweszx 1^ct0851-8651569^ssr foid hu hk/ni512923197410241294/p1^
            //osi hu ctct 13688514890^tktl/0825/21jan/kwe140^@
            cmd += ssInfo + ctInfo + cardInfo + osiInfo + tktlInfo + "^@";

            //预订座位
            string strResult = "";
            if (PublicClass.SendCommand(cmd, 1, ref strResult))
            {
                //分析PNR
                string PNR = PublicClass.GetPNRCodeByYuDingResult(strResult);

                //复制到粘贴板
                Clipboard.SetText(PNR);

                MessageBox.Show("预订PNR结果：" + PNR);
                return;
            }
            else
            {
                MessageBox.Show(strResult);
                return;
            }
        }

        #region 根据标准日期格式(2009-01-20)返回20JAN09
        /// <summary>
        /// 根据标准日期格式(2009-01-20)返回20JAN09
        /// </summary>
        /// <param name="Date">返回日期格式（20JAN09）</param>
        private string getEtermDate(string date)
        {
            string strResult = date.Substring(8, 2);
            string strYear = "";

            //判断是否跨年
            if (DateTime.Now.Year < int.Parse(date.Substring(0, 4)))
            {
                strYear = date.Substring(2, 2);
            }

            string month = date.Substring(5, 2);
            if (month == "01")
            {
                strResult += "JAN";
            }
            else if (month == "02")
            {
                strResult += "FEB";
            }
            else if (month == "03")
            {
                strResult += "MAR";
            }
            else if (month == "04")
            {
                strResult += "APR";
            }
            else if (month == "05")
            {
                strResult += "MAY";
            }
            else if (month == "06")
            {
                strResult += "JUN";
            }
            else if (month == "07")
            {
                strResult += "JUL";
            }
            else if (month == "08")
            {
                strResult += "AUG";
            }
            else if (month == "09")
            {
                strResult += "SEP";
            }
            else if (month == "10")
            {
                strResult += "OCT";
            }
            else if (month == "11")
            {
                strResult += "NOV";
            }
            else if (month == "12")
            {
                strResult += "DEC";
            }

            if (strYear == "")
            {
                return strResult;
            }
            else
            {
                return strResult + strYear;
            }
        }

        #endregion 根据标准日期格式(2009-01-20)返回20JAN09

        private void cmbClassList_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPassengerCount.Items.Clear();

            if (cmbClassList.Text.Substring(1).ToUpper() == "A")
            {
                cmbPassengerCount.Items.Add("1");
                cmbPassengerCount.Items.Add("2");
                cmbPassengerCount.Items.Add("3");
                cmbPassengerCount.Items.Add("4");
                cmbPassengerCount.Items.Add("5");
                cmbPassengerCount.Items.Add("6");
                cmbPassengerCount.Items.Add("7");
                cmbPassengerCount.Items.Add("8");
                cmbPassengerCount.Items.Add("9");
            }
            else
            {
                int count = int.Parse(cmbClassList.Text.Substring(1));
                for (int i = 1; i <= count; i++)
                {
                    cmbPassengerCount.Items.Add(i.ToString());
                }
            }
            cmbPassengerCount.SelectedIndex = 0;
        }

        private void DisabledAll()
        {
            lblC1.Visible = false;
            lblP1.Visible = false;
            txtPassenger1.Visible = false;
            txtCard1.Visible = false;

            lblC2.Visible = false;
            lblP2.Visible = false;
            txtPassenger2.Visible = false;
            txtCard2.Visible = false;

            lblC3.Visible = false;
            lblP3.Visible = false;
            txtPassenger3.Visible = false;
            txtCard3.Visible = false;

            lblC4.Visible = false;
            lblP4.Visible = false;
            txtPassenger4.Visible = false;
            txtCard4.Visible = false;

            lblC5.Visible = false;
            lblP5.Visible = false;
            txtPassenger5.Visible = false;
            txtCard5.Visible = false;

            lblC6.Visible = false;
            lblP6.Visible = false;
            txtPassenger6.Visible = false;
            txtCard6.Visible = false;

            lblC7.Visible = false;
            lblP7.Visible = false;
            txtPassenger7.Visible = false;
            txtCard7.Visible = false;

            lblC8.Visible = false;
            lblP8.Visible = false;
            txtPassenger8.Visible = false;
            txtCard8.Visible = false;

            lblC9.Visible = false;
            lblP9.Visible = false;
            txtPassenger9.Visible = false;
            txtCard9.Visible = false;
        }

        private void cmbPassengerCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = int.Parse(cmbPassengerCount.Text);

            DisabledAll();

            switch (count)
            {
                case 1:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;
                    break;
                case 2:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;
                    break;
                case 3:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;
                    break;
                case 4:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;

                    lblC4.Visible = true;
                    lblP4.Visible = true;
                    txtPassenger4.Visible = true;
                    txtCard4.Visible = true;
                    break;
                case 5:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;

                    lblC4.Visible = true;
                    lblP4.Visible = true;
                    txtPassenger4.Visible = true;
                    txtCard4.Visible = true;

                    lblC5.Visible = true;
                    lblP5.Visible = true;
                    txtPassenger5.Visible = true;
                    txtCard5.Visible = true;
                    break;
                case 6:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;

                    lblC4.Visible = true;
                    lblP4.Visible = true;
                    txtPassenger4.Visible = true;
                    txtCard4.Visible = true;

                    lblC5.Visible = true;
                    lblP5.Visible = true;
                    txtPassenger5.Visible = true;
                    txtCard5.Visible = true;

                    lblC6.Visible = true;
                    lblP6.Visible = true;
                    txtPassenger6.Visible = true;
                    txtCard6.Visible = true;
                    break;
                case 7:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;

                    lblC4.Visible = true;
                    lblP4.Visible = true;
                    txtPassenger4.Visible = true;
                    txtCard4.Visible = true;

                    lblC5.Visible = true;
                    lblP5.Visible = true;
                    txtPassenger5.Visible = true;
                    txtCard5.Visible = true;

                    lblC6.Visible = true;
                    lblP6.Visible = true;
                    txtPassenger6.Visible = true;
                    txtCard6.Visible = true;

                    lblC7.Visible = true;
                    lblP7.Visible = true;
                    txtPassenger7.Visible = true;
                    txtCard7.Visible = true;
                    break;
                case 8:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;

                    lblC4.Visible = true;
                    lblP4.Visible = true;
                    txtPassenger4.Visible = true;
                    txtCard4.Visible = true;

                    lblC5.Visible = true;
                    lblP5.Visible = true;
                    txtPassenger5.Visible = true;
                    txtCard5.Visible = true;

                    lblC6.Visible = true;
                    lblP6.Visible = true;
                    txtPassenger6.Visible = true;
                    txtCard6.Visible = true;

                    lblC7.Visible = true;
                    lblP7.Visible = true;
                    txtPassenger7.Visible = true;
                    txtCard7.Visible = true;

                    lblC8.Visible = true;
                    lblP8.Visible = true;
                    txtPassenger8.Visible = true;
                    txtCard8.Visible = true;
                    break;
                case 9:
                    lblC1.Visible = true;
                    lblP1.Visible = true;
                    txtPassenger1.Visible = true;
                    txtCard1.Visible = true;

                    lblC2.Visible = true;
                    lblP2.Visible = true;
                    txtPassenger2.Visible = true;
                    txtCard2.Visible = true;

                    lblC3.Visible = true;
                    lblP3.Visible = true;
                    txtPassenger3.Visible = true;
                    txtCard3.Visible = true;

                    lblC4.Visible = true;
                    lblP4.Visible = true;
                    txtPassenger4.Visible = true;
                    txtCard4.Visible = true;

                    lblC5.Visible = true;
                    lblP5.Visible = true;
                    txtPassenger5.Visible = true;
                    txtCard5.Visible = true;

                    lblC6.Visible = true;
                    lblP6.Visible = true;
                    txtPassenger6.Visible = true;
                    txtCard6.Visible = true;

                    lblC7.Visible = true;
                    lblP7.Visible = true;
                    txtPassenger7.Visible = true;
                    txtCard7.Visible = true;

                    lblC8.Visible = true;
                    lblP8.Visible = true;
                    txtPassenger8.Visible = true;
                    txtCard8.Visible = true;

                    lblC9.Visible = true;
                    lblP9.Visible = true;
                    txtPassenger9.Visible = true;
                    txtCard9.Visible = true;
                    break;
            }
        }
    }
}
