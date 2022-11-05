using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Events;
using BytexDigital.BattlEye.Rcon.Requests;
using BytexDigital.BattlEye.Rcon.Responses;
using Nito.AsyncEx;

namespace BytexDigital.BattlEye.Rcon
{
    public class RconClient
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly AsyncManualResetEvent _connected = new AsyncManualResetEvent();

        private readonly string _password;
        private CancellationTokenSource _connectionCancelTokenSource;
        private NetworkConnection _networkConnection;

        /// <summary>
        ///     Defines the interval after how many milliseconds the <see cref="RconClient" /> is going to reattempt connecting to
        ///     the rcon server.
        /// </summary>
        public int ReconnectInterval { get; set; } = 2500;

        /// <summary>
        ///     Defines whether the <see cref="RconClient" /> should reattempt connecting to the rcon server in the given
        ///     <see cref="ReconnectInterval" />.
        ///     <para>
        ///         If this option is enabled, the client will enter an automatic reconnect loop both after an unsuccessful
        ///         initial connect attempt and after connection loss.
        ///     </para>
        /// </summary>
        public bool ReconnectOnFailure { get; set; } = true;

        /// <summary>
        ///     Returns whether the client is currently successfully connected to an rcon server.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Returns true if the client is currently connected or automatically trying to reconnect.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     <see cref="IPEndPoint" /> the client will attempt connecting to.
        /// </summary>
        public IPEndPoint RemoteEndpoint { get; }

        public RconClient(string ip, int port, string password) : this(
            new IPEndPoint(IPAddress.Parse(ip), port),
            password)
        {
        }

        public RconClient(IPEndPoint remoteEndpoint, string password)
        {
            RemoteEndpoint = remoteEndpoint;
            _password = password;
        }

        /// <summary>
        ///     Triggered when the client successfully connects to an rcon server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        ///     Triggered when the client disconnects from an rcon server.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        ///     Triggered when the client receives a message that is not bound to a request such as a chat message, a connect or a
        ///     disconnect message.
        /// </summary>
        public event EventHandler<string> MessageReceived;

        /// <summary>
        ///     Triggered when a client has connected to the gameserver and had it's GUID verified. Contains parsed information
        ///     about the event.
        /// </summary>
        public event EventHandler<PlayerConnectedArgs> PlayerConnected;

        /// <summary>
        ///     Triggered when a client disconnects from the gameserver. Contains parsed information about the event.
        /// </summary>
        public event EventHandler<PlayerDisconnectedArgs> PlayerDisconnected;

        /// <summary>
        ///     Triggered when a client has been removed forcefully from the gameserver (this could be a kick or a ban). Contains
        ///     parsed information about the event.
        /// </summary>
        public event EventHandler<PlayerRemovedArgs> PlayerRemoved;

        /// <summary>
        ///     Sends a raw string command to the rcon server.
        /// </summary>
        /// <param name="command">Command string to send</param>
        /// <returns>
        ///     <see cref="CommandNetworkRequest" /> which will be acknowledged by the rcon server and (if sent by the rcon
        ///     server) contain the response string.
        /// </returns>
        public CommandNetworkRequest Send(string command)
        {
            var request = new CommandNetworkRequest(command);
            _networkConnection?.Send(request);

            return request;
        }

        /// <summary>
        ///     Sends the command to the rcon server.
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <returns>
        ///     <see cref="CommandNetworkRequest" /> which will be acknowledged by the rcon server and (if sent by the rcon
        ///     server) contain the response string.
        /// </returns>
        public CommandNetworkRequest Send(Command command)
        {
            var request = new CommandNetworkRequest(command);
            _networkConnection?.Send(request);

            return request;
        }

        /// <summary>
        ///     Sends the command to the rcon server, parses the response string and returns it. True means a response was
        ///     received, false means timeout.
        ///     <para>
        ///         This method is only meant to be used with a <see cref="Command" /> implementing
        ///         <see cref="IProvidesResponse{T}" />.
        ///         If not implemented, a <see cref="InvalidOperationException" /> will be thrown.
        ///     </para>
        /// </summary>
        /// <typeparam name="TResponseType">Datatype of the expected result</typeparam>
        /// <typeparam name="TCommandType">Datatype of the command to send</typeparam>
        /// <param name="command">Command to send</param>
        /// <param name="timeout">Timeout after which to cancel waiting</param>
        /// <param name="result">Response object</param>
        /// <returns>True if a response was received, false if a timeout occurred.</returns>
        public bool Fetch<TResponseType, TCommandType>(TCommandType command, int timeout, out TResponseType result)
            where TCommandType : Command, IProvidesResponse<TResponseType>
        {
            var (success, resultData) =
                FetchAsync<TResponseType, TCommandType>(command, new CancellationTokenSource(timeout).Token)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

            result = resultData;

            return success;
        }

        public async Task<(bool, TResponseType)> FetchAsync<TResponseType, TCommandType>(
            TCommandType command,
            CancellationToken cancellationToken = default)
            where TCommandType : Command, IProvidesResponse<TResponseType>
        {
            var request = Send(command);
            var success = await request.WaitUntilResponseReceivedAsync(cancellationToken);

            if (success && request.ResponseReceived) return (true, command.GetResponse());

            return (false, default);
        }


        /// <summary>
        ///     Blocking call which will wait until the client has successfully connected to the rcon server. Returned value true
        ///     means connected, false means timeout.
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
        ///     Non-blocking call which will wait until the client has successfully connected to the rcon server. Returned value
        ///     true means connected, false means timeout.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if connected, false if timed out.</returns>
        public async Task<bool> WaitUntilConnectedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _connected.WaitAsync(cancellationToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Blocking call whcih will wait until the client has successfully connected to the rcon server.
        /// </summary>
        public void WaitUntilConnected()
        {
            _connected.Wait();
        }

        /// <summary>
        ///     Attempts connecting to the rcon server. If true is returned, the client has connected successfully, otherwise
        ///     false. If <see cref="ReconnectOnFailure" /> is true, then a loop is internally started that will repeatedly
        ///     attempt to connect.
        /// </summary>
        /// <returns>True if success, otherwise false</returns>
        public bool Connect()
        {
            if (IsRunning) return IsConnected;

            var success = AttemptConnect();

            if (!success && ReconnectOnFailure)
            {
                IsRunning = true;

                Task.Run(
                    async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            await Task.Delay(ReconnectInterval);

                            if (_cancellationTokenSource.IsCancellationRequested) break;

                            var loopSuccess = AttemptConnect();

                            if (loopSuccess) break;
                        }
                    });
            }

            return success;
        }

        /// <summary>
        ///     Disconnects the client from the rcon server and/or cancels any automatic reconnect attempt.
        /// </summary>
        public void Disconnect()
        {
            _cancellationTokenSource?.Cancel();
            _connectionCancelTokenSource?.Cancel();
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private bool AttemptConnect()
        {
            // Link cancel token of the connection to the overarching cancel token of the RconClient.
            _connectionCancelTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);

            _networkConnection = new NetworkConnection(RemoteEndpoint, _connectionCancelTokenSource.Token);
            _networkConnection.Disconnected += OnDisconnected;
            _networkConnection.MessageReceived += OnMessageReceived;
            _networkConnection.ProtocolEvent += OnProtocolEvent;

            _networkConnection.BeginReceiving();

            var loginRequest = new LoginNetworkRequest(_password);
            _networkConnection.Send(loginRequest);

            var received = loginRequest.WaitUntilResponseReceived(3000);

            if (received)
            {
                if (loginRequest.Response is LoginNetworkResponse response && response.Success)
                {
                    _networkConnection.BeginHeartbeat();
                    _connected.Set();

                    IsConnected = true;
                    Connected?.Invoke(this, EventArgs.Empty);

                    return true;
                }
            }

            _connectionCancelTokenSource.Cancel();
            
            _networkConnection.Dispose();
            _networkConnection = null;

            return false;
        }

        private void OnProtocolEvent(object sender, GenericParsedEventArgs e)
        {
            switch (e.Arguments)
            {
                case PlayerConnectedArgs playerConnectedArgs:
                    PlayerConnected?.Invoke(this, playerConnectedArgs);
                    break;
                case PlayerDisconnectedArgs playerDisconnectedArgs:
                    PlayerDisconnected?.Invoke(this, playerDisconnectedArgs);
                    break;
                case PlayerRemovedArgs playerRemovedArgs:
                    PlayerRemoved?.Invoke(this, playerRemovedArgs);
                    break;
            }
        }

        private void OnMessageReceived(object sender, string e)
        {
            MessageReceived?.Invoke(sender, e);
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            if (!IsConnected) return;

            Disconnected?.Invoke(this, EventArgs.Empty);

            _connected.Reset();
            _connectionCancelTokenSource?.Cancel();
            
            _networkConnection.Dispose();
            _networkConnection = null;
            
            IsConnected = false;
            IsRunning = false;

            if (_connectionCancelTokenSource != null && _connectionCancelTokenSource.IsCancellationRequested)
            {
                if (ReconnectOnFailure) Connect();
            }
        }
    }
}