using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class TlvBase : PacketBase
    {
        /// <summary>
        /// 獲取tlv類型
        /// </summary>
        /// <returns>short</returns>
        public virtual ushort GetTlvCmd() => 0;

        /// <summary>
        /// 獲取tlv包體
        /// </summary>
        /// <returns>StreamBuilder</returns>
        public virtual byte[] GetTlvBody() => null;

        public override byte[] GetBytes()
        {
            ushort tlvCmd = GetTlvCmd();
            byte[] tlvBody = GetTlvBody();

            StreamBuilder builder = new StreamBuilder();
            builder.PushUInt16(tlvCmd);
            builder.PushBytes(tlvBody, false, true);
            return builder.GetBytes();
        }
    }
}
