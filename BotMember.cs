namespace Konata.Core
{
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
        /// Member gender
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Member birth
        /// </summary>
        public int Brith { get; set; }

        public BotMember()
        {
            Name = "";
            NickName = "";
        }
    }
}
