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
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte [] recievedBytes = new byte[256];
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
            string Command = "00";
            string Parameter = "";
            int CheckSum = 24;
            string sendString="",receiveString="";
            updateUI("Test");
            
            try
            {
        //        int count1 = Parameter.Length + 8;
                //string CountString = String.Format("{0:X2}", count1);
                //int Checksum = 0;
                //string SendString = "$" + CountString + Command + Parameter;

                //for (int x = 0;x <= SendString.Length - 1;x++)
                //    Checksum += SendString.ToCharArray()[x].);
                //Next

                //Dim CheckString As String = String.Format("{0:X2}", Checksum);

                //SendString = SendString & CheckString.Substring(CheckString.Length - 2, 2) & ENDCHAR;
                
                
                if (!clientSocket.Connected)
                    clientSocket.Connect(IPAddress.Parse("192.168.55.1"),4000);
                byte[] sendPacket = new byte[8];
                sendPacket = Encoding.UTF8.GetBytes("$12013039054002E2" + vbCr);
                MessageBox.Show(sendPacket[5].ToString());
                MessageBox.Show(sendPacket[0].ToString("X"));
                string so = "0";
                MessageBox.Show(String.Format("{0:X2}",so));
                for (int j = 0; j < sendPacket.Length; j++)
                {
                    //CheckSum = CheckSum + Convert.ToInt16(sendPacket[j].ToString("X"));
                    sendString += String.Format(sendPacket[j].ToString("X"));
                }
                MessageBox.Show(sendString+CheckSum.ToString());
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


    }
}