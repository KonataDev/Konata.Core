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
        private bool _isExit;
        private Core _msfCore;

        private EventProc _eventProc;
        private EventQueue _eventQueue;
        private EventMutex _eventLock;

        public delegate bool EventProc(EventType e, params object[] a);

        public Bot(uint uin, string password)
        {
            _isExit = false;

            _eventLock = new EventMutex();
            _eventQueue = new EventQueue();

            _msfCore = new Core(this, uin, password);
        }

        /// <summary>
        /// 注冊事件委托回調方法
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool RegisterDelegate(EventProc callback)
        {
            if (_eventProc != null)
            {
                return false;
            }

            _eventProc = callback;
            return true;
        }

        /// <summary>
        /// 開始運行。此方法會阻塞綫程
        /// </summary>
        public void Run()
        {
            if (_isExit)
            {
                return;
            }

            // 啓動時投遞訊息
            PostEvent(EventFilter.User, EventType.BotStart);

            // 進入事件循環
            _isExit = false;
            while (!_isExit)
            {
                Event coreEvent;
                if (!GetEvent(out coreEvent) || coreEvent._type == EventType.Idle)
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

            if (e._filter == EventFilter.System)
            {
                OnSystemEvent(e);
                return;
            }

            // 將事件交給前端
            OnUserEvent(e);
        }

        private void OnUserEvent(Event e)
        {
            _eventProc(e._type, e._args);
        }

        private void OnSystemEvent(Event e)
        {
            switch (e._type)
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
            _msfCore.Connect();
            _msfCore.WtLoginTgtgt();
        }

        private void OnLoginFailed(Event e)
        {
            _msfCore.DisConnect();
        }

        private void OnHeartBeat(Event e)
        {
            _msfCore.Heartbeat_Alive();
        }

        private void OnWtLoginVerifySliderCaptcha(Event e)
        {
            if (e._args == null
                || e._args.Length != 1
                || !(e._args[0] is string))
            {
                return;
            }

            _msfCore.WtLoginCheckSlider((string)e._args[0]);
        }

        private void OnWtLoginVerifySmsCaptcha(Event e)
        {
            if (e._args == null
                || e._args.Length != 1
                || !(e._args[0] is string))
            {
                return;
            }

            _msfCore.WtLoginCheckSms((string)e._args[0]);
        }

        private void OnWtLoginRefreshSms(Event e)
        {
            if (e._args != null)
            {
                return;
            }

            _msfCore.WtLoginRefreshSms();
        }

        private void OnWtLoginSuccess(Event e)
        {
            if (e._args != null)
            {
                return;
            }

            _msfCore.StatSvc_RegisterClient();
            _msfCore.OidbSvc_0xdc9();
            _msfCore.OidbSvc_0x480_9();
            _msfCore.OidbSvc_0x5eb_22();
            _msfCore.OidbSvc_0x5eb_15();
            _msfCore.OidbSvc_oidb_0xd82();
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
            _eventLock.WaitOne();
            {
                _eventQueue.Enqueue(e);
            }
            _eventLock.ReleaseMutex();
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
            _eventLock.WaitOne();
            {
                if (_eventQueue.Count <= 0)
                {
                    _eventLock.ReleaseMutex();

                    e = Event.Idle;
                    return false;
                }

                e = _eventQueue.Dequeue();
            }
            _eventLock.ReleaseMutex();

            return true;
        }

        #endregion
    }
}
