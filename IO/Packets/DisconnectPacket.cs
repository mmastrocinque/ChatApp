using IO.Interfaces;
using IO.Packets.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO.Packets
{
    public class DisconnectPacket(Guid uID) : Packet
    {
        [JsonIgnore]
        public byte OpCode => OpCodeConstants.DisconnectPacketOpCode;
        public Guid UID { get; set; } = uID;

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
