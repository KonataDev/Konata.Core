using Konata.Msf.Utils.Crypt;
using System;

namespace Konata.Msf.Packets.Tlv
{
    public abstract class TlvBase : Packet
    {
        protected void PackGeneric()
        {
            PutTlvCmd();
            EnterBarrier(2, Endian.Big);
            PutTlvBody();
            LeaveBarrier();
        }

        protected void PackEncrypted(ICryptor cryptor, byte[] cryptKey)
        {
            PutTlvCmd();
            EnterBarrierEncrypted(2, Endian.Big, cryptor, cryptKey);
            PutTlvBody();
            LeaveBarrier();
        }

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
    }
}
