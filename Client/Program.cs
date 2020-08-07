using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        private const int port = 8888;
        private const string ServerIP = "127.0.0.1";
        private static Random random = new Random();

        static void Main(string[] args)
        {


            Console.WriteLine("Ограничить скорость передачи? {0 - если нет / другое число, если да}");
            int limitation = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Начать передачу? {y/n}");
            while (Console.ReadLine() != "y" )
            { 

            }

            TcpClient client = new TcpClient();

            NetworkStream stream;

            try
            {
               
                client.Connect(ServerIP, port);

                stream = client.GetStream();                

                string message = "";
                int size = 0;
                while(true)
                {
                    //сообщение
                    message = RandomString(random.Next(60, 100));
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    
                    if (limitation != 0)
                    {
                        size = (data.Length < limitation) ? data.Length : limitation;
                        stream.Write(data, 0, size);
                    }
                                                                                      
                    else                    
                        stream.Write(data, 0, data.Length);                     
                    
                    //ответ от сервера
                    StringBuilder response = new StringBuilder();
                    data = new byte[32];
                    int dataSize = 0;
                    do
                    {
                        dataSize = stream.Read(data, 0, data.Length);
                        response.Append(Encoding.UTF8.GetString(data, 0, dataSize));
                    }
                    while (stream.DataAvailable); // пока данные есть в потоке

                    Console.WriteLine($"Ответ от сервера: {response}");

                    
                }
                                

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {               
                client.Close();
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    
}
