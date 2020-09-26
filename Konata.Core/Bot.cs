using System;
using System.Collections.Generic;
using System.Threading;
using Konata.Msf;

namespace Konata
{
    using EventMutex = Mutex;
    using EventQueue = Queue<Event>;
    using EventWorkers = ThreadPool;

    public class Bot
    {
        public delegate bool EventProc(EventType e, params object[] a);

        private Core _msfCore;

        private bool _botIsExit;
        private Thread _botThread;

        private EventProc _eventProc;
        private EventQueue _eventQueue;
        private EventMutex _eventLock;

        public Bot(uint uin, string password)
        {
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
            if (_eventProc == null)
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
            // 啓動時投遞訊息
            PostEvent(EventType.BotStart);

            // 進入事件循環
            while (!_botIsExit)
            {
                Event coreEvent;
                if (!GetEvent(out coreEvent) || coreEvent._type == EventType.Idle)
                {
                    Thread.Sleep(0);
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
                case EventType.Login: OnLogin(e); break;
                case EventType.HeartBeat: OnHeartBeat(e); break;
            }
        }

        private void OnLogin(Event e)
        {
            _msfCore.WtLoginTgtgt();
        }

        private void OnHeartBeat(Event e)
        {
            // _msfCore.DoHeartBeat();
        }

        #endregion

        #region Protocol Interoperation Methods

        public void Login()
        {
            PostEvent(EventType.Login);
            // return _msfCore.Connect() && _msfCore.DoLogin();
        }

        public void SubmitSliderTicket(string sigSission, string sigTicket)
        {
            PostEvent(EventType.VerifySliderCaptcha, sigSission, sigTicket);
        }

        #endregion

        #region Event Methods

        public void PostEvent(EventType type)
        {
            PostSystemEvent(type, null);
        }

        public void PostEvent(EventType type, params object[] args)
        {
            PostSystemEvent(type, args);
        }

        internal void PostEvent(EventFilter filter, EventType type,
            params object[] args)
        {
            PostEvent(new Event(filter, type, args));
        }

        internal void PostEvent(Event e)
        {
            _eventLock.WaitOne();
            {
                _eventQueue.Enqueue(e);
            }
            _eventLock.ReleaseMutex();
        }

        internal void PostSystemEvent(EventType type, params object[] args)
        {
            PostEvent(new Event(EventFilter.System, type, args));
        }

        internal void PostUserEvent(EventType type, params object[] args)
        {
            PostEvent(new Event(EventFilter.User, type, args));
        }

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
