using System;

namespace Konata.Msf.Packets.Tlvs
{
    public class TlvBase : Packet
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

        public byte[] GetBytes()
        {
            PutUshortBE(GetTlvCmd());
            PutBytes(GetTlvBody());
            return base.GetBytes();
        }
    }
}
