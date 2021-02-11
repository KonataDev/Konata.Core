using System;
using Konata.Utils.Protobuf;

namespace Konata.Core.Packet.Protobuf
{
    public class PrivateMsgPullRequest : ProtoTreeRoot
    {
        public PrivateMsgPullRequest(byte[] syncCookie)
        {
            AddLeafVar("08", 0);                          // sync_flag
            AddLeafBytes("12", syncCookie);               // sync_cookie
            AddLeafVar("18", 0);                          // ramble_flag
            AddLeafVar("20", 20);                         // latest_ramble_number
            AddLeafVar("28", 3);                          // other_ramble_number
            AddLeafVar("30", 1);                          // online_sync_flag
            AddLeafVar("38", 1);                          // context_flag
            // addLeafVar("40", 0);                       // whisper_session_id
            AddLeafVar("48", 0);                          // msg_req_type
            // addLeafBytes("52", new byte[0]);           // pubaccount_cookie
            // addLeafBytes("5A", new byte[0]);           // msg_ctrl_buf
            AddLeafBytes("62", new byte[0]);              // bytes_server_buf
        }
    }
}
