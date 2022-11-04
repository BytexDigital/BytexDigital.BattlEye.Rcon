using System;
using System.Collections.Generic;
using System.Linq;
using BytexDigital.BattlEye.Rcon.HashAlgorithms;

namespace BytexDigital.BattlEye.Rcon
{
    public abstract class NetworkMessage
    {
        public bool Sent { get; private set; }
        public DateTime? SentTimeUtc { get; private set; }

        internal abstract byte[] GetPayloadBytes();

        internal byte[] ToBytes()
        {
            var crc32 = new Crc32();

            var payload = GetPayloadBytes();
            var bytes = new List<byte>();
            var checksumContent = new List<byte> { 0xFF };

            checksumContent.AddRange(payload);

            var checksum = crc32.ComputeHash(checksumContent.ToArray());

            bytes.Add((byte) 'B');
            bytes.Add((byte) 'E');
            bytes.AddRange(checksum.Reverse());
            bytes.Add(0xFF);
            bytes.AddRange(payload);

            return bytes.ToArray();
        }

        internal void MarkSent()
        {
            Sent = true;
            SentTimeUtc = DateTime.UtcNow;
        }
    }
}