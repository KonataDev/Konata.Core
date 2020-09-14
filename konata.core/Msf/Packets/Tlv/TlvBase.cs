using System;

namespace Konata.Msf.Packets.Tlv
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
            EnterBarrier(2, Endian.Big);
            PutTlvBody();
            LeaveBarrier();

            return base.GetBytes();
        }
    }
}
