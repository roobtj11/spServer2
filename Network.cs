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

        if (textReceived.StartsWith("INCOMPLETE"))
        {
            string combinedtext = null;
            while (textReceived.StartsWith("INCOMPLETE"))
            {
                combinedtext = combinedtext + textReceived.Substring(10);
                textReceived = incomplete_reciever(socket);
            }
            textReceived = combinedtext + textReceived;
        }

        string[] a = textReceived.Split("\r");
        return a[0];
    }

    public static string incomplete_reciever(Socket socket)
    {
        var buffer = new byte[2048];
        var numBytesReceived = socket.Receive(buffer);
        var textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived);
        return textReceived;

    }

    public static void sendmessage(Socket socket, string message)
    {
        message = message + "\r";
        string WholeMessage = message;
        

        var bytes = Encoding.ASCII.GetBytes(message);
        List<byte[]?> packets = new List<byte[]?>();
        if(bytes.Length > 2000)
        {
            
            while (bytes.Length > 2000)
            {
                message = "INCOMPLETE" + message;
                string newmessage = message.Substring(0, message.Length / 2);
                message = message.Substring(newmessage.Length);
                bytes = Encoding.ASCII.GetBytes(newmessage);
                packets.Add(bytes);
            }
            packets.Add(Encoding.ASCII.GetBytes(message));
            foreach(var packet in packets)
            {
                socket.Send(packet);
            }
        }
        else
        {
            socket.Send(bytes);
        }
    }

}
