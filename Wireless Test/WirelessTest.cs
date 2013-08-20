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
        byte [] recievedBytes = new byte[256];
        public WirelessTest()
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerAsync(5);
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
            string Command = "00";
            string Parameter = "";
            int CheckSum = 24;
            string sendString="",receiveString="";
            //$0F10HHIIIII,CHKS,CR
            updateUI("Test");
            try
            {             
                if (!clientSocket.Connected)
                    clientSocket.Connect(IPAddress.Parse("192.168.55.1"),4000);
                byte[] sendPacket = new byte[8];
                sendPacket = Encoding.UTF8.GetBytes("$12013039054002" + CalculateChecksum("$12013039054002") + vbCr);

                for (int j = 0; j < sendPacket.Length; j++)
                {
                    //CheckSum = CheckSum + Convert.ToInt16(sendPacket[j].ToString("X"));
                    sendString += String.Format(sendPacket[j].ToString("X"));
                }
                int i = clientSocket.Send(sendPacket);
                updateUI(String.Format("Sent {0} bytes", i));
                clientSocket.ReceiveTimeout = 4000;


                i=clientSocket.Receive(recievedBytes);
                updateUI(String.Format("Received {0} bytes", i));
                for (int l = 0;l<i;l++)
                {
                    receiveString += recievedBytes[l].ToString(); 
                }
                receiveString=Encoding.ASCII.GetString(recievedBytes);
                updateUI("Test: "+receiveString);
                clientSocket.Disconnect(true);
                clientSocket.Close();
                
                //You need to close the send code
            }
            catch (SocketException ex)
            {
                MessageBox.Show(String.Format("{0}.\nError code: {1}.", ex.Message, ex.ErrorCode));
            }
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
                    processProtocol(tbInput.Text);
                    tbInput.Clear();
                }
                catch (Exception ex)
                {
                    tbOutput.Clear();
                    updateUI("Client has disconnected!!!~!!!" + System.Environment.NewLine + ex);
                }
            }
        }

        private void processProtocol(string p)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (!clientSocket.Connected)
                clientSocket.Connect(IPAddress.Parse("192.168.55.1"), 4000);
            byte[] sendPacket = new byte[256];
            sendPacket = Encoding.UTF8.GetBytes(p + CalculateChecksum(p) + vbCr);

            int i = clientSocket.Send(sendPacket);
            updateUI(String.Format("Sent {0} bytes", i));
            updateUI("Sent: " + Encoding.ASCII.GetString(sendPacket));
            clientSocket.ReceiveTimeout = 4000;

            i = clientSocket.Receive(recievedBytes);
            updateUI(String.Format("Received {0} bytes", i));
            
            updateUI("Received: " + Encoding.ASCII.GetString(recievedBytes));
            clientSocket.Disconnect(true);
            clientSocket.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            num++;
            MessageBox.Show(num.ToString());
        }
    }
}