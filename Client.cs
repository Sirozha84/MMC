using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace MMC
{
    static class Client
    {
        public static bool Connected = false;
        static Thread Listen;
        static NetworkStream Stream;
        static BinaryReader NetReader;
        static BinaryWriter NetWriter;
        public static List<string> Messages = new List<string>();

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        public static void Connect()
        {
            if (Connected) return;
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(Properties.Settings.Default.Server, 7770);
                Stream = client.GetStream();
                NetReader = new BinaryReader(Stream);
                NetWriter = new BinaryWriter(Stream);
                //Просимся подключиться, сообщаем имя
                NetWriter.Write("Аноним");
                Listen = new Thread(Listener);
                Listen.Start();
                Connected = true;
            }
            catch { Connected = false; }
        }

        /// <summary>
        /// Отключение от сервера
        /// </summary>
        public static void Disconnect()
        {
            try
            {
                Listen.Abort();
                BinaryWriter writer = new BinaryWriter(Stream);
                writer.Write("End");
            }
            catch { }
            Connected = false;
        }

        static void Listener()
        {
            while (true)
            {
                try
                {
                    Messages.Add(NetReader.ReadString());
                }
                catch
                {
                    //Листенер отвалился, отключаемся от сервера...
                }
            }
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message"></param>
        public static void SendMessage(string message)
        {
            try
            {
                NetWriter.Write("Message");
                NetWriter.Write("All");
                NetWriter.Write(message);
            }
            catch { Connected = false; }
        }

        /// <summary>
        /// Смена ника
        /// </summary>
        /// <param name="newnick"></param>
        public static void ChangeNick(string newnick)
        {
            try
            {
                if (newnick != "")
                //На самом деле надо сделать проверку на стороне сервера, где будет проверено
                //можно ли вообще иметь такой ник
                {
                    NetWriter.Write("Rename");
                    NetWriter.Write(newnick);
                }
            }
            catch { Connected = false; }
        }
    }
}
