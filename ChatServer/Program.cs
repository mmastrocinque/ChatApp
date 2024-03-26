using ChatServer;
using IO.Packets;
using System.Net;
using System.Net.Sockets;

internal class Program
{
    static List<Client> _users = new List<Client>();
    static TcpListener? _listener;
    public static Guid ID = new Guid();
    private static void Main(string[] args)
    {
        _listener = new TcpListener(IPAddress.Parse("192.168.1.131"), 7891);
        _listener.Start();

        while (true)
        {
            var client = new Client(_listener.AcceptTcpClient());
            client.ConnectionSuccessfulAction += BroadcastConnection;
            client.UserDisconnectedAction += BroadcastDisconnect;
            _users.Add(client);
            _users = _users.Where(o=> o.ClientSocket.Connected).ToList();

            /* Broadcast the conenction to everyone on the server */


        }

    }
    static void BroadcastConnection()
    {
        var broadcastPacket = new PacketBuilder();
        Dictionary<string, Guid> usersDict = _users
            .Where(u => u.Username != null)
            .Where(c => c.ClientSocket.Connected)
            .ToDictionary(o => o.Username!, elementSelector: o => o.UID);
        if (usersDict == null || usersDict.Count <= 0)
        {
            Console.WriteLine("No users to write to?");
            return;
        }
        broadcastPacket.WritePacket(new BroadcastPacket(usersDict));
        foreach (var user in _users)
        {
            user.Send(broadcastPacket.GetPacketBytes());
        }
    }

    public static void BroadcastMessage(MessagePacket message)
    {
        if (message.SenderUid != ID)
        {
            var sourceClient = _users.FirstOrDefault(x => x.UID == message.SenderUid);

            if (sourceClient == null)
            {
                Console.WriteLine("Bad UID fir source client / client not found");
                return;
            }

            message.Message = $"[{DateTimeOffset.Now}]: {sourceClient.Username}: {message.Message}";


        }
        else
        {
            message.Message = $"[{DateTimeOffset.Now}]: {message.Message}";
        }

        foreach (var user in _users)
        {
            var packet = new PacketBuilder();
            packet.WritePacket(message);
            user.ClientSocket.Client.Send(packet.GetPacketBytes());
        }
    }

    public static void BroadcastDisconnect(DisconnectPacket disconnectPacket)
    {
        Client? disconnectedUser = _users.Where(x => x.UID == disconnectPacket.UID).FirstOrDefault();
        if (disconnectedUser == null) { return; }
        _users.Remove(disconnectedUser);
        foreach (var user in _users)
        {
            PacketBuilder packet = new PacketBuilder();
            packet.WritePacket(disconnectPacket);
            user.ClientSocket.Client.Send(packet.GetPacketBytes());
        }
        BroadcastMessage(new MessagePacket(ID, $"[{DateTimeOffset.Now}] {disconnectedUser.Username} has disconnected from Chat"));

    }

}