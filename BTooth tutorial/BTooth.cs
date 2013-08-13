using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using genericSerial;

//J2KN
//"{00001101-0000-1000-8000-00805f9b34fb}"

//My Phone
//
namespace BTooth_tutorial
{
    public partial class BTooth : Form
    {
        List<string> items;
        BluetoothDeviceInfo deviceInfo;
        BluetoothDeviceInfo[] devices;
        BluetoothClient client = new BluetoothClient();
        string myPin = "1234";
        Guid mUUID = new Guid("{00001101-0000-1000-8000-00805f9b34fb}");
        bool serverStarted = false;
        bool ready = false;
        byte [] message;
        public BTooth()
        {
            InitializeComponent();
            items = new List<string>();
        }

        private void bGo_Click(object sender, EventArgs e)
        {
            tbOutput.Clear();
            listBox1.Enabled = true;
            if (serverStarted)
            {
                updateUI("Server already started");
                return;
            }
            if (rbClient.Checked)
            {
                startScan();
            }
            else
            {
                connectAsServer();
            }
        }

        private void startScan()
        {
            listBox1.DataSource = null;
            listBox1.Items.Clear();
            items.Clear();
            Thread bluetoothScanThread = new Thread(new ThreadStart(scan));
            bluetoothScanThread.Start();
        }

        private void scan()
        {            
            updateUI("Starting Scan...");
            
            devices = client.DiscoverDevicesInRange();
            updateUI("Scan complete");
            updateUI(devices.Length.ToString() + " devices discovered");
            foreach(BluetoothDeviceInfo d in devices)
            {
                items.Add(d.DeviceName);
            }

            updateDeviceList();
        }
        private void connectAsServer()
        {
            Thread bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            bluetoothServerThread.Start();
        }
        
        
        public void ServerConnectThread()
        {
            serverStarted = true;
            updateUI("Server started, waiting for clients");
            BluetoothListener blueListener = new BluetoothListener(mUUID);
            blueListener.Start();
            BluetoothClient conn = blueListener.AcceptBluetoothClient();
            updateUI("Client has connected");

            Stream mStream = conn.GetStream();
            while(true)
            {
                try
                {
                    //handle server connection
                    byte[] received = new byte[1024];
                    mStream.Read(received, 0, received.Length);
                    updateUI("Received " + Encoding.ASCII.GetString(received)+System.Environment.NewLine);
                    byte[] sent = Encoding.ASCII.GetBytes("Hello World");
                    mStream.Write(sent, 0, sent.Length);
                } catch(Exception ex){
                    updateUI("Client has disconnected!!!~!!!"+System.Environment.NewLine + ex);
                }

            }
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

        private void updateDeviceList()
        {
            Func<int> del = delegate()
            {
                listBox1.DataSource = items;
                return 0;
            };
            Invoke(del);
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            deviceInfo = devices.ElementAt(listBox1.SelectedIndex);
            tbOutput.Clear();
            listBox1.Enabled = false;
            updateUI(deviceInfo.DeviceName + " was selected, attempting connect. "+System.Environment.NewLine+"Address = "+ deviceInfo.DeviceAddress);
            if (pairDevice())
            {
                updateUI("device paired...");
                Thread bluetoothClientThread = new Thread(new ThreadStart(ClientConnectThread));
                bluetoothClientThread.Start();
            }
            else
            {
                updateUI("Pair Failed");
            }
        }
        private void ClientConnectThread()
        {
            updateUI("attempting connection...");
            client.BeginConnect(deviceInfo.DeviceAddress, mUUID,this.BluetoothClientConnectCallback,client);
        }
        void BluetoothClientConnectCallback(IAsyncResult result)
        {
            //if (result.IsCompleted)
            try
            {
                client = (BluetoothClient)result.AsyncState;
                client.EndConnect(result);

                Stream stream = client.GetStream();
                stream.ReadTimeout = 1000;
                updateUI("CONNECTED!!!");
                //bGo.Enabled = false;
            }
            catch (Exception ex)
            {
                updateUI("Did Not Connect:"+System.Environment.NewLine+ex.ToString());
            }
        }
        
        private bool pairDevice()
        {
            if (!deviceInfo.Authenticated)
            {
                if (!BluetoothSecurity.PairRequest(deviceInfo.DeviceAddress, myPin))
                {
                    return false;
                }
            }
            return true;
        }

        private void tbText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ready=true;
                tbText.Clear();
                
                Stream mStream =client.GetStream();
                mStream.ReadTimeout = 4000;
                try
                {
                    while (true)
                    {                    
                        //handle server connection
                        message = Encoding.ASCII.GetBytes(tbText.Text);
                        mStream.Write(message, 0, message.Length);
                        byte[] received = new byte[1024];
                        mStream.Read(received, 0, received.Length);
                        updateUI("Received " + Encoding.ASCII.GetString(received));
                    }
                }
                catch (Exception ex)
                {
                    updateUI("Client has disconnected!!!~!!!" + System.Environment.NewLine + ex);
                    bGo.Enabled=true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SerialSearch sSearch= new SerialSearch();
            sSearch.ShowDialog();
            this.Close();
        }
    }
}
