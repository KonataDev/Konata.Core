using System;
using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Common
{
    /// <summary>
    /// Bot group definitions
    /// </summary>
    public class BotGroup
    {
        /// <summary>
        /// Group uin
        /// </summary>
        public uint Uin { get; set; }

        /// <summary>
        /// Group code
        /// </summary>
        public ulong Code { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Group owner uin
        /// </summary>
        public uint OwnerUin { get; set; }

        /// <summary>
        /// Group admins
        /// </summary>
        public List<uint> AdminUins { get; }

        /// <summary>
        /// Group member count
        /// </summary>
        public uint MemberCount { get; set; }

        /// <summary>
        /// Group max member count
        /// </summary>
        public uint MaxMemberCount { get; set; }

        /// <summary>
        /// Group muted
        /// </summary>
        public uint Muted { get; set; }

        /// <summary>
        /// Muted me
        /// </summary>
        public uint MutedMe { get; set; }

        /// <summary>
        /// Group Avatar Url
        /// </summary>
        public string AvatarUrl
            => "https://p.qlogo.cn/gh/{uin}/{uin}/0/".Replace("{uin}", Uin.ToString());

        /// <summary>
        /// Last update time
        /// </summary>
        internal DateTime LastUpdate { get; set; }

        /// <summary>
        /// Group members
        /// </summary>
        internal Dictionary<uint, BotMember> Members { get; }

        internal BotGroup()
        {
            Name = "";
            Members = new();
            AdminUins = new();
        }
    }
}
