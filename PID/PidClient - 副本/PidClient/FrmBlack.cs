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
    public partial class FrmBlack : Form
    {
        public FrmBlack()
        {
            InitializeComponent();
        }

        private void FrmBlack_Shown(object sender, EventArgs e)
        {
            richTextBox1.HideSelection = false;
            richTextBox1.AppendText(">");
            richTextBox1.Focus();
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case '':
                    //在当前光标位置插入起始符
                    richTextBox1.Text.Insert(richTextBox1.SelectionStart, ">");
                    break;
            }
            
        }

        //获取待发送指令
        private string GetSendCommand()
        {
            string cmd = "";

            string Content = richTextBox1.Text;
            //按照分隔符分割
            string[] sl = Content.Split('\n');

            //获得光标所在行号
            int RowIndex = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);

            //光标所在列号
            int col = richTextBox1.SelectionStart - (richTextBox1.GetFirstCharIndexFromLine(1 + richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) - 1));

            //当前行内容
            string curLineContent = sl[RowIndex].Substring(0,col);

            int i=1;
            cmd = curLineContent;
            //查找最后一个起始符
            while(cmd.IndexOf(">")==-1 && (RowIndex-i>=0))
            {
                cmd = sl[RowIndex-i]+"\r\n"+cmd;
                i++;
            }

            if (cmd.IndexOf(">") != -1)
            {
                cmd = cmd.Substring(cmd.LastIndexOf(">") + 1);
            }
            
            return cmd;
          
            ////当前最大行数
            //int MaxLines = richTextBox1.Lines.Count();            

            ////检索当前光标所在行的第一个字符索引
            //int curLineFirstCharIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();

            ////下一行第一个字符索引
            //int NextLineFirstCharIndex = richTextBox1.GetFirstCharIndexFromLine(RowIndex + 1);



            //richTextBox1.Lines.

            //int curIndex = richTextBox1.SelectionStart;
            ////设置光标到开始位置
            //richTextBox1.SelectionStart = 0;
            //richTextBox1.SelectionLength = richTextBox1.TextLength;

            

            //richTextBox1.SelectionStart = 5;
            //richTextBox1.SelectionLength = 2;
            //textBox1.Text = richTextBox1.SelectedRtf; 
            

            //行值.Text = (1 + richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart)).ToString();

            //列值.Text = (1 + richTextBox1.SelectionStart - (richTextBox1.GetFirstCharIndexFromLine(1 + richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) - 1))).ToString();

            //return cmd;
        }

        //发送指令
        private string SendCommandAndGetResult()
        {
            //获取待发送指令
            string cmd =GetSendCommand();

            //预处理指令
            string ErrorMessage = "";
            string precmd = PrepareCommand.DuelCommand(cmd, ref ErrorMessage);

            if (precmd == "")
            {
                if (ErrorMessage != "")
                {
                    richTextBox1.AppendText("\n"+ErrorMessage);
                }
                richTextBox1.AppendText("\n>");
                richTextBox1.SelectionStart = richTextBox1.TextLength;
            }
            else
            {
                string strResult = "";
                if (PublicClass.SendCommand(precmd,1, ref strResult))
                {
                    //格式化返回结果（主要处理AVH返回）
                    strResult = PublicClass.FormatResult(precmd, strResult);

                    richTextBox1.AppendText("\n" + strResult + "\n>");
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    //分析返回结果
                    PrepareCommand.AnalyseCommandResult(precmd, strResult);
                }
                else
                {
                    MessageBox.Show(strResult);
                }
            }

            return cmd;
        }

        //在当前光标位置插入粘贴板内容
        private void PasteToRichTextBox()
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                richTextBox1.Paste();
            }
        }


        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F12:
                    //提交指令
                    string strResult = SendCommandAndGetResult();
                    break;
                case Keys.Escape:
                    //在当前光标位置插入起始符
                    int curindex = richTextBox1.SelectionStart;
                    richTextBox1.Text = richTextBox1.Text.Insert(richTextBox1.SelectionStart, ">");
                    richTextBox1.SelectionStart = curindex + 1;
                    break;
            }
        }



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //添加指令提示
            //string cmd = GetSendCommand();
            //if (cmd.ToLower() == "av" || cmd.ToLower() == "avh")
            //{
                
            //}
        }
    }
}
