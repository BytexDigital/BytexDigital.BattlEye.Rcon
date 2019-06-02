using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Domain;
using BytexDigital.BattlEye.Rcon.Events;
using BytexDigital.BattlEye.Rcon.Requests;
using BytexDigital.BattlEye.Rcon.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.BattlEye.Rcon {
    public class RconClient {
        public int ReconnectInterval { get; set; } = 2500;
        public bool ReconnectOnFailure { get; set; } = true;
        public bool IsConnected { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<string> MessageReceived;
        public event EventHandler<PlayerConnectedArgs> PlayerConnected;
        public event EventHandler<PlayerDisconnectedArgs> PlayerDisconnected;
        public event EventHandler<PlayerRemovedArgs> PlayerRemoved;

        private IPEndPoint _remoteEndpoint;
        private string _password;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _connectionCancelTokenSource;
        private NetworkConnection _networkConnection;
        private ManualResetEventSlim _connected = new ManualResetEventSlim();

        public RconClient(string ip, int port, string password) : this(new IPEndPoint(IPAddress.Parse(ip), port), password) { }

        public RconClient(IPEndPoint remoteEndpoint, string password) {
            _remoteEndpoint = remoteEndpoint;
            _password = password;
        }

        public CommandNetworkRequest Send(string command) {
            var request = new CommandNetworkRequest(command);
            _networkConnection?.Send(request);

            return request;
        }

        public CommandNetworkRequest Send(Command command) {
            var request = new CommandNetworkRequest(command);
            _networkConnection?.Send(request);

            return request;
        }

        public bool Fetch<ResponseType>(Command command, int timeout, out ResponseType result) {
            if (!(command is IProvidesResponse<ResponseType>)) {
                throw new InvalidOperationException("Given command does not support this operation " +
                    "(Either this command does not provide a response or the response type is different to the given one.");
            }

            var request = Send(command);
            bool success = request.WaitUntilResponseReceived(timeout);

            if (success && request.ResponseReceived) {
                result = (command as IProvidesResponse<ResponseType>).GetResponse();
                return true;
            }

            result = default(ResponseType);
            return false;
        }

        public bool WaitUntilConnected(int timeout) {
            return _connected.Wait(timeout);
        }

        public void WaitUntilConnected() {
            _connected.Wait();
        }

        public bool Connect() {
            if (IsRunning) {
                return IsConnected;
            }

            bool success = AttemptConnect();

            if (!success && ReconnectOnFailure) {
                IsRunning = true;

                Task.Run(async () => {
                    while (!_cancellationTokenSource.IsCancellationRequested) {
                        await Task.Delay(ReconnectInterval);

                        if (_cancellationTokenSource.IsCancellationRequested) {
                            break;
                        }

                        bool loopSuccess = AttemptConnect();

                        if (loopSuccess) {
                            break;
                        }
                    }
                });
            }

            return success;
        }

        public void Disconnect() {
            _cancellationTokenSource?.Cancel();
            _connectionCancelTokenSource?.Cancel();
        }

        private bool AttemptConnect() {
            _connectionCancelTokenSource = new CancellationTokenSource();
            _networkConnection = new NetworkConnection(_remoteEndpoint, _connectionCancelTokenSource.Token);
            _networkConnection.BeginReceiving();
            _networkConnection.Disconnected += OnDisconnected;
            _networkConnection.MessageReceived += OnMessageReceived;
            _networkConnection.ProtocolEvent += OnProtocolEvent;

            var loginRequest = new LoginNetworkRequest(_password);
            _networkConnection.Send(loginRequest);

            bool received = loginRequest.WaitUntilResponseReceived(3000);

            if (!received) {
                return false;
            }

            var response = loginRequest.Response as LoginNetworkResponse;

            if (response.Success) {
                _networkConnection.BeginHeartbeat();
                _connected.Set();

                IsConnected = true;
                Connected?.Invoke(this, new EventArgs());
            }

            return response.Success;
        }

        private void OnProtocolEvent(object sender, GenericParsedEventArgs e) {
            if (e.Arguments is PlayerConnectedArgs playerConnectedArgs) {
                PlayerConnected?.Invoke(this, playerConnectedArgs);
            }

            if (e.Arguments is PlayerDisconnectedArgs playerDisconnectedArgs) {
                PlayerDisconnected?.Invoke(this, playerDisconnectedArgs);
            }

            if (e.Arguments is PlayerRemovedArgs playerRemovedArgs) {
                PlayerRemoved?.Invoke(this, playerRemovedArgs);
            }
        }

        private void OnMessageReceived(object sender, string e) => MessageReceived?.Invoke(sender, e);

        private void OnDisconnected(object sender, EventArgs e) {
            Disconnected?.Invoke(this, new EventArgs());
            _connected.Reset();
            _connectionCancelTokenSource?.Cancel();
            IsConnected = false;
            IsRunning = false;

            if (ReconnectOnFailure) {
                Connect();
            }
        }
    }
}
