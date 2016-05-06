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
            MessageBox.Show("My Micro Chat\nВерсия: " + Application.ProductVersion +
                "\nАвтор: Сергей Гордеев", "О программе MMC",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Client.Connect();
            Log("Подключение к серверу \"" + Properties.Settings.Default.Server + "\" установлено");
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
            Client.SendMessage(textBoxMessage.Text);
            textBoxMessage.Text = "";
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button1_Click(null, null);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client.Disconnect();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            подключитьсяToolStripMenuItem.Enabled = !Client.Connected;
            отключитьсяToolStripMenuItem.Enabled = Client.Connected;
            while (Client.Messages.Count > 0)
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
            if (Client.Messages.Count > 0)
            {
                string s = Client.Messages[0];
                Client.Messages.RemoveAt(0);
                return s;
            }
            return null;
        }

        private void никToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client.ChangeNick(Question("Введите своё имя в чате (ник):"));
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

        private void подключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client.Connect();
        }

        private void отключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client.Disconnect();
        }
    }
}
