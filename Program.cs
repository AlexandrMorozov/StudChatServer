using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StudChatServer
{
    class Program
    {
        string host = Dns.GetHostName();
        //Присвоение IP-адреса серверу
        static string server_host =Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        private const int server_port = 9933;//Порт сервера
        private static Thread server_thread;//Прототип потока сервера
        static void Main(string[] args)
        {
            server_thread = new Thread(StartServer);//Запуск соединения с сервером через поток
            server_thread.IsBackground = true;
            server_thread.Start();//Запуск потока
            while (true)
                HandlerCommands(Console.ReadLine());
        }
        private static void HandlerCommands(string cmd)//Вывод информации о новом подключении на консоль
        {
            cmd = cmd.ToLower();
            if (cmd.Contains("/getusers"))
            {
                int countUsers = Server.Clients.Count;
                for (int i = 0; i < countUsers; i++)
                {
                    Console.WriteLine("[{0}]: {1}", i, Server.Clients[i].UserName);
                }
            }
        }
        private static void StartServer()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(server_host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, server_port);
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);//Создание объекта сокета
            socket.Bind(ipEndPoint);
            socket.Listen(1000);//Прослушивание сервером сокета на предмет новых подключений
            Console.WriteLine("Server has been started on IP: {0}.", ipEndPoint);
            while (true)
            {
                try//Создание нового подключения
                {
                    Socket user = socket.Accept();
                    Server.NewClient(user);
                }
                catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }
            }

        }
    }
}
