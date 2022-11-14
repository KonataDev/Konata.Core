// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Common
{
    /// <summary>
    /// Bot member definitions
    /// </summary>
    public class BotMember
    {
        /// <summary>
        /// Member uin
        /// </summary>
        public uint Uin { get; set; }

        /// <summary>
        /// Member name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Member nick name
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Member special title
        /// </summary>
        public string SpecialTitle { get; set; }

        /// <summary>
        /// Member special title expird time
        /// </summary>
        public uint SpecialTitleExpiredTime { get; set; }

        /// <summary>
        /// Member age
        /// </summary>
        public uint Age { get; set; }

        /// <summary>
        /// Member face Id
        /// </summary>
        public byte FaceId { get; set; }

        /// <summary>
        /// Member gender
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// Member level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Member join time
        /// </summary>
        public uint JoinTime { get; set; }

        /// <summary>
        /// Member last speak time
        /// </summary>
        public uint LastSpeakTime { get; set; }

        /// <summary>
        /// Member role
        /// </summary>
        public RoleType Role { get; set; }

        /// <summary>
        /// Member Avatar Url
        /// </summary>
        public string AvatarUrl
            => "https://q1.qlogo.cn/g?b=qq&nk={uin}&s=0".Replace("{uin}", Uin.ToString());

        /// <summary>
        /// Member is admin (except owner)
        /// </summary>
        internal bool IsAdmin { get; set; }

        /// <summary>
        /// Mute timestamp
        /// </summary>
        public uint MuteTimestamp { get; set; }

        internal BotMember()
        {
            Name = "";
            NickName = "";
        }
    }

    /// <summary>
    /// Member role
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        ///  Normal member
        /// </summary>
        Member = 1,

        /// <summary>
        /// Administator
        /// </summary>
        Admin,

        /// <summary>
        /// Owner
        /// </summary>
        Owner
    }
}
