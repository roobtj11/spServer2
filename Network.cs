using System.Net;
using System.Net.Sockets;
using System.Text;

public static class Network
{
    private const string k_GlobalIp = "127.0.0.1";
    private const string k_LocalIp = "127.0.0.1";
    private const int k_Port = 7777;
    //public static Socket Start(){
        
        
     //   return listener;
    //}

    public static string recievemessage(Socket socket)
    {
        var buffer = new byte[2048];
        var numBytesReceived = socket.Receive(buffer);
        var textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived);
        string[] a = textReceived.Split("\r");
        return a[0];
    }

    public static void sendmessage(Socket socket, string message)
    {
        message = message + "\r";
        //Console.WriteLine(message.Length.ToString());

        var bytes = Encoding.ASCII.GetBytes(message);
        socket.Send(bytes);
    }

}
