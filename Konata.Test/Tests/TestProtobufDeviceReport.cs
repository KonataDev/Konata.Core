using System;
using Konata.Library.Protobuf;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufDeviceReport : Test
    {
        public override bool Run()
        {
            var report = new DeviceReport(
                "G9009WKEU1BOL1",
                "",
                "REL",
                "c0ab6bb259",
                "samsung/klteduosctc/klte:6.0.1/MMB29M/G9009WKEU1CQB2:user/release-keys",
                "",
                "8edadfb1e4a02cdc",
                "G9009WKEU1BOL1",
                "lineage_kltechnduo-userdebug 8.1.0 OPM2.171026.006.H1 c0ab6bb259");

            Print(ProtoSerializer.Serialize(report));

            return true;
        }
    }
}
