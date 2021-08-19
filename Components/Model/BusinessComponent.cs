using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Utils;
using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Logics;
using Konata.Core.Logics.Model;

// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Model
{
    [Component("BusinessComponent", "Konata Business Component")]
    internal class BusinessComponent : InternalComponent
    {
        private const string TAG = "BusinessComponent";
        private readonly Dictionary<Type, List<BaseLogic>> _businessLogics;

        public BusinessComponent()
        {
            _businessLogics = new();

            // Load all business logics
            Reflection.EnumAttributes<BusinessLogicAttribute>((type, _) =>
            {
                // Event to subscribe 
                var events = type.GetCustomAttributes<EventSubscribeAttribute>();

                // Logic instance
                var constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                var instance = (BaseLogic) constructor[0].Invoke(new object[] {this});

                // Bind logic withevents
                foreach (var i in events)
                {
                    // Create the key
                    if (!_businessLogics.TryGetValue(i.Event, out var list))
                    {
                        list = new();
                        _businessLogics.Add(i.Event, list);
                    }

                    // Append logics
                    list.Add(instance);
                }

                // Save the cache
                switch (instance)
                {
                    case MessagingLogic messaging:
                        Messaging = messaging;
                        break;

                    case OperationLogic operation:
                        Operation = operation;
                        break;

                    case WtExchangeLogic wtxchg:
                        WtExchange = wtxchg;
                        break;

                    case CacheSyncLogic cache:
                        CacheSync = cache;
                        break;
                }
            });
        }

        /// <summary>
        /// Business logics
        /// </summary>
        /// <param name="task"></param>
        internal override void EventHandler(KonataTask task)
        {
            if (task.EventPayload is ProtocolEvent protocolEvent)
            {
                // Handle event
                if (_businessLogics.TryGetValue
                    (protocolEvent.GetType(), out var logics))
                {
                    foreach (var i in logics)
                    {
                        try
                        {
                            // Execute a business logic
                            i.Incoming(protocolEvent);
                        }
                        catch (Exception e)
                        {
                            LogE(TAG, $"The logic '{i.GetType()}'" +
                                      " was thrown an exception:");
                            LogE(TAG, e);
                        }
                    }
                }

                // No handler
                else
                {
                    LogW(TAG, "The event has no logic to handle.");
                }
            }
        }

        #region Business Logics

        private WtExchangeLogic WtExchange { get; set; }

        private OperationLogic Operation { get; set; }

        private MessagingLogic Messaging { get; set; }

        private CacheSyncLogic CacheSync { get; set; }

        public Task<bool> Login()
            => WtExchange.Login();

        public Task<bool> Logout()
            => WtExchange.Logout();

        public void SubmitSmsCode(string code)
            => WtExchange.SubmitSmsCode(code);

        public void SubmitSliderTicket(string ticket)
            => WtExchange.SubmitSliderTicket(ticket);

        public Task<bool> SetOnlineStatus(OnlineStatusEvent.Type status)
            => WtExchange.SetOnlineStatus(status);

        public OnlineStatusEvent.Type GetOnlineStatus()
            => WtExchange.OnlineType;

        public Task<int> GroupKickMember(uint groupUin, uint memberUin, bool preventRequest)
            => Operation.GroupKickMember(groupUin, memberUin, preventRequest);

        public Task<int> GroupMuteMember(uint groupUin, uint memberUin, uint timeSeconds)
            => Operation.GroupMuteMember(groupUin, memberUin, timeSeconds);

        public Task<int> GroupPromoteAdmin(uint groupUin, uint memberUin, bool toggleAdmin)
            => Operation.GroupPromoteAdmin(groupUin, memberUin, toggleAdmin);

        public Task<int> SendGroupMessage(uint groupUin, MessageChain message)
            => Messaging.SendGroupMessage(groupUin, message);

        public Task<int> SendPrivateMessage(uint friendUin, MessageChain message)
            => Messaging.SendPrivateMessage(friendUin, message);

        internal void SyncGroupList()
            => CacheSync.SyncGroupList();

        internal Task<bool> SyncGroupMemberList(uint groupUin)
            => CacheSync.SyncGroupMemberList(groupUin);

        internal Task<bool> SyncFriendList()
            => CacheSync.SyncFriendList();

        #endregion
    }
}
