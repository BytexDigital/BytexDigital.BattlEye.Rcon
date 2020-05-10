using Nito.AsyncEx;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.BattlEye.Rcon.Requests
{
    public abstract class NetworkRequest : NetworkMessage
    {
        public NetworkResponse Response { get; protected set; }
        public bool ResponseReceived { get { return _responseReceived.IsSet; } }
        public DateTime? ResponseReceivedTimeUtc { get; protected set; }
        public bool Acknowledged { get { return _acknowledged.IsSet; } }
        public DateTime? AcknowledgedTimeUtc { get; private set; }

        private AsyncManualResetEvent _acknowledged = new AsyncManualResetEvent(false);

        private AsyncManualResetEvent _responseReceived = new AsyncManualResetEvent(false);

        public bool WaitUntilResponseReceived(int timeout)
        {
            try
            {
                _responseReceived.Wait(new CancellationTokenSource(timeout).Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> WaitUntilResponseReceivedAsync(CancellationToken? cancellationToken)
        {
            try
            {
                await _responseReceived.WaitAsync(cancellationToken ?? CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void WaitUntilResponseReceived()
        {
            _responseReceived.Wait();
        }

        public void WaitUntilAcknowledged()
        {
            _acknowledged.Wait();
        }

        public bool WaitUntilAcknowledged(int timeout)
        {
            try
            {
                _acknowledged.Wait(new CancellationTokenSource(timeout).Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> WaitUntilAcknowledgedAsync(CancellationToken? cancellationToken)
        {
            try
            {
                await _acknowledged.WaitAsync(cancellationToken ?? CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void MarkAcknowledged()
        {
            AcknowledgedTimeUtc = DateTime.UtcNow;
            _acknowledged.Set();
        }

        internal void SetResponse(NetworkResponse networkResponse)
        {
            Response = networkResponse;
        }

        internal void MarkResponseReceived()
        {
            ProcessResponse(Response);

            ResponseReceivedTimeUtc = DateTime.UtcNow;
            _responseReceived.Set();

            if (!Acknowledged)
            {
                MarkAcknowledged();
            }
        }

        protected virtual void ProcessResponse(NetworkResponse networkResponse)
        {

        }
    }
}
