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
using System.Threading;

namespace MMC
{
    public partial class Form1 : Form
    {
        //Thread Listen = new Thread(new ThreadStart(Listener));
        Thread Listen = new Thread(new ParameterizedThreadStart(Listener));
        static NetworkStream Stream;
        static List<string> Messages = new List<string>();

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
            TcpClient client = new TcpClient();
            client.Connect("localhost", 80);
            Stream = client.GetStream();
            BinaryReader reader = new BinaryReader(Stream);
            BinaryWriter writer = new BinaryWriter(Stream);
            //Просимся подключиться, сообщаем имя
            writer.Write("Sirozha");
            string n;
            do
            {
                n = reader.ReadString();
                if (n != "End") listBox1.Items.Add(n);
            } while (n != "End");
            Log("Подключение установлено");
            Listen.Start(this);
        }

        static void Listener(object o)
        {
            while (true)
            {
                BinaryReader reader = new BinaryReader(Stream);
                Messages.Add(reader.ReadString());
            }
        }

        void Log(string str)
        {
            textBox1.Text += DateTime.Now.ToString("[HH:mm] ") + str + Environment.NewLine;
            textBox1.Select(textBox1.Text.Length - 1, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BinaryReader reader = new BinaryReader(Stream);
            BinaryWriter writer = new BinaryWriter(Stream);
            writer.Write(textBox2.Text);
            textBox2.Text = "";
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button1_Click(null, null);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Listen.Abort();
            BinaryWriter writer = new BinaryWriter(Stream);
            writer.Write("End");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (Messages.Count > 0)
            {
                Log(Messages[0]);
                Messages.RemoveAt(0);
            }
        }
    }
}
