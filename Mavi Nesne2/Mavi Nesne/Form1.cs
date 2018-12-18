using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge;
using AForge.Math.Geometry;
using System.IO.Ports;

namespace Mavi_Nesne
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice kamera;
        private FilterInfoCollection cihazlar;
        public Form1()
        {
            InitializeComponent();
        }
        int R;
        int G;
        int B;
        int pin;
        int proje = 1;
        int state;
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.DataSource = SerialPort.GetPortNames();
            cihazlar = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo cihaz in cihazlar)
            {
                comboBox1.Items.Add(cihaz.Name);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
                pictureBox1.Image = null;
                pictureBox1.Invalidate();
                kamera = new VideoCaptureDevice(cihazlar[comboBox1.SelectedIndex].MonikerString);
                kamera.NewFrame += new NewFrameEventHandler (kamera_NewFrame);
                kamera.Start();
        }
        private void kamera_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            image1.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = image;
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor = new RGB(Color.FromArgb(R, G, B)); 
            filter.Radius = 100;
            filter.ApplyInPlace(image1);        
            cevreal(image1);
        }
        private void cevreal(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 2;
            blobCounter.MinHeight = 2;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);
            blobCounter.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            
            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0)
                {
                    Rectangle objectRect = rects[0];
                    Graphics g = Graphics.FromImage(image);
                    using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }
                    int objectX = objectRect.X + (objectRect.Width / 2); //Dikdörtgenin Koordinatlari alınır.
                    int objectY = objectRect.Y + (objectRect.Height / 2);//Dikdörtgenin Koordinatlari alınır.
                    g.DrawString(objectX.ToString() + "X" + objectY.ToString() + "Y", new Font("Arial", 12), Brushes.Red, new System.Drawing.Point(100, 2));
                    g.Dispose();
                    if (proje == 1)
                    {
                        if (objectX > 0 && objectX < 106 && objectY < 80)
                        {
                            pin = 1;
                        }
                        if (objectX > 106 && objectX < 212 && objectY < 80)
                        {
                            pin = 2;
                        }
                        if (objectX > 212 && objectX < 320 && objectY < 80)
                        {
                            pin = 3;
                        }
                        if (objectX > 0 && objectX < 106 && objectY > 80 && objectY < 160)
                        {
                            pin = 4;
                        }
                        if (objectX > 106 && objectX < 212 && objectY > 80 && objectY < 160)
                        {
                            pin = 5;
                        }
                        if (objectX > 212 && objectX < 320 && objectY > 80 && objectY < 160)
                        {
                            pin = 6;
                        }
                        if (objectX > 0 && objectX < 106 && objectY > 160 && objectY < 240)
                        {
                            pin = 7;
                        }
                        if (objectX > 106 && objectX < 212 && objectY > 160 && objectY < 240)
                        {
                            pin = 8;
                        }
                        if (objectX > 212 && objectX < 320 && objectY > 160 && objectY < 240)
                        {
                            pin = 9;
                        }
                        if (serialPort1.IsOpen)
                        {
                            serialPort1.Write(pin.ToString());
                        }
                    }
                    else if (proje==2)
                    {
                        if (objectX > 0 && objectX <= 130 )
                        {
                            state = 1;
                        }
                        else if (objectX > 130 && objectX <=190)
                        {
                            state = 2;
                        }
                        else if (objectX > 190 && objectX <= 320)
                        {
                            state = 3;
                        }

                    }
                    if (serialPort1.IsOpen)
                    {
                        serialPort1.Write(state.ToString());
                    }
                    
                }
            }
            pictureBox2.Image = image;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (kamera.IsRunning)
                kamera.Stop();
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R = trackBar1.Value;
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            G = trackBar2.Value;
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            B = trackBar3.Value;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.BaudRate = 9600;
            serialPort1.PortName = comboBox2.SelectedItem.ToString();
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
               MessageBox.Show("Port Açık");
           }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            MessageBox.Show("Port Kapalı");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (kamera.IsRunning)
            {
                kamera.Stop(); 
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Programı Kapatmak İstiyor Musunuz ?", "Dikkat",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
                Application.Exit();
            }
            else
            {
            }
    }

        private void proje2gecis_Click(object sender, EventArgs e)
        {
            if (proje==1)
            {
                proje = 2;
                proje2gecis.BackColor = Color.FromArgb(255, 0, 0);
                proje2gecis.ForeColor = Color.White;
            }
            else if (proje==2)
            {
                proje = 1;
                proje2gecis.BackColor = Color.FromArgb(255, 255, 255);
                proje2gecis.ForeColor = Color.Black;
            }
        }
    }
}
