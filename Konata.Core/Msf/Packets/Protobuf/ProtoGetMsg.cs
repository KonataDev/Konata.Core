using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    public class ProtoGetMsg : ProtoTreeRoot
    {
        public ProtoGetMsg(byte[] syncCookie)
        {
            addLeafVar("08", 0);                          // sync_flag
            addLeafBytes("12", syncCookie);               // sync_cookie
            addLeafVar("18", 0);                          // ramble_flag
            addLeafVar("20", 20);                         // latest_ramble_number
            addLeafVar("28", 3);                          // other_ramble_number
            addLeafVar("30", 1);                          // online_sync_flag
            addLeafVar("38", 1);                          // context_flag
            // addLeafVar("40", 0);                       // whisper_session_id
            addLeafVar("48", 0);                          // msg_req_type
            // addLeafBytes("52", null);                  // pubaccount_cookie
            // addLeafBytes("5A", null);                  // msg_ctrl_buf
            addLeafBytes("62", null);                     // bytes_server_buf
        }
    }
}
