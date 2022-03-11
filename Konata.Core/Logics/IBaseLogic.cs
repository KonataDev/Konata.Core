using Konata.Core.Events;

namespace Konata.Core.Logics
{
    public interface IBaseLogic
    {
        public void Incoming(ProtocolEvent e);
    }
}
