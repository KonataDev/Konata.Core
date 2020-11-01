using System;
using System.Threading;
using System.Collections.Generic;
using Konata.Msf;

namespace Konata
{
    using EventMutex = Mutex;
    using EventQueue = Queue<Event>;
    using EventWorkers = ThreadPool;

    public class Bot
    {
        private bool isExit;
        private Core msfCore;

        private EventProc eventProc;
        private EventQueue eventQueue;
        private EventMutex eventLock;

        public delegate bool EventProc(EventType e, params object[] a);

        public Bot(uint uin, string password)
        {
            isExit = false;

            eventLock = new EventMutex();
            eventQueue = new EventQueue();

            msfCore = new Core(this, uin, password);
        }

        /// <summary>
        /// 注冊事件委托回調方法
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool RegisterDelegate(EventProc callback)
        {
            if (eventProc != null)
            {
                return false;
            }

            eventProc = callback;
            return true;
        }

        /// <summary>
        /// 開始運行。此方法會阻塞綫程
        /// </summary>
        public void Run()
        {
            if (isExit)
            {
                return;
            }

            // 啓動時投遞訊息
            PostEvent(EventFilter.User, EventType.BotStart);

            // 進入事件循環
            isExit = false;
            while (!isExit)
            {
                Event coreEvent;
                if (!GetEvent(out coreEvent) || coreEvent.type == EventType.Idle)
                {
                    Thread.Sleep(1);
                    continue;
                }

                // 處理事件
                EventWorkers.QueueUserWorkItem(ProcessEvent, coreEvent);
            }
        }

        #region In-System Event Handlers 

        private void ProcessEvent(object o)
        {
            var e = (Event)o;

            if (e.filter == EventFilter.System)
            {
                OnSystemEvent(e);
                return;
            }

            // 將事件交給前端
            OnUserEvent(e);
        }

        private void OnUserEvent(Event e)
        {
            eventProc(e.type, e.args);
        }

        private void OnSystemEvent(Event e)
        {
            switch (e.type)
            {
                case EventType.WtLogin: OnLogin(e); break;
                case EventType.LoginFailed: OnLoginFailed(e); break;
                case EventType.HeartBeat: OnHeartBeat(e); break;
                case EventType.WtLoginSendSms: OnWtLoginRefreshSms(e); break;
                case EventType.WtLoginVerifySliderCaptcha: OnWtLoginVerifySliderCaptcha(e); break;
                case EventType.WtLoginVerifySmsCaptcha: OnWtLoginVerifySmsCaptcha(e); break;
                case EventType.WtLoginOK: OnWtLoginSuccess(e); break;
            }
        }

        private void OnLogin(Event e)
        {
            msfCore.Connect();
            msfCore.WtLoginTgtgt();
        }

        private void OnLoginFailed(Event e)
        {
            msfCore.DisConnect();
        }

        private void OnHeartBeat(Event e)
        {
            msfCore.Heartbeat_Alive();
        }

        private void OnWtLoginVerifySliderCaptcha(Event e)
        {
            if (e.args == null
                || e.args.Length != 1
                || !(e.args[0] is string))
            {
                return;
            }

            msfCore.WtLoginCheckSlider((string)e.args[0]);
        }

        private void OnWtLoginVerifySmsCaptcha(Event e)
        {
            if (e.args == null
                || e.args.Length != 1
                || !(e.args[0] is string))
            {
                return;
            }

            msfCore.WtLoginCheckSms((string)e.args[0]);
        }

        private void OnWtLoginRefreshSms(Event e)
        {
            if (e.args != null)
            {
                return;
            }

            msfCore.WtLoginRefreshSms();
        }

        private void OnWtLoginSuccess(Event e)
        {
            if (e.args != null)
            {
                return;
            }

            msfCore.StatSvc_RegisterClient();
        }

        #endregion

        #region Protocol Interoperation Methods

        /// <summary>
        /// 執行登錄
        /// </summary>
        public void Login()
        {
            PostEvent(EventFilter.System, EventType.WtLogin);
        }

        /// <summary>
        /// 提交滑塊驗證碼
        /// </summary>
        /// <param name="ticket"></param>
        public void SubmitSliderTicket(string ticket)
        {
            PostEvent(EventFilter.System, EventType.WtLoginVerifySliderCaptcha, ticket);
        }

        /// <summary>
        /// 提交SMS驗證碼
        /// </summary>
        /// <param name="smsCode"></param>
        public void SubmitSmsCode(string smsCode)
        {
            PostEvent(EventFilter.System, EventType.WtLoginVerifySmsCaptcha, smsCode);
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// 投遞事件
        /// </summary>
        /// <param name="e"></param>
        internal void PostEvent(Event e)
        {
            eventLock.WaitOne();
            {
                eventQueue.Enqueue(e);
            }
            eventLock.ReleaseMutex();
        }

        /// <summary>
        /// 投遞事件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="type"></param>
        /// <param name="args"></param>
        internal void PostEvent(EventFilter filter, EventType type,
            params object[] args)
        {
            PostEvent(new Event(filter, type, args));
        }

        /// <summary>
        /// 投遞系統事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        internal void PostSystemEvent(EventType type, params object[] args)
        {
            PostEvent(new Event(EventFilter.System, type, args));
        }

        /// <summary>
        /// 投遞用戶事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        internal void PostUserEvent(EventType type, params object[] args)
        {
            PostEvent(new Event(EventFilter.User, type, args));
        }

        /// <summary>
        /// 從隊列獲取一個事件
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool GetEvent(out Event e)
        {
            eventLock.WaitOne();
            {
                if (eventQueue.Count <= 0)
                {
                    eventLock.ReleaseMutex();

                    e = Event.Idle;
                    return false;
                }

                e = eventQueue.Dequeue();
            }
            eventLock.ReleaseMutex();

            return true;
        }

        #endregion
    }
}
