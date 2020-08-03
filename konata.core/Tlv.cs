using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Konata.Utils;

namespace Konata
{
    class Tlv
    {

        public byte[] t001(UInt64 Uin, byte[] IPAddress)
        {
            TlvBuilder _builder = new TlvBuilder(1);
            _builder.PushInt16(1); // _ip_ver
            _builder.PushInt32(new Random().Next());
            _builder.PushBytes(IPAddress);
            _builder.PushInt16(0);
            return _builder.GetPacket();
        }

        public byte[] t018(Int64 AppID, Int32 AppClientVersion, UInt64 Uin, Int32 PreservedBeZero)
        {
            TlvBuilder _builder = new TlvBuilder(18);
            _builder.PushInt16(1); // _ping_version
            _builder.PushInt32(1536); // _sso_version
            _builder.PushInt32((Int32)AppID);
            _builder.PushInt32(AppClientVersion);
            _builder.PushInt32((Int32)Uin);
            _builder.PushInt16((Int16)PreservedBeZero);
            _builder.PushInt16(0);
            return _builder.GetPacket();
        }

        public byte[] t106()
        {

            return new byte[0];
        }

        public byte[] t100()
        {

            return new byte[0];
        }

        public byte[] t116()
        {

            return new byte[0];
        }

        public byte[] t107()
        {

            return new byte[0];
        }

        public byte[] t142()
        {

            return new byte[0];
        }

        public byte[] t144()
        {

            return new byte[0];
        }

        public byte[] t145()
        {

            return new byte[0];
        }

        public byte[] t147()
        {

            return new byte[0];
        }

        public byte[] t154()
        {

            return new byte[0];
        }

        public byte[] t141()
        {

            return new byte[0];
        }

        public byte[] t008()
        {

            return new byte[0];
        }

        public byte[] t511()
        {

            return new byte[0];
        }

        public byte[] t187()
        {

            return new byte[0];
        }

        public byte[] t188()
        {

            return new byte[0];
        }

        public byte[] t191()
        {

            return new byte[0];
        }

        public byte[] t202()
        {

            return new byte[0];
        }

        public byte[] t177()
        {

            return new byte[0];
        }

        public byte[] t516()
        {

            return new byte[0];
        }

        public byte[] t521()
        {

            return new byte[0];
        }

        public byte[] t525()
        {

            return new byte[0];
        }

    }

}
