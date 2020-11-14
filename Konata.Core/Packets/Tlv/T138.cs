using System;

namespace Konata.Packets.Tlv
{
    public class T138Body : TlvBody
    {

        public readonly uint _chgTimeUnknown262;
        public readonly uint _chgTimeA2;
        public readonly uint _chgTimeA8;
        public readonly uint _chgTimeD2;
        public readonly uint _chgTimeSid;
        public readonly uint _chgTimeStWeb;
        public readonly uint _chgTimeLsKey;
        public readonly uint _chgTimeSKey;
        public readonly uint _chgTimeVKey;

        public T138Body(uint chgTimeUnknown262, uint chgTimeA2, uint chgTimeA8
            , uint chgTimeD2, uint chgTimeSid, uint chgTimeStWeb, uint chgTimeLsKey,
            uint chgTimeSKey, uint chgTimeVKey)
            : base()
        {
            _chgTimeA2 = chgTimeA2;
            _chgTimeA8 = chgTimeA8;
            _chgTimeD2 = chgTimeD2;
            _chgTimeSid = chgTimeSid;
            _chgTimeStWeb = chgTimeStWeb;
            _chgTimeLsKey = chgTimeLsKey;
            _chgTimeSKey = chgTimeSKey;
            _chgTimeVKey = chgTimeVKey;
            _chgTimeUnknown262 = chgTimeUnknown262;

            PutUintBE(0x09);

            PutUshortBE(0x0106);
            {
                PutUintBE(_chgTimeUnknown262);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x010a);
            {
                PutUintBE(_chgTimeA2);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x011c);
            {
                PutUintBE(_chgTimeLsKey);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x0102);
            {
                PutUintBE(_chgTimeA8);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x0103);
            {
                PutUintBE(_chgTimeStWeb);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x0120);
            {
                PutUintBE(_chgTimeSKey);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x0136);
            {
                PutUintBE(_chgTimeVKey);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x0143);
            {
                PutUintBE(_chgTimeD2);
            }
            PutUshortBE(0x0000);

            PutUshortBE(0x0164);
            {
                PutUintBE(_chgTimeSid);
            }
            PutUshortBE(0x0000);
        }

        public T138Body(byte[] data)
            : base(data)
        {
            //0x0106 unknown
            //0x010A a2
            //0x011C lskey
            //0x0102 a8
            //0x0103 stweb
            //0x0120 skey
            //0x0136 vkey
            //0x0143 d2
            //0x0164 sid

            var count = TakeUintBE(out var _);

            for (int i = 0; i < count; ++i)
            {
                var key = TakeUshortBE(out var _);

                switch (key)
                {
                    case 0x0106: TakeUintBE(out _chgTimeUnknown262); break;
                    case 0x010a: TakeUintBE(out _chgTimeA2); break;
                    case 0x011c: TakeUintBE(out _chgTimeLsKey); break;
                    case 0x0102: TakeUintBE(out _chgTimeA8); break;
                    case 0x0103: TakeUintBE(out _chgTimeStWeb); break;
                    case 0x0120: TakeUintBE(out _chgTimeSKey); break;
                    case 0x0136: TakeUintBE(out _chgTimeVKey); break;
                    case 0x0143: TakeUintBE(out _chgTimeD2); break;
                    case 0x0164: TakeUintBE(out _chgTimeSid); break;
                }

                TakeUintBE(out var _);
            }
        }
    }
}
