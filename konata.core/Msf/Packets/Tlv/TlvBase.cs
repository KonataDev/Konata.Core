using System;

namespace Konata.Msf.Packets.Tlvs
{
    public class TlvBase : Packet
    {
        /// <summary>
        /// 寫入tlv類型
        /// </summary>
        /// <returns></returns>
        public virtual void PutTlvCmd()
        {
            // DO Nothing Here.
        }

        /// <summary>
        /// 寫入tlv數據
        /// </summary>
        /// <returns></returns>
        public virtual void PutTlvBody()
        {
            // DO Nothing Here.
        }

        public override byte[] GetBytes()
        {
            PutTlvCmd();
            PutTlvBody();
            return base.GetBytes();
        }
    }
}
