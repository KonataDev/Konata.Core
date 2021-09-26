using System;

namespace Konata.Core.Exceptions.Model
{
    public class MessagingException : Exception
    {
        public MessagingException(Exception e)
            : base("Send message failed.", e)
        {
        }

        public MessagingException(string message)
            : base(message)
        {
        }

        public MessagingException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
