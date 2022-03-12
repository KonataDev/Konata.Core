using System.Threading.Tasks;
using Konata.Core.Events;

namespace Konata.Core.Logics
{
    public interface IBaseLogic
    {
        public Task Incoming(ProtocolEvent e);
    }
}
