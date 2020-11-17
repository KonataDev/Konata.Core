using System;
using System.Threading;
using System.Threading.Tasks;
using Konata.Events;

namespace Konata.Test.Tests
{
    class TestEventPumper : Test
    {
        private Pumper pumper;

        public override bool Run()
        {
            pumper = new Pumper();
            var thread = new Thread(pumper.Run);

            thread.Start();
            Thread.Sleep(100);

            Thread[] task = new Thread[1];
            for (int i = 0; i < task.Length; ++i)
            {
                task[i] = new Thread(pumper.StartFlood);
                task[i].Start();
            }

            Thread.Sleep(5000);
            var seq = pumper.GetComponent<Component>().GetSequence();

            pumper.Exit();
            for (int i = 0; i < task.Length; ++i)
            {
                task[i].Abort();
            }

            Print($"Performance: {seq / 5}/sec.");

            return true;
        }

        private class Pumper : EventPumper
        {
            private static readonly EventTest test
                = new EventTest { };

            public Pumper()
                : base()
            {
                RegisterComponent(new Component(this));
            }

            public void StartFlood()
            {
                for (int i = 0; i < 1000_0000; ++i)
                {
                    PostEvent<Component>(test);
                }
            }
        }

        private class Component : EventComponent
        {
            public Component(EventPumper eventPumper)
                : base(eventPumper)
            {

            }

            private int sequence;

            public override EventParacel OnEvent(EventParacel eventParacel)
            {
                Interlocked.Add(ref sequence, 1);
                return EventParacel.Accept;
            }

            public int GetSequence()
                => sequence;
        }

        private class EventTest : EventParacel { };
    }
}
