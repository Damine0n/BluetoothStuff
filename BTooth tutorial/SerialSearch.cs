using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InTheHand.Net;
using InTheHand.Net.Ports;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.IO;
using System.IO.Ports;


namespace genericSerial
{
    public partial class SerialSearch : Form
    {
        private Queue<byte> recievedData = new Queue<byte>();
        private string readBuffer = string.Empty;
        private int Bytenumber;
        private char[] byteEnd = new char[3];
        static SerialPort _serial = null;
        
        public SerialSearch()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //richTextBox1.Text = serialPort1.Read().ToString();
            System.Diagnostics.Debug.WriteLine("Test1");
            Console.WriteLine("Test");
            Console.Read();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                string text = textBox1.Text;
                serialPort1.WriteLine(text);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            listBox1.Items.Clear();
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            richTextBox1.Text += "The following serial ports were found:";

            // Display each port name to the console. 
            foreach (string port in ports)
            {
                listBox1.Items.Add(port);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int i=0;
            string lines="";
            List<string> buffer = new List<string>();
            serialPort1.Close();
            serialPort1.PortName = listBox1.SelectedItem.ToString();
            listBox1.Items.Clear();
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); 
                richTextBox1.Text += "\nOpened";
                string text = textBox1.Text;
                serialPort1.WriteLine(text);
                while(i<10){
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    lines+=serialPort1.ReadExisting()+"\n";
                    i++;
                }
             }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {// this is a workaround because of an SDK bug
            StringBuilder received = new StringBuilder();
            while (_serial.BytesToRead > 0)
                received.Append(ReadString());
            //string received = ReadString();
            // write the text out to the screen
            richTextBox1.Clear();
            richTextBox1.Text="Message from HQ:";
            richTextBox1.Text+="\n"+received.ToString();
            richTextBox1.Refresh();
            // send back an acknowledgement
            SendString("Agent 007 acknowledges message: " + received);
        }
        static string ReadString()
        {
            StringBuilder output = new StringBuilder();
            int bufferSize = _serial.BytesToRead;
            if (bufferSize > 0)
            {
                byte[] buffer = new byte[bufferSize];
                int read = _serial.Read(buffer, 0, buffer.Length);
                output.Append(System.Text.Encoding.UTF8.GetChars(buffer));
                return output.ToString();
            }
            return "";
        }
        static void SendString(string message)
        {
            char[] charArray = message.ToCharArray();
            byte[] byteArray = new byte[charArray.Length];
            for (int i = 0; i < message.Length; i++)
            {
                byteArray[i] = (byte)charArray[i];
            }
            _serial.Write(byteArray, 0, charArray.Length);
        }
        //    //byte[] data = new byte[serialPort1.BytesToRead];
        //    //serialPort1.Read(data,0,data.Length);

        //    //data.ToList().ForEach(b => recievedData.Enqueue(b));
        //    try
        //    {
        //        int count = serialPort1.BytesToRead;
        //        byte[] ByteArray = new byte[count];
        //        MessageBox.Show(serialPort1.Read(ByteArray, 0, count).ToString());
        //        richTextBox1.Text = serialPort1.Read(ByteArray, 0, count).ToString();
        //        byteEnd = serialPort1.NewLine.ToCharArray();

        //        // get number off bytes in buffer 
        //        Bytenumber = serialPort1.BytesToRead;

        //        // read one byte from buffer 
        //        //ByteToRead = SerialPort1.ReadByte() 

        //        // read one char from buffer 
        //        //CharToRead = SerialPort1.ReadChar() 

        //        // read until string "90" 
        //        //readBuffer1 = SerialPort1.ReadTo("90") 

        //        // read entire string until .Newline  
        //        readBuffer = serialPort1.ReadLine();
        //        //data to UI thread 
        //        this.Invoke(new EventHandler(DoUpdate));
        //        processdata();
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBox1.Text="read " + ex.Message;
        //    } 
            
        //}

        //private void DoUpdate(object sender, EventArgs e)
        //{
        //    richTextBox1.Text += readBuffer;
        //}

        //private void processdata()
        //{
        //    // Determine if we have a "packet" in the queue
        //    if(recievedData.Count > 5)
        //    {
        //        var packet = Enumerable.Range(0, 50).Select(i => recievedData.Dequeue());
        //    }
        //}

        //public void Dispose()
        //{
        //    if (serialPort1 != null)
        //        serialPort1.Dispose();
        //}

        private void SerialSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }
    }
}
