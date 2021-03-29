using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace inLab1OmerKose25224
{
    public partial class Form1 : Form
    {

        bool connected = false;
        bool terminating = false;
        
        Socket clientSocket;

        private void form1FormClosing(object sender, EventArgs e)
        {
            connected = false;
            terminating = false;
            Environment.Exit(0);
        }

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(form1FormClosing);
            InitializeComponent();
        }

        private void Recieve()
        {
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);

                    int sum = 0;
                    string incommingMessage = Encoding.Default.GetString(buffer);
                    incommingMessage = incommingMessage.Substring(0, incommingMessage.IndexOf("\0"));
                    richTextBoxLog.AppendText("Server: " + incommingMessage + "\n");


                    int number = 0;
                    Int32.TryParse(incommingMessage, out number);

                    while (number != 0)
                    {
                        sum += number % 10;
                        number /= 10;
                    }

                    

                    string message = sum + " " + textBoxName.Text;
                    richTextBoxLog.AppendText("Message sent: " + message + "\n");

                    byte[] buffer2 = Encoding.Default.GetBytes(message);
                    clientSocket.Send(buffer2);

                    clientSocket.Receive(buffer);
                    incommingMessage = Encoding.Default.GetString(buffer);
                    incommingMessage = incommingMessage.Substring(0, incommingMessage.IndexOf("\0"));
                    richTextBoxLog.AppendText("Server: " + incommingMessage + "\n");

                    connected = false; // TERMINATE THE LOOP AFTER 1 CYCLE
                }
                catch
                {
                    if (!terminating)
                    {
                        richTextBoxLog.AppendText("The client is disconnected.");
                    }
                    clientSocket.Close();
                    connected = false; // TERMINATE THE LOOP
                }
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBoxIp.Text;
            int portNumber;

            if(Int32.TryParse(textBoxPort.Text, out portNumber))
            {
                //richTextBoxLog.AppendText(portNumber.ToString());
                try
                {
                    clientSocket.Connect(IP, portNumber);
                    connected = true;
                    richTextBoxLog.AppendText("Connection established... \n");

                    Thread recieveThread = new Thread(Recieve);
                    recieveThread.Start();
                }
                catch
                {
                    richTextBoxLog.AppendText("Could not connect to the server. \n");
                }
            }
            else
            {
                richTextBoxLog.AppendText("Problem occurred while connecting. \n");
            }
        }
    }
}
