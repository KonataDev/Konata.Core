using System.Collections.Concurrent;
using Konata.Core.Attributes;

// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Model
{
    [Component("ConfigComponent", "Konata Config Management Component")]
    internal class ConfigComponent : InternalComponent
    {
        public BotKeyStore KeyStore { get; private set; }

        public BotDevice DeviceInfo { get; private set; }

        public BotConfig GlobalConfig { get; private set; }

        /// <summary>
        /// Sync cookie
        /// </summary>
        public byte[] SyncCookie
        {
            get => KeyStore.Account.SyncCookie;
            internal set => KeyStore.Account.SyncCookie = value;
        }

        private ConcurrentDictionary<uint, BotGroup> _groupList;
        private ConcurrentDictionary<uint, BotMember> _friendList;

        public ConfigComponent()
        {
            _groupList = new();
            _friendList = new();
        }

        public void LoadKeyStore(BotKeyStore keyStore, string imei)
        {
            KeyStore = keyStore;
            KeyStore.Initial(imei);
        }

        public void LoadConfig(BotConfig config)
            => GlobalConfig = config;

        public void LoadDeviceInfo(BotDevice device)
            => DeviceInfo = device;

        /// <summary>
        /// Get member information
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin</param>
        /// <param name="memberUin"><b>[In]</b> Member uin</param>
        /// <param name="member"><b>[Out]</b> Member information</param>
        public bool TryGetMemberInfo(uint groupUin,
            uint memberUin, out BotMember member)
        {
            // Get member information
            member = null;
            return _groupList.TryGetValue(groupUin, out var group)
                   && group.Members.TryGetValue(memberUin, out member);
        }

        /// <summary>
        /// Get group information
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin</param>
        /// <param name="group"><b>[Out]</b> Group information</param>
        /// <returns></returns>
        public bool TryGetGroupInfo(uint groupUin, out BotGroup group)
        {
            return _groupList.TryGetValue(groupUin, out group);
        }

        /// <summary>
        /// Get friend information
        /// </summary>
        /// <param name="friendUin"></param>
        /// <returns></returns>
        public BotMember GetFriendInfo(uint friendUin)
        {
            return _friendList.TryGetValue(friendUin, out var friend) ? friend : null;
        }

        /// <summary>
        /// Add or update group
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        public BotGroup TouchGroupInfo(BotGroup groupInfo)
        {
            // Touch if the group does not exist 
            if (!_groupList.ContainsKey(groupInfo.Uin))
            {
                // Add the group
                _groupList.TryAdd(groupInfo.Uin, groupInfo);
            }

            else
            {
                // Update group information
                _groupList[groupInfo.Uin] = groupInfo;
            }

            return groupInfo;
        }

        /// <summary>
        /// Add or update group information
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin</param>
        /// <param name="groupName"><b>[In]</b> Group name</param>
        public BotGroup TouchGroupInfo(uint groupUin, string groupName = null)
        {
            // Touch if the group does not exist 
            if (!_groupList.TryGetValue(groupUin, out var group))
            {
                group = new BotGroup
                {
                    Uin = groupUin,
                    Name = groupName,
                };

                // Touch the group
                _groupList.TryAdd(groupUin, group);
            }

            else
            {
                // Update group name
                if (groupName?.Length > 0
                    && groupName != group.Name)
                {
                    group.Name = groupName;
                }
            }

            return group;
        }

        /// <summary>
        /// Touch group member info
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin</param>
        /// <param name="memberUin"><b>[In]</b> Member uin</param>
        /// <param name="memberCardName"><b>[In]</b> Member card name</param>
        public BotMember TouchGroupMemberInfo(uint groupUin,
            uint memberUin, string memberCardName)
        {
            // Touch the group first
            var group = TouchGroupInfo(groupUin);

            // Update cache
            if (!group.Members.TryGetValue
                (memberUin, out var member))
            {
                member = new BotMember
                {
                    Uin = memberUin,
                    Name = memberCardName,
                    NickName = memberCardName,
                };

                // Touch the member
                group.Members.Add(memberUin, member);
            }

            else
            {
                // Update member card name
                if (memberCardName.Length > 0
                    && memberCardName != group.Name)
                {
                    member.NickName = memberCardName;
                }
            }

            return member;
        }
    }
}
