using IO.Interfaces;
using IO.Packets.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO.Packets
{
    public class BroadcastPacket : Packet
    {
        [JsonIgnore]
        public byte OpCode => OpCodeConstants.BroadcastPacketOpCode;

        public Dictionary<string, Guid>? Users;

        public BroadcastPacket(Dictionary<string, Guid>? users)
        {
            this.Users = users;
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
