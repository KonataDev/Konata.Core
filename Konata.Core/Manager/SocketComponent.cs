using Konata.Core.Service;
using Konata.Runtime.Base;
using Konata.Runtime.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Manager
{
    public class SocketComponent:Component
    {
        public ISocket Socket { get; set; }

        public SocketComponent()
        {
            this.Recycle = false;
        }

        public override void Dispose()
        {
            SocketService service = ServiceManager.Instance.GetService<SocketService>();
            if (service != null&&this.Socket!=null)
            {
                service.UnloadSocketInstance(Parent);
            }
            this.Socket = null;
            base.Dispose();
        }
    }
}
