using IO.Interfaces;
using IO.Packets.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO.Packets
{
    [method: JsonConstructor]
    public class IdentifierPacket(string username, Guid? uID = null) : Packet
    {
        [JsonIgnore]
        public byte OpCode => OpCodeConstants.IdentifierPacketOpCode;

        public string Username = username;
        public Guid? UID = uID;
        public Guid? ServerUid;

        public string? ToJsonMessage()
        {
            return JsonSerializer.Serialize(this, options: new()
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true,

            });
        }

        public bool ValidIdentity()
        {
            return this.Username != null && this.UID.HasValue && this.ServerUid.HasValue;
        }
    }
}


