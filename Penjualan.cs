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

namespace WindowsFormsApp1_mdi
{
    public partial class Penjualan : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private ZXing.BarcodeReader barcodeReader;
        private FilterInfoCollection devices;

        public Penjualan()
        {
            InitializeComponent();
            barcodeReader = new ZXing.BarcodeReader();
        }

        private void Penjualan_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        private void Penjualan_Load(object sender, EventArgs e)
        {

            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                listBox1.Items.Add(device.Name);
            }

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

            listView1.View = View.Details;
            listView1.Columns.Add("Nama Item", 150); 
            listView1.Columns.Add("Jumlah", 100);   
            listView1.Columns.Add("Harga", 100);


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

                Parsingquery(result.Text);

                akhir = result;
                
            }

            pictureBox1.Image = resizedBitmap;
        }
 
      //tidak di gunakan , digunakan pada saat debug program
        private void Parsing(string s)
        {
            ListViewItem newItem = new ListViewItem(s); 
            newItem.SubItems.Add("10");                       
            newItem.SubItems.Add("$20");

            listView1.Items.Add(newItem);
        }

        private void Parsingquery(string s)
        {
            string connectionString = "server=localhost;userid=r{sesuaikan};password={sesuaikan};database={sesuaikan}";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM list_barang where code_barang = @code";

                    var parameter = new { code = s };

                     
                    var selectedData = connection.Query(query, parameter);

                   
                    foreach (var data in selectedData)
                    {
                        //Console.WriteLine($"Nama: {data.nama_barang}, Harga: {data.harga}");
                        ListViewItem newItem = new ListViewItem(data.nama_barang);
                        newItem.SubItems.Add("1");
                        newItem.SubItems.Add(data.harga);

                        listView1.Items.Add(newItem);


                        int semua = 0 ;

                        foreach (ListViewItem item in listView1.Items)
                        {
                            int total;
                            
                            if(int.TryParse(item.SubItems[2].Text, out total)){

                                semua += total;
                            }
                        }


                        textBox_total.Text = semua.ToString();

                    }


                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void bayar(object sender, EventArgs e)
        {
            int total = int.Parse(textBox_total.Text);
            int bayar = int.Parse(textBox_uang.Text);

            int kembalian = bayar - total;

            textBox_kembalian.Text = kembalian.ToString();


        }

    }
}

