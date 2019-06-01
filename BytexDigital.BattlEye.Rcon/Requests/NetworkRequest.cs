using BytexDigital.BattlEye.Rcon.Requests;
using System;
using System.Threading;

namespace BytexDigital.BattlEye.Rcon.Requests {
    public abstract class NetworkRequest : NetworkMessage {
        public NetworkResponse Response { get; protected set; }
        public bool ResponseReceived { get { return _responseReceived.IsSet; } }
        public DateTime? ResponseReceivedTimeUtc { get; protected set; }
        public bool Acknowledged { get { return _acknowledged.IsSet; } }
        public DateTime? AcknowledgedTimeUtc { get; private set; }

        private ManualResetEventSlim _acknowledged = new ManualResetEventSlim(false);

        private ManualResetEventSlim _responseReceived = new ManualResetEventSlim(false);

        public bool WaitUntilResponseReceived(int timeout) {
            return _responseReceived.Wait(timeout);
        }

        public void WaitUntilResponseReceived() {
            _responseReceived.Wait();
        }

        public void WaitUntilAcknowledged() {
            _acknowledged.Wait();
        }

        public bool WaitUntilAcknowledged(int timeout) {
            return _acknowledged.Wait(timeout);
        }

        internal void MarkAcknowledged() {
            AcknowledgedTimeUtc = DateTime.UtcNow;
            _acknowledged.Set();
        }

        internal void SetResponse(NetworkResponse networkResponse) {
            Response = networkResponse;
        }

        internal void MarkResponseReceived() {
            ProcessResponse(Response);

            ResponseReceivedTimeUtc = DateTime.UtcNow;
            _responseReceived.Set();

            if (!Acknowledged) {
                MarkAcknowledged();
            }
        }

        protected virtual void ProcessResponse(NetworkResponse networkResponse) {

        }
    }
}
