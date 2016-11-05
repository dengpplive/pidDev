using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using IClientAttributeSpace;

namespace testSend
{
    public partial class Form1 : Form
    {
        Socket sk = null;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sk.Connect(Dns.GetHostName(), 351);
            if (sk.Connected)
            {
                textBox1.Text = "success";
            }
            Thread t = new Thread(Recivie);
            t.IsBackground = true;
            t.Start();
        }

        void Recivie()
        {
            byte[] _byteData = new byte[sk.ReceiveBufferSize];
            do
            {
                try
                {
                    int byteSize = sk.Receive(_byteData);
                    byte[] _newbyte = new byte[byteSize];
                    Buffer.BlockCopy(_byteData, 0, _newbyte, 0, byteSize); //数据到newbyte

                    string Server_Rec_Info = Deserialize(_newbyte);
                    textBox1.Text = Server_Rec_Info;
                    Thread.Sleep(1);
                    if (sk.Connected==false)
                    {
                        sk.Close();
                        textBox1.Text = "close";
                    }
                }
                catch (Exception)
                {
                }

            } while (true);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            ClientInfo a = new ClientInfo(textBox2.Text, "123", textBox2.Text, textBox3.Text);
            sk.Send(Serialize(a));
        }


        private string Deserialize(byte[] _byte)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(_byte);
            object obj = bf.Deserialize(ms);
            ms.Close();
            return (string)obj;
        }

        private byte[] Serialize(object ds)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, ds);
            byte[] _byte = new byte[ms.ToArray().Length];
            _byte = ms.ToArray();
            ms.Close();
            return _byte;
        }

    }

 
}
