using System;
using Konata.Msf;

namespace Konata.Debug
{
    internal class Program
    {
        public static void Main()
        {
            Bot bot = new Bot(2051118019, "12345678");
            bot.RegisterDelegate(EventProc);
            bot.Run();

            Console.Write("Exit.");
        }

        private static bool EventProc(EventType eventType, params object[] args)
        {
            switch (eventType)
            {
                case EventType.BotStart:
                    return OnBootstrap();
                case EventType.VerifySliderCaptcha:
                    return OnSliderCaptcha((string)args[0], (string)args[1]);
                case EventType.VerifyImageCaptcha:
                    return OnImageCaptcha();
                case EventType.GroupMessage:
                    return OnGroupMessage();
                case EventType.PrivateMessage:
                    return OnPrivateMessage();
            }

            return false;
        }

        private static bool OnBootstrap()
        {
            // Do your initialize operations.
            return false;
        }

        private static bool OnSliderCaptcha(string sigSission, string sigUrl)
        {
            return false;
        }

        private static bool OnImageCaptcha()
        {
            return false;
        }

        private static bool OnPrivateMessage()
        {
            return false;
        }

        private static bool OnGroupMessage()
        {
            return false;
        }

    }
}
