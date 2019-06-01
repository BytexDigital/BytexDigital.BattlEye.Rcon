using BytexDigital.BattlEye.Rcon.Events;
using BytexDigital.BattlEye.Rcon.Requests;
using BytexDigital.BattlEye.Rcon.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.BattlEye.Rcon {
    public class NetworkConnection {
        private IPEndPoint _remoteEndpoint;
        private CancellationToken _cancellationToken;
        private UdpClient _udpClient;
        private NetworkMessageHandler _handler;
        private SequenceCounter _sequenceCounter;
        private DateTime _lastSent = new DateTime();

        public event EventHandler<string> MessageReceived;
        public event EventHandler<GenericParsedEventArgs> ProtocolEvent;
        public event EventHandler Disconnected;

        public NetworkConnection(IPEndPoint remoteEndpoint, CancellationToken cancellationToken) {
            _remoteEndpoint = remoteEndpoint;
            _cancellationToken = cancellationToken;
            _udpClient = new UdpClient(remoteEndpoint.AddressFamily);
            _udpClient.Connect(_remoteEndpoint);
            _handler = new NetworkMessageHandler(this);
            _sequenceCounter = new SequenceCounter();
        }

        public void BeginReceiving() {
            Task.Run(() => Receive());
        }

        public void BeginHeartbeat() {
            Task.Run(() => Heartbeat());
        }

        public void Send(NetworkMessage networkMessage) {
            _handler.Cleanup();

            if (networkMessage is SequentialNetworkRequest sequentialNetworkRequest) {
                sequentialNetworkRequest.SetSequenceNumber(_sequenceCounter.Next());
            }

            if (networkMessage is NetworkRequest networkRequest) {
                _handler.Track(networkRequest);
            }

            byte[] data = networkMessage.ToBytes();
            _udpClient.Send(data, data.Length);

            networkMessage.MarkSent();

            _lastSent = DateTime.UtcNow;
        }

        internal void FireMessageReceived(string message) => MessageReceived?.Invoke(this, message);

        internal void FireProtocolEvent(GenericParsedEventArgs args) => ProtocolEvent?.Invoke(this, args);

        private async void Receive() {
            try {
                while (!_cancellationToken.IsCancellationRequested) {
                    var result = await _udpClient.ReceiveAsync().WithCancellation(_cancellationToken);
                    _handler.Handle(result.Buffer);
                }
            } catch (OperationCanceledException) {

            }
        }

        private async void Heartbeat() {
            while (!_cancellationToken.IsCancellationRequested) {
                await Task.Delay(3000);

                if (_cancellationToken.IsCancellationRequested) {
                    return;
                }

                var keepAlivePacket = new CommandNetworkRequest("");

                Send(keepAlivePacket);
                keepAlivePacket.WaitUntilAcknowledged(5000);

                if (!keepAlivePacket.Acknowledged) {
                    try {
                        Disconnected?.Invoke(this, new EventArgs());
                    } catch { }
                }
            }
        }
    }
}
