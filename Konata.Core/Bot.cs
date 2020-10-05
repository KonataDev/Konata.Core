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
                case EventType.WtLoginSendSms: OnRefreshSms(e); break;
                case EventType.WtLoginVerifySliderCaptcha: OnVerifySliderCaptcha(e); break;
                case EventType.WtLoginVerifySmsCaptcha: OnVerifySmsCaptcha(e); break;
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
            // _msfCore.DoHeartBeat();
        }

        private void OnVerifySliderCaptcha(Event e)
        {
            if (e._args == null
                || e._args.Length != 2
                || !(e._args[0] is string)
                || !(e._args[1] is string))
            {
                return;
            }

            _msfCore.WtLoginCheckSlider((string)e._args[0], (string)e._args[1]);
        }

        private void OnVerifySmsCaptcha(Event e)
        {
            if (e._args == null
                || e._args.Length != 2
                || !(e._args[0] is string)
                || !(e._args[1] is byte[])
                || !(e._args[2] is string))
            {
                return;
            }

            _msfCore.WtLoginCheckSms((string)e._args[0], (byte[])e._args[1], (string)e._args[2]);
        }

        private void OnRefreshSms(Event e)
        {
            if (e._args == null
                || e._args.Length != 2
                || !(e._args[0] is string)
                || !(e._args[0] is byte[]))
            {
                return;
            }

            _msfCore.WtLoginRefreshSms((string)e._args[0], (byte[])e._args[1]);
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
        /// <param name="sigSission"></param>
        /// <param name="sigTicket"></param>
        public void SubmitSliderTicket(string sigSission, string sigTicket)
        {
            PostEvent(EventFilter.System, EventType.WtLoginVerifySliderCaptcha,
                sigSission, sigTicket);
        }

        /// <summary>
        /// 提交SMS驗證碼
        /// </summary>
        /// <param name="sigSission"></param>
        /// <param name="sigSecret"></param>
        /// <param name="sigSmsCode"></param>
        public void SubmitSmsCode(string sigSission, byte[] sigSecret, string sigSmsCode)
        {
            PostEvent(EventFilter.System, EventType.WtLoginVerifySmsCaptcha,
               sigSission, sigSecret, sigSmsCode);
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
