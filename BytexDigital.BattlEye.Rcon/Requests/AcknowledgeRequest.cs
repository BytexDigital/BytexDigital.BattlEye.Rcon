using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Requests
{
    public class AcknowledgeRequest : NetworkMessage
    {
        public byte SequenceNumber { get; private set; }

        public AcknowledgeRequest(byte sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
        }

        internal override byte[] GetPayloadBytes()
        {
            var bytes = new List<byte>();

            bytes.Add(0x02);
            bytes.Add(SequenceNumber);

            return bytes.ToArray();
        }
    }
}
