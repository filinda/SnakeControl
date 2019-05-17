using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video.DirectShow;
using AForge.Video;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        VideoCaptureDevice frame;
        FilterInfoCollection Devices;

        byte[] data = new byte[10];
        bool moove = false;
        byte xdirect = 125;
        byte ydirect = 125;


        SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();
            data[0] = 15;
            data[9] = 15;
            serialPort = new SerialPort("COM6", 115200);
            try
            {
                serialPort.Open();
            }
            catch (IOException e)
            {
            } timer1.Start();
        }

        void Start_cam()
        {
            Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            frame = new VideoCaptureDevice(Devices[0].MonikerString);
            frame.NewFrame += new AForge.Video.NewFrameEventHandler(NewFrame_event);
            frame.Start();
        }

        string output;
        void NewFrame_event(object send, NewFrameEventArgs e)
        {
            try
            {
                pictureBox1.Image = (Image)e.Frame.Clone();
            }
            catch (Exception ex) { }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Up) {
                ydirect = 155;
            }
            if (e.KeyCode == Keys.Right)
            {
                xdirect = 155;
            }
            if (e.KeyCode == Keys.Left)
            {
                xdirect = 95;
            }
            if (e.KeyCode == Keys.Down)
            {
                ydirect = 95;
            }

            if (e.KeyCode == Keys.W)
            {
                moove = true;
            }

            if (e.KeyCode == Keys.F)
            {
                data[8] = 2;
            }

            if (e.KeyCode == Keys.G)
            {
                data[8] = 1;
            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up &  ydirect == 155)
            {
                ydirect = 125;
            }
            if (e.KeyCode == Keys.Right & xdirect == 155)
            {
                xdirect = 125;
            }
            if (e.KeyCode == Keys.Left & xdirect == 95)
            {
                xdirect = 125;
            }
            if (e.KeyCode == Keys.Down & ydirect == 95)
            {
                ydirect = 125;
            }

            if (e.KeyCode == Keys.W)
            {
                moove = false;
            }
        }

        float time = 0, last = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {

            data[1] = xdirect;
            data[2] = ydirect;

            time = DateTime.Now.Millisecond+DateTime.Now.Second*1000;
                label1.Text = "" + time;
                if (moove)
                {
                    for (int i = 3; i < 8; i += 1)
                    {
                        data[i] = (byte)(125 + (50 * Math.Sin(time / 200.0 + i * 0.9)));
                    }
                }
                else {
                    for (int i = 3; i < 8; i += 2)
                    {
                        data[i] = (byte)(125);
                    }
                }

                if (serialPort.IsOpen)
                {
                    serialPort.Write(data, 0, 10);
                }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Start_cam();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start_cam();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            frame.Stop();
        }
    }
}
