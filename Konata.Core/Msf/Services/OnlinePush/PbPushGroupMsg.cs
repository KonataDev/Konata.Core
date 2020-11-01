using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Services.OnlinePush
{
    public class PbPushGroupMsg : Service
    {
        private PbPushGroupMsg()
        {
            Register("OnlinePush.PbPushGroupMsg", this);
        }

        public static Service Instance { get; } = new PbPushGroupMsg();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            var packet = (Packet)args[0];
            var length = packet.TakeUintBE(out var _);

            return Handle_PbPushGroupMsg(core, packet.TakeBytes(out var _, length - 4));
        }

        private bool Handle_PbPushGroupMsg(Core core, byte[] pbData)
        {
            var root = new ProtoTreeRoot(pbData, true);
            {
                root.GetLeafString("0A.0A.4A.42", out var groupName);
                root.GetLeafVar("0A.0A.4A.08", out var groupNumber);

                root.GetLeafString("0A.0A.4A.22", out var memberName);
                root.GetLeafVar("0A.0A.08", out var memberNumber);

                root.GetLeafString("0A.1A.0A.12.0A.0A", out var groupMsg);

                Console.WriteLine($"{groupName}[{groupNumber}] - {memberName}[{memberNumber}]: {groupMsg}");
            }
            return true;
        }
    }
}
