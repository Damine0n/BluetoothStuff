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

        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<byte> bytes = new List<byte>();
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
            Console.WriteLine("TEST");
            Console.ReadLine();
            try
            {
                if (!clientSocket.Connected)
                    clientSocket.Connect(IPAddress.Parse("192.168.55.1"),4000);

                int i = clientSocket.Send(Encoding.UTF8.GetBytes("$0800EC,CR"));
                MessageBox.Show(String.Format("Sent {0} bytes",i));
                clientSocket.ReceiveTimeout = 4000;
                i=clientSocket.Receive(bytes.ToArray());
                updateUI(String.Format("Received {0} bytes", i));
                updateUI(bytes.ToString());
                clientSocket.Disconnect(true);
                clientSocket.Close();
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //You need to close the send code
            }
            catch (SocketException ex)
            {
                MessageBox.Show(String.Format("{0} Error code: {1}.", ex.Message, ex.ErrorCode));
            }
        }
    }
}