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
            подключитьсяToolStripMenuItem_Click(null, null);
        }


        void Log(params string[] str)
        {
            richTextBoxLog.SelectionColor = Color.DimGray;
            richTextBoxLog.AppendText(DateTime.Now.ToString("[HH:mm] "));
            if (str.Length == 1)
            {
                richTextBoxLog.SelectionColor = Color.White;
                richTextBoxLog.AppendText(str[0]);
            }
            if (str[0] == "Message" & str.Length > 2)
            {
                richTextBoxLog.SelectionColor = Color.DarkGreen;
                richTextBoxLog.AppendText("<");
                richTextBoxLog.SelectionColor = Color.Green;
                richTextBoxLog.AppendText(str[1]);
                richTextBoxLog.SelectionColor = Color.DarkGreen;
                richTextBoxLog.AppendText("> ");
                richTextBoxLog.SelectionColor = Color.Yellow;
                richTextBoxLog.AppendText(str[2]);
            }
            if (str[0] == "Incoming" & str.Length > 1)
            {
                richTextBoxLog.SelectionColor = Color.Green;
                richTextBoxLog.AppendText(str[1]);
                richTextBoxLog.SelectionColor = Color.DarkGreen;
                richTextBoxLog.AppendText(" вошёл в чат");
            }
            if (str[0] == "Exit" & str.Length > 1)
            {
                richTextBoxLog.SelectionColor = Color.Red;
                richTextBoxLog.AppendText(str[1]);
                richTextBoxLog.SelectionColor = Color.DarkRed;
                richTextBoxLog.AppendText(" вышел из чата");
            }
            if (str[0] == "Rename" & str.Length > 2)
            {
                richTextBoxLog.SelectionColor = Color.Blue;
                richTextBoxLog.AppendText(str[1]);
                richTextBoxLog.SelectionColor = Color.DarkBlue;
                richTextBoxLog.AppendText(" теперь ");
                richTextBoxLog.SelectionColor = Color.Blue;
                richTextBoxLog.AppendText(str[2]);
            }

            richTextBoxLog.AppendText(Environment.NewLine);
            richTextBoxLog.ScrollToCaret();
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
            Properties.Settings.Default.Save();
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
                    Log("Message", name, mes);
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
                    Log("Incoming", ReadMessage());
                }
                if (message == "Exit")
                {
                    Log("Exit", ReadMessage());
                }
                if (message == "Rename")
                {
                    Log("Rename", ReadMessage(), ReadMessage());
                    //string oldname = ReadMessage();
                    //string newname = ReadMessage();
                    //Log(oldname + " теперь " + newname);
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
            string nick = Question("Введите своё имя в чате (ник):");
            Properties.Settings.Default.NickName = nick;
            Client.ChangeNick(nick);
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
            if (Client.Connected)
                Log("Подключение к серверу \"" + Properties.Settings.Default.Server + "\" установлено");
        }

        private void отключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client.Disconnect();
            Log("Подключение разорвано");
            listBoxUsers.Items.Clear();
        }
    }
}
