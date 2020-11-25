using Konata.Core.Builder;
using Konata.Core.MQ;
using Konata.Core.Extensions;
using Konata.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core;
using System.Diagnostics;
using Konata.Core.Base.Event;

namespace Konata.Console
{
    public class test
    {
        public void Handle(EventArgs arg)
        {
            System.Console.WriteLine($"I Received OnDataFixed Event!!!{arg}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            InjectManager.Instance.LoadNewAssembly(typeof(Program).Assembly.GetName().Name, typeof(Program).Assembly);

            test t = new test();

            Action<EventArgs> a = t.Handle;
            // event EventHandler ee+=(e)=>{t.Handle(e);};
            t = null;
            

            EventManager.Instance.RegisterListener("Konata.Console.OnDataFixed", (arg)=> { t.Handle(arg); }, false);
            List<string> events = EventManager.Instance.GetEventList();
            foreach (string e in events)
            {
                System.Console.WriteLine(e);
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long stime = sw.ElapsedMilliseconds;
            EventManager.Instance.RunEvent("DataComing", null,true);
            for(int i = 0; i < 1000; i++)
            {
                EventManager.Instance.RunEvent("Konata.Console.OnDataFixed", null);
            }
            
            long etime = sw.ElapsedMilliseconds;
            sw.Stop();
            System.Console.WriteLine($"Time uesd {etime - stime}ms");
            //t = null;
            //GC.Collect();
            //EventManager.Instance.RunEvent("Konata.Console.OnDataFixed", null);
            //IMQ<string> MQ = new MQBuilder<string>()
            //        .SetCustomMQ(typeof(KonataMemMQ<string>))
            //        .SetExternalTaskQueue(TaskQueue.DefaultConcurrentQueue)
            //        .MQConfig(config =>
            //        {
            //            config.MaxProcessMTask = 4;
            //        })
            //        .AddMQReceiver(async (data) =>
            //        {
            //            System.Console.WriteLine($"Service 1 received data {data}");
            //            System.Console.WriteLine("Service 1 Do Work");
            //            await Task.Delay(50);
            //        })
            //        .AddMQReceiver(async (data) =>
            //        {
            //            System.Console.WriteLine($"Service 2 received data {data}");
            //            System.Console.WriteLine("Service 2 Do Work");
            //            await Task.Delay(70);
            //        })
            //        .Build();
            //MQ.StartTakeProcess();

            //Task.Run(async () =>
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        MQ.Add($"A:this is {i} time added");
            //        await Task.Delay(100);
            //    }
            //});
            //Task.Run(async() =>
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        MQ.Add($"B:this is {i} time added");
            //        await Task.Delay(200);
            //    }
            //});
            //Task.Run(async () =>
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        MQ.Add($"C:this is {i} time added");
            //        await Task.Delay(300);
            //    }
            //});
            //Task.Run(async () =>
            //{
            //    await Task.Delay(1000);
            //    System.Console.WriteLine("StopMQ!");
            //    MQ.StopTakeProcess();
            //    await Task.Delay(2000);
            //    System.Console.WriteLine("ReStartMQ");
            //    MQ.StartTakeProcess();
            //});

            //Task.Run(async () =>
            //{
            //    for (int i = 0; i < 50; i++)
            //    {
            //        MQ.Add($"D:this is {i} time added");
            //        await Task.Delay(400);
            //    }
            //});
            System.Console.ReadLine();
        }
    }
}
