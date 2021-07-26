using System;

using Konata.Core.Events;
using Konata.Core.Events.Model;

namespace Konata.Core
{
    public partial class Bot
    {
        public event EventHandler<CaptchaEvent> OnCaptcha;
        public event EventHandler<OnlineStatusEvent> OnOnlineStatusChanged;
        public event EventHandler<GroupMessageEvent> OnGroupMessage;
        public event EventHandler<GroupMuteMemberEvent> OnGroupMute;
        public event EventHandler<PrivateMessageEvent> OnPrivateMessage;
        public event EventHandler<GroupPokeEvent> OnGroupPoke;
        //public event EventHandler<PrivatePokenEvent> OnPrivatePoke;
        public event EventHandler<LogEvent> OnLog;

        public override void PostEventToEntity(BaseEvent anyEvent)
        {
            switch (anyEvent)
            {
                case CaptchaEvent ce:
                    OnCaptcha?.Invoke(this, ce);
                    break;

                case OnlineStatusEvent ose:
                    OnOnlineStatusChanged?.Invoke(this, ose);
                    break;

                case GroupMessageEvent gme:
                    OnGroupMessage?.Invoke(this, gme);
                    break;

                case PrivateMessageEvent pme:
                    OnPrivateMessage?.Invoke(this, pme);
                    break;

                case GroupMuteMemberEvent gmme:
                    OnGroupMute?.Invoke(this, gmme);
                    break;

                case GroupPokeEvent gpe:
                    OnGroupPoke?.Invoke(this, gpe);
                    break;

                //case PrivatePokeEvent ppe:
                //    OnPrivatePoke?.Invoke(this, ppe);
                //    break;

                case LogEvent le:
                    OnLog?.Invoke(this, le);
                    break;
            }
        }
    }
}
