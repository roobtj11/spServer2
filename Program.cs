using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Timers;


public class vBallTV
{

    static Dictionary<string, Users> accounts = new Dictionary<string, Users>();
    static Dictionary<int, Games> GameDict = new Dictionary<int, Games>();
    //static List<Games> GameList = new List<Games> ();
    static int gamecount = 0;
    private const string k_GlobalIp = "127.0.0.1";
    private const string k_LocalIp = "127.0.0.1";
    private const int k_Port = 7777;
    static List<Socket> sockets = new List<Socket>();
    public static bool serveropen=true;
    public static void  Main(string[] args)
    {
        LoadUsers();
        LoadGames();
        var ipAddress = IPAddress.Parse(k_LocalIp);
        var localEp = new IPEndPoint(ipAddress, k_Port);
        using var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEp);
        listener.Listen();
        Console.WriteLine("Waiting...");


        Task.Run(() => open());
        while (serveropen)
        {
            try
            {
                
                var handler = listener.Accept();
                sockets.Add(handler);
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
    private static void open()
    {
        for(; ; )
        {
            if (!serveropen)
            {
                System.Environment.Exit(18);
                break;
            }
        }
    }
    public static void ClientHandler(Socket handler)
    {
        try
        {
            Console.WriteLine("{0} connected", handler.RemoteEndPoint);
            string[] wResponse = WelcomeMenu(handler);
            if(wResponse[0] == "True")
            {
                SignedIn(handler, wResponse[1], int.Parse(wResponse[2]));
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

        for (; ; )
        {
            string[] response = new string[3];
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

    private static string CreateNewUser(Socket handler)
    {
        for (; ; )
        {
            string username = Network.recievemessage(handler);
            if (username == "quit")
               {
                Console.WriteLine("UserBackedOut");
                return "back";
            }
        
            if (accounts.ContainsKey(username))
            {
                Network.sendmessage(handler, "E:1");
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
        string[] response = new string[3];
        for (; ; )
        {

            string line = Network.recievemessage(handler);

            if (line == "back")
            {
                Console.WriteLine(line);
                string[] response2 = { line };
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

    public static void SignedIn(Socket handler, string username, int accountLV)
    {
        for (; ; )
        {
            SendGames(handler);
            string message = Network.recievemessage(handler);

            if (message == "OpenGame")
            {
                message = Network.recievemessage(handler);
                int game = int.Parse(message);
                //SendGameUpdate(handler, game);
                OpenGame(handler, game, accountLV);
            }
            else if (message == "NewGame")
            {
                for (; ; )
                {
                    message = Network.recievemessage(handler);
                    if (message == "Back")
                    {
                        break;
                    }
                    else if (message == "SaveGames")
                    {
                        ExportGames(handler);
                    }
                    else if (message == "Close Server")
                    {
                        if (Network.recievemessage(handler) == accounts[username].getPasswordHash() && accountLV == 3)
                        {
                            Network.sendmessage(handler, "Begining to Close the Server");
                            ExportUsers();
                            ExportGames(handler);
                            foreach (Socket Socket in sockets)
                            {
                                Network.sendmessage(Socket,"Server Closed");
                                disconnect(Socket);
                            }
                            serveropen = false;
                            throw new Exception("Close Server");
                        }
                        else
                        {
                            Network.sendmessage(handler, "Failed");
                        }
                        
                    }
                    else
                    {
                        string[] teams = message.Split(',');
                        GameDict.Add(gamecount, new Games(gamecount, teams[0], teams[1]));
                        message = "Game created under ID: " + gamecount;
                        Network.sendmessage(handler, message);
                        SendGameUpdate(handler, gamecount);
                        gamecount++;
                    }
                }
            }
        }
    }

    public static void SendGameUpdate(Socket handler, int GameID)
    {
        foreach(var Socket in sockets)
        {
            string game = "GAMEUPDATE%%%" + JsonSerializer.Serialize(GameDict[GameID]);
            Network.sendmessage(Socket, game);
        }
        
    }
    public static void OpenGame(Socket handler, int GameID, int accountLV)
    {
        for(; ; )
        {
            string input = Network.recievemessage(handler);
            if (input == "Back")
            {
                return;
            }
            else if(input == "GameUpdate")
            {
                RecieveGameUpdate(handler, GameID, accountLV);
            }
        }
    }
    static void RecieveGameUpdate(Socket handler, int GameID, int accountLV)
    {
        string gameupdate = Network.recievemessage(handler);
        Games a = JsonSerializer.Deserialize<Games>(gameupdate);
        if (accountLV < 2 && a.print_WO_Msg() != GameDict[GameID].print_WO_Msg())
        {
            Network.sendmessage(handler, "ERROR TERMINATING CONNECTION");
            disconnect(handler);
        }
        else
        {
            GameDict[GameID] = a;
            //Console.WriteLine(GameDict[GameID].t1Scores[GameDict[GameID].CurrentSet-1].ToString());
            SendGameUpdate(handler, GameID);
            if (a.GameOver)
            {
                ExportGames(handler);
            }
        }
    }
    
    public static void SendGames(Socket handler)
    {
        Stack<string> games = new Stack<string>();
        var keys = GameDict.Keys.ToList();
        foreach (var key in keys)
        {
            Games a = GameDict[key];
            string jsonString = JsonSerializer.Serialize(a);
            //Games b = JsonSerializer.Deserialize<Games>(jsonString);
            games.Push(jsonString);
            //Console.WriteLine(b.print());
        }
        string stackString = JsonSerializer.Serialize<Stack<string>>(games);
        Network.sendmessage(handler, stackString);
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
    private static void LoadGames()
    {
        using (var reader = new StreamReader("games.csv"))
        {
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                
                //GameList.Add(new Games(count, parts[1], parts[2], bool.Parse(parts[3]), parts[4], int.Parse(parts[5]), int.Parse(parts[6]), int.Parse(parts[7]), int.Parse(parts[8]), int.Parse(parts[9]), int.Parse(parts[10]), int.Parse(parts[11]), int.Parse(parts[12]), int.Parse(parts[13])));
                
                GameDict.Add(gamecount, JsonSerializer.Deserialize<Games>(line));
                Console.WriteLine(GameDict[gamecount].print());
                
                //Console.WriteLine( GameList[gamecount].print());
                gamecount++;
                //Console.WriteLine(login.GetValueOrDefault(parts[0]).print());
            }
        }
    }
    static void ExportGames(Socket Handler)
    {
        Console.WriteLine("saving log");
        using (var adder = new StreamWriter("games.csv"))
        {
            adder.WriteLine("GameNumber,Team1,Team2,GameOver,Winner,CurrentSet,T1S1Score,T1S2Score,T1S3Score,T2S1Score,T2S2Score,T2S3Score,T1SetsWone,T2SetsWon,Messages");
            var keys = GameDict.Keys.ToList();
            foreach(var key in keys)
            {
                adder.WriteLine(JsonSerializer.Serialize(GameDict[key]));
                Console.WriteLine(JsonSerializer.Serialize(GameDict[key]));
            }
        }
        Console.WriteLine("LOG SAVED");
        Network.sendmessage(Handler,"Games Saved");
    }

    private static void ExportUsers()
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
}