using System.Collections.Generic;
using Konata.Core.Message.Model;

namespace Konata.Core.Events.Model
{
    public class GroupPicUpEvent : ProtocolEvent
    {
        /// <summary>s
        /// <b>[In]</b> <br/>
        /// Group uin <br/>
        /// </summary>
        public uint GroupUin { get; }

        /// <summary>s
        /// <b>[In]</b> <br/>
        /// Self uin <br/>
        /// </summary>
        public uint SelfUin { get; }

        /// <summary>s
        /// <b>[In]</b> <br/>
        /// Image to upload <br/>
        /// </summary>
        public List<ImageChain> UploadImages { get; }

        /// <summary>
        /// <b>[In] [Out]</b> <br/>
        /// Image upload info <br/>
        /// </summary>
        public List<PicUpInfo> UploadInfo { get; }

        private GroupPicUpEvent(uint groupUin, uint selfUin,
            List<ImageChain> uploadImages) : base(6000, true)
        {
            GroupUin = groupUin;
            SelfUin = selfUin;
            UploadImages = uploadImages;
        }

        private GroupPicUpEvent(int resultCode,
            List<PicUpInfo> uploadInfo) : base(resultCode)
        {
            UploadInfo = uploadInfo;
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="selfUin"></param>
        /// <param name="uploadImages"></param>
        /// <returns></returns>
        internal static GroupPicUpEvent Create(uint groupUin, uint selfUin,
            List<ImageChain> uploadImages) => new(groupUin, selfUin, uploadImages);

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="uploadInfo"></param>
        /// <returns></returns>
        internal static GroupPicUpEvent Result(int resultCode,
            List<PicUpInfo> uploadInfo) => new(resultCode, uploadInfo);
    }

    public class PicUpInfo
    {
        public uint Ip { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public uint UploadId { get; set; }

        public byte[] UploadTicket { get; set; }

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
