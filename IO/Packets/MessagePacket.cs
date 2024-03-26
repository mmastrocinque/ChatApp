using IO.Interfaces;
using IO.Packets.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO.Packets
{
    public class MessagePacket : Packet
    {

        [JsonIgnore]
        public byte OpCode => OpCodeConstants.MessagePacketOpCode;

        public Guid SenderUid;
        public string Message;

        public MessagePacket(Guid senderUid, string message)
        {
            this.SenderUid = senderUid;
            this.Message = message;
        }

        public string? ToJsonMessage()
        {
            return JsonSerializer.Serialize(this, options: new()
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true,

            });

        }

    }
}
