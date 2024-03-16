using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1_mdi.view_barang;
using MySql.Data.MySqlClient;
using Dapper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace WindowsFormsApp1_mdi
{
    public partial class Edit_barang : Form
    {
        view_barang view;
        string code = Global.code_brg;
        public Edit_barang()
        {
            InitializeComponent();
        }

        private void load(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;userid=root;password=root;database=mesin_kasir";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM list_barang where code_barang = @code_query";
                    var prams = new { code_query = code };
                    var selectedData = connection.Query(query,prams);

                    foreach (var data in selectedData)
                    {

                        textBox1.Text = data.code_barang;
                        textBox2.Text = data.nama_barang;
                        textBox3.Text = data.harga;
                    }

                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            }


        private void update_data(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;userid=root;password=root;database=mesin_kasir";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE list_barang SET code_barang=@code_baru, nama_barang = @nama_baru, harga = @harga_baru WHERE code_barang = @code_query";
                    var prams = new { code_query = code , code_baru = textBox1.Text, nama_baru=textBox2.Text, harga_baru = textBox3.Text };
                    var selectedData = connection.Execute(query, prams);

                    if (selectedData > 0)
                    {
                        MessageBox.Show("Data berhasil di ubah");
                        this.Close();
                        view_barang view = new view_barang();
                        view.Show();
                        view.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Data Gagal di ubah");
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
