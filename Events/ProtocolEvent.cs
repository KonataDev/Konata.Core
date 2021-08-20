// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Events
{
    public class ProtocolEvent : BaseEvent
    {
        public bool WaitForResponse { get; protected set; }

        public int SessionSequence { get; protected set; }

        public int ResultCode { get; protected set; }

        public uint Timeout { get; protected set; }

        internal ProtocolEvent(uint timeout, bool wait)
        {
            Timeout = timeout;
            WaitForResponse = wait;
        }

        internal ProtocolEvent(int resultCode)
            => ResultCode = resultCode;

        /// <summary>
        /// Set session sequence
        /// </summary>
        /// <param name="sessionSeq"></param>
        internal void SetSessionSequence(int sessionSeq)
            => SessionSequence = sessionSeq;
    }
}
