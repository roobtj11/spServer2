﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

public class vBallTV
{
    public delegate void MyEventHandler(object source, EventArgs e);

    public static Timer aTimer;

    static Dictionary<string, Users> accounts = new Dictionary<string, Users>();
    private const string k_GlobalIp = "127.0.0.1";
    private const string k_LocalIp = "127.0.0.1";
    private const int k_Port = 7777;
    public static void  Main(string[] args)
    {
        LoadUsers();
        var ipAddress = IPAddress.Parse(k_LocalIp);
        var localEp = new IPEndPoint(ipAddress, k_Port);
        using var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEp);
        listener.Listen();
        Console.WriteLine("Waiting...");
        for (; ; )
        {
            try
            {
                var handler = listener.Accept();
                var thread = new Thread(new ThreadStart(() => ClientHandler(handler)));
                thread.Start();
            }
            catch
            {
                Console.WriteLine("user Forcefully Disconnected 1");
            }
        }
        //listen forever for connections

    }

    public static void ClientHandler(Socket handler)
    {
        try
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 2000;
            aTimer.Elapsed += (sender, e) => OnTimedEvent(sender,e,handler);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            Console.WriteLine("{0} connected", handler.RemoteEndPoint);
            string[] wResponse = WelcomeMenu(handler);
            if(wResponse[0] == "True")
            {
                SignedIn(wResponse[1], int.Parse(wResponse[2]));
            }
            else
            {
                disconnect(handler);
                return;
            }
        }
        catch
        {
            disconnect(handler);
            return;
        }
        
    }

    public static string[] WelcomeMenu(Socket handler)
    {
        try
        {
            for (; ; )
            {
                string[] response = { };
                string loginORcreate = Network.recievemessage(handler);
                if (loginORcreate == "Quit")
                    throw new Exception("a");
                else if (loginORcreate == "C")
                {
                    Console.WriteLine("{0} is trying to create a new user", handler.RemoteEndPoint);
                    string uname = CreateNewUser(handler);
                    if (uname == "back")
                    {
                        
                    }
                    else
                    {
                        response[0] = "True";
                        response[1] = uname;
                        response[2] = "1";
                        return response;
                    }
                }
                else if (loginORcreate == "L")
                {
                    Console.WriteLine("hit");
                    string[] sResponse = SignIn(handler);
                    if (sResponse[0] == "back")
                    {
                        
                    }
                    else if (sResponse[0] == "True")
                    {
                        return sResponse;
                    }
                    else
                    {
                        response[0] = "false";
                        return response;
                    }
                }
            }


        }
        catch
        {
            string[] response = { "false" };
            return response;
        }
    }

    private static string CreateNewUser(Socket handler)
    {
        string username = Network.recievemessage(handler);
        if (username == "back")
        {
            Console.WriteLine("UserBackedOut");
            return "back";
        }
        for (; ; )
        {
            if (accounts.ContainsKey(username))
            {
                Network.sendmessage(handler, "E:1\r");
            }
            else
            {
                Network.sendmessage(handler, "null");
                accounts.Add(username, new Users(username, Network.recievemessage(handler)));
                Console.WriteLine("New User: '" + username + "' has been created. And is now signed in as a standard user.");
                ExportUsers();
                return username;
            }
        }
    }
    private static string[] SignIn(Socket handler)
    {
        string[] response = { };
        for (; ; )
        {
            
            string line = Network.recievemessage(handler);
            
            if (line == "back")
            {
                Console.WriteLine(line);
                string[] response2 = { line};
                return response2;
            }
            string[] parts = line.Split(',');
            string username = parts[0];
            string password = parts[1];
            
            
            if (accounts.ContainsKey(username) && accounts.GetValueOrDefault(username).getPasswordHash() == password)
            {
                Console.WriteLine("{0} has signed in as {1}, as a {2} account.", handler.RemoteEndPoint, username, accounts.GetValueOrDefault(username).printAccountName());
                string approved = "Approved," + accounts.GetValueOrDefault(username).printAccountPerm();
                Network.sendmessage(handler, approved);
                Network.sendmessage(handler, "1%hello");

                response[0] = "True";
                response[1] = username;
                response[2] = accounts.GetValueOrDefault(username).printAccountPerm().ToString();
                return response;
            }
            else
            {
                Network.sendmessage(handler, "DNE");
            }
        }

    }

    public static void SignedIn(string username, int accountLV)
    {

    }


    public static void disconnect(Socket socket)
    {
        Console.WriteLine("User Disconnected");
        socket.Shutdown(SocketShutdown.Both);
        
    }

    private static void LoadUsers()
    {
        using (var reader = new StreamReader("users.csv"))
        {
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] parts = line.Split(',');
                accounts.Add(parts[0], new Users(parts[0], parts[1], parts[2]));
                //Console.WriteLine(login.GetValueOrDefault(parts[0]).print());
            }
        }
    }
    static void ExportUsers()
    {
        Console.WriteLine("saving log");
        using (var adder = new StreamWriter("users.csv"))
        {
            adder.WriteLine("Username,Password,Account Type");
            var keys = accounts.Keys.ToList();
            foreach (var key in keys)
            {
                adder.WriteLine(key.ToString() + "," + accounts[key].print());
            }
        }
    }
    private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e, Socket handler)
    {
        Console.WriteLine("The Elapsed event was raised at {0} {1}", e.SignalTime, handler.RemoteEndPoint);

        disconnect(handler);
        return;
    }
}