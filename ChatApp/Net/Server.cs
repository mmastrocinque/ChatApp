using IO.Packets;
using IO.Packets.Constants;
using System.Net.Sockets;
using System.Windows;

namespace ChatClient.Net
{
    public class Server
    {
        private TcpClient _client;
        public PacketReader? _reader;
        public Guid? ServerUid;
        public bool isConnected => _client.Connected;
        public Server()
        {
            _client = new TcpClient();

        }
        public event Action connectedEvent;
        public event Action recievedBroadcastEvent;
        public event Action recievedDisconnectEvent;
        public event Action recievedMessageEvent;




        public void ConnectToServer(string username, string serverAddress)
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect(serverAddress, 7891);
                    _reader = new PacketReader(_client.GetStream());
                    EstablishConnection(username);
                    ReadPackets();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failled to connect to server {serverAddress} with username {username}");
                }
               

            }
        }

        private void EstablishConnection(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var connectPacket = new PacketBuilder();
                IdentifierPacket identifierPacket = new(username);
                connectPacket.WritePacket(identifierPacket);
                _client.Client.Send(connectPacket.GetPacketBytes());
            }
        }
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true && _reader != null)
                {


                    var opcode = _reader.ReadOpCode();
                    switch (opcode)
                    {
                        case OpCodeConstants.IdentifierPacketOpCode:
                            connectedEvent.Invoke();
                            break;
                        case OpCodeConstants.MessagePacketOpCode:
                            recievedMessageEvent.Invoke();
                            break;
                        case OpCodeConstants.BroadcastPacketOpCode:
                            recievedBroadcastEvent.Invoke();
                            break;
                        case OpCodeConstants.DisconnectPacketOpCode:
                            recievedDisconnectEvent.Invoke();
                            break;
                        default:
                            Console.WriteLine("Not a valid Op code");
                            break;
                    }

                }
            });
        }


        public void SendMessageToServer(MessagePacket myMessage)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WritePacket(myMessage);
            this._client.Client.Send(messagePacket.GetPacketBytes());

        }
    }
}
