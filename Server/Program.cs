using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        const int port = 8888;
        const string localIP = "127.0.0.1";
        static TcpListener listener;
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Ограничить скорость передачи? {0 - если нет / другое число, если да}");
                int limitation = Convert.ToInt32(Console.ReadLine());
                listener = new TcpListener(IPAddress.Parse(localIP), port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    //Пока есть запросы к серверу
                    if (listener.Pending())
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Client clientObject = new Client(client, limitation);                                        
                                                                                                  
                        // Отдельный поток для каждого клиента
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
