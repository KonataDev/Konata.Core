using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Konata.Core.Services;

internal class Sequence
{
    private ConcurrentDictionary<string, int> _sessionSequence { get; }

    /// <summary>
    /// Get Session
    /// </summary>
    public uint Session { get; }

    private int _globalSequence;

    public Sequence()
    {
        Session = 0x54B87ADC;

        _globalSequence = 25900;
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
            return sequence;

        // Record this sequence
        sequence = GetNewSequence();
        if (_sessionSequence.TryAdd(service, sequence))
            return sequence;

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
