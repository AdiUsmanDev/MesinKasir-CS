using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Dapper;
using System.Collections;

namespace WindowsFormsApp1_mdi
{
    public partial class view_barang : Form
    {
        public view_barang()
        {
            InitializeComponent();
            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void view_barang_load(object sender, EventArgs s)
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Code Barang", 150);
            listView1.Columns.Add("Nama Item", 150);
            listView1.Columns.Add("Harga", 100);
            string connectionString = "server=localhost;userid=root;password=root;database=mesin_kasir";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM list_barang where code_barang";
                    var selectedData = connection.Query(query);

                    foreach (var data in selectedData)
                    {

                        ListViewItem newItem = new ListViewItem(data.code_barang);
                        newItem.SubItems.Add(data.nama_barang);
                        newItem.SubItems.Add(data.harga);

                        listView1.Items.Add(newItem);
                    }

                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                string selectedValue = e.Item.Text; // Mendapatkan nilai teks dari item yang dipilih
                MessageBox.Show("Selected Value: " + selectedValue);
            }
        }
    }
}
