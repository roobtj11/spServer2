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
        var buffer = new byte[1024];
        var numBytesReceived = socket.Receive(buffer);
        var textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived);
        return textReceived;
    }

    public static void sendmessage(Socket socket, string message)
    {
        var bytes = Encoding.ASCII.GetBytes(message + "\r");
        socket.Send(bytes);
    }

}
