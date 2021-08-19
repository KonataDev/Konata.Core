using System;
using System.Collections.Generic;

using Konata.Core.Message.Model;

namespace Konata.Core.Events.Model
{
    public class GroupPicUpEvent : ProtocolEvent
    {
        public uint SelfUin { get; set; }

        public uint GroupUin { get; set; }

        public List<ImageChain> Images { get; set; }

        /// <summary>
        /// Image upload info
        /// </summary>
        public List<PicUpInfo> UploadInfo { get; set; }

        public GroupPicUpEvent()
            => WaitForResponse = true;
    }

    public class PicUpInfo
    {
        public uint Ip { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public uint ImageId { get; set; }

        public byte[] ServiceTicket { get; set; }

        public bool UseCached { get; set; }

        public CachedPicInfo CachedInfo { get; set; }
    }

    public class CachedPicInfo
    {
        public ImageType Type { get; set; }

        public byte[] Hash { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public uint Length { get; set; }
    }
}
