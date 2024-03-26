using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using IO.Packets;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string DEFAULT_MESSAGE = "Enter Username";
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageToServerCommand { get; set; }
        public string Username { get; set; } = DEFAULT_MESSAGE;
        public string Message { get; set; } = string.Empty;
        public string ServerIp { get; set; } = "192.168.1.131";
        public Guid? UserId { get; set; }
        private Server _server;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {

            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.connectedEvent += this.ClientConnected;
            _server.recievedBroadcastEvent += this.BroadcastRecieved;
            _server.recievedMessageEvent += this.MessageRecieved;
            _server.recievedDisconnectEvent += this.DisconnectRecieved;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username, ServerIp), o => CanConnect());
            SendMessageToServerCommand = new RelayCommand(o => SendMessage(), o => !string.IsNullOrEmpty(Message) && this._server.isConnected);
        }

        private void ClientConnected()
        {
            if ((this._server?._reader) == null)
            {
                Console.WriteLine("No server reader to read so why are we here?");
            }
            else
            {
                var identity = this._server._reader.ReadPacket<IdentifierPacket>();
                if (identity == null || !identity.ValidIdentity())
                {
                    Console.WriteLine("We didn't get a valid identity back");
                    return;
                }
                var user = new UserModel()
                {
                    Username = identity.Username,
                    UID = identity.UID
                };
                this.UserId = identity.UID!.Value;
                this._server.ServerUid = identity.UID!.Value;
                if (!Users.Any(x => x.UID == user.UID))
                {
                    Application.Current.Dispatcher.Invoke(() => Users.Add(user));

                }
            }


        }

        private void BroadcastRecieved()
        {

            if ((this._server?._reader) == null)
            {
                Console.WriteLine("No server reader to read so why are we here?");
                return;
            }
            var broadcast = this._server._reader.ReadPacket<BroadcastPacket>();
            if (broadcast == null || broadcast?.Users == null)
            {
                Console.WriteLine("Failled to parse broadcast packet");
                return;
            }
            List<UserModel> users = broadcast.Users.Select(u => new UserModel()
            {
                Username = u.Key,
                UID = u.Value
            }).ToList();

            Application.Current.Dispatcher.Invoke(() => Users.Clear());
            foreach (var user in users)
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }

        }

        private void MessageRecieved()
        {
            if ((this._server?._reader) == null)
            {
                Console.WriteLine("No server reader to read so why are we here?");
                return;
            }
            var messagePacket = this._server._reader.ReadPacket<MessagePacket>();
            if (messagePacket?.Message == null)
            {
                Console.WriteLine("Failled to parse message packet");
                return;
            }
            var message = messagePacket.Message;
            if (message == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));


        }

        private void DisconnectRecieved()
        {
            if ((this._server?._reader) == null)
            {
                Console.WriteLine("No server reader to read so why are we here?");
                return;
            }
            var disconnectPacket = this._server._reader.ReadPacket<DisconnectPacket>();
            if (disconnectPacket == null)
            {
                Console.WriteLine("Failled to parse disconnect packet");
                return;
            }
            var user = Users.Where(x => x.UID == disconnectPacket.UID).FirstOrDefault();
            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
            }

        }

        private void SendMessage()
        {
            _server.SendMessageToServer(new(this.UserId!.Value, this.Message));
            this.Message = string.Empty;
            OnPropertyChanged(nameof(Message));
        }
        private bool CanConnect()
        {
            return !string.IsNullOrEmpty(Username) && !this._server.isConnected && this.Username != DEFAULT_MESSAGE;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

