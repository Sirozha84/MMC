using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace MMS_Server
{
    class Server
    {
        static List<Client> Clients = new List<Client>();
        static void Main(string[] args)
        {
            //Запускаем сервер
            try
            {
                TcpListener server = new TcpListener(IPAddress.Any, 80);
                server.Start();
                Console.WriteLine("MMC Server     Версия: 0.0.1     Автор: Сергей Гордеев");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Сервер запущен...");
                while (true)
                {
                    ThreadPool.QueueUserWorkItem(call, server.AcceptTcpClient());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Обработка запроса
        /// </summary>
        /// <param name="clientobject"></param>
        static void call(object clientobject)
        {
            TcpClient client = clientobject as TcpClient;
            using (NetworkStream stream = client.GetStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(stream);
                //Клиент просится подключиться, говорит своё имя
                string name = reader.ReadString();
                //Добавляем клиента в список и сообщаем полный список имён
                Clients.Add(new Client(stream, name));
                Clients.ForEach(o => writer.Write(o.Name));
                writer.Write("End");
                bool exit = false;
                do
                {
                    string s = reader.ReadString();
                    //Console.WriteLine(s);
                    //Сообщаем всем клиентам что сказал данный
                    Clients.ForEach(o => o.Send(s));
                    exit = s == "End";
                } while (!exit);
                //Тут клиент покинул чат, удаляем его...


            }
        }
    }

    class Client
    {
        public NetworkStream Stream;
        public string Name;
        public Client(NetworkStream Stream, string Name)
        {
            this.Stream = Stream;
            this.Name = Name;
        }
        public void Send(string str)
        {
            BinaryWriter writer = new BinaryWriter(Stream);
            writer.Write(str);
        }
    }
}
