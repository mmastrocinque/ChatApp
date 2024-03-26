using IO.Packets;
using IO.Packets.Constants;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Client
    {


        public string? Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        private PacketReader _packetReader;
        private PacketBuilder _packetBuilder;
        public Action<DisconnectPacket> UserDisconnectedAction { get; set; }
        public Action ConnectionSuccessfulAction { get; set; }
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new(ClientSocket.GetStream());
            Task.Run(() => Process());

        }
        private void Close()
        {
            ClientSocket.Close();

            Console.WriteLine("Connection Closed");
        }

        public void Send(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            ClientSocket.Client.Send(bytes);


        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opCode = _packetReader.ReadOpCode();
                    switch (opCode)
                    {
                        case OpCodeConstants.IdentifierPacketOpCode:
                            ProcessIdentity();
                            break;
                        case OpCodeConstants.MessagePacketOpCode:
                            ProcessMessage();
                            break;
                        case OpCodeConstants.DisconnectPacketOpCode:
                            ProcessDisconnect();
                            break;
                        default:
                            Close();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ProcessDisconnect();
                    break;

                }
            }
        }



        public void ProcessIdentity()
        {
            IdentifierPacket? identifierPacket = _packetReader.ReadPacket<IdentifierPacket>();
            if (identifierPacket?.Username == null)
            {
                Console.WriteLine("This is awkward");
                ArgumentNullException.ThrowIfNull(identifierPacket?.Username);
                return;
            }
            this.Username = identifierPacket.Username;
            Console.WriteLine($"[{DateTimeOffset.Now}]: Client {UID} has connected with username: {this.Username}");
            identifierPacket.UID = this.UID;
            identifierPacket.ServerUid = Program.ID;
            PacketBuilder identitiypacket = new();
            identitiypacket.WritePacket(identifierPacket);
            Send(identitiypacket.GetPacketBytes());
            this.ConnectionSuccessfulAction.Invoke();

        }


        private void ProcessMessage()
        {
            MessagePacket? messagePacket = _packetReader.ReadPacket<MessagePacket>();
            if (messagePacket == null)
            {
                Console.WriteLine("Invalid message packet: This is awkward");
                return;
            }
            Console.WriteLine($"[{DateTimeOffset.Now}]: Message Recieved {messagePacket.Message}");
            Program.BroadcastMessage(messagePacket);
        }

        private void ProcessDisconnect()
        {
            Console.WriteLine($"[{DateTimeOffset.Now}]: {Username}: Client {Username}:{UID} has disconnected");
            this.UserDisconnectedAction.Invoke(new(this.UID));
            this.ClientSocket.Close();
        }
    }
}
