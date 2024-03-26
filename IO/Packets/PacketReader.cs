using IO.Interfaces;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace IO.Packets
{
    public class PacketReader : BinaryReader
    {


        public PacketReader(NetworkStream input) : base(input, Encoding.ASCII, true)
        {
        }

        public byte ReadOpCode()
        {
            return ReadByte();

        }
        public T? ReadPacket<T>() where T : class, Packet?
        {
            byte[] msgBuffer;
            int length = this.ReadInt32();
            msgBuffer = new byte[length];
            if (this.Read(msgBuffer, 0, length) != length)
            {
                Console.WriteLine("Mismatched number of bytes read");
                return null;
            }
            else
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(msgBuffer, options: new()
                    {
                        PropertyNameCaseInsensitive = true,
                        IncludeFields = true,
                    });

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

            }

        }

    }
}
