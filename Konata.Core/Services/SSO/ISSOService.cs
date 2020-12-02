﻿using System;
using System.Text;

using Konata.Core.Events;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services
{
    /// <summary>
    /// SSO Service interface
    /// </summary>
    public interface ISSOService
    {
        /// <summary>
        /// In-coming business
        /// </summary>
        /// <param name="ssoMessage"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool HandleInComing(SSOMessage ssoMessage, out KonataEventArgs output);

        /// <summary>
        /// Out-going business
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool HandleOutGoing(KonataEventArgs eventArgs, out byte[] output);
    }
}
