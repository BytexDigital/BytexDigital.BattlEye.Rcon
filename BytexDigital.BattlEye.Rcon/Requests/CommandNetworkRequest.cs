using System.Collections.Generic;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Responses;

namespace BytexDigital.BattlEye.Rcon.Requests
{
    public class CommandNetworkRequest : SequentialNetworkRequest
    {
        private readonly StringEncoder _stringEncoder = new StringEncoder();
        public string Payload { get; }
        public Command Command { get; }

        public CommandNetworkRequest(string payload)
        {
            Payload = payload;
        }

        public CommandNetworkRequest(Command command)
        {
            Payload = command.Content;
            Command = command;
        }

        internal override byte[] GetPayloadBytes()
        {
            var bytes = new List<byte>();

            bytes.Add(0x01);
            bytes.Add(SequenceNumber.Value);
            bytes.AddRange(_stringEncoder.GetBytes(Payload));

            return bytes.ToArray();
        }

        protected override void ProcessResponse(NetworkResponse networkResponse)
        {
            var response = networkResponse as CommandNetworkResponse;

            if (Command != null && Command is IHandlesResponse responseHandler)
                responseHandler.Handle(response.Content);
        }
    }
}