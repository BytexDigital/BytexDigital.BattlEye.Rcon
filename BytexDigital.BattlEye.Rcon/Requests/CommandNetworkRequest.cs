using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Requests {
    public class CommandNetworkRequest : SequentialNetworkRequest {
        public string Payload { get; private set; }
        public Command Command { get; private set; }

        public CommandNetworkRequest(string payload) {
            Payload = payload;
        }

        public CommandNetworkRequest(Command command) {
            Payload = command.Content;
            Command = command;
        }

        internal override byte[] GetPayloadBytes() {
            var bytes = new List<byte>();

            bytes.Add(0x01);
            bytes.Add(SequenceNumber.Value);
            bytes.AddRange(Encoding.ASCII.GetBytes(Payload));

            return bytes.ToArray();
        }

        protected override void ProcessResponse(NetworkResponse networkResponse) {
            var response = networkResponse as CommandNetworkResponse;
            
            if (Command != null && Command is IHandlesResponse responseHandler) {
                responseHandler.Handle(response.Content);
            }
        }
    }
}
