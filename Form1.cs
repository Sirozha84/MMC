using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;

namespace MMC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("My Micro Chat\nВерсия: 0.0.1\n" +
                "Автор: Сергей Гордеев", "О программе MMC",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect("localhost", 80);
                using (NetworkStream stream = client.GetStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);
                    //Просимся подключиться, сообщаем имя
                    writer.Write("Sirozha");
                    string n;
                    do {
                        n = reader.ReadString();
                        if (n != "End") listBox1.Items.Add(n);
                    } while (n != "End");
                    Log("Подключение установлено");
                }
            }
        }

        void Log(string str)
        {
            textBox1.Text += DateTime.Now.ToString("[HH:mm] ") + str;
        }
    }
}
