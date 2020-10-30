using System;
using Konata.Msf.Packets.Wup;
using Konata.Library.IO;

namespace Konata.Msf.Services.MessageSvc
{
    public class PushNotify : Service
    {
        private PushNotify()
        {
            Register("MessageSvc.PushNotify", this);
        }

        public static Service Instance { get; } = new PushNotify();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            // 未知多餘頭部
            var packet = (Packet)args[0];
            packet.TakeUintBE(out var len);
            packet.EatBytes(len - 4);
            packet.TakeUintBE(out len);
            var unipacket = new UniPacket(packet.TakeBytes(out var _, len - 4));

            if (unipacket.packageFuncName != "PushNotify")
                return false;

            return Handle_Notify(core);
        }

        private bool Handle_Notify(Core core)
        {




            // TODO: MessageSvc.PbGetMsg
            return true;
        }
    }
}
