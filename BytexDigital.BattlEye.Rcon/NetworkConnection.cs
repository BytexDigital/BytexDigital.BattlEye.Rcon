using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BytexDigital.BattlEye.Rcon.Events;
using BytexDigital.BattlEye.Rcon.Requests;

namespace BytexDigital.BattlEye.Rcon
{
    public class NetworkConnection : IDisposable
    {
        private readonly NetworkMessageHandler _handler;
        private readonly IPEndPoint _remoteEndpoint;
        private readonly SequenceCounter _sequenceCounter;
        private readonly UdpClient _udpClient;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastSent;

        public NetworkConnection(IPEndPoint remoteEndpoint, CancellationToken cancellationToken)
        {
            _remoteEndpoint = remoteEndpoint;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _udpClient = new UdpClient(remoteEndpoint.AddressFamily);
            _udpClient.Connect(_remoteEndpoint);
            _handler = new NetworkMessageHandler(this);
            _sequenceCounter = new SequenceCounter();
        }

        public event EventHandler<string> MessageReceived;
        public event EventHandler<GenericParsedEventArgs> ProtocolEvent;
        public event EventHandler Disconnected;

        public void BeginReceiving()
        {
            _ = Task.Run(Receive, _cancellationTokenSource.Token);
        }

        public void BeginHeartbeat()
        {
            _ = Task.Run(Heartbeat, _cancellationTokenSource.Token);
        }

        public void Send(NetworkMessage networkMessage)
        {
            _handler.Cleanup();

            if (networkMessage is SequentialNetworkRequest sequentialNetworkRequest)
                sequentialNetworkRequest.SetSequenceNumber(_sequenceCounter.Next());

            if (networkMessage is NetworkRequest networkRequest) _handler.Track(networkRequest);

            var data = networkMessage.ToBytes();
            _udpClient.Send(data, data.Length);

            networkMessage.MarkSent();

            _lastSent = DateTime.UtcNow;
        }

        internal void FireMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        internal void FireProtocolEvent(GenericParsedEventArgs args)
        {
            ProtocolEvent?.Invoke(this, args);
        }

        private async void Receive()
        {
            try
            {
                var closeTask = Task.Delay(-1, _cancellationTokenSource.Token);

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var receiveTask = _udpClient.ReceiveAsync();
                    var task = await Task.WhenAny(receiveTask, closeTask).ConfigureAwait(false);

                    if (task == closeTask) break;
                    if (receiveTask.IsFaulted) continue;

                    var result = receiveTask.Result;

                    try
                    {
                        _handler.Handle(result.Buffer);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private async void Heartbeat()
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(3000, _cancellationTokenSource.Token);

                    if (_cancellationTokenSource.IsCancellationRequested) return;

                    var keepAlivePacket = new CommandNetworkRequest("");

                    Send(keepAlivePacket);
                    keepAlivePacket.WaitUntilAcknowledged(5000);

                    if (keepAlivePacket.Acknowledged) continue;

                    try
                    {
                        Disconnected?.Invoke(this, EventArgs.Empty);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            
            try
            {
                _udpClient?.Close();
            }
            catch
            {
                // ignored
            }

            try
            {
                _udpClient?.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}
