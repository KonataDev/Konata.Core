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
                case EventType.WtLoginVerifySliderCaptcha:
                    return OnSliderCaptcha((string)args[0], (string)args[1]);
                case EventType.WtLoginVerifyImageCaptcha:
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
            return true;
        }

        private static bool OnSliderCaptcha(string sigSission, string sigUrl)
        {
            Console.WriteLine($"  SigSession => {sigSission}");
            Console.WriteLine($"  CaptchaUrl => {sigUrl}");

            var sigTicket = "";

            while (sigTicket == "")
            {
                Console.Write($"Please input the ticket: ");
                sigTicket = Console.ReadLine();

                if(sigTicket.Length < 50)
                {
                    sigTicket = "";
                    Console.WriteLine("Wrong ticket. length < 50");
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
