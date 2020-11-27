using System;

namespace Konata.Runtime.Base.Event
{
    public interface IEvent
    {
        void Handle(KonataEventArgs arg);
    }
}
