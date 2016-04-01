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
    public partial class FormMain : Form
    {
        //Thread Listen = new Thread(new ThreadStart(Listener));
        Thread Listen = new Thread(new ParameterizedThreadStart(Listener));
        static NetworkStream Stream;
        static BinaryReader NetReader;
        static BinaryWriter NetWriter;
        static List<string> Messages = new List<string>();
        static string ServerName = "localhost";

        public FormMain()
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
            client.Connect(ServerName, 80);
            Stream = client.GetStream();
            NetReader = new BinaryReader(Stream);
            NetWriter = new BinaryWriter(Stream);
            //Просимся подключиться, сообщаем имя
            NetWriter.Write("Аноним");
            Log("Подключение к серверу \""+ ServerName+"\" установлено");
            Listen.Start(this);
        }

        static void Listener(object o)
        {
            while (true)
            {
                Messages.Add(NetReader.ReadString());
            }
        }

        void Log(string str)
        {
            if (textBoxLog.Text != "") textBoxLog.Text += Environment.NewLine;
            textBoxLog.Text += DateTime.Now.ToString("[HH:mm] ") + str;
            textBoxLog.Select(textBoxLog.Text.Length - 1, 0);
            textBoxLog.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetWriter.Write("Message");
            NetWriter.Write("All");
            NetWriter.Write(textBoxMessage.Text);
            textBoxMessage.Text = "";
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
                string message = ReadMessage();
                if (message == "Message")
                {
                    string name = ReadMessage();
                    string to = ReadMessage();
                    string mes = ReadMessage();
                    Log("<" + name + "> " + mes);
                }
                if (message == "RenewUserList")
                {
                    listBoxUsers.Items.Clear();
                    string n;
                    do
                    {
                        n = ReadMessage();
                        if (n != "End") listBoxUsers.Items.Add(n);
                    } while (n != "End");
                }
                if (message == "Incoming")
                {
                    Log(ReadMessage() + " вошёл в чат");
                }
                if (message == "Exit")
                {
                    Log(ReadMessage() + " вышел из чата");
                }
                if (message == "Rename")
                {
                    string oldname = ReadMessage();
                    string newname = ReadMessage();
                    Log(oldname + " теперь " + newname);
                }
            }
        }

        string ReadMessage()
        {
            if (Messages.Count > 0)
            {
                string s = Messages[0];
                Messages.RemoveAt(0);
                return s;
            }
            return null;
        }

        private void никToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newnick = Question("Введите своё имя в чате (ник):");
            if (newnick != "")
                //На самом деле надо сделать проверку на стороне сервера, где будет проверено
                //можно ли вообще иметь такой ник
            {
                NetWriter.Write("Rename");
                NetWriter.Write(newnick);
            }
        }

        string Question(string question)
        {
            using (FormTextInput form = new FormTextInput(question))
            {
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK) return form.Answer;
                else return "";
            }
        }

        private void серверToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Пока только этот...");
        }
    }
}
