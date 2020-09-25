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

        private static bool EventProc(Event eventId, params object[] args)
        {
            switch (eventId)
            {
                case Event.OnBotStart:
                    return OnBootstrap();
                case Event.OnVerifySliderCaptcha:
                    return OnSliderCaptcha((string)args[0], (string)args[1]);
                case Event.OnVerifyImageCaptcha:
                    return OnImageCaptcha();
                case Event.OnGroupMessage:
                    return OnGroupMessage();
                case Event.OnPrivateMessage:
                    return OnPrivateMessage();
            }
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

        private static bool OnImageCaptcha(string sigSission, string sigUrl)
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
