using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Server
{
    class Client
    {
        public static int count = 0;
        public TcpClient client;
        private int ID;
        private int limit = 0;
        private static int capacity = 0;
        private int capacityUsing = 0;
        private bool seted = false;
        private static int statusChanged = 0;
        private int meChanged = 0;
        public Client(TcpClient tcpClient)
        {
            client = tcpClient;
            ID = ++count;
        }
        public Client(TcpClient tcpClient, int limit)
        {
            client = tcpClient;
            ID = ++count;
            this.limit = limit;
            capacity = limit;
            meChanged = statusChanged;
        }

        /// <summary>
        /// Установка скорости для клиента
        /// </summary>
        /// <param name="smth"></param>
        /// <returns></returns>
        private int SetSpeed(int bytes)
        {
            int speed = 0;            
            if (bytes <= capacity)
            {
                capacity -= bytes;
                speed = bytes;

            }
            else
            {
                speed = limit / count;
                if (statusChanged == meChanged)
                    statusChanged++;

                meChanged++;
                capacity += (capacityUsing - speed);
            }

            capacityUsing = speed;
            return speed;
        }
        
        public void Process()
        {           
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[256]; // буфер для получаемых данных
                int offset = 0;   // отступ
                int size = data.Length;
                while (client.Connected)
                {
                    // получаем сообщение
                    
                    StringBuilder incomeMessage = new StringBuilder();
                    int bytes = 0;                   
                   
                    do
                    {                                 
                        bytes = stream.Read(data, offset, size);
                        if (!seted)
                        {
                            if (limit != 0)
                                size = SetSpeed(bytes);                            
                            seted = true;

                        }  
                        if (statusChanged != meChanged)
                        {
                            size = SetSpeed(bytes);
                        }
                        string fragment = Encoding.UTF8.GetString(data, offset, bytes);
                        incomeMessage.Append(fragment);
                        //offset += limit;
                        Console.WriteLine($"Обработаны символы: {fragment} (байт в секунду от клиента {ID}:  {bytes} b/s)");
                        Thread.Sleep(1000);
                    }
                    while (stream.DataAvailable);
                    
                    string message = incomeMessage.ToString();
                    offset = 0;
                    if (message == "")
                        break;
                    Console.WriteLine($"Сообщение от клиента {ID}: {message}");
                    
                    string answer = "Данные получены";
                    data = Encoding.UTF8.GetBytes(answer);
                    stream.Write(data, 0, data.Length);
                    data = new byte[256];
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
                count--;
                capacity += capacityUsing;
                statusChanged++;
            }
        }
    }
}
