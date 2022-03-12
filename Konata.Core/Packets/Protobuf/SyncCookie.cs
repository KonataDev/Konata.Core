using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

internal class SyncCookie : ProtoTreeRoot
{
	public SyncCookie(long timeStamp)
	{
		AddLeafVar("08", timeStamp);
		AddLeafVar("10", timeStamp);
		AddLeafVar("28", 2267374858);
		AddLeafVar("48", 1657171111);
		AddLeafVar("58", 1828320251);
		AddLeafVar("68", timeStamp);
		AddLeafVar("70", 0);
	}
}
