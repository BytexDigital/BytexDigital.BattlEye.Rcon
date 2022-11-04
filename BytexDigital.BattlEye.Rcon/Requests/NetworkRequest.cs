using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace BytexDigital.BattlEye.Rcon.Requests
{
    public abstract class NetworkRequest : NetworkMessage
    {
        private readonly AsyncManualResetEvent _acknowledged = new AsyncManualResetEvent(false);

        private readonly AsyncManualResetEvent _responseReceived = new AsyncManualResetEvent(false);
        public NetworkResponse Response { get; protected set; }
        public bool ResponseReceived => _responseReceived.IsSet;
        public DateTime? ResponseReceivedTimeUtc { get; protected set; }
        public bool Acknowledged => _acknowledged.IsSet;
        public DateTime? AcknowledgedTimeUtc { get; private set; }

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

        public async Task<bool> WaitUntilResponseReceivedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _responseReceived.WaitAsync(cancellationToken);
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

        public async Task<bool> WaitUntilAcknowledgedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _acknowledged.WaitAsync(cancellationToken);
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

            if (!Acknowledged) MarkAcknowledged();
        }

        protected virtual void ProcessResponse(NetworkResponse networkResponse)
        {
        }
    }
}