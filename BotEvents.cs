using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;

// ReSharper disable EventNeverSubscribedTo.Global

namespace Konata.Core
{
    public partial class Bot
    {
        public event EventHandler<LogEvent> OnLog;
        public event EventHandler<CaptchaEvent> OnCaptcha;
        public event EventHandler<OnlineStatusEvent> OnOnlineStatusChanged;
        public event EventHandler<GroupMessageEvent> OnGroupMessage;
        public event EventHandler<GroupMuteMemberEvent> OnGroupMute;
        public event EventHandler<PrivateMessageEvent> OnPrivateMessage;
        public event EventHandler<GroupMessageRecallEvent> OnGroupMessageRecall;
        public event EventHandler<GroupPokeEvent> OnGroupPoke;
        public event EventHandler<GroupSettingsAnonymousEvent> OnGroupSettingsAnonymous;

        //public event EventHandler<PrivatePokenEvent> OnPrivatePoke;

        private Dictionary<Type, Action<BaseEvent>> _dict;

        /// <summary>
        /// Handlers initialization
        /// </summary>
        private void InitializeHandlers()
            => _dict = new()
            {
                {typeof(LogEvent), e => OnLog?.Invoke(this, (LogEvent) e)},
                {typeof(CaptchaEvent), e => OnCaptcha?.Invoke(this, (CaptchaEvent) e)},
                {typeof(OnlineStatusEvent), e => OnOnlineStatusChanged?.Invoke(this, (OnlineStatusEvent) e)},
                {typeof(GroupMessageEvent), e => OnGroupMessage?.Invoke(this, (GroupMessageEvent) e)},
                {typeof(PrivateMessageEvent), e => OnPrivateMessage?.Invoke(this, (PrivateMessageEvent) e)},
                {typeof(GroupMuteMemberEvent), e => OnGroupMute?.Invoke(this, (GroupMuteMemberEvent) e)},
                {typeof(GroupPokeEvent), e => OnGroupPoke?.Invoke(this, (GroupPokeEvent) e)},
                {typeof(GroupMessageRecallEvent), e => OnGroupMessageRecall?.Invoke(this, (GroupMessageRecallEvent) e)},
                {typeof(GroupSettingsAnonymousEvent), e => OnGroupSettingsAnonymous?.Invoke(this, (GroupSettingsAnonymousEvent) e)},
            };

        /// <summary>
        /// Post event to userend
        /// </summary>
        /// <param name="anyEvent"></param>
        public override void PostEventToEntity(BaseEvent anyEvent)
            => _dict[anyEvent.GetType()].Invoke(anyEvent);
    }
}
