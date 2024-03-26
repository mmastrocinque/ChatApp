using IO.Interfaces;
using System.Text;
using System.Text.Json;

namespace IO.Packets
{
    public class PacketBuilder
    {
        readonly MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opCode)
        {
            _ms.WriteByte(opCode);
        }


        public void WriteMessage(object msg)
        {
            var encodedMsg = JsonSerializer.Serialize(msg, options: new()
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true,
            });
            var msgLength = encodedMsg.Length;
            _ms.Write(BitConverter.GetBytes(msgLength));
            _ms.Write(Encoding.UTF8.GetBytes(encodedMsg));
        }

        public void WritePacket(Packet packet)
        {
            _ms.WriteByte(packet.OpCode);
            string jsonMsg = packet.ToJsonMessage() ?? string.Empty;


            var encodedMsg = Encoding.UTF8.GetBytes(jsonMsg);
            var msgLength = encodedMsg.Length;
            if (msgLength <= 0) { Console.WriteLine("Invalid message length"); return; }
            _ms.Write(BitConverter.GetBytes(msgLength));
            _ms.Write(Encoding.UTF8.GetBytes(jsonMsg));

        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }


    }
}
