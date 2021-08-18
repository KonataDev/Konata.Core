using System;
using System.Linq;
using System.Collections.Concurrent;
using Konata.Core.Attributes;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable MemberCanBePrivate.Global
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

        private readonly ConcurrentDictionary<uint, BotGroup> _groupList;
        private readonly ConcurrentDictionary<uint, BotMember> _friendList;

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
                _groupList[groupInfo.Uin].Name = groupInfo.Name;
                _groupList[groupInfo.Uin].MemberCount = groupInfo.MemberCount;
                _groupList[groupInfo.Uin].MaxMemberCount = groupInfo.MaxMemberCount;
                _groupList[groupInfo.Uin].Muted = groupInfo.Muted;
                _groupList[groupInfo.Uin].MutedMe = groupInfo.MutedMe;
                _groupList[groupInfo.Uin].LastUpdate = DateTime.Now;
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
        /// Add or update group member
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin</param>
        /// <param name="memberInfo">><b>[In]</b> Member info</param>
        /// <returns></returns>
        public BotMember TouchGroupMemberInfo(uint groupUin, BotMember memberInfo)
        {
            // Touch if the group does not exist 
            if (!TryGetMemberInfo(groupUin, memberInfo.Uin, out var member))
            {
                // Add the member
                _groupList[groupUin].Members
                    .Add(memberInfo.Uin, memberInfo);
            }

            else
            {
                // Update member information
                member.Age = memberInfo.Age;
                member.Name = memberInfo.Name;
                member.NickName = member.NickName;
                member.Gender = memberInfo.Gender;
                member.Level = member.Level;
                member.FaceId = member.FaceId;
                member.IsAdmin = member.IsAdmin;
                member.MuteTimestamp = member.MuteTimestamp;
                member.LastSpeakTime = member.LastSpeakTime;
                member.SpecialTitle = member.SpecialTitle;
                member.SpecialTitleExpiredTime = member.SpecialTitleExpiredTime;
            }

            return memberInfo;
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

        /// <summary>
        /// Check if lacks the member cache
        /// </summary>
        /// <returns></returns>
        public bool IsLackMemberCacheForGroup(uint groupUin)
        {
            // Check the cache
            if (TryGetGroupInfo(groupUin, out var group))
            {
                return group.MemberCount != group.Members.Count;
            }

            // Group not existed
            return false;
        }

        /// <summary>
        /// Get group uin from a code
        /// </summary>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        public ulong GetGroupUin(uint groupCode)
            => _groupList.FirstOrDefault(x => x.Value.Code == groupCode).Key;

        /// <summary>
        /// Get group code from an uin
        /// </summary>
        /// <param name="groupUin"></param>
        /// <returns></returns>
        public ulong GetGroupCode(uint groupUin)
            => TryGetGroupInfo(groupUin, out var group) ? group.Code : 0;
    }
}
