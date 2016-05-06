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
        static bool Connected = false;
        static Thread Listen = new Thread(Listener);
        static NetworkStream Stream;
        static BinaryReader NetReader;
        static BinaryWriter NetWriter;
        public static List<string> Messages = new List<string>();

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        public static void Connect()
        {
            TcpClient client = new TcpClient();
            client.Connect(Properties.Settings.Default.Server, 7770);
            Stream = client.GetStream();
            NetReader = new BinaryReader(Stream);
            NetWriter = new BinaryWriter(Stream);
            //Просимся подключиться, сообщаем имя
            NetWriter.Write("Аноним");
            Listen.Start();
        }

        /// <summary>
        /// Отключение от сервера
        /// </summary>
        public static void Disconnect()
        {
            Listen.Abort();
            BinaryWriter writer = new BinaryWriter(Stream);
            writer.Write("End");
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
            NetWriter.Write("Message");
            NetWriter.Write("All");
            NetWriter.Write(message);
        }

        /// <summary>
        /// Смена ника
        /// </summary>
        /// <param name="newnick"></param>
        public static void ChangeNick(string newnick)
        {
            if (newnick != "")
            //На самом деле надо сделать проверку на стороне сервера, где будет проверено
            //можно ли вообще иметь такой ник
            {
                NetWriter.Write("Rename");
                NetWriter.Write(newnick);
            }
        }
    }
}
