using System.Threading.Tasks;
using Konata.Core.Events;

namespace Konata.Core.Components.Logics;

internal interface IBaseLogic
{
    public Task Incoming(ProtocolEvent e);
}
