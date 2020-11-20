using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Konata.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            BlockingCollection<string> b = new BlockingCollection<string>();

            Task.Run(async () =>
            {
                while (!b.IsCompleted&&!source.Token.IsCancellationRequested)
                {
                    string x = b.Take();
                    System.Console.WriteLine(x);
                    await Task.Delay(700);
                }
                System.Console.WriteLine("MQ Finished");
            });

            Task.Run(async () => {
                for (int i = 0; i < 5; i++)
                {
                    b.Add($"the {i} produce");
                    System.Console.WriteLine($"create {i} data");
                    await Task.Delay(800);
                }
                await Task.Delay(2000);
                for (int i = 5; i < 10; i++)
                {
                    b.Add($"the {i} produce");
                    System.Console.WriteLine($"create {i} data");
                    await Task.Delay(500);
                }
                b.CompleteAdding();
                
            });

            System.Console.ReadLine();
        }
    }
}
