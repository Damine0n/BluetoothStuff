using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Wireless_Test
{
    public partial class WirelessTest : Form
    {
        string vbCr = "\r";
        int num = 0;
        byte [] receivedBytes = new byte[256];
        bool stat=true;
        public WirelessTest()
        {
            InitializeComponent();
        }

        private void updateUI(string message)
        {
            Func<int> del = delegate()
            {
                tbOutput.AppendText(message + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string sendString = "$0802", receiveString = "";
            //$0F10HHIIIII,CHKS,CR
            processProtocol(sendString, true);
            //try
            //{             
            //    if (!clientSocket.Connected)
            //        clientSocket.Connect(IPAddress.Parse("192.168.55.1"),4000);
            //    byte[] sendPacket = new byte[8];
            //    sendPacket = Encoding.UTF8.GetBytes(sendString + CalculateChecksum(sendString) + vbCr);

            //    for (int j = 0; j < sendPacket.Length; j++)
            //    {
            //        sendString += String.Format(sendPacket[j].ToString("X"));
            //    }
            //    int i = clientSocket.Send(sendPacket);
            //    updateUI(String.Format("Sent {0} bytes", i));
            //    clientSocket.ReceiveTimeout = 4000;


            //    i=clientSocket.Receive(receivedBytes);
            //    updateUI(String.Format("Received {0} bytes", i));
            //    for (int l = 0;l<i;l++)
            //    {
            //        receiveString += receivedBytes[l].ToString(); 
            //    }
            //    receiveString=Encoding.ASCII.GetString(receivedBytes);
            //    updateUI("Received: "+receiveString);
            //    clientSocket.Disconnect(true);
            //    clientSocket.Close();
                
            //    //You need to close the send code
            //}
            //catch (SocketException ex)
            //{
            //    MessageBox.Show(String.Format("{0}.\nError code: {1}.", ex.Message, ex.ErrorCode));
            //}
        }
        private string CalculateChecksum(string dataToCalculate)
        {
            byte[] byteToCalculate = Encoding.ASCII.GetBytes(dataToCalculate);
            int checksum = 0;
            foreach (byte chData in byteToCalculate)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum.ToString("X2");
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                try
                {
                    processProtocol(tbInput.Text,true);
                    tbInput.Clear();
                }
                catch (Exception ex)
                {
                    tbOutput.Clear();
                    updateUI("Client has disconnected!!!~!!!" + System.Environment.NewLine + ex);
                }
            }
        }

        private void processProtocol(string p,bool status)
        {
            //$1085;0300D2;A1
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            byte[] sendPacket = new byte[256];
            if (!clientSocket.Connected)
                    clientSocket.Connect(IPAddress.Parse("192.168.55.1"), 4000);
            if (status.Equals(true))
            {
                sendPacket = Encoding.UTF8.GetBytes(p + CalculateChecksum(p) + vbCr);
                int i = clientSocket.Send(sendPacket);
                updateUI(String.Format("Sent {0} bytes", i));
                updateUI("Sent: " + Encoding.ASCII.GetString(sendPacket));
                clientSocket.ReceiveTimeout = 4000;

                i = clientSocket.Receive(receivedBytes);
                updateUI(String.Format("Received {0} bytes", i));
                string []arr = Encoding.ASCII.GetString(receivedBytes).Split(';');
                updateUI("Received: " + String.Format("{0:P1}",Convert.ToInt32(arr[1],16)));
                for (int j = 2; j < arr.Length-4;j++ )
                {
                    updateUI(Convert.ToInt32(arr[j].Substring(2,4),16).ToString().Insert(1,"."));
                }
                updateUI("Received: " + Encoding.ASCII.GetString(receivedBytes));
            }
            else 
            {
                sendPacket = Encoding.UTF8.GetBytes(p + CalculateChecksum(p) + vbCr);
                clientSocket.Send(sendPacket);
                clientSocket.ReceiveTimeout = 4000;

                int i = clientSocket.Receive(receivedBytes);
                updateUI(String.Format("Received {0} bytes", i));

                updateUI("Received: " + Encoding.ASCII.GetString(receivedBytes));
            }
            clientSocket.Disconnect(true);
            clientSocket.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            processProtocol(tbInput.Text,stat);
            stat = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tbOutput.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}