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
    public partial class FrmYuDing : Form
    {
        //主窗口
        public FrmMain tmpMainForm = null;

        //航班查询结果
        private string avResult = "";
        //出发城市
        private string fromcitycode = "";
        private string fromcityname = "";

        //到达城市
        private string tocitycode = "";
        private string tocityname = "";

        //航班日期
        private string FlightDate = "";

        //组织显示的结构
        DataSet GetTabStruct()
        {
            //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("序号");
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
        DataTable GetUserData(string avResult)
        {
            DataSet ds = GetTabStruct();
            string[] sl = avResult.Split('^');
            int index = 1;
            foreach (string item in sl)
            {
                if (item.Trim() == "")
                {
                    continue;
                }

                string[] sl2 = item.Split(',');

                //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^
                //2014-01-15,11:55,14:20,HU,7148,FAZ6A6YABAS1*9,CTU,PEK,333,0,1,1,False,,T2T1^
                //2014-01-15,12:00,14:40,ZH,1406,YABASAUA*9,CTU,PEK,330,0,1,1,True,,T2T3^
                //2014-01-15,12:00,14:40,CA,1406,YABASAN1UA*9,CTU,PEK,330,0,1,1,False,,T2T3^
                //2014-01-15,12:15,14:45,CZ,6162,F7W7S7YAB7*9,CTU,PEK,321,0,1,1,False,,T2T2^
                //2014-01-15,13:00,15:35,CA,421,C4YABASANAU2*9,CTU,PEK,320,0,1,1,False,,T2T3^
                //2014-01-15,13:30,16:10,3U,8887,FAA7YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^
                //2014-01-15,13:50,16:25,ZH,1416,F1YABASAUA*9,CTU,PEK,330,0,1,1,True,,T2T3^
                //2014-01-15,13:50,16:25,CA,1416,F1YABASAUA*9,CTU,PEK,330,0,1,1,False,,T2T3^2014-01-15,14:00,16:40,CA,4103,F2YABASAN2UA*9,CTU,PEK,321,0,1,1,False,,T2T3^2014-01-15,14:00,16:40,ZH,4103,F2YABASAUA*9,CTU,PEK,321,0,0,1,True,,T2T3^2014-01-15,15:00,17:30,CA,4109,F8A3O1W5YAUA*9,CTU,PEK,333,0,1,1,False,,T2T3^2014-01-15,15:00,17:30,ZH,4109,F8YAUA*9,CTU,PEK,333,0,0,1,True,,T2T3^2014-01-15,15:30,18:05,3U,8893,F4YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^2014-01-15,16:00,18:35,CA,4105,F1CADAZ4R1*9,CTU,PEK,340,0,1,1,False,,T2T3^2014-01-15,16:00,18:35,ZH,4105,F1CA*9,CTU,PEK,340,0,1,1,True,,T2T3^2014-01-15,16:30,19:15,3U,8889,F1YABATA*9,CTU,PEK,321,0,1,1,False,,T1T3^2014-01-15,17:00,19:25,ZH,4117,F7YABASAUA*9,CTU,PEK,321,0,1,1,True,,T2T3^2014-01-15,17:00,19:25,CA,4117,F7A5YABASAN1UA*9,CTU,PEK,321,0,1,1,False,,T2T3^2014-01-15,18:00,20:40,CA,4119,FAA6YASAN1UA*9,CTU,PEK,33A,0,1,1,False,,T2T3^2014-01-15,18:00,20:40,ZH,4119,FAYASAUA*9,CTU,PEK,33A,0,1,1,True,,T2T3^2014-01-15,18:45,21:30,CA,4197,F8A6O3YABASAN3UA*9,CTU,PEK,319,0,1,1,False,,T2T3^2014-01-15,18:45,21:30,ZH,4197,F8YABASAUA*9,CTU,PEK,319,0,1,1,True,,T2T3^2014-01-15,19:00,21:45,CA,4111,YASAN2UA*9,CTU,PEK,319,0,1,1,False,,T2T3^2014-01-15,19:00,21:45,ZH,4111,YASAUA*9,CTU,PEK,319,0,1,1,True,,T2T3^2014-01-15,19:30,22:10,3U,8891,F2A2YABATA*9,CTU,PEK,321,0,1,1,False,,T1T3^2014-01-15,20:30,23:10,3U,8895,F1A1Y7B5T7*9,CTU,PEK,320,0,1,1,False,,T1T3^2014-01-15,20:55,23:25,HU,7142,FAZ1A1YABAKAS5*9,CTU,PEK,787,0,0,1,False,,T2T1^2014-01-15,21:00,23:50,ZH,1426,F5YABASAUA*9,CTU,PEK,738,0,1,1,True,,T2T3^2014-01-15,21:00,23:50,CA,1426,F5A2YABASAN2UA*9,CTU,PEK,738,0,1,1,False,,T2T3^2014-01-15,21:10,23:45,CZ,3904,A4FAP1WAS1YABAUAQARA*9,CTU,PEK,333,0,1,1,False,P11,T2T2^2014-01-15,21:30,00:05,3U,8547,F6A6YABATA*9,CTU,PEK,320,0,1,1,False,,T1T3


                DataRow dr = ds.Tables[0].NewRow();
                dr[0] = index;
                dr[1] = sl2[0];
                dr[2] = sl2[1];
                dr[3] = sl2[2];
                dr[4] = sl2[3] ;
                dr[5] = sl2[4];
                dr[6] = sl2[5].Substring(0,sl2[5].Length-2);
                dr[7] = fromcityname;
                dr[8] = tocityname;
                dr[9] = sl2[8];
                dr[10] = sl2[14];
                ds.Tables[0].Rows.Add(dr);
                index++;
            }
            return ds.Tables[0];
        }

        public FrmYuDing()
        {
            InitializeComponent();
        }

        //查询航班
        private void button1_Click(object sender, EventArgs e)
        {
            //发送到PID端判断城市、日期的正确性
            string strResult = "";
            if (PublicClass.SendCommand(textBox1.Text.Trim() + "|" + textBox2.Text.Trim() + "|" + dateTimePicker1.Value.ToString("yyyy-MM-dd"), 2, ref strResult))
            {
                if (strResult.IndexOf("|") == -1)
                {
                    MessageBox.Show(strResult);
                    return;
                }
                else
                {
                    string[] sl = strResult.Split('|');
                    fromcitycode = sl[0];
                    fromcityname = sl[1];
                    tocitycode = sl[2];
                    tocityname = sl[3];
                    FlightDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");

                    //调用IBE接口查询航班
                    IBEServices.WebService1SoapClient ibe = new IBEServices.WebService1SoapClient();
                    avResult = ibe.getIBEAVData(fromcitycode, tocitycode, dateTimePicker1.Value.ToString("yyyy-MM-dd"), "00:00:00");

                    //2014-01-15,11:30,14:10,3U,8885,F2A2YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^
                    //2014-01-15,11:55,14:20,HU,7148,FAZ6A6YABAS1*9,CTU,PEK,333,0,1,1,False,,T2T1^
                    //2014-01-15,12:00,14:40,ZH,1406,YABASAUA*9,CTU,PEK,330,0,1,1,True,,T2T3^
                    //2014-01-15,12:00,14:40,CA,1406,YABASAN1UA*9,CTU,PEK,330,0,1,1,False,,T2T3^
                    //2014-01-15,12:15,14:45,CZ,6162,F7W7S7YAB7*9,CTU,PEK,321,0,1,1,False,,T2T2^
                    //2014-01-15,13:00,15:35,CA,421,C4YABASANAU2*9,CTU,PEK,320,0,1,1,False,,T2T3^
                    //2014-01-15,13:30,16:10,3U,8887,FAA7YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^
                    //2014-01-15,13:50,16:25,ZH,1416,F1YABASAUA*9,CTU,PEK,330,0,1,1,True,,T2T3^
                    //2014-01-15,13:50,16:25,CA,1416,F1YABASAUA*9,CTU,PEK,330,0,1,1,False,,T2T3^2014-01-15,14:00,16:40,CA,4103,F2YABASAN2UA*9,CTU,PEK,321,0,1,1,False,,T2T3^2014-01-15,14:00,16:40,ZH,4103,F2YABASAUA*9,CTU,PEK,321,0,0,1,True,,T2T3^2014-01-15,15:00,17:30,CA,4109,F8A3O1W5YAUA*9,CTU,PEK,333,0,1,1,False,,T2T3^2014-01-15,15:00,17:30,ZH,4109,F8YAUA*9,CTU,PEK,333,0,0,1,True,,T2T3^2014-01-15,15:30,18:05,3U,8893,F4YABATA*9,CTU,PEK,330,0,1,1,False,,T1T3^2014-01-15,16:00,18:35,CA,4105,F1CADAZ4R1*9,CTU,PEK,340,0,1,1,False,,T2T3^2014-01-15,16:00,18:35,ZH,4105,F1CA*9,CTU,PEK,340,0,1,1,True,,T2T3^2014-01-15,16:30,19:15,3U,8889,F1YABATA*9,CTU,PEK,321,0,1,1,False,,T1T3^2014-01-15,17:00,19:25,ZH,4117,F7YABASAUA*9,CTU,PEK,321,0,1,1,True,,T2T3^2014-01-15,17:00,19:25,CA,4117,F7A5YABASAN1UA*9,CTU,PEK,321,0,1,1,False,,T2T3^2014-01-15,18:00,20:40,CA,4119,FAA6YASAN1UA*9,CTU,PEK,33A,0,1,1,False,,T2T3^2014-01-15,18:00,20:40,ZH,4119,FAYASAUA*9,CTU,PEK,33A,0,1,1,True,,T2T3^2014-01-15,18:45,21:30,CA,4197,F8A6O3YABASAN3UA*9,CTU,PEK,319,0,1,1,False,,T2T3^2014-01-15,18:45,21:30,ZH,4197,F8YABASAUA*9,CTU,PEK,319,0,1,1,True,,T2T3^2014-01-15,19:00,21:45,CA,4111,YASAN2UA*9,CTU,PEK,319,0,1,1,False,,T2T3^2014-01-15,19:00,21:45,ZH,4111,YASAUA*9,CTU,PEK,319,0,1,1,True,,T2T3^2014-01-15,19:30,22:10,3U,8891,F2A2YABATA*9,CTU,PEK,321,0,1,1,False,,T1T3^2014-01-15,20:30,23:10,3U,8895,F1A1Y7B5T7*9,CTU,PEK,320,0,1,1,False,,T1T3^2014-01-15,20:55,23:25,HU,7142,FAZ1A1YABAKAS5*9,CTU,PEK,787,0,0,1,False,,T2T1^2014-01-15,21:00,23:50,ZH,1426,F5YABASAUA*9,CTU,PEK,738,0,1,1,True,,T2T3^2014-01-15,21:00,23:50,CA,1426,F5A2YABASAN2UA*9,CTU,PEK,738,0,1,1,False,,T2T3^2014-01-15,21:10,23:45,CZ,3904,A4FAP1WAS1YABAUAQARA*9,CTU,PEK,333,0,1,1,False,P11,T2T2^2014-01-15,21:30,00:05,3U,8547,F6A6YABATA*9,CTU,PEK,320,0,1,1,False,,T1T3

                    //解析IBE返回航班信息
                    dataGridView1.DataSource = GetUserData(avResult);
                }
            }
            else
            {
                MessageBox.Show("与服务器通讯异常！");
                return;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string[] sl=avResult.Split('^');

            //
            int index = e.RowIndex;

            string selectFlight = sl[index];

            //转到选择舱位及录入乘机人信息页面
            if ((tmpMainForm.tmpFrmSelectAndYuDing == null) || (tmpMainForm.tmpFrmSelectAndYuDing.IsDisposed))
            {
                tmpMainForm.tmpFrmSelectAndYuDing = new FrmSelectAndYuDing();
            }
            tmpMainForm.tmpFrmSelectAndYuDing.WindowState = FormWindowState.Maximized;
            tmpMainForm.tmpFrmSelectAndYuDing.MdiParent = tmpMainForm;
            tmpMainForm.tmpFrmSelectAndYuDing.fromcitycode = fromcitycode;
            tmpMainForm.tmpFrmSelectAndYuDing.fromcityname = fromcityname;
            tmpMainForm.tmpFrmSelectAndYuDing.tocitycode = tocitycode;
            tmpMainForm.tmpFrmSelectAndYuDing.tocityname = tocityname;
            tmpMainForm.tmpFrmSelectAndYuDing.FlightDate = FlightDate;
            tmpMainForm.tmpFrmSelectAndYuDing.selectFlight = selectFlight;

            tmpMainForm.tmpFrmSelectAndYuDing.tmpYuDing = this;
            tmpMainForm.tmpFrmSelectAndYuDing.tmpMain = tmpMainForm;
            //初始化显示
            tmpMainForm.tmpFrmSelectAndYuDing.InitShow();

            tmpMainForm.tmpFrmSelectAndYuDing.Show();
            tmpMainForm.tmpFrmSelectAndYuDing.Focus();
        }
    }
}
