using System.Net;

namespace Venusos_Botnet_Server
{
    public class Command
    {
        public string Method { get; set; }
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public int Duration { get; set; }
        public int CpuUsage { get; set; }
    }
}
