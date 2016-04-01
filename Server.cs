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
        public static List<Client> Clients = new List<Client>();
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
                Client User = new Client(stream, name);
                Clients.Add(User);
                //Оповестим всех о присоединении
                Client.SendToAll("Incoming", User);
                Client.SendToAll(User.Name, User);
                Client.SendUserList();
                bool exit = false;
                do
                {
                    string command = reader.ReadString();
                    if (command == "Message")
                    {
                        Client.SendToAll("Message");
                        Client.SendToAll(User.Name);
                        Client.SendToAll(reader.ReadString());
                        Client.SendToAll(reader.ReadString());
                    }
                    if (command == "Rename")
                    {
                        string oldname = User.Name;
                        User.Name = reader.ReadString();

                        //Надо сообщить в чат о смене имени
                        Client.SendToAll("Rename");
                        Client.SendToAll(oldname);
                        Client.SendToAll(User.Name);
                        Client.SendUserList();
                        Client.SendUserList();
                    }
                    exit = command == "End";
                } while (!exit);
                //Тут клиент покинул чат, удаляем его...
                Clients.Remove(User);
                //И оповещаем остальных об его уходе
                Client.SendToAll("Exit", User);
                Client.SendToAll(User.Name, User);
                Client.SendUserList();
            }
        }
    }

    class Client
    {
        public NetworkStream Stream;
        public string Name;
        BinaryReader Reader;
        BinaryWriter Writer;

        public Client(NetworkStream Stream, string Name)
        {
            this.Stream = Stream;
            this.Name = Name;
            Reader = new BinaryReader(Stream);
            Writer = new BinaryWriter(Stream);

        }
        void Send(string str)
        {
            Writer.Write(str);
        }

        static public void SendToAll(string str)
        {
            Server.Clients.ForEach(o => o.Send(str));
        }

        static public void SendToAll(string str, Client Except)
        {
            Server.Clients.ForEach(o =>
            {
                if (o != Except) o.Send(str);
            });
        }

        static public void SendUserList()
        {
            Server.Clients.ForEach(o =>
            {
                o.Send("RenewUserList");
                Server.Clients.ForEach(c => o.Send(c.Name));
                o.Send("End");
            });
        }
    }
}
