using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace StudChatServer
{
    public class Client
    {
        private string user_name;//Имя инициатора подключения
        public string user_ID;//ID имя инициатора подключения
        public string companion_ID;//ID собеседника инициатора подключения
        private Socket _handler;//Сокет подключения
        private Thread _userThread;
        public Client(Socket socket)
        {
            _handler = socket;
            _userThread = new Thread(Listner);
            _userThread.IsBackground = true;
            _userThread.Start();
        }
        public string UserName
        {
            get { return user_name; }
        }
        private void Listner()//Прослушивание сокета на предмет поступления сообщений
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];//Буффер приёма сообщений
                    int bytesRec = _handler.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);//Получение сообщений
                    MessageType(data);//Определение типа сообщения
                }
                catch { Server.EndClient(this); return; }
            }
        }
        public void End()//Завершение подключения
        {
            try
            {
                _handler.Close();
                try
                {
                    _userThread.Abort();
                }
                catch { } 
            }
            catch (Exception exp) { Console.WriteLine("Error with end: {0}.", exp.Message); }
        }
        private void MessageType(string data)//Определение типа сообщения
        {
            if (data.Contains("#setname"))//Сообщение для установки данных получателя сообщений на сервере
            {
                user_name = data.Split('&')[1];
                user_ID= data.Split('&')[2];
                companion_ID = data.Split('&')[3];
                return;
            }
            if (data.Contains("#newmsg"))//Сообщение, отправленное пользователем собеседнику
            {
                string message = data.Split('&')[1];
                string time= data.Split('&')[2];
                ChatController.AddMessage(user_name, message,user_ID,companion_ID,time);//Добавление сообщения
                return;
            }
        }
        public void UpdateChat(Client client)//Обновление подключений
        {
            Send(ChatController.GetChat(client));//Отпрвака сообщений получателям
        }
        public void Send(string command)//Отпрвака сообщений получателям
        {
            try
            {
                int bytesSent = _handler.Send(Encoding.UTF8.GetBytes(command));
                if (bytesSent > 0) Console.WriteLine("Success");
            }
            catch (Exception exp) { Console.WriteLine("Error with send command: {0}.", exp.Message); Server.EndClient(this); }
        }
    }
}
