using System;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing.Presentation;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Dapper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1_mdi
{
    public partial class Input_barang : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private ZXing.BarcodeReader barcodeReader;
        private FilterInfoCollection devices;
        public Input_barang()
        {
            InitializeComponent();
            barcodeReader = new ZXing.BarcodeReader();

        }

        private void Input_barang_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        private void stop_input(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        private void start_input(object sender, EventArgs e)
        {

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[1].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("No camera devices found.");
            }



        }
        private HashSet<string> scannedResults = new HashSet<string>();

        private bool IsDuplicateScan(Result result)
        {
            string scannedText = result.Text;
            if (scannedResults.Contains(scannedText))
            {
                return true;
            }
            else
            {
                scannedResults.Add(scannedText);
                return false;
            }
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Bitmap resizedBitmap = new Bitmap(bitmap, pictureBox1.Width, pictureBox1.Height);


            ZXing.BarcodeReader barcodeReader = new ZXing.BarcodeReader();
            Result result = barcodeReader.Decode(bitmap);
            Result akhir = null;
            if (result != null && !IsDuplicateScan(result))
            {

                //Parsingquery(result.Text);
                inputquery(result.Text, textBox1.Text, textBox2.Text);

                akhir = result;

            }

            pictureBox1.Image = resizedBitmap;
        }

        private void inputquery(string s ,string e , string a)
        {
            string connectionString = "server=localhost;userid=root;password=root;database=mesin_kasir";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO list_barang (code_barang, nama_barang, harga) VALUES (@code, @nama,@harga);";

                    var parameter = new { code = s , nama = e, harga = a};


                    var selectedData = connection.Execute(query, parameter);

                    if(selectedData > 0)
                    {
                        MessageBox.Show("Data berhasil di insert");
                    }
                    else
                    {
                        MessageBox.Show("Data gagal  di insert");
                    }
                    
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


    }
    }
