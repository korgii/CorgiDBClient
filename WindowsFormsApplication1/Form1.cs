using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string SetLabel1Text { get { return label1.Text; } set { label1.Text = value; } }
        public string SetLabel2Text { get { return label2.Text; } set { label2.Text = value; } }
        public string SetLabel3Text { get { return label3.Text; } set { label3.Text = value; } }
        public string SetLabel4Text { get { return label4.Text; } set { label4.Text = value; } }

        public ComboBox GetComboBox1 { get { return comboBox1; } }
        public ComboBox GetComboBox2 { get { return comboBox2; } }

        public Image ChangePictureBoxImage
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DBClient.accessToken))
            {
                MessageBox.Show("You must enter a valid access token");
                return;
            }

            DBClient.RunDropboxApi();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            DBClient.accessToken = textBox1.Text;
        }

        public void ListFilesToComboBox(List<string> list, ComboBox comboBox)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = list;
            comboBox.DataSource = bs;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            linkLabel1.Links.Remove(linkLabel1.Links[0]);
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "https://blogs.dropbox.com/developers/2014/05/generate-an-access-token-for-your-own-account/");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DBClient.BeforeDownload();
        }
    }
}
