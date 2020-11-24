using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base.Event
{
    public interface IEvent
    {
        void Handle(EventArgs arg);
    }
}
