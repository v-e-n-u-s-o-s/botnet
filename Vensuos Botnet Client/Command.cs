namespace Vensuos_Botnet_Client
{
    public class Command
    {
        public string Method { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int Duration { get; set; }
        public int Threads { get; set; }
    }
}
