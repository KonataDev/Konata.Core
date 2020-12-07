using System;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

using Konata.Runtime.Base;

namespace Konata.Core.Manager
{
    public class SsoInfoManager : Component
    {
        private int _globalSequence;
        private ConcurrentDictionary<string, int> _serviceSequence;

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
        public uint Session { get; set; }

        public SsoInfoManager()
        {
            _globalSequence = 10000;
            _serviceSequence = new ConcurrentDictionary<string, int>();
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
        /// Get sequence by service name
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public int GetServiceSequence(string service)
        {
            // Get service sequence by name
            if (_serviceSequence.TryGetValue(service, out var sequence))
            {
                return sequence;
            }

            sequence = GetNewSequence();

            // Record this sequence
            if (_serviceSequence.TryAdd(service, sequence))
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
            => _serviceSequence.TryRemove(service, out var _) || true;
    }
}
