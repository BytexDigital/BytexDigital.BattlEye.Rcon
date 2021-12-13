using BytexDigital.BattlEye.Rcon.Requests;
using BytexDigital.BattlEye.Rcon.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BytexDigital.BattlEye.Rcon
{
    public class NetworkMessageHandler
    {
        private List<NetworkRequest> _networkRequests;
        private NetworkConnection _networkConnection;
        private readonly StringEncoder _stringEncoder;

        public NetworkMessageHandler(NetworkConnection networkConnection)
        {
            _networkRequests = new List<NetworkRequest>();
            _stringEncoder = new StringEncoder();
            _networkConnection = networkConnection;
        }

        public void Track(NetworkRequest networkRequest)
        {
            Cleanup();

            lock (_networkRequests)
            {
                _networkRequests.Add(networkRequest);
            }
        }

        public void Handle(IEnumerable<byte> data)
        {
            var payload = data.Skip(7);
            var type = payload.First();

            if (type == 0x00)
            {
                HandleLoginResponse(payload.Skip(1));
            }
            else if (type == 0x01)
            {
                HandleCommandResponse(payload.Skip(1));
            }
            else if (type == 0x02)
            {
                HandleMessagePacket(payload.Skip(1));
            }
        }

        public void Cleanup()
        {
            lock (_networkRequests)
            {
                var toRemoves = _networkRequests.Where(x => !x.Acknowledged && DateTime.UtcNow - x.SentTimeUtc > TimeSpan.FromSeconds(5)).ToArray();

                foreach (var toRemove in toRemoves)
                {
                    RemoveRequest(toRemove);
                }
            }
        }

        private void HandleLoginResponse(IEnumerable<byte> data)
        {
            bool success = data.First() == 0x01;

            lock (_networkRequests)
            {
                foreach (var loginRequest in _networkRequests.Where(x => x is LoginNetworkRequest).ToArray())
                {
                    RemoveRequest(loginRequest);

                    loginRequest.SetResponse(new LoginNetworkResponse(success));
                    loginRequest.MarkResponseReceived();
                }
            }
        }

        private void HandleMessagePacket(IEnumerable<byte> data)
        {
            byte sequenceNumber = data.First();
            var payload = data.Skip(1);
            string content = _stringEncoder.GetString(payload.ToArray());

            _networkConnection.Send(new AcknowledgeRequest(sequenceNumber));

            try { _networkConnection.FireMessageReceived(content); } catch { }

            const string playerConnectedPattern = @"Verified GUID \((\S{32})\) of player #(\d+) (.+)";
            const string playerDisconnectedPattern = @"Player #(\d+) (.+) disconnected";
            const string playerRemovedPattern = @"Player #(\d+) (.+) \((\S{32})\) has been kicked by BattlEye: Admin (Kick|Ban)(?: \((.+)\))?";

            if (Regex.IsMatch(content, playerConnectedPattern))
            {
                foreach (Match match in Regex.Matches(content, playerConnectedPattern))
                {
                    string guid = match.Groups[1].Value;
                    int id = Convert.ToInt32(match.Groups[2].Value);
                    string name = match.Groups[3].Value;

                    try { _networkConnection.FireProtocolEvent(new Events.GenericParsedEventArgs(new Events.PlayerConnectedArgs(id, guid, name))); } catch { }
                }
            }

            if (Regex.IsMatch(content, playerDisconnectedPattern))
            {
                foreach (Match match in Regex.Matches(content, playerDisconnectedPattern))
                {
                    int id = Convert.ToInt32(match.Groups[1].Value);
                    string name = match.Groups[2].Value;

                    try { _networkConnection.FireProtocolEvent(new Events.GenericParsedEventArgs(new Events.PlayerDisconnectedArgs(id, name))); } catch { }
                }
            }

            if (Regex.IsMatch(content, playerRemovedPattern))
            {
                foreach (Match match in Regex.Matches(content, playerRemovedPattern))
                {
                    int id = Convert.ToInt32(match.Groups[1].Value);
                    string name = match.Groups[2].Value;
                    string guid = match.Groups[3].Value;
                    bool isBan = match.Groups[4].Value == "Ban";
                    string reason = match.Groups.Count > 5 ? match.Groups[5].Value : null;

                    try { _networkConnection.FireProtocolEvent(new Events.GenericParsedEventArgs(new Events.PlayerRemovedArgs(id, guid, name, isBan, reason))); } catch { }
                }
            }
        }

        private void HandleCommandResponse(IEnumerable<byte> data)
        {
            var sequenceNumber = data.First();
            var request = GetRequest(sequenceNumber);

            if (request == null)
            {
                return;
            }

            var remainingBytes = data.Skip(1);

            if (remainingBytes.Count() > 0)
            {
                var header = remainingBytes.First();

                if (header == 0x00)
                { // Multi-part message
                    if (request.Response == null)
                    {
                        request.SetResponse(new CommandNetworkResponse(""));
                    }

                    var expectedAmount = data.Skip(2).First();
                    var currentIndex = data.Skip(3).First();
                    var partData = data.Skip(4);

                    string result = _stringEncoder.GetString(partData.ToArray());
                    (request.Response as CommandNetworkResponse).AppendContent(result);

                    if (expectedAmount == currentIndex + 1)
                    { // End of multi-part message
                        RemoveRequest(request);
                        request.MarkResponseReceived();
                    }
                }
                else
                {
                    RemoveRequest(request);

                    string result = _stringEncoder.GetString(data.Skip(1).ToArray());

                    request.SetResponse(new CommandNetworkResponse(result));
                    request.MarkResponseReceived();
                }
            }

            if (!request.Acknowledged)
            {
                request.MarkAcknowledged();
            }
        }

        private SequentialNetworkRequest GetRequest(byte sequenceNumber)
        {
            lock (_networkRequests)
            {
                return _networkRequests.Where(x => x is SequentialNetworkRequest)
                    .Cast<SequentialNetworkRequest>()
                    .FirstOrDefault(x => x.SequenceNumber == sequenceNumber);
            }
        }

        private void RemoveRequest(NetworkRequest networkRequest)
        {
            lock (_networkRequests)
            {
                _networkRequests.Remove(networkRequest);
            }
        }
    }
}
