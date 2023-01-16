using System.Net;
using System.Net.Sockets;
class netcode
{
    public static bool connected = false;

    static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }

    static string localIP = GetLocalIPAddress();
    public static TcpListener listener;
    public static TcpClient client = null;
    static NetworkStream nwStream = null;

    public static async Task Host()
    {
        Console.WriteLine(localIP);
        // IPAddress localAdd = System.Net.IPAddress.Parse(localIP);
        //start Listening for client
        listener = new System.Net.Sockets.TcpListener(IPAddress.Any, 5000);
        Console.WriteLine("waiting for client");
        listener.Start();
        client = await listener.AcceptTcpClientAsync();     //replace client with a list object to be able to receive multiple clients instead of overwriting
        nwStream = client.GetStream();
        connected = true;
    }
    public static async Task Connect()
    {
        const int PORT_NO = 5000;
        Console.WriteLine("Enter server IP");
        // string SERVER_IP = Console.ReadLine();
        Thread read = new Thread(() =>
        {
            string SERVER_IP = Console.ReadLine();
            client = new TcpClient(SERVER_IP, PORT_NO);
            nwStream = client.GetStream();
            connected = true;
        });
        read.Start();
    }


    public static async Task<System.Numerics.Vector2> Receive()
    {

        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---read incoming stream---
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        //---convert the data received into a string---
        string dataReceived = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
        var data = dataReceived.Split(",");
        try
        {
            int x = Int32.Parse(data[0]);
            int y = Int32.Parse(data[1]);
            return (new System.Numerics.Vector2(x, y));
        }
        catch { return (System.Numerics.Vector2.Zero); };
    }
    public static async Task Send(int x, int y)
    {
        //---write back the text to the client---
        string textToSend = $"{x},{y}";
        byte[] bytesToSend = System.Text.ASCIIEncoding.ASCII.GetBytes(textToSend);
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
    }

    // client.Close();
    // listener.Stop();
}