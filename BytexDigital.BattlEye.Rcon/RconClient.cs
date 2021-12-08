using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Events;
using BytexDigital.BattlEye.Rcon.Requests;
using BytexDigital.BattlEye.Rcon.Responses;

using Nito.AsyncEx;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.BattlEye.Rcon
{
    public class RconClient
    {
        /// <summary>
        /// Defines the interval after how many milliseconds the <see cref="RconClient"/> is going to reattempt connecting to the rcon server.
        /// </summary>
        public int ReconnectInterval { get; set; } = 2500;
        /// <summary>
        /// Defines whether the <see cref="RconClient"/> should reattempt connecting to the rcon server in the given <see cref="ReconnectInterval"/>.
        /// <para>If this option is enabled, the client will enter an automatic reconnect loop both after an unsuccessful initial connect attempt and after connection loss.</para>
        /// </summary>
        public bool ReconnectOnFailure { get; set; } = true;
        /// <summary>
        /// Returns whether the client is currently successfully connected to an rcon server.
        /// </summary>
        public bool IsConnected { get; private set; } = false;
        /// <summary>
        /// Returns true if the client is currently connected or automatically trying to reconnect.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// <see cref="IPEndPoint"/> the client will attempt connecting to.
        /// </summary>
        public IPEndPoint RemoteEndpoint { get; }

        /// <summary>
        /// Triggered when the client successfully connects to an rcon server.
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// Triggered when the client disconnects from an rcon server.
        /// </summary>
        public event EventHandler Disconnected;
        /// <summary>
        /// Triggered when the client receives a message that is not bound to a request such as a chat message, a connect or a disconnect message.
        /// </summary>
        public event EventHandler<string> MessageReceived;
        /// <summary>
        /// Triggered when a client has connected to the gameserver and had it's GUID verified. Contains parsed information about the event.
        /// </summary>
        public event EventHandler<PlayerConnectedArgs> PlayerConnected;
        /// <summary>
        /// Triggered when a client disconnects from the gameserver. Contains parsed information about the event.
        /// </summary>
        public event EventHandler<PlayerDisconnectedArgs> PlayerDisconnected;
        /// <summary>
        /// Triggered when a client has been removed forcefully from the gameserver (this could be a kick or a ban). Contains parsed information about the event.
        /// </summary>
        public event EventHandler<PlayerRemovedArgs> PlayerRemoved;

        private string _password;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _connectionCancelTokenSource;
        private NetworkConnection _networkConnection;
        private AsyncManualResetEvent _connected = new AsyncManualResetEvent();

        public RconClient(string ip, int port, string password) : this(new IPEndPoint(IPAddress.Parse(ip), port), password) { }

        public RconClient(IPEndPoint remoteEndpoint, string password)
        {
            RemoteEndpoint = remoteEndpoint;
            _password = password;
        }

        /// <summary>
        /// Sends a raw string command to the rcon server.
        /// </summary>
        /// <param name="command">Command string to send</param>
        /// <returns><see cref="CommandNetworkRequest"/> which will be acknowledged by the rcon server and (if sent by the rcon server) contain the response string.</returns>
        public CommandNetworkRequest Send(string command)
        {
            var request = new CommandNetworkRequest(command);
            _networkConnection?.Send(request);

            return request;
        }

        /// <summary>
        /// Sends the command to the rcon server.
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <returns><see cref="CommandNetworkRequest"/> which will be acknowledged by the rcon server and (if sent by the rcon server) contain the response string.</returns>
        public CommandNetworkRequest Send(Command command)
        {
            var request = new CommandNetworkRequest(command);
            _networkConnection?.Send(request);

            return request;
        }

        /// <summary>
        /// Sends the command to the rcon server, parses the response string and returns it. True means a response was received, false means timeout.
        /// <para>This method is only meant to be used with a <see cref="Command"/> implementing <see cref="IProvidesResponse{T}"/>.
        /// If not implemented, a <see cref="InvalidOperationException"/> will be thrown.</para>
        /// </summary>
        /// <typeparam name="ResponseType">Datatype of the expected result</typeparam>
        /// <param name="command">Command to send</param>
        /// <param name="timeout">Timeout after which to cancel waiting</param>
        /// <param name="result">Response object</param>
        /// <returns>True if a response was received, false if a timeout occurred.</returns>
        public bool Fetch<ResponseType, CommandType>(CommandType command, int timeout, out ResponseType result) where CommandType : Command, IProvidesResponse<ResponseType>
        {
            (bool success, var resultData) = FetchAsync<ResponseType, CommandType>(command, new CancellationTokenSource(timeout).Token).ConfigureAwait(false).GetAwaiter().GetResult();

            result = resultData;

            return success;
        }

        public async Task<(bool, ResponseType)> FetchAsync<ResponseType, CommandType>(CommandType command, CancellationToken? cancellationToken) where CommandType : Command, IProvidesResponse<ResponseType>
        {
            var request = Send(command);
            bool success = await request.WaitUntilResponseReceivedAsync(cancellationToken ?? CancellationToken.None);

            if (success && request.ResponseReceived)
            {
                return (true, (command as IProvidesResponse<ResponseType>).GetResponse());
            }

            return (false, default);
        }


        /// <summary>
        /// Blocking call which will wait until the client has successfully connected to the rcon server. Returned value true means connected, false means timeout.
        /// </summary>
        /// <param name="timeout">Milliseconds after which to cancel waiting.</param>
        /// <returns>True if connected, false if timed out.</returns>
        public bool WaitUntilConnected(int timeout)
        {
            try
            {
                _connected.Wait(new CancellationTokenSource(timeout).Token);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Non-blocking call which will wait until the client has successfully connected to the rcon server. Returned value true means connected, false means timeout.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if connected, false if timed out.</returns>
        public async Task<bool> WaitUntilConnectedAsync(CancellationToken? cancellationToken = null)
        {
            try
            {
                await _connected.WaitAsync(cancellationToken ?? CancellationToken.None);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Blocking call whcih will wait until the client has successfully connected to the rcon server.
        /// </summary>
        public void WaitUntilConnected()
        {
            _connected.Wait();
        }

        /// <summary>
        /// Attempts connecting to the rcon server. If true is returned, the client has connected successfully, otherwise false.
        /// </summary>
        /// <returns>True if success, otherwise false</returns>
        public bool Connect()
        {
            if (IsRunning)
            {
                return IsConnected;
            }

            bool success = AttemptConnect();

            if (!success && ReconnectOnFailure)
            {
                IsRunning = true;

                Task.Run(async () =>
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(ReconnectInterval);

                        if (_cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        bool loopSuccess = AttemptConnect();

                        if (loopSuccess)
                        {
                            break;
                        }
                    }
                });
            }

            return success;
        }

        /// <summary>
        /// Disconnects the client from the rcon server and/or cancels any automatic reconnect attempt.
        /// </summary>
        public void Disconnect()
        {
            _cancellationTokenSource?.Cancel();
            _connectionCancelTokenSource?.Cancel();
            Disconnected?.Invoke(this, new EventArgs());
        }

        private bool AttemptConnect()
        {
            _connectionCancelTokenSource = new CancellationTokenSource();
            _networkConnection = new NetworkConnection(RemoteEndpoint, _connectionCancelTokenSource.Token);
            _networkConnection.BeginReceiving();
            _networkConnection.Disconnected += OnDisconnected;
            _networkConnection.MessageReceived += OnMessageReceived;
            _networkConnection.ProtocolEvent += OnProtocolEvent;

            var loginRequest = new LoginNetworkRequest(_password);
            _networkConnection.Send(loginRequest);

            bool received = loginRequest.WaitUntilResponseReceived(3000);

            if (!received)
            {
                return false;
            }

            var response = loginRequest.Response as LoginNetworkResponse;

            if (response.Success)
            {
                _networkConnection.BeginHeartbeat();
                _connected.Set();

                IsConnected = true;
                Connected?.Invoke(this, new EventArgs());
            }

            return response.Success;
        }

        private void OnProtocolEvent(object sender, GenericParsedEventArgs e)
        {
            if (e.Arguments is PlayerConnectedArgs playerConnectedArgs)
            {
                PlayerConnected?.Invoke(this, playerConnectedArgs);
            }

            if (e.Arguments is PlayerDisconnectedArgs playerDisconnectedArgs)
            {
                PlayerDisconnected?.Invoke(this, playerDisconnectedArgs);
            }

            if (e.Arguments is PlayerRemovedArgs playerRemovedArgs)
            {
                PlayerRemoved?.Invoke(this, playerRemovedArgs);
            }
        }

        private void OnMessageReceived(object sender, string e) => MessageReceived?.Invoke(sender, e);

        private void OnDisconnected(object sender, EventArgs e)
        {
            if (!IsConnected)
            {
                return;
            }

            Disconnected?.Invoke(this, new EventArgs());
            _connected.Reset();
            _connectionCancelTokenSource?.Cancel();
            IsConnected = false;
            IsRunning = false;

            if (_connectionCancelTokenSource != null && _connectionCancelTokenSource.IsCancellationRequested)
            {
                if (ReconnectOnFailure)
                {
                    Connect();
                }
            }
        }
    }
}
