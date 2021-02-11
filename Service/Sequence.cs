using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Konata.Core.Service
{
    public class Sequence
    {
        private int _globalSequence;
        private ConcurrentDictionary<string, int> _sessionSequence;
        private uint _sessionCookie;

        /// <summary>
        /// Get sequence with auto increment
        /// </summary>
        public int NewSequence { get => GetNewSequence(); }

        /// <summary>
        /// Get/Set current sequence
        /// </summary>
        public int CurrentSequence { get => _globalSequence; }

        /// <summary>
        /// Get Session
        /// </summary>
        public uint Session { get => _sessionCookie; set => _sessionCookie = value; }

        public Sequence()
        {
            _globalSequence = 25900;

            _sessionCookie = 0x54B87ADC;
            _sessionSequence = new ConcurrentDictionary<string, int>();
        }

        /// <summary>
        /// Get sequence with auto increment
        /// </summary>
        /// <returns></returns>
        public int GetNewSequence()
        {
            Interlocked.CompareExchange(ref _globalSequence, 10000, int.MaxValue);
            return Interlocked.Add(ref _globalSequence, 1);
        }

        /// <summary>
        /// Get sequence by service session
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public int GetSessionSequence(string service)
        {
            // Get service sequence by name
            if (_sessionSequence.TryGetValue(service, out var sequence))
            {
                return sequence;
            }

            sequence = GetNewSequence();

            // Record this sequence
            if (_sessionSequence.TryAdd(service, sequence))
            {
                return sequence;
            }

            throw new Exception("Get service sequence failed.");
        }

        /// <summary>
        /// Destroy sequence by service name
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool DestroyServiceSequence(string service)
            => _sessionSequence.TryRemove(service, out var _) || true;

    }
}