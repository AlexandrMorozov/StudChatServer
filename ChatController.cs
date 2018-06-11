using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudChatServer
{
    public static class ChatController//Управление подключениями
    {
        private const int _maxMessage = 1000;//Максимальное количество хранимых сообщений на сервере
        public static List<Message> Chat = new List<Message>();//Список хранимых сообщений
        public struct Message
        {
            public string user_name;//Имя отправителя
            public string data;//Информация сообщения
            public string user_ID;//ID отправителя
            public string companion_ID;//ID получателя
            public string time;//Время отправки сообщения
            public Message(string name, string msg,string user_id,string companion_id,string time_)
            {
                user_name = name;
                data = msg;
                user_ID = user_id;
                companion_ID = companion_id;
                time = time_;
            }
        }
        //Добавление сообщения в список хранимых сообщений
        public static void AddMessage(string userName, string msg,string user_id,string companion_id,string time)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(msg)) return;
                int countMessages = Chat.Count;
                if (countMessages > _maxMessage) ClearChat();//Очистка чата при превышении лимита храненимых сообщений
                Message newMessage = new Message(userName, msg,user_id,companion_id,time);//Создание объекта сообщения
                Chat.Add(newMessage);//Добавление сообщения
                Console.WriteLine("New message from {0}.", userName);
                Server.UpdateAllChats();//
            }
            catch (Exception exp) { Console.WriteLine("Error with addMessage: {0}.", exp.Message); }
        }
        public static void ClearChat()//Очистка списка хранимых сообщений
        {
            Chat.Clear();
        }
        public static string GetChat(Client client)
        {
            try
            {
                string data = "#updatechat&";//Индекс обновления чата для отправки сообщений
                int count_messages = Chat.Count;
                if (count_messages <= 0) return string.Empty;
                for (int i = 0; i < count_messages; i++)//Поиск подключения получателя
                {
                    //Если ID получателя и ID подключения совпадают
                    if ((Chat[i].user_ID==client.user_ID && Chat[i].companion_ID == client.companion_ID) || (Chat[i].user_ID == client.companion_ID && Chat[i].companion_ID == client.user_ID))
                    {
                        data += String.Format("{0}~{1}~{2}|", Chat[i].user_name, Chat[i].data,Chat[i].time);//Добавление сообщения к строке данных
                    }                
                }
                return data;
            }
            catch (Exception exp) { Console.WriteLine("Error with getChat: {0}", exp.Message); return string.Empty; }
        }
    }
}
