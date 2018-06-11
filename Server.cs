using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace StudChatServer
{
    public static class Server
    {
        public static List<Client> Clients = new List<Client>();//Список подключений
        public static void NewClient(Socket handle)//Создание нового подключения
        {
            try
            {
                Client new_client = new Client(handle);//Создание объекта подключения
                Clients.Add(new_client);//Добавление объекта в список
                Console.WriteLine("New client connected: {0}", handle.RemoteEndPoint);
            }
            catch (Exception exp) { Console.WriteLine("Error with addNewClient: {0}.", exp.Message); }
        }
        public static void EndClient(Client client)//Разрыв подключения
        {
            try
            {
                client.End();
                Clients.Remove(client);//Удаление объекта из списка
                Console.WriteLine("User {0} has been disconnected.", client.UserName);
            }
            catch (Exception exp) { Console.WriteLine("Error with endClient: {0}.", exp.Message); }
        }
        public static void UpdateAllChats()//Отправка подключений для обновления
        {
            try
            {
                int count_users = Clients.Count;
                for (int i = 0; i < count_users; i++)
                {
                    Clients[i].UpdateChat(Clients[i]);
                }
            }
            catch (Exception exp) { Console.WriteLine("Error with updateAlLChats: {0}.", exp.Message); }
        }

    }
}
