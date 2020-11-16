using System;

namespace Konata.Events
{
    public class EventAccountCtl : EventParacel
    {
        public enum EventType
        {
            GetTroopList,
            GetFriendList,
        }

        public EventType Type { get; set; }

        /// <summary>
        /// For EventType.GetTroopList and EventType.GetFriendList
        /// </summary>
        public uint SelfUin { get; set; }
    }
}
