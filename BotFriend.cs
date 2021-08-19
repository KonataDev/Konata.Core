namespace Konata.Core
{
    public class BotFriend
    {
        /// <summary>
        /// Friend uin
        /// </summary>
        public uint Uin { get; set; }

        /// <summary>
        /// Friend name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Friend remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Friend faceid
        /// </summary>
        public byte FaceId { get; set; }

        /// <summary>
        /// Friend gender
        /// </summary>
        public byte Gender { get; set; }

        public BotFriend()
        {
            Name = "";
            Remark = "";
        }
    }
}
