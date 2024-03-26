using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO.Interfaces
{
    public interface Packet
    {
        [JsonIgnore]
        byte OpCode { get; }

        public string? ToJsonMessage();

    }
}
