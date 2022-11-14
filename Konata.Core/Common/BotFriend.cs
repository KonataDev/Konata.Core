// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Common
{
    /// <summary>
    /// Bot friend definitions
    /// </summary>
    public class BotFriend
    {
        /// <summary>
        /// Friend uin
        /// </summary>
        public uint Uin { get; internal set; }

        /// <summary>
        /// Friend name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Friend remark
        /// </summary>
        public string Remark { get; internal set; }

        /// <summary>
        /// Friend face id
        /// </summary>
        public byte FaceId { get; internal set; }

        /// <summary>
        /// Friend gender
        /// </summary>
        public byte Gender { get; internal set; }

        /// <summary>
        /// Friend Avatar Url
        /// </summary>
        public string AvatarUrl
            => "https://q1.qlogo.cn/g?b=qq&nk={uin}&s=0".Replace("{uin}", Uin.ToString());

        internal BotFriend()
        {
            Name = "";
            Remark = "";
        }
    }
}
