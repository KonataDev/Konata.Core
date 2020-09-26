using System;
using Konata.Msf;

namespace Konata.Debug
{
    internal class Program
    {
        static Bot bot;

        public static void Main()
        {
            bot = new Bot(2051118019, "12345678");
            bot.RegisterDelegate(EventProc);
            bot.Run();

            Console.Write("Exit.");
        }

        private static bool EventProc(EventType type, params object[] args)
        {
            switch (type)
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

        /// <summary>
        /// 框架啓動事件
        /// </summary>
        /// <returns></returns>
        private static bool OnBootstrap()
        {
            bot.Login();
            return false;
        }

        private static bool OnSliderCaptcha(string sigSission, string sigUrl)
        {
            Console.WriteLine($"  SigSession => {sigSission}");
            Console.WriteLine($"  CaptchaUrl => {sigUrl}");
            Console.Write($"Please input the ticket: ");

            var sigTicket = "";

            while (sigTicket == "")
            {
                sigTicket = Console.ReadLine();

                if(sigTicket.Length < 50)
                {
                    sigTicket = "";
                    Console.WriteLine("Wrong ticket. length < 50");
                    Console.Write($"Please input the ticket: ");
                }
            }

            // 提交驗證
            bot.SubmitSliderTicket(sigSission, sigTicket);

            return true;
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
