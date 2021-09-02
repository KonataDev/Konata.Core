using System.Collections.Generic;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Packets.SvcPush
{
    public class SvcPushConfig : UniPacket
    {
        public List<ServerList> ServerList { get; private set; }

        public byte[] SessionToken { get; private set; }

        public SvcPushConfig(byte[] payload)
            : base(payload, (userdata, r) =>
            {
                // Initialize
                var p = (SvcPushConfig) userdata;
                p.ServerList = new();

                // Get struct
                var tree = (JStruct) r["0.2.5"];
                {
                    // Get session key
                    p.SessionToken = ((JSimpleList) tree["1"]).Value;

                    // Get server list
                    foreach (JStruct i in (JList) tree["0.0.1"])
                    {
                        p.ServerList.Add(new ServerList
                        {
                            Host = (string) (JString) i["1"],
                            Port = (int) (JNumber) i["2"],
                        });
                    }
                }
            })
        {
        }
    }

    public struct ServerList
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}
