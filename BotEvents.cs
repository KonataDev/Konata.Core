using System;
using System.Collections.Generic;
using System.Threading;
using Konata.Core.Events;
using Konata.Core.Events.Model;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable EventNeverSubscribedTo.Global

namespace Konata.Core
{
    public partial class Bot
    {
        /// <summary>
        /// Handle log event
        /// </summary>
        public event EventHandler<LogEvent> OnLog;

        /// <summary>
        /// Handle captcha event
        /// </summary>
        public event EventHandler<CaptchaEvent> OnCaptcha;

        /// <summary>
        /// On online status changed event
        /// </summary>
        public event EventHandler<OnlineStatusEvent> OnOnlineStatusChanged;

        /// <summary>
        /// On group message event
        /// </summary>
        public event EventHandler<GroupMessageEvent> OnGroupMessage;

        /// <summary>
        /// On private message event
        /// </summary>
        public event EventHandler<PrivateMessageEvent> OnPrivateMessage;

        /// <summary>
        /// On group mute event
        /// </summary>
        public event EventHandler<GroupMuteMemberEvent> OnGroupMute;

        /// <summary>
        /// On group recall message event
        /// </summary>
        public event EventHandler<GroupMessageRecallEvent> OnGroupMessageRecall;

        /// <summary>
        /// On group poke event
        /// </summary>
        public event EventHandler<GroupPokeEvent> OnGroupPoke;

        private Dictionary<Type, Action<BaseEvent>> _dict;

        /// <summary>
        /// Handlers initialization
        /// </summary>
        private void InitializeHandlers()
        {
            _dict = new()
            {
                {typeof(LogEvent), e => OnLog?.Invoke(this, (LogEvent) e)},
                {typeof(CaptchaEvent), e => OnCaptcha?.Invoke(this, (CaptchaEvent) e)},
                {typeof(OnlineStatusEvent), e => OnOnlineStatusChanged?.Invoke(this, (OnlineStatusEvent) e)},
                {typeof(GroupMessageEvent), e => OnGroupMessage?.Invoke(this, (GroupMessageEvent) e)},
                {typeof(PrivateMessageEvent), e => OnPrivateMessage?.Invoke(this, (PrivateMessageEvent) e)},
                {typeof(GroupMuteMemberEvent), e => OnGroupMute?.Invoke(this, (GroupMuteMemberEvent) e)},
                {typeof(GroupPokeEvent), e => OnGroupPoke?.Invoke(this, (GroupPokeEvent) e)},
                {typeof(GroupMessageRecallEvent), e => OnGroupMessageRecall?.Invoke(this, (GroupMessageRecallEvent) e)},
            };

            // Default group message handler
            OnGroupMessage += (sender, e) =>
            {
                OnLog?.Invoke(sender, LogEvent.Create("Bot",
                    LogLevel.Verbose, $"[Group]{e.GroupUin} " +
                                      $"[Member]{e.MemberUin} {e.Message}"));
            };

            // Default private message handler
            OnPrivateMessage += (sender, e) =>
            {
                OnLog?.Invoke(sender, LogEvent.Create("Bot",
                    LogLevel.Verbose, $"[Friend]{e.FriendUin} {e.Message}"));
            };

            // Default group mute handler
            OnGroupMute += (sender, e) =>
            {
                OnLog?.Invoke(sender, LogEvent.Create("Bot",
                    LogLevel.Verbose, $"[Mute]{e.GroupUin} " +
                                      $"[Operator]{e.OperatorUin} " +
                                      $"[Member]{e.MemberUin} " +
                                      $"[Time]{e.TimeSeconds} sec."));
            };

            // Default group poke handler
            OnGroupPoke += (sender, e) =>
            {
                OnLog?.Invoke(sender, LogEvent.Create("Bot",
                    LogLevel.Verbose, $"[Poke]{e.GroupUin} " +
                                      $"[Operator]{e.OperatorUin} " +
                                      $"[Member]{e.MemberUin}"));
            };

            // Default group recall handler
            OnGroupMessageRecall += (sender, e) =>
            {
                OnLog?.Invoke(sender, LogEvent.Create("Bot",
                    LogLevel.Verbose, $"[Recall]{e.GroupUin} " +
                                      $"[Messageid]{e.MessageId} " +
                                      $"[Member]{e.MemberUin}"));
            };
        }

        /// <summary>
        /// Post event to user end
        /// </summary>
        /// <param name="anyEvent"></param>
        public override void PostEventToEntity(BaseEvent anyEvent)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // Call user event
                    _dict[anyEvent.GetType()].Invoke(anyEvent);
                }
                catch (Exception e)
                {
                    // Suppress exceptions
                    OnLog?.Invoke(this, LogEvent.Create("Bot",
                        LogLevel.Exception, $"{e.StackTrace}\n{e.Message}"));
                }
            });
        }

        /// <summary>
        /// Retrieve the handler is registered
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        internal bool HandlerRegistered<TEvent>()
            where TEvent : BaseEvent => _dict[typeof(TEvent)] != null;
    }
}
