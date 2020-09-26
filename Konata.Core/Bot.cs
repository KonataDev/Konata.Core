using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Konata.Msf;

namespace Konata
{
    using EventQueue = Queue<Event>;
    using EventMutex = Mutex;

    public class Bot
    {
        public delegate bool EventProc(Event e, params object[] a);

        private Core _msfCore;

        private bool _botIsExit;
        private Thread _botThread;

        private EventProc _eventProc;
        private EventQueue _eventQueue;
        private EventMutex _eventLock;

        public Bot(uint uin, string password)
        {
            _eventLock = new Mutex();
            _eventQueue = new EventQueue();

            _msfCore = new Core(uin, password);

            //_botThread = new Thread(BotThread);
            //_botThread.Start();
        }

        public bool Login()
        {
            PostEvent(Event.DoLogin);
            // return _msfCore.Connect() && _msfCore.DoLogin();
        }

        ///
        public bool RegisterDelegate(EventProc callback)
        {
            if (_eventProc == null)
            {
                return false;
            }

            _eventProc = callback;
            return true;
        }

        ///
        private void Run()
        {
            PostEvent(Event.OnBotStart);

            while (!_botIsExit)
            {
                Event coreEvent;

                if (!_msfcore.PullEvent(out coreEvent, out arguments)
                    || coreEvent == Event.Idle)
                {
                    Thread.Sleep(0);
                }

                EventProc(coreEvent);
            }
        }

        private void EventProc(Event eventId, params object[] args)
        {

        }

        public void PostEvent(EventType type)
        {
            PostEvent(type, null);
        }

        public void PostEvent(EventType type, params object[] args)
        {
            _eventLock.WaitOne();
            {
                _eventQueue.Enqueue(new Event(type, args));
            }
            _eventLock.ReleaseMutex();
        }

        private bool GetEvent(out EventType type, out object[] args)
        {
            type = EventType.Idle;
            args = null;

            _eventLock.WaitOne();
            {
                if (_eventQueue.Count <= 0)
                {
                    _eventLock.ReleaseMutex();
                    return false;
                }

                var item = _eventQueue.Dequeue();
                type = item._type;
                args = item._args;
            }
            _eventLock.ReleaseMutex();

            return true;
        }
    }
}
